
// SAMPLED DATA
 
using System.IO;
using System.IO.Compression;
using System.Xml;
using System.Text;
using System;
using System.Collections.Generic;

using IG.Lib;
using IG.Neural;

namespace IG.Num
{

    class TestClass
    {


        /// <summary>Loads sampled data and Definition data from multible CSV files.
        /// Sampled data consist of one output and multiple input parameters.
        /// Input parameters are the same in all files but output parameter should be different.</summary>
        /// <param name="fileNames">Path to the file where sampled data are saved.</param>
        /// <param name="sampledData">Sampled data set.</param>
        /// $A Tako78 Mar11;
        public static void LoadSampledDataCombinedOutputsJSON(ref SampledDataSet sampledData, params string[] fileNames)
        {
            if (string.IsNullOrEmpty(fileNames[0]))
                throw new ArgumentNullException("File names not specified.");
            if (sampledData == null)
                sampledData = new SampledDataSet();
            int filesNum = fileNames.Length;
            SampledDataSet[] inputSets = new SampledDataSet[filesNum];
            for (int i = 0; i < filesNum; ++i)
            {
                SampledDataSet.LoadSampledDataJson(fileNames[i], ref  inputSets[i]);
            }
            SampledDataSet.SampledDataCombineOutputs(ref sampledData, inputSets);

        }

    }  // class TestClass
 

}