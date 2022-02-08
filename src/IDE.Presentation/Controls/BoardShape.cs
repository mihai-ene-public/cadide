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
    public abstract class BoardShape : Shape
    {
        #region MaskedBrush

        public static readonly DependencyProperty MaskedBrushProperty =
               DependencyProperty.Register(
                       "MaskedBrush",
                       typeof(Brush),
                       typeof(BoardShape),
                       new FrameworkPropertyMetadata(
                               (Brush)null,
                               FrameworkPropertyMetadataOptions.AffectsRender |
                               FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));

        public Brush MaskedBrush
        {
            get { return (Brush)GetValue(MaskedBrushProperty); }
            set { SetValue(MaskedBrushProperty, value); }
        }

        #endregion

        #region DimmedBrush

        public static readonly DependencyProperty DimmedBrushProperty =
               DependencyProperty.Register(
                       "DimmedBrush",
                       typeof(Brush),
                       typeof(BoardShape),
                       new FrameworkPropertyMetadata(
                               (Brush)null,
                               FrameworkPropertyMetadataOptions.AffectsRender |
                               FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));

        public Brush DimmedBrush
        {
            get { return (Brush)GetValue(DimmedBrushProperty); }
            set { SetValue(DimmedBrushProperty, value); }
        }

        #endregion

        #region IsMasked

        public static readonly DependencyProperty IsMaskedProperty =
    DependencyProperty.Register(
            "IsMasked",
            typeof(bool),
            typeof(BoardShape),
            new FrameworkPropertyMetadata(
                    false,
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));

        public bool IsMasked
        {
            get { return (bool)GetValue(IsMaskedProperty); }
            set { SetValue(IsMaskedProperty, value); }
        }

        #endregion

        #region IsFilled

        public static readonly DependencyProperty IsFilledProperty =
    DependencyProperty.Register(
            "IsFilled",
            typeof(bool),
            typeof(BoardShape),
            new FrameworkPropertyMetadata(
                    false,
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));

        public bool IsFilled
        {
            get { return (bool)GetValue(IsFilledProperty); }
            set { SetValue(IsFilledProperty, value); }
        }

        #endregion

        #region DocumentHasHighlightedNets

        public static readonly DependencyProperty DocumentHasHighlightedNetsProperty =
    DependencyProperty.Register(
            "DocumentHasHighlightedNets",
            typeof(bool),
            typeof(BoardShape),
            new FrameworkPropertyMetadata(
                    false,
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));

        public bool DocumentHasHighlightedNets
        {
            get { return (bool)GetValue(DocumentHasHighlightedNetsProperty); }
            set { SetValue(DocumentHasHighlightedNetsProperty, value); }
        }

        #endregion


        #region IsHighlighted

        public static readonly DependencyProperty IsHighlightedProperty =
    DependencyProperty.Register(
            "IsHighlighted",
            typeof(bool),
            typeof(BoardShape),
            new FrameworkPropertyMetadata(
                    false,
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));

        public bool IsHighlighted
        {
            get { return (bool)GetValue(IsHighlightedProperty); }
            set { SetValue(IsHighlightedProperty, value); }
        }

        #endregion


        protected Brush GetBrush(Brush startBrush)
        {
            if (IsMasked)
                return MaskedBrush;
            if (DocumentHasHighlightedNets && IsHighlighted == false)
                return DimmedBrush;

            return startBrush;
        }
    }
}
