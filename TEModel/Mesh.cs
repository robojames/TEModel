using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
namespace TEModel
{
    class Mesh
    {
        List<float> Modified_X_Distinct;
        
        List<float> Modified_Y_Distinct;
        
        List<Coordinate> CV_Coordinates;
        
        Coordinate[,] Coordinate_Array;

        public Node[,] Node_Array;

        public List<Layer> Layer_List;

        List<Material> MaterialList;

        string material;

 
        public Mesh(List<float> Initial_X, List<float> Initial_Y, int n_Divisions, List<Layer> Layer_List, List<Material> Material_List)
        {
            this.Layer_List = Layer_List;

            
            MaterialList = Material_List;

            Console.WriteLine("Generating Mesh Lines...");
            Generate_Lines(Initial_X, Initial_Y, n_Divisions);

            Console.WriteLine("Calculating Coordinate Pairs...");
            Generate_Coordinate_Pairs();

            Console.WriteLine("Calculating Node Positions, CV Widths/Heights, and Materials...");
            Generate_Nodes();

            Console.WriteLine("Calculating delta_x's and delta_y's...");
            Calculate_dX_dY();

            foreach (Node node in Node_Array)
            {
                node.T = 300.0f;
                node.T_Past = 300.0f;
            }

            Console.WriteLine("Marking Nodes for Spatially Variant Source Terms...");
            Mark_Nodes_For_Source_Terms();

            Console.WriteLine("Calculating and Setting Spatial Source Terms...");
            Set_Source_Terms(4.0f);

            Console.WriteLine("Initializing Influence Coefficients...");
            Initialize_Influence_Coefficients(999999.0f);

            Console.WriteLine("Calculating Interfacial Conductivities...");
            Calculate_Interface_Conductivities();

        }

        private void Set_Source_Terms(float I)
        {
            float Electron_Constant_BiTE = 2.17f * (float)Math.Pow(10, -6);
            float Joule_Constant = 2.17f * (float)Math.Pow(10,-6);
            float Copper_Rho_E = 1.68f * (float)Math.Pow(10, -8);
            float BiTe_Rho_E = (9.2f) * (float)Math.Pow(10, -4);
            float alpha_BiTE = 0.378f * (float)Math.Pow(10, -4);
            

            foreach (Node node in Node_Array)
            {
                if (node.has_Electron_Pumping_Top == true && (node.Material == "BiTe" | node.Material == "Copper"))
                {
                    node.sp = (I / Electron_Constant_BiTE) * alpha_BiTE;
                    //Debug.WriteLine("First:  " + node.sp);
                }

                if (node.has_Joule_Heating == true && node.Node_Material.Material_Name == "Copper")
                {
                    //node.sc = (I / Joule_Constant) * (I / Joule_Constant) * Copper_Rho_E;
                    node.sc = I * I * Copper_Rho_E;
                    //node.sc = I * I * Copper_Rho_E * 0.0024f;
                    //Debug.WriteLine("Second:  " + node.sc);
                }

                if (node.has_Joule_Heating == true && node.Node_Material.Material_Name == "BiTe")
                {
                    //node.sc = (I / Joule_Constant) * (I / Joule_Constant) * BiTe_Rho_E;
                    node.sc = I * I * BiTe_Rho_E;
                    //node.sc = I * I * BiTe_Rho_E * 7.15819f * (float)Math.Pow(10, -4);
                    //Debug.WriteLine("Third:  " + node.sc);
                }

                if (node.has_Electron_Pumping_Bottom == true)
                {
                    node.sp = (I / Electron_Constant_BiTE) * alpha_BiTE;
                    //Debug.WriteLine("Fourth:  " + node.sp);
                }
            }
        }

