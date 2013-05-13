using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TEModel
{
    class ElementGeometry
    {
        public List<Layer> LayerList;
        public List<float> xList;
        public List<float> yList;

        public ElementGeometry()
        {
            LayerList = new List<Layer>();
            
            // Generate Layers of Individual TE Element
            LayerList = GenerateGeometry();

            Generate_X_Y_Lines();

        }

        private List<Layer> GenerateGeometry()
        {
            List<Layer> Geometry_Layer_List = new List<Layer>();

            // Need to eliminate Nodes numbering requirement from Layer Class
            float x0 = 0.0f;
            float y0 = 0.00132080f;

            float xf = 0.001397f;
            float yf = 0.0f;

            string TE_Mat = "BiTe";

            // Node numbering requirement not needed for uniform mesh generation
            Layer Element = new Layer(x0, y0, xf, yf, TE_Mat, 0);

            Geometry_Layer_List.Add(Element);

            return Geometry_Layer_List;
        }

        private void Generate_X_Y_Lines()
        {
            List<float> x = new List<float>();
            List<float> y = new List<float>();

            x.Add(LayerList[0].Layer_x0);
            x.Add(LayerList[0].Layer_xf);

            y.Add(LayerList[0].Layer_y0);
            y.Add(LayerList[0].Layer_yf);

            float[] X_sort = x.ToArray();
            float[] Y_sort = y.ToArray();

            Array.Sort(X_sort);
            Array.Sort(Y_sort);

            xList = X_sort.ToList();
            yList = Y_sort.ToList();

            
        }
    }
}
