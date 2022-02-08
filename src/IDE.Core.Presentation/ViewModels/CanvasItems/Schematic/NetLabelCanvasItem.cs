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
    //TODO
    /*NETLABEL
		textColor
		backColor
		x,y
		orientation
		NetName
		font stuff
			fontfamily, fontSize
			bold, italic, underline
      */
    public class NetLabelCanvasItem : NetSegmentCanvasItem
                                      , ILabelCanvasItem

    {
        public NetLabelCanvasItem()
        {
            TextColor = XColors.White;
            TextDecoration = TextDecorationEnum.None;
            FontSize = 8;
            FontFamily = "Segoe UI"; //new FontFamily("Segoe UI");

            PropertyChanged += NetLabelCanvasItem_PropertyChanged;
        }

        void NetLabelCanvasItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Net.Name" || e.PropertyName == nameof(Net))
                OnPropertyChanged(nameof(NetName));
        }



        [Display(Order = 1)]
        [DisplayName("Net (this)")]
        [Description("Choose an existing net or create a new one for the net segment this label belongs to")]
        [Browsable(true)]
        [Editor(EditorNames.NetDesignerItemEditor, EditorNames.NetDesignerItemEditor)]
        [MarksDirty]
        public string NetSegmentName
        {
            get { return NetName; }
            set
            {
                if (NetName == value)
                    return;

                var canvasModel = GetCurrentCanvas();
                var netManager = canvasModel.GetNetManager();
                //we want the net with lowest id (the oldest)
                //var nets = canvasModel.Items.OfType<NetSegmentCanvasItem>().Where(n => n.Net != null).Select(n => n.Net).OrderBy(n => n.Id).ToList();
                //var newNet = nets.FirstOrDefault(n => n.Name == value);
                var newNet = netManager.Get(value);

                //if no net was found, it means we want to create a new net
                //if (newNet == null)
                //    newNet = new SchematicNet
                //    {
                //        Id = LibraryItem.GetNextId(),
                //        Name = value
                //    };
                if (newNet == null)
                    newNet = netManager.Add(value);

                //build the segment this label belongs to
                var segmentsNet = new List<NetSegmentCanvasItem>();
                //we need the same reference (not the net name) because it is most probable this is a segment or portion of it
                var candidateSegments = canvasModel.Items.OfType<NetSegmentCanvasItem>().Where(n => n.Net == Net).ToList();
                //get closest wire to this label
                var closestWire = GetClosestNetWire(candidateSegments.OfType<NetWireCanvasItem>().ToList());

                //foreach net segment item change the reference of the net
                var segmentItems = BuildNetSegment(canvasModel, closestWire, candidateSegments);
                segmentItems.Add(this);

                //unhighlight
                HighlightOwnedNet(false);

                foreach (var item in segmentItems)
                    item.Net = (SchematicNet)newNet;

                //Parent.ClearSelectedItems();//?
                HighlightOwnedNet(true);

                OnPropertyChanged(nameof(NetSegmentName));
                OnPropertyChanged(nameof(NetName));
            }
        }

        IDrawingViewModel GetCurrentCanvas()
        {
            var canvasDoc = DocumentHelper.GetCurrentDocument<ICanvasDesignerFileViewModel>();
            return canvasDoc?.CanvasModel;
        }

        [Display(Order = 2)]
        [DisplayName("Net name (all)")]
        [Description("Rename the entire net with the specified name")]
        [MarksDirty]
        public string NetName
        {
            get
            {
                if (Net != null)
                    return Net.Name;
                return "Not assigned";
            }
            set
            {
                if (Net == null)
                    throw new Exception("Label does not belong to any net");

                var canvasModel = GetCurrentCanvas();
                if (canvasModel != null)
                {
                    var netsToRename = canvasModel.Items.OfType<NetSegmentCanvasItem>().Where(n => n.Net != null && n.Net.Name == Net.Name)
                                                      .Select(n => n.Net).ToList();
                    netsToRename.ForEach(n => n.Name = value);
                }
                else
                {
                    Net.Name = value;
                }

                OnPropertyChanged(nameof(NetName));
                OnPropertyChanged(nameof(NetSegmentName));
            }
        }

        [Browsable(false)]
        public string Text { get { return NetName; } }

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

        NetWireCanvasItem GetClosestNetWire(IList<NetWireCanvasItem> wires)
        {
            NetWireCanvasItem wire = null;
            const double minDistance = 3.5;//mm
            double lastDistance = 1e6;

            var fontSizeMM = MilimetersToDpiHelper.ConvertToMM(fontSize);

            foreach (var netwire in wires)
            {
                var lblPoint = new XPoint(X, Y + fontSizeMM);
                foreach(var seg in netwire.GetSegments())
                {
                    var point = seg.GetPointDistanceToSegment(lblPoint);
                    var distance = (lblPoint - point).Length;
                    //var distance = LineToPointDistance((ILineCanvasItem)netwire, );
                    if (distance <= minDistance && distance < lastDistance)
                    {
                        lastDistance = distance;
                        wire = netwire;
                    }
                }
                
            }

            return wire;
        }

        List<NetSegmentCanvasItem> BuildNetSegment(IDrawingViewModel canvasModel, NetWireCanvasItem startingWire, IList<NetSegmentCanvasItem> candidates)
        {
            var returnSegment = new List<NetSegmentCanvasItem>();
            if (startingWire == null)
                return returnSegment;

            returnSegment.Add(startingWire);
            var added = true;//just added to segment

            //wires and junctions
            var sourceCandidates = new List<NetSegmentCanvasItem>();
            //we must first add the wires then the junctions
            sourceCandidates.AddRange(candidates.OfType<NetWireCanvasItem>());
            sourceCandidates.AddRange(candidates.OfType<JunctionCanvasItem>());

            sourceCandidates.Remove(startingWire);

            while (added)
            {
                var toAdd = new List<NetSegmentCanvasItem>();

                foreach (var thisSegmentItem in returnSegment)
                {
                    var intersections = sourceCandidates.Where(n => GeometryHelper.Intersects(thisSegmentItem, n)).ToList();
                    toAdd.AddRange(intersections);
                }
                returnSegment.AddRange(toAdd);
                toAdd.ForEach(n => sourceCandidates.Remove(n));

                added = toAdd.Count > 0;
            }

            //pin refs. Check if any of pins in this net intersect with any of elements in return segment
            var hitPins = new List<PinCanvasItem>();

            var netLines = returnSegment.OfType<NetWireCanvasItem>().ToList();
            var points = new List<XPoint>();
            netLines.ForEach(n =>
            {
                points.Add(n.StartPoint);
                points.Add(n.EndPoint);
            });
            points = points.Distinct().ToList();

            var width = 0.5;

            var pinHelper = new PinCanvasItemHelper(canvasModel);

            foreach (var point in points)
            {
                var hp = pinHelper.GetPinsCollision(point, width);
                hitPins.AddRange(hp);
            }

            returnSegment.AddRange(hitPins);

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
            var rect = TextHelper.MeasureString(Text, fontFamily, fontSize);
            rect.Offset(x, y);
            rect.Width = Math.Round(rect.Width, 4);
            rect.Height = Math.Round(rect.Height, 4);

            return rect;
        }

        protected override void LoadFromPrimitiveInternal(IPrimitive primitive)
        {
            var t = (NetLabel)primitive;

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
            var t = new NetLabel();

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
            return $"Label ({NetName})";
        }
    }
}
