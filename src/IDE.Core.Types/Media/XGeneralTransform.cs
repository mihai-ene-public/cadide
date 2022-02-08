using System;

namespace IDE.Core.Types.Media
{
    /// <summary>
    /// GeneralTransform class provides services to transform points and rects
    /// </summary>
    public abstract class XGeneralTransform
    {
        /// <summary>
        /// Transform a point
        /// </summary>
        /// <param name="inPoint">Input point</param>
        /// <param name="result">Output point</param>
        /// <returns>True if the point was transformed successfuly, false otherwise</returns>
        public abstract bool TryTransform(XPoint inPoint, out XPoint result);

        /// <summary>
        /// Transform a point
        /// 
        /// If the transformation does not succeed, this will throw an InvalidOperationException.
        /// If you don't want to try/catch, call TryTransform instead and check the boolean it
        /// returns.
        ///
        /// Note that this method will always succeed when called on a subclass of Transform
        /// </summary>
        /// <param name="point">Input point</param>
        /// <returns>The transformed point</returns>
        public XPoint Transform(XPoint point)
        {
            XPoint transformedPoint;

            if (!TryTransform(point, out transformedPoint))
            {
                throw new InvalidOperationException();// SR.Get(SRID.GeneralTransform_TransformFailed, null));
            }

            return transformedPoint;
        }

        /// <summary>
        /// Transforms the bounding box to the smallest axis aligned bounding box
        /// that contains all the points in the original bounding box
        /// </summary>
        /// <param name="rect">Bounding box</param>
        /// <returns>The transformed bounding box</returns>
        public abstract XRect TransformBounds(XRect rect);


        /// <summary>
        /// Returns the inverse transform if it has an inverse, null otherwise
        /// </summary>        
        public abstract XGeneralTransform Inverse { get; }

        /// <summary>
        /// Returns a best effort affine transform
        /// </summary>
        internal virtual XTransform AffineTransform
        {
          //  [FriendAccessAllowed] // Built into Core, also used by Framework.
            get { return null; }
        }
    }
}
