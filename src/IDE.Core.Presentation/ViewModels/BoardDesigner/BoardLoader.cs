using System.Linq;
using IDE.Core.Storage;
using IDE.Core.Designers;
using IDE.Core.Interfaces;
using System.Collections.Generic;
using IDE.Core.Common;
using IDE.Core;
using IDE.Core.Types.Media;

namespace IDE.Documents.Views
{
    public class BoardLoader
    {
        private readonly IDispatcherHelper dispatcher;
        private readonly ISolutionProjectNodeModel project;

        public BoardLoader(IDispatcherHelper _dispatcher, ISolutionProjectNodeModel _project)
        {
            dispatcher = _dispatcher;
            project = _project;
        }

        public void Load(BoardDocument boardDocument, IBoardDesigner board)
        {
            //assign a new id if needed
            if (boardDocument.Id == 0)
            {
                boardDocument.Id = LibraryItem.GetNextId();
            }

            var canvasModel = board.CanvasModel;

            canvasModel.DocumentWidth = boardDocument.DocumentWidth;
            canvasModel.DocumentHeight = boardDocument.DocumentHeight;

            LoadLayers(boardDocument, board);

            LoadRules(boardDocument, board);

            //plain items
            var plainItems = new List<ISelectableItem>();
            if (boardDocument.PlainItems != null)
            {
                foreach (var primitive in boardDocument.PlainItems)
                {
                    var canvasItem = (BoardCanvasItemViewModel)primitive.CreateDesignerItem();
                    canvasItem.LayerDocument = board;
                    plainItems.Add(canvasItem);
                }
            }

            dispatcher.RunOnDispatcher(() =>
            {
                foreach (BoardCanvasItemViewModel item in plainItems)
                {
                    item.LoadLayers();
                    canvasModel.AddItem(item);
                }
            });

            //board outline
            LoadBoardOutline(boardDocument, board);

            var canvasItems = new List<ISelectableItem>();

            var footprints = new List<FootprintBoardCanvasItem>();
            //footprints
            if (boardDocument.Components != null)
            {
                foreach (var fpInstance in boardDocument.Components)
                // Parallel.ForEach(boardDocument.Components, fpInstance =>
                {
                    var canvasItem = new FootprintBoardCanvasItem()
                    {
                        ProjectModel = project,
                        BoardModel = board,
                    };

                    canvasItem.LoadFromPrimitive(fpInstance);

                    footprints.Add(canvasItem);
                }
                // );

                //project?.ClearCachedItems();
            }

            //net classes
            board.NetClasses.Clear();
            board.NetClasses = boardDocument.Classes.Cast<INetClassBaseItem>().ToList();

            //net list
            board.NetList.Clear();
            foreach (var brdNet in boardDocument.Nets)
            {
                BoardNetDesignerItem brdNetItem = null;

                //the logic with "Undefined" net was removed starting with v1.12
                //we keep this for a while
                if (brdNet.NetId != 0)
                {
                    brdNetItem = new BoardNetDesignerItem(board)
                    {
                        Id = brdNet.NetId,
                        Name = brdNet.Name,
                        ClassId = brdNet.ClassId,
                    };
                    board.NetList.Add(brdNetItem);
                }
                    

                //pads
                if (brdNet.Pads != null)
                {
                    brdNet.Pads.ForEach(p =>
                    {
                        var fpInstance = //canvasModel.Items.OfType<FootprintBoardCanvasItem>()
                                        footprints
                                        .FirstOrDefault(f => f.FootprintPrimitive.Id == p.FootprintInstanceId);
                        if (fpInstance != null)
                        {
                            var pad = fpInstance.Pads.FirstOrDefault(pp => pp.Number == p.PadNumber);
                            if (pad != null)
                            {
                                pad.Signal = brdNetItem;
                            }
                        }
                    });
                }

                if (brdNet.Items != null)
                {
                    canvasItems = new List<ISelectableItem>();
                    foreach (var primitive in brdNet.Items)
                    {
                        var canvasItem = (BoardCanvasItemViewModel)primitive.CreateDesignerItem();
                        canvasItem.LayerDocument = board;
                        canvasItems.Add(canvasItem);

                    }

                    dispatcher.RunOnDispatcher(() =>
                    {
                        foreach (BoardCanvasItemViewModel item in canvasItems)
                        {
                            item.LoadLayers();
                            canvasModel.AddItem(item);

                            if (item is ISignalPrimitiveCanvasItem s)
                                s.Signal = brdNetItem;
                        }

                        //canvasModel.AddItems(canvasItems);
                    });
                }
            }
            //}
            //);

            //we add planes if we don't have any already loaded
            LoadMissingPlanes(boardDocument, board);

            dispatcher.RunOnDispatcher(() =>
            {
                foreach (var f in footprints)
                {
                    canvasModel.AddItem(f);
                }
            });
        }

