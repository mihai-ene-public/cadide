namespace IDE.Core.Types.Media3D
{
    public class XTransform3DGroup : XTransform3D
    {
        public XTransform3DGroup() { }

        ///<summary>
        ///     Return the current transformation value.
        ///</summary>
        public override XMatrix3D Value
        {
            get
            {

                var transform = new XMatrix3D();
                Append(ref transform);

                return transform;
            }
        }

        /// <summary>
        ///     Children - Transform3DCollection.  Default value is new FreezableDefaultValueFactory(Transform3DCollection.Empty).
        /// </summary>
        public XTransform3DCollection Children { get; set; } = new XTransform3DCollection();

        ///// <summary>
        /////     Whether the transform is affine.
        ///// </summary>
        //public override bool IsAffine
        //{
        //    get
        //    {

        //        var children = Children;
        //        if (children != null)
        //        {
        //            for (int i = 0, count = children.Count; i < count; ++i)
        //            {
        //                XTransform3D transform = children.Internal_GetItem(i);
        //                if (!transform.IsAffine)
        //                {
        //                    return false;
        //                }
        //            }
        //        }

        //        return true;
        //    }
        //}

        internal override void Append(ref XMatrix3D matrix)
        {
            var children = Children;
            if (children != null)
            {
                for (int i = 0, count = children.Count; i < count; i++)
                {
                    children[i].Append(ref matrix);
                }
            }
        }
    }
}
