using IDE.Core.BOM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IDE.Core.Storage
{
    /// <summary>
    /// A part is an instance of a component
    /// It can have multiple gates (see Instance class)
    /// <para>if we have 2 LM324 we have 2 separate Part</para>
    /// </summary>
    public class Part : BaseViewModel
    {
        [XmlAttribute("id")]
        public string Id { get; set; }

        string name;
        /// <summary>
        /// Name of the component: U1, U2, R1, R2
        /// </summary>
        [XmlAttribute("name")]
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        /// <summary>
        /// the library the referred component belongs to. Used for solving
        /// </summary>
        [XmlAttribute("compLibrary")]
        public string ComponentLibrary { get; set; }

        /// <summary>
        /// id of the referenced component. Used for solving
        /// </summary>
        [XmlAttribute("compId")]
        public string ComponentId { get; set; }

        [XmlAttribute("compName")]
        public string ComponentName { get; set; }

        /// <summary>
        /// Comment for a component: for a res: 1K, 1%, 0.25W; LM324
        /// </summary>
        [XmlAttribute("comment")]
        public string Comment { get; set; }

        /// <summary>
        /// Extra properties we need to add on top of what is defined in component
        /// <para>same name overrides the property</para>
        /// </summary>
        [XmlArray("properties")]
        [XmlArrayItem("property")]
        public List<Property> Properties
        {
            get; set;
        }


            [XmlElement("bom")]
            public BomSpec Bom { get; set; }

#if VERSION20
        //[XmlElement("variant")]
        //public List<variant> variants
        //{
        //    get; set;
        //}
#endif

        public Part Clone()
        {
            var p = (Part)MemberwiseClone();
            p.Id = LibraryItem.GetNextId();
            return p;
        }

    }


}
