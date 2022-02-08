using System;

namespace IDE.Core.Types.Media
{
    public struct XVector
    {
        #region Constructors

        /// <summary>
        /// Constructor which sets the vector's initial values
        /// </summary>
        /// <param name="x"> double - The initial X </param>
        /// <param name="y"> double - THe initial Y </param>
        public XVector(double x, double y)
        {
            _x = x;
            _y = y;
        }

        #endregion Constructors

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

        internal double _x;
        internal double _y;

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Length Property - the length of this Vector
        /// </summary>
        public double Length
        {
            get
            {
                return Math.Sqrt(_x * _x + _y * _y);
            }
        }

        /// <summary>
        /// LengthSquared Property - the squared length of this Vector
        /// </summary>
        public double LengthSquared
        {
            get
            {
                return _x * _x + _y * _y;
            }
        }

        /// <summary>
        /// Normalize - Updates this Vector to maintain its direction, but to have a length
        /// of 1.  This is equivalent to dividing this Vector by Length
        /// </summary>
        public void Normalize()
        {
            // Avoid overflow
            this /= Math.Max(Math.Abs(_x), Math.Abs(_y));
            this /= Length;
        }

        /// <summary>
        /// CrossProduct - Returns the cross product: vector1.X*vector2.Y - vector1.Y*vector2.X
        /// </summary>
        /// <returns>
        /// Returns the cross product: vector1.X*vector2.Y - vector1.Y*vector2.X
        /// </returns>
        /// <param name="vector1"> The first Vector </param>
        /// <param name="vector2"> The second Vector </param>
        public static double CrossProduct(XVector vector1, XVector vector2)
        {
            return vector1._x * vector2._y - vector1._y * vector2._x;
        }

        /// <summary>
        /// AngleBetween - the angle between 2 vectors
        /// </summary>
        /// <returns>
        /// Returns the the angle in degrees between vector1 and vector2
        /// </returns>
        /// <param name="vector1"> The first XVector </param>
        /// <param name="vector2"> The second XVector </param>
        public static double AngleBetween(XVector vector1, XVector vector2)
        {
            double sin = vector1._x * vector2._y - vector2._x * vector1._y;
            double cos = vector1._x * vector2._x + vector1._y * vector2._y;

            return Math.Atan2(sin, cos) * (180 / Math.PI);
        }

        /// <summary>
        /// returns angle between vectors in radians
        /// </summary>
        public static double AngleBetweenRadians(XVector vector1, XVector vector2)
        {
            double sin = vector1._x * vector2._y - vector2._x * vector1._y;
            double cos = vector1._x * vector2._x + vector1._y * vector2._y;

            return Math.Atan2(sin, cos);
        }

        /// <summary>
        /// Compares two Vector instances for exact equality.
        /// Note that double values can acquire error when operated upon, such that
        /// an exact comparison between two values which are logically equal may fail.
        /// Furthermore, using this equality operator, Double.NaN is not equal to itself.
        /// </summary>
        /// <returns>
        /// bool - true if the two Vector instances are exactly equal, false otherwise
        /// </returns>
        /// <param name='vector1'>The first Vector to compare</param>
        /// <param name='vector2'>The second Vector to compare</param>
        public static bool operator ==(XVector vector1, XVector vector2)
        {
            return vector1.X == vector2.X &&
                   vector1.Y == vector2.Y;
        }

