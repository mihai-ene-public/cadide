using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Media.Media3D;
namespace STPConverter
{
    public class MeshModel
    {
        public string Meta;
        //private readonly IList<Vector3D> _points;
        //private readonly IList<int> _triangles;

        //IList<IList<Vector3D>> _points;
        //IList<IList<int>> _triangles;

        public MeshModel()//IList<Vector3D> points, IList<int> triangles)
        {
            //_points = points;
            //_triangles = triangles;
        }

        //public void AddSurface(IList<Vector3D> points, IList<int> triangles)
        //{
        //    _points.Add(points);
        //    _triangles.Add(triangles);
        //}

        //public IList<Vector3D> Points
        //{
        //    get { return _points; }
        //}

        //public IList<int> Triangles
        //{
        //    get { return _triangles; }
        //}

        //public override string ToString()
        //{
        //    return String.Format("<MeshModel({0}, {1})>",
        //        String.Join("|", Points.Select(x => x.ToString()).ToArray()),
        //        String.Join("|", Triangles.Select(x => x.ToString(CultureInfo.InvariantCulture)).ToArray()));
        //}
    }
}
