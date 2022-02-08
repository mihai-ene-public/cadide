using IDE.Core.Types.Media;
using System;
using System.Collections;

namespace IDE.Core.Types.Media3D
{
    public struct XVector3D
    {
        #region Constructors

        /// <summary>
        /// Constructor that sets vector's initial values.
        /// </summary>
        /// <param name="x">Value of the X coordinate of the new vector.</param>
        /// <param name="y">Value of the Y coordinate of the new vector.</param>
        /// <param name="z">Value of the Z coordinate of the new vector.</param>
        public XVector3D(double x, double y, double z)
        {
            _x = x;
            _y = y;
            _z = z;
        }

        #endregion Constructors

        internal double _x;
        internal double _y;
        internal double _z;

        #region Public Properties

        /// <summary>
        ///     X - double.  Default value is 0.
        /// </summary>
        public double X
        {
            get
            {
                return _x;
            }

            set
            {
                _x = value;
            }

        }

        /// <summary>
        ///     Y - double.  Default value is 0.
        /// </summary>
        public double Y
        {
            get
            {
                return _y;
            }

            set
            {
                _y = value;
            }

        }

        /// <summary>
        ///     Z - double.  Default value is 0.
        /// </summary>
        public double Z
        {
            get
            {
                return _z;
            }

            set
            {
                _z = value;
            }

        }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Length of the vector.
        /// </summary>
        public double Length
        {
            get
            {
                return Math.Sqrt(_x * _x + _y * _y + _z * _z);
            }
        }

        /// <summary>
        /// Length of the vector squared.
        /// </summary>
        public double LengthSquared
        {
            get
            {
                return _x * _x + _y * _y + _z * _z;
            }
        }

        /// <summary>
        /// Compares two XVector3D instances for exact equality.
        /// Note that double values can acquire error when operated upon, such that
        /// an exact comparison between two values which are logically equal may fail.
        /// Furthermore, using this equality operator, Double.NaN is not equal to itself.
        /// </summary>
        /// <returns>
        /// bool - true if the two XVector3D instances are exactly equal, false otherwise
        /// </returns>
        /// <param name='vector1'>The first XVector3D to compare</param>
        /// <param name='vector2'>The second XVector3D to compare</param>
        public static bool operator ==(XVector3D vector1, XVector3D vector2)
        {
            return vector1.X == vector2.X &&
                   vector1.Y == vector2.Y &&
                   vector1.Z == vector2.Z;
        }

        /// <summary>
        /// Compares two XVector3D instances for exact inequality.
        /// Note that double values can acquire error when operated upon, such that
        /// an exact comparison between two values which are logically equal may fail.
        /// Furthermore, using this equality operator, Double.NaN is not equal to itself.
        /// </summary>
        /// <returns>
        /// bool - true if the two XVector3D instances are exactly unequal, false otherwise
        /// </returns>
        /// <param name='vector1'>The first XVector3D to compare</param>
        /// <param name='vector2'>The second XVector3D to compare</param>
        public static bool operator !=(XVector3D vector1, XVector3D vector2)
        {
            return !(vector1 == vector2);
        }
        /// <summary>
        /// Compares two XVector3D instances for object equality.  In this equality
        /// Double.NaN is equal to itself, unlike in numeric equality.
        /// Note that double values can acquire error when operated upon, such that
        /// an exact comparison between two values which
        /// are logically equal may fail.
        /// </summary>
        /// <returns>
        /// bool - true if the two XVector3D instances are exactly equal, false otherwise
        /// </returns>
        /// <param name='vector1'>The first XVector3D to compare</param>
        /// <param name='vector2'>The second XVector3D to compare</param>
        public static bool Equals(XVector3D vector1, XVector3D vector2)
        {
            return vector1.X.Equals(vector2.X) &&
                   vector1.Y.Equals(vector2.Y) &&
                   vector1.Z.Equals(vector2.Z);
        }

