using System;
using System.Text;

namespace IDE.Core.Types.Media
{
    public struct XColor : IEquatable<XColor>
    {

        ///<summary>
        /// A
        ///</summary>
        public byte A
        {
            get
            {
                return sRgbColor.a;
            }
            set
            {
                scRgbColor.a = (float)value / 255.0f;
                sRgbColor.a = value;
            }
        }

        /// <value>The Red channel as a byte whose range is [0..255].
        /// the value is not allowed to be out of range</value>
        /// <summary>
        /// R
        /// </summary>
        public byte R
        {
            get
            {
                return sRgbColor.r;
            }
            set
            {
                scRgbColor.r = sRgbToScRgb(value);
                sRgbColor.r = value;
            }
        }

        ///<value>The Green channel as a byte whose range is [0..255].
        /// the value is not allowed to be out of range</value><summary>
        /// G
        ///</summary>
        public byte G
        {
            get
            {
                return sRgbColor.g;
            }
            set
            {
                scRgbColor.g = sRgbToScRgb(value);
                sRgbColor.g = value;
            }
        }

        ///<value>The Blue channel as a byte whose range is [0..255].
        /// the value is not allowed to be out of range</value><summary>
        /// B
        ///</summary>
        public byte B
        {
            get
            {
                return sRgbColor.b;
            }
            set
            {
                scRgbColor.b = sRgbToScRgb(value);
                sRgbColor.b = value;
            }
        }

        public string ToHexString()
        {
            var sb = new StringBuilder();

            sb.AppendFormat("#{0:X2}", this.sRgbColor.a);
            sb.AppendFormat("{0:X2}", this.sRgbColor.r);
            sb.AppendFormat("{0:X2}", this.sRgbColor.g);
            sb.AppendFormat("{0:X2}", this.sRgbColor.b);

            return sb.ToString();
        }

        private struct MILColorF // this structure is the "milrendertypes.h" structure and should be identical for performance
        {
            public float a, r, g, b;
        };

        private MILColorF scRgbColor;

        private struct MILColor
        {
            public byte a, r, g, b;
        }

        private MILColor sRgbColor;

       // private bool isFromScRgb;

        ///<summary>
        /// private helper function to set context values from a color value with a set context and ScRgb values
        ///</summary>
        private static float sRgbToScRgb(byte bval)
        {
            float val = ((float)bval / 255.0f);

            if (!(val > 0.0))       // Handles NaN case too. (Though, NaN isn't actually
                                    // possible in this case.)
            {
                return (0.0f);
            }
            else if (val <= 0.04045)
            {
                return (val / 12.92f);
            }
            else if (val < 1.0f)
            {
                return (float)Math.Pow(((double)val + 0.055) / 1.055, 2.4);
            }
            else
            {
                return (1.0f);
            }
        }

        ///<summary>
        /// private helper function to set context values from a color value with a set context and ScRgb values
        ///</summary>
        ///
        private static byte ScRgbTosRgb(float val)
        {
            if (!(val > 0.0))       // Handles NaN case too
            {
                return (0);
            }
            else if (val <= 0.0031308)
            {
                return ((byte)((255.0f * val * 12.92f) + 0.5f));
            }
            else if (val < 1.0)
            {
                return ((byte)((255.0f * ((1.055f * (float)Math.Pow((double)val, (1.0 / 2.4))) - 0.055f)) + 0.5f));
            }
            else
            {
                return (255);
            }
        }

        public static XColor FromHexString(string trimmedColor)
        {
            int a, r, g, b;
            a = 255;

            if (trimmedColor.Length > 7)
            {
                a = ParseHexChar(trimmedColor[1]) * 16 + ParseHexChar(trimmedColor[2]);
                r = ParseHexChar(trimmedColor[3]) * 16 + ParseHexChar(trimmedColor[4]);
                g = ParseHexChar(trimmedColor[5]) * 16 + ParseHexChar(trimmedColor[6]);
                b = ParseHexChar(trimmedColor[7]) * 16 + ParseHexChar(trimmedColor[8]);
            }
            else if (trimmedColor.Length > 5)
            {
                r = ParseHexChar(trimmedColor[1]) * 16 + ParseHexChar(trimmedColor[2]);
                g = ParseHexChar(trimmedColor[3]) * 16 + ParseHexChar(trimmedColor[4]);
                b = ParseHexChar(trimmedColor[5]) * 16 + ParseHexChar(trimmedColor[6]);
            }
            else if (trimmedColor.Length > 4)
            {
                a = ParseHexChar(trimmedColor[1]);
                a = a + a * 16;
                r = ParseHexChar(trimmedColor[2]);
                r = r + r * 16;
                g = ParseHexChar(trimmedColor[3]);
                g = g + g * 16;
                b = ParseHexChar(trimmedColor[4]);
                b = b + b * 16;
            }
            else
            {
                r = ParseHexChar(trimmedColor[1]);
                r = r + r * 16;
                g = ParseHexChar(trimmedColor[2]);
                g = g + g * 16;
                b = ParseHexChar(trimmedColor[3]);
                b = b + b * 16;
            }

            return FromArgb((byte)a, (byte)r, (byte)g, (byte)b);
        }

