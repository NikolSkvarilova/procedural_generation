using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace skvarilova_rocnikova_prace
{
    /// <summary>
    /// Object storing the color and threshold
    /// </summary>
    public class D_color
    {
        private char[] AllowedChar = { 'A', 'B', 'C', 'D', 'E', 'F', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '#' };
        private string hex;
        private double threshold;
        public string Hex
        {
            get
            {
                return hex;
            }
            set
            {
                bool tmp = true;
                for (int i = 0; i < value.Length; i++)
                {
                    if (!AllowedChar.Contains(value.ToUpper()[i])) {
                        tmp = false;
                    }
                    else
                    { }
                }
                if (tmp) 
                {
                    if (value.Contains('#'))
                    {
                        hex = value.ToUpper();
                    }
                    else
                    {
                        hex = '#' + value.ToUpper();
                    }
                }
            }
        }
        public double Threshold
        {
            get
            {
                return threshold;
            }
            set
            {
                if (value <= 1 && value >=0)
                {
                    threshold = value;
                }
            }
        }

        /// <summary>
        /// Constructor for the D_color object
        /// </summary>
        /// <param name="HEX">Color in HEX format</param>
        /// <param name="th">Threshold</param>
        public D_color(string HEX, double th)
        {
            if (HEX.Contains('#'))
            {
                hex = HEX.ToUpper();
            }
            else
            {
                hex = '#' + HEX.ToUpper();
            }
            threshold = th;
        }

        public D_color() { }

        /// <summary>
        /// Transform HEX to RGB
        /// </summary>
        /// <returns>int[] with RGB values</returns>
        public int[] GetRGB()
        {
            int[] Res = new int[]
            {
                Convert.ToInt32($"{hex[1]}{hex[2]}", 16),
                Convert.ToInt32($"{hex[3]}{hex[4]}", 16),
                Convert.ToInt32($"{hex[5]}{hex[6]}", 16)
            };
            
            return Res;
        }
    }
}
