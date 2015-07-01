// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using System.Collections;

using IG.Lib;


namespace IG.Num
{
    
    /// <summary>Contains Parameters that define neural network architecture and trainig procedure, together with 
    /// achieved results after training such as various error norms.
    /// <para>Not thread safe!</para></summary>
    /// <remarks>This class is used for storing parameters of neural networks and restoring them at a later time, in
    /// order to repeat training under similar condition or simply to analyse performance of  neural networks.</remarks>
    /// $A Igor Jul10 May12;
    public partial class NeuralTrainingParameters
    {
        
        #region DefaultValues

        /// <summary>Default value for learning rate in neural networks.</summary>
        public static double DefaultLearningRate = 0.1;

        /// <summary>Default value of momentum for neural networks.</summary>
        public static double DefaultMomentum = 0.5;
        
        /// <summary>Default value of the sigmoid alpha value (used in networks with sigmoid activation functions).</summary>
        public static double DefaultSigmoidAlphaValue = 1.5;

        /// <summary>Default value for number of epochs in bundle (i.e. number of epochs performed at once, without any
        /// checks or output operations between).</summary>
        public static int DefaultEpochsInBundle;

        /// <summary>Default number of hidden layers.</summary>
        public static int DefaultNumHiddenLayers = 1;

        /// <summary>Default number of hidden neurons in a layer</summary>
        public static int DefaultNumHiddenNeurons = 20;

        // Stopping criteria:

        /// <summary>Default value for maximal number of epochs.</summary>
        public static int DefaultMaxEpochs = 40000;


        /// <summary> Default value for tollerance on RMS error in neural networks. </summary>
        public static IVector DefaultToleranceRms = null; // new Vector(0.01, 0.01, 0.01);

        /// <summary> Default value for tollerance on max. abs. error in neural networks. </summary>
        public static IVector DefaultToleranceMax = null; // new Vector(0.01, 0.01, 0.01);


        /// <summary>Default value for the tolerance on RMS error, relative to the output range.</summary>
        public static double DefaultToleranceRmsRelativeToRangeScalar = 0.1;

        /// <summary>Default value for the tolerance on max. abs. error, relative to the output range.</summary>
        public static double DefaultToleranceMaxRelativeToRangeScalar = 0;

        /// <summary>Default number of input neurons.</summary>
        public static int DefaultInputLength = 1;

        /// <summary>Default number of output neurons.</summary>
        public static int DefaultOutputLength = 1;

        /// <summary>Default number of input safety factor.</summary>
        public static double DefaultInputBoundSafetyFactor = 1.5;

        /// <summary>Default number of output safety factor.</summary>
        public static double DefaultOutputBoundSafetyFactor = 1.5;

        #endregion DefaultValues


        #region TrainingParameters

        protected double _learningRate = DefaultLearningRate;

        /// <summary>Learning rate.</summary>
        public double LearningRate
        {
            get { return _learningRate; }
            set { _learningRate = value; }
        }

        protected double _momentum = DefaultMomentum;

        /// <summary>Momentum. Specifies how much changes of weight in the previous iterations affect 
        /// changes in the current iterations.</summary>
        public double Momentum
        {
            get { return _momentum; }
            set { _momentum = value; }
        }


        protected double _sigmoidAlphaValue = DefaultSigmoidAlphaValue;

        /// <summary>Sigmoid alpha value (used in networks with sigmoid activation functions).</summary>
        public double SigmoidAlphaValue
        {
            get { return _sigmoidAlphaValue; }
            set { _sigmoidAlphaValue = value; }
        }

        int _inputLength = DefaultInputLength, _outputLength = DefaultOutputLength;

        /// <summary>Gets or sets the number of input neurons.</summary>
        public virtual int InputLength
        {
            get { return _inputLength; }
            set { _inputLength = value; }
        }

        /// <summary>Gets or sets the number of output neurons.</summary>
        public virtual int OutputLength
        {
            get { return _outputLength; }
            set { _outputLength = value; }
        }

        /// <summary>Gets or sets input safety factor.</summary>
        double _inputBoundSafetyFactor = DefaultInputBoundSafetyFactor;

        /// <summary>Gets or sets input safety factor.</summary>
        public virtual double InputBoundSafetyFactor
        {
            get { return _inputBoundSafetyFactor; }
            set { _inputBoundSafetyFactor = value; }
        }

        /// <summary>Gets or sets output safety factor.</summary>
        double _outputBoundSafetyFactor = DefaultOutputBoundSafetyFactor;

        /// <summary>Gets or sets output safety factor.</summary>
        public virtual double OutputBoundSafetyFactor
        {
            get { return _outputBoundSafetyFactor; }
            set { _outputBoundSafetyFactor = value; }
        }

        // Stopping criteria:
        
        protected int _maxEpochs = DefaultMaxEpochs;

        /// <summary>Maximal number of epochs performed in the training procedure.</summary>
        public int MaxEpochs
        {
            get { return _maxEpochs; }
            set { _maxEpochs = value; }
        }

        
        protected int _epochInBundle = DefaultEpochsInBundle;

        /// <summary>Number of epochs in bundle (i.e. number of epochs performed at once, without any
        /// checks or output operations between).
        /// <para>This parameter does not affect the training procedure in terms of results.</para></summary>
        public int EpochsInBundle
        {
            get { return _epochInBundle; }
            set { _epochInBundle = value; }
        }

        protected IVector _outputRange;

        /// <summary> Range from actual outputs. </summary>
        /// $A Tako78 Octl12;
        public IVector OutputRange
        {
            get
            {
                if (OutputNeuronRange != null)
                {
                    int outputLength = OutputLength;
                    _outputRange = new Vector(outputLength);
                    for (int i = 0; i < outputLength; i++)
                    {
                        double minValue, maxValue;

                        minValue = OutputNeuronRange.GetMin(i);
                        maxValue = OutputNeuronRange.GetMax(i);
                        _outputRange[i] = maxValue - minValue;
                    }
                }
                return _outputRange;
            }
            set
            {
                _outputRange = value;
            }
        }

        protected IBoundingBox _outputNeuronRange;

        /// <summary> Bounding box from actual outputs. </summary>
        /// $A Tako78 Octl12;
        protected IBoundingBox OutputNeuronRange
        {
            get 
            {
                return _outputNeuronRange; 
            }
            set
            {
                _outputNeuronRange = value;
            }
        }

        protected IVector _inputRange;

        /// <summary> Range from actual inputs. </summary>
        /// $A Tako78 Octl12;
        public IVector InputRange
        {
            get
            {
                if (InputNeuronRange != null)
                {
                    int inputLength = InputLength;
                    _inputRange = new Vector(inputLength);
                    for (int i = 0; i < inputLength; i++)
                    {
                        double minValue, maxValue;

                        minValue = InputNeuronRange.GetMin(i);
                        maxValue = InputNeuronRange.GetMax(i);
                        _inputRange[i] = maxValue - minValue;
                    }
                }
                return _inputRange;
            }
            set
            {
                _inputRange = value;
            }
        }

        protected IBoundingBox _inputNeuronRange;

        /// <summary> Bounding box from actual inputs. </summary>
        /// $A Tako78 Octl12;
        protected IBoundingBox InputNeuronRange
        {
            get
            {
                return _inputNeuronRange;
            }
            set
            {
                _inputNeuronRange = value;
            }
        }

        protected IVector _toleranceRms;

        /// <summary>Tolerance over RMS error of output over training points.
        /// Training will continue until error becomes below tolerance or until maximal number of epochs is reached.
        /// If less or equal than 0 then this tolerance is not taken into account.</summary>
        public IVector ToleranceRms
        {
            get { return _toleranceRms; }
            set
            {
                if (value != null)
                {
                    if (value.Length != OutputLength)
                        throw new ArgumentException("Dimension of the specified vector of tolerances on RMS values " + value.Length
                            + " is different than the number of outputs " + OutputLength + ".");
                }
                _toleranceRms = value;
            }
        }

        

        protected IVector _toleranceMax;

        /// <summary>Tolerance on maximal error of output over training points.
        /// Training will continue until error becomes below tolerance or until maximal number of epochs is reached.
        /// If less or equal than 0 then this tolerance is not taken into account.</summary>
        public IVector ToleranceMax
        {
            get { return _toleranceMax; }
            set
            {
                if (value != null)
                {
                    if (value.Length != OutputLength)
                        throw new ArgumentException("Dimension of the specified vector of tolerances on maximal values " + value.Length
                            + " is different than the number of outputs " + OutputLength + ".");
                }
                _toleranceMax = value;
            }
        }


        /// Auxiliary properties for defining tolerances in a relative way:

        private IVector _tolRmsRelative;

        /// <summary>Relative tolerances on RMS errors of outputs over training points, relative to the 
        /// correspoinding ranges of output data.</summary>
        /// <seealso cref="NeuralApproximatorBase.ToleranceRmsRelativeToRange"/>
        public IVector ToleranceRmsRelativeToRange
        {
            get
            {
                return _tolRmsRelative;
            }
            set
            {
                if (value != null)
                {
                    if (value.Length != OutputLength)
                        throw new ArgumentException("Dimension of the specified vector of relative RMS tolerances " + value.Length
                            + " is different than the number of outputs " + OutputLength + ".");
                }
                _tolRmsRelative = value;
            }
        }


        private double _tolRmsRelativeScalar = DefaultToleranceRmsRelativeToRangeScalar;

        /// <summary>Scalar through which all components of the Relative tolerances on RMS errors of outputs
        /// can be set to the same value.</summary>
        /// <seealso cref="NeuralApproximatorBase.ToleranceRmsRelativeToRangeScalar"/>
        public double ToleranceRmsRelativeToRangeScalar
        {
            get { return _tolRmsRelativeScalar; }
            set { _tolRmsRelativeScalar = value; }
        }

        private IVector _tolMaxRelative;

        /// <summary>Relative tolerances on max. abs. errors of outputs over training points, relative to the 
        /// correspoinding ranges of output data.</summary>
        /// <seealso cref="NeuralApproximatorBase.ToleranceMaxRelativeToRange"/>
        public IVector ToleranceMaxRelativeToRange
        {
            get
            {
                return _tolMaxRelative;
            }
            set
            {
                if (value != null)
                {
                    if (value.Length != OutputLength)
                        throw new ArgumentException("Dimension of the specified vector of relative max. abs. tolerances " + value.Length
                            + " is different than the number of outputs " + OutputLength + ".");
                }
                _tolMaxRelative = value;
            }
        }

        private double _tolMaxRelativeScalar = DefaultToleranceMaxRelativeToRangeScalar;

        /// <summary>Scalar through which all components of the Relative tolerances on max. abs. errors of outputs
        /// can be set to the same value.</summary>
        /// <seealso cref="NeuralApproximatorBase.ToleranceMaxRelativeToRangeScalar"/>
        public double ToleranceMaxRelativeToRangeScalar
        {
            get { return _tolMaxRelativeScalar; }
            set { _tolMaxRelativeScalar = value; }
        }


        #endregion TrainingParameters


        #region NetworkArchitecture


        protected int _numHiddenLayers = DefaultNumHiddenLayers;

        /// <summary>Number of Hideden layers.</summary>
        public int NumHiddenLayers
        {
            get { return _numHiddenLayers; }
            set 
            {
                _numHiddenLayers = value;
                if (_numHiddenNeurons != null)
                {
                    if (_numHiddenNeurons.Length != value)
                        _numHiddenNeurons = null;
                }
            }
        }

        protected int[] _numHiddenNeurons = null;
        
        /// <summary>Numbers of neurons in each hidden layer.</summary>
        public int[] NumHidenNeurons
        {
            get 
            {
                if (_numHiddenLayers <=0)
                    _numHiddenNeurons = null;
                else 
                {
                    // _numHiddenLayers > 0:
                    bool reallocate = false;
                    if (_numHiddenNeurons == null)
                        reallocate = true;
                    else if (_numHiddenNeurons.Length != _numHiddenLayers)
                        reallocate = true;
                    if (reallocate)
                    {
                        _numHiddenNeurons = new int[_numHiddenLayers];
                        for (int i = 0; i < _numHiddenLayers; ++i)
                            _numHiddenNeurons[i] = DefaultNumHiddenNeurons;
                    }
                }
                return _numHiddenNeurons;
            }
            set 
            {
                _numHiddenNeurons = value;
                if (value != null)
                {
                    _numHiddenLayers = value.Length;
                }
            }
        }

        #endregion NetworkArchitecture


        #region TrainingResults

        protected bool _isNetworkTrained = false;

        /// <summary>Whether the network is trained (and results exist).</summary>
        public bool IsNetworkTrained
        {
            get { return _isNetworkTrained; }
            set { _isNetworkTrained = value; }
        }

        protected IVector _errorsTrainingRms;

        /// <summary>RMS errors calculated on training data.</summary>
        public IVector ErrorsTrainingRms
        {
            get { return _errorsTrainingRms; }
            set { _errorsTrainingRms = value; }
        }

        public List<IVector> _errorsTrainingRmsList = null;

        /// <summary>Convergence List of Rms errors calculated on training data.</summary>
        public List<IVector> ErrorsTrainingRmsList
        {
            get { return _errorsTrainingRmsList; }
            set { _errorsTrainingRmsList = value; }
        }

        protected IVector _errorsTrainingMax;

        /// <summary>Maximal errors calculated on training data.</summary>
        public IVector ErrorsTrainingMax
        {
            get { return _errorsTrainingMax; }
            set { _errorsTrainingMax = value; }
        }

        public List<IVector> _errorsTrainingMaxList = null;

        /// <summary>Convergence List of Maximal errors calculated on training data.</summary>
        public List<IVector> ErrorsTrainingMaxList
        {
            get { return _errorsTrainingMaxList; }
            set { _errorsTrainingMaxList = value; }
        }

        protected IVector _errorsTrainingMeanAbs;

        /// <summary>Mean absolute errors calculated on training data.</summary>
        public IVector ErrorsTrainingMeanAbs
        {
            get { return _errorsTrainingMeanAbs; }
            set { _errorsTrainingMeanAbs = value; }
        }

        protected IVector _errorsVerificationRms;

        /// <summary>RMS errors calculated on verification data.</summary>
        public IVector ErrorsVerificationRms
        {
            get { return _errorsVerificationRms; }
            set { _errorsVerificationRms = value; }
        }

        public List<IVector> _errorsVerificationRmsList = null;

        /// <summary>Convergence List of RMS errors calculated on verification data.</summary>
        public List<IVector> ErrorsVerificationRmsList
        {
            get { return _errorsVerificationRmsList; }
            set { _errorsVerificationRmsList = value; }
        }

        protected IVector _errorsVerificationMax;

        /// <summary>Maximal errors calculated on verification data.</summary>
        public IVector ErrorsVerificationMax
        {
            get { return _errorsVerificationMax; }
            set { _errorsVerificationMax = value; }
        }

        public List<IVector> _errorsVerificationMaxList = null;

        /// <summary>Convergence List of Maximal errors calculated on verification data.</summary>
        public List<IVector> ErrorsVerificationMaxList
        {
            get { return _errorsVerificationMaxList; }
            set { _errorsVerificationMaxList = value; }
        }

        protected IVector _errorsVerificationMeanAbs;

        /// <summary>Maximal errors calculated on training data.</summary>
        public IVector ErrorsVerificationMeanAbs
        {
            get { return _errorsVerificationMeanAbs; }
            set { _errorsVerificationMeanAbs = value; }
        }

        //public List<IVector> _errorsRmsList = null;

        ///// <summary>Convergence List of RMS errors.</summary>
        //public List<IVector> ConvergenceListRms
        //{
        //    get { return _errorsRmsList; }
        //    set { _errorsRmsList = value; }
        //}

        public bool _convergenceRmsEnabled = false;

        public bool SaveConvergenceRms
        {
            get { return _convergenceRmsEnabled; }
            set { _convergenceRmsEnabled = value; }
        }

        // Convergence data:

