using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using System.Globalization;
using IDE.Core.Model.Gerber.Primitives.Apertures;

namespace IDE.Core.Gerber
{
    //TODO AM (Aperture Macro)
    //TODO AB (Aperture Block) X2
    //TODO LM (Level Mirroring); LR (L. Rotation); LS (Scaling)
    //TODO Attributes (X2)

    /// <summary>
    /// Represents a Gerber format file containing commands adhering to the RS-274-X specification.
    /// This class provides numerous helper functions that write command strings to the file.
    /// </summary>
    public class Gerber274XWriter : StreamWriter
    {

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="Gerber274XWriter"/> class.
        /// </summary>
        /// <param name="filePath">The filename of the file.</param>
        public Gerber274XWriter(string filePath) : base(filePath)
        {
        }

        public Gerber274XWriter(Stream stream) : base(stream)
        {
        }
        #endregion

        string dFormat = "00";
        //private string gFormat = "00";
        //private string mFormat = "00";
        string coordinateFormat = "00.000";
        int nextApertureId = 10;
        GraphicsState graphicState = new GraphicsState();

        IFormatProvider formatProvider = CultureInfo.InvariantCulture;

        public override IFormatProvider FormatProvider => formatProvider;

        #region Method - FormatStatement

        /// <summary>
        /// Writes a Format Statement command to the file with the specified options.
        /// This function assumes that leading zeros are omitted and absolute coordinates. X2 - compatible
        /// </summary>
        /// <param name="xDigitsBeforeDecimal">The number of digits before the decimal for x-coordinates.</param>
        /// <param name="xDigitsAfterDecimal">The number of digits after the decimal for x-coordinates.</param>
        /// <param name="yDigitsBeforeDecimal">The number of digits before the decimal for y-coordinates.</param>
        /// <param name="yDigitsAfterDecimal">The number of digits after the decimal for y-coordinates.</param>
        public void FormatStatement(int digitsBeforeDecimal, int digitsAfterDecimal)
        {

            WriteLine("%FSLAX{0}{1}Y{0}{1}*%", CoerceToRange(digitsBeforeDecimal, 1, 6),
                                               CoerceToRange(digitsAfterDecimal, 4, 6));
            coordinateFormat = CreateFormatString(digitsBeforeDecimal, digitsAfterDecimal);
        }
        #endregion

        #region Method - Mode

        /// <summary>
        /// Writes a Mode command to the file that sets the unit mode to either inches or millimeters. X2 - compatible
        /// </summary>
        /// <param name="mode">The unit mode.</param>
        public void SetMode(Modes mode)
        {
            string m = "IN";
            if (mode == Modes.Millimeters)
                m = "MM";

            WriteLine("%MO{0}*%", m);
        }

        #endregion

        #region Method - Comment

        /// <summary>
        /// Writes a comment to the file.
        /// </summary>
        /// <param name="comment">The comment.</param>
        public void Comment(string comment)
        {
            WriteLine("G04 {0}*", comment);
        }

        #endregion

        #region Method - StepRepeat
        /// <summary>
        /// Writes a Step and Repeat command to the file.  This creates a grid that is <c>numX</c> columns by <c>numY</c> rows, spaced
        /// <c>stepX</c> and <c>stepY</c> units apart (measured from origin-to-origin).
        /// </summary>
        /// <param name="numX">The number of columns.</param>
        /// <param name="numY">The number of rows.</param>
        /// <param name="stepX">The column spacing (mm or in, depending on mode).</param>
        /// <param name="stepY">The row spacing (mm or in, depending on mode).</param>
        public void StepRepeatBegin(int numX, int numY, double stepX, double stepY)
        {
            WriteLine("%SRX{0}Y{1}I{2}J{3}*%", numX.ToString(),
                                              numY.ToString(),
                                              stepX.ToString(),
                                              stepY.ToString());

        }

        public void StepRepeatEnd()
        {
            WriteLine("%SR*%");
        }
        #endregion

        #region Method - SelectAperture

        /// <summary>
        /// Writes a command to the file that selects a new aperture given its ID numbers.
        /// The ID number must be between 10 and 999, inclusive.
        /// </summary>
        /// <param name="number">The number.</param>
        public void SelectAperture(int number)
        {
            if (graphicState.CurrentAperture == number)
                return;
            graphicState.CurrentAperture = number;

            //both constructs are valid for 274X
            //WriteLine("G54D{0}*", CoerceToRange(number, 10, 999).ToString(dFormat));
            WriteLine("D{0}*", CoerceToRange(number, 10, 999).ToString(dFormat));
        }

