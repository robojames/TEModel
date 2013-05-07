using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace TEModel
{
    class BoundaryCondition
    {
        Node[,] Node_Array;

        float h_Top, h_Bottom, h_Left, h_Right, T_Top, T_Bottom, T_Right, T_Left, Tinf_Top, Tinf_Bottom, Tinf_Right, Tinf_Left;
        float q_Top, q_Bottom, q_Left, q_Right;

        
        int is_TBC_Top, is_TBC_Bottom, is_TBC_Right, is_TBC_Left;

        public BoundaryCondition(Node[,] Node_Array)
        {
            this.Node_Array = Node_Array;

            float dt = 999999.0f;

            h_Top = 0.0f;
            h_Bottom = 0.0f;
            h_Left = 0.0f;
            h_Right = 0.0f;

            T_Top = 280.0f;
            T_Bottom = 243.0f;
            T_Right = 280.0f;
            T_Left = 280.0f;

            Tinf_Top = 0.0f;
            Tinf_Bottom = 0.0f;
            Tinf_Right = 0.0f;
            Tinf_Left = 0.0f;

            q_Top = 0.0f;
            q_Bottom = 0.0f;
            q_Left = 0.0f;
            q_Right = 0.0f;

            is_TBC_Top = 0;
            is_TBC_Bottom = 0;
            is_TBC_Right = 0;
            is_TBC_Left = 0;

            Console.WriteLine("Setting Boundary Conditions...");
            Apply_Boundary_Conditions(dt);

            
        }


        public void Apply_Boundary_Conditions(float dt)
        {

            int max_X = Node_Array.GetLength(0) - 1;
            int max_Y = Node_Array.GetLength(1) - 1;
            // 1 if temperature bc is used, 0 otherwise for is_TBC
            // Top and Bottom
            for (int i = 0; i <= max_X; i++)
            {
                // Top First
                Node_Array[i, max_Y].AE = 0.0f;
                Node_Array[i, max_Y].AW = 0.0f;
                Node_Array[i, max_Y].AN = 0.0f;
                Node_Array[i, max_Y].AS = is_TBC_Top * Node_Array[i, max_Y].Node_Material.k * (Node_Array[i, max_Y].delta_X) / (Node_Array[i, max_Y].d_Y_S);

                Node_Array[i, max_Y].AP0 = is_TBC_Top * (Node_Array[i, max_Y].Node_Material.cp * Node_Array[i, max_Y].Node_Material.rho * Node_Array[i, max_Y].delta_X * Node_Array[i, max_Y].delta_Y) / dt;

                Node_Array[i, max_Y].AP = Node_Array[i, max_Y].AP0 + Node_Array[i, max_Y].AE + Node_Array[i, max_Y].AW + Node_Array[i, max_Y].AN + Node_Array[i, max_Y].AS + h_Top * Node_Array[i, max_Y].delta_X - Node_Array[i, max_Y].sp * Node_Array[i, max_Y].delta_X * Node_Array[i, max_Y].delta_Y * 0.5f * (float)is_TBC_Top + (1.0f * (1.0f - (float)is_TBC_Top));

                Node_Array[i, max_Y].b = T_Top + q_Top * Node_Array[i, max_Y].delta_X + h_Top * Node_Array[i, max_Y].delta_X * Tinf_Top + Node_Array[i, max_Y].sc * Node_Array[i, max_Y].delta_X * Node_Array[i, max_Y].delta_Y * 0.5f * is_TBC_Top;
                Node_Array[i, max_Y].T = T_Top;

                // Bottom now
                Node_Array[i, 0].AE = 0.0f;
                Node_Array[i, 0].AW = 0.0f;
                Node_Array[i, 0].AN = is_TBC_Bottom * Node_Array[i, 0].Node_Material.k * (Node_Array[i, 0].delta_X) / (Node_Array[i, 0].d_Y_N);
                Node_Array[i, 0].AS = 0.0f;

                Node_Array[i, 0].AP0 = is_TBC_Bottom * (Node_Array[i, 0].Node_Material.cp * Node_Array[i, 0].Node_Material.rho * Node_Array[i, 0].delta_X * Node_Array[i, 0].delta_Y) / dt;

                Node_Array[i, 0].AP = Node_Array[i, 0].AP0 + Node_Array[i, 0].AE + Node_Array[i, 0].AW + Node_Array[i, 0].AN + Node_Array[i, 0].AS + h_Top * Node_Array[i, 0].delta_X - Node_Array[i, 0].sp * Node_Array[i, 0].delta_X * Node_Array[i, 0].delta_Y * 0.5f * (float)is_TBC_Bottom + (1.0f * (1.0f - (float)is_TBC_Bottom));

                Node_Array[i, 0].b = T_Bottom + q_Bottom * Node_Array[i, 0].delta_X + h_Bottom * Node_Array[i, 0].delta_X * Tinf_Bottom + Node_Array[i, 0].sc * Node_Array[i, 0].delta_X * Node_Array[i, 0].delta_Y * 0.5f * is_TBC_Bottom;

                Node_Array[i, 0].T = T_Bottom;
            }

            for (int j = 0; j <= max_Y; j++)
            {
                // Left Side
                Node_Array[0, j].AE = is_TBC_Left * Node_Array[0, j].Node_Material.k * (Node_Array[0, j].delta_Y) / (Node_Array[0, j].d_X_E);
                Node_Array[0, j].AW = 0.0f;
                Node_Array[0, j].AN = 0.0f;
                Node_Array[0, j].AS = 0.0f;
                Node_Array[0, j].AP0 = is_TBC_Left * (Node_Array[0, j].Node_Material.rho * Node_Array[0, j].Node_Material.cp * Node_Array[0, j].delta_X * Node_Array[0, j].delta_Y) / dt;

                Node_Array[0, j].AP = Node_Array[0, j].AP0 + Node_Array[0, j].AE + Node_Array[0, j].AW + Node_Array[0, j].AN + Node_Array[0, j].AS + h_Left * Node_Array[0, j].delta_Y - Node_Array[0, j].sp * Node_Array[0, j].delta_X * Node_Array[0, j].delta_Y * 0.5f * (float)is_TBC_Left + (1.0f * (1.0f - (float)is_TBC_Left));
                Node_Array[0, j].b = T_Left + q_Left * Node_Array[0, j].delta_Y + h_Left * Node_Array[0, j].delta_Y * Tinf_Left + Node_Array[0, j].sc * Node_Array[0, j].delta_X * Node_Array[0, j].delta_Y * 0.5f * is_TBC_Left;
                Node_Array[0, j].T = T_Left;

                // Right Side
                Node_Array[max_X, j].AE = 0.0f;
                Node_Array[max_X, j].AW = is_TBC_Right * Node_Array[max_X, j].Node_Material.k * Node_Array[max_X, j].delta_Y / Node_Array[max_X, j].d_X_W;
                Node_Array[max_X, j].AN = 0.0f;
                Node_Array[max_X, j].AS = 0.0f;

                Node_Array[max_X, j].AP0 = is_TBC_Right * (Node_Array[max_X, j].Node_Material.rho * Node_Array[max_X, j].Node_Material.cp * Node_Array[max_X, j].delta_X * Node_Array[max_X, j].delta_Y) / dt;


                Node_Array[max_X, j].AP = Node_Array[max_X, j].AP0 + Node_Array[max_X, j].AE + Node_Array[max_X, j].AW + Node_Array[max_X, j].AN + Node_Array[max_X, j].AS + h_Right * Node_Array[max_X, j].delta_Y - Node_Array[max_X, j].sp * Node_Array[max_X, j].delta_X * Node_Array[max_X, j].delta_Y * 0.5f * (float)is_TBC_Right + (1.0f * (1.0f - (float)is_TBC_Right));
                Node_Array[max_X, j].b = T_Right + q_Right * Node_Array[max_X, j].delta_X + h_Right * Node_Array[max_X, j].delta_X * Tinf_Right + Node_Array[max_X, j].sc * Node_Array[max_X, j].delta_X * Node_Array[max_X, j].delta_Y * 0.5f * is_TBC_Right;
                Node_Array[max_X, j].T = T_Right;


            }
        }

        public void Apply_Boundary_Conditions_Solver()
        {

            int max_X = Node_Array.GetLength(0) - 1;
            int max_Y = Node_Array.GetLength(1) - 1;
            // 1 if temperature bc is used, 0 otherwise for is_TBC
            // Top and Bottom
            for (int i = 0; i < max_X; i++)
            {
                // Top First
                Node_Array[i, max_Y].AE = 0.0f;
                Node_Array[i, max_Y].AW = 0.0f;
                Node_Array[i, max_Y].AN = 0.0f;
                Node_Array[i, max_Y].AS = Node_Array[i, max_Y].Node_Material.k * (Node_Array[i, max_Y].delta_X) / (Node_Array[i, max_Y].d_Y_S);
                Node_Array[i, max_Y].AP = Node_Array[i, max_Y].AE + Node_Array[i, max_Y].AW + Node_Array[i, max_Y].AN + Node_Array[i, max_Y].AS + h_Top * Node_Array[i, max_Y].delta_X - Node_Array[i, max_Y].sp * Node_Array[i, max_Y].delta_X * Node_Array[i, max_Y].delta_Y * 0.5f * (float)is_TBC_Top + (1.0f * (1.0f - (float)is_TBC_Top));

                Node_Array[i, max_Y].b = T_Top + q_Top * Node_Array[i, max_Y].delta_X + h_Top * Node_Array[i, max_Y].delta_X * Tinf_Top + Node_Array[i, max_Y].sc * Node_Array[i, max_Y].delta_X * Node_Array[i, max_Y].delta_Y * 0.5f * is_TBC_Top;

                // Bottom now
                Node_Array[i, 0].AE = 0.0f;
                Node_Array[i, 0].AW = 0.0f;
                Node_Array[i, 0].AN = Node_Array[i, 0].Node_Material.k * (Node_Array[i, 0].delta_X) / (Node_Array[i, 0].d_Y_N);
                Node_Array[i, 0].AS = 0.0f;
                Node_Array[i, 0].AP = Node_Array[i, 0].AE + Node_Array[i, 0].AW + Node_Array[i, 0].AN + Node_Array[i, 0].AS + h_Top * Node_Array[i, 0].delta_X - Node_Array[i, 0].sp * Node_Array[i, 0].delta_X * Node_Array[i, 0].delta_Y * 0.5f * (float)is_TBC_Bottom + (1.0f * (1.0f - (float)is_TBC_Bottom));

                Node_Array[i, 0].b = T_Bottom + q_Bottom * Node_Array[i, 0].delta_X + h_Bottom * Node_Array[i, 0].delta_X * Tinf_Bottom + Node_Array[i, 0].sc * Node_Array[i, 0].delta_X * Node_Array[i, 0].delta_Y * 0.5f * is_TBC_Bottom;

                Node_Array[i, 0].T = T_Bottom;
            }

            for (int j = 0; j < max_Y; j++)
            {
                // Left Side
                Node_Array[0, j].AE = Node_Array[0, j].Node_Material.k * (Node_Array[0, j].delta_Y) / (Node_Array[0, j].d_X_E);
                Node_Array[0, j].AW = 0.0f;
                Node_Array[0, j].AN = 0.0f;
                Node_Array[0, j].AS = 0.0f;
                Node_Array[0, j].AP = Node_Array[0, j].AE + Node_Array[0, j].AW + Node_Array[0, j].AN + Node_Array[0, j].AS + h_Left * Node_Array[0, j].delta_X - Node_Array[0, j].sp * Node_Array[0, j].delta_X * Node_Array[0, j].delta_Y * 0.5f * (float)is_TBC_Left + (1.0f * (1.0f - (float)is_TBC_Left));
                Node_Array[0, j].b = T_Left + q_Left * Node_Array[0, j].delta_Y + h_Left * Node_Array[0, j].delta_Y * Tinf_Left + Node_Array[0, j].sc * Node_Array[0, j].delta_X * Node_Array[0, j].delta_Y * 0.5f * is_TBC_Left;

                // Right Side
                Node_Array[max_X, j].AE = 0.0f;
                Node_Array[max_X, j].AW = Node_Array[max_X, j].Node_Material.k * Node_Array[max_X, j].delta_Y / Node_Array[max_X, j].d_X_W;
                Node_Array[max_X, j].AN = 0.0f;
                Node_Array[max_X, j].AS = 0.0f;
                Node_Array[max_X, j].AP = Node_Array[max_X, j].AE + Node_Array[max_X, j].AW + Node_Array[max_X, j].AN + Node_Array[max_X, j].AS + h_Right * Node_Array[max_X, j].delta_X - Node_Array[max_X, j].sp * Node_Array[max_X, j].delta_X * Node_Array[max_X, j].delta_Y * 0.5f * (float)is_TBC_Right + (1.0f * (1.0f - (float)is_TBC_Right));
                Node_Array[max_X, j].b = T_Right + q_Right * Node_Array[max_X, j].delta_X + h_Right * Node_Array[max_X, j].delta_X * Tinf_Right + Node_Array[max_X, j].sc * Node_Array[max_X, j].delta_X * Node_Array[max_X, j].delta_Y * 0.5f * is_TBC_Right;


            }
        }
    }
}
