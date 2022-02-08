using IDE.Core.Presentation.Placement;
using IDE.Core.Types.Media;
using System;
using System.Collections.Generic;
using System.Text;
using IDE.Core.Collision;
using IDE.Core.Interfaces;
using Moq;

namespace IDE.Core.Presentation.Tests.PlacementTools
{
    public abstract class PlacementToolTest
    {
        static PlacementToolTest()
        {
            var debounceMock = new Mock<IDebounceDispatcher>();

            ServiceProvider.RegisterResolver(t =>
            {
                if (t == typeof(IGeometryHelper))
                    return new GeometryHelper();
                if (t == typeof(IDebounceDispatcher))
                    return debounceMock.Object;

                throw new NotImplementedException();
            });
        }
        protected PlacementTool placementTool;

        // Simulates mouse move
        protected void MouseMove(double x, double y)
        {
            placementTool.PlacementMouseMove(new XPoint(x, y));
        }

        protected void MouseClick(double x, double y)
        {
            placementTool.PlacementMouseUp(new XPoint(x, y));
        }
    }
}
