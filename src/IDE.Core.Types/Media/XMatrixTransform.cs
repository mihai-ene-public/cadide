using System;
using System.Collections.Generic;
using System.Text;

namespace IDE.Core.Types.Media
{
    public class XMatrixTransform : XTransform
    {
        #region Constructors

        ///<summary>
        ///
        ///</summary>
        public XMatrixTransform()
        {
        }

        ///<summary>
        /// Create an arbitrary matrix transformation.
        ///</summary>
        ///<param name="m11">Matrix value at position 1,1</param>
        ///<param name="m12">Matrix value at position 1,2</param>
        ///<param name="m21">Matrix value at position 2,1</param>
        ///<param name="m22">Matrix value at position 2,2</param>
        ///<param name="offsetX">Matrix value at position 3,1</param>
        ///<param name="offsetY">Matrix value at position 3,2</param>
        public XMatrixTransform(
            double m11,
            double m12,
            double m21,
            double m22,
            double offsetX,
            double offsetY
            )
        {
            Matrix = new XMatrix(m11, m12, m21, m22, offsetX, offsetY);
        }

        ///<summary>
        /// Create a matrix transformation from constant transform.
        ///</summary>
        ///<param name="matrix">The constant matrix transformation.</param>
        public XMatrixTransform(XMatrix matrix)
        {
            Matrix = matrix;
        }

        #endregion

        ///<summary>
        /// Return the current transformation value.
        ///</summary>
        public override XMatrix Value
        {
            get
            {
                //  ReadPreamble();

                return Matrix;
            }
        }

        public XMatrix Matrix { get; set; } = new XMatrix();

        #region Internal Methods

        ///<summary>
        /// Returns true if transformation matches the identity transform.
        ///</summary>
        internal override bool IsIdentity
        {
            get
            {
                return Matrix.IsIdentity;// && CanFreeze;
            }
        }

        internal override bool CanSerializeToString() { return true; }

        ///// <summary>
        ///// Creates a string representation of this object based on the format string 
        ///// and IFormatProvider passed in.  
        ///// If the provider is null, the CurrentCulture is used.
        ///// See the documentation for IFormattable for more information.
        ///// </summary>
        ///// <returns>
        ///// A string representation of this object.
        ///// </returns>
        //internal override string ConvertToString(string format, IFormatProvider provider)
        //{
        //    if (!CanSerializeToString())
        //    {
        //        return base.ConvertToString(format, provider);
        //    }

        //    return ((IFormattable)Matrix).ToString(format, provider);
        //}

        internal override void TransformRect(ref XRect rect)
        {
            XMatrix matrix = Matrix;
            MatrixUtil.TransformRect(ref rect, ref matrix);
        }

        internal override void MultiplyValueByMatrix(ref XMatrix result, ref XMatrix matrixToMultiplyBy)
        {
            result = Matrix;
            MatrixUtil.MultiplyMatrix(ref result, ref matrixToMultiplyBy);
        }

        internal static XMatrix s_Matrix = new XMatrix();
        #endregion Internal Methods
    }
}
