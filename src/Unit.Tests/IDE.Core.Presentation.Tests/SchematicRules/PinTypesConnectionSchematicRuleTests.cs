using IDE.Core.Designers;
using IDE.Core.Storage;
using System.Collections.Generic;
using Xunit;

namespace IDE.Core.Presentation.Tests.SchematicRules
{
    public class PinTypesConnectionSchematicRuleTests
    {
        [Theory]
        //Passive - Passive
        [InlineData(PinType.Passive, PinType.Passive, true, SchematicRuleResponse.NoError)]
        //Passive - Input
        [InlineData(PinType.Passive, PinType.Input, true, SchematicRuleResponse.NoError)]
        [InlineData(PinType.Input, PinType.Passive, true, SchematicRuleResponse.NoError)]
        //Passive - Output
        [InlineData(PinType.Passive, PinType.Output, true, SchematicRuleResponse.NoError)]
        [InlineData(PinType.Output, PinType.Passive, true, SchematicRuleResponse.NoError)]
        //Passive - IO
        [InlineData(PinType.Passive, PinType.IO, true, SchematicRuleResponse.NoError)]
        [InlineData(PinType.IO, PinType.Passive, true, SchematicRuleResponse.NoError)]

        [InlineData(PinType.Passive, PinType.OpenCollector, true, SchematicRuleResponse.NoError)]
        [InlineData(PinType.OpenCollector, PinType.Passive, true, SchematicRuleResponse.NoError)]

        [InlineData(PinType.Passive, PinType.OpenEmitter, true, SchematicRuleResponse.NoError)]
        [InlineData(PinType.OpenEmitter, PinType.Passive, true, SchematicRuleResponse.NoError)]

        [InlineData(PinType.Passive, PinType.HiZ, true, SchematicRuleResponse.NoError)]
        [InlineData(PinType.HiZ, PinType.Passive, true, SchematicRuleResponse.NoError)]

        [InlineData(PinType.Passive, PinType.Power, true, SchematicRuleResponse.NoError)]
        [InlineData(PinType.Power, PinType.Passive, true, SchematicRuleResponse.NoError)]

        [InlineData(PinType.Passive, PinType.NoConnect, true, SchematicRuleResponse.NoError)]
        [InlineData(PinType.NoConnect, PinType.Passive, true, SchematicRuleResponse.NoError)]

        //Input
        [InlineData(PinType.Input, PinType.Input, true, SchematicRuleResponse.NoError)]

        [InlineData(PinType.Input, PinType.Output, true, SchematicRuleResponse.NoError)]
        [InlineData(PinType.Output, PinType.Input, true, SchematicRuleResponse.NoError)]

        [InlineData(PinType.Input, PinType.IO, true, SchematicRuleResponse.NoError)]
        [InlineData(PinType.IO, PinType.Input, true, SchematicRuleResponse.NoError)]

        [InlineData(PinType.Input, PinType.OpenCollector, true, SchematicRuleResponse.NoError)]
        [InlineData(PinType.OpenCollector, PinType.Input, true, SchematicRuleResponse.NoError)]

        [InlineData(PinType.Input, PinType.OpenEmitter, true, SchematicRuleResponse.NoError)]
        [InlineData(PinType.OpenEmitter, PinType.Input, true, SchematicRuleResponse.NoError)]

        [InlineData(PinType.Input, PinType.HiZ, true, SchematicRuleResponse.NoError)]
        [InlineData(PinType.HiZ, PinType.Input, true, SchematicRuleResponse.NoError)]

        [InlineData(PinType.Input, PinType.Power, true, SchematicRuleResponse.NoError)]
        [InlineData(PinType.Power, PinType.Input, true, SchematicRuleResponse.NoError)]

        [InlineData(PinType.Input, PinType.NoConnect, true, SchematicRuleResponse.NoError)]
        [InlineData(PinType.NoConnect, PinType.Input, true, SchematicRuleResponse.NoError)]

        //Output
        [InlineData(PinType.Output, PinType.Output, false, SchematicRuleResponse.Error)]

        [InlineData(PinType.Output, PinType.IO, false, SchematicRuleResponse.Warning)]
        [InlineData(PinType.IO, PinType.Output, false, SchematicRuleResponse.Warning)]

        [InlineData(PinType.Output, PinType.OpenCollector, false, SchematicRuleResponse.Error)]
        [InlineData(PinType.OpenCollector, PinType.Output, false, SchematicRuleResponse.Error)]

