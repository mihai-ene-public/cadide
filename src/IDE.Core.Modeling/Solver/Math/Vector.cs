using IDE.Core.Modeling.Solver.Constraints.Primitives;
using System;
using System.Collections.Generic;

namespace IDE.Core.Modeling.Solver.Maths
{
    public class Vector : List<double>
    {
        public int Length
        {
            get { return base.Count; }
        }

        public void Set(int pos, double value)
        {
            base[pos] = value;
        }

        public Vector(int capacity)
        {
            for (int i = 0; i < capacity; i++)
            {
                base.Add(0.0);
            }
        }

        public Vector(List<double> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                base.Add(list[i]);
            }
        }

        public Vector(List<DoubleRefObject> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                base.Add(list[i].Value);
            }
        }

        public double Norm()
        {
            double norm = 0;
            for (int i = 0; i < base.Count; i++)
            {
                var element = base[i];

                norm += element * element;
            }
            return Math.Sqrt(norm);
        }

        public double SquaredNorm()
        {
            double norm = 0;
            for (int i = 0; i < base.Count; i++)
            {
                var element = base[i];
                norm += element * element;
            }
            return norm;
        }

        public static Vector operator +(Vector l, Vector r)
        {
            var max = Math.Max(l.Length, r.Length);
            var result = new Vector(max);
            if (l.Length <= r.Length)
            {
                for (int i = 0; i < l.Length; i++)
                {
                    result[i] = l[i] + r[i];
                }
                for (int i = l.Length; i < r.Length; i++)
                {
                    result[i] = r[i];
                }
            }
            else
            {
                for (int i = 0; i < r.Length; i++)
                {
                    result[i] = l[i] + r[i];
                }
                for (int i = r.Length; i < l.Length; i++)
                {
                    result[i] = l[i];
                }
            }
            return result;
        }

        public static Vector operator *(Vector l, double val)
        {

            var result = new Vector(l.Length);

            for (int i = 0; i < l.Length; i++)
            {
                result[i] = l[i] * val;
            }
            return result;
        }

        public static double operator *(Vector l, Vector r)
        {
            if (l.Length != r.Length)
                throw new IndexOutOfRangeException("The two vectors should have the same length");
            var result = 0.0;

            for (int i = 0; i < l.Length; i++)
            {
                result += l[i] * r[i];
            }
            return result;
        }

        public static Vector operator *(double val, Vector l)
        {

            var result = new Vector(l.Length);

            for (int i = 0; i < l.Length; i++)
            {
                result[i] = l[i] * val;
            }
            return result;
        }

        public static Vector operator -(Vector l, Vector r)
        {
            var max = Math.Max(l.Length, r.Length);
            var result = new Vector(max);
            if (l.Length <= r.Length)
            {
                for (int i = 0; i < l.Length; i++)
                {
                    result[i] = l[i] - r[i];
                }
                for (int i = l.Length; i < r.Length; i++)
                {
                    result[i] = -r[i];
                }
            }
            else
            {
                for (int i = 0; i < r.Length; i++)
                {
                    result[i] = l[i] - r[i];
                }
                for (int i = r.Length; i < l.Length; i++)
                {
                    result[i] = l[i];
                }
            }
            return result;
        }

        public static Vector operator -(Vector l)
        {
            var result = new Vector(l.Length);
            for (int i = 0; i < l.Length; i++)
            {
                result[i] = -l[i];
            }
            return result;
        }

        public static bool operator ==(Vector l, Vector r)
        {
            if (l.Length != r.Length)
                return false;
            for (int i = 0; i < l.Length; i++)
                if (l[i] - r[i] > 1e-12)
                    return false;
            return true;
        }

        public static Vector operator *(Matrix l, Vector r)
        {
            if (l.ColCount != r.Length)
                throw new Exception("Can't multiply matrix and vector if they don't have the correct dimensions!");

            var C = new Vector(l.RowCount);

            for (int i = 0; i < l.RowCount; i++)
            {
                C[i] = 0;
                for (int k = 0; k < r.Length; k++)
                {
                    C[i] += l[i, k] * r[k];
                }
            }
            return C;
        }

        public static bool operator !=(Vector l, Vector r)
        {
            if (l.Length != r.Length)
                return true;
            for (int i = 0; i < l.Length; i++)
                if (l[i] - r[i] > 1e-12)
                    return true;
            return false;
        }

        public static double InnerProduct(Vector l, Vector r)
        {
            var result = 0.0;
            if (l.Length != r.Length)
                throw new Exception("The vectors should have the same length!");
            for (int i = 0; i < l.Length; i++)
            {
                result += l[i] * r[i];
            }
            return result;
        }

        public static Matrix OuterProduct(Vector l, Vector r)
        {
            if (l.Length != r.Length)
                throw new Exception("The vectors should have the same length!");
            var result = new Matrix(l.Length, r.Length);
            for (int i = 0; i < l.Length; i++)
            {
                for (int j = 0; j < l.Length; j++)
                {
                    result[i, j] = l[i] * r[j];
                }
            }
            return result;
        }

        private bool Equals(Vector other)
        {
            return !ReferenceEquals(null, other);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(Vector)) return false;
            return Equals((Vector)obj);
        }

        public override int GetHashCode()
        {
            return 0;
        }
    }
}
