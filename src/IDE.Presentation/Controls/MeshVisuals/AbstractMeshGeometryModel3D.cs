using System.Windows;
using System.Windows.Media;
using HelixToolkit.Wpf.SharpDX;
using HelixToolkit.Wpf.SharpDX.Model;

namespace IDE.Controls
{
    public abstract class AbstractMeshGeometryModel3D : MeshGeometryModel3D
    {
        public AbstractMeshGeometryModel3D()
        {
            var m = PhongMaterials.White;
            m.SpecularShininess = 30f;
            Material = m;
        }

        public static readonly DependencyProperty ColorProperty =
          DependencyProperty.Register("Color", typeof(Color), typeof(AbstractMeshGeometryModel3D), new PropertyMetadata(Colors.White,
              (d, e) =>
              {
                  var c = (Color)e.NewValue;
                  ((d as AbstractMeshGeometryModel3D).Material.Core as PhongMaterialCore).DiffuseColor = new SharpDX.Color4(c.ScR, c.ScG, c.ScB, c.ScA);
              }));

        public Color Color
        {
            get { return (Color)this.GetValue(ColorProperty); }
            set { this.SetValue(ColorProperty, value); }
        }

        protected static void GeometryChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((AbstractMeshGeometryModel3D)d).OnGeometryChanged();
        }

        protected virtual void OnGeometryChanged()
        {
            Geometry = this.Visible ? this.Tessellate() : null;
        }

        protected abstract Geometry3D Tessellate();
    }
}
