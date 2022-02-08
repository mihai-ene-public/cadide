namespace IDE.Core.Types.Media3D
{
    public class XMatrixTransform3D : XTransform3D
    {
        #region Constructors

        /// <summary>
        ///     Default constructor.
        /// </summary>
        public XMatrixTransform3D()
        {
        }

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="matrix">Matrix.</param>
        public XMatrixTransform3D(XMatrix3D matrix)
        {
            Matrix = matrix;
        }

        #endregion Constructors

        #region Public Properties

        /// <summary>
        ///     Retrieves matrix representation of transform.
        /// </summary>
        public override XMatrix3D Value
        {
            get
            {
                return Matrix;
            }
        }

        public XMatrix3D Matrix { get; set; } = new XMatrix3D();

        internal static XMatrix3D s_Matrix = XMatrix3D.Identity;

        #endregion Public Properties

        internal override void Append(ref XMatrix3D matrix)
        {
            matrix = matrix * Matrix;
        }


    }
}