        /// <summary>
        /// Compares two Vector instances for exact inequality.
        /// Note that double values can acquire error when operated upon, such that
        /// an exact comparison between two values which are logically equal may fail.
        /// Furthermore, using this equality operator, Double.NaN is not equal to itself.
        /// </summary>
        /// <returns>
        /// bool - true if the two Vector instances are exactly unequal, false otherwise
        /// </returns>
        /// <param name='vector1'>The first Vector to compare</param>
        /// <param name='vector2'>The second Vector to compare</param>
        public static bool operator !=(XVector vector1, XVector vector2)
        {
            return !(vector1 == vector2);
        }
        /// <summary>
        /// Compares two Vector instances for object equality.  In this equality
        /// Double.NaN is equal to itself, unlike in numeric equality.
        /// Note that double values can acquire error when operated upon, such that
        /// an exact comparison between two values which
        /// are logically equal may fail.
        /// </summary>
        /// <returns>
        /// bool - true if the two Vector instances are exactly equal, false otherwise
        /// </returns>
        /// <param name='vector1'>The first Vector to compare</param>
        /// <param name='vector2'>The second Vector to compare</param>
        public static bool Equals(XVector vector1, XVector vector2)
        {
            return vector1.X.Equals(vector2.X) &&
                   vector1.Y.Equals(vector2.Y);
        }

        /// <summary>
        /// Equals - compares this Vector with the passed in object.  In this equality
        /// Double.NaN is equal to itself, unlike in numeric equality.
        /// Note that double values can acquire error when operated upon, such that
        /// an exact comparison between two values which
        /// are logically equal may fail.
        /// </summary>
        /// <returns>
        /// bool - true if the object is an instance of Vector and if it's equal to "this".
        /// </returns>
        /// <param name='o'>The object to compare to "this"</param>
        public override bool Equals(object o)
        {
            if ((null == o) || !(o is XVector))
            {
                return false;
            }

            XVector value = (XVector)o;
            return XVector.Equals(this, value);
        }

        /// <summary>
        /// Equals - compares this Vector with the passed in object.  In this equality
        /// Double.NaN is equal to itself, unlike in numeric equality.
        /// Note that double values can acquire error when operated upon, such that
        /// an exact comparison between two values which
        /// are logically equal may fail.
        /// </summary>
        /// <returns>
        /// bool - true if "value" is equal to "this".
        /// </returns>
        /// <param name='value'>The Vector to compare to "this"</param>
        public bool Equals(XVector value)
        {
            return XVector.Equals(this, value);
        }
        /// <summary>
        /// Returns the HashCode for this Vector
        /// </summary>
        /// <returns>
        /// int - the HashCode for this Vector
        /// </returns>
        public override int GetHashCode()
        {
            // Perform field-by-field XOR of HashCodes
            return X.GetHashCode() ^
                   Y.GetHashCode();
        }

        #endregion Public Methods

        #region Public Operators
        /// <summary>
        /// Operator -Vector (unary negation)
        /// </summary>
        public static XVector operator -(XVector vector)
        {
            return new XVector(-vector._x, -vector._y);
        }

        /// <summary>
        /// Negates the values of X and Y on this Vector
        /// </summary>
        public void Negate()
        {
            _x = -_x;
            _y = -_y;
        }

        /// <summary>
        /// Operator Vector + Vector
        /// </summary>
        public static XVector operator +(XVector vector1, XVector vector2)
        {
            return new XVector(vector1._x + vector2._x,
                              vector1._y + vector2._y);
        }

        /// <summary>
        /// Add: Vector + Vector
        /// </summary>
        public static XVector Add(XVector vector1, XVector vector2)
        {
            return new XVector(vector1._x + vector2._x,
                              vector1._y + vector2._y);
        }

        /// <summary>
        /// Operator Vector - Vector
        /// </summary>
        public static XVector operator -(XVector vector1, XVector vector2)
        {
            return new XVector(vector1._x - vector2._x,
                              vector1._y - vector2._y);
        }

        /// <summary>
        /// Subtract: Vector - Vector
        /// </summary>
        public static XVector Subtract(XVector vector1, XVector vector2)
        {
            return new XVector(vector1._x - vector2._x,
                              vector1._y - vector2._y);
        }

        /// <summary>
        /// Operator Vector + Point
        /// </summary>
        public static XPoint operator +(XVector vector, XPoint point)
        {
            return new XPoint(point._x + vector._x, point._y + vector._y);
        }

