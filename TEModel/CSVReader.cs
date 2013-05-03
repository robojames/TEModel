using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TEModel
{
    class CSVReader
    {
        public List<float> x;
        public List<float> y;

        public float[] x_Array;
        public float[] y_Array;

        public CSVReader()
        {
            ReadFile();
        }

        public void ReadFile()
        {

            try
            {
                List<float> x0_u = new List<float>();
                List<float> xf_u = new List<float>();
                List<float> y0_u = new List<float>();
                List<float> yf_u = new List<float>();

                string directory = @"C:\Users\James\Documents\GitHub\TEModel\Coordinatefile.csv";

                TextReader dataRead = new StreamReader(directory);

                List<string> Lines = new List<string>();

                while (dataRead.ReadLine() != null)
                {
                    Lines.Add(dataRead.ReadLine());
                }



                for (int i = 0; i < Lines.Count; i++)
                {
                    string[] Data = Lines[i].Split(',');

                    x0_u.Add(float.Parse(Data[1]));
                    xf_u.Add(float.Parse(Data[2]));
                    y0_u.Add(float.Parse(Data[3]));
                    yf_u.Add(float.Parse(Data[4]));

                    //Console.WriteLine("Coordinates Read:  " + x0_u[i] + "  " + xf_u[i] + "  " + y0_u[i] + "  " + yf_u[i]);

                }

                List<float> x_u = new List<float>();
                
                List<float> y_u = new List<float>();
                

                x_u = x0_u.Concat(xf_u).ToList();
                y_u = y0_u.Concat(yf_u).ToList();


                x = x_u.Distinct().ToList();
                y = y_u.Distinct().ToList();

                x_Array = x.ToArray();
                y_Array = y.ToArray();

                Array.Sort(x_Array);
                Array.Sort(y_Array);



                //Console.WriteLine("Initial Size:  " + x_u.Count + "     " + y_u.Count);
                //Console.WriteLine("Final Size:    " + x.Count + "     " + y.Count);

                dataRead.Close();
            }
            catch
            {
                Console.WriteLine("Error saving file, or writing was canceled ");
            }
        }
    }
}

