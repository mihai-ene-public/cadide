using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDE.Core.Gerber
{
    public class ApertureTypes
    {
        public const string Circle = "C";
        public const string Rectangle = "R";
        public const string Obround = "O";//oval
        public const string Polygon = "P";
        /// <summary>
        /// used for macro
        /// </summary>
        public const string RotatedRoundedRectangle = "ROR";

        public string Value { get; private set; }

        //public static explicit operator ApertureTypes(string apertureTypesString)
        //{
        //    return new ApertureTypes { Value = apertureTypesString };
        //}

        public static implicit operator ApertureTypes(string apertureTypesString)
        {
            return new ApertureTypes { Value = apertureTypesString };
        }

        public override string ToString()
        {
            return Value;
        }

        public override bool Equals(object obj)
        {
            var at = obj as ApertureTypes;
            if(at!=null)
            {
                return at.Value == Value;
            }

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
