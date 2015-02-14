using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SFML.Window;
using ArmedAndArmored.Solver;

namespace ArmedAndArmored.Solver
{
    //Figures out how two rectangles should 'connect'.
    public static class PositionSolver
    {
        public const float SFML_ROT = 90.0f;

        public static Vector2f solve(Vector2f position, Rotation rotation, float xLen, float yLen)
        {
            ////lowerArm.Position = 
            //new Vector2f(
            //upperArm.Position.X + (float)(lA * System.Math.Cos(degToRad(upperArm.Rotation + SFML_ROT))),
            //upperArm.Position.Y + (float)(lA * System.Math.Sin(degToRad(upperArm.Rotation + SFML_ROT))));
            float x;
            float y;

            x = position.X + (float)(yLen * Math.Cos(Angle.degToRad(rotation.Value + SFML_ROT)));
            y = position.Y + (float)(yLen * Math.Sin(Angle.degToRad(rotation.Value + SFML_ROT)));

            return new Vector2f(x, y);
        }
    }
}