        #endregion





        public void InterpolateTo(double x, double y)
        {
            graphicState.CurrentX = x;
            graphicState.CurrentY = y;

            WriteLine("X{0}Y{1}D01*", GetCoordinateString(x)
                                    , GetCoordinateString(y));
        }

        public void InterpolateArcTo(double x, double y, double offsetX, double offsetY)
        {
            graphicState.CurrentX = x;
            graphicState.CurrentY = y;

            WriteLine("X{0}Y{1}I{2}J{3}D01*", GetCoordinateString(x)
                                            , GetCoordinateString(y)
                                            , GetCoordinateString(offsetX)
                                            , GetCoordinateString(offsetY));
        }

        public void FlashApertureTo(double x, double y)
        {
            graphicState.CurrentX = x;
            graphicState.CurrentY = y;

            WriteLine("X{0}Y{1}D03*", GetCoordinateString(x)
                                    , GetCoordinateString(y));
        }

        #region Method - LinearInterpolation
        /// <summary>
        /// Writes a command to the file that sets the current interpolation mode to linear.
        /// </summary>
        public void SetLinearInterpolation()
        {
            if (graphicState.Interpolation == InterpolationMode.Linear)
                return;
            graphicState.Interpolation = InterpolationMode.Linear;

            WriteLine("G01*");
        }


        #endregion

        #region Circular Interpolation
        //G02 - clockwise circular interpolation 
        //G03 - counterclockwise circ interpolation
        //G74 - disable 360 deg circ interp
        //G75 - enable 360 deg circ interp

        public void SetCircularInterpolation(CircularDirection direction)
        {
            if (direction == CircularDirection.Clockwise)
            {
                if (graphicState.Interpolation == InterpolationMode.CircularClockwise)
                    return;
                else
                    graphicState.Interpolation = InterpolationMode.CircularClockwise;
            }
            else if (direction == CircularDirection.CounterClockwise)
            {
                if (graphicState.Interpolation == InterpolationMode.CircularCounterClockwise)
                    return;
                else
                    graphicState.Interpolation = InterpolationMode.CircularCounterClockwise;
            }

            var cmd = direction == CircularDirection.Clockwise ? "G02*" : "G03*";
            WriteLine(cmd);
        }

        public void SetCircularInterpolationSingleQuadrant()
        {
            WriteLine("G74*");
        }
        public void SetCircularInterpolationMultiQuadrant()
        {
            WriteLine("G75*");
        }

        #endregion


        #region Method - MoveTo

        /// <summary>
        /// Writes a command to the file that moves the current position to the specified (x,y)
        /// coordinate.
        /// </summary>
        /// <param name="x">The x-coordinate.</param>
        /// <param name="y">The y-coordinate.</param>
        public void MoveTo(double x, double y)
        {
            if (graphicState.CurrentX == x && graphicState.CurrentY == y)
                return;

            graphicState.CurrentX = x;
            graphicState.CurrentY = y;

            WriteLine("X{0}Y{1}D02*", GetCoordinateString(x)
                                    , GetCoordinateString(y));
        }

        #endregion


        #region Method - SetRegionModeStart
        /// <summary>
        /// Writes a command to the file that starts the polygon fill process.
        /// </summary>
        public void SetRegionModeStart()
        {
            if (graphicState.RegionMode == RegionMode.RegionStart)
                return;
            graphicState.RegionMode = RegionMode.RegionStart;

            WriteLine("G36*");
        }
        #endregion

        //between these statements will be written statements with G01 (linear) and G02 interpolation
        //all lines drawn with D01 are considered edges of the polygon.  D02 closes and fills the polygon.

        #region Method - SetRegionModeEnd

        /// <summary>
        /// Writes a command to the file that stops the polygon fill process.
        /// </summary>
        public void SetRegionModeEnd()
        {
            if (graphicState.RegionMode == RegionMode.None)
                return;
            graphicState.RegionMode = RegionMode.None;

            WriteLine("G37*");
        }

        #endregion

