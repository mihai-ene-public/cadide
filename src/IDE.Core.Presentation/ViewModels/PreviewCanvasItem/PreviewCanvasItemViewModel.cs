using IDE.Core;
using IDE.Core.Designers;
using System.Collections.Generic;
using IDE.Core.Interfaces;
using IDE.Core.Storage;
using System;
using System.Linq;
using IDE.Core.Types.Media;
using IDE.Core.Common;

namespace IDE.Documents.Views
{
    //public class PreviewCanvasItemViewModel : BaseViewModel
    //{

    //    protected DrawingViewModel canvasModel;
    //    public DrawingViewModel CanvasModel
    //    {
    //        get
    //        {
    //            return canvasModel;
    //        }
    //        set
    //        {
    //            if (canvasModel != value)
    //            {
    //                canvasModel = value;
    //                OnPropertyChanged(nameof(CanvasModel));
    //            }
    //        }
    //    }

    //    DesignerViewMode designerViewMode = DesignerViewMode.ViewMode2D;
    //    public DesignerViewMode DesignerViewMode
    //    {
    //        get { return designerViewMode; }
    //        set
    //        {
    //            designerViewMode = value;
    //            OnPropertyChanged(nameof(DesignerViewMode));
    //        }
    //    }

    //    public void SetPrimitives(List<ISelectableItem> primitives)
    //    {
    //        LoadPrimitives(primitives);
    //    }

    //    private void LoadPrimitives(IList<ISelectableItem> primitives)
    //    {
    //        canvasModel.Items = primitives;
    //    }

    //    public void ZoomToFit()
    //    {
    //        canvasModel.ZoomToFit();
    //    }

    //    public void PreviewDocument(LibraryItem libraryItem, ISolutionProjectNodeModel projectModel)
    //    {
    //        switch (libraryItem)
    //        {


    //            case Symbol symbol:
    //                PreviewSymbolDocument(symbol);
    //                break;

    //            case Footprint footprint:
    //                PreviewFootprintDocument(footprint);
    //                break;

    //            case ModelDocument model:
    //                PreviewModelDocument(model);
    //                break;

    //            //case ComponentDocument comp:
    //            //    PreviewComponentDocument(comp);
    //            //    break;

    //            case SchematicDocument schematic:
    //                PreviewSchematicDocument(schematic, projectModel);
    //                break;
    //        }
    //    }

    //    private void PreviewSchematicDocument(SchematicDocument schematicDocument, ISolutionProjectNodeModel projectModel)
    //    {

    //        if (schematicDocument.Sheets != null)
    //        {

    //            var netsCache = new List<NetDesignerItem>();

    //            var sheets = new List<SheetDesignerItem>();

    //            foreach (var sheet in schematicDocument.Sheets)
    //            {
    //                var sheetItem = new SheetDesignerItem { Name = sheet.Name };


    //                #region Plain

    //                //plain (basic primitives)
    //                if (sheet.PlainItems != null)
    //                {
    //                    foreach (var primitive in sheet.PlainItems)
    //                    {
    //                        var canvasItem = primitive.CreateDesignerItem();
    //                        sheetItem.Items.Add(canvasItem);
    //                    }
    //                }

    //                #endregion Plain

    //                #region Components

    //                if (sheet.Instances != null && schematicDocument.Parts != null)
    //                {
    //                    foreach (var instance in sheet.Instances)
    //                    {
    //                        var part = schematicDocument.Parts.FirstOrDefault(p => p.Id == instance.PartId);
    //                        if (part != null)
    //                        {
    //                            var symbolItem = new SchematicSymbolCanvasItem();
    //                            symbolItem.ProjectModel = projectModel;
    //                            symbolItem.Part = part;
    //                            symbolItem.Primitive = instance;
    //                            sheetItem.Items.Add(symbolItem);
    //                        }

    //                    }
    //                }

    //                #endregion Components

    //                #region Nets

    //                //nets
    //                if (sheet.Nets != null)
    //                {
    //                    foreach (var netDoc in sheet.Nets)
    //                    {
    //                        //lookup the reference
    //                        var net = netsCache.FirstOrDefault(n => n.Id == netDoc.Id);

    //                        if (net == null)
    //                        {
    //                            net = new NetDesignerItem
    //                            {
    //                                Id = netDoc.Id,
    //                                ClassId = netDoc.ClassId,
    //                                Name = netDoc.Name,
    //                                CanvasModel = CanvasModel
    //                            };

