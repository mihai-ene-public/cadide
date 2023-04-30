using IDE.Core.Interfaces;
using IDE.Core.Storage;
using IDE.Core.Common;
using System.Collections.Generic;
using IDE.Core.Designers;
using System.Linq;
using IDE.Core.Types.Media;
using IDE.Core;
using IDE.Core.Units;

namespace IDE.Documents.Views;

public class PreviewFootprintViewModel : PreviewLibraryItemViewModel
{
    public PreviewFootprintViewModel()
    {
        var docSize = 25.4 * 10;
        var halfSize = docSize * 0.5;
        DocumentWidth = docSize;
        DocumentHeight = docSize;
        Origin = new XPoint(halfSize, halfSize);

        canvasGrid.SetUnit(new MillimeterUnit(0.1));
    }

    public override void PreviewDocument(LibraryItem libraryItem)
    {
        if (libraryItem is Footprint footprint)
            PreviewFootprintDocument(footprint);
    }

    private void PreviewFootprintDocument(Footprint footprint)
    {
        var layeredDoc = LoadFootprintLayers(footprint);

        var primitives = new List<ISelectableItem>();
        foreach (var primitive in footprint.Items)
        {
            var canvasItem = (BoardCanvasItemViewModel)primitive.CreateDesignerItem();
            canvasItem.LayerDocument = layeredDoc;
            canvasItem.LoadLayers();
            primitives.Add(canvasItem);
        }

        LoadPrimitives(primitives);
    }

    ILayeredViewModel LoadFootprintLayers(Footprint fp)
    {
        //load layers
        var layeredDoc = new GenericLayeredViewModel();
        IList<Layer> layers = null;
        if (fp.Layers != null && fp.Layers.Count > 0)
        {
            layers = fp.Layers;
        }
        else
        {
            layers = Footprint.CreateDefaultLayers();
        }
        fp.Layers = layers.ToList();
        var groups = LayerGroup.GetLayerGroupDefaults(layers);

        var layerItems = layers.Select(l => new LayerDesignerItem(layeredDoc)
        {
            LayerName = l.Name,
            LayerId = l.Id,
            LayerType = l.Type,
            LayerColor = XColor.FromHexString(l.Color)
        }).ToList();
        layeredDoc.LayerItems.Clear();
        layeredDoc.LayerItems.AddRange(layerItems);

        return layeredDoc;
    }
}
