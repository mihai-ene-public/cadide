namespace IDE.Core.Presentation.Licensing
{
    public class RadixBase36Encoding : RadixEncoding
    {
        const string k_base36_digits = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        public RadixBase36Encoding()
            : base(k_base36_digits, EndianFormat.Little, true)
        {

        }

        public RadixBase36Encoding(bool includeZeros)
            : base(k_base36_digits, EndianFormat.Little, includeZeros)
        {

        }
    }
}