        protected int _numEpochs = 0;

        /// <summary>Number of epochs actually spent at training.
        /// <para>This may be less than <see cref="MaxEpochs"/> if convergence is reached before.</para></summary>
        public int NumEpochs
        {
            get { return _numEpochs; }
            set { _numEpochs = value; }
        }

        protected double _trainingTime = 0;

        /// <summary>Time spent for training.</summary>
        public double TrainingTime
        {
            get { return _trainingTime; }
            set { _trainingTime = value; }
        }

        protected double _trainingCpuTime = 0;

        /// <summary>CPU time spent for training.</summary>
        public double TrainingCpuTime
        {
            get { return _trainingCpuTime; }
            set { _trainingCpuTime = value; }
        }

        protected List<int> _EpochNumbers;

        /// <summary>List of epoch numbers at which convergence data was sampled.</summary>
        public List<int> EpochNumbers
        {
            get { return _EpochNumbers; }
            protected set { _EpochNumbers = value; }
        }

        /// <summary>Sets the list of epoch numbers at which convergence data was sampled.</summary>
        /// <param name="epochNumbers">Array of epoch numbers from which data is copied.</param>
        public void SetEpochNumbers(int[] epochNumbers)
        {
            if (epochNumbers == null)
                this.EpochNumbers = null;
            else
                this.EpochNumbers = new List<int>(epochNumbers);
        }

        /// <summary>Sets the list of epoch numbers at which convergence data was sampled.</summary>
        /// <param name="epochNumbers">List of epoch numbers from which data is copied.</param>
        public void SetEpochNumbers(List<int> epochNumbers)
        {
            if (epochNumbers == null)
                this.EpochNumbers = null;
            else
                this.EpochNumbers = new List<int>(epochNumbers);
        }

        protected List<double> _EpochErrorsRms;

        /// <summary>List of sampled RMS errors corresponding to epoch numbers from <see cref="EpochNumbers"/>.</summary>
        public List<double> EpochErrorsRms
        {
            get { return _EpochErrorsRms; }
            set { _EpochErrorsRms = value; }
        }
        
        /// <summary>Sets the list of sampled RMS errors that correspond to epoch numbers from <see cref="EpochNumbers"/>.</summary>
        /// <param name="errors">Array from which data is copied.</param>
        public void SetEpochErrorsRms(double[] errors)
        {
            if (errors == null)
                EpochErrorsRms = null;
            else
                EpochErrorsRms = new List<double>(errors);
        }

        /// <summary>Sets the list of sampled RMS errors corresponding to epoch numbers from <see cref="EpochNumbers"/>.</summary>
        /// <param name="errors">List from which data is copied.</param>
        public void SetEpochErrorsRms(List<double> errors)
        {
            if (errors == null)
                EpochErrorsRms = null;
            else
                EpochErrorsRms = new List<double>(errors);
        }

        protected List<double> _EpochErrorsAbs;

        /// <summary>List of sampled absolute errors corresponding to epoch numbers from <see cref="EpochNumbers"/>.</summary>
        public List<double> EpochErrorsAbs
        {
            get { return _EpochErrorsAbs; }
            set { _EpochErrorsAbs = value; }
        }

        /// <summary>Sets the list of sampled absolute errors that correspond to epoch numbers from <see cref="EpochNumbers"/>.</summary>
        /// <param name="errors">Array from which data is copied.</param>
        public void SetEpochErrorsAbs(double[] errors)
        {
            if (errors == null)
                EpochErrorsAbs = null;
            else
                EpochErrorsAbs = new List<double>(errors);
        }

        /// <summary>Sets the list of sampled absolute errors corresponding to epoch numbers from <see cref="EpochNumbers"/>.</summary>
        /// <param name="errors">List from which data is copied.</param>
        public void SetEpochErrorsAbs(List<double> errors)
        {
            if (errors == null)
                EpochErrorsAbs = null;
            else
                EpochErrorsAbs = new List<double>(errors);
        }


        #endregion TrainingResults


        #region Operation 

        // TODO - Tadej: check in CopyTo/CopyFrom that all relevant data is copied!


        /// <summary>Copies current data from the specified neural network approximator.</summary>
        /// <param name="nn">Neural network approximator that data is copied form.</param>
        public void CopyFrom(NeuralApproximatorBase nn)
        {
            // Training parameters:
            LearningRate = nn.LearningRate;
            Momentum = nn.Momentum;
            SigmoidAlphaValue = nn.SigmoidAlphaValue;
            InputLength = nn.InputLength;
            OutputLength = nn.OutputLength;
            InputBoundSafetyFactor = nn.InputBoundsSafetyFactor;
            OutputBoundSafetyFactor = nn.OutputBoundsSafetyFactor;
            MaxEpochs = nn.MaxEpochs;
            EpochsInBundle = nn.EpochsInBundle;
            IVector vecAux = null;
            Vector.Copy(nn.ToleranceRms, ref vecAux);
            ToleranceRms = vecAux;
            vecAux = null;
            Vector.Copy(nn.ToleranceRmsRelativeToRange, ref vecAux);
            ToleranceRmsRelativeToRange = vecAux;
            ToleranceRmsRelativeToRangeScalar = nn.ToleranceRmsRelativeToRangeScalar;
            vecAux = null;
            Vector.Copy(nn.ToleranceMax, ref vecAux);
            ToleranceMax = vecAux;
            vecAux = null;
            Vector.Copy(nn.ToleranceMaxRelativeToRange, ref vecAux);
            ToleranceMaxRelativeToRange = vecAux;
            ToleranceMaxRelativeToRangeScalar = nn.ToleranceMaxRelativeToRangeScalar;
            IBoundingBox outputBounds = null;
            nn.TrainingData.GetOutputRange(ref outputBounds);
            OutputNeuronRange = outputBounds;
            IBoundingBox inputBounds = null;
            nn.TrainingData.GetInputRange(ref inputBounds);
            InputNeuronRange = inputBounds;
            // Architecture
            NumHiddenLayers = nn.NumHiddenLayers;
            NumHidenNeurons = nn.NumHiddenNeurons;
            // Results:
            CopyResultsFrom(nn);
        }


        /// <summary>Copies only results from the trained network to the current object.</summary>
        /// <param name="nn">Neural network approximator which results are copied from.</param>
        public void CopyResultsFrom(NeuralApproximatorBase nn)
        {
            if (nn.NetworkTrained)
            {
                this.IsNetworkTrained = true;
                nn.GetErrorsTrainingRms(ref _errorsTrainingRms);
                nn.GetErrorsTrainingMax(ref _errorsTrainingMax);
                nn.GetErrorsTrainingMeanAbs(ref _errorsTrainingMeanAbs);
                nn.GetErrorsVerificationRms(ref _errorsVerificationRms);
                nn.GetErrorsVerificationMax(ref _errorsVerificationMax);
                nn.GetErrorsVerificationMeanAbs(ref _errorsVerificationMeanAbs);
                
                // TODO: Cas treniranja in ostale podatke o konvergenci lahko nastavis rocno tam, kjer treniras,
                // vendar na koncu dodaj na NeuralAproximator-ju (v base classu), da se bodo ti podatki avtomatsko
                // snemali v interne strukture (ki se na zacetku treniranja resetirajo!!)
                // Potem tu dodaj, da se te strukture prepisejo z nevronske mreze!
                // Snemanje podatkov o konvergenci na NeuralAproximator-ju konvergiraj z ustreznimi flagi, ki jih dodas,
                // npr. IsConvergenceDataRecorded. Na zacetku lahko uporabljas en flag za vse konvergencne podatke,
                // pozneje lahko za vsako vrsto napak uporabis poseben flag.

                this.SaveConvergenceRms = nn.SaveConvergenceRms;
                this.ErrorsTrainingRmsList = null;
                if (nn.EpochNumbers != null)
                {
                    this.EpochNumbers = new List<int>();
                    for (int i = 0; i < nn.EpochNumbers.Count; ++i)
                    {
                        int vecAux = 0;
                        vecAux = nn.EpochNumbers[i];
                        this.EpochNumbers.Add(vecAux);
                    }
                }
                if (nn.ConvergenceErrorsTrainingRmsList != null)
                {
                    this.ErrorsTrainingRmsList = new List<IVector>();
                    for (int i = 0; i < nn.ConvergenceErrorsTrainingRmsList.Count; ++i)
                    {
                        IVector vecAux = null;
                        Vector.Copy(nn.ConvergenceErrorsTrainingRmsList[i], ref vecAux);
                        this.ErrorsTrainingRmsList.Add(vecAux);
                    }
                }
                if (nn.ConvergenceErrorsTrainingMaxList != null)
                {
                    this.ErrorsTrainingMaxList = new List<IVector>();
                    for (int i = 0; i < nn.ConvergenceErrorsTrainingMaxList.Count; ++i)
                    {
                        IVector vecAux = null;
                        Vector.Copy(nn.ConvergenceErrorsTrainingMaxList[i], ref vecAux);
                        this.ErrorsTrainingMaxList.Add(vecAux);
                    }
                }
                if (nn.ConvergenceErrorsVerificationRmsList != null)
                {
                    this.ErrorsVerificationRmsList = new List<IVector>();
                    for (int i = 0; i < nn.ConvergenceErrorsVerificationRmsList.Count; ++i)
                    {
                        IVector vecAux = null;
                        Vector.Copy(nn.ConvergenceErrorsVerificationRmsList[i], ref vecAux);
                        this.ErrorsVerificationRmsList.Add(vecAux);
                    }
                }
                if (nn.ConvergenceErrorsVerificationMaxList != null)
                {
                    this.ErrorsVerificationMaxList = new List<IVector>();
                    for (int i = 0; i < nn.ConvergenceErrorsVerificationMaxList.Count; ++i)
                    {
                        IVector vecAux = null;
                        Vector.Copy(nn.ConvergenceErrorsVerificationMaxList[i], ref vecAux);
                        this.ErrorsVerificationMaxList.Add(vecAux);
                    }
                }
            }
            else
            {
                this.IsNetworkTrained = false;
                ErrorsTrainingRms = null;
                ErrorsTrainingMax = null;
                ErrorsTrainingMeanAbs = null;
                ErrorsVerificationRms = null;
                ErrorsVerificationMax = null;
                ErrorsVerificationMeanAbs = null;
                //ConvergenceListRms = null;
                ErrorsTrainingRmsList = null;
                ErrorsTrainingMaxList = null;
                ErrorsVerificationRmsList = null;
                ErrorsVerificationMaxList = null;
                this.TrainingTime = 0.0;
                this.TrainingCpuTime = 0.0;
                this.EpochNumbers = null;
                this.EpochErrorsRms = null;
                this.EpochErrorsAbs = null;
            }
        }

        /// <summary>Copies data that determine neural network and training procedure (such as network architecture,
        /// training parameters, tolerances, etc.) from the current object to the specified neural network
        /// approximator. This enables to restore training contitions of a peviour training procedure.</summary>
        /// <param name="nn">Neural network approximator that data is copied to.</param>
        public void CopyTo(NeuralApproximatorBase nn)
        {
            // Training parameters:
            nn.LearningRate = LearningRate;
            nn.Momentum = Momentum;
            nn.SigmoidAlphaValue = SigmoidAlphaValue;
            nn.InputLength = InputLength;
            nn.OutputLength = OutputLength;
            nn.InputBoundsSafetyFactor = InputBoundSafetyFactor;
            nn.OutputBoundsSafetyFactor = OutputBoundSafetyFactor;
            nn.MaxEpochs = MaxEpochs;
            nn.EpochsInBundle = EpochsInBundle;
            IVector vecAux = null;
            Vector.Copy(ToleranceRms, ref vecAux);
            nn.ToleranceRms = vecAux;
            vecAux = null;
            Vector.Copy(ToleranceRmsRelativeToRange, ref vecAux);
            nn.ToleranceRmsRelativeToRange = vecAux;
            nn.ToleranceRmsRelativeToRangeScalar = ToleranceRmsRelativeToRangeScalar;
            vecAux = null;
            Vector.Copy(ToleranceMax, ref vecAux);
            nn.ToleranceMax = vecAux;
            vecAux = null;
            Vector.Copy(ToleranceMaxRelativeToRange, ref vecAux);
            nn.ToleranceMaxRelativeToRange = vecAux;
            nn.ToleranceMaxRelativeToRangeScalar = ToleranceMaxRelativeToRangeScalar;

            nn.SaveConvergenceRms = this.SaveConvergenceRms;
            // Remark on commented code blow: Convergence list is not copied because these are results!
            //if (this.ConvergenceListRms == null)
            //    nn.ConvergenceListRms = null;
            //else
            //{
            //    nn.ConvergenceListRms = new List<IVector>();
            //    for (int i = 0; i < this.ConvergenceListRms.Count; ++i)
            //    {
            //        IVector vecAux1 = null;
            //        Vector.Copy(this.ConvergenceListRms[i], ref vecAux1);
            //        nn.ConvergenceListRms.Add(vecAux1);
            //    }
            //}
            // Architecture
            nn.NumHiddenLayers = NumHiddenLayers;
            int[] numHiddenNeruons = NumHidenNeurons;
            for (int i = 0; i < NumHiddenLayers; ++i)
            {
                if (numHiddenNeruons !=null)
                {
                    if (numHiddenNeruons.Length > i)
                        nn.SetNumNeuronsInHiddenLayer(i, numHiddenNeruons[i]);
                    else 
                        nn.SetNumNeuronsInHiddenLayer(i, DefaultNumHiddenNeurons);
                } else
                    nn.SetNumNeuronsInHiddenLayer(i, DefaultNumHiddenNeurons);
            }
        }


        #endregion Operation


        #region StaticMethods

        // Saving / restoring training parameters and results to a JSON file:

        /// <summary>Saves (serializes) the specified training parameters object to the specified JSON file.
        /// File is owerwritten if it exists.</summary>
        /// <param name="trainingParameters">Object that is saved to a file.</param>
        /// <param name="inputFilePath">Path to the file into which object is is saved.</param>
        public static void SaveJson(NeuralTrainingParameters trainingParameters, string filePath)
        {
            SaveJson(trainingParameters, filePath, false /* append */ );
        }

        /// <summary>Saves (serializes) the specified training parameters object to the specified JSON file.</summary>
        /// <param name="trainingParameters">Object that is saved to a file.</param>
        /// <param name="inputFilePath">Path to the file into which object is is saved.</param>
        /// <param name="append">Specifies whether serialized data is appended at the end of the file
        /// in the case that the file already exists.</param>
        public static void SaveJson(NeuralTrainingParameters trainingParameters, string filePath, bool append)
        {
            NeuralTrainingParametersDto dtoOriginal = new NeuralTrainingParametersDto();
            dtoOriginal.CopyFrom(trainingParameters);
            ISerializer serializer = new SerializerJson();
            serializer.Serialize<NeuralTrainingParametersDto>(dtoOriginal, filePath, append);
        }

        /// <summary>Restores (deserializes) a training parameters object from the specified file in JSON format.</summary>
        /// <param name="inputFilePath">File from which object data is restored.</param>
        /// <param name="trainingParametersRestored">Object that is restored by deserialization.</param>
        public static void LoadJson(string filePath, ref NeuralTrainingParameters trainingParametersRestored)
        {
            ISerializer serializer = new SerializerJson();
            NeuralTrainingParametersDto dtoRestored = serializer.DeserializeFile<NeuralTrainingParametersDto>(filePath);
            dtoRestored.CopyTo(ref trainingParametersRestored);
        }


        // Saving / restoring ARRAYs of training parameters and results to a JSON file:

        /// <summary>Saves (serializes) the specified array of training parameters objects to the specified JSON file.
        /// File is owerwritten if it exists.</summary>
        /// <param name="trainingParameters">Array that is saved to a file.</param>
        /// <param name="inputFilePath">Path to the file into which object is is saved.</param>
        public static void SaveJson(NeuralTrainingParameters[] trainingParameters, string filePath)
        {
            SaveJson(trainingParameters, filePath, false /* append */ );
        }

