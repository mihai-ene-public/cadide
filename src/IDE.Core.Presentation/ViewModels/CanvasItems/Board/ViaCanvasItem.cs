using IDE.Core.Interfaces;
using IDE.Core.Storage;
using System.ComponentModel;
using System.Linq;
using IDE.Core.Types.Media;
using System.ComponentModel.DataAnnotations;
using IDE.Core.Types.Attributes;

namespace IDE.Core.Designers
{
    public class ViaCanvasItem : BoardCanvasItemViewModel, ISignalPrimitiveCanvasItem, IViaCanvasItem
    {

        public ViaCanvasItem()
        {
            Drill = 0.3;
            Diameter = 0.7;
            SolderMaskExpansion = 0.1;
        }

        //Hole
        //diameter
        //x, y (center)
        //Drill pair (from layer to layer)
        //net
        //solder mask expansion (top same as bottom)
        //force tenting on top
        //force tenting on bottom


        double diameter;

        [Editor(EditorNames.SizeMilimetersUnitsEditor, EditorNames.SizeMilimetersUnitsEditor)]
        [Display(Order = 2)]
        [MarksDirty]
        public double Diameter
        {
            get { return diameter; }
            set
            {
                diameter = value;
                OnPropertyChanged(nameof(Diameter));
                OnPropertyChanged(nameof(SignalNameFontSize));
                OnPropertyChanged(nameof(SignalNamePosition));
                OnPropertyChanged(nameof(Radius));
                OnPropertyChanged(nameof(SolderMaskDiameter));
            }
        }

        [Browsable(false)]
        public double Radius
        {
            get
            {
                return Diameter / 2;
            }
        }

        double drill;

        [Editor(EditorNames.SizeMilimetersUnitsEditor, EditorNames.SizeMilimetersUnitsEditor)]
        [Display(Order = 3)]
        [MarksDirty]
        public double Drill
        {
            get { return drill; }
            set
            {
                drill = value;

                OnPropertyChanged(nameof(Drill));
                OnPropertyChanged(nameof(DrillRadius));
            }
        }

        [Browsable(false)]
        public double DrillRadius
        {
            get
            {
                return Drill / 2;
            }
        }

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
                //if (signal == value)
                //    return;

                if (signal != null)
                    signal.Items.Remove(this);

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

