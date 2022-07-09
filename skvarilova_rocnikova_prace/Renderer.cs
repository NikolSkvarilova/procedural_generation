using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace skvarilova_rocnikova_prace
{
    class Renderer
    {
        public int Height { get; set; }
        public int Width { get; set; }
        public WriteableBitmap wbmap;
        byte[] pixels;

        public Renderer(int width, int height)
        {
            // https://www.i-programmer.info/programming/wpf-workings/527-writeablebitmap.html
            Width = width;
            Height = height;
            wbmap = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgra32, null); // 96 = dpi
            pixels = new byte[
                wbmap.PixelHeight * wbmap.PixelWidth * wbmap.Format.BitsPerPixel / 8
            ];
            // Setting the first pixel to blue
            pixels[0] = 0xff; // Blue
            pixels[1] = 0x00; // Green
            pixels[2] = 0x00; // Red
            pixels[3] = 0xff; // Alpha

            // Write the pixels
            wbmap.WritePixels(
                new Int32Rect(0, 0,
                wbmap.PixelWidth, wbmap.PixelHeight),
                pixels,
                wbmap.PixelWidth * wbmap.
                    Format.BitsPerPixel / 8, 0);
        }
    }
}
