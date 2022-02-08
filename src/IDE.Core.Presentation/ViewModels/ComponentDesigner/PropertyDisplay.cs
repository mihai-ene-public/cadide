using IDE.Core;
using IDE.Core.Interfaces;

namespace IDE.Documents.Views
{
    public class PropertyDisplay : BaseViewModel
    {
        string name;
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        string _value;
        public string Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
                OnPropertyChanged(nameof(Value));
            }
        }

        PropertyType propertyType;
        public PropertyType PropertyType
        {
            get
            {
                return propertyType;
            }
            set
            {
                propertyType = value;
                OnPropertyChanged(nameof(PropertyType));
            }
        }
    }

}