        /// <summary>
        /// Equals - compares this XVector3D with the passed in object.  In this equality
        /// Double.NaN is equal to itself, unlike in numeric equality.
        /// Note that double values can acquire error when operated upon, such that
        /// an exact comparison between two values which
        /// are logically equal may fail.
        /// </summary>
        /// <returns>
        /// bool - true if the object is an instance of XVector3D and if it's equal to "this".
        /// </returns>
        /// <param name='o'>The object to compare to "this"</param>
        public override bool Equals(object o)
        {
            if ((null == o) || !(o is XVector3D))
            {
                return false;
            }

            XVector3D value = (XVector3D)o;
            return XVector3D.Equals(this, value);
        }

        /// <summary>
        /// Equals - compares this XVector3D with the passed in object.  In this equality
        /// Double.NaN is equal to itself, unlike in numeric equality.
        /// Note that double values can acquire error when operated upon, such that
        /// an exact comparison between two values which
        /// are logically equal may fail.
        /// </summary>
        /// <returns>
        /// bool - true if "value" is equal to "this".
        /// </returns>
        /// <param name='value'>The XVector3D to compare to "this"</param>
        public bool Equals(XVector3D value)
        {
            return XVector3D.Equals(this, value);
        }
        /// <summary>
        /// Returns the HashCode for this XVector3D
        /// </summary>
        /// <returns>
        /// int - the HashCode for this XVector3D
        /// </returns>
        public override int GetHashCode()
        {
            // Perform field-by-field XOR of HashCodes
            return X.GetHashCode() ^
                   Y.GetHashCode() ^
                   Z.GetHashCode();
        }


        /// <summary>
        /// Updates the vector to maintain its direction, but to have a length
        /// of 1. Equivalent to dividing the vector by its Length.
        /// Returns NaN if length is zero.
        /// </summary>
        public void Normalize()
        {
            // Computation of length can overflow easily because it
            // first computes squared length, so we first divide by
            // the largest coefficient.
            double m = Math.Abs(_x);
            double absy = Math.Abs(_y);
            double absz = Math.Abs(_z);
            if (absy > m)
            {
                m = absy;
            }
            if (absz > m)
            {
                m = absz;
            }

            _x /= m;
            _y /= m;
            _z /= m;

            double length = Math.Sqrt(_x * _x + _y * _y + _z * _z);
            this /= length;
        }

        /// <summary>
        /// Computes the angle between two vectors.
        /// </summary>
        /// <param name="vector1">First vector.</param>
        /// <param name="vector2">Second vector.</param>
        /// <returns>
        /// Returns the angle required to rotate vector1 into vector2 in degrees.
        /// This will return a value between [0, 180] degrees.
        /// (Note that this is slightly different from the Vector member
        /// function of the same name.  Signed angles do not extend to 3D.)
        /// </returns>
        public static double AngleBetween(XVector3D vector1, XVector3D vector2)
        {
            vector1.Normalize();
            vector2.Normalize();

            double ratio = DotProduct(vector1, vector2);

            // The "straight forward" method of acos(u.v) has large precision
            // issues when the dot product is near +/-1.  This is due to the
            // steep slope of the acos function as we approach +/- 1.  Slight
            // precision errors in the dot product calculation cause large
            // variation in the output value.
            //
            //        |                   |
            //         \__                |
            //            ---___          |
            //                  ---___    |
            //                        ---_|_
            //                            | ---___
            //                            |       ---___
            //                            |             ---__
            //                            |                  \
            //                            |                   |
            //       -|-------------------+-------------------|-
            //       -1                   0                   1
            //
            //                         acos(x)
            //
            // To avoid this we use an alternative method which finds the
            // angle bisector by (u-v)/2:
            //
            //                            _>
            //                       u  _-  \ (u-v)/2
            //                        _-  __-v
            //                      _=__--      
            //                    .=----------->
            //                            v
            //
            // Because u and v and unit vectors, (u-v)/2 forms a right angle
            // with the angle bisector.  The hypotenuse is 1, therefore
            // 2*asin(|u-v|/2) gives us the angle between u and v.
            //
            // The largest possible value of |u-v| occurs with perpendicular
            // vectors and is sqrt(2)/2 which is well away from extreme slope
            // at +/-1.
            //
            // (See Windows OS Bug #1706299 for details)

            double theta;

            if (ratio < 0)
            {
                theta = Math.PI - 2.0 * Math.Asin((-vector1 - vector2).Length / 2.0);
            }
            else
            {
                theta = 2.0 * Math.Asin((vector1 - vector2).Length / 2.0);
            }

            return M3DUtil.RadiansToDegrees(theta);
        }

