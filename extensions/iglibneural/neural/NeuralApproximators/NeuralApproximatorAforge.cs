// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using IG.Lib;
using IG.Num;

using AForge;
using AForge.Neuro;
using AForge.Neuro.Learning;


namespace IG.Neural
{



    /// <summary>Approximator of response by using neural networks, based on the AforgeDotNet library.
    /// $A Igor Mar11;</summary>
    public class NeuralApproximatorAforge : NeuralApproximatorBase, 
        INeuralApproximator, ILockable
    {

        /// <summary>Consructor.</summary>
        public NeuralApproximatorAforge()
            : base()
        { }


        /// <summary>Static constructor. Ensures registration of neural approximator type by its names, which makes possible 
        /// to construct an approximator based on its registered type name because name can resolve to type object used by 
        /// <see cref="Activator"/> to construct the object. This is used e.g. in <see cref="NeuralApproximatorDtoBase.CreateObject()"/>,
        /// which makes possible the existence of <see cref="NeuralApproximatorBase.LoadJson(string, ref INeuralApproximator)"/></summary>
        static NeuralApproximatorAforge()
        {
            NeuralApproximatorBase.TypesRegistry.RegisterDerivedType<NeuralApproximatorAforge>();
        }

        /// <summary>Does nothing by itself, but causes call to static constructor, which causes the necessary type initializations, i.e., 
        /// registers the current <see cref="INeuralApproximator"/> type in <see cref="NeuralApproximatorBase.TypesRegistry"/>.</summary>
        public new static void InitType()
        { }


        #region Data


        /// <summary>Network(s) used for approximation.</summary>
        protected ActivationNetwork[] _networks;

        /// <summary>Teachers used for network training.</summary>
        protected BackPropagationLearning[] _teachers;

        /// <summary>Prepares the networks array (allocates it if necessary) for storing all neural 
        /// networks of the current object.</summary>
        protected override void PrepareNetworksArray()
        {
            lock (Lock)
            {
                int numNetworks;
                if (MultipleNetworks)
                    numNetworks = OutputLength;
                else
                    numNetworks = 1;
                if (_networks == null)
                    _networks = new ActivationNetwork[numNetworks];
                else if (_networks.Length != numNetworks)
                    _networks = new ActivationNetwork[numNetworks];
                if (_teachers == null)
                    _teachers = new BackPropagationLearning[numNetworks];
                else if (_teachers.Length!=numNetworks)
                    _teachers = new BackPropagationLearning[numNetworks];
            }
        }


        #endregion Data


        #region Operation

        #region OperationAuxiliary

        protected virtual IActivationFunction CreateActivationFunction()
        {
            // Change the targeted range of input and output neurons: 
            this.InputNeuronsRange.Reset();
            this.InputNeuronsRange.UpdateAll(-2.0, 2.0);
            this.OutputNeuronsRange.Reset();
            this.OutputNeuronsRange.UpdateAll(-1.0, 1.0);
            return new BipolarSigmoidFunction(SigmoidAlphaValue);
        }

        #endregion OperationAuxiliary

        /// <summary>Prepares neural network for use.
        /// If networks have not yet been created accordinfg to internal data, they are created.
        /// If networks are already prepared then this method does nothing.</summary>
        /// <remarks>Some things suc as creation of a neural network follow the pattern of lazy evaluation.</remarks>
        public override void PrepareNetwork()
        {
            lock (Lock)
            {
                if (!NetworkPrepared)
                {
                    InvalidateNetworkDependencies();
                    CreateNetwork();
                    NetworkPrepared = true;
                }
            }
        }

        /// <summary>Creates the neural network anew. It the network already exists on the current object, 
        /// it is discarded.</summary>
        public override void CreateNetwork()
        {
            lock (Lock)
            {
                InvalidateNetworkDependencies();
                PrepareNetworksArray();
                int[] neuronsCount = new int[NumHiddenLayers + 1]; // one for output layer
                for (int i = 0; i < NumHiddenLayers; ++i)
                    neuronsCount[i] = GetNumNeuronsInHiddenLayer(i);
                int numNetworks = 0;
                if (_networks != null)
                    numNetworks = _networks.Length;
                if (MultipleNetworks)
                {
                    // Multiple networks, each with one output neuron:
                    neuronsCount[NumHiddenLayers] = 1;
                    // numNetworks = NumOutputs;
                } else
                {
                    // Single network, several output neurons:
                    neuronsCount[NumHiddenLayers] = OutputLength;
                    // numNetworks = 1;
                }
                for (int i = 0; i < numNetworks; ++i)
                {
                    // Create the neural network for output i:
                    _networks[i] = new ActivationNetwork(new BipolarSigmoidFunction(SigmoidAlphaValue),
                        InputLength, neuronsCount);
                    // Create a teacher:
                    BackPropagationLearning teacher = new BackPropagationLearning(_networks[i]);
                    // Set learning rate and momentum
                    teacher.LearningRate = LearningRate;
                    teacher.Momentum = Momentum;
                    _teachers[i] = teacher;
                    _networks[i].Randomize();
                }
                EpochCount = 0;
                NetworkPrepared = true;
            }
        }


        /// <summary>Resets the neural network(s), clears information generated during training.</summary>
        public override void ResetNetwork()
        {
            lock (Lock)
            {
                if (_networks != null)
                {
                    for (int i = 0; i < _networks.Length; ++i)
                    {
                        ActivationNetwork network = _networks[i];
                        if (network != null)
                            network.Randomize();
                    }
                }
            }
        }

        /// <summary>Destroys the neural network.</summary>
        public override void DestroyNetwork()
        {
            lock (Lock)
            {
                _networks = null;
                NetworkPrepared = false;
            }
        }


        /// <summary>Saves the state of the neural network to the specified file.
        /// If the file already exists, its contents are overwritten.</summary>
        /// <param name="fileOrDirectoryPath">Path to the file into which the network is saved
        /// or of a directory into which network is saved (in this case default names are generated).</param>
        protected override void SaveNetworkSpecific(string fileOrDirectoryPath)
        {
            lock (Lock)
            {
                if (_networks != null)
                {
                    for (int i = 0; i < _networks.Length; ++i)
                    {
                        _networks[i].Save(GetNetworkFilePath(fileOrDirectoryPath, i));
                    }
                    NetworkPrepared = true;
                }
            }
        }

        /// <summary>Restores neural network from a file where it has been stored before.</summary>
        /// <param name="fileOrDirectoryPath">Path to the file from which the neural network is read.</param>
        protected override void LoadNetworkSpecific(string fileOrDirectoryPath)
        {
            lock (Lock)
            {
                PrepareNetworksArray();
                if (_networks != null)
                {
                    for (int i = 0; i < _networks.Length; ++i)
                    {
                        _networks[i] = Network.Load(GetNetworkFilePath(fileOrDirectoryPath, i)) as ActivationNetwork;
                    }
                }
                NetworkPrepared = true;
            }
        }


        private double[][] _trainingInputsAForge;
        private double[][] _trainingOutputsAForge;

        private double[][] _singleNetworkOutputs = null; // used by training in case of multiple networks, each with single output


        /// <summary>Gets or sets training inputs.</summary>
        protected double[][] TrainingInputsAForge
        {
            private set { _trainingInputsAForge = value; }
            get
            {
                if (!InternalTrainingDataPrepared)
                    PrepareInternalTrainingData();
                return _trainingInputsAForge;
            }
        }

        /// <summary>Gets or sets training outputs.</summary>
        protected double[][] TrainingOutputsAForge
        {
            set { _trainingOutputsAForge = value; }
            get
            {
                if (!InternalTrainingDataPrepared)
                    PrepareInternalTrainingData();
                return _trainingOutputsAForge;
            }
        }

        /// <summary>Prepares and returns outputs for the specific network in the case with multiple networks.</summary>
        /// <param name="whichNetwork">Specifies which network the outputs are prepared for.</param>
        protected double[][] GetSingleNetworkTrainingOutput(int whichNetwork)
        {
            if (MultipleNetworks)
            {
                for (int i = 0; i < _singleNetworkOutputs.Length; ++i)
                    _singleNetworkOutputs[i][0] = TrainingOutputsAForge[i][whichNetwork];
                return _singleNetworkOutputs;
            } else 
                return null;
        }

        /// <summary>Prepares internal training data that is needed by the native training algorithm.</summary>
        protected override void PrepareInternalTrainingData()
        {
            lock (Lock)
            {
                if (!InternalTrainingDataPrepared)
                {
                    int numElements = 0;
                    if (TrainingData != null)
                        numElements = TrainingData.Length;                    
                    int numInputOutput = NumTrainingPoints;  // number of training points - excluding verification points!
                    if (numInputOutput < 1)
                        throw new InvalidDataException("Invalid training data: number of training samples (excluding verification samples) is less than 1.");
                    double[][] inputs = new double[numInputOutput][];
                    double[][] outputs = new double[numInputOutput][];
                    if (MultipleNetworks)
                    {
                        // Prepare array to hold output data for each individual network in the case of 
                        // multiple networks (each representing its own output value):
                        _singleNetworkOutputs = new double[numInputOutput][];
                        for (int i = 0; i < numInputOutput; ++i)
                            _singleNetworkOutputs[i] = new double[1];
                    }
                    int whichInputOutput = 0;
                    int whichElement = 0;
                    while (whichElement < numElements)
                    {
                        bool isVerificationElement = false;
                        if (VerificationIndices != null)
                            if (VerificationIndices.Contains(whichElement))
                                isVerificationElement = true;
                        if (!isVerificationElement)
                        {
                            SampledDataElement element = TrainingData[whichElement];
                            if (element == null)
                                throw new InvalidDataException("Training element No. " + whichElement + " is not defined (null reference).");
                            if (element.InputParameters==null)
                                throw new InvalidDataException("Training input vector No. " + whichElement + " is not defined (null reference).");
                            if (element.InputParameters.Length != InputLength)
                                throw new InvalidDataException("Training input vector No. " + whichElement + " has wrong dimension, "
                                    + element.InputParameters.Length + " instead of " + InputLength + ".");
                            if (element.OutputValues==null)
                                throw new InvalidDataException("Training output vector No. " + whichElement + " is not defined (null reference).");
                            if (element.OutputValues.Length != OutputLength)
                                throw new InvalidDataException("Training output vector No. " + whichElement + " has wrong dimension, "
                                    + element.OutputValues.Length + " instead of " + OutputLength + ".");
                            inputs[whichInputOutput] = element.InputParameters.ToArray();
                            outputs[whichInputOutput] = element.OutputValues.ToArray();
                            ++whichInputOutput;
                        }
                        ++whichElement;
                    }
                    if (OutputLevel > 0)
                    {
                        Console.WriteLine();
                        Console.WriteLine("Prepared input data befoe mapping (scaling and shifting):");
                        for (int i = 0; i < numInputOutput; ++i)
                        {
                            Console.WriteLine("Input/Output No. " + i + ":");
                            for (int j=0; j<inputs[i].Length;++j)
                            {
                                Console.Write(inputs[i][j].ToString() + " ");
                            }
                            Console.WriteLine();
                            for (int j=0; j<outputs[i].Length;++j)
                            {
                                Console.Write(outputs[i][j].ToString() + " ");
                            }
                            Console.WriteLine();
                        }
                        Console.WriteLine();
                    }
                    // Perform scaling of inputs & outputs:
                    for (int i = 0; i < numInputOutput; ++i)
                    {
                        for (int j = 0; j < inputs[i].Length; ++j)
                        {
                            inputs[i][j] = MapInput(j, inputs[i][j]);
                        }
                        for (int j = 0; j < outputs[i].Length; ++j)
                        {
                            outputs[i][j] = MapOutput(j, outputs[i][j]);
                        }
                    }
                    if (OutputLevel > 0)
                    {
                        Console.WriteLine();
                        Console.WriteLine("Prepared input data after mapping (scaling and shifting):");
                        for (int i = 0; i < numInputOutput; ++i)
                        {
                            Console.WriteLine("Input/Output No. " + i + ":");
                            for (int j = 0; j < inputs[i].Length; ++j)
                            {
                                Console.Write(inputs[i][j].ToString() + " ");
                            }
                            Console.WriteLine();
                            for (int j = 0; j < outputs[i].Length; ++j)
                            {
                                Console.Write(outputs[i][j].ToString() + " ");
                            }
                            Console.WriteLine();
                        }
                        Console.WriteLine();
                    }
                    TrainingInputsAForge = inputs;
                    TrainingOutputsAForge = outputs;
                    InternalTrainingDataPrepared = true;
                }
            }
        }


 
        /// <summary>Trains neural network wiht the specified data, performing the specified number of epochs.
        /// The maximal number of epochs that is set on the current object does not have any effect in this method,
        /// and the method can perform more epochs tha specified by that limit.</summary>
        /// <param name="numEpochs">Number of epochs used in training of the network.</param>
        /// <remarks>This method just enforces a fixed number of epochs and can be used to form more complex training 
        /// procedures. Most common method used for training is that without arguments, which takse into account various
        /// tolerances that may be set on this object and the maximal number of epochs.</remarks>
        protected override void TrainNetworkSpecific(int numEpochs)
        {
            lock (Lock)
            {
                if (!NetworkPrepared)
                {
                    PrepareNetwork();
                    if (!NetworkPrepared)
                        throw new InvalidOperationException("Could not prepare neural network(s) for operation.");
                }
                if (!InternalTrainingDataPrepared)
                {
                    PrepareInternalTrainingData();
                    if (!InternalTrainingDataPrepared)
                        throw new InvalidOperationException("Could not prepare internal training data.");
                }
                InvalidateTrainingDependencies();
                double sumErrors = 0.0;
                double error=0.0;
                double[][] singleNetworkOutputs = null;
                for (int whichNetwork = 0; whichNetwork < _networks.Length; ++whichNetwork)
                {
                    if (BreakTraining)
                        break;
                    // for each network...
                    if (MultipleNetworks)
                        singleNetworkOutputs = GetSingleNetworkTrainingOutput(whichNetwork);
                    for (int i = 0; i < numEpochs; ++i)
                    {
                        if (BreakTraining)
                            break;
                        // Run one epoch of learning procedure:
                        if (MultipleNetworks)
                        {
                            error = _teachers[whichNetwork].RunEpoch(TrainingInputsAForge, singleNetworkOutputs);
                        } else
                        {
                            error = _teachers[whichNetwork].RunEpoch(TrainingInputsAForge, TrainingOutputsAForge);
                        }
                        if (OutputLevel > 2)
                            Console.WriteLine("Training network " + whichNetwork + ", epoch " + (EpochCount + i) 
                                + ", internal error: " + error );
                    }
                    sumErrors += error;
                }
                EpochCount += numEpochs;
                sumErrors /= OutputLength;
                NetworkTrained = true;
                if (OutputLevel>0)
                {
                    Console.WriteLine();
                    Console.WriteLine("Network trained with " + numEpochs + " epochs.");
                    Console.WriteLine("  Internal error after epoch No. " + EpochCount + ": " + sumErrors);
                    if (OutputLevel > 1)
                    {
                        if (_auxErrors==null)
                            _auxErrors = new Vector(OutputLength);
                        // Output various error measures:
                        Console.WriteLine("  Error measures calculated for training points:");
                        IVector errorMeasures = _auxErrors;
                        GetErrorsTrainingRms(ref errorMeasures);
                        Console.WriteLine("  RMS:");
                        Console.WriteLine("  " + errorMeasures.ToString());
                        GetErrorsTrainingMax(ref errorMeasures);
                        Console.WriteLine("  Max.:");
                        Console.WriteLine("  " + errorMeasures.ToString());
                        if (NumVerificationPoints > 0 && CalculateVerificationErrors)
                        {
                            Console.WriteLine("  Error measures calculated for verification points:");
                            GetErrorsVerificationRms(ref errorMeasures);
                            Console.WriteLine("  RMS:");
                            Console.WriteLine("  " + errorMeasures.ToString());
                            GetErrorsVerificationMax(ref errorMeasures);
                            Console.WriteLine("  Max.:");
                            Console.WriteLine("  " + errorMeasures.ToString());
                        }
                    }
                    Console.WriteLine();

                }
            }
        }


        /// <summary>Calculates and returns the approximated output values corresponding to the specified inputs,
        /// by using the current neural network(s).</summary>
        /// <param name="input">Input parameters.</param>
        /// <param name="output">Vector where approximated values are stored.</param>
        /// <returns>Vector of output values generated by the trained neural network.</returns>
        public override void CalculateOutput(IVector input, ref IVector output)
        {
            lock(Lock)
            {
                if (!NetworkTrained)
                    throw new InvalidOperationException("Cannot calculate output, neural network(s) not prepared.");
                if (output==null)
                    output = new Vector(OutputLength);
                else if (output.Length!=OutputLength)
                    output = output.GetNew(OutputLength);
                if (MultipleNetworks)
                {
                    if (_networks==null)
                        throw new InvalidOperationException("Cannot calculate output, neural network(s) not prepared. Warning: inconsistency with 'prepared' flag!");
                    else if (_networks.Length!=OutputLength)
                        throw new InvalidOperationException("Cannot calculate output, neural network(s) not prepared. Warning: inconsistency with 'prepared' flag!");
                    double[] inpParams = input.ToArray();
                    // Map input to matxh neurons range:
                    for (int i=0; i<inpParams.Length; ++i)
                        inpParams[i] = MapInput(i, inpParams[i]);
                    double[] outValue;
                    for (int i=0; i<OutputLength; ++i)
                    {
                        outValue = _networks[i].Compute(inpParams);
                        // Map the calculated value from output neuron range to output data range:
                        output[i] = MapFromNeuralOutput(i, outValue[0]);
                    }
                } else
                {
                    // Single nwtwork with several outputs:
                    if (_networks == null)
                        throw new InvalidOperationException("Cannot calculate output, neural network(s) not prepared properly. Warning: inconsistency with 'prepared' flag!");
                    else if (_networks.Length != 1)
                        throw new InvalidOperationException("Cannot calculate output, neural network(s) not prepared properly. Warning: inconsistency with 'prepared' flag!");
                    double[] inpParams = input.ToArray();
                    double[] outValues = _networks[0].Compute(inpParams);
                    if (outValues == null)
                        throw new InvalidDataException("Array of calculated output values is invalid (null reference).");
                    else if (outValues.Length != OutputLength)
                        throw new InvalidDataException("Array of output values is not of correct length (" 
                            + outValues.Length + " instead of " + OutputLength + ").");
                    // Map the calculated values from output neuron range to output data range:
                    for (int i = 0; i < OutputLength; ++i)
                        output[i] = MapFromNeuralOutput(i, outValues[i]);
                }
            }  // lock
        }  // CalculateOutput(IVector input, ref IVector output)



        #endregion Operation


    }  // class NeuralApproximatorAforge



}
