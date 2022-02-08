using System;
using System.Collections.Generic;
using System.Text;

namespace IDE.Core.Types.Media
{
   public class XScaleTransform:XTransform
    {

        ///<summary>
        /// Create a scale transformation.
        ///</summary>
        public XScaleTransform()
        {
        }

        ///<summary>
        /// Create a scale transformation.
        ///</summary>
        public XScaleTransform(
            double scaleX,
            double scaleY
            )
        {
            ScaleX = scaleX;
            ScaleY = scaleY;
        }

        ///<summary>
        /// Create a scale transformation.
        ///</summary>
        public XScaleTransform(
            double scaleX,
            double scaleY,
            double centerX,
            double centerY
            ) : this(scaleX, scaleY)
        {
            CenterX = centerX;
            CenterY = centerY;
        }

        /// <summary>
        ///     ScaleX - double.  Default value is 1.0.
        /// </summary>
        public double ScaleX
        {
            get; set;
        } = 1.0;

        /// <summary>
        ///     ScaleY - double.  Default value is 1.0.
        /// </summary>
        public double ScaleY
        {
            get; set;
        } = 1.0;

        /// <summary>
        ///     CenterX - double.  Default value is 0.0.
        /// </summary>
        public double CenterX
        {
            get; set;
        } = 0.0;

        /// <summary>
        ///     CenterY - double.  Default value is 0.0.
        /// </summary>
        public double CenterY
        {
            get; set;
        } = 0.0;

        ///<summary>
        /// Return the current transformation value.
        ///</summary>
        public override XMatrix Value
        {
            get
            {
                XMatrix m = new XMatrix();

                m.ScaleAt(ScaleX, ScaleY, CenterX, CenterY);

                return m;
            }
        }

        ///<summary>
        /// Returns true if transformation matches the identity transform.
        ///</summary>
        internal override bool IsIdentity
        {
            get
            {
                return ScaleX == 1 && ScaleY == 1;//&& CanFreeze;
            }
        }

        internal override void TransformRect(ref XRect rect)
        {
            if (rect.IsEmpty)
            {
                return;
            }

            double scaleX = ScaleX;
            double scaleY = ScaleY;
            double centerX = CenterX;
            double centerY = CenterY;

            bool translateCenter = centerX != 0 || centerY != 0;

            if (translateCenter)
            {
                rect.X -= centerX;
                rect.Y -= centerY;
            }

            rect.Scale(scaleX, scaleY);

            if (translateCenter)
            {
                rect.X += centerX;
                rect.Y += centerY;
            }
        }

        /// <summary>
        /// MultiplyValueByMatrix - *result is set equal to "this" * matrixToMultiplyBy.
        /// </summary>
        /// <param name="result"> The result is stored here. </param>
        /// <param name="matrixToMultiplyBy"> The multiplicand. </param>
        internal override void MultiplyValueByMatrix(ref XMatrix result, ref XMatrix matrixToMultiplyBy)
        {
            result = XMatrix.Identity;

            result._m11 = ScaleX;
            result._m22 = ScaleY;
            double centerX = CenterX;
            double centerY = CenterY;

            result._type = MatrixTypes.TRANSFORM_IS_SCALING;

            if (centerX != 0 || centerY != 0)
            {
                result._offsetX = centerX - centerX * result._m11;
                result._offsetY = centerY - centerY * result._m22;
                result._type |= MatrixTypes.TRANSFORM_IS_TRANSLATION;
            }

            MatrixUtil.MultiplyMatrix(ref result, ref matrixToMultiplyBy);
        }
    }
}
