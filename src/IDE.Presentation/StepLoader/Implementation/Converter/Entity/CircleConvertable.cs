using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;
using STPLoader;
using STPLoader.Implementation.Model.Entity;

namespace STPConverter.Implementation.Entity
{
    public class CircleConvertable : IConvertable
    {
        private readonly Circle _circle;
        private readonly IStpModel _model;
        private const int Sides = 64;

        public CircleConvertable(Circle circle, IStpModel model)
        {
            _circle = circle;
            _model = model;
            Init();
        }

        private void Init()
        {
            var placement = _model.Get<Axis2Placement3D>(_circle.PointId);
            var cartesianPoint = _model.Get<CartesianPoint>(placement.PointIds[0]);
            var direction = _model.Get<DirectionPoint>(placement.PointIds[1]);

            Points = new List<Vector3D> { cartesianPoint.Vector };
            var x = new Vector3D(1, 0, 0);
            var y = new Vector3D(0, 1, 0);
            var ax = Math.Acos(Vector3D.DotProduct(direction.Vector, x) / (direction.Vector.Length * x.Length));
            var ay = Math.Acos(Vector3D.DotProduct(direction.Vector, y) / (direction.Vector.Length * y.Length));

            // var rotationMatrix = Matrix3D.Identity;//Matrix3x3.CreateFromYawPitchRoll((float)(Math.PI / 2 - ax), (float)(Math.PI / 2 - ay), 0);
            var tg = new Transform3DGroup();
            tg.Children.Add(new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 0, 1), (Math.PI / 2 - ax))));
            tg.Children.Add(new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(1, 0, 0), (Math.PI / 2 - ay))));
            var rotationMatrix = tg.Value;

            for (int i = 0; i < Sides; i++)
            {
                var angle = 360 - (i * 360d / Sides);
                angle = angle * 2 * Math.PI / 360d;
                // calculate point on unit circle and multiply by radius
                var vector = new Vector3D((float)Math.Cos(angle), (float)Math.Sin(angle), 0) * (float)_circle.Radius;
                // change normal vector to direction vector
                //we might put this back
                // vector = rotationMatrix * vector;
                vector = rotationMatrix.Transform(vector);
                // add midpoint position vector
                vector = vector + cartesianPoint.Vector;
                Points.Add(vector);
            }

            Indices = Enumerable.Range(1, Sides).Select(i => new int[] { 0, i, (i % Sides) + 1 }).SelectMany(d => d).ToList();
        }

        public IList<Vector3D> Points { get; private set; }
        public IList<int> Indices { get; private set; }
    }
}
