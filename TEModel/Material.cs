using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TEModel
{
    class Material
    {

        public double k;
        public double cp;
        public double alpha;
        public double rho;
        public string Material_Name;

        public Material(string Material_Name, double k, double cp, double alpha, double rho)
        {
            this.Material_Name = Material_Name;

            this.k = k;
            this.cp = cp;
            this.alpha = alpha;
            this.rho = rho;

        }
    }
}
