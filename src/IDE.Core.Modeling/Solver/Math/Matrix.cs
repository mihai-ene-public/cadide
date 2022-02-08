using System;
using System.Text;

namespace IDE.Core.Modeling.Solver.Maths
{
    public class Matrix
    {
        private double[][] matrix;
        private int rowCount;
        private int colCount;

        public int RowCount
        {
            get { return rowCount; }
        }

        public int ColCount
        {
            get { return colCount; }
        }

        public Matrix(int rowsCount, int columnsCount)
        {
            rowCount = rowsCount;
            colCount = columnsCount;
            matrix = new double[rowsCount][];
            for (int i = 0; i < rowsCount; i++)
            {
                matrix[i] = new double[columnsCount];
            }
        }

        public Matrix(Matrix B)
            : this(B.rowCount, B.colCount)
        {
            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < colCount; j++)
                {
                    matrix[i][j] = B.matrix[i][j];
                }
            }
        }

        public Matrix(int rowsCount, int columnsCount, int value)
            : this(rowsCount, columnsCount)
        {
            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < colCount; j++)
                {
                    matrix[i][j] = value;
                }
            }
        }

        public double this[int row, int col]
        {
            get { return matrix[row][col]; }
            set { matrix[row][col] = value; }
        }


        public void Set(int row, int column, double value)
        {
            matrix[row][column] = value;
        }

        public double Norm()
        {
            var sum = 0.0;
            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < colCount; j++)
                {
                    sum += matrix[i][j] * matrix[i][j];
                }
            }
            return Math.Sqrt(sum);
        }

        public Matrix Transpose()
        {
            var transposedMatrix = new Matrix(colCount, rowCount);
            for (int i = 0; i < colCount; i++)
            {
                for (int j = 0; j < rowCount; j++)
                {
                    transposedMatrix.matrix[i][j] = matrix[j][i];
                }
            }
            return transposedMatrix;
        }

        public static Matrix operator +(Matrix l, Matrix r)
        {
            if (l.rowCount != r.rowCount || l.colCount != r.colCount)
                throw new Exception("Can't add matrices if they don't have the same dimension!");
            var result = new Matrix(l.rowCount, l.colCount);
            for (int i = 0; i < l.rowCount; i++)
            {
                for (int j = 0; j < l.colCount; j++)
                    result[i, j] = l.matrix[i][j] + r.matrix[i][j];
            }
            return result;
        }

        public static Matrix operator -(Matrix l, Matrix r)
        {
            if (l.rowCount != r.rowCount || l.colCount != r.colCount)
                throw new Exception("Can't substract matrices if they don't have the same dimension!");
            var result = new Matrix(l.rowCount, l.colCount);
            for (int i = 0; i < l.rowCount; i++)
            {
                for (int j = 0; j < l.colCount; j++)
                    result[i, j] = l.matrix[i][j] - r.matrix[i][j];
            }
            return result;
        }

        public static Matrix operator *(Matrix l, Matrix r)
        {
            if (l.colCount != r.rowCount)
                throw new Exception("Can't multiply matrices if they don't have the correct dimensions!");

            Matrix C = new Matrix(l.rowCount, r.colCount);

            for (int i = 0; i < l.rowCount; i++)
            {
                for (int j = 0; j < r.colCount; j++)
                {
                    C.matrix[i][j] = 0;
                    for (int k = 0; k < l.colCount; k++)
                    {
                        C.matrix[i][j] += l.matrix[i][k] * r.matrix[k][j];
                    }
                }
            }
            return C;
        }


        public static Matrix Identity(int rowCount, int colCount)
        {
            var A = new Matrix(rowCount, colCount);

            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < colCount; j++)
                {
                    A.matrix[i][j] = (i == j ? 1.0 : 0.0);
                }
            }
            return A;
        }

        public static Matrix operator *(Matrix l, double val)
        {

            Matrix C = new Matrix(l.rowCount, l.colCount);

            for (int i = 0; i < l.colCount; i++)
            {
                for (int j = 0; j < l.rowCount; j++)
                {
                    C.matrix[i][j] = l.matrix[i][j] * val;
                }
            }
            return C;
        }

        public Matrix(double[] vals, int m)
        {
            this.rowCount = m;
            colCount = (m != 0 ? vals.Length / m : 0);
            var n = colCount;
            if (m * n != vals.Length)
            {
                throw new System.ArgumentException("Array length must be a multiple of m.");
            }
            matrix = new double[m][];
            for (int i = 0; i < m; i++)
            {
                matrix[i] = new double[n];
            }
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    matrix[i][j] = vals[i + j * m];
                }
            }
        }
    }
}