        [InlineData(PinType.Output, PinType.OpenEmitter, false, SchematicRuleResponse.Error)]
        [InlineData(PinType.OpenEmitter, PinType.Output, false, SchematicRuleResponse.Error)]

        [InlineData(PinType.Output, PinType.HiZ, true, SchematicRuleResponse.NoError)]
        [InlineData(PinType.HiZ, PinType.Output, true, SchematicRuleResponse.NoError)]

        [InlineData(PinType.Output, PinType.Power, false, SchematicRuleResponse.Error)]
        [InlineData(PinType.Power, PinType.Output, false, SchematicRuleResponse.Error)]

        [InlineData(PinType.Output, PinType.NoConnect, true, SchematicRuleResponse.NoError)]
        [InlineData(PinType.NoConnect, PinType.Output, true, SchematicRuleResponse.NoError)]


        //IO
        [InlineData(PinType.IO, PinType.IO, true, SchematicRuleResponse.NoError)]

        [InlineData(PinType.IO, PinType.OpenCollector, true, SchematicRuleResponse.NoError)]
        [InlineData(PinType.OpenCollector, PinType.IO, true, SchematicRuleResponse.NoError)]

        [InlineData(PinType.IO, PinType.OpenEmitter, true, SchematicRuleResponse.NoError)]
        [InlineData(PinType.OpenEmitter, PinType.IO, true, SchematicRuleResponse.NoError)]

        [InlineData(PinType.IO, PinType.HiZ, true, SchematicRuleResponse.NoError)]
        [InlineData(PinType.HiZ, PinType.IO, true, SchematicRuleResponse.NoError)]

        [InlineData(PinType.IO, PinType.Power, true, SchematicRuleResponse.NoError)]
        [InlineData(PinType.Power, PinType.IO, true, SchematicRuleResponse.NoError)]

        [InlineData(PinType.IO, PinType.NoConnect, true, SchematicRuleResponse.NoError)]
        [InlineData(PinType.NoConnect, PinType.IO, true, SchematicRuleResponse.NoError)]

        //OpenCollector
        [InlineData(PinType.OpenCollector, PinType.OpenCollector, true, SchematicRuleResponse.NoError)]

        [InlineData(PinType.OpenCollector, PinType.OpenEmitter, true, SchematicRuleResponse.NoError)]
        [InlineData(PinType.OpenEmitter, PinType.OpenCollector, true, SchematicRuleResponse.NoError)]

        [InlineData(PinType.OpenCollector, PinType.HiZ, true, SchematicRuleResponse.NoError)]
        [InlineData(PinType.HiZ, PinType.OpenCollector, true, SchematicRuleResponse.NoError)]

        [InlineData(PinType.OpenCollector, PinType.Power, true, SchematicRuleResponse.NoError)]
        [InlineData(PinType.Power, PinType.OpenCollector, true, SchematicRuleResponse.NoError)]

        [InlineData(PinType.OpenCollector, PinType.NoConnect, true, SchematicRuleResponse.NoError)]
        [InlineData(PinType.NoConnect, PinType.OpenCollector, true, SchematicRuleResponse.NoError)]

        //OpenEmitter
        [InlineData(PinType.OpenEmitter, PinType.OpenEmitter, true, SchematicRuleResponse.NoError)]

        [InlineData(PinType.OpenEmitter, PinType.HiZ, true, SchematicRuleResponse.NoError)]
        [InlineData(PinType.HiZ, PinType.OpenEmitter, true, SchematicRuleResponse.NoError)]

        [InlineData(PinType.OpenEmitter, PinType.Power, true, SchematicRuleResponse.NoError)]
        [InlineData(PinType.Power, PinType.OpenEmitter, true, SchematicRuleResponse.NoError)]

        [InlineData(PinType.OpenEmitter, PinType.NoConnect, true, SchematicRuleResponse.NoError)]
        [InlineData(PinType.NoConnect, PinType.OpenEmitter, true, SchematicRuleResponse.NoError)]

        //HiZ
        [InlineData(PinType.HiZ, PinType.HiZ, true, SchematicRuleResponse.NoError)]

        [InlineData(PinType.HiZ, PinType.Power, true, SchematicRuleResponse.NoError)]
        [InlineData(PinType.Power, PinType.HiZ, true, SchematicRuleResponse.NoError)]

        [InlineData(PinType.HiZ, PinType.NoConnect, true, SchematicRuleResponse.NoError)]
        [InlineData(PinType.NoConnect, PinType.HiZ, true, SchematicRuleResponse.NoError)]

