
// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/

// NEURAL NETWORK APPROXIMATORS

using System.IO;
using System.IO.Compression;
using System.Xml;
using System.Text;
using System;
using System.Collections.Generic;

using IG.Lib;

namespace IG.Num
{


    /// <summary>A data transfer object (DTO) for neural approximation classes that implement
    /// the INeuralApproximator interface.
    /// $A Igor Mar11;</summary>
    public abstract class NeuralApproximatorDtoBase<Type>: SerializationDtoBase<Type, INeuralApproximator>
        where Type: class, INeuralApproximator
    {

        public NeuralApproximatorDtoBase()
        { }


        #region Data

        // Network related parametrs:

        public int InputLength;

        public int OutputLength;

        public int NumHiddenLayers;

        public int[] NumHiddenNeurons;

        // Saving/restoring network state:

        public string NeuralApproximatorType;

        public string NetworkStateFilePath;

        public string NetworkStateRelativePath;

        // Training related parameters:

        public double LearningRate;

        public double SigmoidAlphaValue;

        public double Momentum;

        // Training data: 

        public SampledDataSetDto TrainingData;

        public IndexListDto VerificationIndices;

        // Stopping Criteria for training:

        public int MaxEpochs;

        public int EpochsInBundle;

        
        /// <summary>Variable with old name, which is kept here for compatibility of files
        /// that were created by serialization in previous versions of code.</summary>
        [Obsolete("Kept here for compatibility reasons.")]
        public VectorDtoBase ToleranceRMS
        {
            set
            {
                if (ToleranceRms == null && value != null)
                {
                    // Set the replacing variable, but only if it hasn't been set yet.
                    ToleranceRms = value;
                }
            }
            get { return ToleranceRms; }
        }

        /// <summary>Tolerance over RMS error of output over training points.
        /// Training will continue until error becomes below tolerance or until maximal number of epochs is reached.
        /// If less or equal than 0 then this tolerance is not taken into account.</summary>
        public VectorDtoBase ToleranceRms;


        /// <summary>Relative tolerances on RMS errors of outputs over training points, relative to the 
        /// correspoinding ranges of output data.</summary>
        /// <seealso cref="NeuralApproximatorBase.ToleranceRmsRelativeToRange"/>
        public VectorDtoBase ToleranceRmsRelativeToRange;

        /// <summary>Scalar through which all components of the Relative tolerances on RMS errors of outputs
        /// can be set to the same value.</summary>
        /// <seealso cref="NeuralApproximatorBase.ToleranceRmsRelativeToRangeScalar"/>
        public double ToleranceRmsRelativeToRangeScalar;


        /// <summary>Tolerance on maximal error of output over training points.
        /// Training will continue until error becomes below tolerance or until maximal number of epochs is reached.
        /// If less or equal than 0 then this tolerance is not taken into account.</summary>
        public VectorDtoBase ToleranceMax;

        /// <summary>Relative tolerances on max. abs. errors of outputs over training points, relative to the 
        /// correspoinding ranges of output data.</summary>
        /// <seealso cref="NeuralApproximatorBase.ToleranceMaxRelativeToRange"/>
        public VectorDtoBase ToleranceMaxRelativeToRange;

        /// <summary>Scalar through which all components of the Relative tolerances on max. abs. errors of outputs
        /// can be set to the same value.</summary>
        /// <seealso cref="NeuralApproximatorBase.ToleranceMaxRelativeToRangeScalar"/>
        public double ToleranceMaxRelativeToRangeScalar;


        // Mapping from actual domain ot input/output neurons (range matching):

        public double InputBoundsSafetyFactor;

        public double OutputBoundsSafetyFactor;

        public BoundingBoxDtoBase InputDataBounds;

        public BoundingBoxDtoBase OutputDataBounds;

        public BoundingBoxDtoBase InputNeuronsRange;

        public BoundingBoxDtoBase OutputNeuronsRange;

        // Results:

        ///// <summary>Variable with old name, which is kept here for compatibility of files
        ///// that were created by serialization in previous versions of code.</summary>
        //[Obsolete("Kept here for compatibility reasons.")]
        //public bool SaveConvergenceRMS
        //{
        //    set
        //    {
        //        if (SaveConvergenceRms == default(bool) && value != default(bool))
        //        {
        //            // Set the replacing variable, but only if it hasn't been set yet.
        //            SaveConvergenceRms = value;
        //        }
        //    }
        //    get { return SaveConvergenceRms; }
        //}


        public bool SaveConvergenceRms;


        ///// <summary>Variable with old name, which is kept here for compatibility of files
        ///// that were created by serialization in previous versions of code.</summary>
        //[Obsolete("Kept here for compatibility reasons.")]
        //public VectorDtoBase[] ConvergenceListRMS
        //{
        //    set
        //    {
        //        if (ConvergenceListRms == null && value != null)
        //        {
        //            // Set the replacing variable, but only if it hasn't been set yet.
        //            ConvergenceListRms = value;
        //        }
        //    }
        //    get { return ConvergenceListRms; }
        //}

        public VectorDtoBase[] ConvergenceListRms;
        public int[] EpochNumbers;
        public VectorDtoBase[] ConvergenceErrorsTrainingRmsTable;
        public VectorDtoBase[] ConvergenceErrorsTrainingMaxTable;
        public VectorDtoBase[] ConvergenceErrorsVerificationRmsTable;
        public VectorDtoBase[] ConvergenceErrorsVerificationMaxTable;

        // Miscellaneous data:


        private int _outputLevel = Util.OutputLevel;

        /// <summary>Level of information that is output to the console by some methods.</summary>
        int OutputLevel
        {
            get { return _outputLevel; }
            set { _outputLevel = value; }
        }

        #endregion Data


        #region Operation 

        // public override Type CreateObject();

        /// <summary>Returns true if the specified neural network approximator object is of a correct type, 
        /// according to type indicated by the NeuralApproximatorType property, or false otherwise.</summary>
        /// <param name="obj">Object whose type is checked.</param>
        /// <returns></returns>
        protected bool IsAppropriateType(INeuralApproximator obj)
        {
            if (obj == null)
                return true;
            else if (string.IsNullOrEmpty(this.NeuralApproximatorType))
                return true; // type not specified oncurrent DTO, every object is of appropriate type
            else
            {
                if (this.NeuralApproximatorType == obj.NeuralApproximatorType)
                    return true;
                else
                    return false;
            }
        }

        bool _restoreInternalState = true;

        /// <summary>Sets the flag indicating whether internal state of the (trained) neural network should be restored, 
        /// if possible, when the contents of the current DTO (data transfer object) is copied to a neural approximator
        /// object. Default value of the flag is true.</summary>
        /// <param name="doRestore">Value of the flag to be set. Default is true.</param>
        public void SetRestoringInternalState(bool doRestore)
        {
            _restoreInternalState = doRestore;
        }

        /// <summary>Returns a flag indicating whether internal state of the (trained) neural network should be restored, 
        /// if possible, when the contents of the current DTO (data transfer object) is copied to a neural approximator
        /// object. Default value of the flag is true and can be changed by the SetRestoringInternalState() method.</summary>
        public bool GetRestoringInternalState()
        {
            return _restoreInternalState;
        }

        /// <summary>Copies data from the specified neural approximator object to the current DTO (data transfer object).</summary>
        /// <param name="obj">Object from which data is copied.</param>
        /// <remarks>Actual type of the object is also stored. In this way, neural network approximator of the correct
        /// type can be created when restoring data from a stored location.</remarks>
        protected override void CopyFromPlain(INeuralApproximator obj)
        {
            if (obj == null)
                this.SetNull(true);
            else
            {
                this.SetNull(false);
                lock (obj.Lock)
                {
                    // Network related parameters:
                    this.InputLength = obj.InputLength;
                    this.OutputLength = obj.OutputLength;
                    this.NumHiddenLayers = obj.NumHiddenLayers;
                    this.NumHiddenNeurons = new int[NumHiddenLayers];
                    for (int i = 0; i < NumHiddenLayers; ++i)
                        this.NumHiddenNeurons[i] = obj.GetNumNeuronsInHiddenLayer(i);
                    // Training related parameters:
                    this.LearningRate = obj.LearningRate;
                    this.SigmoidAlphaValue = obj.SigmoidAlphaValue;
                    this.Momentum = obj.Momentum;
                    // Training data: 
                    SerializationDto.CopyFromObject<SampledDataSetDto, SampledDataSet>
                        (obj.TrainingData, ref this.TrainingData);
                    SerializationDto.CopyFromObject<IndexListDto, IndexList>
                        (obj.VerificationIndices, ref this.VerificationIndices);
                    // Stopping criteria for training:
                    this.MaxEpochs = obj.MaxEpochs;
                    this.EpochsInBundle = obj.EpochsInBundle;
                    // this.ToleranceRMS = null; // newly saved data will not use this field
                    SerializationDto.CopyFromObject(obj.ToleranceRms, ref this.ToleranceRms);
                    SerializationDto.CopyFromObject(obj.ToleranceRmsRelativeToRange, ref this.ToleranceRmsRelativeToRange);
                    this.ToleranceRmsRelativeToRangeScalar = obj.ToleranceRmsRelativeToRangeScalar;
                    SerializationDto.CopyFromObject<VectorDtoBase, IVector>(obj.ToleranceMax, ref this.ToleranceMax);
                    SerializationDto.CopyFromObject(obj.ToleranceMaxRelativeToRange, ref this.ToleranceMaxRelativeToRange);
                    this.ToleranceMaxRelativeToRangeScalar = obj.ToleranceMaxRelativeToRangeScalar;
                    // Mapping from actual domain ot input/output neurons (range matching):
                    this.InputBoundsSafetyFactor = obj.InputBoundsSafetyFactor;
                    this.OutputBoundsSafetyFactor = obj.OutputBoundsSafetyFactor;
                    SerializationDto.CopyFromObject<BoundingBoxDtoBase, IBoundingBox>
                        (obj.InputDataBounds, ref this.InputDataBounds);
                    SerializationDto.CopyFromObject<BoundingBoxDtoBase, IBoundingBox>
                        (obj.OutputDataBounds, ref this.OutputDataBounds);
                    SerializationDto.CopyFromObject<BoundingBoxDtoBase, IBoundingBox>
                        (obj.InputNeuronsRange, ref this.InputNeuronsRange);
                    SerializationDto.CopyFromObject<BoundingBoxDtoBase, IBoundingBox>
                        (obj.OutputNeuronsRange, ref this.OutputNeuronsRange);
                    //TO-DO
                    //this.SaveConvergenceRMS = obj.SaveConvergenceRms;
                    this.SaveConvergenceRms = obj.SaveConvergenceRms;
                    if (!obj.SaveConvergenceRms)
                        this.EpochNumbers = null;
                    else
                    {
                        this.EpochNumbers = new int[obj.EpochNumbers.Count];
                        for (int i = 0; i < obj.EpochNumbers.Count; i++)
                        {
                            EpochNumbers[i] = obj.EpochNumbers[i];
                        }
                    }
                    if (!obj.SaveConvergenceRms)
                        this.ConvergenceErrorsTrainingRmsTable = null;
                    else
                    {
                        this.ConvergenceErrorsTrainingRmsTable = new VectorDtoBase[obj.ConvergenceErrorsTrainingRmsList.Count];
                        for (int i = 0; i < obj.ConvergenceErrorsTrainingRmsList.Count; i++)
                        {
                            ConvergenceErrorsTrainingRmsTable[i] = new VectorDtoBase();
                            ConvergenceErrorsTrainingRmsTable[i].CopyFrom(obj.ConvergenceErrorsTrainingRmsList[i]);
                        }
                    }
                    if (!obj.SaveConvergenceRms)
                        this.ConvergenceErrorsTrainingMaxTable = null;
                    else
                    {
                        this.ConvergenceErrorsTrainingMaxTable = new VectorDtoBase[obj.ConvergenceErrorsTrainingMaxList.Count];
                        for (int i = 0; i < obj.ConvergenceErrorsTrainingMaxList.Count; i++)
                        {
                            ConvergenceErrorsTrainingMaxTable[i] = new VectorDtoBase();
                            ConvergenceErrorsTrainingMaxTable[i].CopyFrom(obj.ConvergenceErrorsTrainingMaxList[i]);
                        }
                    }
                    if (!obj.SaveConvergenceRms)
                        this.ConvergenceErrorsVerificationRmsTable = null;
                    else
                    {
                        this.ConvergenceErrorsVerificationRmsTable = new VectorDtoBase[obj.ConvergenceErrorsVerificationRmsList.Count];
                        for (int i = 0; i < obj.ConvergenceErrorsVerificationRmsList.Count; i++)
                        {
                            ConvergenceErrorsVerificationRmsTable[i] = new VectorDtoBase();
                            ConvergenceErrorsVerificationRmsTable[i].CopyFrom(obj.ConvergenceErrorsVerificationRmsList[i]);
                        }
                    }
                    if (!obj.SaveConvergenceRms)
                        this.ConvergenceErrorsVerificationMaxTable = null;
                    else
                    {
                        this.ConvergenceErrorsVerificationMaxTable = new VectorDtoBase[obj.ConvergenceErrorsVerificationMaxList.Count];
                        for (int i = 0; i < obj.ConvergenceErrorsVerificationMaxList.Count; i++)
                        {
                            ConvergenceErrorsVerificationMaxTable[i] = new VectorDtoBase();
                            ConvergenceErrorsVerificationMaxTable[i].CopyFrom(obj.ConvergenceErrorsVerificationMaxList[i]);
                        }
                    }
                    // Miscellaneous data:
                    this.OutputLevel = obj.OutputLevel;
                    // Saving/restoring network state:
                    this.NetworkStateFilePath = obj.NetworkStateFilePath;
                    this.NetworkStateRelativePath = obj.NetworkStateRelativePath;
                    this.NeuralApproximatorType = obj.NeuralApproximatorType;
                } // lock
            }
        }

        /// <summary>Copies contents of the current DTO (data transfer object) to the specified neural approximator object.
        /// If it is indicated on that object that the internal neural network state has been stored to a file then 
        /// this state is restored from that file, too. This enables saving of trained networks for future use.</summary>
        /// <param name="obj">Neural network approximator object where data is stored.</param>
        protected override void CopyToPlain(ref INeuralApproximator obj)
        {
            if (this.GetNull())
                obj = null;
            else
            {
                if (obj == null)
                    obj = CreateObject();
                if (!IsAppropriateType(obj))
                {
                    obj = CreateObject();
                    if (!IsAppropriateType(obj) /* && false */)  // $$$$$$$$$$$$$$$$$$$$$$$$ : remove this semicolon because the test shoulld remain as it is!
                    {

                        Console.WriteLine(Environment.NewLine + Environment.NewLine 
                            + "ERROR: Incorrect neural approximator when copying from DTO to approximator: " + Environment.NewLine
                            + "  DTO type:           " + this.NeuralApproximatorType + Environment.NewLine
                            + "  Target object type: " + obj.NeuralApproximatorType + Environment.NewLine + Environment.NewLine);

                        throw new InvalidOperationException("Can not create neural approximator of the appropriate type. Current type: \""
                            + this.NeuralApproximatorType + "\".");

                    }
                }
                lock (obj.Lock)
                {
                    // Network related parameters:
                    obj.InputLength = this.InputLength;
                    obj.OutputLength = this.OutputLength;
                    obj.NumHiddenLayers = this.NumHiddenLayers;
                    obj.NumHiddenNeurons = this.NumHiddenNeurons;
                    // Training related parameters:
                    obj.LearningRate = this.LearningRate;
                    obj.SigmoidAlphaValue = this.SigmoidAlphaValue;
                    obj.Momentum = this.Momentum;
                    // Training data: 
                    obj.TrainingData = SerializationDto.CopyToObjectReturned<SampledDataSetDto, SampledDataSet>
                        (this.TrainingData, obj.TrainingData);
                    obj.VerificationIndices = SerializationDto.CopyToObjectReturned<IndexListDto, IndexList>
                        (this.VerificationIndices, obj.VerificationIndices);
                    // Stopping criteria for training:
                    obj.MaxEpochs = this.MaxEpochs;
                    obj.EpochsInBundle = this.EpochsInBundle;
                    obj.ToleranceRms = SerializationDto.CopyToObjectReturned<VectorDtoBase, IVector>
                            (this.ToleranceRms, obj.ToleranceRms);
                    //if (obj.ToleranceRms == null)  // try this for compatibility reasons, if vector was stored under the old name
                    //    obj.ToleranceRms = SerializationDto.CopyToObjectReturned<VectorDtoBase, IVector>
                    //            (this.ToleranceRMS, obj.ToleranceRms);
                     obj.ToleranceRmsRelativeToRange = SerializationDto.CopyToObjectReturned<VectorDtoBase, IVector>
                        (this.ToleranceRmsRelativeToRange, obj.ToleranceRmsRelativeToRange);
                    obj.ToleranceRmsRelativeToRangeScalar = this.ToleranceRmsRelativeToRangeScalar;
                    obj.ToleranceMax = SerializationDto.CopyToObjectReturned<VectorDtoBase, IVector>(this.ToleranceMax, obj.ToleranceMax);
                    obj.ToleranceMaxRelativeToRange = SerializationDto.CopyToObjectReturned<VectorDtoBase, IVector>
                        (this.ToleranceMaxRelativeToRange, obj.ToleranceMaxRelativeToRange);
                    obj.ToleranceMaxRelativeToRangeScalar = this.ToleranceMaxRelativeToRangeScalar;
                    // Mapping from actual domain of input/output neurons (range matching):
                    obj.InputBoundsSafetyFactor = this.InputBoundsSafetyFactor;
                    obj.OutputBoundsSafetyFactor = this.OutputBoundsSafetyFactor;
                    SerializationDto.CopyToObjectReturned<BoundingBoxDtoBase, IBoundingBox>
                        (this.InputDataBounds, obj.InputDataBounds);
                    SerializationDto.CopyToObjectReturned<BoundingBoxDtoBase, IBoundingBox>
                        (this.OutputDataBounds, obj.OutputDataBounds);
                    SerializationDto.CopyToObjectReturned<BoundingBoxDtoBase, IBoundingBox>
                        (this.InputNeuronsRange, obj.InputNeuronsRange);
                    SerializationDto.CopyToObjectReturned<BoundingBoxDtoBase, IBoundingBox>
                        (this.OutputNeuronsRange, obj.OutputNeuronsRange);
                    //TO-DO
                    obj.SaveConvergenceRms = this.SaveConvergenceRms;
                    obj.ConvergenceErrorsTrainingRmsList = null;
                    if (this.EpochNumbers == null)
                        obj.EpochNumbers = null;
                    else
                    {
                        obj.EpochNumbers = new List<int>();
                        for (int i = 0; i < this.EpochNumbers.Length; i++)
                        {
                            int tmpEpochNumber = 0;
                            int tmpEpochNumberDto = this.EpochNumbers[i];
                            tmpEpochNumber = tmpEpochNumberDto;
                            obj.EpochNumbers.Add(tmpEpochNumber);
                        }
                    }
                    if (this.ConvergenceErrorsTrainingRmsTable == null)
                        obj.ConvergenceErrorsTrainingRmsList = null;
                    else
                    {
                        obj.ConvergenceErrorsTrainingRmsList = new List<IVector>();
                        for (int i = 0; i < this.ConvergenceErrorsTrainingRmsTable.Length; i++)
                        {
                            IVector tmpErrorsRms = null;
                            VectorDtoBase tmpErrorsRmsDto = this.ConvergenceErrorsTrainingRmsTable[i];
                            tmpErrorsRmsDto.CopyTo(ref tmpErrorsRms);
                            obj.ConvergenceErrorsTrainingRmsList.Add(tmpErrorsRms);
                        }
                    }
                    if (this.ConvergenceErrorsTrainingMaxTable == null)
                        obj.ConvergenceErrorsTrainingMaxList = null;
                    else
                    {
                        obj.ConvergenceErrorsTrainingMaxList = new List<IVector>();
                        for (int i = 0; i < this.ConvergenceErrorsTrainingMaxTable.Length; i++)
                        {
                            IVector tmpErrorsRms = null;
                            VectorDtoBase tmpErrorsRmsDto = this.ConvergenceErrorsTrainingMaxTable[i];
                            tmpErrorsRmsDto.CopyTo(ref tmpErrorsRms);
                            obj.ConvergenceErrorsTrainingMaxList.Add(tmpErrorsRms);
                        }
                    }
                    if (this.ConvergenceErrorsVerificationRmsTable == null)
                        obj.ConvergenceErrorsVerificationRmsList = null;
                    else
                    {
                        obj.ConvergenceErrorsVerificationRmsList = new List<IVector>();
                        for (int i = 0; i < this.ConvergenceErrorsVerificationRmsTable.Length; i++)
                        {
                            IVector tmpErrorsRms = null;
                            VectorDtoBase tmpErrorsRmsDto = this.ConvergenceErrorsVerificationRmsTable[i];
                            tmpErrorsRmsDto.CopyTo(ref tmpErrorsRms);
                            obj.ConvergenceErrorsVerificationRmsList.Add(tmpErrorsRms);
                        }
                    }
                    if (this.ConvergenceErrorsVerificationMaxTable == null)
                        obj.ConvergenceErrorsVerificationMaxList = null;
                    else
                    {
                        obj.ConvergenceErrorsVerificationMaxList = new List<IVector>();
                        for (int i = 0; i < this.ConvergenceErrorsVerificationMaxTable.Length; i++)
                        {
                            IVector tmpErrorsRms = null;
                            VectorDtoBase tmpErrorsRmsDto = this.ConvergenceErrorsVerificationMaxTable[i];
                            tmpErrorsRmsDto.CopyTo(ref tmpErrorsRms);
                            obj.ConvergenceErrorsVerificationMaxList.Add(tmpErrorsRms);
                        }
                    }
                    // Miscellaneous data:
                    obj.OutputLevel = this.OutputLevel;
                    // Saving/restoring network state:
                    obj.NetworkStateRelativePath = this.NetworkStateRelativePath;
                    if (GetRestoringInternalState())
                    {
                        // Load internal network(s), try with relative path first:
                        // REMARK:
                        // At this point, we can not restore network state using relative path
                        // because we don't know the path of the

                        if (!string.IsNullOrEmpty(this.NetworkStateFilePath))
                        {
                            try
                            {
                                obj.LoadNetwork(this.NetworkStateFilePath);
                            }
                            catch { }
                        }

                    }
                }  // lock
            }
        }

        #endregion Operation 

    } // class NeuralApproximatorBaseDto<Type>



