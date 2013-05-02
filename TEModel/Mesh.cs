using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TEModel
{
    class Mesh
    {
        List<float> Modified_X;
        List<float> Modified_Y;
        List<Coordinate> CV_Coordinates;
 
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

        }

        public void Generate_Coordinate_Pairs()
        {
            CV_Coordinates = new List<Coordinate>();

            int n_Coordinate_Pairs = 0;

            foreach (float x_pos in Modified_X)
            {
                foreach (float y_pos in Modified_Y)
                {
                    CV_Coordinates.Add(new Coordinate(x_pos, y_pos));
                    n_Coordinate_Pairs++;
                }
            }

            Console.WriteLine("Coordinate Pairs Generated:  " + n_Coordinate_Pairs.ToString());
        }

        public void Generate_Lines(List<float> Initial_X, List<float> Initial_Y, int n_Divisions)
        {
            Modified_X = new List<float>();
            Modified_Y = new List<float>();

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

            Console.WriteLine("Initial Node Count(X):  " + Initial_X.Count + "         Modified Node Count:  " + Modified_X.Count);
            Console.WriteLine("Initial Node Count(X):  " + Initial_Y.Count + "         Modified Node Count:  " + Modified_Y.Count);
        }
    }
}
