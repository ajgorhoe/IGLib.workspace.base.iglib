// Copyright (c) Igor Grešovnik, IGLib license; http://www2.arnes.si/~ljc3m2/igor/ioptlib/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using IG.Lib;

namespace IG.Num
{


    /// <summary>Interface for simulators and other systems (used e.g. for optimization) that 
    /// can calculate vector response as a function of vector input parameters.</summary>
    /// $A Igor Oct08 Aug11;
    public interface IResponseEvaluatorVectorSimple
    {
        
        /// <summary>Calculates simulator's response at the specified input parameters.</summary>
        /// <param name="inputParameters">Input parameters for which response is calculated.</param>
        /// <param name="outputValues">Vector object where the calculated response is stored after calculation.</param>
        void CalculateVectorResponse(IVector inputParameters, ref IVector outputValues);

    }  // interface IResponseEvaluatorVector

}
