using System;

namespace IDE.Core.Types.Media3D
{
    public abstract class XGeneralTransform3D
    {
        /// <summary>
        /// Constructor
        /// </summary>
        internal XGeneralTransform3D()
        {
        }

        /// <summary>
        /// Transform a point
        /// </summary>
        /// <param name="inPoint">Input point</param>
        /// <param name="result">Output point</param>
        /// <returns>True if the point was transformed successfuly, false otherwise</returns>
        public abstract bool TryTransform(XPoint3D inPoint, out XPoint3D result);

        /// <summary>
        /// Transform a point
        /// 
        /// If the transformation does not succeed, this will throw an InvalidOperationException.
        /// If you don't want to try/catch, call TryTransform instead and check the boolean it
        /// returns.
        ///
        /// </summary>
        /// <param name="point">Input point</param>
        /// <returns>The transformed point</returns>
        public XPoint3D Transform(XPoint3D point)
        {
            XPoint3D transformedPoint;

            if (!TryTransform(point, out transformedPoint))
            {
                throw new InvalidOperationException("GeneralTransform_TransformFailed");
            }

            return transformedPoint;
        }

        ///// <summary>
        ///// Transforms the bounding box to the smallest axis aligned bounding box
        ///// that contains all the points in the original bounding box
        ///// </summary>
        ///// <param name="rect">Bounding box</param>
        ///// <returns>The transformed bounding box</returns>
        //public abstract Rect3D TransformBounds(Rect3D rect);


        /// <summary>
        /// Returns the inverse transform if it has an inverse, null otherwise
        /// </summary>        
        public abstract XGeneralTransform3D Inverse { get; }

        ///// <summary>
        ///// Returns a best effort affine transform
        ///// </summary>
        //internal abstract XTransform3D AffineTransform
        //{
        //    [FriendAccessAllowed] // Built into Core, also used by Framework.
        //    get;
        //}
    }
}
