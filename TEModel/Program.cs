using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace TEModel
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(" ************************************************************************ ");
            Console.WriteLine("                     Welcome to the TEModel Program                       ");
            Console.WriteLine(" ************************************************************************ ");

            double max_Node_Position_Y = 0;

            CSVHandler Get_Current_Profile = new CSVHandler();

            List<double> Current_Profile = new List<double>();

            Current_Profile = Get_Current_Profile.Read_Current_Profile();

                        

             // Can be used to iterate over a current of 1-5 A to the TEM module
             // to construct steady-state temperature plots
            //for (int i = 0; i <= Current_Profile.Count - 1; i++)
            //{

                // Time in seconds
                double time = 55;

                // Timestep in seconds, set to 9999999 for Steady State Solution
                double timestep = 0.05;
                //double timestep = 99999999;

                // Required iterations, set to 1 for Steady State Solution
                int req_iter = (int)(time / timestep);
                //int req_iter = 1;

                // Current Applied
                //double current = 4;
                //double current = (double)Current_Profile[i];

                // Number of divisions specified in both the X and Y directions
                // in order to form a more dense mesh (30-x, 30-y is mesh independent)
                int divisions_X = 20;
                int divisions_Y = 20;

                // Creates object to write .CSV files for data output
                CSVHandler csv = new CSVHandler();

                // Creates the geometry object and calls the objects
                // constructor which generates a list of Layer objects
                // and a series of X and Y coordinates on which to draw
                // the control volume lines.
                TEMGeometry myGeom_Element = new TEMGeometry();

                // Creating two lists (arrays) of doubles to hold the initial x and y positions
                // of interest from the geometry file
                List<double> InitialX = new List<double>();
                List<double> InitialY = new List<double>();

                // Sets InitialX and InitialY to the array of x and y positions which are
                // output from the geometry class for the TEM (or any other imported class)
                InitialX = myGeom_Element.x_array.ToList();
                InitialY = myGeom_Element.y_array.ToList();

                // Initializes all the material names and thermophysical properties
                Material_Initializer myMaterials = new Material_Initializer();

                // Creates mesh based on the number of divisions and initial coordinates
                // in both the x and y direction.  Also requires a list of layers, materials
                // and an input current for initialization of the source terms
               // Mesh mesh = new Mesh(InitialX, InitialY, divisions_X, divisions_Y, myGeom_Element.Layer_List, myMaterials.Material_List, Current_Profile[0], timestep);

                // Writes the mesh information (to be plotted in MatLAB) to a .CSV file
              //  csv.WriteMesh(mesh.Node_Array);

                Mesh mesh = new Mesh(InitialX, InitialY, divisions_X, divisions_Y, myGeom_Element.Layer_List, myMaterials.Material_List, Current_Profile[0], timestep);

            // Finds highest Y value and assigns it to Max_Node_Position_Y
                foreach (Node node in mesh.Node_Array)
                {
                    if (node.y_Position >= max_Node_Position_Y)
                    {
                        max_Node_Position_Y = node.y_Position;
                    }
                }

                Console.WriteLine("Max Node Position (Y) : " + max_Node_Position_Y.ToString());

                List<double> Temperaturevst = new List<double>();


                foreach (Node node in mesh.Node_Array)
                {
                    node.T = 273.0;
                    node.T_Past = 273.0;
                }
                
            // t < req_iter
                for (int t = 0; t < Current_Profile.Count; t++)
                {
                    Console.WriteLine("Applied Current for time step " + t.ToString() + " is " + Current_Profile[t].ToString());

                    double current = ((Current_Profile[t]));


                    mesh.Set_Source_Terms(Current_Profile[t]);
                    mesh.Initialize_Influence_Coefficients(timestep);
                    

                    // Creates object which applies the boundary conditions to the node array,
                    // boundary conditions are specified WITHIN the BoundaryCondition.cs file
                    BoundaryCondition myBoundaries = new BoundaryCondition(mesh.Node_Array, timestep);

                    // Solves the temperature field for the given time-step and current
                    Solver mySolver = new Solver(0.0001f, mesh, t, Current_Profile.Count);

                    // Writes the temperature field, and midline temperature through a BiTE element.
                    //csv.Write_Temperature_Field(mesh.Node_Array, ((t * timestep)).ToString("N2") + "_" + current);
                    //csv.Write_Mid_Field(mesh.Node_Array, ((t * timestep)).ToString("N2") + "_" + current);

                    double average_T = 0;
                    int Ceramic_Nodes = 0;

                    foreach (Node node in mesh.Node_Array)
                    {
                        node.T_Past = node.T;
                        node.res = 200;

                        if (node.y_Position > 0.0025 && node.y_Position <= 0.003 && node.Material == "Ceramic")
                        {
                            average_T = average_T + node.T;
                            Ceramic_Nodes++;
                        }
                    }

                    average_T = (average_T / (double)Ceramic_Nodes) - 273.15;

                    Console.WriteLine("Average T: " + average_T + " from an averaged value for " + Ceramic_Nodes + " nodes.");

                    Temperaturevst.Add(average_T);

                } 

                csv.Write_Temperature_Versus_Time(Temperaturevst);

            //}
            Console.WriteLine("Finished...");

            Console.ReadLine();

        }
    }
}
