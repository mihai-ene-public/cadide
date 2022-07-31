using IDE.Core.Gerber;
using IDE.Core.Model.Gerber.Primitives.Attributes;

namespace IDE.Core.Model.Gerber.Primitives.Apertures
{
    public class ApertureDefinitionBase
    {
        public int Number { get; set; }

        public ApertureTypes ApertureType { get; protected set; }

        public Dictionary<string, ApertureGerberAttribute> ApertureAttributes { get; set; } = new Dictionary<string, ApertureGerberAttribute>();

        internal AperFunctionType? GetFunctionApertureType()
        {
            if (ApertureAttributes.TryGetValue(nameof(StandardApertureAttributes.AperFunction), out var attribute))
            {
                var functionAttribute = attribute as AperFunctionApertureGerberAttribute;

                return functionAttribute?.AperFunctionType;
            }

            return null;
        }

        protected bool HasSameApertureFunctionAs(ApertureDefinitionBase other)
        {
            var thisAperType = GetFunctionApertureType();
            var otherAperType = other.GetFunctionApertureType();

            //if we don't have an aperture function specified, then it's the same function
            if (thisAperType == null && otherAperType == null)
                return true;

            return thisAperType == otherAperType;
        }
    }



}
