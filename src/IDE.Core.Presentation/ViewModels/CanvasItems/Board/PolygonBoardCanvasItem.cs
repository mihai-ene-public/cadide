using IDE.Core.Interfaces;
using IDE.Core.Storage;
using IDE.Core.Common;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using IDE.Core.Types.Media;
using IDE.Core.Presentation;
using System.ComponentModel.DataAnnotations;
using IDE.Core.Types.Attributes;
using IDE.Core.Presentation.Utilities;

namespace IDE.Core.Designers
{
    public class PolygonBoardCanvasItem : SingleLayerBoardCanvasItem, ISignalPrimitiveCanvasItem, IPolygonBoardCanvasItem//, IPlainDesignerItem
    {
        public PolygonBoardCanvasItem()
        {
            PolygonPoints = new ObservableCollection<XPoint>();

            BorderWidth = 0;//0.254;

            PropertyChanged += PolygonBoardCanvasItem_PropertyChanged;

            debounceRepurPoly = ServiceProvider.Resolve<IDebounceDispatcher>();
        }

        void PolygonBoardCanvasItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {

            switch (e.PropertyName)
            {
                case nameof(PolygonPoints):
                    OnPropertyChanged(nameof(SignalNameFontSize));
                    OnPropertyChanged(nameof(SignalNamePosition));
                    break;

                case nameof(IsPlaced):
                case nameof(IsFilled):
                    OnPropertyChanged(nameof(IsPlaceholderVisible));
                    break;
            }



            var repourPropNamesExcluded = new[]
            {
                nameof(PolygonGeometry),
                nameof(IsSelected),
                nameof(SignalNameFontSize),
                nameof(SignalNamePosition),
                nameof(IsPlaceholderVisible)
            };

            if (!repourPropNamesExcluded.Contains(e.PropertyName))//?? && !IsPlaced)
            {
                if (!IsPlaceholderVisible)
                {
                    debounceRepurPoly.Debounce(200, async p => await RepourPolygonAsync());
                }
            }

        }

        double borderWidth;

        [Editor(EditorNames.SizeMilimetersUnitsEditor, EditorNames.SizeMilimetersUnitsEditor)]
        [Display(Order = 2)]
        [MarksDirty]
        public double BorderWidth
        {
            get { return borderWidth; }
            set
            {
                borderWidth = value;

                OnPropertyChanged(nameof(BorderWidth));
            }
        }

        bool isFilled = true;

        [Display(Order = 3)]
        [MarksDirty]
        public bool IsFilled
        {
            get { return isFilled; }

            set
            {
                isFilled = value;
                OnPropertyChanged(nameof(IsFilled));
            }
        }

        [Browsable(false)]
        public bool IsPlaceholderVisible => !IsPlaced || !isFilled;

        //TODO: we need an editor for this. Applies for boards only
        IBoardNetDesignerItem signal;

        [Display(Order = 4)]
        [Editor(EditorNames.BoardNetDesignerItemEditor, EditorNames.BoardNetDesignerItemEditor)]
        [MarksDirty]
        public IBoardNetDesignerItem Signal
        {
            get
            {
                return signal;
            }
            set
            {
                //var setSignal = value;
                ////the signal must exist
                //if (setSignal != null)
                //{
                //    var brd = LayerDocument as IBoardModel;
                //    var s = brd.NetList.FirstOrDefault(n => n.Name == setSignal.Name);
                //    if (s == null)
                //        setSignal = null;
                //}


                //if (setSignal == null && signal != null)
                //    signal.Items.Remove(this);

                //signal = setSignal;

                //if (signal != null && !signal.Items.Contains(this))
                //    signal.Items.Add(this);

                if (signal != null)
                    signal.Items.Remove(this);

                //todo: we need this assignment on all signal items
                if (value != null && value.Id <= 0)
                    signal = null;
                else
                    signal = value;

                if (signal != null)
                    signal.Items.Add(this);

                OnPropertyChanged(nameof(Signal));
            }
        }

