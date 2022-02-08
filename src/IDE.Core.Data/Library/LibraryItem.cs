using IDE.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IDE.Core.Storage
{
    public class LibraryItem: ILibraryItem
    {


        [XmlAttribute("id")]
        public long Id { get; set; }

        /// <summary>
        /// item name is given by the filename. Used for display mainly.
        /// </summary>
        [XmlAttribute("name")]
        public string Name
        {
            get; set;
        }

        /// <summary>
        /// Library name.
        /// <para>Can be NULL if the symbol belongs to the local library it is in. </para>
        /// </summary>
        [XmlAttribute("library")]
        public string Library
        {
            get; set;
        }

        [XmlAttribute("namespace")]
        public string Namespace
        {
            get; set;
        }

        [XmlIgnore]
        public bool IsLocal
        {
            get
            {
                return Library == null
                     || Library == "local"
                     || Library == ".";
            }
        }

        [XmlIgnore]
        public string FoundPath { get; set; }

        [XmlIgnore]
        public DateTime? LastAccessed { get; set; }

      
        

        static long lastId = 0;
        public static long GetNextId()
        {
            //max long is 9223372036854775807
            var candidate = long.Parse(DateTime.Now.ToString("yyyyMMddHHmmssff"))*1000L;

            while (candidate <= lastId)
                candidate++;

            lastId = candidate;
            return candidate;
        }
    }
}
