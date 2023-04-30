using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using IDE.Core.Interfaces;
using IDE.Core.Storage;
using IDE.Core.Types.Media;

namespace IDE.Core.Designers
{
    public class RegionBoardCanvasItem : BoardCanvasItemViewModel//SingleLayerBoardCanvasItem
                                        , IRegionCanvasItem
    {
        public RegionBoardCanvasItem()
        {
            CanEdit = false;
        }

        //[Browsable(false)]
        //public new LayerDesignerItem Layer { get; set; }

        [Browsable(false)]
        public int LayerId { get; set; }

        double startPointX;

        [Browsable(false)]
        public double StartPointX
        {
            get { return startPointX; }
            set
            {
                startPointX = value;
                OnPropertyChanged(nameof(StartPointX));
                OnPropertyChanged(nameof(StartPoint));
            }
        }

        double startPointY;

        [Browsable(false)]
        public double StartPointY
        {
            get { return startPointY; }
            set
            {
                startPointY = value;
                OnPropertyChanged(nameof(StartPointY));
                OnPropertyChanged(nameof(StartPoint));
            }
        }

        [Browsable(false)]
        public XPoint StartPoint
        {
            get { return new XPoint(startPointX, startPointY); }
        }

        double width = 0.2;

        [Browsable(false)]
        public double Width
        {
            get { return width; }
            set
            {
                width = value;
                OnPropertyChanged(nameof(Width));
            }
        }

        ////we don't want this region to be selected
        //[Browsable(false)]
        //public bool IsSelected { get { return false; }set { } } 

        [Browsable(false)]
        public ObservableCollection<IRegionItem> Items { get; set; } = new ObservableCollection<IRegionItem>();

        public static RegionBoardCanvasItem FromData(RegionBoard region)
        {
            var r = new RegionBoardCanvasItem
            {
                StartPointX = region.StartPointX,
                StartPointY = region.StartPointY,
                Width = region.Width
            };
            r.Items.AddRange(region.Items.Select(d => BaseRegionItem.FromData(d)));

            return r;
        }

        public RegionBoard ToData()
        {
            var r = new RegionBoard
            {
                LayerId = LayerId,
                StartPointX = StartPointX,
                StartPointY = StartPointY,
                Width = Width,
                Items = Items.Select(c => (BaseRegionPrimitive)c.ToPrimitive()).ToList()
            };

            return r;
        }


        public override void Translate(double dx, double dy)
        {
            //throw new NotImplementedException();

            StartPointX += dx;
            StartPointY += dy;

            foreach (var regionItem in Items)
            {
                regionItem.EndPointX += dx;
                regionItem.EndPointY += dy;
            }
        }

        public override XRect GetBoundingRectangle()
        {
            var minX = startPointX;
            var minY = startPointY;

            var maxX = startPointX;
            var maxY = startPointY;

            foreach (var regionItem in Items)
            {
                if (regionItem.EndPointX < minX)
                    minX = regionItem.EndPointX;
                if (regionItem.EndPointX > maxX)
                    maxX = regionItem.EndPointX;

                if (regionItem.EndPointY < minY)
                    minY = regionItem.EndPointY;
                if (regionItem.EndPointY > maxY)
                    maxY = regionItem.EndPointY;
            }

            return new XRect(new XPoint(minX, minY), new XPoint(maxX, maxY));
        }

        //public override Adorner CreateAdorner(UIElement element)
        //{
        //    return null;
        //}

        public override void MirrorX()
        {
        }

        public override void MirrorY()
        {
        }

        public override void Rotate(double angle = 90)
        {
        }

        public override void LoadLayers()
        {
        }

        public override object Clone()
        {
            var data = ToData();

            return FromData(data);
        }
    }

    public class BaseRegionItem : BaseViewModel, IRegionItem
    {
        double endPointX;
        public double EndPointX
        {
            get { return endPointX; }
            set
            {
                endPointX = value;
                OnPropertyChanged(nameof(EndPointX));
                OnPropertyChanged(nameof(EndPoint));
            }
        }

        double endPointY;
        public double EndPointY
        {
            get { return endPointY; }
            set
            {
                endPointY = value;
                OnPropertyChanged(nameof(EndPointY));
                OnPropertyChanged(nameof(EndPoint));
            }
        }

        public XPoint EndPoint
        { get { return new XPoint(endPointX, endPointY); } }

        public virtual IRegionPrimitive ToPrimitive()
        {
            throw new NotSupportedException("You must inherit");
        }

        public static BaseRegionItem FromData(IRegionPrimitive primitive)
        {
            if (primitive is LineRegionPrimitive)
            {
                var line = primitive as LineRegionPrimitive;
                var item = new LineRegionItem
                {
                    EndPointX = line.EndPointX,
                    EndPointY = line.EndPointY,
                };
                return item;
            }
            else if (primitive is ArcRegionPrimitive)
            {
                var arc = primitive as ArcRegionPrimitive;
                var item = new ArcRegionItem
                {
                    EndPointX = arc.EndPointX,
                    EndPointY = arc.EndPointY,
                    SizeDiameter = arc.SizeDiameter,
                    IsLargeArc = arc.IsLargeArc,
                    SweepDirection = arc.SweepDirection
                };
                return item;
            }

            throw new NotSupportedException();

        }


    }

    public class LineRegionItem : BaseRegionItem, ILineRegionItem
    {
        public override IRegionPrimitive ToPrimitive()
        {
            var p = new LineRegionPrimitive
            {
                EndPointX = EndPointX,
                EndPointY = EndPointY,
            };
            return p;
        }
    }

    public class ArcRegionItem : BaseRegionItem, IArcRegionItem
    {
        double sizeDiameter = 3.0d;
        public double SizeDiameter
        {
            get { return sizeDiameter; }
            set
            {
                sizeDiameter = value;
                OnPropertyChanged(nameof(SizeDiameter));
            }
        }

        XSweepDirection sweepDirection;
        public XSweepDirection SweepDirection
        {
            get { return sweepDirection; }
            set
            {
                sweepDirection = value;
                OnPropertyChanged(nameof(SweepDirection));
            }
        }

        bool isLargeArc;
        public bool IsLargeArc
        {
            get { return isLargeArc; }
            set
            {
                isLargeArc = value;
                OnPropertyChanged(nameof(IsLargeArc));
            }
        }

        public override IRegionPrimitive ToPrimitive()
        {
            var p = new ArcRegionPrimitive
            {
                EndPointX = EndPointX,
                EndPointY = EndPointY,
                IsLargeArc = IsLargeArc,
                SizeDiameter = SizeDiameter,
                SweepDirection = this.SweepDirection
            };
            return p;
        }
    }
}
