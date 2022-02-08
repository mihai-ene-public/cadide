using IDE.Core.Types.Media;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace IDE.Core.Types.MeshBuilding
{
    public sealed class Vector2Collection : FastList<XPoint>
    {
        public Vector2Collection()
        {
        }

        public Vector2Collection(int capacity)
            : base(capacity)
        {
        }

        public Vector2Collection(IEnumerable<XPoint> items)
            : base(items)
        {
        }

        public static Vector2Collection Parse(string source)
        {
            IFormatProvider formatProvider = CultureInfo.InvariantCulture;

            var th = new TokenizerHelper(source, formatProvider);
            var resource = new Vector2Collection();

            XPoint value;

            while (th.NextToken())
            {
                value = new XPoint(
                    Convert.ToDouble(th.GetCurrentToken(), formatProvider),
                    Convert.ToDouble(th.NextTokenRequired(), formatProvider));

                resource.Add(value);
            }

            return resource;
        }

        public string ConvertToString(string format, IFormatProvider provider)
        {
            if (this.Count == 0)
            {
                return String.Empty;
            }

            var str = new StringBuilder();
            for (int i = 0; i < this.Count; i++)
            {
                //str.AppendFormat(provider, "{0:" + format + "}", this[i]);
                str.AppendFormat(provider, "{0},{1}", this[i].X, this[i].Y);
                if (i != this.Count - 1)
                {
                    str.Append(" ");
                }
            }

            return str.ToString();
        }
    }

}
