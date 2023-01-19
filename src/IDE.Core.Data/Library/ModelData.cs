using IDE.Core.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IDE.Core.Storage
{
    /* A model is added to a library by browsing to a path in 2 ways:
     *      - when u set the model for a footprint
     *      - add the model to the library specifically
     * You can update the model by right clicking in a list/a button on the same line...
     * Deleting the model from the library, will keep the data for the footprint, but will raise an error on compile
     * 
     */
    public class ModelData// : LibraryItem
    {


        [XmlAttribute("centerX")]
        public double CenterX { get; set; }

        [XmlAttribute("centerY")]
        public double CenterY { get; set; }

        [XmlAttribute("centerZ")]
        public double CenterZ { get; set; }

        [XmlAttribute("rotationX")]
        public double RotationX { get; set; }

        [XmlAttribute("rotationY")]
        public double RotationY { get; set; }

        [XmlAttribute("rotationZ")]
        public double RotationZ { get; set; }

        [XmlAttribute("modelId")]
        public string ModelId { get; set; }

        /// <summary>
        /// item name is given by the filename. Used for display mainly.
        /// </summary>
        [XmlAttribute("modelName")]
        public string ModelName
        {
            get; set;
        }

        /// <summary>
        /// Library name.
        /// <para>Can be NULL if the symbol belongs to the local library it is in. </para>
        /// </summary>
        [XmlAttribute("modelLibrary")]
        public string ModelLibrary
        {
            get; set;
        }

        ///// <summary>
        ///// cached version of the model
        ///// </summary>
        //[XmlElement("model")]
        //public ModelDocument Model { get; set; }
    }
}
