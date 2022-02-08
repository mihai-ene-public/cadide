using IDE.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;


namespace IDE.Core.Storage
{
    public class NetClass : NetClassBaseItem
    {
    }

    public class NetGroup : NetClassBaseItem
    {
        [XmlElement("class", typeof(NetClass))]
        [XmlElement("group", typeof(NetGroup))]
        public List<NetClassBaseItem> Children { get; set; } = new List<NetClassBaseItem>();
    }

    public class NetClassBaseItem: INetClassBaseItem
    {
        [XmlAttribute("id")]
        public long Id
        {
            get; set;
        }


        [XmlAttribute("name")]
        public string Name
        {
            get; set;
        }
    }
}
