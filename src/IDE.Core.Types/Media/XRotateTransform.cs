using System;
using System.Collections.Generic;
using System.Text;

namespace IDE.Core.Types.Media
{
   public class XRotateTransform:XTransform
    {

        public XRotateTransform()
        {
        }

        ///<summary>
        /// Create a rotation transformation in degrees.
        ///</summary>
        ///<param name="angle">The angle of rotation in degrees.</param>
        public XRotateTransform(double angle)
        {
            Angle = angle;
        }

        ///<summary>
        /// Create a rotation transformation in degrees.
        ///</summary>
        public XRotateTransform(
            double angle,
            double centerX,
            double centerY
            ) : this(angle)
        {
            CenterX = centerX;
            CenterY = centerY;
        }

        ///<summary>
        /// Return the current transformation value.
        ///</summary>
        public override XMatrix Value
        {
            get
            {

                var m = new XMatrix();

                m.RotateAt(Angle, CenterX, CenterY);

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
                return Angle == 0;//&& CanFreeze;
            }
        }

        /// <summary>
        ///     Angle - double.  Default value is 0.0.
        /// </summary>
        public double Angle
        {
            get; set;
        } = 0.0d;

        /// <summary>
        ///     CenterX - double.  Default value is 0.0.
        /// </summary>
        public double CenterX
        {
            get;set;
        }

        /// <summary>
        ///     CenterY - double.  Default value is 0.0.
        /// </summary>
        public double CenterY
        {
            get;set;
        }
    }
}
