using IDE.Core.Designers;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace IDE.Controls
{
    public class SmdPadShape : Shape
    {

        #region CornerRadius

        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register(nameof(CornerRadius), typeof(double), typeof(SmdPadShape),
                new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsRender));

        [TypeConverter(typeof(LengthConverter))]
        public double CornerRadius
        {
            get { return (double)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }
        #endregion

        #region CopperBrush

        public static readonly DependencyProperty CopperBrushProperty =
               DependencyProperty.Register(
                       "CopperBrush",
                       typeof(Brush),
                       typeof(SmdPadShape),
                       new FrameworkPropertyMetadata(
                               (Brush)null,
                               FrameworkPropertyMetadataOptions.AffectsRender |
                               FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));

        public Brush CopperBrush
        {
            get { return (Brush)GetValue(CopperBrushProperty); }
            set { SetValue(CopperBrushProperty, value); }
        }

        #endregion

        #region CopperVisible

        public static readonly DependencyProperty CopperVisibleProperty =
        DependencyProperty.Register(
                "CopperVisible",
                typeof(bool),
                typeof(SmdPadShape),
                new FrameworkPropertyMetadata(
                        false,
                        FrameworkPropertyMetadataOptions.AffectsRender |
                        FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));

        public bool CopperVisible
        {
            get { return (bool)GetValue(CopperVisibleProperty); }
            set { SetValue(CopperVisibleProperty, value); }
        }

        #endregion

        #region CopperZIndex

        public static readonly DependencyProperty CopperZIndexProperty =
   DependencyProperty.Register(
           "CopperZIndex",
           typeof(int),
           typeof(SmdPadShape),
           new FrameworkPropertyMetadata(
                   0,
                   FrameworkPropertyMetadataOptions.AffectsRender |
                   FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));

        public int CopperZIndex
        {
            get { return (int)GetValue(CopperZIndexProperty); }
            set { SetValue(CopperZIndexProperty, value); }
        }

        #endregion

        #region PasteBrush

        public static readonly DependencyProperty PasteBrushProperty =
             DependencyProperty.Register(
                     "PasteBrush",
                     typeof(Brush),
                     typeof(SmdPadShape),
                     new FrameworkPropertyMetadata(
                             (Brush)null,
                             FrameworkPropertyMetadataOptions.AffectsRender |
                             FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));

        public Brush PasteBrush
        {
            get { return (Brush)GetValue(PasteBrushProperty); }
            set { SetValue(PasteBrushProperty, value); }
        }

        #endregion

        #region PasteVisible

        public static readonly DependencyProperty PasteVisibleProperty =
     DependencyProperty.Register(
             "PasteVisible",
             typeof(bool),
             typeof(SmdPadShape),
             new FrameworkPropertyMetadata(
                     false,
                     FrameworkPropertyMetadataOptions.AffectsRender |
                     FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));

        public bool PasteVisible
        {
            get { return (bool)GetValue(PasteVisibleProperty); }
            set { SetValue(PasteVisibleProperty, value); }
        }

        #endregion

        #region PasteZIndex

        public static readonly DependencyProperty PasteZIndexProperty =
   DependencyProperty.Register(
           "PasteZIndex",
           typeof(int),
           typeof(SmdPadShape),
           new FrameworkPropertyMetadata(
                   0,
                   FrameworkPropertyMetadataOptions.AffectsRender |
                   FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));

        public int PasteZIndex
        {
            get { return (int)GetValue(PasteZIndexProperty); }
            set { SetValue(PasteZIndexProperty, value); }
        }

        #endregion

        #region SolderBrush

        public static readonly DependencyProperty SolderBrushProperty =
          DependencyProperty.Register(
                  "SolderBrush",
                  typeof(Brush),
                  typeof(SmdPadShape),
                  new FrameworkPropertyMetadata(
                          (Brush)null,
                          FrameworkPropertyMetadataOptions.AffectsRender |
                          FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));

        public Brush SolderBrush
        {
            get { return (Brush)GetValue(SolderBrushProperty); }
            set { SetValue(SolderBrushProperty, value); }
        }

        #endregion

        #region SolderVisible

        public static readonly DependencyProperty SolderVisibleProperty =
    DependencyProperty.Register(
            "SolderVisible",
            typeof(bool),
            typeof(SmdPadShape),
            new FrameworkPropertyMetadata(
                    false,
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));

        public bool SolderVisible
        {
            get { return (bool)GetValue(SolderVisibleProperty); }
            set { SetValue(SolderVisibleProperty, value); }
        }

        #endregion

        #region SolderZIndex

        public static readonly DependencyProperty SolderZIndexProperty =
   DependencyProperty.Register(
           "SolderZIndex",
           typeof(int),
           typeof(SmdPadShape),
           new FrameworkPropertyMetadata(
                   0,
                   FrameworkPropertyMetadataOptions.AffectsRender |
                   FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));

        public int SolderZIndex
        {
            get { return (int)GetValue(SolderZIndexProperty); }
            set { SetValue(SolderZIndexProperty, value); }
        }

        #endregion

        #region PadNumber

        public static readonly DependencyProperty PadNumberProperty =
  DependencyProperty.Register(
          "PadNumber",
          typeof(string),
          typeof(SmdPadShape),
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
         typeof(SmdPadShape),
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
         typeof(SmdPadShape),
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
        typeof(SmdPadShape),
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
       typeof(SmdPadShape),
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



            if (SolderVisible)
                list.Add(new DrawData { ZIndex = GetZIndex(SolderZIndex), Fill = SolderBrush });

            if (PasteVisible)
                list.Add(new DrawData { ZIndex = GetZIndex(PasteZIndex), Fill = PasteBrush });

            if (CopperVisible)
                list.Add(new DrawData { ZIndex = GetZIndex(CopperZIndex), Fill = CopperBrush });

            if (list.Count == 0)
                return;

            list = list.OrderBy(l => l.ZIndex).ToList();

            foreach (var item in list)
            {
                drawingContext.DrawRoundedRectangle(item.Fill, pen, rect, cornerRadius, cornerRadius);
            }

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

        class DrawData
        {
            public int ZIndex { get; set; }
            public Brush Fill { get; set; }
        }
    }
}
