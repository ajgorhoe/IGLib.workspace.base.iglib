﻿// AForge Fuzzy Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © Andrew Kirillov, 2008-2010
// andrew.kirillov@aforgenet.com
//
// Copyright © Fabio L. Caversan, 2008-2010
// fabio.caversan@gmail.com
//
namespace AForge.Fuzzy
{
    using System;
    using AForge;

    /// <summary>
    /// Maximum CoNorm, used to calculate the linguistic value of a OR operation. 
    /// </summary>
    /// 
    /// <remarks><para>The maximum CoNorm uses a maximum operator to compute the OR
    /// among two fuzzy memberships.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // creating 2 fuzzy sets to represent Cool (Temperature) and Near (Distance)
    /// TrapezoidalFunction function1 = new TrapezoidalFunction( 13, 18, 23, 28 );
    /// FuzzySet fsCool = new FuzzySet( "Cool", function1 );
    /// TrapezoidalFunction function2 = new TrapezoidalFunction( 23, 28, 33, 38 );
    /// FuzzySet fsNear = new FuzzySet( "Near", function2 );
    /// 
    /// // getting memberships
    /// double m1 = fsCool.GetMembership( 15 );
    /// double m2 = fsNear.GetMembership( 35 );
    /// 
    /// // computing the membership of "Cool OR Near"
    /// MaximumCoNorm OR = new MaximumCoNorm( );
    /// double result = OR.Evaluate( m1, m2 );
    ///              
    /// // show result
    /// Console.WriteLine( result );
    /// </code>
    /// </remarks>
    /// 
    /// <seealso cref="ICoNorm"/>
    /// 
    public class MaximumCoNorm : ICoNorm
    {
        /// <summary>
        /// Calculates the numerical result of the OR operation applied to
        /// two fuzzy membership values.
        /// </summary>
        /// 
        /// <param name="membershipA">A fuzzy membership value, [0..1].</param>
        /// <param name="membershipB">A fuzzy membership value, [0..1].</param>
        /// 
        /// <returns>The numerical result of the binary operation OR applied to <paramref name="membershipA"/>
        /// and <paramref name="membershipB"/>.</returns>
        /// 
        public double Evaluate( double membershipA, double membershipB )
        {
            return Math.Max( membershipA, membershipB );
        }
    }
}

