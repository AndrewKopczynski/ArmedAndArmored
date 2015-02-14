using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmedAndArmored
{
    public static class RotationSolver
    {
        //Solves the difference between two rotations
        // Note: This uses the rotation class's logic to wrap around angles
        public static Rotation diff(Rotation a, Rotation b)
        {
            return new Rotation(a.Value - b.Value);
        }

        //Compares the two angles, and returns the smaller angle between them
        // Used for clockwise vs counter-clockwise logic
        public static Rotation smallestDiff(Rotation a, Rotation b)
        {
            return new Rotation(Math.Min(diff(a, b).Value, diff(b, a).Value));
        }
    }
}
