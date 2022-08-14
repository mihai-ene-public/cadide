using IDE.Core.Model.Gerber.Primitives.Apertures;
using IDE.Core.Model.Gerber.Primitives.Attributes;
using IDE.Core.Types.Media;

namespace IDE.Core.Gerber
{
    public class GerberPickAndPlacePrimitive : GerberPrimitive
    {
        public string PartName { get; set; }

        public double Rot { get; set; }

        public XPoint CentroidPos { get; set; }
        public XPoint? Pin1Pos { get; set; }

        public string Manufacturer { get; set; }

        /// <summary>
        /// Manufacturer part number
        /// </summary>
        public string Mpn { get; set; }

        public string Value { get; set; }

        public PickAndPlaceMountType? MountType { get; set; }

        public string FootprintName { get; set; }

        public string SupplierName { get; set; }

        public string SupplierPartNumber { get; set; }

        public List<GerberPrimitive> CourtyardItems { get; set; } = new List<GerberPrimitive>();
        public List<GerberPrimitive> FootprintItems { get; set; } = new List<GerberPrimitive>();

        ApertureDefinitionBase centroidAperture;
        ApertureDefinitionBase pin1Aperture;
        ApertureDefinitionBase footprintOutlineAperture;
        ApertureDefinitionBase courtyardOutlineAperture;

        protected override void CreateApertures()
        {
            centroidAperture = new ApertureDefinitionCircle { Diameter = 0.300 };
            centroidAperture.ApertureAttributes[nameof(StandardApertureAttributes.AperFunction)] = new AperFunctionApertureGerberAttribute()
            {
                AperFunctionType = AperFunctionType.ComponentMain
            };
            cachedApertures.Add(centroidAperture);

            pin1Aperture = new ApertureDefinitionPolygon { OuterDiameter = 0.3600, NumberVertices = 4 };
            pin1Aperture.ApertureAttributes[nameof(StandardApertureAttributes.AperFunction)] = new AperFunctionApertureGerberAttribute()
            {
                AperFunctionType = AperFunctionType.ComponentPin
            };
            cachedApertures.Add(pin1Aperture);

            footprintOutlineAperture = new ApertureDefinitionCircle { Diameter = 0.100 };
            footprintOutlineAperture.ApertureAttributes[nameof(StandardApertureAttributes.AperFunction)] = new AperFunctionApertureGerberAttribute()
            {
                AperFunctionType = AperFunctionType.ComponentOutline,
                Value = ComponentOutlineValue.Footprint
            };
            cachedApertures.Add(footprintOutlineAperture);

            courtyardOutlineAperture = new ApertureDefinitionCircle { Diameter = 0.100 };
            courtyardOutlineAperture.ApertureAttributes[nameof(StandardApertureAttributes.AperFunction)] = new AperFunctionApertureGerberAttribute()
            {
                AperFunctionType = AperFunctionType.ComponentOutline,
                Value = ComponentOutlineValue.Courtyard
            };
            cachedApertures.Add(courtyardOutlineAperture);
        }

        public override void WriteGerber(Gerber274XWriter gerberWriter)
        {
            gerberWriter.DeleteAllAttributes();

            if (!string.IsNullOrEmpty(PartName))
            {
                gerberWriter.WriteObjectAttribute($".C,{PartName}");
                gerberWriter.WriteObjectAttribute($".CRot,{Rot:0.00}");

                if (!string.IsNullOrEmpty(Manufacturer))
                    gerberWriter.WriteObjectAttribute($@".CMfr,""{Manufacturer}""");

                if (!string.IsNullOrEmpty(Mpn))
                    gerberWriter.WriteObjectAttribute($@".CMPN,""{Mpn}""");

                if (!string.IsNullOrEmpty(Value))
                    gerberWriter.WriteObjectAttribute($@".CVal,""{Value}""");

                if (MountType != null)
                    gerberWriter.WriteObjectAttribute($@".CMnt,""{MountType}""");

                if (!string.IsNullOrEmpty(FootprintName))
                    gerberWriter.WriteObjectAttribute($@".CFtp,""{FootprintName}""");

                if (!string.IsNullOrEmpty(SupplierName) && !string.IsNullOrEmpty(SupplierPartNumber))
                    gerberWriter.WriteObjectAttribute($@".CSup,""{SupplierName}"",""{SupplierPartNumber}""");
            }

            gerberWriter.SetLevelPolarity(Polarity.Dark);

            //write centroid
            gerberWriter.SelectAperture(centroidAperture.Number);
            gerberWriter.FlashApertureTo(CentroidPos.X, CentroidPos.Y);

            //write pin 1
            if (Pin1Pos != null)
            {
                var pin1Pos = Pin1Pos.Value;
                gerberWriter.SelectAperture(pin1Aperture.Number);
                gerberWriter.FlashApertureTo(pin1Pos.X, pin1Pos.Y);
            }

            //footprint outlines
            if (FootprintItems.Count > 0)
            {
                gerberWriter.SelectAperture(footprintOutlineAperture.Number);
                foreach (var item in FootprintItems)
                    item.WriteGerberShape(gerberWriter);
            }

            //courtyard outlines
            if (CourtyardItems.Count > 0)
            {
                gerberWriter.SelectAperture(courtyardOutlineAperture.Number);
                foreach (var item in CourtyardItems)
                    item.WriteGerberShape(gerberWriter);
            }

            //gerberWriter.DeleteAllAttributes();
        }
    }

    public enum PickAndPlaceMountType
    {
        TH,
        SMD,
        Fiducial,
        Other
    }
}
