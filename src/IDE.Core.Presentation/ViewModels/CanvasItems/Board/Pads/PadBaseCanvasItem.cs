using IDE.Core.Interfaces;
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
    public abstract class PadBaseCanvasItem : SingleLayerBoardCanvasItem, IPadCanvasItem
    {
        string number;

        [Display(Order = 1)]
        [MarksDirty]
        public string Number
        {
            get
            {
                return number;
            }
            set
            {
                number = value;
                OnPropertyChanged(nameof(Number));
            }
        }

        [Browsable(false)]
        public string FootprintInstanceId
        {
            get
            {
                var fpt = ParentObject as FootprintBoardCanvasItem;
                if (fpt != null)
                    return fpt.FootprintPrimitive.Id;

                return null;
            }
        }

        [Browsable(false)]
        /// <summary>
        /// the items that will form this pad. It is composed from SingleLayerBoardCanvasItems and holes
        /// </summary>
        public IList<BoardCanvasItemViewModel> Items { get; set; } = new ObservableCollection<BoardCanvasItemViewModel>();

        private bool autoGenerateSolderMask = true;

        [MarksDirty]
        public bool AutoGenerateSolderMask
        {
            get { return autoGenerateSolderMask; }
            set
            {
                autoGenerateSolderMask = value;
                OnPropertyChanged(nameof(AutoGenerateSolderMask));

            }
        }


        private bool adjustSolderMaskInRules = true;

        [MarksDirty]
        public bool AdjustSolderMaskInRules
        {
            get { return adjustSolderMaskInRules; }
            set
            {
                adjustSolderMaskInRules = value;
                OnPropertyChanged(nameof(AdjustSolderMaskInRules));

            }
        }


        private bool autoGeneratePasteMask = true;

        [MarksDirty]
        public bool AutoGeneratePasteMask
        {
            get { return autoGeneratePasteMask; }
            set
            {
                autoGeneratePasteMask = value;
                OnPropertyChanged(nameof(AutoGeneratePasteMask));

            }
        }


        private bool adjustPasteMaskInRules = true;

        [MarksDirty]
        public bool AdjustPasteMaskInRules
        {
            get { return adjustPasteMaskInRules; }
            set
            {
                adjustPasteMaskInRules = value;
                OnPropertyChanged(nameof(AdjustPasteMaskInRules));

            }
        }

        [Browsable(false)]
        public double PadNameFontSize
        {
            get
            {
                var rect = GetBoundingRectangle();

                var fs = CanvasTextHelper.GetFontSize(number, new XSize(rect.Width, rect.Height), signal != null);

                return fs;
            }
        }

        [Browsable(false)]
        public IPosition PadNamePosition
        {
            get
            {
                var fontSize = PadNameFontSize;
                //var sLen = 1.0d;
                //if (!string.IsNullOrEmpty(number))
                //    sLen = number.Length;


                //var baseFontSize = 0.8;
                //var left = 0.08 * fontSize / baseFontSize;
                //var right = 0.08 * fontSize / baseFontSize;
                //var top = 0.3 * fontSize / baseFontSize;
                //var bottom = 0.18 * fontSize / baseFontSize;

                //var w = sLen * (fontSize + left + right);
                //var h = fontSize + top + bottom;

                ////wpf rounds off to 1/96 points for width and height
                ////so we must convert to dpi, round the value and then back to mm
                //var dotsWidth = Math.Round(MilimetersToDpiHelper.ConvertToDpi(w));
                //var dotsHeight = Math.Round(MilimetersToDpiHelper.ConvertToDpi(h));
                //if (dotsWidth < 1.00d)
                //    dotsWidth = 1.00d;
                //if (dotsHeight < 1.00d)
                //    dotsHeight = 1.00d;
                //w = MilimetersToDpiHelper.ConvertToMM(dotsWidth);
                //h = MilimetersToDpiHelper.ConvertToMM(dotsHeight);

                var size = CanvasTextHelper.GetSizeForPadText(number, fontSize);

                if (signal != null)
                {
                    return new PositionData
                    {
                        X = -0.25 * size.Width,
                        Y = -0.25 * fontSize
                    };
                }

                return new PositionData
                {
                    X = -0.25 * size.Width,
                    Y = -0.5 * size.Height
                };
            }
        }

        [Browsable(false)]
        public int PadTextZIndex
        {
            get
            {
                //if (Layer != null)
                //    return Layer.ZIndex + 1;
                return 5000;//always on top
            }
        }

        IBoardNetDesignerItem signal;

        //[Browsable(false)]
        [Display(Order = 4)]
        [Editor(EditorNames.BoardNetDesignerItemEditor, EditorNames.BoardNetDesignerItemEditor)]
        [MarksDirty]
        public IBoardNetDesignerItem Signal
        {
            get
            {
                return signal;
            }
            set
            {
                //if (signal == value)
                //    return;

                if (signal != null)
                {
                    var toRemove = signal.Pads.Where(p => p.Number == Number && p.FootprintInstanceId == FootprintInstanceId).ToList();

                    foreach (var p in toRemove)
                        signal.Pads.Remove(p);

                    signal.Items.Remove(this);
                }


                signal = value;

                if (signal != null)
                {
                    var fpt = ParentObject as FootprintBoardCanvasItem;
                    if (fpt != null)
                        signal.Pads.Add(this);
                    else
                        signal.Items.Add(this);
                }

                OnPropertyChanged(nameof(Signal));
            }
        }

        public void AssignSignal(IBoardNetDesignerItem newSignal)
        {
            signal = newSignal;
        }

        [Browsable(false)]
        public double SignalNameFontSize
        {
            get
            {
                //var w = GetBoundingRectangle().Width;
                //var d = 0.5 * 0.8 * w;

                //var hasSignal = signal != null && !string.IsNullOrEmpty(signal.Name);
                //if (hasSignal)
                //    d = 0.9 * w / signal.Name.Length;
                //return d;

                var rect = GetBoundingRectangle();

                if (signal == null)
                    return 0.5 * 0.8 * Math.Min(rect.Width, rect.Height);

                var fs = CanvasTextHelper.GetFontSize(signal.Name, new XSize(rect.Width, rect.Height), signal != null);

                return fs;
            }
        }

        [Browsable(false)]
        public IPosition SignalNamePosition
        {
            get
            {
                var fontSize = SignalNameFontSize;

                if (signal == null)
                    return null;

                var size = CanvasTextHelper.GetSizeForPadText(signal.Name, fontSize);

                return new PositionData
                {
                    X = -0.25 * size.Width,
                    Y = -0.9 * size.Height//-0.75 * fontSize
                };
            }
        }



        double GetSignalTextLength(string signalName)
        {
            //assume width of letter is the same as height
            if (!string.IsNullOrEmpty(signalName))
                return SignalNameFontSize * signalName.Length;

            return 0;
        }

        [Browsable(false)]
        public bool PadSignalNameIsVisible
        {
            get
            {
                //we have a signal and it fits
                var hasSignal = signal != null;
                return hasSignal;
                //var visibleSize = width;
                ////if (Parent != null)
                ////    visibleSize *= Parent.Scale;
                //return hasSignal && visibleSize >= GetSignalTextLength(PadRefDesignerItem.Signal.Name);
            }
        }

        double x;

        /// <summary>
        /// Location X of rectangle in current coordinate system
        /// </summary>
        [Editor(EditorNames.PositionXUnitsEditor, EditorNames.PositionXUnitsEditor)]
        [Display(Order = 6)]
        [MarksDirty]
        public double X
        {
            get { return x; }
            set
            {
                x = value;
                OnPropertyChanged(nameof(X));
                // OnPropertyChanged(nameof(Rect));
                //OnPropertyChanged(nameof(RectPasteMask));
                //OnPropertyChanged(nameof(RectSolderMask));
            }
        }

        double y;

        /// <summary>
        /// Location X of rectangle in current coordinate system
        /// </summary>
        [Editor(EditorNames.PositionYUnitsEditor, EditorNames.PositionYUnitsEditor)]
        [Display(Order = 7)]
        [MarksDirty]
        public double Y
        {
            get { return y; }
            set
            {
                y = value;
                OnPropertyChanged(nameof(Y));
                //OnPropertyChanged(nameof(Rect));
                //OnPropertyChanged(nameof(RectPasteMask));
                //OnPropertyChanged(nameof(RectSolderMask));
            }
        }

        double rot;

        [Display(Order = 8)]
        [MarksDirty]
        public double Rot
        {
            get { return rot; }
            set
            {
                rot = (int)value % 360;
                OnPropertyChanged(nameof(Rot));
            }
        }

        protected double width;

        [Editor(EditorNames.SizeMilimetersUnitsEditor, EditorNames.SizeMilimetersUnitsEditor)]
        [Display(Order = 4)]
        [MarksDirty]
        public double Width
        {
            get { return width; }
            set
            {
                width = value;
                OnPropertyChanged(nameof(Width));
                // OnPropertyChanged(nameof(PadNameFontSize));
            }
        }

        protected double height;

        [Editor(EditorNames.SizeMilimetersUnitsEditor, EditorNames.SizeMilimetersUnitsEditor)]
        [Display(Order = 5)]
        [MarksDirty]
        public double Height
        {
            get { return height; }
            set
            {
                height = value;
                OnPropertyChanged(nameof(Height));
            }
        }

        protected double cornerRadius;

        [Editor(EditorNames.SizeMilimetersUnitsEditor, EditorNames.SizeMilimetersUnitsEditor)]
        [Display(Order = 2)]
        [MarksDirty]
        public double CornerRadius
        {
            get { return cornerRadius; }
            set
            {
                cornerRadius = value;
                OnPropertyChanged(nameof(CornerRadius));
            }
        }

        //protected override int GetZIndex()
        //{
        //    return 0;
        //}

        public override void Translate(double dx, double dy)
        {
            X += dx;
            Y += dy;
        }

        public override void MirrorX()
        {
            ScaleX *= -1;
        }

        public override void MirrorY()
        {
            ScaleY *= -1;
        }

        public override void TransformBy(XMatrix matrix)
        {
            var p = new XPoint(x, y);
            p = matrix.Transform(p);
            X = p.X;
            Y = p.Y;

            // Rot = GetRotationSafe(GetWorldRotation(rot, matrix));

            var rotAngle = GetRotationAngleFromMatrix(matrix);
            Rot = RotateSafe(Rot, rotAngle);
        }

        protected override XTransform GetLocalRotationTransform()
        {
            return new XRotateTransform(Rot);
        }

        protected override XTransform GetLocalTranslationTransform()
        {
            return new XTranslateTransform(X, Y);
        }

        public override void Rotate(double angle = 90)
        {
            Rot = RotateSafe(Rot, angle);
        }

        public override void RemoveFromCanvas()
        {
            Signal = null;

            base.RemoveFromCanvas();
        }

        protected virtual void EnsurePrimitives()
        {

        }

        public override void LoadLayers()
        {
            base.LoadLayers();

            EnsurePrimitives();
        }
    }

}
