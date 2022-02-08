using IDE.Core.Common;
using IDE.Core.Interfaces;
using IDE.Core.Storage;
using IDE.Core.Types.Attributes;
using IDE.Core.Types.Media;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reflection;

namespace IDE.Core.Designers
{
    //consider renaming to TextStrokedLineBoardCanvasItem
    public class TextSingleLineBoardCanvasItem : SingleLayerBoardCanvasItem, IPlainDesignerItem, ITextMonoLineCanvasItem
    {
        public TextSingleLineBoardCanvasItem()
        {
            FontName = "Default";
            Text = "Text";
            PropertyChanged += TextSingleLineBoardCanvasItem_PropertyChanged;
        }

        void TextSingleLineBoardCanvasItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Layer))
            {
                foreach (var letter in LetterItems)
                    foreach (var item in letter.Items.OfType<SingleLayerBoardCanvasItem>())
                        item.AssignLayer(Layer);
            }
        }

        FontFileModel innerFont = new FontFileModel();

        string fontName;
        [Display(Order = 2)]
        [MarksDirty]
        public string FontName
        {
            get { return fontName; }
            set
            {
                if (fontName == value)
                    return;

                fontName = value;
                innerFont.FontName = fontName;
                //ReloadFont();
                ReloadText();
                OnPropertyChanged(nameof(FontName));
            }
        }

        // Dictionary<string, List<ISelectableItem>> fontItems = new Dictionary<string, List<ISelectableItem>>();
        //void ReloadFont()
        //{
        //    try
        //    {
        //        if (string.IsNullOrEmpty(fontName))
        //            return;

        //        var fontFolder = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location),
        //                                       "Templates",
        //                                       "Fonts",
        //                                       fontName);
        //        //load all characters for this font
        //        fontItems.Clear();
        //        foreach (var fontFile in Directory.GetFiles(fontFolder, "*.symbol"))
        //        {
        //            var fontSymbol = XmlHelper.Load<FontSymbol>(fontFile);
        //            fontSymbol.Name = Path.GetFileNameWithoutExtension(fontFile);

        //            var items = fontSymbol.GetDesignerPrimitiveItems();
        //            items.ForEach(c => c.CanEdit = false);
        //            fontItems.Add(fontSymbol.Name.ToUpper(), items);
        //        }

        //    }
        //    catch
        //    {//log to output

        //    }
        //}


        double fontSize = 1;

        [Display(Order = 3)]
        [MarksDirty]
        public double FontSize //in mm
        {
            get { return fontSize; }
            set
            {
                if (fontSize == value) return;

                fontSize = value;
                innerFont.FontSize = FontSize;
                OnPropertyChanged(nameof(FontSize));

                //reload to size 1
                //ReloadFont();
                ReloadText();

                //foreach (var letter in LetterItems)
                //    foreach (var item in letter.Items)
                //    {
                //        if (item is ArcBoardCanvasItem)
                //        {
                //            var arc = item as ArcBoardCanvasItem;
                //            arc.BorderWidth = strokeWidth;
                //            arc.StartPointX *= fontSize;
                //            arc.StartPointY *= fontSize;
                //            arc.EndPointX *= fontSize;
                //            arc.EndPointY *= fontSize;
                //            arc.Radius *= fontSize;//?
                //            arc.AssignLayer(Layer);
                //        }
                //        else if (item is LineBoardCanvasItem)
                //        {
                //            var line = item as LineBoardCanvasItem;
                //            line.Width = strokeWidth;
                //            line.X1 *= fontSize;
                //            line.Y1 *= fontSize;
                //            line.X2 *= fontSize;
                //            line.Y2 *= fontSize;
                //            line.AssignLayer(Layer);
                //        }
                //    }
            }
        }

        double strokeWidth = 0.2;

        [Display(Order = 4)]
        [MarksDirty]
        public double StrokeWidth //in mm
        {
            get { return strokeWidth; }
            set
            {
                if (strokeWidth == value) return;

                strokeWidth = value;
                innerFont.StrokeWidth = strokeWidth;
                OnPropertyChanged(nameof(StrokeWidth));

                ReloadText();//?

                //foreach (var letter in LetterItems)
                //    foreach (var item in letter.Items)
                //    {
                //        if (item is ArcBoardCanvasItem)
                //        {
                //            (item as ArcBoardCanvasItem).BorderWidth = strokeWidth;
                //        }
                //        else if (item is LineBoardCanvasItem)
                //        {
                //            (item as LineBoardCanvasItem).Width = strokeWidth;
                //        }
                //    }
            }
        }

        [Browsable(false)]
        public IList<ILetterItem> LetterItems { get; set; } = new ObservableCollection<ILetterItem>();

        string text;
        [Display(Order = 5)]
        [MarksDirty]
        public string Text
        {
            get { return text; }
            set
            {
                if (text == value) return;
                text = value;
                ReloadText();
                OnPropertyChanged(nameof(Text));
            }
        }

        void ReloadText()
        {
            LetterItems.Clear();
            if (string.IsNullOrEmpty(text)) return;

            //foreach (var character in text.ToCharArray())
            //{
            //    var li = new LetterItem() { FontSize = FontSize };
            //    var charString = character.ToString();
            //    var upperCharString = charString.ToUpper();
            //    List<ISelectableItem> letterStrokes = null;
            //    if (fontItems.ContainsKey(charString))
            //        letterStrokes = fontItems[charString];
            //    else if (fontItems.ContainsKey(upperCharString))
            //        letterStrokes = fontItems[upperCharString];
            //    else letterStrokes = new List<ISelectableItem>();//space

            //    if (letterStrokes != null)
            //        li.Items.AddRange(letterStrokes.Select(l => (BaseCanvasItem)l.Clone()));

            //    LetterItems.Add(li);
            //}

            var items = innerFont.GetTextItems(text, Layer);
            LetterItems.AddRange(items);
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
                rot = value;
                OnPropertyChanged(nameof(Rot));
            }
        }

        public override void Translate(double dx, double dy)
        {
            X += dx;
            Y += dy;
        }

        public override XRect GetBoundingRectangle()
        {
            var w = fontSize;
            if (!string.IsNullOrEmpty(text))
                w = text.Length * fontSize;

            var r = new XRect(x, y, w, fontSize);
            return r;
        }

        protected override void LoadFromPrimitiveInternal(IPrimitive primitive)
        {
            var t = (TextSingleLineBoard)primitive;
            LayerId = t.layerId;
            FontName = t.FontName;
            FontSize = t.FontSize;
            Text = t.Value;
            X = t.x;
            Y = t.y;
            Rot = t.rot;
            StrokeWidth = t.StrokeWidth;
            IsLocked = t.IsLocked;
        }

        public override IPrimitive SaveToPrimitive()
        {
            var t = new TextSingleLineBoard();

            t.Value = Text;
            t.x = X;
            t.y = Y;
            t.rot = Rot;
            t.FontName = FontName;
            t.FontSize = FontSize;
            t.StrokeWidth = StrokeWidth;
            t.layerId = (Layer?.LayerId).GetValueOrDefault();
            t.IsLocked = IsLocked;

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

        public override void TransformBy(XMatrix matrix)
        {
            var p = new XPoint(x, y);
            p = matrix.Transform(p);
            X = p.X;
            Y = p.Y;

            Rot = GetWorldRotation(rot, matrix);
        }

        protected override XTransform GetLocalRotationTransform()
        {
            return new XRotateTransform(Rot);
        }

        protected override XTransform GetLocalTranslationTransform()
        {
            return new XTranslateTransform(X, Y);
        }

        protected override XTransform GetLocalMirrorTransform()
        {
            if (ParentObject == null && IsMirrored())
            {
                return new XScaleTransform
                {
                    ScaleX = -1,
                };
            }

            return base.GetLocalMirrorTransform();
        }

        protected override XTransform GetGlobalMirror(IContainerSelectableItem parentObject)
        {
            if (IsMirrored())
            {
                return new XScaleTransform
                {
                    ScaleX = -1,
                };
            }


            return XTransform.Identity;
        }

        public override void Rotate()
        {
            Rot = RotateSafe(Rot);
        }

        public override void LoadLayers()
        {
            base.LoadLayers();

            foreach (var letter in LetterItems)
            {
                foreach (SingleLayerBoardCanvasItem li in letter.Items)
                {
                    li.LayerDocument = LayerDocument;
                    li.AssignLayer(Layer);
                }

            }
        }

        public override string ToString()
        {
            //trim the text to show for a single line
            var maxLen = 10;
            var t = Text.Replace(Environment.NewLine, " ");
            if (t.Length > maxLen)
                t = Text.Substring(0, maxLen);

            return $"Text ({t})";
        }
    }

    public class LetterItem : ILetterItem
    {
        public double FontSize { get; set; }
        public IList<ISelectableItem> Items { get; set; } = new ObservableCollection<ISelectableItem>();
    }

    public class DesignatorBoardCanvasItem : TextSingleLineBoardCanvasItem
    {
        protected override XTransform GetLocalRotationTransform()
        {
            //return new XRotateTransform(Rot);
            var parentObject = ParentObject as FootprintBoardCanvasItem;
            if (parentObject != null)
            {
                return new XRotateTransform(parentObject.PartNamePosition.Rotation);
            }

            return XTransform.Identity;
        }

        protected override XTransform GetLocalTranslationTransform()
        {
            var parentObject = ParentObject as FootprintBoardCanvasItem;
            var posX = X;
            var posY = Y;
            if (parentObject != null)
            {
                posX += parentObject.PartNamePosition.X;
                posY += parentObject.PartNamePosition.Y;
            }

            return new XTranslateTransform(posX, posY);
        }

        protected override XTransform GetLocalMirrorTransform()
        {
            if (ParentObject == null && IsMirrored())
            {
                return new XScaleTransform
                {
                    ScaleX = -1,
                };
            }

            return base.GetLocalMirrorTransform();
        }

        protected override XTransform GetGlobalMirror(IContainerSelectableItem parentObject)
        {
            if (IsMirrored())
            {
                return new XScaleTransform
                {
                    ScaleX = -1,
                };
            }


            return XTransform.Identity;
        }
        protected override XTransform GetGlobalRotation(IContainerSelectableItem parentObject)
        {
            return XTransform.Identity;
        }

        public override object Clone()
        {
            var clone = (IPrimitive)SaveToPrimitive().Clone();
            var designator = new DesignatorBoardCanvasItem();
            designator.LoadFromPrimitive(clone);

            designator.LayerDocument = LayerDocument;
            designator.ParentObject = ParentObject;
            designator.AssignLayer(Layer);

            return designator;
        }
    }

    public class FontFileModel : BaseViewModel
    {
        string fontName;
        public string FontName
        {
            get { return fontName; }
            set
            {
                if (fontName == value)
                    return;

                fontName = value;
                ReloadFont();
                //ReloadText();
                OnPropertyChanged(nameof(FontName));
            }
        }

        double fontSize = 1;

        [Display(Order = 3)]
        public double FontSize //in mm
        {
            get { return fontSize; }
            set
            {
                if (fontSize == value) return;

                fontSize = value;
                OnPropertyChanged(nameof(FontSize));

                ////reload to size 1
                //ReloadFont();
                //ReloadText();

                //foreach (var letter in LetterItems)
                //{
                //    foreach (var item in letter.Items)
                //    {
                //        if (item is ArcBoardCanvasItem)
                //        {
                //            var arc = item as ArcBoardCanvasItem;
                //            arc.BorderWidth = strokeWidth;
                //            arc.StartPointX *= fontSize;
                //            arc.StartPointY *= fontSize;
                //            arc.EndPointX *= fontSize;
                //            arc.EndPointY *= fontSize;
                //            arc.Radius *= fontSize;//?
                //            arc.AssignLayer(Layer);
                //        }
                //        else if (item is LineBoardCanvasItem)
                //        {
                //            var line = item as LineBoardCanvasItem;
                //            line.Width = strokeWidth;
                //            line.X1 *= fontSize;
                //            line.Y1 *= fontSize;
                //            line.X2 *= fontSize;
                //            line.Y2 *= fontSize;
                //            line.AssignLayer(Layer);
                //        }
                //    }
                //}
            }
        }

        public double strokeWidth = 0.2;
        public double StrokeWidth //in mm
        {
            get { return strokeWidth; }
            set
            {
                if (strokeWidth == value) return;

                strokeWidth = value;
                OnPropertyChanged(nameof(StrokeWidth));

                //foreach (var letter in LetterItems)
                //    foreach (var item in letter.Items)
                //    {
                //        if (item is ArcBoardCanvasItem)
                //        {
                //            (item as ArcBoardCanvasItem).BorderWidth = strokeWidth;
                //        }
                //        else if (item is LineBoardCanvasItem)
                //        {
                //            (item as LineBoardCanvasItem).Width = strokeWidth;
                //        }
                //    }
            }
        }

        Dictionary<string, List<ISelectableItem>> fontItems = new Dictionary<string, List<ISelectableItem>>();

        //public IList<ILetterItem> LetterItems { get; set; } = new ObservableCollection<ILetterItem>();

        void ReloadFont()
        {
            try
            {
                if (string.IsNullOrEmpty(fontName))
                    return;

                var fontPath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location),
                                               "Templates",
                                               "Fonts",
                                               fontName + ".font");

                if (File.Exists(fontPath))
                {
                    var fontFileData = XmlHelper.Load<FontDocument>(fontPath);
                    //load all characters for this font
                    fontItems.Clear();
                    //foreach (var fontFile in Directory.GetFiles(fontFolder, "*.symbol"))
                    foreach (var fontSymbol in fontFileData.Symbols)
                    {
                        //var fontSymbol = XmlHelper.Load<FontSymbol>(fontFile);
                        //fontSymbol.Name = Path.GetFileNameWithoutExtension(fontFile);

                        var items = fontSymbol.GetDesignerPrimitiveItems();
                        items.ForEach(c => c.CanEdit = false);
                        fontItems.Add(fontSymbol.Name, items);
                    }
                }
            }
            catch
            {//log to output

            }
        }

        public List<LetterItem> GetTextItems(string text, ILayerDesignerItem layer)
        {
            var letterItems = new List<LetterItem>();

            if (string.IsNullOrEmpty(text) == false)
            {
                foreach (var character in text.ToCharArray())
                {
                    var li = new LetterItem() { FontSize = FontSize };
                    var charString = character.ToString();
                    var upperCharString = charString.ToUpper();
                    List<ISelectableItem> letterStrokes = null;
                    if (fontItems.ContainsKey(charString))
                        letterStrokes = fontItems[charString];
                    else if (fontItems.ContainsKey(upperCharString))
                        letterStrokes = fontItems[upperCharString];
                    else letterStrokes = new List<ISelectableItem>();//space

                    if (letterStrokes != null)
                    {
                        //li.Items.AddRange(letterStrokes.Select(l => (BaseCanvasItem)l.Clone()));

                        foreach (var ls in letterStrokes)
                        {
                            var item = (SingleLayerBoardCanvasItem)ls.Clone();
                            item.CanEdit = false;
                            ApplyFontSize(item);
                            ApplyLayer(item, layer);


                            li.Items.Add(item);
                        }
                    }


                    letterItems.Add(li);
                }
            }

            return letterItems;
        }

        void ApplyFontSize(SingleLayerBoardCanvasItem item)
        {
            if (item is ArcBoardCanvasItem)
            {
                var arc = item as ArcBoardCanvasItem;
                arc.BorderWidth = strokeWidth;
                arc.StartPointX *= fontSize;
                arc.StartPointY *= fontSize;
                arc.EndPointX *= fontSize;
                arc.EndPointY *= fontSize;
                arc.Radius *= fontSize;//?
            }
            else if (item is LineBoardCanvasItem)
            {
                var line = item as LineBoardCanvasItem;
                line.Width = strokeWidth;
                line.X1 *= fontSize;
                line.Y1 *= fontSize;
                line.X2 *= fontSize;
                line.Y2 *= fontSize;
            }
        }

        void ApplyLayer(SingleLayerBoardCanvasItem item, ILayerDesignerItem layer)
        {
            item.AssignLayer(layer);
        }
    }

    /*
    public class DesignatorBoardCanvasItem : SingleLayerBoardCanvasItem, IPlainDesignerItem
    {
        public DesignatorBoardCanvasItem()
        {
            FontName = "Default";
            Text = "Text";
            PropertyChanged += DesignatorBoardCanvasItem_PropertyChanged;
        }

        void DesignatorBoardCanvasItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Layer))
            {
                //foreach (var letter in LetterItems)
                //    foreach (var item in letter.Items.OfType<SingleLayerBoardCanvasItem>())
                //        item.AssignLayer(Layer);

                EnsureLayerForItems();
            }
        }

        string fontName;
        public string FontName
        {
            get { return fontName; }
            set
            {
                if (fontName == value)
                    return;

                fontName = value;
                ReloadFont();
                ReloadText();
                OnPropertyChanged(nameof(FontName));
            }
        }

        Dictionary<string, List<BaseCanvasItem>> fontItems = new Dictionary<string, List<BaseCanvasItem>>();
        void ReloadFont()
        {
            try
            {
                if (string.IsNullOrEmpty(fontName))
                    return;

                var fontFolder = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location),
                                               "Templates",
                                               "Fonts",
                                               fontName);
                //load all characters for this font
                fontItems.Clear();
                foreach (var fontFile in Directory.GetFiles(fontFolder, "*.symbol"))
                {
                    var fontSymbol = XmlHelper.Load<FontSymbol>(fontFile);
                    fontSymbol.Name = Path.GetFileNameWithoutExtension(fontFile);

                    var items = fontSymbol.GetDesignerPrimitiveItems();
                    items.ForEach(c => c.CanEdit = false);
                    fontItems.Add(fontSymbol.Name, items);
                }

            }
            catch
            {//log to output

            }
        }


        double fontSize = 1;

        [Display(Order = 3)]
        public double FontSize //in mm
        {
            get { return fontSize; }
            set
            {
                if (fontSize == value) return;

                fontSize = value;
                OnPropertyChanged(nameof(FontSize));

                //reload to size 1
                ReloadFont();
                ReloadText();

                foreach (var letter in LetterItems)
                    foreach (var item in letter.Items)
                    {
                        if (item is ArcBoardCanvasItem)
                        {
                            var arc = item as ArcBoardCanvasItem;
                            arc.BorderWidth = strokeWidth;
                            arc.StartPointX *= fontSize;
                            arc.StartPointY *= fontSize;
                            arc.EndPointX *= fontSize;
                            arc.EndPointY *= fontSize;
                            arc.SizeDiameter *= fontSize;//?
                            arc.AssignLayer(Layer);
                        }
                        else if (item is LineBoardCanvasItem)
                        {
                            var line = item as LineBoardCanvasItem;
                            line.Width = strokeWidth;
                            line.X1 *= fontSize;
                            line.Y1 *= fontSize;
                            line.X2 *= fontSize;
                            line.Y2 *= fontSize;
                            line.AssignLayer(Layer);
                        }
                    }
            }
        }

        public double strokeWidth = 0.2;
        public double StrokeWidth //in mm
        {
            get { return strokeWidth; }
            set
            {
                if (strokeWidth == value) return;

                strokeWidth = value;
                OnPropertyChanged(nameof(StrokeWidth));

                foreach (var letter in LetterItems)
                    foreach (var item in letter.Items)
                    {
                        if (item is ArcBoardCanvasItem)
                        {
                            (item as ArcBoardCanvasItem).BorderWidth = strokeWidth;
                        }
                        else if (item is LineBoardCanvasItem)
                        {
                            (item as LineBoardCanvasItem).Width = strokeWidth;
                        }
                    }
            }
        }

        [Browsable(false)]
        public ObservableCollection<LetterItem> LetterItems { get; set; } = new ObservableCollection<LetterItem>();

        string text;
        [Display(Order = 8)]
        public string Text
        {
            get { return text; }
            set
            {
                if (text == value) return;
                text = value;
                ReloadText();
                OnPropertyChanged(nameof(Text));
            }
        }

        void ReloadText()
        {
            LetterItems.Clear();
            if (string.IsNullOrEmpty(text)) return;

            foreach (var character in text.ToCharArray())
            {
                var li = new LetterItem() { FontSize = FontSize };
                var charString = character.ToString();
                var upperCharString = charString.ToUpper();
                List<BaseCanvasItem> letterStrokes = null;
                if (fontItems.ContainsKey(charString))
                    letterStrokes = fontItems[charString];
                else if (fontItems.ContainsKey(upperCharString))
                    letterStrokes = fontItems[upperCharString];
                else letterStrokes = new List<BaseCanvasItem>();//space

                if (letterStrokes != null)
                    li.Items.AddRange(letterStrokes.Select(l => (BaseCanvasItem)l.Clone()));

                LetterItems.Add(li);
            }
        }

        double x;

        /// <summary>
        /// Location X of rectangle in current coordinate system
        /// </summary>
        [Editor(typeof(PositionXUnitsEditor), typeof(PositionXUnitsEditor))]
        [Display(Order = 9)]
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
        [Editor(typeof(PositionYUnitsEditor), typeof(PositionYUnitsEditor))]
        [Display(Order = 10)]
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
        [Display(Order = 12)]
        public double Rot
        {
            get { return rot; }
            set
            {
                rot = value;
                OnPropertyChanged(nameof(Rot));
            }
        }

        public override void Translate(double dx, double dy)
        {
            X += dx;
            Y += dy;
        }

        public override Rect GetBoundingRectangle()
        {
            var w = fontSize;
            if (!string.IsNullOrEmpty(text))
                w = text.Length * fontSize;

            var r = new Rect(x, y, w, fontSize);
            return r;
        }

        public override void PlacingMouseMove(PlacementStatus status, Point _mousePosition)
        {
            //var mousePosition = SnapToGrid(_mousePosition);
            //switch (status)
            //{
            //    case PlacementStatus.Ready:
            //        X = mousePosition.X;
            //        Y = mousePosition.Y;
            //        break;
            //}
        }

        public override void PlacingMouseUp(PlacementData status, Point _mousePosition)
        {
            //var mousePosition = SnapToGrid(_mousePosition);
            //switch (status.PlacementStatus)
            //{
            //    case PlacementStatus.Ready:
            //        X = mousePosition.X;
            //        Y = mousePosition.Y;
            //        IsPlaced = true;
            //        Parent.OnDrawingChanged();

            //        //create another text
            //        var canvasItem = (TextSingleLineBoardCanvasItem)Activator.CreateInstance(GetType());
            //        canvasItem.X = mousePosition.X;
            //        canvasItem.Y = mousePosition.Y;
            //        canvasItem.Layer = Layer;

            //        Parent.PlacingObject = new PlacementData
            //        {
            //            PlacementStatus = PlacementStatus.Ready,
            //            PlacingObjects = new List<ISelectableItem> { canvasItem }
            //        };

            //        Parent.AddItem(canvasItem);
            //        break;
            //}
        }

        protected override void LoadFromPrimitive()
        {
            //var t = (TextSingleLineBoard)primitive;

            //FontName = t.FontName;
            //FontSize = t.FontSize;
            //Text = t.Value;
            //X = t.x;
            //Y = t.y;
            //Rot = t.rot;
            //LayerId = t.layerId;
        }

        protected override void SaveToPrimitive()
        {
            //var t = new TextSingleLineBoard();
            //primitive = t;

            //t.Value = Text;
            //t.x = X;
            //t.y = Y;
            //t.rot = Rot;
            //t.FontName = FontName;
            //t.FontSize = FontSize;
            //t.layerId = (Layer?.LayerId).GetValueOrDefault();
        }

        public override Adorner CreateAdorner(FrameworkElement element)
        {
            return null;
        }

        public override void MirrorX()
        {
            ScaleX *= -1;
        }

        public override void MirrorY()
        {
            ScaleY *= -1;
        }

        public override void TransformBy(Matrix matrix)
        {
            var p = new Point(x, y);
            p = matrix.Transform(p);
            X = p.X;
            Y = p.Y;

            var rotAngle = GetRotationAngleFromMatrix(matrix);
            Rot = RotateSafe(Rot, rotAngle);

        }

        public override void Rotate()
        {
            Rot = RotateSafe(Rot);
        }

        public override void LoadLayers()
        {
            base.LoadLayers();

            EnsureLayerForItems();
        }

        void EnsureLayerForItems()
        {
            foreach (var letter in LetterItems)
            {
                foreach (SingleLayerBoardCanvasItem li in letter.Items)
                {
                    li.LayerDocument = LayerDocument;
                    li.AssignLayer(Layer);
                }

            }
        }

        public override string ToString()
        {
            //trim the text to show for a single line
            var maxLen = 10;
            var t = Text.Replace(Environment.NewLine, " ");
            if (t.Length > maxLen)
                t = Text.Substring(0, maxLen);

            return $"Designator ({t})";
        }
    }
    */
}
