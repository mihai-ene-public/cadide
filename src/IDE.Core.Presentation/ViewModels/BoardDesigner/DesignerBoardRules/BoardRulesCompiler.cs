using IDE.Core.Interfaces;
using IDE.Core.Interfaces.Compilers;
using IDE.Core.Presentation.Compilers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IDE.Core.Designers
{

    /// <summary>
    /// checks board rules rules
    /// </summary>
    public class BoardRulesCompiler : AbstractCompiler, IBoardRulesCompiler
    {
        //public BoardRulesManager(IBoardDesigner boardModel, string pFileName, string pProjectName)
        //{
        //    board = boardModel;
        //    fileName = pFileName;
        //    projectName = pProjectName;

        //}

        ///// <summary>
        ///// loads the rules in cache
        ///// </summary>
        ///// <param name="boardModel"></param>
        //public BoardRulesManager(IBoardDesigner boardModel)
        //    : this(boardModel, null, null)
        //{
        //    LoadRulesLinear(board.Rules, cachedRules);
        //}

        List<IBoardRuleModel> cachedRules = new List<IBoardRuleModel>();


        public async Task<CompilerResult> Compile(IBoardDesigner board)
        {
            var isValid = true;
            var errors = new List<IErrorMessage>();

            await Task.CompletedTask;

            try
            {
                var rules = new List<IBoardRuleModel>();
                LoadRulesLinear(board.Rules, rules);

                //var rulesCheckList = new List<BaseRuleCheck>();
                var itemsWithRulesList = new List<ItemRules>();
                var pairedItemsRules = new List<PairedItemsRule>();
                foreach (var canvasItem in board.CanvasModel.GetItems())
                {
                    //reset fault state
                    if (canvasItem is BoardCanvasItemViewModel boardItem)
                        boardItem.IsFaulty = false;

                    var rulesForThisItem = new List<IBoardRuleModel>();
                    foreach (var rule in rules)
                    {
                        if (rule.RuleAppliesToItem(canvasItem))
                        {
                            //rule with the same type will be overriden for single item rules
                            if (!rule.IsPairedRule())
                            {
                                var existingRule = rulesForThisItem.FirstOrDefault(r => r.GetType() == rule.GetType());
                                if (existingRule != null)
                                    rulesForThisItem.Remove(existingRule);
                            }

                            rulesForThisItem.Add(rule);
                        }
                    }

                    itemsWithRulesList.Add(new ItemRules
                    {
                        Item = canvasItem,
                        Rules = rulesForThisItem
                    });
                }

                //check single items rules
                foreach (var itemRule in itemsWithRulesList)
                {
                    // string message = null;
                    foreach (var rule in itemRule.Rules)
                    {
                        if (rule.IsPairedRule() == false)
                        {
                            var checkResult = new RuleCheckResult();
                            var res = rule.CheckItem(itemRule.Item, checkResult);

                            ApplyFaulty(itemRule.Item, !res);

                            if (!res)
                            {
                                errors.Add(GetViolation(checkResult, board));
                                isValid = false;
                            }
                        }
                    }
                }

                for (int i = 0; i < itemsWithRulesList.Count - 1; i++)
                {
                    var item1 = itemsWithRulesList[i];

                    for (int j = i + 1; j < itemsWithRulesList.Count; j++)
                    {
                        var item2 = itemsWithRulesList[j];

                        var pairedRulesForThisPair = new List<IBoardRuleModel>();

                        BuildPairRules(item1.Rules, item1.Item, item2.Item, pairedRulesForThisPair);
                        BuildPairRules(item2.Rules, item1.Item, item2.Item, pairedRulesForThisPair);

                        foreach (var rule in pairedRulesForThisPair)
                        {
                            if (rule.RuleAppliesToItemsPair(item1.Item, item2.Item))
                            {
                                var checkResult = new RuleCheckResult();
                                var res = rule.CheckItems(item1.Item, item2.Item, checkResult);

                                ApplyFaulty(item1.Item, !res);
                                ApplyFaulty(item2.Item, !res);

                                if (!res)
                                {
                                    errors.Add(GetViolation(checkResult, board));
                                    isValid = false;
                                }
                            }
                        }
                    }
                }
            }
            catch //(Exception ex)
            {
                //output.AppendLine("Error: " + ex.Message);
                //  RegisterViolation("Error: " + ex.Message);
            }

            return new CompilerResult
            {
                Success = isValid,
                Errors = errors
            };
        }

        void ApplyFaulty(ISelectableItem canvasItem, bool isFaulty)
        {
            var boardItem = canvasItem as BoardCanvasItemViewModel;
            if (boardItem != null)
            {
                boardItem.IsFaulty |= isFaulty;
            }
        }

        void BuildPairRules(List<IBoardRuleModel> rulesSource, ISelectableItem item1, ISelectableItem item2, List<IBoardRuleModel> outputPairRules)
        {
            foreach (var rule in rulesSource)
            {
                if (rule.IsPairedRule() && rule.RuleAppliesToItemsPair(item1, item2))
                {
                    //same rule was processed
                    if (outputPairRules.Contains(rule))
                        continue;

                    //remove the same type of rule defined for the same pair
                    var existingRule = outputPairRules.FirstOrDefault(r => r.GetType() == rule.GetType());
                    if (existingRule != null)
                        outputPairRules.Remove(existingRule);

                    outputPairRules.Add(rule);
                }
            }
        }

        void LoadRulesLinear(IList<IBoardRuleModel> sourceRules, IList<IBoardRuleModel> outputRules)
        {
            foreach (var rule in sourceRules.Where(r => r.IsEnabled))
            {
                if (rule is GroupRuleModel)
                    LoadRulesLinear(( rule as GroupRuleModel ).Children, outputRules);
                else
                    outputRules.Add(rule);
            }
        }


        private IErrorMessage GetViolation(RuleCheckResult result, IBoardDesigner board)
        {
            var projectName = board.ProjectNode.Name;
            return BuildErrorMessage(result.Message, projectName, board, result.Location?.Location);
        }

        /// <summary>
        /// returns the clearance in mm between 2 items
        /// </summary>
        public double GetElectricalClearance(IBoardDesigner board, ISelectableItem item1, ISelectableItem item2, double defaultClearance = 0.254)
        {
            if (cachedRules.Count == 0)
            {
                LoadRulesLinear(board.Rules, cachedRules);
            }
            var ec = cachedRules.OfType<ElectricalClearanceRuleModel>()
                          .Where(r => r.RuleAppliesToItemsPair(item1, item2))
                          .LastOrDefault();

            if (ec == null)
                return defaultClearance;

            return ec.MinClearance;
        }
    }

    /// <summary>
    /// Item, Rules
    /// </summary>
    class ItemRules
    {
        public ISelectableItem Item { get; set; }

        public List<IBoardRuleModel> Rules { get; set; }
    }

    /// <summary>
    /// Item1, Item2, PairedRule
    /// </summary>
    class PairedItemsRule
    {
        public ISelectableItem Item1 { get; set; }

        public ISelectableItem Item2 { get; set; }

        public AbstractBoardRule PairedRule { get; set; }
    }

}