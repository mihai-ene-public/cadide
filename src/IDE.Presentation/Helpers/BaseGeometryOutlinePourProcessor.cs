using IDE.Core.Interfaces.Geometries;
using IDE.Core.Interfaces;
using IDE.Core.Types.Media;
using IDE.Core.Common.Geometries;
using System.Collections;
using System.Collections.Generic;
using System;

namespace IDE.Core.Designers
{
    public class BaseGeometryOutlinePourProcessor
    {
        public BaseGeometryOutlinePourProcessor()
        {
            _geometryHelper = ServiceProvider.Resolve<IGeometryOutlineHelper>();
        }

        protected IGeometryOutlineHelper _geometryHelper;

        public IGeometryOutline GetGeometry(ISelectableItem thisItem, IBoardDesigner board)
        {
            if (thisItem == null)
                return null;

            var thisGeometry = GetGeometry(thisItem);
            if (thisGeometry == null)
                return null;

            var thermalItems = new List<ISelectableItem>();

            var restOfItems = new List<ItemWithClearance>();

            var toRemoveGeometries = new List<IGeometryOutline>();
            var toAddGeometries = new List<IGeometryOutline>();
            toAddGeometries.Add(thisGeometry);

            if (thisItem is IExcludeItems excludesItemsItem)
            {
                var excludedItems = excludesItemsItem.GetExcludedItems();
                foreach (ItemWithClearance excludedItem in excludedItems)
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
                                BuildThermalsForPad(excludedItem, thermalItems, gt.ThermalWidth);
                            }
                        }
                    }

                    restOfItems.Add(excludedItem);
                }
            }

            //----> clear all items from the poly

            //clear the rest of items
            foreach (var item in restOfItems)
            {
                RemoveGeometry(item, toRemoveGeometries);
            }

            //do clipping; extract geometries
            var finalGeometry = GeometryOutline.Combine(toAddGeometries, toRemoveGeometries, GeometryCombineMode.Exclude);

            toAddGeometries.Clear();
            toRemoveGeometries.Clear();

            toAddGeometries.Add(finalGeometry);

            //add thermals
            foreach (var thermalItem in thermalItems)
            {
                AddItemGeometry(thermalItem, toAddGeometries);//?
            }

            //do clipping; add thermals
            finalGeometry = GeometryOutline.Combine(toAddGeometries, null, GeometryCombineMode.Union);
           
            return finalGeometry;
        }


        protected virtual GeometryOutline GetGeometry(ISelectableItem item)
        {
            return null;
        }

        protected void RemoveGeometry(ItemWithClearance item, IList<IGeometryOutline> toRemove)
        {
            var geomItem = _geometryHelper.GetGeometry(item.CanvasItem, true, item.Clearance);
            if (geomItem != null)
                toRemove.Add(geomItem);
        }

        protected void AddGeometry(ItemWithClearance item, IList<IGeometryOutline> toAdd)
        {
            AddItemGeometry(item.CanvasItem, toAdd, item.Clearance);
        }

        private void AddItemGeometry(ISelectableItem item, IList<IGeometryOutline> toAdd, double clearance = 0)
        {
            var geomItem = _geometryHelper.GetGeometry(item, true, clearance);
            if (geomItem != null)
                toAdd.Add(geomItem);
        }

        protected void BuildThermalsForPad(ItemWithClearance padItem, List<ISelectableItem> thermalItems, double thermalWidth)
        {
            var pad = padItem.CanvasItem as IPadCanvasItem;
            var clearance = padItem.Clearance;

            var t = pad.GetTransform();
            var position = new XPoint(t.Value.OffsetX, t.Value.OffsetY);
            var rot = 0.0d;
            var placement = FootprintPlacement.Top;
            if (pad.ParentObject is IFootprintBoardCanvasItem fp)
                placement = fp.Placement;
            rot = GetWorldRotation(t, placement);

            //horizontal
            thermalItems.Add(new PadSmdCanvasItem
            {
                X = position.X,
                Y = position.Y,
                Width = pad.Width + 2 * clearance,
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
                Height = pad.Height + 2 * clearance,
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
            return GetRotationAngleFromMatrix(transform.Value);
        }

        double GetRotationAngleFromMatrix(XMatrix matrix)
        {
            var rads = -Math.Atan2(matrix.M21, matrix.M11);
            var rotAngle = rads * 180 / Math.PI;
            return rotAngle;
        }
    }
}
