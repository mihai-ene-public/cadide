using IDE.Core.Interfaces;
using IDE.Core.Storage;
using IDE.Core.Types.Attributes;
using IDE.Core.Types.Media;
using IDE.Documents.Views;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IDE.Core.Designers
{
    public class RadialLEDParametricPackageMeshItem : ParametricPackageMeshItem
    {
        public RadialLEDParametricPackageMeshItem()
        {
            PropertyChanged += async (s, e) =>
            {
                var properties = new[]
                {
                    nameof(D),
                    nameof(L),
                    nameof(A),
                    nameof(EE),
                    nameof(B),
                    nameof(BodyColor),
                };

                if (!isLoading && properties.Contains(e.PropertyName))
                    await GenerateItems();
            };
        }

        double a = 11.6;

        [Display(Order = 1)]
        [Editor(EditorNames.SizeMilimetersUnitsEditor, EditorNames.SizeMilimetersUnitsEditor)]
        [MarksDirty]
        /// <summary>
        /// Height of the package + lead length
        /// </summary>
        public double A
        {
            get { return a; }
            set
            {
                a = value;
                OnPropertyChanged(nameof(A));
            }
        }


        double d = 5;

        [Display(Order = 2)]
        [Editor(EditorNames.SizeMilimetersUnitsEditor, EditorNames.SizeMilimetersUnitsEditor)]
        [MarksDirty]
        /// <summary>
        /// Length of the package. Includes the length of the pads
        /// </summary>
        public double D
        {
            get { return d; }
            set
            {
                d = value;
                OnPropertyChanged(nameof(D));
            }
        }

        double ee = 2.54d;

        [Display(Order = 3, Name = "e")]
        [Editor(EditorNames.SizeMilimetersUnitsEditor, EditorNames.SizeMilimetersUnitsEditor)]
        [MarksDirty]
        /// <summary>
        /// e - pin pitch
        /// </summary>
        public double EE//e
        {
            get { return ee; }
            set
            {
                ee = value;
                OnPropertyChanged(nameof(EE));
            }
        }

        double l = 3;

        [Display(Order = 4)]
        [Editor(EditorNames.SizeMilimetersUnitsEditor, EditorNames.SizeMilimetersUnitsEditor)]
        [MarksDirty]
        /// <summary>
        /// Length of pad in mm
        /// </summary>
        public double L
        {
            get { return l; }
            set
            {
                l = value;
                OnPropertyChanged(nameof(L));
            }
        }

        double b = 0.6d;

        [Display(Order = 5, Name = "b")]
        [Editor(EditorNames.SizeMilimetersUnitsEditor, EditorNames.SizeMilimetersUnitsEditor)]
        [MarksDirty]
        /// <summary>
        /// Pad width in mm
        /// </summary>
        public double B
        {
            get { return b; }
            set
            {
                b = value;
                OnPropertyChanged(nameof(B));
            }
        }

        XColor _bodyColor = XColors.Red;

        [Editor(EditorNames.XColorEditor, EditorNames.XColorEditor)]
        public XColor BodyColor
        {
            get { return _bodyColor; }
            set
            {
                _bodyColor = value;
                OnPropertyChanged(nameof(BodyColor));
            }
        }

        public override async Task GenerateItems()
        {
            var generator = new RadialLEDPackageGenerator
            {
                D = D,
                A = A,
                EE = EE,
                B = B,
                L = L,
                BodyColor = _bodyColor
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

                var r = primitive as RadialLEDParametricPackage3DItem;

                D = r.D;
                L = r.L;
                A = r.A;
                EE = r.EE;
                B = r.B;
                BodyColor = XColor.FromHexString(r.Color);
            }
            finally
            {
                isLoading = false;
            }

            GenerateItems().ConfigureAwait(false);
        }

        public override IPrimitive SaveToPrimitive()
        {
            var r = new RadialLEDParametricPackage3DItem();
            SaveToPrimitiveBase(r);

            r.D = D;
            r.L = L;
            r.A = A;
            r.EE = EE;
            r.B = B;
            r.Color = BodyColor.ToHexString();

            return r;
        }

        public override string ToString()
        {
            return "Radial LED";
        }

    }

}
