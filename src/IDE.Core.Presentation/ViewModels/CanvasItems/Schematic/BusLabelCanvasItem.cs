using IDE.Core.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using IDE.Core.Converters;
using IDE.Core.Interfaces;
using IDE.Core.Types.Media;
using IDE.Core.Presentation.Placement;
using System.ComponentModel.DataAnnotations;
using IDE.Core.Types.Attributes;
using IDE.Core.Presentation.Utilities;

namespace IDE.Core.Designers
{
    public class BusLabelCanvasItem : BusSegmentCanvasItem, ILabelCanvasItem
    {
        public BusLabelCanvasItem()
        {

            TextColor = XColors.White;
            TextDecoration = TextDecorationEnum.None;
            FontSize = 8;
            FontFamily = "Segoe UI";

            PropertyChanged += BusLabelCanvasItem_PropertyChanged;
        }

        void BusLabelCanvasItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Bus.Name" || e.PropertyName == nameof(Bus))
                OnPropertyChanged(nameof(BusName));
        }

        [Display(Order = 1)]
        [DisplayName("Bus (this)")]
        [Description("Choose an existing bus or create a new one for the bus segment this label belongs to")]
        [Browsable(true)]
        [Editor(EditorNames.BusDesignerItemEditor, EditorNames.BusDesignerItemEditor)]
        [MarksDirty]
        public string BusSegmentName
        {
            get { return BusName; }
            set
            {
                if (BusName == value)
                    return;

                var canvasModel = GetCurrentCanvas();
                var busManager = canvasModel.GetBusManager();

                var newBus = busManager.Get(value);
                //if no net was found, it means we want to create a new net
                if (newBus == null)
                    newBus = busManager.Add(value);

                //build the segment this label belongs to
                var segmentsNet = new List<BusSegmentCanvasItem>();
                //we need the same reference (not the net name) because it is most probable this is a segment or portion of it
                var candidateSegments = canvasModel.Items.OfType<BusSegmentCanvasItem>().Where(n => n.Bus == Bus).ToList();
                //get closest wire to this label
                var closestWire = GetClosestNetWire(candidateSegments.OfType<BusWireCanvasItem>().ToList());

                //foreach net segment item change the reference of the net
                var segmentItems = BuildBusSegment(closestWire, candidateSegments);
                segmentItems.Add(this);

                //unhighlight
                HighlightOwnedBus(false);

                foreach (var item in segmentItems)
                    item.Bus = (SchematicBus)newBus;

                HighlightOwnedBus(true);

                OnPropertyChanged(nameof(BusSegmentName));
                OnPropertyChanged(nameof(BusName));
            }
        }

        [Display(Order = 2)]
        [DisplayName("Bus name (all)")]
        [Description("Rename the entire bus with the specified name")]
        [MarksDirty]
        public string BusName
        {
            get
            {
                if (Bus != null)
                    return Bus.Name;
                return "Not assigned";
            }
            set
            {
                if (Bus == null)
                    throw new Exception("Label does not belong to any bus");

                var canvasModel = GetCurrentCanvas();
                if (canvasModel != null)
                {
                    var netsToRename = canvasModel.Items.OfType<BusSegmentCanvasItem>().Where(n => n.Bus != null && n.Bus.Name == Bus.Name)
                                                      .Select(n => n.Bus).ToList();
                    netsToRename.ForEach(n => n.Name = value);
                }
                else
                {
                    Bus.Name = value;
                }

                OnPropertyChanged(nameof(BusName));
                OnPropertyChanged(nameof(BusSegmentName));
            }
        }

        IDrawingViewModel GetCurrentCanvas()
        {
            var canvasDoc = DocumentHelper.GetCurrentDocument<ICanvasDesignerFileViewModel>();
            return canvasDoc?.CanvasModel;
        }

        [Browsable(false)]
        public string Text { get { return BusName; } }

        XColor textColor;

        [Display(Order = 3)]
        [Editor(EditorNames.XColorEditor, EditorNames.XColorEditor)]
        [MarksDirty]
        public XColor TextColor
        {
            get { return textColor; }

            set
            {
                textColor = value;
                OnPropertyChanged(nameof(TextColor));
            }
        }


        string fontFamily;

        [Display(Order = 4)]
        [Editor(EditorNames.FontFamilyEditor, EditorNames.FontFamilyEditor)]
        [MarksDirty]
        public string FontFamily
        {
            get { return fontFamily; }
            set
            {
                fontFamily = value;
                OnPropertyChanged(nameof(FontFamily));
            }
        }

        double fontSize;

        [Display(Order = 5)]
        [MarksDirty]
        public double FontSize
        {
            get { return fontSize; }
            set
            {
                fontSize = value;
                OnPropertyChanged(nameof(FontSize));
            }
        }

        bool bold;

        [Display(Order = 6)]
        [MarksDirty]
        public bool Bold
        {
            get { return bold; }
            set
            {
                bold = value;
                OnPropertyChanged(nameof(Bold));
            }
        }

        bool italic;

        [Display(Order = 7)]
        [MarksDirty]
        public bool Italic
        {
            get { return italic; }
            set
            {
                italic = value;
                OnPropertyChanged(nameof(Italic));
            }
        }

        TextDecorationEnum textDecoration;

        [Display(Order = 8)]
        [MarksDirty]
        public TextDecorationEnum TextDecoration
        {
            get
            {
                return textDecoration;
            }
            set
            {
                textDecoration = value;
                OnPropertyChanged(nameof(TextDecoration));
            }
        }

        double x;