        private void Mark_Nodes_For_Source_Terms()
        {
            int n_Nodes_Marked = 0;

            List<float> y_Pos_BiTe = new List<float>();

            foreach (Node node in Node_Array)
            {
                if (node.Node_Material.Material_Name == "BiTe")
                {
                    y_Pos_BiTe.Add(node.y_Position);
                }
            }

            float y_Max_BiTe = y_Pos_BiTe.Max();
            float y_Min_BiTe = y_Pos_BiTe.Min();

            foreach (Node node in Node_Array)
            {
                if (node.Node_Material.Material_Name == "BiTe" && node.y_Position == y_Max_BiTe)
                {
                    node.has_Electron_Pumping_Top = true;
                    n_Nodes_Marked++;
                }

                if (node.Node_Material.Material_Name == "BiTe" && node.y_Position == y_Min_BiTe)
                {
                    node.has_Electron_Pumping_Bottom = true;
                    n_Nodes_Marked++;
                }

                if (node.Node_Material.Material_Name == "BiTe" )
                {
                    node.has_Joule_Heating = true;
                    n_Nodes_Marked++;
                }

                if (node.Node_Material.Material_Name == "Copper")
                {
                    node.has_Joule_Heating = true;
                    n_Nodes_Marked++;
                }
            }

            // This is the problem
            List<Node> NodeList_Bottom = new List<Node>();
            List<Node> NodeList_Top = new List<Node>();

            for (int i = 1; i < Node_Array.GetLength(0) - 1; i++)
            {
                for (int j = 1; j < Node_Array.GetLength(1) - 1; j++)
                {
                    if (Node_Array[i,j].has_Electron_Pumping_Bottom == true)
                    {
                        NodeList_Bottom.Add(Node_Array[i, j - 1]);
                    }

                    if (Node_Array[i, j].has_Electron_Pumping_Top == true)
                    {
                        NodeList_Top.Add(Node_Array[i, j + 1]);
                    }
                }
            }

            foreach (Node node in NodeList_Bottom)
            {
                node.has_Electron_Pumping_Bottom = true;
            }

            foreach (Node node in NodeList_Top)
            {
                node.has_Electron_Pumping_Top = true;
            }
            

            Console.WriteLine("Nodes marked for source terms:  " + n_Nodes_Marked);

        }

        private void Initialize_Influence_Coefficients_AP()
        {
            for (int i = 1; i < Node_Array.GetLength(0) - 1; i++)
            {
                for (int j = 1; j < Node_Array.GetLength(1) - 1; j++)
                {
                    Node_Array[i, j].AP = Node_Array[i, j].AE + Node_Array[i, j].AW + Node_Array[i, j].AS + Node_Array[i, j].AN + Node_Array[i, j].AP0 - (Node_Array[i, j].sp * Node_Array[i, j].delta_Y * Node_Array[i, j].delta_X);
                }
            }
        }

        public void Initialize_Influence_Coefficients(float dt)
        {

            for (int i = 1; i < Node_Array.GetLength(0) - 1; i++)
            {
                for (int j = 1; j < Node_Array.GetLength(1) - 1; j++)
                {
                    Node_Array[i, j].AE = ((Node_Array[i - 1, j].Node_Material.k) * (Node_Array[i, j].delta_Y)) / (Node_Array[i, j].d_X_E);
                    Node_Array[i, j].AS = (Node_Array[i, j - 1].Node_Material.k * Node_Array[i, j].delta_X) / (Node_Array[i, j].d_Y_S);
                    Node_Array[i, j].AW = (Node_Array[i + 1, j].Node_Material.k * Node_Array[i, j].delta_Y) / (Node_Array[i, j].d_X_W);
                    Node_Array[i, j].AN = (Node_Array[i, j + 1].Node_Material.k * Node_Array[i, j].delta_X) / (Node_Array[i, j].d_Y_N);

                    Node_Array[i, j].AP0 = (Node_Array[i, j].Node_Material.rho * Node_Array[i, j].Node_Material.cp * Node_Array[i, j].delta_X * Node_Array[i, j].delta_Y) / dt;

                    Node_Array[i, j].AP = Node_Array[i, j].AE + Node_Array[i, j].AW + Node_Array[i, j].AS + Node_Array[i, j].AN + Node_Array[i, j].AP0 - (Node_Array[i, j].sp * Node_Array[i, j].delta_Y * Node_Array[i, j].delta_X);

                    Node_Array[i, j].b = Node_Array[i, j].sc * Node_Array[i, j].delta_X * Node_Array[i, j].delta_Y + Node_Array[i, j].AP0 * Node_Array[i,j].T_Past;
                }
            }

            Calculate_Interface_Conductivities();

            Initialize_Influence_Coefficients_AP();

        }

