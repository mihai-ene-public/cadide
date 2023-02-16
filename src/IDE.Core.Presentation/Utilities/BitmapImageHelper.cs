using IDE.Core.Interfaces;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace IDE.Core;

public class BitmapImageHelper : IBitmapImageHelper
{
    public ImageData GetImageData(byte[] imageBytes)
    {
        var image = Image.Load<Rgba32>(imageBytes);

        var bits = new byte[image.Width * image.Height * 3];
        var maskBits = new byte[image.Width * image.Height];
        int i = 0;
        int j = 0;

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