        void LoadMissingPlanes(BoardDocument boardDocument, IBoardDesigner board)
        {
            var canvasModel = board.CanvasModel;
            foreach (var layer in board.LayerItems.Where(l => l.LayerType == LayerType.Plane))
            {
                var plane = layer.Items.OfType<PlaneBoardCanvasItem>().FirstOrDefault();
                if (plane == null)
                {
                    //add the plane, but don't add to any net; we let the user to set the net
                    var planePrimtive = new PlaneBoard() { layerId = layer.LayerId };
                    var planeBoard = (BoardCanvasItemViewModel)planePrimtive.CreateDesignerItem();
                    planeBoard.LayerDocument = board;

                    dispatcher.RunOnDispatcher(() =>
                    {
                        planeBoard.LoadLayers();
                        canvasModel.AddItem(planeBoard);
                    });
                }
            }
        }

        void LoadBoardOutline(BoardDocument boardDocument, IBoardDesigner board)
        {
            var canvasModel = board.CanvasModel;

            var canvasItems = new List<ISelectableItem>();
            RegionBoard boardOutline = null;
            if (boardDocument.BoardOutline == null
                || boardDocument.BoardOutline.Items == null
                || boardDocument.BoardOutline.Items.Count == 0)
            {
                //create default
                boardOutline = RegionBoard.CreateDefault();
            }
            else
                boardOutline = boardDocument.BoardOutline;

            var boardOutlineItem = RegionBoardCanvasItem.FromData(boardOutline);
            boardOutlineItem.ZIndex = -5000;
            //canvasModel.AddItem(boardOutlineItem);
            canvasItems.Add(boardOutlineItem);

            board.BoardOutline = boardOutlineItem;

            //create default canvas items if needed
            var layerId = (int)LayerType.BoardOutline + 1;
            var outlineLayer = board.LayerItems.FirstOrDefault(l => l.LayerId == layerId);
            var hasItems = false;
            if (outlineLayer != null)
                hasItems = outlineLayer.Items.OfType<SingleLayerBoardCanvasItem>().Any();
            if (hasItems == false)
            {
                var regionPrimitives = RegionBoard.GetDefaultBoardOutlineCanvasItems().ToList();
                foreach (var primitive in regionPrimitives)
                {
                    var canvasItem = (BoardCanvasItemViewModel)primitive.CreateDesignerItem();
                    canvasItem.LayerDocument = board;
                    // canvasItem.LoadLayers();
                    // canvasModel.AddItem(canvasItem);
                    canvasItems.Add(canvasItem);
                }
            }

            dispatcher.RunOnDispatcher(() =>
            {
                foreach (BoardCanvasItemViewModel item in canvasItems)
                {
                    item.LoadLayers();
                    canvasModel.AddItem(item);
                }
            });
        }

