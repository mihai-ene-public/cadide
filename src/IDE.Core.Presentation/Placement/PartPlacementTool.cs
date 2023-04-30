using System.Linq;
using IDE.Core.Designers;
using IDE.Core.Interfaces;
using IDE.Core.Storage;
using IDE.Core.Types.Media;
using IDE.Documents.Views;

namespace IDE.Core.Presentation.Placement;

public class PartPlacementTool : PlacementTool, IPartPlacementTool
{
    SchematicSymbolCanvasItem GetItem() => canvasItem as SchematicSymbolCanvasItem;

    public override bool Show()
    {
        var itemSelectDlg = new ItemSelectDialogViewModel(TemplateType.Component, CanvasModel.GetCurrentProjectInfo());

        if (itemSelectDlg.ShowDialog() == true)
        {
            var componentItemDisplay = itemSelectDlg.SelectedItem;
            if (componentItemDisplay != null)
            {
                dialogSelectedItem = componentItemDisplay;
                return true;
            }
        }

        return false;
    }

    public override void SetupCanvasItem()
    {
        var partInstance = canvasItem as SchematicSymbolCanvasItem;
        var placedComponent = dialogSelectedItem as ComponentItemDisplay;

        var compDoc = placedComponent.Document as ComponentDocument;

        if (!string.IsNullOrEmpty(compDoc.Comment))
        {
            partInstance.Comment = compDoc.Comment;
            partInstance.ShowComment = true;
        }

        if (string.IsNullOrEmpty(compDoc.Prefix))
            compDoc.Prefix = "P";
        var namedParts = CanvasModel.Items.OfType<SchematicSymbolCanvasItem>()
                                          .Where(p => p.PartName != null && p.PartName.StartsWith(compDoc.Prefix))
                                          .Select(p => p.PartName).ToList();

        var partName = partInstance.GetNextPartName(namedParts, compDoc.Prefix);

        partInstance._Project = CanvasModel.GetCurrentProjectInfo();

        partInstance.Part = new Part
        {
            ComponentId = compDoc.Id,
            ComponentLibrary = compDoc.Library,
            ComponentName = compDoc.Name,
            Id = LibraryItem.GetNextId(),
            Name = partName,
        };
        partInstance.LoadFromPrimitive( new Instance
        {
            Id = LibraryItem.GetNextId(),
            PartId = partInstance.Part.Id,
            //first gate
            GateId = (compDoc.Gates?.Select(g => g.Id).FirstOrDefault()).GetValueOrDefault()
        });
        partInstance.IsPlaced = false;
    }

    public override void PlacementMouseMove(XPoint mousePosition)
    {
        var mp = CanvasModel.SnapToGrid(mousePosition);

        var item = GetItem();

        switch (PlacementStatus)
        {
            case PlacementStatus.Ready:
                item.X = mp.X;
                item.Y = mp.Y;
                break;
        }
    }

    public override void PlacementMouseUp(XPoint mousePosition)
    {
        var mp = CanvasModel.SnapToGrid(mousePosition);

        var item = GetItem();

        switch (PlacementStatus)
        {
            case PlacementStatus.Ready:
                item.X = mp.X;
                item.Y = mp.Y;
                item.IsPlaced = true;
                CommitPlacement();

                //create another
                var newItem = (SchematicSymbolCanvasItem)canvasItem.Clone();

                //same part by default
                newItem.Part = item.Part;

                var componentDocument = item.ComponentDocument;

                //comment
                if (!string.IsNullOrEmpty(componentDocument.Comment))
                {
                    item.Comment = componentDocument.Comment;
                    item.ShowComment = true;
                }

                var currentGate = componentDocument.Gates.FirstOrDefault(g => g.Id == (item.SymbolPrimitive).GateId);
                var nextGateIndex = (componentDocument.Gates.IndexOf(currentGate) + 1) % componentDocument.Gates.Count;
                //another part if we overflow gates
                if (nextGateIndex == 0)
                {
                    if (string.IsNullOrEmpty(componentDocument.Prefix))
                        componentDocument.Prefix = "P";

                    var partName = GetNextPartName(item);


                    newItem.Part = new Part
                    {
                        ComponentId = componentDocument.Id,
                        ComponentLibrary = componentDocument.Library,
                        ComponentName = componentDocument.Name,
                        Id = LibraryItem.GetNextId(),
                        //CachedComponent = componentDocument,
                        Name = partName,
                    };
                }
                newItem.LoadFromPrimitive( new Instance
                {
                    Id = LibraryItem.GetNextId(),
                    PartId = newItem.Part.Id,
                    GateId = componentDocument.Gates[nextGateIndex].Id,
                    Rot = item.Rot,
                    ScaleX = item.ScaleX,
                    ScaleY = item.ScaleY
                });
                newItem.IsPlaced = false;

                PlacementStatus = PlacementStatus.Ready;
                canvasItem = newItem;
                CanvasModel.AddItem(canvasItem);
                break;
        }
    }

    public string GetNextPartName(SchematicSymbolCanvasItem schematicPart)
    {
        var componentDocument = schematicPart.ComponentDocument;
        if (string.IsNullOrEmpty(componentDocument.Prefix))
            componentDocument.Prefix = "P";
        var namedParts = CanvasModel.Items.OfType<SchematicSymbolCanvasItem>()
                             .Where(p => p.PartName != null && p.PartName.StartsWith(componentDocument.Prefix))
                             .Select(p => p.PartName).ToList();

        return schematicPart.GetNextPartName(namedParts, componentDocument.Prefix);
    }
}
