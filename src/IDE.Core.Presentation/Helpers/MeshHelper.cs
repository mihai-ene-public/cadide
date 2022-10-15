using System;
using System.Collections.Generic;
using System.Linq;
using IDE.Core.Common.Geometries;
using IDE.Core.Interfaces;
using IDE.Core.Interfaces.Geometries;
using IDE.Core.Types.Media;
using IDE.Core.Types.Media3D;
using TriangleNet.Geometry;
using TriangleNet.Meshing;
using IDE.Presentation.Extensions;
using IDE.Core.Types.MeshBuilding;

namespace IDE.Core.Collision;

public class MeshHelper : IMeshHelper
{
    public MeshHelper()
    {
    }

    private void TriangulatePoly(XMeshBuilder meshBuilder, XVector3D textDirection, XPoint3D p0, XPoint3D p1, TriangleNet.Geometry.Polygon polygon, bool reverse = false)
    {
        var mesher = new GenericMesher();
        var options = new ConstraintOptions();
        var mesh = mesher.Triangulate(polygon, options);

        var u = textDirection;
        u.Normalize();
        var z = p1 - p0;
        var h = z.Length;
        z.Normalize();
        var v = XVector3D.CrossProduct(z, u);


        // Convert the triangles
        foreach (var t in mesh.Triangles)
        {
            var v0 = t.GetVertex(0);
            var v1 = t.GetVertex(1);
            var v2 = t.GetVertex(2);

            if (reverse)
            {
                // Add the top triangle.
                // Project the X/Y vertices onto a plane defined by textdirection, p0 and p1.                
                meshBuilder.AddTriangle(
                    Project(v0, p0, u, v, z, h),
                    Project(v2, p0, u, v, z, h),
                    Project(v1, p0, u, v, z, h)
                    );

                // Add the bottom triangle.
                meshBuilder.AddTriangle(
                    Project(v2, p0, u, v, z, 0),
                    Project(v0, p0, u, v, z, 0),
                    Project(v1, p0, u, v, z, 0)
                    );
            }
            else
            {
                // Add the top triangle.
                // Project the X/Y vertices onto a plane defined by textdirection, p0 and p1.                
                meshBuilder.AddTriangle(Project(v0, p0, u, v, z, h), Project(v1, p0, u, v, z, h), Project(v2, p0, u, v, z, h));

                // Add the bottom triangle.
                meshBuilder.AddTriangle(Project(v2, p0, u, v, z, 0), Project(v1, p0, u, v, z, 0), Project(v0, p0, u, v, z, 0));
            }

        }
    }

    private void TriangulatePoly(XMeshBuilder meshBuilder, XVector3D axisX, XPoint3D p0, XVector3D axisUp, double thickness, TriangleNet.Geometry.Polygon polygon, bool reverse = false)
    {
        var mesher = new GenericMesher();
        var options = new ConstraintOptions();
        var mesh = mesher.Triangulate(polygon, options);

        var u = axisX;
        u.Normalize();
        var z = axisUp;
        var h = thickness;
        z.Normalize();
        var v = XVector3D.CrossProduct(z, u);


        // Convert the triangles
        foreach (var t in mesh.Triangles)
        {
            var v0 = t.GetVertex(0);
            var v1 = t.GetVertex(1);
            var v2 = t.GetVertex(2);

            if (reverse)
            {
                // Add the top triangle.
                // Project the X/Y vertices onto a plane defined by textdirection, p0 and p1.                
                meshBuilder.AddTriangle(
                    Project(v0, p0, u, v, z, h),
                    Project(v2, p0, u, v, z, h),
                    Project(v1, p0, u, v, z, h)
                    );

                // Add the bottom triangle.
                meshBuilder.AddTriangle(
                    Project(v2, p0, u, v, z, 0),
                    Project(v0, p0, u, v, z, 0),
                    Project(v1, p0, u, v, z, 0)
                    );
            }
            else
            {
                // Add the top triangle.
                // Project the X/Y vertices onto a plane defined by textdirection, p0 and p1.                
                meshBuilder.AddTriangle(Project(v0, p0, u, v, z, h), Project(v1, p0, u, v, z, h), Project(v2, p0, u, v, z, h));

                // Add the bottom triangle.
                meshBuilder.AddTriangle(Project(v2, p0, u, v, z, 0), Project(v1, p0, u, v, z, 0), Project(v0, p0, u, v, z, 0));
            }

        }
    }

