using IDE.Core.Units;
using System.ComponentModel;
using IDE.Core.Interfaces;
using IDE.Core.Types.Media;

namespace IDE.Core.Designers
{

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

            currentUnit = new MillimeterUnit(1);
            GridSizeModel = new LengthUnitsViewModel();
            GridSizeModel.CurrentValue = currentUnit.CurrentValue;
            //GridSizeModel.SetSelectedItem(currentUnit);
            GridSizeModel.PropertyChanged += GridSizeModel_PropertyChanged;
        }

        //this is to store the right units, the default is mm
        //we could have later in Application.Options units to be stored by default
        AbstractUnit currentUnit;

        /// <summary>
        /// Unit of grid in milimeters
        /// </summary>
        public AbstractUnit CurrentUnit
        {
            get { return currentUnit; }
        }

        public LengthUnitsViewModel GridSizeModel { get; set; }

        public XRect ViewPortSize
        {
            get
            {
                currentUnit.ConvertFrom(GridSizeModel.SelectedItem);
                //todo: we will do a converter for rect
                //var screenPoints = MilimetersToScreenPointsDoubleConverter.Instance.Convert(currentUnit.CurrentValue);

                return new XRect(0, 0, currentUnit.CurrentValue, currentUnit.CurrentValue);
            }
        }

        public double MinorDistance
        {
            get
            {
                currentUnit.ConvertFrom(GridSizeModel.SelectedItem);
                return currentUnit.CurrentValue;
            }
        }

        public double MajorDistance
        {
            get
            {
                return 10 * MinorDistance;
            }
        }

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


        private void GridSizeModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(LengthUnitsViewModel.CurrentValue))
            {
                currentUnit.ConvertFrom(GridSizeModel.SelectedItem);

                OnPropertyChanged(nameof(ViewPortSize));
                OnPropertyChanged(nameof(MinorDistance));
                OnPropertyChanged(nameof(MajorDistance));
            }
        }
    }
}
