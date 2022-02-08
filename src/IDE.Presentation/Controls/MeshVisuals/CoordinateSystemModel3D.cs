using System.Collections.Generic;
using System.Windows;
using HelixToolkit.Wpf.SharpDX;
using HelixToolkit.Wpf.SharpDX.Model;
using HelixToolkit.Wpf.SharpDX.Model.Scene;
using Media = System.Windows.Media;

namespace IDE.Controls
{
    public class CoordinateSystemModel3D : ScreenSpacedElement3D
    {
        /// <summary>
        /// <see cref="AxisXColor"/>
        /// </summary>
        public static readonly DependencyProperty AxisXColorProperty = DependencyProperty.Register("AxisXColor", typeof(Media.Color), typeof(CoordinateSystemModel3D),
            new PropertyMetadata(Media.Colors.Red,
                (d, e) =>
                {
                    GetInternalSceneNode(d).AxisXColor = ((Media.Color)e.NewValue).ToColor4();
                }));
        /// <summary>
        /// <see cref="AxisYColor"/>
        /// </summary>
        public static readonly DependencyProperty AxisYColorProperty = DependencyProperty.Register("AxisYColor", typeof(Media.Color), typeof(CoordinateSystemModel3D),
            new PropertyMetadata(Media.Colors.Green,
                (d, e) =>
                {
                    GetInternalSceneNode(d).AxisYColor = ((Media.Color)e.NewValue).ToColor4();
                }));
        /// <summary>
        /// <see cref="AxisZColor"/>
        /// </summary>
        public static readonly DependencyProperty AxisZColorProperty = DependencyProperty.Register("AxisZColor", typeof(Media.Color), typeof(CoordinateSystemModel3D),
            new PropertyMetadata(Media.Colors.Blue,
                (d, e) =>
                {
                    GetInternalSceneNode(d).AxisZColor = ((Media.Color)e.NewValue).ToColor4();
                }));
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty LabelColorProperty = DependencyProperty.Register("LabelColor", typeof(Media.Color), typeof(CoordinateSystemModel3D),
            new PropertyMetadata(Media.Colors.Gray,
                (d, e) =>
                {
                    GetInternalSceneNode(d).LabelColor = ((Media.Color)e.NewValue).ToColor4();
                }));

        /// <summary>
        /// The coordinate system label x property
        /// </summary>
        public static readonly DependencyProperty CoordinateSystemLabelXProperty = DependencyProperty.Register(
                "CoordinateSystemLabelX", typeof(string), typeof(CoordinateSystemModel3D), new PropertyMetadata("X",
                (d, e) =>
                {
                    GetInternalSceneNode(d).LabelX = e.NewValue as string;
                }));

        /// <summary>
        /// The coordinate system label Y property
        /// </summary>
        public static readonly DependencyProperty CoordinateSystemLabelYProperty = DependencyProperty.Register(
                "CoordinateSystemLabelY", typeof(string), typeof(CoordinateSystemModel3D), new PropertyMetadata("Y",
                (d, e) =>
                {
                    GetInternalSceneNode(d).LabelY = e.NewValue as string;
                }));

        /// <summary>
        /// The coordinate system label Z property
        /// </summary>
        public static readonly DependencyProperty CoordinateSystemLabelZProperty = DependencyProperty.Register(
                "CoordinateSystemLabelZ", typeof(string), typeof(CoordinateSystemModel3D), new PropertyMetadata("Z",
                (d, e) =>
                {
                    GetInternalSceneNode(d).LabelZ = e.NewValue as string;
                }));

        private static CoordinateSystemXNode GetInternalSceneNode(DependencyObject d) => ((d as Element3DCore).SceneNode as CoordinateSystemXNode);

        /// <summary>
        /// Axis X Color
        /// </summary>
        public Media.Color AxisXColor
        {
            set
            {
                SetValue(AxisXColorProperty, value);
            }
            get
            {
                return (Media.Color)GetValue(AxisXColorProperty);
            }
        }
        /// <summary>
        /// Axis Y Color
        /// </summary>
        public Media.Color AxisYColor
        {
            set
            {
                SetValue(AxisYColorProperty, value);
            }
            get
            {
                return (Media.Color)GetValue(AxisYColorProperty);
            }
        }
        /// <summary>
        /// Axis Z Color
        /// </summary>
        public Media.Color AxisZColor
        {
            set
            {
                SetValue(AxisZColorProperty, value);
            }
            get
            {
                return (Media.Color)GetValue(AxisZColorProperty);
            }
        }
        /// <summary>
        /// Label Color
        /// </summary>
        public Media.Color LabelColor
        {
            set
            {
                SetValue(LabelColorProperty, value);
            }
            get
            {
                return (Media.Color)GetValue(LabelColorProperty);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public string CoordinateSystemLabelX
        {
            set
            {
                SetValue(CoordinateSystemLabelXProperty, value);
            }
            get
            {
                return (string)GetValue(CoordinateSystemLabelXProperty);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string CoordinateSystemLabelY
        {
            set
            {
                SetValue(CoordinateSystemLabelYProperty, value);
            }
            get
            {
                return (string)GetValue(CoordinateSystemLabelYProperty);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string CoordinateSystemLabelZ
        {
            set
            {
                SetValue(CoordinateSystemLabelZProperty, value);
            }
            get
            {
                return (string)GetValue(CoordinateSystemLabelZProperty);
            }
        }


        protected override SceneNode OnCreateSceneNode()
        {
            return new CoordinateSystemXNode();
        }

        public override bool HitTest(HitTestContext context, ref List<HitTestResult> hits)
        {
            return false;
        }
    }
}
