using System;
using System.Collections.Generic;
using System.Text;

namespace IDE.Core.Types.Media
{
    public class XTranslateTransform : XTransform
    {
        public XTranslateTransform()
        {
        }

        ///<summary>
        /// Create a translation transformation.
        ///</summary>
        ///<param name="offsetX">Displacement amount in x direction.</param>
        ///<param name="offsetY">Displacement amount in y direction.</param>
        public XTranslateTransform(
            double offsetX,
            double offsetY
            )
        {
            X = offsetX;
            Y = offsetY;
        }

        ///<summary>
        /// Return the current transformation value.
        ///</summary>
        public override XMatrix Value
        {
            get
            {

                XMatrix matrix = XMatrix.Identity;

                matrix.Translate(X, Y);

                return matrix;
            }
        }

        /// <summary>
        ///     X - double.  Default value is 0.0.
        /// </summary>
        public double X
        {
            get; set;
        } = 0.0d;

        /// <summary>
        ///     Y - double.  Default value is 0.0.
        /// </summary>
        public double Y
        {
            get; set;
        } = 0.0d;

        ///<summary>
        /// Returns true if transformation matches the identity transform.
        ///</summary>
        internal override bool IsIdentity
        {
            get
            {
                return X == 0 && Y == 0;// && CanFreeze;
            }
        }

        #region Internal Methods

        internal override void TransformRect(ref XRect rect)
        {
            if (!rect.IsEmpty)
            {
                rect.Offset(X, Y);
            }
        }

        /// <summary>
        /// MultiplyValueByMatrix - result is set equal to "this" * matrixToMultiplyBy.
        /// </summary>
        /// <param name="result"> The result is stored here. </param>
        /// <param name="matrixToMultiplyBy"> The multiplicand. </param>
        internal override void MultiplyValueByMatrix(ref XMatrix result, ref XMatrix matrixToMultiplyBy)
        {
            result = XMatrix.Identity;

            // Set the translate + type
            result._offsetX = X;
            result._offsetY = Y;
            result._type = MatrixTypes.TRANSFORM_IS_TRANSLATION;

            MatrixUtil.MultiplyMatrix(ref result, ref matrixToMultiplyBy);
        }

        #endregion Internal Methods
    }
}