        public void AssignSignal(IBoardNetDesignerItem newSignal)
        {
            signal = newSignal;
        }

        int drawOrder;
        //[Editor(typeof(OrderEditor), typeof(OrderEditor))]
        [Display(Order = 5)]
        [Description("The order that this polygon will draw in relation to other polygons on the same layer")]
        [MarksDirty]
        public int DrawOrder
        {
            get
            {
                return drawOrder;
            }
            set
            {
                // if (drawOrder != value)
                {

                    drawOrder = value;
                    OnPropertyChanged(nameof(DrawOrder));
                }
            }
        }

        PolygonType polygonType;

        [Display(Order = 6)]
        [MarksDirty]
        public PolygonType PolygonType
        {
            get { return polygonType; }
            set
            {
                polygonType = value;
                OnPropertyChanged(nameof(PolygonType));
            }
        }

        bool generateThermals = true;

        [Display(Order = 7)]
        [MarksDirty]
        public bool GenerateThermals
        {
            get { return generateThermals; }
            set
            {
                generateThermals = value;
                OnPropertyChanged(nameof(GenerateThermals));
            }
        }

        double thermalWidth;

        [Editor(EditorNames.SizeMilimetersUnitsEditor, EditorNames.SizeMilimetersUnitsEditor)]
        [Display(Order = 8)]
        [MarksDirty]
        public double ThermalWidth
        {
            get { return thermalWidth; }
            set
            {
                thermalWidth = value;

                OnPropertyChanged(nameof(ThermalWidth));
            }
        }

        PositionData signalNamePosition = new PositionData();
        [Browsable(false)]
        public PositionData SignalNamePosition
        {
            get
            {
                if (SignalNameIsVisible)
                {
                    var mp = MiddlePoint;

                    signalNamePosition.X = mp.X - 0.25 * GetSignalTextLength();
                    signalNamePosition.Y = mp.Y - SignalNameFontSize * 0.5;
                }

                return signalNamePosition;
            }
        }

        [Browsable(false)]
        public double SignalNameFontSize
        {
            get
            {
                //return 0.8 * diameter;


                var d = 0.8d;// * r.Width;

                const double maxSize = 2;

                if (SignalNameIsVisible)
                {
                    var r = GetBoundingRectangle();
                    d = 1.6 * r.Width / Signal.Name.Length;
                }
                if (d > maxSize)
                    d = maxSize;

                return d;
            }
        }

        //[Browsable(false)]
        //public PolygonBoardCanvasItem This
        //{
        //    get { return this; }
        //}

        IGeometry polygonGeometry;
        [Browsable(false)]
        public IGeometry PolygonGeometry
        {
            get { return polygonGeometry; }
            set
            {
                polygonGeometry = value;
                OnPropertyChanged(nameof(PolygonGeometry));
            }
        }


        //todo: refactoring this should not be here (poly repour)
        IDebounceDispatcher debounceRepurPoly;//= new DebounceDispatcher();
        public async Task RepourPolygonAsync()
        {
            if (!IsPlaced)
                return;

            var isEditing = false;
            if (LayerDocument is IFileBaseViewModel file)
            {
                isEditing = file.State == DocumentState.IsEditing;
            }

            if (isEditing)
            {
                var sw = Stopwatch.StartNew();

                var thisBoard = LayerDocument as IBoardDesigner;

                // var proc = ServiceProvider.Resolve<IPolygonGeometryPourProcessor>();
                var proc = ServiceProvider.Resolve<IPolygonGeometryOutlinePourProcessor>();
                var dispatcher = ServiceProvider.Resolve<IDispatcherHelper>();
                await Task.Run(() => proc.GetGeometry(this, thisBoard))
                    .ContinueWith(t =>
                    {
                        dispatcher.RunOnDispatcher(() =>
                        PolygonGeometry = new GeometryWrapper(t.Result));
                    });

                sw.Stop();
                Debug.WriteLine($"Geometry generated: {sw.ElapsedMilliseconds} ms");
            }

        }

