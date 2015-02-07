using System;
using System.Collections.Generic;
using System.Diagnostics;

using SFML.Graphics;
using SFML.Window;

using Box2DX.Collision;
using Box2DX.Common;
using Box2DX.Dynamics;

namespace ArmedAndArmored
{
    public static class Program
    {
        public const int WINDOW_WIDTH = 1280;
        public const int WINDOW_HEIGHT = 720;

        public const int ZOOM_MIN_FACTOR = 4;
        public static Vector2f ZOOM_MIN = new Vector2f(WINDOW_WIDTH / ZOOM_MIN_FACTOR, WINDOW_HEIGHT / ZOOM_MIN_FACTOR);

        public const int ZOOM_MAX_FACTOR = 3;
        public static Vector2f ZOOM_MAX = new Vector2f(WINDOW_WIDTH * ZOOM_MAX_FACTOR, WINDOW_HEIGHT * ZOOM_MAX_FACTOR);

        private static readonly SFML.Graphics.Color CornflowerBlue = new SFML.Graphics.Color(100, 149, 237);

        private static RenderWindow m_window;

        private static View m_view;
        private static float m_zoomFactor = 1.0f;

        private static int calls = 0;

        private static List<Keyboard.Key> m_keyboard = new List<Keyboard.Key>();

        public static void Main()
        {

            Vec2 gravity = new Vec2(0, 10.0f);

            AABB worldAabb = new AABB();
            worldAabb.LowerBound.Set(-0.0f, 0.0f); //negative coord
            worldAabb.UpperBound.Set(200.0f, 200.0f); //positive coord

            Box2DX.Dynamics.World world = new Box2DX.Dynamics.World(worldAabb, gravity, false);
            //CreateGround(world, 500, 500);

            Vector2f worldPos = new Vector2f(0f, 0f); ;

            m_window = new RenderWindow(new VideoMode(WINDOW_WIDTH, WINDOW_HEIGHT), "SFML Window");
            m_window.SetFramerateLimit(60);
            m_window.Closed += (sender, eventArgs) => m_window.Close();
            m_window.GainedFocus += new EventHandler(OnGainedFocus);
            m_window.LostFocus += new EventHandler(OnLostFocus);
            //m_window.MouseButtonPressed += new EventHandler<MouseButtonEventArgs>(MouseButtonPressed);
            m_window.MouseWheelMoved += new EventHandler<MouseWheelEventArgs>(OnMouseScroll);
            m_window.KeyPressed += new EventHandler<KeyEventArgs>(OnKeyDown);
            m_window.KeyReleased += new EventHandler<KeyEventArgs>(OnKeyUp);


            //m_window.MouseButtonPressed += (sender, args) =>
            //            {
            //                int mouseX = (int) worldPos.X;
            //                int mouseY = (int) worldPos.Y;
 
            //                CreateBox(world, mouseX, mouseY);
            //            };

            //Image image = new Image("image here");

            Vector2f center = new Vector2f(WINDOW_WIDTH / 2, WINDOW_HEIGHT / 2);
            Vector2f size = new Vector2f(WINDOW_WIDTH, WINDOW_HEIGHT);

            m_view = new View(center, size);


            Font font = new Font(@"..\..\fonts\arial.ttf");
            Text text = new Text("FPS: ", font);
            text.CharacterSize = 16;
            text.Color = SFML.Graphics.Color.Black;

            Stopwatch timer = Stopwatch.StartNew();

            long delta = 0;
            long frames = 0;
            int fps = 60;

            IKSolver ik = new IKSolver();

            while (m_window.IsOpen())
            {
                m_window.DispatchEvents();


                //UPDATE ===============================================================================================================
                Vector2f lineOrigin = new Vector2f(0, 0);
                float lineX = lineOrigin.X + worldPos.X;
                float lineY = lineOrigin.Y + worldPos.Y;
                double lineL = System.Math.Sqrt( (lineX * lineX) + (lineY * lineY) );
                double lineR = System.Math.Atan2(lineY, lineX);

                lineR = (lineR * 180) / System.Math.PI;

                RectangleShape debugXY = new RectangleShape(new Vector2f((float)lineL, 1));
                RectangleShape debugX = new RectangleShape(new Vector2f((float)lineX, 5));
                RectangleShape debugY = new RectangleShape(new Vector2f(5, (float)lineY));

                debugX.FillColor = SFML.Graphics.Color.Red;
                debugY.FillColor = SFML.Graphics.Color.Green;
                debugXY.Rotation = (float)lineR;

                moveCamera(delta);
                delta = timer.ElapsedMilliseconds;
                timer.Restart();

                worldPos = m_window.MapPixelToCoords(Mouse.GetPosition(m_window));

                if (frames % 15 == 0)
                    fps = updateFPS(delta);
                text.DisplayedString = "Approx FPS: " + fps + " | Frame Time: " + delta + "ms"
                    + " Zoom: " + m_zoomFactor.ToString() + "View:" + m_view.Size.X.ToString() + "," + m_view.Size.Y.ToString()
                    + " Calls: " + calls.ToString()
                    + " X: " + (m_view.Center.X - (m_view.Size.X / 2))
                    + " Y: " + (m_view.Center.Y - (m_view.Size.Y / 2))
                    + " \nMouse:" + worldPos.ToString()
                    + " \nMeters X:" + (worldPos.X / 30f) + "m"
                    + " \nMeters Y:" + (worldPos.Y / 30f) + "m"
                    + " \nWorld Bodies:" + world.GetBodyCount().ToString();
                   ;

                world.Step(1 / (float)fps, 8, 1);
                m_window.Clear(CornflowerBlue);

                //test class

                RectangleShape armA = new RectangleShape(new Vector2f(10, 200));
                RectangleShape armB = new RectangleShape(new Vector2f(10, 200));

                armA.Position = new Vector2f(500, 500);

                armA.FillColor = SFML.Graphics.Color.Cyan;
                armB.FillColor = SFML.Graphics.Color.Magenta;

                ik.solve(armA, armB, worldPos, delta);

                armA = new RectangleShape(ik.UpperArm);
                armB = new RectangleShape(ik.LowerArm);

                text.DisplayedString = ik.WentFast.ToString();

                //DRAW =================================================================================================================
                m_window.Draw(debugX);
                m_window.Draw(debugY);
                m_window.Draw(debugXY);
                m_window.Draw(armA);
                m_window.Draw(armB);
                m_window.Draw(ik.UpperArmReachRadius);
                m_window.Draw(ik.LowerArmReachRadius);

                Body body = world.GetBodyList();
                while (body.GetNext() != null)
                {
                    Sprite box = (Sprite)body.GetUserData();

                    box.Position = new Vector2f(30.0f * body.GetPosition().X, 30.0f * body.GetPosition().Y);

                    box.Rotation = body.GetAngle() * 180 / Settings.Pi;

                    m_window.Draw(box);

                    body = body.GetNext();
                }

                //
                View temp = new View(new FloatRect(0, 0, WINDOW_WIDTH, WINDOW_HEIGHT));
                updateDebugText(text);
                m_window.SetView(temp);
                m_window.Draw(text);
                m_window.Display();
                frames++;
                //low fps test
                //System.Threading.Thread.Sleep(100);
            }
        }