        /// <summary>
        /// Add: Vector + Point
        /// </summary>
        public static XPoint Add(XVector vector, XPoint point)
        {
            return new XPoint(point._x + vector._x, point._y + vector._y);
        }

        /// <summary>
        /// Operator Vector * double
        /// </summary>
        public static XVector operator *(XVector vector, double scalar)
        {
            return new XVector(vector._x * scalar,
                              vector._y * scalar);
        }

        /// <summary>
        /// Multiply: Vector * double
        /// </summary>
        public static XVector Multiply(XVector vector, double scalar)
        {
            return new XVector(vector._x * scalar,
                              vector._y * scalar);
        }

        /// <summary>
        /// Operator double * Vector
        /// </summary>
        public static XVector operator *(double scalar, XVector vector)
        {
            return new XVector(vector._x * scalar,
                              vector._y * scalar);
        }

        /// <summary>
        /// Multiply: double * Vector
        /// </summary>
        public static XVector Multiply(double scalar, XVector vector)
        {
            return new XVector(vector._x * scalar,
                              vector._y * scalar);
        }

        /// <summary>
        /// Operator XVector / double
        /// </summary>
        public static XVector operator /(XVector vector, double scalar)
        {
            return vector * (1.0 / scalar);
        }

        /// <summary>
        /// Multiply: XVector / double
        /// </summary>
        public static XVector Divide(XVector vector, double scalar)
        {
            return vector * (1.0 / scalar);
        }

        /// <summary>
        /// Operator XVector * Matrix
        /// </summary>
        public static XVector operator *(XVector vector, XMatrix matrix)
        {
            return matrix.Transform(vector);
        }

        /// <summary>
        /// Multiply: XVector * Matrix
        /// </summary>
        public static XVector Multiply(XVector vector, XMatrix matrix)
        {
            return matrix.Transform(vector);
        }

        /// <summary>
        /// Operator XVector * XVector, interpreted as their dot product
        /// </summary>
        public static double operator *(XVector vector1, XVector vector2)
        {
            return vector1._x * vector2._x + vector1._y * vector2._y;
        }

        /// <summary>
        /// Multiply - Returns the dot product: vector1.X*vector2.X + vector1.Y*vector2.Y
        /// </summary>
        /// <returns>
        /// Returns the dot product: vector1.X*vector2.X + vector1.Y*vector2.Y
        /// </returns>
        /// <param name="vector1"> The first XVector </param>
        /// <param name="vector2"> The second XVector </param>
        public static double Multiply(XVector vector1, XVector vector2)
        {
            return vector1._x * vector2._x + vector1._y * vector2._y;
        }

        /// <summary>
        /// Determinant - Returns the determinant det(vector1, vector2)
        /// </summary>
        /// <returns>
        /// Returns the determinant: vector1.X*vector2.Y - vector1.Y*vector2.X
        /// </returns>
        /// <param name="vector1"> The first XVector </param>
        /// <param name="vector2"> The second XVector </param>
        public static double Determinant(XVector vector1, XVector vector2)
        {
            return vector1._x * vector2._y - vector1._y * vector2._x;
        }

        /// <summary>
        /// Explicit conversion to Size.  Note that since Size cannot contain negative values,
        /// the resulting size will contains the absolute values of X and Y
        /// </summary>
        /// <returns>
        /// Size - A Size equal to this XVector
        /// </returns>
        /// <param name="vector"> XVector - the XVector to convert to a Size </param>
        public static explicit operator XSize(XVector vector)
        {
            return new XSize(Math.Abs(vector._x), Math.Abs(vector._y));
        }

        /// <summary>
        /// Explicit conversion to Point
        /// </summary>
        /// <returns>
        /// Point - A Point equal to this Vector
        /// </returns>
        /// <param name="vector"> Vector - the Vector to convert to a Point </param>
        public static explicit operator XPoint(XVector vector)
        {
            return new XPoint(vector._x, vector._y);
        }
        #endregion Public Operators
    }
}
