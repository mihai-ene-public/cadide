using IDE.Core.Storage;
using System;
using System.ComponentModel;
using IDE.Core.Interfaces;
using IDE.Core.Common;
using IDE.Core.Types.Media;
using System.ComponentModel.DataAnnotations;

namespace IDE.Core.Designers
{

    public abstract class BaseCanvasItem : BaseViewModel
                                         //, ISelectableItem
                                         , IBaseCanvasItem
    {


        public BaseCanvasItem()
        {
            IsPlaced = false;
            canEdit = true;
        }

        private IGeometryHelper geometryHelper;
        protected IGeometryHelper GeometryHelper
        {
            get
            {
                if (geometryHelper == null)
                    geometryHelper = ServiceProvider.Resolve<IGeometryHelper>();

                return geometryHelper;
            }
        }

        ///// <summary>
        ///// the canvas this item is displayed
        ///// </summary>
        //[Browsable(false)]
        //public IDrawingViewModel Parent { get; set; }

        /// <summary>
        /// the logical parent object this item belongs to
        /// <para>this is SchematicSymbolDesignerItem for Primitives</para>
        /// </summary>
        [Browsable(false)]
        public ISelectableItem ParentObject { get; set; }

        //[Browsable(false)]
        //public int Id { get; set; }

        bool isSelected;

        [Browsable(false)]
        public bool IsSelected
        {
            get
            {
                return GetIsSelectedInternal();
            }
            set
            {
                SetIsSelectedInternal(value);
            }
        }

        public virtual void ToggleSelect()
        {
            SetIsSelectedInternal(!IsSelected);
        }

        protected virtual bool GetIsSelectedInternal()
        {
            if (canEdit)
                return isSelected;
            return false;
        }

        protected virtual void SetIsSelectedInternal(bool value)
        {
            if (canEdit)
            {
                isSelected = value;
                OnPropertyChanged(nameof(IsSelected));
            }
        }

        protected int zIndex;
        [Editor(EditorNames.ZIndexEditor, EditorNames.ZIndexEditor)]
        [Display(Order = 100)]
        public virtual int ZIndex
        {
            get
            {
                return GetZIndex();
            }
            set
            {
                if (zIndex != value)
                {

                    zIndex = value;
                    OnPropertyChanged(nameof(ZIndex));
                }
            }
        }

        protected virtual int GetZIndex()
        {
            return zIndex;
        }

        bool isLocked;

        [Display(Order = 1000)]
        // [Browsable(false)]
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

        bool isPlaced = false;
        /// <summary>
        /// returns true if this item is placed on canvas 
        /// </summary>
        [Browsable(false)]
        public bool IsPlaced
        {
            get { return isPlaced; }
            set
            {
                isPlaced = value;
                OnPropertyChanged(nameof(IsPlaced));
            }
        }

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

        double scaleX = 1.0;
        [Browsable(false)]
        public double ScaleX
        {
            get
            {
                return scaleX;
            }
            set
            {
                scaleX = value;
                OnPropertyChanged(nameof(ScaleX));
            }
        }

        double scaleY = 1.0;
        [Browsable(false)]
        public double ScaleY
        {
            get
            {
                return scaleY;
            }
            set
            {
                scaleY = value;
                OnPropertyChanged(nameof(ScaleY));
            }
        }

        public virtual bool IsMirrored()
        {
            return false;
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
        public virtual bool CanClone
        {
            get { return true; }
        }

        //protected IPrimitive primitive;

        //[Browsable(false)]
        //public IPrimitive Primitive
        //{
        //    get
        //    {
        //        SaveToPrimitive();
        //        return primitive;
        //    }
        //    set
        //    {
        //        primitive = value;
        //        LoadFromPrimitive();
        //        IsPlaced = true;
        //    }
        //}

        public abstract XRect GetBoundingRectangle();

        public abstract void Translate(double dx, double dy);

        public virtual void TransformBy(XMatrix matrix)
        {

        }

        protected double RotateSafe(double rot, double amount = 90.0d)
        {
            rot += amount;
            if (rot < 0.0d)
                rot += 360;

            rot = ((int)rot % 360);


            return rot;
        }

        protected double GetRotationSafe(double rot)
        {
            if (rot < 0)
                return 360 + rot;
            return (int)rot % 360;
        }

        protected double GetRotationAngleFromMatrix(XMatrix matrix)
        {
            var rads = -Math.Atan2(matrix.M21, matrix.M11);
            var rotAngle = rads * 180 / Math.PI;
            return rotAngle;
        }

        protected double GetScaleXFromMatrix(ref XMatrix matrix)
        {
            var sX = Math.Sign(matrix.M11) * Math.Sqrt(matrix.M11 * matrix.M11 + matrix.M12 * matrix.M12);

            return Math.Round(sX);
        }

        protected double GetScaleYFromMatrix(ref XMatrix matrix)
        {
            var sY = Math.Sign(matrix.M22) * Math.Sqrt(matrix.M21 * matrix.M21 + matrix.M22 * matrix.M22);

            return Math.Round(sY);
        }

        protected double GetWorldRotation(double rot, XMatrix matrix)
        {
            var rotAngle = GetRotationAngleFromMatrix(matrix);
            var fp = ParentObject as FootprintBoardCanvasItem;
            var myRot = rot;
            if (fp != null)
            {
                if (fp.Placement == FootprintPlacement.Bottom)
                {
                    myRot = 180.0d - myRot;

                    var sign = Math.Sign(rotAngle);
                    rotAngle = sign * (180 - Math.Abs(rotAngle));
                }

                myRot += rotAngle;
            }

            return myRot;
        }

        public virtual XTransform GetTransform()
        {
            var t = new XTransformGroup();

            //local rotation
            t.Children.Add(GetLocalRotationTransform());

            //local mirror
            t.Children.Add(GetLocalMirrorTransform());

            //local translation
            t.Children.Add(GetLocalTranslationTransform());

            //we need to cast to the container
            var parentObject = ParentObject as IContainerSelectableItem;

            if (parentObject != null)
            {
                t.Children.Add(GetGlobalRotation(parentObject));
                t.Children.Add(GetGlobalMirror(parentObject));
                t.Children.Add(GetGlobalTranslation(parentObject));
            }


            return t;
        }

        protected virtual XTransform GetLocalRotationTransform()
        {
            return XTransform.Identity;
        }

        protected virtual XTransform GetLocalMirrorTransform()
        {
            return XTransform.Identity;
        }

        protected virtual XTransform GetLocalTranslationTransform()
        {
            return XTransform.Identity;
        }

        protected virtual XTransform GetGlobalRotation(IContainerSelectableItem parentObject)
        {
            return new XRotateTransform(parentObject.Rot);
        }

        protected virtual XTransform GetGlobalMirror(IContainerSelectableItem parentObject)
        {
            // if (parentObject != null && parentObject.Placement == FootprintPlacement.Bottom)
            if (parentObject != null && parentObject.IsMirrored())
            {
                var scaleX = parentObject.ScaleX;
                if (parentObject.ScaleX > 0 && parentObject.ScaleY > 0)
                    scaleX = -1;

                return new XScaleTransform
                {
                    ScaleX = scaleX,
                    ScaleY = parentObject.ScaleY
                };
            }

            return XTransform.Identity;
        }

        protected virtual XTransform GetGlobalTranslation(IContainerSelectableItem parentObject)
        {
            return new XTranslateTransform(parentObject.X, parentObject.Y);
        }

        public void LoadFromPrimitive(IPrimitive primitive)
        {
            LoadFromPrimitiveInternal(primitive);
            IsPlaced = true;
        }

        protected virtual void LoadFromPrimitiveInternal(IPrimitive primitive)
        {
        }

        public virtual IPrimitive SaveToPrimitive() { return null; }

        public abstract void MirrorX();


        public abstract void MirrorY();


        public abstract void Rotate();

        public virtual void RemoveFromCanvas()
        {

        }

        public virtual object Clone()
        {
            ////we don't allow parts on boards to be cloned
            //if (this is FootprintBoardCanvasItem)
            //    return null;

            var clone = (IPrimitive)SaveToPrimitive().Clone();

            if (this is SchematicSymbolCanvasItem)
            {
                var instance = (Instance)clone;
                var symbol = this as SchematicSymbolCanvasItem;
                var part = symbol.Part;

                //Instance
                //Part
                //ProjectModel

                //return MemberwiseClone();
                var schPart = new SchematicSymbolCanvasItem();
                // schPart.Parent = symbol.Parent;
                schPart.ProjectModel = symbol.ProjectModel;
                var newPart = part.Clone();
                //newPart.Name = symbol.GetNextPartName();
                schPart.Part = newPart;
                instance.PartId = newPart.Id;
                instance.Id = LibraryItem.GetNextId();
                schPart.LoadFromPrimitive(instance);
                schPart.Rot = symbol.Rot;
                schPart.ScaleX = symbol.ScaleX;
                schPart.ScaleY = symbol.ScaleY;
                schPart.IsPlaced = false;
                return schPart;
            }
            if (this is NetSegmentCanvasItem)
            {
                var netSegment = (NetSegmentCanvasItem)/*(NetSegmentItem)*/clone.CreateDesignerItem();
                var net = (this as NetSegmentCanvasItem).Net;
                netSegment.AssignNet(net);
                netSegment.IsPlaced = false;
                return netSegment;
            }
            if (this is BusSegmentCanvasItem)
            {
                var netSegment = (BusSegmentCanvasItem)clone.CreateDesignerItem();
                var net = (this as BusSegmentCanvasItem).Bus;
                netSegment.AssignBus(net);
                netSegment.IsPlaced = false;
                return netSegment;
            }
            if (clone is SchematicPrimitive)
            {
                var p = ((SchematicPrimitive)clone).CreateDesignerItem();
                p.IsPlaced = false;
                return p;
            }
            if (this is BoardCanvasItemViewModel)
            {

                //todo: for signal items, the signal should not be copied; it should be set as if the item is placed (with collision, and guess the signal)
                //traces are lost when not having a signal

                var thisBrdItem = this as BoardCanvasItemViewModel;
                var canvasItem = (BoardCanvasItemViewModel)((LayerPrimitive)clone).CreateDesignerItem();
                canvasItem.LayerDocument = thisBrdItem.LayerDocument;
                canvasItem.ParentObject = thisBrdItem.ParentObject;
                canvasItem.IsPlaced = false;

                if (this is ISignalPrimitiveCanvasItem)
                {
                    var thisSi = this as ISignalPrimitiveCanvasItem;
                    var cloneSig = canvasItem as ISignalPrimitiveCanvasItem;
                    cloneSig.AssignSignal(thisSi.Signal);
                }

                if (thisBrdItem is SingleLayerBoardCanvasItem)
                {
                    var thisSingleLayerItem = thisBrdItem as SingleLayerBoardCanvasItem;
                    var cloneSingleLayerItem = canvasItem as SingleLayerBoardCanvasItem;

                    cloneSingleLayerItem.AssignLayer(thisSingleLayerItem.Layer);
                }
                else
                {
                    canvasItem.LoadLayers();
                }


                return canvasItem;
            }


            //for boards and footprints we need the document layers
            //todo re-think the layer primitives so that we don't need the layer reference
            return MemberwiseClone();

        }
    }
}
