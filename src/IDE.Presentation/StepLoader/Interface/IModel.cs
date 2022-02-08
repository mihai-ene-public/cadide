using System.Collections;
using System.Collections.Generic;
using System.Windows.Media.Media3D;
using BasicLoader.Interface;
using CADLoader.Implementation.Parser;

namespace BasicLoader
{
    /// <summary>
    /// 
    /// </summary>
    public interface IModel
    {
        /// <summary>
        /// 
        /// </summary>
        IList<IPart> Parts { get; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        IConstraint GetConstraint(IModel a, IModel b);
        /// <summary>
        /// 
        /// </summary>
        IList<Facet> Facets { get; }
        /// <summary>
        /// 
        /// </summary>
        IList<Vector3D> Vertices { get; }
        /// <summary>
        /// 
        /// </summary>
        IList<int> Triangles { get; }
        /// <summary>
        /// 
        /// </summary>
        string Name { get; }
    }
}