using IDE.Core.Designers;
using IDE.Core.Interfaces;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace IDE.Controls
{
    public class ThtPadShape : Shape
    {

        #region CornerRadius

        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register(nameof(CornerRadius), typeof(double), typeof(ThtPadShape),
                new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsRender));

        [TypeConverter(typeof(LengthConverter))]
        public double CornerRadius
        {
            get { return (double)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }
        #endregion


        #region TopCopperBrush

        public static readonly DependencyProperty TopCopperBrushProperty =
               DependencyProperty.Register(
                       "TopCopperBrush",
                       typeof(Brush),
                       typeof(ThtPadShape),
                       new FrameworkPropertyMetadata(
                               (Brush)null,
                               FrameworkPropertyMetadataOptions.AffectsRender |
                               FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));

        public Brush TopCopperBrush
        {
            get { return (Brush)GetValue(TopCopperBrushProperty); }
            set { SetValue(TopCopperBrushProperty, value); }
        }

        #endregion

        #region TopCopperVisible

        public static readonly DependencyProperty TopCopperVisibleProperty =
        DependencyProperty.Register(
                "TopCopperVisible",
                typeof(bool),
                typeof(ThtPadShape),
                new FrameworkPropertyMetadata(
                        false,
                        FrameworkPropertyMetadataOptions.AffectsRender |
                        FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));

        public bool TopCopperVisible
        {
            get { return (bool)GetValue(TopCopperVisibleProperty); }
            set { SetValue(TopCopperVisibleProperty, value); }
        }

        #endregion

        #region TopCopperZIndex

        public static readonly DependencyProperty TopCopperZIndexProperty =
   DependencyProperty.Register(
           "TopCopperZIndex",
           typeof(int),
           typeof(ThtPadShape),
           new FrameworkPropertyMetadata(
                   0,
                   FrameworkPropertyMetadataOptions.AffectsRender |
                   FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));

        public int TopCopperZIndex
        {
            get { return (int)GetValue(TopCopperZIndexProperty); }
            set { SetValue(TopCopperZIndexProperty, value); }
        }

        #endregion

        #region TopPasteBrush

        public static readonly DependencyProperty TopPasteBrushProperty =
             DependencyProperty.Register(
                     "TopPasteBrush",
                     typeof(Brush),
                     typeof(ThtPadShape),
                     new FrameworkPropertyMetadata(
                             (Brush)null,
                             FrameworkPropertyMetadataOptions.AffectsRender |
                             FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));

        public Brush TopPasteBrush
        {
            get { return (Brush)GetValue(TopPasteBrushProperty); }
            set { SetValue(TopPasteBrushProperty, value); }
        }

        #endregion

        #region TopPasteVisible

        public static readonly DependencyProperty TopPasteVisibleProperty =
     DependencyProperty.Register(
             "TopPasteVisible",
             typeof(bool),
             typeof(ThtPadShape),
             new FrameworkPropertyMetadata(
                     false,
                     FrameworkPropertyMetadataOptions.AffectsRender |
                     FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));

        public bool TopPasteVisible
        {
            get { return (bool)GetValue(TopPasteVisibleProperty); }
            set { SetValue(TopPasteVisibleProperty, value); }
        }

        #endregion

        #region TopPasteZIndex

        public static readonly DependencyProperty TopPasteZIndexProperty =
   DependencyProperty.Register(
           "TopPasteZIndex",
           typeof(int),
           typeof(ThtPadShape),
           new FrameworkPropertyMetadata(
                   0,
                   FrameworkPropertyMetadataOptions.AffectsRender |
                   FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));

        public int TopPasteZIndex
        {
            get { return (int)GetValue(TopPasteZIndexProperty); }
            set { SetValue(TopPasteZIndexProperty, value); }
        }

        #endregion

        #region TopSolderBrush

        public static readonly DependencyProperty TopSolderBrushProperty =
          DependencyProperty.Register(
                  "TopSolderBrush",
                  typeof(Brush),
                  typeof(ThtPadShape),
                  new FrameworkPropertyMetadata(
                          (Brush)null,
                          FrameworkPropertyMetadataOptions.AffectsRender |
                          FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));

        public Brush TopSolderBrush
        {
            get { return (Brush)GetValue(TopSolderBrushProperty); }
            set { SetValue(TopSolderBrushProperty, value); }
        }

        #endregion

        #region TopSolderVisible

        public static readonly DependencyProperty TopSolderVisibleProperty =
    DependencyProperty.Register(
            "TopSolderVisible",
            typeof(bool),
            typeof(ThtPadShape),
            new FrameworkPropertyMetadata(
                    false,
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));

        public bool TopSolderVisible
        {
            get { return (bool)GetValue(TopSolderVisibleProperty); }
            set { SetValue(TopSolderVisibleProperty, value); }
        }

        #endregion

        #region TopSolderZIndex

        public static readonly DependencyProperty TopSolderZIndexProperty =
   DependencyProperty.Register(
           "TopSolderZIndex",
           typeof(int),
           typeof(ThtPadShape),
           new FrameworkPropertyMetadata(
                   0,
                   FrameworkPropertyMetadataOptions.AffectsRender |
                   FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));

        public int TopSolderZIndex
        {
            get { return (int)GetValue(TopSolderZIndexProperty); }
            set { SetValue(TopSolderZIndexProperty, value); }
        }

        #endregion

        //
        #region BottomCopperBrush

        public static readonly DependencyProperty BottomCopperBrushProperty =
               DependencyProperty.Register(
                       "BottomCopperBrush",
                       typeof(Brush),
                       typeof(ThtPadShape),
                       new FrameworkPropertyMetadata(
                               (Brush)null,
                               FrameworkPropertyMetadataOptions.AffectsRender |
                               FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));

        public Brush BottomCopperBrush
        {
            get { return (Brush)GetValue(BottomCopperBrushProperty); }
            set { SetValue(BottomCopperBrushProperty, value); }
        }

        #endregion

        #region BottomCopperVisible

        public static readonly DependencyProperty BottomCopperVisibleProperty =
        DependencyProperty.Register(
                "BottomCopperVisible",
                typeof(bool),
                typeof(ThtPadShape),
                new FrameworkPropertyMetadata(
                        false,
                        FrameworkPropertyMetadataOptions.AffectsRender |
                        FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));

        public bool BottomCopperVisible
        {
            get { return (bool)GetValue(BottomCopperVisibleProperty); }
            set { SetValue(BottomCopperVisibleProperty, value); }
        }

        #endregion

        #region BottomCopperZIndex

        public static readonly DependencyProperty BottomCopperZIndexProperty =
   DependencyProperty.Register(
           "BottomCopperZIndex",
           typeof(int),
           typeof(ThtPadShape),
           new FrameworkPropertyMetadata(
                   0,
                   FrameworkPropertyMetadataOptions.AffectsRender |
                   FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));

        public int BottomCopperZIndex
        {
            get { return (int)GetValue(BottomCopperZIndexProperty); }
            set { SetValue(BottomCopperZIndexProperty, value); }
        }

        #endregion

        #region BottomPasteBrush

        public static readonly DependencyProperty BottomPasteBrushProperty =
             DependencyProperty.Register(
                     "BottomPasteBrush",
                     typeof(Brush),
                     typeof(ThtPadShape),
                     new FrameworkPropertyMetadata(
                             (Brush)null,
                             FrameworkPropertyMetadataOptions.AffectsRender |
                             FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));

        public Brush BottomPasteBrush
        {
            get { return (Brush)GetValue(BottomPasteBrushProperty); }
            set { SetValue(BottomPasteBrushProperty, value); }
        }

        #endregion

        #region BottomPasteVisible

        public static readonly DependencyProperty BottomPasteVisibleProperty =
     DependencyProperty.Register(
             "BottomPasteVisible",
             typeof(bool),
             typeof(ThtPadShape),
             new FrameworkPropertyMetadata(
                     false,
                     FrameworkPropertyMetadataOptions.AffectsRender |
                     FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));

        public bool BottomPasteVisible
        {
            get { return (bool)GetValue(BottomPasteVisibleProperty); }
            set { SetValue(BottomPasteVisibleProperty, value); }
        }

        #endregion

        #region BottomPasteZIndex

        public static readonly DependencyProperty BottomPasteZIndexProperty =
   DependencyProperty.Register(
           "BottomPasteZIndex",
           typeof(int),
           typeof(ThtPadShape),
           new FrameworkPropertyMetadata(
                   0,
                   FrameworkPropertyMetadataOptions.AffectsRender |
                   FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));

        public int BottomPasteZIndex
        {
            get { return (int)GetValue(BottomPasteZIndexProperty); }
            set { SetValue(BottomPasteZIndexProperty, value); }
        }

        #endregion

        #region BottomSolderBrush

        public static readonly DependencyProperty BottomSolderBrushProperty =
          DependencyProperty.Register(
                  "BottomSolderBrush",
                  typeof(Brush),
                  typeof(ThtPadShape),
                  new FrameworkPropertyMetadata(
                          (Brush)null,
                          FrameworkPropertyMetadataOptions.AffectsRender |
                          FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));

        public Brush BottomSolderBrush
        {
            get { return (Brush)GetValue(BottomSolderBrushProperty); }
            set { SetValue(BottomSolderBrushProperty, value); }
        }

        #endregion

        #region BottomSolderVisible

        public static readonly DependencyProperty BottomSolderVisibleProperty =
    DependencyProperty.Register(
            "BottomSolderVisible",
            typeof(bool),
            typeof(ThtPadShape),
            new FrameworkPropertyMetadata(
                    false,
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));

        public bool BottomSolderVisible
        {
            get { return (bool)GetValue(BottomSolderVisibleProperty); }
            set { SetValue(BottomSolderVisibleProperty, value); }
        }

        #endregion

        #region BottomSolderZIndex

        public static readonly DependencyProperty BottomSolderZIndexProperty =
   DependencyProperty.Register(
           "BottomSolderZIndex",
           typeof(int),
           typeof(ThtPadShape),
           new FrameworkPropertyMetadata(
                   0,
                   FrameworkPropertyMetadataOptions.AffectsRender |
                   FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));

        public int BottomSolderZIndex
        {
            get { return (int)GetValue(BottomSolderZIndexProperty); }
            set { SetValue(BottomSolderZIndexProperty, value); }
        }

        #endregion


        #region Drill

        public static readonly DependencyProperty DrillProperty =
   DependencyProperty.Register(
           "Drill",
           typeof(double),
           typeof(ThtPadShape),
           new FrameworkPropertyMetadata(
                   0.0d,
                   FrameworkPropertyMetadataOptions.AffectsRender |
                   FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));

        public double Drill
        {
            get { return (double)GetValue(DrillProperty); }
            set { SetValue(DrillProperty, value); }
        }

        #endregion

        #region DrillOffset

        public static readonly DependencyProperty DrillOffsetProperty =
  DependencyProperty.Register(
          "DrillOffset",
          typeof(Point),
          typeof(ThtPadShape),
          new FrameworkPropertyMetadata(
                 new Point(0, 0),
                  FrameworkPropertyMetadataOptions.AffectsRender |
                  FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));

        public Point DrillOffset
        {
            get { return (Point)GetValue(DrillOffsetProperty); }
            set { SetValue(DrillOffsetProperty, value); }
        }

        #endregion

        #region DrillType

        public static readonly DependencyProperty DrillTypeProperty =
             DependencyProperty.Register(
                     "DrillType",
                     typeof(DrillType),
                     typeof(ThtPadShape),
                     new FrameworkPropertyMetadata(
                            DrillType.Drill,
                             FrameworkPropertyMetadataOptions.AffectsRender |
                             FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));

        public DrillType DrillType
        {
            get { return (DrillType)GetValue(DrillTypeProperty); }
            set { SetValue(DrillTypeProperty, value); }
        }

        #endregion

        #region SlotHeight

        public static readonly DependencyProperty SlotHeightProperty =
   DependencyProperty.Register(
           "SlotHeight",
           typeof(double),
           typeof(ThtPadShape),
           new FrameworkPropertyMetadata(
                   0.0d,
                   FrameworkPropertyMetadataOptions.AffectsRender |
                   FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));

        public double SlotHeight
        {
            get { return (double)GetValue(SlotHeightProperty); }
            set { SetValue(SlotHeightProperty, value); }
        }

        #endregion

        #region SlotRot

        public static readonly DependencyProperty SlotRotProperty =
   DependencyProperty.Register(
           "SlotRot",
           typeof(double),
           typeof(ThtPadShape),
           new FrameworkPropertyMetadata(
                   0.0d,
                   FrameworkPropertyMetadataOptions.AffectsRender |
                   FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));

        public double SlotRot
        {
            get { return (double)GetValue(SlotRotProperty); }
            set { SetValue(SlotRotProperty, value); }
        }

        #endregion

        #region PadNumber

        public static readonly DependencyProperty PadNumberProperty =
  DependencyProperty.Register(
          "PadNumber",
          typeof(string),
          typeof(ThtPadShape),
          new FrameworkPropertyMetadata(
                  null,
                  FrameworkPropertyMetadataOptions.AffectsRender |
                  FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));

        public string PadNumber
        {
            get { return (string)GetValue(PadNumberProperty); }
            set { SetValue(PadNumberProperty, value); }
        }

        #endregion

        #region PadNumberFontSize

        public static readonly DependencyProperty PadNumberFontSizeProperty =
 DependencyProperty.Register(
         "PadNumberFontSize",
         typeof(double),
         typeof(ThtPadShape),
         new FrameworkPropertyMetadata(
                 0.0d,
                 FrameworkPropertyMetadataOptions.AffectsRender |
                 FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));

        public double PadNumberFontSize
        {
            get { return (double)GetValue(PadNumberFontSizeProperty); }
            set { SetValue(PadNumberFontSizeProperty, value); }
        }

        #endregion

        #region SignalName

        public static readonly DependencyProperty SignalNameProperty =
 DependencyProperty.Register(
         "SignalName",
         typeof(string),
         typeof(ThtPadShape),
         new FrameworkPropertyMetadata(
                 null,
                 FrameworkPropertyMetadataOptions.AffectsRender |
                 FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));

        public string SignalName
        {
            get { return (string)GetValue(SignalNameProperty); }
            set { SetValue(SignalNameProperty, value); }
        }

        #endregion

        #region SignalNameFontSize

        public static readonly DependencyProperty SignalNameFontSizeProperty =
DependencyProperty.Register(
        "SignalNameFontSize",
        typeof(double),
        typeof(ThtPadShape),
        new FrameworkPropertyMetadata(
                0.0d,
                FrameworkPropertyMetadataOptions.AffectsRender |
                FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));

        public double SignalNameFontSize
        {
            get { return (double)GetValue(SignalNameFontSizeProperty); }
            set { SetValue(SignalNameFontSizeProperty, value); }
        }

        #endregion

        #region SignalNameIsVisible

        public static readonly DependencyProperty SignalNameIsVisibleProperty =
DependencyProperty.Register(
       "SignalNameIsVisible",
       typeof(bool),
       typeof(ThtPadShape),
       new FrameworkPropertyMetadata(
               false,
               FrameworkPropertyMetadataOptions.AffectsRender |
               FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));

        public bool SignalNameIsVisible
        {
            get { return (bool)GetValue(SignalNameIsVisibleProperty); }
            set { SetValue(SignalNameIsVisibleProperty, value); }
        }

        #endregion

        protected override Geometry DefiningGeometry
        {
            get
            {
                var cornerRadius = CornerRadius;
                var rect = GetRect();

                return new RectangleGeometry(rect, cornerRadius, cornerRadius);
            }
        }

        private Rect GetRect()
        {
            var width = Width;
            var height = Height;

            var rect = new Rect(-0.5 * width, -0.5 * height, width, height);

            return rect;
        }

        private Rect GetSlotRect()
        {
            var width = Drill;
            var height = SlotHeight;

            var rect = new Rect(-0.5 * width, -0.5 * height, width, height);

            return rect;
        }

        private int GetZIndex(int actualZIndex)
        {
            if (actualZIndex == LayerDesignerItem.SelectedLayerZIndex)
                return actualZIndex;
            return 0;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            var pen = new Pen();
            var rect = GetRect();
            var cornerRadius = CornerRadius;
            var height = Height;

            var list = new List<DrawData>();

           

            

            if (BottomSolderVisible)
                list.Add(new DrawData { ZIndex = GetZIndex(BottomSolderZIndex), Fill = BottomSolderBrush });

            if (BottomPasteVisible)
                list.Add(new DrawData { ZIndex = GetZIndex(BottomPasteZIndex), Fill = BottomPasteBrush });

            if (BottomCopperVisible)
                list.Add(new DrawData { ZIndex = GetZIndex(BottomCopperZIndex), Fill = BottomCopperBrush });

            if (TopSolderVisible)
                list.Add(new DrawData { ZIndex = GetZIndex(TopSolderZIndex), Fill = TopSolderBrush });

            if (TopPasteVisible)
                list.Add(new DrawData { ZIndex = GetZIndex(TopPasteZIndex), Fill = TopPasteBrush });

            if (TopCopperVisible)
                list.Add(new DrawData { ZIndex = GetZIndex(TopCopperZIndex), Fill = TopCopperBrush });

            if (list.Count > 0)
            {
                list = list.OrderBy(l => l.ZIndex).ToList();

                foreach (var item in list)
                {
                    drawingContext.DrawRoundedRectangle(item.Fill, pen, rect, cornerRadius, cornerRadius);
                }
            }

            //draw the slot always; it is multilayer
            DrawSlot(drawingContext);

            //don't draw pad numbers if nothing on any layers is visible
            if (list.Count == 0)
                return;

            //pad number
            var padNumber = PadNumber;
            var padNumberFontSize = PadNumberFontSize;
            if (!string.IsNullOrEmpty(padNumber))
            {
                var tf = new Typeface("Segoe UI");
                var brush = new SolidColorBrush(Colors.White);
                var pixelsPerDip = 1.0d;// 96/96
                var ft = new FormattedText(padNumber, CultureInfo.InvariantCulture, FlowDirection.LeftToRight, tf, padNumberFontSize, brush, pixelsPerDip);
                ft.TextAlignment = TextAlignment.Center;

                drawingContext.DrawText(ft, new Point(0, -0.5 * height));
            }

            //signal name
            var signalName = SignalName;
            var signalNameFontSize = SignalNameFontSize;
            var showSignalName = SignalNameIsVisible;
            if (showSignalName && !string.IsNullOrEmpty(signalName))
            {
                var tf = new Typeface("Segoe UI");
                var brush = new SolidColorBrush(Colors.White);
                var pixelsPerDip = 1.0d;// 96/96
                var ft = new FormattedText(signalName, CultureInfo.InvariantCulture, FlowDirection.LeftToRight, tf, signalNameFontSize, brush, pixelsPerDip);
                ft.TextAlignment = TextAlignment.Center;

                drawingContext.DrawText(ft, new Point(0, 0));
            }
        }

        private void DrawSlot(DrawingContext drawingContext)
        {
            var drill = Drill;
            var radius = 0.5 * drill;
            var color = Colors.White;
            color.ScA = 0.5f;
            var pen = new Pen();
            var brush = new SolidColorBrush(color);
            var drillOffset = DrillOffset;

            switch (DrillType)
            {
                case DrillType.Drill:
                    drawingContext.DrawEllipse(brush, pen, DrillOffset, radius, radius);
                    break;
                case DrillType.Slot:
                    var slotRect = GetSlotRect();
                    var slotRot = SlotRot;

                    var gt = new TransformGroup();
                    gt.Children.Add(new RotateTransform(slotRot));
                    gt.Children.Add(new TranslateTransform(drillOffset.X, drillOffset.Y));

                    drawingContext.PushTransform(gt);

                    drawingContext.DrawRoundedRectangle(brush, pen, slotRect, radius, radius);

                    drawingContext.Pop();

                    break;
            }
        }

        class DrawData
        {
            public int ZIndex { get; set; }
            public Brush Fill { get; set; }
        }
    }

}
