using System.Xml.Serialization;

namespace IDE.Core.Storage
{
    //public enum pinVisible
    //{
    //    off,
    //    pad,
    //    pin,
    //    both,
    //}


    public enum PinType
    {
        Passive,
        
        Input,
        Output,
        /// <summary>
        /// Input/Output, Bidirectional
        /// </summary>
        IO,

        OpenCollector,
        OpenEmitter,
        HiZ,

        //Supply,
        [XmlEnum("Supply")]
        Power ,//= Supply,
        
        NoConnect
    }
}
