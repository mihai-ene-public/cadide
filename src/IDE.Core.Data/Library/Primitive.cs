using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using IDE.Core.Interfaces;

namespace IDE.Core.Storage
{

    public abstract class Primitive : IPrimitive
    {
        public object Clone()
        {
            return MemberwiseClone();
        }
    }

    public abstract class SchematicPrimitive : Primitive
    {
        [XmlAttribute("zIndex")]
        public int ZIndex { get; set; }
    }

    public abstract class LayerPrimitive : Primitive
    {
        //some primitives don't care about board placement; these will ignore placement
    }

    public abstract class MeshPrimitive : Primitive
    {
        /// <summary>
        /// Identity this item
        /// </summary>
        [XmlAttribute("id")]
        public long Id { get; set; }

        [XmlAttribute("fillColor")]
        public string FillColor { get; set; }

        [XmlAttribute("padNumber")]
        public int PadNumber { get; set; }

        [XmlIgnore]
        public bool PadNumberSpecified { get { return PadNumber > 0; } }

    }
}
