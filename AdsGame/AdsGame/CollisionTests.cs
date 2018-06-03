using Microsoft.Xna.Framework;
using System;

namespace AdsGame
{
    // https://gamedevelopment.tutsplus.com/tutorials/when-worlds-collide-simulating-circle-circle-collisions--gamedev-769
    // https://yal.cc/rectangle-circle-intersection-test/

    class Circle
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Radius { get; set; }
        public int Mass { get; set; }
        public float SpeedX { get; set; }
        public float SpeedY { get; set; }
        public Rectangle Bounds => bounds;

        public Circle(int x, int y, int radius, int mass)
        {
            X = x;
            Y = y;
            Radius = radius;
            Mass = mass;
            bounds = new Rectangle(
                (int)(X - Radius / 2),
                (int)(Y - Radius / 2),
                Radius * 2,
                Radius * 2);
            SpeedX = 0;
            SpeedY = 0;
        }

        private Rectangle bounds;
    }

    static class CollisionManager
    {
        // NOTE: dynamic circle, fixed rectangle
        public static bool Collide(Rectangle r1, Rectangle r2)
        {
            return r1.Intersects(r2);
        }

        public static bool Collide(Circle c, Rectangle r)
        {
            int collX, collY;
            return Collide(c, r, out collX, out collY);
        }

        public static bool Collide(Circle c, Rectangle r, out int rectX, out int rectY)
        {
            rectX = Math.Max(r.Left, Math.Min(c.X, r.Right));
            rectY = Math.Max(r.Top, Math.Min(c.Y, r.Bottom)); // TODO or switch top and botton?
            var deltaX = c.X - rectX;
            var deltaY = c.Y - rectY;
            return (deltaX * deltaX + deltaY * deltaY) < (c.Radius * c.Radius);
        }

        public static bool Update(Circle c, Rectangle r)
        {
            if (!Collide(c, r))
            {
                return false;
            }

            // Move the circle back to see where, on the rectangle, the collision took place
            int rectX, rectY;
            c.X -= (int)c.SpeedX;
            c.Y -= (int)c.SpeedY;
            Collide(c, r, out rectX, out rectY); // Returns false, we want rectX and rectY

            // TODO && Corner.TopRight in AllowedCorners
            if (rectX == r.Right && rectY == r.Top) // Upper right corner
            {
                if (c.SpeedX > 0)
                {
                    c.SpeedY = -c.SpeedY;
                }
                else if (c.SpeedY < 0)
                {
                    c.SpeedX = -c.SpeedX;
                }
                else
                {
                    c.SpeedY = -c.SpeedY;
                    c.SpeedX = -c.SpeedX;
                }
            }
            // TODO && Corner.BottomRight in AllowedCorners
            else if (rectX == r.Right && rectY == r.Bottom) // Lower right corner
            {
                if (c.SpeedX > 0)
                {
                    c.SpeedY = -c.SpeedY;
                }
                else if (c.SpeedY > 0)
                {
                    c.SpeedX = -c.SpeedX;
                }
                else
                {
                    c.SpeedY = -c.SpeedY;
                    c.SpeedX = -c.SpeedX;
                }
            }
            // TODO && Corner.BottomLeft in AllowedCorners
            else if (rectX == r.Left && rectY == r.Bottom) // Lower left corner
            {
                if (c.SpeedX < 0)
                {
                    c.SpeedY = -c.SpeedY;
                }
                else if (c.SpeedY > 0)
                {
                    c.SpeedX = -c.SpeedX;
                }
                else
                {
                    c.SpeedY = -c.SpeedY;
                    c.SpeedX = -c.SpeedX;
                }
            }
            // TODO && Corner.TopLeft in AllowedCorners
            else if (rectX == r.Left && rectY == r.Top) // Upper left corner
            {
                if (c.SpeedX < 0)
                {
                    c.SpeedY = -c.SpeedY;
                }
                else if (c.SpeedY < 0)
                {
                    c.SpeedX = -c.SpeedX;
                }
                else
                {
                    c.SpeedY = -c.SpeedY;
                    c.SpeedX = -c.SpeedX;
                }
            }
            else if (rectY == r.Top || rectY == r.Bottom) // Top and bottom edges, not the corners
            {
                c.SpeedY = -c.SpeedY;
            }
            else // Left and right edges, not corners
            {
                c.SpeedY = -c.SpeedY;
            }
            return true;
        }

        public static bool Collide(Circle c1, Circle c2)
        {
            if (!Collide(c1.Bounds, c2.Bounds))
            {
                return false;
            }
            var distanceSquared = (c1.X - c2.X) * (c1.X - c2.X) + (c1.Y - c2.Y) * (c1.Y - c2.Y);
            return distanceSquared < (c1.Radius + c2.Radius) * (c1.Radius + c2.Radius);
        }

        public static bool Collide(Circle c1, Circle c2, out float collisionX, out float collisionY)
        {
            if (Collide(c1, c2))
            {
                collisionX = (c1.X * c2.Radius + c2.X * c1.Radius) / (c1.Radius + c2.Radius);
                collisionY = (c1.Y * c2.Radius + c2.Y * c1.Radius) / (c1.Radius + c2.Radius);
                return true;
            }
            else
            {
                collisionX = 0;
                collisionY = 0;
                return false;
            }
        }

        public static bool Update(Circle c1, Circle c2)
        {
            if (Collide(c1, c2))
            {
                var newC1SpeedX = (c1.SpeedX * (c1.Mass - c2.Mass) + (2 * c2.Mass * c2.SpeedX)) / (c1.Mass + c2.Mass);
                var newC1SpeedY = (c1.SpeedY * (c1.Mass - c2.Mass) + (2 * c2.Mass * c2.SpeedY)) / (c1.Mass + c2.Mass);
                var newC2SpeedX = (c2.SpeedX * (c2.Mass - c1.Mass) + (2 * c1.Mass * c1.SpeedX)) / (c1.Mass + c2.Mass);
                var newC2SpeedY = (c2.SpeedY * (c2.Mass - c1.Mass) + (2 * c1.Mass * c1.SpeedY)) / (c1.Mass + c2.Mass);

                c1.SpeedX = newC1SpeedX;
                c1.SpeedY = newC1SpeedY;
                c2.SpeedX = newC2SpeedX;
                c2.SpeedY = newC2SpeedY;

                c1.X = c1.X + (int)c1.SpeedX;
                c1.Y = c1.Y + (int)c1.SpeedY;
                c2.X = c2.X + (int)c2.SpeedX;
                c2.Y = c2.Y + (int)c2.SpeedY;

                return true;
            }
            return false;
        }

    }

    class CollisionTests
    {
    }
}
