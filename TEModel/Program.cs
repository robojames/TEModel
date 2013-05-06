using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace TEModel
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(" ************************************************************************ ");
            Console.WriteLine("                     Welcome to the TEModel Program                       ");
            Console.WriteLine(" ************************************************************************ ");

            List<float> X_Lines = new List<float>();
            List<float> Y_Lines = new List<float>();

            int divisions_Per_Line = 20;

            // Generate lines;

            List<float> Main_X_Coords = new List<float>();
            List<float> Main_Y_Coords = new List<float>();

            TEMGeometry myGeom = new TEMGeometry();

            List<float> InitialX = new List<float>();
            List<float> InitialY = new List<float>();

            InitialX = myGeom.x_array.ToList();
            InitialY = myGeom.y_array.ToList();

            Material_Initializer myMaterials = new Material_Initializer();

            Mesh mesh = new Mesh(InitialX, InitialY, divisions_Per_Line, myGeom.Layer_List, myMaterials.Material_List);

            CSVHandler csv = new CSVHandler();


            BoundaryCondition myBoundaries = new BoundaryCondition(mesh.Node_Array);

            csv.WriteMesh(mesh.Node_Array);


            Solver mySolver = new Solver(mesh.Node_Array, 0.0001f, mesh, myBoundaries);

            csv.Write_Temperature_Field(mesh.Node_Array);
            csv.WritedT(mySolver.dT);

            Console.WriteLine("Finished...");

            //Console.ReadLine();

        }
    }
}
