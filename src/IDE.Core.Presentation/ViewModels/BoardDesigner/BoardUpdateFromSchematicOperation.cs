using IDE.Core;
using IDE.Core.BOM;
using IDE.Core.Build;
using IDE.Core.Designers;
using IDE.Core.Excelon;
using IDE.Core.Gerber;
using IDE.Core.Interfaces;
using IDE.Core.PDF;
using IDE.Core.Presentation.ObjectFinding;
using IDE.Core.Presentation.Utilities;
using IDE.Core.Storage;
using IDE.Core.Types.Media;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;


namespace IDE.Documents.Views
{
    public class BoardUpdateFromSchematicOperation
    {
        public BoardUpdateFromSchematicOperation(IDispatcherHelper dispatcher, IObjectFinder objectFinder)
        {
            _dispatcher = dispatcher;
            _objectFinder = objectFinder;
        }

        private readonly IDispatcherHelper _dispatcher;
        private readonly IObjectFinder _objectFinder;

        public async Task Update(IBoardDesigner board, SchematicDocument schematic, ISolutionProjectNodeModel project)
        {
            //update parts (footprints)
            var components = await UpdatePartsFromSchematic(board, schematic, project);

            //update net list
            await UpdateNetList(board, schematic, components);

            //net classes
            UpdateNetClasses(board, schematic);

            //refresh signals for pads
            await Task.Run(() =>
            {
                foreach (var fp in board.CanvasModel.GetFootprints())
                {
                    //this will refresh the primitive (will save primitive so far: pos, rot, etc)
                    var p = fp.SaveToPrimitive();

                    fp.LoadFromPrimitive(p);
                }
            });

        }

