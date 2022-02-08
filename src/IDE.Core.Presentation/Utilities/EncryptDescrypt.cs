using IDE.Core.Presentation.Licensing;
using System;

namespace IDE.Core
{
    public static class EncryptDescrypt
    {
        public static string EncodeToBase64(string toEncode)
        {
            var encoder = new RadixBase36Encoding(false);
            string encoded = null;
            if (toEncode != null)
            {
                encoded = encoder.Encode(toEncode);
                encoded = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(encoded));
            }
            return encoded;
        }

        public static string DecodeFromBase64(string toDecode)
        {
            var encoder = new RadixBase36Encoding(false);
            string decoded = null;
            if (toDecode != null)
            {
                decoded = System.Text.Encoding.ASCII.GetString(Convert.FromBase64String(toDecode));
                decoded = System.Text.Encoding.ASCII.GetString(encoder.Decode(decoded));
            }

            return decoded;
        }
    }
}
