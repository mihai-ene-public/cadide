using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace IDE.Core
{
    public class XmlHelper
    {
        public static void Save(object obj, string filePath)
        {
            var ser = new XmlSerializer(obj.GetType());
            var ns = new XmlSerializerNamespaces();
            ns.Add(string.Empty, string.Empty);

            using (var ms = new MemoryStream())
            {
                using (var sw = new StreamWriter(ms))
                {
                    ser.Serialize(sw, obj, ns);
                    sw.Flush();
                }

                File.WriteAllBytes(filePath, ms.ToArray());
            }
        }



        public static T Load<T>(string filePath)
        {
            var ser = new XmlSerializer(typeof(T));
            using (var sr = new StreamReader(filePath))
            {
                var p = (T)ser.Deserialize(sr);
                return p;
            }
        }

        public static object Load(string filePath, Type objectType)
        {
            var ser = new XmlSerializer(objectType);
            using (var sr = new StreamReader(filePath))
            {
                var p = ser.Deserialize(sr);
                return p;
            }
        }

        //////////////////////////////////
        public static object GetObjectFromXml(Stream stream, Type type)
        {
            XmlSerializer s = new XmlSerializer(type);
            object ret = s.Deserialize(stream);
            return ret;
        }

        public static T GetObjectFromXmlString<T>(string xmlString)
        {
            using (StringReader sr = new StringReader(xmlString))
            {
                XmlSerializer ser = new XmlSerializer(typeof(T));
                var o = ser.Deserialize(sr);

                return (T)o;
            }
        }

        public static T GetObjectfromStream<T>(Stream stream)
        {
            using (StreamReader sr = new StreamReader(stream))
            {
                XmlSerializer ser = new XmlSerializer(typeof(T));
                var o = ser.Deserialize(sr);

                return (T)o;
            }
        }

        public static string SerializeObjectToXmlString(object obj)
        {
            var sb = new StringBuilder();
            using (StringWriter sw = new StringWriter(sb))
            {
                XmlSerializer ser = new XmlSerializer(obj.GetType());

                ser.Serialize(sw, obj);

                sw.Flush();
                sw.Close();
            }

            return sb.ToString();
        }



        public static void SerializeObjectToStream<T>(string xmlPath, object obj)// where T : class
        {
            using (StreamWriter sw = new StreamWriter(xmlPath))
            {
                XmlSerializer ser = new XmlSerializer(typeof(T));

                ser.Serialize(sw, obj);

                sw.Flush();
                sw.Close();
            }
        }

        public static void SerializeObjectToStream(object obj, Stream stream)
        {
            XmlSerializer s = new XmlSerializer(obj.GetType());
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            s.Serialize(stream, obj, ns);
            stream.Flush();
            stream.Close();
        }

    }
}
