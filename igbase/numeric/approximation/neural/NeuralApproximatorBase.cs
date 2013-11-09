// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using IG.Lib;


namespace IG.Num
{

    /// <summary>Approximator of response by using neural networks.
    /// $A Igor Mar11;</summary>
    public interface INeuralApproximator : IVectorApproximator, ILockable
    {

        #region Data
        
        /// <summary>Level of output printed to console when performing actions.</summary>
        int OutputLevel
        { get; set; } 

        /// <summary>Flag indicating whether multiple neural networks are used to approximate
        /// multiple outputs (one network for each output)</summary>
        bool MultipleNetworks
        { get; set; }

        /// <summary>Gets or sets the number of input neurons.</summary>
        int InputLength
        { get; set; }

        /// <summary>Gets or sets the number of output neurons.</summary>
        int OutputLength
        { get; set; }

        /// <summary>Sets the numbers of neurons in each hidden layer.
        /// Numbers are set by an array of integers in which each element contains the number of neurons in
        /// the corresponding hidden layer (indexed from 0). 
        /// Hidden layers are those not containing input or output neurons.</summary>
        /// <remarks>Getter is protected cecause we don't want users of the class to set number of neurons in individual layers.</remarks>
        int[] NumHiddenNeurons
        { get; set; }


        /// <summary>Gets or sets the number of hidden layers of the neural network 
        /// (these are layers that don't contain input or output neurons).</summary>
        int NumHiddenLayers
        { get; set; }

        /// <summary>Returns the number of neurons in the specified hidden layer.
        /// Hidden layers are those not containing input or output neurons.</summary>
        /// <param name="whichLayer">Index of the hidden layer for which number of neurons is returned.</param>
        int GetNumNeuronsInHiddenLayer(int whichLayer);

        /// <summary>Sets the number of neurons in the specified hidden layer.
        /// Hidden layers are those not containing input or output neurons.</summary>
        /// <param name="whichLayer">Index of the hidden layer for which number of neurons is returned.</param>
        /// <param name="numNeurons">Prescribed number of neurons in the specified layer.</param>
        void SetNumNeuronsInHiddenLayer(int whichLayer, int numNeurons);

        /// <summary>Sets the numbers of neurons in each hidden layer. Can be called with table of integers as 
        /// argument, kor with variable number of integer parameters.
        /// Numbers are set by an array of integers in which each element contains the number of neurons in
        /// the corresponding hidden layer (indexed from 0). 
        /// Hidden layers are those not containing input or output neurons.</summary>
        /// <param name="numNeurons">Array containing the prescribed numbers of neurons in each hidden layer.
        /// Instead of array, a variable number of integer parameters can be specified.</param>
        void SetHiddenLayers(params int[] numNeurons);

        #endregion Data


        #region Operation 

        /// <summary>Gets a flag telling whether the network is prepared for operation (training and calculation of output).</summary>
        bool NetworkPrepared
        { get; }

        /// <summary>Alpha value specifying the shape of the activation function.</summary>
        double SigmoidAlphaValue
        { get; set; }

        /// <summary>Gets or sets learning rate.</summary>
        double LearningRate
        { get; set; }

        /// <summary>Gets or sets momentum.</summary>
        double Momentum
        { get; set; }

        /// <summary>Number of learning epochs performed up to the current moment.</summary>
        int EpochCount
        { get; }

        /// <summary>Maximal number of epochs in training.</summary>
        int MaxEpochs
        { get; set; }

        /// <summary>Number of epochs in a single training bundle.
        /// This number of epochs is performed at once when training, without checking 
        /// convergence criteria between. Larger value means slightly more efficient training 
        /// (because of less checks) but rougher criteria checks.</summary>
        int EpochsInBundle
        { get; set; }

        /// <summary>Tolerance over RMS error of output over training points.
        /// Training will continue until error becomes below tolerance or until maximal number of epochs is reached.
        /// If less or equal than 0 then this tolerance is not taken into account.</summary>
        IVector ToleranceRms
        { get; set; }

        /// <summary>Tolerance on maximal error of output over training points.
        /// Training will continue until error becomes below tolerance or until maximal number of epochs is reached.
        /// If less or equal than 0 then this tolerance is not taken into account.</summary>
        IVector ToleranceMax
        { get; set; }

        /// <summary>Relative tolerances on RMS errors of outputs over training points, relative to the 
        /// correspoinding ranges of output data.</summary>
        /// <remarks><para>When this vector is set to a non-null value, elements of RMS tolerances vector
        /// (property <see cref="ToleranceRms"/>) are set to the specified fractions of the ranges 
        /// of the corresponding output values in training parameters (defined by the property 
        /// <see cref="OutputDataBounds"/>) if these are defined (otherwise this may happen when the
        /// property gets defined).</para>
        /// <para>Whenever this property is defined, re-setting the property <see cref="OutputDataBounds"/> will
        /// cause recalculation of <see cref="ToleranceRms"/> according to elements of this property and the
        /// ranges of corresponding output values.</para>
        /// <para>Setting this property undefines the <see cref="ToleranceRmsRelativeToRangeScalar"/> property,
        /// even if it is set to null.</para>
        /// <para>If the scalar relative tolerance is set (property <see cref="ToleranceRmsRelativeToRangeScalar"/>)
        /// and this property is not set yet, the getter of this property will evaluate to a vector whose
        /// elements are equal to the scalar relative tolerance.</para></remarks>
        IVector ToleranceRmsRelativeToRange
        { get; set; }

        
        /// <summary>Scalar through which all components of the Relative tolerances on RMS errors of outputs
        /// can be set to the same value.</summary>
        /// <remarks><para>Getter returns a value greater than 0 only if the property has been previously set.</para>
        /// <para>Setting a vector of relative tolerances (property <see cref="ToleranceRmsRelativeToRange"/>) sets 
        /// this property to 0, even it it is set to null.</para>
        /// <para>Setting the property to a value greater than 0 automatically sets the vector of relative tolerances 
        /// (property <see cref="ToleranceRmsRelativeToRange"/>) in such a way that all elements are equal to this property.</para></remarks>
        double ToleranceRmsRelativeToRangeScalar
        { get; set; }
        
        /// <summary>Relative tolerances on max. abs. errors of outputs over training points, relative to the 
        /// correspoinding ranges of output data.</summary>
        /// <remarks><para>When this vector is set to a non-null value, elements of max. abs. tolerances vector
        /// (property <see cref="ToleranceMax"/>) are set to the specified fractions of the ranges 
        /// of the corresponding output values in training parameters (defined by the property 
        /// <see cref="OutputDataBounds"/>) if these are defined (otherwise this may happen when the
        /// property gets defined).</para>
        /// <para>Whenever this property is defined, re-setting the property <see cref="OutputDataBounds"/> will
        /// cause recalculation of <see cref="ToleranceMax"/> according to elements of this property and the
        /// ranges of corresponding output values.</para>
        /// <para>Setting this property undefines the <see cref="ToleranceMaxRelativeToRangeScalar"/> property,
        /// even if it is set to null.</para>
        /// If the scalar relative tolerance is set (property <see cref="ToleranceMaxRelativeToRangeScalar"/>)
        /// and this property is not set yet, the getter of this property will evaluate to a vector whose
        /// elements are equal to the scalar relative tolerance.</remarks>
        IVector ToleranceMaxRelativeToRange
        { get; set; }
 
        
        /// <summary>Scalar through which all components of the Relative tolerances on max. abs. errors of outputs
        /// can be set to the same value.</summary>
        /// <remarks><para>Getter returns a value greater than 0 only if the property has been previously set.</para>
        /// <para>Setting a vector of relative tolerances (property <see cref="ToleranceMaxRelativeToRange"/>) sets 
        /// this property to 0, even it it is set to null.</para>
        /// <para>Setting the property to a value greater than 0 automatically sets the vector of relative tolerances 
        /// (property <see cref="ToleranceMaxRelativeToRange"/>) in such a way that all elements are equal to this property.</para></remarks>
        double ToleranceMaxRelativeToRangeScalar
        { get; set; }

        /// <summary> Flag to enable Rms error convergence colection. 
        /// Default is false. </summary>
        bool SaveConvergenceRms
        { get; set; }

        ///// <summary> RMS error convergence list. 
        ///// Saved after every set of epochs. </summary>
        //List<IVector> ConvergenceRmsList
        //{ get; set; }

        /// <summary> List of epoch numbers at which convergence data was sampled. 
        /// Saved after every set of epochs. </summary>
        /// $A Tako78 Sep12;
        List<int> EpochNumbers
        { get; set; }

        /// <summary> Convergence List of Rms errors calculated on training data. 
        /// Saved after every set of epochs. </summary>
        List<IVector> ConvergenceErrorsTrainingRmsList
        { get; set; }

        /// <summary> Convergence List of Maximum errors calculated on training data. 
        /// Saved after every set of epochs. </summary>
        List<IVector> ConvergenceErrorsTrainingMaxList
        { get; set; }

        /// <summary> Convergence List of Rms errors calculated on verification data. 
        /// Saved after every set of epochs. </summary>
        List<IVector> ConvergenceErrorsVerificationRmsList
        { get; set; }

        /// <summary> Convergence List of Maximum errors calculated on verification data. 
        /// Saved after every set of epochs. </summary>
        List<IVector> ConvergenceErrorsVerificationMaxList
        { get; set; }

        /// <summary>Prepares neural network for use.
        /// If networks have not yet been created according to internal data, they are created.
        /// If networks are already prepared then this method does nothing.</summary>
        /// <remarks>Some things suc as creation of a neural network follow the pattern of lazy evaluation.</remarks>
        void PrepareNetwork();

        /// <summary>Creates the neural network anew. If the network already exists on the current object, 
        /// it is discarded.</summary>
        void CreateNetwork();

        /// <summary>Resets the neural network.</summary>
        void ResetNetwork();

        /// <summary>Destroys the neural network.</summary>
        void DestroyNetwork();

        /// <summary>Returns an absolute path to the file for storing the specified neural network contained 
        /// on the current object, with respect to suggested file path and index of the network.
        /// The returned path is in the same directory as suggested file path and has the same file extension (if any).
        /// If the suggested path represents a directory, then some default suggested path is assumed.
        /// If there is only one network then the returned file path is the same as the suggested one
        /// (or the same as default file name within the suggested directory, if a directory path is proposed).</summary>
        /// <param name="fileOrDirectoryPath">Suggested file or directory path, can be a relative path.</param>
        /// <param name="whichNetwork">Index of the network for which path of the file to store the network is returned.</param>
        /// <returns></returns>
        string GetNetworkFilePath(string fileOrDirectoryPath, int whichNetwork);

        /// <summary>Gets string representation of type of the current object. This is used e.g. in deserialization in order to prevent
        /// that wrong type of internal representation would be read in.</summary>
        string NeuralApproximatorType
        { get; }

        /// <summary>Path where the curren network state has been saved, or null if the current state has not been saved yet.
        /// The SaveFile  methods takes care that the file path is stored when network state is saved.
        /// InvalidateTrainingDependencies() takes care that this file path is set to null if network state has changed after last save.</summary>
        string NetworkStateFilePath
        { get; }
        
        /// <summary>Relative path where the curren network state has been saved. Auxiliary property used in 
        /// deserialization.
        /// When the whole Neural network approximator is saved to a file, tis path is updated in such a way
        /// that it points to the fiele where the network state has been saved, but relative to the path where
        /// the whole approximator is saved. This enables restore of the saved network state at a later time, 
        /// even if the containing directory has moved within the file system or has even been copied to another system.</summary>
        string NetworkStateRelativePath
        { get; set; }

        /// <summary>Saves the state of the neural network to the specified file.
        /// If the file already exists, its contents are overwritten.</summary>
        /// <param name="filePath">Path to the file into which the network is saved.</param>
        void SaveNetwork(string filePath);

        /// <summary>Restores neural network from a file where it has been stored before.</summary>
        /// <param name="filePath">Path to the file from which the neural network is read.</param>
        void LoadNetwork(string filePath);


        #endregion Operation


        #region Training

        
        /// <summary>Gets or sets the training data.</summary>
        SampledDataSet TrainingData
        { get; set; }

        /// <summary>Saves network's training data to the specified JSON file.
        /// File is owerwritten if it exists.</summary>
        /// <param name="filePath">Path to the file where training data is saved.</param>
        void SaveTrainingDataJson(string filePath);

        /// <summary>Restores training data from the specified file in JSON format.</summary>
        /// <param name="filePath">File from which training data is restored.</param>
        void LoadTrainingDataJson(string filePath);

        /// <summary>Gets or sets indices of training data elements that are used for verification of
        /// how precise appeoximation is. These elements are excluded from training of neural network.</summary>
        /// <remarks>Object of type IndexList that contains indices of elements used for verification,
        /// always contains a sorted list of unique indices.</remarks>
        IndexList VerificationIndices
        { get; set; }
        
        /// <summary>Bounds on input data, used for scaling from actual input to input used by neural network.
        /// Scaling is performed because of the bound codomain and image of activation functions.</summary>
        IBoundingBox InputDataBounds
        { get; set; }

        
        /// <summary>Bounds on output data, used for scaling from actual output to output produced by neural network.
        /// Scaling is performed because of the bound codomain and image of activation functions.</summary>
        IBoundingBox OutputDataBounds
        { get; set; }
        
        /// <summary>Safety factor by which interval lenghts of input data bounds are enlarged after
        /// bounds are automatically determined from the range of input data in the training set.
        /// Setter re-calculated the input data bounds and therefore invalidates training data dependencies.</summary>
        double InputBoundsSafetyFactor
        { get; set; }
        
        /// <summary>Safety factor by which interval lenghts of output data bounds are enlarged after
        /// bounds are automatically determined from the range of output data in the training set.
        /// Setter re-calculated the output data bounds and therefore invalidates training data dependencies.</summary>
        double OutputBoundsSafetyFactor
        { get; set; }

        
        /// <summary>Gets the range in which data should be for input neurons, used for scaling 
        /// from actual input to input used by neural network.
        /// This depends on the activation function.</summary>
        /// <remarks>Setter is not public.</remarks>
        IBoundingBox InputNeuronsRange
        { get; }
        
        /// <summary>Gets the range of the data output from output neurons, used for scaling 
        /// from actual output to output produced by neural network.
        /// This will normally depend on the activation function.</summary>
        /// <remarks>Setter is not public.</remarks>
        IBoundingBox OutputNeuronsRange
        { get; }

        
        /// <summary>Sets the neurons input range. Bounds for all input neurons are set equally.</summary>
        /// <param name="min">Lower bound for all input neurons.</param>
        /// <param name="max">Upper bound for all input neurons.</param>
        void SetNeuronsInputRange(double min, double max);
        
        /// <summary>Sets the neurons output range. Bounds for all output neurons are set equally.</summary>
        /// <param name="min">Lower bound for all output neurons.</param>
        /// <param name="max">Upper bound for all output neurons.</param>
        void SetNeuronsOutputRange(double min, double max);
        
        /// <summary>Recalculates input data bounds by taking into account the training data set of the current object.</summary>
        /// <remarks>Training data dependencies are invalidated by this method because setting bounds invalidates them.</remarks>
        void RecalculateInputDataBounds();

        /// <summary>Recalculates output data bounds by taking into account the training data set of the current object.</summary>
        /// <remarks>Training data dependencies are invalidated by this method because setting bounds invalidates them.</remarks>
        void RecalculateOutputDataBounds();
        
        /// <summary>Recalculates input and output data bounds by taking into account the training data set of the current object.</summary>
        /// <remarks>Training data dependencies are invalidated by this method because setting bounds invalidates them.</remarks>
         void RecalculateDataBounds();
        
        /// <summary>Recalculates input data bounds by taking into account the specified training data set.</summary>
        /// <param name="trainingData">Training data set accourding to which input bounds are adjusted.</param>
        /// <remarks>Training data dependencies are invalidated by this method because setting bounds invalidates them.</remarks>
        void RecalculateInputDataBounds(SampledDataSet trainingData);
        
        /// <summary>Recalculates output data bounds by taking into account the specified training data set.</summary>
        /// <param name="trainingData">Training data set accourding to which output bounds are adjusted.</param>
        /// <remarks>Training data dependencies are invalidated by this method because setting bounds invalidates them.</remarks>
        void RecalculateOutputDataBounds(SampledDataSet trainingData);
        
        /// <summary>Recalculates input and output data bounds by taking into account the specified training data set.</summary>
        /// <param name="trainingData">Training data set accourding to which input and output bounds are adjusted.</param>
        /// <remarks>Training data dependencies are invalidated by this method because setting bounds invalidates them.</remarks>
        void RecalculateDataBounds(SampledDataSet trainingData);

        /// <summary>Gets number of all training points, including verification points.</summary>
        int NumAllTrainingPoints
        { get; }

        /// <summary>Gets number of training points (this excludes verification points).</summary>
        int NumTrainingPoints
        { get; }

        /// <summary>Gets number of verification points.</summary>
        int NumVerificationPoints
        { get; }

        /// <summary>Calculates the RMS (root mean square) of the errors of output values for the training 
        /// elements of the training set (this excludes verification points).</summary>
        /// <param name="errors"></param>
        void GetErrorsTrainingRms(ref IVector errors);

