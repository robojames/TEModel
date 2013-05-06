using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TEModel
{
    class Node
    {
        /// <summary>
        /// Node identification number
        /// </summary>
        public int ID;

        public float res;
        public float err;
        /// <summary>
        /// X Position of the Node
        /// </summary>
        public float x_Position;

        /// <summary>
        /// Y Position of the Node
        /// </summary>
        public float y_Position;

        /// <summary>
        /// C.V. width in the y-direction
        /// </summary>
        public float delta_Y;

        /// <summary>
        /// C.V. width in the x-direction
        /// </summary>
        public float delta_X;

        /// <summary>
        /// Distance between this node and the node to the east
        /// </summary>
        public float d_X_E;

        /// <summary>
        /// Distance between this node and the node to the west
        /// </summary>
        public float d_X_W;

        /// <summary>
        /// Distance between this node and the node to the north
        /// </summary>
        public float d_Y_N;

        /// <summary>
        /// Distance between this node and the node to the south
        /// </summary>
        public float d_Y_S;


        public string Material;
        public bool has_Joule_Heating = false;
        public bool has_Electron_Pumping_Top = false;
        public bool has_Electron_Pumping_Bottom = false;
        public float AP;
        public float AN;
        public float AE;
        public float AS;
        public float AW;
        public float AP0;
        public float sp;
        public float b;
        public float sc;

        public float T;
        public float T_Past;

        public Material Node_Material;


        public Node(float X, float Y, float DY, float DX, int ID)
        {
            delta_X = DX;
            delta_Y = DY;
            x_Position = X;
            y_Position = Y;
            this.ID = ID;
            this.sp = 0;
            this.sc = 0;
            this.b = 0;
            this.AP = 0;
            this.AE = 0;
            this.AW = 0;
            this.AN = 0;
            this.AS = 0;
            this.AP0 = 0;
            
        }
    }
}
