using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IDE.Core.Interfaces;
using IDE.Core.Storage;
using IDE.Core.Types.Media;

namespace IDE.Core.Designers
{
    public abstract class AbstractSchematicRule : BaseViewModel, ISchematicRuleModel
    {

        bool isEnabled = true;
        public bool IsEnabled
        {
            get
            {
                return isEnabled;
            }
            set
            {
                isEnabled = value;
                OnPropertyChanged(nameof(IsEnabled));
            }
        }

        public IList<SchematicRuleResponse> SchematicRuleResponses => Enum.GetValues(typeof(SchematicRuleResponse)).Cast<SchematicRuleResponse>().ToList();


        public abstract void LoadFromData(ISchematicRuleData rule);

        public abstract SchematicRuleData SaveToSchematicRule();
    }
}
