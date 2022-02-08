using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace IDE.Core.Units
{
    public class LengthUnitsViewModel : UnitViewModel
    {
        public LengthUnitsViewModel()
        {
            units = new ObservableCollection<AbstractUnit>();
            units.Add(new MilUnit());
            units.Add(new MillimeterUnit());

            if (units.Count > 0)
                selectedItem = units[1];

            CurrentValue = 0;
        }

        public IList<string> DefaultValues
        {
            get
            {
                return new List<string>
                {
                    "1 mil",
                    "5 mil",
                    "10 mil",
                    "20 mil",
                    "25 mil",
                    "50 mil",
                    "100 mil",
                    "0.025 mm",
                    "0.10 mm",
                    "0.25 mm",
                    "0.50 mm",
                    "1.00 mm",
                    "2.50 mm",
                };
            }
        }
    }

    #region Length Units (distance units: mm, mils, inches, etc)

    /// <summary>
    /// This is a length unit 1mil = 1/1000 inches
    /// <para>1" = 25.4 mm</para>
    /// <para>1 mil = 25.4/1000 mm = 0.0254 mm </para>
    /// </summary>
    public class MilUnit : AbstractUnit
    {
        public MilUnit() : this(0)
        {
        }

        public MilUnit(double value) : base(value)
        {
            MinValue = 0;
            MaxValue = double.MaxValue;
            //DefaultValues = new ObservableCollection<double>() { 10, 12, 16, 24 };

            DisplayNameLong = "mil";
            DisplayNameShort = "mil";
        }

        public override double ConvertFrom(AbstractUnit otherUnit)
        {
            if (otherUnit is MilUnit || otherUnit is AdimensionalUnit)
                CurrentValue = otherUnit.CurrentValue;
            else if (otherUnit is MillimeterUnit)
                CurrentValue = otherUnit.CurrentValue / 0.0254;
            else throw new NotSupportedException(otherUnit.ToString());

            return CurrentValue;
        }

        public override string GetUnitRangeMessage()
        {
            return string.Format("Enter a value in the range of {0} - {1} mil",
                               string.Format("{0:0}", MinValue),
                               string.Format("{0:0}", MaxValue));
        }
    }

    /// <summary>
    /// 1 mm = 1/0.0254 mil
    /// </summary>
    public class MillimeterUnit : AbstractUnit
    {
        public MillimeterUnit() : this(0)
        {
            //
            //
        }

        public MillimeterUnit(double value) : base(value)
        {
            MinValue = 0;
            MaxValue = double.MaxValue;
            //DefaultValues = new ObservableCollection<double>() { 10, 12, 16, 24 };

            DisplayNameLong = "millimeters";
            DisplayNameShort = "mm";
        }

        public override double ConvertFrom(AbstractUnit otherUnit)
        {
            if (otherUnit is MilUnit)
                CurrentValue = otherUnit.CurrentValue * 0.0254;
            else if (otherUnit is MillimeterUnit || otherUnit is AdimensionalUnit)
                CurrentValue = otherUnit.CurrentValue;
            else throw new NotSupportedException(otherUnit.ToString());

            return CurrentValue;
        }

        public override string GetUnitRangeMessage()
        {
            return string.Format("Enter a value in the range of {0} - {1} mm",
                             string.Format("{0:0}", MinValue),
                             string.Format("{0:0}", MaxValue));
        }
    }

    #endregion Length Units

    public class AdimensionalUnit : AbstractUnit
    {
        public AdimensionalUnit(double value) : base(value)
        {
            DisplayNameLong = "ul";
            DisplayNameShort = "ul";
        }

        public override double ConvertFrom(AbstractUnit otherUnit)
        {
            CurrentValue = otherUnit.CurrentValue;
            return CurrentValue;
        }

        public override string GetUnitRangeMessage()
        {
            return null;
        }
    }
}
