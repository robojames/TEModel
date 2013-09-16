using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace TEModel
{

    class Solver
    {
        /// <summary>
        /// Mesh object which is linked to the passed in mesh of the solver's constructor
        /// </summary>
        Mesh Solver_Mesh_Object;


        /// <summary>
        /// Node array containing all of the nodes of the mesh
        /// </summary>
        public Node[,] Nodes;

        /// <summary>
        /// Tolerance for the solution (residual limit) to reach convergence
        /// </summary>
        double sol_Tol;

        int current_dt;
        int total_dt;
        
        /// <summary>
        /// Constructor for the Solver
        /// </summary>
        /// <param name="sol_Tol">Convergence Residual</param>
        /// <param name="myMesh">Mesh Object</param>
        /// <param name="myBoundaryConditions">Boundary Condition object</param>
        public Solver(double sol_Tol, Mesh myMesh, int current_time_step, int total_time_steps)
        {
            this.Nodes = myMesh.Node_Array;
            this.sol_Tol = sol_Tol;
            current_dt = current_time_step;
            total_dt = total_time_steps;
            // Assigns passed in mesh object to local mesh object
            Solver_Mesh_Object = myMesh;

            // Calls the Solve() function to use TDMA
            Solve();
        }

        /// <summary>
        /// Solves the temperature field using TDMA
        /// </summary>
        public void Solve()
        {
            // Number of iterations
            int n_iter = 0;

            // P's and Q's utilized for the TDMA algorithm
            double[] P_X = new double[Nodes.GetLength(0)];
            double[] Q_X = new double[Nodes.GetLength(0)];

            double[] P_Y = new double[Nodes.GetLength(1)];
            double[] Q_Y = new double[Nodes.GetLength(1)];

            // Max_Err set to 1, then updated in value by calculating the residuals for each iteration
            double max_Err = 1;

            // Maximum number of nodes in both the x and y direction
            int x_nodes_max = Nodes.GetLength(0) - 1;
            int y_nodes_max = Nodes.GetLength(1) - 1;

            while (max_Err > sol_Tol)
            {
                // Traveling in positive x-direction
                for (int j = 1; j < y_nodes_max; j++)
                {
                    P_X[0] = Nodes[0, j].AE / Nodes[0, j].AP;
                    Q_X[0] = Nodes[0, j].b / Nodes[0, j].AP;

                    for (int i = 1; i <= x_nodes_max; i++)
                    {
                        P_X[i] = Nodes[i, j].AE / (Nodes[i, j].AP - Nodes[i, j].AW * P_X[i - 1]);
                        Q_X[i] = (Nodes[i, j].b + Nodes[i, j].AN * Nodes[i, j + 1].T + Nodes[i, j].AS * Nodes[i, j - 1].T + Nodes[i, j].AW * Q_X[i - 1]) / (Nodes[i, j].AP - Nodes[i, j].AW * P_X[i - 1]);
                    }


                    Nodes[x_nodes_max, j].T = Q_X[x_nodes_max]; 

                    for (int i = x_nodes_max; i-- > 0;)
                    {
                        Nodes[i, j].T = P_X[i] * Nodes[i + 1, j].T + Q_X[i];
                    }

                }

                //Traveling in positive y-direction
                //for (int i = 1; i < x_nodes_max; i++)
                //{
                //    P_Y[0] = Nodes[i, 0].AN / Nodes[i, 0].AP;
                //    Q_Y[0] = Nodes[i, 0].b / Nodes[i, 0].AP;

                //    for (int j = 1; j <= y_nodes_max; j++)
                //    {
                //        P_Y[j] = Nodes[i, j].AN / (Nodes[i, j].AP - Nodes[i, j].AS * P_Y[j - 1]);
                //        Q_Y[j] = (Nodes[i, j].b + Nodes[i, j].AE * Nodes[i + 1, j].T + Nodes[i, j].AW * Nodes[i - 1, j].T + Nodes[i, j].AS * Q_Y[j - 1]) / (Nodes[i, j].AP - Nodes[i, j].AS * P_Y[j - 1]);
                //    }

                //    Nodes[i, y_nodes_max].T = Q_Y[y_nodes_max];

                //    for (int j = y_nodes_max - 1; j >= 0; --j)
                //    {
                //        Nodes[i, j].T = P_Y[j] * Nodes[i, j + 1].T + Q_Y[j];
                //    }
                //}

                double residuals = Calculate_Residuals();

                // Every 100 iterations the residual and iteration number, along with the current time step is printed to the user
                if (n_iter % 100 == 0)
                {
                    Console.WriteLine("Residual:  " + (Math.Abs(residuals)).ToString("N4") + " Iteration:  " + n_iter + "       " + "Time:  "  + current_dt.ToString() + "/" + total_dt.ToString());
                }
                 
                n_iter++;

                // Special case in case the solution is diverging.
                if ((n_iter > 5000) |  Math.Abs(residuals) <= 0.0001f)
                {
                    phipast_to_phi();
                    break;
                }
            } // End While Loop 

        } // End Function

        public void phipast_to_phi()
        {
            // Setting phipast to what was just calculated
            for (int i = 0; i < Nodes.GetLength(0); i++) // X Dimension
            {
                for (int j = 0; j < Nodes.GetLength(1); j++) // Y Dimension
                {
                    //  Iterates over the columns (Y) while holding the row constant,
                    //  column by column.
                    Nodes[i, j].err = Nodes[i, j].T - Nodes[i, j].T_Past;

                    Nodes[i, j].T_Past = Nodes[i, j].T;

                }

            }
        }

        public double Calculate_Average_Error()
        {
            double average_Error = 0;

            // Each object in nodal array needs to be initialized as objects
            for (int i = 0; i < Nodes.GetLength(0); i++) // X Dimension
            {
                for (int j = 0; j < Nodes.GetLength(1); j++) // Y Dimension
                {
                    //  Iterates over the columns (Y) while holding the row constant,
                    //  column by column.
                    average_Error += Math.Abs(Nodes[i, j].err);
                }

            }

            average_Error /= (Nodes.GetLength(0) + Nodes.GetLength(1));

            //Debug.WriteLine(average_Error.ToString());

            return average_Error;
        }


        public double Calculate_Residuals()
        {
            double avg_Res = 0;

            // Iterate through both x/y planes and calculate the residual at each node
            for (int i = 1; i < Nodes.GetLength(0) - 1; i++)
            {
                for (int j = 1; j < Nodes.GetLength(1) - 1; j++)
                {
                    if (i > 5 && j > 5 && (j < Nodes.GetLength(1) - 6) && (i < Nodes.GetLength(0) - 6))
                    {
                        Nodes[i, j].res = Nodes[i, j].AP * Nodes[i, j].T - (Nodes[i + 1, j].AE * Nodes[i + 1, j].T + Nodes[i - 1, j].AW * Nodes[i - 1, j].T + Nodes[i, j + 1].AN * Nodes[i, j + 1].T + Nodes[i, j - 1].AS * Nodes[i, j - 1].T + Nodes[i, j].b);

                        avg_Res += Nodes[i, j].res;
                    }

                }

            }


            avg_Res = avg_Res / (double)(Nodes.GetLength(0) * Nodes.GetLength(1));
            
            return avg_Res;

        }
                
    }
}