    private XVector3D Project(Vertex v, XPoint3D p0, XVector3D x, XVector3D y, XVector3D z, double h)
    {
        var p = ( p0 + x * v.X - y * v.Y + z * h );
        return new XVector3D(p.X, p.Y, p.Z);
    }

    public IEnumerable<XPoint> ToSegments(IEnumerable<XPoint> input)
    {
        bool first = true;
        var previous = default(XPoint);
        foreach (var point in input)
        {
            if (!first)
            {
                yield return previous;
                yield return point;
            }
            else
            {
                first = false;
            }

            previous = point;
        }
    }

    public IMeshModel BuildMeshFromShapes(IEnumerable<IShape> unionItems, IEnumerable<IShape> extractItems, double thickness)
    {
        var mg = BuildMeshFromShapesInternal(unionItems, extractItems, thickness);
        //return mg.ToMeshModel();
        return mg;
    }

    private IMeshModel BuildMeshFromShapesInternal(IEnumerable<IShape> unionShapes, IEnumerable<IShape> extractShapes, double thickness)
    {
        var geometryOutlineHelper = ServiceProvider.Resolve<IGeometryOutlineHelper>();
        var builder = new XMeshBuilder();

        IList<IList<XPoint>> fills = new List<IList<XPoint>>();
        IList<IList<XPoint>> holes = new List<IList<XPoint>>();

        foreach (var shape in unionShapes)
        {
            if (shape is IPouredPolygonShape)
                continue;

            var geomItem = geometryOutlineHelper.GetShapeGeometry(shape);

            if (geomItem == null)
                continue;

            AppendOutlines(geomItem, fills);
        }

        foreach (var shape in extractShapes)
        {
            var geomItem = geometryOutlineHelper.GetShapeGeometry(shape);

            if (geomItem == null)
                continue;

            AppendOutlines(geomItem, holes);
        }

        fills = GeometryOutline.Combine(fills, null, Common.Geometries.GeometryCombineMode.Union);
        holes = GeometryOutline.Combine(holes, null, Common.Geometries.GeometryCombineMode.Union);

        ExtrudeOutlines(builder, fills, holes, thickness);

        //polys last
        var polyShapes = unionShapes.OfType<IPouredPolygonShape>().ToList();
        if (polyShapes.Count > 0)
        {
            fills = new List<IList<XPoint>>();

            foreach (var poly in polyShapes)
            {
                var geomItem = poly.FinalGeometry;

                if (geomItem == null)
                    continue;

                AppendOutlines(geomItem, fills);
            }

            ExtrudeOutlines(builder, fills, holes, thickness);
        }

        builder.Normals = null;
        builder.ComputeNormalsAndTangents(MeshFaces.Default, builder.HasTangents);

        return builder.ToMeshModel(XColors.Silver);
    }

    private void AppendOutlines(IGeometryOutline geometry, IList<IList<XPoint>> contours)
    {
        if (geometry is GeometryOutlines outlines)
        {
            for (int i = 0; i < outlines.Outlines.Count; i++)
            {
                var outline = outlines.GetOutlinePoints(i);
                if (outline.Count > 0)
                {
                    //close the outline
                    if (outline[0] != outline[outline.Count - 1])
                        outline.Add(outline[0]);

                    contours.Add(outline);
                }
            }
        }
        else
        {
            var outline = geometry.GetOutline();
            if (outline.Count > 0)
            {
                //close the outline
                if (outline[0] != outline[outline.Count - 1])
                    outline.Add(outline[0]);

                contours.Add(outline);
            }
        }
    }

