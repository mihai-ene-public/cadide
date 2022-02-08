using IDE.Core.Interfaces;
using IDE.Core.Storage;
using IDE.Core.Types.Attributes;
using IDE.Documents.Views;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IDE.Core.Designers
{
    public class SOT223ParametricPackageMeshItem : ParametricPackageMeshItem
    {
        public SOT223ParametricPackageMeshItem() 
        {
            PropertyChanged += async (s, e) =>
            {
                var properties = new[]
                {
                    nameof(NumberPads),
                    nameof(EE),
                    nameof(D),
                    nameof(A),
                    nameof(A1),
                    nameof(E),
                    nameof(E1),
                    nameof(B),
                    nameof(B1),
                    nameof(L),
                };

                if (!isLoading && properties.Contains(e.PropertyName))
                    await GenerateItems();
            };
        }

        int numberPads = 4;

        [Browsable(false)]
        [Display(Order = 1,Name ="Number of pads")]
        [Editor(EditorNames.GenericValueEditor, EditorNames.GenericValueEditor)]
        [MarksDirty]
        public int NumberPads
        {
            get { return numberPads; }
            set
            {
                numberPads = value;
                OnPropertyChanged(nameof(NumberPads));
            }
        }

        [Browsable(false)]
        public List<int> NumberPadsAvailable => new List<int> { 4 };

        double d = 3.0d;

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

        double e = 2.3d;

        [Display(Order = 3)]
        [Editor(EditorNames.SizeMilimetersUnitsEditor, EditorNames.SizeMilimetersUnitsEditor)]
        [MarksDirty]
        public double E
        {
            get { return e; }
            set
            {
                e = value;
                OnPropertyChanged(nameof(E));
            }
        }

        double e1 = 1.4;

        [Display(Order = 4)]
        [Editor(EditorNames.SizeMilimetersUnitsEditor, EditorNames.SizeMilimetersUnitsEditor)]
        [MarksDirty]
        /// <summary>
        /// Body width
        /// </summary>
        public double E1
        {
            get { return e1; }
            set
            {
                e1 = value;
                OnPropertyChanged(nameof(E1));
            }
        }

        double a = 1.12d;

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

        double a1 = 0.1;

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

        double ee = 1.03d;

        [Display(Order = 7,Name ="e")]
        [Editor(EditorNames.SizeMilimetersUnitsEditor, EditorNames.SizeMilimetersUnitsEditor)]
        [MarksDirty]
        /// <summary>
        /// Pin pitch
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

        double l = 0.25d;

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

        double b = 0.5d;

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

        double b1 = 1.2d;

        [Display(Order = 10,Name ="b1")]
        [Editor(EditorNames.SizeMilimetersUnitsEditor, EditorNames.SizeMilimetersUnitsEditor)]
        [MarksDirty]
        /// <summary>
        /// Pad width in mm
        /// </summary>
        public double B1
        {
            get { return b1; }
            set
            {
                b1 = value;
                OnPropertyChanged(nameof(B1));
            }
        }

       

        public override async Task GenerateItems()
        {
            var generator = new SOT223PackageGenerator
            {
                D = D,
                E1 = E1,
                A = A,
                A1 = A1,
                E = E,
                B = B,
                B1 = B1, 
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

                var r = primitive as SOT223ParametricPackage3DItem;

                D = r.D;
                E1 = r.E1;
                A = r.A;
                A1 = r.A1;
                E = r.E;
                B = r.B;
                B1 = r.B1;
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
            var r = new SOT223ParametricPackage3DItem();
            SaveToPrimitiveBase(r);

            r.D = D;
            r.E1 = E1;
            r.A = A;
            r.A1 = A1;
            r.E = E;
            r.B = B;
            r.B1 = B1;
            r.EE = EE;
            r.L = L;
            r.NumberPads = NumberPads;

            return r;
        }

        public override string ToString()
        {
            return "SOT223 package";
        }

    }

}
