using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;
using STPLoader;
using STPLoader.Implementation.Model.Entity;

namespace STPConverter.Implementation.Entity
{
    class SurfaceConvertable : IConvertable
    {
        private readonly Surface _surface;
        private readonly IStpModel _model;

        public SurfaceConvertable(Surface surface, IStpModel model)
        {
            _surface = surface;
            _model = model;
            Init();
        }

        private void Init()
        {
            var child = Convertable(_surface, _model);
            Points = child.Points;
            Indices = child.Indices;
        }

        private static IConvertable Convertable(Surface surface, IStpModel model)
        {
            if (surface.GetType() == typeof (CylindricalSurface))
            {
                return new CylindricalSurfaceConvertable(surface, model);
            }
            else if (surface.GetType() == typeof(ConicalSurface))
            {
                return new ConicalSurfaceConvertable(surface, model);
            }
            else if (surface.GetType() == typeof(Plane))
            {
                return new PlaneConvertable(surface, model);
            }
            else if (surface.GetType() == typeof(ToroidalSurface))
            {
                return new ToroidalSurfaceConvertable(surface, model);
            }
            throw new Exception("No convertable found!");
        }

        public IList<Vector3D> Points { get; private set; }
        public IList<int> Indices { get; private set; }
    }

    internal class ToroidalSurfaceConvertable : IConvertable
    {
        private readonly Surface _surface;
        private readonly IStpModel _model;

        public ToroidalSurfaceConvertable(Surface surface, IStpModel model)
        {
            _surface = surface;
            _model = model;
            Init();
        }

        private void Init()
        {
            Points = new List<Vector3D>();
            Indices = new List<int>();
            
        }

        public IList<Vector3D> Points { get; private set; }
        public IList<int> Indices { get; private set; }
    }
    
    internal class ConicalSurfaceConvertable : IConvertable
    {
        private readonly Surface _surface;
        private readonly IStpModel _model;

        public ConicalSurfaceConvertable(Surface surface, IStpModel model)
        {
            _surface = surface;
            _model = model;
            Init();
        }

        private void Init()
        {
            Points = new List<Vector3D>();
            Indices = new List<int>();
        }

        public IList<Vector3D> Points { get; private set; }
        public IList<int> Indices { get; private set; }
    }
}