        /// <summary>Calculates the RMS (root mean square) of the errors of output values for the verification 
        /// elements of the training set.</summary>
        /// <param name="errors"></param>
        void GetErrorsVerificationRms(ref IVector errors);
        
        /// <summary>Calculates the maximum absolute errors of output values for the training 
        /// elements of the training set (this excludes verification points).</summary>
        /// <param name="errors"></param>
        void GetErrorsTrainingMax(ref IVector errors);

        /// <summary>Calculates the maximum absolute errors of output values for the verification 
        /// elements of the training set.</summary>
        /// <param name="errors"></param>
        void GetErrorsVerificationMax(ref IVector errors);
            
        /// <summary>Calculates the mean absolute errors of output values for the training 
        /// elements of the training set (this excludes verification points).</summary>
        /// <param name="errors"></param>
        void GetErrorsTrainingMeanAbs(ref IVector errors);
            
        /// <summary>Calculates the mean absolute errors of output values for the verification 
        /// elements of the training set.</summary>
        /// <param name="errors"></param>
        void GetErrorsVerificationMeanAbs(ref IVector errors);
        
        /// <summary>Whether network has been trained since the training data was set.</summary>
        bool NetworkTrained
        { get; }

        /// <summary>Invalidates all data that must be recalculated after training of the network is done.
        /// This method is called after training or additional training of the network is performed.
        /// Invalidation is achieved throughthe the appropriate flags.</summary>
        void InvalidateTrainingDependencies();
            
        /// <summary>Invalidates all data that must be re-calculated after training data changes.
        /// This method is called after training data is modified.
        /// Invalidation is achieved throughthe the appropriate flags.</summary>
        void InvalidateTrainingDataDependencies();
            
        /// <summary>Invalidates all data that must be re-calculated after the neural network itself changes.
        /// This method must be called after the internal neural network is re-defined (or are re-defined).
        /// Invalidation is achieved throughthe the appropriate flags.</summary>
        void InvalidateNetworkDependencies();
            
        /// <summary>Flags that signalizes (if true) that training should be broken on external request.</summary>
        bool BreakTraining
        { get; set; }

        /// <summary>Trains neural network wiht the specified data, performing the specified number of epochs.</summary>
        /// <param name="numEpochs">Number of epochs used in training of the network.</param>
        void TrainNetwork(int numEpochs);
        
        /// <summary>Trains neural network until stopping criteria are met (in terms of errors and 
        /// number of epochs performed.</summary>
        void TrainNetwork();

        /// <summary>Returns true if the stopping criteria for training is met, with respect to current settings,
        /// errors and number of epochs already performed, and false otherwise.</summary>
        bool StopTrainingCriteriaMet();


        #endregion Training


        #region Calculation 

            
        /// <summary>Calculates and returns the approximated outputs corresponding to the specified inputs,
        /// by using the current neural network.</summary>
        /// <param name="input">Input parameters.</param>
        /// <returns>Vector of output values generated by the trained neural network.</returns>
        /// <remarks>Currently, only all outputs at once can be calculated. This makes no difference
        /// in the arrangement with a single network with multiple outputs, but does when several 
        /// networks with single output each are used. If the implementation changes in the future
        /// then performance configuratins should be taken into account carefully, and tracking input
        /// for which input parameters the outputs have been calculated might be necessary.</remarks>
        void CalculateOutput(IVector input, ref IVector output);
        
        /// <summary>Calculates and returns the specified output by using the neural network.</summary>
        double CalculateOutput(IVector input, int whichElement);

        /// <summary>Calculates and returns the required output values corresponding to the specified inputs,
        /// by using the current neural network(s).</summary>
        /// <param name="input">Input parameters for which output values are calculated.</param>
        /// <param name="indices">Array of indices of the output values to be returned.</param>
        /// <param name="filteredOutput">Vector where filtered output values are stored.</param>
        void CalculateOutput(IVector input, int[] indices, ref IVector filteredOutput);


        #endregion Calculation

    }  // INeuralApproximator



