﻿namespace Hekune.Math
{
    using System;

    using System.ComponentModel;
    using System.Xml.Serialization;
    using System.Drawing;
    using Hekune.Math;
    using Hekune.Objects;
    using Hekune.Core;
    using System.Windows.Forms;

    using Math = System.Math;

    /// <summary>
    /// vector of doubles with three components (x,y,z)
    /// </summary>
    /// <author>Richard Potter BSc(Hons)</author>
    /// <created>Jun-04</created>
    /// <modified>Jan-2014</modified>
    /// <version>1.20</version>
    /// <Changes version="1.20">
    /// Magnitude is now a property
    /// Abs(...) now returns magnitude, Recommend: use magnitude property instead
    /// Equality opeartions now have a tolerance (note that greater and less than type operations do not)
    /// IsUnit methods also have a tolerance
    /// Generic IEquatable and IComparable interfaces implemented
    /// IFormattable interface (ToString(format, format provider) implemented
    /// Mixed product function implemented
    /// </Changes>
    /// <Changes vesion="x">
    /// Square components method was pointing to the square root static method
    /// Added comments about meaning of comparison operations on two vectors
    /// Changed references to degrees to radians
    /// Changed the implementation of Angle to help avoid NaN results
    /// The vector class is now immutable, property setters have been removed and mutable methods return a new vector
    /// Updated comments to read better under intellisense.
    /// Added Projection, Rejection and Reflection operations
    /// Split up operations into regions
    /// Added scale operations which used to be accessable through the magnitude mutable property
    /// Changed the Equals(object) method to use Equals(vector)
    /// Added component rounding operations
    /// Added rotate arround x, y or z (with or without axis offsets)
    /// Added Equals overloads with an absolute tolerace parameters
    /// Added IsUnitVector overloads with absolute tolerance parameters
    /// Added IsPerpendicular overloads with absolute toleerance parameters
    /// Added CompareTo overloads with absolut tolerance parameters
    /// Fixed an infinity issue in the vector.CompareTo method
    /// Unit testing proved that NaN == NaN is not the same as Nan.Equals(NaN) - Altered .Equals method to be consistent with the .Net framework
    /// Added IsNaN methods
    /// Added NormalizeOrDefault method
    /// Added special cases where we can normalize vectors with infinite components.
    /// IsPerpendicular now uses NormalizeOrDefault to account for special cases of infinty.
    /// Renamed Double.AlmostEquals methods to AlmostEqualsWithAbsTolerance and EqualToUps.
    /// </Changes>
    /// <remarks>
    /// IComparable has been implemened on the vector type however comparing two vectors has no meaning, we are comparing the magnitude of two vectors for convinience. 
    /// It would be more accurate to compare the magnitudes explicitly.
    /// </remarks>
    [ImmutableObject(true), Serializable]
    public struct vector
        : IComparable, IComparable<vector>, IEquatable<vector>, IFormattable
    {

        #region Class Variables

        /// <summary>
        /// The X component of the vector.
        /// </summary>
        private readonly double x;

        /// <summary>
        /// The Y component of the vector.
        /// </summary>
        private readonly double y;

        /// <summary>
        /// The Z component of the vector.
        /// </summary>
        private readonly double z;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor for the vector class accepting three doubles.
        /// </summary>
        /// <param name="x">The new x value for the vector</param>
        /// <param name="y">The new y value for the vector</param>
        /// <param name="z">The new z value for the vector</param>
        /// <implementation>
        /// Uses the mutator properties for the vector components to allow verification of input (if implemented)
        /// This results in the need for pre-initialisation initialisation of the vector components to 0 
        /// Due to the necessity for struct's variables to be set in the constructor before moving control
        /// </implementation>
        public vector(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        /// <summary>
        /// Constructor for the vector class from an array.
        /// </summary>
        /// <param name="xyz">Array representing the new values for the vector</param>
        /// <implementation>
        /// Uses the VectorArray property to avoid validation code duplication 
        /// </implementation>
        /// <exception cref="System.ArgumentException">
        /// Thrown if the array argument does not contain exactly three components 
        /// </exception>
        public vector(double[] xyz)
        {
            if (xyz.Length == 3)
            {
                this.x = xyz[0];
                this.y = xyz[1];
                this.z = xyz[2];
            }
            else
            {
                throw new ArgumentException(THREE_COMPONENTS);
            }
        }

        /// <summary>
        /// Constructor for the vector class from another vector object.
        /// </summary>
        /// <param name="v1">vector representing the new values for the vector</param>
        /// <implementation>
        /// Copies values from vector v1 to this vector, does not hold a reference to object v1 
        /// </implementation>
        public vector(vector v1)
        {
            this.x = v1.X;
            this.y = v1.Y;
            this.z = v1.Z;
        }

        #endregion

        #region Accessors & Mutators

        /// <summary>
        /// Get the x component of the vector.
        /// </summary>
        public double X
        {
            get
            {
                return this.x;
            }
        }

        /// <summary>
        /// Get the y component of the vector.
        /// </summary>
        public double Y
        {
            get
            {
                return this.y;
            }
        }

        /// <summary>
        /// Get the z component of the vector.
        /// </summary>
        public double Z
        {
            get
            {
                return this.z;
            }
        }

        /// <summary>
        /// Gets the magnitude (aka. length or absolute value) of the vector
        /// </summary>
        public double Magnitude
        {
            get
            {
                return Math.Sqrt(this.SumComponentSqrs());
            }
        }

        /// <summary>
        /// Gets the vector as an array.
        /// </summary> 
        [XmlIgnore]
        public double[] Array
        {
            get
            {
                return new[] { this.x, this.y, this.z };
            }
        }

        /// <summary>
        /// An index accessor for a vector, mapping index [0] -> X, [1] -> Y and [2] -> Z.
        /// </summary>
        /// <param name="index">The array index referring to a component within the vector (i.e. x, y, z)</param>
        /// <exception cref="System.ArgumentException">
        /// Thrown if the requested index does not corrispond to one of the three components x,y,z
        /// </exception>
        public double this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return this.X;
                    case 1:
                        return this.Y;
                    case 2:
                        return this.Z;
                    default:
                        throw new ArgumentException(THREE_COMPONENTS, "index");
                }
            }
        }

        #endregion

        #region Operators

        /// <summary>
        /// Addition of two vectors.
        /// </summary>
        /// <param name="v1">vector to be added to </param>
        /// <param name="v2">vector to be added</param>
        /// <returns>vector representing the sum of two Vectors</returns>
        /// <Acknowledgement>This code is adapted from CSOpenGL - Lucas Viñas Livschitz </Acknowledgement>
        public static vector operator +(vector v1, vector v2)
        {
            return new vector(
                v1.X + v2.X,
                v1.Y + v2.Y,
                v1.Z + v2.Z);
        }

        public static vector operator +(vector v1, transformator trans)
        {
            vector vDest = v1;
            vDest = vector.Yaw(vDest, trans.yaw);
            vDest = vector.Pitch(vDest, trans.pitch);
            vDest = vector.Roll(vDest, trans.roll);
            vDest += trans.position;
            return vDest;
        }

        public static vector operator *(vector v1, transformator trans)
        {
            vector vDest = v1;
            vDest = vector.Yaw(vDest, trans.yaw);
            vDest = vector.Pitch(vDest, trans.pitch);
            vDest = vector.Roll(vDest, trans.roll);
            return vDest;
        }

        public static vector operator -(vector v1, transformator trans)
        {
            
            vector vDest = v1;
            vDest -= trans.position;
            vDest *= trans;
            return vDest + trans.position;
        }

        public Point ToD()
        {
                vector coord = this;
                coord -= projectInfo.mainCamera.posNRot;
            double theZ = coord.Z;
            if (theZ > 0) { coord /= System.Math.Pow(theZ, projectInfo.mainCamera.depthStretch); }
            else { coord *= 20000; }
            PointF dest2D = new PointF(Convert.ToSingle(coord.X * projectInfo.halfCanvasSize.Width + projectInfo.halfCanvasSize.Width),
                    Convert.ToSingle(coord.Y * projectInfo.halfCanvasSize.Height + projectInfo.halfCanvasSize.Height));
            if (dest2D.X < Int16.MinValue) dest2D.X = Int16.MinValue;
            if (dest2D.X > Int16.MaxValue) dest2D.X = Int16.MaxValue;
            if (dest2D.Y < Int16.MinValue) dest2D.Y = Int16.MinValue;
            if (dest2D.Y > Int16.MaxValue) dest2D.Y = Int16.MaxValue;
            return new Point(Convert.ToInt16(dest2D.X), Convert.ToInt16(dest2D.Y));
        }

        /// <summary>
        /// Subtraction of two vectors.
        /// </summary>
        /// <param name="v1">vector to be subtracted from </param>
        /// <param name="v2">vector to be subtracted</param>
        /// <returns>vector representing the difference of two Vectors</returns>
        /// <Acknowledgement>This code is adapted from CSOpenGL - Lucas Viñas Livschitz </Acknowledgement>
        public static vector operator -(vector v1, vector v2)
        {
            return new vector(
                v1.X - v2.X,
                v1.Y - v2.Y,
                v1.Z - v2.Z);
        }

        /// <summary>
        /// Product of a vector and a scalar value.
        /// </summary>
        /// <param name="v1">vector to be multiplied </param>
        /// <param name="s2">Scalar value to be multiplied by </param>
        /// <returns>vector representing the product of the vector and scalar</returns>
        /// <Acknowledgement>This code is adapted from CSOpenGL - Lucas Viñas Livschitz </Acknowledgement>
        public static vector operator *(vector v1, double s2)
        {
            return
                new vector(
                        v1.X * s2,
                        v1.Y * s2,
                        v1.Z * s2);
        }

        /// <summary>
        /// Product of a scalar value and a vector.
        /// </summary>
        /// <param name="s1">Scalar value to be multiplied </param>
        /// <param name="v2">vector to be multiplied by </param>
        /// <returns>vector representing the product of the scalar and vector</returns>
        /// <Acknowledgement>This code is adapted from CSOpenGL - Lucas Viñas Livschitz </Acknowledgement>
        /// <Implementation>
        /// Using the commutative law 'scalar x vector'='vector x scalar'.
        /// Thus, this function calls 'operator*(vector v1, double s2)'.
        /// This avoids repetition of code.
        /// </Implementation>
        public static vector operator *(double s1, vector v2)
        {
            return v2 * s1;
        }

        /// <summary>
        /// Division of a vector and a scalar value.
        /// </summary>
        /// <param name="v1">vector to be divided </param>
        /// <param name="s2">Scalar value to be divided by </param>
        /// <returns>vector representing the division of the vector and scalar</returns>
        /// <Acknowledgement>This code is adapted from CSOpenGL - Lucas Viñas Livschitz </Acknowledgement>
        public static vector operator /(vector v1, double s2)
        {
            return new vector(
                        v1.X / s2,
                        v1.Y / s2,
                        v1.Z / s2);
        }

        /// <summary>
        /// Negation of a vector.
        /// Invert the direction of the vector
        /// Make vector negative (-vector)
        /// </summary>
        /// <param name="v1">vector to be negated  </param>
        /// <returns>Negated vector</returns>
        /// <Acknowledgement>This code is adapted from Exocortex - Ben Houston </Acknowledgement>
        public static vector operator -(vector v1)
        {
            return new vector(
                -v1.X,
                -v1.Y,
                -v1.Z);
        }

        /// <summary>
        /// Reinforcement of a vector.
        /// Make vector positive (+vector).
        /// </summary>
        /// <param name="v1">vector to be reinforced </param>
        /// <returns>Reinforced vector</returns>
        /// <Acknowledgement>This code is adapted from Exocortex - Ben Houston </Acknowledgement>
        /// <Implementation>
        /// Using the rules of Addition (i.e. '+-x' = '-x' and '++x' = '+x')
        /// This function actually  does nothing but return the argument as given
        /// </Implementation>
        public static vector operator +(vector v1)
        {
            return new vector(
                +v1.X,
                +v1.Y,
                +v1.Z);
        }

        /// <summary>
        /// Compare the magnitude of two vectors (less than).
        /// </summary>
        /// <param name="v1">vector to be compared </param>
        /// <param name="v2">vector to be compared with</param>
        /// <returns>True if v1 less than v2</returns>
        /// <remarks>
        /// Comparing two vectors has no meaning, we are comparing the magnitude of two vectors for convinience. 
        /// It would be more accurate to compare the magnitudes explicitly using v1.Magnitude < v2.Magnitude. 
        /// </remarks>
        public static bool operator <(vector v1, vector v2)
        {
            return v1.SumComponentSqrs() < v2.SumComponentSqrs();
        }

        /// <summary>
        /// Compare the magnitude of two vectors (greater than).
        /// </summary>
        /// <param name="v1">vector to be compared </param>
        /// <param name="v2">vector to be compared with</param>
        /// <returns>True if v1 greater than v2</returns>
        /// /// <remarks>
        /// Comparing two vectors has no meaning, we are comparing the magnitude of two vectors for convinience. 
        /// It would be more accurate to compare the magnitudes explicitly using v1.Magnitude > v2.Magnitude. 
        /// </remarks>
        public static bool operator >(vector v1, vector v2)
        {
            return v1.SumComponentSqrs() > v2.SumComponentSqrs();
        }

        /// <summary>
        /// Compare the magnitude of two vectors (less than or equal to).
        /// </summary>
        /// <param name="v1">vector to be compared </param>
        /// <param name="v2">vector to be compared with</param>
        /// <returns>True if v1 less than or equal to v2</returns>
        /// /// <remarks>
        /// Comparing two vectors has no meaning, we are comparing the magnitude of two vectors for convinience. 
        /// It would be more accurate to compare the magnitudes explicitly using v1.Magnitude <= v2.Magnitude. 
        /// </remarks>
        public static bool operator <=(vector v1, vector v2)
        {
            return v1.SumComponentSqrs() <= v2.SumComponentSqrs();
        }

        /// <summary>
        /// Compare the magnitude of two vectors (greater than or equal to).
        /// </summary>
        /// <param name="v1">vector to be compared </param>
        /// <param name="v2">vector to be compared with</param>
        /// <returns>True if v1 greater than or equal to v2</returns>
        /// /// <remarks>
        /// Comparing two vectors has no meaning, we are comparing the magnitude of two vectors for convinience. 
        /// It would be more accurate to compare the magnitudes explicitly using v1.Magnitude >= v2.Magnitude. 
        /// </remarks>
        public static bool operator >=(vector v1, vector v2)
        {
            return v1.SumComponentSqrs() >= v2.SumComponentSqrs();
        }

        /// <summary>
        /// Compare two Vectors for equality.
        /// Are two Vectors equal.
        /// </summary>
        /// <param name="v1">vector to be compared for equality </param>
        /// <param name="v2">vector to be compared to </param>
        /// <returns>Boolean decision (truth for equality)</returns>
        /// <implementation>
        /// Checks the equality of each pair of components, all pairs must be equal
        /// </implementation>
        public static bool operator ==(vector v1, vector v2)
        {
            return
                v1.X == v2.X &&
                v1.Y == v2.Y &&
                v1.Z == v2.Z;
        }

        /// <summary>
        /// Negative comparator of two Vectors.
        /// Are two Vectors different.
        /// </summary>
        /// <param name="v1">vector to be compared for in-equality </param>
        /// <param name="v2">vector to be compared to </param>
        /// <returns>Boolean decision (truth for in-equality)</returns>
        /// <Acknowledgement>This code is adapted from CSOpenGL - Lucas Viñas Livschitz </Acknowledgement>
        /// <implementation>
        /// Uses the equality operand function for two vectors to prevent code duplication
        /// </implementation>
        public static bool operator !=(vector v1, vector v2)
        {
            return !(v1 == v2);
        }

        #endregion

        #region Magnitude operation

        /// <summary>
        /// Scale a vector.
        /// Change the magnitude of a vector. 
        /// </summary>
        /// <param name="vector">The vector on which to set the magnitude</param>
        /// <param name="magnitude">The magnitude to be set</param>
        /// <returns></returns>
        public static vector Scale(vector vector, double magnitude)
        {
            if (magnitude < 0)
            {
                throw new ArgumentOutOfRangeException("magnitude", magnitude, NEGATIVE_MAGNITUDE);
            }

            if (vector == new vector(0, 0, 0))
            {
                throw new ArgumentException(ORIGIN_VECTOR_MAGNITUDE, "vector");
            }

            return vector * (magnitude / vector.Magnitude);
        }

        /// <summary>
        /// Scale this vector.
        /// Change the magnitude of this vector.
        /// </summary>
        /// <param name="magnitude">The magnitude to be set</param>
        /// <returns>A vector based on this vector with the given magnitude</returns>
        public vector Scale(double magnitude)
        {
            return vector.Scale(this, magnitude);
        }

        #endregion

        #region Product Operations

        /// <summary>
        /// Determine the cross product of two Vectors.
        /// Determine the vector product.
        /// Determine the normal vector (vector 90° to the plane).
        /// </summary>
        /// <param name="v1">The vector to multiply</param>
        /// <param name="v2">The vector to multiply by</param>
        /// <returns>vector representing the cross product of the two vectors</returns>
        /// <implementation>
        /// Cross products are non commutable
        /// </implementation>
        /// <Acknowledgement>This code is adapted from CSOpenGL - Lucas Viñas Livschitz </Acknowledgement>
        public static vector CrossProduct(vector v1, vector v2)
        {
            return
                new vector(
                        v1.Y * v2.Z - v1.Z * v2.Y,
                        v1.Z * v2.X - v1.X * v2.Z,
                        v1.X * v2.Y - v1.Y * v2.X);
        }

        /// <summary>
        /// Determine the cross product of this vector and another.
        /// Determine the vector product.
        /// Determine the normal vector (vector 90° to the plane).
        /// </summary>
        /// <param name="other">The vector to multiply by</param>
        /// <returns>vector representing the cross product of the two vectors</returns>
        /// <implementation>
        /// Uses the CrossProduct function to avoid code duplication
        /// <see cref="CrossProduct(vector, vector)"/>
        /// </implementation>
        public vector CrossProduct(vector other)
        {
            return CrossProduct(this, other);
        }

        /// <summary>
        /// Determine the dot product of two vectors.
        /// </summary>
        /// <param name="v1">The vector to multiply</param>
        /// <param name="v2">The vector to multiply by</param>
        /// <returns>Scalar representing the dot product of the two vectors</returns>
        /// <implementation>
        /// </implementation>
        /// <Acknowledgement>This code is adapted from CSOpenGL - Lucas Viñas Livschitz </Acknowledgement>
        public static double DotProduct(vector v1, vector v2)
        {
            return
                v1.X * v2.X +
                v1.Y * v2.Y +
                v1.Z * v2.Z;
        }

        /// <summary>
        /// Determine the dot product of this vector and another.
        /// </summary>
        /// <param name="other">The vector to multiply by</param>
        /// <returns>Scalar representing the dot product of the two vectors</returns>
        /// <implementation>
        /// <see cref="DotProduct(vector)"/>
        /// </implementation>
        public double DotProduct(vector other)
        {
            return DotProduct(this, other);
        }

        /// <summary>
        /// Determine the mixed product of three vectors.
        /// Determine volume (with sign precision) of parallelepiped spanned on given vectors.
        /// Determine the scalar triple product of three vectors.
        /// </summary>
        /// <param name="v1">The first vector</param>
        /// <param name="v2">The second vector</param>
        /// <param name="v3">The third vector</param>
        /// <returns>Scalar representing the mixed product of the three vectors</returns>
        /// <implementation>
        /// Mixed products are non commutable
        /// <see cref="CrossProduct(vector, vector)"/>
        /// <see cref="DotProduct(vector, vector)"/>
        /// </implementation>
        /// <Acknowledgement>This code was provided by Michał Bryłka</Acknowledgement>
        public static double MixedProduct(vector v1, vector v2, vector v3)
        {
            return DotProduct(CrossProduct(v1, v2), v3);
        }

        /// <summary>
        /// Determine the mixed product of three vectors.
        /// Determine volume (with sign precision) of parallelepiped spanned on given vectors.
        /// Determine the scalar triple product of three vectors.
        /// </summary>
        /// <param name="other_v1">The second vector</param>
        /// <param name="other_v2">The third vector</param>
        /// <returns>Scalar representing the mixed product of the three vectors</returns>
        /// <implementation>
        /// Mixed products are non commutable
        /// <see cref="MixedProduct(vector, vector, vector)"/>
        /// Uses MixedProduct(vector, vector, vector) to avoid code duplication
        /// </implementation>
        public double MixedProduct(vector other_v1, vector other_v2)
        {
            return DotProduct(CrossProduct(this, other_v1), other_v2);
        }

        #endregion

        #region Normalize Operations

        /// <summary>
        /// Get the normalized unit vector with a magnitude of one.
        /// </summary>
        /// <param name="v1">The vector to be normalized.</param>
        /// <returns>The normalized vector.</returns>
        /// <implementation>
        /// Uses the <see cref="Magnitude"/> property to avoid code duplication.
        /// Uses the private <see cref="NormalizeOrNaN(vector)"/> method to avoid code duplication.
        /// </implementation>
        /// <exception cref="NormalizeVectorException">
        /// Thrown when the normalisation of a zero magnitude vector is attempted.
        /// </exception>
        /// <exception cref="NormalizeVectorException">
        /// Thrown when the normalisation of a NaN magnitude vector is attempted.
        /// </exception>
        /// <remarks>
        /// Exceptions will be thrown if the vector being normalized has a magnitude of 0 or of NaN.
        /// </remarks>
        public static vector Normalize(vector v1)
        {
            // Special Cases
            if (double.IsInfinity(v1.Magnitude))
            {
                v1 = NormalizeSpecialCasesOrOrigional(v1);

                // If this wasnt' a special case throw an exception
                if (v1.IsNaN())
                {
                }
            }

            // Check that we are not attempting to normalize a vector of magnitude 0
            if (v1.Magnitude == 0)
            {
            }

            // Check that we are not attempting to normalize a vector of NaN
            if (v1.IsNaN())
            {
            }

            // Run the normalization as usual
            return NormalizeOrNaN(v1);
        }

        /// <summary>
        /// Get the normalized unit vector with a magnitude of one.
        /// </summary>
        /// <param name="v1">The vector to be normalized.</param>
        /// <returns>
        /// Vector (0,0,0) if the magnitude is zero, vector (NaN,NaN,NaN) if the magnitude is NaN, or the normalized vector.
        /// </returns>
        /// <implementation>
        /// Uses the <see cref="Magnitude"/> property to avoid code duplication.
        /// Uses the private <see cref="NormalizeOrNaN(vector)"/> method to avoid code duplication.
        /// </implementation>
        public static vector NormalizeOrDefault(vector v1)
        {
            // Special Cases
            v1 = NormalizeSpecialCasesOrOrigional(v1);

            /* Check that we are not attempting to normalize a vector of magnitude 0;
                if we are then retun v(0,0,0) */
            if (v1.Magnitude == 0)
            {
                return Origin;
            }

            /* Check that we are not attempting to normalize a vector with NaN components;
                if we are then return v(NaN,NaN,NaN) */
            if (v1.IsNaN())
            {
                return NaN;
            }

            // Run the normalization as usual
            return NormalizeOrNaN(v1);
        }

        /// <summary>
        /// Get the normalized unit vector with a magnitude of one.
        /// </summary>
        /// <returns>The normalized vector.</returns>
        /// <implementation>
        /// Uses the static <see cref="Normalize(vector)"/> method to avoid code duplication."/> 
        /// </implementation>
        /// <exception cref="NormalizeVectorException">
        /// Thrown when the normalisation of a zero magnitude vector is attempted.
        /// </exception>
        /// <exception cref="NormalizeVectorException">
        /// Thrown when the normalisation of a NaN magnitude vector is attempted.
        /// </exception>
        /// <remarks>
        /// Exceptions will be thrown if the vector being normalized has a magnitude of 0 or of NaN.
        /// </remarks>
        public vector Normalize()
        {
            return Normalize(this);
        }

        /// <summary>
        /// Get the normalized unit vector with a magnitude of one.
        /// </summary>
        /// <returns>
        /// Vector (0,0,0) if the magnitude is zero, vector (NaN,NaN,NaN) if the magnitude is NaN, or the normalized vector.
        /// </returns>
        /// <implementation>
        /// Uses the static <see cref="NormalizeOrDefault(vector)"/> method to avoid code duplication.
        /// </implementation>
        public vector NormalizeOrDefault()
        {
            return NormalizeOrDefault(this);
        }

        /// <summary>
        /// Get the normalized unit vector with a magnitude of one.
        /// </summary>
        /// <param name="v1">The vector to be normalized</param>
        /// <returns>The normalized vector or vector (NaN,NaN,NaN) if the magnitude is 0 or NaN</returns>
        /// <implementation>
        /// Uses the <see cref="Magnitude"/> property to avoid code duplication.
        /// </implementation>
        /// <remarks>This normalization method does not take account of special cases.</remarks>
        /// <Acknowledgement>This code is adapted from Exocortex - Ben Houston </Acknowledgement>
        private static vector NormalizeOrNaN(vector v1)
        {
            // find the inverse of the vectors magnitude
            double inverse = 1 / v1.Magnitude;

            return new vector(
                // multiply each component by the inverse of the magnitude
                v1.X * inverse,
                v1.Y * inverse,
                v1.Z * inverse);
        }

        /// <summary>
        /// This method is used to normalize special cases of vectors where the components are infinite and/or zero only.
        /// Other vectors will be returned un-normalized.
        /// </summary>
        /// <param name="v1">The vector to be normalized if it is a special case</param>
        /// <returns>Normialized special case vectors, NaN or the origional vector</returns>
        private static vector NormalizeSpecialCasesOrOrigional(vector v1)
        {
            if (double.IsInfinity(v1.Magnitude))
            {
                var x = v1.X == 0 ? 0 : v1.X == -0 ? -0 : double.IsPositiveInfinity(v1.X) ? 1 : double.IsNegativeInfinity(v1.X) ? -1 : double.NaN;
                var y = v1.Y == 0 ? 0 : v1.Y == -0 ? -0 : double.IsPositiveInfinity(v1.Y) ? 1 : double.IsNegativeInfinity(v1.Y) ? -1 : double.NaN;
                var z = v1.Z == 0 ? 0 : v1.Z == -0 ? -0 : double.IsPositiveInfinity(v1.Z) ? 1 : double.IsNegativeInfinity(v1.Z) ? -1 : double.NaN;

                return new vector(x, y, z);
            }

            return v1;
        }

        #endregion

        #region Interpolation Operations

        /// <summary>
        /// Take an interpolated value from between two vectors or an extrapolated value if allowed.
        /// </summary>
        /// <param name="v1">The vector to interpolate from (where control ==0)</param>
        /// <param name="v2">The vector to interpolate to (where control ==1)</param>
        /// <param name="control">The interpolated point between the two vectors to retrieve (fraction between 0 and 1), or an extrapolated point if allowed</param>
        /// <param name="allowExtrapolation">True if the control may represent a point not on the vertex between v1 and v2</param>
        /// <returns>The value at an arbitrary distance (interpolation) between two vectors or an extrapolated point on the extended virtex</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown when the control is not between values of 0 and 1 and extrapolation is not allowed
        /// </exception>
        /// <Acknowledgement>This code is adapted from Exocortex - Ben Houston </Acknowledgement>
        public static vector Interpolate(vector v1, vector v2, double control, bool allowExtrapolation)
        {
            if (!allowExtrapolation && (control > 1 || control < 0))
            {
                // Error message includes information about the actual value of the argument
                throw new ArgumentOutOfRangeException(
                    "control",
                    control,
                    INTERPOLATION_RANGE + "\n" + ARGUMENT_VALUE + control);
            }

            return new vector(
                v1.X * (1 - control) + v2.X * control,
                v1.Y * (1 - control) + v2.Y * control,
                v1.Z * (1 - control) + v2.Z * control);
        }

        /// <summary>
        /// Take an interpolated value from between two vectors.
        /// </summary>
        /// <param name="v1">The vector to interpolate from (where control ==0)</param>
        /// <param name="v2">The vector to interpolate to (where control ==1)</param>
        /// <param name="control">The interpolated point between the two vectors to retrieve (fraction between 0 and 1)</param>
        /// <returns>The value at an arbitrary distance (interpolation) between two vectors</returns>
        /// <implementation>
        /// <see cref="Interpolate(vector, vector, double, bool)"/>
        /// Uses the Interpolate(vector,vector,double,bool) method to avoid code duplication
        /// </implementation>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown when the control is not between values of 0 and 1
        /// </exception>
        /// <Acknowledgement>This code is adapted from Exocortex - Ben Houston </Acknowledgement>
        public static vector Interpolate(vector v1, vector v2, double control)
        {
            return Interpolate(v1, v2, control, false);
        }


        /// <summary>
        /// Take an interpolated value from between two Vectors.
        /// </summary>
        /// <param name="other">The vector to interpolate to (where control ==1)</param>
        /// <param name="control">The interpolated point between the two vectors to retrieve (fraction between 0 and 1)</param>
        /// <returns>The value at an arbitrary distance (interpolation) between two vectors</returns>
        /// <implementation>
        /// <see cref="Interpolate(vector, vector, double)"/>
        /// Overload for Interpolate method, finds an interpolated value between this vector and another
        /// Uses the Interpolate(vector,vector,double) method to avoid code duplication
        /// </implementation>
        public vector Interpolate(vector other, double control)
        {
            return Interpolate(this, other, control);
        }

        /// <summary>
        /// Take an interpolated value from between two vectors or an extrapolated value if allowed.
        /// </summary>
        /// <param name="other">The vector to interpolate to (where control ==1)</param>
        /// <param name="control">The interpolated point between the two vectors to retrieve (fraction between 0 and 1), or an extrapolated point if allowed</param>
        /// <param name="allowExtrapolation">True if the control may represent a point not on the vertex between v1 and v2</param>
        /// <returns>The value at an arbitrary distance (interpolation) between two vectors or an extrapolated point on the extended virtex</returns>
        /// <implementation>
        /// <see cref="Interpolate(vector, vector, double, bool)"/>
        /// Uses the Interpolate(vector,vector,double,bool) method to avoid code duplication
        /// </implementation>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown when the control is not between values of 0 and 1 and extrapolation is not allowed
        /// </exception>
        public vector Interpolate(vector other, double control, bool allowExtrapolation)
        {
            return Interpolate(this, other, control);
        }

        #endregion

        #region Distance Operations

        /// <summary>
        /// Find the distance between two vectors.
        /// Apply Pythagoras theorem on two vectors.
        /// </summary>
        /// <param name="v1">The vector to find the distance from </param>
        /// <param name="v2">The vector to find the distance to </param>
        /// <returns>The distance between two Vectors</returns>
        /// <implementation>
        /// </implementation>
        public static double Distance(vector v1, vector v2)
        {
            return Math.Sqrt(
                (v1.X - v2.X) * (v1.X - v2.X) +
                (v1.Y - v2.Y) * (v1.Y - v2.Y) +
                (v1.Z - v2.Z) * (v1.Z - v2.Z));
        }

        /// <summary>
        /// Find the distance between two vectors.
        /// Apply Pythagoras theorem on two vectors.
        /// </summary>
        /// <param name="other">The vector to find the distance to </param>
        /// <returns>The distance between two Vectors</returns>
        /// <implementation>
        /// <see cref="Distance(vector, vector)"/>
        /// Overload for Distance method, finds distance between this vector and another
        /// Uses the Distance(vector,vector) method to avoid code duplication
        /// </implementation>
        public double Distance(vector other)
        {
            return Distance(this, other);
        }

        #endregion

        #region Angle Oprations

        /// <summary>
        /// Find the angle between two vectors.
        /// </summary>
        /// <param name="v1">The vector to discern the angle from </param>
        /// <param name="v2">The vector to discern the angle to</param>
        /// <returns>The angle between two positional Vectors</returns>
        /// <implementation>
        /// </implementation>
        /// <Acknowledgement>F.Hill, 2001, Computer Graphics using OpenGL, 2ed </Acknowledgement>
        /// <Acknowledgement>Wrapping this calculation in Math.Min(...) was suggested by Dennis E. Cox</Acknowledgement>
        public static double Angle(vector v1, vector v2)
        {
            /* If the two vectors are equal then the angle is 0 even if there are infintite components to the vectors.
               If there are NaN components then the angle should be NaN */
            if (v1 == v2)
            {
                return 0;
            }

            return
                Math.Acos(
                    Math.Min(1.0f, NormalizeOrDefault(v1).DotProduct(NormalizeOrDefault(v2))));
        }

        /// <summary>
        /// Find the angle between this vector and another.
        /// </summary>
        /// <param name="other">The vector to discern the angle to</param>
        /// <returns>The angle between two positional Vectors</returns>
        /// <implementation>
        /// <see cref="Angle(vector, vector)"/>
        /// Uses the Angle(vector,vector) method to avoid code duplication
        /// </implementation>
        public double Angle(vector other)
        {
            return Angle(this, other);
        }

        #endregion

        #region Min and Max Operations

        /// <summary>
        /// Compare the magnitude of two vectors and return the greater.
        /// </summary>
        /// <param name="v1">The vector to compare</param>
        /// <param name="v2">The vector to compare with</param>
        /// <returns>
        /// The greater of the two Vectors (based on magnitude)
        /// </returns>
        /// <Acknowledgement>This code is adapted from Exocortex - Ben Houston </Acknowledgement>
        /// <remarks>
        /// Comparing two vectors has no meaning, we are comparing the magnitude of two vectors for convinience. 
        /// It would be more accurate to compare the magnitudes explicitly using Math.Max(v1.Magnitude, v2.Magnitude). 
        /// </remarks>
        public static vector Max(vector v1, vector v2)
        {
            return v1 >= v2 ? v1 : v2;
        }

        /// <summary>
        /// Compare the magnitude of two vectors and return the greater.
        /// </summary>
        /// <param name="other">The vector to compare with</param>
        /// <returns>
        /// The greater of the two Vectors (based on magnitude)
        /// </returns>
        /// <implementation>
        /// <see cref="Max(vector, vector)"/>
        /// Uses function Max(vector, vector) to avoid code duplication
        /// </implementation>
        /// <remarks>
        /// Comparing two vectors has no meaning, we are comparing the magnitude of two vectors for convinience. 
        /// It would be more accurate to compare the magnitudes explicitly using v1.Magnitude.Max(v2.Magnitude). 
        /// </remarks>
        public vector Max(vector other)
        {
            return Max(this, other);
        }

        /// <summary>
        /// Compares the magnitude of two vectors and return the lesser.
        /// </summary>
        /// <param name="v1">The vector to compare</param>
        /// <param name="v2">The vector to compare with</param>
        /// <returns>
        /// The lesser of the two Vectors (based on magnitude)
        /// </returns>
        /// <Acknowledgement>This code is adapted from Exocortex - Ben Houston </Acknowledgement>
        /// /// <remarks>
        /// Comparing two vectors has no meaning, we are comparing the magnitude of two vectors for convinience. 
        /// It would be more accurate to compare the magnitudes explicitly using Math.Min(v1.Magnitude,v2.Magnitude). 
        /// </remarks>
        public static vector Min(vector v1, vector v2)
        {
            return v1 <= v2 ? v1 : v2;
        }

        /// <summary>
        /// Compare the magnitude of two vectors and return the greater.
        /// </summary>
        /// <param name="other">The vector to compare with</param>
        /// <returns>
        /// The lesser of the two Vectors (based on magnitude)
        /// </returns>
        /// <implementation>
        /// <see cref="Min(vector, vector)"/>
        /// Uses function Min(vector, vector) to avoid code duplication
        /// </implementation>
        /// <remarks>
        /// Comparing two vectors has no meaning, we are comparing the magnitude of two vectors for convinience. 
        /// It would be more accurate to compare the magnitudes explicitly using v1.Magnitude.Min(v2.Magnitude). 
        /// </remarks>
        public vector Min(vector other)
        {
            return Min(this, other);
        }

        #endregion

        #region Rotation Operations

        #region Yaw, Pitch and Roll Operations

        /// <summary>
        /// Rotates a vector around the Y axis.
        /// Change the yaw of a vector.
        /// </summary>
        /// <param name="v1">The vector to be rotated</param>
        /// <param name="rad">The angle to rotate the vector around in radians</param>
        /// <returns>vector representing the rotation around the Y axis</returns>
        /// <remarks>This assumes a cartesian axis system of up:Y, right:X, depth:-Z</remarks>
        public static vector Yaw(vector v1, double rad)
        {
            return RotateY(v1, rad);
        }

        /// <summary>
        /// Rotates a vector around the Y axis.
        /// Change the yaw of the vector.
        /// </summary>
        /// <param name="rad">The angle to rotate the vector around in radians</param>
        /// <returns>vector representing the rotation around the Y axis</returns>
        /// <implementation>
        /// <see cref="Yaw(vector, Double)"/>
        /// Uses function Yaw(vector, double) to avoid code duplication
        /// </implementation>
        /// <remarks>This assumes a cartesian axis system of up:Y, right:X, depth:-Z</remarks>
        public vector Yaw(double rad)
        {
            return Yaw(this, rad);
        }

        /// <summary>
        /// Rotates a vector around the X axis.
        /// Change the pitch of a vector.
        /// </summary>
        /// <param name="v1">The vector to be rotated</param>
        /// <param name="rad">The angle to rotate the vector around in radians</param>
        /// <returns>vector representing the rotation around the X axis</returns>
        /// <remarks>This assumes a cartesian axis system of up:Y, right:X, depth:-Z</remarks>
        public static vector Pitch(vector v1, double rad)
        {
            return RotateX(v1, rad);
        }

        /// <summary>
        /// Rotates a vector around the X axis.
        /// Change the pitch of a vector.
        /// </summary>
        /// <param name="rad">The angle to rotate the vector around in radians</param>
        /// <returns>vector representing the rotation around the X axis</returns>
        /// <see cref="Pitch(vector, Double)"/>
        /// <implementation>
        /// Uses function Pitch(vector, double) to avoid code duplication
        /// </implementation>
        /// <remarks>This assumes a cartesian axis system of up:Y, right:X, depth:-Z</remarks>
        public vector Pitch(double rad)
        {
            return Pitch(this, rad);
        }

        /// <summary>
        /// Rotates a vector around the Z axis.
        /// Change the roll of a vector.
        /// </summary>
        /// <param name="v1">The vector to be rotated</param>
        /// <param name="rad">The angle to rotate the vector around in radians</param>
        /// <returns>vector representing the rotation around the Z axis</returns>
        /// <remarks>This assumes a cartesian axis system of up:Y, right:X, depth:-Z</remarks>
        public static vector Roll(vector v1, double rad)
        {
            return RotateZ(v1, rad);
        }

        /// <summary>
        /// Rotates a vector around the Z axis.
        /// Change the roll of a vector.
        /// </summary>
        /// <param name="rad">The angle to rotate the vector around in radians</param>
        /// <returns>vector representing the rotation around the Z axis</returns>
        /// <implementation>
        /// <see cref="Roll(vector, Double)"/>
        /// Uses function Roll(vector, double) to avoid code duplication
        /// </implementation>
        /// <remarks>This assumes a cartesian axis system of up:Y, right:X, depth:-Z</remarks>
        public vector Roll(double rad)
        {
            return Roll(this, rad);
        }

        #endregion

        #region Rotate Around X, Y or Z Operations

        /// <summary>
        /// Rotates a vector around the X axis.
        /// </summary>
        /// <param name="v1">The vector to be rotated</param>
        /// <param name="rad">The angle to rotate the vector around in radians</param>
        /// <returns>Vector representing the rotation around the X axis</returns>
        public static vector RotateX(vector v1, double rad)
        {
            double x = v1.X;
            double y = (v1.Y * Math.Cos(rad)) - (v1.Z * Math.Sin(rad));
            double z = (v1.Y * Math.Sin(rad)) + (v1.Z * Math.Cos(rad));
            return new vector(x, y, z);
        }

        /// <summary>
        /// Rotates this vector around the X axis.
        /// </summary>
        /// <param name="rad">The angle to rotate the vector around in radians</param>
        /// <returns>Vector representing the rotation around the X axis</returns>
        public vector RotateX(double rad)
        {
            return RotateX(this, rad);
        }

        /// <summary>
        /// Rotates a vector around the Y axis.
        /// </summary>
        /// <param name="v1">The vector to be rotated</param>
        /// <param name="rad">The angle to rotate the vector around in radians</param>
        /// <returns>Vector representing the rotation around the Y axis</returns>
        public static vector RotateY(vector v1, double rad)
        {
            double x = (v1.Z * Math.Sin(rad)) + (v1.X * Math.Cos(rad));
            double y = v1.Y;
            double z = (v1.Z * Math.Cos(rad)) - (v1.X * Math.Sin(rad));
            return new vector(x, y, z);
        }

        /// <summary>
        /// Rotates this vector around the Y axis.
        /// </summary>
        /// <param name="rad">The angle to rotate the vector around in radians</param>
        /// <returns>Vector representing the rotation around the Y axis</returns>
        public vector RotateY(double rad)
        {
            return RotateY(this, rad);
        }

        /// <summary>
        /// Rotates a vector around the Z axis.
        /// </summary>
        /// <param name="v1">The vector to be rotated</param>
        /// <param name="rad">The angle to rotate the vector around in radians</param>
        /// <returns>Vector representing the rotation around the Z axis</returns>
        public static vector RotateZ(vector v1, double rad)
        {
            double x = (v1.X * Math.Cos(rad)) - (v1.Y * Math.Sin(rad));
            double y = (v1.X * Math.Sin(rad)) + (v1.Y * Math.Cos(rad));
            double z = v1.Z;
            return new vector(x, y, z);
        }

        /// <summary>
        /// Rotates this vector around the Z axis.
        /// </summary>
        /// <param name="rad">The angle to rotate the vector around in radians</param>
        /// <returns>Vector representing the rotation around the Z axis</returns>
        public vector RotateZ(double rad)
        {
            return RotateZ(this, rad);
        }

        #endregion

        #region Arbitrary rotation

        /// <summary>
        /// Rotates a vector around the X axis with offsets for Y and Z
        /// </summary>
        /// <param name="v1">The vector to be rotated</param>
        /// <param name="rad">The angle to rotate the vector around in radians</param>
        /// <param name="yOff">The Y axis offset</param>
        /// <param name="zOff">The Z axis offset</param>
        /// <returns>Vector representing the rotation around the X axis</returns>
        public static vector RotateX(vector v1, double yOff, double zOff, double rad)
        {
            double x = v1.X;
            double y = (v1.Y * Math.Cos(rad)) - (v1.Z * Math.Sin(rad)) + (yOff * (1 - Math.Cos(rad)) + zOff * Math.Sin(rad));
            double z = (v1.Y * Math.Sin(rad)) + (v1.Z * Math.Cos(rad)) + (zOff * (1 - Math.Cos(rad)) - yOff * Math.Sin(rad));
            return new vector(x, y, z);
        }

        /// <summary>
        /// Rotates this vector around the X axis with offsets for Y and Z
        /// </summary>
        /// <param name="rad">The angle to rotate the vector around in radians</param>
        /// <param name="yOff">The Y axis offset</param>
        /// <param name="zOff">The Z axis offset</param>
        /// <returns>Vector representing the rotation around the X axis</returns>
        public vector RotateX(double yOff, double zOff, double rad)
        {
            return RotateX(this, yOff, zOff, rad);
        }

        /// <summary>
        /// Rotates a vector around the Y axis with offsets for X and Z
        /// </summary>
        /// <param name="v1">The vector to be rotated</param>
        /// <param name="rad">The angle to rotate the vector around in radians</param>
        /// <param name="xOff">The X axis offset</param>
        /// <param name="zOff">The Z axis offset</param>
        /// <returns>Vector representing the rotation around the X axis</returns>
        public static vector RotateY(vector v1, double xOff, double zOff, double rad)
        {
            double x = (v1.Z * Math.Sin(rad)) + (v1.X * Math.Cos(rad)) + (xOff * (1 - Math.Cos(rad)) - zOff * Math.Sin(rad));
            double y = v1.Y;
            double z = (v1.Z * Math.Cos(rad)) - (v1.X * Math.Sin(rad)) + (zOff * (1 - Math.Cos(rad)) + xOff * Math.Sin(rad));
            return new vector(x, y, z);
        }

        /// <summary>
        /// Rotates this vector around the Y axis with offsets for X and Z
        /// </summary>
        /// <param name="rad">The angle to rotate the vector around in radians</param>
        /// <param name="xOff">The X axis offset</param>
        /// <param name="zOff">The Z axis offset</param>
        /// <returns>Vector representing the rotation around the X axis</returns>
        public vector RotateY(double xOff, double zOff, double rad)
        {
            return RotateY(this, xOff, zOff, rad);
        }

        /// <summary>
        /// Rotates a vector around the Z axis with offsets for X and Y
        /// </summary>
        /// <param name="v1">The vector to be rotated</param>
        /// <param name="rad">The angle to rotate the vector around in radians</param>
        /// <param name="xOff">The X axis offset</param>
        /// <param name="yOff">The Y axis offset</param>
        /// <returns>Vector representing the rotation around the X axis</returns>
        public static vector RotateZ(vector v1, double xOff, double yOff, double rad)
        {
            double x = (v1.X * Math.Cos(rad)) - (v1.Y * Math.Sin(rad)) + (xOff * (1 - Math.Cos(rad)) + yOff * Math.Sin(rad));
            double y = (v1.X * Math.Sin(rad)) + (v1.Y * Math.Cos(rad)) + (yOff * (1 - Math.Cos(rad)) - xOff * Math.Sin(rad));
            double z = v1.Z;
            return new vector(x, y, z);
        }

        /// <summary>
        /// Rotates this vector around the Z axis with offsets for X and Y
        /// </summary>
        /// <param name="rad">The angle to rotate the vector around in radians</param>
        /// <param name="xOff">The X axis offset</param>
        /// <param name="yOff">The Y axis offset</param>
        /// <returns>Vector representing the rotation around the X axis</returns>
        public vector RotateZ(double xOff, double yOff, double rad)
        {
            return RotateZ(this, xOff, yOff, rad);
        }

        #endregion

        #endregion

        #region Projection, Rejection and Reflection Operations

        /// <summary>
        /// Projects the specified v1 onto the specified v2.
        /// The vector resolute of v1 in the direction of v2.
        /// </summary>
        /// <param name="v1">The vector that will be projected.</param>
        /// <param name="v2">A nonzero vector that v1 will be projected upon. The direction to project.</param>
        /// <returns>The projected vector</returns>
        /// <acknowlagement>Provided by Poggel, Steven Buraje in comments on CodeProject ( http://www.codeproject.com/Articles/17425/A-Vector-Type-for-C )</acknowlagement>
        public static vector Projection(vector v1, vector v2)
        {
            return new vector(v2 * (v1.DotProduct(v2) / Math.Pow(v2.Magnitude, 2)));
        }

        /// <summary>
        /// Projects the vector onto the specified vector.
        /// The vector resolute this vector in a direction specified by a vector.
        /// </summary>
        /// <param name="direction">A nonzero vector that the vector will be projected upon. The direction to project.</param>
        /// <returns>The projected vector</returns>
        /// <acknowlagement>Provided by Poggel, Steven Buraje in comments on CodeProject ( http://www.codeproject.com/Articles/17425/A-Vector-Type-for-C )</acknowlagement>
        public vector Projection(vector direction)
        {
            return Projection(this, direction);
        }

        /// <summary>
        /// Vector rejection of the specified v1 onto the specified v2.
        /// </summary>
        /// <param name="v1">The vector subject of vector rejection.</param>
        /// <param name="v2">A nonzero vector for which the vector rejection of v1 will be calculated. The direction.</param>
        /// <returns>The vector rejection of v1 onto v2</returns>
        public static vector Rejection(vector v1, vector v2)
        {
            return v1 - v1.Projection(v2);
        }

        /// <summary>
        /// Vector rejection of the this vector onto the specified vector.
        /// </summary>
        /// <param name="direction">A nonzero vector for which the vector rejection of this vector will be calculated. The direction.</param>
        /// <returns>The vector rejection of this vector and the given direction</returns>
        public vector Rejection(vector direction)
        {
            return Rejection(this, direction);
        }

        /// <summary>
        /// Refelct a vector about 
        /// </summary>
        /// <param name="reflector"></param>
        /// <returns></returns>
        public vector Reflection(vector reflector)
        {
            this = vector.Reflection(this, reflector);
            return this;
        }

        /// <summary>
        /// Reflect v1 about v2
        /// </summary>
        /// <param name="v1">The vector to be reflected</param>
        /// <param name="v2">The vector to reflect about</param>
        /// <returns>The reflected vector</returns>
        /// <acknowlagement>Provided by Poggel, Steven Buraje in comments on CodeProject ( http://www.codeproject.com/Articles/17425/A-Vector-Type-for-C )</acknowlagement>
        public static vector Reflection(vector v1, vector v2)
        {
            // if v2 has a right angle to vector, return -vector and stop
            if (Math.Abs(Math.Abs(v1.Angle(v2)) - Math.PI / 2) < Double.Epsilon)
            {
                return -v1;
            }

            vector retval = new vector(2 * v1.Projection(v2) - v1);
            return retval.Scale(v1.Magnitude);
        }

        #endregion

        #region Abs Operations

        /// <summary>
        /// Find the absolute value of a vector.
        /// Find the magnitude of a vector.
        /// </summary>
        /// <returns>A vector representing the absolute values of the vector</returns>
        /// <implementation>
        /// An alternative interface to the magnitude property
        /// </implementation>
        public static Double Abs(vector v1)
        {
            return v1.Magnitude;
        }

        /// <summary>
        /// Find the absolute value of a vector.
        /// Find the magnitude of a vector.
        /// </summary>
        /// <returns>A vector representing the absolute values of the vector</returns>
        /// <implementation>
        /// An alternative interface to the magnitude property
        /// </implementation>
        public double Abs()
        {
            return this.Magnitude;
        }

        #endregion

        #region Component Operations

        /// <summary>
        /// The sum of a vector's components.
        /// </summary>
        /// <param name="v1">The vector whose scalar components to sum</param>
        /// <returns>The sum of the Vectors X, Y and Z components</returns>
        public static double SumComponents(vector v1)
        {
            return v1.X + v1.Y + v1.Z;
        }

        /// <summary>
        /// The sum of this vector's components.
        /// </summary>
        /// <returns>The sum of the Vectors X, Y and Z components</returns>
        /// <implementation>
        /// <see cref="SumComponents(vector)"/>
        /// The Components.SumComponents(vector) function has been used to prevent code duplication
        /// </implementation>
        public double SumComponents()
        {
            return SumComponents(this);
        }

        /// <summary>
        /// The sum of a Vector's squared components.
        /// </summary>
        /// <param name="v1">The vector whose scalar components to square and sum</param>
        /// <returns>The sum of the Vectors X^2, Y^2 and Z^2 components</returns>
        public static double SumComponentSqrs(vector v1)
        {
            vector v2 = SqrComponents(v1);
            return v2.SumComponents();
        }

        /// <summary>
        /// The sum of this vector's squared components.
        /// </summary>
        /// <returns>The sum of the Vectors X^2, Y^2 and Z^2 components</returns>
        /// <implementation>
        /// <see cref="SumComponentSqrs(vector)"/>
        /// The Components.SumComponentSqrs(vector) function has been used to prevent code duplication
        /// </implementation>
        public double SumComponentSqrs()
        {
            return SumComponentSqrs(this);
        }

        /// <summary>
        /// The individual multiplication to a power of a vectors's components.
        /// </summary>
        /// <param name="v1">The vector whose scalar components to multiply by a power</param>
        /// <param name="power">The power by which to multiply the components</param>
        /// <returns>The multiplied vector</returns>
        public static vector PowComponents(vector v1, double power)
        {
            return new vector(
                Math.Pow(v1.X, power),
                Math.Pow(v1.Y, power),
                Math.Pow(v1.Z, power));
        }

        /// <summary>
        /// The individual multiplication to a power of this vectors's components.
        /// </summary>
        /// <param name="power">The power by which to multiply the components</param>
        /// <returns>The multiplied vector</returns>
        /// <implementation>
        /// <see cref="PowComponents(vector, Double)"/>
        /// The Components.PowComponents(vector, double) function has been used to prevent code duplication
        /// </implementation>
        public vector PowComponents(double power)
        {
            return PowComponents(this, power);
        }

        /// <summary>
        /// The individual square root of a vectors's components.
        /// </summary>
        /// <param name="v1">The vector whose scalar components to square root</param>
        /// <returns>The rooted vector</returns>
        public static vector SqrtComponents(vector v1)
        {
            return new vector(
                Math.Sqrt(v1.X),
                Math.Sqrt(v1.Y),
                Math.Sqrt(v1.Z));
        }

        /// <summary>
        /// The individual square root of this vector's components.
        /// </summary>
        /// <returns>The rooted vector</returns>
        /// <implementation>
        /// <see cref="SqrtComponents(vector)"/>
        /// The Components.SqrtComponents(vector) function has been used to prevent code duplication
        /// </implementation>
        public vector SqrtComponents()
        {
            return SqrtComponents(this);
        }

        /// <summary>
        /// The vectors's components squared.
        /// </summary>
        /// <param name="v1">The vector whose scalar components are to square</param>
        /// <returns>The squared vector</returns>
        public static vector SqrComponents(vector v1)
        {
            return new vector(
                v1.X * v1.X,
                v1.Y * v1.Y,
                v1.Z * v1.Z);
        }

        /// <summary>
        /// The vectors's components squared.
        /// </summary>
        /// <returns>The squared vector</returns>
        /// <implementation>
        /// <see cref="SqrtComponents(vector)"/>
        /// The Components.SqrComponents(vector) function has been used to prevent code duplication
        /// </implementation>
        public vector SqrComponents()
        {
            return SqrComponents(this);
        }

        #region Round Components

        /// <summary>
        /// Round a vectors components to the nearest integral values.
        /// </summary>
        /// <param name="v1">The vector who's components are to be rounded</param>
        /// <returns>The rounded vector</returns>
        /// <remarks>If the fractional component of a is halfway between two integers, one of which is even and the other odd, then the even number is returned.</remarks>
        public static vector Round(vector v1)
        {
            return new vector(Math.Round(v1.X), Math.Round(v1.Y), Math.Round(v1.Z));
        }

        /// <summary>
        /// Round a vectors components to a specified precision.
        /// </summary>
        /// <param name="v1">The vector who's components are to be rounded</param>
        /// <param name="digits">The number of decimal points to round to</param>
        /// <returns>The rounded vector</returns>
        public static vector Round(vector v1, int digits)
        {
            return new vector(Math.Round(v1.X, digits), Math.Round(v1.Y, digits), Math.Round(v1.Z, digits));
        }

        /// <summary>
        /// Round a vectors components.
        /// </summary>
        /// <param name="v1">The vector who's components are to be rounded</param>
        /// <param name="mode">An enum to specify how rounding should happen for numbers midway between two other number</param>
        /// <returns>The rounded vector</returns>
        public static vector Round(vector v1, MidpointRounding mode)
        {
            return new vector(Math.Round(v1.X, mode), Math.Round(v1.Y, mode), Math.Round(v1.Z, mode));
        }

        /// <summary>
        /// Round a vectors components to a specified precision.
        /// </summary>
        /// <param name="v1">The vector who's components are to be rounded</param>
        /// <param name="digits">The number of decimal points to round to</param>
        /// <param name="mode">An enum to specify how rounding should happen for numbers midway between two other number</param>
        /// <returns>The rounded vector</returns>
        public static vector Round(vector v1, int digits, MidpointRounding mode)
        {
            return new vector(Math.Round(v1.X, digits, mode), Math.Round(v1.Y, digits, mode), Math.Round(v1.Z, digits, mode));
        }

        /// <summary>
        /// Round this vectors components to the nearest integral values.
        /// </summary>
        /// <returns>The rounded vector</returns>
        /// <remarks>If the fractional component of a is halfway between two integers, one of which is even and the other odd, then the even number is returned.</remarks>
        public vector Round()
        {
            return new vector(Math.Round(this.X), Math.Round(this.Y), Math.Round(this.Z));
        }

        /// <summary>
        /// Round this vectors components to a specified precision.
        /// </summary>
        /// <param name="digits">The number of decimal points to round to</param>
        /// <returns>The rounded vector</returns>
        public vector Round(int digits)
        {
            return new vector(Math.Round(this.X, digits), Math.Round(this.Y, digits), Math.Round(this.Z, digits));
        }

        /// <summary>
        /// Round this vectors components.
        /// </summary>
        /// <param name="mode">An enum to specify how rounding should happen for numbers midway between two other number</param>
        /// <returns>The rounded vector</returns>
        public vector Round(MidpointRounding mode)
        {
            return new vector(Math.Round(this.X, mode), Math.Round(this.Y, mode), Math.Round(this.Z, mode));
        }

        /// <summary>
        /// Round this vectors components to a specified precision.
        /// </summary>
        /// <param name="digits">The number of decimal points to round to</param>
        /// <param name="mode">An enum to specify how rounding should happen for numbers midway between two other number</param>
        /// <returns>The rounded vector</returns>
        public vector Round(int digits, MidpointRounding mode)
        {
            return new vector(Math.Round(this.X, digits, mode), Math.Round(this.Y, digits, mode), Math.Round(this.Z, digits, mode));
        }

        #endregion

        #endregion

        #region Standard Opeartions (ToString, CompareTo etc)

        /// <summary>
        /// Textual description of the vector.
        /// </summary>
        /// <Implementation>
        /// Uses ToString(string, IFormatProvider) to avoid code duplication
        /// </Implementation>
        /// <returns>Text (String) representing the vector</returns>
        public override string ToString()
        {
            return this.ToString(null, null);
        }

        /// <summary>
        /// Verbose textual description of the vector.
        /// </summary>
        /// <returns>Text (string) representing the vector</returns>
        public string ToVerbString()
        {
            string output = null;

            if (this.IsUnitVector())
            {
                output += UNIT_VECTOR;
            }
            else
            {
                output += POSITIONAL_VECTOR;
            }

            output += string.Format("( x={0}, y={1}, z={2} )", this.X, this.Y, this.Z);
            output += MAGNITUDE + this.Magnitude;

            return output;
        }

        /// <summary>
        /// Textual description of the vector.
        /// </summary>
        /// <param name="format">Formatting string: 'x','y','z' or '' followed by standard numeric format string characters valid for a double precision floating point</param>
        /// <param name="formatProvider">The culture specific fromatting provider</param>
        /// <returns>Text (String) representing the vector</returns>
        public string ToString(string format, IFormatProvider formatProvider)
        {
            // If no format is passed
            if (format == null || format == "")
            {
                return string.Format("({0}, {1}, {2})", this.X, this.Y, this.Z);
            }

            char firstChar = format[0];
            string remainder = null;

            if (format.Length > 1)
            {
                remainder = format.Substring(1);
            }

            switch (firstChar)
            {
                case 'x': return this.X.ToString(remainder, formatProvider);
                case 'y': return this.Y.ToString(remainder, formatProvider);
                case 'z': return this.Z.ToString(remainder, formatProvider);
                default:
                    return String.Format(
                        "({0}, {1}, {2})",
                        this.X.ToString(format, formatProvider),
                        this.Y.ToString(format, formatProvider),
                        this.Z.ToString(format, formatProvider));
            }
        }

        /// <summary>
        /// Get the hashcode.
        /// </summary>
        /// <returns>Hashcode for the object instance</returns>
        /// <implementation>
        /// Required in order to implement comparator operations (i.e. ==, !=)
        /// </implementation>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = this.x.GetHashCode();
                hashCode = (hashCode * 397) ^ this.y.GetHashCode();
                hashCode = (hashCode * 397) ^ this.z.GetHashCode();
                return hashCode;
            }
        }

        /// <summary>
        /// Comparator.
        /// </summary>
        /// <param name="other">The other object (which should be a vector) to compare to</param>
        /// <returns>Truth if two vectors are equal within a tolerance</returns>
        /// <implementation>
        /// Checks if the object argument is a vector object 
        /// Uses the equality operator function to avoid code duplication
        /// Required in order to implement comparator operations (i.e. ==, !=)
        /// </implementation>
        /// <remarks>NaN and NaN components will be equal in this method but not in ==, see http://blogs.msdn.com/b/shawnfa/archive/2004/07/19/187792.aspx </remarks>
        public override bool Equals(object other)
        {
            // Check object other is a vector object
            if (other is vector)
            {
                // Convert object to vector
                vector otherVector = (vector)other;

                // Check for equality
                return otherVector.Equals(this);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Comparator within a tolerance
        /// </summary>
        /// <param name="other">The other object to compare to</param>
        /// <param name="tolerance">The tolerance to use when comparing the vector components</param>
        /// <returns>Truth if two objects are vectors and are equal within a tolerance</returns>
        /// <remarks>NaN and NaN components will be equal in this method but not in ==, see http://blogs.msdn.com/b/shawnfa/archive/2004/07/19/187792.aspx </remarks>
        public bool Equals(object other, double tolerance)
        {
            if (other is vector)
            {
                return this.Equals((vector)other, tolerance);
            }
            return false;
        }

        /// <summary>
        /// Comparator.
        /// </summary>
        /// <param name="other">The other vector to compare to</param>
        /// <returns>Truth if two vectors are equal</returns>
        /// <implementation>
        /// Uses the equality operator function to avoid code duplication
        /// </implementation>
        /// <remarks>NaN and NaN components will be equal in this method but not in ==, see http://blogs.msdn.com/b/shawnfa/archive/2004/07/19/187792.aspx </remarks>
        public bool Equals(vector other)
        {
            return
               this.X.Equals(other.X) &&
               this.Y.Equals(other.Y) &&
               this.Z.Equals(other.Z);
        }

        /// <summary>
        /// Comparator within a tolerance
        /// </summary>
        /// <param name="other">The other vector to compare to</param>
        /// <param name="tolerance">The tolerance to use when comparing the vector components</param>
        /// <returns>Truth if two vectors are equal within a tolerance</returns>
        /// <remarks>NaN and NaN components will be equal in this method but not in ==, see http://blogs.msdn.com/b/shawnfa/archive/2004/07/19/187792.aspx </remarks>
        public bool Equals(vector other, double tolerance)
        {
            return
                this.X.AlmostEqualsWithAbsTolerance(other.X, tolerance) &&
                this.Y.AlmostEqualsWithAbsTolerance(other.Y, tolerance) &&
                this.Z.AlmostEqualsWithAbsTolerance(other.Z, tolerance);
        }

        /// <summary>
        /// Compares the magnitude of this instance against the magnitude of the supplied vector.
        /// </summary>
        /// <param name="other">The vector to compare this instance with</param>
        /// <returns>
        /// -1: The magnitude of this instance is less than the others magnitude
        /// 0: The magnitude of this instance equals the magnitude of the other
        /// 1: The magnitude of this instance is greater than the magnitude of the other
        /// </returns>
        /// <implementation>
        /// Implemented to fulfil the IComparable interface
        /// </implementation>
        /// <remarks>
        /// Comparing two vectors has no meaning, we are comparing the magnitude of two vectors for convinience. 
        /// It would be more accurate to compare the magnitudes explicitly using v1.Magnitude.CompareTo(v2.Magnitude). 
        /// </remarks>
        /// <Acknowledgement>This code is adapted from Exocortex - Ben Houston </Acknowledgement>
        public int CompareTo(vector other)
        {
            if (this < other)
            {
                return -1;
            }

            if (this > other)
            {
                return 1;
            }

            return 0;
        }

        /// <summary>
        /// Compares the magnitude of this instance against the magnitude of the supplied vector.
        /// </summary>
        /// <param name="other">The vector to compare this instance with</param>
        /// <returns>
        /// -1: The magnitude of this instance is less than the others magnitude
        /// 0: The magnitude of this instance equals the magnitude of the other
        /// 1: The magnitude of this instance is greater than the magnitude of the other
        /// </returns>
        /// <implementation>
        /// Implemented to fulfil the IComparable interface
        /// </implementation>
        /// <exception cref="ArgumentException">
        /// Throws an exception if the type of object to be compared is not known to this class
        /// </exception>
        /// <remarks>
        /// Comparing two vectors has no meaning, we are comparing the magnitude of two vectors for convinience. 
        /// It would be more accurate to compare the magnitudes explicitly using v1.Magnitude.CompareTo(v2.Magnitude). 
        /// </remarks>
        /// <Acknowledgement>This code is adapted from Exocortex - Ben Houston </Acknowledgement>
        public int CompareTo(object other)
        {
            if (other is vector)
            {
                return this.CompareTo((vector)other);
            }

            // Error condition: other is not a vector object
            throw new ArgumentException(
                // Error message includes information about the actual type of the argument
                NON_VECTOR_COMPARISON + "\n" + ARGUMENT_TYPE + other.GetType().ToString(),
                "other");
        }

        /// <summary>
        /// Compares the magnitude of this instance against the magnitude of the supplied vector.
        /// </summary>
        /// <param name="other">The vector to compare this instance with</param>
        /// <returns>
        /// -1: The magnitude of this instance is less than the others magnitude
        /// 0: The magnitude of this instance equals the magnitude of the other
        /// 1: The magnitude of this instance is greater than the magnitude of the other
        /// </returns>
        /// <implementation>
        /// Implemented to fulfil the IComparable interface
        /// </implementation>
        /// <remarks>
        /// Comparing two vectors has no meaning, we are comparing the magnitude of two vectors for convinience. 
        /// It would be more accurate to compare the magnitudes explicitly using v1.Magnitude.CompareTo(v2.Magnitude). 
        /// </remarks>
        /// <Acknowledgement>This code is adapted from Exocortex - Ben Houston </Acknowledgement>
        public int CompareTo(vector other, double tolerance)
        {
            var bothInfinite = double.IsInfinity(this.SumComponentSqrs()) && double.IsInfinity(other.SumComponentSqrs());

            if (this.Equals(other, tolerance) || bothInfinite)
            {
                return 0;
            }

            if (this < other)
            {
                return -1;
            }

            return 1;
        }

        /// <summary>
        /// Compares the magnitude of this instance against the magnitude of the supplied vector.
        /// </summary>
        /// <param name="other">The vector to compare this instance with</param>
        /// <returns>
        /// -1: The magnitude of this instance is less than the others magnitude
        /// 0: The magnitude of this instance equals the magnitude of the other
        /// 1: The magnitude of this instance is greater than the magnitude of the other
        /// </returns>
        /// <implementation>
        /// Implemented to fulfil the IComparable interface
        /// </implementation>
        /// <exception cref="ArgumentException">
        /// Throws an exception if the type of object to be compared is not known to this class
        /// </exception>
        /// <remarks>
        /// Comparing two vectors has no meaning, we are comparing the magnitude of two vectors for convinience. 
        /// It would be more accurate to compare the magnitudes explicitly using v1.Magnitude.CompareTo(v2.Magnitude). 
        /// </remarks>
        /// <Acknowledgement>This code is adapted from Exocortex - Ben Houston </Acknowledgement>
        public int CompareTo(object other, double tolerance)
        {
            if (other is vector)
            {
                return this.CompareTo((vector)other, tolerance);
            }

            // Error condition: other is not a vector object
            throw new ArgumentException(
                // Error message includes information about the actual type of the argument
                NON_VECTOR_COMPARISON + "\n" + ARGUMENT_TYPE + other.GetType().ToString(),
                "other");
        }

        #endregion

        #region Decisions

        /// <summary>
        /// Checks if a vector is a unit vector within a tolerance.
        /// Checks if the Vector has been normalized within a tolerance.
        /// Checks if a vector has a magnitude of 1 within a tolerance.
        /// </summary>
        /// <param name="v1">
        /// The vector to be checked for Normalization
        /// </param>
        /// <param name="tolerance">The tolerance to use when comparing the magnitude</param>
        /// <returns>Truth if the vector is a unit vector within a tolerance</returns>
        /// <implementation>
        /// Uses the <see cref="Magnitude"/> property to avoid code duplication
        /// </implementation>
        public static bool IsUnitVector(vector v1, double tolerance)
        {
            return v1.Magnitude.AlmostEqualsWithAbsTolerance(1, tolerance);
        }

        /// <summary>
        /// Checks if a vector is a unit vector.
        /// Checks if the Vector has been normalized.
        /// Checks if a vector has a magnitude of 1.
        /// </summary>
        /// <param name="v1">
        /// The vector to be checked for Normalization
        /// </param>
        /// <returns>Truth if the vector is a unit vector</returns>
        public static bool IsUnitVector(vector v1)
        {
            return v1.Magnitude == 1;
        }

        /// <summary>
        /// Checks if the vector a unit vector.
        /// Checks if the vector has been normalized.
        /// Checks if the vector has a magnitude of 1.
        /// </summary>
        /// <returns>Truth if this vector is a unit vector</returns>
        /// <implementation>
        /// Uses the <see cref="IsUnitVector(vector)"/> method in the check to avoid code duplication
        /// </implementation>
        public bool IsUnitVector()
        {
            return IsUnitVector(this);
        }

        /// <summary>
        /// Checks if the vector is a unit vector within a tolerance.
        /// Checks if the vector has been normalized within a tolerance.
        /// Checks if the vector has a magnitude of 1 within a tolerance.
        /// </summary>
        /// <param name="tolerance">The tolerance to use when comparing the magnitude</param>
        /// <returns>Truth if this vector is a unit vector within a tolerance</returns>
        /// <implementation>
        /// Uses the <see cref="IsUnitVector(vector)"/> method to avoid code duplication
        /// </implementation>
        public bool IsUnitVector(double tolerance)
        {
            return IsUnitVector(this, tolerance);
        }

        /// <summary>
        /// Checks if a face normal vector represents back face.
        /// Checks if a face is visible, given the line of sight.
        /// </summary>
        /// <param name="normal">
        /// The vector representing the face normal vector
        /// </param>
        /// <param name="lineOfSight">
        /// The unit vector representing the direction of sight from a virtual camera
        /// </param>
        /// <returns>Truth if the vector (as a normal) represents a back face</returns>
        /// <implementation>
        /// Uses the DotProduct function in the check to avoid code duplication
        /// </implementation>
        public static bool IsBackFace(vector normal, vector lineOfSight)
        {
            return normal.DotProduct(lineOfSight) < 0;
        }

        /// <summary>
        /// Checks if a face normal vector represents back face.
        /// Checks if a face is visible, given the line of sight.
        /// </summary>
        /// <param name="lineOfSight">
        /// The unit vector representing the direction of sight from a virtual camera
        /// </param>
        /// <returns>Truth if the vector (as a normal) represents a back face</returns>
        /// <implementation>
        /// <see cref="vector.IsBackFace(vector, vector)"/> 
        /// Uses the isBackFace(vector, vector) function in the check to avoid code duplication
        /// </implementation>
        public bool IsBackFace(vector lineOfSight)
        {
            return IsBackFace(this, lineOfSight);
        }

        /// <summary>
        /// Checks if two vectors are perpendicular within a tolerance.
        /// Checks if two vectors are orthogonal within a tolerance.
        /// Checks if one vector is the normal of the other within a tolerance.
        /// </summary>
        /// <param name="v1">
        /// The vector to be checked for orthogonality
        /// </param>
        /// <param name="v2">
        /// The vector to be checked for orthogonality to
        /// </param>
        /// <param name="tolerance">The absolute difference tolerance to use when comparing the dot product to 0</param>
        /// <returns>Truth if the two vectors are perpendicular within a tolerance</returns>
        /// <implementation>
        /// Uses the <see cref="DotProduct(vector)"/> function in the check to avoid code duplication.
        /// Uses the <see cref="NormalizeOrDefault(vector)"/> function in the check to avoid code duplication and to include spcial cased of inifite values.
        /// Of the three flavours of AlmostEquals that have been implemented the absolute value tolerance is the only one 
        /// that makes senses when comparing to 0. If we later implement relative and/or ulp tolerance, this method would not benefit from the overloads.
        /// </implementation>
        /// <remarks>
        /// The tolerance is not used for special case vectors that have only infinite and zero components. 
        /// We only consider special cases where the zero components are exactly zero.
        /// </remarks>
        public static bool IsPerpendicular(vector v1, vector v2, double tolerance)
        {
            // Use normalization of special cases to handle special cases of IsPerpendicular
            v1 = NormalizeSpecialCasesOrOrigional(v1);
            v2 = NormalizeSpecialCasesOrOrigional(v2);

            // If either vector is vector(0,0,0) the vectors are not perpendicular
            if (v1 == Zero || v2 == Zero)
            {
                return false;
            }

            // Is perpendicular
            return v1.DotProduct(v2).AlmostEqualsWithAbsTolerance(0, tolerance);
        }

        /// <summary>
        /// Checks if two vectors are perpendicular.
        /// Checks if two vectors are orthogonal.
        /// Checks if one vector is the normal of the other.
        /// </summary>
        /// <param name="v1">
        /// The vector to be checked for orthogonality
        /// </param>
        /// <param name="v2">
        /// The vector to be checked for orthogonality to
        /// </param>
        /// <returns>Truth if the two Vectors are perpendicular</returns>
        /// <implementation>
        /// Uses the <see cref="DotProduct(vector)"/> function in the check to avoid code duplication.
        /// Uses the <see cref="NormalizeOrDefault(vector)"/> function in the check to avoid code duplication and to include spcial cased of inifite values.
        /// </implementation>
        public static bool IsPerpendicular(vector v1, vector v2)
        {
            // Use normalization of special cases to handle special cases of IsPerpendicular
            v1 = NormalizeSpecialCasesOrOrigional(v1);
            v2 = NormalizeSpecialCasesOrOrigional(v2);

            // If either vector is vector(0,0,0) the vectors are not perpendicular
            if (v1 == Zero || v2 == Zero)
            {
                return false;
            }

            // Is perpendicular
            return v1.DotProduct(v2).Equals(0);
        }

        /// <summary>
        /// Checks if this vector is perpendicular to another.
        /// Checks if this vector is orthogonal to another.
        /// Checks if this vector is the Normal of the other.
        /// </summary>
        /// <param name="other">
        /// The vector to be checked for orthogonality
        /// </param>
        /// <returns>Truth if the two vectors are perpendicular</returns>
        /// <implementation>
        /// Uses the <see cref="IsPerpendicualr(vector, vector)"/> function in the check to avoid code duplication.
        /// </implementation>
        public bool IsPerpendicular(vector other)
        {
            return IsPerpendicular(this, other);
        }

        /// <summary>
        /// Checks if this vector is perpendicular to another within a tolerance.
        /// Checks if this vector is orthogonal to another within a tolerance.
        /// Checks if this vector is the Normal of the other within a tolerance.
        /// </summary>
        /// <param name="other"> The vector to be checked for orthogonality </param>
        /// <param name="tolerance">The absolute difference tolerance to use when comparing the dot product to 0</param>
        /// <returns>Truth if the two vectors are perpendicular</returns>
        /// <implementation>
        /// Uses the <see cref="IsPerpendicualr(vector, vector, double)"/> function in the check to avoid code duplication.
        /// Of the three flavours of AlmostEquals that have been implemented the absolute value tolerance is the only one 
        /// that makes senses when comparing to 0. If we later implement relative and/or ulp tolerance, this method would not benefit from the overloads.
        /// </implementation>
        public bool IsPerpendicular(vector other, double tolerance)
        {
            return IsPerpendicular(this, other, tolerance);
        }

        /// <summary>
        /// Checks if any component of a vector is Not A Number (NaN)
        /// </summary>
        /// <param name="v1">The vector checked for NaN components</param>
        /// <returns>Truth if any component of the vector is NaN</returns>
        public static bool IsNaN(vector v1)
        {
            return double.IsNaN(v1.X) || double.IsNaN(v1.Y) || double.IsNaN(v1.Z);
        }

        /// <summary>
        /// Checks if any component of this vector is Not A Number (NaN)
        /// </summary>
        /// <returns>Truth if any component of the vector is NaN</returns>
        public bool IsNaN()
        {
            return IsNaN(this);
        }

        #endregion

        #region Cartesian Vectors

        /// <summary>
        /// Vector representing the Cartesian origin.
        /// </summary>
        /// <Acknowledgement>This code is adapted from Exocortex - Ben Houston </Acknowledgement>
        public static readonly vector Origin = new vector(0, 0, 0);

        /// <summary>
        /// Vector representing the Cartesian X axis.
        /// </summary>
        /// <Acknowledgement>This code is adapted from Exocortex - Ben Houston </Acknowledgement>
        public static readonly vector XAxis = new vector(1, 0, 0);

        /// <summary>
        /// Vector representing the Cartesian Y axis.
        /// </summary>
        /// <Acknowledgement>This code is adapted from Exocortex - Ben Houston </Acknowledgement>
        public static readonly vector YAxis = new vector(0, 1, 0);

        /// <summary>
        /// Vector representing the Cartesian Z axis.
        /// </summary>
        /// <Acknowledgement>This code is adapted from Exocortex - Ben Houston </Acknowledgement>
        public static readonly vector ZAxis = new vector(0, 0, 1);

        #endregion

        #region Messages

        /// <summary>
        /// Exception message descriptive text, 
        /// used when an attemp is made to normalize a vector with magnitude NaN.
        /// </summary>
        private const string NORMALIZE_NaN = "Cannot normalize a vector when it's magnitude is NaN";

        /// <summary>
        /// Exception message descriptive text, 
        /// used when an attempt is made to normalize a vector with magnitude 0.
        /// </summary>
        private const string NORMALIZE_0 = "Cannot normalize a vector when it's magnitude is zero";

        /// <summary>
        /// Exception message descriptive text, 
        /// used when an attempt is made to normalize a vector with an infinite magnitude.
        /// </summary>
        private const string NORMALIZE_Inf = "Cannot normalize a vector when it's magnitude is infinite except under special conditions";

        /// <summary>
        /// Exception message descriptive text, 
        /// used for a failure for an array argument to have three components when three are needed .
        /// </summary>
        private const string THREE_COMPONENTS = "Array must contain exactly three components , (x,y,z)";

        /// <summary>
        /// Exception message descriptive text, 
        /// used when interpolation is attempted with a control parameter not between 0 and 1.
        /// </summary>
        private const string INTERPOLATION_RANGE = "Control parameter must be a value between 0 & 1";

        /// <summary>
        /// Exception message descriptive text, 
        /// used when attempting to compare a vector to an object which is not a type of vector. 
        /// </summary>
        private const string NON_VECTOR_COMPARISON = "Cannot compare a vector to a non-vector";

        /// <summary>
        /// Exception message additional information text, 
        /// used when adding type information of the given argument into an error message.
        /// </summary>
        private const string ARGUMENT_TYPE = "The argument provided is a type of ";

        /// <summary>
        /// Exception message additional information text, 
        /// used when adding value information of the given argument into an error message.
        /// </summary>
        private const string ARGUMENT_VALUE = "The argument provided has a value of ";

        /// <summary>
        /// Exception message additional information text, 
        /// used when adding length (number of components in an array) information of the given argument into an error message.
        /// </summary>
        private const string ARGUMENT_LENGTH = "The argument provided has a length of ";

        /// <summary>
        /// Exception message descriptive text 
        /// Used when attempting to set a vectors magnitude to a negative value 
        /// </summary>
        private const string NEGATIVE_MAGNITUDE = "The magnitude of a vector must be a positive value, (i.e. greater than 0)";

        /// <summary>
        /// Exception message descriptive text, 
        /// used when attempting to set a vectors magnitude where the vector represents the origin
        /// </summary>
        private const string ORIGIN_VECTOR_MAGNITUDE = "Cannot change the magnitude of vector(0,0,0)";

        ///////////////////////////////////////////////////////////////////////////////

        private const string UNIT_VECTOR = "Unit vector composing of ";

        private const string POSITIONAL_VECTOR = "Positional vector composing of  ";

        private const string MAGNITUDE = " of magnitude ";

        ///////////////////////////////////////////////////////////////////////////////

        #endregion

        #region Constants

        /// <summary>
        /// The smallest vector possible (based on the double precision floating point structure).
        /// </summary>
        public static readonly vector MinValue = new vector(Double.MinValue, Double.MinValue, Double.MinValue);

        /// <summary>
        /// The largest vector possible (based on the double precision floating point structure).
        /// </summary>
        public static readonly vector MaxValue = new vector(Double.MaxValue, Double.MaxValue, Double.MaxValue);

        /// <summary>
        /// The smallest positive (non-zero) vector possible (based on the double precision floating point structure).
        /// </summary>
        public static readonly vector Epsilon = new vector(Double.Epsilon, Double.Epsilon, Double.Epsilon);

        /// <summary>
        /// Vector with components and magnitude of zero
        /// </summary>
        public static readonly vector Zero = Origin;

        /// <summary>
        /// Vector with components of NaN
        /// </summary>
        public static readonly vector NaN = new vector(double.NaN, double.NaN, double.NaN);

        #endregion
    }
    public static class DoubleExtension
    {
        /// <summary>
        /// Comparator within an absolute tolerance
        /// </summary>
        /// <param name="a">The double to compare to</param>
        /// <param name="b">The double to compare with</param>
        /// <param name="maxAbsoluteError">The tolerance for the comparison compared against the difference of the two doubles</param>
        /// <returns>Truth if the doubles are equal within a tolerance</returns>
        /// <remarks>
        /// Use this tolerant equality method if comparing against zero. The tolerance should be a small multiple of <see cref="double.Epsilon"/>.
        /// Also, check that you are not comparing floats and doubles.
        /// </remarks>
        public static bool AlmostEqualsWithAbsTolerance(this double a, double b, double maxAbsoluteError)
        {
            double diff = Math.Abs(a - b);

            if (a.Equals(b))
            {
                // shortcut, handles infinities
                return true;
            }

            return diff <= maxAbsoluteError;
        }

        /// <summary>
        /// Comparator within an absolute or relative tolerance
        /// </summary>
        /// <param name="a">The double to compare to</param>
        /// <param name="b">The double to compare with</param>
        /// <param name="maxAbsoluteError">The absolute tolerance for the comparison</param>
        /// <param name="maxRelativeError">The relative tolerance for the comparison</param>
        /// <returns>Truth if the doubles are equal within a tolerance</returns>
        /// <acknowalgement>http://www.cygnus-software.com/papers/comparingfloats/comparingfloats.htm</acknowalgement>
        /// <remarks>
        /// Quote from: http://www.cygnus-software.com/papers/comparingfloats/comparingfloats.htm
        /// If you are comparing against a non-zero number then relative epsilons or ULPs based comparisons are probably what you want. 
        /// You’ll probably want some small multiple of double.Epsilon for your relative epsilon, or some small number of ULPs. 
        /// An absolute epsilon could be used if you knew exactly what number you were comparing against.</remarks>
        public static bool AlmostEqualsWithAbsOrRelativeTolerance(
            this double a,
            double b,
            double maxAbsoluteError,
            double maxRelativeError)
        {
            // Needed if we are comparing to zero
            if (AlmostEqualsWithAbsTolerance(a, b, maxAbsoluteError))
            {
                return true;
            }

            // Check the relative tolerance
            double absA = Math.Abs(a);
            double absB = Math.Abs(b);

            double relativeError;
            if (absB > absA)
            {
                relativeError = Math.Abs((a - b) / b);
            }
            else
            {
                relativeError = Math.Abs((a - b) / a);
            }

            return relativeError <= maxRelativeError;
        }

        /// <summary>
        /// Comparator within a numer of units in the last place that we are tolerant of (the numerb of "floats" difference that is acceptable)
        /// </summary>
        /// <param name="a">The double to compare to</param>
        /// <param name="b">The double to compare with</param>
        /// <param name="maxUlps">The Units in the Last Place (ulp)s that we are tolerant of</param>
        /// <returns>Truth if the doubles are equal within a tolerance</returns>
        /// <acknowalgement>http://www.cygnus-software.com/papers/comparingfloats/comparingfloats.htm</acknowalgement>
        /// <remarks>
        /// Quote from: http://www.cygnus-software.com/papers/comparingfloats/comparingfloats.htm
        /// If you are comparing against a non-zero number then relative epsilons or ULPs based comparisons are probably what you want. 
        /// You’ll probably want some small multiple of double.Epsilon for your relative epsilon, or some small number of ULPs. 
        /// An absolute epsilon could be used if you knew exactly what number you were comparing against.</remarks>
        public static bool AlmostEqualsWithAbsOrUlpsTolerance(
            this double a,
            double b,
            double maxAbsoluteError,
            long maxUlps)
        {
            // Needed if we are comparing to zero
            if (AlmostEqualsWithAbsTolerance(a, b, maxAbsoluteError))
            {
                return true;
            }

            // Check the ULPS tolerance
            long longA = BitConverter.DoubleToInt64Bits(a);
            longA = longA < 0 ? (long)(0x8000000000000000 - (ulong)longA) : longA;

            long longB = BitConverter.DoubleToInt64Bits(b);
            longB = longB < 0 ? (long)(0x8000000000000000 - (ulong)longB) : longB;

            long diff = Math.Abs(longA - longB);
            return diff <= maxUlps;
        }
    }
}