        static void updateMe(Sprite sprite, long delta)
        {
            if (sprite.Position.X < WINDOW_WIDTH)
                sprite.Position = new Vector2f(sprite.Position.X + 1, 0);
            else
                sprite.Position = new Vector2f(0 - sprite.TextureRect.Width, 0);
            //sprite.Scale = new Vector2f(sprite.Position.X/10, 1f);
        }

        static void moveCamera(long delta)
        {
            float amount = 300f * (delta / 1000f);

            if (IsKeyDown(Keyboard.Key.LShift))
                amount *= 15f;

            if (IsKeyDown(Keyboard.Key.W))
                m_view.Move(new Vector2f(0, -amount));
            else if (IsKeyDown(Keyboard.Key.S))
                m_view.Move(new Vector2f(0, amount));

            if (IsKeyDown(Keyboard.Key.A))
                m_view.Move(new Vector2f(-amount, 0));
            else if (IsKeyDown(Keyboard.Key.D))
                m_view.Move(new Vector2f(amount, 0));

            if (IsKeyDown(Keyboard.Key.Space))
            {
                m_view.Reset(new FloatRect(-600, -350, WINDOW_WIDTH, WINDOW_HEIGHT));
            }

            float smoothAmount = (float)0.8f * (delta / 1000f);

            if (m_zoomFactor > 1f)
            {
                if (m_zoomFactor - smoothAmount < 1f)
                    m_zoomFactor = 1f;
                else
                    m_zoomFactor -= smoothAmount;
            }
            else if (m_zoomFactor < 1f)
            {
                if (m_zoomFactor + smoothAmount > 1f)
                    m_zoomFactor = 1f;
                else
                    m_zoomFactor += smoothAmount;
            }

            m_view.Zoom(m_zoomFactor);

            if (m_view.Size.X > ZOOM_MAX.X && m_view.Size.Y > ZOOM_MAX.Y)
                m_view.Size = ZOOM_MAX;
            else if (m_view.Size.X < ZOOM_MIN.X && m_view.Size.Y < ZOOM_MIN.Y)
                m_view.Size = ZOOM_MIN;

            m_window.SetView(m_view);
        }

