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
    public class TextToMilimetersSizeConverter : IValueConverter
    {

        //a reference to the canvas grid
        public CanvasGrid CanvasGrid { get; set; }

        //value is in mm; convert to canvas grid
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is double))
                return Binding.DoNothing;
            if (CanvasGrid == null || CanvasGrid.GridSizeModel == null || CanvasGrid.GridSizeModel.SelectedItem == null)
                return Binding.DoNothing;

            var mm = new MillimeterUnit((double)value);
            //we need a copy of this
            var canvasUnit = (AbstractUnit)Activator.CreateInstance(CanvasGrid.GridSizeModel.SelectedItem.GetType());

            canvasUnit.ConvertFrom(mm);

            //we could have a setting for the number of decimals
            return canvasUnit.CurrentValue.ToString("0.####");

            // return $"{canvasUnit.CurrentValue.ToString("#.####")} {canvasUnit.DisplayNameShort}";
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
                    return val;
            }

            else if (vstr.Contains("mil"))
            {
                vstr = vstr.Replace("mil", "").Trim();
                if (MathParser.TryParseExpression(vstr, out val))
                {
                    var mil = new MilUnit(val);
                    var mm = new MillimeterUnit();
                    mm.ConvertFrom(mil);
                    return mm.CurrentValue;
                }
            }
            else
            {
                vstr = vstr.Trim();
                if (vstr.EndsWith("."))
                    return new[] { Binding.DoNothing, Binding.DoNothing };

                if (MathParser.TryParseExpression(vstr, out val))
                {
                    //we need a copy of this
                    var canvasUnit = (AbstractUnit)Activator.CreateInstance(CanvasGrid.GridSizeModel.SelectedItem.GetType(), val);
                    var mm = new MillimeterUnit();
                    mm.ConvertFrom(canvasUnit);
                    return mm.CurrentValue;
                }
            }

            return Binding.DoNothing;
        }

      
    }
}
