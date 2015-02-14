using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmedAndArmored
{
    public static class Delta
    {
        public const double DELTA_DIV = 1000f; 

        public static double calc(double num, double delta)
        {
            return (num * (delta / DELTA_DIV));
        }
    }
}