        private void Calculate_Interface_Conductivities()
        {
            int n_Adjusted_Interfaces = 0;

            for (int i = 1; i < Node_Array.GetLength(0) - 1; i++)
            {
                for (int j = 1; j < Node_Array.GetLength(1) - 1; j++)
                {
                    Material P_Material = Node_Array[i, j].Node_Material;
                    Material N_Material = Node_Array[i, j + 1].Node_Material;
                    Material E_Material = Node_Array[i + 1, j].Node_Material;
                    Material S_Material = Node_Array[i, j - 1].Node_Material;
                    Material W_Material = Node_Array[i - 1, j].Node_Material;
                    
                    //
                    // Adjust Northern Conductivity
                    //
                    if (P_Material.Material_Name != N_Material.Material_Name)
                    {
                        float delt_n = Node_Array[i, j + 1].y_Position - Node_Array[i, j].y_Position;
                        float delt_n_plus = (0.5f * Node_Array[i, j + 1].delta_Y);

                        float f_n = delt_n_plus / delt_n;

                        float k_n = 1.0f / (((1 - f_n) / Node_Array[i, j].Node_Material.k) + (f_n / Node_Array[i, j + 1].Node_Material.k));

                        Node_Array[i, j].AN = (k_n * Node_Array[i, j].delta_X) / Node_Array[i, j].d_Y_N;

                        n_Adjusted_Interfaces++;
                    }

                    //
                    // Adjust Eastern Conductivity
                    //
                    if (P_Material.Material_Name != E_Material.Material_Name)
                    {
                        float delt_e = Node_Array[i + 1, j].x_Position - Node_Array[i, j].x_Position;
                        float delt_e_plus = (0.5f * Node_Array[i + 1, j].delta_X);

                        float f_e = delt_e_plus / delt_e;

                        float k_e = 1.0f / (((1 - f_e) / Node_Array[i, j].Node_Material.k) + (f_e / Node_Array[i + 1, j].Node_Material.k));

                        Node_Array[i, j].AE = (k_e * Node_Array[i, j].delta_Y) / Node_Array[i, j].d_X_E;

                        n_Adjusted_Interfaces++;
                    }

                    //
                    // Adjust Southern Conductivity
                    //
                    if (P_Material.Material_Name != S_Material.Material_Name)
                    {
                        float delt_s = Node_Array[i, j].y_Position - Node_Array[i, j - 1].y_Position;
                        float delt_s_plus = (0.5f * Node_Array[i, j - 1].delta_Y);

                        float f_s = delt_s_plus / delt_s;

                        float k_s = 1.0f / (((1 - f_s) / (Node_Array[i, j].Node_Material.k)) + (f_s / Node_Array[i, j - 1].Node_Material.k));

                        Node_Array[i, j].AS = (k_s * Node_Array[i, j].delta_X) / Node_Array[i, j].d_Y_S;

                        n_Adjusted_Interfaces++;
                    }

                    //
                    // Adjust Western Conductivity
                    //
                    if (P_Material.Material_Name != W_Material.Material_Name)
                    {
                        float delt_w = Node_Array[i - 1, j].x_Position - Node_Array[i, j].x_Position;
                        float delt_w_plus = (0.5f * Node_Array[i - 1, j].delta_X);

                        float f_w = delt_w_plus / delt_w;

                        float k_w = 1.0f / (((1 - f_w) / Node_Array[i, j].Node_Material.k) + (f_w / Node_Array[i + 1, j].Node_Material.k));

                        Node_Array[i, j].AW = (k_w * Node_Array[i, j].delta_Y) / Node_Array[i, j].d_X_W;

                        n_Adjusted_Interfaces++;
                    }
                }
            }

            //Console.WriteLine("Interfaces Adjusted:  " + n_Adjusted_Interfaces);
        }