        //Power
        [InlineData(PinType.Power, PinType.Power, true, SchematicRuleResponse.NoError)]

        [InlineData(PinType.Power, PinType.NoConnect, true, SchematicRuleResponse.NoError)]
        [InlineData(PinType.NoConnect, PinType.Power, true, SchematicRuleResponse.NoError)]

        //NoConnect
        [InlineData(PinType.NoConnect, PinType.NoConnect, true, SchematicRuleResponse.NoError)]
        public void CheckItems(PinType pinType1, PinType pinType2, bool expectedValid, SchematicRuleResponse expectedRuleResponse)
        {
            var rule = new PinTypesConnectionSchematicRule();
            //build pin types matrix response

            rule.PinTypesConnections = new List<PinTypesConnectionResponseSpec>
            {
                //Passive
                new PinTypesConnectionResponseSpec{  PinType1 = PinType.Passive, PinType2 = PinType.Passive, Response = SchematicRuleResponse.NoError},

                new PinTypesConnectionResponseSpec{  PinType1 = PinType.Passive, PinType2 = PinType.Input, Response = SchematicRuleResponse.NoError},
                new PinTypesConnectionResponseSpec{  PinType1 = PinType.Passive, PinType2 = PinType.Output, Response = SchematicRuleResponse.NoError},
                new PinTypesConnectionResponseSpec{  PinType1 = PinType.Passive, PinType2 = PinType.IO, Response = SchematicRuleResponse.NoError},

                new PinTypesConnectionResponseSpec{  PinType1 = PinType.Passive, PinType2 = PinType.OpenCollector, Response = SchematicRuleResponse.NoError},
                new PinTypesConnectionResponseSpec{  PinType1 = PinType.Passive, PinType2 = PinType.OpenEmitter, Response = SchematicRuleResponse.NoError},
                new PinTypesConnectionResponseSpec{  PinType1 = PinType.Passive, PinType2 = PinType.HiZ, Response = SchematicRuleResponse.NoError},

                new PinTypesConnectionResponseSpec{  PinType1 = PinType.Passive, PinType2 = PinType.Power, Response = SchematicRuleResponse.NoError},
                new PinTypesConnectionResponseSpec{  PinType1 = PinType.Passive, PinType2 = PinType.NoConnect, Response = SchematicRuleResponse.NoError},

                //Input
                new PinTypesConnectionResponseSpec{  PinType1 = PinType.Input, PinType2 = PinType.Input, Response = SchematicRuleResponse.NoError},
                new PinTypesConnectionResponseSpec{  PinType1 = PinType.Input, PinType2 = PinType.Output, Response = SchematicRuleResponse.NoError},
                new PinTypesConnectionResponseSpec{  PinType1 = PinType.Input, PinType2 = PinType.IO, Response = SchematicRuleResponse.NoError},

                new PinTypesConnectionResponseSpec{  PinType1 = PinType.Input, PinType2 = PinType.OpenCollector, Response = SchematicRuleResponse.NoError},
                new PinTypesConnectionResponseSpec{  PinType1 = PinType.Input, PinType2 = PinType.OpenEmitter, Response = SchematicRuleResponse.NoError},
                new PinTypesConnectionResponseSpec{  PinType1 = PinType.Input, PinType2 = PinType.HiZ, Response = SchematicRuleResponse.NoError},

                new PinTypesConnectionResponseSpec{  PinType1 = PinType.Input, PinType2 = PinType.Power, Response = SchematicRuleResponse.NoError},
                new PinTypesConnectionResponseSpec{  PinType1 = PinType.Input, PinType2 = PinType.NoConnect, Response = SchematicRuleResponse.NoError},

                //Output
                new PinTypesConnectionResponseSpec{  PinType1 = PinType.Output, PinType2 = PinType.Output, Response = SchematicRuleResponse.Error},
                new PinTypesConnectionResponseSpec{  PinType1 = PinType.Output, PinType2 = PinType.IO, Response = SchematicRuleResponse.Warning},

                new PinTypesConnectionResponseSpec{  PinType1 = PinType.Output, PinType2 = PinType.OpenCollector, Response = SchematicRuleResponse.Error},
                new PinTypesConnectionResponseSpec{  PinType1 = PinType.Output, PinType2 = PinType.OpenEmitter, Response = SchematicRuleResponse.Error},
                new PinTypesConnectionResponseSpec{  PinType1 = PinType.Output, PinType2 = PinType.HiZ, Response = SchematicRuleResponse.NoError},

                new PinTypesConnectionResponseSpec{  PinType1 = PinType.Output, PinType2 = PinType.Power, Response = SchematicRuleResponse.Error},
                new PinTypesConnectionResponseSpec{  PinType1 = PinType.Output, PinType2 = PinType.NoConnect, Response = SchematicRuleResponse.NoError},

                //IO
                new PinTypesConnectionResponseSpec{  PinType1 = PinType.IO, PinType2 = PinType.IO, Response = SchematicRuleResponse.NoError},

                new PinTypesConnectionResponseSpec{  PinType1 = PinType.IO, PinType2 = PinType.OpenCollector, Response = SchematicRuleResponse.NoError},
                new PinTypesConnectionResponseSpec{  PinType1 = PinType.IO, PinType2 = PinType.OpenEmitter, Response = SchematicRuleResponse.NoError},
                new PinTypesConnectionResponseSpec{  PinType1 = PinType.IO, PinType2 = PinType.HiZ, Response = SchematicRuleResponse.NoError},

                new PinTypesConnectionResponseSpec{  PinType1 = PinType.IO, PinType2 = PinType.Power, Response = SchematicRuleResponse.NoError},
                new PinTypesConnectionResponseSpec{  PinType1 = PinType.IO, PinType2 = PinType.NoConnect, Response = SchematicRuleResponse.NoError},

                //OpenCollector
                new PinTypesConnectionResponseSpec{  PinType1 = PinType.OpenCollector, PinType2 = PinType.OpenCollector, Response = SchematicRuleResponse.NoError},
                new PinTypesConnectionResponseSpec{  PinType1 = PinType.OpenCollector, PinType2 = PinType.OpenEmitter, Response = SchematicRuleResponse.NoError},
                new PinTypesConnectionResponseSpec{  PinType1 = PinType.OpenCollector, PinType2 = PinType.HiZ, Response = SchematicRuleResponse.NoError},

                new PinTypesConnectionResponseSpec{  PinType1 = PinType.OpenCollector, PinType2 = PinType.Power, Response = SchematicRuleResponse.NoError},
                new PinTypesConnectionResponseSpec{  PinType1 = PinType.OpenCollector, PinType2 = PinType.NoConnect, Response = SchematicRuleResponse.NoError},

                //OpenEmitter
                new PinTypesConnectionResponseSpec{  PinType1 = PinType.OpenEmitter, PinType2 = PinType.OpenEmitter, Response = SchematicRuleResponse.NoError},
                new PinTypesConnectionResponseSpec{  PinType1 = PinType.OpenEmitter, PinType2 = PinType.HiZ, Response = SchematicRuleResponse.NoError},

                new PinTypesConnectionResponseSpec{  PinType1 = PinType.OpenEmitter, PinType2 = PinType.Power, Response = SchematicRuleResponse.NoError},
                new PinTypesConnectionResponseSpec{  PinType1 = PinType.OpenEmitter, PinType2 = PinType.NoConnect, Response = SchematicRuleResponse.NoError},

                //HiZ
                new PinTypesConnectionResponseSpec{  PinType1 = PinType.HiZ, PinType2 = PinType.HiZ, Response = SchematicRuleResponse.NoError},

                new PinTypesConnectionResponseSpec{  PinType1 = PinType.HiZ, PinType2 = PinType.Power, Response = SchematicRuleResponse.NoError},
                new PinTypesConnectionResponseSpec{  PinType1 = PinType.HiZ, PinType2 = PinType.NoConnect, Response = SchematicRuleResponse.NoError},

                //Power
                new PinTypesConnectionResponseSpec{  PinType1 = PinType.Power, PinType2 = PinType.Power, Response = SchematicRuleResponse.NoError},
                new PinTypesConnectionResponseSpec{  PinType1 = PinType.Power, PinType2 = PinType.NoConnect, Response = SchematicRuleResponse.NoError},

                //No Connect
                 new PinTypesConnectionResponseSpec{  PinType1 = PinType.NoConnect, PinType2 = PinType.NoConnect, Response = SchematicRuleResponse.NoError},
            };

            var pin1 = new PinCanvasItem
            {
                PinType = pinType1
            };
            var pin2 = new PinCanvasItem
            {
                PinType = pinType2
            };

            var result = rule.CheckItems(pin1, pin2);

            Assert.Equal(expectedValid, result.IsValid);
            Assert.Equal(expectedRuleResponse, result.CheckResponse);
        }
    }
}
