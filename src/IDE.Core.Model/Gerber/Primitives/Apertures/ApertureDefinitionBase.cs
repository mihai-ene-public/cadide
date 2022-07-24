using IDE.Core.Gerber;

namespace IDE.Core.Model.Gerber.Primitives.Apertures
{
    public class ApertureDefinitionBase
    {
        public int Number { get; set; }

        public ApertureTypes ApertureType { get; protected set; }
    }



}
