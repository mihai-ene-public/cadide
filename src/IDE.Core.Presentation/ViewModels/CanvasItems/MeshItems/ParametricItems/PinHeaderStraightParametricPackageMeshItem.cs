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
    public class PinHeaderStraightParametricPackageMeshItem : ParametricPackageMeshItem
    {
        public PinHeaderStraightParametricPackageMeshItem()
        {
            PropertyChanged += async (s, e) =>
            {
                var properties = new[]
                {
                    nameof(NumberRows),
                    nameof(NumberColumns),
                    nameof(IsFemale),
                    nameof(D),
                    nameof(E),
                    nameof(PinPitchE),
                    nameof(PinPitchD),
                    nameof(B),
                    nameof(L),
                    nameof(L1),
                    nameof(L2),
                };

                if (!isLoading && properties.Contains(e.PropertyName))
                    await GenerateItems();
            };
        }

        int numberRows = 1;

        [Display(Order = 1, Name ="Number rows")]
        [Editor(EditorNames.GenericValueEditor, EditorNames.GenericValueEditor)]
        [MarksDirty]
        public int NumberRows
        {
            get { return numberRows; }
            set
            {
                numberRows = value;
                OnPropertyChanged(nameof(NumberRows));
            }
        }

        int numberColumns = 5;

        [Display(Order = 2, Name ="Number columns")]
        [Editor(EditorNames.GenericValueEditor, EditorNames.GenericValueEditor)]
        [MarksDirty]
        public int NumberColumns
        {
            get { return numberColumns; }
            set
            {
                numberColumns = value;
                OnPropertyChanged(nameof(NumberColumns));
            }
        }

        double e = 2.54;

        [Display(Order = 3)]
        [Editor(EditorNames.SizeMilimetersUnitsEditor, EditorNames.SizeMilimetersUnitsEditor)]
        [MarksDirty]
        /// <summary>
        /// E - width
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

        double d = 12.70d;

        [Display(Order = 4)]
        [Editor(EditorNames.SizeMilimetersUnitsEditor, EditorNames.SizeMilimetersUnitsEditor)]
        [MarksDirty]
        /// <summary>
        /// D - body height
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

        double pinPitchE = 2.54d;

        [Display(Order = 5, Name ="e")]
        [Editor(EditorNames.SizeMilimetersUnitsEditor, EditorNames.SizeMilimetersUnitsEditor)]
        [MarksDirty]
        /// <summary>
        /// D - body height
        /// </summary>
        public double PinPitchE
        {
            get { return pinPitchE; }
            set
            {
                pinPitchE = value;
                OnPropertyChanged(nameof(PinPitchE));
            }
        }

        double pinPitchD = 2.54d;

        [Display(Order = 6,Name ="d")]
        [Editor(EditorNames.SizeMilimetersUnitsEditor, EditorNames.SizeMilimetersUnitsEditor)]
        [MarksDirty]
        /// <summary>
        /// D - body height
        /// </summary>
        public double PinPitchD
        {
            get { return pinPitchD; }
            set
            {
                pinPitchD = value;
                OnPropertyChanged(nameof(PinPitchD));
            }
        }

        double b = 0.6d;

        [Display(Order = 7,Name ="b")]
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

        double l = 2.54d;

        [Display(Order = 8)]
        [Editor(EditorNames.SizeMilimetersUnitsEditor, EditorNames.SizeMilimetersUnitsEditor)]
        [MarksDirty]
        public double L
        {
            get { return l; }
            set
            {
                l = value;
                OnPropertyChanged(nameof(L));
            }
        }

        double l1 = 3d;

        [Display(Order = 9)]
        [Editor(EditorNames.SizeMilimetersUnitsEditor, EditorNames.SizeMilimetersUnitsEditor)]
        [MarksDirty]
        public double L1
        {
            get { return l1; }
            set
            {
                l1 = value;
                OnPropertyChanged(nameof(L1));
            }
        }


        double l2 = 5.84d;

        [Display(Order = 10)]
        [Editor(EditorNames.SizeMilimetersUnitsEditor, EditorNames.SizeMilimetersUnitsEditor)]
        [MarksDirty]
        public double L2
        {
            get { return l2; }
            set
            {
                l2 = value;
                OnPropertyChanged(nameof(L2));
            }
        }

        bool isFemale = false;

        [Display(Order = 11)]
        [MarksDirty]
        public bool IsFemale
        {
            get { return isFemale; }
            set
            {
                isFemale = value;
                OnPropertyChanged(nameof(IsFemale));
            }
        }

        public override async Task GenerateItems()
        {
            var generator = new PinHeaderStraightPackageGenerator
            {
                D = D,
                E = E,
                B = B,
                L = L,
                NumberColumns = NumberColumns,
                NumberRows = NumberRows,
                L1 = L1,
                L2 = L2,
                PinPitchD = PinPitchD,
                PinPitchE = PinPitchE,
                IsFemale = IsFemale
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

                var r = primitive as PinHeaderStraightParametricPackage3DItem;

                D = r.D;
                E = r.E;
                B = r.B;
                L = r.L;
                NumberColumns = r.NumberColumns;
                NumberRows = r.NumberRows;
                L1 = r.L1;
                L2 = r.L2;
                PinPitchD = r.PinPitchD;
                PinPitchE = r.PinPitchE;
                IsFemale = r.IsFemale;
            }
            finally
            {
                isLoading = false;
            }

            GenerateItems().ConfigureAwait(false);
        }

        public override IPrimitive SaveToPrimitive()
        {
            var r = new PinHeaderStraightParametricPackage3DItem();
            SaveToPrimitiveBase(r);

            r.D = D;
            r.E = E;
            r.B = B;
            r.L = L;
            r.NumberColumns = NumberColumns;
            r.NumberRows = NumberRows;
            r.L1 = L1;
            r.L2 = L2;
            r.PinPitchD = PinPitchD;
            r.PinPitchE = PinPitchE;
            r.IsFemale = IsFemale;

            return r;
        }

        public override string ToString()
        {
            return "Pin Header - Straight package";
        }

    }

}
