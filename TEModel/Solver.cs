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

        Mesh Solver_Mesh_Object;
        BoundaryCondition Boundary_C_Object;
        public Node[,] Nodes;
        float sol_Tol;
        public List<float> dT = new List<float>();

        public Solver(Node[,] Nodes, float sol_Tol, Mesh myMesh, BoundaryCondition myBoundaryConditions)
        {
            this.Boundary_C_Object = myBoundaryConditions;
            this.Nodes = Nodes;
            this.sol_Tol = sol_Tol;
            Solver_Mesh_Object = myMesh;
            Solve();
        }

        public void Solve()
        {

            int n_iter = 0;

                       
            float[] P_X = new float[Nodes.GetLength(0)];
            float[] Q_X = new float[Nodes.GetLength(0)];

            float[] P_Y = new float[Nodes.GetLength(1)];
            float[] Q_Y = new float[Nodes.GetLength(1)];

            float max_Err = 1;

            int x_nodes_max = Nodes.GetLength(0) - 1;
            int y_nodes_max = Nodes.GetLength(1) - 1;

            while (max_Err > sol_Tol)
            {
                // Traveling in positive x-direction, VERIFIED
                // A:  AP
                // B:  AE
                // C:  AW
                // D:  b


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

                    for (int i = x_nodes_max - 1; i >= 0; i--)
                    {
                        Nodes[i, j].T = P_X[i] * Nodes[i + 1, j].T + Q_X[i];
                    }

                }


                // Traveling in positive y-direction -- VERIFIED
                // A: AP
                // B: AN
                // C: AS
                // D: b
                for (int i = 1; i < x_nodes_max; i++)
                {
                    P_Y[0] = Nodes[i, 0].AN / Nodes[i, 0].AP;
                    Q_Y[0] = Nodes[i, 0].b / Nodes[i, 0].AP;
                    for (int j = 1; j <= y_nodes_max; j++)
                    {
                        P_Y[j] = Nodes[i, j].AN / (Nodes[i, j].AP - Nodes[i, j].AS * P_Y[j - 1]);
                        Q_Y[j] = (Nodes[i, j].b + Nodes[i, j].AE * Nodes[i + 1, j].T + Nodes[i, j].AW * Nodes[i - 1, j].T + Nodes[i, j].AS * Q_Y[j - 1]) / (Nodes[i, j].AP - Nodes[i, j].AS * P_Y[j - 1]);
                    }

                    Nodes[i, y_nodes_max].T = Q_Y[y_nodes_max];

                    for (int j = y_nodes_max - 1; j >= 0; j--)
                    {
                        Nodes[i, j].T = P_Y[j] * Nodes[i, j + 1].T + Q_Y[j];
                    }
                }

                //// Traveling in negative x-direction -- VERIFIED

                for (int j = 1; j < y_nodes_max; j++)
                {

                    P_X[0] = Nodes[x_nodes_max, j].AW / Nodes[x_nodes_max, j].AP;
                    Q_X[0] = Nodes[x_nodes_max, j].b / Nodes[x_nodes_max, j].AP;

                    for (int i = 1; i <= x_nodes_max; i++)
                    {

                        P_X[i] = Nodes[x_nodes_max - i + 1, j].AW / (Nodes[x_nodes_max - i + 1, j].AP - Nodes[x_nodes_max - i + 1, j].AE * P_X[i - 1]);
                        Q_X[i] = (Nodes[x_nodes_max - i + 1, j].b + Nodes[x_nodes_max - i + 1, j].AN * Nodes[x_nodes_max - i + 1, j + 1].T + Nodes[x_nodes_max - i + 1, j].AS * Nodes[x_nodes_max - i + 1, j - 1].T + Nodes[x_nodes_max - i + 1, j].AE * Q_X[i - 1]) / (Nodes[x_nodes_max - i + 1, j].AP - Nodes[x_nodes_max - i + 1, j].AE * P_X[i - 1]);
                    }


                    Nodes[0, j].T = Q_X[x_nodes_max];

                    for (int i = x_nodes_max - 1; i >= 1; i--)
                    {
                        Nodes[x_nodes_max - i + 1, j].T = P_X[i] * Nodes[x_nodes_max - i, j].T + Q_X[i];
                    }

                }


                // Travel in negative y-direction
                for (int i = 1; i < x_nodes_max; i++)
                {
                    P_Y[0] = Nodes[i, y_nodes_max].AS / Nodes[i, y_nodes_max].AP;
                    Q_Y[0] = Nodes[i, y_nodes_max].b / Nodes[i, y_nodes_max].AP;

                    for (int j = 1; j <= y_nodes_max; j++)
                    {
                        P_Y[j] = Nodes[i, y_nodes_max + 1 - j].AS / (Nodes[i, y_nodes_max + 1 - j].AP - Nodes[i, y_nodes_max + 1 - j].AN * P_Y[j - 1]);
                        Q_Y[j] = (Nodes[i, y_nodes_max + 1 - j].b + Nodes[i, y_nodes_max + 1 - j].AE * Nodes[i + 1, y_nodes_max + 1 - j].T + Nodes[i, y_nodes_max + 1 - j].AW * Nodes[i - 1, y_nodes_max + 1 - j].T + Nodes[i, y_nodes_max + 1 - j].AN * Q_Y[j - 1]) / (Nodes[i, y_nodes_max + 1 - j].AP - Nodes[i, y_nodes_max + 1 - j].AN * P_Y[j - 1]);
                    }

                    Nodes[i, 1].T = Q_Y[y_nodes_max];

                    for (int j = y_nodes_max - 1; j >= 1; j--)
                    {
                        Nodes[i, y_nodes_max + 1 - j].T = P_Y[j] * Nodes[i, y_nodes_max - j].T + Q_Y[j];
                    }
                }

                phipast_to_phi(Nodes);

                float max_Err_Plot = (float)Calculate_Average_Error(Nodes);

                Console.WriteLine("Max Error:  " + max_Err_Plot + "        Residual:  " + Calculate_Residuals(Nodes) + " Iteration:  " + n_iter);

                //Solver_Mesh_Object.Initialize_Influence_Coefficients(999999.0f);
                //Boundary_C_Object.Apply_Boundary_Conditions_Solver();
                 
                n_iter++;

                if (n_iter > 1500)
                    break;

            } // End While Loop 

        } // End Function

        public void phipast_to_phi(Node[,] Nodes)
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

        public double Calculate_Average_Error(Node[,] Nodes)
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

        public float Calculate_Change_Of_T(float prev)
        {
            float avg_Change;

            List<float> Values = new List<float>();

            for (int i = 1; i < Nodes.GetLength(0) - 1; i++)
            {
                for (int j = 1; j < Nodes.GetLength(1) - 1; j++)
                {
                    float change = Math.Abs(Nodes[i, j].T - prev);
                    Values.Add(change);
                }
            }

            avg_Change = Values.Sum() / Values.Count();

            Values.Clear();

            return avg_Change;
        }

        public double Calculate_Residuals(Node[,] Nodes)
        {
            double avg_Res = 0;

            // Iterate through both x/y planes and calculate the residual at each node
            for (int i = 1; i < Nodes.GetLength(0) - 1; i++)
            {
                for (int j = 1; j < Nodes.GetLength(1) - 1; j++)
                {
                    Nodes[i, j].res = Nodes[i, j].AP * Nodes[i, j].T - (Nodes[i + 1, j].AE * Nodes[i + 1, j].T + Nodes[i - 1, j].AW * Nodes[i - 1, j].T + Nodes[i, j + 1].AN * Nodes[i, j + 1].T + Nodes[i, j - 1].AS * Nodes[i, j - 1].T + Nodes[i, j].b);

                    
                    avg_Res += Nodes[i, j].res;

                }

            }

            //// Average Residuals
            //foreach (Node node in Nodes)
            //{
            //    avg_Res += node.res;
            //}

            avg_Res = avg_Res / (double)(Nodes.GetLength(0) * Nodes.GetLength(1));
            
            return avg_Res;

        }
                
    }
}
