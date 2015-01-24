using System;
using SFML.Graphics;
using SFML.Window;

namespace ArmedAndArmored
{
    class DebugCircle
    {
        private CircleShape debugCircle;
        private float debugThickness = 1.00000f;

        public DebugCircle(float radius, Vector2f center)
        {
            debugCircle = new CircleShape(radius);

            debugCircle.Position = new Vector2f(center.X - debugCircle.Radius, center.Y - debugCircle.Radius);
            debugCircle.OutlineThickness = debugThickness;
            debugCircle.FillColor = SFML.Graphics.Color.Transparent;
            debugCircle.OutlineColor = SFML.Graphics.Color.Yellow;
        }

        public CircleShape Circle { get { return debugCircle; } }
    }
}
