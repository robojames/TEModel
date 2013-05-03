using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TEModel
{
    class Layer
    {
        /// <summary>
        /// Maximum x-direction [m]
        /// </summary>
        public const float x_max = 0.040f;

        /// <summary>
        /// Maximum y-direction [m]
        /// </summary>
        public const float y_max = 0.004864f;


        // Holds value of Layer Area
        /// <summary>
        /// Layer area [m^2]
        /// </summary>
        public float Layer_Area;

        /// <summary>
        /// Point on x-axis which indicates the upper left corner of the layer rectangle's distance from origin [m]
        /// </summary>
        public float Layer_x0;

        /// <summary>
        /// Point on x-axis which indicates the lower right corner of the layer rectangle's distance from origin [m]
        /// </summary>
        public float Layer_xf;

        /// <summary>
        /// Point on the y-axis which indicates the upper left corner of the layer rectangle's distance from origin [m]
        /// </summary>
        public float Layer_y0;

        /// <summary>
        /// Point on the y-axis which indicates the lower right corner of the layer rectangle's distance from origin [m]
        /// </summary>
        public float Layer_yf;


        /// <summary>
        /// Struct which holds two coordinate pairs representing the upper right corner (x0,y0)
        /// and the lower right corner (xf,yf) which represents a rectangle.  This rectangle
        /// encloses a certain part of the computational domain.
        /// </summary>
        public struct Rectangle
        {
            public float x_0, y_0, x_f, y_f;

            // Constructor for the Rectangle struct, requires initialization of both
            // coordinate pairs to define the rectangle.
            public Rectangle(float px_0, float py_0, float px_f, float py_f)
            {
                x_0 = px_0;
                y_0 = py_0;
                x_f = px_f;
                y_f = py_f;
            }

            // Area()
            //
            // Calculates the area of the rectangular area
            public float Area()
            {
                // Dx = Final x - Initial x
                float dx = (x_f - x_0);

                // Since y0 is always higher than yf, dy is calculated as y0-yf.
                float dy = (y_0 - y_f);

                // Returns the area of the computational layer [m^2]
                return (dx * dy);
            }
        }

        // Static int (retained in memory) to hold an ID number for each layer.  This is
        // potentially used in debugging, and to ensure that each CV area is consistent 
        // with the physical reality.
        /// <summary>
        /// Layer ID number
        /// </summary>
        private static int LayerID = 0;

        // The Layer_ID is the value of LayerID specific to this individual Layer.  Each
        // time the Layer constructor is called, LayerID is assigned to Layer_ID and then 
        // incremented
        /// <summary>
        /// Layer ID number
        /// </summary>
        private int Layer_ID;

        
        // Rectangle for this layer.  
        /// <summary>
        /// This layer's rectangle
        /// </summary>
        public Rectangle Layer_Rectangle;

        // Holds the string value of the material that is used on this layer.  This will
        // eventually be used in nodal assignment to match layer materials to their respective
        // nodes.
        /// <summary>
        /// Material string for this layer
        /// </summary>
        public string Layer_Material;

        // Layer()
        //
        // Layer constructor which requires the initialization of both coordinate pairs, material name
        // and the ErrorHandler function to be passed in.
        /// <summary>
        /// Layer Constructor
        /// </summary>
        /// <param name="local_ErrorHandler">Main UI ErrorHandler</param>
        /// <param name="x_0">X-coordinate, upper left corner</param>
        /// <param name="y_0">Y-coordinate, upper left corner</param>
        /// <param name="x_f">X-coordinate, lower right corner</param>
        /// <param name="y_f">Y-coordinate, lower right corner</param>
        /// <param name="Mat_Name">Material for this layer</param>
        /// <param name="n_Nodes">Number of nodes for this layer</param>
        public Layer(float x_0, float y_0, float x_f, float y_f, string Mat_Name, int n_Nodes)
        {
            
            Layer_Material = Mat_Name;

            Layer_Rectangle = new Rectangle(x_0, y_0, x_f, y_f);

            Layer_x0 = x_0;
            Layer_xf = x_f;
            Layer_y0 = y_0;
            Layer_yf = y_f;


            Layer_Area = Layer_Rectangle.Area();

            Layer_ID = LayerID++;
        }

        /// <summary>
        /// Get the layer ID
        /// </summary>
        /// <returns>This layer's ID</returns>
        public int getID()
        {
            // Simply returns the Layer_ID to the user.
            return Layer_ID;
        }





    }
}
