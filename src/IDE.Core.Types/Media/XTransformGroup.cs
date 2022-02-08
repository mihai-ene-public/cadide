using System;
using System.Collections.Generic;
using System.Text;

namespace IDE.Core.Types.Media
{
   public  class XTransformGroup:XTransform
    {
        public XTransformGroup() { }

        #region Value
        ///<summary>
        /// Return the current transformation value.
        ///</summary>
        public override XMatrix Value
        {
            get
            {

                XTransformCollection children = Children;
                if ((children == null) || (children.Count == 0))
                {
                    return new XMatrix();
                }

                XMatrix transform = children[0].Value;

                for (int i = 1; i < children.Count; i++)
                {
                    transform *= children[i].Value;
                }

                return transform;
            }
        }


        public XTransformCollection Children { get; set; } = new XTransformCollection();
        #endregion

        #region IsIdentity
        ///<summary>
        /// Returns true if transformation matches the identity transform.
        ///</summary>
        internal override bool IsIdentity
        {
            get
            {
                XTransformCollection children = Children;
                if ((children == null) || (children.Count == 0))
                {
                    return true;
                }

                for (int i = 0; i < children.Count; i++)
                {
                    if (!children[i].IsIdentity)
                    {
                        return false;
                    }
                }

                return true;
            }
        }
        #endregion

        #region Internal Methods

        internal override bool CanSerializeToString() { return false; }

        #endregion Internal Methods

      //  #region Public Methods

        ///// <summary>
        /////     Shadows inherited Clone() with a strongly typed
        /////     version for convenience.
        ///// </summary>
        //public new XTransformGroup Clone()
        //{
        //    return (XTransformGroup)base.Clone();
        //}

        ///// <summary>
        /////     Shadows inherited CloneCurrentValue() with a strongly typed
        /////     version for convenience.
        ///// </summary>
        //public new TransformGroup CloneCurrentValue()
        //{
        //    return (TransformGroup)base.CloneCurrentValue();
        //}
    }

    
}
