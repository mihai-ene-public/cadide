using IDE.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IDE.Core.Settings
{
    public abstract class BasicSetting : BasicSettingNode
    {
    }


    public abstract class BasicSettingNode : ISettingData
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

    }
}
