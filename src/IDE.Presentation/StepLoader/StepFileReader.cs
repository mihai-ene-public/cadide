using System.IO;
using System.Windows.Threading;
using STPLoader.Implementation.Parser;
using System.Collections.Generic;

namespace IDE.Documents.Views
{
    //public class StepFileReader : ModelReader
    //{
    //    public StepFileReader()
    //    {

    //    }


    //    public override List<Object3D> Read(Stream stream, ModelInfo info = default)
    //    {
    //        var objects = new List<Object3D>();

    //        var parser = new StpParser();
    //        var model = parser.Parse(stream);

    //        var closedShells = STPConverter.Converter.GetClosedShells(model);


    //        foreach (var shell in closedShells)
    //        {
    //            // foreach (var s in shell.Convertables)
    //            {
    //                if (shell.Points.Count == 0)
    //                    continue;
    //                var mb = new MeshBuilder();

    //                var scale = 25.4f;

    //                foreach (var p in shell.Points)
    //                    mb.Positions.Add(new SharpDX.Vector3((float)p.X * scale, (float)p.Y * scale, (float)p.Z * scale));
    //                foreach (var p in shell.Indices)
    //                    mb.TriangleIndices.Add(p);

    //                //HelixToolkit is very buggy and not tested
    //                //should make a diff between branches
    //                //mb.Append(s.Points.Select(p => new Point3D(p.X, p.Y, p.Z)).ToList(), s.Indices);//, null, null);// new List<Vector3D>(), new List<System.Windows.Point>());

    //                var geometry = new Object3D
    //                {
    //                    Geometry = mb.ToMesh(),
    //                };
    //                objects.Add(geometry);
    //            }
    //        }

    //        return objects;
    //    }
       

    //    /*
    //    IxMilia.Step.Items.StepAxis2Placement3D
    //    IxMilia.Step.Items.StepCartesianPoint
    //    IxMilia.Step.Items.StepEdgeCurve
    //    IxMilia.Step.Items.StepDirection
    //    IxMilia.Step.Items.StepCircle
    //    IxMilia.Step.Items.StepLine
    //    IxMilia.Step.Items.StepOrientedEdge
    //    IxMilia.Step.Items.StepFaceOuterBound??
    //    IxMilia.Step.Items.StepCylindricalSurface??
    //    IxMilia.Step.Items.StepVertexPoint
    //    IxMilia.Step.Items.StepPlane??
    //    IxMilia.Step.Items.StepVector
    //    IxMilia.Step.Items.StepEdgeLoop

    //    Unsupported item SURFACE_STYLE_USAGE at 21, 6
    //    Unsupported item PRESENTATION_STYLE_ASSIGNMENT at 24, 6
    //    Unsupported item APPLICATION_CONTEXT at 26, 7
    //    Unsupported item ADVANCED_FACE at 27, 7
    //    Unsupported item PRODUCT_CONTEXT at 32, 7
    //    Unsupported item PRESENTATION_LAYER_ASSIGNMENT at 42, 7
    //    Unsupported item SURFACE_STYLE_FILL_AREA at 44, 7
    //    Unsupported item STYLED_ITEM at 46, 7
    //    Unsupported item COLOUR_RGB at 49, 7
    //    Unsupported item SURFACE_SIDE_STYLE at 65, 7
    //    Unsupported item FILL_AREA_STYLE_COLOUR at 78, 7
    //    Unsupported item MECHANICAL_DESIGN_GEOMETRIC_PRESENTATION_REPRESENTATION at 96, 7
    //    Unsupported item APPLICATION_PROTOCOL_DEFINITION at 110, 7
    //    Unsupported item UNCERTAINTY_MEASURE_WITH_UNIT at 121, 8
    //    Unsupported item SPHERICAL_SURFACE at 137, 8
    //    Unsupported item FILL_AREA_STYLE at 151, 8
    //    Unsupported item PRODUCT_DEFINITION_SHAPE at 186, 8
    //    Unsupported item PRODUCT_DEFINITION_CONTEXT at 239, 8
    //    Unsupported item PRODUCT at 292, 8
    //    Unsupported item SHAPE_DEFINITION_REPRESENTATION at 456, 8
    //    Unsupported item CLOSED_SHELL at 510, 8
    //    Unsupported item ADVANCED_BREP_SHAPE_REPRESENTATION at 568, 8
    //    Unsupported item MANIFOLD_SOLID_BREP at 596, 8
    //    Unsupported item PRODUCT_DEFINITION at 932, 8
    //    Unsupported item PRODUCT_DEFINITION_FORMATION_WITH_SPECIFIED_SOURCE at 997, 8
    //    Unsupported item PRODUCT_RELATED_PRODUCT_CATEGORY at 1049, 9
    //    */
    //}
}
