using System;
using System.Collections.Generic;
using System.Text;

namespace IDE.Core.Types.Media3D
{
    public struct XPoint3D
    {

        #region Constructors

        /// <summary>
        /// Constructor that sets point's initial values.
        /// </summary>
        /// <param name="x">Value of the X coordinate of the new point.</param>
        /// <param name="y">Value of the Y coordinate of the new point.</param>
        /// <param name="z">Value of the Z coordinate of the new point.</param>
        public XPoint3D(double x, double y, double z)
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
        /// Offset - update point position by adding offsetX to X, offsetY to Y, and offsetZ to Z.
        /// </summary>
        /// <param name="offsetX">Offset in the X direction.</param>
        /// <param name="offsetY">Offset in the Y direction.</param>
        /// <param name="offsetZ">Offset in the Z direction.</param>
        public void Offset(double offsetX, double offsetY, double offsetZ)
        {
            _x += offsetX;
            _y += offsetY;
            _z += offsetZ;
        }

        /// <summary>
        /// XPoint3D + Vector3D addition.
        /// </summary>
        /// <param name="point">Point being added.</param>
        /// <param name="vector">Vector being added.</param>
        /// <returns>Result of addition.</returns>
        public static XPoint3D operator +(XPoint3D point, XVector3D vector)
        {
            return new XPoint3D(point._x + vector._x,
                               point._y + vector._y,
                               point._z + vector._z);
        }

        /// <summary>
        /// XPoint3D + Vector3D addition.
        /// </summary>
        /// <param name="point">Point being added.</param>
        /// <param name="vector">Vector being added.</param>
        /// <returns>Result of addition.</returns>
        public static XPoint3D Add(XPoint3D point, XVector3D vector)
        {
            return new XPoint3D(point._x + vector._x,
                               point._y + vector._y,
                               point._z + vector._z);
        }

        /// <summary>
        /// XPoint3D - Vector3D subtraction.
        /// </summary>
        /// <param name="point">Point from which vector is being subtracted.</param>
        /// <param name="vector">Vector being subtracted from the point.</param>
        /// <returns>Result of subtraction.</returns>
        public static XPoint3D operator -(XPoint3D point, XVector3D vector)
        {
            return new XPoint3D(point._x - vector._x,
                               point._y - vector._y,
                               point._z - vector._z);
        }

        /// <summary>
        /// XPoint3D - Vector3D subtraction.
        /// </summary>
        /// <param name="point">Point from which vector is being subtracted.</param>
        /// <param name="vector">Vector being subtracted from the point.</param>
        /// <returns>Result of subtraction.</returns>
        public static XPoint3D Subtract(XPoint3D point, XVector3D vector)
        {
            return new XPoint3D(point._x - vector._x,
                               point._y - vector._y,
                               point._z - vector._z);
        }

        /// <summary>
        /// Subtraction.
        /// </summary>
        /// <param name="point1">Point from which we are subtracting the second point.</param>
        /// <param name="point2">Point being subtracted.</param>
        /// <returns>Vector between the two points.</returns>
        public static XVector3D operator -(XPoint3D point1, XPoint3D point2)
        {
            return new XVector3D(point1._x - point2._x,
                                point1._y - point2._y,
                                point1._z - point2._z);
        }

        /// <summary>
        /// Subtraction.
        /// </summary>
        /// <param name="point1">Point from which we are subtracting the second point.</param>
        /// <param name="point2">Point being subtracted.</param>
        /// <returns>Vector between the two points.</returns>
        public static XVector3D Subtract(XPoint3D point1, XPoint3D point2)
        {
            var v = new XVector3D();
            Subtract(ref point1, ref point2, out v);
            return v;
        }

        /// <summary>
        /// Faster internal version of Subtract that avoids copies
        ///
        /// p1 and p2 to a passed by ref for perf and ARE NOT MODIFIED
        /// </summary>
        internal static void Subtract(ref XPoint3D p1, ref XPoint3D p2, out XVector3D result)
        {
            result._x = p1._x - p2._x;
            result._y = p1._y - p2._y;
            result._z = p1._z - p2._z;
        }

        /// <summary>
        /// XPoint3D * Matrix3D multiplication.
        /// </summary>
        /// <param name="point">Point being transformed.</param>
        /// <param name="matrix">Transformation matrix applied to the point.</param>
        /// <returns>Result of the transformation matrix applied to the point.</returns>
        public static XPoint3D operator *(XPoint3D point, XMatrix3D matrix)
        {
            return matrix.Transform(point);
        }

        public static XPoint3D operator *(XPoint3D vector, double scalar)
        {
            return new XPoint3D(vector._x * scalar,
                                vector._y * scalar,
                                vector._z * scalar);
        }

