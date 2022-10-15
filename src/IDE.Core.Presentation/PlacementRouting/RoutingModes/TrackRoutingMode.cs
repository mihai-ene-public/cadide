using IDE.Core.Designers;
using IDE.Core.Interfaces;
using IDE.Core.Presentation.Placement;
using IDE.Core.Types.Media;
using System.Collections.Generic;

namespace IDE.Core.Presentation.PlacementRouting
{
    public class TrackRoutingMode
    {

        public TrackRoutingMode()
        {
        }


        protected BoardObstacleProvider obstaclesProvider;

        protected IDrawingViewModel canvasModel;

        public PlacementTool PlacementTool => (PlacementTool)canvasModel.PlacementTool;

        public PlacementStatus PlacementStatus
        {
            get { return PlacementTool.PlacementStatus; }
            set { PlacementTool.PlacementStatus = value; }
        }

        public IDrawingViewModel CanvasModel
        {
            get { return canvasModel; }
            set { canvasModel = value; }
        }


        protected/*static*/ RoutingBehavior currentRoutingBehavior;
        protected/*static*/ List<RoutingBehavior> supportedRoutingBehaviors = new List<RoutingBehavior>();

        public virtual void PlacementMouseMove(XPoint mousePosition)
        {
            //updateStartItem(true)

            var mp = canvasModel.SnapToGrid(mousePosition);
            currentRoutingBehavior.PlacementMouseMove(mp);
        }

        public virtual void PlacementMouseUp(XPoint mousePosition)
        {
            //updateStartItem(ignorePads: false)
            //performRouting()

            //var mp = PlacementTool.CanvasModel.SnapToGrid(mousePosition);
            //currentRoutingBehavior.PlacementMouseUp(mp);
        }

        //void updateStartItem()
        //  updates and sets the startPoint of routing with options to snap to an item from the HitTest under the mousePos

        void EnsureRoutingBehaviors()
        {
            if (supportedRoutingBehaviors.Count == 0)
            {
                //supportedRoutingBehaviors.Add(new StopAtObstaclesRoutingBehavior(this));
                supportedRoutingBehaviors.Add(new IgnoreObstaclesRoutingBehavior(this));

                //these below are left to be implemented later
                //supportedRoutingBehaviors.Add(new WalkArroundObstaclesRoutingBehavior(this));
                //supportedRoutingBehaviors.Add(new PushObstaclesRoutingBehavior(this));
            }
            else
            {
                foreach (var behavior in supportedRoutingBehaviors)
                    behavior.TrackRoutingMode = this;
            }
            EnsureCurrentRoutingBehavior();
        }

        void EnsureCurrentRoutingBehavior()
        {
            if (currentRoutingBehavior == null && supportedRoutingBehaviors.Count > 0)
                currentRoutingBehavior = supportedRoutingBehaviors[0];

            currentRoutingBehavior.CurrentSegment = (TrackBoardCanvasItem)PlacementTool.CanvasItem;
            currentRoutingBehavior.TrackRoutingMode = this;
        }

        public void Start()
        {
            EnsureRoutingBehaviors();

            if (obstaclesProvider == null)
                obstaclesProvider = new BoardObstacleProvider(canvasModel);
            obstaclesProvider.BuildObstacles();
        }

        //public List<ObstacleItem> GetObstaclesInRectangle(XRect rect)
        //{
        //    return obstaclesProvider.GetObstaclesInRectangle(rect);
        //}

        public virtual void ChangeMode()
        {
            var index = 1 + supportedRoutingBehaviors.IndexOf(currentRoutingBehavior);
            index = index % supportedRoutingBehaviors.Count;
            currentRoutingBehavior = supportedRoutingBehaviors[index];


            //currentRoutingBehavior.CurrentSegment = (TrackBoardCanvasItem)PlacementTool.CanvasItem;
            EnsureCurrentRoutingBehavior();
        }

        public virtual void CyclePlacement()
        {
            currentRoutingBehavior?.CyclePlacementMode();
        }


    }
}
