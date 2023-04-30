using System.ComponentModel;
using IDE.Core.Types.Media;
using System.ComponentModel.DataAnnotations;
using IDE.Core.Interfaces;
using System.Collections.Generic;
using System.Linq;
using IDE.Core.Storage;
using System.Collections.ObjectModel;
using IDE.Core.Types.Attributes;
using IDE.Core.Presentation.Utilities;

namespace IDE.Core.Designers
{
    public class BusWireCanvasItem : BusSegmentCanvasItem, IBusWireCanvasItem
    {
        public BusWireCanvasItem() : base()
        {
            Width = 0.6;
            LineColor = XColor.FromHexString("#FF0000CD");
        }

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

        double width;

        [Editor(EditorNames.SizeMilimetersUnitsEditor, EditorNames.SizeMilimetersUnitsEditor)]
        [Display(Order = 1)]
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
            }
        }

        XColor lineColor;
        [Display(Order = 2)]
        [Editor(EditorNames.XColorEditor, EditorNames.XColorEditor)]
        [MarksDirty]
        public XColor LineColor
        {
            get { return lineColor; }

            set
            {
                lineColor = value;
                OnPropertyChanged(nameof(LineColor));
            }
        }

        public ObservableCollection<string> Nets
        {
            get { return Bus?.Nets; }
            set
            {
                if (Bus == null)
                    return;
                Bus.Nets = value;
            }
        }

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
            if (!selected)
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

        [Browsable(false)]
        public int SegmentCount => Points.Count - 1;

        [Browsable(false)]
        public int SelectedSegment { get; set; } = -1;

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

        public override void TransformBy(XMatrix matrix)
        {
            for (int i = 0; i < Points.Count; i++)
            {
                var p = Points[i];
                p = matrix.Transform(p);
                Points[i] = p;
            }
            OnPropertyChanged(nameof(Points));
        }

        public override IPrimitive SaveToPrimitive()
        {
            var wire = new BusWire();

            wire.Points = Points.Select(v => new Vertex { x = v.X, y = v.Y }).ToList();
            wire.width = Width;
            wire.LineColor = LineColor.ToHexString();
            
            return wire;
        }

        protected override void LoadFromPrimitiveInternal(IPrimitive primitive)
        {
            var wire = (BusWire)primitive;

            Points.Clear();
            var vertices = wire.Points.Select(v => new XPoint(v.x, v.y)).ToList();
            Points.AddRange(vertices);

            Width = wire.width;
            LineColor = XColor.FromHexString(wire.LineColor);

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

        public override void MirrorX()
        {
        }

        public override void MirrorY()
        {
        }

        public override void Rotate(double angle = 90)
        {
        }


        public override string ToString()
        {
            return $"Bus wire";
        }
    }
}
