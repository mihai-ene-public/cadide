using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace IDE.Core.Presentation.Licensing
{
    class BASE36
    {
        private const string alphabet = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private static readonly char[] _charArray = alphabet.ToCharArray();
        const int kByteBitCount = 8; // number of bits in a byte
        static readonly double kBase36CharsLengthDivisor = Math.Log(alphabet.Length, 2);
        static readonly BigInteger kBigInt36 = new BigInteger(36);

        public static string Encode(ulong input)
        {
            var sb = new StringBuilder();
            do
            {
                sb.Append(_charArray[input % (ulong)alphabet.Length]);
                input /= (ulong)alphabet.Length;
            } while (input != 0);

            return Reverse(sb.ToString());
        }

        public static string Encode(byte[] bytes, bool bigEndian = false)
        {
            var resultLength = (int)Math.Ceiling(bytes.Length * kByteBitCount / kBase36CharsLengthDivisor);
            var result = new List<char>(resultLength);

            var dividend = new BigInteger(bytes);
            // IsZero's computation is less complex than evaluating "dividend > 0"
            // which invokes BigInteger.CompareTo(BigInteger)
            while (!dividend.IsZero)
            {
                BigInteger remainder;
                dividend = BigInteger.DivRem(dividend, kBigInt36, out remainder);
                int digitIndex = Math.Abs((int)remainder);
                result.Add(alphabet[digitIndex]);
            }

            // orientate the characters in big-endian ordering
            if (!bigEndian)
                result.Reverse();
            // ToArray will also trim the excess chars used in length prediction
            return new string(result.ToArray());
        }

        public static string Encode(string input, bool bigEndian = false)
        {
            var bytes = Encoding.UTF8.GetBytes(input);
            return Encode(bytes, bigEndian);
        }

        public static long Decode(string input)
        {
            long _result = 0;
            double _pow = 0;
            for (int _i = input.Length - 1; _i >= 0; _i--)
            {
                char _c = input[_i];
                int pos = alphabet.IndexOf(_c);
                if (pos > -1)
                    _result += pos * (long)Math.Pow(alphabet.Length, _pow);
                else
                    return -1;
                _pow++;
            }
            return _result;
        }

        private static string Reverse(string s)
        {
            var charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }
    }
}
