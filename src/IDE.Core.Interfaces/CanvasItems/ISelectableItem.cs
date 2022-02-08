using IDE.Core.Types.Media;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;


namespace IDE.Core.Interfaces
{
    public interface ISelectableItem : ICanvasItem, ICloneable, INotifyPropertyChanged
    {
        bool IsSelected { get; set; }
        bool IsEditing { get; set; }

        bool IsPlaced { get; set; }

        bool IsLocked { get; set; }

        bool CanEdit { get; set; }

        int ZIndex { get; set; }

        ISelectableItem ParentObject { get; set; }

        // IPrimitive Primitive { get; set; }

        void ToggleSelect();

        void LoadFromPrimitive(IPrimitive primitive);

        IPrimitive SaveToPrimitive();

        XRect GetBoundingRectangle();

        void Translate(double dx, double dy);

        void TransformBy(XMatrix matrix);

        XTransform GetTransform();

        void MirrorX();


        void MirrorY();


        void Rotate();

        /// <summary>
        /// implements cleanup needed to be done when removed from canvas
        /// </summary>
        void RemoveFromCanvas();

    }

    //public interface IHighlightable
    //{
    //    bool IsHighLighted { get; set; }
    //}

    public interface IBaseCanvasItem : ISelectableItem
    {

    }
}
