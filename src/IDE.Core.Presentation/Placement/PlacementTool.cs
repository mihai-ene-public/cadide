using IDE.Core.Designers;
using IDE.Core.Interfaces;
using IDE.Core.Types.Media;
using IDE.Core.ViewModels;
using System;
using System.Collections.Generic;

namespace IDE.Core.Presentation.Placement
{
    public class PlacementTool : IPlacementTool//<T> where T: ISelectableItem
    {

        public PlacementTool()
        {
            GeometryHelper = ServiceProvider.Resolve<IGeometryHelper>();
        }

        protected IGeometryHelper GeometryHelper;

        /// <summary>
        /// canvasItem that we are currently placing
        /// </summary>
        protected ISelectableItem canvasItem;

        public ISelectableItem CanvasItem
        {
            get { return canvasItem; }
            set { canvasItem = value; }
        }

        /// <summary>
        /// in the case we show a dialog (addding a part in schematic) the selectedItem from the dialog is stored here
        /// <para>May be NULL</para>
        /// </summary>
        protected object dialogSelectedItem;

        public IDrawingViewModel CanvasModel { get; set; }

        public PlacementStatus PlacementStatus { get; set; }

        protected ISolutionProjectNodeModel CurrentProject => CanvasModel.FileDocument.ProjectNode;

        public virtual void PlacementMouseMove(XPoint mousePosition)
        {

        }

        public virtual void PlacementMouseUp(XPoint mousePosition)
        {

        }

        public virtual bool Show()
        {
            return true;
        }

        public virtual void ChangeMode()//TAB
        {

        }

        public virtual void CyclePlacement()//SPACE
        {
            if (canvasItem != null)
                canvasItem.Rotate();
        }

        public virtual void StartPlacement(Type canvasItemType)
        {
            CanvasModel.ClearSelectedItems();
            CanvasModel.CancelPlacement();

            var c = (ISelectableItem)Activator.CreateInstance(canvasItemType);

            StartPlacementInternal(c);
        }

        public virtual void StartPlacement(ISelectableItem placementItem)
        {
            CanvasModel.ClearSelectedItems();
            CanvasModel.CancelPlacement();

            StartPlacementInternal(placementItem);
        }

        public virtual void StartPlacement()
        {
            CanvasModel.CancelPlacement();

            StartPlacementInternal(null);
        }

        void StartPlacementInternal(ISelectableItem placementItem)
        {
            canvasItem = placementItem;
            CanvasModel.PlacementTool = this;
            PlacementStatus = PlacementStatus.Ready;

            SetupCanvasItem();

            CanvasModel.AddItem(canvasItem);


            //show properties for the current placing item UpdateSelection()?
            var pw = ServiceProvider.GetToolWindow<PropertiesToolWindowViewModel>(false);
            if (pw != null)
            {
                pw.SelectedObject = canvasItem;
                pw.IsVisible = true;
                pw.IsActive = true;
            }
        }

        ILayeredViewModel GetLayeredDocument()
        {
            return CanvasModel.FileDocument as ILayeredViewModel;
        }

        public virtual void SetupCanvasItem()
        {



            if (canvasItem is SingleLayerBoardCanvasItem boardItem)
            {
                var layeredDoc = GetLayeredDocument();
                if (layeredDoc != null)
                {
                    boardItem.LayerDocument = layeredDoc;
                    if (layeredDoc.SelectedLayer != null)
                        boardItem.LayerId = layeredDoc.SelectedLayer.LayerId;
                    boardItem.LoadLayers();
                    boardItem.AssignLayerForced(boardItem.Layer);
                }
                //boardItem.Layer = layersWindow.SelectedLayer;
            }
            else if (canvasItem is BoardCanvasItemViewModel brdItem)
            {
                var layeredDoc = GetLayeredDocument();
                if (layeredDoc != null)
                {
                    brdItem.LayerDocument = layeredDoc;
                    brdItem.LoadLayers();
                }
            }

            //else if (canvasItem is MultiLayerBoardCanvasItem multiLayerItem)
            //{
            //    //var boardItem = canvasItem as MultiLayerBoardCanvasItem;
            //    var layeredDoc = GetLayeredDocument();
            //    if (layeredDoc != null)
            //    {
            //        multiLayerItem.LayerDocument = layeredDoc;
            //        multiLayerItem.LoadLayers();
            //    }

            //}
        }

