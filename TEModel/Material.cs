using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TEModel
{
    class Material
    {

        public float k;
        public float cp;
        public float alpha;
        public float rho;
        public string Material_Name;

        public Material(string Material_Name, float k, float cp, float alpha, float rho)
        {
            this.Material_Name = Material_Name;

            this.k = k;
            this.cp = cp;
            this.alpha = alpha;
            this.rho = rho;

        }
    }
}
