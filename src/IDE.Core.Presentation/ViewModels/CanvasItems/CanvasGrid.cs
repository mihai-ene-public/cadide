using IDE.Core.Units;
using System.ComponentModel;
using IDE.Core.Interfaces;
using IDE.Core.Types.Media;

namespace IDE.Core.Designers;


/// <summary>
/// this is the Grid for the drawing canvas
/// size is in mm on behind, but it can be changed to any supported units
/// </summary>
public class CanvasGrid : BaseViewModel, ICanvasGrid
{
    public CanvasGrid()
    {
        IsEnabled = true;
        IsVisible = true;

        _currentUnitInternal = new MillimeterUnit(1);
        GridSizeModel = new LengthUnitsViewModel();
        GridSizeModel.CurrentValue = _currentUnitInternal.CurrentValue;
        //GridSizeModel.SetSelectedItem(currentUnit);
        GridSizeModel.PropertyChanged += GridSizeModel_PropertyChanged;
    }

    //this is to store the right units, the default is mm
    //we could have later in Application.Options units to be stored by default
    private readonly AbstractUnit _currentUnitInternal;

    public ICanvasGridUnit CurrentUnit => GridSizeModel.SelectedItem;

    public LengthUnitsViewModel GridSizeModel { get; set; }

    public XRect ViewPortSize
    {
        get
        {
            _currentUnitInternal.ConvertFrom(GridSizeModel.SelectedItem);
            //todo: we will do a converter for rect
            //var screenPoints = MilimetersToScreenPointsDoubleConverter.Instance.Convert(currentUnit.CurrentValue);

            return new XRect(0, 0, _currentUnitInternal.CurrentValue, _currentUnitInternal.CurrentValue);
        }
    }

    public double MinorDistance
    {
        get
        {
            _currentUnitInternal.ConvertFrom(GridSizeModel.SelectedItem);
            return _currentUnitInternal.CurrentValue;
        }
    }

    public double MajorDistance
    {
        get
        {
            return 10 * MinorDistance;
        }
    }

    public double GridSize => _currentUnitInternal.CurrentValue;

    bool isEnabled;

    /// <summary>
    /// true if items will snap to grid
    /// </summary>
    public bool IsEnabled
    {
        get { return isEnabled; }
        set
        {
            isEnabled = value;
            OnPropertyChanged(nameof(IsEnabled));
        }
    }

    bool isVisible;

    /// <summary>
    /// true if grid is visible on designer
    /// </summary>
    public bool IsVisible
    {
        get { return isVisible; }
        set
        {
            isVisible = value;
            OnPropertyChanged(nameof(IsVisible));
        }
    }

    public bool CanSnapToGrid
    {
        get { return IsVisible && IsEnabled; }
    }

    public void SetUnit(ICanvasGridUnit unit)
    {
        GridSizeModel.SelectedItem = (AbstractUnit)unit;
    }

    public void SetValue(double value)
    {
        GridSizeModel.CurrentValue = value;
    }

    private void GridSizeModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(LengthUnitsViewModel.CurrentValue))
        {
            _currentUnitInternal.ConvertFrom(GridSizeModel.SelectedItem);

            OnPropertyChanged(nameof(ViewPortSize));
            OnPropertyChanged(nameof(MinorDistance));
            OnPropertyChanged(nameof(MajorDistance));
        }
    }
}
