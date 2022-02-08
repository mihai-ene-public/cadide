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
    public class AxialParametricPackageMeshItem : ParametricPackageMeshItem
    {
        public AxialParametricPackageMeshItem()
        {
            PropertyChanged += async (s, e) =>
            {
                var properties = new[]
                {
                    nameof(Appearance),
                    nameof(Placement),
                    nameof(IsDiode),
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

        ComponentAppearance appearance;

        [Browsable(false)]
        public ComponentAppearance Appearance
        {
            get { return appearance; }
            set
            {
                appearance = value;
                OnPropertyChanged(nameof(Appearance));
            }
        }

        ComponentPlacement placement;

        [Browsable(false)]
        public ComponentPlacement Placement
        {
            get { return placement; }
            set
            {
                placement = value;
                OnPropertyChanged(nameof(Placement));
            }
        }




        double d = 2.54 * 3;
        /// <summary>
        /// Length of the package. Includes the length of the pads
        /// </summary>
        [Display(Order = 1)]
        [Editor(EditorNames.SizeMilimetersUnitsEditor, EditorNames.SizeMilimetersUnitsEditor)]
        [MarksDirty]
        public double D
        {
            get { return d; }
            set
            {
                d = value;
                OnPropertyChanged(nameof(D));
            }
        }

        double d1 = 2.54 * 2;

        [Display(Order = 2)]
        [Editor(EditorNames.SizeMilimetersUnitsEditor, EditorNames.SizeMilimetersUnitsEditor)]
        [MarksDirty]
        /// <summary>
        /// Length of the body cilinder
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

        double a = 4;

        [Display(Order = 3)]
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

        double e = 2.0d;

        [Display(Order = 4)]
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

        double b = 0.5d;

        [Display(Name = "b", Order = 5)]
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

        bool isDiode;

        [Display(Name = "Is diode", Order = 5)]
        [MarksDirty]
        public bool IsDiode
        {
            get { return isDiode; }
            set
            {
                isDiode = value;
                OnPropertyChanged(nameof(IsDiode));
            }
        }

        public override async Task GenerateItems()
        {
            var generator = new AxialPackageGenerator
            {
                Appearance = Appearance,
                Placement = Placement,
                IsDiode = IsDiode,
                D = D,
                D1 = D1,
                A = A,
                E = E,
                B = B
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

                var r = primitive as AxialParametricPackage3DItem;

                Appearance = r.Appearance;
                Placement = r.Placement;
                IsDiode = r.IsDiode;
                D = r.D;
                D1 = r.D1;
                A = r.A;
                E = r.E;
                B = r.B;
            }
            finally
            {
                isLoading = false;
            }

            GenerateItems().ConfigureAwait(false); //.GetAwaiter().GetResult();
        }

        public override IPrimitive SaveToPrimitive()
        {
            var r = new AxialParametricPackage3DItem();

            SaveToPrimitiveBase(r);

            r.Appearance = Appearance;
            r.Placement = Placement;
            r.IsDiode = IsDiode;
            r.D = D;
            r.D1 = D1;
            r.A = A;
            r.E = E;
            r.B = B;

            return r;
        }

        public override string ToString()
        {
            return "Axial package";
        }
    }
}