        /// <summary>Saves (serializes) the specified array of training parameters objects to the specified JSON file.</summary>
        /// <param name="trainingParameters">Array that is saved to a file.</param>
        /// <param name="inputFilePath">Path to the file into which object is is saved.</param>
        /// <param name="append">Specifies whether serialized data is appended at the end of the file
        /// in the case that the file already exists.</param>
        public static void SaveJson(NeuralTrainingParameters[] trainingParameters, string filePath, bool append)
        {
            ArrayDto<NeuralTrainingParameters, NeuralTrainingParametersDto> dtoOriginal =
                new ArrayDto<NeuralTrainingParameters, NeuralTrainingParametersDto>();
            dtoOriginal.CopyFrom(trainingParameters);
            ISerializer serializer = new SerializerJson();
            serializer.Serialize<ArrayDto<NeuralTrainingParameters, NeuralTrainingParametersDto>>(dtoOriginal, filePath, append);
        }

        /// <summary>Restores (deserializes) an array of training parameters objects from the specified file in JSON format.</summary>
        /// <param name="inputFilePath">File from which array of objects is restored.</param>
        /// <param name="trainingParametersRestored">Array of objects that is restored by deserialization.</param>
        public static void LoadJson(string filePath, ref NeuralTrainingParameters[] trainingParametersRestored)
        {
            ISerializer serializer = new SerializerJson();
            ArrayDto<NeuralTrainingParameters, NeuralTrainingParametersDto> dtoRestored =
                serializer.DeserializeFile<ArrayDto<NeuralTrainingParameters, NeuralTrainingParametersDto>>(filePath);
            dtoRestored.CopyTo(ref trainingParametersRestored);
        }


        // Saving / restoring LISTs of training parameters and results to a JSON file:

        /// <summary>Saves (serializes) the specified list of training parameters objects to the specified JSON file.
        /// File is owerwritten if it exists.</summary>
        /// <param name="trainingParameters">List that is saved to a file.</param>
        /// <param name="inputFilePath">Path to the file into which object is is saved.</param>
        public static void SaveJson(List<NeuralTrainingParameters> trainingParameters, string filePath)
        {
            SaveJson(trainingParameters, filePath, false /* append */ );
        }

        /// <summary>Saves (serializes) the specified list of training parameters objects to the specified JSON file.</summary>
        /// <param name="trainingParameters">List that is saved to a file.</param>
        /// <param name="inputFilePath">Path to the file into which object is is saved.</param>
        /// <param name="append">Specifies whether serialized data is appended at the end of the file
        /// in the case that the file already exists.</param>
        public static void SaveJson(List<NeuralTrainingParameters> trainingParameters, string filePath, bool append)
        {
            ListDto<NeuralTrainingParameters, NeuralTrainingParametersDto> dtoOriginal =
                new ListDto<NeuralTrainingParameters, NeuralTrainingParametersDto>();
            dtoOriginal.CopyFrom(trainingParameters);
            ISerializer serializer = new SerializerJson();
            serializer.Serialize<ListDto<NeuralTrainingParameters, NeuralTrainingParametersDto>>(dtoOriginal, filePath, append);
        }

        /// <summary>Restores (deserializes) a list of training parameters objects from the specified file in JSON format.</summary>
        /// <param name="inputFilePath">File from which list of objects is restored.</param>
        /// <param name="trainingParametersRestored">Array of objects that is restored by deserialization.</param>
        public static void LoadJson(string filePath, ref List<NeuralTrainingParameters> trainingParametersRestored)
        {
            ISerializer serializer = new SerializerJson();
            ListDto<NeuralTrainingParameters, NeuralTrainingParametersDto> dtoRestored =
                serializer.DeserializeFile<ListDto<NeuralTrainingParameters, NeuralTrainingParametersDto>>(filePath);
            dtoRestored.CopyTo(ref trainingParametersRestored);
        }

        /// <summary>Saves the specified list of training parameters objects to the specified CSV file.</summary>
        /// <param name="trainingParameters">List that is saved to a file.</param>
        /// <param name="inputFilePath">Path to the file into which object is is saved.</param>
        /// $A Tako78 Sep12;
        public static void SaveCSV(List<NeuralTrainingParameters> trainingParameters, string filePath)
        {
            int numSamples;
            int outputLenght;
            int numElements;
            numSamples = trainingParameters.Count;
            outputLenght = trainingParameters[0].OutputLength;
            numElements = 10;
            using (StreamWriter writer = new StreamWriter(filePath, false))
            {
                // Settings
                string inputLengthName = "Input_Length";
                string outputLengthName = "Output_Length";
                string learningRateName = "Learning_Rate";
                string momentumName = "Momentum";
                string sigmoidAlphaValueName = "Alpha_Value";
                string numHiddenLayersName = "Num_Hidden_Layers";
                string numHidenNeurons1Name = "Num_Hiden_Neurons_1";
                string numHidenNeurons2Name = "Num_Hiden_Neurons_2";
                string numHidenNeurons3Name = "Num_Hiden_Neurons_3";
                string maxEpochsName = "Max_Epochs";
                string epochInBundleName = "Epoch_in_Bundle";
                string inputBoundSafetyFactorName = "Input_safety_factor";
                string outputBoundSafetyFactorName = "Output_safety_factor";      
                string toleranceRmsName = "Tolerance_Rms";              
                // Results
                string epochNumberName = "Epoch_Number";
                string [] errorsTrainingMeanAbs = new string[outputLenght];
                string [] errorsTrainingRmsName = new string[outputLenght];
                string [] errorsTrainingMaxName = new string[outputLenght];
                string [] errorsVerificationRmsName = new string[outputLenght];
                string [] errorsVerificationMaxName = new string[outputLenght];
                string [] errorsTrainingRmsTableName = new string[outputLenght];
                string [] errorsTrainingMaxTableName = new string[outputLenght];
                string [] errorsVerificationRmsTableName = new string[outputLenght];
                string [] errorsVerificationMaxTableName = new string[outputLenght];

                for (int i = 0; i < outputLenght; i++)
                {
                    //errorsTrainingMeanAbs [i] = "Errors_Training_Mean_Abs_" + i;
                    //errorsTrainingRmsName[i] = "Errors_Training_Rms_" + i;
                    //errorsTrainingMaxName[i] = "Errors_Training_Max_" + i;
                    //errorsVerificationRmsName[i] = "Errors_Verification_Rms_" + i;
                    //errorsVerificationMaxName[i] = "Errors_Verification_Max_" + i;
                    errorsTrainingRmsTableName[i] = "Errors_Training_Rms_Table_" + i;
                    errorsTrainingMaxTableName[i] = "Errors_Training_Max_Table_" + i;
                    errorsVerificationRmsTableName[i] = "Errors_Verification_Rms_Table_" + i;
                    errorsVerificationMaxTableName[i] = "Errors_Verification_Max_Table_" + i;
                }
                string trainingTimeName = "Training_Time";

                // Write setting descriptions to file
                writer.Write(inputLengthName + ";");
                writer.Write(outputLengthName + ";");
                writer.Write(learningRateName + ";");
                writer.Write(momentumName + ";");
                writer.Write(sigmoidAlphaValueName + ";");
                writer.Write(numHiddenLayersName + ";");
                writer.Write(numHidenNeurons1Name + ";");
                writer.Write(numHidenNeurons2Name + ";");
                writer.Write(numHidenNeurons3Name + ";");
                writer.Write(maxEpochsName + ";");
                writer.Write(epochInBundleName + ";");
                writer.Write(inputBoundSafetyFactorName + ";");
                writer.Write(outputBoundSafetyFactorName + ";");
                writer.Write(toleranceRmsName + ";");

                // Write results descriptions to file
                writer.Write(epochNumberName + ";");
                for (int i = 0; i < outputLenght; i++)
                {
                    //writer.Write(errorsTrainingMeanAbs[i] + ";");
                    //writer.Write(errorsTrainingRmsName[i] + ";");
                    //writer.Write(errorsTrainingMaxName[i] + ";");
                    //writer.Write(errorsVerificationRmsName[i] + ";");
                    //writer.Write(errorsVerificationMaxName[i] + ";");
                    writer.Write(errorsTrainingRmsTableName[i] + ";");
                    writer.Write(errorsTrainingMaxTableName[i] + ";");
                    writer.Write(errorsVerificationRmsTableName[i] + ";");
                    writer.Write(errorsVerificationMaxTableName[i] + ";");
                    
                }
                writer.Write(trainingTimeName + ";");
                writer.WriteLine();

                // write settings and results to file
                for (int i = 0; i < numSamples; i++)
                {
                    writer.Write(trainingParameters[i].InputLength + ";");
                    writer.Write(trainingParameters[i].OutputLength + ";");
                    writer.Write(trainingParameters[i].LearningRate + ";");
                    writer.Write(trainingParameters[i].Momentum + ";");
                    writer.Write(trainingParameters[i].SigmoidAlphaValue + ";");
                    writer.Write(trainingParameters[i].NumHiddenLayers + ";");
                    writer.Write(trainingParameters[i].NumHidenNeurons[0] + ";");
                    if (trainingParameters[i].NumHiddenLayers >= 2)
                        writer.Write(trainingParameters[i].NumHidenNeurons[1] + ";");
                    else
                        writer.Write("null" + ";");
                    if (trainingParameters[i].NumHiddenLayers == 3)
                        writer.Write(trainingParameters[i].NumHidenNeurons[2] + ";");
                    else
                        writer.Write("null" + ";");                        
                    writer.Write(trainingParameters[i].MaxEpochs + ";");
                    writer.Write(trainingParameters[i].EpochsInBundle + ";");
                    writer.Write(trainingParameters[i].InputBoundSafetyFactor + ";");
                    writer.Write(trainingParameters[i].OutputBoundSafetyFactor + ";");
                    writer.Write(trainingParameters[i].ToleranceRms[0] + ";");

                    writer.Write(trainingParameters[i].EpochNumbers.Count + ";");
                    for (int j = 0; j < outputLenght; j++)
                    {
                        //writer.Write(trainingParameters[i].ErrorsTrainingMeanAbs[j] + ";");
                        //writer.Write(trainingParameters[i].ErrorsTrainingRms[j] + ";");
                        //Console.WriteLine(trainingParameters[i].ErrorsTrainingRms[j] + ";");
                        //writer.Write(trainingParameters[i].ErrorsTrainingMax[j] + ";");
                        //writer.Write(trainingParameters[i].ErrorsVerificationRms[j] + ";");              
                        //writer.Write(trainingParameters[i].ErrorsVerificationMax[j] + ";");
                        //List<IVector> errorsTrainingRmsList = new List<IVector>();
                        //errorsTrainingRmsList = trainingParameters[i].ErrorsTrainingRmsList;
                        writer.Write(trainingParameters[i].ErrorsTrainingRmsList[trainingParameters[i].ErrorsTrainingRmsList.Count-1][j] + ";");
                        if (j == 0)
                            Console.WriteLine(trainingParameters[i].ErrorsTrainingRmsList[trainingParameters[i].ErrorsTrainingRmsList.Count - 1][j] + ";");
                        writer.Write(trainingParameters[i].ErrorsTrainingMaxList[trainingParameters[i].ErrorsTrainingMaxList.Count-1][j] + ";");
                        writer.Write(trainingParameters[i].ErrorsVerificationRmsList[trainingParameters[i].ErrorsVerificationRmsList.Count-1][j] + ";");
                        writer.Write(trainingParameters[i].ErrorsVerificationMaxList[trainingParameters[i].ErrorsVerificationMaxList.Count-1][j] + ";");
                    }
                    writer.Write(trainingParameters[i].TrainingTime + ";");
                    
                    writer.WriteLine();
                }

                writer.Close();
            }
        }


        
        /// <summary> Calculate average number of the desired number of first elements in the convergence list. </summary>
        /// <param name="ErrorConvergence">List of convergence errors. </param>
        /// <param name="NumLastErrors"> Number of elemnts in the convergence list. </param>
        /// <returns> Average number. </returns>
        protected static double ErrorAverageCalc(List<IVector> ErrorConvergence, int NumLastErrors, int NumBundles, IVector scalingLength)
        {
            int counta;
            if (NumBundles == 0)
                counta = ErrorConvergence.Count - 1;
            else
                counta = NumBundles;

            double errorTrainingRmsa = 0;
            if (NumLastErrors == 0)
                NumLastErrors = 1;

            for (int i = counta; i > counta - NumLastErrors; i--)
            {
                double norm;
                if (scalingLength == null)
                    norm = ErrorConvergence[i].NormEuclidean;
                else
                    norm = VectorBase.NormWeighted(ErrorConvergence[i], scalingLength);
                // TODO: replace first component with norm!
                errorTrainingRmsa = errorTrainingRmsa // + ErrorConvergence[i][0]
                 + norm; 
            }
            errorTrainingRmsa = errorTrainingRmsa / NumLastErrors;

            return errorTrainingRmsa;

        }

        /// <summary> Calculate average number of the desired number of first elements in the convergence list. </summary>
        /// <param name="ErrorConvergence">List of convergence errors. </param>
        /// <param name="NumLastErrors"> Number of elemnts in the convergence list. </param>
        /// <returns> Average number. </returns>
        protected static double ErrorAverageCalc(List<IVector> ErrorConvergence, int NumLastErrors, int NumBundles)
        {
            int counta;
            if (NumBundles == 0)
                counta = ErrorConvergence.Count - 1;
            else
                counta = NumBundles;

            double errorTrainingRmsa = 0;
            if (NumLastErrors == 0)
                NumLastErrors = 1;

            for (int i = counta; i > counta - NumLastErrors; i--)
            {
                //double norm;
                //if (ScalingLengths == null)
                //    norm = ErrorConvergence[i].NormEuclidean;
                //else
                //    norm = VectorBase.NormWeighted(ErrorConvergence[i], ScalingLengths);
                // TODO: replace first component with norm!
                errorTrainingRmsa = errorTrainingRmsa + ErrorConvergence[i][0];
                      // + OutputNornm(ErrorConvergence[i]); // TODO: Containing method must be instance method!
            }
            errorTrainingRmsa = errorTrainingRmsa / NumLastErrors;

            return errorTrainingRmsa;
        }


        #endregion StaticMethods

    }  // class NeuralTrainingParameters


    /// <summary>Contains Parameters that define neural network architecture limits and trainig parameter limits.
    /// <para>Not thread safe!</para></summary>
    /// <remarks>This class is used for storing training parameter limits of neural networks and restoring them at a later time, in
    /// order to repeat training under similar condition or simply to analyse performance of  neural networks.</remarks>
    /// $A Igor Jul10 Aug12 Nov12; Tako78 Aug12;
    public class NeuralTrainingLimits
    {

        #region DefaultLimitValues

        /// <summary> Default value for minimum learning rate in neural networks. </summary>
        public static double DefaultLearningRateMin = 0.1;
        /// <summary> Default value for maximum learning rate in neural networks. </summary>
        public static double DefaultLearningRateMax = 0.6;
        /// <summary> Default value for number of learning rates in neural networks. </summary>
        public static int DefaultLearningRateNum = 5;

        /// <summary> Default value for minimum momentum in neural networks. </summary>
        public static double DefaultMomentumMin = 0.3;
        /// <summary> Default value for maximum momentum in neural networks. </summary>
        public static double DefaultMomentumMax = 0.8;
        /// <summary> Default value for number of momentums in neural networks. </summary>
        public static int DefaultMomentumNum = 5;

        /// <summary> Default value for minimum alpha value in neural networks. </summary>
        public static double DefaultAlphaMin = 1.0;
        /// <summary> Default value for maximum alpha value in neural networks. </summary>
        public static double DefaultAlphaMax = 2.0;
        /// <summary> Default value for number of alpha values in neural networks. </summary>
        public static int DefaultAlphaNum = 5;

