using IDE.Core.Types.Media;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace IDE.Core.Interfaces
{
    public interface ITextCanvasItem : ITextBaseCanvasItem
    {


        double Width { get; set; }

        string FontFamily { get; }



        string Text { get; }

        bool Italic { get; }

        bool Bold { get; }
    }

    public interface ITextSchematicCanvasItem : ITextCanvasItem
    {
        XColor TextColor { get; set; }
    }

    public interface ITextBaseCanvasItem : ISelectableItem
    {
        double X { get; set; }

        double Y { get; set; }

        double Rot { get; set; }

        double FontSize { get; }
    }

    public interface IGlobalTextPrimitive
    {
        bool Bold { get; set; }
        string FontFamily { get; set; }
        double FontSize { get; set; }
        bool Italic { get; set; }
        double Rot { get; set; }
        string Text { get; set; }
        double Width { get; set; }
        double X { get; set; }
        double Y { get; set; }
    }
}