        /// <summary>
        /// Operator -Vector (unary negation).
        /// </summary>
        /// <param name="vector">Vector being negated.</param>
        /// <returns>Negation of the given vector.</returns>
        public static XVector3D operator -(XVector3D vector)
        {
            return new XVector3D(-vector._x, -vector._y, -vector._z);
        }

        /// <summary>
        /// Negates the values of X, Y, and Z on this XVector3D
        /// </summary>
        public void Negate()
        {
            _x = -_x;
            _y = -_y;
            _z = -_z;
        }

        public XVector3D FindAnyPerpendicular()
        {
            var n = this;//copy this
            n.Normalize();
            var u = XVector3D.CrossProduct(new XVector3D(0, 1, 0), n);
            if (u.LengthSquared < 1e-3)
            {
                u = XVector3D.CrossProduct(new XVector3D(1, 0, 0), n);
            }

            return u;
        }

        public bool IsUndefined()
        {
            return double.IsNaN(_x) && double.IsNaN(_y) && double.IsNaN(_z);
        }

        /// <summary>
        /// Vector addition.
        /// </summary>
        /// <param name="vector1">First vector being added.</param>
        /// <param name="vector2">Second vector being added.</param>
        /// <returns>Result of addition.</returns>
        public static XVector3D operator +(XVector3D vector1, XVector3D vector2)
        {
            return new XVector3D(vector1._x + vector2._x,
                                vector1._y + vector2._y,
                                vector1._z + vector2._z);
        }

        /// <summary>
        /// Vector addition.
        /// </summary>
        /// <param name="vector1">First vector being added.</param>
        /// <param name="vector2">Second vector being added.</param>
        /// <returns>Result of addition.</returns>
        public static XVector3D Add(XVector3D vector1, XVector3D vector2)
        {
            return new XVector3D(vector1._x + vector2._x,
                                vector1._y + vector2._y,
                                vector1._z + vector2._z);
        }

        /// <summary>
        /// Vector subtraction.
        /// </summary>
        /// <param name="vector1">Vector that is subtracted from.</param>
        /// <param name="vector2">Vector being subtracted.</param>
        /// <returns>Result of subtraction.</returns>
        public static XVector3D operator -(XVector3D vector1, XVector3D vector2)
        {
            return new XVector3D(vector1._x - vector2._x,
                                vector1._y - vector2._y,
                                vector1._z - vector2._z);
        }

        /// <summary>
        /// Vector subtraction.
        /// </summary>
        /// <param name="vector1">Vector that is subtracted from.</param>
        /// <param name="vector2">Vector being subtracted.</param>
        /// <returns>Result of subtraction.</returns>
        public static XVector3D Subtract(XVector3D vector1, XVector3D vector2)
        {
            return new XVector3D(vector1._x - vector2._x,
                                vector1._y - vector2._y,
                                vector1._z - vector2._z);
        }

        /// <summary>
        /// XVector3D + XPoint3D addition.
        /// </summary>
        /// <param name="vector">Vector by which we offset the point.</param>
        /// <param name="point">Point being offset by the given vector.</param>
        /// <returns>Result of addition.</returns>
        public static XPoint3D operator +(XVector3D vector, XPoint3D point)
        {
            return new XPoint3D(vector._x + point._x,
                               vector._y + point._y,
                               vector._z + point._z);
        }

        /// <summary>
        /// XVector3D + XPoint3D addition.
        /// </summary>
        /// <param name="vector">Vector by which we offset the point.</param>
        /// <param name="point">Point being offset by the given vector.</param>
        /// <returns>Result of addition.</returns>
        public static XPoint3D Add(XVector3D vector, XPoint3D point)
        {
            return new XPoint3D(vector._x + point._x,
                               vector._y + point._y,
                               vector._z + point._z);
        }

