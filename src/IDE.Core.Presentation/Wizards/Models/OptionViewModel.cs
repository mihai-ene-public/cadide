using System;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace IDE.Core.Wizards
{

    public interface IWizardOption
    {
        bool IsSelected { get; }
        string DisplayName { get; }
    }

    /// <summary>
    /// Represents a single selectable value with a user-friendly name that can be selected by the user.
    /// The name displayed will be taken from a camel-case split of the type (as in an enum name; Coffee.DarkBlend becomes "Dark Blend")
    /// or yes/no for bools.
    /// </summary>
    /// <typeparam name="TValue">The type of value represented by the option.</typeparam>
    /// <remarks>IComparable so you can sort the list before rendering</remarks>
    public class OptionViewModel<TValue> : IComparable<OptionViewModel<TValue>>, IWizardOption
    {

        /// <summary>
        /// A collection of messages which may appear next to an option explaining why it's disabled.
        /// </summary>
        private readonly ObservableCollection<string> _Messages;
        private bool _enabled = true;
        const int UNSET_SORT_VALUE = Int32.MinValue;

        readonly string _displayName;
        bool _isSelected;
        readonly int _sortValue;
        readonly string _toolTip;
        readonly TValue _value;

        public bool Enabled
        {
            get { return _enabled; }
            set
            {
                _enabled = value;
                OnPropertyChanged("Enabled");
            }
        }

        public OptionViewModel(TValue value)
            : this(value, UNSET_SORT_VALUE)
        {
            _Messages = new ObservableCollection<string>();
        }

        public OptionViewModel(TValue value, int sortValue)
            : this(value, sortValue, null)
        {
            _Messages = new ObservableCollection<string>();
        }

        public OptionViewModel(TValue value, int sortValue, string toolTip)
            : this(value, sortValue, toolTip, null)
        {
            _Messages = new ObservableCollection<string>();
        }

        public OptionViewModel(TValue value, int sortValue, string toolTip, string altLabel)
        {
            if (string.IsNullOrEmpty(altLabel))
            {
                if (typeof(TValue) == typeof(bool))
                {
                    _displayName = (Convert.ToBoolean(value)) ? "Yes" : "No";
                }
                else
                {
                    _displayName = value.ToString().SplitCamelCase();
                }
            }
            else
            {
                _displayName = altLabel;
            }
            _value = value;
            _sortValue = sortValue;
            _toolTip = toolTip;
            _Messages = new ObservableCollection<string>();
        }

        public ObservableCollection<string> Messages
        {
            get
            {
                return _Messages;
            }
        }

        /// <summary>
        /// Returns the user-friendly name of this option.
        /// </summary>
        public string DisplayName
        {
            get { return _displayName; }
        }

        public string ToolTip
        {
            get { return _toolTip; }
        }

        /// <summary>
        /// Gets/sets whether this option is in the selected state.
        /// When this property is set to a new value, this object's
        /// PropertyChanged event is raised.
        /// </summary>
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (value == _isSelected)
                {
                    return;
                }
                _isSelected = value;
                OnPropertyChanged("IsSelected");
            }
        }

        /// <summary>
        /// Returns the value used to sort this option.
        /// The default sort value is Int32.MinValue.
        /// </summary>
        public int SortValue
        {
            get { return _sortValue; }
        }

        public TValue Value { get { return _value; } }

        /// <summary>
        /// Returns the underlying value of this option.
        /// Note: this is a method, instead of a property,
        /// so that the UI cannot bind to it.
        /// </summary>
        public TValue GetValue()
        {
            return _value;
        }

        //IComparable<OptionViewModel<TValue>>
        public int CompareTo(OptionViewModel<TValue> other)
        {
            if (other == null)
                return -1;

            if (this.SortValue == UNSET_SORT_VALUE && other.SortValue == UNSET_SORT_VALUE)
            {
                return this.DisplayName.CompareTo(other.DisplayName);
            }
            else if (this.SortValue != UNSET_SORT_VALUE && other.SortValue != UNSET_SORT_VALUE)
            {
                return this.SortValue.CompareTo(other.SortValue);
            }
            else if (this.SortValue != UNSET_SORT_VALUE && other.SortValue == UNSET_SORT_VALUE)
            {
                return -1;
            }
            else
            {
                return +1;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is OptionViewModel<TValue>)
                return _value.Equals((obj as OptionViewModel<TValue>)._value);
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

    }

}
