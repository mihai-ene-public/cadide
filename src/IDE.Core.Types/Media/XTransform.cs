using System.Collections.Generic;
using System.Text;

namespace IDE.Core.Types.Media
{

    public abstract class XTransform : XGeneralTransform
    {
        internal XTransform()
        {
        }

        ///<summary>
        /// Identity transformation.
        ///</summary>
        public static XTransform Identity
        {
            get
            {
                return s_identity;
            }
        }

        private static XTransform MakeIdentityTransform()
        {
            XTransform identity = new XMatrixTransform(XMatrix.Identity);
            //identity.Freeze();
            return identity;
        }

        private static XTransform s_identity = MakeIdentityTransform();

        ///<summary>
        /// Return the current transformation value.
        ///</summary>
        public abstract XMatrix Value { get; }

        ///<summary>
        /// Returns true if transformation if the transformation is definitely an identity.  There are cases where it will
        /// return false because of computational error or presence of animations (And we're interpolating through a
        /// transient identity) -- this is intentional.  This property is used internally only.  If you need to check the
        /// current matrix value for identity, use Transform.Value.Identity.
        ///</summary>
        internal abstract bool IsIdentity { get; }

        internal virtual bool CanSerializeToString() { return false; }

        #region Perf Helpers

        internal virtual void TransformRect(ref XRect rect)
        {
            XMatrix matrix = Value;
            MatrixUtil.TransformRect(ref rect, ref matrix);
        }

        /// <summary>
        /// MultiplyValueByMatrix - result is set equal to "this" * matrixToMultiplyBy.
        /// </summary>
        /// <param name="result"> The result is stored here. </param>
        /// <param name="matrixToMultiplyBy"> The multiplicand. </param>
        internal virtual void MultiplyValueByMatrix(ref XMatrix result, ref XMatrix matrixToMultiplyBy)
        {
            result = Value;
            MatrixUtil.MultiplyMatrix(ref result, ref matrixToMultiplyBy);
        }

        ///// <SecurityNote>
        ///// Critical -- references and writes out to memory addresses. The
        /////             caller is safe if the pointer points to a D3DMATRIX
        /////             value.
        ///// </SecurityNote>
        //[SecurityCritical]
        //internal unsafe virtual void ConvertToD3DMATRIX(/* out */ D3DMATRIX* milMatrix)
        //{
        //    Matrix matrix = Value;
        //    MILUtilities.ConvertToD3DMATRIX(&matrix, milMatrix);
        //}

        #endregion

        /// <summary>
        /// Consolidates the common logic of obtain the value of a 
        /// Transform, after checking the transform for null.
        /// </summary>
        /// <param name="transform"> Transform to obtain value of. </param>
        /// <param name="currentTransformValue"> 
        ///     Current value of 'transform'.  Matrix.Identity if
        ///     the 'transform' parameter is null.
        /// </param>
        internal static void GetTransformValue(
            XTransform transform,
            out XMatrix currentTransformValue
            )
        {
            if (transform != null)
            {
                currentTransformValue = transform.Value;
            }
            else
            {
                currentTransformValue = XMatrix.Identity;
            }
        }

        /// <summary>
        /// Transforms a point
        /// </summary>
        /// <param name="inPoint">Input point</param>
        /// <param name="result">Output point</param>
        /// <returns>True if the point was successfully transformed</returns>
        public override bool TryTransform(XPoint inPoint, out XPoint result)
        {
            XMatrix m = Value;
            result = m.Transform(inPoint);
            return true;
        }

        /// <summary>
        /// Transforms the bounding box to the smallest axis aligned bounding box
        /// that contains all the points in the original bounding box
        /// </summary>
        /// <param name="rect">Bounding box</param>
        /// <returns>The transformed bounding box</returns>
        public override XRect TransformBounds(XRect rect)
        {
            TransformRect(ref rect);
            return rect;
        }


        /// <summary>
        /// Returns the inverse transform if it has an inverse, null otherwise
        /// </summary>        
        public override XGeneralTransform Inverse
        {
            get
            {
                //  ReadPreamble();

                XMatrix matrix = Value;

                if (!matrix.HasInverse)
                {
                    return null;
                }

                matrix.Invert();
                return new XMatrixTransform(matrix);
            }
        }

        /// <summary>
        /// Returns a best effort affine transform
        /// </summary>        
        internal override XTransform AffineTransform
        {
            //  [FriendAccessAllowed] // Built into Core, also used by Framework.
            get
            {
                return this;
            }
        }
    }
}
