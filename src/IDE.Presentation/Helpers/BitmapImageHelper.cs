using IDE.Core.Interfaces;
using System.IO;
using System.Windows.Media.Imaging;

namespace IDE.Core
{
    public class BitmapImageHelper : IBitmapImageHelper
    {
        public ImageData GetImageData(byte[] imageBytes)
        {
            var image = new BitmapImage();

            using (var byteStream = new MemoryStream(imageBytes))
            {
                image.BeginInit();
                image.UriSource = null;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.StreamSource = byteStream;
                image.EndInit();

                // return image;
            }

            int stride = image.PixelWidth * 4;
            int size = image.PixelHeight * stride;
            byte[] pixels = new byte[size];
            image.CopyPixels(pixels, stride, 0);

            var bits = new byte[image.PixelWidth * image.PixelHeight * 3];
            var maskBits = new byte[image.PixelWidth * image.PixelHeight];
            int i = 0;
            int j = 0;

            // Start at the top row
            for (int y = 0; y < image.PixelHeight; y++)
            {
                for (int x = 0; x < image.PixelWidth; x++)
                {
                    int index = y * stride + 4 * x;
                    byte blue = pixels[index];
                    byte green = pixels[index + 1];
                    byte red = pixels[index + 2];
                    byte alpha = pixels[index + 3];


                    maskBits[j++] = alpha;
                    bits[i++] = red;
                    bits[i++] = green;
                    bits[i++] = blue;
                }
            }

            return new ImageData
            {
                Bits = bits,
                MaskBits = maskBits,
                PixelWidth = image.PixelWidth,
                PixelHeight = image.PixelHeight
            };
        }
    }
}
