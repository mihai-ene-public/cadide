using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDE.Core.Gerber
{
    public class ApertureDefinitionBase
    {
        public int Number { get; set; }

        public ApertureTypes ApertureType { get; protected set; }
    }

    public class ApertureDefinitionCircle : ApertureDefinitionBase
    {
        public ApertureDefinitionCircle()
        {
            ApertureType = ApertureTypes.Circle;
        }

        public double Diameter { get; set; }

        public override bool Equals(object obj)
        {
            var ad = obj as ApertureDefinitionCircle;
            if (ad != null)
            {
                return //ApertureType == ad.ApertureType && 
                        Diameter == ad.Diameter;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class ApertureDefinitionRectangle : ApertureDefinitionBase
    {
        public ApertureDefinitionRectangle()
        {
            ApertureType = ApertureTypes.Rectangle;
        }

        public double Width { get; set; }
        public double Height { get; set; }


        public override bool Equals(object obj)
        {
            var ad = obj as ApertureDefinitionRectangle;
            if (ad != null)
            {
                return //ApertureType == ad.ApertureType &&
                        Width == ad.Width && Height == ad.Height;

            }

            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class ApertureDefinitionRotatedRoundedRectangle : ApertureDefinitionBase
    {
        public ApertureDefinitionRotatedRoundedRectangle()
        {
            ApertureType = ApertureTypes.RotatedRoundedRectangle;
        }

        public double Width { get; set; }
        public double Height { get; set; }

        public double Rot { get; set; }
        public double CornerRadius { get; set; }


        public override bool Equals(object obj)
        {
            var ad = obj as ApertureDefinitionRotatedRoundedRectangle;
            if (ad != null)
            {
                return Width == ad.Width && Height == ad.Height && ad.Rot == Rot && CornerRadius == ad.CornerRadius;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

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

    public abstract class ApertureMacroPrimitive
    {
        public int Code { get; protected set; }

        public abstract void WriteTo(TextWriter writer);

        protected string WriteDouble(double number)
        {
            return number.ToString(CultureInfo.InvariantCulture);
        }
    }

    public class AMCommentPrimitive : ApertureMacroPrimitive
    {
        public AMCommentPrimitive()
        {
            Code = 0;
        }

        public string Comment { get; set; }

        public override void WriteTo(TextWriter writer)
        {
            writer.WriteLine($"{Code} {Comment}*");
        }
    }

    public class AMCirclePrimitive : ApertureMacroPrimitive
    {
        public AMCirclePrimitive()
        {
            Code = 1;
            Exposure = AMPrimitiveExposure.On;
        }

        public AMPrimitiveExposure Exposure { get; set; }

        public double Diameter { get; set; }

        public double CenterX { get; set; }

        public double CenterY { get; set; }

        ///// <summary>
        ///// Rotation angle of the center, in degrees counterclockwise.
        ///// </summary>
        //public double Rotation { get; set; }

        //rotation will be taken from the definition of the aperture (ADD)

        public override void WriteTo(TextWriter writer)
        {
            writer.WriteLine($"{Code},{(int)Exposure},{WriteDouble(Diameter)},{WriteDouble(CenterX)},{WriteDouble(CenterY)},$1*");
        }
    }

    public class AMVectorLinePrimitive : ApertureMacroPrimitive
    {
        public AMVectorLinePrimitive()
        {
            Code = 20;
        }

        public AMPrimitiveExposure Exposure { get; set; }

        public double Width { get; set; }

        public double StartPointX { get; set; }

        public double StartPointY { get; set; }

        public double EndPointX { get; set; }
        public double EndPointY { get; set; }

        public override void WriteTo(TextWriter writer)
        {
            writer.WriteLine($"{Code},{(int)Exposure},{WriteDouble(Width)},{WriteDouble(StartPointX)},{WriteDouble(StartPointY)},{WriteDouble(EndPointX)},{WriteDouble(EndPointY)},$1*");
        }
    }

    public class AMCenterLinePrimitive : ApertureMacroPrimitive
    {
        public AMCenterLinePrimitive()
        {
            Code = 21;
        }

        public AMPrimitiveExposure Exposure { get; set; }

        public double Width { get; set; }
        public double Height { get; set; }

        public double CenterPointX { get; set; }

        public double CenterPointY { get; set; }

        public override void WriteTo(TextWriter writer)
        {
            if (Width == 0.00d || Height == 0.00d)
                return;
            writer.WriteLine($"{Code},{(int)Exposure},{WriteDouble( Width)},{WriteDouble(Height)},{WriteDouble(CenterPointX)},{WriteDouble(CenterPointY)},$1*");
        }
    }

    public class AMOutlinePrimitive : ApertureMacroPrimitive
    {
        public AMOutlinePrimitive()
        {
            Code = 4;

            throw new NotImplementedException();
        }

        public override void WriteTo(TextWriter writer)
        {
            //writer.WriteLine($"0 {Comment}*");
        }
    }

    /// <summary>
    /// regular polygon defined by the number of vertices n, the center point and the diameter of the circumscribed circle
    /// </summary>
    public class AMPolygonPrimitive : ApertureMacroPrimitive
    {
        public AMPolygonPrimitive()
        {
            Code = 5;
        }

        public AMPrimitiveExposure Exposure { get; set; }

        public int NumberVertices { get; set; }
        public double CircleDiameter { get; set; }

        public double CenterPointX { get; set; }

        public double CenterPointY { get; set; }

        public override void WriteTo(TextWriter writer)
        {
            //writer.WriteLine($"0 {Comment}*");
            writer.WriteLine($"{Code},{(int)Exposure},{NumberVertices},{WriteDouble(CenterPointX)},{WriteDouble(CenterPointY)},{WriteDouble(CircleDiameter)},$1*");
        }
    }

    /// <summary>
    /// a cross hair centered on concentric rings. Exposure is always on.
    /// </summary>
    public class AMMoirePrimitive : ApertureMacroPrimitive
    {
        public AMMoirePrimitive()
        {
            Code = 6;
        }

        public double CenterPointX { get; set; }

        public double CenterPointY { get; set; }
        public double OuterDiameter { get; set; }
        public double RingThickness { get; set; }

        /// <summary>
        /// gap between rings
        /// </summary>
        public double RingsGap { get; set; }

        public int MaxNumberRings { get; set; }
        public double CrossHairThickness { get; set; }
        public double CrossHairLength { get; set; }

        public override void WriteTo(TextWriter writer)
        {
            // writer.WriteLine($"0 {Comment}*");
            writer.WriteLine($"{Code},{WriteDouble(CenterPointX)},{WriteDouble(CenterPointY)},{WriteDouble(OuterDiameter)},{WriteDouble(RingThickness)},{WriteDouble(RingsGap)},{MaxNumberRings},{WriteDouble(CrossHairThickness)},{WriteDouble(CrossHairLength)},$1*");
        }
    }

    /// <summary>
    /// The thermal primitive is a ring (annulus) interrupted by four gaps. Exposure is always on.
    /// </summary>
    public class AMThermalPrimitive : ApertureMacroPrimitive
    {
        public AMThermalPrimitive()
        {
            Code = 7;
        }

        public double CenterPointX { get; set; }

        public double CenterPointY { get; set; }
        public double OuterDiameter { get; set; }
        public double InnerDiameter { get; set; }
        public double GapThickness { get; set; }

        public override void WriteTo(TextWriter writer)
        {
            writer.WriteLine($"{Code},{WriteDouble(CenterPointX)},{WriteDouble(CenterPointY)},{WriteDouble(OuterDiameter)},{WriteDouble(InnerDiameter)},{WriteDouble(GapThickness)},$1*");
        }
    }

    public enum AMPrimitiveExposure
    {
        Off = 0,
        On = 1
    }



}