    //                            netsCache.Add(net);
    //                        }


    //                        //foreach (var segment in netDoc.Segments)
    //                        //{
    //                        if (netDoc.Items != null)
    //                        {
    //                            foreach (var primitive in netDoc.Items)
    //                            {
    //                                if (primitive is PinRef pinRef)
    //                                {
    //                                    var part = sheetItem.Items.OfType<SchematicSymbolCanvasItem>()
    //                                                              .FirstOrDefault(p => p.SymbolPrimitive.Id == pinRef.PartInstanceId);
    //                                    if (part != null)
    //                                    {
    //                                        var pin = part.Pins.FirstOrDefault(p => p.Number == pinRef.Pin);
    //                                        if (pin != null)
    //                                            pin.Net = net;
    //                                    }
    //                                }
    //                                else
    //                                {
    //                                    var canvasItem = primitive.CreateDesignerItem();
    //                                    if (canvasItem is NetSegmentCanvasItem)
    //                                    {
    //                                        (canvasItem as NetSegmentCanvasItem).Net = net;
    //                                    }
    //                                    sheetItem.Items.Add(canvasItem);

    //                                }

    //                            }
    //                        }

    //                        //}
    //                    }
    //                }

    //                #endregion Nets

    //                sheets.Add(sheetItem);
    //            }

    //            var firstSheet = sheets.FirstOrDefault();
    //            if (firstSheet == null)
    //                return;

    //            LoadPrimitives(firstSheet.Items);
    //        }

    //    }

    //    private void PreviewSymbolDocument(Symbol symbol)
    //    {
    //        var primitiveItems = symbol.GetDesignerPrimitiveItems();

    //        LoadPrimitives(primitiveItems);
    //    }

    //    private void PreviewFootprintDocument(Footprint footprint)
    //    {
    //        var layeredDoc = LoadFootprintLayers(footprint);

    //        var primitives = new List<ISelectableItem>();
    //        foreach (var primitive in footprint.Items)
    //        {
    //            var canvasItem = (BoardCanvasItemViewModel)primitive.CreateDesignerItem();
    //            canvasItem.LayerDocument = layeredDoc;
    //            canvasItem.LoadLayers();
    //            primitives.Add(canvasItem);
    //        }

    //        LoadPrimitives(primitives);
    //    }

    //    ILayeredViewModel LoadFootprintLayers(Footprint fp)
    //    {
    //        //load layers
    //        var layeredDoc = new GenericLayeredViewModel();
    //        IList<Layer> layers = null;
    //        if (fp.Layers != null && fp.Layers.Count > 0)
    //        {
    //            layers = fp.Layers;
    //        }
    //        else
    //        {
    //            layers = IDE.Core.Storage.Footprint.CreateDefaultLayers();
    //        }
    //        fp.Layers = layers.ToList();
    //        var groups = LayerGroup.GetLayerGroupDefaults(layers);

    //        var layerItems = layers.Select(l => new LayerDesignerItem(layeredDoc)
    //        {
    //            LayerName = l.Name,
    //            LayerId = l.Id,
    //            LayerType = l.Type,
    //            LayerColor = XColor.FromHexString(l.Color)
    //        }).ToList();
    //        layeredDoc.LayerItems.Clear();
    //        layeredDoc.LayerItems.AddRange(layerItems);

    //        return layeredDoc;
    //    }

    //    private void PreviewModelDocument(ModelDocument modelDocument)
    //    {
    //        //switch to 3D
    //        //throw new NotImplementedException();
    //        var primitives = new List<ISelectableItem>();
    //        if (modelDocument.Items != null)
    //        {
    //            foreach (var primitive in modelDocument.Items)
    //            {
    //                var canvasItem = primitive.CreateDesignerItem();
    //                primitives.Add(canvasItem);
    //            }
    //        }

    //        LoadPrimitives(primitives);
    //        DesignerViewMode = DesignerViewMode.ViewMode3D;
    //        canvasModel.OnPropertyChanged(nameof(canvasModel.Items));
    //    }

    //    //private void PreviewComponentDocument(ComponentDocument comp)
    //    //{
    //    //    //for now, we have nothing here
    //    //}
    //}
}