        /// <summary>
        /// XVector3D - XPoint3D subtraction.
        /// </summary>
        /// <param name="vector">Vector by which we offset the point.</param>
        /// <param name="point">Point being offset by the given vector.</param>
        /// <returns>Result of subtraction.</returns>
        public static XPoint3D operator -(XVector3D vector, XPoint3D point)
        {
            return new XPoint3D(vector._x - point._x,
                               vector._y - point._y,
                               vector._z - point._z);
        }

        /// <summary>
        /// XVector3D - XPoint3D subtraction.
        /// </summary>
        /// <param name="vector">Vector by which we offset the point.</param>
        /// <param name="point">Point being offset by the given vector.</param>
        /// <returns>Result of subtraction.</returns>
        public static XPoint3D Subtract(XVector3D vector, XPoint3D point)
        {
            return new XPoint3D(vector._x - point._x,
                               vector._y - point._y,
                               vector._z - point._z);
        }

        /// <summary>
        /// Scalar multiplication.
        /// </summary>
        /// <param name="vector">Vector being multiplied.</param>
        /// <param name="scalar">Scalar value by which the vector is multiplied.</param>
        /// <returns>Result of multiplication.</returns>
        public static XVector3D operator *(XVector3D vector, double scalar)
        {
            return new XVector3D(vector._x * scalar,
                                vector._y * scalar,
                                vector._z * scalar);
        }

        /// <summary>
        /// Scalar multiplication.
        /// </summary>
        /// <param name="vector">Vector being multiplied.</param>
        /// <param name="scalar">Scalar value by which the vector is multiplied.</param>
        /// <returns>Result of multiplication.</returns>
        public static XVector3D Multiply(XVector3D vector, double scalar)
        {
            return new XVector3D(vector._x * scalar,
                                vector._y * scalar,
                                vector._z * scalar);
        }

        /// <summary>
        /// Scalar multiplication.
        /// </summary>
        /// <param name="scalar">Scalar value by which the vector is multiplied</param>
        /// <param name="vector">Vector being multiplied.</param>
        /// <returns>Result of multiplication.</returns>
        public static XVector3D operator *(double scalar, XVector3D vector)
        {
            return new XVector3D(vector._x * scalar,
                                vector._y * scalar,
                                vector._z * scalar);
        }

        /// <summary>
        /// Scalar multiplication.
        /// </summary>
        /// <param name="scalar">Scalar value by which the vector is multiplied</param>
        /// <param name="vector">Vector being multiplied.</param>
        /// <returns>Result of multiplication.</returns>
        public static XVector3D Multiply(double scalar, XVector3D vector)
        {
            return new XVector3D(vector._x * scalar,
                                vector._y * scalar,
                                vector._z * scalar);
        }

        /// <summary>
        /// Scalar division.
        /// </summary>
        /// <param name="vector">Vector being divided.</param>
        /// <param name="scalar">Scalar value by which we divide the vector.</param>
        /// <returns>Result of division.</returns>
        public static XVector3D operator /(XVector3D vector, double scalar)
        {
            return vector * (1.0 / scalar);
        }

        /// <summary>
        /// Scalar division.
        /// </summary>
        /// <param name="vector">Vector being divided.</param>
        /// <param name="scalar">Scalar value by which we divide the vector.</param>
        /// <returns>Result of division.</returns>
        public static XVector3D Divide(XVector3D vector, double scalar)
        {
            return vector * (1.0 / scalar);
        }

        /// <summary>
        /// XVector3D * Matrix3D multiplication
        /// </summary>
        /// <param name="vector">Vector being tranformed.</param>
        /// <param name="matrix">Transformation matrix applied to the vector.</param>
        /// <returns>Result of multiplication.</returns>
        public static XVector3D operator *(XVector3D vector, XMatrix3D matrix)
        {
            return matrix.Transform(vector);
        }

        /// <summary>
        /// XVector3D * Matrix3D multiplication
        /// </summary>
        /// <param name="vector">Vector being tranformed.</param>
        /// <param name="matrix">Transformation matrix applied to the vector.</param>
        /// <returns>Result of multiplication.</returns>
        public static XVector3D Multiply(XVector3D vector, XMatrix3D matrix)
        {
            return matrix.Transform(vector);
        }