        /// <summary>
        /// XPoint3D * Matrix3D multiplication.
        /// </summary>
        /// <param name="point">Point being transformed.</param>
        /// <param name="matrix">Transformation matrix applied to the point.</param>
        /// <returns>Result of the transformation matrix applied to the point.</returns>
        public static XPoint3D Multiply(XPoint3D point, XMatrix3D matrix)
        {
            return matrix.Transform(point);
        }

        /// <summary>
        /// Explicit conversion to Vector3D.
        /// </summary>
        /// <param name="point">Given point.</param>
        /// <returns>Vector representing the point.</returns>
        public static explicit operator XVector3D(XPoint3D point)
        {
            return new XVector3D(point._x, point._y, point._z);
        }

        /// <summary>
        /// Compares two XPoint3D instances for exact equality.
        /// Note that double values can acquire error when operated upon, such that
        /// an exact comparison between two values which are logically equal may fail.
        /// Furthermore, using this equality operator, Double.NaN is not equal to itself.
        /// </summary>
        /// <returns>
        /// bool - true if the two XPoint3D instances are exactly equal, false otherwise
        /// </returns>
        /// <param name='point1'>The first XPoint3D to compare</param>
        /// <param name='point2'>The second XPoint3D to compare</param>
        public static bool operator ==(XPoint3D point1, XPoint3D point2)
        {
            return point1.X == point2.X &&
                   point1.Y == point2.Y &&
                   point1.Z == point2.Z;
        }

        /// <summary>
        /// Compares two XPoint3D instances for exact inequality.
        /// Note that double values can acquire error when operated upon, such that
        /// an exact comparison between two values which are logically equal may fail.
        /// Furthermore, using this equality operator, Double.NaN is not equal to itself.
        /// </summary>
        /// <returns>
        /// bool - true if the two XPoint3D instances are exactly unequal, false otherwise
        /// </returns>
        /// <param name='point1'>The first XPoint3D to compare</param>
        /// <param name='point2'>The second XPoint3D to compare</param>
        public static bool operator !=(XPoint3D point1, XPoint3D point2)
        {
            return !(point1 == point2);
        }
        /// <summary>
        /// Compares two XPoint3D instances for object equality.  In this equality
        /// Double.NaN is equal to itself, unlike in numeric equality.
        /// Note that double values can acquire error when operated upon, such that
        /// an exact comparison between two values which
        /// are logically equal may fail.
        /// </summary>
        /// <returns>
        /// bool - true if the two XPoint3D instances are exactly equal, false otherwise
        /// </returns>
        /// <param name='point1'>The first XPoint3D to compare</param>
        /// <param name='point2'>The second XPoint3D to compare</param>
        public static bool Equals(XPoint3D point1, XPoint3D point2)
        {
            return point1.X.Equals(point2.X) &&
                   point1.Y.Equals(point2.Y) &&
                   point1.Z.Equals(point2.Z);
        }

        /// <summary>
        /// Equals - compares this XPoint3D with the passed in object.  In this equality
        /// Double.NaN is equal to itself, unlike in numeric equality.
        /// Note that double values can acquire error when operated upon, such that
        /// an exact comparison between two values which
        /// are logically equal may fail.
        /// </summary>
        /// <returns>
        /// bool - true if the object is an instance of XPoint3D and if it's equal to "this".
        /// </returns>
        /// <param name='o'>The object to compare to "this"</param>
        public override bool Equals(object o)
        {
            if ((null == o) || !(o is XPoint3D))
            {
                return false;
            }

            XPoint3D value = (XPoint3D)o;
            return XPoint3D.Equals(this, value);
        }

        /// <summary>
        /// Equals - compares this XPoint3D with the passed in object.  In this equality
        /// Double.NaN is equal to itself, unlike in numeric equality.
        /// Note that double values can acquire error when operated upon, such that
        /// an exact comparison between two values which
        /// are logically equal may fail.
        /// </summary>
        /// <returns>
        /// bool - true if "value" is equal to "this".
        /// </returns>
        /// <param name='value'>The XPoint3D to compare to "this"</param>
        public bool Equals(XPoint3D value)
        {
            return XPoint3D.Equals(this, value);
        }
        /// <summary>
        /// Returns the HashCode for this XPoint3D
        /// </summary>
        /// <returns>
        /// int - the HashCode for this XPoint3D
        /// </returns>
        public override int GetHashCode()
        {
            // Perform field-by-field XOR of HashCodes
            return X.GetHashCode() ^
                   Y.GetHashCode() ^
                   Z.GetHashCode();
        }

     

#endregion Public Methods
    }
}