        /// <summary> Default value for minimum input bound safety factor value in neural networks. </summary>
        public static double DefaultInputSafetyFactorMin = 1.4;
        /// <summary> Default value for maximum input bound safety factor value in neural networks. </summary>
        public static double DefaultInputSafetyFactorMax = 1.4;
        /// <summary> Default value for number of input bound safety factors values in neural networks. </summary>
        public static int DefaultInputSafetyFactorNum = 1;

        /// <summary> Default value for minimum output bound safety factor value in neural networks. </summary>
        public static double DefaultOutputSafetyFactorMin = 1.4;
        /// <summary> Default value for maximum output bound safety factor value in neural networks. </summary>
        public static double DefaultOutputSafetyFactorMax = 1.4;
        /// <summary> Default value for number of output bound safety factors values in neural networks. </summary>
        public static int DefaultOutputSafetyFactorNum = 1;

        /// <summary> Default flag for enabling different layers in neural networks. </summary>
        public static bool DefaultEnableArchitectureTest = false;

        ///// <summary> Default value for minimum number of hidden layers in neural networks. </summary>
        //public static int DefaultNumHiddenLayersMin = 1;
        ///// <summary> Default value for maximum number of hidden layers in neural networks. </summary>
        //public static int DefaultNumHiddenLayersMax = 3;
        /// <summary> Default value for number of hidden layers in neural networks. </summary>
        public static int DefaultNumHiddenLayersNum = 1;

        /// <summary> Default value for number of hidden neurons in first hidden layer in neural networks. </summary>
        public static int DefaultNumHiddenNeuronsFirstMin = 5;
        /// <summary> Default value for number of hidden neurons in first hidden layer in neural networks. </summary>
        public static int DefaultNumHiddenNeuronsFirstMax = 25;
        /// <summary> Default value for number of hidden neurons in first hidden layer in neural networks. </summary>
        public static int DefaultNumHiddenNeuronsFirstNum = 1;

        /// <summary> Default value for number of hidden neurons in second hidden layer in neural networks. </summary>
        public static int DefaultNumHiddenNeuronsSecondMin = 2;
        /// <summary> Default value for number of hidden neurons in second hidden layer in neural networks. </summary>
        public static int DefaultNumHiddenNeuronsSecondMax = 10;
        /// <summary> Default value for number of hidden neurons in second hidden layer in neural networks. </summary>
        public static int DefaultNumHiddenNeuronsSecondNum = 1;

        /// <summary> Default value for number of hidden neurons in third hidden layer in neural networks. </summary>
        public static int DefaultNumHiddenNeuronsThirdMin = 2;
        /// <summary> Default value for number of hidden neurons in third hidden layer in neural networks. </summary>
        public static int DefaultNumHiddenNeuronsThirdMax = 10;
        /// <summary> Default value for number of hidden neurons in third hidden layer in neural networks. </summary>
        public static int DefaultNumHiddenNeuronsThirdNum = 1;
        
        /// <summary> Default value for maximum number of epochs in neural networks. </summary>
        public static int DefaultMaxEpochs = 1000;
        /// <summary> Default value for number of epochs in bundle in neural networks. </summary>
        public static int DefaultEpochBundle = 100;

        /// <summary> Flag for enabling toelrance that represent a percentage of the output range. </summary>
        public static bool DefaultEnableRangeTolerance = false;

        /// <summary> Default value for number of input neurons. </summary>
        public static int DefaultInputLenght = 5;
        /// <summary> Default value for number of output neurons. </summary>
        public static int DefaultOutputLenght = 3;

        #endregion DefaultLimitValues


        #region TrainintLimitParameters

        protected double _learningRateMin = NeuralTrainingParameters.DefaultLearningRate;

        /// <summary>Minimum limit for learning rate.</summary>
        public double LearningRateMin
        {
            get { return _learningRateMin; }
            set { _learningRateMin = value; }
        }

        protected double _learningRateMax = NeuralTrainingParameters.DefaultLearningRate;

        /// <summary>Maximum limit for learning rate.</summary>
        public double LearningRateMax
        {
            get { return _learningRateMax; }
            set { _learningRateMax = value; }
        }

        protected int _learningRateNum = 1;  // DefaultLearningRateNum;

        /// <summary>Number of learning rates.</summary>
        public int LearningRateNum
        {
            get { return _learningRateNum; }
            set { _learningRateNum = value; }
        }

        // Custom values of learning rate:


        protected double _momentumMin = DefaultMomentumMin;

        /// <summary>Minimum limit for momentum.</summary>
        public double MomentumMin
        {
            get { return _momentumMin; }
            set { _momentumMin = value; }
        }

        protected double _momentumMax = DefaultMomentumMax;

        /// <summary>Maximum limit for momentum.</summary>
        public double MomentumMax
        {
            get { return _momentumMax; }
            set { _momentumMax = value; }
        }

        protected int _momentumNum = DefaultMomentumNum;

        /// <summary>Number of momentums.</summary>
        public int MomentumNum
        {
            get { return _momentumNum; }
            set { _momentumNum = value; }
        }


        protected double _alphaMin = DefaultAlphaMin;

        /// <summary>Minimum limit for alpha value.</summary>
        public double AlphaMin
        {
            get { return _alphaMin; }
            set { _alphaMin = value; }
        }

        protected double _alphaMax = DefaultAlphaMax;

        /// <summary>Maximum limit for alpha value.</summary>
        public double AlphaMax
        {
            get { return _alphaMax; }
            set { _alphaMax = value; }
        }

        protected int _alphaNum = DefaultAlphaNum;

        /// <summary>Number of alpha value.</summary>
        public int AlphaNum
        {
            get { return _alphaNum; }
            set { _alphaNum = value; }
        }


        protected double _inputSafetyFactorMin = DefaultInputSafetyFactorMin;

        /// <summary>Minimum limit for input safety factor value.</summary>
        public double InputSafetyFactorMin
        {
            get { return _inputSafetyFactorMin; }
            set { _inputSafetyFactorMin = value; }
        }

        protected double _inputSafetyFactorMax = DefaultInputSafetyFactorMax;

        /// <summary>Maximum limit for input safety factor value.</summary>
        public double InputSafetyFactorMax
        {
            get { return _inputSafetyFactorMax; }
            set { _inputSafetyFactorMax = value; }
        }

        protected int _inputSafetyFactorNum = DefaultInputSafetyFactorNum;

        /// <summary>Number of input safety factor values.</summary>
        public int InputSafetyFactorNum
        {
            get { return _inputSafetyFactorNum; }
            set { _inputSafetyFactorNum = value; }
        }


        protected double _outputSafetyFactorMin = DefaultOutputSafetyFactorMin;

        /// <summary>Minimum limit for output safety factor value.</summary>
        public double OutputSafetyFactorMin
        {
            get { return _outputSafetyFactorMin; }
            set { _outputSafetyFactorMin = value; }
        }

        protected double _outputSafetyFactorMax = DefaultOutputSafetyFactorMax;

        /// <summary>Maximum limit for output safety factor value.</summary>
        public double OutputSafetyFactorMax
        {
            get { return _outputSafetyFactorMax; }
            set { _outputSafetyFactorMax = value; }
        }

        protected int _outputSafetyFactorNum = DefaultOutputSafetyFactorNum;

        /// <summary>Number of output safety factor values.</summary>
        public int OutputSafetyFactorNum
        {
            get { return _outputSafetyFactorNum; }
            set { _outputSafetyFactorNum = value; }
        }


        protected int _maxEpochs = DefaultMaxEpochs;

        /// <summary>Maximum number of epochs performed in training.</summary>
        public int MaxEpochs
        {
            get { return _maxEpochs; }
            set { _maxEpochs = value; }
        }

        protected int _epochBundle = DefaultEpochBundle;

        /// <summary>Number of epochs in boundle.</summary>
        public int EpochBundle
        {
            get { return _epochBundle; }
            set { _epochBundle = value; }
        }

        protected int _inputLenght = DefaultInputLenght;

        /// <summary>Number of input neurons.</summary>
        public int InputLenght
        {
            get { return _inputLenght; }
            set { _inputLenght = value; }
        }

        protected int _outputLenght = DefaultOutputLenght;

        /// <summary>Number of output neurons.</summary>
        public int OutputLength
        {
            get { return _outputLenght; }
            set { _outputLenght = value; }
        }

        // TODO: Check whether EnableRangeTolerance is necessary!

        protected bool _enableRangeTolerance = DefaultEnableRangeTolerance;

        /// <summary>Flag for enabling toelrance that represent a percentage of the output range.</summary>
        public bool EnableRangeTolerance
        {
            get { return _enableRangeTolerance; }
            set { _enableRangeTolerance = value; }
        }
        
        protected IVector _toleranceRms ; // = DefaultToleranceRms;

        /// <summary>Tolerance for RMS.</summary>
        public IVector ToleranceRms
        {
            get { return _toleranceRms; }
            set {
                if (value != null)
                {
                    if (value.Length != OutputLength)
                        throw new ArgumentException("Dimension of the specified vector of tolerances on RMS values " + value.Length
                            + " is different than the number of outputs " + OutputLength + ".");
                }
                _toleranceRms = value;
            }
        }
        
        protected IVector _toleranceMax;

        /// <summary>Maximum tolerance for max. abs. difference.</summary>
        public IVector ToleranceMax
        {
            get { return _toleranceMax; }
            set {
                if (value != null)
                {
                    if (value.Length != OutputLength)
                        throw new ArgumentException("Dimension of the specified vector of tolerances on maximal values " + value.Length
                            + " is different than the number of outputs " + OutputLength + ".");
                } 
                _toleranceMax = value;
            }
        }


        /// Auxiliary properties for defining tolerances in a relative way:

        private IVector _tolRmsRelative;

        /// <summary>Relative tolerances on RMS errors of outputs over training points, relative to the 
        /// correspoinding ranges of output data.</summary>
        /// <seealso cref="NeuralApproximatorBase.ToleranceRmsRelativeToRange"/>
        public IVector ToleranceRmsRelativeToRange
        {
            get
            {
                return _tolRmsRelative;
            }
            set
            {
                if (value != null)
                {
                    if (value.Length != OutputLength)
                        throw new ArgumentException("Dimension of the specified vector of relative RMS tolerances " + value.Length
                            + " is different than the number of outputs " + OutputLength + ".");
                }
                _tolRmsRelative = value;
            }
        }


        private double _tolRmsRelativeScalar = NeuralTrainingParameters.DefaultToleranceRmsRelativeToRangeScalar;

        /// <summary>Scalar through which all components of the Relative tolerances on RMS errors of outputs
        /// can be set to the same value.</summary>
        /// <seealso cref="NeuralApproximatorBase.ToleranceRmsRelativeToRangeScalar"/>
        public double ToleranceRmsRelativeToRangeScalar
        {
            get { return _tolRmsRelativeScalar; }
            set { _tolRmsRelativeScalar = value; }
        }

        private IVector _tolMaxRelative;

        /// <summary>Relative tolerances on max. abs. errors of outputs over training points, relative to the 
        /// correspoinding ranges of output data.</summary>
        /// <seealso cref="NeuralApproximatorBase.ToleranceMaxRelativeToRange"/>
        public IVector ToleranceMaxRelativeToRange
        {
            get
            {
                return _tolMaxRelative;
            }
            set
            {
                if (value != null)
                {
                    if (value.Length != OutputLength)
                        throw new ArgumentException("Dimension of the specified vector of relative max. abs. tolerances " + value.Length
                            + " is different than the number of outputs " + OutputLength + ".");
                }
                _tolMaxRelative = value;
            }
        }

        private double _tolMaxRelativeScalar = NeuralTrainingParameters.DefaultToleranceMaxRelativeToRangeScalar;

        /// <summary>Scalar through which all components of the Relative tolerances on max. abs. errors of outputs
        /// can be set to the same value.</summary>
        /// <seealso cref="NeuralApproximatorBase.ToleranceMaxRelativeToRangeScalar"/>
        public double ToleranceMaxRelativeToRangeScalar
        {
            get { return _tolMaxRelativeScalar; }
            set { _tolMaxRelativeScalar = value; }
        }



        #endregion TrainintLimitParameters


        #region NetworkArchitecture

        protected bool _enableArchitectureTest = DefaultEnableArchitectureTest;

        /// <summary>Flag for enabling test in architecture of ANN.</summary>
        public bool EnableArchitectureTest
        {
            get { return _enableArchitectureTest; }
            set { _enableArchitectureTest = value; }
        }
        
        
        //protected int _numHiddenLayersMin = DefaultNumHiddenLayersMin;

        ///// <summary>Minimum number of hidden layers in neural network.</summary>
        //public int NumHiddenLayersMin
        //{
        //    get { return _numHiddenLayersMin; }
        //    set { _numHiddenLayersMin = value; }
        //}

        //protected int _numHiddenLayersMax = DefaultNumHiddenLayersMax;

        ///// <summary>Maximim number of hidden layers in neural network.</summary>
        //public int NumHiddenLayersMax
        //{
        //    get { return _numHiddenLayersMax; }
        //    set { _numHiddenLayersMax = value; }
        //}


        // Remark: tabulating for different numbers of hidden layers is not supported.

        protected int _numHiddenLayersNum = DefaultNumHiddenLayersNum;

        /// <summary>Number of hidden layers in neural network.</summary>
        public int NumHiddenLayersNum
        {
            get { return _numHiddenLayersNum; }
            set { _numHiddenLayersNum = value; }
        }

        protected int _numHiddenNeuronsFirstMin = DefaultNumHiddenNeuronsFirstMin;

        /// <summary>Minimum number of hidden neurons in first hidden layer.</summary>
        public int NumHiddenNeuronsFirstMin
        {
            get { return _numHiddenNeuronsFirstMin; }
            set {
                if (value != _numHiddenNeuronsFirstMin)
                {
                    //// Invalidate dependencies:
                    //if (NumHiddenNeuronsFirstValues != null)
                    //    NumHiddenNeuronsFirstValues = null;
                }
                _numHiddenNeuronsFirstMin = value; }
        }

        protected int _numHiddenNeuronsFirstMax = DefaultNumHiddenNeuronsFirstMax;

        /// <summary>Maximum number of hidden neurons in first hidden layer.</summary>
        public int NumHiddenNeuronsFirstMax
        {
            get { return _numHiddenNeuronsFirstMax; }
            set {
                if (value != _numHiddenNeuronsFirstMax)
                {
                    //// Invalidate dependencies:
                    //if (NumHiddenNeuronsFirstValues != null)
                    //    NumHiddenNeuronsFirstValues = null;
                }
                _numHiddenNeuronsFirstMax = value;
            }
        }


        protected int _numHiddenNeuronsFirstNum = DefaultNumHiddenNeuronsFirstNum;

        /// <summary>Number of hidden neurons in first hidden layer.</summary>
        public int NumHiddenNeuronsFirstNum
        {
            get { return _numHiddenNeuronsFirstNum; }
            set {
                if (value != _numHiddenNeuronsFirstNum)
                {
                    //// Invalidate dependencies:
                    //if (NumHiddenNeuronsFirstValues != null)
                    //    NumHiddenNeuronsFirstValues = null;
                }
                _numHiddenNeuronsFirstNum = value; 
            }
        }


        protected int[] _numHiddenNeuronsFirstValues;
        protected bool randomTableEnabled = false;

