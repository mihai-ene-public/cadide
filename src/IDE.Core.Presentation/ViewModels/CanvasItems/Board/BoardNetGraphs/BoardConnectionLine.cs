using IDE.Core.Types.Media;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace IDE.Core.Designers
{
    public class BoardConnectionLine : BaseViewModel
    {
        double x1;

        /// <summary>
        /// X coord in top-left coordinate system
        /// </summary>
        [Display(Order = 4)]
        [Description("X coordonate of the first point defining the line")]
        [Editor(EditorNames.PositionXUnitsEditor, EditorNames.PositionXUnitsEditor)]
        public double X1
        {
            get { return x1; }
            set
            {
                x1 = value;
                OnPropertyChanged(nameof(X1));
            }
        }

        double y1;

        [Description("Y coordonate of the first point defining the line")]
        [Display(Order = 5)]
        [Editor(EditorNames.PositionYUnitsEditor, EditorNames.PositionYUnitsEditor)]
        public double Y1
        {
            get { return y1; }
            set
            {
                y1 = value;
                OnPropertyChanged(nameof(Y1));
            }
        }


        double x2;

        [Description("X coordonate of the second point defining the line")]
        [Display(Order = 6)]
        [Editor(EditorNames.PositionXUnitsEditor, EditorNames.PositionXUnitsEditor)]
        public double X2
        {
            get { return x2; }
            set
            {
                x2 = value;
                OnPropertyChanged(nameof(X2));
            }
        }

        double y2;

        [Description("Y coordonate of the second point defining the line")]
        [Display(Order = 7)]
        [Editor(EditorNames.PositionYUnitsEditor, EditorNames.PositionYUnitsEditor)]
        public double Y2
        {
            get { return y2; }
            set
            {
                y2 = value;
                OnPropertyChanged(nameof(Y2));
            }
        }

        double width;

        /// <summary>
        /// Width of the wire in mm
        /// </summary>
        [Description("Width (thickness) of the line")]
        [Editor(EditorNames.SizeMilimetersUnitsEditor, EditorNames.SizeMilimetersUnitsEditor)]
        [Display(Order = 1)]
        public double Width
        {
            get
            {
                return width;
            }
            set
            {
                width = value;
                OnPropertyChanged(nameof(Width));
            }
        }

        XColor lineColor;
        [Display(Order = 2)]
        public XColor LineColor
        {
            get { return lineColor; }

            set
            {
                lineColor = value;
                OnPropertyChanged(nameof(LineColor));
            }
        }

        public BoardNetDesignerItem Net { get; set; }
    }
}
