using IDE.Core.Interfaces;
using IDE.Core.Storage;
using IDE.Core.Common;
using System.Collections.Generic;
using IDE.Core.Designers;
using System.Linq;
using IDE.Core.Units;

namespace IDE.Documents.Views;

public class PreviewSchematicViewModel : PreviewLibraryItemViewModel
{
    public PreviewSchematicViewModel()
    {
        ( (CanvasGrid)canvasModel.CanvasGrid ).GridSizeModel.SelectedItem = new MilUnit(50);
    }

    public override void PreviewDocument(LibraryItem libraryItem)
    {
        if (libraryItem is SchematicDocument schematic)
            PreviewSchematicDocument(schematic);
    }

    private void PreviewSchematicDocument(SchematicDocument schematicDocument)
    {

        if (schematicDocument.Sheets != null)
        {

            var netsCache = new List<SchematicNet>();

            var sheets = new List<SheetDesignerItem>();

            foreach (var sheet in schematicDocument.Sheets)
            {
                var sheetItem = new SheetDesignerItem { Name = sheet.Name };


                #region Plain

                //plain (basic primitives)
                if (sheet.PlainItems != null)
                {
                    foreach (var primitive in sheet.PlainItems)
                    {
                        var canvasItem = primitive.CreateDesignerItem();
                        sheetItem.Items.Add(canvasItem);
                    }
                }

                #endregion Plain

                #region Components

                if (sheet.Instances != null && schematicDocument.Parts != null)
                {
                    foreach (var instance in sheet.Instances)
                    {
                        var part = schematicDocument.Parts.FirstOrDefault(p => p.Id == instance.PartId);
                        if (part != null)
                        {
                            var symbolItem = new SchematicSymbolCanvasItem() { _Project = _project };
                            symbolItem.Part = part;
                            symbolItem.LoadFromPrimitive(instance);
                            sheetItem.Items.Add(symbolItem);
                        }

                    }
                }

                #endregion Components

                #region Nets

                //nets
                if (sheet.Nets != null)
                {
                    foreach (var netDoc in sheet.Nets)
                    {
                        //lookup the reference
                        var net = netsCache.FirstOrDefault(n => n.Id == netDoc.Id);

                        if (net == null)
                        {
                            net = new SchematicNet
                            {
                                Id = netDoc.Id,
                                ClassId = netDoc.ClassId,
                                Name = netDoc.Name,
                            };

                            netsCache.Add(net);
                        }


                        //foreach (var segment in netDoc.Segments)
                        //{
                        if (netDoc.Items != null)
                        {
                            foreach (var primitive in netDoc.Items)
                            {
                                if (primitive is PinRef pinRef)
                                {
                                    var part = sheetItem.Items.OfType<SchematicSymbolCanvasItem>()
                                                              .FirstOrDefault(p => p.SymbolPrimitive.Id == pinRef.PartInstanceId);
                                    if (part != null)
                                    {
                                        var pin = part.Pins.FirstOrDefault(p => p.Number == pinRef.Pin);
                                        if (pin != null)
                                            pin.Net = net;
                                    }
                                }
                                else
                                {
                                    var canvasItem = primitive.CreateDesignerItem();
                                    if (canvasItem is NetSegmentCanvasItem)
                                    {
                                        ( canvasItem as NetSegmentCanvasItem ).Net = net;
                                    }
                                    sheetItem.Items.Add(canvasItem);

                                }

                            }
                        }

                        //}
                    }
                }

                #endregion Nets

                sheets.Add(sheetItem);
            }

            var firstSheet = sheets.FirstOrDefault();
            if (firstSheet == null)
                return;

            LoadPrimitives(firstSheet.Items);
        }

    }

}
