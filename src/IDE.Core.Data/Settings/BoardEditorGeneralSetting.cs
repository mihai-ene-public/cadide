using IDE.Core.Storage;
using IDE.Core.Types.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;

namespace IDE.Core.Settings
{
    public class BoardEditorGeneralSetting : BasicSetting
    {
        public bool OnlineDRC { get; set; } = true;

        public bool RepourPolysOnDocumentChange { get; set; } = true;
        public bool ShowUnconnectedSignals { get; set; } = true;

    }

    public class BoardEditorColorsSetting : BasicSetting
    {
        public string CanvasBackground { get; set; } = "#FF616161";

        public string GridColor { get; set; } = "#FF808080";

        //GridStyle: Lines/Dots
        //LayerColors

    }

    public class BoardEditorRoutingSetting : BasicSetting
    {
        public bool IgnoreObstacles { get; set; } = true;
        public bool PushObstacles { get; set; } = true;
        public bool WalkAroundObstacles { get; set; } = true;
        public bool StopAtFirstObstacle { get; set; } = true;

    }

    public class BoardEditorPrimitiveDefaults:BasicSetting
    {
        public BoardEditorPrimitiveDefaults()
        {

        }

        public BoardEditorPrimitiveDefaults(bool createDefaults)
        {
            if (createDefaults)
                CreateDefaultPrimitives();
        }


        [XmlArray("primitives")]
        [XmlArrayItem("circle", typeof(CircleBoard))]
        [XmlArrayItem("polygon", typeof(PolygonBoard))]
        [XmlArrayItem("rectangle", typeof(RectangleBoard))]
        [XmlArrayItem("text", typeof(TextBoard))]
        [XmlArrayItem("mono", typeof(TextSingleLineBoard))]
        [XmlArrayItem("wire", typeof(LineBoard))]
        [XmlArrayItem("arc", typeof(ArcBoard))]
        [XmlArrayItem("hole", typeof(Hole))]
        [XmlArrayItem("pad", typeof(Pad))]
        [XmlArrayItem("smd", typeof(Smd))]
        public List<LayerPrimitive> Primitives { get; set; } = new List<LayerPrimitive>();

        public T GetPrimitive<T>() where T : LayerPrimitive
        {
            return Primitives.OfType<T>().FirstOrDefault();
        }

        void CreateDefaultPrimitives()
        {
            Primitives = new List<LayerPrimitive>
            {
                new ArcBoard
                {
                     BorderWidth = 0.2,
                     SizeDiameter = 2,
                },
                new CircleBoard
                {
                     BorderWidth = 0.5,
                },
                new LineBoard
                {
                    width = 0.2
                },
                new TextBoard
                {
                    TextAlign = XTextAlignment.Center,
                    TextDecoration = TextDecorationEnum.None,
                    FontSize = 24,
                    FontFamily = "Segoe UI",
                    Value = "Text"
                },
                new RectangleBoard
                {
                    BorderWidth = 0.5,
                }
                ,
                new PolygonBoard
                {
                    BorderWidth = 0.5,
                },
                new Pad
                {
                    Width = 0.5,
                    Height=0.6
                },
                new Smd
                {
                   Width = 0.5,
                    Height=0.6
                },
                //Junction?
                //NetLabel
                //NetWire
            };
        }

    }
}
