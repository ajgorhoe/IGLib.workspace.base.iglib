// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using IG.Lib;

namespace IG.Num
{
    public class NeuralApproximatorAforgeFake : NeuralApproximatorBase
    {

        public override double CalculateOutput(IVector input, int whichElement)
        {
            return base.CalculateOutput(input, whichElement);
        }

        protected override void TrainNetworkSpecific(int numEpochs)
        {
            throw new NotImplementedException();
        }

        protected override void PrepareInternalTrainingData()
        {
            throw new NotImplementedException();
        }

        protected override void LoadNetworkSpecific(string filePath, bool useSerializationBinderIfSpecified = true)
        {
            throw new NotImplementedException();
        }

        protected override void SaveNetworkSpecific(string filePath)
        {
            throw new NotImplementedException();
        }

        public override void DestroyNetwork()
        {
            throw new NotImplementedException();
        }

        public override void ResetNetwork()
        {
            throw new NotImplementedException();
        }

        public override void CreateNetwork()
        {
            throw new NotImplementedException();
        }

        public override void PrepareNetwork()
        {
            throw new NotImplementedException();
        }

        protected override void PrepareNetworksArray()
        {
            throw new NotImplementedException();
        }

        public override void CalculateOutput(IVector input, int[] indices, ref IVector filteredOutput)
        {
            base.CalculateOutput(input, indices, ref filteredOutput);
        }


        public override void CalculateOutput(IVector input, ref IVector output)
        {
            throw new NotImplementedException();
        }

    }


}
