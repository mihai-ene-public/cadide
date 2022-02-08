using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Globalization;

namespace IDE.Core.Excelon
{
    /// <summary>
    /// NC Drill File
    /// 
    /// http://www.apcircuits.com/resources/information/nc_codes.html
    /// </summary>
    public class NCDrillFileWriter : StreamWriter
    {


        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="NCDrillFile"/> class.
        /// </summary>
        /// <param name="filename">The filename of the file.</param>
        public NCDrillFileWriter(string filePath) : base(filePath)
        {
        }

        public NCDrillFileWriter(Stream stream) : base(stream)
        {
        }

        #endregion

        int nextToolId = 1;
        string coordinateFormat = "00.000";
        NCGraphicsState graphicState = new NCGraphicsState();


        #region WriteHeader
        ///// <summary>
        ///// Writes the header.
        ///// </summary>
        //public void WriteHeader()
        //{
        //    StreamWriter sw = GetStream();
        //    sw.WriteLine("G92");//?
        //    sw.WriteLine("M48");
        //    sw.WriteLine("INCH,TZ");//we will have leading zeroes LZ
        //    sw.WriteLine("FMAT,2");
        //}

        public void StartHeader()
        {
            WriteLine("M48");
            //WriteLine("FMAT,2");
        }

        public void SetUnits(NCUnits units)
        {
            //we will have leading zeroes LZ by default
            string m = "INCH,LZ";
            if (units == NCUnits.Millimeters)
                m = "METRIC,LZ";

            WriteLine(m);
        }

        //call SetUnits first
        public void SetFormat(int digitsBeforeDecimal, int digitsAfterDecimal)
        {
            //range values is determined by the current units
            digitsBeforeDecimal = CoerceToRange(digitsBeforeDecimal, 3, 4);
            digitsAfterDecimal = CoerceToRange(digitsAfterDecimal, 3, 4);

            WriteLine($";FILE_FORMAT={digitsBeforeDecimal}:{digitsAfterDecimal}");//add as a comment
            coordinateFormat = CreateFormatString(digitsBeforeDecimal, digitsAfterDecimal);
        }

        /// <summary>
        /// Adds the tool.
        /// </summary>
        /// <param name="holeDiameter">The hole diameter.</param>
        /// <returns></returns>
        public int AddTool(double holeDiameter)
        {
            int id = GetNextToolId();

            // TODO: Make this format configurable
            WriteLine("T{0:00}C{1}", id, holeDiameter.ToString("f3"));

            return id;
        }

        public void EndHeader()
        {
            WriteLine("%");
        }

        #endregion

        /// <summary>
        /// Writes the footer of the NCDrill file.
        /// </summary>
        public void WriteEndFile()
        {
            WriteLine("M30");
        }

        #region Method - SelectTool
        /// <summary>
        /// Writes a command to the file that selects the tool with the specified number.
        /// <c>number</c> must be greater than or equal to 1.
        /// </summary>
        /// <param name="number">The number of the tool to select.</param>
        public void SelectTool(int number)
        {
            if (graphicState.CurrentTool == number)
                return;
            graphicState.CurrentTool = number;

            WriteLine("T{0:00}", number); // 2 digits, leading zeros
        }
        #endregion


        #region Method - DrillAt
        /// <summary>
        /// Places a drill location at location (x,y) using the currently-selected tool.
        /// </summary>
        /// <param name="x">The x-coordinate.</param>
        /// <param name="y">The y-coordinate.</param>
        public void DrillAt(double x, double y)
        {
            graphicState.CurrentX = x;
            graphicState.CurrentY = y;

            //string format = "00.0000";
            //WriteLine("X{0}Y{1}", x.ToString(format), y.ToString(format));
            WriteLine("X{0}Y{1}", GetCoordinateString(x), GetCoordinateString(y));
        }
        #endregion

        public void WriteComment(string comment)
        {
            WriteLine(";" + comment);
        }

        /// <summary>
        /// enters routing mode; Positions to the specified coordinates
        /// </summary>
        public void StartRoutingFrom(double x, double y)
        {
            WriteLine($"G00X{GetCoordinateString(x)}Y{GetCoordinateString(y)}");
        }

        /// <summary>
        /// plunge into work
        /// </summary>
        public void BeginRouting()
        {
            WriteLine("M15");
        }

        public void RouteLinearTo(double x, double y)
        {
            WriteLine($"G01X{GetCoordinateString(x)}Y{GetCoordinateString(y)}");
        }

        public void RouteArcClockWiseTo(double x,double y, double radius)
        {
            WriteLine($"G02X{GetCoordinateString(x)}Y{GetCoordinateString(y)}A{GetCoordinateString(radius)}");
        }

        public void RouteArcCounterClockWiseTo(double x, double y, double radius)
        {
            WriteLine($"G03X{GetCoordinateString(x)}Y{GetCoordinateString(y)}A{GetCoordinateString(radius)}");
        }

        public void RetractWithClamping()
        {
            WriteLine("M16");
        }

        public void RetractWithoutClamping()
        {
            WriteLine("M17");
        }

        // ----------------------------------------
        // Private Helper Methods
        // ----------------------------------------


        #region Method - GetNextToolId

        int GetNextToolId()
        {
            int id = nextToolId;
            nextToolId++;
            return id;
        }
        #endregion

        string GetCoordinateString(double position)
        {
            var s = position.ToString(coordinateFormat, CultureInfo.InvariantCulture);
            s = s.Replace(".", "");
            s = s.TrimEnd('0');//we have leading zero included. Remove trailing zero
            if (string.IsNullOrEmpty(s))
                return "0";
            return s;
        }

        /// <summary>
        /// Creates the format string having the specified number of zeros.
        /// </summary>
        /// <param name="numZeros">The number of zeros.</param>
        /// <returns>A format string having numZeros zeros.</returns>
        string CreateFormatString(int numZeros)
        {
            return new string('0', numZeros);
        }

        /// <summary>
        /// Creates a format string having the specified number of digits before and after the decimal point.
        /// </summary>
        /// <param name="digitsBeforeDecimal">The number of digits before the decimal.</param>
        /// <param name="digitsAfterDecimal">The number of digits after the decimal.</param>
        /// <returns>A string in the format "m.n" where m and n are the number of zeros before and after the decimal, respectively</returns>
        string CreateFormatString(int digitsBeforeDecimal, int digitsAfterDecimal)
        {
            var sb = new StringBuilder();
            sb.Append(CreateFormatString(digitsBeforeDecimal));
            sb.Append(".");
            sb.Append(CreateFormatString(digitsAfterDecimal));
            return sb.ToString();
        }

        int CoerceToRange(int value, int min, int max)
        {
            if (value > max) return max;
            if (value < min) return min;
            return value;
        }
    }
}