    /// <summary>Base class for neural network approximators.</summary>
    /// $A Igor Mar11;
    public abstract class NeuralApproximatorBase : VectorApproximatorBase, 
        INeuralApproximator, ILockable
    {

        public NeuralApproximatorBase(): base()
        { }


        #region ThreadLocking

        private object _mainLock = new object();

        /// <summary>This object's central lock object to be used by other object.
        /// Do not use this object for locking in class' methods, for this you should use 
        /// InternalLock.</summary>
        public object Lock { get { return _mainLock; } }

        #endregion ThreadLocking

        #region Data

        private int _outputLevel = Util.OutputLevel;

        private int _inputLength;
        private int _outputLength;
        private int[] _numHiddenNeurons;
        private bool _multipleNetworks = true;

        private bool _networkPrepared = false;
        private double _sigmoidAlphaValue = NeuralTrainingParameters.DefaultSigmoidAlphaValue;
        private double _learningRate = NeuralTrainingParameters.DefaultLearningRate;
        private double _momentum = NeuralTrainingParameters.DefaultMomentum;

        private int _epochCount = 0;

        private bool _convergenceRmsEnabled = false;

        // Stopping criteria for training:

        private int _maxEpochs = NeuralTrainingParameters.DefaultMaxEpochs;

        private int _epochsInBundle = NeuralTrainingParameters.DefaultEpochsInBundle;
        
        // private List<IVector> _convergenceRmsList = null;

        private List<int> _epochNumbers = null;
        private List<IVector> _convergenceErrorsTrainingRmsList = null;
        private List<IVector> _convergenceErrorsTrainingMaxList = null;
        private List<IVector> _convergenceErrorsVerificationRmsList = null;
        private List<IVector> _convergenceErrorsVerificationMaxList = null;

        /// <summary>Level of output printed to console when performing actions.</summary>
        public int OutputLevel
        { get { return _outputLevel; } set { _outputLevel = value; } }

        /// <summary>Flag indicating whether multiple neural networks are used to approximate
        /// multiple outputs (one network for each output)</summary>
        public virtual bool MultipleNetworks
        {
            get { lock (Lock) { return _multipleNetworks; } }
            set 
            {
                lock (Lock)
                {
                    if (value != _multipleNetworks)
                        DestroyNetwork();
                    _multipleNetworks = value;
                }
            }
        }

        /// <summary>Gets or sets the number of input neurons.</summary>
        public override int InputLength
        {
            get { lock(Lock) { return _inputLength; } }
            set {
                lock(Lock)
                {
                    if (value != _inputLength)
                    {
                        InvalidateNetworkDependencies();
                        DestroyNetwork();
                        _inputLength = value;
                        SetNeuronsInputRange(_defaultNeuronMinInput, _defaultNeuronMaxInput);
                    }
                }
            }
        }

        /// <summary>Gets or sets the number of output neurons.</summary>
        public override int OutputLength
        {
            get { lock(Lock) { return _outputLength; } }
            set {
                lock(Lock)
                {
                    if (value != _outputLength)
                    {
                        InvalidateNetworkDependencies();
                        DestroyNetwork();
                        _outputLength = value;
                        SetNeuronsOutputRange(_defaultNeuronMinOutput, _defaultNeuronMaxOutput);
                        if (ToleranceRms != null)
                        {
                            if (ToleranceRms.Length != value)
                                ToleranceRms = null;
                        }
                        if (ToleranceMax != null)
                        {
                            if (ToleranceMax.Length != value)
                                ToleranceMax = null;
                        }
                        if (ToleranceRmsRelativeToRange != null)
                        {
                            if (ToleranceRmsRelativeToRange.Length != value)
                                ToleranceRmsRelativeToRange = null;
                        }
                        if (ToleranceMaxRelativeToRange != null)
                        {
                            if (ToleranceMaxRelativeToRange.Length != value)
                                ToleranceMaxRelativeToRange = null;
                        }

                    }
                }
            }
        }

        /// <summary>Gets or sets the numbers of neurons in each hidden layer.
        /// When setting, contents of array are copied, not only a reference.
        /// Numbers are set by an array of integers in which each element contains the number of neurons in
        /// the corresponding hidden layer (indexed from 0). 
        /// Hidden layers are those not containing input or output neurons.</summary>
        /// <remarks>Getter is protected cecause we don't want users of the class to set number of neurons in individual layers.</remarks>
        public virtual int[] NumHiddenNeurons
        {
            get { lock(Lock) { return _numHiddenNeurons; } }
            set 
            {
                lock(Lock)
                {
                    bool isDifferent = false;
                    if (value==null)
                        isDifferent = true;
                    else if (_numHiddenNeurons==null)
                        isDifferent = true;
                    else if (value.Length!=_numHiddenNeurons.Length)
                        isDifferent = true;
                    else
                    {
                        for (int i=0; i<value.Length; ++i)
                        {
                            if (value[i]!=_numHiddenNeurons[i])
                                isDifferent = true;
                        }
                    }
                    if (isDifferent)
                    {
                        DestroyNetwork();
                        //_numHiddenNeurons = value; 
                        if (value == null)
                            _numHiddenNeurons = value;
                        else
                        {
                            if (_numHiddenNeurons==null)
                                _numHiddenNeurons = new int[value.Length];
                            else
                            if (_numHiddenNeurons.Length != value.Length)
                                _numHiddenNeurons = new int[value.Length];
                            for (int i = 0; i < value.Length; ++i)
                                _numHiddenNeurons[i] = value[i];
                        }
                    }
                }
            }
        }

        /// <summary>Gets or sets the number of hidden layers of the neural network 
        /// (these are layers that don't contain input or output neurons).</summary>
        public virtual int NumHiddenLayers
        {
            get 
            {
                lock (Lock)
                {
                    if (NumHiddenNeurons == null)
                        return 0;
                    else
                        return NumHiddenNeurons.Length;
                }
            }
            set
            {
                lock (Lock)
                {
                    if (value <= 0)
                    {
                        NumHiddenNeurons = null;
                    }
                    else
                    {
                        if (NumHiddenNeurons == null)
                            NumHiddenNeurons = new int[value];
                        else if (_numHiddenNeurons.Length != value)
                        {
                            int[] numNeurons = new int[value];
                            for (int i = 0; i < value; ++i)
                                numNeurons[i] = NeuralTrainingParameters.DefaultNumHiddenNeurons;
                            NumHiddenNeurons = numNeurons;
                        }
                    }
                }
            }
        }

        /// <summary>Returns the number of neurons in the specified hidden layer.
        /// Hidden layers are those not containing input or output neurons.</summary>
        /// <param name="whichLayer">Index of the hidden layer for which number of neurons is returned.</param>
        public virtual int GetNumNeuronsInHiddenLayer(int whichLayer)
        {
            lock (Lock)
            {
                int[] layers = NumHiddenNeurons;
                if (layers==null)
                    throw new InvalidOperationException("Can not get the number of neurons in hidden layer No. " 
                        + whichLayer + ": there are no hidden layers!");
                if (whichLayer < 0 || whichLayer >= layers.Length)
                    throw new ArgumentOutOfRangeException("Can not get the number of neurons in hidden layer No. "
                        + whichLayer + ": index should be between 0 and " + (layers.Length-1) + ".");
                return layers[whichLayer];
            }
        }

        /// <summary>Sets the number of neurons in the specified hidden layer.
        /// Hidden layers are those not containing input or output neurons.</summary>
        /// <param name="whichLayer">Index of the hidden layer for which number of neurons is returned.</param>
        /// <param name="numNeurons">Prescribed number of neurons in the specified layer.</param>
        public virtual void SetNumNeuronsInHiddenLayer(int whichLayer, int numNeurons)
        {
            lock (Lock)
            {
                int[] layers = NumHiddenNeurons;
                if (layers==null)
                    throw new InvalidOperationException("Can not set the number of neurons in hidden layer No. " 
                        + whichLayer + ": there are no hidden layers!");
                if (whichLayer < 0 || whichLayer >= layers.Length)
                    throw new ArgumentOutOfRangeException("Can not det the number of neurons in hidden layer No. "
                        + whichLayer + ": index should be between 0 and " + (layers.Length-1) + ".");
                layers[whichLayer] = numNeurons;
            }
        }

        /// <summary>Sets the numbers of neurons in each hidden layer. Can be called with table of integers as 
        /// argument, kor with variable number of integer parameters.
        /// Numbers are set by an array of integers in which each element contains the number of neurons in
        /// the corresponding hidden layer (indexed from 0). 
        /// Hidden layers are those not containing input or output neurons.</summary>
        /// <param name="numNeurons">Array containing the prescribed numbers of neurons in each hidden layer.
        /// Instead of array, a variable number of integer parameters can be specified.</param>
        public virtual void SetHiddenLayers(params int[] numNeurons)
        { NumHiddenNeurons = numNeurons; }



        #endregion Data

        #region Operation

        /// <summary>Gets a flag telling whether the network is prepared for operation (training and calculation of output).</summary>
        public bool NetworkPrepared
        {
            get { lock (Lock) { return _networkPrepared; } }
            protected set { lock (Lock) { _networkPrepared = value; } }
        }

        /// <summary>Alpha value specifying the shape of the activation function.</summary>
        public double SigmoidAlphaValue
        {
            get { lock (Lock) { return _sigmoidAlphaValue; } }
            set
            {
                if (value != _sigmoidAlphaValue)
                    DestroyNetwork();
                _sigmoidAlphaValue = value;
            }
        }

        /// <summary>Gets or sets learning rate.</summary>
        public double LearningRate
        {
            get { lock (Lock) { return _learningRate; } }
            set { lock (Lock) { _learningRate = value; } }
        }

        /// <summary>Gets or sets momentum.</summary>
        public double Momentum
        {
            get { lock (Lock) { return _momentum; } }
            set { lock (Lock) { _momentum = value; } }
        }

        /// <summary>Number of learning epochs performed up to the current moment.</summary>
        public int EpochCount
        {
            get { lock (Lock) { return _epochCount; } }
            protected set { lock (Lock) { _epochCount = value; } }
        }

        /// <summary>Maximal number of epochs in training.</summary>
        public int MaxEpochs
        {
            get { return _maxEpochs; }
            set { lock (Lock) { _maxEpochs = value; } }
        }

        /// <summary>Number of epochs in a single training bundle.
        /// This number of epochs is performed at once when training, without checking 
        /// convergence criteria between. Larger value means slightly more efficient training 
        /// (because of less checks) but rougher criteria checks.</summary>
        public int EpochsInBundle
        {
            get { return _epochsInBundle; }
            set { lock (Lock) { _epochsInBundle = value; } }
        }




        private IVector _toleranceRms = null;

        /// <summary>Tolerances on RMS errors of outputs over training points.
        /// Training will continue until error becomes below tolerance or until maximal number of epochs is reached.
        /// If less or equal than 0 then this tolerance is not taken into account.</summary>
        public IVector ToleranceRms
        {
            get {
                if (_toleranceRms == null)
                {
                    IVector tolRel = ToleranceRmsRelativeToRange;  // this will set relative tolerances if defined, and consequently absolute tolerances
                }
                return _toleranceRms; 
            }
            set
            {
                lock (Lock)
                {
                    if (value != null)
                    {
                        if (value.Length != OutputLength)
                            throw new ArgumentException("Dimension of the specified vector of tolerances on RMS values " + value.Length
                                +" is different than the number of outputs " + OutputLength + ".");
                        ToleranceRmsRelativeToRange = null; // if tolerances are manually set, relative tol. are re-set!
                    }
                    _toleranceRms = value;
                }
            }
        }

        private IVector _toleranceMax = null;

        /// <summary>Tolerances on maximal errors of outputs over training points.
        /// Training will continue until error becomes below tolerance or until maximal number of epochs is reached.
        /// If less or equal than 0 then this tolerance is not taken into account.</summary>
        public IVector ToleranceMax
        {
            get {
                if (_toleranceMax == null)
                {
                    IVector tolRel = ToleranceMaxRelativeToRange;  // this will set relative tolerances if defined, and consequently absolute tolerances
                }
                return _toleranceMax;
            }
            set 
            { 
                lock (Lock) 
                {
                    if (value != null)
                    {
                        if (value.Length != OutputLength)
                            throw new ArgumentException("Dimension of the specified vector of tolerances on maximal values " + value.Length
                                + " is different than the number of outputs " + OutputLength + ".");
                        ToleranceMaxRelativeToRange = null; // if tolerances are manually set, relative tol. are re-set!
                    }
                    _toleranceMax = value; 
                } 
            }
        }


        /// Auxiliary properties for defining tolerances in a relative way:

        private IVector _tolRmsRelative;

        /// <summary>Relative tolerances on RMS errors of outputs over training points, relative to the 
        /// correspoinding ranges of output data.</summary>
        /// <remarks><para>When this vector is set to a non-null value, elements of RMS tolerances vector
        /// (property <see cref="ToleranceRms"/>) are set to the specified fractions of the ranges 
        /// of the corresponding output values in training parameters (defined by the property 
        /// <see cref="OutputDataBounds"/>) if these are defined (otherwise this may happen when the
        /// property gets defined).</para>
        /// <para>Whenever this property is defined, re-setting the property <see cref="OutputDataBounds"/> will
        /// cause recalculation of <see cref="ToleranceRms"/> according to elements of this property and the
        /// ranges of corresponding output values.</para>
        /// <para>Setting this property undefines the <see cref="ToleranceRmsRelativeToRangeScalar"/> property,
        /// even if it is set to null.</para>
        /// <para>If the scalar relative tolerance is set (property <see cref="ToleranceRmsRelativeToRangeScalar"/>)
        /// and this property is not set yet, the getter of this property will evaluate to a vector whose
        /// elements are equal to the scalar relative tolerance.</para></remarks>
        public IVector ToleranceRmsRelativeToRange
        {
            get
            { lock (Lock) {
                if (_tolRmsRelative == null && _tolRmsRelativeScalar>0)
                {
                    ToleranceRmsRelativeToRange = new Vector(OutputLength, _tolRmsRelativeScalar);
                }
                return _tolRmsRelative; 
            } }
            set {
                lock (Lock)
                {
                    _tolRmsRelative = value;
                    if (value != null)
                    {
                        SetRmsToleranceRelstiveToRange();
                    }
                    _tolRmsRelativeScalar = 0;  // when vector version is set, scalar version is undefined
                }
            }
        }


        private double _tolRmsRelativeScalar;

        /// <summary>Scalar through which all components of the Relative tolerances on RMS errors of outputs
        /// can be set to the same value.</summary>
        /// <remarks><para>Getter returns a value greater than 0 only if the property has been previously set.</para>
        /// <para>Setting a vector of relative tolerances (property <see cref="ToleranceRmsRelativeToRange"/>) sets 
        /// this property to 0, even it it is set to null.</para>
        /// <para>Setting the property to a value greater than 0 automatically sets the vector of relative tolerances 
        /// (property <see cref="ToleranceRmsRelativeToRange"/>) in such a way that all elements are equal to this property.</para></remarks>
        public double ToleranceRmsRelativeToRangeScalar
        {
            get { lock (Lock) { return _tolRmsRelativeScalar; } }
            set
            {
                lock (Lock)
                {
                    _tolRmsRelativeScalar = value;
                    if (value > 0)
                    {
                        ToleranceRmsRelativeToRange = new Vector(OutputLength, value);
                        _tolRmsRelativeScalar = value; // because setting vector property clears it
                    }
                }
            }
        }

        /// <summary>Updates the tolerances on RMS errors of outputs according to the relative tolerances 
        /// (defined by <see cref="ToleranceRmsRelativeToRange"/>) and the ranges in output data
        /// (defined by <see cref="OutputDataBounds"/>), if both are defined.</summary>
        protected void SetRmsToleranceRelstiveToRange()
        {
            lock (Lock)
            {
                IVector tolRelative = ToleranceRmsRelativeToRange;
                double tolRelativeScalar = ToleranceRmsRelativeToRangeScalar;
                if (tolRelative == null)
                {
                    IVector tolRel = ToleranceRmsRelativeToRange;  // this will set relative tolerances if defined, and consequently absolute tolerances
                } else if (_outputDataBounds != null)
                {
                    if (tolRelative.Length != _outputLength)
                        throw new InvalidDataException("Dimension of relative RMS tolerances is different than output length.");
                    if (_outputDataBounds.Dimension != _outputLength)
                        throw new InvalidDataException("Dimension of output data bounds " + _outputDataBounds.Dimension
                            + " is different than number of neural network otupts " + _outputLength + ".");
                    IVector newTolerances = ToleranceRms;
                    _outputDataBounds.GetIntervalLengths(ref newTolerances);
                    VectorBase.ArrayProductPlain(newTolerances, tolRelative, tolRelative);
                    ToleranceRms = newTolerances;
                    _tolRmsRelative = tolRelative;  // since setter for ToleranceRMS will reset the value
                    _tolRmsRelativeScalar = tolRelativeScalar;
                }
            }
        }

        private IVector _tolMaxRelative;

        /// <summary>Relative tolerances on max. abs. errors of outputs over training points, relative to the 
        /// correspoinding ranges of output data.</summary>
        /// <remarks><para>When this vector is set to a non-null value, elements of max. abs. tolerances vector
        /// (property <see cref="ToleranceMax"/>) are set to the specified fractions of the ranges 
        /// of the corresponding output values in training parameters (defined by the property 
        /// <see cref="OutputDataBounds"/>) if these are defined (otherwise this may happen when the
        /// property gets defined).</para>
        /// <para>Whenever this property is defined, re-setting the property <see cref="OutputDataBounds"/> will
        /// cause recalculation of <see cref="ToleranceMax"/> according to elements of this property and the
        /// ranges of corresponding output values.</para>
        /// <para>Setting this property undefines the <see cref="ToleranceMaxRelativeToRangeScalar"/> property,
        /// even if it is set to null.</para>
        /// If the scalar relative tolerance is set (property <see cref="ToleranceMaxRelativeToRangeScalar"/>)
        /// and this property is not set yet, the getter of this property will evaluate to a vector whose
        /// elements are equal to the scalar relative tolerance.</remarks>
        public IVector ToleranceMaxRelativeToRange
        {
            get
            {
                lock (Lock)
                {
                    if (_tolMaxRelative == null && _tolMaxRelativeScalar > 0)
                    {
                        ToleranceMaxRelativeToRange = new Vector(OutputLength, _tolMaxRelativeScalar);
                    }
                    return _tolMaxRelative;
                }
            }
            set
            {
                lock (Lock)
                {
                    _tolMaxRelative = value;
                    if (value != null)
                    {
                        SetMaxToleranceRelstiveToRange();
                    }
                    _tolMaxRelativeScalar = 0;  // when vector version is set, scalar version is undefined
                }
            }
        }

        private double _tolMaxRelativeScalar;

        /// <summary>Scalar through which all components of the Relative tolerances on max. abs. errors of outputs
        /// can be set to the same value.</summary>
        /// <remarks><para>Getter returns a value greater than 0 only if the property has been previously set.</para>
        /// <para>Setting a vector of relative tolerances (property <see cref="ToleranceMaxRelativeToRange"/>) sets 
        /// this property to 0, even it it is set to null.</para>
        /// <para>Setting the property to a value greater than 0 automatically sets the vector of relative tolerances 
        /// (property <see cref="ToleranceMaxRelativeToRange"/>) in such a way that all elements are equal to this property.</para></remarks>
        public double ToleranceMaxRelativeToRangeScalar
        {
            get { lock (Lock) { return _tolMaxRelativeScalar; } }
            set
            {
                lock (Lock)
                {
                    _tolMaxRelativeScalar = value;
                    if (value > 0)
                    {
                        ToleranceMaxRelativeToRange = new Vector(OutputLength, value);
                        _tolMaxRelativeScalar = value; // because setting vector property clears it
                    }
                }
            }
        }

        /// <summary>Updates the tolerances on max. abs. errors of outputs according to the relative tolerances 
        /// (defined by <see cref="ToleranceMaxRelativeToRange"/>) and the ranges in output data
        /// (defined by <see cref="OutputDataBounds"/>), if both are defined.</summary>
        protected void SetMaxToleranceRelstiveToRange()
        {
            lock (Lock)
            {
                IVector tolRelative = ToleranceMaxRelativeToRange;
                double tolRelativeScalar = ToleranceMaxRelativeToRangeScalar;
                if (tolRelative == null)
                {
                    IVector tolRel = ToleranceMaxRelativeToRange;  // this will set relative tolerances if defined, and consequently absolute tolerances
                }
                else if (_outputDataBounds != null)
                {
                    if (tolRelative.Length != _outputLength)
                        throw new InvalidDataException("Dimension of relative max. abs. tolerances is different than output length.");
                    if (_outputDataBounds.Dimension != _outputLength)
                        throw new InvalidDataException("Dimension of output data bounds " + _outputDataBounds.Dimension
                            + " is different than number of neural network otupts " + _outputLength + ".");
                    IVector newTolerances = ToleranceMax;
                    _outputDataBounds.GetIntervalLengths(ref newTolerances);
                    VectorBase.ArrayProductPlain(newTolerances, tolRelative, tolRelative);
                    ToleranceMax = newTolerances;
                    _tolMaxRelative = tolRelative;  // since setter for ToleranceMax will reset the value
                    _tolMaxRelativeScalar = tolRelativeScalar;
                }
            }
        }


        // Tools for storing convergence data for later use:

        ///// <summary>RMS error convergence list. 
        ///// Saved after every set of epochs. </summary>
        //public List<IVector> ConvergenceRmsList
        //{
        //    get { return _convergenceRmsList; }
        //    set { _convergenceRmsList = value; }
        //}

        /// <summary> List of epoch numbers at which convergence data was sampled. 
        /// Saved after every set of epochs. </summary>
        /// $A Tako78 Sep12;
        public List<int> EpochNumbers
        {
            get { return _epochNumbers; }
            set { _epochNumbers = value; }
        }
        /// <summary>Convergence List of RMS errors calculated on training data. 
        /// Saved after every set of epochs. </summary>
        public List<IVector> ConvergenceErrorsTrainingRmsList
        {
            get { return _convergenceErrorsTrainingRmsList; }
            set { _convergenceErrorsTrainingRmsList = value; }
        }

        /// <summary>Convergence List of Maximum errors calculated on training data. 
        /// Saved after every set of epochs. </summary>
        public List<IVector> ConvergenceErrorsTrainingMaxList
        {
            get { return _convergenceErrorsTrainingMaxList; }
            set { _convergenceErrorsTrainingMaxList = value; }
        }

        /// <summary>Convergence List of Rms errors calculated on verification data.
        /// Saved after every set of epochs. </summary>
        public List<IVector> ConvergenceErrorsVerificationRmsList
        {
            get { return _convergenceErrorsVerificationRmsList; }
            set { _convergenceErrorsVerificationRmsList = value; }
        }

        /// <summary>Convergence List of Maximum errors calculated on verification data.
        /// Saved after every set of epochs. </summary>
        public List<IVector> ConvergenceErrorsVerificationMaxList
        {
            get { return _convergenceErrorsVerificationMaxList; }
            set { _convergenceErrorsVerificationMaxList = value; }
        }

        /// <summary> Flag to enable RMS error convergence colection. </summary>
        public bool SaveConvergenceRms
        {
            get { return _convergenceRmsEnabled; }
            set { _convergenceRmsEnabled = value; }
        }

        /// <summary>Prepares the networks array (allocates it if necessary) for storing all neural 
        /// networks of the current object.</summary>
        protected abstract void PrepareNetworksArray();


        /// <summary>Prepares neural network for use.
        /// If networks have not yet been created accordinfg to internal data, they are created.
        /// If networks are already prepared then this method does nothing.</summary>
        /// <remarks>Some things suc as creation of a neural network follow the pattern of lazy evaluation.</remarks>
        public abstract void PrepareNetwork();

        /// <summary>Creates the neural network anew. It the network already exists on the current object, 
        /// it is discarded.</summary>
        public abstract void CreateNetwork();

        /// <summary>Resets the neural network.</summary>
        public abstract void ResetNetwork();

        /// <summary>Destroys the neural network.</summary>
        public abstract void DestroyNetwork();


        protected const string _defaultNetworkFilename = "network.dat";

        /// <summary>Returns an absolute path to the file for storing the specified neural network contained 
        /// on the current object, with respect to suggested file path and index of the network.
        /// The returned path is in the same directory as suggested file path and has the same file extension (if any).
        /// If the suggested path represents a directory, then some default suggested path is assumed.
        /// If there is only one network then the returned file path is the same as the suggested one
        /// (or the same as default file name within the suggested directory, if a directory path is proposed).</summary>
        /// <param name="fileOrDirectoryPath">Suggested file or directory path, can be a relative path.</param>
        /// <param name="whichNetwork">Index of the network for which path of the file to store the network is returned.</param>
        /// <returns></returns>
        public string GetNetworkFilePath(string fileOrDirectoryPath, int whichNetwork)
        {
            if (string.IsNullOrEmpty(fileOrDirectoryPath))
            {
                throw new ArgumentNullException("File or directory path to store neural network(s) is not specified.");
            }
            bool isDirectory = false;
            string absolutePath = Path.GetFullPath(fileOrDirectoryPath);
            if (Directory.Exists(fileOrDirectoryPath))
                isDirectory = true;
            else if (Path.GetDirectoryName(absolutePath) == absolutePath)
                isDirectory = true;
            string directoryPath = null;
            if (isDirectory)
            {
                directoryPath = absolutePath;
                fileOrDirectoryPath = Path.Combine(fileOrDirectoryPath, _defaultNetworkFilename);
                absolutePath = Path.GetFullPath(fileOrDirectoryPath);
            }
            else
                directoryPath = Path.GetDirectoryName(absolutePath);
            if (MultipleNetworks)
            {
                string fileName = Path.GetFileNameWithoutExtension(absolutePath);
                string extension = Path.GetExtension(absolutePath);
                return Path.Combine(directoryPath, fileName + "_" + whichNetwork.ToString() + extension);
            }
            else
                return absolutePath;
        }

        private string _networkFilePath = null;

        private string _networkRelativePath = null;

        /// <summary>Gets string representation of type of the current object. This is used e.g. in deserialization in order to prevent
        /// that wrong type of internal representation would be read in.</summary>
        public string NeuralApproximatorType
        {
            get { return this.GetType().ToString(); }
        }


        ///// <summary>Tests whether creation of a neural approximator according to a specified type can be successful.</summary>
        //public static void TestTypeCreation()
        //{
        //    Console.WriteLine();
        //    Console.WriteLine("Before creating a new object of the stored type, using FullName: ");
        //    NeuralApproximatorAforge approximatorAforge = new NeuralApproximatorAforge();
        //    string typeString;
        //    typeString = approximatorAforge.GetType().FullName;
        //    Type approximatorType = Type.GetType(typeString);
        //    INeuralApproximator approximator = null;
        //    try
        //    {
        //        approximator = (INeuralApproximator) Activator.CreateInstance(approximatorType);
        //    }
        //    catch { }
        //    Console.WriteLine("After creating an object.");
        //    if (approximator == null)
        //        Console.WriteLine("ERROR: created approximator is null!");
        //    else if (approximator as NeuralApproximatorAforge == null)
        //    {
        //        Console.WriteLine("ERROR: the created approximator is not of the correct type!");
        //    }
        //    else
        //    {
        //        Console.WriteLine("O.K., approximator of correct type created. Type str.: " + typeString);
        //    }
        //    Console.WriteLine();
        //    Console.WriteLine();
        //    Console.WriteLine("Before creating a new object of the stored type, using AssemblyQualifiedName: ");
        //    approximatorAforge = new NeuralApproximatorAforge();
        //    typeString = approximatorAforge.GetType().AssemblyQualifiedName;
        //    approximatorType = Type.GetType(typeString);
        //    approximator = null;
        //    try
        //    {
        //        approximator = (INeuralApproximator) Activator.CreateInstance(approximatorType);
        //    }
        //    catch { }
        //    Console.WriteLine("After creating an object.");
        //    if (approximator == null)
        //        Console.WriteLine("ERROR: created approximator is null!");
        //    else if (approximator as NeuralApproximatorAforge == null)
        //    {
        //        Console.WriteLine("ERROR: the created approximator is not of the correct type!");
        //    }
        //    else
        //    {
        //        Console.WriteLine("O.K., approximator of correct type created. Type str.: " + typeString);
        //    }
        //    Console.WriteLine();
        //}


        /// <summary>Path where the curren network state has been saved, or null if the current state has not been saved yet.
        /// The SaveFile  methods takes care that the file path is stored when network state is saved.
        /// InvalidateTrainingDependencies() takes care that this file path is set to null if network state has changed after last save.</summary>
        public string NetworkStateFilePath
        {
            get { lock (Lock) { return _networkFilePath; } }
            protected internal set { lock (Lock) {
                if (value == null)
                    _networkFilePath = null;
                else
                    _networkFilePath = string.Copy(value); 
            } }
        }

        /// <summary>Relative path where the curren network state has been saved. Auxiliary property used in 
        /// deserialization.
        /// When the whole Neural network approximator is saved to a file, tis path is updated in such a way
        /// that it points to the fiele where the network state has been saved, but relative to the path where
        /// the whole approximator is saved. This enables restore of the saved network state at a later time, 
        /// even if the containing directory has moved within the file system or has even been copied to another system.</summary>
        public string NetworkStateRelativePath
        {
            get { lock (Lock) { return _networkRelativePath; } }
            set { lock (Lock) {
                if (value == null)
                    _networkRelativePath = null;
                else
                    _networkRelativePath = string.Copy(value); 
            } }
        }

        /// <summary>Saves the state of the neural network to the specified file.
        /// If the file already exists, its contents are overwritten.</summary>
        /// <param name="filePath">Path to the file into which the network is saved.</param>
        public void SaveNetwork(string filePath)
        {
            filePath = Path.GetFullPath(filePath);
            NetworkStateFilePath = filePath;  // store path where network state has been saved
            SaveNetworkSpecific(filePath);

            Type ta = typeof(NeuralApproximatorAforgeFake);

        }

        /// <summary>Restores neural network from a file where it has been stored before.</summary>
        /// <param name="filePath">Path to the file from which the neural network is read.</param>
        public void LoadNetwork(string filePath)
        {
            filePath = Path.GetFullPath(filePath);
            NetworkStateFilePath = filePath;  // store path from which network state has been restored
            LoadNetworkSpecific(filePath);
            this.NetworkTrained = true;
        }

        /// <summary>Saves the state of the neural network to the specified file.
        /// If the file already exists, its contents are overwritten.</summary>
        /// <param name="filePath">Path to the file into which the network is saved.</param>
        protected abstract void SaveNetworkSpecific(string filePath);

        /// <summary>Restores neural network from a file where it has been stored before.</summary>
        /// <param name="filePath">Path to the file from which the neural network is read.</param>
        protected abstract void LoadNetworkSpecific(string filePath);


        #region Training

        private SampledDataSet _trainingData;

        private IndexList _verificationIndices;

        private IBoundingBox _inputDataBounds;

        private IBoundingBox _outputDataBounds;

        private IBoundingBox _inputNeuronRange;

        private IBoundingBox _outputNeuronsRange;
        

        private double _inputBoundsSafetyFactor = 1.5;
        private double _outputBoundsSafetyFactor = 2.0;

        protected double _defaultNeuronMinInput = -2;
        protected double _defaultNeuronMaxInput =  2;
        protected double _defaultNeuronMinOutput = -1.0;
        protected double _defaultNeuronMaxOutput =  1.0;


        private bool _internalTrainingDataPrepared = false;

        private bool _trainingOutputsCalculated = false;

        private bool _verificationOutputsCalculated = false;

        private bool _networkTrained;

 
        /// <summary>List of calculated outputs in points contained in (all) training data.</summary>
        protected List<IVector> _calculatedOutputs = null;

        /// <summary>Gets or sets the training data.</summary>
        public SampledDataSet TrainingData
        {
            get { lock (Lock) { return _trainingData; } }
            set
            {
                lock (Lock)
                {
                    // InternalTrainingDataPrepared = false;
                    // NetworkTrained = false;
                    InvalidateTrainingDataDependencies();
                    if (VerificationIndices!=null)
                        if (VerificationIndices.Length >= TrainingData.Length)
                            throw new InvalidDataException("Number of data elements in the training set is less or equal to the number of verification indices.");
                    _trainingData = value;
                    if (value != null)
                    {
                        this.InputLength = value.InputLength;
                        this.OutputLength = value.OutputLength;
                        // Calculate bounds on input and output parameters:
                        RecalculateDataBounds();
                    }
                }
            }
        }

        // TODO: Tadej, komentiraj!
        public void SetTrainingAndVerificationData(SampledDataSet trainingData, SampledDataSet verificationData)
        {
            lock (Lock)
            {
                if (_trainingData == null)
                    _trainingData = new SampledDataSet();
                _trainingData.Clear();
                _trainingData.Add(trainingData);
                int firstVefification = _trainingData.Length;
                _trainingData.Add(verificationData);
                IndexList verificationIndices = new IndexList();
                for (int i = firstVefification; i < _trainingData.Length; ++i)
                {
                    verificationIndices.Add(i);
                }
                VerificationIndices = verificationIndices;
            }
        }

        public void GetTrainingData(ref SampledDataSet trainingData)
        {
            if (trainingData == null)
                trainingData = new SampledDataSet();
            else
                trainingData.Clear();
            for (int i = 0; i < _trainingData.Length; ++i)
            {
                if (!VerificationIndices.Contains(i))
                    trainingData.Add(_trainingData[i]);
            }
        }

        public void GetVerificationData(ref SampledDataSet veerificationData)
        {
            if (veerificationData == null)
                veerificationData = new SampledDataSet();
            else
                veerificationData.Clear();
            for (int i = 0; i < _trainingData.Length; ++i)
            {
                if (!VerificationIndices.Contains(i))
                    veerificationData.Add(_trainingData[i]);
            }
        }

        public void GetTrainingAndVerificationData(ref SampledDataSet trainingData, ref SampledDataSet verificationData)
        {
            GetTrainingData(ref trainingData);
            GetVerificationData(ref verificationData);
        }




        /// <summary>Saves network's training data to the specified JSON file.
        /// File is owerwritten if it exists.</summary>
        /// <param name="filePath">Path to the file where training data is saved.</param>
        public void SaveTrainingDataJson(string filePath)
        {
            lock (Lock)
            {
                SampledDataSetDto dtoOriginal = new SampledDataSetDto();
                dtoOriginal.CopyFrom(TrainingData);
                ISerializer serializer = new SerializerJson();
                serializer.Serialize<SampledDataSetDto>(dtoOriginal, filePath);
            }
        }

        /// <summary>Restores training data from the specified file in JSON format.</summary>
        /// <param name="filePath">File from which training data is restored.</param>
        public void LoadTrainingDataJson(string filePath)
        {
            lock (Lock)
            {
                ISerializer serializer = new SerializerJson();
                SampledDataSetDto dtoRestored = serializer.DeserializeFile<SampledDataSetDto>(filePath);
                SampledDataSet dataRestored = TrainingData;
                dtoRestored.CopyTo(ref dataRestored);
                TrainingData = dataRestored;
            }
        }


        /// <summary>Gets or sets indices of training data elements that are used for verification of
        /// how precise appeoximation is. These elements are excluded from training of neural network.</summary>
        /// <remarks>Object of type IndexList that contains indices of elements used for verification,
        /// always contains a sorted list of unique indices.</remarks>
        public virtual IndexList VerificationIndices
        {
            get { lock (Lock) { return _verificationIndices; } }
            set
            {
                lock (Lock)
                {
                    InvalidateTrainingDataDependencies();
                    InternalTrainingDataPrepared = false;
                    if (value != null)
                    {
                        if (TrainingData!=null)
                            if (value.Length >= TrainingData.Length)
                                throw new InvalidDataException("Number of verification indices is greater or equal to the number of data elements in the training set.");
                        if (value.Length > 0)
                        {
                            int lastIndex = value[value.Length - 1];
                            if (lastIndex >= TrainingData.Length)
                                throw new InvalidDataException("The last verification index (" + lastIndex
                                    + ")  is out of range, should be less than training data length (" + TrainingData.Length + ").");
                        }
                    }
                    _verificationIndices = value;
                }
            }
        }


        /// <summary>Bounds on input data, used for scaling from actual input to input used by neural network.
        /// Scaling is performed because of the bound codomain and image of activation functions.</summary>
        public virtual IBoundingBox InputDataBounds
        {
            get { lock (Lock) { return _inputDataBounds; } }
            set 
            { 
                lock (Lock) 
                { 
                    _inputDataBounds = value; 
                    InvalidateTrainingDataDependencies(); 
                } 
            }
        }

        /// <summary>Bounds on output data, used for scaling from actual output to output produced by neural network.
        /// Scaling is performed because of the bound codomain and image of activation functions.</summary>
        public virtual IBoundingBox OutputDataBounds
        {
            get { lock (Lock) { return _outputDataBounds; } }
            set 
            { 
                lock (Lock) 
                { 
                    _outputDataBounds = value; 
                    InvalidateTrainingDataDependencies(); 
                } 
            }
        }

        /// <summary>Safety factor by which interval lenghts of input data bounds are enlarged after
        /// bounds are automatically determined from the range of input data in the training set.
        /// Setter re-calculated the input data bounds and therefore invalidates training data dependencies.</summary>
        public virtual double InputBoundsSafetyFactor
        {
            get { lock (Lock) { return _inputBoundsSafetyFactor; } }
            set 
            { 
                lock (Lock) 
                {
                    if (value < 1.0)
                        throw new ArgumentException("Invalid input bound safety factor " + value + ", should be greater or equal to 1.");
                    _inputBoundsSafetyFactor = value;
                    InvalidateTrainingDataDependencies();
                    RecalculateInputDataBounds();
                } 
            }
        }

        /// <summary>Safety factor by which interval lenghts of output data bounds are enlarged after
        /// bounds are automatically determined from the range of output data in the training set.
        /// Setter re-calculated the output data bounds and therefore invalidates training data dependencies.</summary>
        public virtual double OutputBoundsSafetyFactor
        {
            get { lock (Lock) { return _outputBoundsSafetyFactor; } }
            set 
            { 
                lock (Lock) 
                {
                    if (value < 1.0)
                        throw new ArgumentException("Invalid output bound safety factor " + value + ", should be greater or equal to 1.");
                    _outputBoundsSafetyFactor = value;
                    RecalculateOutputDataBounds();
                } 
            }
        }


        /// <summary>Gets the range in which data should be for input neurons, used for scaling 
        /// from actual input to input used by neural network.
        /// This depends on the activation function.</summary>
        /// <remarks>Setter is not public.</remarks>
        public virtual IBoundingBox InputNeuronsRange
        {
            get { lock (Lock) { return _inputNeuronRange; } }
            protected set 
            {
                lock (Lock)
                {
                    _inputNeuronRange = value; 
                    InvalidateTrainingDataDependencies();
                }
            }
        }

        /// <summary>Gets the range of the data output from output neurons, used for scaling 
        /// from actual output to output produced by neural network.
        /// This will normally depend on the activation function.</summary>
        /// <remarks>Setter is not public.</remarks>
        public virtual IBoundingBox OutputNeuronsRange
        {
            get { lock (Lock) { return _outputNeuronsRange; } }
            protected set 
            { 
                lock (Lock) 
                { 
                    _outputNeuronsRange = value; 
                    InvalidateTrainingDataDependencies(); 
                } 
            }
        }


        /// <summary>Sets the neurons input range. Bounds for all input neurons are set equally.</summary>
        /// <param name="min">Lower bound for all input neurons.</param>
        /// <param name="max">Upper bound for all input neurons.</param>
        public void SetNeuronsInputRange(double min, double max)
        {
            lock (Lock)
            {
                if (InputNeuronsRange == null)
                    InputNeuronsRange = new BoundingBox(InputLength);
                else
                    InputNeuronsRange.SetDimensionAndReset(InputLength);
                for (int i = 0; i < InputLength; ++i)
                    InputNeuronsRange.Update(i, min, max);
                InvalidateTrainingDataDependencies();
            }
        }

        /// <summary>Sets the neurons output range. Bounds for all output neurons are set equally.</summary>
        /// <param name="min">Lower bound for all output neurons.</param>
        /// <param name="max">Upper bound for all output neurons.</param>
        public void SetNeuronsOutputRange(double min, double max)
        {
            lock (Lock)
            {
                if (OutputNeuronsRange == null)
                    OutputNeuronsRange = new BoundingBox(OutputLength);
                else
                    OutputNeuronsRange.SetDimensionAndReset(OutputLength);
                for (int i = 0; i < OutputLength; ++i)
                    OutputNeuronsRange.Update(i, min, max);
                InvalidateTrainingDataDependencies();
            }
        }


        /// <summary>Recalculates input data bounds by taking into account the training data set of the current object.</summary>
        /// <remarks>Training data dependencies are invalidated by this method because setting bounds invalidates them.</remarks>
        public void RecalculateInputDataBounds()
        {
            RecalculateInputDataBounds(this.TrainingData);
        }

        /// <summary>Recalculates output data bounds by taking into account the training data set of the current object.</summary>
        /// <remarks>Training data dependencies are invalidated by this method because setting bounds invalidates them.</remarks>
        public void RecalculateOutputDataBounds()
        {
            RecalculateOutputDataBounds(this.TrainingData);
        }

        /// <summary>Recalculates input and output data bounds by taking into account the training data set of the current object.</summary>
        /// <remarks>Training data dependencies are invalidated by this method because setting bounds invalidates them.</remarks>
        public void RecalculateDataBounds()
        {
            RecalculateDataBounds(this.TrainingData);
        }

        /// <summary>Recalculates input data bounds by taking into account the specified training data set.</summary>
        /// <param name="trainingData">Training data set accourding to which input bounds are adjusted.</param>
        /// <remarks>Training data dependencies are invalidated by this method because setting bounds invalidates them.</remarks>
        public void RecalculateInputDataBounds(SampledDataSet trainingData)
        {
            lock (Lock)
            {
                if (trainingData == null)
                {
                    InputDataBounds = null;
               }
                else {
                    // Calculate bounds on input parameters:
                    IBoundingBox inputBox = InputDataBounds;
                    trainingData.GetInputRange(ref inputBox);
                    if (inputBox != null)
                        inputBox.ExpandOrShrinkInterval(InputBoundsSafetyFactor, 1.0e-12 /* zero interval length replacement */);
                    InputDataBounds = inputBox;
                }
            }
        }


        /// <summary>Recalculates output data bounds by taking into account the specified training data set.</summary>
        /// <param name="trainingData">Training data set accourding to which output bounds are adjusted.</param>
        /// <remarks>Training data dependencies are invalidated by this method because setting bounds invalidates them.</remarks>
        public void RecalculateOutputDataBounds(SampledDataSet trainingData)
        {
            lock (Lock)
            {
                if (trainingData == null)
                {
                    OutputDataBounds = null;
                } else
                {
                    // Calculate bounds on output parameters:
                    IBoundingBox outputBox = OutputDataBounds;
                    trainingData.GetOutputRange(ref outputBox);
                    if (outputBox != null)
                        outputBox.ExpandOrShrinkInterval(OutputBoundsSafetyFactor, 1.0e-12 /* zero interval length replacement */);
                    OutputDataBounds = outputBox;
                }
            }
        }


        /// <summary>Recalculates input and output data bounds by taking into account the specified training data set.</summary>
        /// <param name="trainingData">Training data set accourding to which input and output bounds are adjusted.</param>
        /// <remarks>Training data dependencies are invalidated by this method because setting bounds invalidates them.</remarks>
        public void RecalculateDataBounds(SampledDataSet trainingData)
        {
            lock(Lock)
            {
                RecalculateInputDataBounds(trainingData);
                RecalculateOutputDataBounds(trainingData);
            }
        }


        /// <summary>Maps (scales & shifts) and returns specific input value to the value prepared for the 
        /// corresponding input neuron.</summary>
        /// <param name="componentIndex">Index of the input value within the input vector.</param>
        /// <param name="value">Value that is mapped.</param>
        /// <returns>Input value mapped for the input to neural network.</returns>
        protected virtual double MapInput(int componentIndex, double value)
        {
            //if (InputDataBounds==null || InputNeuronsRange == null)
            //    return value;
            //else
            //{
            //    double minData = InputDataBounds.GetMin(componentIndex);
            //    double maxData = InputDataBounds.GetMax(componentIndex);
            //    double minNeuron = InputNeuronsRange.GetMin(componentIndex);
            //    double maxNeuron = InputNeuronsRange.GetMax(componentIndex);
            //    return minNeuron + (value - minData) * (maxNeuron - minNeuron) / (maxData - minData);
            //}
            return BoundingBox.Map(InputDataBounds, InputNeuronsRange, componentIndex, value);
        }

        /// <summary>Inverse maps (scales & shifts) and returns specific input value back from the neural input to the actual input.</summary>
        /// <param name="componentIndex">Index of the input value within the input vector.</param>
        /// <param name="value">Value that is mapped back from neural input.</param>
        /// <returns>Input value mapped back from the neural input.</returns>
        protected virtual double MapFromNeuralInput(int componentIndex, double value)
        {
            //if (InputDataBounds==null || InputNeuronsRange == null)
            //    return value;
            //else
            //{
            //    double minData = InputDataBounds.GetMin(componentIndex);
            //    double maxData = InputDataBounds.GetMax(componentIndex);
            //    double minNeuron = InputNeuronsRange.GetMin(componentIndex);
            //    double maxNeuron = InputNeuronsRange.GetMax(componentIndex);
            //    return minData + (value - minNeuron) * (maxData-minData) / (maxNeuron-minNeuron);
            //}
            return BoundingBox.Map(InputNeuronsRange, InputDataBounds, componentIndex, value);
        }

        /// <summary>Maps (scales & shifts) vector of input values to the vector of neuron inputs.</summary>
        /// <param name="values">Vector of input values to be mapped.</param>
        /// <param name="mappedValues">Vector where mapped values are stored.</param>
        protected virtual void MapInput(IVector values, ref IVector mappedValues)
        {
            //if (values == null)
            //    throw new ArgumentNullException("Vector to be mapped is not specified (null reference).");
            //else if (values.Length != InputLength)
            //    throw new ArgumentNullException("Dimension of the vector do be mapped (" + values.Length
            //        + ") is not equal to neural network input dimension (" + InputLength + ").");
            //if (mappedValues == null)
            //    mappedValues = new Vector(InputLength);
            //else if (mappedValues.Length != InputLength)
            //    Vector.Resize(ref mappedValues, InputLength);
            //for (int i = 0; i < InputLength; ++i)
            //    mappedValues[i] = MapInput(i, values[i]);
            BoundingBox.Map(InputDataBounds, InputNeuronsRange, values, ref mappedValues);
        }

        /// <summary>Inverse maps (scales & shifts) vector of neural input values back to the vector of actual inputs.</summary>
        /// <param name="values">Vector of neural inputs to be mapped.</param>
        /// <param name="mappedValues">Vector where inverse mapped values are stored.</param>
        protected virtual void MapFromNeuralInput(IVector values, ref IVector mappedValues)
        {
            //if (values == null)
            //    throw new ArgumentNullException("Vector to be inverse mapped is not specified (null reference).");
            //else if (values.Length != InputLength)
            //    throw new ArgumentNullException("Dimension of the vector do be inverse mapped (" + values.Length
            //        + ") is not equal to neural network input dimension (" + InputLength + ").");
            //if (mappedValues == null)
            //    mappedValues = new Vector(InputLength);
            //else if (mappedValues.Length != InputLength)
            //    Vector.Resize(ref mappedValues, InputLength);
            //for (int i = 0; i < InputLength; ++i)
            //    mappedValues[i] = MapFromNeuralInput(i, values[i]);
            BoundingBox.Map(InputNeuronsRange, InputDataBounds, values, ref mappedValues);
        }

        /// <summary>Maps (scales & shifts) and returns specific output value to the output value of the 
        /// corresponding output neuron.</summary>
        /// <param name="componentIndex">Index of the output value within the output vector.</param>
        /// <param name="value">Value that is mapped.</param>
        /// <returns>Output value mapped for the output to neural network.</returns>
        protected virtual double MapOutput(int componentIndex, double value)
        {
            //if (OutputDataBounds==null || OutputNeuronsRange == null)
            //    return value;
            //else
            //{
            //    double minData = OutputDataBounds.GetMin(componentIndex);
            //    double maxData = OutputDataBounds.GetMax(componentIndex);
            //    double minNeuron = OutputNeuronsRange.GetMin(componentIndex);
            //    double maxNeuron = OutputNeuronsRange.GetMax(componentIndex);
            //    return minNeuron + (value - minData) * (maxNeuron-minNeuron) / (maxData-minData);
            //}
            return BoundingBox.Map(OutputDataBounds, OutputNeuronsRange, componentIndex, value);
        }

        /// <summary>Inverse maps (scales & shifts) and returns specific output value back from the neural output to the actual output.</summary>
        /// <param name="componentIndex">Index of the output value within the output vector.</param>
        /// <param name="value">Value that is mapped back from neural output.</param>
        /// <returns>Output value mapped back from the neural output.</returns>
        protected virtual double MapFromNeuralOutput(int componentIndex, double value)
        {
            //if (OutputDataBounds == null || OutputNeuronsRange == null)
            //    return value;
            //else
            //{
            //    double minData = OutputDataBounds.GetMin(componentIndex);
            //    double maxData = OutputDataBounds.GetMax(componentIndex);
            //    double minNeuron = OutputNeuronsRange.GetMin(componentIndex);
            //    double maxNeuron = OutputNeuronsRange.GetMax(componentIndex);
            //    return minData + (value - minNeuron) * (maxData-minData) / (maxNeuron-minNeuron);
            //}
            return BoundingBox.Map(OutputNeuronsRange, OutputDataBounds, componentIndex, value);
        }

        /// <summary>Maps (scales & shifts) vector of output values to the vector of neuron outputs.</summary>
        /// <param name="values">Vector of output values to be mapped.</param>
        /// <param name="mappedValues">Vector where mapped values are stored.</param>
        protected virtual void MapOutput(IVector values, ref IVector mappedValues)
        {
            //if (values == null)
            //    throw new ArgumentNullException("Vector to be mapped is not specified (null reference).");
            //else if (values.Length != OutputLength)
            //    throw new ArgumentNullException("Dimension of the vector do be mapped (" + values.Length
            //        + ") is not equal to neural network output dimension (" + OutputLength + ").");
            //if (mappedValues == null)
            //    mappedValues = new Vector(OutputLength);
            //else if (mappedValues.Length != OutputLength)
            //    Vector.Resize(ref mappedValues, OutputLength);
            //for (int i = 0; i < OutputLength; ++i)
            //    mappedValues[i] = MapOutput(i, values[i]);
            BoundingBox.Map(OutputDataBounds, OutputNeuronsRange, values, ref mappedValues);
        }

        /// <summary>Inverse maps (scales & shifts) vector of neural output values back to the vector of actual outputs.</summary>
        /// <param name="values">Vector of neural outputs to be mapped.</param>
        /// <param name="mappedValues">Vector where inverse mapped values are stored.</param>
        protected virtual void MapFromNeuralOutput(IVector values, ref IVector mappedValues)
        {
            //if (values == null)
            //    throw new ArgumentNullException("Vector to be inverse mapped is not specified (null reference).");
            //else if (values.Length != OutputLength)
            //    throw new ArgumentNullException("Dimension of the vector do be inverse mapped (" + values.Length
            //        + ") is not equal to neural network output dimension (" + OutputLength + ").");
            //if (mappedValues == null)
            //    mappedValues = new Vector(OutputLength);
            //else if (mappedValues.Length != OutputLength)
            //    Vector.Resize(ref mappedValues, OutputLength);
            //for (int i = 0; i < OutputLength; ++i)
            //    mappedValues[i] = MapFromNeuralOutput(i, values[i]);
            BoundingBox.Map(OutputNeuronsRange, OutputDataBounds, values, ref mappedValues);
        }



        /// <summary>Gets number of all training points, including verification points.</summary>
        public int NumAllTrainingPoints
        {
            get
            {
                lock (Lock)
                {
                    int num = 0;
                    if (TrainingData != null)
                        num += TrainingData.Length;
                    return num;
                }
            }
        }

        /// <summary>Gets number of training points (this excludes verification points).</summary>
        public virtual int NumTrainingPoints
        {
            get 
            {
                lock (Lock)
                {
                    int num = 0;
                    if (TrainingData != null)
                        num += TrainingData.Length;
                    if (VerificationIndices != null)
                        num -= VerificationIndices.Length;
                    return num;
                }
            }
        }

        /// <summary>Gets number of verification points.</summary>
        public virtual int NumVerificationPoints
        {
            get 
            {
                lock (Lock)
                {
                    int num = 0;
                    if (VerificationIndices != null)
                        num += VerificationIndices.Length;
                    return num;
                }
            }
        }

        /// <summary>Whether outputs have been calculated, after last training, in the training points (excluding verification points).</summary>
        protected bool TrainingOutputsCalculated
        { get { lock (Lock) { return _trainingOutputsCalculated; } } set { lock (Lock) { _trainingOutputsCalculated = value; } } }

        /// <summary>Whether outputs have been calculated, after last training, in the training points (excluding verification points).</summary>
        protected bool VerificationOutputsCalculated
        { get { lock (Lock) { return _verificationOutputsCalculated; } } set { lock (Lock) { _verificationOutputsCalculated = value; } } }



        /// <summary>Calculates outputs in training points contained in training set, either in training points,
        /// in verification points, or both.</summary>
        /// <param name="calculateTrainingOutputs">Whether outputs are calculated in training points.</param>
        /// <param name="calculateVerificationOutputs">Whether outputs are calculated in verification points.</param>
        protected void CalculateTrainingVerificationOutputs(bool calculateTrainingOutputs, bool calculateVerificationOutputs)
        {
            lock (Lock)
            {
                if (TrainingData == null)
                    Util.ResizeList<IVector>(ref _calculatedOutputs, 0, null);
                else if ((calculateTrainingOutputs && !TrainingOutputsCalculated) || (calculateVerificationOutputs && !VerificationOutputsCalculated))
                {
                    int numCalculatedTrainingPoints = 0;
                    int numCalculatedVerificationPoints = 0;
                    bool resize = false;
                    if (_calculatedOutputs == null)
                        resize = true;
                    else if (_calculatedOutputs.Count != TrainingData.Length)
                        resize = true;
                    if (resize)
                        Util.ResizeList<IVector>(ref _calculatedOutputs, TrainingData.Length, null);
                    for (int i = 0; i < TrainingData.Length; ++i)
                    {
                        bool calculateThis = false;
                        bool isVerificationPoint = false;
                        if (VerificationIndices!=null)
                            if (VerificationIndices.Contains(i))
                                isVerificationPoint = true;
                        if ((calculateTrainingOutputs && !isVerificationPoint) || (calculateVerificationOutputs && isVerificationPoint))
                            calculateThis = true;
                        if (calculateThis)
                        {
                            SampledDataElement element = TrainingData[i];
                            if (element == null)
                                throw new InvalidDataException("Training data element No. " + i + "not defined (null reference).");
                            IVector outputs = _calculatedOutputs[i];
                            CalculateOutput(element.InputParameters, ref outputs);
                            _calculatedOutputs[i] = outputs;
                            if (isVerificationPoint)
                                ++ numCalculatedVerificationPoints;
                            else
                                ++ numCalculatedTrainingPoints;
                        }
                    }
                    if (calculateTrainingOutputs && numCalculatedTrainingPoints>0)
                        TrainingOutputsCalculated = true;
                    if (calculateVerificationOutputs && numCalculatedVerificationPoints>0)
                        VerificationOutputsCalculated = true;
                }
            }
        }


        /// <summary>Calculates outputs in training points of the training data set (this excludes verification points).</summary>
        protected void CalculateTrainingOutputs()
        {
            CalculateTrainingVerificationOutputs(true, false);
        }

        /// <summary>Calculates outputs in verification points of the training data set.</summary>
        protected void CalculateVerificationOutputs()
        {
            CalculateTrainingVerificationOutputs(false /* verification outputs */, true /* verification outputs */);
        }


        /// <summary>Calculates error measures - RMS (root mean square) of the differences - for the specified arrays
        /// of prescribed and calculated output values in a set of sampling points.</summary>
        /// <param name="dimOutput">Dimension of output values.</param>
        /// <param name="prescribed">Array of prescribed output values (e.g. from training data, measurements, etc.).</param>
        /// <param name="calculated">Array of calculated output values (e.g. by a trained neural network).</param>
        /// <param name="errors">Vector where error measures for each indivitual output value are stored.</param>
        public static void CalculateErrorsRms(int dimOutput, IVector[] prescribed, IVector[] calculated, ref IVector errors)
        {
            if (prescribed == null || calculated == null)
            {
                if (prescribed == null)
                    throw new ArgumentNullException("Array of prescribed output values vectors is not specified (null reference).");
                else if (calculated == null)
                    throw new ArgumentNullException("Array of calculated output values vectors is not specified (null reference).");
            } else if (prescribed.Length != calculated.Length)
            {
                throw new ArgumentNullException("Number of calculated output vectors (" + calculated.Length 
                    + ") does not match the number of prescribed output vectors (" + prescribed.Length + ").");
            }
            int numPoints = prescribed.Length;
            if (numPoints < 1)
                throw new ArgumentException("Number of output points for error calculation is less than 1.");
            if (errors==null)
                Vector.Resize(ref errors, dimOutput);
            else if (errors.Length!=dimOutput)
                Vector.Resize(ref errors, dimOutput);
            errors.SetZero();
            for (int i=0; i<numPoints; ++i)
            {
                IVector presc = prescribed[i];
                IVector calc = calculated[i];
                if (presc==null || calc==null)
                {
                    if (presc == null)
                        throw new InvalidDataException("Vector of prescribed values No. " + i + " is not defined (null reference).");
                    if (calc==null)
                        throw new InvalidDataException("Vector of calculated values No. " + i + " is not defined (null reference).");
                } else if (presc.Length != dimOutput)
                    throw new InvalidDataException("Vector of prescribed values No. " + i + " has wrong dimension ("
                        + presc.Length + " instead of " + dimOutput + ").");
                else if (calc.Length != dimOutput)
                    throw new InvalidDataException("Vector of calculated values No. " + i + " has wrong dimension ("
                        + calc.Length + " instead of " + dimOutput + ").");
                for (int k = 0; k < dimOutput; ++k)
                {
                    double dif = calc[k] - presc[k];
                    errors[k] += dif * dif;
                }
            }
            for (int k = 0; k < dimOutput; ++k)
            {
                if (numPoints > 0)
                    errors[k] /= (double)numPoints;
                errors[k] = Math.Sqrt(errors[k]);
                
            }
        }

        /// <summary>Calculates error measures - mean absolute value of the differences - for the specified arrays
        /// of prescribed and calculated output values in a set of sampling points.</summary>
        /// <param name="dimOutput">Dimension of output values.</param>
        /// <param name="prescribed">Array of prescribed output values (e.g. from training data, measurements, etc.).</param>
        /// <param name="calculated">Array of calculated output values (e.g. by a trained neural network).</param>
        /// <param name="errors">Vector where error measures for each indivitual output value are stored.</param>
        public static void CalculateErrorsMeanAbs(int dimOutput, IVector[] prescribed, IVector[] calculated, ref IVector errors)
        {
            if (prescribed == null || calculated == null)
            {
                if (prescribed == null)
                    throw new ArgumentNullException("Array of prescribed output values vectors is not specified (null reference).");
                else if (calculated == null)
                    throw new ArgumentNullException("Array of calculated output values vectors is not specified (null reference).");
            } else if (prescribed.Length != calculated.Length)
            {
                throw new ArgumentNullException("Number of calculated output vectors (" + calculated.Length 
                    + ") does not match the number of prescribed output vectors (" + prescribed.Length + ").");
            }
            int numPoints = prescribed.Length;
            if (numPoints < 1)
                throw new ArgumentException("Number of output points for error calculation is less than 1.");
            if (errors==null)
                Vector.Resize(ref errors, dimOutput);
            else if (errors.Length!=dimOutput)
                Vector.Resize(ref errors, dimOutput);
            errors.SetZero();
            for (int i=0; i<numPoints; ++i)
            {
                IVector presc = prescribed[i];
                IVector calc = calculated[i];
                if (presc==null || calc==null)
                {
                    if (presc == null)
                        throw new InvalidDataException("Vector of prescribed values No. " + i + " is not defined (null reference).");
                    if (calc==null)
                        throw new InvalidDataException("Vector of calculated values No. " + i + " is not defined (null reference).");
                } else if (presc.Length != dimOutput)
                    throw new InvalidDataException("Vector of prescribed values No. " + i + " has wrong dimension ("
                        + presc.Length + " instead of " + dimOutput + ").");
                else if (calc.Length != dimOutput)
                    throw new InvalidDataException("Vector of calculated values No. " + i + " has wrong dimension ("
                        + calc.Length + " instead of " + dimOutput + ").");
                for (int k = 0; k < dimOutput; ++k)
                {
                    double dif = calc[k] - presc[k];
                    errors[k] += Math.Abs(dif);
                }
            }
            for (int k = 0; k < dimOutput; ++k)
            {
                if (numPoints > 0)
                    errors[k] /= (double)numPoints;
            }
        }

        /// <summary>Calculates error measures - maximum absolute value of the differences - for the specified arrays
        /// of prescribed and calculated output values in a set of sampling points.</summary>
        /// <param name="dimOutput">Dimension of output values.</param>
        /// <param name="prescribed">Array of prescribed output values (e.g. from training data, measurements, etc.).</param>
        /// <param name="calculated">Array of calculated output values (e.g. by a trained neural network).</param>
        /// <param name="errors">Vector where error measures for each indivitual output value are stored.</param>
        public static void CalculateErrorsMax(int dimOutput, IVector[] prescribed, IVector[] calculated, ref IVector errors)
        {
            if (prescribed == null || calculated == null)
            {
                if (prescribed == null)
                    throw new ArgumentNullException("Array of prescribed output values vectors is not specified (null reference).");
                else if (calculated == null)
                    throw new ArgumentNullException("Array of calculated output values vectors is not specified (null reference).");
            } else if (prescribed.Length != calculated.Length)
            {
                throw new ArgumentNullException("Number of calculated output vectors (" + calculated.Length 
                    + ") does not match the number of prescribed output vectors (" + prescribed.Length + ").");
            }
            int numPoints = prescribed.Length;
            if (numPoints < 1)
                throw new ArgumentException("Number of output points for error calculation is less than 1.");
            if (errors==null)
                Vector.Resize(ref errors, dimOutput);
            else if (errors.Length!=dimOutput)
                Vector.Resize(ref errors, dimOutput);
            errors.SetZero();
            for (int i=0; i<numPoints; ++i)
            {
                IVector presc = prescribed[i];
                IVector calc = calculated[i];
                if (presc==null || calc==null)
                {
                    if (presc == null)
                        throw new InvalidDataException("Vector of prescribed values No. " + i + " is not defined (null reference).");
                    if (calc==null)
                        throw new InvalidDataException("Vector of calculated values No. " + i + " is not defined (null reference).");
                } else if (presc.Length != dimOutput)
                    throw new InvalidDataException("Vector of prescribed values No. " + i + " has wrong dimension ("
                        + presc.Length + " instead of " + dimOutput + ").");
                else if (calc.Length != dimOutput)
                    throw new InvalidDataException("Vector of calculated values No. " + i + " has wrong dimension ("
                        + calc.Length + " instead of " + dimOutput + ").");
                for (int k = 0; k < dimOutput; ++k)
                {
                    double dif = calc[k] - presc[k];
                    double difAbs = Math.Abs(dif);
                    if (difAbs>errors[k])
                        errors[k] = difAbs;
                }
            }
        }



        private List<IVector> _prescribed = null; // auxiliary data for prepareErrorData
        private List<IVector> _calculated = null; // auxiliary data for prepareErrorData

        /// <summary>Prepares data for calculation of various error measures over training points or in 
        /// verification points after training of the neural network(s).</summary>
        /// <param name="dimOutput">Location where number (dimension) of output values is stored.</param>
        /// <param name="prescribed">Array where prescribed values from the training set are stored.</param>
        /// <param name="calculated">Array where calculated values in the corresponding points from the training set are stored.</param>
        /// <param name="takeTrainingPoints">Specifies whether training points (i.e., excludingverification points) 
        /// are taken from the training set to constitute the output data.</param>
        /// <param name="takeVerificationPoints">Specifies whether verification points are taken from the training set.</param>
        protected virtual void PrepareErrorData(ref int dimOutput, ref IVector[] prescribed, ref IVector[] calculated, 
            bool takeTrainingPoints, bool takeVerificationPoints)
        {
            lock (Lock)
            {
                dimOutput = OutputLength;
                if (_prescribed == null)
                    _prescribed = new List<IVector>();
                if (_calculated==null)
                    _calculated = new List<IVector>();
                _prescribed.Clear();
                _calculated.Clear();
                CalculateTrainingVerificationOutputs(takeTrainingPoints, takeVerificationPoints);
                if (takeTrainingPoints && !TrainingOutputsCalculated)
                    throw new InvalidOperationException("Can not calculate outputs in training points.");
                if (takeVerificationPoints && !VerificationOutputsCalculated)
                    throw new InvalidOperationException("Can not calculate outputs in verification points.");
                int numPointsTakenIntoAccount = 0;
                for (int i = 0; i < TrainingData.Length; ++i)
                {
                    bool takeThis = false;
                    bool isVerificationPoint = false;
                    if (VerificationIndices != null)
                        if (VerificationIndices.Contains(i))
                            isVerificationPoint = true;
                    if ((takeTrainingPoints && !isVerificationPoint) || (takeVerificationPoints && isVerificationPoint))
                        takeThis = true;
                    if (takeThis)
                    {
                        ++numPointsTakenIntoAccount;
                        _prescribed.Add(TrainingData.GetOutputValues(i));
                        _calculated.Add(_calculatedOutputs[i]);
                     }
                }
                prescribed = _prescribed.ToArray();
                calculated = _calculated.ToArray();
            }
        }


        /// <summary>Calculates the RMS (root mean square) of the errors of output values for the training 
        /// elements of the training set (this excludes verification points).</summary>
        /// <param name="errors"></param>
        public void GetErrorsTrainingRms(ref IVector errors)
        {
            bool takeTrainingPoints = true;
            bool takeVerificationPoints = false;
            int dimOutput = 0;
            IVector[] prescribed = null;
            IVector[] calculated = null;
            PrepareErrorData(ref dimOutput, ref prescribed, ref calculated, takeTrainingPoints, takeVerificationPoints);
            CalculateErrorsRms(dimOutput, prescribed, calculated, ref errors);
        }


        /// <summary>Calculates the RMS (root mean square) of the errors of output values for the verification 
        /// elements of the training set.</summary>
        /// <param name="errors"></param>
        public void GetErrorsVerificationRms(ref IVector errors)
        {
            bool takeTrainingPoints = false;
            bool takeVerificationPoints = true;
            int dimOutput = 0;
            IVector[] prescribed = null;
            IVector[] calculated = null;
            PrepareErrorData(ref dimOutput, ref prescribed, ref calculated, takeTrainingPoints, takeVerificationPoints);
            CalculateErrorsRms(dimOutput, prescribed, calculated, ref errors);
        }

        /// <summary>Calculates the maximum absolute errors of output values for the training 
        /// elements of the training set (this excludes verification points).</summary>
        /// <param name="errors"></param>
        public void GetErrorsTrainingMax(ref IVector errors)
        {
            bool takeTrainingPoints = true;
            bool takeVerificationPoints = false;
            int dimOutput = 0;
            IVector[] prescribed = null;
            IVector[] calculated = null;
            PrepareErrorData(ref dimOutput, ref prescribed, ref calculated, takeTrainingPoints, takeVerificationPoints);
            CalculateErrorsMax(dimOutput, prescribed, calculated, ref errors);
        }


        /// <summary>Calculates the maximum absolute errors of output values for the verification 
        /// elements of the training set.</summary>
        /// <param name="errors"></param>
        public void GetErrorsVerificationMax(ref IVector errors)
        {
            bool takeTrainingPoints = false;
            bool takeVerificationPoints = true;
            int dimOutput = 0;
            IVector[] prescribed = null;
            IVector[] calculated = null;
            PrepareErrorData(ref dimOutput, ref prescribed, ref calculated, takeTrainingPoints, takeVerificationPoints);
            CalculateErrorsMax(dimOutput, prescribed, calculated, ref errors);
        }

        /// <summary>Calculates the mean absolute errors of output values for the training 
        /// elements of the training set (this excludes verification points).</summary>
        /// <param name="errors"></param>
        public void GetErrorsTrainingMeanAbs(ref IVector errors)
        {
            bool takeTrainingPoints = true;
            bool takeVerificationPoints = false;
            int dimOutput = 0;
            IVector[] prescribed = null;
            IVector[] calculated = null;
            PrepareErrorData(ref dimOutput, ref prescribed, ref calculated, takeTrainingPoints, takeVerificationPoints);
            CalculateErrorsMeanAbs(dimOutput, prescribed, calculated, ref errors);
        }


        /// <summary>Calculates the mean absolute errors of output values for the verification 
        /// elements of the training set.</summary>
        /// <param name="errors"></param>
        public void GetErrorsVerificationMeanAbs(ref IVector errors)
        {
            bool takeTrainingPoints = false;
            bool takeVerificationPoints = true;
            int dimOutput = 0;
            IVector[] prescribed = null;
            IVector[] calculated = null;
            PrepareErrorData(ref dimOutput, ref prescribed, ref calculated, takeTrainingPoints, takeVerificationPoints);
            CalculateErrorsMeanAbs(dimOutput, prescribed, calculated, ref errors);
        }




        /// <summary>Gets or sets a flag indicating whether internal training data is prepared.
        /// This flag is used internally for signalization between methods that deal with training data.</summary>
        protected bool InternalTrainingDataPrepared
        {
            get { return _internalTrainingDataPrepared; }
            set { _internalTrainingDataPrepared = value; }
        }

        /// <summary>Whether network has been trained since the training data was set.</summary>
        public bool NetworkTrained
        {
            get { lock (Lock) { return _networkTrained; } }
            protected set
            {
                lock (Lock)
                {
                    // After training (either initial or additional) is performed or preceeding 
                    // training becomes invalid, dependent data must be invalidated:
                    InvalidateTrainingDependencies();
                    _networkTrained = value;
                }
            }
        }


        /// <summary>Prepares internal training data that is needed by the native training algorithm.
        /// When overridden, this method must set the <see cref="InternalTrainingDataPrepared"/> flag to true.</summary>
        protected abstract void PrepareInternalTrainingData();
        

        /// <summary>Invalidates all data that must be recalculated after training of the network is done.
        /// This method is called after training or additional training of the network is performed.
        /// Invalidation is achieved throughthe the appropriate flags.</summary>
        public virtual void InvalidateTrainingDependencies()
        {
            TrainingOutputsCalculated = false;
            VerificationOutputsCalculated = false;
            NetworkStateFilePath = null;
        }

        /// <summary>Invalidates all data that must be re-calculated after training data changes.
        /// This method is called after training data is modified.
        /// Invalidation is achieved throughthe the appropriate flags.</summary>
        public virtual void InvalidateTrainingDataDependencies()
        {
            InvalidateTrainingDependencies();
            NetworkTrained = false;
            InternalTrainingDataPrepared = false;
            EpochCount = 0;
        }

        /// <summary>Invalidates all data that must be re-calculated after the neural network itself changes.
        /// This method must be called after the internal neural network is re-defined (or are re-defined).
        /// Invalidation is achieved throughthe the appropriate flags.</summary>
        public virtual void InvalidateNetworkDependencies()
        {
            NetworkPrepared = false;
            InvalidateTrainingDataDependencies();
        }


        private bool _breakTraining = false;

        /// <summary>Flags that signalizes (if true) that training should be broken on external request.</summary>
        public bool BreakTraining
        {
            get { return _breakTraining; }
            set { _breakTraining = value; }
        }

        protected bool _calculateVerificationErrors = false;

        public bool CalculateVerificationErrors
        {
            get{ return _calculateVerificationErrors; }
            set { _calculateVerificationErrors = value; }
        }

        /// <summary>Trains neural network wiht the specified data, performing the specified number of epochs.</summary>
        /// <param name="numEpochs">Number of epochs used in training of the network.</param>
        public void TrainNetwork(int numEpochs) 
        {
            //ConvergenceRmsList = new List<IVector>();
            EpochNumbers = new List<int>();
            ConvergenceErrorsTrainingRmsList = new List<IVector>();
            ConvergenceErrorsTrainingMaxList = new List<IVector>();
            ConvergenceErrorsVerificationRmsList = new List<IVector>();
            ConvergenceErrorsVerificationMaxList = new List<IVector>();

            TrainNetworkSpecific(numEpochs);

            // Convergence
            if (SaveConvergenceRms)
            {
                //// Convergence of Rms errors calculated on training data.
                //IVector tmpConvergence1 = new Vector(OutputLength);
                //GetErrorsTrainingRms(ref tmpConvergence1);
                //ConvergenceRmsList.Add(tmpConvergence1);

                // Convergence of epochs
                int tmpEpochNumbers = 0;
                tmpEpochNumbers = EpochCount;
                EpochNumbers.Add(tmpEpochNumbers);

                // Convergence of Rms errors calculated on training data.
                IVector tmpConvergence = new Vector(OutputLength);
                GetErrorsTrainingRms(ref tmpConvergence);
                ConvergenceErrorsTrainingRmsList.Add(tmpConvergence);

                // Convergence of Maximum errors calculated on training data. 
                tmpConvergence = null;
                tmpConvergence = new Vector(OutputLength);
                GetErrorsTrainingMax(ref tmpConvergence);
                ConvergenceErrorsTrainingMaxList.Add(tmpConvergence);

                // Convergence of Rms errors calculated on verification data. 
                tmpConvergence = null;
                tmpConvergence = new Vector(OutputLength);
                GetErrorsVerificationRms(ref tmpConvergence);
                ConvergenceErrorsVerificationRmsList.Add(tmpConvergence);

                // Convergence of Maximim errors calculated on verification data. 
                tmpConvergence = null;
                tmpConvergence = new Vector(OutputLength);
                GetErrorsVerificationMax(ref tmpConvergence);
                ConvergenceErrorsVerificationMaxList.Add(tmpConvergence);
            }
        }

        /// <summary>Trains neural network wiht the specified data, performing the specified number of epochs.
        /// This method must be implemented in derived classes and is specific to specific network type.</summary>
        /// <param name="numEpochs">Number of epochs used in training of the network.</param>
        protected abstract void TrainNetworkSpecific(int numEpochs);
        
        /// <summary>Performs a specified number of training iterations where the specified number of epochs are run
        /// in each iteration.</summary>
        /// <param name="numEpochs">Number of epochs run in each iteration.</param>
        /// <param name="numIterations">Number of iterations.</param>
        public void TrainNetworkMultiple(int numEpochs, int numIterations)
        {
            BreakTraining = false;
            for (int i = 0; i < numIterations; ++i)
            {
                if (BreakTraining)
                    break;
                TrainNetworkSpecific(numEpochs);
            }
        }

        /// <summary>Performs a specified number of training iterations where the prescribed number of epochs 
        /// (contained in the EpochsInBundle property) are run in each iteration.</summary>
        /// <param name="numIterations">Number of iterations.</param>
        public void TrainNetworkMultiple(int NumIterations)
        {
            BreakTraining = false;
            TrainNetworkMultiple(EpochsInBundle, NumIterations);
        }

        /// <summary>Trains neural network until stopping criteria are met (in terms of errors and 
        /// number of epochs performed.</summary>
        public virtual void TrainNetwork()
        {
            BreakTraining = false;
            lock (Lock)
            {
                //ConvergenceRmsList = new List<IVector>();
                EpochNumbers = new List<int>();
                ConvergenceErrorsTrainingRmsList = new List<IVector>();
                ConvergenceErrorsTrainingMaxList = new List<IVector>();
                ConvergenceErrorsVerificationRmsList = new List<IVector>();
                ConvergenceErrorsVerificationMaxList = new List<IVector>();

                while (!StopTrainingCriteriaMet())
                {
                    if (BreakTraining)
                        break;
                    TrainNetworkSpecific(EpochsInBundle);
                    
                    // Convergence
                    if (SaveConvergenceRms)
                    {
                        //// Convergence of Rms errors calculated on training data.
                        //IVector tmpConvergence1 = new Vector(OutputLength);
                        //GetErrorsTrainingRms(ref tmpConvergence1);
                        //ConvergenceRmsList.Add(tmpConvergence1);

                        // Convergence of epochs
                        int tmpEpochNumbers = 0;
                        tmpEpochNumbers = EpochCount;
                        EpochNumbers.Add(tmpEpochNumbers);

                        // Convergence of Rms errors calculated on training data.
                        IVector tmpConvergence = new Vector(OutputLength);
                        GetErrorsTrainingRms(ref tmpConvergence);
                        ConvergenceErrorsTrainingRmsList.Add(tmpConvergence);

                        // Convergence of Maximum errors calculated on training data. 
                        tmpConvergence = null;
                        tmpConvergence = new Vector(OutputLength);
                        GetErrorsTrainingMax(ref tmpConvergence);
                        ConvergenceErrorsTrainingMaxList.Add(tmpConvergence);

                        // Convergence of Rms errors calculated on verification data. 
                        tmpConvergence = null;
                        tmpConvergence = new Vector(OutputLength);
                        GetErrorsVerificationRms(ref tmpConvergence);
                        ConvergenceErrorsVerificationRmsList.Add(tmpConvergence);

                        // Convergence of Maximim errors calculated on verification data. 
                        tmpConvergence = null;
                        tmpConvergence = new Vector(OutputLength);
                        GetErrorsVerificationMax(ref tmpConvergence);
                        ConvergenceErrorsVerificationMaxList.Add(tmpConvergence);
                    }
                }
            }
        }

        protected Vector _auxErrors = null;  // auxiliary vector for calculation of errors

        /// <summary>Returns true if the stopping criteria for training is met, with respect to current settings,
        /// errors and number of epochs already performed, and false otherwise.</summary>
        public virtual bool StopTrainingCriteriaMet()
        {
            lock (Lock)
            {
                bool doStop = false;
                if (EpochCount >= MaxEpochs)
                {
                    doStop = true;
                } else
                {
                    bool toleranceNotMet = false;
                    bool tolerancesChecked = false;
                    if (ToleranceRms != null)
                    {
                        try
                        {
                            if (_auxErrors == null)
                                _auxErrors = new Vector(OutputLength);
                            IVector errorsRms = _auxErrors;
                            GetErrorsTrainingRms(ref errorsRms);
                            if (errorsRms == null)
                                throw new InvalidDataException("Vector of calculated RMS errors is null.");
                            else if (errorsRms.Length != OutputLength)
                                throw new InvalidDataException("Vector of calculated RMS errors has wrong dimension, "
                                    + errorsRms.Length + " instead of " + OutputLength + ".");
                            else
                            {
                                for (int i = 0; i < errorsRms.Length; ++i)
                                {
                                    if (ToleranceRms[i] > 0 && errorsRms[i] > ToleranceRms[i])
                                    {
                                        toleranceNotMet = true;
                                        break;
                                    }
                                }
                                tolerancesChecked = true;
                            }
                        }
                        catch (Exception) { }
                    }
                    if (ToleranceMax != null && !toleranceNotMet)
                    {
                        try
                        {
                            if (_auxErrors == null)
                                _auxErrors = new Vector(OutputLength);
                            IVector errorsMax = _auxErrors;
                            GetErrorsTrainingMax(ref errorsMax);
                            if (errorsMax == null)
                                throw new InvalidDataException("Vector of calculated maximal errors is null.");
                            else if (errorsMax.Length != OutputLength)
                                throw new InvalidDataException("Vector of calculated maximal errors has wrong dimension, "
                                    + errorsMax.Length + " instead of " + OutputLength + ".");
                            else
                            {
                                for (int i = 0; i < errorsMax.Length; ++i)
                                {
                                    if (ToleranceMax[i] > 0 && errorsMax[i] > ToleranceMax[i])
                                    {
                                        toleranceNotMet = true;
                                        break;
                                    }
                                }
                                tolerancesChecked = true;
                            }
                        }
                        catch (Exception) { }
                    }
                    if (tolerancesChecked && !toleranceNotMet)
                        doStop = true;
                }
                return doStop;
            }
        }

        #endregion Training

        #region Calculation 

        ///// <summary>Calculates and returns the approximated outputs corresponding to the specified inputs,
        ///// by using the current neural network.</summary>
        ///// <param name="input">Input parameters.</param>
        ///// <returns>Vector of output values generated by the trained neural network.</returns>
        ///// <remarks>Currently, only all outputs at once can be calculated. This makes no difference
        ///// in the arrangement with a single network with multiple outputs, but does when several 
        ///// networks with single output each are used. If the implementation changes in the future
        ///// then performance configuratins should be taken into account carefully, and tracking input
        ///// for which input parameters the outputs have been calculated might be necessary.</remarks>
        //public abstract void CalculateOutput(IVector input, ref IVector output);


        private IVector _output = null; // auxiliary for CalculateOutput(...)

        private IVector _outputFiltered1 = null;  // auxiliary for CalculateOutput(...)

        /// <summary>Calculates and returns the specified output by using the neural network.</summary>
        public override double CalculateOutput(IVector input, int whichElement)
        {
            lock (Lock)
            {
                if (_outputFiltered1 == null)
                    _outputFiltered1 = new Vector(1);
                int[] elementIndices = new int[] { whichElement };
                CalculateOutput(input, elementIndices, ref _outputFiltered1);
                if (_outputFiltered1 == null)
                    throw new InvalidDataException("Invalid output value element, storage reference is null.");
                else if (_outputFiltered1.Length != 1)
                    throw new InvalidDataException("Invalid output value element, data length should be 1.");
                else
                    return _outputFiltered1[0];
            }
        }

        /// <summary>Calculates and returns the required output values corresponding to the specified inputs,
        /// by using the current neural network(s).</summary>
        /// <param name="input">Input parameters for which output values are calculated.</param>
        /// <param name="indices">Array of indices of the output values to be returned.</param>
        /// <param name="filteredOutput">Vector where filtered output values are stored.</param>
        public override void CalculateOutput(IVector input, int[] indices, ref IVector filteredOutput)
        {
            lock (Lock)
            {
                // Remark:
                // Implementation of this function is not very good, especially for cases where multiple neural networks are used. 
                // Currently it calculates all outputs and then filters it  
                // according to the specified indices, while it should calculate only those outputs that are necessary.
                if (indices == null)
                    filteredOutput = null;
                else if (indices.Length == 0)
                    filteredOutput = null;
                else
                {
                    if (_output == null)
                        _output = new Vector(OutputLength);
                    CalculateOutput(input, ref _output);
                    if (_output == null)
                        throw new InvalidDataException("Calculated approximated values are wrong - null vector.");
                    else
                    {
                        Vector.Resize(ref filteredOutput, indices.Length);
                        for (int i = 0; i < indices.Length; ++i)
                        {
                            if (indices[i] < 0 || indices[i] >= _output.Length)
                                throw new InvalidDataException("Output value index no. " + i + " is out of range ("
                                + indices[i] + ", should be between " + 0 + " and " + (_output.Length - 1).ToString() + ").");
                            filteredOutput[i] = _output[indices[i]];
                        }
                    }
                }
            } // lock
        }  // CalculateOutput(...)

        #endregion Calculation

        #endregion Operation

        #region Misc

        /// <summary>Returns string describing the current neural network approximator.</summary>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Training  related parameters: ");
            sb.AppendLine("  Learning rate:        " + this.LearningRate);
            sb.AppendLine("  Sigmoid alpha value:  " + this.SigmoidAlphaValue);
            sb.AppendLine("  Momentum:             " + this.Momentum);
            sb.AppendLine("Training - stopping criteria:");
            sb.AppendLine("  Max. number of epochs:   " + this.MaxEpochs);
            sb.AppendLine("  Tolerance in RMS error:  " + this.ToleranceRms);
            sb.AppendLine("  Tolerance in max. error: " + this.ToleranceMax);
            sb.AppendLine("Data mapping parameters:");
            sb.AppendLine("  Input bounds safety factor:  " + this.InputBoundsSafetyFactor);
            sb.AppendLine("  Output bounds safety factor: " + this.OutputBoundsSafetyFactor);
            sb.AppendLine("  Input data bounds: ");
            sb.AppendLine("  " + this.InputDataBounds.ToString());
            sb.AppendLine("  Output data bounds: ");
            sb.AppendLine("  " + this.OutputDataBounds.ToString());
            sb.AppendLine(  "Neurons' input range: ");
            sb.AppendLine("  " + this.InputNeuronsRange);
            sb.AppendLine("  Neuron's output range: ");
            sb.AppendLine("  " + this.OutputNeuronsRange);
            //sb.AppendLine();

            return sb.ToString(); 
        }