    /// <summary>A data transfer object (DTO) for neural approximation classes that implement
    /// the INeuralApproximator interface.
    /// $A Igor Mar11;</summary>
    public class NeuralApproximatorDtoBase: NeuralApproximatorDtoBase<INeuralApproximator>
    {

        public NeuralApproximatorDtoBase()
            : base()
        { }


        /// <summary>Creates and returns a new bounding box cast to the interface type IBoundingBox.</summary>
        public override INeuralApproximator CreateObject()
        {
            return NeuralApproximatorBase.CreateApproximator(NeuralApproximatorType);

            //if (string.IsNullOrEmpty(NeuralApproximatorType))
            //{
            //    throw new InvalidOperationException($"Neural approximator type is not specified on the {GetType().Name} object.");
            //    //return new NeuralApproximatorAforge();
            //}
            //else
            //{
            //    INeuralApproximator ret = null;
            //    //try
            //    //{
            //        Type approximatorType = Type.GetType(NeuralApproximatorType);
            //    if (approximatorType == null)
            //    {
            //        throw new InvalidOperationException($"Could not resolve the neural approximator ({typeof(INeuralApproximator).Name}) type from name {NeuralApproximatorType}."
            //            + Environment.NewLine + "  Maybe you are stating name that is not assembly-qualified in a different assembly than the one containing type definition.");
            //    }
            //        ret = (INeuralApproximator)Activator.CreateInstance(approximatorType);
            //    //}
            //    //catch { }
            //    //finally
            //    //{
            //    //    if (ret == null)
            //    //        ret = new NeuralApproximatorAforgeFake();
            //    //}

            //    return ret;
            //}
        }

    } // class NeuralApproximatorDtoBase




    /// <summary>A data transfer object (DTO) for the NeuralApproximatorAforge class.
    /// $A Igor Mar11;</summary>
    public class NeuralApproximatorAForgeFakeDto : NeuralApproximatorDtoBase<NeuralApproximatorAforgeFake>
    {

        public NeuralApproximatorAForgeFakeDto()
            : base()
        { }



        #region Operation 

        public override NeuralApproximatorAforgeFake CreateObject()
        {
            return new NeuralApproximatorAforgeFake();
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