        /// <summary>Values of number of hidden neurons in the first layers that will appear in the table.</summary>
        /// <remarks>Specifying an array of values for the number of hidden neurons in the first layer makes possible
        /// to use custom values, not just equidistant values between minimal and maximal value.
        /// <para>If array of values is set, the number of values is set to the length of the array, minimal value
        /// is set to its first element and maximal value is set to its last element.</para></remarks>
        public int[] NumHiddenNeuronsFirstValues
        {
            get { return _numHiddenNeuronsFirstValues; }
            set {
                bool modified = false;
                if (value != _numHiddenNeuronsFirstValues)
                    modified = true;
                if (value != null)
                {
                    if (value.Length == 0)
                    {
                        throw new ArgumentException("Array of values has no elements.");
                    }
                }
                // Set the value:
                _numHiddenNeuronsFirstValues = value;
                if (modified)
                {
                    // Invalidate dependencies:
                    if (value != null)
                    {
                        if (value.Length > 0)
                        {
                            if (value.Length != NumHiddenNeuronsFirstNum)
                                NumHiddenNeuronsFirstNum = value.Length;//NumHiddenLayersNum = value.Length;
                            NumHiddenNeuronsFirstMin = value[0];
                            NumHiddenNeuronsFirstMax = value[value.Length - 1];
                            randomTableEnabled = true;
                        }
                    }
                }
            }
        }


        protected int _numHiddenNeuronsSecondMin = DefaultNumHiddenNeuronsSecondMin;

        /// <summary>Minimum number of hidden neurons in second hidden layer.</summary>
        public int NumHiddenNeuronsSecondMin
        {
            get { return _numHiddenNeuronsSecondMin; }
            set
            {
                if (value != _numHiddenNeuronsSecondMin)
                {
                    //// Invalidate dependencies:
                    //if (NumHiddenNeuronsSecondValues != null)
                    //    NumHiddenNeuronsSecondValues = null;
                }
                _numHiddenNeuronsSecondMin = value;
            }
        }

        protected int _numHiddenNeuronsSecondMax = DefaultNumHiddenNeuronsSecondMax;

        /// <summary>Maximum number of hidden neurons in second hidden layer.</summary>
        public int NumHiddenNeuronsSecondMax
        {
            get { return _numHiddenNeuronsSecondMax; }
            set
            {
                if (value != _numHiddenNeuronsSecondMax)
                {
                    //// Invalidate dependencies:
                    //if (NumHiddenNeuronsSecondValues != null)
                    //    NumHiddenNeuronsSecondValues = null;
                }
                _numHiddenNeuronsSecondMax = value;
            }
        }

        protected int _numHiddenNeuronsSecondNum = DefaultNumHiddenNeuronsSecondNum;

        /// <summary>Number of hidden neurons in second hidden layer.</summary>
        public int NumHiddenNeuronsSecondNum
        {
            get { return _numHiddenNeuronsSecondNum; }
            set
            {
                if (value != _numHiddenNeuronsSecondNum)
                {
                    //// Invalidate dependencies:
                    //if (NumHiddenNeuronsSecondValues != null)
                    //    NumHiddenNeuronsSecondValues = null;
                }
                _numHiddenNeuronsSecondNum = value;
            }
        }

        protected int[] _numHiddenNeuronsSecondValues;

        /// <summary>Values of number of hidden neurons in the second layers that will appear in the table.</summary>
        /// <remarks>Specifying an array of values for the number of hidden neurons in the second layer makes possible
        /// to use custom values, not just equidistant values between minimal and maximal value.
        /// <para>If array of values is set, the number of values is set to the length of the array, minimal value
        /// is set to its first element and maximal value is set to its last element.</para></remarks>
        public int[] NumHiddenNeuronsSecondValues
        {
            get { return _numHiddenNeuronsSecondValues; }
            set
            {
                bool modified = false;
                if (value != _numHiddenNeuronsSecondValues)
                    modified = true;
                if (value != null)
                {
                    if (value.Length == 0)
                    {
                        throw new ArgumentException("Array of values has no elements.");
                    }
                }
                // Set the value:
                _numHiddenNeuronsSecondValues = value;
                if (modified)
                {
                    // Invalidate dependencies:
                    if (value != null)
                    {
                        if (value.Length > 0)
                        {
                            if (value.Length != NumHiddenNeuronsSecondNum)
                                NumHiddenNeuronsSecondNum = value.Length;//NumHiddenLayersNum = value.Length;
                            NumHiddenNeuronsSecondMin = value[0];
                            NumHiddenNeuronsSecondMax = value[value.Length - 1];
                        }
                    }
                }
            }
        }


        protected int _numHiddenNeuronsThirdMin = DefaultNumHiddenNeuronsThirdMin;

        /// <summary>Minimum number of hidden neurons in third hidden layer.</summary>
        public int NumHiddenNeuronsThirdMin
        {
            get { return _numHiddenNeuronsThirdMin; }
            set
            {
                if (value != _numHiddenNeuronsThirdMin)
                {
                    //// Invalidate dependencies:
                    //if (NumHiddenNeuronsThirdValues != null)
                    //    NumHiddenNeuronsThirdValues = null;
                }
                _numHiddenNeuronsThirdMin = value;
            }
        }

        protected int _numHiddenNeuronsThirdMax = DefaultNumHiddenNeuronsThirdMax;

        /// <summary>Maximum number of hidden neurons in third hidden layer.</summary>
        public int NumHiddenNeuronsThirdMax
        {
            get { return _numHiddenNeuronsThirdMax; }
            set
            {
                if (value != _numHiddenNeuronsThirdMax)
                {
                    //// Invalidate dependencies:
                    //if (NumHiddenNeuronsThirdValues != null)
                    //    NumHiddenNeuronsThirdValues = null;
                }
                _numHiddenNeuronsThirdMax = value;
            }
        }

        protected int _numHiddenNeuronsThirdNum = DefaultNumHiddenNeuronsThirdNum;

        /// <summary>Number of hidden neurons in third hidden layer.</summary>
        public int NumHiddenNeuronsThirdNum
        {
            get { return _numHiddenNeuronsThirdNum; }
            set
            {
                if (value != _numHiddenNeuronsThirdNum)
                {
                    //// Invalidate dependencies:
                    //if (NumHiddenNeuronsThirdValues != null)
                    //    NumHiddenNeuronsThirdValues = null;
                }
                _numHiddenNeuronsThirdNum = value;
            }
        }

        protected int[] _numHiddenNeuronsThirdValues;

        /// <summary>Values of number of hidden neurons in the third layer that will appear in the table.</summary>
        /// <remarks>Specifying an array of values for the number of hidden neurons in the third layer makes possible
        /// to use custom values, not just equidistant values between minimal and maximal value.
        /// <para>If the array of values is set, the number of values is set to the length of the array, minimal value
        /// is set to its first element and maximal value is set to its last element.</para></remarks>
        public int[] NumHiddenNeuronsThirdValues
        {
            get { return _numHiddenNeuronsThirdValues; }
            set
            {
                bool modified = false;
                if (value != _numHiddenNeuronsThirdValues)
                    modified = true;
                if (value != null)
                {
                    if (value.Length == 0)
                    {
                        throw new ArgumentException("Array of values has no elements.");
                    }
                }
                // Set the value:
                _numHiddenNeuronsThirdValues = value;
                if (modified)
                {
                    // Invalidate dependencies:
                    if (value != null)
                    {
                        if (value.Length > 0)
                        {
                            if (value.Length != NumHiddenNeuronsThirdNum)
                                NumHiddenNeuronsThirdNum = value.Length;//NumHiddenLayersNum = value.Length;
                            NumHiddenNeuronsThirdMin = value[0];
                            NumHiddenNeuronsThirdMax = value[value.Length - 1];
                        }
                    }
                }
            }
        }

        /// <summary>Creates and returns a copy of the specified array of integers.</summary>
        /// <param name="original">Array whose copy is returned.</param>
        protected int[] GetArrayCopy(int[] original)
        {
            int[] ret = null;
            if (original != null)
            {
                int length = original.Length;
                if (length == 0)
                    ret = new int[0];
                else
                {
                    ret = new int[length];
                    for (int i = 0; i < length; ++i)
                    {
                        ret[i] = original[i];
                    }
                }
            }
            return ret;
        }


        /// <summary>Creates and returns an array of integers that is a copy of the specified array of 
        /// double values (double to integer conversion made by rounding).</summary>
        /// <param name="original">Array whose copy is returned.</param>
        protected int[] GetArrayCopyInt(double[] original)
        {
            int[] ret = null;
            if (original != null)
            {
                int length = original.Length;
                if (length == 0)
                    ret = new int[0];
                else
                {
                    ret = new int[length];
                    for (int i = 0; i < length; ++i)
                    {
                        ret[i] = (int) Math.Round(original[i]);
                    }
                }
            }
            return ret;
        }


        /// <summary>Prepares values of numbers of neurons in individual layers according to parameters.
        /// <para>For each layer, numbers of neurons in that layers to be used in the table are the same.</para></summary>
        /// <param name="numLayers">Number of hidden layers (this is fixed for the tables where this method is used; the
        /// containing class also does not support tables where number of layers would vary).</param>
        /// <param name="numHiddenNeuronsValues">Values for the number of hidden neurons in different layers (common for all layers).</param>
        public void PrepareNumHiddenNeuronsValuesArray(int numLayers, params int[] numHiddenNeuronsValues)
        {
            NumHiddenLayersNum = numLayers;
            if (numLayers < 3)
                NumHiddenNeuronsThirdNum = 1;
            else
                NumHiddenNeuronsThirdValues = GetArrayCopy(numHiddenNeuronsValues);
            if (numLayers < 2)
                NumHiddenNeuronsSecondNum = 1;
            else
                NumHiddenNeuronsSecondValues = GetArrayCopy(numHiddenNeuronsValues);
            if (numLayers < 1)
                NumHiddenNeuronsFirstNum = 1;
            else
                NumHiddenNeuronsFirstValues = GetArrayCopy(numHiddenNeuronsValues);
        }


        /// <summary>Prepares values of numbers of neurons in individual layers according to parameters in such 
        /// a way that intervals between these values grow exponentially.</summary>
        /// <param name="numLayers">Number of hidden layers (this is fixed for the tables where this method is used; the
        /// containing class also does not support tables where number of layers would vary).</param>
        /// <param name="minNumNeurons">Minimal value for the number fo neurons in hidden layers.</param>
        /// <param name="maxNumNeurons">Maxmial value for the number of neurons in hidden layers.</param>
        /// <param name="numValues">Number of values for the number of neurons in hidden layers.</param>
        /// <param name="intervalGrowthFactor">Factor by which length of each next iterval befoore 
        /// successive values for the number of neurons in hidden layers is extended.</param>
        public void PrepareNumHiddenNeuronsValuesArray(int numLayers, int minNumNeurons, 
            int maxNumNeurons, int numValues, double intervalGrowthFactor)
        {
            double min = minNumNeurons;
            double max = maxNumNeurons;
            GridGenerator1d grid = new GridGenerator1d(min, max, numValues, intervalGrowthFactor);
            PrepareNumHiddenNeuronsValuesArray(numLayers, GetArrayCopyInt(grid.GetNodeTable()));
        }


        #endregion


        #region Operation

        public delegate void DoForParameters(List<NeuralTrainingParameters> trainingParameters,
            List<int> dimensions, int which);



        public void DoForParameters_CreateTable(List<NeuralTrainingParameters> trainingParameters,
            List<int> dimensions, int which)
        {
            // Does nothing.
        }

        /// <summary>Creates a table of training parameters.</summary>
        /// <param name="trainingParameters">List where table of parameters is stored.
        /// Must be different than null. Eventual elements already contained will be deleted.</param>
        /// <param name="tableDimensions">List of dimensions of the generated table. If specified
        /// then table dimensions will be put on the list (the list is cleared at the beinning).</param>
        public void CreateTrainingTable(List<NeuralTrainingParameters> trainingParameters, List<int> tableDimensions)
        {
            bool createDimensions = true;
            if (tableDimensions==null)
                createDimensions = false;
            IterateThroughMultidimensionalTable(trainingParameters, tableDimensions, true /* createTable */,
                createDimensions, null  /* DoForParameters_CreateTable ;  doOnElement */);
        }


        /// <summary>Creates a table of training parameters.</summary>
        /// <param name="trainingParameters">List where table of parameters is stored.
        /// Must be different than null. Eventual elements already contained will be deleted.</param>
        public void CreateTrainingTable(List<NeuralTrainingParameters> trainingParameters)
        {
            CreateTrainingTable(trainingParameters, null);
        }

        /// <summary> Prepares table of neurons in geometric sequence. </summary>
        /// <param name="minNeurons"> Minimum number of neurons. </param>
        /// <param name="maxNeurons"> Maximum number of neurons. </param>
        /// <param name="numNeurons"> Number of neurons in sequence. </param>
        /// <param name="neurpnsTable"> Table of neurons. </param>
        /// $A Tako78 Nov12;
        private void PrepareNeuronsTable(int minNeurons, int maxNeurons, int numNeurons, double growthFactorref, ref int[] neurpnsTable)
        {
            double[] tmpNumNeurons = new double[numNeurons];
            neurpnsTable = new int[numNeurons];
            if (numNeurons > 1)
            {
                if (minNeurons >= maxNeurons)
                    throw new ArgumentException("Minimum value of neurons is bigger than maximum vale of neurons.");

                //Prepare table of neurons in hidden layers
                GridGenerator1d greedGenerator = new GridGenerator1d();
                greedGenerator.CoordinateFirst = minNeurons;
                greedGenerator.CoordinateLast = maxNeurons;
                greedGenerator.GrowthFactor = growthFactorref;
                greedGenerator.NumNodes = numNeurons;
                tmpNumNeurons = greedGenerator.GetNodeTable();

                for (int i = 0; i < tmpNumNeurons.Length; i++)
                {
                    neurpnsTable[i] = (int)tmpNumNeurons[i];
                }
            }
            else
                neurpnsTable[0] = maxNeurons;

        }

        private void PrepareNeuronTable(ref int[] neuronsFirstLayer, ref int[] neuronsSecondLayer, ref int[] neuronsThirdLayer)
        {
            // initialize data
            neuronsFirstLayer = null;
            neuronsSecondLayer = null;
            neuronsThirdLayer = null;
            double growthFactor = 1.0;

            // Check if Architecture test is enabled
            int countNumHiddenLayer;
            int tmpNumHiddenNeuronsFirstNum, tmpNumHiddenNeuronsSecondNum, tmpNumHiddenNeuronsThirdNum;
            if (EnableArchitectureTest)
            {
                countNumHiddenLayer = NumHiddenLayersNum;
                tmpNumHiddenNeuronsFirstNum = NumHiddenNeuronsFirstNum;
                tmpNumHiddenNeuronsSecondNum = NumHiddenNeuronsSecondNum;
                tmpNumHiddenNeuronsThirdNum = NumHiddenNeuronsThirdNum;
            }
            else
            {
                countNumHiddenLayer = 1;
                tmpNumHiddenNeuronsFirstNum = 1;
                tmpNumHiddenNeuronsSecondNum = 1;
                tmpNumHiddenNeuronsThirdNum = 1;
            }
            // Prepare table of neurons from minimum value to maximum value
            if (randomTableEnabled == false)
            {
                growthFactor = 1;

                // Prepare tables of neurons for each layer
                if (NumHiddenLayersNum > 0)
                    PrepareNeuronsTable(NumHiddenNeuronsFirstMin, NumHiddenNeuronsFirstMax, tmpNumHiddenNeuronsFirstNum, growthFactor, ref neuronsFirstLayer);
                if (NumHiddenLayersNum > 1)
                    PrepareNeuronsTable(NumHiddenNeuronsSecondMin, NumHiddenNeuronsSecondMax, tmpNumHiddenNeuronsSecondNum, growthFactor, ref neuronsSecondLayer);
                if (NumHiddenLayersNum > 2)
                    PrepareNeuronsTable(NumHiddenNeuronsThirdMin, NumHiddenNeuronsThirdMax, tmpNumHiddenNeuronsThirdNum, growthFactor, ref neuronsThirdLayer);
            }
            else
            {
                if (NumHiddenLayersNum > 0)
                {
                    neuronsFirstLayer = null;
                    neuronsFirstLayer = GetArrayCopy(NumHiddenNeuronsFirstValues);
                }
                if (NumHiddenLayersNum > 1)
                {
                    neuronsSecondLayer = null;
                    neuronsSecondLayer = GetArrayCopy(NumHiddenNeuronsSecondValues);
                }
                if (NumHiddenLayersNum > 2)
                {
                    neuronsThirdLayer = null;
                    neuronsThirdLayer = GetArrayCopy(NumHiddenNeuronsThirdValues);
                }
            }

            

        }

