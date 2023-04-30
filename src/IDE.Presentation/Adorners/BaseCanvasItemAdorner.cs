using IDE.Core.Converters;
using IDE.Core.Designers;
using IDE.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace IDE.Core.Adorners
{
    public abstract class BaseCanvasItemAdorner : Adorner
    {
        public BaseCanvasItemAdorner(UIElement adornedElement)
            : base(adornedElement)
        {
            visualChildren = new VisualCollection(this);

            canvasModel = adornedElement.FindParentDataContext<ICanvasDesignerFileViewModel>();
        }

        protected VisualCollection visualChildren;

        protected ICanvasDesignerFileViewModel canvasModel;

        protected override int VisualChildrenCount { get { return visualChildren.Count; } }
        protected override Visual GetVisualChild(int index) { return visualChildren[index]; }
    }
}
