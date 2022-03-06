using IDE.Core.Interfaces;
using IDE.Core.Presentation.Compilers;
using IDE.Core.Storage;
using IDE.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IDE.Core.Designers
{
    public class SchematicRulesCompiler : AbstractCompiler, ISchematicRulesCompiler
    {
        public async Task<CompilerResult> Compile(ISchematicDesigner schematic)
        {
            var isValid = true;
            var errors = new List<IErrorMessage>();

            await Task.CompletedTask;

            try
            {
                var rules = new List<ISchematicRuleModel>();
                LoadRulesLinear(schematic.Rules, rules);

                var nets = GetNets(schematic);

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
                            errors.Add(GetViolation(violation, schematic));
                        }
                    }
                }
            }
            catch
            {

            }

            return new CompilerResult
            {
                Success = isValid,
                Errors = errors
            };
        }



        private void LoadRulesLinear(IList<ISchematicRuleModel> sourceRules, IList<ISchematicRuleModel> outputRules)
        {
            outputRules.AddRange(sourceRules.Where(r => r.IsEnabled));
        }

        private IList<SchematicNet> GetNets(ISchematicDesigner schematic)
        {
            IList<SchematicNet> nets = new List<SchematicNet>();

            if (schematic == null)
                return nets;

            var schNets = schematic.GetNets();

            var netGroups = schNets.GroupBy(n => n.Name);
            foreach (var ng in netGroups)
            {
                var newNet = new SchematicNet
                {
                    Name = ng.Key,
                    NetItems = ( from net in ng.Cast<SchematicNet>()
                                 from ni in net.NetItems
                                 select ni ).ToList()
                };

                nets.Add(newNet);
            }

            return nets;
        }

        private IErrorMessage GetViolation(SchematicRuleCheckResult result, ISchematicDesigner schematic)
        {
            var projectName = schematic.ProjectNode.Name;
            switch (result.CheckResponse)
            {
                case SchematicRuleResponse.Error:
                    return BuildErrorMessage(result.Message, projectName, schematic, result.Location?.Location);

                case SchematicRuleResponse.Warning:
                    return BuildWarningMessage(result.Message, projectName, schematic, result.Location?.Location);
            }

            return null;

        }
    }
}
