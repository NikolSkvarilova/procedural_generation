using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using Color = System.Drawing.Color;

namespace skvarilova_rocnikova_prace
{
    class Render : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        // Bias (seed) is used to generate new image each time.
        // It is a number which is added to x, y positions and then the sum is given to the noise function as a position.
        // So instead the x, y being in the range 0 -> width, 0 -> height, the range is bias -> width + bias, bias -> height + bias
        public int Bias { get; set; }
        public ObservableCollection<D_color> Colors { get; set; }
        public Bitmap Image;
        // Where is the image being saved
        public string Filename;
        // Whenever we set new width/height, we must create new Bitmap with the new size
        private int width;
        private int height;
        public int Width {
            get { return width; }
            set
            {
                width = value;
                Image = new Bitmap(width, height, PixelFormat.Format24bppRgb);
            }
        }
        public int Height
        {
            get { return height; }
            set
            {
                height = value;
                Image = new Bitmap(width, height, PixelFormat.Format24bppRgb);
            }
        }
        // Used to zoom in and to make the step between each pixel smaller
        public double NoiseScale { get; set; }
        private Random _random { get; set; }

        // Instances of the noises

        private ValueNoise valueNoise { get; set; }
        private GradientNoise gradientNoise { get; set; }
        private CellularNoise cellularNoise { get; set; }
        // Interface fpr communitating with the internal settings of the noise.
        public int NumOfCells 
        { 
            get
            {
                return cellularNoise.NumOfPoints;
            }
            set
            {
                cellularNoise.NumOfPoints = value;
                cellularNoise.GeneratePoints();
            }
        }

        public Render(int w, int h, ObservableCollection<D_color> colors, int n) // Width, height of the grid
        {
            width = w;
            height = h;
            Image = new Bitmap(width, height, PixelFormat.Format24bppRgb);
            Filename = "./image.png";
            NoiseScale = 0.05;
            Colors = colors;
            _random = new Random();
            Bias = _random.Next(5000);
            valueNoise = new ValueNoise();
            gradientNoise = new GradientNoise();
            cellularNoise = new CellularNoise(n, width, height);
            NumOfCells = n;
        }
        /// <summary>
        /// Generates random seed for value and gradient noise. Generates new points for cellular noise.
        /// </summary>
        private void PrepareRandomSeed()
        {
            Bias = _random.Next(5000);
            CallChange("bias");
            PrepareCellularNoise();
            cellularNoise.GeneratePoints();
        }
        /// <summary>
        /// Updates the width and height set in cellular noise.
        /// </summary>
        private void PrepareCellularNoise()
        {
            cellularNoise.ResX = width;
            cellularNoise.ResY = height;
        }
        /// <summary>
        /// Pick the correct noise, calculate the value of the noise.
        /// </summary>
        /// <param name="noise">Type of the noise ("value", "gradient", "cellular")</param>
        /// <param name="st">2D vector containing the x, y position</param>
        /// <returns>Value in the range <0, 1>.</returns>
        private double CalculateNoise(string noise, Vector2 st)
        {
            switch (noise)
            {
                case "value":
                    return CalculateValueNoise(st);
                case "gradient":
                    return CalculateGradientNoise(st);
                case "cellular":
                    PrepareCellularNoise();
                    return CalculateCellularNoise(st);
                default:
                    return _random.NextDouble();
            }
        }

        /// <summary>
        /// Use Noise funciton of the Value Noise
        /// </summary>
        /// <param name="st">2D vector containing the position</param>
        /// <returns>Value of the noise</returns>
        private double CalculateValueNoise(Vector2 st)
        {
            Vector2 v = new Vector2((st.X + Bias) * NoiseScale, (st.Y + Bias) * NoiseScale);
            return valueNoise.Noise(v);
        }

        /// <summary>
        /// Use Noise funciton of the Gradient Noise
        /// </summary>
        /// <param name="st">2D vector containing the position</param>
        /// <returns>Value of the noise</returns>
        private double CalculateGradientNoise(Vector2 st)
        {
            Vector2 v = new Vector2((st.X + Bias) * NoiseScale, (st.Y + Bias) * NoiseScale);
            return gradientNoise.Noise(v);
        }

        /// <summary>
        /// Use Noise funciton of the Cellular Noise
        /// </summary>
        /// <param name="st">2D vector containing the position</param>
        /// <returns>Value of the noise</returns>
        private double CalculateCellularNoise(Vector2 st)
        {
            Vector2 v = new Vector2(st.X, st.Y);
            PrepareCellularNoise();
            return cellularNoise.Noise(v);
        }

        /// <summary>
        /// Based on the calculated noise value, pick the color from the color list by threshold.
        /// </summary>
        /// <param name="val">The calculated value in the range <0; 1></param>
        /// <param name="colors">Sorted list of D_color objects.</param>
        /// <returns></returns>
        private int[] GetColorFromNoiseValue(double val, List<D_color> colors)
        {
            int[] c = { 0, 0, 0 };
            foreach (var entry in colors)
            {
                if (val < entry.Threshold)
                {
                    c = entry.GetRGB();
                    break;
                }
            }
            return c;
        }