        static void updateDebugText(Text text)
        {
            //text.Position = new Vector2f(m_view.Center.X - (m_view.Size.X / 2), m_view.Center.Y - (m_view.Size.Y / 2));
        }

        static int updateFPS(long delta)
        {
            return (int)(1f / (((double)delta) / 1000f));
        }

        static void OnMouseScroll(object sender, MouseWheelEventArgs e)
        {
            //System.Windows.Forms.MessageBox.Show("aaa");
            m_zoomFactor -= 0.05f * e.Delta;
        }

        static void OnGainedFocus(object sender, EventArgs e)
        {
            //use for future click on
        }

        static void OnLostFocus(object sender, EventArgs e)
        {
            //use for future click off
        }

        static void MouseButtonPressed(object sender, MouseButtonEventArgs e)
        {
            //keep for future clickin n dickin
        }

        static void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (!m_keyboard.Contains(e.Code))
                m_keyboard.Add(e.Code);
        }

        static void OnKeyUp(object sender, KeyEventArgs e)
        {
            m_keyboard.Remove(e.Code);
        }

        static bool IsKeyDown(Keyboard.Key key)
        {
            return m_keyboard.Contains(key);
        }

        static void CreateGround(Box2DX.Dynamics.World world, float x, float y)
        {
            float platformWidth = 400f;

            // We need to define a body with the position
            BodyDef bodyDef = new BodyDef();

            // Box2D doesn't use pixel measurement
            // So we need to divide by 30
            // 1m = 30px
            bodyDef.Position.Set(x / 30.0f, y / 30.0f);

            // Create the physics body
            Body body = world.CreateBody(bodyDef);

            // Define a new shape def
            PolygonDef shapeDef = new PolygonDef();
            shapeDef.SetAsBox((platformWidth) / 30.0f, (16.0f / 2) / 30.0f);
            shapeDef.Density = 0.0f;

            // Create a texture from filename
            Texture groundTexture = new Texture("../../graphics/forest/tile_grass_3.bmp");
            groundTexture.Repeated = true;

            // Create a sprite based on texture
            // In order to draw it properly we need
            // to set the origin
            Sprite sprite = new Sprite(groundTexture) { Origin = new Vector2f(platformWidth, 8) };
            sprite.TextureRect = new IntRect(0, 0, 800, 40);

            // We then set the user data in the body
            // so we can access it later on
            body.SetUserData(sprite);

            // Finalize the shape and body
            body.CreateShape(shapeDef);
            body.SetMassFromShapes();
        }

        // Create a box
        public static void CreateBox(Box2DX.Dynamics.World world, int mouseX, int mouseY)
        {
            // We need to define a body with the position
            BodyDef bodyDef = new BodyDef();

            // Box2D doesn't use pixel measurement
            // So we need to divide by 30
            // 1m = 30px
            bodyDef.Position.Set(mouseX / 30.0f, mouseY / 30.0f);

            // Create the physics body
            Body body = world.CreateBody(bodyDef);

            // Define a new shape def
            PolygonDef shapeDef = new PolygonDef();
            shapeDef.SetAsBox((32.0f / 2) / 30.0f, (32.0f / 2) / 30.0f);

            // Set the shape properties
            shapeDef.Density = 1.0f;
            shapeDef.Friction = 0.8f;
            shapeDef.Restitution = 1.1f;

            // Create a texture from filename
            Texture boxTexture = new Texture("../../graphics/forest/box.bmp");

            // Create a sprite based on texture
            // In order to draw it properly we need
            // to set the origin
            Sprite sprite = new Sprite(boxTexture) { Origin = new Vector2f(16, 16) };

            // We then set the user data in the body
            // so we can access it later on
            body.SetUserData(sprite);

            // Finalize the shape and body
            body.CreateShape(shapeDef);
            body.SetMassFromShapes();
            calls++;
        }
    }
}