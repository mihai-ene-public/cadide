using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IDE.Core.Storage
{
    public class Description
    {
        //language will be ignored

        [XmlText]
        public string Value
        {
            get; set;
        }
    }
}
