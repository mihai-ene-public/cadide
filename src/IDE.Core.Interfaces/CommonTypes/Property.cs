using IDE.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IDE.Core.Storage
{
    public class Property : IProperty 
    {

        string name;
        [XmlAttribute("name")]
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
                //OnPropertyChanged(nameof(Name));
            }
        }

        string _value;

        [XmlAttribute("value")]
        public string Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
                //OnPropertyChanged(nameof(Value));
            }
        }

        PropertyType propertyType;

        [XmlAttribute("type")]
        public PropertyType Type
        {
            get
            {
                return propertyType;
            }
            set
            {
                propertyType = value;
               // OnPropertyChanged(nameof(PropertyType));
            }
        }

    }


}
