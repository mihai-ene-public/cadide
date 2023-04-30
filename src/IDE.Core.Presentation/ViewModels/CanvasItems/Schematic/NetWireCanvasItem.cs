using System;
using System.Collections.Generic;
using System.Linq;
using IDE.Core.Storage;
using System.ComponentModel;
using IDE.Core.Interfaces;
using IDE.Core.Types.Media;
using IDE.Core.Presentation.Placement;
using System.ComponentModel.DataAnnotations;
using IDE.Core.Types.Attributes;
using System.Collections.ObjectModel;
using IDE.Core.Presentation.Utilities;

namespace IDE.Core.Designers
{

    public class NetWireCanvasItem : NetSegmentCanvasItem
                                   , INetWireCanvasItem
    {
        public NetWireCanvasItem() : base()
        {
            Width = 0.2;
            LineColor = XColor.FromHexString("#FF0000FF");
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

        LineStyle lineStyle;

        [Display(Order = 3)]
        [Browsable(false)]
        public LineStyle LineStyle
        {
            get { return lineStyle; }
            set
            {
                lineStyle = value;
                OnPropertyChanged(nameof(LineStyle));
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


        ////we need the start or the end of the line because we don't want to make connections with a net that we are crossing
        ////used by line adorner
        //public void HandleLinePoint(XPoint linePointMM, IDrawingViewModel canvasModel)
        //{
        //    //check if we intersect a point of this wire with other net segments
        //    #region Wire with other net segments

        //    var intersectedWires = new List<NetWireCanvasItem>();
        //    var addJunction = false;
        //    var netElements = canvasModel.Items.OfType<NetSegmentCanvasItem>().Where(ns => ns != this && ns.IsPlaced).ToList();
        //    foreach (var netSegm in netElements)
        //    {
        //        //ignore colision with net labels or pinrefs
        //        if (netSegm is NetLabelCanvasItem)
        //            continue;

        //        if (netSegm is ICanvasItem)
        //        {
        //            var geom = netSegm as ICanvasItem;
        //            if (geom != null)
        //            {
        //                var intersects = GeometryHelper.ItemIntersectsPoint(geom, linePointMM, Width);
        //                if (!intersects)
        //                    continue;

        //                //assign this net to be that intersected net
        //                NetDesignerItem sourceNet = Net;//oldNet
        //                NetDesignerItem destNet = null; //newNet
        //                if (netSegm.Net != null)
        //                {
        //                    destNet = netSegm.Net;
        //                }
        //                if (sourceNet != null && destNet != null)
        //                {
        //                    //the new net will be from the source if source net is named; otherwise we take it from the destination
        //                    if (sourceNet.IsNamed() && !destNet.IsNamed())
        //                    {
        //                        ChangeNet(destNet, sourceNet, canvasModel);
        //                    }
        //                    else
        //                    {
        //                        ChangeNet(sourceNet, destNet, canvasModel);
        //                    }
        //                }
        //                else
        //                {
        //                    Net = destNet;
        //                }



        //                if (netSegm is NetWireCanvasItem)
        //                {
        //                    //if we are on a net wire but not on a start or end, add a junction
        //                    var otherNetWire = netSegm as NetWireCanvasItem;
        //                    if (linePointMM != otherNetWire.StartPoint && linePointMM != otherNetWire.EndPoint)
        //                    {
        //                        addJunction = true;
        //                    }
        //                    else
        //                    {
        //                        intersectedWires.Add(otherNetWire);
        //                    }
        //                }
        //            }

        //        }
        //    }
        //    if (intersectedWires.Count > 1)
        //    {
        //        addJunction = true;
        //    }
        //    if (addJunction)
        //    {
        //        var junctionExists = canvasModel.Items.OfType<JunctionCanvasItem>()
        //                                      .Any(j => j.X == linePointMM.X && j.Y == linePointMM.Y);

        //        if (!junctionExists)
        //        {
        //            var junctionItem = (JunctionCanvasItem)Activator.CreateInstance(typeof(JunctionCanvasItem));
        //            junctionItem.X = linePointMM.X;
        //            junctionItem.Y = linePointMM.Y;
        //            junctionItem.Net = Net;
        //            junctionItem.IsPlaced = true;
        //            canvasModel.AddItem(junctionItem);
        //        }
        //    }

        //    #endregion Wire with other net segments

        //    //check if we intersect a point of this wire with other pins
        //    #region Wire with pins

        //    var pinHelper = new PinCanvasItemHelper(canvasModel);
        //    var hitPins = pinHelper.GetPinsCollision(linePointMM, width);

        //    //we take the 1st pin; we could show a list with the pins
        //    var hitPin = hitPins.FirstOrDefault();
        //    ////////////////////////////////////////////

        //    if (hitPin != null)
        //    {

        //        //create a net if we don't have one at this point
        //        if (Net == null)
        //        {
        //            Net = new NetDesignerItem
        //            {
        //                //ClassId = 0,
        //                Id = LibraryItem.GetNextId(),
        //                CanvasModel = canvasModel
        //            };
        //            Net.Name = hitPin.PinType == PinType.Power ? hitPin.Name : $"Net{Net.Id}";
        //        }
        //        else
        //        {
        //            //we are on a named net but we hit a supply pin; ask what net to use
        //            if (hitPin.PinType == PinType.Power)
        //            {
        //                var supplyPinNet = new NetDesignerItem
        //                {
        //                    Id = LibraryItem.GetNextId(),
        //                    CanvasModel = canvasModel,
        //                    Name = hitPin.Name
        //                };

        //                if (Net.IsNamed())
        //                {
        //                    var candidateNets = new[] { supplyPinNet, Net }.OrderBy(n => n.Name).ToList();
        //                    var itemPickerDialog = ServiceProvider.Resolve<IItemPickerDialog>();
        //                    itemPickerDialog.LoadData(candidateNets);
        //                    var res = itemPickerDialog.ShowDialog();

        //                    if (res.GetValueOrDefault() == false)
        //                        return;
        //                    Net = itemPickerDialog.SelectedItem as NetDesignerItem;
        //                }
        //                else
        //                {
        //                    Net.Name = supplyPinNet.Name;
        //                }

        //            }
        //        }
        //        hitPin.Net = Net;


        //    }

        //    #endregion Wire with pins
        //}

        /*
        /// <summary>
        /// Change all the items belongong to the sourceNet to the new destination Net
        /// </summary>
        /// <param name="sourceNet"></param>
        /// <param name="destNet"></param>
        void ChangeNet(NetDesignerItem sourceNet, NetDesignerItem destNet, IDrawingViewModel canvasModel)
        {
            if (sourceNet == null || destNet == null)
                return;
            var netItems = canvasModel.Items.OfType<NetSegmentCanvasItem>()
                                       .Where(n => n.Net != null && n.Net.Name == sourceNet.Name)
                                       .ToList();

            //just change the reference
            netItems.ForEach(n => n.Net = destNet);
        }
        */

        public override IPrimitive SaveToPrimitive()
        {
            var wire = new NetWire();

            wire.Points = Points.Select(v => new Vertex { x = v.X, y = v.Y }).ToList();
            wire.Width = Width;
            wire.LineColor = LineColor.ToHexString();

            return wire;
        }

        protected override void LoadFromPrimitiveInternal(IPrimitive primitive)
        {
            var wire = (NetWire)primitive;
            var wirePoints = wire.Points.Select(v => new XPoint(v.x, v.y)).ToList();
            if (wirePoints.Count == 0)
            {
                //read from old style
                wirePoints = new List<XPoint>();
                wirePoints.Add(new XPoint(wire.X1, wire.Y1));
                wirePoints.Add(new XPoint(wire.X2, wire.Y2));
            }
            Points = wirePoints;

            Width = wire.Width;
            LineColor = XColor.FromHexString(wire.LineColor);

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
            return $"Net wire ({Net})";
        }
    }


}
