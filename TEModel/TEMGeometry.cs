using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace TEModel
{
    class TEMGeometry
    {
        /// <summary>
        /// Array of doubles to hold x coordinates for CV placement
        /// </summary>
        public double[] x_array;

        /// <summary>
        /// Array of double to hold y coordinates for CV placement
        /// </summary>
        public double[] y_array;

        /// <summary>
        /// List of layer objects that comprise the entire geometry of the TEM
        /// </summary>
        public List<Layer> Layer_List;

        // Series of constants defining in meters the thicknesses and widths of various
        // repetitive layers for easy loop-generation of the geometry

        /// <summary>
        /// Thickness of the BiTE elements [m] in x-direction
        /// </summary>
        private const double BiTE_Thickness = 0.00146050f;

        /// <summary>
        /// Thickness of the airgap between BiTe Elements [m]
        /// </summary>
        private const double BiTE_AirGap = 0.00093980f;

        /// <summary>
        /// Copper connector widths [m] in the x-direction
        /// </summary>
        private const double CE_Width = 0.00386080f;

        /// <summary>
        /// Height of the air gap
        /// </summary>
        private const double AIR_Height = 0.00170688f;

        /// <summary>
        /// Fixes issue with the ceramic being too short on one side as a consequence of 
        /// an error within the CAD model
        /// </summary>
        private const double Extension = 0.000254f;

        // Convention goes:  X0, Y0, XF, YF for each material layer
        //

        /// <summary>
        /// Coordinates for the bottom of the TEM ceramic
        /// </summary>
        private double[] Coord_Ceramic_Base_Bottom = new double[4] { 0.0f, 0.0007620f, 0.04003040f + Extension, 0.00f };

        /// <summary>
        /// Coordinates for the top of the TEM ceramic
        /// </summary>
        private double[] Coord_Ceramic_Base_Top = new double[4] { 0.0f, 0.00363093f, 0.04003040f + Extension, 0.00286893f };

        /// <summary>
        /// Coordinates for the left air gap (not in between BiTe elements)
        /// </summary>
        private double[] Coord_Left_Air_Gap = new double[4] { 0.0f, 0.00286893f, 0.00140970f, 0.00076200f };

        /// <summary>
        /// Coordinates for the right air gap (not in between BiTe elements)
        /// </summary>
        private double[] Coord_Right_Air_Gap = new double[4] { 0.03887470f, 0.00286893f, 0.04003040f + Extension, 0.00076200f };

        /// <summary>
        /// Coordinates for the first non-repeated copper connector piece on the left side
        /// </summary>
        private double[] Coord_First_Bot_CU = new double[4] { 0.00140970f, 0.00116205f, 0.00287020f, 0.00076200f }; //

        /// <summary>
        /// Coordinates for the first non-repeated copper connector piece on the right side
        /// </summary>
        private double[] Coord_Last_Bot_CU = new double[4] { 0.03741420f, 0.00116205f, 0.03887470f, 0.00076200f }; //

        /// <summary>
        /// Coordinates of the BiTE element (first repeated element)
        /// </summary>
        private double[] Coord_BiTE = new double[4] { 0.00140970f, 0.00247015f, 0.00287020f, 0.00116205f }; //

        /// <summary>
        /// Coordinates of the first repeated (on bottom) copper connective piece
        /// </summary>
        private double[] Coord_Cu_Bottom = new double[4] { 0.00381000f, 0.00116205f, 0.00767080f, 0.00076200f }; //

        /// <summary>
        /// Coordinates of the first repeated (on top) copper connective piece
        /// </summary>
        private double[] Coord_Cu_Top = new double[4] { 0.00140970f, 0.00286893f, 0.00527050f, 0.00247015f }; //

        /// <summary>
        /// Coordinate of the first repeated (upmost) air box
        /// </summary>
        private double[] Coord_Top_AirGaps = new double[4] { 0.00527050f, 0.00286893f, 0.00621030f, 0.00116205f }; // 

        /// <summary>
        /// Coordinate of the first repeated (lowermost) air box
        /// </summary>
        private double[] Coord_Bottom_AirGaps = new double[4] { 0.00287020f, 0.00247015f, 0.003810f, 0.00076200f }; //

        // Constructor for the TEMGeometry.cs class.  Requires passing in of the local error
        // handler but nothing else.  Every other value is generated from other classes as
        // the TEMGeometry is very specific.  This file is one of the few that might require
        // editing as the solution process marches on
        /// <summary>
        /// Constructor for TEM Geometry
        /// </summary>
        public TEMGeometry()
        {

            // Initializes a new List of Layers
            Layer_List = new List<Layer>();

           
            // Function to generate the geometry of the TEM (a list of layers)
            Layer_List = GenerateGeometry();

            // Generates the series of x and y coordinates for CV placement
            Generate_X_Y_Lines();

                       
        }

        /// <summary>
        /// Generates X lines and Y lines for CV placement
        /// </summary>
        private void Generate_X_Y_Lines()
        {
            List<double> x_u = new List<double>();
            List<double> y_u = new List<double>();

            foreach (Layer layer in Layer_List)
            {
                x_u.Add(layer.Layer_x0);
                x_u.Add(layer.Layer_xf);
                y_u.Add(layer.Layer_y0);
                y_u.Add(layer.Layer_yf);

            }

            double[] x = new double[x_u.Count()];
            double[] y = new double[y_u.Count()];

            x = x_u.Distinct().ToArray();
            y = y_u.Distinct().ToArray();

            Array.Sort(x);
            Array.Sort(y);

            this.x_array = x;
            this.y_array = y;

            Debug.WriteLine(this.x_array.Count());
            Debug.WriteLine(this.y_array.Count());

            List<double> xList = x.ToList();
            List<double> yList = y.ToList();


            Int32 index_X = 0;
            double tol_X = 0.0000005f;


            while (index_X < (xList.Count - 1))
            {
                double value = Math.Abs(xList[index_X] - xList[index_X + 1]);

                //Debug.WriteLine("Original Value:  " + xList[index_X] + "        " + value);

                if (Math.Abs((xList[index_X] - xList[index_X + 1])) < tol_X)
                {
                    xList.RemoveAt(index_X);
                    //Debug.WriteLine("Removed!");
                }
                else
                {
                    index_X++;
                }
            }

            foreach (double val in xList)
            {
                //Debug.WriteLine(val);
            }

            Int32 index_Y = 0;
            double tol_Y = 0.00005f;

            while (index_Y < (yList.Count - 1))
            {
                double value = Math.Abs(yList[index_Y] - yList[index_Y + 1]);

                //Debug.WriteLine("Original Value:  " + yList[index_Y] + "        " + value);

                if (Math.Abs((yList[index_Y] - yList[index_Y + 1])) < tol_Y)
                {
                    yList.RemoveAt(index_Y);
                    //Debug.WriteLine("Removed!");
                }
                else
                {
                    index_Y++;
                }
            }

            foreach (double val in yList)
            {
                //Debug.WriteLine(val);
            }

            this.x_array = xList.ToArray();
            this.y_array = yList.ToArray();


            //Debug.WriteLine(this.x_array.Count());
            //Debug.WriteLine(this.y_array.Count());

           
        }



        // GenerateGeometry
        //
        // Main function which generates the geometry of the TEM, and organizes it into a list of layers
        /// <summary>
        /// Generates geometry of the TEM, which is essentially a list of rectangular coordinates
        /// </summary>
        /// <returns>List (array) of Layer objects</returns>
        public List<Layer> GenerateGeometry()
        {
           
            // Generates Layers for both top and bottom ceramic pieces of TEM geometry
            Layer TEM_Bottom = new Layer(Coord_Ceramic_Base_Bottom[0], Coord_Ceramic_Base_Bottom[1], Coord_Ceramic_Base_Bottom[2], Coord_Ceramic_Base_Bottom[3], "Ceramic");
            Layer TEM_Top = new Layer(Coord_Ceramic_Base_Top[0], Coord_Ceramic_Base_Top[1], Coord_Ceramic_Base_Top[2], Coord_Ceramic_Base_Top[3], "Ceramic");

            Layer_List.Add(TEM_Bottom);
            Layer_List.Add(TEM_Top);
            // End of ceramic geometry creation

            // Generates Layers for air gaps to the far left and far right of the computational domain
            Layer Left_Air_Gap = new Layer(Coord_Left_Air_Gap[0], Coord_Left_Air_Gap[1], Coord_Left_Air_Gap[2], Coord_Left_Air_Gap[3], "Air");
            Layer Right_Air_Gap = new Layer(Coord_Right_Air_Gap[0], Coord_Right_Air_Gap[1], Coord_Right_Air_Gap[2], Coord_Right_Air_Gap[3], "Air");

            Layer_List.Add(Left_Air_Gap);
            Layer_List.Add(Right_Air_Gap);
            // End creation of Air Gaps

            // Generates Layer for first and last Cu pieces on the bottom (stubbed off ones)
            Layer First_Bot_Cu = new Layer(Coord_First_Bot_CU[0], Coord_First_Bot_CU[1], Coord_First_Bot_CU[2], Coord_First_Bot_CU[3], "Copper");
            Layer Last_Bot_Cu = new Layer(Coord_Last_Bot_CU[0], Coord_Last_Bot_CU[1], Coord_Last_Bot_CU[2], Coord_Last_Bot_CU[3], "Copper");

            Layer_List.Add(First_Bot_Cu);
            Layer_List.Add(Last_Bot_Cu);
            // End creation of the first and last bottom Cu pieces

            // Generates Layers for each the rest of the bottom Cu pieces
            for (int i = 0; i < 7; i++)
            {
                double x0 = Coord_Cu_Bottom[0] + ((double)i * (BiTE_AirGap + CE_Width));
                double y0 = Coord_Cu_Bottom[1];
                double xf = x0 + CE_Width;
                double yf = Coord_Cu_Bottom[3]; 

                Layer_List.Add(new Layer(x0, y0, xf, yf, "Copper"));

               
            }
            // End creation of bottom Cu pieces

            // Generates Layers for each of the top of the Cu pieces
            for (int i = 0; i < 8; i++)
            {
                double x0 = Coord_Cu_Top[0] + ((double)i * (BiTE_AirGap + CE_Width));
                double y0 = Coord_Cu_Top[1];
                double xf = x0 + CE_Width;
                double yf = Coord_Cu_Top[3];

                Layer_List.Add(new Layer(x0, y0, xf, yf, "Copper"));
            }
            // End creation of the top Cu pieces

            // Generates layers for each of the Bismuth Telluride Pieces
            for (int i = 0; i < 16; i++)
            {
                double x0 = Coord_BiTE[0] + ((double)i * (BiTE_Thickness + BiTE_AirGap));
                double y0 = Coord_BiTE[1];
                double xf = x0 + BiTE_Thickness;
                double yf = Coord_BiTE[3];

                Layer_List.Add(new Layer(x0, y0, xf, yf, "BiTe"));

                Debug.WriteLine("BiTe Coordinates:  xf -> " + xf);
            }
            // End creation of the Bismuth Telluride Pieces

            // Generates layers for air gaps that 'double' near top
            for (int i = 0; i < 8; i++)
            {
                double x0 = Coord_Top_AirGaps[0] + ((double)i * (2.0f * (BiTE_Thickness + BiTE_AirGap)));
                double y0 = Coord_Top_AirGaps[1];
                double xf = x0 + BiTE_AirGap;
                double yf = Coord_Top_AirGaps[3];

                Layer_List.Add(new Layer(x0, y0, xf, yf, "Air"));
            }
            // End creation of layers for top air gaps

            // Generates layers for air gaps that 'double' near bottom
            for (int i = 0; i < 8; i++)
            {
                double x0 = Coord_Bottom_AirGaps[0] + ((double)i * (2.0f * (BiTE_Thickness + BiTE_AirGap)));
                double y0 = Coord_Bottom_AirGaps[1];
                double xf = x0 + BiTE_AirGap;
                double yf = Coord_Bottom_AirGaps[3];

                Layer_List.Add(new Layer(x0, y0, xf, yf, "Air"));
            }
            // End creation of layers for the bottom air gaps

            return Layer_List;
        }

         
    }
}

    