using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace IDE.Core.Presentation.Tests
{
    public static class Helpers
    {
        public static T GetObjectFromXmlString<T>(string xmlString)
        {
            using (StringReader sr = new StringReader(xmlString))
            {
                XmlSerializer ser = new XmlSerializer(typeof(T));
                var o = ser.Deserialize(sr);

                return (T)o;
            }
        }
    }
}
