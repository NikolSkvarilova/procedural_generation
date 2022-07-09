using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace skvarilova_rocnikova_prace
{
    class ValueNoise
    {
        // Great source: https://thebookofshaders.com/11/

        private Vector2[] edgeVectors =
        {
            new Vector2(1.0, 0.0),
            new Vector2(0.0, 1.0),
            new Vector2(1.0, 1.0)
        };

        public ValueNoise() { }

        /// <summary>
        ///  This functions does linear interpolation between two values with some sort of weight.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="weight"></param>
        /// <returns>Interpolated value</returns>
        public double Mix(double start, double end, double weight)
        {
            return start * (1 - weight) + end * weight;
        }

        /// <summary>
        /// Generates pseudo-random value.
        /// </summary>
        /// <param name="num">Seed</param>
        /// <returns>Random number generated from the input</returns>
        public double Rand(double num)
        {
            double tmp = Math.Sin(num) * 1.0;
            double res = Fract(tmp);
            return res;
        }

        /// <summary>
        /// Extracts the decimal part of number but as a double
        /// </summary>
        /// <param name="num"></param>
        /// <returns>8.5 --> 8.0</returns>
        public double Floor(double num)
        {
            return Math.Truncate(num) * 1.0;
        }

        /// <summary>
        /// Calculates the dot product of two vectors
        /// </summary>
        /// <param name="a">Vector 1</param>
        /// <param name="b">Vector 2</param>
        /// <returns>Their dot product</returns>
        public double Dot(Vector2 a, Vector2 b)
        {
            return a.X * b.X + a.Y * b.Y;
        }

        /// <summary>
        /// Return the fractal part of double
        /// </summary>
        /// <param name="num">Input number</param>
        /// <returns>8.5 --> 0.5</returns>
        public double Fract(double num)
        {
            double n = num - Math.Truncate(num);
            return Math.Abs(n);
        }

        /// <summary>
        /// If the n is lower than min_v or bigger than the max_v, it returns min_v or max_v.
        /// If it is in the range, it returns n.
        /// </summary>
        /// <param name="n">The number you want to clamp</param>
        /// <param name="min_v">Minimal value</param>
        /// <param name="max_v">Maximal value</param>
        /// <returns></returns>
        public double Clamp(double n, double min_v, double max_v)
        {
            if (n < min_v) return min_v;
            if (n > max_v) return max_v;
            return n;
        }

        /// <summary>
        /// Perform Hermite interpolation
        /// </summary>
        /// <param name="e0">First edge</param>
        /// <param name="e1">Second edge</param>
        /// <param name="x">Number you want to calculate the value for</param>
        /// <returns>Interpolated value</returns>
        public double Smoothstep(double e0, double e1, double x)
        {
            // Hermite interpolation
            double t = Clamp((x - e0) / (e1 - e0), 0.0, 1.0);
            return t * t * (3.0 - 2.0 * t);
        }

        /// <summary>
        /// Run the floor function against a vector
        /// </summary>
        /// <param name="st">Input vector</param>
        /// <returns>Vector with floored x, y</returns>
        public Vector2 Floor(Vector2 st)
        {
            return new Vector2(Floor(st.X), Floor(st.Y));
        }

        /// <summary>
        /// Run the fract function against a vector
        /// </summary>
        /// <param name="st">Input vector</param>
        /// <returns>Vector with fracted x, y</returns>
        public Vector2 Fract(Vector2 st)
        {
            return new Vector2(Fract(st.X), Fract(st.Y)); ;
        }

        /// <summary>
        /// Generates a pseudorandom value from vector
        /// </summary>
        /// <param name="st">Input vector</param>
        /// <returns>Pseudoranom value</returns>
        public double Random(Vector2 st)
        {
            return Fract(Math.Sin(Dot(st, new Vector2(12.9898, 78.233))) * 43758.5453123);
        }

        /// <summary>
        /// Calculate the noise value
        /// </summary>
        /// <param name="st">Input vector (x, y position)</param>
        /// <returns>Number in the range <0; 1></returns>
        public double Noise(Vector2 st)
        {
            Vector2 i = Floor(st); // The decimal part
            Vector2 f = Fract(st); // The fractal part 

            double a = Random(i);
            double b = Random(i + edgeVectors[0]);
            double c = Random(i + edgeVectors[1]);
            double d = Random(i + edgeVectors[2]);

            Vector2 u = new Vector2(Smoothstep(0.0, 1.0, f.X), Smoothstep(0.0, 1.0, f.Y));

            return Mix(a, b, u.X) + (c - a) * u.Y * (1.0 - u.X) + (d - b) * u.X * u.Y;
        }
    }
}
