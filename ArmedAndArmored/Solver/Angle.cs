using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmedAndArmored.Solver
{
    //I mean at this point I should really just make math functions that return
    // stuff in degrees so I stop messing up SFML rotations.
    public static class Angle
    {
        public static double radToDeg(double a) { return (a * 180) / Math.PI; }
        public static double degToRad(double a) { return (a * Math.PI) / 180; }
    }
}
