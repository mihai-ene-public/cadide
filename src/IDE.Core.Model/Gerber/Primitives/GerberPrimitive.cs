using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IDE.Core.Gerber;

namespace IDE.Core.Gerber
{
    public class GerberPrimitive
    {

        public Polarity Polarity { get; set; } = Polarity.Dark;


        protected List<ApertureDefinitionBase> cachedApertures = new List<ApertureDefinitionBase>();

        protected List<ApertureDefinitionBase> cachedClearanceApertures = new List<ApertureDefinitionBase>();

        public List<ApertureDefinitionBase> GetApertures()
        {
            if (cachedApertures.Count == 0)
                CreateApertures();

            return cachedApertures;
        }

        protected virtual void CreateApertures()
        {
        }

        public List<ApertureDefinitionBase> GetClearanceAperture()
        {
            if (cachedClearanceApertures.Count == 0)
                CreateClearanceApertures();

            return cachedClearanceApertures;
        }

        protected virtual void CreateClearanceApertures()
        {
        }

        public virtual void WriteGerber(Gerber274XWriter gerberWriter) { }
    }
}
