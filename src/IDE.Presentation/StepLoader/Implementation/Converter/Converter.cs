using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;
using STPConverter.Implementation.Entity;
using STPLoader;
using STPLoader.Implementation.Model.Entity;

namespace STPConverter
{
    public class Converter //: IConverter
    {
        /*
        public static MeshModel Convert(IStpModel model)
        {
            //var vectors = new List<Vector3D>();
            //var indices = new List<int>();

            //GetValue<ClosedShell>(model, indices, vectors);
            //var mesh = new MeshModel(vectors, indices);

           var cList= GetConvertables<ClosedShell>(model);
            var mesh = new MeshModel();

            foreach (var c in cList)
                mesh.AddSurface(c.Points, c.Indices);

            return mesh;
        }
        */

        public static IList<ClosedShellConvertable> GetClosedShells(IStpModel model)
        {
            return GetConvertables<ClosedShell>(model).Cast<ClosedShellConvertable>().ToList();
        }

        /*
        private static void GetValue<T>(IStpModel model)//, List<int> indices, List<Vector3D> vectors)
                                    where T : Entity
        {
            foreach (var element in model.All<T>())
            {
                var offset = vectors.Count;
                var convertable = CreateConvertable(element, model);
                var circleVectors = convertable.Points;
                var circleIndices = convertable.Indices;
                vectors.AddRange(circleVectors);
                indices.AddRange(circleIndices.Select(x => x + offset));
            }
        }
        */

        static IList<IConvertable> GetConvertables<T>(IStpModel model)
                                where T : Entity
        {
            var l = new List<IConvertable>();
            var elements = model.All<T>().ToList();
            foreach (var element in elements)
            {
                var convertable = CreateConvertable(element, model);
                if (convertable != null)
                    l.Add(convertable);
            }

            return l;
        }

        private static IConvertable CreateConvertable<T>(T element, IStpModel model) where T : Entity
        {
            var type = typeof(T);
            if (type == typeof(Circle))
            {
                return new CircleConvertable(element as Circle, model);
            }
            if (type == typeof(Line))
            {
                return new LineConvertable(element as Line, model);
            }
            if (type == typeof(AdvancedFace))
            {
                return new AdvancedFaceConvertable(element as AdvancedFace, model);
            }
            if (type == typeof(ClosedShell))
            {
                return new ClosedShellConvertable(element as ClosedShell, model);
            }
            throw new Exception("Not supported");
        }

    }
}
