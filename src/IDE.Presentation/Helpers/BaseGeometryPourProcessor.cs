using IDE.Core.Converters;
using IDE.Core.Interfaces;
using IDE.Core.Types.Media;
using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace IDE.Core.Designers
{
    public class BaseGeometryPourProcessor
    {
        public BaseGeometryPourProcessor()
        {
            GeometryHelper = ServiceProvider.Resolve<IGeometryHelper>();
        }

        protected IGeometryHelper GeometryHelper;

        protected double tolerance = 5e-3;

        public object GetGeometry(ISelectableItem thisItem, IBoardDesigner board)
        {
            if (thisItem == null)
                return null;



            Geometry finalGeometry = GetGeometry(thisItem);
            if (finalGeometry == null)
                return null;

            var thermalItems = new List<ISelectableItem>();

            var padsOfThisSignal = new List<ItemWithClearance>();
            var restOfItems = new List<ItemWithClearance>();

            if (thisItem is IExcludeItems excludesItemsItem)
            {
                var excludedItems = excludesItemsItem.GetExcludedItems();
                foreach (ItemWithClearance inflatedItem in excludedItems)
                {
                    var clearanceItem = inflatedItem.CanvasItem;

                    if (clearanceItem is IPadCanvasItem pad)
                    {
                        var polySignal = thisItem as ISignalPrimitiveCanvasItem;
                        if (polySignal.Signal != null && pad.Signal != null &&
                            polySignal.Signal.Name == pad.Signal.Name)
                        {
                            padsOfThisSignal.Add(inflatedItem);
                            if (thisItem is IGenerateThermals gt && gt.GenerateThermals)
                            {
                                BuildThermalsForPad(inflatedItem, thermalItems, gt.ThermalWidth);
                            }
                        }
                        else
                        {
                            restOfItems.Add(inflatedItem);
                        }
                    }
                    else
                    {
                        restOfItems.Add(inflatedItem);
                    }
                }
            }

            //remove pads
            foreach (var inflatedPad in padsOfThisSignal)
            {
                var clearanceItem = inflatedPad.CanvasItem;
                finalGeometry = RemoveGeometry(clearanceItem, finalGeometry);
            }

            //add thermals
            foreach (var thermalItem in thermalItems)
            {
                finalGeometry = AddGeometry(thermalItem, finalGeometry);
            }

            //clear the rest of items
            foreach (var item in restOfItems)
            {
                finalGeometry = RemoveGeometry(item.CanvasItem, finalGeometry);
            }

            var finalTran = new TransformGroup();
            var ss = MilimetersToDpiHelper.MillimetersToDpiTransformFactor;
            finalTran.Children.Add(new ScaleTransform(ss, ss));
            finalGeometry.Transform = finalTran;

            finalGeometry.Freeze();
            return finalGeometry;
        }


        protected virtual Geometry GetGeometry(ISelectableItem item)
        {
            return Geometry.Empty;
        }

        protected Geometry RemoveGeometry(ISelectableItem clearanceItem, Geometry finalGeometry)
        {
            var geomItem = (Geometry)GeometryHelper.GetGeometry(clearanceItem, tolerance);

            var newGeometry = finalGeometry;

            if (!geomItem.IsEmpty())
            {
                var t = clearanceItem.GetTransform();

                geomItem.Transform = t.ToMatrixTransform();

                return Geometry.Combine(finalGeometry, geomItem, GeometryCombineMode.Exclude, null, tolerance, ToleranceType.Absolute);
            }

            return newGeometry;
        }

        protected Geometry AddGeometry(ISelectableItem canvasItem, Geometry finalGeometry)
        {
            var geomItem = (Geometry)GeometryHelper.GetGeometry(canvasItem, tolerance);

            var newGeometry = finalGeometry;

            if (!geomItem.IsEmpty())
            {
                var t = canvasItem.GetTransform();// as TransformGroup;

                geomItem.Transform = t.ToMatrixTransform();

                //if (GeometryHelper.Intersects(geomItem, finalGeometry))
                {
                    newGeometry = Geometry.Combine(finalGeometry, geomItem, GeometryCombineMode.Union, null, tolerance, ToleranceType.Absolute);
                }
            }

            return newGeometry;
        }

        protected void BuildThermalsForPad(ItemWithClearance padInflated, List<ISelectableItem> thermalItems, double thermalWidth)
        {
            //pad is already inflated with the clearance
            //var thermalWidth = thisPoly.ThermalWidth; //;//in mm (6 mil)

            var pad = padInflated.CanvasItem as IPadCanvasItem;
            var clearance = padInflated.Clearance;

            var t = pad.GetTransform();
            var position = new XPoint(t.Value.OffsetX, t.Value.OffsetY);
            var placement = FootprintPlacement.Top;
            if (pad.ParentObject is IFootprintBoardCanvasItem fp)
                placement = fp.Placement;
            var rot = GetWorldRotation(t, placement);

            //horizontal
            thermalItems.Add(new PadSmdCanvasItem
            {
                X = position.X,
                Y = position.Y,
                Width = pad.Width,
                Height = thermalWidth,
                Rot = rot,
                CornerRadius = 0
            });

            //vertical
            thermalItems.Add(new PadSmdCanvasItem
            {
                X = position.X,
                Y = position.Y,
                Width = thermalWidth,
                Height = pad.Height,
                Rot = rot,
                CornerRadius = 0
            });
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
            return Geometry2DHelper.GetRotationAngleFromMatrix(transform.Value);
        }
    }
}
