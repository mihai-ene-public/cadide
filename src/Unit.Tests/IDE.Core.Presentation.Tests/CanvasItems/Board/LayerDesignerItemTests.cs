using IDE.Core.Designers;
using IDE.Core.Interfaces;
using IDE.Core.Types.Media;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;


namespace IDE.Core.Presentation.Tests.CanvasItems.Board
{
    public class LayerDesignerItemTests
    {
        [Theory]
        [InlineData(1, false, true, true, false)]
        [InlineData(2, false, true, true, true)]
        [InlineData(1, true, false, true, false)]
        [InlineData(2, true, false, false, false)]
        [InlineData(1, true, true, true, false)]
        [InlineData(2, true, true, false, false)]
        public void HandleLayer(int selectedLayerId, bool hideLayers, bool dimmLayers, bool expectedIsVisible, bool expectedIsMasked)
        {
            var boardModelMock = new Mock<ILayeredViewModel>();

            var boardModel = boardModelMock.Object;
            var layer = new LayerDesignerItem(boardModel)
            {
                LayerId = 1
            };

            var otherLayers = new List<LayerDesignerItem>
            {
                new LayerDesignerItem(boardModel)
                {
                    LayerId = 2
                },
                new LayerDesignerItem(boardModel)
                {
                    LayerId = 3
                },
            };

            var allLayers = new List<ILayerDesignerItem>();
            allLayers.Add(layer);
            allLayers.AddRange(otherLayers);

            var newG = new LayerGroupDesignerItem();
            newG.LoadLayers(allLayers);

            var selectedLayer = allLayers.FirstOrDefault(l => l.LayerId == selectedLayerId);

            boardModelMock.SetupGet(x => x.SelectedLayerGroup).Returns(newG);
            boardModelMock.SetupGet(x => x.SelectedLayer).Returns(selectedLayer);
            boardModelMock.SetupGet(x => x.HideUnselectedLayer).Returns(hideLayers);
            boardModelMock.SetupGet(x => x.MaskUnselectedLayer).Returns(dimmLayers);

            //act
            layer.HandleLayer();

            //assert
            Assert.Equal(expectedIsVisible, layer.IsVisible);
            Assert.Equal(expectedIsMasked, layer.IsMasked);
        }
    }
}