    private void ExtrudeOutlines(XMeshBuilder builder, IList<IList<XPoint>> fills, IList<IList<XPoint>> holes, double start, double thickness)
    {
        if (fills.Count == 0)
            return;

        var textDirection = new XVector3D(1, 0, 0);
        var p0 = new XPoint3D(0, 0, -start);
        var p1 = new XPoint3D(0, 0, -( start + thickness ));

        // Build the polygon to mesh (using Triangle.NET to triangulate)
        var polygon = new TriangleNet.Geometry.Polygon();
        int marker = 0;

        foreach (var fillOutline in fills)
        {
            //it could have outlines that qualifies as holes

            //close the outline
            if (fillOutline[0] != fillOutline[fillOutline.Count - 1])
                fillOutline.Add(fillOutline[0]);

            var isHole = GeometryOutline.GetOutlineOrientation(fillOutline) == GeometryOutlineOrientation.Negative;

            try
            {

                polygon.AddContour(fillOutline.Select(p => new Vertex(p.X, p.Y)), marker++, hole: isHole);
            }
            catch { }
            builder.AddExtrudedSegments(ToSegments(fillOutline).ToList(), textDirection, (XVector3D)p0, (XVector3D)p1);
        }

        foreach (var holeOutline in holes)
        {
            //close the outline
            if (holeOutline[0] != holeOutline[holeOutline.Count - 1])
                holeOutline.Add(holeOutline[0]);

            polygon.AddContour(holeOutline.Select(p => new Vertex(p.X, p.Y)), marker++, hole: true);

            builder.AddExtrudedSegments(ToSegments(holeOutline.Reverse()).ToList(), textDirection, (XVector3D)p0, (XVector3D)p1);
        }

        TriangulatePoly(builder, textDirection, p0, p1, polygon, reverse: true);
    }

    private void ExtrudeOutlines(XMeshBuilder builder, IList<IList<XPoint>> fills, IList<IList<XPoint>> holes, double thickness)
    {
        ExtrudeOutlines(builder, fills, holes, 0.0d, thickness);
    }

    public IMeshModel CreateTube(IList<XPoint3D> xpath, double diameter, int thetaDiv, bool capped)
    {
        //Material padMaterial = MaterialHelper.CreateMaterial(Colors.Silver);

        var meshBuilder = new XMeshBuilder();// false, false);
        var path = xpath.Select(p => (XVector3D)p).ToList();
        meshBuilder.AddTube(path, diameter, thetaDiv, false, capped, capped);

        if (meshBuilder.CreateNormals)
        {
            meshBuilder.Normals = null;
            meshBuilder.ComputeNormalsAndTangents(MeshFaces.Default, meshBuilder.HasTangents);
        }

        var meshModel = meshBuilder.ToMeshModel(XColors.Silver);
        return meshModel;
    }

    public IMeshModel CreateTube(IList<XPoint3D> xpath, IList<XPoint> xSection, bool capped)
    {
        //Material padMaterial = MaterialHelper.CreateMaterial(Colors.Silver);

        var meshBuilder = new XMeshBuilder();
        var path = xpath.Select(p => (XVector3D)p).ToList();
        var section = xSection;//.Select(p => p.ToVector2()).ToList();

        var sectionXAxis = new XVector3D(0, 1, 0);

        meshBuilder.AddTube(path, null, null, null, section, sectionXAxis, false, capped, capped);

        if (meshBuilder.CreateNormals)
        {
            meshBuilder.Normals = null;
            meshBuilder.ComputeNormalsAndTangents(MeshFaces.Default, meshBuilder.HasTangents);
        }

        var meshModel = meshBuilder.ToMeshModel(XColors.Silver);
        return meshModel;
    }

    public IMeshModel CreateTubes(IList<IList<XPoint3D>> xpaths, IList<IList<XPoint>> xSections)
    {
        //Material padMaterial = MaterialHelper.CreateMaterial(Colors.Silver);

        var meshBuilder = new XMeshBuilder();

        for (int i = 0; i < xpaths.Count; i++)
        {
            var xpath = xpaths[i];
            var xSection = xSections[i];
            var path = xpath.Select(p => (XVector3D)p).ToList();
            var section = xSection;//.Select(p => p).ToList();

            var sectionXAxis = new XVector3D(0, 1, 0);

            meshBuilder.AddTube(path, null, null, null, section, sectionXAxis, false, false, true, true);
        }

        if (meshBuilder.CreateNormals)
        {
            meshBuilder.Normals = null;
            meshBuilder.ComputeNormalsAndTangents(MeshFaces.Default, meshBuilder.HasTangents);
        }

        var meshModel = meshBuilder.ToMeshModel(XColors.Silver);
        return meshModel;
    }

