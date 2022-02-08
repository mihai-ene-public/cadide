using IDE.Core.Coordinates;
using IDE.Core.Designers;
using IDE.Core.Units;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace IDE.Core.Converters
{
    //this converter will keep account of coordinates system and origin point
    public abstract class TextToMilimetersPositionConverter : IValueConverter
    {

        //a reference to the canvas grid
        public CanvasGrid CanvasGrid { get; set; }

        //we will get this from a service; set by application settings
        //current CS used by app; an app wide setting
        protected AbstractCoordinateSystem ApplicationCoordinateSystem
        {
            get { return new TopLeftCoordinatesSystem(); }
        }

        protected abstract Axis Axis { get; }

        //value is in mm in top-left CS; convert to canvas grid
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var topLeftCS = new TopLeftCoordinatesSystem();
            if (!(value is double))
                return Binding.DoNothing;
            if (CanvasGrid == null || CanvasGrid.GridSizeModel == null || CanvasGrid.GridSizeModel.SelectedItem == null)
                return Binding.DoNothing;

            var mm = new MillimeterUnit((double)value);
            //we need a copy of this
            var canvasUnit = (AbstractUnit)Activator.CreateInstance(CanvasGrid.GridSizeModel.SelectedItem.GetType());

            canvasUnit.ConvertFrom(mm);

            var val = ApplicationCoordinateSystem.ConvertValueFrom(canvasUnit.CurrentValue, Axis, topLeftCS);

            //we could have a setting for the number of decimals
            return val.ToString("0.####");

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //todo: we could use regex to match what we need
            //@"-?\d+(?:\.\d+)?"
            /// "^-?\d(.\d+)?[^.]/"
            var vstr = value as string;
            if (string.IsNullOrEmpty(vstr))
                return Binding.DoNothing;
            vstr = vstr.ToLower();

            double val;
            if (vstr.Contains("mm"))
            {
                vstr = vstr.Replace("mm", "").Trim();
                if (MathParser.TryParseExpression(vstr, out val))
                    return GetTopLeftCoord(val);
            }

            else if (vstr.Contains("mil"))
            {
                vstr = vstr.Replace("mil", "").Trim();
                if (MathParser.TryParseExpression(vstr, out val))
                {
                    var mil = new MilUnit(val);
                    var mm = new MillimeterUnit();
                    mm.ConvertFrom(mil);
                    return GetTopLeftCoord(mm.CurrentValue);
                }
            }
            else
            {
                vstr = vstr.Trim();
                if (vstr.EndsWith("."))
                    return Binding.DoNothing;

                if (MathParser.TryParseExpression(vstr, out val))
                {
                    //we need a copy of this
                    var canvasUnit = (AbstractUnit)Activator.CreateInstance(CanvasGrid.GridSizeModel.SelectedItem.GetType(), val);
                    var mm = new MillimeterUnit();
                    mm.ConvertFrom(canvasUnit);
                    return GetTopLeftCoord(mm.CurrentValue);
                }
            }

            return Binding.DoNothing;
        }

        double GetTopLeftCoord(double otherVal)
        {
            var topLeftCS = new TopLeftCoordinatesSystem();
            var val = topLeftCS.ConvertValueFrom(otherVal, Axis, ApplicationCoordinateSystem);
            return val;
        }
    }

    public class TextToMilimetersPositionXConverter : TextToMilimetersPositionConverter
    {
        protected override Axis Axis
        {
            get
            {
                return Axis.X;
            }
        }
    }

    public class TextToMilimetersPositionYConverter : TextToMilimetersPositionConverter
    {
        protected override Axis Axis
        {
            get
            {
                return Axis.Y;
            }
        }
    }
}