        private void Calculate_dX_dY()
        {
            for (int i = 0; i < Node_Array.GetLength(0); i++)
            {
                for (int j = 0; j < Node_Array.GetLength(1); j++)
                {
                    if ((i == 0) && (j == 0))
                    {
                        float dXE = Node_Array[i + 1, j].x_Position - Node_Array[i, j].x_Position;
                        float dYN = Node_Array[i, j + 1].y_Position - Node_Array[i, j].y_Position;
                        float dXW = 0.0f;
                        float dYS = 0.0f;

                        Node_Array[i, j].d_X_E = dXE;
                        Node_Array[i, j].d_X_W = dXW;
                        Node_Array[i, j].d_Y_N = dYN;
                        Node_Array[i, j].d_Y_S = dYS;
                    }
                    else if ((i == 0) && (j > 0) && (j != (Node_Array.GetLength(1) - 1)))
                    {
                        float dXE = Node_Array[i + 1, j].x_Position - Node_Array[i, j].x_Position;
                        float dXW = 0.0f;
                        float dYN = Node_Array[i, j + 1].y_Position - Node_Array[i, j].y_Position;
                        float dYS = Node_Array[i, j].y_Position - Node_Array[i, j - 1].y_Position;

                        Node_Array[i, j].d_X_E = dXE;
                        Node_Array[i, j].d_X_W = dXW;
                        Node_Array[i, j].d_Y_N = dYN;
                        Node_Array[i, j].d_Y_S = dYS;
                    }
                    else if ((i > 0) && (j == 0) && (i != (Node_Array.GetLength(0) - 1)))
                    {
                        float dXE = Node_Array[i + 1, j].x_Position - Node_Array[i, j].x_Position;
                        float dXW = Node_Array[i, j].x_Position - Node_Array[i - 1, j].x_Position;
                        float dYN = Node_Array[i, j + 1].y_Position - Node_Array[i, j].y_Position;
                        float dYS = 0.0f;

                        Node_Array[i, j].d_X_E = dXE;
                        Node_Array[i, j].d_X_W = dXW;
                        Node_Array[i, j].d_Y_N = dYN;
                        Node_Array[i, j].d_Y_S = dYS;
                    }
                    else if ((i == (Node_Array.GetLength(0) - 1)) && j == 0)
                    {
                        float dYN = Node_Array[i, j + 1].y_Position - Node_Array[i, j].y_Position;
                        float dXW = Node_Array[i, j].x_Position - Node_Array[i - 1, j].x_Position;
                        float dYS = 0.0f;
                        float dXE = 0.0f;

                        Node_Array[i, j].d_X_E = dXE;
                        Node_Array[i, j].d_X_W = dXW;
                        Node_Array[i, j].d_Y_N = dYN;
                        Node_Array[i, j].d_Y_S = dYS;

                    }
                    else if ((j == (Node_Array.GetLength(1) - 1) && i == 0))
                    {
                        float dXE = Node_Array[i + 1, j].x_Position - Node_Array[i, j].x_Position;
                        float dYS = Node_Array[i, j].y_Position - Node_Array[i, j - 1].y_Position;
                        float dXW = 0.0f;
                        float dYN = 0.0f;

                        Node_Array[i, j].d_X_E = dXE;
                        Node_Array[i, j].d_X_W = dXW;
                        Node_Array[i, j].d_Y_N = dYN;
                        Node_Array[i, j].d_Y_S = dYS;
                    }
                    else if ((j == (Node_Array.GetLength(1) - 1)) && (i == (Node_Array.GetLength(0) - 1)))
                    {
                        float dYS = Node_Array[i, j].y_Position - Node_Array[i, j - 1].y_Position;
                        float dXW = Node_Array[i, j].x_Position - Node_Array[i - 1, j].x_Position;
                        float dXE = 0.0f;
                        float dYN = 0.0f;

                        Node_Array[i, j].d_X_E = dXE;
                        Node_Array[i, j].d_X_W = dXW;
                        Node_Array[i, j].d_Y_N = dYN;
                        Node_Array[i, j].d_Y_S = dYS;
                    }
                    else if (i > 0 && j > 0 && (i != (Node_Array.GetLength(0) - 1)) && (j != (Node_Array.GetLength(1) - 1)))
                    {
                        float dYS = Node_Array[i, j].y_Position - Node_Array[i, j - 1].y_Position;
                        float dXW = Node_Array[i, j].x_Position - Node_Array[i - 1, j].x_Position;
                        float dXE = Node_Array[i + 1, j].x_Position - Node_Array[i, j].x_Position;
                        float dYN = Node_Array[i, j + 1].y_Position - Node_Array[i, j].y_Position;

                        Node_Array[i, j].d_X_E = dXE;
                        Node_Array[i, j].d_X_W = dXW;
                        Node_Array[i, j].d_Y_N = dYN;
                        Node_Array[i, j].d_Y_S = dYS;

                    }
                    else if (i > 0 && (j == (Node_Array.GetLength(1) - 1)) && i != (Node_Array.GetLength(0) - 1))
                    {
                        float dYS = Node_Array[i, j].y_Position - Node_Array[i, j - 1].y_Position;
                        float dXW = Node_Array[i, j].x_Position - Node_Array[i - 1, j].x_Position;
                        float dXE = Node_Array[i + 1, j].x_Position - Node_Array[i, j].x_Position;
                        float dYN = 0.0f;

                        Node_Array[i, j].d_X_E = dXE;
                        Node_Array[i, j].d_X_W = dXW;
                        Node_Array[i, j].d_Y_N = dYN;
                        Node_Array[i, j].d_Y_S = dYS;

                    }
                    else if (j > 0 && (i == (Node_Array.GetLength(0) - 1)) && j != (Node_Array.GetLength(1) - 1))
                    {
                        float dYS = Node_Array[i, j].y_Position - Node_Array[i, j - 1].y_Position;
                        float dXW = Node_Array[i, j].x_Position - Node_Array[i - 1, j].x_Position;
                        float dXE = 0.0f;
                        float dYN = Node_Array[i, j + 1].y_Position - Node_Array[i, j].y_Position;

                        Node_Array[i, j].d_X_E = dXE;
                        Node_Array[i, j].d_X_W = dXW;
                        Node_Array[i, j].d_Y_N = dYN;
                        Node_Array[i, j].d_Y_S = dYS;
                    }


                }

                
            }

            foreach (Node node in Node_Array)
            {
                if (node.d_X_E == 0.0f && node.d_X_W == 0.0f && node.d_Y_N == 0.0f && node.d_Y_S == 0.0f)
                {
                    Console.WriteLine("Error on Node:  " + node.ID);
                }

            }
        }

