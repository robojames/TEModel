using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace TEModel
{
    class Node
    {
        /// <summary>
        /// Node identification number
        /// </summary>
        public int ID;

        /// <summary>
        /// Residual [W] of the node
        /// </summary>
        public double res;

        /// <summary>
        /// Error (T - T Past) of the node
        /// </summary>
        public double err;


        /// <summary>
        /// X Position of the Node
        /// </summary>
        public double x_Position;

        /// <summary>
        /// Y Position of the Node
        /// </summary>
        public double y_Position;

        /// <summary>
        /// C.V. width in the y-direction
        /// </summary>
        public double delta_Y;

        /// <summary>
        /// C.V. width in the x-direction
        /// </summary>
        public double delta_X;

        /// <summary>
        /// Distance between this node and the node to the east
        /// </summary>
        public double d_X_E;

        /// <summary>
        /// Distance between this node and the node to the west
        /// </summary>
        public double d_X_W;

        /// <summary>
        /// Distance between this node and the node to the north
        /// </summary>
        public double d_Y_N;

        /// <summary>
        /// Distance between this node and the node to the south
        /// </summary>
        public double d_Y_S;

        /// <summary>
        /// Index in the x direction
        /// </summary>
        public int i;

        /// <summary>
        /// Index in the y direction
        /// </summary>
        public int j;

        /// <summary>
        /// String to identify which material (initially) belongs to the node.  Later
        /// the material object is used to identify this (and to link with all the
        /// thermophysical properties)
        /// </summary>
        public string Material;

        /// <summary>
        /// Flag to indicate whether the node has Joule Heating
        /// </summary>
        public bool has_Joule_Heating = false;

        /// <summary>
        /// Flag to indicate whether the node has electron pumping and
        /// is located on the upper surface of the TEM
        /// </summary>
        public bool has_Electron_Pumping_Top = false;

        /// <summary>
        /// Flag to indicate whether the node has electron pumping
        /// and is located on the lower surface of the TEM
        /// </summary>
        public bool has_Electron_Pumping_Bottom = false;

        /// <summary>
        /// Influence coefficient of the current node
        /// </summary>
        public double AP;

        /// <summary>
        /// Influence coefficient of the northern node
        /// </summary>
        public double AN;

        /// <summary>
        /// Influence coefficient of the eastern node
        /// </summary>
        public double AE;

        /// <summary>
        /// Influence coefficient of the southern node
        /// </summary>
        public double AS;

        /// <summary>
        /// Influence coefficient of the western node
        /// </summary>
        public double AW;

        /// <summary>
        /// 'Influence Coefficient' of the previous time step, so to speak
        /// </summary>
        public double AP0;

        /// <summary>
        /// Temperature dependent source term of the node
        /// </summary>
        public double sp;

        /// <summary>
        /// RHS of the TDMA equation for the node
        /// </summary>
        public double b;

        /// <summary>
        /// Constant source term of the node
        /// </summary>
        public double sc;

        /// <summary>
        /// Temperature of the node
        /// </summary>
        public double T;

        /// <summary>
        /// Temperature of the previous time step of the node
        /// </summary>
        public double T_Past;

        /// <summary>
        /// Material object belonging to the node
        /// </summary>
        public Material Node_Material;

        /// <summary>
        /// Constructor for the Node Object
        /// </summary>
        /// <param name="X">X Position [m]</param>
        /// <param name="Y">Y Position [m]</param>
        /// <param name="DY">CV width in y-direction [m]</param>
        /// <param name="DX">CV width in x-direction [m]</param>
        /// <param name="ID">Node ID</param>
        /// <param name="i">Index in x-direction</param>
        /// <param name="j">Index in y-direction</param>
        public Node(double X, double Y, double DY, double DX, int ID, int i, int j)
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
            this.j = j;
            this.i = i;
            
        }

        public void Validate_Node()
        {
            //if (this.AP < 0 | this.AE < 0 | this.AW < 0 | this.AN < 0 | this.AS < 0)
            //{
            //    Debug.WriteLine("Error with Node.");
            //}

            //if (this.delta_X <= 0 | this.delta_Y <= 0)
            //{
            //    Debug.WriteLine("Error with CV Width or Height");
            //}

            //if (this.d_X_E <= 0 | this.d_X_W <= 0 | this.d_Y_N <= 0 | this.d_Y_S <= 0)
            //{
            //    Debug.WriteLine("Error with DX or DY's at " + this.x_Position + "       " + this.y_Position);
            //}
        }
    }
}
