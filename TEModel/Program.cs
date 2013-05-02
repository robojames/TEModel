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

            int divisions_Per_Line = 5;

            // Generate lines;

            List<float> Main_X_Coords = new List<float>();
            List<float> Main_Y_Coords = new List<float>();

            CSVReader CsvRead = new CSVReader();

            List<float> InitialX = new List<float>();
            List<float> InitialY = new List<float>();

            InitialX = CsvRead.x_Array.ToList();
            InitialY = CsvRead.y_Array.ToList();

            Mesh mesh = new Mesh(InitialX, InitialY, divisions_Per_Line);

            Console.ReadLine();

        }
    }
}
