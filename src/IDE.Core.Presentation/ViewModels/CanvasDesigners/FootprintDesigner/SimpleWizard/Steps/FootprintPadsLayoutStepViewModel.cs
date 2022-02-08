using IDE.Core.Wizards;
using System.Collections.Generic;

namespace IDE.Documents.Views
{
    public class FootprintPadsLayoutStepViewModel : WizardStepViewModelBase<FootprintWizardItem>
    {
        public FootprintPadsLayoutStepViewModel(FootprintWizardItem c)
            : base(c)
        {

        }


        RegularOptionGroupViewModel<PadNamingStyle> namingStyles;
        public RegularOptionGroupViewModel<PadNamingStyle> NamingStyles
        {
            get
            {
                if (namingStyles == null)
                {
                    var list = new List<OptionViewModel<PadNamingStyle>>()
                    {
                        new OptionViewModel<PadNamingStyle>(PadNamingStyle.Numeric, 0, null, "Numeric"),
                        new OptionViewModel<PadNamingStyle>(PadNamingStyle.AlphaNumeric, 0, null, "AlphaNumeric"),
                    };

                    foreach (var option in list)
                    {
                        if (option.GetValue() == BusinessObject.PadNamingStyle)
                            option.IsSelected = true;
                        option.PropertyChanged += (s, e) =>
                        {
                            var o = s as OptionViewModel<PadNamingStyle>;
                            if (option.IsSelected)
                                BusinessObject.PadNamingStyle = option.GetValue();
                        };
                    }

                    namingStyles = new RegularOptionGroupViewModel<PadNamingStyle> { OptionModels = list.AsReadOnly() };
                }
                return namingStyles;
            }
        }

        public override string DisplayName
        {
            get { return "Pads Layout"; }
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