        /// <summary>
        /// Maps value in one range to another
        /// </summary>
        /// <param name="s">The value you want to map from the first range</param>
        /// <param name="a1">Start of the first range</param>
        /// <param name="a2">End of the first range</param>
        /// <param name="b1">Start of the second range</param>
        /// <param name="b2">End of the second range</param>
        /// <returns>Mapped value</returns>
        private double Map(double s, double a1, double a2, double b1, double b2)
        {
            return b1 + (s - a1) * (b2 - b1) / (a2 - a1);
        }

        /// <summary>
        /// Calculate the gradient color between two thresholds
        /// </summary>
        /// <param name="noise_value">Value in the range <0; 1> calculated by Noise function</param>
        /// <param name="colors">Sorted list of D_color objects</param>
        /// <returns>RGB array</returns>
        private double[] CalculateGradient(double noise_value, List<D_color> colors)
        {
            double[] res = { 0, 0, 0 };
            int[] c1 = { 0, 0, 0 };
            int[] c2 = { 0, 0, 0 };
            double t1 = 0;
            double t2 = 0;
            // Picking the colors and thresholds where the value sits in between
            for (int i = 0; i < colors.Count(); i++)
            {
                if (noise_value < colors[i].Threshold)
                {
                    c1 = colors[i].GetRGB();
                    t1 = colors[i].Threshold;
                    if (i > 0)
                    {
                        c2 = colors[i - 1].GetRGB();
                        t2 = colors[i - 1].Threshold;
                    }
                    else
                    {
                        c2 = c1;
                        t2 = t1;
                    }
                    break;
                }
            }
            double new_value = Map(noise_value, t2, t1, 0.0, 1.0);
            res[0] = new_value * (double)c1[0] + (1 - new_value) * (double)c2[0];
            res[1] = new_value * (double)c1[1] + (1 - new_value) * (double)c2[1];
            res[2] = new_value * (double)c1[2] + (1 - new_value) * (double)c2[2];
            return res;
        }

        /// <summary>
        /// Generates new set of pixels and saves them to the bitmap
        /// </summary>
        /// <param name="colorMode">"Colored" or "gray"</param>
        /// <param name="random_seed">Calculate the seed randomly</param>
        /// <param name="noise">Type of the noise ("value", "gradient", "cellular")</param>
        /// <param name="gradient">Calculate the gradient between two colors</param>
        /// <param name="dark">Used for cellular noise - dark & light closest points</param>
        public void ChangePixels(string colorMode, bool random_seed, string noise, bool gradient, bool dark)
        {
            if (random_seed) { PrepareRandomSeed(); }
            if (dark) { cellularNoise.Dark = true; }
            else { cellularNoise.Dark = false; }

            double noise_value;

            // Ordered list of D_color objects
            List<D_color> colors = new List<D_color>(Colors);
            colors = colors.OrderBy(o => o.Threshold).ToList();

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    noise_value = CalculateNoise(noise, new Vector2(x, y));

                    if (colorMode == "colored")
                    {
                        if (gradient)
                        {
                            double[] c = CalculateGradient(noise_value, colors);
                            Image.SetPixel(x, y, Color.FromArgb((byte)c[0], (byte)c[1], (byte)c[2]));
                        } else                                                           
                        {
                            int[] c = GetColorFromNoiseValue(noise_value, colors);
                            Image.SetPixel(x, y, Color.FromArgb((byte)c[0], (byte)c[1], (byte)c[2]));
                        }
                    } else if (colorMode == "gray")
                    {
                        Image.SetPixel(x, y, Color.FromArgb((byte)(noise_value * 255), (byte)(noise_value * 255), (byte)(noise_value * 255)));    
                    } else
                    {
                        // Random colors
                        Image.SetPixel(x, y, Color.FromArgb((byte)_random.Next(0, 255), (byte)_random.Next(0, 255), (byte)_random.Next(0, 255)));
                    }
                }
            }
        }

        /// <summary>
        /// Used for drawing the Bitmap; the code was found here https://stackoverflow.com/questions/22499407/how-to-display-a-bitmap-in-a-wpf-image
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }

        /// <summary>
        /// Saving the image to PNG file
        /// </summary>
        /// <param name="image">The image itself</param>
        /// <param name="filePath">Place to save it</param>
        public void Save(BitmapImage image, string filePath)
        {
            BitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(image));

            using (var fileStream = new System.IO.FileStream(filePath, System.IO.FileMode.Create))
            {
                encoder.Save(fileStream);
            }
        }

        /// <summary>
        /// Used when saving the image
        /// </summary>
        /// <param name="bitmap">Bitmap containing the image</param>
        /// <returns>BitmapImage which you can pass to the Save method</returns>
        public BitmapImage ToBitmapImage(Bitmap bitmap)
        {
            using (var memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Png);
                memory.Position = 0;

                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();

                return bitmapImage;
            }
        }

        /// <summary>
        /// Used to notify about change in value of variable
        /// </summary>
        /// <param name="attr">Name of the variable which has changed</param>
        public void CallChange(string attr)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(attr));
            }
        }
    }
}
