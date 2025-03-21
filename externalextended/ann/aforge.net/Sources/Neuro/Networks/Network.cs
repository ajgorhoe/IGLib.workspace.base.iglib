// AForge Neural Net Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright � Andrew Kirillov, 2005-2009
// andrew.kirillov@aforgenet.com
//

namespace AForge.Neuro
{
    using System;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters.Binary;

    /// <summary>
    /// Base neural network class.
    /// </summary>
    /// 
    /// <remarks>This is a base neural netwok class, which represents
    /// collection of neuron's layers.</remarks>
    /// 
    [Serializable]
    public abstract class Network
    {
        /// <summary>
        /// Network's inputs count.
        /// </summary>
        protected int inputsCount;

        /// <summary>
        /// Network's layers count.
        /// </summary>
        protected int layersCount;

        /// <summary>
        /// Network's layers.
        /// </summary>
        protected Layer[] layers;

        /// <summary>
        /// Network's output vector.
        /// </summary>
        protected double[] output;

        /// <summary>
        /// Network's inputs count.
        /// </summary>
        public int InputsCount
        {
            get { return inputsCount; }
        }

        /// <summary>
        /// Network's layers count.
        /// </summary>
        public int LayersCount
        {
            get { return layersCount; }
        }

        /// <summary>
        /// Network's output vector.
        /// </summary>
        /// 
        /// <remarks><para>The calculation way of network's output vector is determined by
        /// layers, which comprise the network.</para>
        /// 
        /// <para><note>The property is not initialized (equals to <see langword="null"/>) until
        /// <see cref="Compute"/> method is called.</note></para>
        /// </remarks>
        /// 
        public double[] Output
        {
            get { return output; }
        }

