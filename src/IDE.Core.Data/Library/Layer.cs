using IDE.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IDE.Core.Storage
{
    //we could split this in enums for each layer type
    public class LayerConstants
    {
        //Signals
        public const int SignalTopLayerId = 1;
        //Middle layers
        //...
        public const int SignalBottomLayerId = 99;//  isSignal = (id / 100 == 0)

        ////Plane
        //public const int PlaneLayerId1 = 101;
        //public const int PlaneLayerId2 = 102;
        //public const int PlaneLayerId3 = 103;
        //public const int PlaneLayerId4 = 104;
        //public const int PlaneLayerId5 = 105;
        //public const int PlaneLayerId6 = 106;
        //public const int PlaneLayerId7 = 107;
        //public const int PlaneLayerId8 = 108;
        ////..
        //public const int PlaneLayerId16 = 116;

        //Mechanical
        public const int MechanicalTopLayerId = 201;
        //..
        public const int MechanicalBottomLayerId = 299;


        //Paste Mask
        public const int PasteTopLayerId = 301;
        public const int PasteBottomLayerId = 399;

        //Solder Mask
        public const int SolderTopLayerId = 401;
        public const int SolderBottomLayerId = 499;

        //Silkscreen
        public const int SilkscreenTopLayerId = 501;
        public const int SilkscreenBottomLayerId = 599;

        //Multilayer
        public const int MultiLayerMillingId = 701;

        public const int BoardOutline = 1001;

        static readonly Dictionary<int, int> LayerPairs = new Dictionary<int, int>
                    {
                        {SignalTopLayerId, SignalBottomLayerId },
                        {PasteTopLayerId,  PasteBottomLayerId },
                        {SolderTopLayerId, SolderBottomLayerId },
                        {SilkscreenTopLayerId, SilkscreenBottomLayerId },
                        {MechanicalTopLayerId, MechanicalBottomLayerId },
                    };

        public static int GetPairedLayer(int currentLayer, FootprintPlacement placementToBe)
        {
            var isTopOrBottom = LayerPairs.ContainsKey(currentLayer)
                             || LayerPairs.ContainsValue(currentLayer);
            if (isTopOrBottom)
            {
                var isTopLyer = LayerPairs.ContainsKey(currentLayer);
                var isBottom = LayerPairs.ContainsValue(currentLayer);

                if (isTopLyer)
                {
                    return placementToBe == FootprintPlacement.Top ? currentLayer : LayerPairs[currentLayer];
                }
                if (isBottom)
                {
                    return placementToBe == FootprintPlacement.Bottom ? currentLayer : LayerPairs.FirstOrDefault(kvp => kvp.Value == currentLayer).Key;
                }
            }

            throw new NotSupportedException();
        }

        //public static bool IsTopLayer(int layerId)
        //{
        //   return LayerPairs.ContainsKey(layerId);
        //}

        //public static bool IsBottomLayer(int layerId)
        //{
        //    return LayerPairs.ContainsValue(layerId);
        //}

        public static int GetCompanionLayer(int signalLayer, LayerType layerType)
        {
            var signalNumber = signalLayer % ((int)LayerType.Signal + 100);

            return signalNumber + (int)layerType;
        }
    }

    /// <summary>
    /// a layer stored in a board or footprint, etc
    /// </summary>
    public class Layer
    {

        public Layer()
        {
            // IsVisible = true;
        }

        [XmlAttribute("id")]
        public int Id { get; set; }

        [XmlAttribute("stackOrder")]
        public int StackOrder { get; set; }

        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("layerType")]
        //layer type
        public LayerType Type { get; set; }

        [XmlAttribute("color")]
        public string Color { get; set; }

        [XmlAttribute("material")]
        public string Material { get; set; }

        [XmlAttribute("thickness")]
        public double Thickness { get; set; } = 0.01;

        [XmlAttribute("dielectricConstant")]
        public double DielectricConstant { get; set; }

        [XmlAttribute("plot")]
        public bool Plot { get; set; }

        [XmlAttribute("mirrorPlot")]
        public bool MirrorPlot { get; set; }

        [XmlAttribute("gerberFileName")]
        public string GerberFileName { get; set; }

        [XmlAttribute("gerberExtension")]
        public string GerberExtension { get; set; } = "GBR";


        //[XmlAttribute("visible")]
        //public bool IsVisible { get; set; }

        #region Signal Layers
        public static Layer GetTopCopperLayer()
        {
            return new Layer
            {
                Id = LayerConstants.SignalTopLayerId,
                Name = "Top",
                Color = "#FFA52A2A",
                Type = LayerType.Signal,
                Plot = true,
                Thickness = 0.035,
            };
        }

        //the rest of layers go here

        public static Layer GetBottomCopperLayer()
        {
            return new Layer
            {
                Id = LayerConstants.SignalBottomLayerId,
                Name = "Bottom",
                Color = "#FF0000CD",
                Type = LayerType.Signal,
                Plot = true,
                Thickness = 0.035,
            };
        }

        #endregion

        public static Layer GetTopSilkscreenLayer()
        {
            return new Layer
            {
                Id = LayerConstants.SilkscreenTopLayerId,
                Name = "Top Silkscreen",
                Color = "#FFE1E100",
                Type = LayerType.SilkScreen,
                Plot = true,
                Thickness = 0.015,
                DielectricConstant = 3.2
            };
        }

        public static Layer GetBottomSilkscreenLayer()
        {
            return new Layer
            {
                Id = LayerConstants.SilkscreenBottomLayerId,
                Name = "Bottom Silkscreen",
                Color = "#FFFFD700",
                Type = LayerType.SilkScreen,
                Plot = true,
                Thickness = 0.015,
                DielectricConstant = 3.2
            };
        }

        public static Layer GetTopPasteLayer()
        {
            return new Layer
            {
                Id = LayerConstants.PasteTopLayerId,
                Name = "Top Paste",
                Color = "#FFAAAAAA",
                Type = LayerType.PasteMask,
                Plot = true,
                Thickness = 0.2,
            };
        }

        public static Layer GetMillingLayer()
        {
            return new Layer
            {
                Id = LayerConstants.MultiLayerMillingId,
                Name = "Milling",
                Color = "#80FFFFFF",
                Type = LayerType.MultiLayer,
                Plot = false,

            };
        }

        public static Layer GetDielectricLayer()
        {
            return new Layer
            {
                Id = 601,
                Name = "Dielectric",
                Color = "#FFD2691E",
                Type = LayerType.Dielectric,
                Thickness = 1
            };
        }

        public static Layer GetBoardOutlineLayer()
        {
            return new Layer
            {
                Id = LayerConstants.BoardOutline,
                Name = "Board Outline",
                Color = "#FFFFA500",
                Type = LayerType.BoardOutline,
                Thickness = 0
            };
        }

        public static Layer GetTopMechanicalLayer()
        {
            return new Layer
            {
                Id = LayerConstants.MechanicalTopLayerId,
                Name = "Mechanical Top",
                Color = "#80FFFFFF",
                Type = LayerType.Mechanical
            };
        }

        public static Layer GetBottomMechanicalLayer()
        {
            return new Layer
            {
                Id = LayerConstants.MechanicalBottomLayerId,
                Name = "Mechanical Bottom",
                Color = "#80FFFFFF",
                Type = LayerType.Mechanical
            };
        }

        public static Layer GetBottomPasteLayer()
        {
            return new Layer
            {
                Id = LayerConstants.PasteBottomLayerId,
                Name = "Bottom Paste",
                Color = "#FFC0C0C0",
                Type = LayerType.PasteMask,
                Plot = true,
                Thickness = 0.2,
            };
        }

        public static Layer GetTopSolderLayer()
        {
            return new Layer
            {
                Id = LayerConstants.SolderTopLayerId,
                Name = "Top Solder",
                Color = "#FF800080",
                Type = LayerType.SolderMask,
                Plot = true,
                Thickness = 0.01
            };
        }

        public static Layer GetBottomSolderLayer()
        {
            return new Layer
            {
                Id = LayerConstants.SolderBottomLayerId,
                Name = "Bottom Solder",
                Color = "#FF8F008F",
                Type = LayerType.SolderMask,
                Plot = true,
                Thickness = 0.01
            };
        }

        //todo: mewch drawing, top/bottom assembly (Mechanical
        //manuf notes (Generic)
        //board outline
    }

    public class LayerRef
    {
        [XmlAttribute("id")]
        public int Id { get; set; }
    }

    public class LayerGroup
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlElement("layer")]
        public List<LayerRef> Layers { get; set; }

        [XmlIgnore]
        public bool IsReadOnly { get; set; } = true;

        public static List<LayerGroup> GetLayerGroupDefaults(IList<Layer> layers)
        {
            return new List<LayerGroup>
            {
                new LayerGroup
                {
                    Name = "All Layers",
                    Layers = layers.Where(l=>l.Type != LayerType.Dielectric).Select(l=> new LayerRef {Id = l.Id }).ToList()
                },
                new LayerGroup
                {
                    Name = "Signal",
                    Layers = layers.Where(l=>l.Type == LayerType.Signal).Select(l=> new LayerRef {Id = l.Id }).ToList()
                }
            };
        }
    }

    public class LayerPair
    {
        [XmlAttribute("layerIdStart")]
        public int LayerIdStart { get; set; }

        [XmlAttribute("layerIdEnd")]
        public int LayerIdEnd { get; set; }
    }
}