        /// <summary>Iterates through all elements of the table of training results defined by the current object,
        /// and does whatever is specified by the parameters.</summary>
        /// <param name="tableResults">Table where training parameters and results are stored.
        /// Must be specified when this table is created (i.e. parameter <paramref name="createTable"/> is true) or when
        /// something must be done on each element (i.e. function parameter is <paramref name="doOnElement"/> defined).</param>
        /// <param name="tableDimensions">Table where dimensions are stored. Must be specified when the
        /// table of dimensions will be created (i.e. the parameter <paramref name="createDimensions"/> is true).</param>
        /// <param name="createTable">Specifies whether the table of training parameters and results will be
        /// created by the function call. If true then the parameter <paramref name="tableResults"/>
        /// must be specified.</param>
        /// <param name="createDimensions">Specifies whether the table of dimensions will be created when iterating
        /// over elements. If true then the parameter <paramref name="tableDimensions"/> must be specified.</param>
        /// <param name="doOnElement">Delegate that is executed on each element of the table.
        /// Index of the current element, as well as the table of the resulet (parameter 
        /// <paramref name="tableResults"/>) and the table of dimensions 
        /// (parameter <paramref name="tableDimensions"/>) are passed to the delegate when executed.</param>
        public void IterateThroughMultidimensionalTable(List<NeuralTrainingParameters> tableResults,
            List<int> tableDimensions, bool createTable, bool createDimensions,
            DoForParameters doOnElement)
        {
            // Table of neurons
            int[] neuronsFirstLayer = null;
            int[] neuronsSecondLayer = null;
            int[] neuronsThirdLayer = null;
            int secondLayerNum = 1;
            int thirdLayerNum = 1;
            PrepareNeuronTable(ref neuronsFirstLayer, ref neuronsSecondLayer, ref neuronsThirdLayer);

            tableResults.Clear();
            double stepLearningRate;
            double stepMomentum;
            double stepAlpha;
            double stepInputSafetyFactor;
            double stepOutputSafetyFactor;

            if (LearningRateNum == 1)
                stepLearningRate = 0.0;
            else
                stepLearningRate = (LearningRateMax - LearningRateMin) / ((double)LearningRateNum - 1.0);
            if (MomentumNum == 1)
                stepMomentum = 0.0;
            else
                stepMomentum = (MomentumMax - MomentumMin) / ((double)MomentumNum - 1.0);
            if (AlphaNum == 1)
                stepAlpha = 0.0;
            else
                stepAlpha = (AlphaMax - AlphaMin) / ((double)AlphaNum - 1.0);
            if (InputSafetyFactorNum == 1)
                stepInputSafetyFactor = 0.0;
            else
                stepInputSafetyFactor = (InputSafetyFactorMax - InputSafetyFactorMin) / ((double)InputSafetyFactorNum - 1.0);
            if (OutputSafetyFactorNum == 1)
                stepOutputSafetyFactor = 0.0;
            else
                stepOutputSafetyFactor = (OutputSafetyFactorMax - OutputSafetyFactorMin) / ((double)OutputSafetyFactorNum - 1.0);

            // Check if Architecture test is enabled
            int countNumHiddenLayer;
            if (EnableArchitectureTest)
            {
                countNumHiddenLayer = NumHiddenLayersNum;
            }
            else
            {
                countNumHiddenLayer = 1;
            }

            if (countNumHiddenLayer > 1)
                secondLayerNum = neuronsSecondLayer.Length;
            if (countNumHiddenLayer > 2)
                thirdLayerNum = neuronsThirdLayer.Length;
            // Compete list of tables
            List<int[]> completeNeuronList = new List<int[]>();
            for (int i = 0; i < neuronsFirstLayer.Length; i++)
            {
                for (int j = 0; j < secondLayerNum; j++)
                {
                    for (int k = 0; k < thirdLayerNum; k++)
                    {
                        int[] hiddenNeurons = new int[countNumHiddenLayer];
                        hiddenNeurons[0] = neuronsFirstLayer[i];
                        if (countNumHiddenLayer > 1)
                            hiddenNeurons[1] = neuronsSecondLayer[j];
                        if (countNumHiddenLayer > 2)
                            hiddenNeurons[2] = neuronsThirdLayer[k];
                        completeNeuronList.Add(hiddenNeurons);
                    }                           
                }     
            }

            // Iterate over elements of training table
            if (createTable && tableResults != null)
                    tableResults.Clear();
            if (createDimensions && tableDimensions != null)
                tableDimensions.Clear();
            int elementIndex = -1;  // index of the current element
            if (createDimensions && LearningRateNum > 1 && tableDimensions != null)
                tableDimensions.Add(LearningRateNum);
            for (int iLearningRate = 0; iLearningRate < LearningRateNum; ++iLearningRate)
            {
                if (createDimensions && MomentumNum > 1 && tableDimensions != null)
                    tableDimensions.Add(MomentumNum);
                for (int iMomentum = 0; iMomentum < MomentumNum; ++iMomentum)
                {
                    if (createDimensions && AlphaNum > 1 && tableDimensions != null)
                        tableDimensions.Add(AlphaNum);
                    for (int iAlphaNum = 0; iAlphaNum < AlphaNum; ++iAlphaNum)
                    {
                        if (createDimensions && InputSafetyFactorNum > 1 && tableDimensions != null)
                            tableDimensions.Add(InputSafetyFactorNum);
                        for (int iInputSafetyFactorNum = 0; iInputSafetyFactorNum < InputSafetyFactorNum; ++iInputSafetyFactorNum)
                        {
                            if (createDimensions && OutputSafetyFactorNum > 1 && tableDimensions != null)
                                tableDimensions.Add(OutputSafetyFactorNum);
                            for (int iOutputSafetyFactorNum = 0; iOutputSafetyFactorNum < OutputSafetyFactorNum; ++iOutputSafetyFactorNum)
                            {
                                if (createDimensions && countNumHiddenLayer > 1 && tableDimensions != null)
                                    tableDimensions.Add(countNumHiddenLayer);
                                for (int ihiddenLayers = 0; ihiddenLayers < completeNeuronList.Count; ihiddenLayers++)
                                {
                                    // Prepare a new set of training parameters as member of a 3D array:
                                    NeuralTrainingParameters par = new NeuralTrainingParameters();
                                    par.LearningRate = LearningRateMin + (double)iLearningRate * stepLearningRate;
                                    par.Momentum = MomentumMin + (double)iMomentum * stepMomentum;
                                    par.SigmoidAlphaValue = AlphaMin + (double)iAlphaNum * stepAlpha;
                                    par.InputBoundSafetyFactor = InputSafetyFactorMin + (double)iInputSafetyFactorNum * stepInputSafetyFactor;
                                    par.OutputBoundSafetyFactor = OutputSafetyFactorMin + (double)iOutputSafetyFactorNum * stepOutputSafetyFactor;
                                    par.NumHiddenLayers = countNumHiddenLayer;
                                    par.InputLength = InputLenght;
                                    par.OutputLength = OutputLength;
                                    par.MaxEpochs = MaxEpochs;
                                    par.EpochsInBundle = EpochBundle;
                                    if (ToleranceRms.Length != OutputLength)
                                        throw new ArgumentException("Dimension of the specified vector of tolerances on RMS values " + ToleranceRms.Length
                                            + " is different than the number of outputs " + OutputLength + ".");

                                    IVector vecAux = null;
                                    Vector.Copy(this.ToleranceRms, ref vecAux);
                                    par.ToleranceRms = vecAux;
                                    vecAux = null;
                                    Vector.Copy(this.ToleranceRmsRelativeToRange, ref vecAux);
                                    par.ToleranceRmsRelativeToRange = vecAux;
                                    par.ToleranceRmsRelativeToRangeScalar = this.ToleranceRmsRelativeToRangeScalar;
                                    vecAux = null;
                                    Vector.Copy(this.ToleranceMax, ref vecAux);
                                    par.ToleranceMax = vecAux;
                                    vecAux = null;
                                    Vector.Copy(this.ToleranceMaxRelativeToRange, ref vecAux);
                                    par.ToleranceMaxRelativeToRange = vecAux;
                                    par.ToleranceMaxRelativeToRangeScalar = this.ToleranceMaxRelativeToRangeScalar;

                                    // Copy hidden neurons in each hidden layer
                                    par.NumHidenNeurons = completeNeuronList[ihiddenLayers];

                                    tableResults.Add(par);
                                }  // if (createTable)

                                if (doOnElement != null && tableResults != null)
                                {
                                    doOnElement(tableResults, tableDimensions, elementIndex);
                                }
                            }
                        }
                    }
                }
            }
        }

        ///// <summary>Iterates through all elements of the table of training results defined by the current object,
        ///// and does whatever is specified by the parameters.</summary>
        ///// <param name="tableResults">Table where training parameters and results are stored.
        ///// Must be specified when this table is created (i.e. parameter <paramref name="createTable"/> is true) or when
        ///// something must be done on each element (i.e. function parameter is <paramref name="doOnElement"/> defined).</param>
        ///// <param name="tableDimensions">Table where dimensions are stored. Must be specified when the
        ///// table of dimensions will be created (i.e. the parameter <paramref name="createDimensions"/> is true).</param>
        ///// <param name="createTable">Specifies whether the table of training parameters and results will be
        ///// created by the function call. If true then the parameter <paramref name="tableResults"/>
        ///// must be specified.</param>
        ///// <param name="createDimensions">Specifies whether the table of dimensions will be created when iterating
        ///// over elements. If true then the parameter <paramref name="tableDimensions"/> must be specified.</param>
        ///// <param name="doOnElement">Delegate that is executed on each element of the table.
        ///// Index of the current element, as well as the table of the resulet (parameter 
        ///// <paramref name="tableResults"/>) and the table of dimensions 
        ///// (parameter <paramref name="tableDimensions"/>) are passed to the delegate when executed.</param>
        //public void IterateThroughMultidimensionalTable(List<NeuralTrainingParameters> tableResults,
        //    List<int> tableDimensions, bool createTable, bool createDimensions,
        //    DoForParameters doOnElement)
        //{
        //    int[] neuronsFirstLayer = null;
        //    int[] neuronsSecondLayer = null;
        //    int[] neuronsThirdLayer = null;

        //    PrepareNeuronTable(ref neuronsFirstLayer, ref neuronsSecondLayer, ref neuronsThirdLayer);

        //    tableResults.Clear();
        //    double stepLearningRate;
        //    double stepMomentum;
        //    double stepAlpha;
        //    double stepInputSafetyFactor;
        //    double stepOutputSafetyFactor;
        //    double stepNumHiddenNeuronsFirst;
        //    double stepNumHiddenNeuronsSecond;
        //    double stepNumHiddenNeuronsThird;
        //    int stepNumHiddenLayer = 1;
        //    if (LearningRateNum == 1)
        //        stepLearningRate = 0.0;
        //    else
        //        stepLearningRate = (LearningRateMax - LearningRateMin) / ((double)LearningRateNum - 1.0);
        //    if (MomentumNum == 1)
        //        stepMomentum = 0.0;
        //    else
        //        stepMomentum = (MomentumMax - MomentumMin) / ((double)MomentumNum - 1.0);
        //    if (AlphaNum == 1)
        //        stepAlpha = 0.0;
        //    else
        //        stepAlpha = (AlphaMax - AlphaMin) / ((double)AlphaNum - 1.0);
        //    if (InputSafetyFactorNum == 1)
        //        stepInputSafetyFactor = 0.0;
        //    else
        //        stepInputSafetyFactor = (InputSafetyFactorMax - InputSafetyFactorMin) / ((double)InputSafetyFactorNum - 1.0);
        //    if (OutputSafetyFactorNum == 1)
        //        stepOutputSafetyFactor = 0.0;
        //    else
        //        stepOutputSafetyFactor = (OutputSafetyFactorMax - OutputSafetyFactorMin) / ((double)OutputSafetyFactorNum - 1.0);
        //    if (NumHiddenNeuronsFirstNum == 1)
        //        stepNumHiddenNeuronsFirst = 0;
        //    else
        //        stepNumHiddenNeuronsFirst = (double)(NumHiddenNeuronsFirstMax - NumHiddenNeuronsFirstMin) / (double)(NumHiddenNeuronsFirstNum - 1);
        //    if (NumHiddenNeuronsSecondNum == 1)
        //        stepNumHiddenNeuronsSecond = 0;
        //    else
        //        stepNumHiddenNeuronsSecond = (double)(NumHiddenNeuronsSecondMax - NumHiddenNeuronsSecondMin) / (double)(NumHiddenNeuronsSecondNum - 1);
        //    if (NumHiddenNeuronsThirdNum == 1)
        //        stepNumHiddenNeuronsThird = 0;
        //    else
        //        stepNumHiddenNeuronsThird = (double)(NumHiddenNeuronsThirdMax - NumHiddenNeuronsThirdMin) / (double)(NumHiddenNeuronsThirdNum - 1);

        //    // Check if Architecture test is enabled
        //    int countNumHiddenLayer;
        //    int tmpNumHiddenNeuronsFirstNum, tmpNumHiddenNeuronsSecondNum, tmpNumHiddenNeuronsThirdNum;
        //    if (EnableArchitectureTest)
        //    {
        //        countNumHiddenLayer = NumHiddenLayersNum;
        //        tmpNumHiddenNeuronsFirstNum = NumHiddenNeuronsFirstNum;
        //        tmpNumHiddenNeuronsSecondNum = NumHiddenNeuronsSecondNum;
        //        tmpNumHiddenNeuronsThirdNum = NumHiddenNeuronsThirdNum;
        //    }
        //    else
        //    {
        //        countNumHiddenLayer = 1;
        //        tmpNumHiddenNeuronsFirstNum = 1;
        //        tmpNumHiddenNeuronsSecondNum = 1;
        //        tmpNumHiddenNeuronsThirdNum = 1;
        //    }

        //    // Iterate over elements of training table
        //    if (createTable && tableResults != null)
        //        tableResults.Clear();
        //    if (createDimensions && tableDimensions != null)
        //        tableDimensions.Clear();
        //    int elementIndex = -1;  // index of the current element
        //    if (createDimensions && LearningRateNum > 1 && tableDimensions != null)
        //        tableDimensions.Add(LearningRateNum);
        //    for (int iLearningRate = 0; iLearningRate < LearningRateNum; ++iLearningRate)
        //    {
        //        if (createDimensions && MomentumNum > 1 && tableDimensions != null)
        //            tableDimensions.Add(MomentumNum);
        //        for (int iMomentum = 0; iMomentum < MomentumNum; ++iMomentum)
        //        {
        //            if (createDimensions && AlphaNum > 1 && tableDimensions != null)
        //                tableDimensions.Add(AlphaNum);
        //            for (int iAlphaNum = 0; iAlphaNum < AlphaNum; ++iAlphaNum)
        //            {
        //                if (createDimensions && InputSafetyFactorNum > 1 && tableDimensions != null)
        //                    tableDimensions.Add(InputSafetyFactorNum);
        //                for (int iInputSafetyFactorNum = 0; iInputSafetyFactorNum < InputSafetyFactorNum; ++iInputSafetyFactorNum)
        //                {
        //                    if (createDimensions && OutputSafetyFactorNum > 1 && tableDimensions != null)
        //                        tableDimensions.Add(OutputSafetyFactorNum);
        //                    for (int iOutputSafetyFactorNum = 0; iOutputSafetyFactorNum < OutputSafetyFactorNum; ++iOutputSafetyFactorNum)
        //                    {
        //                        if (createDimensions && countNumHiddenLayer > 1 && tableDimensions != null)
        //                            tableDimensions.Add(countNumHiddenLayer);

