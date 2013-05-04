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

            List<float> T_Temp = new List<float>();

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
                // Traveling in positive x-direction
                // A:  AP
                // B:  AE
                // C:  AW
                // D:  b

                for (int j = 1; j < y_nodes_max; j++)
                {
                    P_X[0] = Nodes[0, j].AE / Nodes[0, j].AP;
                    Q_X[0] = Nodes[0, j].b / Nodes[0, j].AP;

                    for (int i = 1; i < x_nodes_max + 1; i++)
                    {
                        P_X[i] = Nodes[i, j].AE / (Nodes[i, j].AP - Nodes[i, j].AW * P_X[i - 1]);
                        Q_X[i] = (Nodes[i, j].b + Nodes[i, j].AN * Nodes[i, j + 1].T + Nodes[i, j].AS * Nodes[i, j - 1].T + Nodes[i, j].AW * Q_X[i - 1]) / (Nodes[i, j].AP - Nodes[i, j].AW * P_X[i - 1]);
                    }



                    Nodes[x_nodes_max, j].T = Q_X[x_nodes_max];

                    for (int i = x_nodes_max - 1; i >= 0; --i)
                    {
                        Nodes[i, j].T = P_X[i] * Nodes[i + 1, j].T + Q_X[i];
                    }

                }


                Solver_Mesh_Object.Initialize_Influence_Coefficients(9999.0f);
                Boundary_C_Object.Apply_Boundary_Conditions();


                // Traveling in positive y-direction
                // A: AP
                // B: AN
                // C: AS
                // D: b
                for (int i = 1; i < x_nodes_max; i++)
                {
                    P_Y[0] = Nodes[i, 0].AN / Nodes[i, 0].AP;
                    Q_Y[0] = Nodes[i, 0].b / Nodes[i, 0].AP;

                    for (int j = 1; j < y_nodes_max + 1; j++)
                    {
                        P_Y[j] = Nodes[i, j].AN / (Nodes[i, j].AP - Nodes[i, j].AS * P_Y[j - 1]);
                        Q_Y[j] = (Nodes[i, j].b + Nodes[i, j].AE * Nodes[i + 1, j].T + Nodes[i, j].AW * Nodes[i - 1, j].T + Nodes[i, j].AS * Q_Y[j - 1]) / (Nodes[i, j].AP - Nodes[i, j].AS * P_Y[j - 1]);
                    }

                    Nodes[i, y_nodes_max].T = Q_Y[y_nodes_max];

                    for (int j = y_nodes_max - 1; j >= 0; --j)
                    {
                        Nodes[i, j].T = P_Y[j] * Nodes[i, j + 1].T + Q_Y[j];

                    }
                }


                Solver_Mesh_Object.Initialize_Influence_Coefficients(9999.0f);
                Boundary_C_Object.Apply_Boundary_Conditions();


             
                if ((n_iter % 10) == 0)
                {
                    Debug.WriteLine(Calculate_Average_Residuals().ToString());
                }




                n_iter++;


            } // End While Loop 

        } // End Function

        public float Calculate_Average_Residuals()
        {
            float Residual_Average;

            List<float> Residuals = new List<float>();

            for (int i = 1; i < Nodes.GetLength(0) - 1; i++)
            {
                for (int j = 1; j < Nodes.GetLength(1) - 1; j++)
                {
                    float temp_Residual = Math.Abs(Nodes[i, j].AP * Nodes[i, j].T - Nodes[i + 1, j].AE * Nodes[i + 1, j].T - Nodes[i - 1, j].AW * Nodes[i - 1, j].T - Nodes[i, j + 1].AN * Nodes[i, j + 1].T - Nodes[i, j - 1].AS * Nodes[i, j - 1].T - Nodes[i, j].b);
                    Residuals.Add(temp_Residual);
                }
            }

            Residual_Average = Residuals.Sum() / (float)Residuals.Count();

            Residuals.Clear();

            return Residual_Average;
        }
    }
}