        private const int s_zeroChar = (int)'0';
        private const int s_aLower = (int)'a';
        private const int s_aUpper = (int)'A';

        static private int ParseHexChar(char c)
        {
            int intChar = (int)c;

            if ((intChar >= s_zeroChar) && (intChar <= (s_zeroChar + 9)))
            {
                return (intChar - s_zeroChar);
            }

            if ((intChar >= s_aLower) && (intChar <= (s_aLower + 5)))
            {
                return (intChar - s_aLower + 10);
            }

            if ((intChar >= s_aUpper) && (intChar <= (s_aUpper + 5)))
            {
                return (intChar - s_aUpper + 10);
            }
            throw new FormatException("Illegal token");
        }

        ///<summary>
        /// Color - sRgb legacy interface, assumes Rgb values are sRgb, alpha channel is linear 1.0 gamma
        ///</summary>
        public static XColor FromArgb(byte a, byte r, byte g, byte b)// legacy sRGB interface, bytes are required to properly round trip
        {
            XColor c1 = new XColor();

            c1.scRgbColor.a = (float)a / 255.0f;
            c1.scRgbColor.r = sRgbToScRgb(r);  // note that context is undefined and thus unloaded
            c1.scRgbColor.g = sRgbToScRgb(g);
            c1.scRgbColor.b = sRgbToScRgb(b);
            c1.sRgbColor.a = a;
            c1.sRgbColor.r = ScRgbTosRgb(c1.scRgbColor.r);
            c1.sRgbColor.g = ScRgbTosRgb(c1.scRgbColor.g);
            c1.sRgbColor.b = ScRgbTosRgb(c1.scRgbColor.b);

            //c1.isFromScRgb = false;

            return c1;
        }

        ///<summary>
        /// Color - sRgb legacy interface, assumes Rgb values are sRgb
        ///</summary>
        internal static XColor FromUInt32(uint argb)// internal legacy sRGB interface
        {
            var c1 = new XColor();

            c1.sRgbColor.a = (byte)((argb & 0xff000000) >> 24);
            c1.sRgbColor.r = (byte)((argb & 0x00ff0000) >> 16);
            c1.sRgbColor.g = (byte)((argb & 0x0000ff00) >> 8);
            c1.sRgbColor.b = (byte)(argb & 0x000000ff);
            c1.scRgbColor.a = (float)c1.sRgbColor.a / 255.0f;
            c1.scRgbColor.r = sRgbToScRgb(c1.sRgbColor.r);  // note that context is undefined and thus unloaded
            c1.scRgbColor.g = sRgbToScRgb(c1.sRgbColor.g);
            c1.scRgbColor.b = sRgbToScRgb(c1.sRgbColor.b);


            return c1;
        }

        public static XColor FromScRgb(float a, float r, float g, float b)
        {
            var c1 = new XColor();

            c1.scRgbColor.r = r;
            c1.scRgbColor.g = g;
            c1.scRgbColor.b = b;
            c1.scRgbColor.a = a;
            if (a < 0.0f)
            {
                a = 0.0f;
            }
            else if (a > 1.0f)
            {
                a = 1.0f;
            }

            c1.sRgbColor.a = (byte)((a * 255.0f) + 0.5f);
            c1.sRgbColor.r = ScRgbTosRgb(c1.scRgbColor.r);
            c1.sRgbColor.g = ScRgbTosRgb(c1.scRgbColor.g);
            c1.sRgbColor.b = ScRgbTosRgb(c1.scRgbColor.b);

            //c1.isFromScRgb = true;

            return c1;
        }

        ///<summary>
        /// Color - sRgb legacy interface, assumes Rgb values are sRgb
        ///</summary>
        public static XColor FromRgb(byte r, byte g, byte b)// legacy sRGB interface, bytes are required to properly round trip
        {
            XColor c1 = XColor.FromArgb(0xff, r, g, b);
            return c1;
        }

        public override int GetHashCode()
        {
            return this.scRgbColor.GetHashCode(); //^this.context.GetHashCode();
        }

