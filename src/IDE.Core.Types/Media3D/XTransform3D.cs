namespace IDE.Core.Types.Media3D
{
    public abstract class XTransform3D:XGeneralTransform3D
    {
        // Prevent 3rd parties from extending this abstract base class.
        internal XTransform3D() { }

        #region Public Methods

        /// <summary>
        ///     Transforms the given point.
        /// </summary>
        /// <param name="point">Point to transform.</param>
        /// <returns>Transformed point.</returns>
        public new XPoint3D Transform(XPoint3D point)
        {
            // this function is included due to forward compatability reasons            
            return base.Transform(point);
        }


        /// <summary>
        ///     Transforms the given vector.
        /// </summary>
        /// <param name="vector">Vector to transform.</param>
        /// <returns>Transformed vector.</returns>
        public XVector3D Transform(XVector3D vector)
        {
            return Value.Transform(vector);
        }

        ///// <summary>
        /////     Transforms the given point.
        ///// </summary>
        ///// <param name="point">Point to transform.</param>
        ///// <returns>Transformed point.</returns>
        //public Point4D Transform(Point4D point)
        //{
        //    return Value.Transform(point);
        //}

        /// <summary>
        ///     Transforms the given list of points.
        /// </summary>
        /// <param name="points">List of points.</param>
        public void Transform(XPoint3D[] points)
        {
            Value.Transform(points);
        }

        /// <summary>
        ///     Transforms the given list of vectors.
        /// </summary>
        /// <param name="vectors">List of vectors.</param>
        public void Transform(XVector3D[] vectors)
        {
            Value.Transform(vectors);
        }

        ///// <summary>
        /////     Transforms the given list of points.
        ///// </summary>
        ///// <param name="points">List of points.</param>
        //public void Transform(Point4D[] points)
        //{
        //    Value.Transform(points);
        //}

        /// <summary>
        /// Transform a point
        /// </summary>
        /// <param name="inPoint">Input point</param>
        /// <param name="result">Output point</param>
        /// <returns>True if the point was transformed successfuly, false otherwise</returns>
        public override bool TryTransform(XPoint3D inPoint, out XPoint3D result)
        {
            result = Value.Transform(inPoint);
            return true;
        }

        ///// <summary>
        ///// Transforms the bounding box to the smallest axis aligned bounding box
        ///// that contains all the points in the original bounding box
        ///// </summary>
        ///// <param name="rect">Bounding box</param>
        ///// <returns>The transformed bounding box</returns>
        //public override XRect3D TransformBounds(XRect3D rect)
        //{
        //    return M3DUtil.ComputeTransformedAxisAlignedBoundingBox(ref rect, this);
        //}

        /// <summary>
        /// Returns the inverse transform if it has an inverse, null otherwise
        /// </summary>        
        public override XGeneralTransform3D Inverse
        {
            get
            {

                var matrix = Value;

                if (!matrix.HasInverse)
                {
                    return null;
                }

                matrix.Invert();
                return new XMatrixTransform3D(matrix);
            }
        }

        ///// <summary>
        ///// Returns a best effort affine transform
        ///// </summary>
        //internal override Transform3D AffineTransform
        //{
        //    [FriendAccessAllowed] // Built into Core, also used by Framework.
        //    get
        //    {
        //        return this;
        //    }
        //}

        #endregion Public Methods

        //------------------------------------------------------
        //
        //  Public Properties
        //
        //------------------------------------------------------

        #region Public Properties

        /// <summary>
        ///     Identity transformation.
        /// </summary>
        public static XTransform3D Identity
        {
            get
            {
                // Make sure identity matrix is initialized.
                if (s_identity == null)
                {
                    var identity = new XMatrixTransform3D();
                    s_identity = identity;
                }
                return s_identity;
            }
        }

        ///// <summary>
        /////     Determines whether the matrix is affine.
        ///// </summary>
        //public abstract bool IsAffine { get; }


        /// <summary>
        ///     Return the current transformation value.
        /// </summary>
        public abstract XMatrix3D Value { get; }

        #endregion Public Properties

        internal abstract void Append(ref XMatrix3D matrix);

        //------------------------------------------------------
        //
        //  Private Fields
        //
        //------------------------------------------------------

        #region Private Fields

        private static XTransform3D s_identity;

        #endregion Private Fields
    }
}
