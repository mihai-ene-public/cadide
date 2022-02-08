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
    public class CrystalSMDParametricPackageMeshItem : ParametricPackageMeshItem
    {
        public CrystalSMDParametricPackageMeshItem()
        {
            PropertyChanged += async (s, e) =>
            {
                var properties = new[]
                {
                    nameof(D),
                    nameof(D1),
                    nameof(B),
                    nameof(L),
                    nameof(A),
                    nameof(E),
                };

                if (!isLoading && properties.Contains(e.PropertyName))
                    await GenerateItems();
            };
        }

        double d = 13.2d;

        [Display(Order = 1)]
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

        double d1 = 11.7d;

        [Display(Order = 2)]
        [Editor(EditorNames.SizeMilimetersUnitsEditor, EditorNames.SizeMilimetersUnitsEditor)]
        [MarksDirty]
        /// <summary>
        /// Body width
        /// </summary>
        public double D1
        {
            get { return d1; }
            set
            {
                d1 = value;
                OnPropertyChanged(nameof(D1));
            }
        }

       

        double e = 5d;

        [Display(Order = 3)]
        [Editor(EditorNames.SizeMilimetersUnitsEditor, EditorNames.SizeMilimetersUnitsEditor)]
        [MarksDirty]
        /// <summary>
        /// Pin pitch
        /// </summary>
        public double E
        {
            get { return e; }
            set
            {
                e = value;
                OnPropertyChanged(nameof(E));
            }
        }

        double a = 4.3;

        [Display(Order = 4)]
        [Editor(EditorNames.SizeMilimetersUnitsEditor, EditorNames.SizeMilimetersUnitsEditor)]
        [MarksDirty]
        /// <summary>
        /// Height of the package
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

        double l = 4d;

        [Display(Order = 5)]
        [Editor(EditorNames.SizeMilimetersUnitsEditor, EditorNames.SizeMilimetersUnitsEditor)]
        [MarksDirty]
        /// <summary>
        /// Length of landing pad in mm
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

        double b = 0.63d;

        [Display(Order = 4,Name ="b")]
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

        

        public override async Task GenerateItems()
        {
            var generator = new CrystalSMDPackageGenerator
            {
                D = D,
                A = A,
                E = E,
                D1 = D1,
                B = B,
                L = L,
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

                var r = primitive as CrystalSMDParametricPackage3DItem;

                D = r.D;
                D1 = r.D1;
                B = r.B;
                L = r.L;
                A = r.A;
                E = r.E;
            }
            finally
            {
                isLoading = false;
            }

            GenerateItems().ConfigureAwait(false);
        }

        public override IPrimitive SaveToPrimitive()
        {
            var r = new CrystalSMDParametricPackage3DItem();

            SaveToPrimitiveBase(r);

            r.D = D;
            r.D1 = D1;
            r.B = B;
            r.L = L;
            r.A = A;
            r.E = E;

            return r;
        }

        public override string ToString()
        {
            return "Crystal SMD package";
        }

    }

}
