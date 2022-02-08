using IDE.Core.Types.Media;
using IDE.Core.Types.Media3D;
using System.Collections.Generic;
using System.Xml;

namespace IDE.Core.Interfaces
{
    public interface IMeshHelper
    {
        //object BuildMeshFromItems(IEnumerable<ICanvasItem> unionItems, IEnumerable<ICanvasItem> extractItems, double start, double thickness);

        IMeshModel BuildMeshFromItems2(IEnumerable<ICanvasItem> unionItems, IEnumerable<ICanvasItem> extractItems, double start, double thickness);

        bool IsPointInPolygon(IList<XPoint> polygon, XPoint testPoint);


       // IList<ISelectableItem> GetModel3DGroupChildren(object model3DGroup);

        //object ApplyColoredModel(XColor color, object model3DGroupObj);

        IMeshModel GetMeshModelFromCData(XmlCDataSection cData);

        XmlCDataSection GetCDataFromMeshModel(IMeshModel model);

        IMeshModel CreateTube(IList<XPoint3D> xpath, double diameter, int thetaDiv, bool capped);

        IMeshModel CreateTube(IList<XPoint3D> xpath, IList<XPoint> section, bool capped);

        IMeshModel CreateTubes(IList<IList<XPoint3D>> xpaths, IList<IList<XPoint>> xSections);

        IMeshModel ExtrudeOutline(IList<XPoint> points, double height);

        IMeshModel ExtrudeOutlines(IList<IList<XPoint>> fills, IList<IList<XPoint>> holes, double thickness);

        IMeshModel CreateLoftedGeometry(IList<IList<XPoint3D>> positions);

        IMeshModel RevolveOutline(IList<XPoint> points, XPoint3D origin, XVector3D axis, int thetaDiv = 32);
    }
}