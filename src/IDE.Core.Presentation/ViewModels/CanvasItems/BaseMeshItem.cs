using IDE.Core.Common;
using IDE.Core.Interfaces;
using IDE.Core.Storage;
using IDE.Core.Types.Media;
using IDE.Core.Types.Media3D;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace IDE.Core.Designers
{
    public abstract class BaseMeshItem : BaseViewModel
                                       , ISelectableItem
    {

        public BaseMeshItem()
        {
            IsPlaced = false;
            canEdit = true;


            PropertyChanged += DesignerItemBaseViewModel_PropertyChanged;
        }

        /// <summary>
        /// the logical parent object this item belongs to
        /// <para>this is SchematicSymbolDesignerItem for Primitives</para>
        /// </summary>
        [Browsable(false)]
        public ISelectableItem ParentObject { get; set; }

        [Browsable(false)]
        public int Id { get; set; }

        bool isSelected;

        [Browsable(false)]
        public bool IsSelected
        {
            get
            {
                return isSelected;
            }
            set
            {
                if (isSelected == value)
                    return;

                isSelected = value;

                if (ParentObject != null)
                {
                    ParentObject.IsSelected = value;
                    if (ParentObject is GroupMeshItem)
                    {
                        var g = ParentObject as GroupMeshItem;
                        foreach (var s in g.Items)
                            s.IsSelected = value;
                    }
                }

                OnPropertyChanged(nameof(IsSelected));
            }
        }

        bool isEditing = false;

        [Browsable(false)]
        public bool IsEditing
        {
            get { return isEditing; }
            set
            {
                isEditing = value;
                OnPropertyChanged(nameof(IsEditing));
            }
        }

        [Browsable(false)]
        public int ZIndex { get; set; }//it is required by the interface, it shouldn't be used

        XColor fillColor;
        [Display(Order = 1)]
        [Editor(EditorNames.XColorEditor, EditorNames.XColorEditor)]
        public virtual XColor FillColor
        {
            get
            {
                return fillColor;
            }

            set
            {
                fillColor = value;
                OnPropertyChanged(nameof(FillColor));
                OnPropertyChanged(nameof(PresentedFillColor));
                OnPropertyChanged(nameof(IsTransparent));
            }
        }

        [Browsable(false)]
        public XColor PresentedFillColor
        {
            get
            {
                var highlightColor = BlendColor(fillColor, XColors.White, 0.3);
                var c = IsSelected ? highlightColor : fillColor;
                return c;
            }
        }

        [Browsable(false)]
        public bool IsTransparent => PresentedFillColor.A < 255;

        XColor BlendColor(XColor backColor, XColor color, double amount)
        {
            byte r = (byte)((color.R * amount) + backColor.R * (1 - amount));
            byte g = (byte)((color.G * amount) + backColor.G * (1 - amount));
            byte b = (byte)((color.B * amount) + backColor.B * (1 - amount));
            return XColor.FromArgb(color.A, r, g, b);
        }

        int padNumber;

        [Display(Order = 999)]
        public int PadNumber
        {
            get
            {
                return padNumber;
            }
            set
            {
                if (padNumber != value)
                {
                    padNumber = value;
                    OnPropertyChanged(nameof(PadNumber));
                }
            }
        }

        bool isLocked;

        [Display(Order = 1000)]
        [Browsable(false)]
        public bool IsLocked
        {
            get
            {
                return isLocked;
            }
            set
            {
                if (isLocked != value)
                {

                    isLocked = value;
                    OnPropertyChanged(nameof(IsLocked));
                }
            }
        }

        /// <summary>
        /// returns true if this item is placed on canvas 
        /// </summary>
        [Browsable(false)]
        public bool IsPlaced { get; set; }

        bool canEdit;
        [Browsable(false)]
        public bool CanEdit
        {
            get { return canEdit; }
            set
            {
                canEdit = value;
                OnPropertyChanged(nameof(CanEdit));
            }
        }

        public virtual void ToggleSelect()
        {
            IsSelected = !isSelected;
        }
        public abstract void Translate(double dx, double dy, double dz);

        public void LoadFromPrimitive(IPrimitive primitive)
        {
            LoadFromPrimitiveInternal(primitive);
            IsPlaced = true;
        }

        protected virtual void LoadFromPrimitiveInternal(IPrimitive primitive)
        {
        }

        public virtual IPrimitive SaveToPrimitive() { return null; }

        void DesignerItemBaseViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IsSelected))
                OnPropertyChanged(nameof(PresentedFillColor));

            if (e.PropertyName == nameof(IsSelected)
                || e.PropertyName == nameof(PresentedFillColor))
                return;
        }

        public virtual void TransformBy(XMatrix3D matrix)
        {

        }

        protected double RotateSafe(double rot, double amount = 90.0d)
        {
            rot += amount;
            rot = ((int)rot % 360);

            return rot;
        }

        protected double GetRotationAngleFromMatrix(XMatrix3D matrix)
        {
            var rads = -Math.Atan2(matrix.M21, matrix.M11);
            var rotAngle = rads * 180 / Math.PI;
            return rotAngle;
        }

        public abstract void MirrorX();

        public abstract void MirrorY();

        public abstract void Rotate();

        public object Clone()
        {
            var clone = SaveToPrimitive().Clone();
            if (clone is MeshPrimitive)
                return ((MeshPrimitive)clone).CreateDesignerItem();

            return MemberwiseClone();
        }

        //this shouldnt be for a 3D canvas item
        public XRect GetBoundingRectangle()
        {
            return XRect.Empty;
        }

        public XTransform GetTransform()
        {
            throw new NotImplementedException();
        }

        public void Translate(double dx, double dy)
        {

        }

        public void TransformBy(XMatrix matrix)
        {

        }

        public virtual void RemoveFromCanvas()
        {

        }
    }
}
