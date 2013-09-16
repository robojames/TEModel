using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TEModel
{
    class Material_Initializer
    {
        // A series of const doubles which represent several material properties of materials utilized in the numeric model:
        //
        // rho_XX = Density of Material XX [kg/m^3]
        // k_XX = Thermal Conductivity of Material XX [W/mK]
        // cp_XX = Heat Capacity of Material XX [J/kgK]
        // alpha_XX = Seebeck Coefficient of Material XX [V/K]

        /// <summary>
        /// Density of Copper [kg/m^3] at 20 C
        /// </summary>
        const double rho_Copper = 8960.0f;

        /// <summary>
        /// Thermal Conductivity of Copper [W / m K] at 20 C
        /// </summary>
        const double k_Copper = 386.0f;

        /// <summary>
        /// Heat Capacity of Copper [J/kg K] at 20 C
        /// </summary>
        const double cp_Copper = 390.0f;

        /// <summary>
        /// Density of Ceramic Alumina used in TEM [kg/m^3] at 20 C
        /// </summary>
        const double rho_Ceramic = 3750.0f;

        /// <summary>
        /// Thermal Conductivity of Ceramic Alumina used in TEM [W / m K] at 20 C
        /// </summary>
        const double k_Ceramic = 35.0f;
        
        /// <summary>
        /// Heat Capacity of Ceramic Alumina used in TEM [J/kg K] at 20 C
        /// </summary>
        const double cp_Ceramic = 870.0f;

        /// <summary>
        /// Density of Bismuth Telluride [kg/m^3] at 20 C
        /// </summary>
        const double rho_BiTe = 7700.0f;

        /// <summary>
        /// Thermal Conductivity of Bismuth Telluride [W / m K] at 20 C
        /// </summary>
        const double k_BiTe = 1.48f;
        
        /// <summary>
        /// Heat Capacity of Bismuth Telluride [J/kg K] at 20 C
        /// </summary>
        const double cp_BiTe = 122.0f;
        
        /// <summary>
        /// Density of glass [kg/m^3] at 20 C
        /// </summary>
        const double rho_Glass = 2225.0f;
        
        /// <summary>
        /// Thermal Conductivity of Glass [W / m K] at 20 C
        /// </summary>
        const double k_Glass = 14.0f;

        /// <summary>
        /// Heat Capacity of glass [J/kg K] at 20 C
        /// </summary>
        const double cp_Glass = 835.0f;

        // FIX THESE VALUES HAHHHHHH
        
        /// <summary>
        /// Density of Air [kg/m^3] at 20 C
        /// </summary>
        const double rho_Air = 1.204f;

        /// <summary>
        /// Thermal Conductivity of Air [W / m K] at 20 C
        /// </summary>
        const double k_Air = 0.02570f;

        /// <summary>
        /// Heat Capacity of air [J/kg K] at 20 C
        /// </summary>
        const double cp_Air = 1005.0f;

        /// <summary>
        /// Seebeck coefficient of bismuth telluride [V / K] at 20 C
        /// </summary>
        const double alpha_BiTE = 0.00025f; 



        /// <summary>
        /// Material list for passing into other sub-classes
        /// </summary>
        public List<Material> Material_List;

        public Material_Initializer()
        {
            Create_Materials();

            Console.WriteLine("Adding Materials...");

        }

        // Create_Materials()
        /// <summary>
        /// Initializes materials utilized in the numeric simulation.  Currently set up to allow for the determination 
        /// of constant thermal conductivities though a simple modification to the material class will allow for eventual 
        /// interpolation of the thermal conductivity 
        /// </summary>
        private void Create_Materials()
        {
            Material_List = new List<Material>();

            // Number of materials to be created.  Could possibly be pulled from the main UI if necessary.
            const int n_Materials = 5;

            // String array to hold the material names for visualization and error reporting
            string[] MatList_Name = new string[n_Materials] { "Copper", "Glass", "BiTe", "Ceramic" , "Air"};

            // double array to hold value of thermal conductivites
            double[] k = new double[n_Materials] { k_Copper, k_Glass, k_BiTe, k_Ceramic, k_Air};

            double[] rho = new double[n_Materials] { rho_Copper, rho_Glass, rho_BiTe, rho_Ceramic, rho_Air };

            double[] cp = new double[n_Materials] { cp_Copper, cp_Glass, cp_BiTe, cp_Ceramic, cp_Air };

            // Iterates from 0 to n_Materials - 1 to create n_Materials number of Material objects.  
            for (int i = 0; i < n_Materials; i++ )
            {
                // Creates new Material object and adds it to the Material_List for later recollection or modification
                if (MatList_Name[i] == "BiTe")
                {
                    Material_List.Add(new Material(MatList_Name[i], k[i], cp[i], alpha_BiTE, rho[i]));
                }
                else
                {
                    Material_List.Add(new Material(MatList_Name[i], k[i], cp[i], 0.0f, rho[i]));
                }

            }

        }
    }
}