        /// <summary>
        /// Vector dot product.
        /// </summary>
        /// <param name="vector1">First vector.</param>
        /// <param name="vector2">Second vector.</param>
        /// <returns>Dot product of two vectors.</returns>
        public static double DotProduct(XVector3D vector1, XVector3D vector2)
        {
            return DotProduct(ref vector1, ref vector2);
        }

        /// <summary>
        /// Faster internal version of DotProduct that avoids copies
        ///
        /// vector1 and vector2 to a passed by ref for perf and ARE NOT MODIFIED
        /// </summary>
        internal static double DotProduct(ref XVector3D vector1, ref XVector3D vector2)
        {
            return vector1._x * vector2._x +
                   vector1._y * vector2._y +
                   vector1._z * vector2._z;
        }

        /// <summary>
        /// Vector cross product.
        /// </summary>
        /// <param name="vector1">First vector.</param>
        /// <param name="vector2">Second vector.</param>
        /// <returns>Cross product of two vectors.</returns>
        public static XVector3D CrossProduct(XVector3D vector1, XVector3D vector2)
        {
            XVector3D result;
            CrossProduct(ref vector1, ref vector2, out result);
            return result;
        }

        /// <summary>
        /// Faster internal version of CrossProduct that avoids copies
        ///
        /// vector1 and vector2 to a passed by ref for perf and ARE NOT MODIFIED
        /// </summary>
        internal static void CrossProduct(ref XVector3D vector1, ref XVector3D vector2, out XVector3D result)
        {
            result._x = vector1._y * vector2._z - vector1._z * vector2._y;
            result._y = vector1._z * vector2._x - vector1._x * vector2._z;
            result._z = vector1._x * vector2._y - vector1._y * vector2._x;
        }

        /// <summary>
        /// XVector3D to XPoint3D conversion.
        /// </summary>
        /// <param name="vector">Vector being converted.</param>
        /// <returns>Point representing the given vector.</returns>
        public static explicit operator XPoint3D(XVector3D vector)
        {
            return new XPoint3D(vector._x, vector._y, vector._z);
        }

        ///// <summary>
        ///// Explicit conversion to Size3D.  Note that since Size3D cannot contain negative values,
        ///// the resulting size will contains the absolute values of X, Y, and Z.
        ///// </summary>
        ///// <param name="vector">The vector to convert to a size.</param>
        ///// <returns>A size equal to this vector.</returns>
        //public static explicit operator Size3D(XVector3D vector)
        //{
        //    return new Size3D(Math.Abs(vector._x), Math.Abs(vector._y), Math.Abs(vector._z));
        //}

        #endregion Public Methods
    }

    public class XRotateTransform3D : XTransform3D
    {
        #region Constructors

        /// <summary>
        ///     Default constructor.
        /// </summary>
        public XRotateTransform3D() { }

        /// <summary>
        ///     Constructor from Rotation3D.
        /// </summary>
        /// <param name="rotation">Rotation3D.</param>
        public XRotateTransform3D(XRotation3D rotation)
        {
            Rotation = rotation;
        }

        /// <summary>
        ///     Constructor from Rotation3D and center point.
        /// </summary>
        /// <param name="rotation">Rotation3D.</param>
        /// <param name="center">Center point.</param>
        public XRotateTransform3D(XRotation3D rotation, XPoint3D center)
        {
            Rotation = rotation;
            CenterX = center.X;
            CenterY = center.Y;
            CenterZ = center.Z;
        }


        /// <summary>
        ///     Constructor from Rotation3D and center point.
        /// </summary>
        /// <param name="rotation">Rotation3D.</param>
        /// <param name="centerX">X center</param>
        /// <param name="centerY">Y center</param>
        /// <param name="centerZ">Z center</param>
        public XRotateTransform3D(XRotation3D rotation, double centerX, double centerY, double centerZ)
        {
            Rotation = rotation;
            CenterX = centerX;
            CenterY = centerY;
            CenterZ = centerZ;
        }

        #endregion Constructors

        #region Public Properties

        /// <summary>
        ///     CenterX - double.  Default value is 0.0.
        /// </summary>
        public double CenterX
        {
            get
            {
                return _cachedCenterXValue;
            }
            set
            {
                _cachedCenterXValue = value;
            }
        }