        async Task<List<ComponentDocument>> UpdatePartsFromSchematic(IBoardDesigner board, SchematicDocument schematic, ISolutionProjectNodeModel project)
        {
            var canvasModel = board.CanvasModel;

            //there are refresh issues when adding new parts(footprints)
            var components = new List<ComponentDocument>();
            //an improvement for new parts is to place/group by position from schematic; it would help placement
            if (schematic.Parts != null && schematic.Sheets != null)
            {
                //footprints in board that are not in schematic
                var schPartIds = schematic.Parts.Select(p => p.Id).ToList();

                var footprintsToRemove = canvasModel.GetFootprints()
                                          .Where(p => p.FootprintPrimitive == null || !schPartIds.Contains(p.FootprintPrimitive.PartId))
                                          .ToList();

                //ParentProject.CreateCacheItems(TemplateType.Component);
                //ParentProject.CreateCacheItems(TemplateType.Footprint);

                //add footprints
                var footprintsToAdd = new List<FootprintBoardCanvasItem>();
                foreach (var part in schematic.Parts)
                {
                    //this footprintId could be taken from Component.FootprintId
                    //below is just the cached version which could be different after footprint is changed in the component
                    //we could have an update from libraries command
                    var compId = part.ComponentId;
                    //var component = await Task.Run(() => project.FindObject(TemplateType.Component, part.ComponentLibrary, compId) as Core.Storage.ComponentDocument);
                    var component =  _objectFinder.FindObject<ComponentDocument>(project.Project, part.ComponentLibrary, compId);

                    if (component != null)
                        components.Add(component);

                    //!if the component is not found, something is wrong
                    if (component != null && component.Footprint != null)
                    {
                        var fptLibName = component.Footprint.LibraryName;
                        if (string.IsNullOrEmpty(fptLibName) || fptLibName == "local")
                            component.Footprint.LibraryName = component.Library;

                        var footprintId = component.Footprint.footprintId;
                        var footprintLibrary = component.Footprint.LibraryName;

                        //var footprint = await Task.Run(() => ParentProject.FindObject(TemplateType.Footprint, component.Footprint.LibraryName, component.Footprint.footprintId) as Footprint);
                        ////compare some hashes between footprint on this board and from component
                        ////update with the new footprint if they are different
                        ////add the footprint if this does not exist in our board
                        //if (footprint == null || footprint.Id == 0)
                        //{
                        //    //todo record a message
                        //    continue;//next part
                        //}

                        var canvasItem = canvasModel.GetFootprints()
                                                      .FirstOrDefault(f => f.FootprintPrimitive != null && f.FootprintPrimitive.PartId == part.Id);
                        BoardComponentInstance fpInstance = null;


                        var addToCanvas = false;
                        if (canvasItem == null)
                        {
                            fpInstance = new BoardComponentInstance
                            {
                                PartId = part.Id,
                                PartName = part.Name,
                                ComponentId = component.Id,
                                ComponentLibrary = component.Library,
                                FootprintId = footprintId,
                                Library = footprintLibrary,
                                Id = LibraryItem.GetNextId(),//todo keep the old one or create a new one
                                x = 0,
                                y = 0,
                                PartNameX = 0,
                                PartNameY = 0,
                                Placement = FootprintPlacement.Top,
                            };

                            canvasItem = new FootprintBoardCanvasItem
                            {
                                ProjectModel = project,
                                BoardModel = board,
                            };

                            canvasItem.LoadFromPrimitive(fpInstance);

                            //calculate rectangle for all items
                            var rect = XRect.Empty;
                            foreach (var item in canvasItem.Items)
                            {
                                rect.Union(item.GetBoundingRectangle());
                            }

                            if (!rect.IsEmpty)
                                canvasItem.PartNamePosition.Y = 0.5 * rect.Height + 0.1;

                            addToCanvas = true;
                        }
                        else
                        {
                            fpInstance = (BoardComponentInstance)canvasItem.SaveToPrimitive();//FootprintPrimitive;

                            fpInstance.PartId = part.Id;
                            fpInstance.PartName = part.Name;
                            //fpInstance.CachedPart = part;
                            fpInstance.FootprintId = footprintId;
                            fpInstance.Library = footprintLibrary;

                            //force reload Primitive, pads, etc
                            canvasItem.LoadFromPrimitive(fpInstance);
                        }

                        if (addToCanvas)
                        {
                            footprintsToAdd.Add(canvasItem);
                        }
                    }

                }//foreach sch.Part

                //ParentProject.ClearCachedItems();

                //place footprints
                //this order list could be a setting
                var placementOrder = new[] { "U", "Q" };
                var outlineRectangle = board.BoardOutline.GetBoundingRectangle();//GeometryHelper.GetRegionGeometry(boardOutlineItem).Bounds;
                var margin = 5.0d;
                var footprintsToPlace = footprintsToAdd.ToList();
                var startX = outlineRectangle.X + outlineRectangle.Width + margin;
                var posY = margin;
                var posX = startX;

                while (footprintsToPlace.Count > 0)
                {
                    foreach (var currentPrefixToPlace in placementOrder)
                    {
                        var crtPrefixParts = footprintsToPlace.Where(p => p.PartName.StartsWith(currentPrefixToPlace)).ToList();
                        PositionParts(crtPrefixParts, startX, ref posX, ref posY);
                        crtPrefixParts.ForEach(p => footprintsToPlace.Remove(p));
                    }

                    //rest of parts
                    PositionParts(footprintsToPlace, startX, ref posX, ref posY);
                    footprintsToPlace.Clear();
                }

                _dispatcher.RunOnDispatcher(() =>
                {
                    canvasModel.RemoveItems(footprintsToRemove);
                    //add footprints
                    canvasModel.AddItems(footprintsToAdd);
                });
            }

            return components;
        }
        void PositionParts(List<FootprintBoardCanvasItem> parts, double startX, ref double posX, ref double posY)
        {
            var maxX = 250.0d;
            foreach (var p in parts)
            {
                p.X = posX;
                p.Y = posY;

                var partRect = p.GetBoundingRectangle();
                posX += partRect.Width + 1;
                if (posX > maxX)
                {
                    posX = startX;
                    posY += partRect.Height + 1;
                }

            }
        }