        private void Generate_Nodes()
        {
            //List<Node> Node_List = new List<Node>();

            Node_Array = new Node[Coordinate_Array.GetLength(0) - 1, Coordinate_Array.GetLength(1) - 1];

            int ID = 0;

            for (int i = 0; i < Coordinate_Array.GetLength(0) - 1; i++)
            {
                for (int j = 0; j < Coordinate_Array.GetLength(1) - 1; j++)
                {
                    Coordinate upper_Left = Coordinate_Array[i, j + 1];
                    Coordinate upper_Right = Coordinate_Array[i + 1, j + 1];
                    Coordinate lower_Left = Coordinate_Array[i, j];
                    Coordinate lower_Right = Coordinate_Array[i + 1, j];

                    float DY = upper_Right.Y - lower_Right.Y;
                    float DX = upper_Right.X - upper_Left.X;
                    float X = lower_Left.X + (0.500f) * (DX);
                    float Y = lower_Left.Y + (0.500f) * (DY);

                    foreach (Layer layer in Layer_List)
                    {
                        if ((upper_Left.X >= layer.Layer_x0) && (lower_Right.X <= layer.Layer_xf) && (upper_Left.Y <= layer.Layer_y0) && (lower_Right.Y >= layer.Layer_yf))
                        {
                            material = layer.Layer_Material;
                        }                        
                    }

                    Node_Array[i, j] = new Node(X, Y, DY, DX, ID);
                    Node_Array[i, j].Material = material;

                    

                    ID++;
                }
            }

            // Node Corrections
            for (int i = 1; i < Node_Array.GetLength(0) - 1; i++)
            {
                for (int j = 1; j < Node_Array.GetLength(1) - 1; j++)
                {
                    if (Node_Array[i, j].Material == "BiTe" && (Node_Array[i + 1, j].Material != "BiTe"))
                    {
                        Node_Array[i + 1, j].Material = "Air";
                    }
                    else if (Node_Array[i, j].Material == "BiTe" && (Node_Array[i - 1, j].Material != "BiTe"))
                    {
                        Node_Array[i - 1, j].Material = "Air";
                    }

                    if (Node_Array[i, j].Material == "Copper" && (Node_Array[i + 1, j].Material != "Copper"))
                    {
                        Node_Array[i + 1, j].Material = "Air";
                    }
                    else if (Node_Array[i, j].Material == "Copper" && (Node_Array[i - 1, j].Material != "Copper"))
                    {
                        Node_Array[i - 1, j].Material = "Air";
                    }

                    if (Node_Array[i, j].x_Position > 0.02613f && Node_Array[i, j].Material == "Air" && Node_Array[i,j].x_Position < 0.00276f)
                    {
                        Node_Array[i, j].Material = "Copper";
                    }
                }
            }

            for (int i = 1; i < Node_Array.GetLength(0) - 1; i++)
            {
                for (int j = 1; j < Node_Array.GetLength(1) - 1; j++)
                {
                    if (Node_Array[i, j].Material == "BiTe")
                    {
                        for (int k = 0; k < Node_Array.GetLength(1) - 1; k++)
                        {
                            if (Node_Array[i, k].Material == "Air")
                            {
                                Node_Array[i, k].Material = "Copper";
                            }
                        }
                    }
                }
            }

            foreach (Node node in Node_Array)
            {
                foreach (Material mat in MaterialList)
                {
                    if (node.Material == mat.Material_Name)
                    {
                        node.Node_Material = mat;
                    }
                }
            }

            Console.WriteLine("Nodes Created:  " + ID);
        }

