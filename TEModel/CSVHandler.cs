using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
namespace TEModel
{
    class CSVHandler
    {
        // Strings to hold file locations (where to save the .CSV files)
        // However, check each function as a lot of these names are appended to include
        // information about the file--such as current (A) and time step information
        string W_directory = @"C:\Users\James\Documents\GitHub\TEModel\Mesh.csv";
        string WT_directory = @"C:\Users\James\Documents\GitHub\TEModel\T_Field_";
        string WdT_directory = @"C:\Users\James\Documents\GitHub\TEModel\T_Field_Delta.csv";
        string Write_Mid_T = @"C:\Users\James\Documents\GitHub\TEModel\T_Mid_T_";
        string Read_Current_Profile_directory = @"C:\Users\James\Documents\GitHub\TEModelD\1Kpersec.csv";
        string Write_Temperature_versus_Time_Directory = @"C:\Users\James\Documents\GitHub\TEModelD\Tvst.csv";

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
                                Console.WriteLine("Error with Mesh!  Mat_ID is 0");
                                break;
                        }

                        int spFlag;

                        if (Nodes[i, j].sp != 0 | Nodes[i, j].sc != 0)
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

        public void WritedT(List<double> dT)
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

        public void Write_Temperature_Field(Node[,] Nodes, string AppendedFileName)
        {
            try
            {
                // 1 Copper
                // 2 BiTe
                // 3 Ceramic
                // 4 Air

                TextWriter dataWrite = new StreamWriter(WT_directory + AppendedFileName + ".csv");

                List<string> Lines = new List<string>();

                dataWrite.WriteLine("XPOS" + "," + "YPOS" + "," + "TEMP");

                for (int i = 0; i < Nodes.GetLength(0); i++)
                {
                    for (int j = 0; j < Nodes.GetLength(1); j++)
                    {
                        //if (i > 1 && j > 1 && i < (Nodes.GetLength(0) - 1) && j < (Nodes.GetLength(1) - 1))
                        //{
                        dataWrite.WriteLine(Nodes[i, j].x_Position + "," + Nodes[i, j].y_Position + "," + Nodes[i, j].T);
                        //}
                    }
                }

                dataWrite.Close();
            }
            catch
            {
                Console.WriteLine("Error saving file, or writing was canceled ");
            }
        }

        public void Write_Mid_Field(Node[,] Nodes, string AppendedFileName)
        {
            try
            {

                TextWriter dataWrite = new StreamWriter(Write_Mid_T + AppendedFileName + ".csv");

                List<string> Lines = new List<string>();

                dataWrite.WriteLine("XPOS" + "," + "YPOS" + "," + "TEMP");

                int idx = 0;

                foreach (Node node in Nodes)
                {
                    if (node.x_Position >= 0.01660 && node.x_Position <= 0.01670)
                    {
                        idx = node.i;
                        Debug.WriteLine("Node found:  " + idx + " at x position:  " + node.x_Position);
                    }
                }

                if (idx == 0)
                {
                    Console.WriteLine("ERROR Finding midline temperature.... re-run simulation by adjusting idx tolerance");
                }

                for (int i = 0; i < Nodes.GetLength(0); i++)
                {
                    for (int j = 0; j < Nodes.GetLength(1); j++)
                    {
                        // This pulls the middle indexed node out--however, this catches the air pocket
                        // as opposed to a BiTe element of interest.  
                        //int idx = (int)Math.Round(Nodes.GetLength(0) / 2.0);

                        if (i == idx)
                        {
                            dataWrite.WriteLine(Nodes[i, j].x_Position + "," + Nodes[i, j].y_Position + "," + Nodes[i, j].T);
                        }

                    }
                }

                dataWrite.Close();
            }
            catch
            {
                Console.WriteLine("Error saving file, or writing was canceled ");
            }
        }

        public List<double> Read_Current_Profile()
        {
            List<double> Current_Profile = new List<double>();

            TextReader dataRead = new StreamReader(Read_Current_Profile_directory);

            string templines;
            string[] temparray = new string[5];

            while ((templines = dataRead.ReadLine()) != null)
            {
                temparray = templines.Split(',');

                Current_Profile.Add(double.Parse(temparray[4]));
            }

            dataRead.Close();

            return Current_Profile;
        }

        public void Write_Temperature_Versus_Time(List<double> Temperature)
        {
            TextWriter dataWrite = new StreamWriter(Write_Temperature_versus_Time_Directory);

            List<string> Lines = new List<string>();

            dataWrite.WriteLine("Time (s)" + "," + "Temperature (C)");

            for (int i = 0; i < Temperature.Count; i++)
            {
                dataWrite.WriteLine(((double)i * 0.05) + "," + Temperature[i]);
            }

            dataWrite.Close();
        }
    }
}

