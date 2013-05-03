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
        string material;

 
        public Mesh(List<float> Initial_X, List<float> Initial_Y, int n_Divisions, List<Layer> Layer_List)
        {
            this.Layer_List = Layer_List;

            Console.WriteLine("Generating Mesh Lines...");
            Generate_Lines(Initial_X, Initial_Y, n_Divisions);

            Console.WriteLine("Calculating Coordinate Pairs...");
            Generate_Coordinate_Pairs();

            Console.WriteLine("Calculating Node Positions, CV Widths/Heights, and Materials...");
            Generate_Nodes();

            Console.WriteLine("Calculating delta_x's and delta_y's...");
            Calculate_dX_dY();

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
                        if (upper_Left.X >= layer.Layer_x0 && lower_Right.X <= layer.Layer_xf && upper_Left.Y <= layer.Layer_y0 && lower_Right.Y >= layer.Layer_yf)
                        {
                            material = layer.Layer_Material;
                        }                        
                    }

                    Node_Array[i, j] = new Node(X, Y, DY, DX, ID);
                    Node_Array[i, j].Material = material;

                    ID++;
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
