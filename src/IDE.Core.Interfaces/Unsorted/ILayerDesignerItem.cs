using IDE.Core.Types.Media;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace IDE.Core.Interfaces
{
    public interface ILayerDesignerItem: ISelectableItem
    {
        int LayerId { get; set; }

        int StackOrder { get; set; }

        XColor LayerColor { get; set; }

        XColor DimmedColor { get;  }

        string LayerName { get; set; }

        LayerType LayerType { get; set; }

        bool Plot { get; set; }

        bool MirrorPlot { get; set; }

        string GerberFileName { get; set; }

        string GerberExtension { get; set; }

        string Material { get; set; }

        double Thickness { get; set; }

        double DielectricConstant { get; set; }

        bool IsVisible { get; set; }

        bool IsCurrentLayer { get; set; }

        bool IsBottomLayer { get; }
        bool IsTopLayer { get; }

        IList<ISelectableItem> Items { get; set; }

        void HandleLayer();
    }
}
