using IDE.Core.Interfaces;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.IO;

namespace IDE.Core
{
    public class BitmapImageHelper : IBitmapImageHelper
    {
        public ImageData GetImageData(byte[] imageBytes)
        {
            //var image = new BitmapImage();

            //using (var byteStream = new MemoryStream(imageBytes))
            //{
            //    image.BeginInit();
            //    image.UriSource = null;
            //    image.CacheOption = BitmapCacheOption.OnLoad;
            //    image.StreamSource = byteStream;
            //    image.EndInit();
            //}
            var image = Image.Load<Rgba32>(imageBytes);

            //int stride = image.Width * 4;
            //int size = image.Height * stride;
            //byte[] pixels = new byte[size];
            //var rgbaPixels = new Rgba32[image.Width * image.Height];
            ////image.CopyPixels(pixels, stride, 0);
            //image.CopyPixelDataTo(rgbaPixels);

            var bits = new byte[image.Width * image.Height * 3];
            var maskBits = new byte[image.Width * image.Height];
            int i = 0;
            int j = 0;

            //// Start at the top row
            //for (int y = 0; y < image.Height; y++)
            //{
            //    for (int x = 0; x < image.Width; x++)
            //    {
            //        int index = y * stride + 4 * x;
            //        byte blue = pixels[index];
            //        byte green = pixels[index + 1];
            //        byte red = pixels[index + 2];
            //        byte alpha = pixels[index + 3];


            //        maskBits[j++] = alpha;
            //        bits[i++] = red;
            //        bits[i++] = green;
            //        bits[i++] = blue;
            //    }
            //}
            image.ProcessPixelRows(accessor =>
            {
                for (int y = 0; y < accessor.Height; y++)
                {
                    var pixelRow = accessor.GetRowSpan(y);

                    for (int x = 0; x < pixelRow.Length; x++)
                    {
                        // Get a reference to the pixel at position x
                        ref Rgba32 pixel = ref pixelRow[x];

                        byte blue = pixel.B;
                        byte green = pixel.G;
                        byte red = pixel.R;
                        byte alpha = pixel.A;

                        maskBits[j++] = alpha;
                        bits[i++] = red;
                        bits[i++] = green;
                        bits[i++] = blue;
                    }
                }
            });

            return new ImageData
            {
                Bits = bits,
                MaskBits = maskBits,
                PixelWidth = image.Width,
                PixelHeight = image.Height
            };
        }
    }
}
