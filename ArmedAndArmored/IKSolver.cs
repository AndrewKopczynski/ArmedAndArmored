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

        private double upperArmSpeed = -1;
        private double lowerArmSpeed = -1;

        private Rotation rotA = new Rotation();
        private Rotation lastRotA = new Rotation();

        private Rotation rotB = new Rotation();
        private Rotation lastRotB = new Rotation();

        private bool wentFast;

        public void solve(RectangleShape upper, RectangleShape lower, Vector2f pointAt, double delta)
        {
            float speed;        //holder for speed
            float cw;           //used to figure out if the arm should go clockwise or not
            float ccw;          //same as cw, but for counter clockwise

            wentFast = false;
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
            XL = radToDeg(XL);
            a = radToDeg(a);
            b = radToDeg(b);

            //SFML offset, because 0 degrees is down in SFML
            XL += SFML_ROT;

            //Apply rotations
            rotA.Rot = (float)(XL - a);
            rotB.Rot = (float)(180 - b + rotA.Rot);

            if (upperArmSpeed > 0)
            {
                ccw = RotationSolver.diff(rotA, lastRotA).Rot;
                cw = RotationSolver.diff(lastRotA, rotA).Rot;
                speed = (float)Delta.calc(upperArmSpeed, delta);

                if (RotationSolver.smallestDiff(rotA, lastRotA).Rot > speed)
                {
                    wentFast = true;
                    if (cw > ccw)
                    {
                        rotA.Rot = lastRotA.Rot + speed;
                    }
                    else if (ccw > cw)
                    {
                        rotA.Rot = lastRotA.Rot - speed;
                    }
                }
            }

            if (lowerArmSpeed > 0)
            { 
                ccw = RotationSolver.diff(rotB, lastRotB).Rot;
                cw = RotationSolver.diff(lastRotB, rotB).Rot;
                speed = (float)Delta.calc(lowerArmSpeed, delta);

                if (Math.Abs(RotationSolver.smallestDiff(rotB, lastRotB).Rot) > speed)
                {
                    wentFast = true;
                    if (cw > ccw)
                    {
                        rotB.Rot = lastRotB.Rot + speed;
                    }
                    else if (ccw > cw)
                    {
                        rotB.Rot = lastRotB.Rot - speed;
                    }
                }
            }

            lastRotA.Rot = rotA.Rot;
            lastRotB.Rot = rotB.Rot;

            upperArm.Rotation = rotA.Rot;
            lowerArm.Rotation = rotB.Rot;

            //Adjust positioning of second arm now
            lowerArm.Position = new Vector2f(upperArm.Position.X + (float)(lA * System.Math.Cos(degToRad(upperArm.Rotation + SFML_ROT))), upperArm.Position.Y + (float)(lA * System.Math.Sin(degToRad(upperArm.Rotation + SFML_ROT))));
        }

        public RectangleShape UpperArm { get { return new RectangleShape(upperArm); } }
        public RectangleShape LowerArm { get { return new RectangleShape(lowerArm); } }
        public CircleShape UpperArmReachRadius { get { return new DebugCircle(lA, upperArm.Position).Circle; } }
        public CircleShape LowerArmReachRadius { get { return new DebugCircle(lB, lowerArm.Position).Circle; } }
        public bool WentFast { get { return wentFast; } }
        public double UpperArmSpeed
        {
            get { return upperArmSpeed; }
            set { upperArmSpeed = value; }
        }

        //because I'm lazy
        private double radToDeg(double a) { return (a * 180) / Math.PI; }
        private double degToRad(double a) { return (a * Math.PI) / 180; }
    }
}
