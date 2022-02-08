using IDE.Core.Interfaces;
using IDE.Core.Storage;
using IDE.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IDE.Core.Designers
{
    public class SchematicRulesManager
    {


        public SchematicRulesManager(ISchematicDesigner schModel, string pFileName, string pProjectName)
        {
            _schModel = schModel;
            _pFileName = pFileName;
            _pProjectName = pProjectName;
        }

        private readonly ISchematicDesigner _schModel;
        private readonly string _pFileName;
        private readonly string _pProjectName;

        IFileBaseViewModel innerFile { get { return _schModel as IFileBaseViewModel; } }

        public bool CheckRules()
        {
            var isValid = true;

            try
            {


                var rules = new List<ISchematicRuleModel>();
                LoadRulesLinear(_schModel.Rules, rules);

                var nets = GetNets();

                var items = new List<ISelectableItem>();
                var pairedItems = new List<Tuple<ISelectableItem, ISelectableItem>>();

                foreach (var net in nets)
                {
                    var pins = net.NetItems.OfType<PinCanvasItem>().ToList();
                    items.AddRange(pins);

                    //no pairs
                    if (pins.Count < 2)
                        continue;

                    //create pairs of pins
                    for (int i = 0; i < pins.Count - 1; i++)
                    {
                        for (int j = i + 1; j < pins.Count; j++)
                        {
                            pairedItems.Add(new Tuple<ISelectableItem, ISelectableItem>(pins[i], pins[j]));
                        }
                    }
                }

                foreach (var rule in rules)
                {
                    var ruleViolations = new List<SchematicRuleCheckResult>();

                    switch (rule)
                    {
                        case NetSchematicRule netRule:
                            {
                                foreach (var net in nets)
                                {
                                    var checkResult = netRule.CheckNet(net);
                                    if (!checkResult.IsValid)
                                        ruleViolations.Add(checkResult);
                                }

                                break;
                            }

                        case SingleItemSchematicRule singleItemSchematicRule:
                            {
                                foreach (var item in items)
                                {
                                    var checkResult = singleItemSchematicRule.CheckItem(item);
                                    if (!checkResult.IsValid)
                                        ruleViolations.Add(checkResult);
                                }

                                break;
                            }

                        case PairedItemsSchematicRule pairedItemsSchematicRule:
                            {
                                foreach (var pair in pairedItems)
                                {
                                    var checkResult = pairedItemsSchematicRule.CheckItems(pair.Item1, pair.Item2);
                                    if (!checkResult.IsValid)
                                        ruleViolations.Add(checkResult);
                                }

                                break;
                            }
                    }

                    if (ruleViolations.Count > 0)
                    {
                        isValid = false;

                        foreach (var violation in ruleViolations)
                        {
                            RegisterViolation(violation);
                        }
                    }


                }


            }
            catch
            {

            }

            return isValid;
        }



        private void LoadRulesLinear(IList<ISchematicRuleModel> sourceRules, IList<ISchematicRuleModel> outputRules)
        {
            outputRules.AddRange(sourceRules.Where(r => r.IsEnabled));
        }

        private IList<SchematicNet> GetNets()
        {
            IList<SchematicNet> nets = new List<SchematicNet>();

            var schematic = _schModel as SchematicDesignerViewModel;

            if (schematic == null)
                return nets;

            var schNets = schematic.GetNets();

            var netGroups = schNets.GroupBy(n => n.Name);
            foreach (var ng in netGroups)
            {
                var newNet = new SchematicNet
                {
                    Name = ng.Key,
                    NetItems = (from net in ng
                                from ni in net.NetItems
                                select ni).ToList()
                };

                nets.Add(newNet);
            }

            return nets;
        }

        private void RegisterViolation(SchematicRuleCheckResult result)
        {
            switch(result.CheckResponse)
            {
                case SchematicRuleResponse.Error:
                    innerFile.AddCompileError(result.Message, _pFileName, _pProjectName, result.Location?.Location);
                    break;

                case SchematicRuleResponse.Warning:
                    innerFile.AddCompileWarning(result.Message, _pFileName, _pProjectName, result.Location?.Location);
                    break;
            }
            
        }
    }
}