        //                        for (int iHiddenLayerNum = 0; iHiddenLayerNum < countNumHiddenLayer; ++iHiddenLayerNum)
        //                        {
        //                            if (createDimensions && tmpNumHiddenNeuronsFirstNum > 1 && tableDimensions != null)
        //                                tableDimensions.Add(tmpNumHiddenNeuronsFirstNum);
        //                            for (int iNumHiddenNeuronsFirst = 0; iNumHiddenNeuronsFirst < tmpNumHiddenNeuronsFirstNum; ++iNumHiddenNeuronsFirst)
        //                            {
        //                                if (iHiddenLayerNum < 1)
        //                                    tmpNumHiddenNeuronsSecondNum = 1;
        //                                else
        //                                    tmpNumHiddenNeuronsSecondNum = NumHiddenNeuronsSecondNum;
        //                                if (createDimensions && tmpNumHiddenNeuronsSecondNum > 1 && tableDimensions != null)
        //                                    tableDimensions.Add(tmpNumHiddenNeuronsSecondNum);
        //                                for (int iNumHiddenNeuronsSecond = 0; iNumHiddenNeuronsSecond < tmpNumHiddenNeuronsSecondNum; ++iNumHiddenNeuronsSecond)
        //                                {
        //                                    if (iHiddenLayerNum < 2)
        //                                        tmpNumHiddenNeuronsThirdNum = 1;
        //                                    else
        //                                        tmpNumHiddenNeuronsThirdNum = NumHiddenNeuronsThirdNum;

        //                                    if (createDimensions && tmpNumHiddenNeuronsThirdNum != 1 && tableDimensions != null)
        //                                        tableDimensions.Add(tmpNumHiddenNeuronsThirdNum);
        //                                    for (int iNumHiddenNeuronsThird = 0; iNumHiddenNeuronsThird < tmpNumHiddenNeuronsThirdNum; ++iNumHiddenNeuronsThird)
        //                                    {
        //                                        ++elementIndex; // index of the current table element (1D)
        //                                        if (createTable && tableResults!=null)
        //                                        {
        //                                            // Prepare a new set of training parameters as member of a 3D array:
        //                                            NeuralTrainingParameters par = new NeuralTrainingParameters();
        //                                            par.LearningRate = LearningRateMin + (double)iLearningRate * stepLearningRate;
        //                                            par.Momentum = MomentumMin + (double)iMomentum * stepMomentum;
        //                                            par.SigmoidAlphaValue = AlphaMin + (double)iAlphaNum * stepAlpha;
        //                                            par.InputBoundSafetyFactor = InputSafetyFactorMin + (double)iInputSafetyFactorNum * stepInputSafetyFactor;
        //                                            par.OutputBoundSafetyFactor = OutputSafetyFactorMin + (double)iOutputSafetyFactorNum * stepOutputSafetyFactor;
        //                                            par.NumHiddenLayers = iHiddenLayerNum + stepNumHiddenLayer;
        //                                            par.InputLength = InputLenght;
        //                                            par.OutputLength = OutputLength;
        //                                            par.MaxEpochs = MaxEpochs;
        //                                            par.EpochsInBundle = EpochBundle;
        //                                            if (ToleranceRms.Length != OutputLength)
        //                                                throw new ArgumentException("Dimension of the specified vector of tolerances on RMS values " + ToleranceRms.Length
        //                                                    + " is different than the number of outputs " + OutputLength + ".");

        //                                            IVector vecAux = null;
        //                                            Vector.Copy(this.ToleranceRms, ref vecAux);
        //                                            par.ToleranceRms = vecAux;
        //                                            vecAux = null;
        //                                            Vector.Copy(this.ToleranceRmsRelativeToRange, ref vecAux);
        //                                            par.ToleranceRmsRelativeToRange = vecAux;
        //                                            par.ToleranceRmsRelativeToRangeScalar = this.ToleranceRmsRelativeToRangeScalar;
        //                                            vecAux = null;
        //                                            Vector.Copy(this.ToleranceMax, ref vecAux);
        //                                            par.ToleranceMax = vecAux;
        //                                            vecAux = null;
        //                                            Vector.Copy(this.ToleranceMaxRelativeToRange, ref vecAux);
        //                                            par.ToleranceMaxRelativeToRange = vecAux;
        //                                            par.ToleranceMaxRelativeToRangeScalar = this.ToleranceMaxRelativeToRangeScalar;

        //                                            int actualLayerNum = 0;
        //                                            // Check for number of hidden layers
        //                                            if (EnableArchitectureTest)
        //                                                actualLayerNum = iHiddenLayerNum + 1;
        //                                            else
        //                                                actualLayerNum = NumHiddenLayersNum;
        //                                            // Copy hidden neurons in each hidden layer
        //                                            int[] tmpNumHiddenNeurons = new int[actualLayerNum];
        //                                            if (tmpNumHiddenNeurons.Length >= 1)
        //                                            {
        //                                                if (NumHiddenNeuronsFirstValues == null)
        //                                                    tmpNumHiddenNeurons[0] = (int) Math.Round(NumHiddenNeuronsFirstMin + iNumHiddenNeuronsFirst * stepNumHiddenNeuronsFirst);
        //                                                else
        //                                                    tmpNumHiddenNeurons[0] = NumHiddenNeuronsFirstValues[iNumHiddenNeuronsFirst];
        //                                            }
        //                                            if (tmpNumHiddenNeurons.Length >= 2)
        //                                            {
        //                                                if (NumHiddenNeuronsSecondValues == null)
        //                                                    tmpNumHiddenNeurons[1] = (int) Math.Round(NumHiddenNeuronsSecondMin + iNumHiddenNeuronsSecond * stepNumHiddenNeuronsSecond);
        //                                                else
        //                                                    tmpNumHiddenNeurons[1] = NumHiddenNeuronsSecondValues[iNumHiddenNeuronsSecond];
        //                                            }
        //                                            if (tmpNumHiddenNeurons.Length >= 3)
        //                                            {
        //                                                if (NumHiddenNeuronsThirdValues == null)
        //                                                    tmpNumHiddenNeurons[2] = (int) Math.Round(NumHiddenNeuronsThirdMin + iNumHiddenNeuronsThird * stepNumHiddenNeuronsThird);
        //                                                else
        //                                                    tmpNumHiddenNeurons[2] = NumHiddenNeuronsThirdValues[iNumHiddenNeuronsThird];
        //                                            }
        //                                            par.NumHidenNeurons = tmpNumHiddenNeurons;

        //                                            tableResults.Add(par);
        //                                        }  // if (createTable)

        //                                        if (doOnElement != null && tableResults != null)
        //                                        {
        //                                            doOnElement(tableResults, tableDimensions, elementIndex);
        //                                        }
        //                                    }
        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}

        #endregion


        #region StaticMethods

        // Saving / restoring ARRAYs of training limits to a JSON file:

        /// <summary>Saves (serializes) the specified array of training limits objects to the specified JSON file.
        /// File is owerwritten if it exists.</summary>
        /// <param name="trainingParameters">Array that is saved to a file.</param>
        /// <param name="inputFilePath">Path to the file into which object is is saved.</param>
        public static void SaveJson(NeuralTrainingLimits trainingLimits, string filePath)
        {
            SaveJson(trainingLimits, filePath, false /* append */ );
        }

        /// <summary>Saves (serializes) the specified array of training limits objects to the specified JSON file.</summary>
        /// <param name="trainingParameters">Array that is saved to a file.</param>
        /// <param name="inputFilePath">Path to the file into which object is is saved.</param>
        /// <param name="append">Specifies whether serialized data is appended at the end of the file
        /// in the case that the file already exists.</param>
        public static void SaveJson(NeuralTrainingLimits trainingLimits, string filePath, bool append)
        {
            NeuralTrainingLimitsDto dtoOriginal = new NeuralTrainingLimitsDto();
            dtoOriginal.CopyFrom(trainingLimits);
            ISerializer serializer = new SerializerJson();
            serializer.Serialize<NeuralTrainingLimitsDto>(dtoOriginal, filePath, append);
        }

        /// <summary>Restores (deserializes) an array of training parameters objects from the specified file in JSON format.</summary>
        /// <param name="inputFilePath">File from which array of objects is restored.</param>
        /// <param name="trainingParametersRestored">Array of objects that is restored by deserialization.</param>
        public static void LoadJson(string filePath, ref NeuralTrainingLimits trainingLimits)
        {
            ISerializer serializer = new SerializerJson();
            NeuralTrainingLimitsDto dtoRestored = serializer.DeserializeFile<NeuralTrainingLimitsDto>(filePath);
            dtoRestored.CopyTo(ref trainingLimits);
        }

