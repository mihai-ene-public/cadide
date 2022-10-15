namespace IDE.Core.Interfaces
{
    public class ImageData
    {
        public int PixelWidth { get; set; }

        public int PixelHeight { get; set; }

        public byte[] Bits { get; set; }

        public byte[] MaskBits { get; set; }
    }
}
