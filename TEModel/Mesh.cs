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
        
 
        public Mesh(List<float> Initial_X, List<float> Initial_Y, int n_Divisions)
        {
            Console.WriteLine("Generating Mesh Lines...");
            Generate_Lines(Initial_X, Initial_Y, n_Divisions);

            Console.WriteLine("Calculating Coordinate Pairs...");
            Generate_Coordinate_Pairs();

            Console.WriteLine("Calculating Node Positions...");
            Generate_Nodes();

        }

        public void Generate_Nodes()
        {
            //List<Node> Node_List = new List<Node>();

            Node[,] NodeArray = new Node[Coordinate_Array.GetLength(0), Coordinate_Array.GetLength(1)];

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
                    float DX = upper_Right.X - upper_Right.X;
                    float X = lower_Left.X + (0.5f) * (DX);
                    float Y = lower_Left.Y + (0.5f) * (DY);

                    NodeArray[i, j] = new Node(X, Y, DY, DX, ID);

                    //Node_List.Add(new Node(X, Y, DY, DX, ID));

                    //Debug.WriteLine(Node_List.Last().x_Position + " , " + Node_List.Last().y_Position);

                    ID++;
                }
            }

            //Console.WriteLine("Nodes Created:  " + Node_List.Count);
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
