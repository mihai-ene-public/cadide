using IDE.Core.Interfaces;
using IDE.Core.Storage;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IDE.Core.Designers
{
    /*
     * Takes a net (NetDesignerItem one or more grouped by name)
     * and checks for all pins the pinType against the specified connection matrix
     * 
     * OR
     * 
     * A pair of connected pins is formed outside the rule; the rule checks these 2 pins that assumes they are connected
     */
    public class PinTypesConnectionSchematicRule : PairedItemsSchematicRule
    {

        #region Response Properties

        //Passive
        public PinTypesConnectionResponseSpec PassivePassiveResponse => GetConnectionSpec(PinType.Passive, PinType.Passive);
        public PinTypesConnectionResponseSpec PassiveInputResponse => GetConnectionSpec(PinType.Passive, PinType.Input);
        public PinTypesConnectionResponseSpec PassiveOutputResponse => GetConnectionSpec(PinType.Passive, PinType.Output);
        public PinTypesConnectionResponseSpec PassiveIOResponse => GetConnectionSpec(PinType.Passive, PinType.IO);
        public PinTypesConnectionResponseSpec PassiveOpenCollectorResponse => GetConnectionSpec(PinType.Passive, PinType.OpenCollector);
        public PinTypesConnectionResponseSpec PassiveOpenEmitterResponse => GetConnectionSpec(PinType.Passive, PinType.OpenEmitter);
        public PinTypesConnectionResponseSpec PassiveHiZResponse => GetConnectionSpec(PinType.Passive, PinType.HiZ);
        public PinTypesConnectionResponseSpec PassivePowerResponse => GetConnectionSpec(PinType.Passive, PinType.Power);
        public PinTypesConnectionResponseSpec PassiveNoConnectResponse => GetConnectionSpec(PinType.Passive, PinType.NoConnect);

        //Input
        public PinTypesConnectionResponseSpec InputInputResponse => GetConnectionSpec(PinType.Input, PinType.Input);
        public PinTypesConnectionResponseSpec InputOutputResponse => GetConnectionSpec(PinType.Input, PinType.Output);
        public PinTypesConnectionResponseSpec InputIOResponse => GetConnectionSpec(PinType.Input, PinType.IO);
        public PinTypesConnectionResponseSpec InputOpenCollectorResponse => GetConnectionSpec(PinType.Input, PinType.OpenCollector);
        public PinTypesConnectionResponseSpec InputOpenEmitterResponse => GetConnectionSpec(PinType.Input, PinType.OpenEmitter);
        public PinTypesConnectionResponseSpec InputHiZResponse => GetConnectionSpec(PinType.Input, PinType.HiZ);
        public PinTypesConnectionResponseSpec InputPowerResponse => GetConnectionSpec(PinType.Input, PinType.Power);
        public PinTypesConnectionResponseSpec InputNoConnectResponse => GetConnectionSpec(PinType.Input, PinType.NoConnect);

        //Output
        public PinTypesConnectionResponseSpec OutputOutputResponse => GetConnectionSpec(PinType.Output, PinType.Output);
        public PinTypesConnectionResponseSpec OutputIOResponse => GetConnectionSpec(PinType.Output, PinType.IO);
        public PinTypesConnectionResponseSpec OutputOpenCollectorResponse => GetConnectionSpec(PinType.Output, PinType.OpenCollector);
        public PinTypesConnectionResponseSpec OutputOpenEmitterResponse => GetConnectionSpec(PinType.Output, PinType.OpenEmitter);
        public PinTypesConnectionResponseSpec OutputHiZResponse => GetConnectionSpec(PinType.Output, PinType.HiZ);
        public PinTypesConnectionResponseSpec OutputPowerResponse => GetConnectionSpec(PinType.Output, PinType.Power);
        public PinTypesConnectionResponseSpec OutputNoConnectResponse => GetConnectionSpec(PinType.Output, PinType.NoConnect);

        //IO
        public PinTypesConnectionResponseSpec IOIOResponse => GetConnectionSpec(PinType.IO, PinType.IO);
        public PinTypesConnectionResponseSpec IOOpenCollectorResponse => GetConnectionSpec(PinType.IO, PinType.OpenCollector);
        public PinTypesConnectionResponseSpec IOOpenEmitterResponse => GetConnectionSpec(PinType.IO, PinType.OpenEmitter);
        public PinTypesConnectionResponseSpec IOHiZResponse => GetConnectionSpec(PinType.IO, PinType.HiZ);
        public PinTypesConnectionResponseSpec IOPowerResponse => GetConnectionSpec(PinType.IO, PinType.Power);
        public PinTypesConnectionResponseSpec IONoConnectResponse => GetConnectionSpec(PinType.IO, PinType.NoConnect);

        //OpenCollector
        public PinTypesConnectionResponseSpec OpenCollectorOpenCollectorResponse => GetConnectionSpec(PinType.OpenCollector, PinType.OpenCollector);
        public PinTypesConnectionResponseSpec OpenCollectorOpenEmitterResponse => GetConnectionSpec(PinType.OpenCollector, PinType.OpenEmitter);
        public PinTypesConnectionResponseSpec OpenCollectorHiZResponse => GetConnectionSpec(PinType.OpenCollector, PinType.HiZ);
        public PinTypesConnectionResponseSpec OpenCollectorPowerResponse => GetConnectionSpec(PinType.OpenCollector, PinType.Power);
        public PinTypesConnectionResponseSpec OpenCollectorNoConnectResponse => GetConnectionSpec(PinType.OpenCollector, PinType.NoConnect);

        //OpenEmitter
        public PinTypesConnectionResponseSpec OpenEmitterOpenEmitterResponse => GetConnectionSpec(PinType.OpenEmitter, PinType.OpenEmitter);
        public PinTypesConnectionResponseSpec OpenEmitterHiZResponse => GetConnectionSpec(PinType.OpenEmitter, PinType.HiZ);
        public PinTypesConnectionResponseSpec OpenEmitterPowerResponse => GetConnectionSpec(PinType.OpenEmitter, PinType.Power);
        public PinTypesConnectionResponseSpec OpenEmitterNoConnectResponse => GetConnectionSpec(PinType.OpenEmitter, PinType.NoConnect);

        //HiZ
        public PinTypesConnectionResponseSpec HiZHiZResponse => GetConnectionSpec(PinType.HiZ, PinType.HiZ);
        public PinTypesConnectionResponseSpec HiZPowerResponse => GetConnectionSpec(PinType.HiZ, PinType.Power);
        public PinTypesConnectionResponseSpec HiZNoConnectResponse => GetConnectionSpec(PinType.HiZ, PinType.NoConnect);

        //Power
        public PinTypesConnectionResponseSpec PowerPowerResponse => GetConnectionSpec(PinType.Power, PinType.Power);
        public PinTypesConnectionResponseSpec PowerNoConnectResponse => GetConnectionSpec(PinType.Power, PinType.NoConnect);

        //NC
        public PinTypesConnectionResponseSpec NoConnectNoConnectResponse => GetConnectionSpec(PinType.NoConnect, PinType.NoConnect);

        #endregion Response Properties


        public List<PinTypesConnectionResponseSpec> PinTypesConnections { get; set; } = new List<PinTypesConnectionResponseSpec>();

        private PinTypesConnectionResponseSpec GetConnectionSpec(PinType pinType1, PinType pinType2)
        {
            var spec = PinTypesConnections.FirstOrDefault(c => (c.PinType1 == pinType1 && c.PinType2 == pinType2)
                                                               || (c.PinType1 == pinType2 && c.PinType2 == pinType1));

            return spec;
        }

        private SchematicRuleResponse GetRuleResponse(PinType pinType1, PinType pinType2)
        {
            var spec = GetConnectionSpec(pinType1, pinType2);

            if (spec != null)
                return spec.Response;

            return SchematicRuleResponse.NoError;
        }

        public override SchematicRuleCheckResult CheckItems(ISelectableItem item1, ISelectableItem item2)
        {
            var res = new SchematicRuleCheckResult { IsValid = true };

            if (item1 is PinCanvasItem pin1 && item2 is PinCanvasItem pin2)
            {
                var ruleResponse = GetRuleResponse(pin1.PinType, pin2.PinType);
                if (ruleResponse == SchematicRuleResponse.NoError)
                    return res;

                var netName = pin1.Net?.Name;
                res.IsValid = false;
                res.Message = $"Net {netName} connects a pin {pin1.Name} of type {pin1.PinType} with another pin {pin2.Name} of type {pin2.PinType}";
                res.CheckResponse = ruleResponse;
            }
            return res;
        }

        public override void LoadFromData(ISchematicRuleData rule)
        {
            var r = rule as PinTypesConnectionSchematicRuleData;

            IsEnabled = r.IsEnabled;
            PinTypesConnections = r.PinTypesConnections;
        }

        public override SchematicRuleData SaveToSchematicRule()
        {
            return new PinTypesConnectionSchematicRuleData
            {
                IsEnabled = IsEnabled,
                PinTypesConnections = PinTypesConnections
            };
        }
    }


}
