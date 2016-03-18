using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;


using IG.Num;
using IG.Neural;


namespace IG.Neural
{
    
    
    /// <summary>Classes that contain neural approximator.</summary>
    /// $A Igor Feb13;
    public interface INeuralApproximatorContainer 
    {

        /// <summary>Gets or sets th the neural approximator for the containing class.</summary>
        INeuralApproximator NeuralApproximator { get; set; }

    }



}
