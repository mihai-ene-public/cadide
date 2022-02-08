using IDE.Core.Wizards;
using System.Collections.Generic;

namespace IDE.Documents.Views
{
    public enum PackageType
    {
        Resistors,

        Capacitors,

        Diodes,


        /// <summary>
        /// Dual In-line Package (DIP)
        /// </summary>
        DIP,

        /// <summary>
        /// Small Outline Package
        /// </summary>
        SOP,

        /// <summary>
        /// Quad Pack (QUAD)
        /// </summary>
        QFP,

        /// <summary>
        /// Leadless Chip Carrier
        /// </summary>
        LCC,

        /// <summary>
        /// Ball Grid Array
        /// </summary>
        BGA,

        /// <summary>
        /// Pin Grid Array
        /// </summary>
        PGA,

        /// <summary>
        /// Staggered Ball Grid Array
        /// </summary>
        SBGA,

        /// <summary>
        /// Staggered Pin Grid Array
        /// </summary>
        SPGA,

        EdgeConnector
    }

    public enum PadType
    {
        SMD,

        THT
    }

    public enum PadShape
    {
        ///// <summary>
        ///// Rectangular shape with rounded corners
        ///// </summary>
        //Round,

        Circular,

        Rectangular
    }

    public enum PadPlacement
    {
        Linear,

        Rectangular,

        Quad,

        /// <summary>
        /// BGA / PGA / SBGA /SPGA
        /// </summary>
        GateArray
    }

    //used for BGA
    public enum PadNamingStyle
    {
        Numeric,
        AlphaNumeric
    }

    public class FootprintPackageTypeStepViewModel : WizardStepViewModelBase<FootprintWizardItem>
    {
        public FootprintPackageTypeStepViewModel(FootprintWizardItem c)
            : base(c)
        {
        }

        RegularOptionGroupViewModel<PackageType> packageTypes;
        public RegularOptionGroupViewModel<PackageType> PackageTypes
        {
            get
            {
                if (packageTypes == null)
                {
                    var list = new List<OptionViewModel<PackageType>>()
                    {
                        new OptionViewModel<PackageType>(PackageType.Resistors, 0, null, "Resistors"),
                        new OptionViewModel<PackageType>(PackageType.Capacitors, 0, null, "Capacitors"),
                        new OptionViewModel<PackageType>(PackageType.Diodes, 0, null, "Diodes"),
                        new OptionViewModel<PackageType>(PackageType.DIP, 0, null, "Dual In-line Package (DIP)"),
                        new OptionViewModel<PackageType>(PackageType.SOP, 0, null, "Small Outline Package (SOP)"),
                        new OptionViewModel<PackageType>(PackageType.QFP, 0, null, "Quad Flat Package (QFP)"),
                        new OptionViewModel<PackageType>(PackageType.LCC, 0, null, "Leadless Chip Carrier (LCC)"),
                        //BGA, PGA on release
                        //new OptionViewModel<PackageType>(PackageType.BGA, 0, null, "Ball Grid Array (BGA)"),
                        //new OptionViewModel<PackageType>(PackageType.PGA, 0, null, "Pin Grid Array (PGA)"),
                        //we will suport staggered a little bit later, on release
                        //new OptionViewModel<PackageType>(PackageType.SBGA, 0, null, "Staggered Ball Grid Array (SBGA)"),
                        //new OptionViewModel<PackageType>(PackageType.SPGA, 0, null, "Staggered Pin Grid Array (SPGA)"),
                        //new OptionViewModel<PackageType>(PackageType.EdgeConnector, 0, null, "Edge Connector"),
                    };

                    foreach (var option in list)
                    {
                        if (option.GetValue() == BusinessObject.PackageType)
                            option.IsSelected = true;
                        option.PropertyChanged += (s, e) =>
                        {
                            var o = s as OptionViewModel<PackageType>;
                            if (option.IsSelected)
                                BusinessObject.PackageType = option.GetValue();
                        };
                    }

                    packageTypes = new RegularOptionGroupViewModel<PackageType> { OptionModels = list.AsReadOnly() };
                }
                return packageTypes;
            }
        }

        public override string DisplayName
        {
            get { return "Package"; }
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