        #region Public Operators
        ///<summary>
        /// Addition operator - Adds each channel of the second color to each channel of the
        /// first and returns the result
        ///</summary>
        public static XColor operator +(XColor color1, XColor color2)
        {
            XColor c1 = FromScRgb(
                  color1.scRgbColor.a + color2.scRgbColor.a,
                  color1.scRgbColor.r + color2.scRgbColor.r,
                  color1.scRgbColor.g + color2.scRgbColor.g,
                  color1.scRgbColor.b + color2.scRgbColor.b);
            return c1;
        }

        ///<summary>
        /// Addition method - Adds each channel of the second color to each channel of the
        /// first and returns the result
        ///</summary>
        public static XColor Add(XColor color1, XColor color2)
        {
            return (color1 + color2);
        }

        /// <summary>
        /// Subtract operator - substracts each channel of the second color from each channel of the
        /// first and returns the result
        /// </summary>
        /// <param name='color1'>The minuend</param>
        /// <param name='color2'>The subtrahend</param>
        /// <returns>Returns the unclamped differnce</returns>
        public static XColor operator -(XColor color1, XColor color2)
        {
            XColor c1 = FromScRgb(
                color1.scRgbColor.a - color2.scRgbColor.a,
                color1.scRgbColor.r - color2.scRgbColor.r,
                color1.scRgbColor.g - color2.scRgbColor.g,
                color1.scRgbColor.b - color2.scRgbColor.b
                );
            return c1;
        }

        ///<summary>
        /// Subtract method - subtracts each channel of the second color from each channel of the
        /// first and returns the result
        ///</summary>
        public static XColor Subtract(XColor color1, XColor color2)
        {
            return (color1 - color2);
        }

        /// <summary>
        /// Multiplication operator - Multiplies each channel of the color by a coefficient and returns the result
        /// </summary>
        /// <param name='color'>The color</param>
        /// <param name='coefficient'>The coefficient</param>
        /// <returns>Returns the unclamped product</returns>
        public static XColor operator *(XColor color, float coefficient)
        {
            XColor c1 = FromScRgb(color.scRgbColor.a * coefficient, color.scRgbColor.r * coefficient, color.scRgbColor.g * coefficient, color.scRgbColor.b * coefficient);

            return c1;
        }

        ///<summary>
        /// Multiplication method - Multiplies each channel of the color by a coefficient and returns the result
        ///</summary>
        public static XColor Multiply(XColor color, float coefficient)
        {
            return (color * coefficient);
        }

        ///<summary>
        /// Equality method for two colors - return true of colors are equal, otherwise returns false
        ///</summary>
        public static bool Equals(XColor color1, XColor color2)
        {
            return (color1 == color2);
        }

        /// <summary>
        /// Compares two colors for exact equality.  Note that float values can acquire error
        /// when operated upon, such that an exact comparison between two values which are logically
        /// equal may fail. see cref="AreClose" for a "fuzzy" version of this comparison.
        /// </summary>
        /// <param name='color'>The color to compare to "this"</param>
        /// <returns>Whether or not the two colors are equal</returns>
        public bool Equals(XColor color)
        {
            return this == color;
        }

        /// <summary>
        /// Compares two colors for exact equality.  Note that float values can acquire error
        /// when operated upon, such that an exact comparison between two vEquals(color);alues which are logically
        /// equal may fail. see cref="AreClose" for a "fuzzy" version of this comparison.
        /// </summary>
        /// <param name='o'>The object to compare to "this"</param>
        /// <returns>Whether or not the two colors are equal</returns>
        public override bool Equals(object o)
        {
            if (o is XColor)
            {
                XColor color = (XColor)o;

                return (this == color);
            }
            else
            {
                return false;
            }
        }

        ///<summary>
        /// IsEqual operator - Compares two colors for exact equality.  Note that float values can acquire error
        /// when operated upon, such that an exact comparison between two values which are logically
        /// equal may fail. see cref="AreClose".
        ///</summary>
        public static bool operator ==(XColor color1, XColor color2)
        {
            if (color1.scRgbColor.r != color2.scRgbColor.r)
            {
                return false;
            }

            if (color1.scRgbColor.g != color2.scRgbColor.g)
            {
                return false;
            }

            if (color1.scRgbColor.b != color2.scRgbColor.b)
            {
                return false;
            }

            if (color1.scRgbColor.a != color2.scRgbColor.a)
            {
                return false;
            }

            return true;


        }

        ///<summary>
        /// !=
        ///</summary>
        public static bool operator !=(XColor color1, XColor color2)
        {
            return (!(color1 == color2));
        }
        #endregion Public Operators
    }
}
