// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using IG.Lib;
using IG.Num;

namespace IG.Neural
{

    [Obsolete("This class is obsolete, use NeuralTrainingElement class instead.")]
    public class NeuralTadej
    {
        #region Analysis Data


        /// <summary>Read the analysis data from data file
        /// Format: { { p1, p2, … }, { reqcalcobj, reqcalcconstr, reqcalcgradobj, reqcalcgradconstr }, cd } </summary>
        /// <param name="inputFilePath">Path to the file where training data are saved.</param>
        /// <param name="parameters">Input and output parameters: { p1, p2, … }.</param>
        /// <param name="reqcalcobj">Flag: reqcalcobj.</param>
        /// <param name="reqcalcconstr">Flag: reqcalcconstr.</param>
        /// <param name="reqcalcgradobj">Flag: reqcalcgradobj.</param>
        /// <param name="reqcalcgradconstr">Flag: reqcalcgradconstr.</param>
        /// <param name="cd">String: cd.</param>
        /// $A Tako78 Mar11;
        public static void ReadAnalysisRequest(string filePath, ref IVector parameters,
            ref bool reqcalcobj, ref bool reqcalcconstr, ref bool reqcalcgradobj, ref bool reqcalcgradconstr, ref string cd)
        {
            string requestString;
            using (StreamReader sr = new StreamReader(filePath))
            {
                requestString = sr.ReadToEnd();
            }
            GetAnalysisRequest(requestString, ref parameters,
            ref reqcalcobj, ref reqcalcconstr, ref reqcalcgradobj, ref reqcalcgradconstr, ref cd);
        }


        /// <summary>Read the analysis request data from data file
        /// Format: { { p1, p2, … }, { reqcalcobj, reqcalcconstr, reqcalcgradobj, reqcalcgradconstr }, cd } </summary>
        /// <param name="requestString">String with request analysis data.</param>
        /// <param name="parameters">Input and output parameters: { p1, p2, … }.</param>
        /// <param name="reqcalcobj">Flag: reqcalcobj.</param>
        /// <param name="reqcalcconstr">Flag: reqcalcconstr.</param>
        /// <param name="reqcalcgradobj">Flag: reqcalcgradobj.</param>
        /// <param name="reqcalcgradconstr">Flag: reqcalcgradconstr.</param>
        /// <param name="cd">String: cd.</param>
        /// $A Tako78 Mar11;
        public static void GetAnalysisRequest(string requestString, ref IVector parameters,
            ref bool reqcalcobj, ref bool reqcalcconstr, ref bool reqcalcgradobj, ref bool reqcalcgradconstr, ref string cd)
        {
            // Format:
            // { { p1, p2, … }, { reqcalcobj, reqcalcconstr, reqcalcgradobj, reqcalcgradconstr }, cd }

            // Legend:
            //•	calcobj – flag for the objective function
            //•	calcconstr – flag for constraint functions
            //•	calcgradobj – gradient of the objective function
            //•	calcgradconstr – gradients of constraint functions
            //obj – value of the objective functions
            //constr1, constr2, … - values of the constraint functions
            //dobjdp1, dobjdp2, ... – derivatives of the objective function with respect to individual parameters (components of the objective function gradient)
            //dconstr1dp1, …, dconstr2dp1, dconstr2dp2 – derivatives of individual constraint functions with respect to individual optimization parameters – components of gradients of the constraint functions (e.g. dconstr2dp3 is the derivative of the second constraint function with respect to the third parameter)
            //errorcode – integer error code of analysis – 0 for no error, usually a negative number for errors, values are function specific
            //reqcalcob , reqcalcconstr, reqcalcgradobj and reqcalcgradconstr are request flags for calculation of the various values, as have been passed to the analysis function. The same as with parameter values, these flags are requested only for verification. In vast majority of cases these flags will not be used by the optimization program, and they can simply be set to 1.

            if (requestString == null)
                throw new ArgumentNullException("Analysis request string is empty.");
            requestString = requestString.Trim();
            string[] requestLine = requestString.Split('{','[',']', '}');
            requestLine[2] = requestLine[2].Trim();
            //read parameter data
            string[] param = requestLine[2].Split(',');
            if (parameters == null)
                parameters = new Vector(param.Length);
            for (int i = 0; i < param.Length; i++)
            {
                param[i] = param[i].Trim();
                parameters[i] = double.Parse(param[i], System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.NumberFormatInfo.InvariantInfo);
            }
            //read boolean data
            requestLine[5].Trim();
            string[] strBool = requestLine[4].Split(',');
            bool[] tmpBool = new bool[strBool.Length];
            for (int i = 0; i < strBool.Length; i++)
            {
                strBool[i] = strBool[i].Trim();
                tmpBool[i] = IG.Lib.UtilStr.ToBoolean(strBool[i]);
            }
            reqcalcobj = tmpBool[0];
            reqcalcconstr = tmpBool[1];
            reqcalcgradobj = tmpBool[2];
            reqcalcgradconstr = tmpBool[3];
            //read cd data
            requestLine[5] = requestLine[5].Trim();
            cd = requestLine[5];
        }

        /// <summary>Read the analysis result data from data file
        /// Format:  </summary>
        /// <param name="inputFilePath">Path to the file where training data are saved.</param>
        /// <param name="parameters">Input and output parameters: { p1, p2, … }.</param>
        /// <param name="calcobj">Flag for the objective function.</param>
        /// <param name="calcconstr">Flag for constraint functions.</param>
        /// <param name="calcgradobj">Gradient of the objective function.</param>
        /// <param name="calcgradconstr">Gradients of constraint functions.</param>
        /// <param name="obj">Value of the objective functions.</param>
        /// <param name="constr">Values of the constraint functions.</param>
        /// <param name="dobjdp">Derivatives of the objective function.</param>
        /// <param name="dconstr">Derivatives of individual constraint functions.</param>
        /// <param name="errorcode">Integer error code of analysis.</param>
        /// <param name="reqcalcobj">Flag for calculation of the various values.</param>
        /// <param name="reqcalcconstr">Flag for calculation of the various values.</param>
        /// <param name="reqcalcgradobj">Flag for calculation of the various values.</param>
        /// <param name="reqcalcgradconstr">Flag for calculation of the various values.</param>
        /// $A Tako78 Apr7;
        public static void ReadAnalysisResult(string filePath, ref IVector parameters, ref bool calcobj, ref bool calcconstr,
            ref bool calcgradobj, ref bool calcgradconstr, ref double obj, ref IVector constr, ref IVector dobjdp, ref IVector[] dconstr,
            ref int errorcode, ref bool reqcalcobj, ref bool reqcalcconstr, ref bool reqcalcgradobj, ref bool reqcalcgradconstr)
        {
            string resultString;
            using (StreamReader sr = new StreamReader(filePath))
            {
                resultString = sr.ReadToEnd();
            }
            GetAnalysisResult(resultString, ref parameters, ref calcobj, ref calcconstr,
                ref calcgradobj, ref calcgradconstr, ref obj, ref constr, ref dobjdp, ref dconstr,
                ref errorcode, ref reqcalcobj, ref reqcalcconstr, ref reqcalcgradobj, ref reqcalcgradconstr);
        }

        /// <summary>Read the analysis result data from data file
        /// Format:  </summary>
        /// <param name="resultString">String with result analysis data.</param>
        /// <param name="parameters">Input and output parameters: { p1, p2, … }.</param>
        /// <param name="calcobj">Flag for the objective function.</param>
        /// <param name="calcconstr">Flag for constraint functions.</param>
        /// <param name="calcgradobj">Gradient of the objective function.</param>
        /// <param name="calcgradconstr">Gradients of constraint functions.</param>
        /// <param name="obj">Value of the objective functions.</param>
        /// <param name="constr">Values of the constraint functions.</param>
        /// <param name="dobjdp">Derivatives of the objective function.</param>
        /// <param name="dconstr">Derivatives of individual constraint functions.</param>
        /// <param name="errorcode">Integer error code of analysis.</param>
        /// <param name="reqcalcobj">Flag for calculation of the various values.</param>
        /// <param name="reqcalcconstr">Flag for calculation of the various values.</param>
        /// <param name="reqcalcgradobj">Flag for calculation of the various values.</param>
        /// <param name="reqcalcgradconstr">Flag for calculation of the various values.</param>
        /// $A Tako78 Apr7;
        public static void GetAnalysisResult(string requestString, ref IVector parameters, ref bool calcobj, ref bool calcconstr,
    ref bool calcgradobj, ref bool calcgradconstr, ref double obj, ref IVector constr, ref IVector dobjdp, ref IVector[] dconstr,
    ref int errorcode, ref bool reqcalcobj, ref bool reqcalcconstr, ref bool reqcalcgradobj, ref bool reqcalcgradconstr)
        {
            if (requestString == null)
                throw new ArgumentNullException("Analysis result string is empty.");
            requestString = requestString.Trim();
            string[] resultLine = requestString.Split('{', '}');
            //Read parameters
            string[] param = resultLine[2].Split(',');
            if (parameters == null)
                parameters = new Vector(param.Length);
            for (int i = 0; i < param.Length; i++)
            {
                param[i] = param[i].Trim();
                parameters[i] = double.Parse(param[i], System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.NumberFormatInfo.InvariantInfo);
            }
            //Read request flags for calculation
            int tmpReqFlag = resultLine.Length - 3;
            string[] regFlag = resultLine[tmpReqFlag].Split(',');
            bool[] boolReqFlag = new bool[regFlag.Length];
            for (int i = 0; i < regFlag.Length; i++)
            {
                regFlag[i] = regFlag[i].Trim();
                boolReqFlag[i] = IG.Lib.UtilStr.ToBoolean(regFlag[i]);
            }
            reqcalcobj = boolReqFlag[0];
            reqcalcconstr = boolReqFlag[1];
            reqcalcgradobj = boolReqFlag[2];
            reqcalcgradconstr = boolReqFlag[3];
            //Read error code
            tmpReqFlag = resultLine.Length - 5;
            resultLine[tmpReqFlag] = resultLine[tmpReqFlag].Trim();
            string[] tmpError = resultLine[tmpReqFlag].Split(',');
            errorcode = int.Parse(tmpError[1]);
            //Read calcobj, obj, calcconstr
            string[] tmpLine4 = resultLine[4].Split(',');
            calcobj = IG.Lib.UtilStr.ToBoolean(tmpLine4[0]);
            tmpLine4[1] = tmpLine4[1].Trim();
            obj = double.Parse(tmpLine4[1], System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.NumberFormatInfo.InvariantInfo);
            calcconstr = IG.Lib.UtilStr.ToBoolean(tmpLine4[2]);
            //Read constr
            string[] tmpConstr = resultLine[5].Split(',');
            resultLine[5] = resultLine[5].Trim();
            if (constr == null)
                constr = new Vector(tmpConstr.Length);
            if (resultLine[5] != "")
            {
                for (int i = 0; i < tmpConstr.Length; i++)
                {
                    tmpConstr[i] = tmpConstr[i].Trim();
                    constr[i] = double.Parse(tmpConstr[i], System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.NumberFormatInfo.InvariantInfo);
                }
            }
            //Read calcgradobj
            string[] tmpCalcgradobj = resultLine[6].Split(',');
            calcgradobj = IG.Lib.UtilStr.ToBoolean(tmpCalcgradobj[1]);
            //Read dobjdp
            string[] tmpDobjdp = resultLine[7].Split(',');
            resultLine[7] = resultLine[7].Trim();
            if (dobjdp == null)
                dobjdp = new Vector(tmpDobjdp.Length);
            if (resultLine[7] != "")
            {
                for (int i = 0; i < tmpDobjdp.Length; i++)
                {
                    tmpDobjdp[i] = tmpDobjdp[i].Trim();
                    dobjdp[i] = double.Parse(tmpDobjdp[i], System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.NumberFormatInfo.InvariantInfo);
                }
            }

            //Read calcgradconstr
            string[] tmpCalcgradconstr = resultLine[8].Split(',');
            calcgradconstr = IG.Lib.UtilStr.ToBoolean(tmpCalcgradconstr[1]);
            //Read dconstr
            List<double[]> dataDconstr = new List<double[]>();
            for (int i = 10; i < (resultLine.Length - 6); i++)
            {

                string[] tmp = resultLine[i].Split(',');
                resultLine[i] = resultLine[i].Trim();
                double[] tmpDconstr = new double[tmp.Length];

                if (resultLine[i] != "")
                {
                    for (int j = 0; j < tmp.Length; j++)
                    {
                        tmp[j] = tmp[j].Trim();
                        tmpDconstr[j] = double.Parse(tmp[j], System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.NumberFormatInfo.InvariantInfo);
                    }
                    dataDconstr.Add(tmpDconstr);
                }
                i++;

            }
            if (dconstr == null)
                dconstr = new IVector[dataDconstr.Count];
            else if (dconstr.Length != dataDconstr.Count)
                dconstr = new IVector[dataDconstr.Count];
            for (int i = 0; i < dconstr.Length; ++i)
                dconstr[i] = new Vector(dataDconstr[i]);
        }


