// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using IG.Lib;

namespace IG.Num
{

    /// <summary>Interface for artificial neural network -based models that we can operate on.</summary>
    /// <remarks>Incorporated to official ANN libraries in April 2013.</remarks>
    /// $A Igor xx Apr13;
    public interface INeuralModel
    {

        /// <summary>Traint artificial neural network.</summary>
        INeuralApproximator TrainedNetwork
        {
            get;
        }

        /// <summary>Neural data definition.</summary>
        InputOutputDataDefiniton NeuralDataDefinition
        {
            get;
        }

    }

    
}
