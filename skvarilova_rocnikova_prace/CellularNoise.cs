using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace skvarilova_rocnikova_prace
{
    /// <summary>
    /// Cellular noise works a bit differently. According to its different behavior, we have pay attention to more variables and attributes.
    /// </summary>
    class CellularNoise
    {
        // Resolution of the image
        public int ResX { get; set; }
        public int ResY { get; set; }
        // The random points
        private Vector2[] Points { get; set; }
        // Number of those points
        public int NumOfPoints { get; set; }
        public bool Dark { get; set; }

        /// <summary>
        /// Constructor of the cellular noise function
        /// </summary>
        /// <param name="n">Number of points</param>
        /// <param name="resolution_x">Width of the image</param>
        /// <param name="resolution_y">Height of the image</param>
        public CellularNoise(int n, int resolution_x, int resolution_y)
        {
            ResX = resolution_x;
            ResY = resolution_y;
            NumOfPoints = n;
            GeneratePoints();
        }

        /// <summary>
        /// Generates random points on the surface
        /// </summary>
        public void GeneratePoints()
        {
            Points = new Vector2[NumOfPoints];
            Random r = new Random();
            for (int i = 0; i < NumOfPoints; i++)
            {
                Points[i] = new Vector2(r.Next(0, ResX), r.Next(0, ResY));
            }
        }

        /// <summary>
        /// Calculate the distance between two points
        /// </summary>
        /// <param name="a">First point</param>
        /// <param name="b">Second point</param>
        /// <returns>Distavce between first and second point</returns>
        public double GetDistance(Vector2 a, Vector2 b)
        {
            return Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2));
        }

        /// <summary>
        /// Calculate the noise value
        /// </summary>
        /// <param name="st">Input vector (x, y position)</param>
        /// <returns>Number in the range <0; 1></returns>
        public double Noise(Vector2 st)
        {
            double m_dist = double.MaxValue;
            for (int i = 0; i < Points.Count(); i++)
            {
                double dist = GetDistance(st, Points[i]);
                m_dist = Math.Min(m_dist, dist);
            }
            if (Dark) return 1.0 - (m_dist / ResY);
            return m_dist / ResY;
        }
    }
}
