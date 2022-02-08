using IDE.Core.Interfaces;
using IDE.Core.Types.Media;

namespace IDE.Core.Presentation.PlacementRouting
{
    public class IgnoreObstaclesRoutingBehavior : RoutingBehavior
    {
        public IgnoreObstaclesRoutingBehavior(TrackRoutingMode routingMode) : base(routingMode)
        {

        }

        public override void PlacementMouseMove(XPoint mousePosition)
        {
            if (trackRoutingMode.PlacementTool == null)
                return;

            var status = trackRoutingMode.PlacementTool.PlacementStatus;
            switch (status)
            {
                case PlacementStatus.Ready:
                    currentSegment.Points[0] = mousePosition;
                    currentSegment.Points[1] = mousePosition;

                    currentSegment.Signal = null;
                    RemoveMiddlePoint();

                    //PointsChanged();
                    break;
                case PlacementStatus.Started:
                    {
                        switch (placementMode)
                        {
                            case TracePlacementMode.Single:
                                RemoveMiddlePoint();
                                currentSegment.Points[1] = mousePosition;

                               // currentSegment.OnPropertyChanged(nameof(currentSegment.Points));
                                break;

                            case TracePlacementMode.DiagonalDirect:
                                {
                                    EnsureMiddlePoint();
                                    //diagonal
                                    var diagonalPoint = GetDiagonalDirectPoint(mousePosition,currentSegment.Points[0], currentSegment.Points[2]);
                                    currentSegment.Points[1] = diagonalPoint;
                                    //direct
                                    currentSegment.Points[2] = mousePosition;

                                    //currentSegment.OnPropertyChanged(nameof(currentSegment.Points));
                                    break;
                                }

                            case TracePlacementMode.DirectDiagonal:
                                {
                                    EnsureMiddlePoint();
                                    var diagonalPoint = GetDirectDiagonalPoint(mousePosition, currentSegment.Points[0], currentSegment.Points[2]);
                                    //direct
                                    currentSegment.Points[1] = diagonalPoint;

                                    //diagonal
                                    currentSegment.Points[2] = mousePosition;

                                    //currentSegment.OnPropertyChanged(nameof(currentSegment.Points));
                                    break;
                                }
                        }
                        break;
                    }
            }

            PointsChanged();
        }
    }
}