        public void SetLevelPolarity(Polarity polarity)
        {
            if (graphicState.LevelPolarity == polarity)
                return;
            graphicState.LevelPolarity = polarity;

            var pStr = "D";
            if (polarity == Polarity.Clear)
                pStr = "C";
            WriteLine("%LP{0}*%", pStr);
        }


        #region AD - Apperture Definition
        //any Aperture Definition will be added without holes in the aperture.
        //To define a pad with hole we will flash the pad, add any copper, at the end flash the hole (circle aperture) with clear mode (LPD*)




        public int AddApertureDefinition(ApertureTypes apertureType, params double[] parameters)
        {
            int id = GetNextId();
            WriteLine(GetApertureDefinitionString(id, apertureType, parameters));
            return id;
        }

        /// <summary>
        /// Writes a command to the file that adds new aperture definition to the file.  The new aperture is
        /// a circle with the specified diameter.
        /// </summary>
        /// <param name="diameter">The outer diameter.</param>
        /// <returns>A new ID number.  ID numbers begin at 10 and count upwards as they are assigned.</returns>
        public int AddApertureDefinitionCircle(double diameter)
        {
            return AddApertureDefinition(ApertureTypes.Circle, diameter);
        }

        public int AddApertureDefinitionRectangle(double xSize, double ySize)
        {
            return AddApertureDefinition(ApertureTypes.Rectangle, xSize, ySize);
        }

        public int AddApertureDefinitionObround(double xSize, double ySize)
        {
            return AddApertureDefinition(ApertureTypes.Obround, xSize, ySize);
        }

        //TODO: polygon (P)

        public int AddApertureDefinitionRotatedRoundedRectangle(ApertureDefinitionRotatedRoundedRectangle aperture)//double width, double height, double rot, bool isRounded)
        {
            /*
%AMRORD11*
21,1,0.06,0.03,0,0,$1*
1,1,0.06,0.0,0.015,$1*
1,1,0.06,0.0,-0.015,$1*
%
%ADD11RORD11,45*%
            */
            int id = GetNextId();
            var am = new ApertureMacro
            {
                Name = $"RORD{id.ToString(dFormat)}"
            };

            //primitives are built in a generic way; we need to optimize this

            //add two overlapping rectangles as box body
            //horizontal rectangle
            am.Primitives.Add(new AMCenterLinePrimitive
            {
                Exposure = AMPrimitiveExposure.On,
                Width = aperture.Width,
                Height = aperture.Height - 2 * aperture.CornerRadius,
                CenterPointX = 0,
                CenterPointY = 0
            });

            if (aperture.CornerRadius > 0.00d)
            {
                //todo: optimize for Width == Height

                //vertical rectangle
                am.Primitives.Add(new AMCenterLinePrimitive
                {
                    Exposure = AMPrimitiveExposure.On,
                    Width = aperture.Width - 2 * aperture.CornerRadius,
                    Height = aperture.Height,
                    CenterPointX = 0,
                    CenterPointY = 0
                });

                //add four circles for the rounded corners
                var diameter = 2 * aperture.CornerRadius;
                am.Primitives.Add(new AMCirclePrimitive
                {
                    Exposure = AMPrimitiveExposure.On,
                    Diameter = diameter,
                    CenterX = 0.5 * aperture.Width - aperture.CornerRadius,
                    CenterY = 0.5 * aperture.Height - aperture.CornerRadius
                });
                am.Primitives.Add(new AMCirclePrimitive
                {
                    Exposure = AMPrimitiveExposure.On,
                    Diameter = diameter,
                    CenterX = -0.5 * aperture.Width + aperture.CornerRadius,
                    CenterY = 0.5 * aperture.Height - aperture.CornerRadius
                });
                am.Primitives.Add(new AMCirclePrimitive
                {
                    Exposure = AMPrimitiveExposure.On,
                    Diameter = diameter,
                    CenterX = -0.5 * aperture.Width + aperture.CornerRadius,
                    CenterY = -0.5 * aperture.Height + aperture.CornerRadius
                });
                am.Primitives.Add(new AMCirclePrimitive
                {
                    Exposure = AMPrimitiveExposure.On,
                    Diameter = diameter,
                    CenterX = 0.5 * aperture.Width - aperture.CornerRadius,
                    CenterY = -0.5 * aperture.Height + aperture.CornerRadius
                });
            }
            am.WriteTo(this);

            //there is an issue that for some values when aperture.Rot is 0, then the string gets as -0
            var rot = 0.00d;
            if (aperture.Rot != rot)
            {
                rot = Math.Round(aperture.Rot, 2);
            }
            var definition = $"%ADD{id.ToString(dFormat)}{am.Name},{rot}*%";
            WriteLine(definition);

            return id;
        }

