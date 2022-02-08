using System;
using System.Collections.Generic;
using System.Windows.Media.Media3D;
using BasicLoader;
using BasicLoader.Interface;
using CADLoader;
using CADLoader.Implementation.Parser;

namespace STPLoader.Implementation.Model
{
    /// <summary>
    /// 
    /// </summary>
    public class StpFile : IStpModel
    {
        /// <summary>
        /// 
        /// </summary>
		private StpHeader _header;
        /// <summary>
        /// 
        /// </summary>
		private StpData _data;

        private readonly IList<Facet> _facets = new List<Facet>();
        private readonly IList<Vector3D> _vertices = new List<Vector3D>();
        private readonly IList<int> _triangles = new List<int>();
        private readonly IList<IPart> _parts = new List<IPart>();

        /// <summary>
        /// 
        /// </summary>
	    public StpHeader Header
        {
            get { return _header; }
            set { _header = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public StpData Data
        {
            get { return _data; }
            set { _data = value; }
        }

        public override string ToString()
        {
            return String.Format("<StpFile({0}, {1})>", Header, Data);
        }

        public IList<T> All<T>() where T : Entity.Entity
        {
            return _data.All<T>();
        }

        public IDictionary<long, Entity.Entity> All()
        {
            return _data.All();
        }

        public Entity.Entity Get(long id)
        {
            return _data.Get(id);
        }

        public T Get<T>(long id) where T : Entity.Entity
        {
            return _data.Get<T>(id);
        }

        public IList<IPart> Parts
        {
            get { return _parts; }
        }

        public IConstraint GetConstraint(IModel a, IModel b)
        {
            throw new NotImplementedException();
        }

        public IList<Facet> Facets
        {
            get { return _facets; }
        }

        public IList<Vector3D> Vertices
        {
            get { return _vertices; }
        }

        public IList<int> Triangles
        {
            get { return _triangles; }
        }

        public string Name
        {
            get { return Header.Name.Name; }
        }
    }

}

