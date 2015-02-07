using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmedAndArmored
{
    //Solves degree rotation and roll-over problems for me.
    class Rotation
    {
        private float rotation;

        public Rotation() { }

        public Rotation(float rot)
        {
            rotation = solveRotation(rot);
        }

        private float solveRotation(float rot)
        {
            //reduce rotations over 360 to [0-359] ones
            if(rot > 360)
            {
                rot = rot % 360;
            }

            //turn negative rotations into positive ones
            while (rot < 0)
            {
                rot = 360 - (-1 * rot);
            }

            return rot;
        }

        public float Rot
        {
            get { return rotation; }
            set { rotation = solveRotation(value); }
        }

        public override string ToString()
        {
            return "[RotationSolver] " + rotation.ToString();
        }
    }
}
