using System;
using System.Collections.Generic;
using System.Linq;
using IDE.Core.Storage;
using System.ComponentModel;
using IDE.Core.Interfaces;
using IDE.Core.Common;
using IDE.Core.Types.Media;
using System.ComponentModel.DataAnnotations;
using IDE.Core.Types.Attributes;
using System.Collections.ObjectModel;
using IDE.Core.Presentation.ObjectFinding;

namespace IDE.Core.Designers
{
    //it displays in a canvas all the elements forming the symbol. Cannot move/select elements.
    public class SchematicSymbolCanvasItem : BaseCanvasItem, ISymbolCanvasItem, IContainerSelectableItem
    {

        public SchematicSymbolCanvasItem()
        {
        }

        [Browsable(false)]
        /*required*/
        public ProjectInfo _Project { get; set; }

        [DisplayName("Name")]
        [Display(Order = 1)]
        [MarksDirty]
        public string PartName
        {
            get
            {
                if (part != null)
                    return part.Name;
                return "not set";
            }
            set
            {
                if (part != null)
                    part.Name = value;
                OnPropertyChanged(nameof(PartName));
                OnPropertyChanged(nameof(SymbolName));
            }
        }

        bool showName = true;

        [DisplayName("Show Name")]
        [Display(Order = 2)]
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
        [MarksDirty]
        public IPosition CommentPosition { get; set; } = new PositionData { Y = 3 };

        string comment;

        [Display(Order = 3)]
        [MarksDirty]
        public string Comment
        {
            get { return comment; }
            set
            {
                comment = value;
                OnPropertyChanged(nameof(Comment));
            }
        }

        bool showComment = false;

        [DisplayName("Show Comment")]
        [Display(Order = 4)]
        [MarksDirty]
        public bool ShowComment
        {
            get { return showComment; }
            set
            {
                showComment = value;
                OnPropertyChanged(nameof(ShowComment));
            }
        }

        double x;

        /// <summary>
        /// Location X of rectangle in current coordinate system
        /// </summary>
        [Editor(EditorNames.PositionXUnitsEditor, EditorNames.PositionXUnitsEditor)]
        [Display(Order = 5)]
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
        [Display(Order = 6)]
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

        [Display(Order = 7)]
        [MarksDirty]
        public double Rot
        {
            get { return rot; }
            set
            {
                if (rot == value)
                    return;
                rot = value % 360;
                OnPropertyChanged(nameof(Rot));
            }
        }

        double displayWidth;
        [Browsable(false)]
        public double DisplayWidth
        {
            get
            {
                return displayWidth;
            }
            set
            {
                displayWidth = value;
                OnPropertyChanged(nameof(DisplayWidth));
            }
        }

        double displayHeight;
        [Browsable(false)]
        public double DisplayHeight
        {
            get
            {
                return displayHeight;
            }
            set
            {
                displayHeight = value;
                OnPropertyChanged(nameof(DisplayHeight));
            }
        }

        [Browsable(false)]
        public Instance SymbolPrimitive { get; private set; }

        IList<ISelectableItem> items = new ObservableCollection<ISelectableItem>();

        //it will need some items that are not selectable
        [Browsable(false)]
        public IList<ISelectableItem> Items
        {
            get { return items; }
            set
            {
                items = new ObservableCollection<ISelectableItem>(value);

                OnPropertyChanged(nameof(Items));
            }
        }

        [Browsable(false)]
        [MarksDirty]
        public IPosition SymbolNamePosition { get; set; } = new PositionData();


        /// <summary>
        /// Part name and gate number (R1, Q1, U1-A)
        /// </summary>
        [Browsable(false)]
        public string SymbolName
        {
            get
            {
                var symbolName = part.Name;
                if (Gate != null && componentDocument != null && componentDocument.Gates.Count > 1)
                {
                    char c = 'A';
                    c = (char)((byte)c + (byte)(Gate.Id - 1));
                    symbolName += "-" + c;//Gate.name;
                }
                return symbolName;
            }
            set
            {
                if (PartName != value)
                    PartName = value;
            }
        }

        // string partName;

        bool isPartNameEditing;

        [Browsable(false)]
        public bool IsPartNameEditing
        {
            get { return isPartNameEditing; }
            set
            {
                isPartNameEditing = value;
                IsEditing = value;
                OnPropertyChanged(nameof(IsPartNameEditing));
            }
        }

        bool isCommentEditing;

        [Browsable(false)]
        public bool IsCommentEditing
        {
            get { return isCommentEditing; }
            set
            {
                isCommentEditing = value;
                IsEditing = value;
                OnPropertyChanged(nameof(IsCommentEditing));
            }
        }

        //[Browsable(false)]
        //public ISolutionProjectNodeModel ProjectModel { get; set; }

        [Browsable(false)]
        public List<PinCanvasItem> Pins { get; private set; } = new List<PinCanvasItem>();

