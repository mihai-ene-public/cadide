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
    public class SOICParametricPackageMeshItem : ParametricPackageMeshItem
    {
        public SOICParametricPackageMeshItem() 
        {
            PropertyChanged += async (s, e) =>
            {
                var properties = new[]
                {
                    nameof(NumberPads),
                    nameof(EE),
                    nameof(D),
                    nameof(D1),
                    nameof(A),
                    nameof(A1),
                    nameof(E),
                    nameof(B),
                    nameof(L),
                };

                if (!isLoading && properties.Contains(e.PropertyName))
                    await GenerateItems();
            };
        }

        int numberPads = 8;

        [Display(Order = 1,Name ="Number of pads")]
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

        double d = 6.2d;

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

        double d1 = 4;

        [Display(Order = 3)]
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

        double ee = 5.0d;

        [Display(Order = 4,Name ="E")]
        [Editor(EditorNames.SizeMilimetersUnitsEditor, EditorNames.SizeMilimetersUnitsEditor)]
        [MarksDirty]
        /// <summary>
        /// Body length
        /// </summary>
        public double EE//E
        {
            get { return ee; }
            set
            {
                ee = value;
                OnPropertyChanged(nameof(EE));
            }
        }

        double a = 1.75;

        [Display(Order = 5)]
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

        double a1 = 0.23;

        [Display(Order = 6)]
        [Editor(EditorNames.SizeMilimetersUnitsEditor, EditorNames.SizeMilimetersUnitsEditor)]
        [MarksDirty]
        /// <summary>
        /// Body offset
        /// </summary>
        public double A1
        {
            get { return a1; }
            set
            {
                a1 = value;
                OnPropertyChanged(nameof(A1));
            }
        }

        double e = 1.27d;

        [Display(Order = 7,Name ="e")]
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

        double l = 0.5d;

        [Display(Order = 8)]
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

        double b = 0.48d;

        [Display(Order = 9,Name ="b")]
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
            var generator = new SOICPackageGenerator
            {
                D = D,
                D1 = D1,
                A = A,
                A1 = A1,
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

                var r = primitive as SOICParametricPackage3DItem;

                D = r.D;
                D1 = r.D1;
                A = r.A;
                A1 = r.A1;
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
            var r = new SOICParametricPackage3DItem();
            SaveToPrimitiveBase(r);

            r.D = D;
            r.D1 = D1;
            r.A = A;
            r.A1 = A1;
            r.E = E;
            r.B = B;
            r.EE = EE;
            r.L = L;
            r.NumberPads = NumberPads;
            
            return r;
        }

        public override string ToString()
        {
            return "SOIC package";
        }

    }

}
