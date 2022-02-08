using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDE.Core.Gerber
{
    public class GraphicsState
    {
        //coorrdinate format (FS)
        public FormatStatement Format { get; set; }
        //unit (MO)
        public Modes Unit { get; set; }
        //Current Point
        public double CurrentX { get; set; }
        public double CurrentY { get; set; }

        //Previous Point
        public double PreviousX { get; set; }
        public double PreviousY { get; set; }

        //Offset
        public double OffsetX { get; set; }
        public double OffsetY { get; set; }
        //Current aperture: D01 and D03 use this aperture
        public int CurrentAperture { get; set; }

        public ApertureState ApertureState { get; set; }

        //Interpolation mode (Linear/Circular - clockwise/counterclockwise)
        public InterpolationMode PreviousInterpolation { get; set; }

        public InterpolationMode Interpolation { get; set; }

        public RegionMode RegionMode { get; set; }

        //quadrant mode (Single, Multi)
        public QuadrantMode QuadrantMode { get; set; }
        //object polarity (LP)
        public Polarity LevelPolarity { get; set; }
        //obj mirroring (LM)
        //obj rotation (LR)
        //obj scaling (LS)
        //region mode(?)
    }

    public class FormatStatement
    {
        public OmitZeros OmmitZeroes { get; set; }

        /// <summary>
        /// number of digits for int side of X coordinate
        /// </summary>
        public int XInt { get; set; }

        /// <summary>
        /// number of digits for decimal side of X coordinate
        /// </summary>

        public int XDec { get; set; }

        /// <summary>
        /// number of digits for int side of Y coordinate
        /// </summary>

        public int YInt { get; set; }

        /// <summary>
        /// number of digits for dec side of Y coordinate
        /// </summary>

        public int YDec { get; set; }

        public GerberCoordinate Coordinate { get; set; }
    }

    public enum InterpolationMode
    {
        Linear,

        CircularClockwise,
        CircularCounterClockwise,

        RegionModeStart,
        RegionModeEnd
    }

    public enum RegionMode
    {
        None,
        RegionStart,
        //RegionEnd
    }

    public enum ApertureState
    {
        /// <summary>
        /// Stroke mode operation
        /// </summary>
        On,

        /// <summary>
        /// MoveTo operation.
        /// </summary>
        Off,

        /// <summary>
        /// Flash mode operation. Current aperture will be copied at the specified point
        /// </summary>
        Flash
    }

    //we could have a base class and have a CircleApertureDefinition (inherited), etc
    public class ApertureDefinition
    {
        public ApertureTypes ApertureType { get; set; }
        public double[] Parameters { get; set; }
    }
}
