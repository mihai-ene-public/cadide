using IDE.Core.Interfaces;
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
    public class SchematicEditorColorsSetting : BasicSetting
    {
        public string CanvasBackground { get; set; } = "#FF616161";

        public string GridColor { get; set; } = "#FF808080";

        //GridStyle: Lines/Dots

    }

    public class SchematicEditorPrimitiveDefaults : BasicSetting
    {
        public SchematicEditorPrimitiveDefaults()
        {

        }

        public SchematicEditorPrimitiveDefaults(bool createDefaults)
        {
            if (createDefaults)
                CreateDefaultPrimitives();
        }

        [XmlArray("primitives")]
        [XmlArrayItem("circle", typeof(Circle))]
        [XmlArrayItem("ellipse", typeof(Ellipse))]
        [XmlArrayItem("image", typeof(ImagePrimitive))]
        [XmlArrayItem("polygon", typeof(Polygon))]
        [XmlArrayItem("rectangle", typeof(Rectangle))]
        [XmlArrayItem("text", typeof(Text))]
        [XmlArrayItem("line", typeof(LineSchematic))]
        [XmlArrayItem("arc", typeof(Arc))]
        [XmlArrayItem("pin", typeof(Pin))]
        public List<SchematicPrimitive> Primitives { get; set; } = new List<SchematicPrimitive>();

        public T GetPrimitive<T>() where T : SchematicPrimitive
        {
            return Primitives.OfType<T>().FirstOrDefault();
        }

        void CreateDefaultPrimitives()
        {
            Primitives = new List<SchematicPrimitive>
            {
                new Arc
                {
                     BorderWidth = 0.5,
                     BorderColor = "#FF000080",
                     FillColor = "#00FFFFFF",
                     Size = new XSize(2,2),
                },
                new Circle
                {
                     BorderWidth = 0.5,
                     BorderColor = "#FF000080",
                     FillColor = "#00FFFFFF"
                },
                new Ellipse
                {
                    BorderWidth = 0.5,
                    BorderColor = "#FF000080",
                    FillColor = "#00FFFFFF"
                },
                new LineSchematic
                {
                    lineStyle = LineStyle.Solid,
                    LineColor = "#FF000080",
                    width = 0.5
                },
                new Text
                {
                    TextAlign = XTextAlignment.Center,
                    TextDecoration = TextDecorationEnum.None,
                    textColor = "#FFFFFF",
                    backgroundColor = "#00FFFFFF",
                    FontSize = 24,
                    FontFamily = "Segoe UI",
                    Value = "Text"
                },
                new Rectangle
                {
                    BorderWidth = 0.5,
                    BorderColor = "#FF000080",
                    FillColor = "#00FFFFFF"
                }
                ,
                new Polygon
                {
                    BorderWidth = 0.5,
                    BorderColor = "#FF000080",
                    FillColor = "#00FFFFFF"
                },
                new Pin
                {
                    PinLength = 3,
                    Width = 0.5,
                    pinType = PinType.Passive,
                    Orientation = pinOrientation.Right,
                    PinColor = "#FF000080",
                    PinNameColor = "#FF000080",
                    PinNumberColor = "#FF000080"
                },
                new ImagePrimitive
                {
                    BorderWidth = 0.5,
                    BorderColor = "#FF000080",
                    FillColor = "#00FFFFFF",
                    CornerRadius = 0
                }
                //Junction?
                //NetLabel
                //NetWire
            };
        }

    }
}
