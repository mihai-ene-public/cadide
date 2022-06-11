using System;
using System.Collections.Generic;
using System.Linq;
using IDE.Core.Common.Geometries;
using IDE.Core.Interfaces;
using IDE.Core.Interfaces.Geometries;
using IDE.Core.Model.GlobalRepresentation.Primitives;
using IDE.Core.Types.Media;
using IDE.Core.Common;
using IDE.Core.Interfaces.Compilers;

namespace IDE.Core.Model.GlobalRepresentation
{
    public class GlobalPrimitivePourProcessor
    {
        public GlobalPrimitivePourProcessor()
        {
            _geometryHelper = ServiceProvider.Resolve<IGeometryOutlineHelper>();
            _boardRulesCompiler = ServiceProvider.Resolve<IBoardRulesCompiler>();
            _globalPrimitiveHelper = new GlobalPrimitiveHelper();
        }

        private readonly IGeometryOutlineHelper _geometryHelper;
        private readonly GlobalPrimitiveHelper _globalPrimitiveHelper;
        private readonly IBoardRulesCompiler _boardRulesCompiler;

        public GlobalPrimitive GetPrimitive(IPolygonBoardCanvasItem thisItem)
        {
            var t = thisItem.GetTransform();

            var fillPrimitive = new GlobalPolygonPrimitive
            {
                Points = thisItem.PolygonPoints.Select(p => t.Transform(p)).ToList(),
                IsFilled = true
            };

            var pouredPolyPrimitive = new GlobalPouredPolygonPrimitive
            {
                FillPrimitive = fillPrimitive,
            };

            var board = ( (dynamic)thisItem ).LayerDocument as IBoardDesigner;
            var excludedItems = GetExcludedItems(thisItem, board);

            var thermalCandidates = new List<GeometryPair>();
            var restOfItems = new List<ItemWithClearance>();

            var toRemoveGeometries = new List<IGeometryOutline>();
            var toRemoveItems = new List<GeometryPair>();
            var toAddGeometries = new List<IGeometryOutline>();

            toAddGeometries.Add(new PolygonGeometryOutline(thisItem.PolygonPoints));

            foreach (var excludedItem in excludedItems)
            {
                var clearanceItem = excludedItem.CanvasItem;

                if (clearanceItem is IPadCanvasItem pad)
                {
                    var polySignal = thisItem as ISignalPrimitiveCanvasItem;
                    if (polySignal.Signal != null && pad.Signal != null &&
                        polySignal.Signal.Name == pad.Signal.Name)
                    {
                        if (thisItem is IGenerateThermals gt && gt.GenerateThermals)
                        {
                            BuildThermalsForPad(excludedItem, thermalCandidates, gt.ThermalWidth);
                        }
                    }
                }

                restOfItems.Add(excludedItem);
            }

            //----> clear all items from the poly

            //clear the rest of items
            foreach (var item in restOfItems)
            {
                RemoveGeometry(item, toRemoveItems);
            }

            //do clipping; extract geometries
            toRemoveGeometries.AddRange(toRemoveItems.Select(p => p.Geometry));
            var finalGeometry = GeometryOutline.Combine(toAddGeometries, toRemoveGeometries, GeometryCombineMode.Exclude);

            toAddGeometries.Clear();
            toRemoveGeometries.Clear();

            var thermals = new List<GeometryPair>();
            //add thermals
            foreach (var tg in thermalCandidates)
            {
                AddThermalGeometry(tg, finalGeometry, thermals);
            }

            toAddGeometries.Add(finalGeometry);
            toAddGeometries.AddRange(thermals.Select(t => t.Geometry));

            //do clipping; add thermals
            finalGeometry = GeometryOutline.Combine(toAddGeometries, null, GeometryCombineMode.Union);

            //var removePrimitives = new List<GlobalPrimitive>();
            //var thermalPrimitives = new List<GlobalPrimitive>();
            //foreach (var )

            pouredPolyPrimitive.FinalGeometry = finalGeometry;
            pouredPolyPrimitive.RemovePrimitives = toRemoveItems.Select(p => p.GlobalPrimitive).ToList();
            pouredPolyPrimitive.Thermals = thermals.Select(t => t.GlobalPrimitive).ToList();

            return pouredPolyPrimitive;
        }

