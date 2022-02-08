using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDE.Core.Interfaces
{
    public interface IBoardDesigner : ILayeredViewModel
                                    , ICanvasWithHighlightedItems
                                    , IDocumentOverview
                                    , IFileBaseViewModel
    {
        IList<IBoardNetDesignerItem> NetList { get;  }

        IList<INetClassBaseItem> NetClasses { get; set; }

        IList<ILayerPairModel> DrillPairs { get; }

        IList<ILayerPairModel> LayerPairs { get; }


        //RegionBoardCanvasItem BoardOutline { get; }
        IRegionCanvasItem BoardOutline { get; set; }

        //we could have a build or compile command for an individual board

        IBoardBuildOptions BuildOptions { get; }

        //IList<AbstractBoardRule> Rules { get; }
        IList<IBoardRuleModel> Rules { get; }


        bool HasHighlightedNets { get; }

        IList<string> OutputFiles { get; }


        // void Compile();

        Task Build();

        void ChangeToCopperLayer(int layerNumber);

        void OnPropertyChanged(string propertyName);
    }

    public interface INetClassBaseItem
    {

    }

    public interface ILayerPairModel
    {
        ILayerDesignerItem LayerStart { get; set; }

        ILayerDesignerItem LayerEnd { get; set; }
    }

    public interface IBoardRuleModel
    {
        long Id { get; set; }

        string Name { get; set; }

        string Comment { get; set; }

        bool IsEnabled { get; set; }

        IGroupBoardRuleModel Parent { get; set; }

        bool IsPairedRule();

        bool RuleAppliesToItem(ISelectableItem item);

        bool RuleAppliesToItemsPair(ISelectableItem item1, ISelectableItem item2);

        bool CheckItem(ISelectableItem item, RuleCheckResult result);

        bool CheckItems(ISelectableItem item1, ISelectableItem item2, RuleCheckResult result);

        void Load(ILayeredViewModel doc);

        void LoadFromData(IBoardRuleData rule);
    }

    public interface IGroupBoardRuleModel : IBoardRuleModel
    {
        IList<IBoardRuleModel> Children { get; set; }

        void AddChild(IBoardRuleModel newRule);

        void RemoveChild(IBoardRuleModel child);
    }

    public interface ISchematicRuleModel
    {
        bool IsEnabled { get; }

        void LoadFromData(ISchematicRuleData rule);
    }
}
