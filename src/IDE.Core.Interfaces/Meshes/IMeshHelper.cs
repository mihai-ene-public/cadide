using IDE.Core.Types.Media;
using IDE.Core.Types.Media3D;
using System.Collections.Generic;
using System.Xml;

namespace IDE.Core.Interfaces
{
    public interface IMeshHelper
    {
        IMeshModel BuildMeshFromShapes(IEnumerable<IShape> unionItems, IEnumerable<IShape> extractItems, double thickness);

        IMeshModel CreateTube(IList<XPoint3D> xpath, double diameter, int thetaDiv, bool capped);

        IMeshModel CreateTube(IList<XPoint3D> xpath, IList<XPoint> section, bool capped);

        IMeshModel CreateTubes(IList<IList<XPoint3D>> xpaths, IList<IList<XPoint>> xSections);

        IMeshModel ExtrudeOutline(IList<XPoint> points, double height);

        IMeshModel ExtrudeOutlines(IList<IList<XPoint>> fills, IList<IList<XPoint>> holes, double thickness);

        IMeshModel CreateLoftedGeometry(IList<IList<XPoint3D>> positions);

        IMeshModel RevolveOutline(IList<XPoint> points, XPoint3D origin, XVector3D axis, int thetaDiv = 32);
    }
}