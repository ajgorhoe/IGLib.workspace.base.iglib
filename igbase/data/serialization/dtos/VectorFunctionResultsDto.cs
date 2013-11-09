// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/

// CLASSES FOR DATA TRANSFER OBJECTS THAT FACILITATE SERIALIZATION OF (OPTIMIZATION) ANALYSIS RESULTS.

using System.IO;
using System.IO.Compression;
using System.Xml;
using System.Text;
using System;
using System.Collections.Generic;

using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

using IG.Lib;

namespace IG.Num
{



    /// <summary>DTO (Data Transfer Objects) for storing contents of vector function evaluation request.</summary>
    /// $A Igor May0;
    public class VectorFunctionRequestDTO : SerializationDtoBase<VectorFunctionResults, VectorFunctionResults>
    {

        #region Construction

        /// <summary>Default constructor.</summary>
        public VectorFunctionRequestDTO()
        { }

        #endregion Construction

        #region Data

        /// <summary>Number of parameters of an optimization problem.</summary>
        public int NumParameters;

        /// <summary>Number of components (functions) of vector functon.</summary>
        public int NumFunctons;

        /// <summary>Indicates whether calculation of functions is/was requested.</summary>
        public bool ReqValues = false;

        /// <summary>Indicates whether calculation of function gradients is/was requested.</summary>
        public bool ReqGradients= false;

        /// <summary>Indicates whether calculation of functions' Hessians is/was requested.</summary>
        public bool ReqHessians = false;

        public VectorDtoBase Parameters;

        #endregion Data


        #region Operation

        /// <summary>Creates and returns a new vector function evaluation request object.</summary>
        public override VectorFunctionResults CreateObject()
        {
            return new VectorFunctionResults();
        }

        /// <summary>Copies data to the current DTO from an vector function results object.</summary>
        /// <param name="vectorFunctionRes">Vector function results object from which data is copied.</param>
        protected override void CopyFromPlain(VectorFunctionResults vectorFunctionRes)
        {
            if (vectorFunctionRes == null)
            {
                this.SetNull(true);
            }
            else
            {
                this.SetNull(false);
                this.NumParameters = vectorFunctionRes.NumParameters;
                this.NumFunctons = vectorFunctionRes.NumFunctions;
                if (vectorFunctionRes.Parameters == null)
                    this.Parameters = null;
                else
                {
                    this.Parameters = new VectorDtoBase();
                    this.Parameters.CopyFrom(vectorFunctionRes.Parameters);
                }
                this.ReqValues = vectorFunctionRes.ReqValues;
                this.ReqGradients = vectorFunctionRes.ReqGradients;
                this.ReqHessians = vectorFunctionRes.ReqHessians;
            }
        }

        /// <summary>Copies data from the current DTO to a vector object.</summary>
        /// <param name="vec">Analysis results object that data is copied to.</param>
        protected override void CopyToPlain(ref VectorFunctionResults vectorFunctionRes)
        {
            if (GetNull())
                vectorFunctionRes = null;
            else
            {
                vectorFunctionRes = new VectorFunctionResults();
                vectorFunctionRes.NumParameters = this.NumParameters;
                vectorFunctionRes.NumFunctions = this.NumFunctons;

                if (Parameters == null)
                    vectorFunctionRes.Parameters = null;
                else
                {
                    IVector vec = null;
                    Parameters.CopyTo(ref vec);
                    vectorFunctionRes.SetParametersReference(vec);
                }
                vectorFunctionRes.ReqValues = this.ReqValues;
                vectorFunctionRes.ReqGradients = this.ReqGradients;
                vectorFunctionRes.ReqHessians = this.ReqHessians;

                vectorFunctionRes.ErrorCode = 0;
                vectorFunctionRes.ErrorString = null;
                vectorFunctionRes.Calculated = false;
            }
        }

        #endregion Operation

    }  // class AnalysisRequestDTO



    /// <summary>DTO (Data Transfer Objects) for storing contents of vector function results.</summary>
    /// $A Igor May10;
    public class VectorFunctionResultsDto : VectorFunctionRequestDTO
    {

        #region Construction

        /// <summary>Default constructor.</summary>
        public VectorFunctionResultsDto()
            : base()
        { }

        #endregion Construction

        #region Data

        /// <summary>Error code.
        ///   0 - everything is OK.
        ///   negative value - something went wrong.</summary>
        public int ErrorCode;

        /// <summary>Error string indicating what went wrong.</summary>
        public String ErrorString;


        /// <summary>Indicates whether calculation of functions is/was requested.</summary>
        public bool CalculatedValues;

        /// <summary>Indicates whether calculation of functions' gradients is/was requested.</summary>
        public bool CalculatedGradients;

        /// <summary>Indicates whether calculation of functions' Hessian is/was requested.</summary>
        public bool CalculatedHessians;