        private IList<ItemWithClearance> GetExcludedItems(IPolygonBoardCanvasItem polyItem, IBoardDesigner board)
        {
            var returnItems = new List<ItemWithClearance>();

            //var isEditing = board != null && board.State == DocumentState.IsEditing;

            if (polyItem.IsPlaced
                && polyItem.IsFilled
                && polyItem.PolygonType != PolygonType.Keepout
                //&& isEditing
                )
            {
                if (polyItem.Layer != null
                    && ( polyItem.Layer.LayerType == LayerType.Signal
                        || polyItem.Layer.LayerType == LayerType.Plane ))
                {
                    var defaultClearance = 0.254d;//mm

                    var canvasModel = board.CanvasModel;

                    var canvasItems = (List<ISelectableItem>)canvasModel.GetItems();

                    var footprints = canvasModel.GetItems().OfType<IFootprintBoardCanvasItem>().ToList();
                    var footprintItems = ( from fp in footprints
                                           from p in fp.Items.OfType<ISignalPrimitiveCanvasItem>().Cast<ISingleLayerBoardCanvasItem>()
                                           where p.ShouldBeOnLayer(polyItem.Layer)//p.Layer == Layer
                                           select p );

                    //todo: that is on layer (we could store it on Layer.Items)
                    var vias = canvasItems.OfType<IViaCanvasItem>();

                    var items = new List<ISelectableItem>();
                    items.AddRange(polyItem.Layer.Items.Where(c => c != this && c is ISignalPrimitiveCanvasItem));
                    items.AddRange(footprintItems);
                    items.AddRange(vias);

                    //var items = polyItem.Layer.Items.Where(c => c != this && c is ISignalPrimitiveCanvasItem)
                    //                   .Select(c => c)
                    //                   .Union(footprintItems)
                    //                   .Union(vias);

                    var trackItems = items.OfType<ITrackBoardCanvasItem>();
                    var allItemsExceptTracks = items.Where(p => !( p is ITrackBoardCanvasItem ));

                    var thisPolyRect = polyItem.GetBoundingRectangle();

                    //tracks
                    foreach (var track in trackItems)
                    {
                        if (polyItem.IsOnSameSignalWith(track))
                            continue;

                        var itemRect = track.GetBoundingRectangle();
                        if (thisPolyRect.Intersects(itemRect) && _geometryHelper.Intersects(polyItem, track))
                        {
                            var clearance = _boardRulesCompiler.GetElectricalClearance(board, polyItem, track, defaultClearance);
                            var inflatedItem = new ItemWithClearance(track, clearance);//GetItemWithClearance(track, clearance);

                            returnItems.Add(inflatedItem);
                        }
                    }

                    //other than tracks
                    foreach (var item in allItemsExceptTracks)
                    {
                        var itemRect = item.GetBoundingRectangle();
                        var isPad = false;
                        if (item is IPadCanvasItem pad)
                        {
                            isPad = true;
                            var t = item.GetTransform();
                            if (t != null)
                                itemRect.Transform(t.Value);
                        }

                        //isPad is a small hack; it doesn't create a proper tranform for the rectangle for a pad
                        if (( thisPolyRect.Intersects(itemRect) || isPad ) && _geometryHelper.Intersects(polyItem, item))
                        {
                            if (item is ISignalPrimitiveCanvasItem otherSignalItem)
                            {
                                if (item is IViaCanvasItem
                                    && polyItem.Signal != null
                                    && otherSignalItem.Signal != null
                                    && polyItem.Signal.Name == otherSignalItem.Signal.Name)
                                    continue;
                            }

                            //we cut from this poly if other poly draw order is lower
                            if (item is IPolygonBoardCanvasItem otherPoly
                                && otherPoly.PolygonType != PolygonType.Keepout
                                && polyItem.DrawOrder <= otherPoly.DrawOrder)
                            {
                                continue;
                            }

                            var clearance = _boardRulesCompiler.GetElectricalClearance(board, polyItem, item, defaultClearance);
                            // var inflatedItem = item;
                            var inflatedItem = new ItemWithClearance(item, clearance);

                            returnItems.Add(inflatedItem);
                        }
                    }
                }
            }

            return returnItems;
        }

        private void RemoveGeometry(ItemWithClearance item, IList<GeometryPair> toRemoveGeometries)
        {
            var geomItem = _geometryHelper.GetGeometry(item.CanvasItem, true, item.Clearance);
            if (geomItem != null)
            {
                var gp = _globalPrimitiveHelper.GetGlobalPrimitive(item.CanvasItem);
                gp.AddClearance(item.Clearance);

                toRemoveGeometries.Add(new GeometryPair
                {
                    Geometry = geomItem,
                    GlobalPrimitive = gp
                });
            }
        }


        private void AddThermalGeometry(GeometryPair geomItem, IGeometryOutline finalGeometry, IList<GeometryPair> thermals)
        {
            // var geomItem = _geometryHelper.GetGeometry(item, true);
            if (geomItem == null || geomItem.Geometry == null)
                return;

            var subj = new List<IGeometryOutline>();
            var clips = new List<IGeometryOutline>();
            subj.Add(finalGeometry);
            clips.Add(geomItem.Geometry);
            var intersection = (GeometryOutlines)GeometryOutline.Combine(subj, clips, GeometryCombineMode.Intersect);

            if (intersection.Outlines.Count == 0)
                return;

            thermals.Add(geomItem);
        }

