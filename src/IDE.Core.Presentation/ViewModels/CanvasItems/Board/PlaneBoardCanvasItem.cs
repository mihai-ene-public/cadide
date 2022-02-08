using IDE.Core.Common;
using IDE.Core.Interfaces;
using IDE.Core.Presentation;
using IDE.Core.Presentation.Utilities;
using IDE.Core.Storage;
using IDE.Core.Types.Attributes;
using IDE.Core.Types.Media;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IDE.Core.Designers
{
    public class PlaneBoardCanvasItem : SingleLayerBoardCanvasItem, IPlaneBoardCanvasItem
    {
        public PlaneBoardCanvasItem()
        {
            //CanEdit = false;//?
            PropertyChanged += PlaneBoardCanvasItem_PropertyChanged;

            debounceRepurPoly = ServiceProvider.Resolve<IDebounceDispatcher>();
        }

        private void PlaneBoardCanvasItem_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var repourPropNamesExcluded = new[]
           {
                nameof(RegionGeometry),
                nameof(IsSelected),
            };

            if (!repourPropNamesExcluded.Contains(e.PropertyName) && !IsPlaced)
            {
                debounceRepurPoly.Debounce(200, async p => await RepourPolygonAsync());
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

        IBoardNetDesignerItem signal;

        [Display(Order = 4)]
        [Editor(EditorNames.BoardNetDesignerItemEditor, EditorNames.BoardNetDesignerItemEditor)]
        [DisplayName]
        [MarksDirty]
        public IBoardNetDesignerItem Signal
        {
            get
            {
                return signal;
            }
            set
            {
                var setSignal = value;
                //the signal must exist
                if (setSignal != null)
                {
                    var brd = LayerDocument as IBoardDesigner;
                    var s = brd.NetList.FirstOrDefault(n => n.Name == setSignal.Name);
                    if (s == null)
                        setSignal = null;
                }


                if (setSignal == null && signal != null)
                    signal.Items.Remove(this);

                signal = setSignal;

                if (signal != null && !signal.Items.Contains(this))
                    signal.Items.Add(this);

                OnPropertyChanged(nameof(Signal));
            }
        }

        public void AssignSignal(IBoardNetDesignerItem newSignal)
        {
            signal = newSignal;
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

        IGeometry regionGeometry;
        [Browsable(false)]
        public IGeometry RegionGeometry
        {
            get { return regionGeometry; }
            set
            {
                regionGeometry = value;
                OnPropertyChanged(nameof(RegionGeometry));
            }
        }

        IDebounceDispatcher debounceRepurPoly;//= new DebounceDispatcher();
        public async Task RepourPolygonAsync()
        {
            var thisBoard = LayerDocument as IBoardDesigner;

            var isEditing = thisBoard != null && (thisBoard as IFileBaseViewModel).State == DocumentState.IsEditing;

            if (isEditing)
            {
                var proc = ServiceProvider.Resolve<IPlaneGeometryPourProcessor>();
                var dispatcher = ServiceProvider.Resolve<IDispatcherHelper>();

                await Task.Run(() => proc.GetGeometry(this, thisBoard))
                    .ContinueWith(t =>
                    {
                        dispatcher.RunOnDispatcher(() =>
                        RegionGeometry = new GeometryWrapper(t.Result));
                    });
            }

        }

        public override XRect GetBoundingRectangle()
        {
            if (this.LayerDocument is IBoardDesigner brd && brd.BoardOutline != null)
            {
                return brd.BoardOutline.GetBoundingRectangle();
            }

            return XRect.Empty;
        }

        public override void MirrorX()
        {
        }

        public override void MirrorY()
        {
        }

        public override void TransformBy(XMatrix matrix)
        {
        }

        public override void Translate(double dx, double dy)
        {

        }

        public override void Rotate()
        {
        }

        protected override void LoadFromPrimitiveInternal(IPrimitive primitive)
        {
            var p = (PlaneBoard)primitive;

            IsFilled = p.IsFilled;
            LayerId = p.layerId;
            GenerateThermals = p.GenerateThermals;
            ThermalWidth = p.ThermalWidth;
            IsLocked = p.IsLocked;
        }

        public override IPrimitive SaveToPrimitive()
        {
            var p = new PlaneBoard();

            p.IsFilled = IsFilled;
            p.layerId = (Layer?.LayerId).GetValueOrDefault();
            p.GenerateThermals = GenerateThermals;
            p.ThermalWidth = ThermalWidth;
            p.IsLocked = IsLocked;

            return p;
        }

        public override void RemoveFromCanvas()
        {
            Signal = null;

            base.RemoveFromCanvas();
        }

        public override bool CanClone => false;

        public override object Clone()
        {
            return this;
        }

        public IList<IItemWithClearance> GetExcludedItems()
        {
            var returnItems = new List<IItemWithClearance>();

            var thisBoard = LayerDocument as IBoardDesigner;

            var isEditing = thisBoard != null && (thisBoard as IFileBaseViewModel).State == DocumentState.IsEditing;

            if (IsPlaced
                && IsFilled
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

                            ////we cut from this poly if other poly draw order is lower
                            //if (item is PolygonBoardCanvasItem otherPoly
                            //    && otherPoly.PolygonType != PolygonType.Keepout
                            //    && DrawOrder <= otherPoly.DrawOrder)
                            //{
                            //    continue;
                            //}

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
    }
}
