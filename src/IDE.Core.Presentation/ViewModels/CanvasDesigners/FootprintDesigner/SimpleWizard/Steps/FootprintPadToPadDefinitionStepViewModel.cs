using IDE.Core.Wizards;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace IDE.Documents.Views
{
    public class FootprintPadToPadDefinitionStepViewModel : WizardStepViewModelBase<FootprintWizardItem>
    {
        public FootprintPadToPadDefinitionStepViewModel(FootprintWizardItem c)
            : base(c)
        {
            BusinessObject.PropertyChanged += BusinessObject_PropertyChanged;
        }

        public double DisplayPadSizeX
        {
            get { return BusinessObject.PadSizeX * 1000; }
        }

        public double DisplayPadSizeY
        {
            get
            {
                if (BusinessObject.PadShape == PadShape.Circular)
                    return DisplayPadSizeX;

                return BusinessObject.PadSizeY * 1000;
            }
        }

        public double DisplayCornerRadius
        {
            get
            {
                if (BusinessObject.PadShape == PadShape.Circular)
                    return DisplayPadSizeX;

                return 0;
            }
        }

        public double DisplayPadDrill
        {
            get { return BusinessObject.PadDrill * 1000; }
        }


        void BusinessObject_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(BusinessObject.PadSizeX):
                    OnPropertyChanged(nameof(DisplayPadSizeX));
                    OnPropertyChanged(nameof(DisplayCornerRadius));
                    break;

                case nameof(BusinessObject.PadSizeY):
                    OnPropertyChanged(nameof(DisplayPadSizeY));
                    break;

                case nameof(BusinessObject.PadDrill):
                    OnPropertyChanged(nameof(DisplayPadDrill));
                    break;

                case nameof(BusinessObject.PadShape):
                    OnPropertyChanged(nameof(DisplayPadSizeY));
                    break;
            }
        }

        public override string DisplayName
        {
            get { return "Pad to Pad Definition"; }
        }

        public override RouteModifier OnNext()
        {
            switch(BusinessObject.PackageType)
            {
                case PackageType.Capacitors:
                case PackageType.Resistors:
                case PackageType.Diodes:
                    return new RouteModifier { ExcludeViewModelTypes = new List<Type> { typeof(FootprintPadsLayoutStepViewModel) } };
            }

            return new RouteModifier { IncludeViewModelTypes = new List<Type> { typeof(FootprintPadsLayoutStepViewModel) } };
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