        string GetApertureDefinitionString(int codeNumber, ApertureTypes apertureType, params double[] parameters)
        {
            var pString = string.Join("X", parameters.Select(p => p.ToString(CultureInfo.InvariantCulture)));
            var ret = $"%ADD{codeNumber.ToString(dFormat)}{apertureType},{pString}*%";
            return ret;
        }

        #endregion AD


        public void WriteFileAttribute(string content)
        {
            WriteLine($"%TF{content}*%");
        }

        public void WriteApertureAttribute(string content)
        {
            WriteLine($"%TA{content}*%");
        }

        public void WriteObjectAttribute(string content)
        {
            WriteLine($"%TO{content}*%");
        }

        public void DeleteAttribute(string attributeName)
        {
            WriteLine($"%TD{attributeName}*%");
        }

        public void DeleteAllAttributes()
        {
            WriteLine($"%TD*%");
        }

        #region Method - EndOfProgram
        /// <summary>
        /// Writes a command to the file that indicates the last line data.
        /// Call this method as the last command before calling Close.
        /// </summary>
        public void EndOfProgram()
        {
            WriteLine("M02*");
        }
        #endregion


        // ----------------------------------------
        // Private Helper Methods
        // ----------------------------------------

        #region Method - CoerceToRange
        /// <summary>
        /// Coerces the value into the range between min and max.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="min">The minimum value.</param>
        /// <param name="max">The maximum value.</param>
        /// <returns></returns>
        private int CoerceToRange(int value, int min, int max)
        {
            if (value > max) return max;
            if (value < min) return min;
            return value;
        }
        #endregion


        #region Method - CoerceString
        /// <summary>
        /// Takes the input string and returns another string that is at most length characters long and uppercase.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <param name="length">The maximum length.</param>
        /// <returns></returns>
        private string CoerceString(string input, int length)
        {
            string output = String.Empty;
            if (input != null)
            {
                if (input.Length > length)
                {
                    output = input.Substring(0, 77);
                }
                else
                {
                    output = input;
                }
            }
            return output.ToUpper();
        }
        #endregion


        #region Method - CreateFormatString
        /// <summary>
        /// Creates the format string having the specified number of zeros.
        /// </summary>
        /// <param name="numZeros">The number of zeros.</param>
        /// <returns>A format string having numZeros zeros.</returns>
        private string CreateFormatString(int numZeros)
        {
            return new string('0', numZeros);
        }
        #endregion


        #region Method - CreateFormatString
        /// <summary>
        /// Creates a format string having the specified number of digits before and after the decimal point.
        /// </summary>
        /// <param name="digitsBeforeDecimal">The number of digits before the decimal.</param>
        /// <param name="digitsAfterDecimal">The number of digits after the decimal.</param>
        /// <returns>A string in the format "m.n" where m and n are the number of zeros before and after the decimal, respectively</returns>
        private string CreateFormatString(int digitsBeforeDecimal, int digitsAfterDecimal)
        {
            var sb = new StringBuilder();
            sb.Append(CreateFormatString(digitsBeforeDecimal));
            sb.Append(".");
            sb.Append(CreateFormatString(digitsAfterDecimal));
            return sb.ToString();
        }
        #endregion


        #region Method - GetNextId
        /// <summary>
        /// Gets the next ID number.  Starts at 10 and increments as new IDs are requested.
        /// </summary>
        /// <returns>An unused ID number</returns>
        private int GetNextId()
        {
            var crt = nextApertureId;
            nextApertureId++;
            return crt;
        }
        #endregion

        string GetCoordinateString(double position)
        {
            var s = position.ToString(coordinateFormat, CultureInfo.InvariantCulture);
            s = s.Replace(".", "");
            var isNegative = position < 0.00d;
            if (isNegative)
                s = s.TrimStart('-');
            s = s.TrimStart('0');//we have leading zero omission
            if (isNegative)
                s = "-" + s;
            if (string.IsNullOrEmpty(s) || s == "-")
                return "0";
            return s;
        }
    }
}
