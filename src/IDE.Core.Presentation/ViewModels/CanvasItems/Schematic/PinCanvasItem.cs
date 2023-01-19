using IDE.Core.Storage;

using System;
using System.ComponentModel;
using IDE.Core.Interfaces;
using IDE.Core.Common;
using System.Windows.Input;
using IDE.Core.Commands;
using IDE.Core.Types.Media;
using IDE.Core.Common.Utilities;
using System.ComponentModel.DataAnnotations;
using IDE.Core.Types.Attributes;

namespace IDE.Core.Designers
{
    //todo
    /*
     * PIN
	- pinName
	- nameVisible
	- pinNumber (designator)
	- pinNumberVisible
	- color (lineColor / textColor )
	NO COLORS FOR PINS for now
    */
    public class PinCanvasItem : //BaseCanvasItem
                                NetSegmentCanvasItem
                               , IPinCanvasItem
    {

        public PinCanvasItem()
        {
            PinLength = 2.54;
            Width = 0.5;
            Number = "1";
            Name = Number.ToString();
            ShowName = true;
            ShowNumber = true;

            PinColor = XColors.Navy;
            PinNameColor = XColors.Navy;
            PinNumberColor = XColors.Navy;
        }



        protected override void PropertyChangedHandleInternal(object sender, PropertyChangedEventArgs e)
        {
            base.PropertyChangedHandleInternal(sender, e);

            if (e.PropertyName == nameof(Net))
                OnPropertyChanged(nameof(IsConnected));
        }


        string number;

        [Display(Order = 1)]
        [MarksDirty]
        public string Number
        {
            get { return number; }
            set
            {
                number = value;

                OnPropertyChanged(nameof(Number));
            }
        }

        [Browsable(false)]
        public SchematicSymbolCanvasItem SchematicPart
        {
            get { return ParentObject as SchematicSymbolCanvasItem; }
        }

        [Browsable(false)]
        public string PartInstanceId
        {
            get
            {
                var part = SchematicPart;
                if (part != null)
                    return part.SymbolPrimitive.Id;

                return null;
            }
        }

        bool showNumber;

        [Display(Order = 2)]
        [MarksDirty]
        public bool ShowNumber
        {
            get { return showNumber; }
            set
            {
                showNumber = value;
                OnPropertyChanged(nameof(ShowNumber));
            }
        }

        [Browsable(false)]
        public IPosition PinNumberPosition { get; set; } = new PositionData { X = 0.3, Y = -1.5 };

        string name;

        [Display(Order = 3)]
        [MarksDirty]
        public string Name
        {
            get { return name; }
            set
            {
                name = value;

                OnPropertyChanged(nameof(Name));
            }
        }

        bool showName;

        [Display(Order = 4)]
        [MarksDirty]
        public bool ShowName
        {
            get { return showName; }
            set
            {
                showName = value;
                OnPropertyChanged(nameof(ShowName));
            }
        }

        [Browsable(false)]
        public IPosition PinNamePosition { get; set; } = new PositionData { X = 1, Y = -1.45d };

        pinOrientation orientation;

        /// <summary>
        /// orientation of the connection point is facing
        /// </summary>
        [Display(Order = 5)]
        [MarksDirty]
        public pinOrientation Orientation
        {
            get { return orientation; }
            set
            {
                orientation = value;

                OnPropertyChanged(nameof(Orientation));
                OnPropertyChanged(nameof(LineX2));
                OnPropertyChanged(nameof(LineY2));
            }
        }

        PinType pinType;

        [Display(Order = 6)]
        [MarksDirty]
        public PinType PinType
        {
            get { return pinType; }
            set
            {
                pinType = value;
                OnPropertyChanged(nameof(PinType));
            }
        }

        double x;

        /// <summary>
        /// Location X of rectangle in current coordinate system
        /// </summary>
        [Editor(EditorNames.PositionXUnitsEditor, EditorNames.PositionXUnitsEditor)]
        [Display(Order = 7)]
        [MarksDirty]
        public double X
        {
            get { return x; }
            set
            {
                x = value;
                OnPropertyChanged(nameof(X));
                OnPropertyChanged(nameof(Position));
                OnPropertyChanged(nameof(LineX2));
            }
        }

        double y;

        /// <summary>
        /// Location X of rectangle in current coordinate system
        /// </summary>
        [Editor(EditorNames.PositionYUnitsEditor, EditorNames.PositionYUnitsEditor)]
        [Display(Order = 8)]
        [MarksDirty]
        public double Y
        {
            get { return y; }
            set
            {
                y = value;
                OnPropertyChanged(nameof(Y));
                OnPropertyChanged(nameof(Position));
                OnPropertyChanged(nameof(LineY2));
            }
        }

        /// <summary>
        /// Pin position. This is also the connection point
        /// </summary>
        [Browsable(false)]
        public XPoint Position
        {
            get { return new XPoint(X, Y); }
        }


        /// <summary>
        /// Connection Point radius
        /// </summary>
        [Browsable(false)]
        public double Radius
        {
            get { return Width / 2; }
        }

        [Browsable(false)]
        public double LineX2
        {
            get
            {
                return PinLength;
            }
        }

        [Browsable(false)]
        public double LineY2
        {
            get
            {
                return 0;
            }
        }

        double width;

        /// <summary>
        /// Thickness of the line of the pin in mm
        /// </summary>
        //[Browsable(false)]
        [DisplayName("Pin thickness")]
        [Editor(EditorNames.SizeMilimetersUnitsEditor, EditorNames.SizeMilimetersUnitsEditor)]
        [MarksDirty]
        public double Width
        {
            get { return width; }
            set
            {
                width = value;
                OnPropertyChanged(nameof(Width));
                OnPropertyChanged(nameof(Radius));
            }
        }



        double pinLength;
        //length in mm
        [DisplayName("Pin length")]
        [Editor(EditorNames.SizeMilimetersUnitsEditor, EditorNames.SizeMilimetersUnitsEditor)]
        [MarksDirty]
        public double PinLength
        {
            get { return pinLength; }
            set
            {
                pinLength = value;
                OnPropertyChanged(nameof(PinLength));
                OnPropertyChanged(nameof(LineX2));
                OnPropertyChanged(nameof(LineY2));
            }
        }

        XColor pinColor;
        /// <summary>
        /// color for the line of the pin
        /// </summary>
        [Editor(EditorNames.XColorEditor, EditorNames.XColorEditor)]
        [MarksDirty]
        public XColor PinColor
        {
            get { return pinColor; }

            set
            {
                pinColor = value;
                OnPropertyChanged(nameof(PinColor));
            }
        }

        XColor pinNameColor;
        /// <summary>
        /// color for the line of the pin
        /// </summary>
        [Editor(EditorNames.XColorEditor, EditorNames.XColorEditor)]
        [MarksDirty]
        public XColor PinNameColor
        {
            get { return pinNameColor; }

            set
            {
                pinNameColor = value;
                OnPropertyChanged(nameof(PinNameColor));
            }
        }

        XColor pinNumberColor;
        /// <summary>
        /// color for the line of the pin
        /// </summary>
        [Editor(EditorNames.XColorEditor, EditorNames.XColorEditor)]
        [MarksDirty]
        public XColor PinNumberColor
        {
            get { return pinNumberColor; }

            set
            {
                pinNumberColor = value;
                OnPropertyChanged(nameof(PinNumberColor));
            }
        }



        //TODO: set Top, Left coordinates for PinNumber text and PinName text as a function of Orientation in DataTemplate

        // bool isConnected;
        [Browsable(false)]
        public bool IsConnected
        {
            get
            {
                return Net != null;
            }
        }

        bool isConnectionHovered;

        [Browsable(false)]
        public bool IsConnectionHovered
        {
            get
            {
                return isConnectionHovered;
            }
            set
            {
                isConnectionHovered = value;
                OnPropertyChanged(nameof(IsConnectionHovered));
            }
        }

        //double displayWidth;
        //[Browsable(false)]
        //public double PinDisplayWidth
        //{
        //    get
        //    {
        //        return displayWidth;
        //    }
        //    set
        //    {
        //        displayWidth = value;
        //        OnPropertyChanged(nameof(PinDisplayWidth));
        //    }
        //}

        //double displayHeight;
        //[Browsable(false)]
        //public double PinDisplayHeight
        //{
        //    get
        //    {
        //        return displayHeight;
        //    }
        //    set
        //    {
        //        displayHeight = value;
        //        OnPropertyChanged(nameof(PinDisplayHeight));
        //    }
        //}

        ICommand disconnectPinCommand;

        [Browsable(false)]
        public ICommand DisconnectPinCommand
        {
            get
            {
                if (disconnectPinCommand == null)
                {
                    disconnectPinCommand = CreateCommand(p =>
                      {
                          Net = null;
                      });
                }

                return disconnectPinCommand;
            }
        }

        public override void Translate(double dx, double dy)
        {
            X += dx;
            Y += dy;
        }

        public override void TransformBy(XMatrix matrix)
        {
            var sp = new XPoint(x, y);

            sp = matrix.Transform(sp);

            X = sp.X;
            Y = sp.Y;

            var rotAngle = GetRotationAngleFromMatrix(matrix);
            var rot = RotateSafe((double)orientation, rotAngle);


            Orientation = (pinOrientation)rot;
        }

        public override XRect GetBoundingRectangle()
        {
            var startPoint = new XPoint();
            var endPoint = new XPoint(PinLength, 0);

            var localTransform = new XTransformGroup();
            //local rotation
            localTransform.Children.Add(GetLocalRotationTransform());
            //local translation
            localTransform.Children.Add(GetLocalTranslationTransform());

            startPoint = localTransform.Transform(startPoint);
            endPoint = localTransform.Transform(endPoint);

            var r = new XRect(startPoint, endPoint);
            r.Inflate(0.5 * width);
            r.Width = Math.Round(r.Width, 4);
            r.Height = Math.Round(r.Height, 4);
            r.X = Math.Round(r.X, 4);
            r.Y = Math.Round(r.Y, 4);

            return r;
        }



        protected override void LoadFromPrimitiveInternal(IPrimitive primitive)
        {
            var p = (Pin)primitive;

            //IsActiveLow = p.IsActiveLow;
            PinType = p.pinType;
            Name = p.Name;
            Number = p.Number;
            Orientation = p.Orientation;
            PinLength = p.PinLength;
            Width = p.Width;
            ShowName = p.ShowName;
            ShowNumber = p.ShowNumber;
            PinNamePosition.X = p.PinNameX;
            PinNamePosition.Y = p.PinNameY;
            PinNamePosition.Rotation = p.PinNameRot;

            PinNumberPosition.X = p.PinNumberX;
            PinNumberPosition.Y = p.PinNumberY;
            PinNumberPosition.Rotation = p.PinNumberRot;

            X = p.x;
            Y = p.y;
            PinColor = XColor.FromHexString(p.PinColor);
            PinNameColor = XColor.FromHexString(p.PinNameColor);
            PinNumberColor = XColor.FromHexString(p.PinNumberColor);

            //var thickness = Width;
            //var pinLen = PinLength;//(double)Converters.PinLengthToDoubleConverter.Instance.Convert(PinLength, PinLength.GetType(), null, null);
            //try
            //{
            //    if (ShowName)
            //    {
            //        //pinLen += (double)TextToGridWidthConverter.Instance.Convert(Name, Name.GetType(), null, null);
            //        //pinLen += GetTextWidth(Name);

            //        dynamic textToGridWidthConverter = TypeActivator.CreateInstanceByTypeName<object>("IDE.Core.Converters.TextToGridWidthConverter");
            //        if (textToGridWidthConverter != null)
            //        {
            //            pinLen += (double)textToGridWidthConverter.Convert(Name, Name.GetType(), null, null);
            //        }
            //    }
            //}
            //catch { }

            //var width = 0.0d;
            //var height = 0.0d;
            //switch (orientation)
            //{
            //    case pinOrientation.Left:
            //    case pinOrientation.Right:
            //        width = pinLen;
            //        height = thickness;
            //        break;
            //    case pinOrientation.Down:
            //    case pinOrientation.Up:
            //        width = thickness;
            //        height = pinLen;
            //        break;
            //}

            //PinDisplayWidth = width;
            //PinDisplayHeight = height;
        }

        double GetTextWidth(string text, double fontSize = 16.0d)
        {
            const double standardGridSize = 10.0;
            // const double fontSize = 16;

            if (string.IsNullOrWhiteSpace(text))
                return 0.0d;

            var textLength = fontSize * text.Length;

            //a whole number of grid size
            var gridUnits = (int)(Math.Round(textLength / standardGridSize + 0.5));
            var width = gridUnits * standardGridSize + 5;

            return width;
        }

        public override IPrimitive SaveToPrimitive()
        {
            var p = new Pin();

            p.pinType = PinType;
            p.Name = Name;
            p.Number = Number;
            p.Orientation = Orientation;
            p.PinLength = PinLength;
            p.Width = Width;
            p.ShowName = ShowName;
            p.ShowNumber = ShowNumber;
            p.x = X;
            p.y = Y;

            p.PinNameX = PinNamePosition.X;
            p.PinNameY = PinNamePosition.Y;
            p.PinNameRot = PinNamePosition.Rotation;

            p.PinNumberX = PinNumberPosition.X;
            p.PinNumberY = PinNumberPosition.Y;
            p.PinNumberRot = PinNumberPosition.Rotation;

            p.PinColor = PinColor.ToHexString();
            p.PinNameColor = PinNameColor.ToHexString();
            p.PinNumberColor = PinNumberColor.ToHexString();

            return p;
        }

        protected override XTransform GetLocalRotationTransform()
        {
            var rot = this.GetAngleOrientation(false);
            return new XRotateTransform(rot);
        }

        protected override XTransform GetLocalTranslationTransform()
        {
            return new XTranslateTransform(X, Y);
        }

        public override void MirrorX()
        {
            if (orientation == pinOrientation.Left)
                Orientation = pinOrientation.Right;
            else if (orientation == pinOrientation.Right)
                Orientation = pinOrientation.Left;
        }

        public override void MirrorY()
        {
            if (orientation == pinOrientation.Up)
                Orientation = pinOrientation.Down;
            else if (orientation == pinOrientation.Down)
                Orientation = pinOrientation.Up;
        }

        public override void Rotate()
        {
            var o = (int)orientation;
            o = (o + 90) % 360;
            Orientation = (pinOrientation)o;
        }

        public override string ToString()
        {
            return $"Pin {Number} ({Name})";
        }
    }

}
