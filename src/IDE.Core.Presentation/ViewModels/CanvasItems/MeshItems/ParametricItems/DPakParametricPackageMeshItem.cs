using IDE.Core.Interfaces;
using IDE.Core.Storage;
using IDE.Core.Types.Attributes;
using IDE.Documents.Views;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IDE.Core.Designers
{
    public class DPakParametricPackageMeshItem : ParametricPackageMeshItem
    {
        public DPakParametricPackageMeshItem()
        {
            PropertyChanged += async (s, e) =>
            {
                var properties = new[]
                {
                    nameof(BodyWidth),
                    nameof(BodyHeight),
                    nameof(BodyExtrusion),
                    nameof(PadWidth),
                    nameof(PadHeight),
                    nameof(PadExtrusion),
                    nameof(PadOuterOffset),
                    nameof(PinWidth),
                    nameof(PinThickness),
                    nameof(PinPitch),
                    nameof(PinBodyExitLength),
                    nameof(PinLengthTotal),
                };

                if (!isLoading && properties.Contains(e.PropertyName))
                    await GenerateItems();
            };
        }

        double bodyWidth = 6.6;

        [Display(Order = 1, Name = "E")]
        [Editor(EditorNames.SizeMilimetersUnitsEditor, EditorNames.SizeMilimetersUnitsEditor)]
        [MarksDirty]
        public double BodyWidth//E
        {
            get { return bodyWidth; }
            set
            {
                bodyWidth = value;
                OnPropertyChanged(nameof(BodyWidth));
            }
        }

        double bodyHeight = 6.1;

        [Display(Order = 2, Name = "D")]
        [Editor(EditorNames.SizeMilimetersUnitsEditor, EditorNames.SizeMilimetersUnitsEditor)]
        [MarksDirty]
        public double BodyHeight//D
        {
            get { return bodyHeight; }
            set
            {
                bodyHeight = value;
                OnPropertyChanged(nameof(BodyHeight));
            }
        }

        double bodyExtrusion = 2.2;

        [Display(Order = 3, Name = "A")]
        [Editor(EditorNames.SizeMilimetersUnitsEditor, EditorNames.SizeMilimetersUnitsEditor)]
        [MarksDirty]
        public double BodyExtrusion//A
        {
            get { return bodyExtrusion; }
            set
            {
                bodyExtrusion = value;
                OnPropertyChanged(nameof(BodyExtrusion));
            }
        }

        double padWidth = 4.3;

        [Display(Order = 4, Name = "E1")]
        [Editor(EditorNames.SizeMilimetersUnitsEditor, EditorNames.SizeMilimetersUnitsEditor)]
        [MarksDirty]
        public double PadWidth//E1
        {
            get { return padWidth; }
            set
            {
                padWidth = value;
                OnPropertyChanged(nameof(PadWidth));
            }
        }

        double padHeight = 5.2;

        [Display(Order = 5, Name = "D1")]
        [Editor(EditorNames.SizeMilimetersUnitsEditor, EditorNames.SizeMilimetersUnitsEditor)]
        [MarksDirty]
        public double PadHeight//D1
        {
            get { return padHeight; }
            set
            {
                padHeight = value;
                OnPropertyChanged(nameof(PadHeight));
            }
        }

        double padExtrusion = 0.5;

        [Display(Order = 6, Name = "c1")]
        [Editor(EditorNames.SizeMilimetersUnitsEditor, EditorNames.SizeMilimetersUnitsEditor)]
        [MarksDirty]
        public double PadExtrusion//c1
        {
            get { return padExtrusion; }
            set
            {
                padExtrusion = value;
                OnPropertyChanged(nameof(PadExtrusion));
            }
        }

        double padOuterOffset = 1.2;

        [Display(Order = 7, Name = "L3")]
        [Editor(EditorNames.SizeMilimetersUnitsEditor, EditorNames.SizeMilimetersUnitsEditor)]
        [MarksDirty]
        public double PadOuterOffset//L3
        {
            get { return padOuterOffset; }
            set
            {
                padOuterOffset = value;
                OnPropertyChanged(nameof(PadOuterOffset));
            }
        }

        double pinWidth = 0.8;

        [Display(Order = 8, Name = "b")]
        [Editor(EditorNames.SizeMilimetersUnitsEditor, EditorNames.SizeMilimetersUnitsEditor)]
        [MarksDirty]
        public double PinWidth //b
        {
            get { return pinWidth; }
            set
            {
                pinWidth = value;
                OnPropertyChanged(nameof(PinWidth));
            }
        }

        double pinThickness = 0.5;

        [Display(Order = 9, Name = "c")]
        [Editor(EditorNames.SizeMilimetersUnitsEditor, EditorNames.SizeMilimetersUnitsEditor)]
        [MarksDirty]
        public double PinThickness //c
        {
            get { return pinThickness; }
            set
            {
                pinThickness = value;
                OnPropertyChanged(nameof(PinThickness));
            }
        }

        double pinPitch = 2.3;

        [Display(Order = 10, Name = "e")]
        [Editor(EditorNames.SizeMilimetersUnitsEditor, EditorNames.SizeMilimetersUnitsEditor)]
        [MarksDirty]
        public double PinPitch //e
        {
            get { return pinPitch; }
            set
            {
                pinPitch = value;
                OnPropertyChanged(nameof(PinPitch));
            }
        }

        double pinBodyExitLength = 1.5;

        /// <summary>
        /// The length of the pin that exits the body before first bending
        /// </summary>
        [Display(Order = 11, Name = "L2")]
        [Editor(EditorNames.SizeMilimetersUnitsEditor, EditorNames.SizeMilimetersUnitsEditor)]
        [MarksDirty]
        public double PinBodyExitLength //L2
        {
            get { return pinBodyExitLength; }
            set
            {
                pinBodyExitLength = value;
                OnPropertyChanged(nameof(PinBodyExitLength));
            }
        }

        double pinLengthTotal = 2.75;

        /// <summary>
        /// The total length of the pin measured on horizontal
        /// </summary>
        [Display(Order = 12, Name = "L1")]
        [Editor(EditorNames.SizeMilimetersUnitsEditor, EditorNames.SizeMilimetersUnitsEditor)]
        [MarksDirty]
        public double PinLengthTotal //L1
        {
            get { return pinLengthTotal; }
            set
            {
                pinLengthTotal = value;
                OnPropertyChanged(nameof(PinLengthTotal));
            }
        }

        public override async Task GenerateItems()
        {
            var generator = new DPakPackageGenerator
            {
                BodyWidth = BodyWidth,
                BodyHeight = BodyHeight,
                BodyExtrusion = BodyExtrusion,
                PadWidth = PadWidth,
                PadHeight = PadHeight,
                PadExtrusion = PadExtrusion,
                PadOuterOffset = PadOuterOffset,
                PinWidth = PinWidth,
                PinThickness = PinThickness,
                PinPitch = PinPitch,
                PinBodyExitLength = PinBodyExitLength,
                PinLengthTotal = PinLengthTotal,
            };

            var items = await generator.GeneratePackage();
            items.ForEach(item => item.ParentObject = this);

            Items = items;
        }

        protected override void LoadFromPrimitiveInternal(IPrimitive primitive)
        {
            try
            {
                isLoading = true;

                base.LoadFromPrimitiveInternal(primitive);

                var r = primitive as DPakParametricPackage3DItem;

                BodyWidth = r.BodyWidth;
                BodyHeight = r.BodyHeight;
                BodyExtrusion = r.BodyExtrusion;
                PadWidth = r.PadWidth;
                PadHeight = r.PadHeight;
                PadExtrusion = r.PadExtrusion;
                PadOuterOffset = r.PadOuterOffset;
                PinWidth = r.PinWidth;
                PinThickness = r.PinThickness;
                PinPitch = r.PinPitch;
                PinBodyExitLength = r.PinBodyExitLength;
                PinLengthTotal = r.PinLengthTotal;
            }
            finally
            {
                isLoading = false;
            }

            GenerateItems().ConfigureAwait(false);
        }

        public override IPrimitive SaveToPrimitive()
        {
            var r = new DPakParametricPackage3DItem();
            SaveToPrimitiveBase(r);

            r.BodyWidth = BodyWidth;
            r.BodyHeight = BodyHeight;
            r.BodyExtrusion = BodyExtrusion;
            r.PadWidth = PadWidth;
            r.PadHeight = PadHeight;
            r.PadExtrusion = PadExtrusion;
            r.PadOuterOffset = PadOuterOffset;
            r.PinWidth = PinWidth;
            r.PinThickness = PinThickness;
            r.PinPitch = PinPitch;
            r.PinBodyExitLength = PinBodyExitLength;
            r.PinLengthTotal = PinLengthTotal;

            return r;

        }

        public override string ToString()
        {
            return "D-Pak package";
        }
    }
}