        double GetSignalTextLength()
        {
            //assume width of letter is the same as height
            if (signal != null && !string.IsNullOrEmpty(signal.Name))
                return SignalNameFontSize * signal.Name.Length;

            return 0;
        }

        [Browsable(false)]
        public bool SignalNameIsVisible
        {
            get
            {
                return signal != null && !string.IsNullOrEmpty(signal.Name) && signal.Id != 0;
            }
        }

        IList<XPoint> polygonPoints;

        [Browsable(false)]
        public IList<XPoint> PolygonPoints
        {
            get
            {
                return polygonPoints;
            }
            set
            {
                polygonPoints = value;
                // Primitive.vertices = polygonPoints.Select(p => new Vertex { x = p.X, y = p.Y }).ToList();

                OnPropertyChanged(nameof(PolygonPoints));
            }
        }

        [Browsable(false)]
        public XPoint MiddlePoint
        {
            get
            {
                //calculate the center of gravity
                var middlePoint = new XPoint();
                foreach (var p in PolygonPoints)
                {
                    middlePoint.Offset(p.X, p.Y);
                }
                middlePoint.X /= PolygonPoints.Count;
                middlePoint.Y /= PolygonPoints.Count;

                return middlePoint;
            }
        }



        public override void Translate(double dx, double dy)
        {
            for (int i = 0; i < PolygonPoints.Count; i++)
            {
                var p = polygonPoints[i];
                p.Offset(dx, dy);
                polygonPoints[i] = p;
            }
            OnPropertyChanged(nameof(PolygonPoints));
        }

        public override XRect GetBoundingRectangle()
        {
            var minPoint = PolygonPoints.FirstOrDefault();
            var maxPoint = PolygonPoints.FirstOrDefault();

            foreach (var point in PolygonPoints)
            {
                if (point.X < minPoint.X)
                    minPoint.X = point.X;
                if (point.Y < minPoint.Y)
                    minPoint.Y = point.Y;
                if (point.X > maxPoint.X)
                    maxPoint.X = point.X;
                if (point.Y > maxPoint.Y)
                    maxPoint.Y = point.Y;
            }

            return new XRect(minPoint, maxPoint);
        }

        //public override void PlacingMouseMove(PlacementStatus status, Point mousePosition)
        //{
        //    var mpX = SnapToGrid(mousePosition.X);
        //    var mpY = SnapToGrid(mousePosition.Y);
        //    switch (status)
        //    {
        //        case PlacementStatus.Ready:
        //            {
        //                if (polygonPoints.Count > 1)
        //                {
        //                    //we update our 2 points at once
        //                    PolygonPoints[polygonPoints.Count - 1] = new Point(mpX, mpY);
        //                    PolygonPoints[polygonPoints.Count - 2] = new Point(mpX, mpY);
        //                    OnPropertyChanged(nameof(PolygonPoints));
        //                }

        //            }
        //            break;
        //        case PlacementStatus.Started:
        //            {
        //                if (PolygonPoints.Count > 0)
        //                {
        //                    PolygonPoints[PolygonPoints.Count - 1] = new Point(mpX, mpY);
        //                    OnPropertyChanged(nameof(PolygonPoints));
        //                }
        //            }
        //            break;
        //    }
        //}

        //public override void PlacingMouseUp(PlacementData status, Point _mousePosition)
        //{
        //    var mousePosition = new Point(SnapToGrid(_mousePosition.X), SnapToGrid(_mousePosition.Y));
        //    switch (status.PlacementStatus)
        //    {
        //        //1st click
        //        case PlacementStatus.Ready:
        //            //add 2 points
        //            PolygonPoints.Add(mousePosition);
        //            PolygonPoints.Add(mousePosition);
        //            status.PlacementStatus = PlacementStatus.Started;
        //            break;
        //        //n-th click
        //        case PlacementStatus.Started:

