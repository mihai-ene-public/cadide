using IDE.Core.Common.Geometries;
using IDE.Core.Coordinates;
using IDE.Core.Interfaces;
using IDE.Core.Storage;
using IDE.Core.Types.Media;
using IDE.Core.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using IxMilia.Dxf;
using IxMilia.Dxf.Entities;

namespace IDE.Core.Importers
{
    public class DxfImporter
    {
        const string dxfExtension = ".dxf";
        const int defaultDestinationLayer = LayerConstants.BoardOutline;

        public List<DxfLayerMappingItem> FootprintLayerMapping { get; set; } = new List<DxfLayerMappingItem>();

        public static string FilesFilter => string.Join(';', $"*{dxfExtension}"
                                                    );

        public double DefaultLineWidth { get; set; } = 0.2;

        public DXFUnits DxfUnits { get; set; } = DXFUnits.mm;

        public IList<LayerPrimitive> Import(string sourceFilePath)
        {
            var result = new List<LayerPrimitive>();

            DxfFile dxfFile = DxfFile.Load(sourceFilePath);

            foreach (DxfEntity entity in dxfFile.Entities)
            {
                var entityPrimitives = GetFootprintPrimitives(entity);
                result.AddRange(entityPrimitives);
            }

            return result;
        }

        TopLeftCoordinatesSystem topLeftCS = CoordinateSystems.TopLeft;
        CartezianCoordinatesSystem cartezianCS = CoordinateSystems.Cartezian;

        double ToCanvasX(double x)
        {
            x = ConvertUnits(x);
            return topLeftCS.ConvertValueFrom(x, Axis.X, cartezianCS);
        }

        double ToCanvasY(double y)
        {
            y = ConvertUnits(y);
            return topLeftCS.ConvertValueFrom(y, Axis.Y, cartezianCS);
        }

        double ConvertUnits(double value)
        {
            switch (DxfUnits)
            {
                case DXFUnits.inch:
                    return 25.4 * value;

                case DXFUnits.mil:
                    return 0.0254 * value;
            }

            return value;
        }

        int GetFootprintLayerIdMapping(string sourceLayerName)
        {
            var fm = FootprintLayerMapping.FirstOrDefault(f => f.SourceLayerName == sourceLayerName);
            if (fm != null)
                return fm.DestLayerId;

            return defaultDestinationLayer;
        }

        IEnumerable<LayerPrimitive> GetFootprintPrimitives(DxfEntity entity)
        {
            var primitives = new List<LayerPrimitive>();
            switch (entity)
            {
                case DxfPolyline polyline:
                    {
                        if (polyline.Vertices.Count < 2)
                            break;

                        var layer = GetFootprintLayerIdMapping(entity.Layer);

                        var sp = polyline.Vertices[0];
                        var startPoint = new XPoint(ToCanvasX(sp.Location.X), ToCanvasY(sp.Location.Y));
                        var bulge = sp.Bulge;
                        for (int i = 1; i < polyline.Vertices.Count; i++)
                        {
                            var ep = polyline.Vertices[i];
                            var endPoint = new XPoint(ToCanvasX(ep.Location.X), ToCanvasY(ep.Location.Y));

                            AddPrimitive(primitives, layer, startPoint, endPoint, bulge);

                            startPoint = endPoint;
                            bulge = ep.Bulge;
                        }

                        //close outline
                        if (sp != polyline.Vertices.Last())
                        {
                            var endPoint = new XPoint(ToCanvasX(sp.Location.X), ToCanvasY(sp.Location.Y));
                            AddPrimitive(primitives, layer, startPoint, endPoint, bulge);
                        }

                        break;
                    }

                case DxfLwPolyline lwPoly:
                    {
                        if (lwPoly.Vertices.Count < 2)
                            break;

                        var layer = GetFootprintLayerIdMapping(entity.Layer);

                        var sp = lwPoly.Vertices[0];
                        var startPoint = new XPoint(ToCanvasX(sp.X), ToCanvasY(sp.Y));
                        var bulge = sp.Bulge;
                        for (int i = 1; i < lwPoly.Vertices.Count; i++)
                        {
                            var ep = lwPoly.Vertices[i];
                            var endPoint = new XPoint(ToCanvasX(ep.X), ToCanvasY(ep.Y));

                            AddPrimitive(primitives, layer, startPoint, endPoint, bulge);

                            startPoint = endPoint;
                            bulge = ep.Bulge;
                        }

                        //close outline
                        if (sp != lwPoly.Vertices.Last())
                        {
                            var endPoint = new XPoint(ToCanvasX(sp.X), ToCanvasY(sp.Y));
                            AddPrimitive(primitives, layer, startPoint, endPoint, bulge);
                        }

                        break;
                    }

                case DxfLine line:
                    {
                        var layer = GetFootprintLayerIdMapping(entity.Layer);
                        var startPoint = line.P1;
                        var endPoint = line.P2;
                        primitives.Add(new LineBoard
                        {
                            x1 = ToCanvasX(startPoint.X),
                            y1 = ToCanvasY(startPoint.Y),
                            x2 = ToCanvasX(endPoint.X),
                            y2 = ToCanvasY(endPoint.Y),
                            width = DefaultLineWidth,
                            layerId = layer
                        });
                        break;
                    }

                    //arc

                    //circle

                    //ellipse

                    //region

                    //spline
            }

            return primitives;
        }

        private void AddPrimitive(IList<LayerPrimitive> primitives, int layer, XPoint startPoint, XPoint endPoint, double lastBulge)
        {
            if (lastBulge != 0.00d)
            {
                var curvature = Math.Round(GetCurvatureFromBulge(lastBulge));
                //if it is positive, it's counterclockwise
                primitives.Add(new ArcBoard
                {
                    SizeDiameter = GetRadiusFromCurvature(curvature, startPoint, endPoint),//?
                    IsLargeArc = Math.Abs(curvature) > 180.0d,
                    SweepDirection = curvature > 0.0d ? XSweepDirection.Counterclockwise : XSweepDirection.Clockwise,
                    StartPoint = startPoint,
                    EndPoint = endPoint,
                    BorderWidth = DefaultLineWidth,
                    layerId = layer
                });
            }
            else
            {
                primitives.Add(new LineBoard
                {
                    x1 = startPoint.X,
                    y1 = startPoint.Y,
                    x2 = endPoint.X,
                    y2 = endPoint.Y,
                    width = DefaultLineWidth,
                    layerId = layer
                });
            }

        }

        double GetRadiusFromCurvature(double curvature, XPoint startPoint, XPoint endPoint)
        {
            var mp = new XPoint(0.5 * (startPoint.X + endPoint.X), 0.5 * (startPoint.Y + endPoint.Y));
            var angleRad = 0.5 * curvature * Math.PI / 180.0d;
            var middleSegmentLen = (endPoint - mp).Length;

            var radius = middleSegmentLen / Math.Sin(angleRad);

            return Math.Abs(radius);
        }

        double GetCurvatureFromBulge(double bulge)
        {
            if (bulge != 0.00d)
            {
                var radians = 4 * Math.Atan(bulge);
                return 180.0 * radians / Math.PI;
            }

            return bulge;
        }
    }
}