    public IMeshModel ExtrudeOutline(IList<XPoint> points, double height)
    {
        if (points == null || points.Count < 2)
            return null;

        var builder = new XMeshBuilder();

        var axisX = new XVector3D(1, 0, 0);

        var startPoint = new XPoint3D(0, 0, 0);
        var endPoint = new XPoint3D(0, 0, 1) * height;

        if (points.Count % 2 == 1)
            points.Add(points[0]);

        //var wpfPoints = points.Select(p => p.ToVector2()).ToList();
        builder.AddExtrudedSegments(points, axisX, (XVector3D)startPoint, (XVector3D)endPoint);

        //top and bottom faces
        var polygon = new TriangleNet.Geometry.Polygon();
        polygon.AddContour(points.Select(p => new Vertex(p.X, p.Y)));

        TriangulatePoly(builder, axisX, startPoint, endPoint, polygon, true);

        //return group;
        var meshModel = builder.ToMeshModel(XColors.Silver);
        return meshModel;
    }

    public IMeshModel ExtrudeOutlines(IList<IList<XPoint>> fills, IList<IList<XPoint>> holes, double thickness)
    {
        var axisX = new XVector3D(1, 0, 0);
        var axisUp = new XVector3D(0, 0, -1);
        var p0 = new XPoint3D(0, 0, 0);
        var p1 = new XPoint3D(0, 0, -thickness);
        var hasThickness = thickness > 0.00d;

        var builder = new XMeshBuilder();

        var polygon = new TriangleNet.Geometry.Polygon();
        int marker = 0;

        foreach (var fillOutline in fills)
        {
            //it could have outlines that qualifies as holes

            //close the outline
            if (fillOutline[0] != fillOutline[fillOutline.Count - 1])
                fillOutline.Add(fillOutline[0]);

            var isHole = GeometryOutline.GetOutlineOrientation(fillOutline) == GeometryOutlineOrientation.Negative;

            polygon.AddContour(fillOutline.Select(p => new Vertex(p.X, p.Y)), marker++, hole: isHole);

            if (hasThickness)
            {
                builder.AddExtrudedSegments(ToSegments(fillOutline).ToList(), axisX, (XVector3D)p0, (XVector3D)p1);
            }
        }

        foreach (var holeOutline in holes)
        {
            //close the outline
            if (holeOutline[0] != holeOutline[holeOutline.Count - 1])
                holeOutline.Add(holeOutline[0]);

            polygon.AddContour(holeOutline.Select(p => new Vertex(p.X, p.Y)), marker++, hole: true);

            if (hasThickness)
            {
                builder.AddExtrudedSegments(ToSegments(holeOutline.Reverse()).ToList(), axisX, (XVector3D)p0, (XVector3D)p1);
            }
        }

        if (hasThickness)
        {
            TriangulatePoly(builder, axisX, p0, p1, polygon, reverse: true);
        }
        else
        {
            TriangulatePoly(builder, axisX, p0, axisUp, thickness, polygon, reverse: false);
        }

        var meshModel = builder.ToMeshModel(XColors.Silver);
        return meshModel;
    }

    public IMeshModel RevolveOutline(IList<XPoint> points, XPoint3D origin, XVector3D axis, int thetaDiv = 32)
    {
        var builder = new XMeshBuilder();

        //var sdkPoints = points.Select(p => p.ToVector2()).ToList();
        builder.AddRevolvedGeometry(points, null, (XVector3D)origin, axis, thetaDiv);

        var meshModel = builder.ToMeshModel(XColors.Silver);
        return meshModel;
    }

    public IMeshModel CreateLoftedGeometry(IList<IList<XPoint3D>> positions)
    {
        var builder = new XMeshBuilder();

        IList<IList<XVector3D>> sdkPoints = new List<IList<XVector3D>>();

        sdkPoints.AddRange(positions.Select(pl => pl.Select(p => (XVector3D)p).ToList()));

        builder.AddLoftedGeometry(sdkPoints, null, null);

        if (builder.CreateNormals)
        {
            builder.Normals = null;
            builder.ComputeNormalsAndTangents(MeshFaces.Default, builder.HasTangents);
        }

        var meshModel = builder.ToMeshModel(XColors.Silver);
        return meshModel;
    }
}

