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
    public class BGAParametricPackageMeshItem : ParametricPackageMeshItem
    {
        public BGAParametricPackageMeshItem()
        {
            PropertyChanged += async (s, e) =>
            {
                var properties = new[]
                {
                    nameof(NumberRows),
                    nameof(NumberColumns),
                    nameof(EE),
                    nameof(D),
                    nameof(D1),
                    nameof(A),
                    nameof(E),
                    nameof(B),
                };

                if (!isLoading && properties.Contains(e.PropertyName))
                    await GenerateItems();
            };
        }

        int numberRows = 4;

        [Display(Order = 1, Name = "Number rows")]
        [Editor(EditorNames.GenericValueEditor, EditorNames.GenericValueEditor)]
        public int NumberRows
        {
            get { return numberRows; }
            set
            {
                numberRows = value;
                OnPropertyChanged(nameof(NumberRows));
            }
        }

        int numberColumns = 4;

        [Display(Order = 2, Name = "Number columns")]
        [Editor(EditorNames.GenericValueEditor, EditorNames.GenericValueEditor)]
        public int NumberColumns
        {
            get { return numberColumns; }
            set
            {
                numberColumns = value;
                OnPropertyChanged(nameof(NumberColumns));
            }
        }

        double e = 3.0d;

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

        double d = 3.0d;

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

        double ee = 0.5d;

        [Display(Order = 5, Name = "e")]
        [Editor(EditorNames.SizeMilimetersUnitsEditor, EditorNames.SizeMilimetersUnitsEditor)]
        [MarksDirty]
        /// <summary>
        /// Lower e - pitch between columns
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


        double d1 = 0.5;

        [Display(Order = 6, Name = "d")]
        [Editor(EditorNames.SizeMilimetersUnitsEditor, EditorNames.SizeMilimetersUnitsEditor)]
        [MarksDirty]
        /// <summary>
        /// d - pitch between rows
        /// </summary>
        public double D1 //d
        {
            get { return d1; }
            set
            {
                d1 = value;
                OnPropertyChanged(nameof(D1));
            }
        }

        double b = 0.3d;

        [Display(Order = 7, Name = "b")]
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

        double a = 1.0d;

        [Display(Order = 8)]
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

        public override async Task GenerateItems()
        {
            var generator = new BGAPackageGenerator
            {
                D = D,
                D1 = D1,
                A = A,
                E = E,
                B = B,
                EE = EE,
                NumberColumns = NumberColumns,
                NumberRows = NumberRows
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

                var r = primitive as BGAParametricPackage3DItem;

                D = r.D;
                D1 = r.D1;
                A = r.A;
                E = r.E;
                B = r.B;
                EE = r.EE;
                NumberColumns = r.NumberColumns;
                NumberRows = r.NumberRows;
            }
            finally
            {
                isLoading = false;
            }

            GenerateItems().ConfigureAwait(false);
        }

        public override IPrimitive SaveToPrimitive()
        {
            var r = new BGAParametricPackage3DItem();

            SaveToPrimitiveBase(r);

            r.D = D;
            r.D1 = D1;
            r.A = A;
            r.E = E;
            r.B = B;
            r.EE = EE;
            r.NumberColumns = NumberColumns;
            r.NumberRows = NumberRows;

            return r;
        }

        public override string ToString()
        {
            return "BGA package";
        }

    }

}
