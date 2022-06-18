using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using System.Xml;
using IDE.Core.Common.Geometries;
using IDE.Core.Designers;
using IDE.Core.Interfaces;
using IDE.Core.Interfaces.Geometries;
using IDE.Core.Types.Media;
using IDE.Core.Types.Media3D;
using IDE.Documents.Views;
using TriangleNet.Geometry;
using TriangleNet.Meshing;
using IDE.Presentation.Extensions;

using LineSegment = System.Windows.Media.LineSegment;
using Point = System.Windows.Point;
using HelixToolkit.Wpf.SharpDX;

namespace IDE.Core.Collision
{
    public class MeshHelper : IMeshHelper
    {
        public MeshHelper()
        {
            GeometryHelper = ServiceProvider.Resolve<IGeometryHelper>();
        }
        IGeometryHelper GeometryHelper;

        public void ExtrudeGeometry(MeshBuilder meshBuilder, Geometry geometry, XVector3D textDirection, XPoint3D p0, XPoint3D p1, bool checkHoles = true)
        {
            if (geometry.IsEmpty()) return;

            var outlineList = new List<List<XPoint[]>>();
            GeometryHelper.AppendOutlines(geometry, outlineList);

            // Build the polygon to mesh (using Triangle.NET to triangulate)
            var polygon = new TriangleNet.Geometry.Polygon();
            int marker = 0;

            var holes = new List<XPoint[]>();

            foreach (var outlines in outlineList)
            {
                var outerOutline = outlines.OrderBy(x => Geometry2DHelper.AreaOfSegment(x)).Last();

                for (int i = 0; i < outlines.Count; i++)
                {
                    var outline = outlines[i];
                    var isHole = false;

                    if (checkHoles)
                    {


                        //isHole = outerOutline != outline &&  IsPointInPolygon(outerOutline, outline[0]);

                        isHole = outerOutline != outline && outline.Any(o => IsPointInPolygon(outerOutline, o));

                        //isHole = outlines.Any(o => OutlineContainsOtherOutline(o, outerOutline));
                    }

                    polygon.AddContour(outline.Select(p => new Vertex(p.X, p.Y)), marker++, isHole);
                    //register holes
                    if (isHole)
                        holes.Add(outline);
                    //meshBuilder.AddExtrudedSegments(ToSegments(outline.Select(p => p.ToPoint())).ToList(), textDirection, p0, p1);
                }

                meshBuilder.AddExtrudedSegments(ToSegments(outerOutline).ToList(), textDirection.ToVector3(), p0.ToVector3(), p1.ToVector3());
            }

            foreach (var hole in holes)
            {
                var isInnerHole = holes.Where(hh => hh != hole).Any(hh => IsHoleInHole(hole, hh));
                if (!isInnerHole)
                {
                    meshBuilder.AddExtrudedSegments(ToSegments(hole).ToList(), textDirection.ToVector3(), p0.ToVector3(), p1.ToVector3());
                }
            }

            TriangulatePoly(meshBuilder, textDirection, p0, p1, polygon);
        }

        private void TriangulatePoly(MeshBuilder meshBuilder, XVector3D textDirection, XPoint3D p0, XPoint3D p1, TriangleNet.Geometry.Polygon polygon, bool reverse = false)
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

        private void TriangulatePoly(MeshBuilder meshBuilder, XVector3D axisX, XPoint3D p0, XVector3D axisUp, double thickness, TriangleNet.Geometry.Polygon polygon, bool reverse = false)
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


        public void ExtrudeGeometry(MeshBuilder meshBuilder, Geometry geometry)
        {
            ExtrudeGeometry(meshBuilder, geometry, new XVector3D(1, 0, 0), new XPoint3D(0, 0, 0), new XPoint3D(0, 0, 1));
        }

        public SharpDX.Vector3 Project(Vertex v, XPoint3D p0, XVector3D x, XVector3D y, XVector3D z, double h)
        {
            return ( p0 + x * v.X - y * v.Y + z * h ).ToVector3();
        }



        public bool IsPointInPolygon(IList<XPoint> polygon, XPoint testPoint)
        {
            bool result = false;
            int j = polygon.Count - 1;
            for (int i = 0; i < polygon.Count; i++)
            {
                if (( polygon[i].Y < testPoint.Y && polygon[j].Y >= testPoint.Y ) || ( polygon[j].Y < testPoint.Y && polygon[i].Y >= testPoint.Y ))
                {
                    if (polygon[i].X + ( ( testPoint.Y - polygon[i].Y ) / ( polygon[j].Y - polygon[i].Y ) * ( polygon[j].X - polygon[i].X ) ) < testPoint.X)
                    {
                        result = !result;
                    }
                }

                j = i;
            }

            return result;
        }

        bool IsHoleInHole(IEnumerable<XPoint> testHole, IEnumerable<XPoint> containerHole)
        {
            return testHole.All(p => IsPointInPolygon(containerHole.ToList(), p));
        }

        bool OutlineContainsOtherOutline(IEnumerable<XPoint> testOutline, IEnumerable<XPoint> containerOutline)
        {
            return testOutline.All(p => testOutline != containerOutline && IsPointInPolygon(containerOutline.ToList(), p));
        }