        public static PlacementTool CreateTool(Type canvasItemType, Type placementToolType = null)
        {
            if (placementToolType != null)
            {
                var placementTool = (PlacementTool)Activator.CreateInstance(placementToolType);
                return placementTool;
            }

            var mapping = GetMapping();

            if (mapping.ContainsKey(canvasItemType))
            {
                placementToolType = mapping[canvasItemType];
                var placementTool = (PlacementTool)Activator.CreateInstance(placementToolType);
                return placementTool;
            }

            throw new NotSupportedException();
        }

        private static Dictionary<Type, Type> GetMapping()
        {
            var mapping = new Dictionary<Type, Type>()
            {
                //sphere
                 { typeof(SphereMeshItem),typeof(SphereMeshItemPlacementTool)},

                //box
                 { typeof(BoxMeshItem),typeof(BoxMeshItemPlacementTool)},

                 //text 3d
                 { typeof(TextMeshItem),typeof(TextMeshItemPlacementTool)},

                //cone
                 { typeof(ConeMeshItem),typeof(ConeMeshItemPlacementTool)},

                //cylinder
                 { typeof(CylinderMeshItem),typeof(CylinderMeshItemPlacementTool)},

                //ellipsoid
                 { typeof(EllipsoidMeshItem),typeof(EllipsoidMeshItemPlacementTool)},

                //extruded poly
                 { typeof(ExtrudedPolyMeshItem),typeof(ExtrudedPolyMeshItemPlacementTool)},


                //group
                { typeof(VolatileGroup3DCanvasItem),typeof(VolatileGroup3DPlacementTool)},
                { typeof(VolatileGroupCanvasItem),typeof(VolatileGroupPlacementTool)},

                //line
                { typeof(LineSchematicCanvasItem),typeof(LinePlacementTool)},
                { typeof(LineBoardCanvasItem),typeof(LinePlacementTool)},

                //arc
                { typeof(ArcCanvasItem),typeof(ArcPlacementTool)},
                { typeof(ArcBoardCanvasItem),typeof(ArcPlacementTool)},

                //text
                { typeof(TextCanvasItem),typeof(TextPlacementTool)},
                { typeof(TextBoardCanvasItem),typeof(TextPlacementTool)},
                { typeof(TextSingleLineBoardCanvasItem),typeof(TextPlacementTool)},

                //rect
                { typeof(RectangleCanvasItem),typeof(RectanglePlacementTool)},
                { typeof(RectangleBoardCanvasItem),typeof(RectanglePlacementTool)},

                //poly
                { typeof(PolygonCanvasItem),typeof(PolygonPlacementTool)},
                { typeof(PolygonBoardCanvasItem),typeof(PolygonPlacementTool)},

                //circle
                { typeof(CircleCanvasItem),typeof(CirclePlacementTool)},
                { typeof(CircleBoardCanvasItem),typeof(CirclePlacementTool)},

                //ellipse
                { typeof(EllipseCanvasItem),typeof(EllipsePlacementTool)},

                //image
                { typeof(ImageCanvasItem),typeof(ImagePlacementTool)},

                //pin
                { typeof(PinCanvasItem),typeof(PinPlacementTool)},

                //hole
                { typeof(HoleCanvasItem),typeof(HolePlacementTool)},

                //pad/smd
                { typeof(PadThtCanvasItem),typeof(PadPlacementTool)},
                { typeof(PadSmdCanvasItem),typeof(PadPlacementTool)},

                //sch net wire
                { typeof(NetWireCanvasItem),typeof(NetWirePlacementTool)},

                //sch bus wire
                { typeof(BusWireCanvasItem),typeof(BusWirePlacementTool)},

                //junction
                { typeof(JunctionCanvasItem),typeof(JunctionPlacementTool)},

                //net label
                { typeof(NetLabelCanvasItem),typeof(NetLabelPlacementTool)},

                //bus label
                { typeof(BusLabelCanvasItem),typeof(BusLabelPlacementTool)},

                //part
                { typeof(SchematicSymbolCanvasItem),typeof(PartPlacementTool)},

                //board via
                { typeof(ViaCanvasItem),typeof(ViaPlacementTool)},

                 ////trace
                // { typeof(TraceBoardCanvasItem),typeof(TracePlacementTool)},

                 //track (new routing)

            };
            return mapping;
        }
    }
}
