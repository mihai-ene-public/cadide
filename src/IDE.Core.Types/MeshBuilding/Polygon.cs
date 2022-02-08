using Int32Collection = System.Collections.Generic.List<int>;
using PointCollection = System.Collections.Generic.List<IDE.Core.Types.Media.XPoint>;

namespace IDE.Core.Types.MeshBuilding
{
    public class Polygon
    {
        // http://softsurfer.com/Archive/algorithm_0101/algorithm_0101.htm
        /// <summary>
        /// The points.
        /// </summary>
        internal PointCollection points;

        /// <summary>
        /// Gets or sets the points.
        /// </summary>
        /// <value>The points.</value>
        public PointCollection Points
        {
            get
            {
                return this.points ?? (this.points = new PointCollection());
            }

            set
            {
                this.points = value;
            }
        }

        /// <summary>
        /// Triangulate the polygon by using the sweep line algorithm
        /// </summary>
        /// <returns>An index collection.</returns>
        public Int32Collection Triangulate()
        {
            return SweepLinePolygonTriangulator.Triangulate(this.points);
        }
    }

}
