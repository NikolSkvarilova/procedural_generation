using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace skvarilova_rocnikova_prace
{
    class Vector2
    {
        public double X { get; set; }
        public double Y { get; set; }
        /// <summary>
        /// Constructor for Vector
        /// </summary>
        /// <param name="a">First value</param>
        /// <param name="b">Second value</param>
        public Vector2(double a, double b)
        {
            X = a;
            Y = b;
        }

        public override string ToString() 
        {
            return $"({X}, {Y})";
        }

        public static Vector2 operator +(Vector2 a, Vector2 b)
        {
            return new Vector2(a.X + b.X, a.Y + b.Y);
        }

        public static Vector2 operator +(Vector2 a, double b)
        {
            return new Vector2(a.X + b, a.Y + b);
        }

        public static Vector2 operator +(double a, Vector2 b)
        {
            return new Vector2(a + b.X, a + b.Y);
        }

        public static Vector2 operator *(Vector2 a, double b)
        {
            return new Vector2(a.X * b, a.Y * b);
        }

        public static Vector2 operator *(double a, Vector2 b)
        {
            return new Vector2(a * b.X, a * b.Y);
        }

        public static Vector2 operator *(Vector2 a, Vector2 b)
        {
            return new Vector2(a.X * b.X, a.Y * b.Y);
        }

        public static Vector2 operator -(Vector2 a, Vector2 b)
        {
            return new Vector2(a.X - b.X, a.Y - b.Y);
        }

        public static Vector2 operator -(double a, Vector2 b)
        {
            return new Vector2(a - b.X, a - b.Y);
        }
    }
}
