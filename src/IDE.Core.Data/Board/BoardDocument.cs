using IDE.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IDE.Core.Storage
{
    /// <summary>
    /// Board file on disk. Serialized.
    /// <para>Any conversion from other documents of other CAD files will be converted to this document</para>
    /// </summary>
    [XmlRoot("board")]
    public class BoardDocument : LibraryItem
    {

        public BoardDocument()
        {
            PlainItems = new List<LayerPrimitive>();
            BoardOutline = new RegionBoard();
            BoardRules = new List<BoardRule>();
            SchematicReference = new SchematicRef();
        }

        [XmlAttribute("documentWidth")]
        public double DocumentWidth { get; set; } = 297;

        [XmlAttribute("documentHeight")]
        public double DocumentHeight { get; set; } = 210;

        //stored units are in mm. We could use this for display, or remove it
        [XmlAttribute("units")]
        public Units BoardUnits { get; set; }

        /// <summary>
        /// Description serving as comment for documentation
        /// </summary>
        [XmlElement("description")]
        public Description Description { get; set; }

        [XmlElement("schematic")]
        public SchematicRef SchematicReference { get; set; }

        [XmlArray("layers")]
        [XmlArrayItem("layer")]
        public List<Layer> Layers { get; set; } = new List<Layer>();

        [XmlArray("layerGroups")]
        [XmlArrayItem("layerGroup")]
        public List<LayerGroup> LayerGroups { get; set; } = new List<LayerGroup>();

        [XmlArray("drillPairs")]
        [XmlArrayItem("pair")]
        public List<LayerPair> DrillPairs { get; set; } = new List<LayerPair>();

        [XmlArray("layerPairs")]
        [XmlArrayItem("pair")]
        public List<LayerPair> LayerPairs { get; set; } = new List<LayerPair>();

        [XmlElement("boardOutline")]
        public RegionBoard BoardOutline { get; set; }

        /// <summary>
        /// Items that do not belong to any net
        /// </summary>
        [XmlArray("items")]
        [XmlArrayItem("circle", typeof(CircleBoard))]
        //[XmlArrayItem("ellipse", typeof(EllipseBoard))]
        [XmlArrayItem("poly", typeof(PolygonBoard))]
        [XmlArrayItem("rect", typeof(RectangleBoard))]
        [XmlArrayItem("text", typeof(TextBoard))]
        [XmlArrayItem("mono", typeof(TextSingleLineBoard))]
        [XmlArrayItem("line", typeof(LineBoard))]
        [XmlArrayItem("arc", typeof(ArcBoard))]
        [XmlArrayItem("hole", typeof(Hole))]
        public List<LayerPrimitive> PlainItems { get; set; }

        //libraries

        /// <summary>
        /// Our version for attributes
        /// </summary>
        [XmlArray("properties")]
        [XmlArrayItem("property")]
        public List<Property> Properties { get; set; }

        //Variants

        //imported from schematic
        [XmlArray("classes")]
        [XmlArrayItem("class", typeof(NetClass))]
        [XmlArrayItem("group", typeof(NetGroup))]
        public List<NetClassBaseItem> Classes { get; set; }



        [XmlArray("parts")]
        [XmlArrayItem("part")]
        public List<BoardComponentInstance> Components { get; set; } = new List<BoardComponentInstance>();

        /// <summary>
        /// Nets
        /// </summary>
        [XmlArray("nets")]
        [XmlArrayItem("net")]
        public List<BoardNet> Nets { get; set; } = new List<BoardNet>();

        [XmlArray("boardDesignRules")]
        //Electrical
        [XmlArrayItem("electricalClearance", typeof(ElectricalClearanceRule))]
        //Routing
        [XmlArrayItem("trackWidth", typeof(TrackWidthRule))]
        [XmlArrayItem("viaDefinition", typeof(ViaDefinitionRule))]
        //Mask
        //[XmlArrayItem("MaskExpansion", typeof(MaskExpansionRule))]
        //Manufacturing
        // [XmlArrayItem("ManufacturingMinAnnularRing", typeof(ManufacturingMinAnnularRingRule))]
        [XmlArrayItem("manufacturingHoleSize", typeof(ManufacturingHoleSizeRule))]
        [XmlArrayItem("manufacturingClearance", typeof(ManufacturingClearanceRule))]
        //[XmlArrayItem("MatchLengths", typeof(MatchLengthsRule))]
        [XmlArrayItem("group", typeof(GroupRule))]
        public List<BoardRule> BoardRules { get; set; }

        [XmlElement("outputOptions")]
        public BoardOutputOptions OutputOptions { get; set; } = new BoardOutputOptions();

        public static List<Layer> CreateDefaultLayers()
        {

            //this is the default for a 2 layers board
            return new List<Layer>
            {
               Layer.GetTopCopperLayer(),
               Layer.GetBottomCopperLayer(),

               Layer.GetTopSilkscreenLayer(),
               Layer.GetBottomSilkscreenLayer(),

               Layer.GetTopPasteLayer(),
               Layer.GetBottomPasteLayer(),

               Layer.GetTopSolderLayer(),
               Layer.GetBottomSolderLayer(),

               //todo board outline on mechanichal 1
            };
        }

        public static List<BoardRule> CreateDefaultBoardRules()
        {
            return new List<BoardRule>
            {
                //Electrical
                new ElectricalClearanceRule(),
                //Routing
                new TrackWidthRule(),
                new ViaDefinitionRule(),
                //Mask
                //new MaskExpansionRule(),
                //Manufacturing
                //new ManufacturingMinAnnularRingRule(),
                new ManufacturingHoleSizeRule(),
                new ManufacturingClearanceRule(),
                //new MatchLengthsRule(),
                //new GroupRule()
            };
        }

        public static List<LayerPair> CreateDefaultDrillPairs()
        {
            return new List<LayerPair>
            {
                new LayerPair
                {
                    LayerIdStart = LayerConstants.SignalTopLayerId,
                    LayerIdEnd = LayerConstants.SignalBottomLayerId
                }
            };
        }


        public static List<LayerPair> CreateDefaultLayerPairs()
        {
            return new List<LayerPair>
            {
                new LayerPair
                {
                    LayerIdStart = LayerConstants.MechanicalTopLayerId,
                    LayerIdEnd = LayerConstants.MechanicalBottomLayerId
                }
            };
        }
    }

    public enum Units
    {
        mm,
        mil
    }



    public class BoardOutputOptions
    {
        #region NC Drill files
        public OutputUnits NCDrillUnits { get; set; } = OutputUnits.mm;
        public int NCDrillFormatBeforeDecimal { get; set; } = 4;
        public int NCDrillFormatAfterDecimal { get; set; } = 4;
        #endregion


        #region Gerber

        public OutputUnits GerberUnits { get; set; } = OutputUnits.mm;
        public int GerberFormatBeforeDecimal { get; set; } = 4;
        public int GerberFormatAfterDecimal { get; set; } = 6;
        public bool GerberPlotBoardOutlineOnAllLayers { get; set; } = true;
        public bool GerberCreateZipFile { get; set; } = true;

        public bool GerberWriteGerberMetadata { get; set; }
        public bool GerberWriteNetListAttributes { get; set; }

        public bool GerberCreateGerberAssemblyDrawings { get; set; }
        public bool GerberCreateGerberPickAndPlaceFiles { get; set; }

        #endregion

        [XmlElement("bom")]
        public BomOutputSpec Bom { get; set; } = new BomOutputSpec();

        [XmlElement("assembly")]
        public AssemblyOutputSpec Assembly { get; set; } = new AssemblyOutputSpec();
    }

    public class BomOutputSpec
    {
        [XmlArray("columns")]
        [XmlArrayItem("column")]
        public List<BomOutputColumn> Columns { get; set; } = new List<BomOutputColumn>();

        [XmlArray("groupBy")]
        [XmlArrayItem("column")]
        public List<BomOutputColumn> GroupColumns { get; set; } = new List<BomOutputColumn>();


    }

    public class AssemblyOutputSpec
    {
        [XmlElement("pickAndPlace")]
        public AssemblyOutputPickAndPlaceSpec PickAndPlace { get; set; } = new AssemblyOutputPickAndPlaceSpec();

        [XmlElement("drawings")]
        public AssemblyOutputDrawingsSpec Drawings { get; set; } = new AssemblyOutputDrawingsSpec();
    }

    public class AssemblyOutputPickAndPlaceSpec
    {
        [XmlAttribute("separator")]
        public string Separator { get; set; } = ",";

        [XmlAttribute("units")]
        public OutputUnits Units { get; set; } = OutputUnits.mm;

        [XmlArray("columns")]
        [XmlArrayItem("column")]
        public List<AssemblyOutputColumn> Columns { get; set; } = new List<AssemblyOutputColumn>();
    }

    public class AssemblyOutputDrawingsSpec
    {
        [XmlArray("layers")]
        [XmlArrayItem("layer")]
        public List<AssemblyDrawingOutputLayer> Layers { get; set; } = new List<AssemblyDrawingOutputLayer>();
    }

    public class BomOutputColumn : BaseViewModel
    {
        bool show = true;

        [XmlAttribute("show")]
        public bool Show
        {
            get { return show; }
            set
            {
                show = value;
                OnPropertyChanged(nameof(Show));
            }
        }

        [XmlAttribute("name")]
        public string ColumnName { get; set; }
    }

    public class AssemblyOutputColumn : BaseViewModel
    {
        bool show = true;

        [XmlAttribute("show")]
        public bool Show
        {
            get { return show; }
            set
            {
                show = value;
                OnPropertyChanged(nameof(Show));
            }
        }

        [XmlAttribute("name")]
        public string ColumnName { get; set; }

        string header;
        [XmlAttribute("header")]
        public string Header
        {
            get { return header; }
            set
            {
                header = value;
                OnPropertyChanged(nameof(Header));
            }
        }
    }

    public class AssemblyDrawingOutputLayer : BaseViewModel
    {
        [XmlAttribute("layerId")]
        public int LayerId { get; set; }


        bool plot = true;

        [XmlAttribute("plot")]
        public bool Plot
        {
            get { return plot; }
            set
            {
                plot = value;
                OnPropertyChanged(nameof(Plot));
            }
        }


        [XmlIgnore]
        public ILayerDesignerItem Layer { get; set; }
    }

}