        public double[] Values;

        public VectorDtoBase[] Gradients;

        public MatrixDtoBase[] Hessians;

        #endregion Data

        #region Operation


        /// <summary>Copies data to the current DTO from an vector function results results object.</summary>
        /// <param name="vectorFunctionRes">Vector function results object from which data is copied.</param>
        protected override void CopyFromPlain(VectorFunctionResults vectorFunctionRes)
        {
            base.CopyFromPlain(vectorFunctionRes);
            if (vectorFunctionRes != null)
            {
                this.ErrorCode = vectorFunctionRes.ErrorCode;
                this.ErrorString = vectorFunctionRes.ErrorString;
                this.CalculatedValues = vectorFunctionRes.CalculatedValues;
                this.CalculatedGradients = vectorFunctionRes.CalculatedGradients;
                this.CalculatedHessians = vectorFunctionRes.CalculatedHessians;
                if (vectorFunctionRes.Values == null)
                    this.Values = null;
                else
                {
                    this.Values = new double[vectorFunctionRes.Values.Count];
                    for (int i = 0; i < this.Values.Length; ++i )
                        this.Values[i] = vectorFunctionRes.Values[i];
                }
                if (vectorFunctionRes.Gradients == null)
                    this.Gradients = null;
                else
                {
                    this.Gradients = new VectorDtoBase[vectorFunctionRes.Gradients.Count];
                    for (int i = 0; i < this.Gradients.Length; ++i)
                    {
                        VectorDtoBase grad = new VectorDtoBase();
                        grad.CopyFrom(vectorFunctionRes.Gradients[i]);
                        this.Gradients[i] = grad;
                    }
                }
                if (vectorFunctionRes.Hessians == null)
                    this.Hessians = null;
                else
                {
                    this.Hessians = new MatrixDtoBase[vectorFunctionRes.Hessians.Count];
                    for (int i = 0; i < this.Hessians.Length; ++i)
                    {
                        MatrixDtoBase hess = new MatrixDtoBase();
                        hess.CopyFrom(vectorFunctionRes.Hessians[i]);
                        this.Hessians[i] = hess;
                    }
                }

            }
        }

        /// <summary>Copies data from the current DTO to a vector function results object.</summary>
        /// <param name="vec">Vector object that data is copied to.</param>
        protected override void CopyToPlain(ref VectorFunctionResults vectorFunctionRes)
        {
            base.CopyToPlain(ref vectorFunctionRes);
            if (vectorFunctionRes != null)
            {
                vectorFunctionRes.ErrorCode = this.ErrorCode;
                vectorFunctionRes.ErrorString = this.ErrorString;

                vectorFunctionRes.CalculatedValues = this.CalculatedValues;
                vectorFunctionRes.CalculatedGradients = this.CalculatedGradients;
                vectorFunctionRes.CalculatedHessians = this.CalculatedHessians;

                if (this.Values == null)
                {
                    vectorFunctionRes.Values = null;
                }
                else
                {
                     int num = this.Values.Length;
                    List<double> functionValues = vectorFunctionRes.Values;
                    Util.ResizeList<double>(ref functionValues, num, 0, true);
                    for (int i = 0; i < num; ++i)
                    {
                        functionValues[i] = this.Values[i];
                    }
                    vectorFunctionRes.Values = functionValues;
                   
                }

                if (this.Gradients == null)
                {
                    vectorFunctionRes.Gradients = null;
                } else
                {
                    int num = this.Gradients.Length;
                    List<IVector> functionGradients = vectorFunctionRes.Gradients;
                    Util.ResizeList<IVector>(ref functionGradients, num, null, true);
                    for (int i = 0; i < num; ++i)
                    {
                        if (this.Gradients[i] == null)
                            functionGradients[i] = null;
                        else
                        {
                            IVector grad = functionGradients[i];
                            this.Gradients[i].CopyTo(ref grad);
                            functionGradients[i] = grad;
                        }
                    }
                    vectorFunctionRes.Gradients = functionGradients;
                }

                if (this.Hessians == null)
                {
                    vectorFunctionRes.Hessians = null;
                } else
                {
                    int num = this.Hessians.Length;
                    List<IMatrix> functionHessians = vectorFunctionRes.Hessians;
                    Util.ResizeList<IMatrix>(ref functionHessians, num, null, true);
                    for (int i = 0; i < num; ++i)
                    {
                        if (this.Hessians[i] == null)
                            functionHessians[i] = null;
                        else
                        {
                            IMatrix hess = functionHessians[i];
                            this.Hessians[i].CopyTo(ref hess);
                            functionHessians[i] = hess;
                        }
                    }
                    vectorFunctionRes.Hessians = functionHessians;
                }

            }
        }


        #endregion Operation


    }  // class VectorFunctionResultsDTO


}