        #endregion

        #region CSV


        /// <summary>Loads training data and Definition data from single CSV file.
        /// <param name="inputFilePath">Path to the file where training data are saved.</param>
        /// <param name="inputLenght">Lenght of input parameters.</param>
        /// <param name="outputLenght">Lenght of output parameters.</param>
        /// <param name="namesSpecified">Flag if names are specified in the file.</param>
        /// <param name="descriptionSpecified">Flag if definitions (descriptions, defaultValue, boundDefiner, minValue, maxValue) are specified in the file.</param>
        /// <param name="trainingData">Training data set.</param>
        /// <param name="definitionData">Definition data set.</param>
        /// $A Tako78 Mar11; June27;
        public static void LoadTrainingDataCSVinOneLine(string filePath, int inputLenght, int outputLenght,
           bool namesSpecified, bool descriptionSpecified, bool titleSpecified,
           ref SampledDataSet trainingData, ref InputOutputDataDefiniton definitionData)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentNullException("File path not specified.");
            // variables 
            int variables = 0;
            string str = null;
            StreamReader reader = null;
            List<double[]> data = new List<double[]>();

            // data names not sbecified, descriptions are specified
            if ((!namesSpecified && descriptionSpecified) || (!namesSpecified && titleSpecified))
                throw new ArgumentException("Data names not specified, but descriptions are.");
            // open selected file
            reader = File.OpenText(filePath);
            // data names not specified
            if (!namesSpecified)
                definitionData = null;  // no definitions, set to null
            else
            {
                if (definitionData == null)
                    definitionData = new InputOutputDataDefiniton();
                else
                {
                    definitionData.InputElementList.Clear();
                    definitionData.OutputElementList.Clear();
                }

                // split line of names
                string line;
                line = reader.ReadLine();
                string[] names = line.Split(';');
                if (names.Length == 1)
                    names = line.Split(',');

                string[] titles = null;
                string[] descriptions = null;
                string[] defaultValue = null;
                string[] minValue = null;
                string[] maxValue = null;
                string[] boundDefiner = null;

                if (titleSpecified)
                {
                    // split line of descriptions
                    line = reader.ReadLine();
                    titles = line.Split(';');
                    if (titles.Length == 1)
                        titles = line.Split(',');
                }
                // descriptions sbecified
                if (descriptionSpecified)
                {
                    // split line of descriptions
                    line = reader.ReadLine();
                    descriptions = line.Split(';');
                    if (descriptions.Length == 1)
                        descriptions = line.Split(',');
                    // split line of defaultValues
                    line = reader.ReadLine();
                    defaultValue = line.Split(';');
                    if (defaultValue.Length == 1)
                        defaultValue = line.Split(',');
                    // split line of bound definer
                    line = reader.ReadLine();
                    boundDefiner = line.Split(';');
                    if (boundDefiner.Length == 1)
                        boundDefiner = line.Split(',');
                    // split line of minimum value
                    line = reader.ReadLine();
                    minValue = line.Split(';');
                    if (minValue.Length == 1)
                        minValue = line.Split(',');
                    // split line of maximum value
                    line = reader.ReadLine();
                    maxValue = line.Split(';');
                    if (maxValue.Length == 1)
                        maxValue = line.Split(',');
                }

                if (names == null)
                    throw new InvalidDataException("Could not read names.");
                if (names.Length != inputLenght + outputLenght)
                    throw new InvalidDataException("Wrong number of names in data definition, "
                        + names.Length + "instead of " + (inputLenght + outputLenght).ToString());
                if (titleSpecified)
                {
                    if (titles == null)
                        throw new InvalidDataException("Could not read descriptions.");
                    if (titles.Length != inputLenght + outputLenght)
                    {
                        throw new InvalidDataException("Wrong number of definitions in data definition.");
                        throw new InvalidDataException("Description lenght " + titles.Length + " instead of " + (inputLenght + outputLenght).ToString());
                    }
                }
                if (descriptionSpecified)
                {
                    if (descriptions == null)
                        throw new InvalidDataException("Could not read descriptions.");
                    if (descriptions.Length != inputLenght + outputLenght)
                    {
                        throw new InvalidDataException("Wrong number of definitions in data definition.");
                        throw new InvalidDataException("Description lenght " + descriptions.Length + " instead of " + (inputLenght + outputLenght).ToString());
                    }
                    if (defaultValue.Length != inputLenght + outputLenght)
                        throw new InvalidDataException("Default value lenght " + defaultValue.Length + " instead of " + (inputLenght).ToString());
                    if (boundDefiner.Length != inputLenght + outputLenght)
                        throw new InvalidDataException("Bound definition lenght " + boundDefiner.Length + " instead of " + (inputLenght).ToString());
                    if (minValue.Length != inputLenght + outputLenght)
                        throw new InvalidDataException("Minimum value lenght " + minValue.Length + " instead of " + (inputLenght).ToString());
                    if (maxValue.Length != inputLenght + outputLenght)
                        throw new InvalidDataException("Maximum value lenght " + maxValue.Length + " instead of " + (inputLenght).ToString());
                }

                // add definition data to NeuralDataDefinition (name and description)
                for (int i = 0; i < inputLenght; i++)
                {
                    double tempDefaultValue = 0.0;
                    double tempMinValue = 0.0;
                    double tempMaxtValue = 0.0;
                    bool tempBoundDefinertValue = false;
                    string name = null, description = null, title = null;
                    if (names != null)
                        name = names[i];
                    if (titles != null)
                        title = titles[i];
                    if (descriptions != null)
                        description = descriptions[i];
                    if (defaultValue != null)
                        tempDefaultValue = double.Parse(defaultValue[i], System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.NumberFormatInfo.InvariantInfo);
                    if (boundDefiner != null)
                    {
                        int tmp;
                        tmp = int.Parse(boundDefiner[i]);
                        if (tmp == 1)
                            tempBoundDefinertValue = true;
                        else if (tmp == 0)
                            tempBoundDefinertValue = false;
                    }
                    if (minValue != null)
                        tempMinValue = double.Parse(minValue[i], System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.NumberFormatInfo.InvariantInfo);
                    if (maxValue != null)
                        tempMaxtValue = double.Parse(maxValue[i], System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.NumberFormatInfo.InvariantInfo);
                    //definitionData.AddInputElement(new NeuralInputElementDefinition(i, name, description));                   
                    InputElementDefinition def = new InputElementDefinition(i, name, name /* $$$$ should be title */, description);
                    def.DefaultValue = tempDefaultValue;
                    def.BoundsDefined = tempBoundDefinertValue;
                    def.MinimalValue = tempMinValue;
                    def.MaximalValue = tempMaxtValue;
                    definitionData.AddInputElement(def);
                }
                for (int i = 0; i < outputLenght; i++)
                {
                    string name = null, description = null, title = null;
                    if (names != null)
                        name = names[i + inputLenght];
                    if (titles != null)
                        title = titles[i + inputLenght];
                    if (descriptions != null)
                        description = descriptions[i + inputLenght];

                    definitionData.AddOutputElement(new OutputElementDefinition(i, name, title , description));
                }
            }
            // read the training data  
            while ((str = reader.ReadLine()) != null)
            {
                // split trainingData 
                string[] strs = str.Split(';');
                if (strs.Length == 1)
                    strs = str.Split(',');

                // allocate data array      
                variables = strs.Length;

                // parse training data     
                double[] tempLineData = new double[inputLenght + outputLenght];
                int j1;
                for (j1 = 0; j1 < variables; j1++)
                {
                    if (strs.Length != (inputLenght + outputLenght))
                    {
                        throw new InvalidDataException("Training data out of range in line " + data.Count);
                    }
                    tempLineData[j1] = double.Parse(strs[j1], System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.NumberFormatInfo.InvariantInfo);
                }
                data.Add(tempLineData);
            }

            // add training data to NeuralTrainingSet
            if (trainingData == null)
                trainingData = new SampledDataSet();
            trainingData.Clear();
            trainingData.InputLength = inputLenght;
            trainingData.OutputLength = outputLenght;

            for (int i = 0; i < data.Count; ++i)
            {
                double[] row = data[i];
                Vector inputData = new Vector(inputLenght);
                for (int j = 0; j < inputLenght; ++j)
                {
                    inputData[j] = row[j];
                }
                Vector outputData = new Vector(outputLenght);
                for (int k = 0; k < outputLenght; k++)
                {
                    outputData[k] = row[k + inputLenght];
                }
                trainingData.AddElement(new SampledDataElement(inputData, outputData));
            }
        }

