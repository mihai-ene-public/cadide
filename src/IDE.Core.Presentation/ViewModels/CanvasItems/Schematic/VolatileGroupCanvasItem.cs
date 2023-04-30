using IDE.Core.Interfaces;
using IDE.Core.Types.Media;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace IDE.Core.Designers
{
    /// <summary>
    /// a temporary group created for placement. Used for import similar items or after paste
    /// <para>This is not saved in document</para>
    /// </summary>
    public class VolatileGroupCanvasItem : BaseCanvasItem
    {


        double x;

        /// <summary>
        /// Location X of rectangle in current coordinate system
        /// </summary>
        [Browsable(false)]
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
        [Browsable(false)]
        public double Y
        {
            get { return y; }
            set
            {
                y = value;
                OnPropertyChanged(nameof(Y));
            }
        }

        //double width;

        double rot;
        //[Display(Order = 11)]
        [Browsable(false)]
        public double Rot
        {
            get { return rot; }
            set
            {
                rot = value;

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



        List<ISelectableItem> items = new List<ISelectableItem>();

        //it will need some items that are not selectable
        [Browsable(false)]
        public List<ISelectableItem> Items
        {
            get { return items; }
            set
            {
                items = value;
                OnPropertyChanged(nameof(Items));
            }
        }

        public override void MirrorX()
        {
            ScaleX *= -1;
        }

        public override void MirrorY()
        {
            ScaleY *= -1;
        }

        public override void Rotate(double angle = 90)
        {
            var r = Rot;
            r += angle;
            r = ((int)r % 360);

            Rot = r;
        }

        public override void Translate(double dx, double dy)
        {
            X += dx;
            Y += dy;
        }

        public override XRect GetBoundingRectangle()
        {
            return new XRect(X, Y, DisplayWidth, DisplayHeight);
        }

    }
}
