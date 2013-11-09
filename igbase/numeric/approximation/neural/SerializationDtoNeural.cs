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


    /// <summary>Transfer Object (DTO) for neural network training set.</summary>
    /// $A Igor Jul10; Tako78 Jun12;
    public class NeuralTrainingParametersDto : SerializationDto<NeuralTrainingParameters>
    {

        #region Data

        #region TrainingParameters

        /// <summary>Learning rate.</summary>
        public double LearningRate;

        /// <summary>Momentum. Specifies how much changes of weight in the previous iterations affect 
        /// changes in the current iterations.</summary>
        public double Momentum;

        /// <summary>Sigmoid alpha value (used in networks with sigmoid activation functions).</summary>
        public double SigmoidAlphaValue;

        /// <summary>Gets or sets the number of input neurons.</summary>
        public int InputLength;

        /// <summary>Gets or sets the number of output neurons.</summary>
        public int OutputLength;


        public double InputBoundSafetyFactor;

        public double OutputBoundSafetyFactor;

        /// <summary> Maximal number of epochs performed in the training procedure. </summary>
        /// $A Tako78 Jul12;
        public int MaxEpochs;

        /// <summary>Number of epochs in bundle (i.e. number of epochs performed at once, without any
        /// checks or output operations between).
        /// <para>This parameter does not affect the training procedure in terms of results.</para></summary>
        /// $A Tako78 Jul12;
        public int EpochsInBundle;

        ///// <summary>Variable with old name, which is kept here for compatibility of files
        ///// that were created by serialization in previous versions of code.</summary>
        //[Obsolete("Kept here for compatibility reasons.")]
        //public VectorDtoBase ToleranceRMS
        //{
        //    set {
        //        if (ToleranceRms == null && value != null)
        //        {
        //            // Set the replacing variable, but only if it hasn't been set yet.
        //            ToleranceRms = value;
        //        }
        //    }
        //    get { return ToleranceRms; }
        //}

        /// <summary> Range from actual inputs. </summary>
        /// $A Tako78 Octl12;
        public VectorDtoBase InputRange;

        /// <summary> Range from actual outputs. </summary>
        /// $A Tako78 Octl12;
        public VectorDtoBase OutputRange;


        /// <summary>Tolerance over RMS error of outputs over training points.
        /// Training will continue until error becomes below tolerance or until maximal number of epochs is reached.
        /// If less or equal than 0 then this tolerance is not taken into account.</summary>
        /// $A Tako78 Jul12; Igor Jul12;
        public VectorDtoBase ToleranceRms;

        /// <summary>Relative tolerances on RMS errors of outputs over training points, relative to the 
        /// correspoinding ranges of output data.</summary>
        /// <seealso cref="NeuralApproximatorBase.ToleranceRmsRelativeToRange"/>
        public VectorDtoBase ToleranceRmsRelativeToRange;

        /// <summary>Scalar through which all components of the Relative tolerances on RMS errors of outputs
        /// can be set to the same value.</summary>
        /// <seealso cref="NeuralApproximatorBase.ToleranceRmsRelativeToRangeScalar"/>
        public double ToleranceRmsRelativeToRangeScalar = NeuralTrainingParameters.DefaultToleranceRmsRelativeToRangeScalar;

        /// <summary>Tolerance on maximal error of outputs over training points.
        /// Training will continue until error becomes below tolerance or until maximal number of epochs is reached.
        /// If less or equal than 0 then this tolerance is not taken into account.</summary>
        /// $A Tako78 Jul12;
        public VectorDtoBase ToleranceMax;

        /// <summary>Relative tolerances on max. abs. errors of outputs over training points, relative to the 
        /// correspoinding ranges of output data.</summary>
        /// <seealso cref="NeuralApproximatorBase.ToleranceMaxRelativeToRange"/>
        public VectorDtoBase ToleranceMaxRelativeToRange;

        /// <summary>Scalar through which all components of the Relative tolerances on max. abs. errors of outputs
        /// can be set to the same value.</summary>
        /// <seealso cref="NeuralApproximatorBase.ToleranceMaxRelativeToRangeScalar"/>
        public double ToleranceMaxRelativeToRangeScalar = NeuralTrainingParameters.DefaultToleranceMaxRelativeToRangeScalar;


        #endregion TrainingParameters


        #region NetworkArchitecture

        /// <summary> Number of Hideden layers. </summary>
        /// $A Tako78 Jul12;
        public int NumHiddenLayers;

        /// <summary> Numbers of neurons in each hidden layer. </summary>
        /// $A Tako78 Jul12;
        public int[] NumHidenNeurons;

        #endregion NetworkArchitecture


        #region TrainingResults

        /// <summary> Whether the network is trained (and results exist). </summary>
        /// $A Tako78 Jul12;
        public bool IsNetworkTrained;

        
        ///// <summary>Variable with old name, which is kept here for compatibility of files
        ///// that were created by serialization in previous versions of code.</summary>
        //[Obsolete("Kept here for compatibility reasons.")]
        //public VectorDtoBase ErrorsTrainingRMS
        //{
        //    set
        //    {
        //        if (ErrorsTrainingRms == null && value != null)
        //        {
        //            // Set the replacing variable, but only if it hasn't been set yet.
        //            ErrorsTrainingRms = value;
        //        }
        //    }
        //    get { return ErrorsTrainingRms; }
        //}

        /// <summary> RMS errors calculated on training data. </summary>
        /// $A Tako78 Jul12;
        public VectorDtoBase ErrorsTrainingRms;


        ///// <summary>Variable with old name, which is kept here for compatibility of files
        ///// that were created by serialization in previous versions of code.</summary>
        //[Obsolete("Kept here for compatibility reasons.")]
        //public VectorDtoBase[] ErrorsTrainingRMS
        //{
        //    set
        //    {
        //        if (ErrorsTrainingRmsTable == null && value != null)
        //        {
        //            // Set the replacing variable, but only if it hasn't been set yet.
        //            ErrorsTrainingRmsTable = value;
        //        }
        //    }
        //    get { return ErrorsTrainingRmsTable; }
        //}

        /// <summary> Convergence Table of RMS errors calculated on training data. </summary>
        /// $A Tako78 Aug12;
        public VectorDtoBase[] ErrorsTrainingRmsTable;

        /// <summary> Maximal errors calculated on training data. </summary>
        /// $A Tako78 Jul12;
        public VectorDtoBase ErrorsTrainingMax;

        /// <summary> Convergence Table of Maximal errors calculated on training data. </summary>
        /// $A Tako78 Aug12;
        public VectorDtoBase[] ErrorsTrainingMaxTable;

        /// <summary> Mean absolute errors calculated on training data. </summary>
        /// $A Tako78 Jul12;
        public VectorDtoBase ErrorsTrainingMeanAbs;

        ///// <summary>Variable with old name, which is kept here for compatibility of files
        ///// that were created by serialization in previous versions of code.</summary>
        //[Obsolete("Kept here for compatibility reasons.")]
        //public VectorDtoBase ErrorsVerificationRMS
        //{
        //    set
        //    {
        //        if (ErrorsVerificationRms == null && value != null)
        //        {
        //            // Set the replacing variable, but only if it hasn't been set yet.
        //            ErrorsVerificationRms = value;
        //        }
        //    }
        //    get { return ErrorsVerificationRms; }
        //}

        /// <summary> RMS errors calculated on verification data. </summary>
        /// $A Tako78 Jul12;
        public VectorDtoBase ErrorsVerificationRms;


        ///// <summary>Variable with old name, which is kept here for compatibility of files
        ///// that were created by serialization in previous versions of code.</summary>
        //[Obsolete("Kept here for compatibility reasons.")]
        //public VectorDtoBase[] ErrorsVerificationRMSTable
        //{
        //    set
        //    {
        //        if (ErrorsVerificationRmsTable == null && value != null)
        //        {
        //            // Set the replacing variable, but only if it hasn't been set yet.
        //            ErrorsVerificationRmsTable = value;
        //        }
        //    }
        //    get { return ErrorsVerificationRmsTable; }
        //}


        /// <summary> Convergence Table of RMS errors calculated on verification data. </summary>
        /// $A Tako78 Aug12;
        public VectorDtoBase[] ErrorsVerificationRmsTable;

        /// <summary> Maximal errors calculated on verification data. </summary>
        /// $A Tako78 Jul12;
        public VectorDtoBase ErrorsVerificationMax;

        /// <summary> Convergence Table of Maximal errors calculated on verification data. </summary>
        /// $A Tako78 Aug12;
        public VectorDtoBase[] ErrorsVerificationMaxTable;

        /// <summary> Maximal errors calculated on training data. </summary>
        /// $A Tako78 Jul12;
        public VectorDtoBase ErrorsVerificationMeanAbs;


        ///// <summary>Variable with old name, which is kept here for compatibility of files
        ///// that were created by serialization in previous versions of code.</summary>
        //[Obsolete("Kept here for compatibility reasons.")]
        //public List<VectorDtoBase> ErrorsRMSList
        //{
        //    set
        //    {
        //        if (ErrorsRmsList == null && value != null)
        //        {
        //            // Set the replacing variable, but only if it hasn't been set yet.
        //            ErrorsRmsList = value;
        //        }
        //    }
        //    get { return ErrorsRmsList; }
        //}

        // TODO: Should this be a list, since this is a DTO?

        public List<VectorDtoBase> ErrorsRmsList;

        /// <summary> Number of epochs actually spent at training. </summary>
        /// $A Tako78 Jul12;
        public int NumEpochs;

        /// <summary> Time spent for training. </summary>
        /// $A Tako78 Jul12;;
        public double TrainingTime;

        /// <summary> CPU time spent for training. </summary>
        /// $A Tako78 Jul12;
        public double TrainingCpuTime;

        /// <summary>List of epoch numbers at which convergence data was sampled.</summary>
        public int[] EpochNumbers;


        ///// <summary>Variable with old name, which is kept here for compatibility of files
        ///// that were created by serialization in previous versions of code.</summary>
        //[Obsolete("Kept here for compatibility reasons.")]
        //public double[] EpochErrorsRMS
        //{
        //    set
        //    {
        //        if (EpochErrorsRms == null && value != null)
        //        {
        //            // Set the replacing variable, but only if it hasn't been set yet.
        //            EpochErrorsRms = value;
        //        }
        //    }
        //    get { return EpochErrorsRms; }
        //}

        /// <summary>List of sampled RMS errors corresponding to epoch numbers from <see cref="EpochNumbers"/>.</summary>
        public double[] EpochErrorsRms;

        /// <summary>List of sampled absolute errors corresponding to epoch numbers from <see cref="EpochNumbers"/>.</summary>
        public double[] EpochErrorsAbs;

        #endregion TrainingResults

        #endregion Data


        #region Operation

        /// <summary>Creates and returns a new object of the corresponding type.</summary>
        public override NeuralTrainingParameters CreateObject()
        {
            NeuralTrainingParameters ret = new NeuralTrainingParameters();
            return ret;
        }


        /// <summary>Copies the specified training parameters to the current DTO.</summary>
        /// <param name="trainingParameters">Object that is copied to the current DTO.</param>
        protected override void CopyFromPlain(NeuralTrainingParameters trainingParameters)
        {
            if (trainingParameters == null)
            {
                this.SetNull(true);
            } else
            {
                this.SetNull(false);
                // Learning parameters:
                this.LearningRate = trainingParameters.LearningRate;
                this.Momentum = trainingParameters.Momentum;
                this.SigmoidAlphaValue = trainingParameters.SigmoidAlphaValue;
                this.InputLength = trainingParameters.InputLength;
                this.OutputLength = trainingParameters.OutputLength;
                this.InputBoundSafetyFactor = trainingParameters.InputBoundSafetyFactor;
                this.OutputBoundSafetyFactor = trainingParameters.OutputBoundSafetyFactor;
                this.MaxEpochs = trainingParameters.MaxEpochs;
                this.EpochsInBundle = trainingParameters.EpochsInBundle;

                if (trainingParameters.InputRange == null)
                    this.InputRange = null;
                else
                {
                    this.InputRange = new VectorDtoBase();
                    this.InputRange.CopyFrom(trainingParameters.InputRange);
                }
                if (trainingParameters.OutputRange == null)
                    this.OutputRange = null;
                else
                {
                    this.OutputRange = new VectorDtoBase();
                    this.OutputRange.CopyFrom(trainingParameters.OutputRange);
                }

                // Stopping criteria:
                //this.ToleranceRMS = null;  // only write ToleranceRms, while ToleranceRMS is kept for compatibility in reading data
                if (trainingParameters.ToleranceRms == null)
                    this.ToleranceRms = null;
                else
                {
                    this.ToleranceRms = new VectorDtoBase();
                    this.ToleranceRms.CopyFrom(trainingParameters.ToleranceRms);
                }
                if (trainingParameters.ToleranceRmsRelativeToRange == null)
                    this.ToleranceRmsRelativeToRange = null;
                else
                {
                    this.ToleranceRmsRelativeToRange = new VectorDtoBase();
                    this.ToleranceRmsRelativeToRange.CopyFrom(trainingParameters.ToleranceRmsRelativeToRange);
                }
                this.ToleranceRmsRelativeToRangeScalar = trainingParameters.ToleranceRmsRelativeToRangeScalar;


                if (trainingParameters.ToleranceMax == null)
                    this.ToleranceMax = null;
                else
                {
                    this.ToleranceMax = new VectorDtoBase();
                    this.ToleranceMax.CopyFrom(trainingParameters.ToleranceMax);
                }
                if (trainingParameters.ToleranceMaxRelativeToRange == null)
                    this.ToleranceMaxRelativeToRange = null;
                else
                {
                    this.ToleranceMaxRelativeToRange = new VectorDtoBase();
                    this.ToleranceMaxRelativeToRange.CopyFrom(trainingParameters.ToleranceMaxRelativeToRange);
                }
                this.ToleranceMaxRelativeToRangeScalar = trainingParameters.ToleranceMaxRelativeToRangeScalar;

                // Network architecture:
                this.NumHiddenLayers = trainingParameters.NumHiddenLayers;

                if (trainingParameters.NumHidenNeurons == null)
                    NumHidenNeurons = null;
                else
                {
                    int num = trainingParameters.NumHidenNeurons.Length;
                    NumHidenNeurons = new int[num];
                    for (int i = 0; i < num; ++i)
                        NumHidenNeurons[i] = trainingParameters.NumHidenNeurons[i];
                }

                // Results:
                this.IsNetworkTrained = trainingParameters.IsNetworkTrained;
                this.NumEpochs = trainingParameters.NumEpochs;
                this.EpochsInBundle = trainingParameters.EpochsInBundle;
                this.TrainingTime = trainingParameters.TrainingTime;
                this.TrainingCpuTime = trainingParameters.TrainingCpuTime;

                if (trainingParameters.ErrorsTrainingRms == null)
                    this.ErrorsTrainingRms = null;
                else
                {
                    this.ErrorsTrainingRms = new VectorDtoBase();
                    this.ErrorsTrainingRms.CopyFrom(trainingParameters.ErrorsTrainingRms);
                }
                if (trainingParameters.ErrorsTrainingRmsList == null)
                    this.ErrorsTrainingRmsTable = null;
                else
                {
                    this.ErrorsTrainingRmsTable = new VectorDtoBase[trainingParameters.ErrorsTrainingRmsList.Count];
                    for (int i = 0; i < trainingParameters.ErrorsTrainingRmsList.Count; i++)
                    {
                        VectorDtoBase tmpErrorsTrainingRms = new VectorDtoBase();
                        tmpErrorsTrainingRms.CopyFrom(trainingParameters.ErrorsTrainingRmsList[i]);
                        this.ErrorsTrainingRmsTable[i] = tmpErrorsTrainingRms;
                    }
                }
                //if (trainingParameters.ErrorsTrainingRMSList == null)
                //    this.ErrorsRMSList = null;
                //else
                //{
                //    this.ErrorsRMSList = new List<VectorDtoBase>();
                //    for (int i = 0; i < trainingParameters.ConvergenceRMSList.Count; i++)
                //    {
                //        VectorDtoBase tmpErrorRMS = new VectorDtoBase();
                //        tmpErrorRMS.CopyFrom(trainingParameters.ConvergenceRMSList[i]);
                //        this.ErrorsRMSList.Add(tmpErrorRMS);
                //    }
                //}
                if (trainingParameters.ErrorsTrainingMax == null)
                    this.ErrorsTrainingMax = null;
                else
                {
                    this.ErrorsTrainingMax = new VectorDtoBase();
                    this.ErrorsTrainingMax.CopyFrom(trainingParameters.ErrorsTrainingMax);
                }
                if (trainingParameters.ErrorsTrainingMaxList == null)
                    this.ErrorsTrainingMaxTable = null;
                else
                {
                    this.ErrorsTrainingMaxTable = new VectorDtoBase[trainingParameters.ErrorsTrainingMaxList.Count];
                    for (int i = 0; i < trainingParameters.ErrorsTrainingMaxList.Count; i++)
                    {
                        VectorDtoBase tmpErrorsTrainingMax = new VectorDtoBase();
                        tmpErrorsTrainingMax.CopyFrom(trainingParameters.ErrorsTrainingMaxList[i]);
                        this.ErrorsTrainingMaxTable[i] = tmpErrorsTrainingMax;
                    }
                }
                if (trainingParameters.ErrorsTrainingMeanAbs == null)
                    this.ErrorsTrainingMeanAbs = null;
                else
                {
                    this.ErrorsTrainingMeanAbs = new VectorDtoBase();
                    this.ErrorsTrainingMeanAbs.CopyFrom(trainingParameters.ErrorsTrainingMeanAbs);
                }
                if (trainingParameters.ErrorsVerificationRms == null)
                    this.ErrorsVerificationRms = null;
                else
                {
                    this.ErrorsVerificationRms = new VectorDtoBase();
                    this.ErrorsVerificationRms.CopyFrom(trainingParameters.ErrorsVerificationRms);
                }
                if (trainingParameters.ErrorsVerificationRmsList == null)
                    this.ErrorsVerificationRmsTable = null;
                else
                {
                    this.ErrorsVerificationRmsTable = new VectorDtoBase[trainingParameters.ErrorsVerificationRmsList.Count];
                    for (int i = 0; i < trainingParameters.ErrorsVerificationRmsList.Count; i++)
                    {
                        VectorDtoBase tmpErrorsVerificationRms = new VectorDtoBase();
                        tmpErrorsVerificationRms.CopyFrom(trainingParameters.ErrorsVerificationRmsList[i]);
                        this.ErrorsVerificationRmsTable[i] = tmpErrorsVerificationRms;
                    }
                }
                if (trainingParameters.ErrorsVerificationMax == null)
                    this.ErrorsVerificationMax = null;
                else
                {
                    this.ErrorsVerificationMax = new VectorDtoBase();
                    this.ErrorsVerificationMax.CopyFrom(trainingParameters.ErrorsVerificationMax);
                }
                if (trainingParameters.ErrorsVerificationMaxList == null)
                    this.ErrorsVerificationMaxTable = null;
                else
                {
                    this.ErrorsVerificationMaxTable = new VectorDtoBase[trainingParameters.ErrorsVerificationMaxList.Count];
                    for (int i = 0; i < trainingParameters.ErrorsVerificationMaxList.Count; i++)
                    {
                        VectorDtoBase tmpErrorsVerificationMax = new VectorDtoBase();
                        tmpErrorsVerificationMax.CopyFrom(trainingParameters.ErrorsVerificationMaxList[i]);
                        this.ErrorsVerificationMaxTable[i] = tmpErrorsVerificationMax;
                    }
                }
                if (trainingParameters.ErrorsVerificationMeanAbs == null)
                    this.ErrorsVerificationMeanAbs = null;
                else
                {
                    this.ErrorsVerificationMeanAbs = new VectorDtoBase();
                    this.ErrorsVerificationMeanAbs.CopyFrom(trainingParameters.ErrorsVerificationMeanAbs);
                }
                //if (trainingParameters.ConvergenceListRms == null)
                //    this.ErrorsRmsList = null;
                //else
                //{
                //    this.ErrorsRmsList = new List<VectorDtoBase>();
                //    for (int i = 0; i < trainingParameters.ConvergenceListRms.Count; i++)
                //    {
                //        VectorDtoBase tmpErrorRms = new VectorDtoBase();
                //        tmpErrorRms.CopyFrom(trainingParameters.ConvergenceListRms[i]);
                //        this.ErrorsRmsList.Add(tmpErrorRms);
                //    }
                //}

                if (trainingParameters.EpochNumbers == null)
                    EpochNumbers = null;
                else
                {
                    int num = trainingParameters.EpochNumbers.Count;
                    EpochNumbers = new int[num];
                    for (int i = 0; i < num; ++i)
                        EpochNumbers[i] = trainingParameters.EpochNumbers[i];
                }
                if (trainingParameters.EpochErrorsRms == null)
                    EpochErrorsRms = null;
                else
                {
                    int num = trainingParameters.EpochErrorsRms.Count;
                    EpochErrorsRms = new double[num];
                    for (int i = 0; i < num; ++i)
                        EpochErrorsRms[i] = trainingParameters.EpochErrorsRms[i];
                }
                if (trainingParameters.EpochErrorsAbs == null)
                    EpochErrorsAbs = null;
                else
                {
                    int num = trainingParameters.EpochErrorsAbs.Count;
                    EpochErrorsAbs = new double[num];
                    for (int i = 0; i < num; ++i)
                        EpochErrorsAbs[i] = trainingParameters.EpochErrorsAbs[i];
                }
            }
        }


        /// <summary>Copies contents of the current DTO to the specified training parameters object.</summary>
        /// <param name="trainingParameters">Object that the current DTO content is copied to.</param>
        protected override void CopyToPlain(ref NeuralTrainingParameters trainingParameters)
        {
            if (this.GetNull())
                trainingParameters = null;
            else
            {
                // Learning parameters:
                trainingParameters = this.CreateObject();
                trainingParameters.LearningRate = this.LearningRate;
                trainingParameters.Momentum = this.Momentum;
                trainingParameters.SigmoidAlphaValue = this.SigmoidAlphaValue;
                trainingParameters.InputLength = this.InputLength;
                trainingParameters.OutputLength = this.OutputLength;
                trainingParameters.InputBoundSafetyFactor = this.InputBoundSafetyFactor;
                trainingParameters.OutputBoundSafetyFactor = this.OutputBoundSafetyFactor;
                trainingParameters.MaxEpochs = this.MaxEpochs;
                trainingParameters.EpochsInBundle = this.EpochsInBundle;

                IVector InputRange = null;
                if (this.InputRange != null)
                    this.InputRange.CopyTo(ref InputRange);
                trainingParameters.InputRange = InputRange;

                IVector OutputRange = null;
                if (this.OutputRange != null)
                    this.OutputRange.CopyTo(ref OutputRange);
                trainingParameters.OutputRange = OutputRange;

                // Stopping criteria:
                IVector tolRms = null;
                if (this.ToleranceRms != null)
                    this.ToleranceRms.CopyTo(ref tolRms);
                //if (tolRms == null)
                //{
                //    // try this for compatibility reasons, if the vector is stored under old name:  
                //    if (this.ToleranceRMS != null)
                //            this.ToleranceRMS.CopyTo(ref tolRms);
                //}
                trainingParameters.ToleranceRms = tolRms;
                IVector tolRmsRel = null;
                if (this.ToleranceRmsRelativeToRange != null)
                    this.ToleranceRmsRelativeToRange.CopyTo(ref tolRmsRel);
                trainingParameters.ToleranceRmsRelativeToRange = tolRmsRel;
                trainingParameters.ToleranceRmsRelativeToRangeScalar = this.ToleranceRmsRelativeToRangeScalar;

                IVector tolMax = null;
                if (this.ToleranceMax != null)
                    this.ToleranceMax.CopyTo(ref tolMax);
                trainingParameters.ToleranceMax = tolMax;
                IVector tolMaxRel = null;
                if (this.ToleranceMaxRelativeToRange != null)
                    this.ToleranceMaxRelativeToRange.CopyTo(ref tolMaxRel);
                trainingParameters.ToleranceMaxRelativeToRange = tolMaxRel;
                trainingParameters.ToleranceMaxRelativeToRangeScalar = this.ToleranceMaxRelativeToRangeScalar;

                // Network architecture:
                trainingParameters.NumHiddenLayers = this.NumHiddenLayers;
                trainingParameters.NumHidenNeurons = this.NumHidenNeurons;

                // Results:
                trainingParameters.IsNetworkTrained = this.IsNetworkTrained;
                trainingParameters.NumEpochs = this.NumEpochs;
                trainingParameters.TrainingTime = this.TrainingTime;
                trainingParameters.TrainingCpuTime = this.TrainingCpuTime;

                IVector errorTrRms = null;
                if (this.ErrorsTrainingRms != null)
                    this.ErrorsTrainingRms.CopyTo(ref errorTrRms);
                trainingParameters.ErrorsTrainingRms = errorTrRms;
                List<IVector> errorTrainingRmsList = null;
                if (this.ErrorsTrainingRmsTable != null)
                {
                    errorTrainingRmsList = new List<IVector>();
                    for (int i = 0; i < this.ErrorsTrainingRmsTable.Length; i++)
                    {
                        IVector tmpErrorsTrainingRms = null;
                        this.ErrorsTrainingRmsTable[i].CopyTo(ref tmpErrorsTrainingRms);
                        errorTrainingRmsList.Add(tmpErrorsTrainingRms);
                    }
                }
                trainingParameters.ErrorsTrainingRmsList = errorTrainingRmsList;
                IVector errorTrMax = null;
                if (this.ErrorsTrainingMax != null)
                    this.ErrorsTrainingMax.CopyTo(ref errorTrMax);
                trainingParameters.ErrorsTrainingMax = errorTrMax;
                List<IVector> errorTrainingMaxList = null;
                if (this.ErrorsTrainingMaxTable != null)
                {
                    errorTrainingMaxList = new List<IVector>();
                    for (int i = 0; i < this.ErrorsTrainingMaxTable.Length; i++)
                    {
                        IVector tmpErrorsTrainingMax = null;
                        this.ErrorsTrainingMaxTable[i].CopyTo(ref tmpErrorsTrainingMax);
                        errorTrainingMaxList.Add(tmpErrorsTrainingMax);
                    }
                }
                trainingParameters.ErrorsTrainingMaxList = errorTrainingMaxList;
                IVector errorTrMeanAbs = null;
                if (this.ErrorsTrainingMeanAbs != null)
                    this.ErrorsTrainingMeanAbs.CopyTo(ref errorTrMeanAbs);
                trainingParameters.ErrorsTrainingMeanAbs = errorTrMeanAbs;
                IVector errorVerRms = null;
                if (this.ErrorsVerificationRms != null)
                    this.ErrorsVerificationRms.CopyTo(ref errorVerRms);
                trainingParameters.ErrorsVerificationRms = errorVerRms;
                List<IVector> errorVerificationRmsList = null;
                if (this.ErrorsVerificationRmsTable != null)
                {
                    errorVerificationRmsList = new List<IVector>();
                    for (int i = 0; i < this.ErrorsVerificationRmsTable.Length; i++)
                    {
                        IVector tmpErrorsVerificationRms = null;
                        this.ErrorsVerificationRmsTable[i].CopyTo(ref tmpErrorsVerificationRms);
                        errorVerificationRmsList.Add(tmpErrorsVerificationRms);
                    }
                }
                trainingParameters.ErrorsVerificationRmsList = errorVerificationRmsList;
                IVector errorVerMax = null;
                if (this.ErrorsVerificationMax != null)
                    this.ErrorsVerificationMax.CopyTo(ref errorVerMax);
                trainingParameters.ErrorsVerificationMax = errorVerMax;
                List<IVector> errorVerificationMaxList = null;
                if (this.ErrorsVerificationMaxTable != null)
                {
                    errorVerificationMaxList = new List<IVector>();
                    for (int i = 0; i < this.ErrorsVerificationMaxTable.Length; i++)
                    {
                        IVector tmpErrorsVerificationMax = null;
                        this.ErrorsVerificationMaxTable[i].CopyTo(ref tmpErrorsVerificationMax);
                        errorVerificationMaxList.Add(tmpErrorsVerificationMax);
                    }
                }
                trainingParameters.ErrorsVerificationMaxList = errorVerificationMaxList;
                IVector errorVerAbs = null;
                if (this.ErrorsVerificationMeanAbs != null)
                    this.ErrorsVerificationMeanAbs.CopyTo(ref errorVerAbs);
                trainingParameters.ErrorsVerificationMeanAbs = errorVerAbs;
                //List<IVector> errorsRmsList = null;
                //if (this.ErrorsRmsList != null)
                //{
                //    errorsRmsList = new List<IVector>();
                //    for (int i=0; i<this.ErrorsRmsList.Count;i++)
                //    {
                //        IVector tmpErrorsRms = null;
                //        this.ErrorsRmsList[i].CopyTo(ref tmpErrorsRms);
                //        errorsRmsList.Add(tmpErrorsRms);
                //    }
                //}
                //trainingParameters.ConvergenceListRms = errorsRmsList;

                trainingParameters.SetEpochNumbers(EpochNumbers);
                trainingParameters.SetEpochErrorsRms(EpochErrorsRms);
                trainingParameters.SetEpochErrorsAbs(EpochErrorsAbs);
            }
        }

        #endregion Operation


    }  // NeuralTrainingParametersDto


    /// <summary>Transfer Object (DTO) for neural network training limits.</summary>
    /// $A Igor Jul10 Aug12; Tako78 Aug12;
    public class NeuralTrainingLimitsDto : SerializationDto<NeuralTrainingLimits>
    {

        #region Data

        #region TrainingParameters

        /// <summary>Minimum limit for learning rate.</summary>
        public double LearningRateMin;
        /// <summary>Maximum limit for learning rate.</summary>
        public double LearningRateMax;
        /// <summary>Number of learning rates.</summary>
        public int LearningRateNum;

        /// <summary>Minimum limit for momentum.</summary>
        public double MomentumMin;
        /// <summary>Maximum limit for momentum.</summary>
        public double MomentumMax;
        /// <summary>Number of momentums.</summary>
        public int MomentumNum;

        /// <summary>Minimum limit for alpha value.</summary>
        public double AlphaMin;
        /// <summary>Maximum limit for alpha value.</summary>
        public double AlphaMax;
        /// <summary>Number of alpha values.</summary>
        public int AlphaNum;

        /// <summary>Minimum limit for input safety factor value.</summary>
        public double InputSafetyFactorMin;
        /// <summary>Maximum limit for input safety factor value.</summary>
        public double InputSafetyFactorMax;
        /// <summary>Number of input safety factor values.</summary>
        public int InputSafetyFactorNum;

        /// <summary>Minimum limit for output safety factor value.</summary>
        public double OutputSafetyFactorMin;
        /// <summary>Maximum limit for output safety factor value.</summary>
        public double OutputSafetyFactorMax;
        /// <summary>Number of output safety factor values.</summary>
        public int OutputSafetyFactorNum;


        /// <summary>Maximum number of epochs performed in training.</summary>
        public int MaxEpochs;
        /// <summary>Number of epochs in boundle.</summary>
        public int EpochBundle;


        // TODO: check whether EnableRangeTolerance is necessary!

        /// <summary>Flag for enabling toelrance that represent a percentage of the output range.</summary>
        public bool EnableRangeTolerance;

        /// <summary>Tolerance over RMS error of outputs over training points.
        /// Training will continue until error becomes below tolerance or until maximal number of epochs is reached.
        /// If less or equal than 0 then this tolerance is not taken into account.</summary>
        /// $A Tako78 Jul12;; Igor Jul12;
        public VectorDtoBase ToleranceRms;

        /// <summary>Relative tolerances on RMS errors of outputs over training points, relative to the 
        /// correspoinding ranges of output data.</summary>
        /// <seealso cref="NeuralApproximatorBase.ToleranceRmsRelativeToRange"/>
        public VectorDtoBase ToleranceRmsRelativeToRange;

        /// <summary>Scalar through which all components of the Relative tolerances on RMS errors of outputs
        /// can be set to the same value.</summary>
        /// <seealso cref="NeuralApproximatorBase.ToleranceRmsRelativeToRangeScalar"/>
        public double ToleranceRmsRelativeToRangeScalar = NeuralTrainingParameters.DefaultToleranceRmsRelativeToRangeScalar;

        /// <summary>Tolerance on maximal error of outputs over training points.
        /// Training will continue until error becomes below tolerance or until maximal number of epochs is reached.
        /// If less or equal than 0 then this tolerance is not taken into account.</summary>
        /// $A Tako78 Jul12;
        public VectorDtoBase ToleranceMax;

        /// <summary>Relative tolerances on max. abs. errors of outputs over training points, relative to the 
        /// correspoinding ranges of output data.</summary>
        /// <seealso cref="NeuralApproximatorBase.ToleranceMaxRelativeToRange"/>
        public VectorDtoBase ToleranceMaxRelativeToRange;

        /// <summary>Scalar through which all components of the Relative tolerances on max. abs. errors of outputs
        /// can be set to the same value.</summary>
        /// <seealso cref="NeuralApproximatorBase.ToleranceMaxRelativeToRangeScalar"/>
        public double ToleranceMaxRelativeToRangeScalar = NeuralTrainingParameters.DefaultToleranceMaxRelativeToRangeScalar;

        /// <summary>Number of input neurons.</summary>
        public int InputLenght;
        /// <summary>Number of output neurons.</summary>
        public int OutputLength;


        #endregion TrainingParameters


        #region NetworkArchitecture

        /// <summary> Flag for enabling test in architecture of ANN. </summary>
        public bool EnableArchitectureTest;

        ///// <summary>Minimum number of hidden layers in neural network.</summary>
        //public int NumHiddenLayersMin;
        ///// <summary>Maximim number of hidden layers in neural network.</summary>
        //public int NumHiddenLayersMax;
        /// <summary>Number of hidden layers in neural network.</summary>
        public int NumHiddenLayersNum;

        /// <summary>Minimum number of hidden neurons in first hidden layer.</summary>
        public int NumHiddenNeuronsFirstMin;
        /// <summary>Maximum number of hidden neurons in first hidden layer.</summary>
        public int NumHiddenNeuronsFirstMax;
        /// <summary>Number of numbers of hidden neurons in first hidden layer.</summary>
        public int NumHiddenNeuronsFirstNum;
        /// <summary>Values for number of hidden neurons in the first hidden layer.</summary>
        public int[] NumHiddenNeuronsFirstValues;

        /// <summary>Minimum number of hidden neurons in second hidden layer.</summary>
        public int NumHiddenNeuronsSecondMin;
        /// <summary>Maximum number of hidden neurons in second hidden layer.</summary>
        public int NumHiddenNeuronsSecondMax;
        /// <summary>Number of numbers of hidden neurons in second hidden layer.</summary>
        public int NumHiddenNeuronsSecondNum;
        /// <summary>Values for number of hidden neurons in the second hidden layer.</summary>
        public int[] NumHiddenNeuronsSecondValues;

        /// <summary>Minimum number of hidden neurons in third hidden layer.</summary>
        public int NumHiddenNeuronsThirdMin;
        /// <summary>Maximum number of hidden neurons in third hidden layer.</summary>
        public int NumHiddenNeuronsThirdMax;
        /// <summary>Number of numbers of hidden neurons in third hidden layer.</summary>
        public int NumHiddenNeuronsThirdNum;
        /// <summary>Values for number of hidden neurons in the third hidden layer.</summary>
        public int[] NumHiddenNeuronsThirdValues;

        #endregion NetworkArchitecture

        #endregion Data


        #region Operation

        /// <summary>Creates and returns a new object of the corresponding type.</summary>
        public override NeuralTrainingLimits CreateObject()
        {
            NeuralTrainingLimits lim = new NeuralTrainingLimits();
            return lim;
        }


        /// <summary>Copies the specified training limits to the current DTO.</summary>
        /// <param name="trainingLimits">Object that is copied to the current DTO.</param>
        protected override void CopyFromPlain(NeuralTrainingLimits trainingLimits)
        {
            if (trainingLimits==null)
            {
                this.SetNull(true);
            } else
            {
                this.SetNull(false);
                // Learning parameters:
                this.LearningRateMin = trainingLimits.LearningRateMin;
                this.LearningRateMax = trainingLimits.LearningRateMax;
                this.LearningRateNum = trainingLimits.LearningRateNum;

                this.MomentumMin = trainingLimits.MomentumMin;
                this.MomentumMax = trainingLimits.MomentumMax;
                this.MomentumNum = trainingLimits.MomentumNum;

                this.AlphaMin = trainingLimits.AlphaMin;
                this.AlphaMax = trainingLimits.AlphaMax;
                this.AlphaNum = trainingLimits.AlphaNum;

                this.InputSafetyFactorMin = trainingLimits.InputSafetyFactorMin;
                this.InputSafetyFactorMax = trainingLimits.InputSafetyFactorMax;
                this.InputSafetyFactorNum = trainingLimits.InputSafetyFactorNum;

                this.OutputSafetyFactorMin = trainingLimits.OutputSafetyFactorMin;
                this.OutputSafetyFactorMax = trainingLimits.OutputSafetyFactorMax;
                this.OutputSafetyFactorNum = trainingLimits.OutputSafetyFactorNum;

                this.MaxEpochs = trainingLimits.MaxEpochs;
                this.EpochBundle = trainingLimits.EpochBundle;
                this.EnableRangeTolerance = trainingLimits.EnableRangeTolerance;
                this.InputLenght = trainingLimits.InputLenght;
                this.OutputLength = trainingLimits.OutputLength;

                if (trainingLimits.ToleranceRms == null)
                    this.ToleranceRms = null;
                else
                {
                    this.ToleranceRms = new VectorDtoBase();
                    this.ToleranceRms.CopyFrom(trainingLimits.ToleranceRms);
                }
                if (trainingLimits.ToleranceRmsRelativeToRange == null)
                    this.ToleranceRmsRelativeToRange = null;
                else
                {
                    this.ToleranceRmsRelativeToRange = new VectorDtoBase();
                    this.ToleranceRmsRelativeToRange.CopyFrom(trainingLimits.ToleranceRmsRelativeToRange);
                }
                this.ToleranceRmsRelativeToRangeScalar = trainingLimits.ToleranceRmsRelativeToRangeScalar;


                if (trainingLimits.ToleranceMax == null)
                    this.ToleranceMax = null;
                else
                {
                    this.ToleranceMax = new VectorDtoBase();
                    this.ToleranceMax.CopyFrom(trainingLimits.ToleranceMax);
                }
                if (trainingLimits.ToleranceMaxRelativeToRange == null)
                    this.ToleranceMaxRelativeToRange = null;
                else
                {
                    this.ToleranceMaxRelativeToRange = new VectorDtoBase();
                    this.ToleranceMaxRelativeToRange.CopyFrom(trainingLimits.ToleranceMaxRelativeToRange);
                }
                this.ToleranceMaxRelativeToRangeScalar = trainingLimits.ToleranceMaxRelativeToRangeScalar;

                // Network architecture:
                this.EnableArchitectureTest = trainingLimits.EnableArchitectureTest;

                //this.NumHiddenLayersMin = trainingLimits.NumHiddenLayersMin;
                //this.NumHiddenLayersMax = trainingLimits.NumHiddenLayersMax;
                this.NumHiddenLayersNum = trainingLimits.NumHiddenLayersNum;

                this.NumHiddenNeuronsFirstMin = trainingLimits.NumHiddenNeuronsFirstMin;
                this.NumHiddenNeuronsFirstMax = trainingLimits.NumHiddenNeuronsFirstMax;
                this.NumHiddenNeuronsFirstNum = trainingLimits.NumHiddenNeuronsFirstNum;
                this.NumHiddenNeuronsFirstValues = trainingLimits.NumHiddenNeuronsFirstValues;

                this.NumHiddenNeuronsSecondMin = trainingLimits.NumHiddenNeuronsSecondMin;
                this.NumHiddenNeuronsSecondMax = trainingLimits.NumHiddenNeuronsSecondMax;
                this.NumHiddenNeuronsSecondNum = trainingLimits.NumHiddenNeuronsSecondNum;
                this.NumHiddenNeuronsSecondValues = trainingLimits.NumHiddenNeuronsSecondValues;

                this.NumHiddenNeuronsThirdMin = trainingLimits.NumHiddenNeuronsThirdMin;
                this.NumHiddenNeuronsThirdMax = trainingLimits.NumHiddenNeuronsThirdMax;
                this.NumHiddenNeuronsThirdNum = trainingLimits.NumHiddenNeuronsThirdNum;
                this.NumHiddenNeuronsThirdValues = trainingLimits.NumHiddenNeuronsThirdValues;

            }
        }


        /// <summary>Copies contents of the current DTO to the specified training limits object.</summary>
        /// <param name="trainingLimits">Object that the current DTO content is copied to.</param>
        protected override void CopyToPlain(ref NeuralTrainingLimits trainingLimits)
        {
            if (this.GetNull())
                trainingLimits = null;
            else
            {
                // Learning parameters:
                trainingLimits.LearningRateMin = this.LearningRateMin;
                trainingLimits.LearningRateMax = this.LearningRateMax;
                trainingLimits.LearningRateNum = this.LearningRateNum;

                trainingLimits.MomentumMin = this.MomentumMin;
                trainingLimits.MomentumMax = this.MomentumMax;
                trainingLimits.MomentumNum = this.MomentumNum;

                trainingLimits.AlphaMin = this.AlphaMin;
                trainingLimits.AlphaMax = this.AlphaMax;
                trainingLimits.AlphaNum = this.AlphaNum;

                trainingLimits.InputSafetyFactorMin = this.InputSafetyFactorMin;
                trainingLimits.InputSafetyFactorMax = this.InputSafetyFactorMax;
                trainingLimits.InputSafetyFactorNum = this.InputSafetyFactorNum;

                trainingLimits.OutputSafetyFactorMin = this.OutputSafetyFactorMin;
                trainingLimits.OutputSafetyFactorMax = this.OutputSafetyFactorMax;
                trainingLimits.OutputSafetyFactorNum = this.OutputSafetyFactorNum;

                trainingLimits.MaxEpochs = this.MaxEpochs;
                trainingLimits.EpochBundle = this.EpochBundle;
                trainingLimits.EnableRangeTolerance = this.EnableRangeTolerance;
                trainingLimits.InputLenght = this.InputLenght;
                trainingLimits.OutputLength = this.OutputLength;

                // Stopping criteria:
                IVector tolRms = null;
                if (this.ToleranceRms != null)
                    this.ToleranceRms.CopyTo(ref tolRms);
                //if (tolRms == null)
                //{
                //    // try this for compatibility reasons, if the vector is stored under old name:  
                //    if (this.ToleranceRMS != null)
                //            this.ToleranceRMS.CopyTo(ref tolRms);
                //}
                trainingLimits.ToleranceRms = tolRms;
                IVector tolRmsRel = null;
                if (this.ToleranceRmsRelativeToRange != null)
                    this.ToleranceRmsRelativeToRange.CopyTo(ref tolRmsRel);
                trainingLimits.ToleranceRmsRelativeToRange = tolRmsRel;
                trainingLimits.ToleranceRmsRelativeToRangeScalar = this.ToleranceRmsRelativeToRangeScalar;

                IVector tolMax = null;
                if (this.ToleranceMax != null)
                    this.ToleranceMax.CopyTo(ref tolMax);
                trainingLimits.ToleranceMax = tolMax;
                IVector tolMaxRel = null;
                if (this.ToleranceMaxRelativeToRange != null)
                    this.ToleranceMaxRelativeToRange.CopyTo(ref tolMaxRel);
                trainingLimits.ToleranceMaxRelativeToRange = tolMaxRel;
                trainingLimits.ToleranceMaxRelativeToRangeScalar = this.ToleranceMaxRelativeToRangeScalar;

                // Network architecture:
                trainingLimits.EnableArchitectureTest = this.EnableArchitectureTest;

                //trainingLimits.NumHiddenLayersMin = this.NumHiddenLayersMin;
                //trainingLimits.NumHiddenLayersMax = this.NumHiddenLayersMax;
                trainingLimits.NumHiddenLayersNum = this.NumHiddenLayersNum;

                trainingLimits.NumHiddenNeuronsFirstMin = this.NumHiddenNeuronsFirstMin;
                trainingLimits.NumHiddenNeuronsFirstMax = this.NumHiddenNeuronsFirstMax;
                trainingLimits.NumHiddenNeuronsFirstNum = this.NumHiddenNeuronsFirstNum;
                trainingLimits.NumHiddenNeuronsFirstValues = this.NumHiddenNeuronsFirstValues;

                trainingLimits.NumHiddenNeuronsSecondMin = this.NumHiddenNeuronsSecondMin;
                trainingLimits.NumHiddenNeuronsSecondMax = this.NumHiddenNeuronsSecondMax;
                trainingLimits.NumHiddenNeuronsSecondNum = this.NumHiddenNeuronsSecondNum;
                trainingLimits.NumHiddenNeuronsSecondValues = this.NumHiddenNeuronsSecondValues;

                trainingLimits.NumHiddenNeuronsThirdMin = this.NumHiddenNeuronsThirdMin;
                trainingLimits.NumHiddenNeuronsThirdMax = this.NumHiddenNeuronsThirdMax;
                trainingLimits.NumHiddenNeuronsThirdNum = this.NumHiddenNeuronsThirdNum;
                trainingLimits.NumHiddenNeuronsThirdValues = this.NumHiddenNeuronsThirdValues;
            }
        }


        #endregion Operation

    }  // class NeuralTrainingLimitsDto


    /// <summary>Transfer Object (DTO) for neural network training results.</summary>
    /// $A Tako78 Aug12;
    public class NeuralTrainingTableDto : SerializationDto<NeuralTrainingTable>
    {

        #region data

        #region neural network limits

        /// <summary>Contains Parameters that define neural network architecture limits and trainig parameter limits.<summary>
        public NeuralTrainingLimitsDto TrainingLimits;

        #endregion neural network limits


        #region neural network parameters

        /// <summary>Contains Parameters that define neural network architecture and trainig procedure, together with 
        /// achieved results after training such as various error norms.<summary>
        public NeuralTrainingParametersDto[] TrainingParameters;

        #endregion neural network parameters


        #endregion data


        #region Operation

        /// <summary>Creates and returns a new object of the corresponding type.</summary>
        public override NeuralTrainingTable CreateObject()
        {
            NeuralTrainingTable res = new NeuralTrainingTable();
            return res;
        }

        /// <summary>Copies the specified training results to the current DTO.</summary>
        /// <param name="trainingParameters">Object that is copied to the current DTO.</param>
        protected override void CopyFromPlain(NeuralTrainingTable trainingResults)
        {
            if (trainingResults==null)
            {
                this.SetNull(true);
            } else
            {
                this.SetNull(false);
                // training limits
                if (trainingResults.TrainingLimits == null)
                    this.TrainingLimits = null;
                else
                {
                    this.TrainingLimits = new NeuralTrainingLimitsDto();
                    this.TrainingLimits.CopyFrom(trainingResults.TrainingLimits);
                }

                // training results
                if (trainingResults.TrainingParameters == null)
                    this.TrainingParameters = null;
                else
                {
                    this.TrainingParameters = new NeuralTrainingParametersDto[trainingResults.TrainingParameters.Count];
                    for (int i = 0; i < trainingResults.TrainingParameters.Count; i++)
                    {
                        NeuralTrainingParametersDto tmpTrainingParameters = new NeuralTrainingParametersDto();
                        tmpTrainingParameters.CopyFrom(trainingResults.TrainingParameters[i]);
                        this.TrainingParameters[i] = tmpTrainingParameters;
                    }
                }


            }
        }

        /// <summary>Copies contents of the current DTO to the specified training results object.</summary>
        /// <param name="trainingParameters">Object that the current DTO content is copied to.</param>
        protected override void CopyToPlain(ref NeuralTrainingTable trainingResults)
        {
            if (this.GetNull())
                trainingResults = null;
            else
            {
                // training limits
                NeuralTrainingLimits trainingLimits = null;
                if (this.TrainingLimits != null)
                    this.TrainingLimits.CopyTo(ref trainingLimits);
                trainingResults.TrainingLimits = trainingLimits;

                // training results
                List<NeuralTrainingParameters> trainingParameters = null;
                if (this.TrainingParameters != null)
                {
                    trainingParameters = new List<NeuralTrainingParameters>();
                    for (int i = 0; i < this.TrainingParameters.Length; i++)
                    {
                        NeuralTrainingParameters tmpTrainingParameters = null;
                        this.TrainingParameters[i].CopyTo(ref tmpTrainingParameters);
                        trainingParameters.Add(tmpTrainingParameters);
                    }
                }
                trainingResults.TrainingParameters = trainingParameters;

            }
        }


        #endregion Operation
    }

}