        public IEnumerable<SharpDX.Vector2> ToSegments(IEnumerable<XPoint> input)
        {
            bool first = true;
            var previous = default(SharpDX.Vector2);
            foreach (var point in input)
            {
                if (!first)
                {
                    yield return previous;
                    yield return point.ToVector2();
                }
                else
                {
                    first = false;
                }

                previous = point.ToVector2();
            }
        }

        public IEnumerable<IList<XPoint[]>> GetTextOutlines(string text, string fontName, FontStyle fontStyle, FontWeight fontWeight, double fontSize)
        {
            var ff = new FontFamily(fontName);
            var tf = new Typeface(ff, fontStyle, fontWeight, FontStretches.Normal);
            var formattedText = new FormattedText(
                text,
                CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                tf,
                fontSize,
                Brushes.Black,
                pixelsPerDip: 1.0d);

            var textGeometry = formattedText.BuildGeometry(new Point(0, 0));
            var outlines = new List<List<XPoint[]>>();
            GeometryHelper.AppendOutlines(textGeometry, outlines);
            return outlines;
        }

        public IMeshModel BuildMeshFromItems2(IEnumerable<ICanvasItem> unionItems, IEnumerable<ICanvasItem> extractItems, double start, double thickness)
        {
            var mg = BuildMeshFromItemsInternal2(unionItems, extractItems, start, thickness);
            return mg.ToMeshModel();
        }

        private MeshGeometry3D BuildMeshFromItemsInternal2(IEnumerable<ICanvasItem> unionItems, IEnumerable<ICanvasItem> extractItems, double start, double thickness)
        {
            var geometryOutlineHelper = ServiceProvider.Resolve<IGeometryOutlineHelper>();
            var builder = new MeshBuilder();

            IList<IList<XPoint>> fills = new List<IList<XPoint>>();
            IList<IList<XPoint>> holes = new List<IList<XPoint>>();

            foreach (ISelectableItem item in unionItems)
            {
                IGeometryOutline geomItem = null;

                if (item is IPolygonBoardCanvasItem)
                {
                    continue;
                }
                else
                {
                    geomItem = geometryOutlineHelper.GetGeometry(item);
                }

                if (geomItem == null)
                    continue;

                geomItem.Transform = item.GetTransform();

                AppendOutlines(geomItem, fills);
            }

            foreach (ISelectableItem item in extractItems)
            {
                var geomItem = geometryOutlineHelper.GetGeometry(item);

                if (geomItem == null)
                    continue;

                geomItem.Transform = item.GetTransform();

                AppendOutlines(geomItem, holes);
            }

            fills = GeometryOutline.Combine(fills, null, Common.Geometries.GeometryCombineMode.Union);
            holes = GeometryOutline.Combine(holes, null, Common.Geometries.GeometryCombineMode.Union);

            ExtrudeOutlines(builder, fills, holes, start, thickness);

            //polys last
            var polyItems = unionItems.OfType<IPolygonBoardCanvasItem>().ToList();
            if (polyItems.Count > 0)
            {
                fills = new List<IList<XPoint>>();

                var polyPourProcessor = ServiceProvider.Resolve<IPolygonGeometryOutlinePourProcessor>();

                foreach (var poly in polyItems)
                {

                    var geomItem = polyPourProcessor.GetGeometry(poly, null);

                    if (geomItem == null)
                        continue;

                    geomItem.Transform = poly.GetTransform();

                    AppendOutlines(geomItem, fills);
                }

                ExtrudeOutlines(builder, fills, holes, start, thickness);
            }

            builder.Normals = null;
            builder.ComputeNormalsAndTangents(MeshFaces.Default, builder.HasTangents);

            return builder.ToMesh();
        }


        public IMeshModel BuildMeshFromShapes(IEnumerable<IShape> unionItems, IEnumerable<IShape> extractItems, double thickness)
        {
            var mg = BuildMeshFromShapesInternal(unionItems, extractItems, thickness);
            return mg.ToMeshModel();
        }

        private MeshGeometry3D BuildMeshFromShapesInternal(IEnumerable<IShape> unionShapes, IEnumerable<IShape> extractShapes, double thickness)
        {
            var geometryOutlineHelper = ServiceProvider.Resolve<IGeometryOutlineHelper>();
            var builder = new MeshBuilder();

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

            foreach (var  shape in extractShapes)
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
            if(polyShapes.Count>0)
            {
                fills = new List<IList<XPoint>>();

                foreach(var poly in polyShapes)
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

            return builder.ToMesh();
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

        private void ExtrudeOutlines(MeshBuilder builder, IList<IList<XPoint>> fills, IList<IList<XPoint>> holes, double start, double thickness)
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
                builder.AddExtrudedSegments(ToSegments(fillOutline).ToList(), textDirection.ToVector3(), p0.ToVector3(), p1.ToVector3());
            }

            foreach (var holeOutline in holes)
            {
                //close the outline
                if (holeOutline[0] != holeOutline[holeOutline.Count - 1])
                    holeOutline.Add(holeOutline[0]);

                polygon.AddContour(holeOutline.Select(p => new Vertex(p.X, p.Y)), marker++, hole: true);

                builder.AddExtrudedSegments(ToSegments(holeOutline.Reverse()).ToList(), textDirection.ToVector3(), p0.ToVector3(), p1.ToVector3());
            }

            TriangulatePoly(builder, textDirection, p0, p1, polygon, reverse: true);
        }

