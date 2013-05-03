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

            int divisions_Per_Line = 14;

            // Generate lines;

            List<float> Main_X_Coords = new List<float>();
            List<float> Main_Y_Coords = new List<float>();

            TEMGeometry myGeom = new TEMGeometry();

            List<float> InitialX = new List<float>();
            List<float> InitialY = new List<float>();

            InitialX = myGeom.x_array.ToList();
            InitialY = myGeom.y_array.ToList();

            

            Mesh mesh = new Mesh(InitialX, InitialY, divisions_Per_Line, myGeom.Layer_List);

            CSVHandler csv = new CSVHandler();

            csv.WriteMesh(mesh.Node_Array);

            Console.ReadLine();

        }
    }
}
