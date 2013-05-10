using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TEModel
{
    class CSVHandler
    {

        string R_directory = @"C:\Users\James Armes\Documents\GitHub\TEModel\Coordinatefile.csv";
        string W_directory = @"C:\Users\James Armes\Documents\GitHub\TEModel\Mesh.csv";
        string WT_directory = @"C:\Users\James Armes\Documents\GitHub\TEModel\T_Field.csv";
        string WdT_directory = @"C:\Users\James Armes\Documents\GitHub\TEModel\T_Field_Delta.csv";


        public List<float> x;
        public List<float> y;

        public float[] x_Array;
        public float[] y_Array;

        
        public void ReadFile()
        {

            try
            {
                List<float> x0_u = new List<float>();
                List<float> xf_u = new List<float>();
                List<float> y0_u = new List<float>();
                List<float> yf_u = new List<float>();

                TextReader dataRead = new StreamReader(R_directory);

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

        public void WriteMesh(Node[,] Nodes)
        {
            try
            {
                // 1 Copper
                // 2 BiTe
                // 3 Ceramic
                // 4 Air

                TextWriter dataWrite = new StreamWriter(W_directory);

                List<string> Lines = new List<string>();

                dataWrite.WriteLine("ID" + "," + "XPOS" + "," + "YPOS" + "," + "Material" + "," + "Flag");

                for (int i = 0; i < Nodes.GetLength(0); i++)
                {
                    for (int j = 0; j < Nodes.GetLength(1); j++)
                    {
                        int Mat_ID;

                        switch (Nodes[i, j].Material)
                        {
                            case "Copper":
                                Mat_ID = 1;
                                break;
                            case "BiTe":
                                Mat_ID = 2;
                                break;
                            case "Ceramic":
                                Mat_ID = 3;
                                break;
                            case "Air":
                                Mat_ID = 4;
                                break;
                            default:
                                Mat_ID = 0;
                                break;
                        }

                        int spFlag;

                        if (Nodes[i, j].sp != 0)
                        {
                            spFlag = 1;
                        }
                        else
                        {
                            spFlag = 0;
                        }

                        dataWrite.WriteLine(Nodes[i, j].ID + "," + Nodes[i, j].x_Position + "," + Nodes[i, j].y_Position + "," + Mat_ID + "," + spFlag);
                    }
                }

                dataWrite.Close();
            }
            catch
            {
                Console.WriteLine("Error saving file, or writing was canceled ");
            }
        }

        public void WritedT(List<float> dT)
        {
            try
            {
                
                TextWriter dataWrite = new StreamWriter(WdT_directory);

                List<string> Lines = new List<string>();

                dataWrite.WriteLine("Iteration" + "," + "Gamma");

                for (int i = 0; i < dT.Count; i++)
                {
                    dataWrite.WriteLine(i + "," + dT[i]);
                }

                dataWrite.Close();
            }
            catch
            {
                Console.WriteLine("Error saving file, or writing was canceled ");
            }
        }

        public void Write_Temperature_Field(Node[,] Nodes)
        {
            try
            {
                // 1 Copper
                // 2 BiTe
                // 3 Ceramic
                // 4 Air

                TextWriter dataWrite = new StreamWriter(WT_directory);

                List<string> Lines = new List<string>();

                dataWrite.WriteLine("XPOS" + "," + "YPOS" + "," + "TEMP");

                for (int i = 0; i < Nodes.GetLength(0); i++)
                {
                    for (int j = 0; j < Nodes.GetLength(1); j++)
                    {
                        
                            dataWrite.WriteLine(Nodes[i, j].x_Position + "," + Nodes[i, j].y_Position + "," + Nodes[i, j].T);
                        
                    }
                }

                dataWrite.Close();
            }
            catch
            {
                Console.WriteLine("Error saving file, or writing was canceled ");
            }
        }
    }
}