        private void ExtrudeOutlines(MeshBuilder builder, IList<IList<XPoint>> fills, IList<IList<XPoint>> holes, double thickness)
        {
            ExtrudeOutlines(builder, fills, holes, 0.0d, thickness);
        }

        public IMeshModel CreateTube(IList<XPoint3D> xpath, double diameter, int thetaDiv, bool capped)
        {
            //Material padMaterial = MaterialHelper.CreateMaterial(Colors.Silver);

            var meshBuilder = new MeshBuilder();// false, false);
            var path = xpath.Select(p => p.ToVector3()).ToList();
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

            var meshBuilder = new MeshBuilder();
            var path = xpath.Select(p => p.ToVector3()).ToList();
            var section = xSection.Select(p => p.ToVector2()).ToList();

            var sectionXAxis = new SharpDX.Vector3(0, 1, 0);

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

            var meshBuilder = new MeshBuilder();

            for (int i = 0; i < xpaths.Count; i++)
            {
                var xpath = xpaths[i];
                var xSection = xSections[i];
                var path = xpath.Select(p => p.ToVector3()).ToList();
                var section = xSection.Select(p => p.ToVector2()).ToList();

                var sectionXAxis = new SharpDX.Vector3(0, 1, 0);

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

            var builder = new MeshBuilder();

            var axisX = new XVector3D(1, 0, 0);

            var startPoint = new XPoint3D(0, 0, 0);
            var endPoint = new XPoint3D(0, 0, 1) * height;

            if (points.Count % 2 == 1)
                points.Add(points[0]);

            var wpfPoints = points.Select(p => p.ToVector2()).ToList();
            builder.AddExtrudedSegments(wpfPoints, axisX.ToVector3(), startPoint.ToVector3(), endPoint.ToVector3());

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

            var builder = new MeshBuilder();

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
                    builder.AddExtrudedSegments(ToSegments(fillOutline).ToList(), axisX.ToVector3(), p0.ToVector3(), p1.ToVector3());
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
                    builder.AddExtrudedSegments(ToSegments(holeOutline.Reverse()).ToList(), axisX.ToVector3(), p0.ToVector3(), p1.ToVector3());
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
            var builder = new MeshBuilder();

            var sdkPoints = points.Select(p => p.ToVector2()).ToList();
            builder.AddRevolvedGeometry(sdkPoints, null, origin.ToVector3(), axis.ToVector3(), thetaDiv);

            var meshModel = builder.ToMeshModel(XColors.Silver);
            return meshModel;
        }

        public IMeshModel CreateLoftedGeometry(IList<IList<XPoint3D>> positions)
        {
            var builder = new MeshBuilder();

            IList<IList<SharpDX.Vector3>> sdkPoints = new List<IList<SharpDX.Vector3>>();

            sdkPoints.AddRange(positions.Select(pl => pl.Select(p => p.ToVector3()).ToList()));

            builder.AddLoftedGeometry(sdkPoints, null, null);

            if (builder.CreateNormals)
            {
                builder.Normals = null;
                builder.ComputeNormalsAndTangents(MeshFaces.Default, builder.HasTangents);
            }

            var meshModel = builder.ToMeshModel(XColors.Silver);
            return meshModel;
        }

        public IMeshModel GetMeshModelFromCData(XmlCDataSection cData)
        {
            var modelGroup = ModelFromCData.GetModel(cData);
            var meshModel = modelGroup.ToMeshModel();
            return meshModel;
        }

        public XmlCDataSection GetCDataFromMeshModel(IMeshModel model)
        {
            var modelGroup = model.ToModel3DGroup();
            return ModelFromCData.GetCData(modelGroup);
        }
    }

    class ModelFromCData
    {
        public static System.Windows.Media.Media3D.Model3DGroup GetModel(XmlCDataSection cdata)
        {
            var content = cdata.Value;
            if (string.IsNullOrEmpty(content))
                return null;

            var dispatcher = ServiceProvider.Resolve<IDispatcherHelper>();
            System.Windows.Media.Media3D.Model3DGroup model = null;
            dispatcher.RunOnDispatcher(() =>
            {
                model = XamlReader.Parse(content) as System.Windows.Media.Media3D.Model3DGroup;
                model.Freeze();
            });
            return model;
        }

        public static XmlCDataSection GetCData(System.Windows.Media.Media3D.Model3DGroup model)
        {
            var sb = new StringBuilder();
            using (var tw = new StringWriter(sb))
            {
                XamlWriter.Save(model, tw);
            }

            var doc = new XmlDocument();
            return doc.CreateCDataSection(sb.ToString());
        }
    }
}
