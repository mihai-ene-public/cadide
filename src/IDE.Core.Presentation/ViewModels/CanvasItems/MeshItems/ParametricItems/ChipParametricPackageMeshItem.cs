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
    public class ChipParametricPackageMeshItem : ParametricPackageMeshItem
    {
        public ChipParametricPackageMeshItem()
        {
            PropertyChanged += async (s, e) =>
            {
                var properties = new[]
                {
                    nameof(D),
                    nameof(L),
                    nameof(A),
                    nameof(E),
                    nameof(IsCapacitor),
                };

                if (!isLoading && properties.Contains(e.PropertyName))
                    await GenerateItems();
            };
        }





        double d = 1.9;

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

        double a = 0.7;

        [Display(Order = 2)]
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

        double e = 1.0d;

        [Display(Order = 3)]
        [Editor(EditorNames.SizeMilimetersUnitsEditor, EditorNames.SizeMilimetersUnitsEditor)]
        [MarksDirty]
        /// <summary>
        /// Width of the package
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

        double l = 0.3;

        [Display(Order = 4)]
        [Editor(EditorNames.SizeMilimetersUnitsEditor, EditorNames.SizeMilimetersUnitsEditor)]
        [MarksDirty]
        /// <summary>
        /// Length of pad 1 in mm
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

        bool isCapacitor = false;

        [Display(Order = 5)]
        [MarksDirty]
        public bool IsCapacitor
        {
            get { return isCapacitor; }
            set
            {
                isCapacitor = value;
                OnPropertyChanged(nameof(IsCapacitor));
            }
        }

        public override async Task GenerateItems()
        {
            var generator = new ChipPackageGenerator
            {
                D = D,
                A = A,
                E = E,
                L = L,
                IsCapacitor = IsCapacitor
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

                var r = primitive as ChipParametricPackage3DItem;

                D = r.D;
                L = r.L;
                A = r.A;
                E = r.E;
                IsCapacitor = r.IsCapacitor;
            }
            finally
            {
                isLoading = false;
            }

            GenerateItems().ConfigureAwait(false);
        }

        public override IPrimitive SaveToPrimitive()
        {
            var r = new ChipParametricPackage3DItem();

            SaveToPrimitiveBase(r);

            r.D = D;
            r.L = L;
            r.A = A;
            r.E = E;
            r.IsCapacitor = IsCapacitor;

            return r;
        }

        public override string ToString()
        {
            return "Chip package";
        }

    }

}