        PositionData signalNamePosition = new PositionData();
        [Browsable(false)]
        public PositionData SignalNamePosition
        {
            get
            {
                if (SignalNameIsVisible)
                {
                    signalNamePosition.X = -0.25 * GetSignalTextLength();
                    signalNamePosition.Y = -0.25 * Radius;
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
                var d = 0.8 * diameter;

                if (SignalNameIsVisible)
                    d = 1.0 * diameter / Signal.Name.Length;
                return d;
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

        double solderMaskExpansion;

        [Editor(EditorNames.SizeMilimetersUnitsEditor, EditorNames.SizeMilimetersUnitsEditor)]
        [Display(Order = 5)]
        [MarksDirty]
        public double SolderMaskExpansion
        {
            get { return solderMaskExpansion; }
            set
            {
                solderMaskExpansion = value;
                OnPropertyChanged(nameof(SolderMaskExpansion));
                OnPropertyChanged(nameof(SolderMaskDiameter));
            }
        }

        [Browsable(false)]
        public double SolderMaskDiameter
        {
            get { return Diameter + SolderMaskExpansion * 2; }
        }

        private bool tentViaOnTop;

        [Display(Order = 5)]
        [Description("Check to cover via (tenting) with soldermask on top")]
        [MarksDirty]
        public bool TentViaOnTop
        {
            get { return tentViaOnTop; }
            set
            {
                tentViaOnTop = value;
                OnPropertyChanged(nameof(TentViaOnTop));
            }
        }

        private bool tentViaOnBottom;

        [Display(Order = 5)]
        [Description("Check to cover via (tenting) with soldermask on bottom")]
        [MarksDirty]
        public bool TentViaOnBottom
        {
            get { return tentViaOnBottom; }
            set
            {
                tentViaOnBottom = value;
                OnPropertyChanged(nameof(TentViaOnBottom));
            }
        }


        double x;

        /// <summary>
        /// Location X of rectangle in current coordinate system
        /// </summary>
        [Editor(EditorNames.PositionXUnitsEditor, EditorNames.PositionXUnitsEditor)]
        [Display(Order = 6)]
        [MarksDirty]
        public double X
        {
            get { return x; }
            set
            {
                x = value;
                OnPropertyChanged(nameof(X));
                OnPropertyChanged(nameof(Center));
                OnPropertyChanged(nameof(SignalNamePosition));
            }
        }

        double y;

        /// <summary>
        /// Location X of rectangle in current coordinate system
        /// </summary>
        [Editor(EditorNames.PositionYUnitsEditor, EditorNames.PositionYUnitsEditor)]
        [Display(Order = 7)]
        [MarksDirty]
        public double Y
        {
            get { return y; }
            set
            {
                y = value;
                OnPropertyChanged(nameof(Y));
                OnPropertyChanged(nameof(Center));
                OnPropertyChanged(nameof(SignalNamePosition));
            }
        }

        [Browsable(false)]
        public XPoint Center
        {
            get
            {
                return new XPoint(X, Y);
            }
        }

        [Editor(EditorNames.DrillPairEditor, EditorNames.DrillPairEditor)]
        [DisplayName("Drill pair")]
        [Display(Order = 8)]
        [MarksDirty]
        public ILayerPairModel DrillPair
        {
            get; set;
        }

        public override void Translate(double dx, double dy)
        {
            X += dx;
            Y += dy;
        }

        public override XRect GetBoundingRectangle()
        {
            return new XRect(X - Diameter * 0.5, Y - Diameter * 0.5, Diameter, Diameter);
        }

        public void AssignOnDrillPair(ILayerDesignerItem layer1, ILayerDesignerItem layer2)
        {
            var layerDoc = LayerDocument;
            ILayerPairModel drillPair = null;

            if (layerDoc is IBoardDesigner brd)
            {
                //we first search for an existing drill pair in our document (board)
                drillPair = brd.DrillPairs.FirstOrDefault(d => (d.LayerStart.LayerId == layer1.LayerId && d.LayerEnd.LayerId == layer2.LayerId)
                                                            || (d.LayerStart.LayerId == layer2.LayerId && d.LayerEnd.LayerId == layer1.LayerId));


                if (drillPair == null)
                {
                    //no drill pairs found; assign the top-bottom (this should exist by default)
                    drillPair = brd.DrillPairs.FirstOrDefault(d => d.LayerStart.LayerId == LayerConstants.SignalTopLayerId && d.LayerEnd.LayerId == LayerConstants.SignalBottomLayerId);
                }
            }

            DrillPair = drillPair;
        }

        private int startLayer;
        private int endLayer;
        protected override void LoadFromPrimitiveInternal(IPrimitive primitive)
        {
            var t = (Via)primitive;

            Drill = t.drill;
            Diameter = t.diameter;
            X = t.x;
            Y = t.y;
            TentViaOnTop = t.TentViaOnTop;
            TentViaOnBottom = t.TentViaOnBottom;
            IsLocked = t.IsLocked;

            startLayer = t.startLayer;
            endLayer = t.endLayer;
        }

        void LoadDrillPair()
        {
            var layerDoc = LayerDocument;
            ILayerPairModel drillPair = null;

            if (layerDoc is IBoardDesigner brd)
            {
                //we first search for an existing drill pair in our document (board)
                drillPair = brd.DrillPairs.FirstOrDefault(d => d.LayerStart.LayerId == startLayer && d.LayerEnd.LayerId == endLayer);
            }
            if (drillPair == null)
            {
                drillPair = new LayerPairModel
                {
                    LayerStart = layerDoc.LayerItems.FirstOrDefault(l => l.LayerId == startLayer),
                    LayerEnd = layerDoc.LayerItems.FirstOrDefault(l => l.LayerId == endLayer),
                };
            }

            DrillPair = drillPair;
        }

        public override IPrimitive SaveToPrimitive()
        {
            var via = new Via();

            via.diameter = Diameter;
            via.drill = Drill;
            via.x = X;
            via.y = Y;
            via.TentViaOnTop = TentViaOnTop;
            via.TentViaOnBottom = TentViaOnBottom;

            via.IsLocked = IsLocked;

            if (DrillPair != null)
            {
                if (DrillPair.LayerStart != null)
                    via.startLayer = DrillPair.LayerStart.LayerId;
                if (DrillPair.LayerEnd != null)
                    via.endLayer = DrillPair.LayerEnd.LayerId;
            }

            return via;
        }

        public override void MirrorX()
        {
        }

        public override void MirrorY()
        {
        }
        public override void TransformBy(XMatrix matrix)
        {
            var c = matrix.Transform(new XPoint(X, Y));

            X = c.X;
            Y = c.Y;
        }

        protected override XTransform GetLocalTranslationTransform()
        {
            return new XTranslateTransform(X, Y);
        }

        public override void Rotate()
        {
        }

        public override void RemoveFromCanvas()
        {
            Signal = null;
        }

        public override void LoadLayers()
        {
            LoadDrillPair();
        }

        public override string ToString()
        {
            var netName = "None";
            if (signal != null)
                netName = signal.Name;
            return $"Via ({Center} - {netName})";
        }
    }
}
