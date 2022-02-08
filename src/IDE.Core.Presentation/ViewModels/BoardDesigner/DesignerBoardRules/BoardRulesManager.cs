using IDE.Core.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace IDE.Core.Designers
{

    /// <summary>
    /// checks board rules rules
    /// </summary>
    public class BoardRulesManager
    {
        public BoardRulesManager(IBoardDesigner boardModel, string pFileName, string pProjectName)
        {
            board = boardModel;
            fileName = pFileName;
            projectName = pProjectName;

        }

        /// <summary>
        /// loads the rules in cache
        /// </summary>
        /// <param name="boardModel"></param>
        public BoardRulesManager(IBoardDesigner boardModel)//, bool loadRulesInCache)
            : this(boardModel, null, null)
        {
            LoadRulesLinear(board.Rules, cachedRules);
        }

        IBoardDesigner board;
        internal string fileName;
        internal string projectName;

        List<IBoardRuleModel> cachedRules = new List<IBoardRuleModel>();

        public IFileBaseViewModel BoardFile { get { return board as IFileBaseViewModel; } }

        /*bool CheckRules()
        public bool CheckRules()
        {
            var isValid = true;

            try
            {
                var rules = new List<AbstractBoardRule>();
                LoadRulesLinear(board.Rules, rules);

                var rulesCheckList = new List<BaseRuleCheck>();

                //create item to rules list; we take the last rule of one type that applies to our item (because it overrides it
                foreach (var canvasItem in board.CanvasModel.GetItems())
                {
                    var rulesForThisItem = new List<AbstractBoardRule>();
                    foreach (var rule in rules)
                    {
                        if (rule.IsEnabled && rule.RuleAppliesToItem(canvasItem))
                        {
                            //rule with the same type will be overriden
                            var existingRule = rulesForThisItem.FirstOrDefault(r => r.GetType() == rule.GetType());
                            if (existingRule != null && !existingRule.IsPairedRule())
                                rulesForThisItem.Remove(existingRule);

                            rulesForThisItem.Add(rule);
                        }
                    }

                    //add rules check list
                    foreach (var rule in rulesForThisItem)
                    {
                        BaseRuleCheck ruleCheck = rulesCheckList.FirstOrDefault(r => r.Rule == rule);
                        if (ruleCheck == null)
                        {
                            if (rule.IsPairedRule())
                                ruleCheck = new PairItemRuleCheck(this) { Rule = rule };
                            else
                                ruleCheck = new SingleItemRuleCheck(this) { Rule = rule };

                            rulesCheckList.Add(ruleCheck);
                        }
                        ruleCheck.Items.Add((BaseCanvasItem)canvasItem);


                    }
                }


                //foreach checkItem; Check()
                foreach (var check in rulesCheckList)
                {
                    var ruleIsValid = check.CheckRule();
                    if (!ruleIsValid)
                        isValid = false;
                }

            }
            catch (Exception ex)
            {
                //output.AppendLine("Error: " + ex.Message);
            }

            return isValid;
        }
        */

        public bool CheckRules()
        {
            var isValid = true;

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
                                RegisterViolation(checkResult);
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
                                    RegisterViolation(checkResult);
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

            return isValid;
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
                    LoadRulesLinear((rule as GroupRuleModel).Children, outputRules);
                else
                    outputRules.Add(rule);
            }
        }


        protected void RegisterViolation(RuleCheckResult result)
        {
            BoardFile.AddCompileError(result.Message, fileName, projectName, result.Location?.Location);
        }

        /// <summary>
        /// returns the clearance in mm between 2 items
        /// </summary>
        /// <param name="item1"></param>
        /// <param name="item2"></param>
        /// <param name="defaultClearance"></param>
        /// <returns></returns>
        public double GetElectricalClearance(ISelectableItem item1, ISelectableItem item2, double defaultClearance = 0.254)
        {
            //var rules = new List<IBoardRuleModel>();
            //LoadRulesLinear(board.Rules, rules);

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

    //class BaseRuleCheck
    //{
    //    public BaseRuleCheck(
    //        BoardRulesManager boardRulesManager)
    //    {
    //        //output = o;
    //        //errors = e;
    //        BoardRulesManager = boardRulesManager;
    //    }

    //    //protected IOutput output;
    //    //protected ErrorsToolWindownViewModel errors;
    //    protected BoardRulesManager BoardRulesManager;

    //    public AbstractBoardRule Rule { get; set; }

    //    public List<BaseCanvasItem> Items { get; set; } = new List<BaseCanvasItem>();

    //    public virtual bool CheckRule()
    //    {
    //        return false;
    //    }

    //    protected void RegisterViolation(RuleCheckResult result)
    //    {
    //        BoardRulesManager.BoardFile.AddCompileError(result.Message, BoardRulesManager.fileName, BoardRulesManager.projectName, result.Location?.Location);
    //    }
    //}

    //class SingleItemRuleCheck : BaseRuleCheck
    //{
    //    public SingleItemRuleCheck(//IOutput o, ErrorsToolWindownViewModel e, 
    //        BoardRulesManager boardRulesManager)
    //        : base(boardRulesManager)
    //    {

    //    }

    //    public override bool CheckRule()
    //    {
    //        var hasErrors = false;
    //        foreach (var item in Items)
    //        {
    //            var checkResult = new RuleCheckResult();
    //            if (Rule.CheckItem(item, checkResult) == false)
    //            {
    //                hasErrors = true;
    //                RegisterViolation(checkResult);
    //            }
    //        }

    //        return !hasErrors;
    //    }
    //}

    //class PairItemRuleCheck : BaseRuleCheck
    //{
    //    public PairItemRuleCheck(BoardRulesManager boardRulesManager)
    //        : base(boardRulesManager)
    //    {

    //    }

    //    public override bool CheckRule()
    //    {
    //        var hasErrors = false;
    //        for (int i = 0; i < Items.Count - 1; i++)
    //        {
    //            var item1 = Items[i];

    //            for (int j = i + 1; j < Items.Count; j++)
    //            {
    //                var checkResult = new RuleCheckResult();
    //                var item2 = Items[j];
    //                if (!Rule.CheckItems(item1, item2, checkResult))
    //                {
    //                    hasErrors = true;
    //                    RegisterViolation(checkResult);
    //                }

    //            }
    //        }

    //        return !hasErrors;
    //    }
    //}
}