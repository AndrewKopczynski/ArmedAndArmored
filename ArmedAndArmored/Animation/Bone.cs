using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SFML.Graphics;
using SFML.Window;

using ArmedAndArmored.Solver;

namespace ArmedAndArmored.Animation
{
    public class Bone
    {
        private Bone previousBone;
        private List<Bone> nextBone = new List<Bone>();
        private Vector2f position;
        private Rotation rotation;
        private Rotation offset;
        private float length;

        public Bone(Vector2f pos, float len)
        {
            position = new Vector2f(pos.X, pos.Y);
            rotation = new Rotation();
            offset = new Rotation();
            length = len;
        }

        public List<Bone>NextBone
        {
            get { return nextBone; }
        }

        public Bone PreviousBone
        {
            get { return previousBone; }
            set { previousBone = value; }
        }

        public Vector2f Position
        {
            get { return position; }
            set { position = new Vector2f(value.X, value.Y); }
        }

        /* Only root bones have rotation. Child bones are slaved to the Parent. */
        public Rotation Rotation
        {
            get { return rotation; }
            set { rotation = new Rotation(value); }
        }

        /* Only child bones have offsets. Root bones rotate in the world. */
        public Rotation Offset
        {
            get { return offset; }
            set { offset = new Rotation(value); }
        }

        public float Length
        {
            get { return length; }
            set { length = value; }
        }

        public RectangleShape DebugDraw
        {
            get
            {
                RectangleShape temp = new RectangleShape(new Vector2f(1, length));
                temp.Position = position;
                temp.Rotation = rotation.Value;

                if(previousBone != null)
                {
                    if(previousBone.previousBone != null)
                    {
                        temp.FillColor = Color.Yellow;
                    }
                    else
                    {
                        temp.FillColor = Color.Red;
                    }
                }


                return temp;
            }
        }

        public void addBone(Bone bone)
        {
            nextBone.Add(bone);
            bone.previousBone = this;
        }

        public void Update()
        {
            //lowerArm.Position = new Vector2f(upperArm.Position.X + (float)(lA * System.Math.Cos(degToRad(upperArm.Rotation + SFML_ROT))), upperArm.Position.Y + (float)(lA * System.Math.Sin(degToRad(upperArm.Rotation + SFML_ROT))));
            if(previousBone != null)
            {
                //this.Position = new Vector2f(previousBone.Position.X + (float)(System.Math.Cos(previousBone.Rotation.Value + 90)), previousBone.Position.Y + (float)(length * Math.Sin(previousBone.Rotation.Value + 90)));
                this.Position = PositionSolver.solve(previousBone.Position, previousBone.Rotation, 0, previousBone.length);
                this.rotation.Value = previousBone.Rotation.Value + offset.Value;
            }
        }
    }
}
