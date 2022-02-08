using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDE.Core.Excelon
{
    public class NCTool
    {
        public int Number { get; set; }

        public double Diameter { get; set; }

        public DrillPlating Plating { get; set; } = DrillPlating.Plated;

        public override bool Equals(object obj)
        {
            var ad = obj as NCTool;
            if (ad != null)
            {
                return Diameter == ad.Diameter && Plating == ad.Plating;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public enum DrillPlating
    {
        Plated,
        NonPlated
    }
}