        #endregion


    } // class NeuralTrainingLimits


    /// <summary>Contains Parameters that define neural network architecture and trainig limits, together with 
    /// achieved results after training such as various error norms.
    /// <para>Not thread safe!</para></summary>
    /// <remarks>This class is used for storing parameters and limits of neural networks and restoring them at a later time, in
    /// order to repeat training under similar condition or simply to analyse performance of  neural networks.</remarks>
    /// $A Igor Jul10 Aug12; Tako78 Aug12;
    public class NeuralTrainingTable: ILockable
    {

        
        #region ThreadLocking

        private object _mainLock = new object();

        /// <summary>This object's central lock object to be used by other object.
        /// Do not use this object for locking in class' methods, for this you should use 
        /// InternalLock.</summary>
        public object Lock { get { return _mainLock; } }

        #endregion ThreadLocking



        #region TrainingLimits

        public NeuralTrainingLimits _trainingLimits = null;

        /// <summary>Contains Parameters that define neural network architecture limits and trainig parameter limits.<summary>
        public NeuralTrainingLimits TrainingLimits
        {
            get { return _trainingLimits; }
            set { 
                _trainingLimits = value;
                TrainingTableDimensions = null;
            }
        }

        #endregion TrainingLimits

        
        #region TrainingResults

        public List<NeuralTrainingParameters> _trainingParameters = null;

        /// <summary>Contains Parameters that define neural network architecture and trainig procedure, together with 
        /// achieved results after training such as various error norms.<summary>
        public List<NeuralTrainingParameters> TrainingParameters
        {
            get 
            { 
                return _trainingParameters; 
            }
            set { _trainingParameters = value; }
        }


        private List<int> _trainingTableDimensions;

        /// <summary>List of dimensions of the table of training results.</summary>
        public List<int> TrainingTableDimensions
        {
            get
            {
                lock (Lock)
                {
                    if (_trainingTableDimensions == null)
                    {
                        UpdateTrainingTableDimensionns();
                    }
                    return _trainingTableDimensions;
                }
            }
            private set {
                lock (Lock)
                {
                    _trainingTableDimensions = value;
                }
            }
        }

        /// <summary>Updates the list of dimensions of the table of training results (contained in
        /// the list <see cref="TrainingParameters"/>).</summary>
        public void UpdateTrainingTableDimensionns()
        {
            lock (Lock)
            {
                NeuralTrainingLimits limits = this.TrainingLimits;
                if (limits == null)
                {
                    TrainingTableDimensions = null;
                } else
                {
                    List<int> dimensions = TrainingTableDimensions;
                    if (dimensions == null)
                    {
                        dimensions = new List<int>();
                        limits.IterateThroughMultidimensionalTable(null, dimensions,
                            false /* createTable */, true /* createDimensions */, null /* doOnElement */);
                    }
                    TrainingTableDimensions = dimensions;
                }
            }
        }

        /// <summary>Gets the element of the training table specified by the indices.</summary>
        /// <param name="indices">Indices of the element in the table of training parameters.</param>
        public NeuralTrainingParameters this[params int[] indices]
        {
            get {
                lock (Lock)
                {
                    if (indices!=null)
                        if (indices.Length == 1)
                        {
                            // When number of dimensions is different than 1, element can be selected by 1 index:
                            return TrainingParameters[indices[0]];
                        }
                    return TrainingParameters[GetIndex(indices)];
                }
            }
        }

        /// <summary>Returns the index of the element in the onedimensional list that corresponds
        /// to the specified indices of the multidimensional table of elements.</summary>
        /// <param name="indices">Indices of the element in the multidimensional table of training results.</param>
        /// <returns>One dimensional index that corresponds to the specified multidimensional indices
        /// of the element of the multidimensional table of training results.</returns>
        public int GetIndex(int[] indices)
        {
            return Util.GetIndex(TrainingTableDimensions, indices);
            //lock (Lock)
            //{
            //    List<int> dimensions = TrainingTableDimensions;
            //    bool zeroDimensionalIndices = false;
            //    int numIndices = 0;
            //    if (indices == null)
            //        zeroDimensionalIndices = true;
            //    else
            //    {
            //        numIndices = indices.Length;
            //        if (indices.Length < 1)
            //            zeroDimensionalIndices = true;
            //    }
            //        if (zeroDimensionalIndices)
            //    {
            //        if (dimensions != null)
            //        {
            //            if (dimensions.Count > 0)
            //            {
            //                throw new ArgumentException("Indices are 0 dimensional while the table has "
            //                    + dimensions.Count + " dimensions.");
            //            }
            //        }
            //        return 0;
            //    } else
            //    {
            //        if (dimensions == null)
            //            throw new ArgumentException("Table dimensions are not defined but element indices are.");
            //        if (dimensions.Count != numIndices)
            //            throw new ArgumentException("Number of indices " + numIndices + " is different than number of dimensions " + dimensions.Count + ".");
            //        int index = 0;
            //        int numElementsPerIndex = 1;
            //        for (int whichIndex = numIndices - 1; whichIndex >= 0; --whichIndex)
            //        {
            //            index += numElementsPerIndex * indices[whichIndex];
            //            numElementsPerIndex *= dimensions[whichIndex];
            //        }
            //        return index;
            //    }
            //}
        }

        /// <summary>Calculates and stores the multidimensional indices of an element of the
        /// multidimensional table of training parameters & results.</summary>
        /// <param name="oneDimensionalIndex">One dimensional index that defines the position of the 
        /// element in the list of elements.</param>
        /// <param name="tableIndices">Variable where multidimensional indices of the element are stored.</param>
        public void GetIndices(int oneDimensionalIndex, ref int[] tableIndices)
        {
            Util.GetIndices(TrainingTableDimensions, oneDimensionalIndex, ref tableIndices);
            //List<int> dimensions = TrainingTableDimensions;
            //int numElements = 1;
            //int numDimensions = 0;
            //if (dimensions != null)
            //    numDimensions = dimensions.Count;
            //if (numDimensions == 0)
            //    tableIndices = null;
            //else
            //{
            //    if (tableIndices == null)
            //        tableIndices = new int[numDimensions];
            //    else if (tableIndices.Length != numDimensions)
            //        tableIndices = new int[numDimensions];
            //    for (int whichDimension = 0; whichDimension < numDimensions; ++whichDimension)
            //    {
            //        numElements*=dimensions[whichDimension];
            //    }
            //    int numElementsPerIndex = numElements;
            //    int numRemainingElements = numElements;
            //    for (int whichDimension = 0; whichDimension < numDimensions; ++whichDimension)
            //    {
            //        numElementsPerIndex /= dimensions[whichDimension];
            //        int currentIndex = numRemainingElements / numElementsPerIndex;
            //        numRemainingElements -= currentIndex * numElementsPerIndex;
            //        tableIndices[whichDimension] = currentIndex;
            //    }
            //}
        }

        #endregion TrainingResults


        #region StaticMethods

        // Saving / restoring training parameters and results to a JSON file:

        /// <summary>Saves (serializes) the specified training parameters object to the specified JSON file.
        /// File is owerwritten if it exists.</summary>
        /// <param name="trainingParameters">Object that is saved to a file.</param>
        /// <param name="inputFilePath">Path to the file into which object is is saved.</param>
        public static void SaveJson(NeuralTrainingTable trainingResults, string filePath)
        {
            SaveJson(trainingResults, filePath, false /* append */ );
        }

        /// <summary>Saves (serializes) the specified training parameters object to the specified JSON file.</summary>
        /// <param name="trainingParameters">Object that is saved to a file.</param>
        /// <param name="inputFilePath">Path to the file into which object is is saved.</param>
        /// <param name="append">Specifies whether serialized data is appended at the end of the file
        /// in the case that the file already exists.</param>
        public static void SaveJson(NeuralTrainingTable trainingResults, string filePath, bool append)
        {
            NeuralTrainingTableDto dtoOriginal = new NeuralTrainingTableDto();
            dtoOriginal.CopyFrom(trainingResults);
            ISerializer serializer = new SerializerJson();
            serializer.Serialize<NeuralTrainingTableDto>(dtoOriginal, filePath, append);
        }

        /// <summary>Restores (deserializes) a training parameters object from the specified file in JSON format.</summary>
        /// <param name="inputFilePath">File from which object data is restored.</param>
        /// <param name="trainingParametersRestored">Object that is restored by deserialization.</param>
        public static void LoadJson(string filePath, ref NeuralTrainingTable trainingResultsRestored)
        {
            ISerializer serializer = new SerializerJson();
            NeuralTrainingTableDto dtoRestored = serializer.DeserializeFile<NeuralTrainingTableDto>(filePath);
            dtoRestored.CopyTo(ref trainingResultsRestored);
        }

        #endregion StaticMethods

    }  // class NeuralTrainingTable


    public partial class NeuralTrainingParameters
    {
       
        private class ComparerErrorTrainingVector : IComparer<IVector>
        {
            int IComparer<IVector>.Compare(IVector a, IVector b)
            {
                if (a[0] > b[0])
                    return 1;

                if (a[0] < b[0])
                    return -1;

                else
                    return 0;
            }
        }

        protected IComparer<IVector> CreateComparerErrorTrainingRmsVector()
        {
            return (IComparer<IVector>)new ComparerErrorTrainingVector();
        }


        #region ComparerClasses

        /// <summary>Base comparer class (implementation of the <see cref="IComparer<NeuralTrainingParameters>"/>
        /// interface) for conmparing objects of type <see cref="NeuralTrainingParameters"/></summary>
        public class ComparerBase : IComparer<NeuralTrainingParameters>
        {

            protected IVector _outputScalingLengths;

            /// <summary>Vector of scaling lengths for calculation of weighted norms.</summary>
            public IVector OutputScalingLengths
            {
                get { return _outputScalingLengths; }
                set { _outputScalingLengths = value; }
            }

            protected double OutputNorm(IVector vec)
            {
                if (OutputScalingLengths == null)
                    return vec.NormEuclidean;
                else return Vector.NormWeightedPlain(vec, OutputScalingLengths);
            }


            protected bool _compareMinError = false;

            /// <summary>Whether the min error in convergence table is used for comparison when 
            /// errors are compared. If false then mean value of last errors is used.</summary>
            public bool CompareMinError
            {

                get { return _compareMinError; }
                set { _compareMinError = value; }
            }

            protected int _numLastErrors = 1;

            /// <summary> Number of last errors in convergence list for calculataing the mean value of error. 
            /// The default valeu is 1 which represent the last error in the converhence list. </summary>
            public int NumLastErrors
            {
                get { return _numLastErrors; }
                set 
                {
                    if (value == 0)
                        value = 1;
                    _numLastErrors = value; 
                }
            }

            protected int _numBundles = 0;

            /// <summary> Number of bundles where sorting of convergences stars. 
            /// The default valeu is 0 which represent normal sorting after the training is done. </summary>
            public int NumBundles
            {
                get { return _numBundles; }
                set
                {
                    _numBundles = value;
                }
            }

            protected bool _compareByTrainingRmsError = true;

            /// <summary>Whether training RMS errors from convergence tavble are compared.</summary>
            public bool CompareByTrainingRmsError
            {
                get { return _compareByTrainingRmsError; }
                set 
                {
                    _compareByTrainingRmsError = value; 
                    if (value)
                    {
                        CompareByTrainingMaxError = false;
                        CompareByVerificationRmsError = false;
                        CompareByVerificationMaxError = false;
                    }
                }
            }

           protected bool _compareByTrainingMaxError = false;

           /// <summary>Whether Maximal absolute training errors from convergence tavble are compared.</summary>
            public bool CompareByTrainingMaxError
            {
                get { return _compareByTrainingMaxError; }
                set 
                {
                    _compareByTrainingMaxError = value;
                    if (value)
                    {
                        CompareByTrainingRmsError = false;
                        CompareByVerificationRmsError = false;
                        CompareByVerificationMaxError = false;
                    } 
                }
            }

            protected bool _compareByVerificationRmsError = false;

            /// <summary>Whether verification RMS errors from convergence tavble are compared.</summary>
            public bool CompareByVerificationRmsError
            {
                get { return _compareByVerificationRmsError; }
                set 
                {
                    _compareByVerificationRmsError = value;
                    if (value)
                    {
                        CompareByTrainingRmsError = false;
                        CompareByTrainingMaxError = false;
                        CompareByVerificationMaxError = false;
                    }   
                }
            }

            protected bool _compareByVerificationMaxError = false;

            /// <summary>Whether Maximal absolute verification errors from convergence tavble are compared.</summary>
            public bool CompareByVerificationMaxError
            {
                get { return _compareByVerificationMaxError; }
                set 
                {
                    _compareByVerificationMaxError = value;
                    if (value)
                    {
                        CompareByTrainingRmsError = false;
                        CompareByTrainingMaxError = false;
                        CompareByVerificationRmsError = false;
                    }  
                }
            }
 

            int IComparer<NeuralTrainingParameters>.Compare(NeuralTrainingParameters a, NeuralTrainingParameters b)
            {
                if (a == null)
                {
                    if (b == null)
                        return 0;
                    else
                        return -1;
                } else
                {
                    if (CompareByTrainingRmsError)
                    {
                        List<IVector> errorsTrainingRmsLista = new List<IVector>();
                        errorsTrainingRmsLista = a.ErrorsTrainingRmsList;
                        List<IVector> errorsTrainingRmsListb = new List<IVector>();
                        errorsTrainingRmsListb = b.ErrorsTrainingRmsList;
                        int counta = errorsTrainingRmsLista.Count - 1;
                        int countb = errorsTrainingRmsListb.Count - 1;

                        if (errorsTrainingRmsLista == null)
                        {
                            if (errorsTrainingRmsListb == null)
                                return 0;
                            else
                                return -1;
                        }
                        else if (errorsTrainingRmsListb == null)
                        {
                            return 1;
                        }
                        else
                        {
                            if (CompareMinError)
                            {
                                // Sort Error rms Training List
                                ComparerErrorTrainingVector TrainingRmsList = new ComparerErrorTrainingVector();
                                errorsTrainingRmsLista.Sort(TrainingRmsList);
                                TrainingRmsList = new ComparerErrorTrainingVector();
                                errorsTrainingRmsListb.Sort(TrainingRmsList);

                                double errorTrainingRmsa = errorsTrainingRmsLista[counta][0];
                                double errorTrainingRmsb = errorsTrainingRmsListb[countb][0];

                                if (errorTrainingRmsa > errorTrainingRmsb)
                                    return 1;

                                    if (errorTrainingRmsa < errorTrainingRmsb)
                                        return -1;

                                    else
                                        return 0;
                            }
                            else
                            {                                
                                double errorTrainingRmsa = 0;
                                double errorTrainingRmsb = 0;

                                errorTrainingRmsa = ErrorAverageCalc(errorsTrainingRmsLista, NumLastErrors, NumBundles);
                                errorTrainingRmsb = ErrorAverageCalc(errorsTrainingRmsListb, NumLastErrors, NumBundles);

                                if (errorTrainingRmsa > errorTrainingRmsb)
                                    return 1;

                                if (errorTrainingRmsa < errorTrainingRmsb)
                                    return -1;

                                else
                                    return 0;
                            }
                        }
                    }
                    if (CompareByTrainingMaxError)
                    {
                        List<IVector> errorsTrainingMaxLista = new List<IVector>();
                        errorsTrainingMaxLista = a.ErrorsTrainingMaxList;
                        List<IVector> errorsTrainingMaxListb = new List<IVector>();
                        errorsTrainingMaxListb = b.ErrorsTrainingMaxList;
                        int counta = errorsTrainingMaxLista.Count - 1;
                        int countb = errorsTrainingMaxListb.Count - 1;

                        if (errorsTrainingMaxLista == null)
                        {
                            if (errorsTrainingMaxListb == null)
                                return 0;
                            else
                                return -1;
                        }
                        else if (errorsTrainingMaxListb == null)
                        {
                            return 1;
                        }
                        else
                        {
                            if (CompareMinError)
                            {
                                // Sort Error Max Training List
                                ComparerErrorTrainingVector TrainingMaxList = new ComparerErrorTrainingVector();
                                errorsTrainingMaxLista.Sort(TrainingMaxList);
                                TrainingMaxList = new ComparerErrorTrainingVector();
                                errorsTrainingMaxListb.Sort(TrainingMaxList);

                                double errorTrainingMaxa = errorsTrainingMaxLista[counta][0];
                                double errorTrainingMaxb = errorsTrainingMaxListb[countb][0];

                                if (errorTrainingMaxa > errorTrainingMaxb)
                                    return 1;

                                if (errorTrainingMaxa < errorTrainingMaxb)
                                    return -1;

                                else
                                    return 0;
                            }
                            else
                            {                             
                                double errorTrainingMaxa = 0;
                                double errorTrainingMaxb = 0;

                                errorTrainingMaxa = ErrorAverageCalc(errorsTrainingMaxLista, NumLastErrors, NumBundles);
                                errorTrainingMaxb = ErrorAverageCalc(errorsTrainingMaxListb, NumLastErrors, NumBundles);

                                if (errorTrainingMaxa > errorTrainingMaxb)
                                    return 1;

                                if (errorTrainingMaxa < errorTrainingMaxb)
                                    return -1;

                                else
                                    return 0;
                            }
                        }
                    }
                    if (CompareByVerificationRmsError)
                    {
                        List<IVector> errorsVerificationRmsLista = new List<IVector>();
                        errorsVerificationRmsLista = a.ErrorsVerificationRmsList;
                        List<IVector> errorsVerificationRmsListb = new List<IVector>();
                        errorsVerificationRmsListb = b.ErrorsVerificationRmsList;
                        int counta = errorsVerificationRmsLista.Count - 1;
                        int countb = errorsVerificationRmsListb.Count - 1;

                        if (errorsVerificationRmsLista == null)
                        {
                            if (errorsVerificationRmsListb == null)
                                return 0;
                            else
                                return -1;
                        }
                        else if (errorsVerificationRmsListb == null)
                        {
                            return 1;
                        }
                        else
                        {
                            if (CompareMinError)
                            {
                                // Sort Error Max Training List
                                ComparerErrorTrainingVector VerificationRmsList = new ComparerErrorTrainingVector();
                                errorsVerificationRmsLista.Sort(VerificationRmsList);
                                VerificationRmsList = new ComparerErrorTrainingVector();
                                errorsVerificationRmsListb.Sort(VerificationRmsList);

                                double errorVerificationRmsa = errorsVerificationRmsLista[counta][0];
                                double errorVerificationRmsb = errorsVerificationRmsListb[countb][0];

                                if (errorVerificationRmsa > errorVerificationRmsb)
                                    return 1;

                                if (errorVerificationRmsa < errorVerificationRmsb)
                                    return -1;

                                else
                                    return 0;

                            }
                            else
                            {
                                double errorVerificationRmsa = 0;
                                double errorVerificationRmsb = 0;

                                errorVerificationRmsa = ErrorAverageCalc(errorsVerificationRmsLista, NumLastErrors, NumBundles);
                                errorVerificationRmsb = ErrorAverageCalc(errorsVerificationRmsListb, NumLastErrors, NumBundles);

                                if (errorVerificationRmsa > errorVerificationRmsb)
                                    return 1;

                                if (errorVerificationRmsa < errorVerificationRmsb)
                                    return -1;

                                else
                                    return 0;
                            }
                        }
                    }
                    if (CompareByVerificationMaxError)
                    {
                        List<IVector> errorsVerificationMaxLista = new List<IVector>();
                        errorsVerificationMaxLista = a.ErrorsVerificationMaxList;
                        List<IVector> errorsVerificationMaxListb = new List<IVector>();
                        errorsVerificationMaxListb = b.ErrorsVerificationMaxList;
                        int counta = errorsVerificationMaxLista.Count - 1;
                        int countb = errorsVerificationMaxListb.Count - 1;

                        if (errorsVerificationMaxLista == null)
                        {
                            if (errorsVerificationMaxListb == null)
                                return 0;
                            else
                                return -1;
                        }
                        else if (errorsVerificationMaxListb == null)
                        {
                            return 1;
                        }
                        else
                        {
                            if (CompareMinError)
                            {
                                // Sort Error Max Verification List
                                ComparerErrorTrainingVector VerificationMaxList = new ComparerErrorTrainingVector();
                                errorsVerificationMaxLista.Sort(VerificationMaxList);
                                VerificationMaxList = new ComparerErrorTrainingVector();
                                errorsVerificationMaxListb.Sort(VerificationMaxList);

                                double errorVerificationMaxa = errorsVerificationMaxLista[counta][0];
                                double errorVerificationMaxb = errorsVerificationMaxListb[countb][0];

                                if (errorVerificationMaxa > errorVerificationMaxb)
                                    return 1;

                                if (errorVerificationMaxa < errorVerificationMaxb)
                                    return -1;

                                else
                                    return 0;
                            }
                            else
                            {
                                double errorVerificationMaxa = 0;
                                double errorVerificationMaxb = 0;

                                errorVerificationMaxa = ErrorAverageCalc(errorsVerificationMaxLista, NumLastErrors, NumBundles);
                                errorVerificationMaxb = ErrorAverageCalc(errorsVerificationMaxListb, NumLastErrors, NumBundles);

                                if (errorVerificationMaxa > errorVerificationMaxb)
                                    return 1;

                                if (errorVerificationMaxa < errorVerificationMaxb)
                                    return -1;

                                else
                                    return 0;
                            }
                        }
                    }

                }
                throw new InvalidOperationException("No defined conditions were hit to define the sorting order of two objects.");
            }

        }

        public class ComparerTrainingTime : IComparer<NeuralTrainingParameters>
        {

            int IComparer<NeuralTrainingParameters>.Compare(NeuralTrainingParameters a, NeuralTrainingParameters b)
            {
                if (a == null)
                {
                    if (b == null)
                        return 0;
                    else
                        return -1;
                }
                else if (b == null)
                {
                    return 1;
                }
                else
                {
                    if (a.TrainingTime == null)
                    {
                        if (b.TrainingTime == null)
                            return 0;
                        else
                            return -1;
                    }
                    else if (b.TrainingTime == null)
                    {
                        return 1;
                    }
                    else
                    {
                        if (a.TrainingTime > b.TrainingTime)
                            return 1;
                        if (a.TrainingTime < b.TrainingTime)
                            return -1;
                        else
                            return 0;
                    }
                }
            }
        }

        #endregion ComparerClasses


        #region ComparerCreation

        public static ComparerTrainingTime CreateComparerTrainingTime()
        {
            return new ComparerTrainingTime();
        }



        #endregion comparerCreation


    } // partial class NeuralTrainingParameters


    /// <summary>Class for testing of Comparers for NeuralTrainingParameters.</summary>
    [Obsolete("Just for testing, remove this class later!")]
    public class TestTrainingParametersComparers
    {


        public void Test()
        {

            // Concaptual demonstration uf use of comparers:

            NeuralTrainingTable tab = new NeuralTrainingTable();
            List<NeuralTrainingParameters> list = tab.TrainingParameters;

            NeuralTrainingParameters.ComparerBase comparer = new NeuralTrainingParameters.ComparerBase();
            comparer.CompareByTrainingRmsError = true;
            comparer.CompareByTrainingMaxError = false;
            // ...

        }


    }


}


