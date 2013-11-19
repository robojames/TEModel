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
        List<double> Modified_X_Distinct;
        
        List<double> Modified_Y_Distinct;
        
        List<Coordinate> CV_Coordinates;
        
        Coordinate[,] Coordinate_Array;

        public Node[,] Node_Array;

        public List<Layer> Layer_List;

        List<Material> MaterialList;

        string material;

  
        public Mesh(List<double> Initial_X, List<double> Initial_Y, int n_Divisions_X, int n_Divisions_Y, List<Layer> Layer_List, List<Material> Material_List, double Current, double dt)
        {
            this.Layer_List = Layer_List;

                       
            MaterialList = Material_List;

            Console.WriteLine("Generating Mesh Lines...");
            Generate_Lines(Initial_X, Initial_Y, n_Divisions_X, n_Divisions_Y);

            Console.WriteLine("Calculating Coordinate Pairs...");
            Generate_Coordinate_Pairs();

            Console.WriteLine("Calculating Node Positions, CV Widths/Heights, and Materials...");
            Generate_Nodes();

            Console.WriteLine("Calculating delta_x's and delta_y's...");
            Calculate_dX_dY();

            Console.WriteLine("Marking Nodes for Spatially Variant Source Terms...");
            Mark_Nodes_For_Source_Terms();

            Console.WriteLine("Calculating and Setting Spatial Source Terms...");
            Set_Source_Terms(Current);

            Console.WriteLine("Initializing Influence Coefficients...");
            Initialize_Influence_Coefficients(dt);

            Console.WriteLine("Calculating Interfacial Conductivities...");
            Calculate_Interface_Conductivities();

            Console.WriteLine("Validating Node Structure...");
            foreach (Node node in Node_Array)
            {
                node.Validate_Node();
            }

        }

        public void Set_Source_Terms(double I)
        {
            int n_Applied = 0;

            double Copper_Rho_E = 1.68f * (double)Math.Pow(10, -8);
            double BiTe_Rho_E = (1.0f) * (double)Math.Pow(10, -5);
            double alpha_BiTE = 2.0 * (double)Math.Pow(10, -4) * 4;//e-4//2.0f * (double)Math.Pow(10, -4);

            

            foreach (Node node in Node_Array)
            {
                
                if (node.has_Electron_Pumping_Top == true && (node.Material == "BiTe" | node.Material == "Copper"))
                {
                    double Ac = 1.9516f * (double)Math.Pow(10, -6);
                    double J = I / Ac;
                    node.sp = (-1.0 * alpha_BiTE * J) / (Ac);
                    n_Applied++;
                }

                if (node.has_Joule_Heating == true && node.Node_Material.Material_Name == "Copper")
                {

                    double Ac = 1.9516f * (double)Math.Pow(10, -6);
                    double J = I / Ac;

                    node.sc = (J * J * Copper_Rho_E);// / node.delta_X;
                    
                    n_Applied++;
                }

                if (node.has_Joule_Heating == true && node.Node_Material.Material_Name == "BiTe")
                {
                    double Ac = 1.9516f * (double)Math.Pow(10, -6);
                    double J = I / Ac;
                    node.sc = (J * J * BiTe_Rho_E);// / node.delta_X;

                    n_Applied++;
                }

                if (node.has_Electron_Pumping_Bottom == true && (node.Material == "BiTe" | node.Material == "Copper"))
                {
                    double Ac = 1.9516f * (double)Math.Pow(10, -6);
                    double J = I / Ac;

                    node.sp = (1.0 * alpha_BiTE * J) / (Ac);

                    n_Applied++;
                }

                if (node.sp != 0)
                {
                    Debug.WriteLine("SP: " + node.sp + " SC: " + node.sc);
                }
            }

            Console.WriteLine("Source Terms Applied to " + n_Applied + " nodes.");
        }

        private void Mark_Nodes_For_Source_Terms()
        {
            int n_Nodes_Marked = 0;

            List<double> y_Pos_BiTe = new List<double>();

            foreach (Node node in Node_Array)
            {
                if (node.Node_Material.Material_Name == "BiTe")
                {
                    y_Pos_BiTe.Add(node.y_Position);
                }
            }

            double y_Max_BiTe = y_Pos_BiTe.Max();
            double y_Min_BiTe = y_Pos_BiTe.Min();

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

            
            List<Node> NodeList_Bottom = new List<Node>();
            List<Node> NodeList_Top = new List<Node>();

            for (int i = 1; i < Node_Array.GetLength(0) - 1; i++)
            {
                for (int j = 1; j < Node_Array.GetLength(1) - 1; j++)
                {
                    if (Node_Array[i,j].has_Electron_Pumping_Bottom == true)
                    {
                        NodeList_Bottom.Add(Node_Array[i, j - 1]);
                        n_Nodes_Marked++;

                    }

                    if (Node_Array[i, j].has_Electron_Pumping_Top == true)
                    {
                        NodeList_Top.Add(Node_Array[i, j + 1]);
                        n_Nodes_Marked++;

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
            for (int i = 0; i < Node_Array.GetLength(0); i++)
            {
                for (int j = 0; j < Node_Array.GetLength(1); j++)
                {
                    Node_Array[i, j].AP = Node_Array[i, j].AE + Node_Array[i, j].AW + Node_Array[i, j].AS + Node_Array[i, j].AN + Node_Array[i, j].AP0 - (Node_Array[i, j].sp * Node_Array[i, j].delta_Y * Node_Array[i, j].delta_X);
                }
            }
        }

        public void Initialize_Influence_Coefficients(double dt)
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
                        double delt_n = Node_Array[i, j + 1].y_Position - Node_Array[i, j].y_Position;
                        double delt_n_plus = (0.5f * Node_Array[i, j + 1].delta_Y);

                        double f_n = delt_n_plus / delt_n;

                        double k_n = 1.0f / (((1 - f_n) / Node_Array[i, j].Node_Material.k) + (f_n / Node_Array[i, j + 1].Node_Material.k));

                        Node_Array[i, j].AN = (k_n * Node_Array[i, j].delta_X) / Node_Array[i, j].d_Y_N;

                        if (Node_Array[i, j].AN < 0)
                        {
                            Debug.WriteLine("Error while adjusting thermal conductivity at interface AN:  (" + i + ", " + j + ")");
                        }

                        n_Adjusted_Interfaces++;
                    }

                    //
                    // Adjust Eastern Conductivity
                    //
                    if (P_Material.Material_Name != E_Material.Material_Name)
                    {
                        double delt_e = Node_Array[i + 1, j].x_Position - Node_Array[i, j].x_Position;
                        double delt_e_plus = (0.5f * Node_Array[i + 1, j].delta_X);

                        double f_e = delt_e_plus / delt_e;

                        double k_e = 1.0f / (((1 - f_e) / Node_Array[i, j].Node_Material.k) + (f_e / Node_Array[i + 1, j].Node_Material.k));

                        Node_Array[i, j].AE = (k_e * Node_Array[i, j].delta_Y) / Node_Array[i, j].d_X_E;

                        if (Node_Array[i, j].AE < 0)
                        {
                            Debug.WriteLine("Error while adjusting thermal conductivity at interface AN:  (" + i + ", " + j + ")");
                        }

                        n_Adjusted_Interfaces++;
                    }

                    //
                    // Adjust Southern Conductivity
                    //
                    if (P_Material.Material_Name != S_Material.Material_Name)
                    {
                        double delt_s = Node_Array[i, j].y_Position - Node_Array[i, j - 1].y_Position;
                        double delt_s_plus = (0.5f * Node_Array[i, j - 1].delta_Y);

                        double f_s = delt_s_plus / delt_s;

                        double k_s = 1.0f / (((1 - f_s) / (Node_Array[i, j].Node_Material.k)) + (f_s / Node_Array[i, j - 1].Node_Material.k));

                        Node_Array[i, j].AS = (k_s * Node_Array[i, j].delta_X) / Node_Array[i, j].d_Y_S;

                        if (Node_Array[i, j].AS < 0)
                        {
                            Debug.WriteLine("Error while adjusting thermal conductivity at interface AN:  (" + i + ", " + j + ")");
                        }

                        n_Adjusted_Interfaces++;
                    }

                    //
                    // Adjust Western Conductivity
                    //
                    if (P_Material.Material_Name != W_Material.Material_Name)
                    {
                        double delt_w = Node_Array[i - 1, j].x_Position - Node_Array[i, j].x_Position;
                        double delt_w_plus = (0.5f * Node_Array[i - 1, j].delta_X);

                        double f_w = delt_w_plus / delt_w;

                        double k_w = 1.0f / (((1 - f_w) / Node_Array[i, j].Node_Material.k) + (f_w / Node_Array[i + 1, j].Node_Material.k));

                        Node_Array[i, j].AW = (k_w * Node_Array[i, j].delta_Y) / Node_Array[i, j].d_X_W;

                        if (Node_Array[i, j].AW < 0)
                        {
                            Debug.WriteLine("Error while adjusting thermal conductivity at interface AN:  (" + i + ", " + j + ")");
                        }

                        n_Adjusted_Interfaces++;
                    }
                }
            }
        }


        private void Calculate_dX_dY()
        {
            for (int i = 0; i < Node_Array.GetLength(0); i++)
            {
                for (int j = 0; j < Node_Array.GetLength(1); j++)
                {
                    if ((i == 0) && (j == 0))
                    {
                        double dXE = Node_Array[i + 1, j].x_Position - Node_Array[i, j].x_Position;
                        double dYN = Node_Array[i, j + 1].y_Position - Node_Array[i, j].y_Position;
                        double dXW = 0.0f;
                        double dYS = 0.0f;

                        if (dXE < 0 | dYN < 0)
                        {
                            Debug.WriteLine(dXE + "     ERROR:  " + i.ToString() + "        " + j.ToString());
                            Debug.WriteLine(dYN + "     ERROR:  " + i.ToString() + "        " + j.ToString());
                        }

                        Node_Array[i, j].d_X_E = dXE;
                        Node_Array[i, j].d_X_W = dXW;
                        Node_Array[i, j].d_Y_N = dYN;
                        Node_Array[i, j].d_Y_S = dYS;
                    }
                    else if ((i == 0) && (j > 0) && (j != (Node_Array.GetLength(1) - 1)))
                    {
                        double dXE = Node_Array[i + 1, j].x_Position - Node_Array[i, j].x_Position;
                        double dXW = 0.0f;
                        double dYN = Node_Array[i, j + 1].y_Position - Node_Array[i, j].y_Position;
                        double dYS = Node_Array[i, j].y_Position - Node_Array[i, j - 1].y_Position;

                        if (dXE < 0 | dYN < 0 | dYS < 0)
                        {
                            Debug.WriteLine(dXE + "     ERROR:  " + i.ToString() + "        " + j.ToString());
                            Debug.WriteLine(dYN + "     ERROR:  " + i.ToString() + "        " + j.ToString());
                            Debug.WriteLine(dYS + "     ERROR:  " + i.ToString() + "        " + j.ToString());
                        }

                        Node_Array[i, j].d_X_E = dXE;
                        Node_Array[i, j].d_X_W = dXW;
                        Node_Array[i, j].d_Y_N = dYN;
                        Node_Array[i, j].d_Y_S = dYS;
                    }
                    else if ((i > 0) && (j == 0) && (i != (Node_Array.GetLength(0) - 1)))
                    {
                        double dXE = Node_Array[i + 1, j].x_Position - Node_Array[i, j].x_Position;
                        double dXW = Node_Array[i, j].x_Position - Node_Array[i - 1, j].x_Position;
                        double dYN = Node_Array[i, j + 1].y_Position - Node_Array[i, j].y_Position;
                        double dYS = 0.0f;

                        if (dXE < 0 | dYN < 0 | dXW < 0)
                        {
                            Debug.WriteLine(dXE + "     ERROR:  " + i.ToString() + "        " + j.ToString());
                            Debug.WriteLine(dYN + "     ERROR:  " + i.ToString() + "        " + j.ToString());
                            Debug.WriteLine(dXW + "     ERROR:  " + i.ToString() + "        " + j.ToString());
                        }

                        Node_Array[i, j].d_X_E = dXE;
                        Node_Array[i, j].d_X_W = dXW;
                        Node_Array[i, j].d_Y_N = dYN;
                        Node_Array[i, j].d_Y_S = dYS;
                    }
                    else if ((i == (Node_Array.GetLength(0) - 1)) && j == 0)
                    {
                        double dYN = Node_Array[i, j + 1].y_Position - Node_Array[i, j].y_Position;
                        double dXW = Node_Array[i, j].x_Position - Node_Array[i - 1, j].x_Position;
                        double dYS = 0.0f;
                        double dXE = 0.0f;

                        if (dXE < 0 | dXW < 0)
                        {
                            Debug.WriteLine(dXE + "     ERROR:  " + i.ToString() + "        " + j.ToString());
                            Debug.WriteLine(dXW + "     ERROR:  " + i.ToString() + "        " + j.ToString());
                        }

                        Node_Array[i, j].d_X_E = dXE;
                        Node_Array[i, j].d_X_W = dXW;
                        Node_Array[i, j].d_Y_N = dYN;
                        Node_Array[i, j].d_Y_S = dYS;

                    }
                    else if ((j == (Node_Array.GetLength(1) - 1) && i == 0))
                    {
                        double dXE = Node_Array[i + 1, j].x_Position - Node_Array[i, j].x_Position;
                        double dYS = Node_Array[i, j].y_Position - Node_Array[i, j - 1].y_Position;
                        double dXW = 0.0f;
                        double dYN = 0.0f;

                        if (dXE < 0 | dYS < 0)
                        {
                            Debug.WriteLine(dXE + "     ERROR:  " + i.ToString() + "        " + j.ToString());
                            Debug.WriteLine(dYS + "     ERROR:  " + i.ToString() + "        " + j.ToString());
                        }

                        Node_Array[i, j].d_X_E = dXE;
                        Node_Array[i, j].d_X_W = dXW;
                        Node_Array[i, j].d_Y_N = dYN;
                        Node_Array[i, j].d_Y_S = dYS;
                    }
                    else if ((j == (Node_Array.GetLength(1) - 1)) && (i == (Node_Array.GetLength(0) - 1)))
                    {
                        double dYS = Node_Array[i, j].y_Position - Node_Array[i, j - 1].y_Position;
                        double dXW = Node_Array[i, j].x_Position - Node_Array[i - 1, j].x_Position;
                        double dXE = 0.0f;
                        double dYN = 0.0f;

                        if (dXE < 0 | dYS < 0)
                        {
                            Debug.WriteLine(dXE + "     ERROR:  " + i.ToString() + "        " + j.ToString());
                            Debug.WriteLine(dYS + "     ERROR:  " + i.ToString() + "        " + j.ToString());
                        }

                        Node_Array[i, j].d_X_E = dXE;
                        Node_Array[i, j].d_X_W = dXW;
                        Node_Array[i, j].d_Y_N = dYN;
                        Node_Array[i, j].d_Y_S = dYS;
                    }
                    else if (i > 0 && j > 0 && (i != (Node_Array.GetLength(0) - 1)) && (j != (Node_Array.GetLength(1) - 1)))
                    {
                        double dYS = Node_Array[i, j].y_Position - Node_Array[i, j - 1].y_Position;
                        double dXW = Node_Array[i, j].x_Position - Node_Array[i - 1, j].x_Position;
                        double dXE = Node_Array[i + 1, j].x_Position - Node_Array[i, j].x_Position;
                        double dYN = Node_Array[i, j + 1].y_Position - Node_Array[i, j].y_Position;

                        if (dYS < 0 | dXW < 0 | dXE < 0 | dYN < 0)
                        {
                            Debug.WriteLine(dXE + "     ERROR:  " + i.ToString() + "        " + j.ToString());
                            Debug.WriteLine(dYS + "     ERROR:  " + i.ToString() + "        " + j.ToString());
                            Debug.WriteLine(dYN + "     ERROR:  " + i.ToString() + "        " + j.ToString());
                            Debug.WriteLine(dXW + "     ERROR:  " + i.ToString() + "        " + j.ToString());
                        }

                        Node_Array[i, j].d_X_E = dXE;
                        Node_Array[i, j].d_X_W = dXW;
                        Node_Array[i, j].d_Y_N = dYN;
                        Node_Array[i, j].d_Y_S = dYS;

                    }
                    else if (i > 0 && (j == (Node_Array.GetLength(1) - 1)) && i != (Node_Array.GetLength(0) - 1))
                    {
                        double dYS = Node_Array[i, j].y_Position - Node_Array[i, j - 1].y_Position;
                        double dXW = Node_Array[i, j].x_Position - Node_Array[i - 1, j].x_Position;
                        double dXE = Node_Array[i + 1, j].x_Position - Node_Array[i, j].x_Position;
                        double dYN = 0.0f;

                        if (dYS < 0 | dXW < 0 | dXE < 0)
                        {
                            Debug.WriteLine(dXE + "     ERROR:  " + i.ToString() + "        " + j.ToString());
                            Debug.WriteLine(dYS + "     ERROR:  " + i.ToString() + "        " + j.ToString());
                            Debug.WriteLine(dXW + "     ERROR:  " + i.ToString() + "        " + j.ToString());
                        }

                        Node_Array[i, j].d_X_E = dXE;
                        Node_Array[i, j].d_X_W = dXW;
                        Node_Array[i, j].d_Y_N = dYN;
                        Node_Array[i, j].d_Y_S = dYS;

                    }
                    else if (j > 0 && (i == (Node_Array.GetLength(0) - 1)) && j != (Node_Array.GetLength(1) - 1))
                    {
                        double dYS = Node_Array[i, j].y_Position - Node_Array[i, j - 1].y_Position;
                        double dXW = Node_Array[i, j].x_Position - Node_Array[i - 1, j].x_Position;
                        double dXE = 0.0f;
                        double dYN = Node_Array[i, j + 1].y_Position - Node_Array[i, j].y_Position;

                        if (dYS < 0 | dXW < 0 | dYN < 0)
                        {
                            Debug.WriteLine(dXW + "     ERROR:  " + i.ToString() + "        " + j.ToString());
                            Debug.WriteLine(dYS + "     ERROR:  " + i.ToString() + "        " + j.ToString());
                            Debug.WriteLine(dYN + "     ERROR:  " + i.ToString() + "        " + j.ToString());
                        }

                        Node_Array[i, j].d_X_E = dXE;
                        Node_Array[i, j].d_X_W = dXW;
                        Node_Array[i, j].d_Y_N = dYN;
                        Node_Array[i, j].d_Y_S = dYS;
                    }
                    else
                    {
                        Debug.WriteLine("Node not set at (i,j):  (" + i + ", " + j + ")");
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

                    double DY = upper_Right.Y - lower_Right.Y;
                    double DX = upper_Right.X - upper_Left.X;
                    double X = lower_Left.X + (0.5) * (DX);
                    double Y = lower_Left.Y + (0.5) * (DY);

                    
                    foreach (Layer layer in Layer_List)
                    {
                        if ((X > layer.Layer_x0) && (X < layer.Layer_xf) && (Y < layer.Layer_y0) && (Y > layer.Layer_yf))
                        {
                            material = layer.Layer_Material;
                        }
                    }

                    Node_Array[i, j] = new Node(X, Y, DY, DX, ID, i, j);
                    Node_Array[i, j].Material = material;

                    ID++;
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

        public void Generate_Lines(List<double> Initial_X, List<double> Initial_Y, int n_Divisions_X, int n_Divisions_Y)
        {
            List<double> Modified_X = new List<double>();
            List<double> Modified_Y = new List<double>();

            // Added to remove artifact position--this may need to be adjusted depending on the mesh density
            Initial_X.RemoveAt(Initial_X.Count - 2);


            for (int i = 0; i < (Initial_X.Count - 1); i++)
            {
                double dx = (Initial_X[i + 1] - Initial_X[i]) / ((double)n_Divisions_X);

                for (int j = 0; j < (n_Divisions_X); j++)
                {
                    double x_point = Initial_X[i] + (dx * (double)j);

                    Modified_X.Add(x_point);

                }
            }

            for (int i = 0; i < (Initial_Y.Count - 1); i++)
            {
                double dy = (Initial_Y[i + 1] - Initial_Y[i]) / ((double)n_Divisions_Y);

                for (int j = 0; j < (n_Divisions_Y); j++)
                {
                    double y_point = Initial_Y[i] + (dy * (double)j);

                    Modified_Y.Add(y_point);
                }
            }

            Modified_X_Distinct = Modified_X.Distinct().ToList();
            Modified_Y_Distinct = Modified_Y.Distinct().ToList();

            
            Console.WriteLine("Initial Line Count(X):  " + Initial_X.Count + "         Modified Line Count:  " + Modified_X.Count);
            Console.WriteLine("Initial Line Count(Y):  " + Initial_Y.Count + "         Modified Line Count:  " + Modified_Y.Count);
        }

        public void Generate_Coordinate_Pairs()
        {
            CV_Coordinates = new List<Coordinate>();

            int n_Coordinate_Pairs = 0;

            foreach (double x_pos in Modified_X_Distinct)
            {
                foreach (double y_pos in Modified_Y_Distinct)
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

        private Coordinate[,] ListtoJaggedArray(IList<IGrouping<double, Coordinate>> CoordinateList)
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

            // Returns Coordinate List[,] to user
            return Coordinates;

        }

    }
}
