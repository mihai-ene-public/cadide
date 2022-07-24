using System.Globalization;
using System.IO;

namespace IDE.Core.Model.Gerber.Primitives.Apertures
{
    public abstract class ApertureMacroPrimitive
    {
        public int Code { get; protected set; }

        public abstract void WriteTo(TextWriter writer);

        protected string WriteDouble(double number)
        {
            number = Math.Round(number, 6);
            return number.ToString(CultureInfo.InvariantCulture);
        }
    }



}