        /// <summary>
        /// Network's layers accessor.
        /// </summary>
        /// 
        /// <param name="index">Layer index.</param>
        /// 
        /// <remarks>Allows to access network's layer.</remarks>
        /// 
        public Layer this[int index]
        {
            get { return layers[index]; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Network"/> class.
        /// </summary>
        /// 
        /// <param name="inputsCount">Network's inputs count.</param>
        /// <param name="layersCount">Network's layers count.</param>
        /// 
        /// <remarks>Protected constructor, which initializes <see cref="inputsCount"/>,
        /// <see cref="layersCount"/> and <see cref="layers"/> members.</remarks>
        /// 
        protected Network( int inputsCount, int layersCount )
        {
            this.inputsCount = Math.Max( 1, inputsCount );
            this.layersCount = Math.Max( 1, layersCount );
            // create collection of layers
            this.layers = new Layer[this.layersCount];
        }

        /// <summary>
        /// Compute output vector of the network.
        /// </summary>
        /// <param name="input">Input vector.</param>
        /// <returns>Returns network's output vector.</returns>
        /// <remarks><para>The actual network's output vecor is determined by layers,
        /// which comprise the layer - represents an output vector of the last layer
        /// of the network. The output vector is also stored in <see cref="Output"/> property.</para>
        /// <para><note>The method may be called safely from multiple threads to compute network's
        /// output value for the specified input values. However, the value of
        /// <see cref="Output"/> property in multi-threaded environment is not predictable,
        /// since it may hold network's output computed from any of the caller threads. Multi-threaded
        /// access to the method is useful in those cases when it is required to improve performance
        /// by utilizing several threads and the computation is based on the immediate return value
        /// of the method, but not on network's output property.</note></para>
        /// </remarks>
        public virtual double[] Compute( double[] input )
        {
            // local variable to avoid mutlithread conflicts
            double [] output = input;

            // compute each layer
            foreach ( Layer layer in layers )
            {
                output = layer.Compute( output );
            }

            // assign output property as well (works correctly for single threaded usage)
            this.output = output;

            return output;
        }

        /// <summary>
        /// Randomize layers of the network.
        /// </summary>
        /// <remarks>Randomizes network's layers by calling <see cref="Layer.Randomize"/> method
        /// of each layer.</remarks>
        public virtual void Randomize( )
        {
            foreach ( Layer layer in layers )
            {
                layer.Randomize( );
            }
        }




        #region BinaryFormatterIssues

        #region AddedCode

        /* IMPORTANT - BINARY SERIALIZATION
        BinaryFormatter class and the associated IFormatter were made obsolete in more recent frameworks
        due to potential security issues, and are not suported since NET 8.
        In legacy IGLib, to enable some legacy software, the project references a NuiGet package that still
        provides the functionality.
        The package is referenced when the target framework is .NET 8 or later. Beside this, compiler warnings
        are issuer in the code that uses these technologies, and informative messages are written to console
        for a limited number of times. These warnigs can be swotched on or off via the
        LanchConsoleWarningsOnBinarFormatterUse property. */

        /// <summary>Specifies whether informational messages are written to console when the BinaryFormatter
        /// is used.</summary>
        public static bool DoLaunchConsoleWarningsOnBinarFormatterUse { get; set; } = true;

        public static int MaxLaunchedBinayFormattingConsoleWarnings { get; set; } = 0;

        private static int NumLaunchedBinaryFormattingConsoleWarinings { get; set; } = 0;

        private static void LaunchBinaryFormattingConsoleWarining()
        {
            try
            {

                if (DoLaunchConsoleWarningsOnBinarFormatterUse &&
                    NumLaunchedBinaryFormattingConsoleWarinings < MaxLaunchedBinayFormattingConsoleWarnings)
                {
#if NET8_0_OR_GREATER
                    Console.WriteLine("\n\nNOTICE: Using BinaryFormatter from System.Runtime.Serialization.Formatters package.\n");
#else
                    Console.WriteLine("\n\nNOTICE: Using built-in BinaryFormatter.\n");
#endif
                }
            }
            finally
            {
                ++NumLaunchedBinaryFormattingConsoleWarinings;
            }
        }

        // Disable the warning / error due to use of IFormatter and BinaryFormatter:
#pragma warning disable SYSLIB0011

        #endregion AddedCode






        /// <summary>
        /// Save network to specified file.
        /// </summary>
        /// <param name="fileName">File name to save network into.</param>
        /// <remarks><para>The neural network is saved using .NET serialization (binary formatter is used).</para></remarks>
        public void Save( string fileName )
        {
            FileStream stream = new FileStream( fileName, FileMode.Create, FileAccess.Write, FileShare.None );
            Save( stream );
            stream.Close( );
        }

        /// <summary>
        /// Save network to specified file.
        /// </summary>
        /// <param name="stream">Stream to save network into.</param>
        /// <remarks><para>The neural network is saved using .NET serialization (binary formatter is used).</para></remarks>
        public void Save( Stream stream )
        {
            LaunchBinaryFormattingConsoleWarining();  // $$
            IFormatter formatter = new BinaryFormatter( );
            formatter.Serialize( stream, this );
        }

        /// <summary>
        /// Load network from specified file.
        /// </summary>
        /// <param name="fileName">File name to load network from.</param>
        /// <returns>Returns instance of <see cref="Network"/> class with all properties initialized from file.</returns>
        /// <remarks><para>Neural network is loaded from file using .NET serialization (binary formater is used).</para></remarks>
        public static Network Load( string fileName )
        {
            FileStream stream = new FileStream( fileName, FileMode.Open, FileAccess.Read, FileShare.Read );
            Network network = Load( stream );
            stream.Close( );

            return network;
        }

        /// <summary>
        /// Load network from specified file.
        /// </summary>
        /// <param name="stream">Stream to load network from.</param>
        /// <returns>Returns instance of <see cref="Network"/> class with all properties initialized from file.</returns>
        /// <remarks><para>Neural network is loaded from file using .NET serialization (binary formater is used).</para></remarks>
        /// 
        public static Network Load( Stream stream )
        {
            LaunchBinaryFormattingConsoleWarining();  // $$
            IFormatter formatter = new BinaryFormatter( );
            Network network = (Network) formatter.Deserialize( stream );
            return network;
        }



        // Re-enable the warning / error due to use of IFormatter and BinaryFormatter:
#pragma warning restore SYSLIB0011

        #endregion BinaryFormatterIssues


    }
}
