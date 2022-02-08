using IDE.Core.Designers;
using IDE.Core.Types.Media;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IDE.Documents.Views
{
    public class BGAPackageGenerator : PackageGenerator
    {
        public override string Name => "BGA";


        int numberRows = 4;

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

        double d1 = 0.5;

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

        double d = 3.0d;
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



        double b = 0.3d;
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

        //double a1 = 0.23;
        ///// <summary>
        ///// Body offset
        ///// </summary>
        //public double A1
        //{
        //    get { return a1; }
        //    set
        //    {
        //        a1 = value;
        //        OnPropertyChanged(nameof(A1));
        //    }
        //}


        public override Task<List<BaseMeshItem>> GeneratePackage()
        {
            return Task.Run(() =>
            {
                var meshItems = new List<BaseMeshItem>();

                //body
                meshItems.Add(new BoxMeshItem
                {
                    X = 0,
                    Y = 0,
                    Z = 0.5 * (A - 0.5 * B) + 0.5 * B,
                    Width = E,
                    Height = A - 0.5 * B,
                    Length = D,
                    CanEdit = false,
                    IsPlaced = false,
                    FillColor = XColors.Black
                });

                var rowOffset = 0.5 * (E - (numberRows - 1) * ee);
                var colOffset = 0.5 * (d - (numberColumns - 1) * d1);

                //balls
                for (int rowIndex = 0; rowIndex < NumberRows; rowIndex++)
                {
                    var z = 0.5 * b;
                    var y = -0.5 * d + colOffset + rowIndex * d1;
                    for (int colIndex = 0; colIndex < NumberColumns; colIndex++)
                    {
                        var x = -0.5 * e + rowOffset + colIndex * ee;

                        meshItems.Add(new SphereMeshItem
                        {
                            X = x,
                            Y = y,
                            Z = z,
                            Radius = 0.5 * b,
                            FillColor = XColors.Silver,
                            IsPlaced = false,
                            CanEdit = false,

                        });
                    }
                }

                return meshItems;
            });
        }
    }
}