        public void LoadLayers(BoardDocument boardDocument, IBoardDesigner board)
        {
            var canvasModel = board.CanvasModel;

            IList<Layer> layers = null;
            if (boardDocument.Layers != null && boardDocument.Layers.Count > 0)
            {
                layers = boardDocument.Layers;
            }
            else
            {
                layers = BoardDocument.CreateDefaultLayers();
                boardDocument.Layers = layers.ToList();
            }
            if (boardDocument.DrillPairs == null || boardDocument.DrillPairs.Count == 0)
                boardDocument.DrillPairs = BoardDocument.CreateDefaultDrillPairs();
            if (boardDocument.LayerPairs == null || boardDocument.LayerPairs.Count == 0)
                boardDocument.LayerPairs = BoardDocument.CreateDefaultLayerPairs();

            var groups = LayerGroup.GetLayerGroupDefaults(layers);//default layers are hard coded
            //user layers are saved in the board doc
            boardDocument.LayerGroups.ForEach(l => l.IsReadOnly = false);
            groups.AddRange(boardDocument.LayerGroups);

            //todo: drill pairs, layer pairs
            var layerOrder = 0;// layers.Count;
            var layerItems = layers.Select(l => new LayerDesignerItem(board)
            {
                LayerName = l.Name,
                LayerId = l.Id,
                StackOrder = l.StackOrder,
                LayerOrder = layerOrder--,
                LayerType = l.Type,
                LayerColor = XColor.FromHexString(l.Color),
                Material = l.Material,
                DielectricConstant = l.DielectricConstant,
                Thickness = l.Thickness,

                GerberFileName = l.GerberFileName,
                GerberExtension = l.GerberExtension,
                Plot = l.Plot,
                MirrorPlot = l.MirrorPlot
            }).ToList();
            //LayerItems = layerItems;


            var groupItems = new List<LayerGroupDesignerItem>();
            foreach (var g in groups)
            {
                var filteredLayers = layerItems.Where(l => g.Layers.Any(gl => gl.Id == l.LayerId)).Cast<ILayerDesignerItem>().ToList();
                var newG = new LayerGroupDesignerItem { Name = g.Name, IsReadOnly = g.IsReadOnly };
                newG.LoadLayers(filteredLayers);
                groupItems.Add(newG);
            }

            dispatcher.RunOnDispatcher(() =>
            {
                board.LayerItems.Clear();
                board.LayerItems.AddRange(layerItems);
                canvasModel.AddItems(layerItems);
                board.LayerGroups.Clear();
                board.LayerGroups.AddRange(groupItems);

                board.SelectedLayerGroup = (ILayerGroupDesignerItem)board.LayerGroups[0];

            });


            //drill pairs
            board.DrillPairs.Clear();
            board.DrillPairs.AddRange(boardDocument.DrillPairs.Select(dp => new LayerPairModel
            {
                LayerStart = board.LayerItems.FirstOrDefault(l => l.LayerId == dp.LayerIdStart),
                LayerEnd = board.LayerItems.FirstOrDefault(l => l.LayerId == dp.LayerIdEnd)
            }));

            //layer pairs
            board.LayerPairs.Clear();
            board.LayerPairs.AddRange(boardDocument.LayerPairs.Select(dp => new LayerPairModel
            {
                LayerStart = board.LayerItems.FirstOrDefault(l => l.LayerId == dp.LayerIdStart),
                LayerEnd = board.LayerItems.FirstOrDefault(l => l.LayerId == dp.LayerIdEnd)
            }));
        }

        public void LoadRules(BoardDocument boardDocument, IBoardDesigner board)
        {
            IList<BoardRule> rules = null;
            if (boardDocument.BoardRules != null && boardDocument.BoardRules.Count > 0)
            {
                rules = boardDocument.BoardRules;
            }
            else
            {
                rules = BoardDocument.CreateDefaultBoardRules();
                boardDocument.BoardRules = rules.ToList();
            }

            board.Rules.Clear();
            //add groups first
            board.Rules.AddRange(rules.OfType<GroupRule>().Select(c => c.CreateRuleItem()));
            //rest of rules
            board.Rules.AddRange(rules.Where(c => !(c is GroupRule)).Select(c => c.CreateRuleItem()));
        }

    }
}
