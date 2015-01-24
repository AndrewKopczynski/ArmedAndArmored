using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SFML.Graphics;
using SFML.Window;

namespace ArmedAndArmored
{
    //Solves IK for two RectangleShapes for very very basic inverse kinematics
    class IKSolver
    {
        //SFML rotational offset
        const float SFML_ROT = 90.00000f;

        private RectangleShape upperArm;
        private RectangleShape lowerArm;

        private float upperSolveable;
        private float lowerSolveable;
        private float lA;
        private float lB;

        private float speed = 25.0f;

        private float rotA;
        private float lastRotA;

        private float rotB;
        private float lastRotB;

        private bool wentFast;

        public void solve(RectangleShape upper, RectangleShape lower, Vector2f pointAt, double delta)
        {
            upperArm = upper;
            lowerArm = lower;

            lA = upperArm.Size.Y;
            lB = lowerArm.Size.Y;

            upperSolveable = lA + lB;
            lowerSolveable = Math.Abs(lA - lB);

            //Figure out basic angle to get arm to point at
            double XL = Math.Atan2(upperArm.Position.Y - pointAt.Y, upperArm.Position.X - pointAt.X);

            //Solve length of the hypotenuse and set limits on L to prevent render bugs
            double L = Math.Pow(upperArm.Position.X - pointAt.X, 2) + Math.Pow(upperArm.Position.Y - pointAt.Y, 2);
            L = Math.Max(Math.Min(Math.Sqrt(L), upperSolveable), lowerSolveable);

            //Cos law
            double a = Math.Acos((Math.Pow(lA, 2) + Math.Pow(L, 2) - Math.Pow(lB, 2)) / (2 * lA * L));
            double b = Math.Acos((Math.Pow(lA, 2) + Math.Pow(lB, 2) - Math.Pow(L, 2)) / (2 * lA * lB));

            //Convert stuff so it's easier
            //TODO: maybe do this stuff in rad and convert just before adjust
            XL = radToDeg(XL);
            a = radToDeg(a);
            b = radToDeg(b);

            //SFML offset
            XL += SFML_ROT;

            //Apply rotations
            rotA = (float)(XL - a);
            rotB = (float)(180 - b + rotA);

            //rotA += SFML_ROT;
            //rotB += SFML_ROT;

            float temp = (float)(speed * (delta / 1000f));

           
            System.Console.WriteLine(rotB.ToString() + " vs " + lastRotB.ToString());

            if (lastRotA + temp > rotA && lastRotA - temp < rotA)
            {
                wentFast = false;
            }
            else if (rotA - lastRotA > temp)
            {
                wentFast = true;
                rotA = lastRotA + temp;
            }
            else if (rotA - lastRotA < temp)
            {
                wentFast = true;
                rotA = lastRotA - temp;
            }
            else
                wentFast = false;

            if (lastRotB + temp > rotB && lastRotB - temp < rotB)
            {
                wentFast = false;
            }
            else if (rotB - lastRotB > temp)
            {
                wentFast = true;
                rotB = lastRotB + temp;
            }
            else if (rotB - lastRotB < temp)
            {
                wentFast = true;
                rotB = lastRotB - temp;
            }
            else
                wentFast = false;
     
 

            lastRotA = rotA;
            lastRotB = rotB;

            upperArm.Rotation = rotA;
            lowerArm.Rotation = rotB;

            //Adjust positioning of second arm now
            lowerArm.Position = new Vector2f(upperArm.Position.X + (float)(lA * System.Math.Cos(degToRad(upperArm.Rotation + SFML_ROT))), upperArm.Position.Y + (float)(lA * System.Math.Sin(degToRad(upperArm.Rotation + SFML_ROT))));
        }

        public RectangleShape UpperArm { get { return new RectangleShape(upperArm); } }
        public RectangleShape LowerArm { get { return new RectangleShape(lowerArm); } }
        public CircleShape UpperArmReachRadius { get { return new DebugCircle(lA, upperArm.Position).Circle; } }
        public CircleShape LowerArmReachRadius { get { return new DebugCircle(lB, lowerArm.Position).Circle; } }
        public bool WentFast { get { return wentFast; } }

        //because I'm lazy
        private double radToDeg(double a) { return (a * 180) / Math.PI; }
        private double degToRad(double a) { return (a * Math.PI) / 180; }
    }
}