        #endregion Misc

        #region StaticMethods

        /// <summary>Saves a neural network approximator to a file.
        /// If the neural netwoek is trained then internal state is also saved to a file.</summary>
        /// <param name="approximator">Neural network approximator to be saved.</param>
        /// <param name="filePath">Path to the file where approximator is saved.</param>
        public static void SaveJson(INeuralApproximator approximator, string filePath)
        {
            SaveJson(approximator, filePath, true);
        }

        /// <summary>Saves a neural network approximator to a file.</summary>
        /// <param name="approximator">Neural network approximator to be saved.</param>
        /// <param name="filePath">Path to the file where approximator is saved.</param>
        /// <param name="saveInternalState">Specifies whether internal state should be saved, too (only in the case that network is trained).</param>
        public static void SaveJson(INeuralApproximator approximator, string filePath, bool saveInternalState)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentException("File path for saving neural network is not specified (null or empty string).");
            approximator.NetworkStateRelativePath = null;
            if (saveInternalState && approximator.NetworkTrained)
            {
                // Save internal state to the specified relative path.
                string dirPath = Path.GetDirectoryName(filePath);
                string fileName = Path.GetFileNameWithoutExtension(filePath);
                fileName = fileName + "_internal.dat";
                string internalPath = Path.Combine(dirPath,fileName);
                approximator.SaveNetwork(internalPath);
                // Calculate and store relative file path of the network internal state.
                if (!string.IsNullOrEmpty(approximator.NetworkStateFilePath))
                    approximator.NetworkStateRelativePath = UtilSystem.GetRelativePath(dirPath, approximator.NetworkStateFilePath);
            }
            NeuralApproximatorDtoBase dto = new NeuralApproximatorDtoBase();
            dto.CopyFrom(approximator);
            ISerializer serializer = new SerializerJson();
            string neuralApproximationPath = filePath;
            serializer.Serialize<NeuralApproximatorDtoBase>(dto, neuralApproximationPath);

        }

        /// <summary>Loads network from a file.</summary>
        /// <param name="filePath">Path to the file.</param>
        /// <param name="approximatorRestored">Neural approximator that is produced by deserialization.</param>
        public static void LoadJson(string filePath, ref INeuralApproximator approximatorRestored)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentException("File path for restoring neural network is not specified (null or empty string).");
            ISerializer serializerRestoring = new SerializerJson();
            NeuralApproximatorDtoBase dtoRestored = serializerRestoring.DeserializeFile<NeuralApproximatorDtoBase>
                (filePath);
            approximatorRestored = new NeuralApproximatorAforgeFake();

            dtoRestored.CopyTo(ref approximatorRestored);
            if (!string.IsNullOrEmpty(approximatorRestored.NetworkStateRelativePath))
            {
                try
                {
                    string pathInternal = Path.Combine(Path.GetDirectoryName(filePath), approximatorRestored.NetworkStateRelativePath);
                    approximatorRestored.LoadNetwork(pathInternal);
                }
                catch (Exception ex) 
                {
                    Console.WriteLine();
                    Console.WriteLine("ERROR when loading internal network state (relative path): ");
                    Console.WriteLine("  " + ex.Message);
                    Console.WriteLine();
                }
            }
            approximatorRestored.NetworkStateRelativePath = null;
        }


        /// <summary>Saves network's training data to the specified JSON file.
        /// File is owerwritten if it exists.</summary>
        /// <param name="filePath">Path to the file where training data is saved.</param>
        public void SaveTrainingDataJson_To_delete(string filePath)
        {
            lock (Lock)
            {
                SampledDataSetDto dtoOriginal = new SampledDataSetDto();
                dtoOriginal.CopyFrom(TrainingData);
                ISerializer serializer = new SerializerJson();
                serializer.Serialize<SampledDataSetDto>(dtoOriginal, filePath);
            }
        }

        /// <summary>Restores training data from the specified file in JSON format.</summary>
        /// <param name="filePath">File from which training data is restored.</param>
        public void LoadTrainingDataJson_To_Delete(string filePath)
        {
            lock (Lock)
            {
                ISerializer serializer = new SerializerJson();
                SampledDataSetDto dtoRestored = serializer.DeserializeFile<SampledDataSetDto>(filePath);
                SampledDataSet dataRestored = TrainingData;
                dtoRestored.CopyTo(ref dataRestored);
                TrainingData = dataRestored;
            }
        }


        #endregion StaticMethods


        #region Tests


        /// <summary>Test back and forth mapping (scaling and shifting) from actual data to data prepared for training, 
        /// and vice versa (checks if transformed data falls withi the prescribed ranges and if backward transformation
        /// yields the same result as forward transformation). 
        /// Testing is performed on all data from the TrainingData property, including verification elements.</summary>
        public void TestMapping()
        {
            Console.WriteLine();
            Console.WriteLine("Test of mapping of input parameters and output values from the training set: ");
            if (TrainingData == null)
                Console.WriteLine("Training data is not specified (null reference).");
            else
            {
                double smallValue = 1.0e-8;
                double toleranceInput = 1.0e-8;
                double toleranceOutput = 1.0e-8;
                IVector[] inputs = null;
                IVector[] outputs = null;
                IVector[] scaledInputs = null;
                IVector[] scaledOutputs = null;
                IVector[] backScaledInputs = null;
                IVector[] backScaledOutputs = null;
                int numElements = TrainingData.Length;
                // Store vectors of input parameters and output values from the training data:
                TrainingData.ExtractInputs(ref inputs);
                TrainingData.ExtractOutputs(ref outputs);
                // Map (scale & shift) inputs and outputs:
                scaledInputs = new IVector[numElements];
                scaledOutputs = new IVector[numElements];
                Console.WriteLine("Scaling input and output values to neural range:");
                Console.WriteLine();
                bool allOK = true;

                BoundingBox actualInputNeuronsRange = new BoundingBox(TrainingData.InputLength);
                BoundingBox actualOutputNeuronsRange = new BoundingBox(TrainingData.OutputLength);

                    for (int i = 0; i < numElements; ++i)
                    {
                        inputs[i] = TrainingData.GetInputParameters(i);
                        outputs[i] = TrainingData.GetOutputValues(i);
                        MapInput(inputs[i], ref scaledInputs[i]);
                        MapOutput(outputs[i], ref scaledOutputs[i]);
                        actualInputNeuronsRange.Update(scaledInputs[i]);
                        actualOutputNeuronsRange.Update(scaledOutputs[i]);

                        bool isVerification = false;
                        if (VerificationIndices != null)
                            if (VerificationIndices.Contains(i))
                                isVerification = true;
                        bool inputOutOfRange = InputNeuronsRange.LiesOutside(scaledInputs[i]);
                        bool outpuOutOfRange = OutputNeuronsRange.LiesOutside(scaledOutputs[i]);
                        if ((inputOutOfRange || outpuOutOfRange))
                        {
                            if (inputOutOfRange)
                            {
                                allOK = false;
                                Console.WriteLine("Error: input data for element No. " + i + " not in range after scaling: ");
                                Console.WriteLine("  " + scaledInputs[i]);
                                Console.WriteLine("  Is verification point: " + isVerification);
                            }
                            if (outpuOutOfRange)
                            {
                                allOK = false;
                                Console.WriteLine("Error: output data for element No. " + i + " not in range after scaling: ");
                                Console.WriteLine("  " + scaledOutputs[i]);
                                Console.WriteLine("  Is verification point: " + isVerification);
                            }
                        }

                }


                Console.WriteLine();
                Console.WriteLine("Actual bounds for neural inputs: ");
                Console.WriteLine("  " + actualInputNeuronsRange.ToString());
                Console.WriteLine("Actual bounds for neural outputs: ");
                Console.WriteLine("  " + actualOutputNeuronsRange.ToString());
                Console.WriteLine();

                if (allOK)
                {
                    Console.WriteLine("All inputs and outputs are in range after scaling.");
                    Console.WriteLine();
                } else
                {
                    Console.WriteLine("Input neurons - lower bounds: ");
                    Console.WriteLine("  " + InputNeuronsRange.Min);
                    Console.WriteLine("Input neurons - upper bounds: ");
                    Console.WriteLine("  " + InputNeuronsRange.Max);
                    Console.WriteLine("Output neurons - lower bounds: ");
                    Console.WriteLine("  " + OutputNeuronsRange.Min);
                    Console.WriteLine("Output neurons - upper bounds: ");
                    Console.WriteLine("  " + OutputNeuronsRange.Max);
                }
                Console.WriteLine("Mapping back from neural to real input/output:");
                backScaledInputs = new IVector[numElements];
                backScaledOutputs = new IVector[numElements];
                IVector diffInput = new Vector(InputLength);
                IVector diffOutput = new Vector(OutputLength);
                double 
                    normInput,
                    normOutput,
                    normDiffInput, 
                    normDiffOutput;
                double sumDiffNormsInput = 0.0;
                double sumDiffNormsOutput = 0.0;
                allOK = true;

                    for (int i = 0; i < numElements; ++i)
                    {
                        MapFromNeuralInput(scaledInputs[i], ref backScaledInputs[i]);
                        MapFromNeuralOutput(scaledOutputs[i], ref backScaledOutputs[i]);
                        Vector.Subtract(inputs[i], backScaledInputs[i], ref diffInput);
                        Vector.Subtract(outputs[i], backScaledOutputs[i], ref diffOutput);
                        normInput = inputs[i].Norm;
                        normOutput = outputs[i].Norm;
                        normDiffInput = diffInput.Norm;
                        normDiffOutput = diffOutput.Norm;
                        sumDiffNormsInput += normDiffInput;
                        sumDiffNormsOutput += normDiffOutput;
                        double relErrInput = normDiffInput / (normInput + smallValue);
                        double relErrOutput = normDiffOutput / (normOutput + smallValue);
                        if (relErrInput > toleranceInput)
                        {
                            allOK = false;
                            Console.WriteLine("Error: false mapping forth and back for input " + i + ", rel. error: " + relErrInput);
                            Console.WriteLine("  Orig. : " + inputs[i]);
                            Console.WriteLine("  Mapped: " + scaledInputs[i]);
                            Console.WriteLine("  Backm.:" + backScaledInputs[i]);
                        }
                        if (relErrOutput > toleranceOutput)
                        {
                            allOK = false;
                            Console.WriteLine("Error: false mapping forth and back for output " + i + ", rel. error: " + relErrOutput);
                            Console.WriteLine("  Orig. : " + outputs[i]);
                            Console.WriteLine("  Mapped: " + scaledOutputs[i]);
                            Console.WriteLine("  Backm.:" + backScaledOutputs[i]);
                        }

                }
                if (allOK)
                {
                    Console.WriteLine("Mapping (scaling and shifting) back and forth is OK.");
                    Console.WriteLine("Sum of norms of differences between original and mapped forth and back: ");
                    Console.WriteLine("  * for input parameters: " + sumDiffNormsInput);
                    Console.WriteLine("  * for output values:    " + sumDiffNormsOutput);
                }

            }
        }


        #endregion Tests

        #region Examples

        /// <summary>Example of saving an entire trained neural network to a file, and then restoring it from a file.
        /// Network internal state is saved by the SaveNetwork() method that is specific to the type of the network,
        /// therefore it is saved to a separate file. The path of this file is savad with the network approximator object itself.
        /// Network is saved only once.</summary>
        /// <param name="directoryPath">Directory where the neural network is saved.</param>
        /// <param name="fileName">Name o the file into which the network is saved.</param>
        /// <param name="internalStateFileName">Name of the file where the state of the trained network 
        /// (internal representation) is stored.</param>
        public static void ExampleSaveNetwork(string directoryPath, string fileName, string internalStateFileName)
        {
            ExampleSaveNetwork(directoryPath, fileName, internalStateFileName, false /* saveRestored */);
        }

        /// <summary>Example of saving an entire trained neural network to a file, and then restoring it from a file.
        /// Network internal state is saved by the SaveNetwork() method that is specific to the type of the network,
        /// therefore it is saved to a separate file. The path of this file is savad with the network approximator object itself.
        /// If the <paramref name="saveRestored"/> flag parameter is true then the restored file ia saved again for comparison.</summary>
        /// <param name="directoryPath">Directory where the neural network is saved.</param>
        /// <param name="fileName">Name o the file into which the network is saved.</param>
        /// <param name="internalStateFileName">Name of the file where the state of the trained network 
        /// (internal representation) is stored.</param>
        /// <param name="saveRestored">If true then the restored neural network is saved again for comparison with the
        /// file where original was saved.</param>
        public static void ExampleSaveNetwork(string directoryPath, string fileName, string internalStateFileName,
            bool saveRestored)
        {
            double
                tolerance = 1.0e-8,
                smallValue = 1.0e-10;
            int numInputs = 3;
            int numOutputs = 1;
            int numAdditionalEpochs = 500;
            if (!Directory.Exists(directoryPath))
                throw new ArgumentException("Directory does not exist: " + directoryPath + ".");
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentException("Name of the file for storing the neural network approximator object not specified.");
            if (string.IsNullOrEmpty(internalStateFileName))
                throw new ArgumentException("Name of the file for storing the internal state of the neural network not specified.");
            Console.WriteLine();
            Console.WriteLine("Test of saving and restoring neural network approximator to/from file.");
            Console.WriteLine("Creating an example neural network and training it wiht generated data...");
            Console.WriteLine("  number of inputs: " + numInputs + ", number of outputs: " + numOutputs);
            
            INeuralApproximator approximator = ExampleQuadratic(numInputs, numOutputs, 0 /* outpuLevel */, 500 /* maxEpochs */);

            // double a = neural.GetErrorsTrainingR

            IVector errTrainRmsFirst = new Vector(approximator.OutputLength);
            approximator.GetErrorsTrainingRms(ref errTrainRmsFirst);
            IVector errVerificationRmsFirst = new Vector(approximator.OutputLength);
            approximator.GetErrorsVerificationRms(ref errVerificationRmsFirst);
            Console.WriteLine("... creation and first training done, errors calculated and stored.");
            Console.WriteLine();
            
            Console.WriteLine();
            Console.WriteLine("Performing additional training, " + numAdditionalEpochs + " additional epochs...");
            approximator.TrainNetwork(numAdditionalEpochs);
            IVector errTrainRmsFinal = new Vector(approximator.OutputLength);
            approximator.GetErrorsTrainingRms(ref errTrainRmsFinal);
            IVector errVerificationRmsFinal = new Vector(approximator.OutputLength);
            approximator.GetErrorsVerificationRms(ref errVerificationRmsFinal);
            Console.WriteLine("... additional training done, errors calculated and stored.");

            Console.WriteLine("Saving internal state of the neural network... ");
            approximator.SaveNetwork(Path.Combine(directoryPath, internalStateFileName));
            Console.WriteLine("... saving internal state done.");
            Console.WriteLine("File path: " + approximator.NetworkStateFilePath);
            Console.WriteLine();
            Console.WriteLine("Saving the entire network approximator...");
            string neuralApproximationPath = Path.Combine(directoryPath, fileName);
            ISerializer serializer = new SerializerJson();
            //NeuralApproximatorDtoBase dto = new NeuralApproximatorDtoBase();
            //dto.CopyFrom(approximator);
            //serializer.Serialize<NeuralApproximatorDtoBase>(dto, neuralApproximationPath);
            //approximator = null;
            NeuralApproximatorBase.SaveJson(approximator, neuralApproximationPath);
            Console.WriteLine("... saving the entire network done.");

            Console.WriteLine();
            Console.WriteLine("Restoring neural network approximator from a file...");
            ISerializer serializerRestoring = new SerializerJson();
            INeuralApproximator approximatorRestored = null; //  new NeuralApproximatorAforge(); ;
            //NeuralApproximatorDtoBase dtoRestored = serializerRestoring.DeserializeFile<NeuralApproximatorDtoBase>
            //    (Path.Combine(directoryPath, fileName));
            //dtoRestored.CopyTo(ref approximatorRestored);
            NeuralApproximatorBase.LoadJson(Path.Combine(directoryPath, fileName), ref approximatorRestored);

            Console.WriteLine("Internal state has  been restored from the following file: ");
            Console.WriteLine("  " + approximatorRestored.NetworkStateFilePath);
            if (saveRestored)
            {
                Console.WriteLine();
                Console.WriteLine("Writing restored network to a verification file... ");
                string internalStateFilenameRestored = Path.GetFileNameWithoutExtension(internalStateFileName)
                    + "_restored" + Path.GetExtension(internalStateFileName);
                string pathInternalStateRestored = Path.Combine(directoryPath, internalStateFilenameRestored);
                approximatorRestored.SaveNetwork(pathInternalStateRestored);
                Console.WriteLine("  Internal state saved again to the following file: ");
                Console.WriteLine("  " + approximatorRestored.NetworkStateFilePath);
                NeuralApproximatorDtoBase dtoRestoredSave = new NeuralApproximatorDtoBase();
                dtoRestoredSave.CopyFrom(approximatorRestored);
                ISerializer serializerRestored = new SerializerJson();
                string fileNameRestored = Path.GetFileNameWithoutExtension(fileName) + "_restored" + Path.GetExtension(fileName);
                string pathRestored = Path.Combine(directoryPath, fileNameRestored);
                serializer.Serialize<NeuralApproximatorDtoBase>(dtoRestoredSave, pathRestored);
                Console.WriteLine("  Complete network saved again to the following file: ");
                Console.WriteLine("  " + pathRestored);
                Console.WriteLine("... Saving restored network to verification file done.");
                Console.WriteLine();
            }

            //Console.WriteLine("Calculating approximation in the first training point: ");
            //IVector testInput = approximatorRestored.TrainingData.GetInputParameters(0);
            //IVector testOutput = null;
            //approximatorRestored.CalculateOutput(testInput, ref testOutput);
            //Console.WriteLine("  Input parameters: ");
            //Console.WriteLine("  " + testInput);
            //Console.WriteLine("  Calculated output values: ");
            //Console.WriteLine("  " + testOutput);

            // Calculate RMS errors over training and verification points by the restored neural network:
            IVector errTrainRmsRestored = new Vector(approximatorRestored.OutputLength);
            approximatorRestored.GetErrorsTrainingRms(ref errTrainRmsRestored);
            IVector errVerificationRmsRestored = new Vector(approximatorRestored.OutputLength);
            approximatorRestored.GetErrorsVerificationRms(ref errVerificationRmsRestored);
            Console.WriteLine("...restoring neural network approximator from a file done, errors calculated.");
            //Console.WriteLine();
            //Console.WriteLine("Neural network approximator after restoring from a file: ");
            //Console.WriteLine(approximatorRestored.ToString());
            Console.WriteLine();
            Console.WriteLine("Errors over training set after various stages:");
            Console.WriteLine("After the first training of the original network: ");
            Console.WriteLine("  RMS error in training points: ");
            Console.WriteLine("  " + errTrainRmsFirst);
            Console.WriteLine("  RMS error in verification points: ");
            Console.WriteLine("  " + errVerificationRmsFirst);
            Console.WriteLine("After the second training of the original network: ");
            Console.WriteLine("  RMS error in training points: ");
            Console.WriteLine("  " + errTrainRmsFinal);
            Console.WriteLine("  RMS error in verification points: ");
            Console.WriteLine("  " + errVerificationRmsFinal );
            Console.WriteLine("After restoring approximator (with a trained network) from a file: ");
            Console.WriteLine("  RMS error in training points: ");
            Console.WriteLine("  " + errTrainRmsRestored);
            Console.WriteLine("  RMS error in verification points: ");
            Console.WriteLine("  " + errVerificationRmsRestored);
            Console.WriteLine();
            Console.WriteLine();
            IVector diff = new Vector(approximatorRestored.OutputLength);
            double relativeError = 0.0;
            Vector.Subtract(errTrainRmsRestored, errTrainRmsFinal, ref diff);
            relativeError = diff.Norm / (errTrainRmsFinal.Norm + smallValue);
            if (relativeError > tolerance)
            {
                Console.WriteLine("ERROR: RMS error over training points does not match " + Environment.NewLine
                    + "between the original and restored network.");
                Console.WriteLine("  Relative error: " + relativeError);
                Console.WriteLine("  Original: " + errTrainRmsFinal);
                Console.WriteLine("  Restored: " + errTrainRmsRestored);
            } else
            {
                Console.WriteLine("RMS error over training points: original and restored networks match.");
                Console.WriteLine("  Relative error: " + relativeError);
            }
            Vector.Subtract(errVerificationRmsRestored, errVerificationRmsFinal, ref diff);
            relativeError = diff.Norm / (errVerificationRmsFinal.Norm + smallValue);
            if (relativeError > tolerance)
            {
                Console.WriteLine("ERROR: RMS error over training points does not match " + Environment.NewLine
                    + "between the original and restored network.");
                Console.WriteLine("  Relative error: " + relativeError);
                Console.WriteLine("  Original: " + errVerificationRmsFinal);
                Console.WriteLine("  Restored: " + errVerificationRmsRestored);
            } else
            {
                Console.WriteLine("RMS error over verification points: original and restored networks match.");
                Console.WriteLine("  Relative error: " + relativeError);
            }
            Console.WriteLine();

            // string networkType = approximatorRestored.GetType().ToString();
            Console.WriteLine("Network type: " + approximatorRestored.GetType());


        }


        /// <summary>Example demonstrating usage of the neural network approximator.
        /// A quadratic function with random coefficients is sampled with enough samples to exactly
        /// specify function coefficients, a part of samples is randomly designated as verification points, 
        /// then neural network is created and trained on training samples, and it is verified how close
        /// the obtained approximation matches actual function in verification points.</summary>
        /// <returns>The trained neural network approximator that includes the training data, and can be
        /// saved to a file and used at a later time.</returns>
        public static INeuralApproximator ExampleQuadratic()
        {
            return ExampleQuadratic(3,1);
        }


        /// <summary>Example demonstrating usage of the neural network approximator.
        /// A quadratic function with random coefficients is sampled with enough samples to exactly
        /// specify function coefficients, a part of samples is randomly designated as verification points, 
        /// then neural network is created and trained on training samples, and it is verified how close
        /// the obtained approximation matches actual function in verification points.</summary>
        /// <param name="inputLength">Number of input parameters of the neural network.</param>
        /// <param name="outputLength">Number of approximated output values of the neural network.</param>
        /// <returns>The trained neural network approximator that includes the training data, and can be
        /// saved to a file and used at a later time.</returns>
        public static INeuralApproximator ExampleQuadratic(int inputLength, int outputLength)
        {
            return ExampleQuadratic(inputLength, outputLength, -1 /* outputLevel internally specified */, -1 /* int maxEpochs */);
        }


        /// <summary>Example demonstrating usage of the neural network approximator.
        /// A quadratic function with random coefficients is sampled with enough samples to exactly
        /// specify function coefficients, a part of samples is randomly designated as verification points, 
        /// then neural network is created and trained on training samples, and it is verified how close
        /// the obtained approximation matches actual function in verification points.</summary>
        /// <param name="inputLength">Number of input parameters of the neural network.</param>
        /// <param name="outputLength">Number of approximated output values of the neural network.</param>
        /// <param name="outputLevel">Level of output written to console (0 - no output).</param>
        /// <param name="MaxEpochs">If greater than 0 then this is the maximal number of epochs used for training.</param>
        /// <returns>The trained neural network approximator that includes the training data, and can be
        /// saved to a file and used at a later time.</returns>
        public static INeuralApproximator ExampleQuadratic(int inputLength, int outputLength, int outputLevel, int maxEpochs)
        {
            // Specify reasonable number of samples and verification points:
            int numTrainingElements = 2 * inputLength * inputLength;
            int numVerificationPoints = (int) Math.Round((double) numTrainingElements * 0.5);
            numTrainingElements += numVerificationPoints;
            // Create training data by randomly sampling a specific quadratic response:
            SampledDataSet samples = SampledDataSet.CreateExampleQuadratic(inputLength, outputLength, numTrainingElements);
            // Speciy which samples will be used for verification of approximation:
            IndexList verificationIndices = IndexList.CreateRandom(numVerificationPoints, 0 /* lowerbound */, numTrainingElements - 1);
            // Create neural network and set basic parameters:
            NeuralApproximatorBase neural = new NeuralApproximatorAforgeFake();
            lock (neural.Lock)
            {
                if (outputLevel < 0)
                    outputLevel = 2;
                neural.OutputLevel = outputLevel;
                neural.InputLength = inputLength;
                neural.OutputLength = outputLength;
                // Set prepared data:
                neural.TrainingData = samples;
                // neural.LoadTrainingDataJson("../../testdata/train.json");
                neural.VerificationIndices = verificationIndices;
                // Set network layout:
                neural.MultipleNetworks = true;
                neural.SetHiddenLayers(20, 20);
                // Set training parameters:
                if (maxEpochs < 0)
                    maxEpochs = 20000;
                neural.MaxEpochs = maxEpochs;
                neural.EpochsInBundle = 1000;
                neural.ToleranceRms = new Vector(outputLength, 0.001);

                // Specify learning parameters:
                neural.LearningRate = 0.08;     // ok: 0.1
                neural.SigmoidAlphaValue = 1.3;  // ok: 1.3
                neural.Momentum = 0.0;   // ok: 0

                // Specify parameters defining the bounds for data applied to input and output neurons:
                neural.InputBoundsSafetyFactor = 1.5;
                neural.OutputBoundsSafetyFactor = 1.5;

                // Change the targeted range of input and output neurons: 
                neural.InputNeuronsRange.Reset();
                neural.InputNeuronsRange.UpdateAll(-2.0, 2.0);
                neural.OutputNeuronsRange.Reset();
                neural.OutputNeuronsRange.UpdateAll(0.1, 0.9);


                // Test output (for debugging purposes):
                if (outputLevel > 0)
                {
                    Console.WriteLine();
                    Console.WriteLine("Neural network data: ");
                    Console.WriteLine(neural.ToString());
                    //Console.WriteLine("Insert <Enter> in order to continue: ");
                    //Console.ReadLine();

                    // Test whether mapping works correctly:
                    neural.TestMapping();
                }

                // Perform training: 
                neural.TrainNetwork();

                if (outputLevel > 0)
                {
                    // Calculate outputs and exact values for first 5 points in the training set:
                    Console.WriteLine("A couple of calculated outputs from the training set (including verification points):");
                    for (int i = 0; i < 5; ++i)
                    {
                        IVector input = samples.GetInputParameters(i);
                        IVector exactOutput = samples.GetOutputValues(i);
                        IVector calculatedOutput = new Vector(outputLength);
                        neural.CalculateOutput(input, ref calculatedOutput);
                        Console.WriteLine();
                        Console.WriteLine("Point No. " + i + " of the training set, is verification point: " + verificationIndices.Contains(i));
                        Console.WriteLine("  Input parameters No. " + i + ":");
                        Console.WriteLine("  " + input.ToString());
                        Console.WriteLine("  Exact output: ");
                        Console.WriteLine("  " + exactOutput.ToString());
                        Console.WriteLine("  Approximated output: ");
                        Console.WriteLine("  " + calculatedOutput.ToString());
                        IVector dif = null;
                        Vector.Subtract(calculatedOutput, exactOutput, ref dif);
                        Console.WriteLine("Norm of dif. between sampled and calculated output: " + dif.Norm);
                    }
                }
            } // lock
            return neural;
        }



        ///// <summary>Similar to ExampleQuadratic(), 
        ///// but designed for quic kuns for other tests. 
        ///// This method will be REMOVED when not needed any more.</summary>
        //public static INeuralApproximator ExampleQuadraticQuick(int inputLength, int outputLength, int printLevel)
        //{
        //    // Specify reasonable number of samples and verification points:
        //    int numTrainingElements = 2 * inputLength * inputLength;
        //    int numVerificationPoints = (int)Math.Round((double)numTrainingElements * 0.5);
        //    numTrainingElements += numVerificationPoints;
        //    // Create training data by randomly sampling a specific quadratic response:
        //    NeuralTrainingSet samples = NeuralTrainingSet.CreateExampleQuadratic(inputLength, outputLength, numTrainingElements);
        //    // Speciy which samples will be used for verification of approximation:
        //    IndexList verificationIndices = IndexList.CreateRandom(numVerificationPoints, 0 /* lowerbound */, numTrainingElements - 1);
        //    // Create neural network and set basic parameters:
        //    INeuralApproximator neural = new NeuralApproximatorAforge();
        //    neural.OutputLevel = 0;
        //    neural.InputLength = inputLength;
        //    neural.OutputLength = outputLength;
        //    // Set prepared data:
        //    neural.TrainingData = samples;
        //    neural.VerificationIndices = verificationIndices;
        //    // Set network layout:
        //    neural.MultipleNetworks = true;
        //    neural.SetHiddenLayers(30, 20);
        //    // Set training parameters:
        //    neural.MaxEpochs = 500;
        //    neural.EpochsInBundle = 100;
        //    neural.ToleranceRMS = new Vector(outputLength, 0.001);

        //    // Specify learning parameters:
        //    neural.LearningRate = 0.1;
        //    neural.SigmoidAlphaValue = 2.0;
        //    neural.Momentum = 0.00;

        //    // Specify parameters defining the bounds for data applied to input and output neurons:
        //    neural.InputBoundsSafetyFactor = 1.5;
        //    neural.OutputBoundsSafetyFactor = 1.5;

        //    // Change the targeted range of input and output neurons: 
        //    neural.InputNeuronsRange.Reset();
        //    neural.InputNeuronsRange.UpdateAll(-2.0, 2.0);
        //    neural.OutputNeuronsRange.Reset();
        //    neural.OutputNeuronsRange.UpdateAll(-0.9, 0.9);


        //    // Test output (for debugging purposes):
        //    Console.WriteLine();
        //    Console.WriteLine("Neural network data: ");
        //    Console.WriteLine(neural.ToString());
        //    Console.WriteLine("Insert <Enter> in order to continue: ");
        //    // Console.ReadLine();

        //    // Perform training: 
        //    neural.TrainNetwork();

        //    // Calculate outputs and exact values for first 5 points in the training set:
        //    Console.WriteLine("A couple of calculated outputs from the training set (including verification points):");
        //    for (int i = 0; i < 5; ++i)
        //    {
        //        IVector input = samples.GetInputParameters(i);
        //        IVector exactOutput = samples.GetOutputValues(i);
        //        IVector calculatedOutput = new Vector(outputLength);
        //        neural.CalculateOutput(input, ref calculatedOutput);
        //        Console.WriteLine();
        //        Console.WriteLine("Point No. " + i + " of the training set, is verification point: " + verificationIndices.Contains(i));
        //        Console.WriteLine("  Input parameters No. " + i + ":");
        //        Console.WriteLine("  " + input.ToString());
        //        Console.WriteLine("  Exact output: ");
        //        Console.WriteLine("  " + exactOutput.ToString());
        //        Console.WriteLine("  Approximated output: ");
        //        Console.WriteLine("  " + calculatedOutput.ToString());
        //        IVector dif = null;
        //        Vector.Subtract(calculatedOutput, exactOutput, ref dif);
        //        Console.WriteLine("Norm of the difference between sampled and calculated output: " + dif.Norm);
        //    }
        //    return neural;
        //}



        #endregion Examples


    }  // abstract class NeuralApproximatorBase



}
