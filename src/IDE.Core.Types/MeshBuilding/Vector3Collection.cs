using IDE.Core.Types.Media3D;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace IDE.Core.Types.MeshBuilding
{
    public sealed class Vector3Collection : FastList<XVector3D>
    {
        public Vector3Collection()
        {
        }

        public Vector3Collection(int capacity)
            : base(capacity)
        {
        }

        public Vector3Collection(IEnumerable<XVector3D> items)
            : base(items)
        {
        }

        public static Vector3Collection Parse(string source)
        {
            IFormatProvider formatProvider = CultureInfo.InvariantCulture;

            var th = new TokenizerHelper(source, formatProvider);
            var resource = new Vector3Collection();

            XVector3D value;

            while (th.NextToken())
            {
                value = new XVector3D(
                    Convert.ToDouble(th.GetCurrentToken(), formatProvider),
                    Convert.ToDouble(th.NextTokenRequired(), formatProvider),
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
                str.AppendFormat(provider, "{0},{1},{2}", this[i].X, this[i].Y, this[i].Z);
                if (i != this.Count - 1)
                {
                    str.Append(" ");
                }
            }

            return str.ToString();
        }
    }

}
