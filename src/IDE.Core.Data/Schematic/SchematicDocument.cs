using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IDE.Core.Storage
{
    /* Color schema for objects (from preferences)
     * Color for nets (will override preferences, stored in xml)
     * 
     * Layers will NOT be imported
     * Grid will NOT be imported (always same value 0.1")
     * Settings will NOT be imported (they are kinda useless)
     * 
     * 
     * We have two options for storing or not the libraries drawing info:
     * 1) to cache it; store it in schematics
     *      - when the schematic is loaded try to solve the reference, if not found, use the cache but show error; it must allow to build a gerber
     * 2) don't cache; throw error; for missing symbols show some empty square with error pattern fill (THIS WILL BE IMPLEMENTED)
     * 3) have an option in Preferences
     * 
     */


    /// <summary>
    /// Schematic file on disk. Serialized.
    /// <para>Any convertion from other documents of other CAD files will be converted to this document</para>
    /// </summary>
    [XmlRoot("schematic")]
    public class SchematicDocument : LibraryItem
    {
        [XmlAttribute("documentWidth")]
        public double DocumentWidth { get; set; } = 297;

        [XmlAttribute("documentHeight")]
        public double DocumentHeight { get; set; } = 210;

        [XmlAttribute("documentSize")]
        public DocumentSize DocumentSize { get; set; } = DocumentSize.A4;

        /// <summary>
        /// Description serving as comment for documentation
        /// </summary>
        [XmlElement("description")]
        public Description Description { get; set; }

        //libraries? - could be used inside sch (as eagle, equivalent to use clause) or outside in some folder of the solution/project as solved lib

        /// <summary>
        /// Our version for attributes
        /// </summary>
        [XmlArray("properties")]
        [XmlArrayItem("property")]
        public List<Property> Properties { get; set; }

        //Variants

        [XmlArray("classes")]
        [XmlArrayItem("class", typeof(NetClass))]
        [XmlArrayItem("group", typeof(NetGroup))]
        public List<NetClassBaseItem> Classes { get; set; }

#if VERSION20
        [XmlArray("modules")]
        [XmlArrayItem("module")]
        public List<Module> Modules { get; set; }
#endif

        [XmlArray("parts")]
        [XmlArrayItem("part")]
        public List<Part> Parts { get; set; } = new List<Part>();

        [XmlArray("sheets")]
        [XmlArrayItem("sheet")]
        public List<Sheet> Sheets { get; set; } = new List<Sheet>();

        //errors will be compiled on the fly

        [XmlArray("rules")]
        [XmlArrayItem("netsWithSinglePin", typeof(NetsWithSinglePinSchematicRuleData))]
        [XmlArrayItem("notConnectedPins", typeof(NotConnectedPinsSchematicRuleData))]
        [XmlArrayItem("pinTypesConnection", typeof(PinTypesConnectionSchematicRuleData))]
        public List<SchematicRuleData> Rules { get; set; } = new List<SchematicRuleData>();

        public void EnsureDefaultRules()
        {
            if (Rules == null || Rules.Count == 0)
            {
                Rules = new List<SchematicRuleData>
                {
                    new PinTypesConnectionSchematicRuleData
                    {
                         IsEnabled = true,
                         PinTypesConnections = new List<PinTypesConnectionResponseSpec>
                         {
                              //Passive
                            new PinTypesConnectionResponseSpec{  PinType1 = PinType.Passive, PinType2 = PinType.Passive, Response = SchematicRuleResponse.NoError},

                            new PinTypesConnectionResponseSpec{  PinType1 = PinType.Passive, PinType2 = PinType.Input, Response = SchematicRuleResponse.NoError},
                            new PinTypesConnectionResponseSpec{  PinType1 = PinType.Passive, PinType2 = PinType.Output, Response = SchematicRuleResponse.NoError},
                            new PinTypesConnectionResponseSpec{  PinType1 = PinType.Passive, PinType2 = PinType.IO, Response = SchematicRuleResponse.NoError},

                            new PinTypesConnectionResponseSpec{  PinType1 = PinType.Passive, PinType2 = PinType.OpenCollector, Response = SchematicRuleResponse.NoError},
                            new PinTypesConnectionResponseSpec{  PinType1 = PinType.Passive, PinType2 = PinType.OpenEmitter, Response = SchematicRuleResponse.NoError},
                            new PinTypesConnectionResponseSpec{  PinType1 = PinType.Passive, PinType2 = PinType.HiZ, Response = SchematicRuleResponse.NoError},

                            new PinTypesConnectionResponseSpec{  PinType1 = PinType.Passive, PinType2 = PinType.Power, Response = SchematicRuleResponse.NoError},
                            new PinTypesConnectionResponseSpec{  PinType1 = PinType.Passive, PinType2 = PinType.NoConnect, Response = SchematicRuleResponse.NoError},

                            //Input
                            new PinTypesConnectionResponseSpec{  PinType1 = PinType.Input, PinType2 = PinType.Input, Response = SchematicRuleResponse.NoError},
                            new PinTypesConnectionResponseSpec{  PinType1 = PinType.Input, PinType2 = PinType.Output, Response = SchematicRuleResponse.NoError},
                            new PinTypesConnectionResponseSpec{  PinType1 = PinType.Input, PinType2 = PinType.IO, Response = SchematicRuleResponse.NoError},

                            new PinTypesConnectionResponseSpec{  PinType1 = PinType.Input, PinType2 = PinType.OpenCollector, Response = SchematicRuleResponse.NoError},
                            new PinTypesConnectionResponseSpec{  PinType1 = PinType.Input, PinType2 = PinType.OpenEmitter, Response = SchematicRuleResponse.NoError},
                            new PinTypesConnectionResponseSpec{  PinType1 = PinType.Input, PinType2 = PinType.HiZ, Response = SchematicRuleResponse.NoError},

                            new PinTypesConnectionResponseSpec{  PinType1 = PinType.Input, PinType2 = PinType.Power, Response = SchematicRuleResponse.NoError},
                            new PinTypesConnectionResponseSpec{  PinType1 = PinType.Input, PinType2 = PinType.NoConnect, Response = SchematicRuleResponse.NoError},

                            //Output
                            new PinTypesConnectionResponseSpec{  PinType1 = PinType.Output, PinType2 = PinType.Output, Response = SchematicRuleResponse.Error},
                            new PinTypesConnectionResponseSpec{  PinType1 = PinType.Output, PinType2 = PinType.IO, Response = SchematicRuleResponse.Warning},

                            new PinTypesConnectionResponseSpec{  PinType1 = PinType.Output, PinType2 = PinType.OpenCollector, Response = SchematicRuleResponse.Error},
                            new PinTypesConnectionResponseSpec{  PinType1 = PinType.Output, PinType2 = PinType.OpenEmitter, Response = SchematicRuleResponse.Error},
                            new PinTypesConnectionResponseSpec{  PinType1 = PinType.Output, PinType2 = PinType.HiZ, Response = SchematicRuleResponse.NoError},

                            new PinTypesConnectionResponseSpec{  PinType1 = PinType.Output, PinType2 = PinType.Power, Response = SchematicRuleResponse.Error},
                            new PinTypesConnectionResponseSpec{  PinType1 = PinType.Output, PinType2 = PinType.NoConnect, Response = SchematicRuleResponse.NoError},

                            //IO
                            new PinTypesConnectionResponseSpec{  PinType1 = PinType.IO, PinType2 = PinType.IO, Response = SchematicRuleResponse.NoError},

                            new PinTypesConnectionResponseSpec{  PinType1 = PinType.IO, PinType2 = PinType.OpenCollector, Response = SchematicRuleResponse.NoError},
                            new PinTypesConnectionResponseSpec{  PinType1 = PinType.IO, PinType2 = PinType.OpenEmitter, Response = SchematicRuleResponse.NoError},
                            new PinTypesConnectionResponseSpec{  PinType1 = PinType.IO, PinType2 = PinType.HiZ, Response = SchematicRuleResponse.NoError},

                            new PinTypesConnectionResponseSpec{  PinType1 = PinType.IO, PinType2 = PinType.Power, Response = SchematicRuleResponse.NoError},
                            new PinTypesConnectionResponseSpec{  PinType1 = PinType.IO, PinType2 = PinType.NoConnect, Response = SchematicRuleResponse.NoError},

                            //OpenCollector
                            new PinTypesConnectionResponseSpec{  PinType1 = PinType.OpenCollector, PinType2 = PinType.OpenCollector, Response = SchematicRuleResponse.NoError},
                            new PinTypesConnectionResponseSpec{  PinType1 = PinType.OpenCollector, PinType2 = PinType.OpenEmitter, Response = SchematicRuleResponse.NoError},
                            new PinTypesConnectionResponseSpec{  PinType1 = PinType.OpenCollector, PinType2 = PinType.HiZ, Response = SchematicRuleResponse.NoError},

                            new PinTypesConnectionResponseSpec{  PinType1 = PinType.OpenCollector, PinType2 = PinType.Power, Response = SchematicRuleResponse.NoError},
                            new PinTypesConnectionResponseSpec{  PinType1 = PinType.OpenCollector, PinType2 = PinType.NoConnect, Response = SchematicRuleResponse.NoError},

                            //OpenEmitter
                            new PinTypesConnectionResponseSpec{  PinType1 = PinType.OpenEmitter, PinType2 = PinType.OpenEmitter, Response = SchematicRuleResponse.NoError},
                            new PinTypesConnectionResponseSpec{  PinType1 = PinType.OpenEmitter, PinType2 = PinType.HiZ, Response = SchematicRuleResponse.NoError},

                            new PinTypesConnectionResponseSpec{  PinType1 = PinType.OpenEmitter, PinType2 = PinType.Power, Response = SchematicRuleResponse.NoError},
                            new PinTypesConnectionResponseSpec{  PinType1 = PinType.OpenEmitter, PinType2 = PinType.NoConnect, Response = SchematicRuleResponse.NoError},

                            //HiZ
                            new PinTypesConnectionResponseSpec{  PinType1 = PinType.HiZ, PinType2 = PinType.HiZ, Response = SchematicRuleResponse.NoError},

                            new PinTypesConnectionResponseSpec{  PinType1 = PinType.HiZ, PinType2 = PinType.Power, Response = SchematicRuleResponse.NoError},
                            new PinTypesConnectionResponseSpec{  PinType1 = PinType.HiZ, PinType2 = PinType.NoConnect, Response = SchematicRuleResponse.NoError},

                            //Power
                            new PinTypesConnectionResponseSpec{  PinType1 = PinType.Power, PinType2 = PinType.Power, Response = SchematicRuleResponse.NoError},
                            new PinTypesConnectionResponseSpec{  PinType1 = PinType.Power, PinType2 = PinType.NoConnect, Response = SchematicRuleResponse.NoError},

                            //No Connect
                             new PinTypesConnectionResponseSpec{  PinType1 = PinType.NoConnect, PinType2 = PinType.NoConnect, Response = SchematicRuleResponse.NoError},
                         }
                    },
                    new NotConnectedPinsSchematicRuleData
                    {
                         IsEnabled = false,
                         RuleResponse = SchematicRuleResponse.NoError
                    },
                    new NetsWithSinglePinSchematicRuleData
                    {
                        IsEnabled = true,
                        RuleResponse = SchematicRuleResponse.Warning
                    }
                };
            }
        }
    }
}