        public void Generate_Lines(List<float> Initial_X, List<float> Initial_Y, int n_Divisions)
        {
            List<float> Modified_X = new List<float>();
            List<float> Modified_Y = new List<float>();

            for (int i = 0; i < (Initial_X.Count - 1); i++)
            {
                float dx = (Initial_X[i + 1] - Initial_X[i]) / ((float)n_Divisions);

                for (int j = 0; j < (n_Divisions); j++)
                {
                    float x_point = Initial_X[i] + (dx * (float)j);

                    Modified_X.Add(x_point);

                }
            }

            for (int i = 0; i < (Initial_Y.Count - 1); i++)
            {
                float dy = (Initial_Y[i + 1] - Initial_Y[i]) / ((float)n_Divisions);

                for (int j = 0; j < (n_Divisions); j++)
                {
                    float y_point = Initial_Y[i] + (dy * (float)j);

                    Modified_Y.Add(y_point);
                }
            }

            Modified_X_Distinct = Modified_X.Distinct().ToList();
            Modified_Y_Distinct = Modified_Y.Distinct().ToList();

            Console.WriteLine("Initial Node Count(X):  " + Initial_X.Count + "         Modified Node Count:  " + Modified_X.Count);
            Console.WriteLine("Initial Node Count(X):  " + Initial_Y.Count + "         Modified Node Count:  " + Modified_Y.Count);
        }

        public void Generate_Coordinate_Pairs()
        {
            CV_Coordinates = new List<Coordinate>();

            int n_Coordinate_Pairs = 0;

            foreach (float x_pos in Modified_X_Distinct)
            {
                foreach (float y_pos in Modified_Y_Distinct)
                {
                    CV_Coordinates.Add(new Coordinate(x_pos, y_pos, n_Coordinate_Pairs));
                    n_Coordinate_Pairs++;
                }
            }

            Console.WriteLine("Coordinate Pairs Generated:  " + n_Coordinate_Pairs.ToString());

            
            var Sorted_Coordinate_Pairs = CV_Coordinates.OrderBy(Coord => Coord.X).ThenBy(Coord => Coord.Y).ToList().GroupBy(Coord => Coord.X).ToList();
            
            Coordinate[,] Temp_Array_of_Coordinates = ListtoJaggedArray(Sorted_Coordinate_Pairs);

            Coordinate_Array = Temp_Array_of_Coordinates;
        }

        private Coordinate[,] ListtoJaggedArray(IList<IGrouping<float, Coordinate>> CoordinateList)
        {
            // Creates new jagged array to hold NodeList data
            var result = new Coordinate[CoordinateList.Count][];

            for (var i = 0; i < CoordinateList.Count; i++)
            {
                result[i] = CoordinateList[i].ToArray();
                 
            }

            Coordinate[,] Coordinates = new Coordinate[Modified_X_Distinct.Count, Modified_Y_Distinct.Count];

            for (int i = 0; i < result.Count(); i++)
            {
                for (int j = 0; j < result[i].Count(); j++)
                {
                    Coordinates[i, j] = result[i][j];//new Coordinate(result[i][j].X, result[i][j].Y);
                }
            }

            // Returns Coordinate List[][] to user
            return Coordinates;

        }

    }
}
