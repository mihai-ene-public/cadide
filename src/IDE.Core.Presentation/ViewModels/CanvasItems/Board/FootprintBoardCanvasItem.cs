using System;
using System.Collections.Generic;
using System.Linq;
using IDE.Core.Storage;
using System.ComponentModel;
using IDE.Core.Interfaces;
using IDE.Core.Common;
using IDE.Core.Types.Media;
using System.ComponentModel.DataAnnotations;
using IDE.Core.Types.Attributes;
using IDE.Core.Presentation.Utilities;
using IDE.Core.Presentation.ObjectFinding;
using IDE.Documents.Views;

namespace IDE.Core.Designers
{
    public class FootprintBoardCanvasItem : SingleLayerBoardCanvasItem//BaseCanvasItem
                                          , IFootprintBoardCanvasItem
    {
        public FootprintBoardCanvasItem()
        {
            PartNameFontSize = 1;
        }

        //todo: if posible get rid of this dispatcher
        private IDispatcherHelper Dispatcher => ServiceProvider.Resolve<IDispatcherHelper>();

        FootprintPlacement placement;
        [Display(Order = 2)]
        [MarksDirty]
        public FootprintPlacement Placement
        {
            get { return placement; }
            set
            {
                if (placement == value) return;

                placement = value;
                OnPropertyChanged(nameof(Placement));

                UpdateLayersByPlacement();
            }
        }

        //ILayerDesignerItem layer;

        [Browsable(false)]
        public new ILayerDesignerItem Layer
        {
            get { return base.Layer; }
            set { base.Layer = value; }
            //get
            //{
            //    return layer;
            //}
            //set
            //{
            //    if (layer != value)
            //    {
            //        layer?.Items.Remove(this);
            //        layer = value;
            //        if (ParentObject == null)
            //            layer?.Items.Add(this);

            //        if (layer != null)
            //        {
            //            LayerId = layer.LayerId;
            //        }

            //        OnPropertyChanged(nameof(Layer));
            //    }
            //}
        }

        [Display(Order = 3)]
        public string PartName { get { return FootprintPrimitive.PartName; } }

        bool showName;

        [Display(Order = 4)]
        [MarksDirty]
        public bool ShowName
        {
            get { return showName; }
            set
            {
                if (showName == value)
                    return;

                showName = value;
                OnPropertyChanged(nameof(ShowName));
            }
        }

        // FontFamily partNameFontFamily;

        //[Display(Order = 2)]
        //public FontFamily PartNameFontFamily
        //{
        //    get { return partNameFontFamily; }
        //    set
        //    {
        //        partNameFontFamily = value;
        //        OnPropertyChanged(nameof(PartNameFontFamily));
        //    }
        //}

        // FontFamily partNameFontFamily;

        //[Display(Order = 2)]
        //public FontFamily PartNameFontFamily
        //{
        //    get { return partNameFontFamily; }
        //    set
        //    {
        //        partNameFontFamily = value;
        //        OnPropertyChanged(nameof(PartNameFontFamily));
        //    }
        //}

        double partNameFontSize;

        //in mm
        [Display(Order = 5)]
        [MarksDirty]
        public double PartNameFontSize
        {
            get { return partNameFontSize; }
            set
            {
                if (partNameFontSize == value) return;

                partNameFontSize = value;
                OnPropertyChanged(nameof(PartNameFontSize));

                if (Designator != null)
                {
                    Designator.FontSize = partNameFontSize;
                }
            }
        }

        double partNameStrokeWidth;

        //in mm
        [Display(Order = 6)]
        [MarksDirty]
        public double PartNameStrokeWidth
        {
            get { return partNameStrokeWidth; }
            set
            {
                if (partNameStrokeWidth == value) return;

                partNameStrokeWidth = value;
                OnPropertyChanged(nameof(PartNameStrokeWidth));

                if (Designator != null)
                {
                    Designator.StrokeWidth = partNameStrokeWidth;
                }
            }
        }

        double x;

        /// <summary>
        /// Location X of rectangle in current coordinate system
        /// </summary>
        [Editor(EditorNames.PositionXUnitsEditor, EditorNames.PositionXUnitsEditor)]
        [Display(Order = 7)]
        [MarksDirty]
        public double X
        {
            get { return x; }
            set
            {
                if (x == value)
                    return;

                x = value;
                OnPropertyChanged(nameof(X));
            }
        }

        double y;

        /// <summary>
        /// Location X of rectangle in current coordinate system
        /// </summary>
        [Editor(EditorNames.PositionYUnitsEditor, EditorNames.PositionYUnitsEditor)]
        [Display(Order = 8)]
        [MarksDirty]
        public double Y
        {
            get { return y; }
            set
            {
                if (y == value)
                    return;

                y = value;
                OnPropertyChanged(nameof(Y));
            }
        }

        double rot;

        [Display(Order = 9)]
        [MarksDirty]
        public double Rot
        {
            get { return rot; }
            set
            {
                if (rot == value)
                    return;

                rot = value % 360;
                OnPropertyChanged(nameof(Rot));
            }
        }

        double displayWidth;
        [Browsable(false)]
        public double DisplayWidth
        {
            get
            {
                return displayWidth;
            }
            set
            {
                displayWidth = value;
                OnPropertyChanged(nameof(DisplayWidth));
            }
        }

        double displayHeight;
        [Browsable(false)]
        public double DisplayHeight
        {
            get
            {
                return displayHeight;
            }
            set
            {
                displayHeight = value;
                OnPropertyChanged(nameof(DisplayHeight));
            }
        }

        //items (primitives) that form the footprint
        IList<ISelectableItem> items = new SpatialItemsSource();

        //it will need some items that are not selectable
        [Browsable(false)]
        public IList<ISelectableItem> Items
        {
            get { return items; }
        }

        //current board we load this footprint on
        [Browsable(false)]
        public ILayeredViewModel BoardModel { get; set; }

        [Browsable(false)]
        public List<IPadCanvasItem> Pads { get; private set; }

        ComponentDocument componentDocument;

        [DisplayName("Component name")]
        [Display(Order = 10000)]
        public string ComponentName => componentDocument?.Name;

        [DisplayName("Component library")]
        [Display(Order = 10001)]
        public string ComponentLibraryName => FootprintPrimitive?.ComponentLibrary;

        [DisplayName("Footprint name")]
        [Display(Order = 10002)]
        public string FootprintNameInfo => CachedFootprint?.Name;

        [DisplayName("Footprint library")]
        [Display(Order = 10003)]
        public string FootprintLibraryName => CachedFootprint?.Library;

        public void TogglePlacement()
        {
            int p = (int)placement;
            p++;
            p = p % (int)FootprintPlacement.Count;
            Placement = (FootprintPlacement)p;
        }

        public override bool IsMirrored()
        {
            return placement == FootprintPlacement.Bottom;
        }

        [Browsable(false)]
        public ILayerDesignerItem SilkscreenLayer
        {
            get
            {
                var layerId = LayerConstants.GetPairedLayer(LayerConstants.SilkscreenTopLayerId, placement);
                if (BoardModel != null && BoardModel.LayerItems != null)
                {
                    return BoardModel.LayerItems.FirstOrDefault(l => l.LayerId == layerId);
                }

                return null;
            }
        }

        

        [Browsable(false)]
        public BoardComponentInstance FootprintPrimitive { get { return currentPrimitive as BoardComponentInstance; } }



        [Browsable(false)]
        public PositionData PartNamePosition { get; set; } = new PositionData();


        DesignatorBoardCanvasItem designator;
        [Browsable(false)]
        public DesignatorBoardCanvasItem Designator
        {
            get { return designator; }
            set
            {
                designator = value;
                OnPropertyChanged(nameof(Designator));
            }
        }

        [Browsable(false)]
        public Footprint CachedFootprint { get; set; }

        private void UpdateLayersByPlacement()
        {
            if (placement == FootprintPlacement.Top)
            {
                Layer = BoardModel?.GetLayer(LayerConstants.SignalTopLayerId);
            }
            else
            {
                Layer = BoardModel?.GetLayer(LayerConstants.SignalBottomLayerId);
            }

            foreach (var item in items)
            {
                if (item is SingleLayerBoardCanvasItem layerItem)
                {
                    //we need the layer pairs for other layers, for now only some pairs are handled
                    try
                    {
                        var layer = layerItem.GetLayer(LayerConstants.GetPairedLayer(layerItem.LayerId, placement));
                        layerItem.Layer = layer;
                    }
                    catch { }
                }
            }

            designator.Layer = designator.GetLayer(LayerConstants.GetPairedLayer(designator.LayerId, placement));

            OnPropertyChanged(nameof(SilkscreenLayer));
        }

        public override void LoadLayers()
        {
            UpdateLayersByPlacement();
        }

        public override XRect GetBoundingRectangle()
        {
            return new XRect(x - 0.5 * displayWidth, y - 0.5 * displayHeight, displayWidth, displayHeight);
        }

        public override void Translate(double dx, double dy)
        {
            X += dx;
            Y += dy;
        }

        IPrimitive currentPrimitive;
        protected override void LoadFromPrimitiveInternal(IPrimitive primitive)
        {
            currentPrimitive = primitive;
            var fpInstance = (BoardComponentInstance)primitive;

            X = fpInstance.x;
            Y = fpInstance.y;
            Rot = fpInstance.rot;

            PartNamePosition.X = fpInstance.PartNameX;
            PartNamePosition.Y = fpInstance.PartNameY;
            PartNamePosition.Rotation = fpInstance.PartNameRot;

            ShowName = fpInstance.ShowName;

            Footprint foundFootprint = null;
            try
            {
                DateTime? lastModified = null;

                if (CachedFootprint != null)
                {
                    if (fpInstance.FootprintId == CachedFootprint.Id && fpInstance.Library == CachedFootprint.Library)
                    {
                        lastModified = CachedFootprint?.LastAccessed;
                    }
                }

                var fileDoc = this.BoardModel as IFileBaseViewModel;
                var project = fileDoc.GetCurrentProjectInfo();

                var objectFinder = ServiceProvider.Resolve<IObjectFinder>();
                foundFootprint = objectFinder.FindObject<Footprint>(project, fpInstance.Library, fpInstance.FootprintId, lastModified);
            }
            catch { }


            if (foundFootprint == null)
                return;

            foundFootprint.LastAccessed = DateTime.Now;

            LoadComponent();

            //load footprint
            {
                CachedFootprint = foundFootprint;

                var oldItems = items.ToList();
                //items.Clear();
                var canvasItems = new List<ISelectableItem>();
                if (CachedFootprint.Items != null)
                {
                    foreach (var footprintItem in CachedFootprint.Items)
                    {
                        var canvasItem = (BoardCanvasItemViewModel)footprintItem.CreateDesignerItem();
                        canvasItem.LayerDocument = BoardModel;
                        canvasItem.ParentObject = this;
                        canvasItem.LoadLayers();
                        if (canvasItem is IPadCanvasItem padItem)
                        {
                            var board = BoardModel as IBoardDesigner;
                            var signal = (from s in board.NetList
                                          from si in s.Pads
                                          where si.Number == padItem.Number && si.FootprintInstanceId == fpInstance.Id
                                          select s).FirstOrDefault();

                            if (signal != null)
                            {
                                //todo: we must remove from signal.Items and from Pads
                                var toRemove = signal.Pads.Where(p => p.Number == padItem.Number && p.FootprintInstanceId == padItem.FootprintInstanceId).ToList();

                                foreach (var p in toRemove)
                                    signal.Pads.Remove(p);

                                padItem.Signal = signal;
                            }
                        }

                        canvasItems.Add(canvasItem);
                    }

                    //designator
                    Designator = new DesignatorBoardCanvasItem
                    {
                        Text = PartName,
                        LayerDocument = BoardModel,
                        ParentObject = this,
                        LayerId = LayerConstants.SilkscreenTopLayerId,
                        Layer = SilkscreenLayer,
                    };

                    Dispatcher.RunOnDispatcher(() =>
                    {
                        items.Clear();
                        items.AddRange(canvasItems.OrderBy(c => c.ZIndex));
                        designator.LoadLayers();
                        var canvasModel = GetCanvasModel();
                        canvasModel?.AddItem(designator);
                    });

                    //load pads
                    var pads = new List<IPadCanvasItem>();
                    pads.AddRange(items.OfType<IPadCanvasItem>());
                    Pads = pads;
                }

                OnPropertyChanged(nameof(Items));
            }

            PartNameFontSize = fpInstance.PartNameFontSize;
            PartNameStrokeWidth = fpInstance.PartNameStrokeWidth;
            Placement = fpInstance.Placement;
            IsLocked = fpInstance.IsLocked;

            //translate all primitives to 0,0; update ItemWidth and ItemHeight
            var rect = XRect.Empty;

            //var xOffset = double.MaxValue;
            //var yOffset = double.MaxValue;
            foreach (var item in Pads)
            {
                var itemRect = item.GetBoundingRectangle();

                //if (xOffset > itemRect.X)
                //    xOffset = itemRect.X;
                //if (yOffset > itemRect.Y)
                //    yOffset = itemRect.Y;

                rect.Union(itemRect);
            }
            if (rect != XRect.Empty)
            {
                ////offset relative to the center of the rectangle
                //foreach (BaseCanvasItem item in Items)
                //    item.Translate(-xOffset - rect.Width * 0.5, -yOffset - rect.Height * 0.5);

                DisplayWidth = rect.Width;
                DisplayHeight = rect.Height;
            }
        }

        void LoadComponent()
        {
            try
            {
                var fp = FootprintPrimitive;
                var fileDoc = this.BoardModel as IFileBaseViewModel;
                var project = fileDoc.GetCurrentProjectInfo();

                var lastRead = componentDocument?.LastAccessed;
                var objectFinder = ServiceProvider.Resolve<IObjectFinder>();
                var comp = objectFinder.FindObject<ComponentDocument>(project, fp.ComponentLibrary, fp.ComponentId, lastRead);

                if (comp != null)
                {
                    componentDocument = comp;
                    componentDocument.LastAccessed = DateTime.Now;
                }

                if (componentDocument == null)
                    throw new Exception("Component not found");

            }
            catch (Exception ex)
            {
                ServiceProvider.GetToolWindow<IOutputToolWindow>().AppendLine(ex.Message);
            }

        }

        ICanvasDesignerFileViewModel GetCanvasModel()
        {
            var canvas = BoardModel as ICanvasDesignerFileViewModel;

            return canvas;
        }

        public override IPrimitive SaveToPrimitive()
        {
            var t = new BoardComponentInstance();

            var fp = FootprintPrimitive;
            if (fp != null)
            {
                t.Id = fp.Id;
                t.PartId = fp.PartId;
                t.PartName = fp.PartName;
                t.ComponentLibrary = fp.ComponentLibrary;
                t.ComponentId = fp.ComponentId;
            }

            t.x = X;
            t.y = Y;
            t.rot = Rot;

            t.PartNameX = PartNamePosition.X;
            t.PartNameY = PartNamePosition.Y;
            t.PartNameRot = PartNamePosition.Rotation;

            t.ShowName = ShowName;
            //t.PartNameFontFamily = PartNameFontFamily.Source;
            t.PartNameFontSize = PartNameFontSize;

            //t.CachedFootprint = CachedPart;
            t.FootprintId = CachedFootprint.Id;
            t.Placement = Placement;
            t.IsLocked = IsLocked;
            t.Library = CachedFootprint.Library;

            return t;
        }

        public override void MirrorX()
        {
        }

        public override void MirrorY()
        {
        }

        public override void TransformBy(XMatrix matrix)
        {
            var p = new XPoint(x, y);
            p = matrix.Transform(p);
            X = p.X;
            Y = p.Y;

            var rotAngle = GetRotationAngleFromMatrix(matrix);
            Rot = RotateSafe(Rot, rotAngle);

        }

        public override void Rotate(double angle = 90)
        {
            Rot = RotateSafe(Rot, angle);
        }

        public override string ToString()
        {
            return PartName;
        }

        public override object Clone()
        {
            var p = SaveToPrimitive();
            var newFootprint = new FootprintBoardCanvasItem()
            {
                BoardModel = BoardModel,
            };

            newFootprint.LoadFromPrimitive(p);

            Dispatcher.RunOnDispatcher(() =>
            {
                var canvasModel = GetCanvasModel();
                canvasModel?.Items.Remove(newFootprint.Designator);
            });

            return newFootprint;
        }
    }

}