        private void BuildThermalsForPad(ItemWithClearance padItem, List<GeometryPair> thermals, double thermalWidth)
        {
            var pad = padItem.CanvasItem as IPadCanvasItem;
            var clearance = padItem.Clearance;

            var t = (XTransformGroup)pad.GetTransform();
            var position = new XPoint(t.Value.OffsetX, t.Value.OffsetY);
            var rot = 0.0d;
            var placement = FootprintPlacement.Top;
            if (pad.ParentObject is IFootprintBoardCanvasItem fp)
                placement = fp.Placement;
            rot = GetWorldRotation(t, placement);

            //we add some tolerance so that when thermal geometry intersects with the final geometry will create some outlines
            const double tolerance = 0.01d;

            //east
            var posEast = new XPoint(0.5 * ( 0.5 * pad.Width + clearance ), 0);
            posEast = t.Transform(posEast);
            //thermalGeometries.Add(new PadSmdCanvasItem
            //{
            //    X = posEast.X,
            //    Y = posEast.Y,
            //    Width = 0.5 * pad.Width + clearance + tolerance,
            //    Height = thermalWidth,
            //    Rot = rot,
            //    CornerRadius = 0
            //});
            thermals.Add(GetThermalGeometry(posEast, rot, 0.5 * pad.Width + clearance + tolerance, thermalWidth));

            //west
            var posWest = new XPoint(-0.5 * ( 0.5 * pad.Width + clearance ), 0);
            posWest = t.Transform(posWest);
            //thermalGeometries.Add(new PadSmdCanvasItem
            //{
            //    X = posWest.X,
            //    Y = posWest.Y,
            //    Width = 0.5 * pad.Width + clearance + tolerance,
            //    Height = thermalWidth,
            //    Rot = rot,
            //    CornerRadius = 0
            //});
            thermals.Add(GetThermalGeometry(posWest, rot, 0.5 * pad.Width + clearance + tolerance, thermalWidth));

            //north
            var posNorth = new XPoint(0, -0.5 * ( 0.5 * pad.Height + clearance ));
            posNorth = t.Transform(posNorth);
            //thermalGeometries.Add(new PadSmdCanvasItem
            //{
            //    X = posNorth.X,
            //    Y = posNorth.Y,
            //    Width = thermalWidth,
            //    Height = 0.5 * pad.Height + clearance + tolerance,
            //    Rot = rot,
            //    CornerRadius = 0
            //});
            thermals.Add(GetThermalGeometry(posNorth, rot, thermalWidth, 0.5 * pad.Height + clearance + tolerance));

            //south
            var posSouth = new XPoint(0, 0.5 * ( 0.5 * pad.Height + clearance ));
            posSouth = t.Transform(posSouth);
            //thermalGeometries.Add(new PadSmdCanvasItem
            //{
            //    X = posSouth.X,
            //    Y = posSouth.Y,
            //    Width = thermalWidth,
            //    Height = 0.5 * pad.Height + clearance + tolerance,
            //    Rot = rot,
            //    CornerRadius = 0
            //});
            thermals.Add(GetThermalGeometry(posSouth, rot, thermalWidth, 0.5 * pad.Height + clearance + tolerance));
        }

        private GeometryPair GetThermalGeometry(XPoint pos, double rot, double width, double height)
        {
            var transform = new XTransformGroup();
            transform.Children.Add(new XRotateTransform(rot));
            transform.Children.Add(new XTranslateTransform(pos.X, pos.Y));

            var geometry = new RectangleGeometryOutline(0, 0, width, height);
            geometry.Transform = transform;

            return new GeometryPair
            {
                Geometry = geometry,
                GlobalPrimitive = new GlobalRectanglePrimitive
                {
                    X = pos.X,
                    Y = pos.Y,
                    Width = width,
                    Height = height,
                    Rot = rot
                }
            };
        }

        double GetWorldRotation(XTransform transform, FootprintPlacement placement)
        {
            var rotAngle = GetRotationTransform(transform);
            var myRot = rotAngle;

            if (placement == FootprintPlacement.Bottom)
            {
                myRot = 180.0d - myRot;
            }

            return myRot;
        }

        double GetRotationTransform(XTransform transform)
        {
            return GetRotationAngleFromMatrix(transform.Value);
        }

        double GetRotationAngleFromMatrix(XMatrix matrix)
        {
            var rads = -Math.Atan2(matrix.M21, matrix.M11);
            var rotAngle = rads * 180 / Math.PI;
            return rotAngle;
        }

    }

    internal class GeometryPair
    {
        //public ICanvasItem CanvasItem { get; set; }
        public IGeometryOutline Geometry { get; set; }

        public GlobalPrimitive GlobalPrimitive { get; set; }
    }
}
