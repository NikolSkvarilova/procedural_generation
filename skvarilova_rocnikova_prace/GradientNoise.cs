using System;
using System.Windows;

namespace skvarilova_rocnikova_prace
{
    /// <summary>
    /// Another version of value noise, which tends to make it more smooth and round, as opposite to blocky (value noise)
    /// We interpolate random gradients, instead of values
    /// Random function here returns a direction (2D vector) instead of single double
    /// </summary>
    class GradientNoise : ValueNoise
    {
        public GradientNoise() { }

        /// <summary>
        /// Calculate pseudorandom value from vector
        /// </summary>
        /// <param name="st">Input vector</param>
        /// <returns>Pseudorandom value</returns>
        public new Vector2 Random(Vector2 st)
        {
            st = new Vector2(   Dot(st, new Vector2(127.1, 311.7)),
                                Dot(st, new Vector2(269.5, 183.3)));
            st.X = Math.Sin(st.X);
            st.Y = Math.Sin(st.Y);
            return -1.0 + 2.0 * Fract(st * 43758.5453123);
        }
        
        /// <summary>
        /// Calculating the nosie value
        /// </summary>
        /// <param name="st">Input vector (x, y positions)</param>
        /// <returns>Number in the range <0; 1></returns>
        public new double Noise(Vector2 st)
        {
            Vector2 i = Floor(st); // The decimal part
            Vector2 f = Fract(st); // The fractal part 
            Vector2 u = f * f * (3.0 - 2.0 * f);
            double num = Mix(
                            Mix(
                                Dot(Random(i + new Vector2(0.0, 0.0)), f - new Vector2(0.0, 0.0)),
                                Dot(Random(i + new Vector2(1.0, 0.0)), f - new Vector2(1.0, 0.0)), u.X),
                            Mix(
                                Dot(Random(i + new Vector2(0.0, 1.0)), f - new Vector2(0.0, 1.0)),
                                Dot(Random(i + new Vector2(1.0, 1.0)), f - new Vector2(1.0, 1.0)), u.X), 
                        u.Y);
            return (num + 1.0) * 0.5; ;
        }
    }
}
