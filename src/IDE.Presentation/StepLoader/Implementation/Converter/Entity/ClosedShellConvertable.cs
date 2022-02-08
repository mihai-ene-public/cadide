using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Media3D;
using STPLoader;
using STPLoader.Implementation.Model.Entity;

namespace STPConverter.Implementation.Entity
{
    /// <summary>
    /// 
    /// </summary>
    public class ClosedShellConvertable : IConvertable
    {
        private readonly ClosedShell _closedShell;
        private readonly IStpModel _model;

        public ClosedShellConvertable(ClosedShell closedShell, IStpModel model)
        {
            _closedShell = closedShell;
            _model = model;
            Init();
        }

        private void Init()
        {
            var faces = _closedShell.PointIds.Select(_model.Get<AdvancedFace>);
            // create convertable for all faces and merge points and indices
            var convertables = faces.Select(face => new AdvancedFaceConvertable(face, _model)).Select(c => Tuple.New(c.Points, c.Indices));

            Points = convertables.Select(c => c.First).SelectMany(p => p).ToList();
            Indices = convertables.Aggregate(Tuple.New(0, new List<int>()), Tuple.AggregateIndices).Second;



            //Convertables = faces.Select(face => new AdvancedFaceConvertable(face, _model)).Cast<IConvertable>().ToList();

        }

        public IList<Vector3D> Points { get; private set; } = new List<Vector3D>();
        public IList<int> Indices { get; private set; } = new List<int>();

        //public IList<IConvertable> Convertables { get;private set; }
    }
}