        /// <summary>Loads training data and Definition data from single CSV file.
        /// <param name="inputFilePath">Path to the file where training data are saved.</param>
        /// <param name="inputLenght">Lenght of input parameters.</param>
        /// <param name="outputLenght">Lenght of output parameters.</param>
        /// <param name="namesSpecified">Flag if names are specified in the file.</param>
        /// <param name="descriptionSpecified">Flag if descriptions are specified in the file.</param>
        /// <param name="trainingData">Training data set.</param>
        /// <param name="definitionData">Definition data set.</param>
        /// $A Tako78 Apr11, June24;
        public static void LoadTrainingDataCSV(string filePath, int inputLenght, int outputLenght,
           bool namesSpecified, bool titleSpecified, bool descriptionSpecified,
           ref SampledDataSet trainingData, ref InputOutputDataDefiniton definitionData)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentNullException("File path not specified.");
            // variables 
            int variables = 0;
            string str = null;
            StreamReader reader = null;
            List<double[]> data = new List<double[]>();

            // data names not sbecified, descriptions are specified
            if ((!namesSpecified && descriptionSpecified) || (!namesSpecified && titleSpecified)) 
                throw new ArgumentException("Data names not specified, but descriptions or titles are.");
            // open selected file
            reader = File.OpenText(filePath);
            // data names not specified
            if (!namesSpecified)
                definitionData = null;  // no definitions, set to null
            else
            {
                if (definitionData == null)
                    definitionData = new InputOutputDataDefiniton();
                else
                {
                    definitionData.InputElementList.Clear();
                    definitionData.OutputElementList.Clear();
                }

                // split line of names
                string line;
                line = reader.ReadLine();
                string[] names = line.Split(';');
                if (names.Length == 1)
                    names = line.Split(',');


                string[] titles = null;
                // title specified
                if (titleSpecified)
                {
                    // split line of titles
                    line = reader.ReadLine();
                    titles = line.Split(';');
                    if (titles.Length == 1)
                        titles = line.Split(',');
                }

                string[] descriptions = null;
                // descriptions specified
                if (descriptionSpecified)
                {
                    // split line of descriptions
                    line = reader.ReadLine();
                    descriptions = line.Split(';');
                    if (descriptions.Length == 1)
                        descriptions = line.Split(',');
                }

                if (names == null)
                    throw new InvalidDataException("Could not read names.");
                if (names.Length != inputLenght + outputLenght)
                    throw new InvalidDataException("Wrong number of names in data definition, "
                        + names.Length + "instead of " + (inputLenght + outputLenght).ToString());
                if (titleSpecified)
                {
                    if (titles == null)
                        throw new InvalidDataException("Could not read titles.");
                    if (titles.Length != inputLenght + outputLenght)
                    {
                        throw new InvalidDataException("Wrong number of definitions in data titles.");
                        throw new InvalidDataException("Titles lenght " + titles.Length + " instead of " + (inputLenght + outputLenght).ToString());
                    }
                }              
                if (descriptionSpecified)
                {
                    if (descriptions == null)
                        throw new InvalidDataException("Could not read descriptions.");
                    if (descriptions.Length != inputLenght + outputLenght)
                    {
                        throw new InvalidDataException("Wrong number of definitions in data definition.");
                        throw new InvalidDataException("Description lenght " + descriptions.Length + " instead of " + (inputLenght + outputLenght).ToString());
                    }
                }

                // add definition data to NeuralDataDefinition (name, title and description)
                for (int i = 0; i < inputLenght; i++)
                {
                    string name = null, title = null, description = null;
                    if (names != null)
                        name = names[i];
                    if (titles != null)
                        title = descriptions[i];
                    if (descriptions != null)
                        description = descriptions[i];
                    definitionData.AddInputElement(new InputElementDefinition(i, name, title /* $$$$ should be title */, description));
                }
                for (int i = 0; i < outputLenght; i++)
                {
                    string name = null, title = null, description = null;
                    if (names != null)
                        name = names[i + inputLenght];
                    if (titles != null)
                        title = titles[i + inputLenght];
                    if (descriptions != null)
                        description = descriptions[i + inputLenght];

                    definitionData.AddOutputElement(new OutputElementDefinition(i, name, title , description));
                }
            }
            // read the training data
            while ((str = reader.ReadLine()) != null)
            {
                // split trainingData 
                string[] strs = str.Split(';');
                if (strs.Length == 1)
                    strs = str.Split(',');

                // allocate data array      
                variables = strs.Length;

                // parse training data     
                double[] tempLineData = new double[inputLenght + outputLenght];
                int j1;
                for (j1 = 0; j1 < variables; j1++)
                {
                    if (strs.Length != (inputLenght + outputLenght))
                    {
                        throw new ArgumentException("Training data out of range in line " + data.Count);
                    }
                    tempLineData[j1] = double.Parse(strs[j1]);
                }
                data.Add(tempLineData);
            }

            // add training data to NeuralTrainingSet
            if (trainingData == null)
                trainingData = new SampledDataSet();
            trainingData.Clear();
            trainingData.InputLength = inputLenght;
            trainingData.OutputLength = outputLenght;

