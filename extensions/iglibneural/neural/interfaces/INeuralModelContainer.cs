using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using IG.Lib;
using IG.Num;

namespace IG.Neural
{

    /// <summary>Interface for classes that contain the <see cref="NeuralModel"/> property that implements the <see cref="INeuralModel"/> interface.</summary>
    public interface INeuralModelContainer
    {

        /// <summary>Artificial neural network - based model.</summary>
        INeuralModel NeuralModel
        {
            get;
        }

        
        /// <summary>Sets the ANN-based model contained in the current object.</summary>
        /// <param name="model">ANN based model that is set.</param>
        /// <remarks>Because of this dedicated method, the setter of the <see cref="NeuralModel"/> property
        /// can be non-public (although some implementations may implement a public setter).</remarks>
        void SetNeuralModel(INeuralModel model);

    }
}