        //            if (polygonPoints.Count < 1)
        //                break;

        //            var firstPoint = polygonPoints.FirstOrDefault();

        //            if (firstPoint != null)
        //            {
        //                const double eps = 0.5;//this could be the grid size


        //                var distance = (mousePosition - firstPoint).Length;
        //                if (polygonPoints.Count >= 3 && distance <= eps)
        //                {
        //                    //remove last point
        //                    PolygonPoints.RemoveAt(PolygonPoints.Count - 1);
        //                    IsPlaced = true;
        //                    Parent.OnDrawingChanged();

        //                    //create another object
        //                    var canvasItem = (PolygonBoardCanvasItem)Activator.CreateInstance(GetType());
        //                    canvasItem.BorderWidth = BorderWidth;
        //                    canvasItem.Layer = Layer;

        //                    Parent.PlacingObject = new PlacementData
        //                    {
        //                        PlacementStatus = PlacementStatus.Ready,
        //                        PlacingObjects = new List<ISelectableItem> { canvasItem }
        //                    };

        //                    Parent.AddItem(canvasItem);
        //                }
        //                else
        //                {
        //                    PolygonPoints.Add(mousePosition);
        //                }
        //            }

        //            break;
        //    }
        //}

        protected override void LoadFromPrimitiveInternal(IPrimitive primitive)
        {
            var p = (PolygonBoard)primitive;

            var vertices = p.vertices.Select(v => new XPoint(v.x, v.y)).ToList();
            PolygonPoints = new ObservableCollection<XPoint>(vertices);
            BorderWidth = p.BorderWidth;
            IsFilled = p.IsFilled;
            LayerId = p.layerId;
            DrawOrder = p.DrawOrder;
            PolygonType = p.Type;
            GenerateThermals = p.GenerateThermals;
            ThermalWidth = p.ThermalWidth;
            IsLocked = p.IsLocked;
        }

        public override IPrimitive SaveToPrimitive()
        {
            var p = new PolygonBoard();

            p.vertices = polygonPoints.Select(v => new Vertex { x = v.X, y = v.Y }).ToList();
            p.BorderWidth = BorderWidth;
            p.IsFilled = IsFilled;
            p.layerId = (Layer?.LayerId).GetValueOrDefault();
            p.DrawOrder = DrawOrder;
            p.Type = PolygonType;
            p.GenerateThermals = GenerateThermals;
            p.ThermalWidth = ThermalWidth;
            p.IsLocked = IsLocked;

            return p;
        }

        public override void MirrorX()
        {
            ScaleX *= -1;
        }

        public override void MirrorY()
        {
            ScaleY *= -1;
        }

        public override void TransformBy(XMatrix matrix)
        {
            for (int i = 0; i < PolygonPoints.Count; i++)
            {
                var p = polygonPoints[i];
                p = matrix.Transform(p);
                polygonPoints[i] = p;
            }
            OnPropertyChanged(nameof(PolygonPoints));
        }

        public override void Rotate()
        {
            var mp = new XPoint();
            //start point is equal endpoint so the actual number of points is n-1
            var points = polygonPoints.Take(polygonPoints.Count - 1).ToList();
            foreach (var p in points)
            {
                mp.Offset(p.X, p.Y);
            }
            mp.X /= points.Count;
            mp.Y /= points.Count;

            var tg = new XTransformGroup();
            var rotateTransform = new XRotateTransform(90)
            {
                CenterX = mp.X,
                CenterY = mp.Y
            };

            tg.Children.Add(rotateTransform);

            TransformBy(tg.Value);
        }

        public override void RemoveFromCanvas()
        {
            Signal = null;

            base.RemoveFromCanvas();
        }

