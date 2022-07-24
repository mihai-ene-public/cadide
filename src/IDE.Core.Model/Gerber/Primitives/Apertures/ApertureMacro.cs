using System.IO;

namespace IDE.Core.Model.Gerber.Primitives.Apertures
{
    public class ApertureMacro
    {
        public string Name { get; set; }//Box will write %AMBox*

        public List<ApertureMacroPrimitive> Primitives { get; set; } = new List<ApertureMacroPrimitive>();

        public void WriteTo(TextWriter writer)
        {
            //start
            writer.WriteLine($"%AM{Name}*");

            //primitives
            foreach (var p in Primitives)
                p.WriteTo(writer);

            //AD
            writer.WriteLine("%");
            //writer.WriteLine($"%ADD{id.ToString(dFormat)}RORD{id.ToString(dFormat)},{rot}*%");
        }
    }



}