        /// <summary>
        ///     CenterY - double.  Default value is 0.0.
        /// </summary>
        public double CenterY
        {
            get
            {
                return _cachedCenterYValue;
            }
            set
            {
                _cachedCenterYValue = value;
            }
        }

        /// <summary>
        ///     CenterZ - double.  Default value is 0.0.
        /// </summary>
        public double CenterZ
        {
            get
            {
                return _cachedCenterZValue;
            }
            set
            {
                _cachedCenterZValue = value;
            }
        }

        /// <summary>
        ///     Rotation - Rotation3D.  Default value is Rotation3D.Identity.
        /// </summary>
        public XRotation3D Rotation
        {
            get
            {
                return _cachedRotationValue;
            }
            set
            {
                _cachedRotationValue = value;
            }
        }

        /// <summary>
        /// Retrieves matrix representing the rotation.
        /// </summary>
        public override XMatrix3D Value
        {
            get
            {
                var rotation = _cachedRotationValue;

                if (rotation == null)
                {
                    return XMatrix3D.Identity;
                }

                var quaternion = rotation.InternalQuaternion;
                var center = new XPoint3D(_cachedCenterXValue, _cachedCenterYValue, _cachedCenterZValue);

                return XMatrix3D.CreateRotationMatrix(ref quaternion, ref center);
            }
        }

        private double _cachedCenterXValue = 0.0;
        private double _cachedCenterYValue = 0.0;
        private double _cachedCenterZValue = 0.0;
        private XRotation3D _cachedRotationValue = XRotation3D.Identity;

        internal const double c_CenterX = 0.0;
        internal const double c_CenterY = 0.0;
        internal const double c_CenterZ = 0.0;
        internal static XRotation3D s_Rotation = XRotation3D.Identity;

        #endregion Public Properties

        internal override void Append(ref XMatrix3D matrix)
        {
            matrix = matrix * Value;
        }
    }

    public abstract class XRotation3D
    {
        static XRotation3D()
        {
            // Create our singleton frozen instance
            s_identity = new XQuaternionRotation3D();
        }

        // Prevent 3rd parties from extending this abstract base class
        internal XRotation3D() { }

        /// <summary>
        ///     Singleton identity Rotation3D.
        /// </summary>
        public static XRotation3D Identity
        {
            get { return s_identity; }
        }

        // Used by animation to get a snapshot of the current rotational
        // configuration for interpolation in Rotation3DAnimations.
        internal abstract XQuaternion InternalQuaternion
        {
            get;
        }

        private static readonly XRotation3D s_identity;
    }

    public class XQuaternionRotation3D : XRotation3D
    {
        #region Constructors

        /// <summary>
        /// Default constructor that creates a rotation with Quaternion (0,0,0,1).
        /// </summary>
        public XQuaternionRotation3D() { }

        /// <summary>
        /// Constructor taking a quaternion.
        /// </summary>
        public XQuaternionRotation3D(XQuaternion quaternion)
        {
            Quaternion = quaternion;
        }

        #endregion Constructors

        public XQuaternion Quaternion
        {
            get
            {
                return _cachedQuaternionValue;
            }
            set
            {
                _cachedQuaternionValue = value;
            }
        }

        // Used by animation to get a snapshot of the current rotational
        // configuration for interpolation in Rotation3DAnimations.
        internal override XQuaternion InternalQuaternion { get { return _cachedQuaternionValue; } }

        private XQuaternion _cachedQuaternionValue = XQuaternion.Identity;

        internal static XQuaternion s_Quaternion = XQuaternion.Identity;
    }

    public class XTranslateTransform3D : XTransform3D
    {
        #region Constructors

        /// <summary>
        ///     Default constructor.
        /// </summary>
        public XTranslateTransform3D() { }

        /// <summary>
        ///     Create translation transform.
        /// </summary>
        public XTranslateTransform3D(XVector3D offset)
        {
            OffsetX = offset.X;
            OffsetY = offset.Y;
            OffsetZ = offset.Z;
        }

        /// <summary>
        ///     Create translation transform.
        /// </summary>
        public XTranslateTransform3D(double offsetX, double offsetY, double offsetZ)
        {
            OffsetX = offsetX;
            OffsetY = offsetY;
            OffsetZ = offsetZ;
        }

