
// NEURAL NETWORK APPROXIMATORS

using System.IO;
using System.IO.Compression;
using System.Xml;
using System.Text;
using System;
using System.Collections.Generic;

using IG.Lib;
using IG.Num;

namespace IG.Neural
{



    /// <summary>A data transfer object (DTO) for the NeuralApproximatorAforge class.
    /// $A Igor Mar11;</summary>
    public class NeuralApproximatorAForgeDto : NeuralApproximatorDtoBase<NeuralApproximatorAforge>
    {

        public NeuralApproximatorAForgeDto()
            : base()
        { }

        #region Data

        

        #endregion Data


        #region Operation 

        public override NeuralApproximatorAforge CreateObject()
        {
            return new NeuralApproximatorAforge();
        }

        protected override void CopyFromPlain(INeuralApproximator obj)
        {
            base.CopyFromPlain(obj);
            if (obj != null)
            {
                // Put specifics here!
            }
        }

        protected override void CopyToPlain(ref INeuralApproximator obj)
        {
            base.CopyToPlain(ref obj);
            if (obj != null)
            {
                // Put specifics here!
            }
        }

        #endregion Operation 


    }  // class NeuralApproximatorAForgeDto


}
