using System;
using System.Windows.Data;
using System.Globalization;
using IDE.Core.Interfaces;
using IDE.Presentation.Extensions;
using System.Linq;
using HelixToolkit.Wpf.SharpDX;

namespace IDE.Core.Converters
{
    public class MeshModelToMeshGeometry3DConverter : IValueConverter
    {
        static MeshModelToMeshGeometry3DConverter instance;
        public static MeshModelToMeshGeometry3DConverter Instance
        {
            get
            {
                if (instance == null)
                    instance = new MeshModelToMeshGeometry3DConverter();

                return instance;
            }
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var model = (IMeshModel)value;
            var mesh = model.Meshes.FirstOrDefault();
            if (mesh == null)
                return null;

            var mg = mesh.ToSharpDXMeshGeometry3D();
            return mg;

            //var meshBuilder = new MeshBuilder();
            //meshBuilder.AddBox(new SharpDX.Vector3(0, 0, 0), 1, 1, 1);
            //return meshBuilder.ToMesh();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
