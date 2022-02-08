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
    public class DFNParametricPackageMeshItem : ParametricPackageMeshItem
    {
        public DFNParametricPackageMeshItem()
        {
            PropertyChanged += async (s, e) =>
            {
                var properties = new[]
                {
                    nameof(NumberPads),
                    nameof(EE),
                    nameof(D),
                    nameof(A),
                    nameof(E),
                    nameof(B),
                    nameof(L),
                };

                if (!isLoading && properties.Contains(e.PropertyName))
                    await GenerateItems();
            };
        }

        int numberPads = 8;

        [Display(Order = 1,Name="Number of pads")]
        [Editor(EditorNames.GenericValueEditor, EditorNames.GenericValueEditor)]
        [MarksDirty]
        /// <summary>
        /// Pin pitch
        /// </summary>
        public int NumberPads
        {
            get { return numberPads; }
            set
            {
                numberPads = value;
                OnPropertyChanged(nameof(NumberPads));
            }
        }

        double e = 3d;

        [Display(Order = 2)]
        [Editor(EditorNames.SizeMilimetersUnitsEditor, EditorNames.SizeMilimetersUnitsEditor)]
        [MarksDirty]
        /// <summary>
        /// Body width
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

        double d = 3d;

        [Display(Order = 3)]
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

        double a = 0.8;

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

        double ee = 0.5d;

        [Display(Order = 5, Name ="e")]
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

        double b = 0.3d;

        [Display(Order = 6,Name ="b")]
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

        double l = 0.5d;

        [Display(Order = 7)]
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

       

        public override async Task GenerateItems()
        {
            var generator = new DFNPackageGenerator
            {
                D = D,
                A = A,
                E = E,
                B = B,
                EE = EE,
                L = L,
                NumberPads = NumberPads,
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

                var r = primitive as DFNParametricPackage3DItem;

                D = r.D;
                A = r.A;
                E = r.E;
                B = r.B;
                EE = r.EE;
                L = r.L;
                NumberPads = r.NumberPads;
            }
            finally
            {
                isLoading = false;
            }

            GenerateItems().ConfigureAwait(false);
        }

        public override IPrimitive SaveToPrimitive()
        {
            var r = new DFNParametricPackage3DItem();

            SaveToPrimitiveBase(r);

            r.D = D;
            r.A = A;
            r.E = E;
            r.B = B;
            r.EE = EE;
            r.L = L;
            r.NumberPads = NumberPads;

            return r;
        }

        public override string ToString()
        {
            return "DFN package";
        }

    }

}