        async Task UpdateNetList(IBoardDesigner board, SchematicDocument schematic, IList<ComponentDocument> components)
        {
            if (schematic.Sheets == null)
                return;

            //this line is to remove the warning
            await Task.CompletedTask;

            var boardNets = new List<BoardNetDesignerItem>();

            #region Build Net list

            foreach (var sheet in schematic.Sheets)
            {
                if (sheet.Nets == null)
                    continue;
                foreach (var net in sheet.Nets)
                {
                    var boardNet = new BoardNetDesignerItem(board)
                    {
                        Id = net.Id,
                        Name = net.Name,
                        ClassId = net.ClassId,
                    };
                    boardNets.Add(boardNet);

                    //pad refs; these might be already in our model; for now we consider all is clear
                    if (net.Items != null)
                    {

                        foreach (var pin in net.Items.OfType<PinRef>())
                        {
                            var partInstance = (from s in schematic.Sheets
                                                from p in s.Instances
                                                select p).FirstOrDefault(p => p.Id == pin.PartInstanceId);
                            if (partInstance != null && schematic.Parts != null)
                            {
                                var part = schematic.Parts.FirstOrDefault(p => p.Id == partInstance.PartId);
                                if (part != null)
                                {
                                    // var solvedComp = await Task.Run(() => ParentProject.FindObject(TemplateType.Component, part.ComponentLibrary, part.ComponentId) as ComponentDocument);

                                    var compLibrary = part.ComponentLibrary;
                                    if (string.IsNullOrEmpty(compLibrary))
                                        compLibrary = "local";

                                    var solvedComp = components.FirstOrDefault(c => c.Library == compLibrary && c.Id == part.ComponentId);

                                    if (solvedComp != null && solvedComp.Footprint != null)
                                    {
                                        var connect = solvedComp.Footprint.Connects.FirstOrDefault(p => p.gateId == partInstance.GateId
                                                                                                               && p.pin == pin.Pin);
                                        var fpInstance = board.CanvasModel.GetFootprints()
                                                                        .FirstOrDefault(p => p.FootprintPrimitive.PartId == part.Id && p.FootprintPrimitive.FootprintId == solvedComp.Footprint.footprintId);

                                        if (connect != null && fpInstance != null)// && fpInstance.Pads != null)
                                        {
                                            var pad = fpInstance.Pads.FirstOrDefault(p => p.Number == connect.pad);

                                            if (pad != null)
                                            {
                                                _dispatcher.RunOnDispatcher(() => pad.Signal = boardNet);
                                            }

                                        }
                                    }

                                }
                            }
                        }
                    }
                }
            }
            #endregion

            //group by net name
            var groupedNets = boardNets.GroupBy(g => g.Name);
            //this will be the source for the new net list
            var retBoardNets = new List<BoardNetDesignerItem>();

            foreach (var g in groupedNets)
            {
                //take lowest net id
                //rebuild pad refs so that they are unique
                var net = new BoardNetDesignerItem(board)
                {
                    Id = g.Min(n => n.Id),
                    Name = g.Key,
                    ClassId = g.FirstOrDefault().ClassId,
                };
                retBoardNets.Add(net);

                var pads = new List<IPadCanvasItem>();
                foreach (var p in from n in g
                                  from pad in n.Pads
                                  select pad)
                {
                    var dupl = pads.FirstOrDefault(pp => pp.FootprintInstanceId == p.FootprintInstanceId
                                                      && pp.Number == p.Number);
                    if (dupl == null)
                        pads.Add(p);
                }
                _dispatcher.RunOnDispatcher(() => net.Pads.AddRange(pads));
            }

            //at this moment we have the new nets with pads
            //todo: we need to import the new nets, loose the old, but keep the routing
            var existingNets = board.NetList.ToList();
            board.NetList.Clear();

            //add the new nets, keep the routing
            foreach (var net in retBoardNets)
            {
                var existingNet = existingNets.FirstOrDefault(n => n.Name == net.Name || n.Id == net.Id);
                _dispatcher.RunOnDispatcher(() =>
                {
                    if (existingNet != null)
                        net.Items.AddRange(existingNet.Items);
                });

                board.NetList.Add(net);
            }

            var toremove = new List<ISelectableItem>();
            //routing to remove; this could be a setting ("Remove routing that no longer belong to nets")
            foreach (var en in existingNets)
            {
                var newNet = board.NetList.FirstOrDefault(n => n.Name == en.Name || n.Id == en.Id);
                if (newNet == null)//doesn;t belong to our new routing; remove it
                {
                    foreach (var routedItem in en.Items.OfType<ISignalPrimitiveCanvasItem>().ToList())
                    {
                        //routedItem.Signal = null;
                        //if (routedItem is SingleLayerBoardCanvasItem)
                        //{
                        //    var s = routedItem as SingleLayerBoardCanvasItem;
                        //    s.Layer = null;//this should remove it from the layer
                        //}
                        //else
                        //{
                        //    //canvasModel.RemoveItem((ISelectableItem)routedItem);
                        //}

                        toremove.Add(routedItem);

                    }
                }
            }

            board.CanvasModel.RemoveItems(toremove);

            //ensure current actualized signal
            foreach (var net in board.NetList)
            {
                //pads
                var pads = net.Pads.ToList();
                net.Pads.Clear();
                foreach (var pad in pads)
                    pad.Signal = net;

                //rest
                var netItems = net.Items.OfType<ISignalPrimitiveCanvasItem>().ToList();
                foreach (var netItem in netItems)
                    net.Items.Remove(netItem);
                foreach (var item in netItems)
                    item.Signal = net;
            }
        }

        void UpdateNetClasses(IBoardDesigner board, SchematicDocument schematic)
        {
            board.NetClasses = schematic.Classes.Cast<INetClassBaseItem>().ToList();
        }
    }
}