        #endregion Constructors

        #region Public Properties

        /// <summary>
        ///     OffsetX - double.  Default value is 0.0.
        /// </summary>
        public double OffsetX
        {
            get
            {
                return _cachedOffsetXValue;
            }
            set
            {
                _cachedOffsetXValue = value;
            }
        }

        /// <summary>
        ///     OffsetY - double.  Default value is 0.0.
        /// </summary>
        public double OffsetY
        {
            get
            {
                return _cachedOffsetYValue;
            }
            set
            {
                _cachedOffsetYValue = value;
            }
        }

        /// <summary>
        ///     OffsetZ - double.  Default value is 0.0.
        /// </summary>
        public double OffsetZ
        {
            get
            {
                return _cachedOffsetZValue;
            }
            set
            {
                _cachedOffsetZValue = value;
            }
        }

        /// <summary>
        ///     Returns transform matrix for this transform.
        /// </summary>
        public override XMatrix3D Value
        {
            get
            {

                XMatrix3D matrix = new XMatrix3D();
                Append(ref matrix);

                return matrix;
            }
        }

        #endregion Public Properties

        internal override void Append(ref XMatrix3D matrix)
        {
            matrix.Translate(new XVector3D(_cachedOffsetXValue, _cachedOffsetYValue, _cachedOffsetZValue));
        }

        private double _cachedOffsetXValue = 0.0;
        private double _cachedOffsetYValue = 0.0;
        private double _cachedOffsetZValue = 0.0;

        internal const double c_OffsetX = 0.0;
        internal const double c_OffsetY = 0.0;
        internal const double c_OffsetZ = 0.0;
    }

    public class XAxisAngleRotation3D : XRotation3D
    {
        #region Constructors

        /// <summary>
        /// Default constructor that creates a rotation with Axis (0,1,0) and Angle of 0.
        /// </summary>
        public XAxisAngleRotation3D() { }

        /// <summary>
        /// Constructor taking axis and angle.
        /// </summary>
        public XAxisAngleRotation3D(XVector3D axis, double angle)
        {
            Axis = axis;
            Angle = angle;
        }

        #endregion Constructors


        #region Public Properties

        /// <summary>
        ///     Axis - Vector3D.  Default value is new Vector3D(0,1,0).
        /// </summary>
        public XVector3D Axis
        {
            get;set;
        }

        /// <summary>
        ///     Angle - double.  Default value is (double)0.0.
        /// </summary>
        public double Angle
        {
            get;set;
        }

        #endregion Public Properties

        // Used by animation to get a snapshot of the current rotational
        // configuration for interpolation in Rotation3DAnimations.
        internal override XQuaternion InternalQuaternion
        {
            get
            {
                if (_cachedQuaternionValue == c_dirtyQuaternion)
                {
                    XVector3D axis = Axis;

                    // Quaternion's axis/angle ctor throws if the axis has zero length.
                    //
                    // This threshold needs to match the one we used in D3DXVec3Normalize (d3dxmath9.cpp)
                    // and in unmanaged code.  See also AxisAngleRotation3D.cpp.
                    if (axis.LengthSquared > DoubleUtil.FLT_MIN)
                    {
                        _cachedQuaternionValue = new XQuaternion(axis, Angle);
                    }
                    else
                    {
                        // If we have a zero-length axis we return identity (i.e.,
                        // we consider this to be no rotation.)
                        _cachedQuaternionValue = XQuaternion.Identity;
                    }
                }

                return _cachedQuaternionValue;
            }
        }

        private XQuaternion _cachedQuaternionValue = c_dirtyQuaternion;

        // Arbitrary quaternion that will signify that our cached quat is dirty
        // Reasonable quaternions are normalized so it's very unlikely that this
        // will ever occurr in a normal application.
        internal static readonly XQuaternion c_dirtyQuaternion = new XQuaternion(
            Math.E, Math.PI, Math.E * Math.PI, 55.0
            );

        internal static XVector3D s_Axis = new XVector3D(0, 1, 0);
        internal const double c_Angle = (double)0.0;
    }
}
