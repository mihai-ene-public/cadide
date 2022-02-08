namespace IDE.Core.Units
{
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows.Input;

    using System.ComponentModel;
    using Commands;

    /// <summary>
    /// Viewmodel class to manage unit conversion based on default values and typed values.
    /// </summary>
    public class UnitViewModel : BaseViewModel, IDataErrorInfo
    {
        #region fields

        protected AbstractUnit selectedItem;

        protected ObservableCollection<AbstractUnit> units;

        string valueTip = string.Empty;

        ICommand setSelectedItemCommand;



        #endregion fields

        #region constructor
        /// <summary>
        /// Class constructor to construct complete viewmodel object from listed parameters.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="converter"></param>
        /// <param name="defaultIndex"></param>
        /// <param name="defaultValue"></param>
        public UnitViewModel(ObservableCollection<AbstractUnit> list,
                             double defaultValue = 100)
        {
            units = new ObservableCollection<AbstractUnit>(list);
            if (units.Count > 0)
                selectedItem = units[0];

            CurrentValue = defaultValue;
        }

        /// <summary>
        /// Standard class constructor is hidden in favour of parameterized constructor.
        /// </summary>
        protected UnitViewModel()
        {
        }
        #endregion constructor

        #region properties

        /// <summary>
        /// Get list of units, their default value lists, itemkeys etc.
        /// </summary>
        public ObservableCollection<AbstractUnit> UnitList
        {
            get
            {
                return units;
            }
        }

        /// <summary>
        /// Get/set currently selected unit key, converter, and default value list.
        /// </summary>
        public AbstractUnit SelectedItem
        {
            get
            {
                return selectedItem;
            }

            set
            {
               // if (selectedItem != value)
                {
                    if (value is AdimensionalUnit)
                    {
                        selectedItem.CurrentValue = value.CurrentValue;
                    }
                    else
                        selectedItem = value;

                    OnPropertyChanged(nameof(SelectedItem));
                    OnPropertyChanged(nameof(CurrentValue));
                }
            }
        }

        #region IDataErrorInfo Interface
        /// <summary>
        /// Source: http://joshsmithonwpf.wordpress.com/2008/11/14/using-a-viewmodel-to-provide-meaningful-validation-error-messages/
        /// </summary>
        public string Error
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Standard property that is part of the <seealso cref="IDataErrorInfo"/> interface.
        /// 
        /// Evaluates if StringValue parameter represents a value within the expected range
        /// and sets a corresponding errormessage in the ValueTip property if not.
        /// 
        /// Source: http://joshsmithonwpf.wordpress.com/2008/11/14/using-a-viewmodel-to-provide-meaningful-validation-error-messages/
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public string this[string propertyName]
        {
            get
            {
                if (propertyName == nameof(CurrentValue))
                {
                    var message = selectedItem.GetUnitRangeMessage();

                    if (selectedItem.IsCurrentValueInRange)
                    {
                        return SetToolTip(null);
                    }
                    else
                        return SetToolTip(message);
                }

                return SetToolTip(null);
            }
        }
        #endregion IDataErrorInfo Interface

        /// <summary>
        /// Get a string that indicates the format of the
        /// expected input or a an error if the current input is not valid.
        /// </summary>
        public string ValueTip
        {
            get
            {
                return valueTip;
            }

            protected set
            {
                if (valueTip != value)
                {
                    valueTip = value;
                    OnPropertyChanged(nameof(ValueTip));
                }
            }
        }

        /// <summary>
        /// Get double value represented in unit as indicated by SelectedItem.Key.
        /// </summary>
        public double CurrentValue
        {
            get
            {
                return SelectedItem.CurrentValue;
            }

            set
            {
                if (SelectedItem.CurrentValue != value)
                {
                    SelectedItem.CurrentValue = value;

                    OnPropertyChanged(nameof(CurrentValue));
                }
            }
        }



        /// <summary>
        /// Get command to be executed when the user has selected a unit
        /// (eg. 'Km' is currently used but user selected 'm' to be used next)
        /// </summary>
        public ICommand SetSelectedItemCommand
        {
            get
            {
                if (setSelectedItemCommand == null)
                    setSelectedItemCommand = CreateCommand(p =>
                    {
                        SetSelectedItem(p as AbstractUnit);
                    },
                    p => true);

                return setSelectedItemCommand;
            }
        }

        #endregion properties

        #region methods
        /// <summary>
        /// Convert current double value from current unit to
        /// unit as indicated by <paramref name="unitKey"/> and
        /// set corresponding SelectedItem.
        /// </summary>
        /// <param name="unitKey">New unit to convert double value into and set SelectedItem to.</param>
        /// <returns></returns>
        public void SetSelectedItem(AbstractUnit unitKey)
        {
            // Find the next selected item
            var newUnit = units.SingleOrDefault(i => i.GetType() == unitKey.GetType());

            // Convert from current item to find the next selected item
            if (newUnit != null)
            {
                var tempValue = newUnit.ConvertFrom(selectedItem);

                if (tempValue < unitKey.MinValue)
                    tempValue = unitKey.MinValue;
                else
                  if (tempValue > unitKey.MaxValue)
                    tempValue = unitKey.MaxValue;

                SelectedItem = newUnit;
                CurrentValue = tempValue;

                OnPropertyChanged(nameof(CurrentValue));
                OnPropertyChanged(nameof(SelectedItem));
            }

        }


        /// <summary>
        /// Set a tip like string to indicate the expected input format
        /// or input errors (if there are any input errors).
        /// </summary>
        /// <param name="strError"></param>
        /// <returns></returns>
        string SetToolTip(string strError)
        {
            ValueTip = strError;
            return strError;
        }



        #endregion methods
    }
}