        //todo: refactoring; move this to poly repour
        public IList<IItemWithClearance> GetExcludedItems()
        {
            var returnItems = new List<IItemWithClearance>();

            var thisBoard = LayerDocument as IBoardDesigner;

            var isEditing = thisBoard != null && (thisBoard as IFileBaseViewModel).State == DocumentState.IsEditing;

            if (IsPlaced
                && IsFilled
                && PolygonType != PolygonType.Keepout
                && isEditing)
            {
                if (thisBoard != null
                    && Layer != null
                    && (Layer.LayerType == LayerType.Signal
                        || Layer.LayerType == LayerType.Plane))
                {
                    var defaultClearance = 0.254d;//mm

                    var rulesManger = new BoardRulesManager(thisBoard);

                    var canvasItems = (List<ISelectableItem>)thisBoard.CanvasModel.GetItems();

                    var footprints = thisBoard.CanvasModel.GetFootprints().ToList();
                    var footprintItems = (from fp in footprints
                                          from p in fp.Items.OfType<ISignalPrimitiveCanvasItem>().Cast<SingleLayerBoardCanvasItem>()
                                          where p.ShouldBeOnLayer(Layer)//p.Layer == Layer
                                          select p);

                    //todo: that is on layer (we could store it on Layer.Items)
                    var vias = canvasItems.OfType<ViaCanvasItem>();

                    var items = Layer.Items.Where(c => c != this && c is ISignalPrimitiveCanvasItem)
                                       .Select(c => c)
                                       .Union(footprintItems)
                                       .Union(vias);

                    var trackItems = items.OfType<ITrackBoardCanvasItem>();
                    var allItemsExceptTracks = items.Where(p => !(p is ITrackBoardCanvasItem));

                    var thisPolyRect = GetBoundingRectangle();

                    //tracks
                    foreach (var track in trackItems)
                    {
                        if (this.IsOnSameSignalWith(track))
                            continue;

                        var itemRect = track.GetBoundingRectangle();
                        if (thisPolyRect.Intersects(itemRect))
                        {
                            var clearance = rulesManger.GetElectricalClearance(this, track, defaultClearance);
                            //var inflatedItem = track;
                            var inflatedItem = GetItemWithClearance(track, clearance);

                            returnItems.Add(inflatedItem);
                        }
                    }

                    //other than tracks
                    foreach (var item in allItemsExceptTracks)
                    {
                        var itemRect = item.GetBoundingRectangle();
                        var isPad = false;
                        if (item is IPadCanvasItem pad)
                        {
                            isPad = true;
                            var t = item.GetTransform();
                            if (t != null)
                                itemRect.Transform(t.Value);
                        }

                        //isPad is a small hack; it doesn't create a proper tranform for the rectangle for a pad
                        if (thisPolyRect.Intersects(itemRect) || isPad)
                        {
                            if (item is ISignalPrimitiveCanvasItem otherSignalItem)
                            {
                                if (item is IViaCanvasItem
                                    && Signal != null
                                    && otherSignalItem.Signal != null
                                    && Signal.Name == otherSignalItem.Signal.Name)
                                    continue;
                            }

                            //we cut from this poly if other poly draw order is lower
                            if (item is PolygonBoardCanvasItem otherPoly
                                && otherPoly.PolygonType != PolygonType.Keepout
                                && DrawOrder <= otherPoly.DrawOrder)
                            {
                                continue;
                            }

                            var clearance = rulesManger.GetElectricalClearance(this, item, defaultClearance);
                            // var inflatedItem = item;
                            var inflatedItem = GetItemWithClearance(item, clearance);

                            returnItems.Add(inflatedItem);
                        }
                    }
                }
            }

            return returnItems;
        }





        public override string ToString()
        {
            return $"Poly";
        }
    }



    public class ItemWithClearance : IItemWithClearance
    {
        public ItemWithClearance(ISelectableItem canvasItem, double clearance)
        {
            CanvasItem = canvasItem;
            Clearance = clearance;
        }

        public ISelectableItem CanvasItem { get; private set; }

        public double Clearance { get; private set; }
    }


}