        /// <summary>
        /// Location X of rectangle in current coordinate system
        /// </summary>
        [Editor(EditorNames.PositionXUnitsEditor, EditorNames.PositionXUnitsEditor)]
        [Display(Order = 9)]
        [MarksDirty]
        public double X
        {
            get { return x; }
            set
            {
                x = value;
                OnPropertyChanged(nameof(X));
            }
        }

        double y;

        /// <summary>
        /// Location X of rectangle in current coordinate system
        /// </summary>
        [Editor(EditorNames.PositionYUnitsEditor, EditorNames.PositionYUnitsEditor)]
        [Display(Order = 10)]
        [MarksDirty]
        public double Y
        {
            get { return y; }
            set
            {
                y = value;
                OnPropertyChanged(nameof(Y));
            }
        }

        double rot;

        [Display(Order = 11)]
        [MarksDirty]
        public double Rot
        {
            get { return rot; }
            set
            {
                rot = value;
                OnPropertyChanged(nameof(Rot));
            }
        }

        double LineToPointDistance(XPoint sp, XPoint ep, XPoint point)
        {
            var A = point.X - sp.X;
            var B = point.Y - sp.Y;
            var C = ep.X - sp.X;
            var D = ep.Y - sp.Y;

            var dot = A * C + B * D;
            var len_sq = C * C + D * D;
            var param = -1.0d;
            if (len_sq != 0) //in case of 0 length line
                param = dot / len_sq;

            double xx, yy;

            if (param < 0)
            {
                xx = sp.X;
                yy = sp.Y;
            }
            else if (param > 1)
            {
                xx = ep.X;
                yy = ep.Y;
            }
            else
            {
                xx = sp.X + param * C;
                yy = sp.Y + param * D;
            }

            var dx = point.X - xx;
            var dy = point.Y - yy;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        BusWireCanvasItem GetClosestNetWire(IList<BusWireCanvasItem> wires)
        {
            BusWireCanvasItem wire = null;
            const double minDistance = 3.5;//mm
            double lastDistance = 1e6;

            var fontSizeMM = MilimetersToDpiHelper.ConvertToMM(fontSize);

            foreach (var buswire in wires)
            {
                //foreach segment
                for (int i = 0; i < buswire.Points.Count - 1; i++)
                {
                    var sp = buswire.Points[i];
                    var ep = buswire.Points[i + 1];
                    var distance = LineToPointDistance(sp, ep, new XPoint(X, Y + fontSizeMM));
                    if (distance <= minDistance && distance < lastDistance)
                    {
                        lastDistance = distance;
                        wire = buswire;
                    }
                }

            }

            return wire;
        }


        List<BusSegmentCanvasItem> BuildBusSegment(BusWireCanvasItem startingWire, IList<BusSegmentCanvasItem> candidates)
        {
            var returnSegment = new List<BusSegmentCanvasItem>();
            if (startingWire == null)
                return returnSegment;

            returnSegment.Add(startingWire);
            var added = true;//just added to segment

            var sourceCandidates = new List<BusSegmentCanvasItem>();
            //we must first add the wires then the junctions
            sourceCandidates.AddRange(candidates.OfType<BusWireCanvasItem>());

            sourceCandidates.Remove(startingWire);

            while (added)
            {
                var toAdd = new List<BusSegmentCanvasItem>();

                foreach (var thisSegmentItem in returnSegment)
                {
                    var intersections = sourceCandidates.Where(n => GeometryHelper.Intersects(thisSegmentItem, n)).ToList();
                    toAdd.AddRange(intersections);
                }
                returnSegment.AddRange(toAdd);
                toAdd.ForEach(n => sourceCandidates.Remove(n));

                added = toAdd.Count > 0;
            }
            return returnSegment;
        }

        public override void Translate(double dx, double dy)
        {
            X += dx;
            Y += dy;
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

        public override XRect GetBoundingRectangle()
        {
            //return new XRect(X, Y, 25, 2);//should calculate these by font
            var rect = TextHelper.MeasureString(Text, fontFamily, fontSize);
            rect.Offset(x, y);
            rect.Width = Math.Round(rect.Width, 4);
            rect.Height = Math.Round(rect.Height, 4);

            return rect;
        }

        protected override void LoadFromPrimitiveInternal(IPrimitive primitive)
        {
            var t = (BusLabel)primitive;

            X = t.x;
            Y = t.y;
            ScaleX = t.ScaleX;
            ScaleY = t.ScaleY;
            TextColor = XColor.FromHexString(t.textColor);
            Rot = t.rot;
            Bold = t.Bold;
            Italic = t.Italic;
            FontFamily = t.FontFamily;
            FontSize = t.FontSize;
            TextDecoration = t.TextDecoration;

        }

        public override IPrimitive SaveToPrimitive()
        {
            var t = new BusLabel();

            t.x = X;
            t.y = Y;
            t.ScaleX = ScaleX;
            t.ScaleY = ScaleY;
            t.textColor = TextColor.ToHexString();
            t.rot = Rot;
            t.Bold = Bold;
            t.Italic = Italic;
            t.FontFamily = FontFamily;//.Source;
            t.FontSize = FontSize;
            t.TextDecoration = TextDecoration;

            return t;
        }

        public override void MirrorX()
        {
            ScaleX *= -1;
        }

        public override void MirrorY()
        {
            ScaleY *= -1;
        }

        public override void Rotate()
        {
            var r = Rot;
            r += 90;
            r = ((int)r % 360);

            Rot = r;
        }

        public override string ToString()
        {
            return $"Label ({BusName})";
        }
    }
}
