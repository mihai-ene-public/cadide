using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;
using HelixToolkit.Wpf.SharpDX;
using HelixToolkit.Wpf.SharpDX.Model;
using HelixToolkit.Wpf.SharpDX.Model.Scene;
using SharpDX;

namespace IDE.Controls
{
    public class ViewBoxModel3DX : ScreenSpacedElement3D
    {
        public ViewBoxModel3DX()
        {
        }

        private static ViewBoxNodeX GetInternalSceneNode(DependencyObject d) => ((d as Element3DCore).SceneNode as ViewBoxNodeX);


        public static readonly DependencyProperty UpDirectionProperty = DependencyProperty.Register("UpDirection", typeof(Vector3), typeof(ViewBoxModel3DX),
            new PropertyMetadata(new Vector3(0, 0, -1),
            (d, e) =>
            {
                GetInternalSceneNode(d).UpDirection = ((Vector3)e.NewValue);
            }));


        public Vector3 UpDirection
        {
            set
            {
                SetValue(UpDirectionProperty, value);
            }
            get
            {
                return (Vector3)GetValue(UpDirectionProperty);
            }
        }


        public static readonly DependencyProperty ViewBoxTextureProperty = DependencyProperty.Register("ViewBoxTexture", typeof(Stream), typeof(ViewBoxModel3DX),
            new PropertyMetadata(null, (d, e) =>
            {
                GetInternalSceneNode(d).ViewBoxTexture = (Stream)e.NewValue;
            }));

        public Stream ViewBoxTexture
        {
            set
            {
                SetValue(ViewBoxTextureProperty, value);
            }
            get
            {
                return (Stream)GetValue(ViewBoxTextureProperty);
            }
        }

        protected override SceneNode OnCreateSceneNode()
        {
            return new ViewBoxNodeX();
        }

        protected override void AssignDefaultValuesToSceneNode(SceneNode node)
        {
            (node as ViewBoxNodeX).UpDirection = this.UpDirection;
            base.AssignDefaultValuesToSceneNode(node);
        }
    }

}