            for (int i = 0; i < data.Count; ++i)
            {
                double[] row = data[i];
                Vector inputData = new Vector(inputLenght);
                for (int j = 0; j < inputLenght; ++j)
                {
                    inputData[j] = row[j];
                }
                Vector outputData = new Vector(outputLenght);
                for (int k = 0; k < outputLenght; k++)
                {
                    outputData[k] = row[k + inputLenght];
                }
                trainingData.AddElement(new SampledDataElement(inputData, outputData));
            }
        }

        /// <summary>Loads definition data from CSV file.</summary>
        /// <param name="inputFilePath">Path to the file where definition data are saved.</param>
        /// <param name="inputLenght">Lenght of input parameters.</param>
        /// <param name="outputLenght">Lenght of output parameters.</param>
        /// <param name="definitionData">Definition data set.</param>
        /// $A Tako78 Mar11; June24;
        public static void LoadDefinitionDataCSV(string filePath, int inputLenght, int outputLenght,
            ref InputOutputDataDefiniton definitionData)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentNullException("File path not specified.");

            if (definitionData == null)
                definitionData = new InputOutputDataDefiniton();

            // variables            
            string line = null; ;
            StreamReader reader = null;
            string[] lineIndex = null;

            string[] inputNames = new string[inputLenght];
            string[] inputTitles = new string[inputLenght];
            string[] inputDescriptions = new string[inputLenght];
            double[] defaultValues = new double[inputLenght];
            bool[] bounds = new bool[inputLenght];
            double[] minValues = new double[inputLenght];
            double[] maxValues = new double[inputLenght];
            string[] outputNames = new string[outputLenght];
            string[] outputTitles = new string[outputLenght];
            string[] outputDescriptions = new string[outputLenght];

            // open selected file
            reader = File.OpenText(filePath);

            // split 1st line of names
            line = reader.ReadLine();
            lineIndex = line.Split(';');
            if (lineIndex.Length == 1)
                lineIndex = line.Split(',');

            // check if input index is specified
            if ((lineIndex[0] == "Input") || (lineIndex[0] == "Input"))
            {
                //read input definition from CSV file
                int n = 0;
                do
                {
                    line = reader.ReadLine();
                    lineIndex = line.Split(';');
                    if (lineIndex.Length == 1)
                        lineIndex = line.Split(',');

                    if (lineIndex[0] == "name" || lineIndex[0] == "Name")
                    {
                        for (int i = 1; i < inputLenght + 1; i++)
                        {
                            if (lineIndex != null)
                                inputNames[i - 1] = lineIndex[i];
                        }
                        continue;
                    }
                    if (lineIndex[0] == "title" || lineIndex[0] == "Title")
                    {
                        for (int i = 1; i < inputLenght + 1; i++)
                        {
                            if (lineIndex != null)
                                inputTitles[i - 1] = lineIndex[i];
                        }
                        continue;
                    }
                    if (lineIndex[0] == "description" || lineIndex[0] == "Description")
                    {
                        for (int i = 1; i < inputLenght + 1; i++)
                        {
                            if (lineIndex != null)
                                inputDescriptions[i - 1] = lineIndex[i];
                        }
                        continue;
                    }
                    if (lineIndex[0] == "default" || lineIndex[0] == "Default")
                    {
                        for (int i = 1; i < inputLenght + 1; i++)
                        {
                            if (lineIndex != null)
                                defaultValues[i - 1] = double.Parse(lineIndex[i]);
                        }
                        continue;
                    }
                    if (lineIndex[0] == "bounds" || lineIndex[0] == "Bounds")
                    {
                        for (int i = 1; i < inputLenght + 1; i++)
                        {
                            if (lineIndex != null)
                            {
                                int tmp;
                                tmp = int.Parse(lineIndex[i]);
                                if (tmp == 1)
                                    bounds[i - 1] = true;
                                else if (tmp == 0)
                                    bounds[i - 1] = false;
                            }
                        }
                        continue;
                    }
                    if (lineIndex[0] == "min" || lineIndex[0] == "Min")
                    {
                        for (int i = 1; i < inputLenght + 1; i++)
                        {
                            if (lineIndex != null)
                                minValues[i - 1] = double.Parse(lineIndex[i]);
                        }
                        continue;
                    }
                    if (lineIndex[0] == "max" || lineIndex[0] == "Max")
                    {
                        for (int i = 1; i < inputLenght + 1; i++)
                        {
                            if (lineIndex != null)
                                maxValues[i - 1] = double.Parse(lineIndex[i]);
                        }
                        continue;
                    }
                    n++;
                }
                while ((lineIndex[0] != "Output") && (lineIndex[0] != "output") && (n < 5));
            }
            else
                throw new InvalidDataException("Input index not specified.");

            // check if output index is specified
            if ((lineIndex[0] == "Output") || (lineIndex[0] == "output"))
            {
                //read output definition from CSV file
                do
                {
                    //line = reader.ReadLine();
                    lineIndex = line.Split(';');
                    if (lineIndex.Length == 1)
                        lineIndex = line.Split(',');

                    if (lineIndex[0] == "name" || lineIndex[0] == "Name")
                    {
                        for (int i = 1; i < outputLenght + 1; i++)
                        {
                            if (lineIndex != null)
                                outputNames[i - 1] = lineIndex[i];
                        }
                    }
                    if (lineIndex[0] == "title" || lineIndex[0] == "Title")
                    {
                        for (int i = 1; i < outputLenght + 1; i++)
                        {
                            if (lineIndex != null)
                                outputTitles[i - 1] = lineIndex[i];
                        }
                    }
                    if (lineIndex[0] == "description" || lineIndex[0] == "Description")
                    {
                        for (int i = 1; i < outputLenght + 1; i++)
                        {
                            if (lineIndex != null)
                                outputDescriptions[i - 1] = lineIndex[i];
                        }
                    }
                }
                while ((line = reader.ReadLine()) != null);
            }
            else
                throw new InvalidDataException("Output index not specified.");

            // write input definition
            for (int i = 0; i < inputLenght; i++)
            {
                string inputName = null;
                string inputTitle = null;
                string inputDescription = null;
                double defaultValue = 0.0;
                bool bound = false;
                double minValue = 0.0;
                double maxValue = 0.0;

                inputName = inputNames[i];
                inputTitle = inputTitles[i];
                inputDescription = inputDescriptions[i];
                defaultValue = defaultValues[i];
                bound = bounds[i];
                minValue = minValues[i];
                maxValue = maxValues[i];

                InputElementDefinition def = new InputElementDefinition(inputName, inputTitle, inputDescription);
                def.DefaultValue = defaultValue;
                def.BoundsDefined = bound;
                def.MinimalValue = minValue;
                def.MaximalValue = maxValue;
                definitionData.AddInputElement(def);
            }

            // write output definition
            for (int i = 0; i < outputLenght; i++)
            {
                string outputName = null;
                string outputTitle = null;
                string outputDescription = null;

                outputName = outputNames[i];
                outputTitle = outputTitles[i];
                outputDescription = outputDescriptions[i];

                definitionData.AddOutputElement(new OutputElementDefinition(outputName, outputTitle , outputDescription));
            }
        }

        /// <summary>Saves training data and Definition data to single CSV file.</summary>
        /// <param name="inputFilePath">Path to the file where training data will be saved.</param>
        /// <param name="trainingData">Training data set.</param>
        /// <param name="namesSpecified">Flag if names will be written in the file.</param>
        /// <param name="descriptionSpecified">Flag if descriptions (descriptions, defaultValue, boundDefiner, minValue, maxValue) will be written in the file.</param>
        /// <param name="definitionData">Definition data set.</param>
        /// $A Tako78 Mar11; June27;
        public static void SaveTrainingDataCSVinOneLine(string filePath, SampledDataSet trainingData,
            bool namesSpecified, bool titleSpecified, bool descriptionSpecified, InputOutputDataDefiniton definitionData)
        {
            int inputVariables;
            int outputVariables;
            int numSamples;
            inputVariables = trainingData.InputLength;
            outputVariables = trainingData.OutputLength;
            numSamples = trainingData.Length;
            // data names not sbecified, descriptions are specified
            if (!namesSpecified && descriptionSpecified)
                throw new ArgumentException("Data names not specified, but descriptions are.");

            using (StreamWriter writer = new StreamWriter(filePath, false))
            {
                // data names not specified
                if (!namesSpecified)
                    definitionData = null;
                else
                {
                    string[] name = new string[inputVariables + outputVariables];
                    string[] title = new string[inputVariables + outputVariables];
                    string[] description = new string[inputVariables + outputVariables];
                    double[] defaultValue = new double[inputVariables];
                    bool[] boundsDefined = new bool[inputVariables];
                    double[] minimalValue = new double[inputVariables];
                    double[] maximalValue = new double[inputVariables];
                    // write data names
                    for (int i = 0; i < inputVariables; i++)
                    {
                        name[i] = definitionData.GetInputElement(i).Name;
                        writer.Write(name[i] + ";");
                    }
                    for (int j = 0; j < outputVariables; j++)
                    {
                        name[j] = definitionData.GetOutputElement(j).Name;
                        writer.Write(name[j]);
                        if (j < outputVariables - 1)
                            writer.Write(";");
                    }
                    writer.WriteLine();
                    if (titleSpecified)
                    {
                        // write titles
                        for (int i = 0; i < inputVariables; i++)
                        {
                            title[i] = definitionData.GetInputElement(i).Title;
                            writer.Write(title[i] + ";");
                        }
                        for (int j = 0; j < outputVariables; j++)
                        {
                            title[j] = definitionData.GetOutputElement(j).Title;
                            writer.Write(title[j]);
                            if (j < outputVariables - 1)
                                writer.Write(";");
                        }
                        writer.WriteLine();
                    }
                    // data descriptions specified
                    if (descriptionSpecified)
                    {
                        // write descriptions data 
                        for (int i = 0; i < inputVariables; i++)
                        {
                            description[i] = definitionData.GetInputElement(i).Description;
                            writer.Write(description[i] + ";");
                        }
                        for (int j = 0; j < outputVariables; j++)
                        {
                            description[j] = definitionData.GetOutputElement(j).Description;
                            writer.Write(description[j]);
                            if (j < outputVariables - 1)
                                writer.Write(";");
                        }
                        writer.WriteLine();
                        for (int i = 0; i < inputVariables; i++)
                        {
                            defaultValue[i] = definitionData.GetInputElement(i).DefaultValue;
                            writer.Write(Convert.ToString(defaultValue[i]));
                            if (i < inputVariables - 1)
                                writer.Write(";");
                        }
                        writer.WriteLine();
                        for (int i = 0; i < inputVariables; i++)
                        {
                            boundsDefined[i] = definitionData.GetInputElement(i).BoundsDefined;
                            writer.Write(Convert.ToInt16(boundsDefined[i]));
                            if (i < inputVariables - 1)
                                writer.Write(";");
                        }
                        writer.WriteLine();
                        for (int i = 0; i < inputVariables; i++)
                        {
                            minimalValue[i] = definitionData.GetInputElement(i).MinimalValue;
                            writer.Write(Convert.ToString(minimalValue[i]));
                            if (i < inputVariables - 1)
                                writer.Write(";");
                        }
                        writer.WriteLine();
                        for (int i = 0; i < inputVariables; i++)
                        {
                            maximalValue[i] = definitionData.GetInputElement(i).MaximalValue;
                            writer.Write(Convert.ToString(maximalValue[i]));
                            if (i < inputVariables - 1)
                                writer.Write(";");
                        }
                        writer.WriteLine();
                    }
                }
                // write data
                for (int i = 0; i < numSamples; i++)
                {
                    IVector inputData = trainingData.GetInputParameters(i);

                    for (int j = 0; j < inputVariables; j++)
                    {
                        Console.Write(inputData[j] + ";");
                        writer.Write(inputData[j] + ";");
                    }
                    IVector outputData = trainingData.GetOutputValues(i);
                    for (int k = 0; k < outputVariables; k++)
                    {
                        Console.Write(outputData[k]);
                        writer.Write(outputData[k]);
                        if (k < outputVariables - 1)
                        {
                            Console.Write(";");
                            writer.Write(";");
                        }
                    }
                    Console.WriteLine();
                    writer.WriteLine();
                }
                writer.Close();
            }
        }

        /// <summary>Saves training data and Definition data to single CSV file.</summary>
        /// <param name="inputFilePath">Path to the file where training data will be saved.</param>
        /// <param name="trainingData">Training data set.</param>
        /// <param name="namesSpecified">Flag if names will be written in the file.</param>
        /// <param name="descriptionSpecified">Flag if descriptions will be written in the file.</param>
        /// <param name="definitionData">Definition data set.</param>
        /// $A Tako78 Mar11; June27;
        public static void SaveTrainingDataCSV(string filePath, SampledDataSet trainingData,
            bool namesSpecified, bool titlesSpecified, bool descriptionSpecified, InputOutputDataDefiniton definitionData)
        {
            if (trainingData == null)
                throw new ArgumentNullException("No data in training block.");

            int inputVariables;
            int outputVariables;
            int numSamples;
            inputVariables = trainingData.InputLength;
            outputVariables = trainingData.OutputLength;
            numSamples = trainingData.Length;
            // data names not sbecified, descriptions are specified
            if ((!namesSpecified && descriptionSpecified) || (!namesSpecified && titlesSpecified))
                throw new ArgumentException("Data names not specified, but descriptions or titles are.");

            using (StreamWriter writer = new StreamWriter(filePath, false))
            {
                // data names not specified
                if (!namesSpecified)
                    definitionData = null;
                else
                {
                    string[] name = new string[inputVariables + outputVariables];
                    string[] title = new string[inputVariables + outputVariables];
                    string[] description = new string[inputVariables + outputVariables];
                    // write data names
                    for (int i = 0; i < inputVariables; i++)
                    {
                        name[i] = definitionData.GetInputElement(i).Name;
                        writer.Write(name[i] + ";");
                    }
                    for (int j = 0; j < outputVariables; j++)
                    {
                        name[j] = definitionData.GetOutputElement(j).Name;
                        writer.Write(name[j]);
                        if (j < outputVariables - 1)
                            writer.Write(";");
                    }
                    writer.WriteLine();
                    if (titlesSpecified)
                    {
                        // write data titles
                        for (int i = 0; i < inputVariables; i++)
                        {
                            title[i] = definitionData.GetInputElement(i).Title;
                            writer.Write(title[i] + ";");
                        }
                        for (int j = 0; j < outputVariables; j++)
                        {
                            title[j] = definitionData.GetOutputElement(j).Title;
                            writer.Write(title[j]);
                            if (j < outputVariables - 1)
                                writer.Write(";");
                        }
                    }
                    // data descriptions specified
                    if (descriptionSpecified)
                    {
                        // write descriptions data 
                        for (int i = 0; i < inputVariables; i++)
                        {
                            description[i] = definitionData.GetInputElement(i).Description;
                            writer.Write(description[i] + ";");
                        }
                        for (int j = 0; j < outputVariables; j++)
                        {
                            description[j] = definitionData.GetOutputElement(j).Description;
                            writer.Write(description[j]);
                            if (j < outputVariables - 1)
                                writer.Write(";");
                        }
                        writer.WriteLine();
                    }
                }
                // write data
                for (int i = 0; i < numSamples; i++)
                {
                    IVector inputData = trainingData.GetInputParameters(i);
                    for (int j = 0; j < inputVariables; j++)
                    {
                        //Console.Write(inputData[j] + ";");
                        writer.Write(inputData[j] + ";");
                    }
                    IVector outputData = trainingData.GetOutputValues(i);
                    for (int k = 0; k < outputVariables; k++)
                    {
                        //Console.Write(outputData[k]);
                        writer.Write(outputData[k]);
                        if (k < outputVariables - 1)
                        {
                            //Console.Write(";");
                            writer.Write(";");
                        }
                    }
                    //Console.WriteLine();
                    writer.WriteLine();
                }
                writer.Close();
            }
        }

        /// <summary>Saves definition data to CSV file.</summary>
        /// <param name="inputFilePath">Path to the file where definition data will be saved.</param>
        /// <param name="definitionData">Definition data set.</param>
        /// $A Tako78 Mar11; June27;
        public static void SaveDefinitionDataCSV(string filePath, InputOutputDataDefiniton definitionData)
        {
            int inputLenght;
            int outputLenght;

            inputLenght = definitionData.InputLength;
            outputLenght = definitionData.OutputLength;

            string[] inputNames = new string[inputLenght];
            string[] inputTitles = new string[inputLenght];
            string[] inputDescriptions = new string[inputLenght];
            double[] defaultValues = new double[inputLenght];
            bool[] bounds = new bool[inputLenght];
            double[] minValues = new double[inputLenght];
            double[] maxValues = new double[inputLenght];
            string[] outputNames = new string[outputLenght];
            string[] outputTitles = new string[outputLenght];
            string[] outputDescriptions = new string[outputLenght];

            using (StreamWriter writer = new StreamWriter(filePath, false))
            {
                if (definitionData == null)
                    throw new ArgumentNullException("No data in definition block.");
                else
                {
                    Console.WriteLine("Input");
                    writer.Write("Input");
                    writer.WriteLine();
                    // write data input names
                    Console.Write("Name;");
                    writer.Write("Name;");
                    for (int i = 0; i < inputLenght; i++)
                    {
                        inputNames[i] = definitionData.GetInputElement(i).Name;
                        Console.Write(inputNames[i]);
                        writer.Write(inputNames[i]);
                        if (i < inputLenght - 1)
                        {
                            Console.Write(";");
                            writer.Write(";");
                        }
                    }
                    Console.WriteLine();
                    writer.WriteLine();
                    // write data input Titles
                    Console.Write("Title;");
                    writer.Write("Title;");
                    for (int i = 0; i < inputLenght; i++)
                    {
                        inputTitles[i] = definitionData.GetInputElement(i).Title;
                        Console.Write(inputNames[i]);
                        writer.Write(inputNames[i]);
                        if (i < inputLenght - 1)
                        {
                            Console.Write(";");
                            writer.Write(";");
                        }
                    }
                    Console.WriteLine();
                    writer.WriteLine();
                    // write data input descriptions
                    Console.Write("Description;");
                    writer.Write("Description;");
                    for (int i = 0; i < inputLenght; i++)
                    {
                        inputDescriptions[i] = definitionData.GetInputElement(i).Description;
                        Console.Write(inputDescriptions[i]);
                        writer.Write(inputDescriptions[i]);
                        if (i < inputLenght - 1)
                        {
                            Console.Write(";");
                            writer.Write(";");
                        }
                    }
                    Console.WriteLine();
                    writer.WriteLine();
                    // write data default value
                    Console.Write("Default;");
                    writer.Write("Default;");
                    for (int i = 0; i < inputLenght; i++)
                    {
                        defaultValues[i] = definitionData.GetInputElement(i).DefaultValue;
                        Console.Write(defaultValues[i]);
                        writer.Write(defaultValues[i]);
                        if (i < inputLenght - 1)
                        {
                            Console.Write(";");
                            writer.Write(";");
                        }
                    }
                    Console.WriteLine();
                    writer.WriteLine();
                    // write data bound
                    Console.Write("Bounds;");
                    writer.Write("Bounds;");
                    for (int i = 0; i < inputLenght; i++)
                    {
                        bounds[i] = definitionData.GetInputElement(i).BoundsDefined;
                        if (i < inputLenght - 1)
                        {
                            if (bounds[i] == true)
                            {
                                Console.Write("1");
                                writer.Write("1");
                            }
                            else if (bounds[i] == false)
                            {
                                Console.Write("0");
                                writer.Write("0");
                            }
                            Console.Write(";");
                            writer.Write(";");
                        }
                    }
                    Console.WriteLine();
                    writer.WriteLine();
                    // write data min value
                    Console.Write("Min;");
                    writer.Write("Min;");
                    for (int i = 0; i < inputLenght; i++)
                    {
                        minValues[i] = definitionData.GetInputElement(i).MinimalValue;
                        Console.Write(minValues[i]);
                        writer.Write(minValues[i]);
                        if (i < inputLenght - 1)
                        {
                            Console.Write(";");
                            writer.Write(";");
                        }
                    }
                    Console.WriteLine();
                    writer.WriteLine();
                    // write data max value
                    Console.Write("Max;");
                    writer.Write("Max;");
                    for (int i = 0; i < inputLenght; i++)
                    {
                        maxValues[i] = definitionData.GetInputElement(i).MaximalValue;
                        Console.Write(maxValues[i]);
                        writer.Write(maxValues[i]);
                        if (i < inputLenght - 1)
                        {
                            Console.Write(";");
                            writer.Write(";");
                        }
                    }
                    Console.WriteLine();
                    writer.WriteLine();
                    Console.WriteLine("Output");
                    writer.Write("Output");
                    writer.WriteLine();
                    // write data output name
                    Console.Write("Name;");
                    writer.Write("Name;");
                    for (int i = 0; i < outputLenght; i++)
                    {
                        outputNames[i] = definitionData.GetInputElement(i).Name;
                        Console.Write(outputNames[i]);
                        writer.Write(outputNames[i]);
                        if (i < outputLenght - 1)
                        {
                            Console.Write(";");
                            writer.Write(";");
                        }
                    }
                    Console.WriteLine();
                    writer.WriteLine();
                    // write data output titles
                    Console.Write("Title;");
                    writer.Write("Title;");
                    for (int i = 0; i < outputLenght; i++)
                    {
                        outputTitles[i] = definitionData.GetInputElement(i).Title;
                        Console.Write(outputTitles[i]);
                        writer.Write(outputTitles[i]);
                        if (i < outputLenght - 1)
                        {
                            Console.Write(";");
                            writer.Write(";");
                        }
                    }
                    Console.WriteLine();
                    writer.WriteLine();
                    // write data output description
                    Console.Write("Description;");
                    writer.Write("Description;");
                    for (int i = 0; i < outputLenght; i++)
                    {
                        outputDescriptions[i] = definitionData.GetInputElement(i).Description;
                        Console.Write(outputDescriptions[i]);
                        writer.Write(outputDescriptions[i]);
                        if (i < outputLenght - 1)
                        {
                            Console.Write(";");
                            writer.Write(";");
                        }
                    }
                    Console.WriteLine();
                }
                writer.Close();
            }
        }

        /// <summary>Loads training data and Definition data from multible CSV files.
        /// Training data consist of one output and multiple input parameters.
        /// Input parameters are the same in all files, output parameter are different.</summary>
        /// <param name="result">Training data set with combined outputs.</param>
        /// <param name="individualSets">Different training data sets with the same inputs and different outputs.</param>
        /// $A Tako78 Mar11;
        public static void SampledDataCombineOutputs(ref SampledDataSet result, params SampledDataSet[] individualSets)
        {
            if (individualSets == null)
                throw new ArgumentNullException("Training sets to be combined are not specified.");
            if (individualSets.Length < 1)
                throw new ArgumentException("No training sets to combine (array length is 0).");

            int numSets = individualSets.Length;
            SampledDataSet firstSet = individualSets[0];
            if (firstSet == null)
                throw new ArgumentException("Training set No. 0 not specified (null reference).");
            int numElements = firstSet.Length;
            int inputLength = firstSet.InputLength;
            int outputLength = firstSet.OutputLength; ;
            for (int i = 0; i < numSets; ++i)
            {
                SampledDataSet currentSet = individualSets[i];
                if (currentSet.Length != numElements)
                    throw new ArgumentException("Inconsistent training set No. " + i + ": inconsistent number of elements.");
            }
            if (result == null)
                result = new SampledDataSet();

            SampledDataSet tmpTrainingData = new SampledDataSet();
            IVector inputData = null;
            IVector tmpOutputData = null;
            IVector outputData = new Vector(numSets);

            //Read input training data from first file         
            tmpTrainingData = individualSets[0];
            //Read output training data from all files
            for (int i = 0; i < tmpTrainingData.Length; ++i)
            {
                inputData = tmpTrainingData.GetInputParameters(i);
                for (int j = 0; j < numSets; j++)
                {
                    Vector tmp = new Vector(numSets);
                    tmpTrainingData = individualSets[j];
                    tmpOutputData = tmpTrainingData.GetOutputValues(i);
                    tmp[j] = tmpOutputData[0];
                    outputData[j] = tmp[j];
                }
                Console.WriteLine("Input");
                Console.WriteLine(inputData);
                Console.WriteLine("Output");
                Console.WriteLine(outputData);
                result.AddElement(new SampledDataElement(inputData, outputData));
            }
        }


        #endregion

        #region JSON


        /// <summary>Loads training data and Definition data from multible CSV files.
        /// Training data consist of one output and multiple input parameters.
        /// Input parameters are the same in all files, output parameter are different.</summary>
        /// <param name="trainingData">Training data set.</param>
        /// <param name="directoryPath">Path to the file where training data are saved.</param>
        /// <param name="fileNames">Name of the files where training data are saved.</param>
        /// $A Tako78 Mar11;
        public static void LoadTrainingDataCombinedOutputsJSON(ref SampledDataSet trainingData, string directoryPath, params string[] fileNames)
        {
            if (string.IsNullOrEmpty(directoryPath))
                throw new ArgumentNullException("File path not specified.");
            if (fileNames == null)
                throw new ArgumentNullException("File names not specified.");
            if (fileNames.Length < 1)
                throw new ArgumentException("No file names (array length 0).");
            int numFiles = fileNames.Length;
            string[] paths = new string[numFiles];
            for (int i = 0; i < numFiles; ++i)
            {
                paths[i] = Path.Combine(directoryPath, fileNames[i]);
            }
            LoadTrainingDataCombinedOutputsJSON(ref trainingData, paths);
        }

        /// <summary>Loads training data and Definition data from multible CSV files.
        /// Training data consist of one output and multiple input parameters.
        /// Input parameters are the same in all files but output parameter should be different.</summary>
        /// <param name="fileNames">Path to the file where training data are saved.</param>
        /// <param name="trainingData">Training data set.</param>
        /// $A Tako78 Mar11;
        public static void LoadTrainingDataCombinedOutputsJSON(ref SampledDataSet trainingData, params string[] fileNames)
        {
            if (string.IsNullOrEmpty(fileNames[0]))
                throw new ArgumentNullException("File names not specified.");
            if (trainingData == null)
                trainingData = new SampledDataSet();
            int filesNum = fileNames.Length;
            SampledDataSet[] inputSets = new SampledDataSet[filesNum];
            for (int i = 0; i < filesNum; ++i)
            {
                NeuralTadej.LoadSampledDataJson(fileNames[i], ref  inputSets[i]);
            }
            NeuralTadej.SampledDataCombineOutputs(ref trainingData, inputSets);
        }

        /// <summary>Saves network's training data to the specified JSON file.
        /// File is owerwritten if it exists.</summary>
        /// <param name="inputFilePath">Path to the file where training data is saved.</param>
        /// $A Tako78 Mar11;
        public static void SaveTrainingDataJson(string filePath, SampledDataSet trainingData)
        {
            {
                SampledDataSetDto dtoOriginal = new SampledDataSetDto();
                dtoOriginal.CopyFrom(trainingData);
                ISerializer serializer = new SerializerJson();
                serializer.Serialize<SampledDataSetDto>(dtoOriginal, filePath);
            }
        }
        
        /// <summary>Saves network's definition data to the specified JSON file.
        /// File is owerwritten if it exists.</summary>
        /// <param name="inputFilePath">Path to the file where definition data is saved.</param>
        /// $A Tako78 Maj31;
        public static void SaveDefinitionDataJson(string filePath, InputOutputDataDefiniton trainingData)
        {
            {
                InputOutputDataDefinitonDto dtoOriginal = new InputOutputDataDefinitonDto();
                dtoOriginal.CopyFrom(trainingData);
                ISerializer serializer = new SerializerJson();
                serializer.Serialize<InputOutputDataDefinitonDto>(dtoOriginal, filePath);
            }
        }
        
        /// <summary>Restores training data from the specified file in JSON format.</summary>
        /// <param name="inputFilePath">File from which training data is restored.</param>
        /// $A Tako78 Mar11;
        public static void LoadSampledDataJson(string filePath, ref SampledDataSet trainingData)
        {
            {
                ISerializer serializer = new SerializerJson();
                SampledDataSetDto dtoRestored = serializer.DeserializeFile<SampledDataSetDto>(filePath);
                //NeuralTrainingSet dataRestored = trainingData;
                dtoRestored.CopyTo(ref trainingData);
                //trainingData = dataRestored;
            }
        }

        /// <summary>Restores definition data from the specified file in JSON format.</summary>
        /// <param name="inputFilePath">File from which definition data is restored.</param>
        /// $A Tako78 Nov11;
        public static void LoadDefinitionDataJson(string filePath, ref InputOutputDataDefiniton definitionData)
        {
            {
                ISerializer serializer = new SerializerJson();
                InputOutputDataDefinitonDto dtoRestored = serializer.DeserializeFile<InputOutputDataDefinitonDto>(filePath);
                dtoRestored.CopyTo(ref definitionData);
            }
        }

        #endregion

        #region Filters

        /// <summary>Returns the standard deviation.</summary>
        /// <param name="trainingData">List of dataset.</param>
        /// $A Tako78 Apr11;
        public static double getStandardDeviation(List<double> doubleList)
        {
            double average = doubleList.Average();
            double sumOfDerivation = 0;
            foreach (double value in doubleList)
            {
                sumOfDerivation += (value) * (value);
            }
            double sumOfDerivationAverage = sumOfDerivation / doubleList.Count;
            return Math.Sqrt(sumOfDerivationAverage - (average * average));
        }
        
        /// <summary>Check the training data set and delete unconsistant datas.</summary>
        /// <param name="trainingData">Training data set.</param>
        /// <param name="smoothTrainingData">New training data set after.</param>
        /// <param name="numStandardDeviation">Number of standard deviation.</param>
        /// $A Tako78 Apr11;
        public static void SmoothingTrainingData(SampledDataSet trainingData, ref SampledDataSet smoothTrainingData, double numStandardDeviation, bool uniqueInput, bool uniqueOutput, bool zeroData)
        {
            if (trainingData == null)
                throw new ArgumentNullException("No data in training block.");

            smoothTrainingData = new SampledDataSet();
            int inputVariables;
            int outputVariables;
            int numSamples;
            inputVariables = trainingData.InputLength;
            outputVariables = trainingData.OutputLength;
            numSamples = trainingData.Length;
            List<double[]> data = new List<double[]>();
            List<int> lineToDelete = new List<int>();
            List<int> columnToDelete = new List<int>();

            //List of Input and output parameters
            for (int i = 0; i < inputVariables; i++)
            {
                double[] tmpInputData = new double[numSamples];
                for (int j = 0; j < numSamples; j++)
                {
                    IVector inputData = trainingData.GetInputParameters(j);
                    tmpInputData[j] = inputData[i];
                }
                data.Add(tmpInputData);
            }
            for (int i = 0; i < outputVariables; i++)
            {
                double[] tmpOutputData = new double[numSamples];
                for (int j = 0; j < numSamples; j++)
                {
                    IVector outputData = trainingData.GetOutputValues(j);
                    tmpOutputData[j] = outputData[i];
                }
                data.Add(tmpOutputData);
            }

            // Input parameters are the same
            if (uniqueInput == true)
            {
                for (int i = 0; i < trainingData.Length; i++)
                {
                    IVector compareInputParam = trainingData.GetInputParameters(i);
                    for (int j = i + 1; j < trainingData.Length; j++)
                    {
                        if (IVector.Equals(compareInputParam, trainingData.GetInputParameters(j)))
                        {
                            if (!lineToDelete.Contains(j))
                                lineToDelete.Add(j);
                        }
                    }
                }
            }

            //Output values are the same
            if (uniqueOutput == true)
            {
                for (int i = 0; i < inputVariables + outputVariables; i++)
                {
                    double[] tmpCompareData = new double[numSamples];
                    tmpCompareData = data[i];
                    for (int j = 0; j < numSamples; j++)
                    {
                        if (i + 1 > inputVariables)
                        {
                            double tmpOutputValue1 = 0.0;
                            tmpOutputValue1 = tmpCompareData[j];

                            for (int k = 0; k < numSamples; k++)
                            {
                                if (k > j)
                                {
                                    double tmpOutputValue2 = 0.0;
                                    tmpOutputValue2 = tmpCompareData[k];
                                    if (tmpOutputValue1 == tmpOutputValue2)
                                    {
                                        if (!lineToDelete.Contains(k))
                                            lineToDelete.Add(k);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            // Datas in one column are the same
            if (zeroData == true)
            {
                for (int i = 0; i < inputVariables + outputVariables; i++)
                {
                    if (data[i].Max() == data[i].Min())
                        if (!columnToDelete.Contains(i))
                            columnToDelete.Add(i);
                }
            }

            // Standard Deviation
            if (numStandardDeviation > 0)
            {
                //Calculate standard deviation for all parameters
                List<double> standardDeviation = new List<double>();
                for (int i = 0; i < data.Count; i++)
                {
                    List<double> tmpData = new List<double>();
                    tmpData.AddRange(data[i]);
                    standardDeviation.Add(getStandardDeviation(tmpData));
                }
                //Compare data with standard deviation limit
                for (int i = 0; i < inputVariables + outputVariables; i++)
                {

                    double[] tmpCompareData = new double[numSamples];
                    tmpCompareData = data[i];
                    double average = data[i].Average();
                    double minBound;
                    double maxBound;
                    minBound = average - (standardDeviation[i] * numStandardDeviation);
                    maxBound = average + (standardDeviation[i] * numStandardDeviation);
                    //Datas are over or under limit
                    for (int j = 0; j < numSamples; j++)
                    {
                        if ((tmpCompareData[j] > maxBound) || (tmpCompareData[j] < minBound))
                        {
                            if (!lineToDelete.Contains(j))
                                lineToDelete.Add(j);
                        }
                    }
                }
            }

            lineToDelete.Sort();
            columnToDelete.Sort();
            // Delete lines and columns in training data
            //List<NeuralTrainingElement> listData = trainingData.GetElementList();
            List<SampledDataElement> listData = new List<SampledDataElement>();
            listData = trainingData.GetElementList();
            // Delete lines
            if (lineToDelete.Count != 0)
            {
                for (int i = lineToDelete.Count; i > 0; i--)
                {
                    int line;
                    line = lineToDelete[i - 1];
                    listData.RemoveAt(line);
                }
            }

            //Delete columns
            int deletedInputElement = 0;
            int deletedOutputElement = 0;
            for (int p = 0; p < columnToDelete.Count; p++)
            {
                if ((columnToDelete[p] > 0) && (columnToDelete[p] < inputVariables))
                    deletedInputElement++;
                else
                    deletedOutputElement++;
            }
            for (int i = 0; i < listData.Count; i++)
            {
                SampledDataElement ElList = listData[i];
                IVector inputEl = ElList.InputParameters;
                IVector tmpInputEl = new Vector(inputVariables - deletedInputElement);
                IVector outputEl = ElList.OutputValues;
                IVector tmpOutputEl = new Vector(outputVariables - deletedOutputElement);

                int inputElIndex = 0;
                int outputElIndex = 0;
                for (int j = 0; j < inputVariables; j++)
                {
                    bool deleteFlag = false;
                    for (int k = 0; k < columnToDelete.Count; k++)
                    {
                        int inputColumn;
                        inputColumn = columnToDelete[k];
                        if (j == inputColumn)
                        {
                            deleteFlag = true;
                        }
                    }
                    if (deleteFlag == false)
                    {
                        tmpInputEl[inputElIndex] = inputEl[j];
                        inputElIndex++;
                    }
                }
                for (int l = 0; l < outputVariables; l++)
                {
                    bool deleteFlag = false;
                    for (int k = 0; k < columnToDelete.Count; k++)
                    {
                        int outputColumn;
                        outputColumn = columnToDelete[k];
                        if (inputVariables + l == outputColumn)
                        {
                            deleteFlag = true;
                        }
                    }
                    if (deleteFlag == false)
                    {
                        tmpOutputEl[outputElIndex] = outputEl[l];
                        outputElIndex++;
                    }
                }
                smoothTrainingData.AddElement(new SampledDataElement(tmpInputEl, tmpOutputEl));
            }
        }


        #endregion

        #region Training Neural Netwotk


        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputLength"></param>
        /// <param name="outputLength"></param>
        /// <param name="trainingData"></param>
        /// <returns></returns>
        /// $A Tako78 Mar11;
        public static INeuralApproximator ExampleCasting(int inputLength, int outputLength, ref SampledDataSet trainingData)
        {
            // Specify reasonable number of samples and verification points:
            int numTrainingElements = trainingData.Length;

            //int numTrainingElements = 50;
            int numVerificationPoints = (int)Math.Round((double)numTrainingElements * 0.01);
            // numTrainingElements += numVerificationPoints;

            // Create training data by randomly sampling a specific quadratic response:
            //NeuralTrainingSet samples = NeuralTrainingSet.CreateExampleQuadratic(inputLength, outputLength, numTrainingElements);
            SampledDataSet sarze = trainingData;

            // Speciy which samples will be used for verification of approximation:
            IndexList verificationIndices = IndexList.CreateRandom(numVerificationPoints, 0 /* lowerbound */, numTrainingElements - 1);

            // Create neural network and set basic parameters:
            INeuralApproximator neural = new NeuralApproximatorAforge();

            neural.OutputLevel = 2;
            neural.InputLength = inputLength;
            neural.OutputLength = outputLength;

            // Set prepared data:
            neural.TrainingData = sarze;
            neural.VerificationIndices = verificationIndices;

            // Set network layout:
            neural.MultipleNetworks = true;
            neural.SetHiddenLayers(40, 20);

            // Set training parameters:
            neural.MaxEpochs = 200000;
            neural.EpochsInBundle = 400;
            neural.ToleranceRms = new Vector(outputLength, 0.0000000000000000001);

            // Specify learning parameters:
            neural.LearningRate = 0.15;
            neural.SigmoidAlphaValue = 2;
            neural.Momentum = 0.6;

            // Specify parameters defining the bounds for data applied to input and output neurons:
            neural.InputBoundsSafetyFactor = 1.3;
            neural.OutputBoundsSafetyFactor = 1.3;

            // Change the targeted range of input and otput neuraons: 
            neural.InputNeuronsRange.Reset();
            neural.InputNeuronsRange.UpdateAll(-2.0, 2.0);
            neural.OutputNeuronsRange.Reset();
            neural.OutputNeuronsRange.UpdateAll(0.0, 1.0);

            // Test output (for debugging purposes):
            Console.WriteLine();
            Console.WriteLine("Neural network data: ");
            Console.WriteLine(neural.ToString());
            Console.WriteLine("Insert <Enter> in order to continue: ");
            //Console.ReadLine();

            // Perform training: 
            neural.TrainNetwork();

            // Calculate outputs and exact values for first 5 points in the training set:
            Console.WriteLine("A couple of calculated outputs from the training set (including verification points):");
            for (int i = 0; i < 5; ++i)
            {
                IVector input = sarze.GetInputParameters(i);
                IVector exactOutput = sarze.GetOutputValues(i);
                IVector calculatedOutput = new Vector(outputLength);
                neural.CalculateOutput(input, ref calculatedOutput);
                Console.WriteLine();
                Console.WriteLine("Point No. " + i + "of the training set, is verification point: " + verificationIndices.Contains(i));
                Console.WriteLine("  Input parameters No. " + i + ":");
                Console.WriteLine("  " + input.ToString());
                Console.WriteLine("  Exact output: ");
                Console.WriteLine("  " + exactOutput.ToString());
                Console.WriteLine("  Approximated output: ");
                Console.WriteLine("  " + calculatedOutput.ToString());
                IVector dif = null;
                Vector.Subtract(calculatedOutput, exactOutput, ref dif);
                Console.WriteLine("Norm of the difference between sampled and calculated output: " + dif.Norm);
            }
            return neural;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputLength"></param>
        /// <param name="outputLength"></param>
        /// <param name="trainingData"></param>
        /// <returns></returns>
        /// $A Tako78 Mar11;
        public static INeuralApproximator ExampleStore(int inputLength, int outputLength, ref SampledDataSet trainingData)
        {
            // Specify reasonable number of samples and verification points:
            int numTrainingElements = trainingData.Length;

            int numVerificationPoints = (int)Math.Round((double)numTrainingElements * 0.13);
            // numTrainingElements += numVerificationPoints;

            // Create training data by randomly sampling a specific quadratic response:
            //NeuralTrainingSet samples = NeuralTrainingSet.CreateExampleQuadratic(inputLength, outputLength, numTrainingElements);
            SampledDataSet sarze = trainingData;

            // Speciy which samples will be used for verification of approximation:
            IndexList verificationIndices = IndexList.CreateRandom(numVerificationPoints, 0 /* lowerbound */, numTrainingElements - 1);

            // Create neural network and set basic parameters:
            NeuralApproximatorBase neural = new NeuralApproximatorAforge();

            neural.OutputLevel = 2;
            neural.InputLength = inputLength;
            neural.OutputLength = outputLength;

            // Set prepared data:
            neural.TrainingData = sarze;
            neural.VerificationIndices = verificationIndices;

            // Set network layout:
            neural.MultipleNetworks = true;
            neural.SetHiddenLayers(65);

            // Set training parameters:
            neural.MaxEpochs = 150000;
            neural.EpochsInBundle = 400;
            neural.ToleranceRms = new Vector(outputLength, 0.001);

            // Specify learning parameters:
            neural.LearningRate = 0.04;
            neural.SigmoidAlphaValue = 2;
            neural.Momentum = 0.6;

            // Specify parameters defining the bounds for data applied to input and output neurons:
            neural.InputBoundsSafetyFactor = 1.3;
            neural.OutputBoundsSafetyFactor = 1.3;

            // Change the targeted range of input and otput neuraons: 
            neural.InputNeuronsRange.Reset();
            neural.InputNeuronsRange.UpdateAll(-2.0, 2.0);
            neural.OutputNeuronsRange.Reset();
            neural.OutputNeuronsRange.UpdateAll(-1.0, 1.0);

            // Test output (for debugging purposes):
            Console.WriteLine();
            Console.WriteLine("Neural network data: ");
            Console.WriteLine(neural.ToString());
            Console.WriteLine("Insert <Enter> in order to continue: ");
            //Console.ReadLine();
            
            // Test output (for debugging purposes):
            if (outputLength > 0)
            {
                Console.WriteLine();
                Console.WriteLine("Neural network data: ");
                Console.WriteLine(neural.ToString());
                //Console.WriteLine("Insert <Enter> in order to continue: ");
                //Console.ReadLine();

                // Test whether mapping works correctly:               
                //neural.TestMapping();
            }

            // Perform training: 
            neural.TrainNetwork();

            // Calculate outputs and exact values for first 5 points in the training set:
            Console.WriteLine("A couple of calculated outputs from the training set (including verification points):");
            using (StreamWriter writer = new StreamWriter("E:/users/tadejk/NeuralNetworkTrain/store_multiple_output10/SavedNN/VerificationPoints.txt"))
            //using (StreamWriter writer = new StreamWriter("../../testdata/SavedNeuralNetwork/VerificationPoints.txt"))
            {
                writer.WriteLine();
                writer.WriteLine("Number of hidden layers: " + neural.NumHiddenLayers);
                writer.WriteLine("Learning rate: " + neural.LearningRate);
                writer.WriteLine("Momentum: " + neural.Momentum);
                writer.WriteLine();

                IVector errorTrain = null;
                IVector errorVer = null;

                IBoundingBox bounds = null;
                neural.TrainingData.GetOutputRange(ref bounds);
                Console.Write("Range: ");
                writer.Write("Range: ");
                for (int i = 0; i < neural.OutputLength; i++)
                {
                    Console.Write((bounds.GetMax(i) - bounds.GetMin(i)) + " ; ");
                    writer.Write((bounds.GetMax(i) - bounds.GetMin(i)) + " ; ");
                }

                Console.WriteLine();
                writer.WriteLine();

                neural.GetErrorsTrainingMeanAbs(ref errorTrain);
                neural.GetErrorsVerificationMeanAbs(ref errorVer);
                Console.WriteLine("Training average abs. error: " + errorTrain);
                writer.WriteLine("Training average abs. error: " + errorTrain);
                Console.WriteLine("Verification average abs. error: " + errorVer);
                writer.WriteLine("Verification average abs. error: " + errorVer);

                for (int i = 0; i < neural.NumTrainingPoints; ++i)
                {
                    if (verificationIndices.Contains(i))
                    {
                        IVector input = sarze.GetInputParameters(i);
                        IVector exactOutput = sarze.GetOutputValues(i);
                        IVector calculatedOutput = new Vector(outputLength);
                        neural.CalculateOutput(input, ref calculatedOutput);
                        Console.WriteLine();
                        writer.WriteLine();
                        Console.WriteLine("Point No. " + i + "of the training set, is verification point: " + verificationIndices.Contains(i));
                        writer.WriteLine("Point No. " + i + "of the training set, is verification point: " + verificationIndices.Contains(i));
                        Console.WriteLine("  Input parameters No. " + i + ":");
                        writer.WriteLine("  Input parameters No. " + i + ":");
                        Console.WriteLine("  " + input.ToString());
                        writer.WriteLine("  " + input.ToString());
                        Console.WriteLine("  Exact output: ");
                        writer.WriteLine("  Exact output: ");
                        Console.WriteLine("  " + exactOutput.ToString());
                        writer.WriteLine("  " + exactOutput.ToString());
                        Console.WriteLine("  Approximated output: ");
                        writer.WriteLine("  Approximated output: ");
                        Console.WriteLine("  " + calculatedOutput.ToString());
                        writer.WriteLine("  " + calculatedOutput.ToString());
                        IVector dif = null;
                        Vector.Subtract(calculatedOutput, exactOutput, ref dif);
                        Console.WriteLine("Norm of the difference between sampled and calculated output: " + dif.Norm);
                        writer.WriteLine("Norm of the difference between sampled and calculated output: " + dif.Norm);
                    }
                }
                writer.Close();
            }
            return neural;
        }


        public static INeuralApproximator TrainNetwork(ref INeuralApproximator neuralApp)
        {
            //Training Data
            SampledDataSet sarze = neuralApp.TrainingData;

            // Test output (for debugging purposes):
            Console.WriteLine();
            Console.WriteLine("Neural network data: ");
            Console.WriteLine(neuralApp.ToString());
            Console.WriteLine("Insert <Enter> in order to continue: ");
            //Console.ReadLine();    

            // Perform training: 
            neuralApp.TrainNetwork();

            // Calculate outputs and exact values for first 5 points in the training set:
            Console.WriteLine("A couple of calculated outputs from the training set (including verification points):");
            for (int i = 0; i < 5; ++i)
            {
                IVector input = sarze.GetInputParameters(i);
                IVector exactOutput = sarze.GetOutputValues(i);
                IVector calculatedOutput = new Vector(neuralApp.OutputLevel);
                neuralApp.CalculateOutput(input, ref calculatedOutput);
                Console.WriteLine();
                Console.WriteLine("Point No. " + i + "of the training set, is verification point: " + neuralApp.VerificationIndices.Contains(i));
                Console.WriteLine("  Input parameters No. " + i + ":");
                Console.WriteLine("  " + input.ToString());
                Console.WriteLine("  Exact output: ");
                Console.WriteLine("  " + exactOutput.ToString());
                Console.WriteLine("  Approximated output: ");
                Console.WriteLine("  " + calculatedOutput.ToString());
                IVector dif = null;
                Vector.Subtract(calculatedOutput, exactOutput, ref dif);
                Console.WriteLine("Norm of the difference between sampled and calculated output: " + dif.Norm);
            }
            return neuralApp;
        }


        public static void StoreNetwork(string directoryPath, string fileName, string internalStateFileName, INeuralApproximator neuralApp,
           bool saveRestored)
        {
            int numInputs = neuralApp.InputLength;
            int numOutputs = neuralApp.OutputLength;
            int numAdditionalEpochs = 500;

            if ((directoryPath == null) || (fileName == null) || (internalStateFileName == null))
            {
                Console.WriteLine("Creating an example neural network and training it wiht generated data...");
                Console.WriteLine("  number of inputs: " + numInputs + ", number of outputs: " + numOutputs);

                INeuralApproximator approximator = NeuralTadej.TrainNetwork(ref neuralApp);
            }
            else
            {
                Console.WriteLine();
                Console.WriteLine("Test of saving and restoring neural network approximator to/from file.");
                Console.WriteLine("Creating an example neural network and training it wiht generated data...");
                Console.WriteLine("  number of inputs: " + numInputs + ", number of outputs: " + numOutputs);

                INeuralApproximator approximator = NeuralTadej.TrainNetwork(ref neuralApp);

                IVector errTrainRMSFirst = new Vector(approximator.OutputLength);
                approximator.GetErrorsTrainingRms(ref errTrainRMSFirst);
                IVector errVerificationRMSFirst = new Vector(approximator.OutputLength);
                approximator.GetErrorsVerificationRms(ref errVerificationRMSFirst);
                Console.WriteLine("... creation and first training done, errors calculated and stored.");
                Console.WriteLine();

                Console.WriteLine();
                Console.WriteLine("Performing additional training, " + numAdditionalEpochs + " additional epochs...");
                approximator.TrainNetwork(numAdditionalEpochs);
                IVector errTrainRMSFinal = new Vector(approximator.OutputLength);
                approximator.GetErrorsTrainingRms(ref errTrainRMSFinal);
                IVector errVerificationRMSFinal = new Vector(approximator.OutputLength);
                approximator.GetErrorsVerificationRms(ref errVerificationRMSFinal);
                Console.WriteLine("... additional training done, errors calculated and stored.");

                Console.WriteLine("Saving internal state of the neural network... ");
                approximator.SaveNetwork(Path.Combine(directoryPath, internalStateFileName));
                Console.WriteLine("... saving internal state done.");
                Console.WriteLine("File path: " + approximator.NetworkStateFilePath);
                Console.WriteLine();
                Console.WriteLine("Saving the entire network approximator...");
                NeuralApproximatorDtoBase dto = new NeuralApproximatorDtoBase();
                dto.CopyFrom(approximator);
                ISerializer serializer = new SerializerJson();
                string neuralApproximationPath = Path.Combine(directoryPath, fileName);
                serializer.Serialize<NeuralApproximatorDtoBase>(dto, neuralApproximationPath);
                approximator = null;
                Console.WriteLine("... saving the entire network done.");

                Console.WriteLine();
                Console.WriteLine("Restoring neural network approximator from a file...");
                ISerializer serializerRestoring = new SerializerJson();
                NeuralApproximatorDtoBase dtoRestored = serializerRestoring.DeserializeFile<NeuralApproximatorDtoBase>
                    (Path.Combine(directoryPath, fileName));
                INeuralApproximator approximatorRestored = new NeuralApproximatorAforge();
                dtoRestored.CopyTo(ref approximatorRestored);
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
            }
        }


        #endregion

        #region Testing

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputLength"></param>
        /// <param name="outputLength"></param>
        /// <returns></returns>
        /// $A Tako78 Mar11;
        public static INeuralApproximator ExampleQuadratic(int inputLength, int outputLength)
        {
            // Specify reasonable number of samples and verification points:
            int numTrainingElements = 5 * inputLength * inputLength;

            //int numTrainingElements = 50;
            int numVerificationPoints = (int)Math.Round((double)numTrainingElements * 0.5);
            numTrainingElements += numVerificationPoints;

            // Create training data by randomly sampling a specific quadratic response:
            SampledDataSet samples = SampledDataSet.CreateExampleQuadratic(inputLength, outputLength, numTrainingElements);

            // Speciy which samples will be used for verification of approximation:
            IndexList verificationIndices = IndexList.CreateRandom(numVerificationPoints, 0 /* lowerbound */, numTrainingElements - 1);

            // Create neural network and set basic parameters:
            INeuralApproximator neural = new NeuralApproximatorAforge();
            neural.OutputLevel = 1;
            neural.InputLength = inputLength;
            neural.OutputLength = outputLength;

            // Set prepared data:
            neural.TrainingData = samples;
            neural.VerificationIndices = verificationIndices;

            // Set network layout:
            neural.MultipleNetworks = true;
            neural.SetHiddenLayers(30, 20);

            // Set training parameters:
            neural.MaxEpochs = 5000;
            neural.EpochsInBundle = 50;
            neural.ToleranceRms = new Vector(outputLength, 0.01);

            // Specify learning parameters:
            neural.LearningRate = 0.1;
            neural.SigmoidAlphaValue = 0.1;
            neural.Momentum = 0.5;

            // Specify parameters defining the bounds for data applied to input and output neurons:
            neural.InputBoundsSafetyFactor = 1.5;
            neural.OutputBoundsSafetyFactor = 1.5;

            // Change the targeted range of input and otput neuraons: 
            neural.InputNeuronsRange.Reset();
            neural.InputNeuronsRange.UpdateAll(-2.0, 2.0);
            neural.OutputNeuronsRange.Reset();
            neural.OutputNeuronsRange.UpdateAll(-1, 1);

            // Test output (for debugging purposes):
            Console.WriteLine();
            Console.WriteLine("Neural network data: ");
            Console.WriteLine(neural.ToString());
            Console.WriteLine("Insert <Enter> in order to continue: ");
            Console.ReadLine();

            // Perform training: 
            neural.TrainNetwork();

            // Calculate outputs and exact values for first 5 points in the training set:
            Console.WriteLine("A couple of calculated outputs from the training set (including verification points):");
            for (int i = 0; i < 5; ++i)
            {
                IVector input = samples.GetInputParameters(i);
                IVector exactOutput = samples.GetOutputValues(i);
                IVector calculatedOutput = new Vector(outputLength);
                neural.CalculateOutput(input, ref calculatedOutput);
                Console.WriteLine();
                Console.WriteLine("Point No. " + i + "of the training set, is verification point: " + verificationIndices.Contains(i));
                Console.WriteLine("  Input parameters No. " + i + ":");
                Console.WriteLine("  " + input.ToString());
                Console.WriteLine("  Exact output: ");
                Console.WriteLine("  " + exactOutput.ToString());
                Console.WriteLine("  Approximated output: ");
                Console.WriteLine("  " + calculatedOutput.ToString());
                IVector dif = null;
                Vector.Subtract(calculatedOutput, exactOutput, ref dif);
                Console.WriteLine("Norm of the difference between sampled and calculated output: " + dif.Norm);
            }
            return neural;
        }

        // <summary>**********TO DO**********.</summary>
        /// <param name="trainingData">Training Data.</param>
        /// <param name="inputColumnSet">List of input Parameters.</param>
        /// <param name="inputlowerBound">Lower coordinate input neurons range.</param>
        /// <param name="inputupperBound">Upper coordinate input neurons range.</param>
        /// <param name="outputColumnSet">List of output Parameters.</param>
        /// <param name="outputlowerBound">Lower coordinate output neurons range.</param>
        /// <param name="outputupperBound">Upper coordinate output neurons range.</param>
        /// $A Tako78 June20;
        public static void neuronsDataRange(SampledDataSet trainingData,
            ref List<double[]> inputColumnSet, double inputlowerBound, double inputupperBound,
            ref List<double[]> outputColumnSet, double outputlowerBound, double outputupperBound)
        {
            if (trainingData == null)
                throw new ArgumentNullException("No input data in column. ");
            if (inputlowerBound == null)
                throw new ArgumentNullException("No lower bound set for input range. ");
            if (inputupperBound == null)
                throw new ArgumentNullException("No upper bound set for input range. ");
            if (outputlowerBound == null)
                throw new ArgumentNullException("No lower bound set for output range. ");
            if (outputupperBound == null)
                throw new ArgumentNullException("No upper bound set for output range. ");

            int inputLenght;
            int outputLenght;
            int numSamples;
            inputLenght = trainingData.InputLength;
            outputLenght = trainingData.OutputLength;
            numSamples = trainingData.Length;

            List<double[]> tmpInputColumnSet = new List<double[]>();
            List<double[]> tmpOutputColumnSet = new List<double[]>();
            List<double[]> tmpInputSet = new List<double[]>();
            List<double[]> tmpOutputSet = new List<double[]>();
            inputColumnSet = new List<double[]>();
            outputColumnSet = new List<double[]>();

            double[] inputMax = new double[inputLenght];
            double[] inputMin = new double[inputLenght];
            double[] inputRange = new double[inputLenght];
            double[] outputMax = new double[outputLenght];
            double[] outputMin = new double[outputLenght];
            double[] outputRange = new double[outputLenght];

            double inputBound = inputupperBound - inputlowerBound;
            double outputBound = outputupperBound - outputlowerBound;

            //List of Input and output parameters
            for (int i = 0; i < inputLenght; i++)
            {
                double[] tmpInputData = new double[numSamples];
                for (int j = 0; j < numSamples; j++)
                {
                    IVector inputData = trainingData.GetInputParameters(j);
                    tmpInputData[j] = inputData[i];
                }
                inputMax[i] = tmpInputData.Max();
                inputMin[i] = tmpInputData.Min();
                inputRange[i] = inputMax[i] - inputMin[i];

                tmpInputColumnSet.Add(tmpInputData);
            }
            for (int i = 0; i < outputLenght; i++)
            {
                double[] tmpOutputData = new double[numSamples];
                for (int j = 0; j < numSamples; j++)
                {
                    IVector outputData = trainingData.GetOutputValues(j);
                    tmpOutputData[j] = outputData[i];
                }
                outputMax[i] = tmpOutputData.Max();
                outputMin[i] = tmpOutputData.Min();
                outputRange[i] = outputMax[i] - outputMin[i];

                tmpOutputColumnSet.Add(tmpOutputData);
            }
            //Transfering data to Neurons Training Range
            for (int i = 0; i < inputLenght; i++)
            {
                double[] tmpInputData = new double[numSamples];
                double[] tempInputData = new double[numSamples];
                tmpInputData = tmpInputColumnSet[i];
                for (int j = 0; j < numSamples; j++)
                {
                    double inputPar = 0.0;
                    inputPar = tmpInputData[j];
                    inputPar = (inputPar - inputMin[i]) / inputRange[i];
                    inputPar = (inputBound * inputPar) + inputlowerBound;
                    tempInputData[j] = inputPar;
                }

                tmpInputSet.Add(tempInputData);
            }
            for (int i = 0; i < outputLenght; i++)
            {
                double[] tmpOutputData = new double[numSamples];
                double[] tempOutputData = new double[numSamples];
                tmpOutputData = tmpOutputColumnSet[i];

                for (int j = 0; j < numSamples; j++)
                {
                    double outputPar = 0.0;
                    outputPar = tmpOutputData[j];
                    outputPar = (outputPar - outputMin[i]) / outputRange[i];
                    outputPar = (outputBound * outputPar) + outputlowerBound;
                    tempOutputData[j] = outputPar;
                }

                tmpOutputSet.Add(tempOutputData);
            }

            //Copy data from columns to training sets
            for (int i = 0; i < numSamples; i++)
            {
                double[] tmpInputData = new double[inputLenght];
                double[] tmpIN = new double[numSamples];
                double[] tmpOutputData = new double[outputLenght];
                double[] tmpOUT = new double[numSamples];

                for (int j = 0; j < inputLenght; j++)
                {
                    tmpIN = tmpInputSet[j];
                    tmpInputData[j] = tmpIN[i];
                }
                inputColumnSet.Add(tmpInputData);

                for (int j = 0; j < outputLenght; j++)
                {
                    tmpOUT = tmpOutputSet[j];
                    tmpOutputData[j] = tmpOUT[i];
                }
                outputColumnSet.Add(tmpOutputData);
            }


        }

        #endregion

        
        /// <summary>Copy training data set to new training data set.</summary>
        /// /// <param name="trainingData">Training data set.</param>
        /// <param name="newtrainingData">New training data set.</param>
        public static void CopyTrainingData(SampledDataSet trainingData, ref SampledDataSet newtrainingData)
        {
            int inputLenght;
            int outputLenght;
            inputLenght = trainingData.InputLength;
            outputLenght = trainingData.OutputLength;
            IVector inputData;
            IVector outputData;
            if (newtrainingData == null)
                newtrainingData = new SampledDataSet();

            for (int i = 0; i < trainingData.Length; ++i)
            {
                inputData = trainingData.GetInputParameters(i);
                outputData = trainingData.GetOutputValues(i);
                newtrainingData.AddElement(new SampledDataElement(inputData, outputData));
            }
        }
        
        /// <summary>Creates an array with 0 elements.</summary>
        /// $A Tako78 Mar11;
        static void TestArray0Elements()
        {
            double[] a;
            a = new double[0];


        }

    }

}