        Part part;
        /// <summary>
        /// the part this gate belongs to
        /// </summary>
        [Browsable(false)]
        public Part Part
        {
            get { return part; }
            set
            {
                if (part != null)
                    part.PropertyChanged -= Part_PropertyChanged;

                part = value;

                if (part != null)
                    part.PropertyChanged += Part_PropertyChanged;

                LoadComponent();

                OnPropertyChanged(nameof(Part));
            }
        }

        const string defaultPartPrefix = "P";
        [Browsable(false)]
        public string PartPrefix
        {
            get
            {
                var p = componentDocument.Prefix;
                if (string.IsNullOrWhiteSpace(p))
                    return defaultPartPrefix;

                return p;
            }
        }

        void Part_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(part.Name))
            {
                OnPropertyChanged(nameof(PartName));
                OnPropertyChanged(nameof(SymbolName));
            }
        }

        Gate Gate { get; set; }

        ComponentDocument componentDocument;
        Symbol symbol;

        [Browsable(false)]
        public ComponentDocument ComponentDocument { get { return componentDocument; } }

        [DisplayName("Component name")]
        [Display(Order = 10000)]
        public string ComponentName => componentDocument?.Name;

        [DisplayName("Component library")]
        [Display(Order = 10001)]
        public string ComponentLibraryName => part?.ComponentLibrary;

        [DisplayName("Symbol name")]
        [Display(Order = 10002)]
        public string SymbolNameInfo => Gate?.symbolName;

        [DisplayName("Symbol library")]
        [Display(Order = 10003)]
        public string SymbolLibraryName => Gate?.LibraryName;

        [DisplayName("Footprint name")]
        [Display(Order = 10004)]
        public string FootprintNameInfo => componentDocument?.Footprint?.footprintName;

        [DisplayName("Footprint library")]
        [Display(Order = 10005)]
        public string FootprintLibraryName => componentDocument?.Footprint?.LibraryName;

        void LoadComponent()
        {
            try
            {
                var lastRead = componentDocument?.LastAccessed;
                var objectFinder = ServiceProvider.Resolve<IObjectFinder>();
                var comp = objectFinder.FindObject<ComponentDocument>(_Project, part.ComponentLibrary, part.ComponentId, lastRead);

                if (comp != null)
                {
                    componentDocument = comp;
                    componentDocument.LastAccessed = DateTime.Now;
                }

                if (componentDocument == null)
                    throw new Exception("Component not found");

            }
            catch (Exception ex)
            {
                ServiceProvider.GetToolWindow<IOutputToolWindow>().AppendLine(ex.Message);
            }

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

            ScaleX *= GetScaleXFromMatrix(ref matrix);
            ScaleY *= GetScaleYFromMatrix(ref matrix);

            var rotAngle = (int)GetRotationSafe(GetRotationAngleFromMatrix(matrix));
            if (rotAngle > 0 && (ScaleX < 0 || ScaleY < 0))
            {
                rotAngle = rotAngle % 180;
            }
            Rot = RotateSafe(Rot, rotAngle);
        }

        public override XRect GetBoundingRectangle()
        {
            return new XRect(x - 0.5 * displayWidth, y - 0.5 * displayHeight, displayWidth, displayHeight);
        }

        public string GetNextPartName(IEnumerable<string> namedParts, string prefix = "P")
        {
            var currentPartName = prefix + "1";
            var partIndex = 1;
            while (namedParts.Any(p => p == currentPartName))
            {
                partIndex++;
                currentPartName = prefix + partIndex;
            }

            return currentPartName;
        }

        public override bool IsMirrored()
        {
            return ScaleX == -1 || ScaleY == -1;
        }

        protected override void LoadFromPrimitiveInternal(IPrimitive primitive)
        {
            var t = (Instance)primitive;

            SymbolPrimitive = t;

            X = t.x;
            Y = t.y;
            ScaleX = t.ScaleX;
            ScaleY = t.ScaleY;
            Rot = t.Rot;

            SymbolNamePosition.X = t.PartNameX;
            SymbolNamePosition.Y = t.PartNameY;
            SymbolNamePosition.Rotation = t.PartNameRot;
            ShowName = t.ShowName;

            CommentPosition.X = t.CommentX;
            CommentPosition.Y = t.CommentY;
            CommentPosition.Rotation = t.CommentRot;
            Comment = t.Comment;
            ShowComment = t.ShowComment;

            //load gate symbol
            var gate = LoadGate(t.GateId);

            //if the gate is different, invalidate the symbol
            if (Gate != null && gate != null && Gate.Id != gate.Id)
                symbol = null;

            Gate = gate;
            var foundSymbol = FindSymbol(gate);

            LoadSymbol(foundSymbol);

        }

        private Gate LoadGate(long gateId)
        {
            if (componentDocument != null)
            {
                var gate = componentDocument.Gates.FirstOrDefault(g => g.Id == gateId);
                //if gate's library is local then it is in the same library as the component
                var gateIsLocal = string.IsNullOrEmpty(gate.LibraryName) || gate.LibraryName == "local";
                if (gateIsLocal)
                {
                    gate.LibraryName = componentDocument.Library;
                }

                return gate;
            }

            return null;
        }

        public void LoadSymbol(Symbol foundSymbol)
        {
            if (foundSymbol == null)
                return;

            var oldPins = Pins.ToList();
            var canvasItems = new List<ISelectableItem>();

            foundSymbol.LastAccessed = DateTime.Now;

            symbol = foundSymbol;

            var symbolPrimitives = symbol.GetDesignerPrimitiveItems();

            foreach (var s in symbolPrimitives)
            {
                //keep the old signal
                if (s is PinCanvasItem pin)
                {
                    var oldPin = oldPins.FirstOrDefault(p => p.Number == pin.Number);
                    if (oldPin != null)
                    {
                        pin.Net = oldPin.Net;
                    }
                }

                s.CanEdit = false;
                s.ParentObject = this;
                canvasItems.Add(s);
            }

            //load pins
            var pins = new List<PinCanvasItem>();
            pins.AddRange(symbolPrimitives.OfType<PinCanvasItem>());
            Pins = pins;

            Items = canvasItems;

            OffsetPrimitives();
        }
        private Symbol FindSymbol(Gate gate)
        {
            Symbol foundSymbol = null;
            if (gate != null)
            {
                DateTime? lastModified = null;

                if (symbol != null)
                {
                    if (gate.symbolId == symbol.Id && gate.LibraryName == symbol.Library)
                    {
                        lastModified = symbol.LastAccessed;
                    }
                }
                var objectFinder = ServiceProvider.Resolve<IObjectFinder>();
                foundSymbol = objectFinder.FindObject<Symbol>(_Project, gate.LibraryName, gate.symbolId, lastModified);
            }

            return foundSymbol;
        }
        private void OffsetPrimitives()
        {
            //translate all primitives to 0,0; update ItemWidth and ItemHeight
            var rect = XRect.Empty;

            var xOffset = double.MaxValue;
            var yOffset = double.MaxValue;
            var boundItems = Pins.Count > 0 ? Pins.Cast<BaseCanvasItem>() : Items.Cast<BaseCanvasItem>();
            foreach (BaseCanvasItem item in boundItems)
            {
                var itemRect = item.GetBoundingRectangle();

                if (xOffset > itemRect.X)
                    xOffset = itemRect.X;
                if (yOffset > itemRect.Y)
                    yOffset = itemRect.Y;

                rect.Union(itemRect);
            }
            if (rect != XRect.Empty)
            {
                foreach (BaseCanvasItem item in Items)
                    item.Translate(-xOffset - rect.Width * 0.5, -yOffset - rect.Height * 0.5);

                DisplayWidth = rect.Width;
                DisplayHeight = rect.Height;
            }
        }

        public override IPrimitive SaveToPrimitive()
        {
            var t = new Instance();

            var sp = SymbolPrimitive;
            if (sp != null)
            {
                t.Id = sp.Id;
                t.PartId = sp.PartId;
                t.GateId = sp.GateId;
            }

            t.x = X;
            t.y = Y;
            t.ScaleX = ScaleX;
            t.ScaleY = ScaleY;
            t.Rot = Rot;

            t.PartNameX = SymbolNamePosition.X;
            t.PartNameY = SymbolNamePosition.Y;
            t.PartNameRot = SymbolNamePosition.Rotation;
            t.ShowName = ShowName;

            t.CommentX = CommentPosition.X;
            t.CommentY = CommentPosition.Y;
            t.CommentRot = CommentPosition.Rotation;
            t.Comment = Comment;
            t.ShowComment = ShowComment;

            return t;
        }

        protected override XTransform GetLocalTranslationTransform()
        {
            return new XTranslateTransform(X, Y);
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
            Rot = RotateSafe(Rot);
        }

        public override string ToString()
        {
            return PartName;
        }
    }

    public class PositionData : BaseViewModel, IPosition
    {
        double x;
        public double X
        {
            get { return x; }
            set
            {
                if (x == value)
                    return;
                x = value;
                OnPropertyChanged(nameof(X));
            }
        }

        double y;
        public double Y
        {
            get { return y; }
            set
            {
                if (y == value)
                    return;
                y = value;
                OnPropertyChanged(nameof(Y));
            }
        }

        double rotation;
        public double Rotation
        {
            get { return rotation; }
            set
            {
                if (rotation == value)
                    return;
                rotation = value;
                OnPropertyChanged(nameof(Rotation));
            }
        }

        public override string ToString()
        {
            return $"{X}, {Y}, {Rotation}";
        }
    }

}
