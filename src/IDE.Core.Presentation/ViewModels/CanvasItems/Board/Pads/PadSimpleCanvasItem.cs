using IDE.Core.Types.Media;
using System.ComponentModel;

namespace IDE.Core.Designers
{
    public class PadSimpleCanvasItem : PadBaseCanvasItem
    {
        public PadSimpleCanvasItem()
        {
            Width = 1;
            Height = 2;

            PropertyChanged += PadSimpleCanvasItem_PropertyChanged;
        }

        private void PadSimpleCanvasItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            PropertyChangedHandle(e.PropertyName);
        }


        protected virtual void PropertyChangedHandle(string propertyName)
        {

        }

        public override XRect GetBoundingRectangle()
        {
            return new XRect(X - 0.5 * Width, Y - 0.5 * Height, Width, Height);
        }
    }

}
