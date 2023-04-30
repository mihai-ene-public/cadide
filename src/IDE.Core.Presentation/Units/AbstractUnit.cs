using System;
using IDE.Core.Interfaces;

namespace IDE.Core.Units;

public abstract class AbstractUnit : ICanvasGridUnit
{
    public AbstractUnit(double value)
    {
        //DefaultValues = new ObservableCollection<double>();
        MinValue = 0;
        MaxValue = 100;
        CurrentValue = value;
    }

    /// <summary>
    /// Display a long descriptive string of the unit stored in this object.
    /// </summary>
    public string DisplayNameLong { get; set; }

    /// <summary>
    /// Display a short string of the unit stored in this object.
    /// </summary>
    public string DisplayNameShort { get; set; }

    /// <summary>
    /// Display a combination of long and short string of the unit stored in this object.
    /// </summary>
    public string DisplayNameLongWithShort
    {
        get
        {
            return $"{DisplayNameShort} ({DisplayNameLong})";
        }
    }

    public double CurrentValue { get; set; }

    public double MinValue { get; set; }

    public double MaxValue { get; set; }

    ///// <summary>
    ///// Get a list of useful default values for the unit stored in this item.
    ///// </summary>
    //public ObservableCollection<double> DefaultValues { get; set; }

    public bool IsCurrentValueInRange
    {
        get
        {
            return CurrentValue >= MinValue && CurrentValue <= MaxValue;
        }
    }

    public abstract double ConvertFrom(AbstractUnit otherUnit);

    public abstract string GetUnitRangeMessage();
}

#region Font Size units

public class ScreenFontPointsUnit : AbstractUnit
{
    public ScreenFontPointsUnit(double value)
        : base(value)
    {
        MinValue = 2;
        MaxValue = 399;
    }

    public override double ConvertFrom(AbstractUnit otherUnit)
    {
        if (otherUnit is ScreenFontPointsUnit)
        {
            CurrentValue = otherUnit.CurrentValue;
        }
        else if (otherUnit is ScreenPercentUnit)
            CurrentValue = otherUnit.CurrentValue * ScreenPercentUnit.OneHundredPercentFont / 100.0;
        else throw new NotSupportedException(otherUnit.ToString());

        return CurrentValue;
    }

    public override string GetUnitRangeMessage()
    {
        return string.Format("Enter a font size in the range of {0} - {1} points",
                            string.Format("{0:0}", MinValue),
                            string.Format("{0:0}", MaxValue));
    }
}

public class ScreenPercentUnit : AbstractUnit
{
    /// <summary>
    /// A font size of 12 is equivalent to 100% (percent) display size.
    /// </summary>
    public const double OneHundredPercentFont = 12.0;

    /// <summary>
    /// This is the standard value to scale against when using percent instead of fontsize.
    /// </summary>
    public const double OneHundredPercent = 100.0;

    public ScreenPercentUnit(double value) : base(value)
    {
        MinValue = 24;
        MaxValue = 200;
    }

    public override double ConvertFrom(AbstractUnit otherUnit)
    {
        if (otherUnit is ScreenPercentUnit)
            CurrentValue = otherUnit.CurrentValue;
        else if (otherUnit is ScreenFontPointsUnit)
            CurrentValue = otherUnit.CurrentValue * (100 / OneHundredPercentFont);
        else throw new NotSupportedException(otherUnit.ToString());

        return CurrentValue;

    }

    public override string GetUnitRangeMessage()
    {
        return string.Format("Enter a percent value in the range of {0} - {1} percent",
                            string.Format("{0:0}", MinValue),
                            string.Format("{0:0}", MaxValue));
    }
}

#endregion Font Size Units
