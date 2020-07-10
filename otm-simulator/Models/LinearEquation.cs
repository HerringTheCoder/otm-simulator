using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Threading.Tasks;

namespace otm_simulator.Models
{
    public class LinearEquation
    {
        public double? Slope { get; set; }

        public double? Intercept { get; set; }

        public LinearEquation FindByTwoPoints(double x1, double y1, double x2, double y2)
        {
            Slope = (y2 - y1) / (x2 - x1);
            Intercept = y1 - Slope * x1;
            return this;          
        }
    }
}
