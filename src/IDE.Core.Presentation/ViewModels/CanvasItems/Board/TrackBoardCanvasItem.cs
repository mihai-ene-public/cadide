using IDE.Core.Converters;
using IDE.Core.Interfaces;
using IDE.Core.Presentation.Utilities;
using IDE.Core.Storage;
using IDE.Core.Types.Attributes;
using IDE.Core.Types.Media;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace IDE.Core.Designers
{
    public class TrackBoardCanvasItem : SingleLayerBoardCanvasItem
                                         , ITrackBoardCanvasItem
    {

        public TrackBoardCanvasItem()
        {
            Width = 0.254;

            //ensure 2 points
            Points.Add(new XPoint());
            Points.Add(new XPoint());

            PropertyChanged += TrackBoardCanvasItem_PropertyChanged;
        }

        private void TrackBoardCanvasItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(Points):
                case nameof(Width):
                case nameof(Signal):
                case nameof(ShowSignalName):
                    UpdateSignalNames();
                    break;
            }
        }

        void UpdateSignalNames()
        {
            var segments = this.GetSegments();

            var sNames = new List<TrackSignalNameItem>();

            if (showSignalName && signal != null && !string.IsNullOrEmpty(signal.Name))
            {
                foreach (var seg in segments)
                {
                    //a rect that would have the segment horizontal with rect width being the segment length
                    var w = (seg.EndPoint - seg.StartPoint).Length;
                    var h = width;
                    var fontSize = CanvasTextHelper.GetFontSize(signal.Name, new XSize(w, h), false);

                    if (fontSize < 0.01d)
                        continue;

                    var sizeText = CanvasTextHelper.GetSizeForPadText(signal.Name, fontSize);
                    var pos = GetSignalNamePosition(seg, fontSize, fontSize * signal.Name.Length,sizeText);
                    sNames.Add(new TrackSignalNameItem
                    {
                        Name = signal.Name,
                        FontSize = fontSize,
                        Position = pos,
                        Width=sizeText.Width,
                        Height=sizeText.Height
                    });
                }
            }

            SignalNames = sNames;
        }

        IPosition GetSignalNamePosition(XLineSegment seg, double fontSize, double textLength, XSize sizeText)
        {
            var sp = seg.StartPoint;
            var ep = seg.EndPoint;
            var newRot = 0.0d;


            if (sp.X == ep.X)
            {
                newRot = -90;
            }

            else if (sp.Y == ep.Y)
            {
                newRot = 0.0d;
            }
            else
            {
                var rads = Math.Atan2(ep.Y - sp.Y, ep.X - sp.X);
                newRot = rads * 180 / Math.PI;
            }

            if (newRot > 90.0d)
                newRot -= 180.0;
            if (newRot < -90.0d)
                newRot += 180;

           

           // var adjY = d;

            //if (width <= 0.5d)
            //    adjY = 0.5;

            //var newPos = new Point((0.5 * (sp.X + ep.X - 0.5 * sizeText.Width)), (0.5 * (sp.Y + ep.Y - 0.5 * sizeText.Height)));// - 0.3 * width));
            //var newPos = new Point((0.5 * (sp.X + ep.X - 0.5 * sizeText.Width)), (0.5 * (sp.Y + ep.Y - 0.5 * sizeText.Height)) - 0.1 * sizeText.Height);// - 0.3 * width));
            //var newPos = new Point((0.5 * (sp.X + ep.X - 0.5 * sizeText.Width)), (0.5 * (sp.Y + ep.Y - 0.5* width)) - adjY * width);// - 0.3 * width));
            var newPos = new XPoint((0.5 * (sp.X + ep.X - 0.5 * sizeText.Width)), (0.5 * (sp.Y + ep.Y)) - 0.75 * fontSize);// - 0.3 * width));
            //var newPos = new Point((0.5 * (sp.X + ep.X)), (0.5 * (sp.Y + ep.Y)));
            var tg = new XTransformGroup();
            var mp = new XPoint(0.5 * (sp.X + ep.X), 0.5 * (sp.Y + ep.Y));
            var rotateTransform = new XRotateTransform(newRot)
            {
                CenterX = mp.X,
                CenterY = mp.Y
            };

            tg.Children.Add(rotateTransform);
            newPos = tg.Value.Transform(newPos);

            return new PositionData
            {
                X = newPos.X,
                Y = newPos.Y,
                Rotation = newRot
            };
        }

        List<TrackSignalNameItem> signalNames;

        [Browsable(false)]
        public List<TrackSignalNameItem> SignalNames
        {
            get { return signalNames; }
            set
            {
                signalNames = value;
                OnPropertyChanged(nameof(SignalNames));
            }
        }

       

        double width;

        /// <summary>
        /// Width of the wire in mm
        /// </summary>
        [Description("Width (thickness) of the line (supports expressions)")]
        [Editor(EditorNames.SizeMilimetersUnitsEditor, EditorNames.SizeMilimetersUnitsEditor)]
        [Display(Order = 2)]
        [MarksDirty]
        public double Width
        {
            get
            {
                return width;
            }
            set
            {
                width = value;
                OnPropertyChanged(nameof(Width));
                //OnPropertyChanged(nameof(SignalNameFontSize));
                //OnPropertyChanged(nameof(SignalNamePosition));
            }
        }


        IBoardNetDesignerItem signal;

        [Display(Order = 3)]
        [MarksDirty]
        public IBoardNetDesignerItem Signal
        {
            get
            {
                return signal;
            }
            set
            {
                if (signal != null)
                    signal.Items.Remove(this);

                signal = value;

                if (signal != null)
                    signal.Items.Add(this);

                OnPropertyChanged(nameof(Signal));
            }
        }

        bool showSignalName = true;

        [Browsable(true)]
        [Display(Order = 4)]
        [MarksDirty]
        public bool ShowSignalName
        {
            get { return showSignalName; }
            set
            {
                showSignalName = value;
                OnPropertyChanged(nameof(ShowSignalName));
            }
        }

        public void AssignSignal(IBoardNetDesignerItem newSignal)
        {
            signal = newSignal;
        }

        /// <summary>
        /// list of Points that make our polyline
        /// </summary>
        //  [Browsable(false)]
        // public PointCollection Points { get; set; } = new PointCollection();

        IList<XPoint> points = new ObservableCollection<XPoint>();

        [Browsable(false)]
        public IList<XPoint> Points
        {
            get
            {
                return points;
            }
            set
            {
                points = new ObservableCollection<XPoint>(value);

                OnPropertyChanged(nameof(Points));
            }
        }

        [Browsable(false)]
        public XPoint StartPoint
        {
            get { return Points[0]; }
            set { Points[0] = value; }
        }

        [Browsable(false)]
        public XPoint EndPoint
        {
            get { return Points[Points.Count - 1]; }
            set { Points[Points.Count - 1] = value; }
        }


        [Browsable(false)]
        public IList<XPoint> SelectedPoints
        {
            get
            {
                var sp = new List<XPoint>();

                if (selectedSegmentStart >= 0
                    && selectedSegmentEnd >= 0
                    && selectedSegmentStart <= selectedSegmentEnd
                    )
                {
                    for (int i = selectedSegmentStart; i <= selectedSegmentEnd; i++)
                    {
                        sp.Add(points[i]);
                    }

                    sp.Add(points[selectedSegmentEnd + 1]);
                }

                return sp;
            }
        }

        int selectedSegmentStart = -1;
        int selectedSegmentEnd = -1;

        [Browsable(false)]
        public int SelectedSegmentStart => selectedSegmentStart;


        [Browsable(false)]
        public int SelectedSegmentEnd => selectedSegmentEnd;

        

        [Browsable(false)]
        public int SegmentCount => Points.Count - 1;

        private bool IsSegmentIndexValid(int segmentIndex)
        {
            return segmentIndex >= 0 && segmentIndex < points.Count - 1;
        }

        public bool IsSegmentSelected(int segmentIndex)
        {
            return segmentIndex >= selectedSegmentStart && segmentIndex <= selectedSegmentEnd;
        }


        protected override void SetIsSelectedInternal(bool value)
        {
            base.SetIsSelectedInternal(value);
            if (value == false)
                ClearSelection();
        }



        public bool HasSelectedSegments()
        {
            return selectedSegmentStart >= 0 && selectedSegmentEnd >= 0;
        }

        public override void ToggleSelect()
        {
            var selected = selectedSegmentStart >= 0 || selectedSegmentEnd >= 0;
            SetIsSelectedInternal(!selected);
        }
        public void SelectSegment(int segmentIndex)
        {
            if (!IsSegmentIndexValid(segmentIndex))
                return;

            selectedSegmentStart = segmentIndex;
            selectedSegmentEnd = segmentIndex;

            SelectionNotify();
        }

        public void SelectSegmentAppend(int segmentIndex)
        {
            if (!IsSegmentIndexValid(segmentIndex))
                return;

            if (selectedSegmentStart == -1)
                selectedSegmentStart = segmentIndex;
            if (selectedSegmentEnd == -1)
                selectedSegmentEnd = segmentIndex;

            if (segmentIndex < selectedSegmentStart)
                selectedSegmentStart = segmentIndex;
            if (segmentIndex > selectedSegmentEnd)
                selectedSegmentEnd = segmentIndex;

            SelectionNotify();
        }



        public void SelectSegmentAtPosition(XPoint mousePositionMM)
        {
            var selectedSegment = this.GetSegmentAtMousePosition(mousePositionMM);

            SelectSegment(selectedSegment);
        }

        public void ToggleSelectSegmentAppendAtPosition(XPoint mousePositionMM)
        {
            var selectedSegment = this.GetSegmentAtMousePosition(mousePositionMM);

            var selected = IsSegmentSelected(selectedSegment);
            if (selected)
            {
                //currently we clear the selection
                //but we could just remove current segment from selection which is more complicated (depends on what is already selected)
                //ClearSelection();//?
            }
            else
            {
                SelectSegmentAppend(selectedSegment);
            }

            SetIsSelectedInternal(!selected);
        }
        public void ClearSelection()
        {
            selectedSegmentStart = -1;
            selectedSegmentEnd = -1;

            SelectionNotify();
        }

        private void SelectionNotify()
        {
            OnPropertyChanged(nameof(SelectedPoints));
        }

       


        //public void RemoveSelectedSegment()
        //{
        //    var brd = LayerDocument as IBoardModel;
        //    var canvasModel = brd.CanvasModel;

        //    if (SelectedSegment >= 0)
        //    {
        //        var segCount = Points.Count - 1;
        //        if (SelectedSegment == 0)
        //        {
        //            Points.RemoveAt(0);
        //        }
        //        else if (SelectedSegment == segCount - 1)
        //        {
        //            Points.RemoveAt(Points.Count - 1);
        //        }
        //        else
        //        {
        //            //remove the segment, split into 2 new tracks; remove this from canvas

        //            var track1 = new TrackBoardCanvasItem()
        //            {
        //                LayerDocument = LayerDocument,
        //                ParentObject = ParentObject,
        //                Layer = Layer,
        //                Signal = Signal,
        //                Width = Width,
        //                IsPlaced = true
        //            };
        //            track1.Points.Clear();
        //            for (int pIndex = 0; pIndex <= SelectedSegment; pIndex++)
        //            {
        //                track1.Points.Add(Points[pIndex]);
        //            }
        //            // track1.Points.Add(splitPoint);


        //            var track2 = new TrackBoardCanvasItem()
        //            {
        //                LayerDocument = LayerDocument,
        //                ParentObject = ParentObject,
        //                Layer = Layer,
        //                Signal = Signal,
        //                Width = Width,
        //                IsPlaced = true
        //            };
        //            track2.Points.Clear();

        //            //track2.Points.Add(splitPoint);

        //            for (int pIndex = SelectedSegment + 1; pIndex < Points.Count; pIndex++)
        //            {
        //                track2.Points.Add(Points[pIndex]);
        //            }

        //            canvasModel.AddItem(track1);
        //            canvasModel.AddItem(track2);

        //            //remove the track
        //            canvasModel.RemoveItem(this);
        //        }

        //        OnPropertyChanged(nameof(Points));

        //        if (Points.Count <= 1)
        //        {
        //            canvasModel.RemoveItem(this);
        //        }
        //    }
        //    else
        //    {
        //        canvasModel.ClearSelectedItems();
        //        canvasModel.UpdateSelection();
        //        canvasModel.RemoveItem(this);
        //    }


        //    //clear selected segment
        //    SelectedSegment = -1;
        //}

        

       



        protected override void LoadFromPrimitiveInternal(IPrimitive primitive)
        {
            var p = (TrackBoard)primitive;

            Points.Clear();
            var vertices = p.Points.Select(v => new XPoint(v.x, v.y)).ToList();
            Points.AddRange(vertices);
            Width = p.width;
            LayerId = p.layerId;
            IsLocked = p.IsLocked;

        }

        public override IPrimitive SaveToPrimitive()
        {
            var p = new TrackBoard();

            p.Points = Points.Select(v => new Vertex { x = v.X, y = v.Y }).ToList();
            p.width = Width;
            p.layerId = (Layer?.LayerId).GetValueOrDefault();
            p.IsLocked = IsLocked;

            return p;
        }

        public override XRect GetBoundingRectangle()
        {
            var minPoint = Points.FirstOrDefault();
            var maxPoint = Points.FirstOrDefault();

            foreach (var point in Points)
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

            var r = new XRect(minPoint, maxPoint);
            r.Inflate(0.5 * width, 0.5 * width);
            return r;
        }

        public override void Translate(double dx, double dy)
        {
            for (int i = 0; i < Points.Count; i++)
            {
                var p = Points[i];
                p.Offset(dx, dy);
                Points[i] = p;
            }
            OnPropertyChanged(nameof(Points));
            OnPropertyChanged(nameof(SelectedPoints));
        }

        public override void MirrorX()
        {
            //ScaleX *= -1;
        }

        public override void MirrorY()
        {
           // ScaleY *= -1;
        }

        public override void TransformBy(XMatrix matrix)
        {
            for (int i = 0; i < Points.Count; i++)
            {
                var p = Points[i];
                p = matrix.Transform(p);
                Points[i] = p.Round();
            }
            OnPropertyChanged(nameof(Points));
        }

        public override void Rotate()
        {
            var mp = new XPoint();
            foreach (var p in Points)
            {
                mp.Offset(p.X, p.Y);
            }
            mp.X /= Points.Count;
            mp.Y /= Points.Count;

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

        public override string ToString()
        {
            var netName = "None";
            if (signal != null)
                netName = signal.Name;

            return $"Track ({netName} - ({StartPoint}; {EndPoint}))";
        }
    }

    public class TrackSignalNameItem
    {
        public string Name { get; set; }

        public double FontSize { get; set; }

        public IPosition Position { get; set; }

        public int TextZIndex => 5000;

        public double Width { get; set; }
        public double Height { get; set; }
    }

    static class CanvasTextHelper
    {
        public static XSize GetSizeForPadText(string text, double fontSize)
        {
            var sLen = 1.0d;
            if (!string.IsNullOrEmpty(text))
                sLen = text.Length;


            var baseFontSize = 0.8;
            var left = 0.08 * fontSize / baseFontSize;
            var right = 0.08 * fontSize / baseFontSize;
            var top = 0.3 * fontSize / baseFontSize;
            var bottom = 0.18 * fontSize / baseFontSize;

            var w = sLen * (fontSize + left + right);
            var h = fontSize + top + bottom;

            //wpf rounds off to 1/96 points for width and height
            //so we must convert to dpi, round the value and then back to mm
            var dotsWidth = Math.Round(MilimetersToDpiHelper.ConvertToDpi(w));
            var dotsHeight = Math.Round(MilimetersToDpiHelper.ConvertToDpi(h));
            if (dotsWidth < 1.00d)
                dotsWidth = 1.00d;
            if (dotsHeight < 1.00d)
                dotsHeight = 1.00d;
            w = MilimetersToDpiHelper.ConvertToMM(dotsWidth);
            h = MilimetersToDpiHelper.ConvertToMM(dotsHeight);

            return new XSize(w, h);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="availableSize"></param>
        /// <param name="hasSignal">if true will return half the size</param>
        /// <returns></returns>
        public static double GetFontSize(string text, XSize availableSize, bool hasSignal)
        {
            var sizeFactor = 0.8d;
            var sLen = 1.0d;
            if (!string.IsNullOrEmpty(text))
                sLen = text.Length;
            // var fontSize = sizeFactor * Math.Min(availableSize.Width, availableSize.Height) / sLen;

            var fontSize = sizeFactor * availableSize.Height;
            var textLen = fontSize * sLen;
            if (textLen > availableSize.Width)
            {
                var scale = availableSize.Width / textLen;
                fontSize *= scale;
            }
            //if(fontSize>availableSize.Height)
            //{
            //    fontSize=availableSize.Height
            //}

            if (hasSignal)
                return 0.5 * fontSize;

            return fontSize;
        }
    }
}
