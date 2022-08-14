using IDE.Core.Gerber;
using IDE.Core.Model.Gerber.Primitives.Attributes;

namespace IDE.Core.Model.Gerber.Primitives.Apertures
{
    public class ApertureDefinitionBase
    {
        public int Number { get; set; }

        public ApertureTypes ApertureType { get; protected set; }

        public Dictionary<string, ApertureGerberAttribute> ApertureAttributes { get; set; } = new Dictionary<string, ApertureGerberAttribute>();

        internal AperFunctionApertureGerberAttribute GetFunctionAperture()
        {
            if (ApertureAttributes.TryGetValue(nameof(StandardApertureAttributes.AperFunction), out var attribute))
            {
                var functionAttribute = attribute as AperFunctionApertureGerberAttribute;

                return functionAttribute;
            }

            return null;
        }

        protected bool HasSameApertureFunctionAs(ApertureDefinitionBase other)
        {
            var thisAperType = GetFunctionAperture();
            var otherAperType = other.GetFunctionAperture();

            //if we don't have an aperture function specified, then it's the same function
            if (thisAperType == null && otherAperType == null)
                return true;

            //if one them is null but the other is not, then they're different
            if (thisAperType == null || otherAperType == null)
                return false;

            return thisAperType.ToString() == otherAperType.ToString();
        }
    }



}
