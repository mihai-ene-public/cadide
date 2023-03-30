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

namespace IDE.Core.Designers;

public class TrackBoardCanvasItem : SingleLayerBoardCanvasItem
                                  , ITrackBoardCanvasItem
{

    public TrackBoardCanvasItem()
    {
        Width = 0.254;

        //ensure 2 points
        Points.Add(new XPoint());
        Points.Add(new XPoint());
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

    public void AssignSignal(IBoardNetDesignerItem newSignal)
    {
        signal = newSignal;
    }

    IList<XPoint> points = new ObservableCollection<XPoint>();

    [Browsable(false)]
    [MarksDirty]
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
    }

    public override void MirrorY()
    {
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

        var fontSize = sizeFactor * availableSize.Height;
        var textLen = fontSize * sLen;
        if (textLen > availableSize.Width)
        {
            var scale = availableSize.Width / textLen;
            fontSize *= scale;
        }

        if (hasSignal)
            return 0.5 * fontSize;

        return fontSize;
    }
}
