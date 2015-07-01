

using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Linq;
using System.Threading;
using System.Text;
using System.IO;
using System.IO.Compression;
using System.Xml;

using IG.Lib;
using IG.Forms;
using IG.Num;

namespace IG.Gr
{

    /// <summary>Contains the <see cref="Main1"/> function that performs some tests on 2D plotting with Zedgraph.</summary>
    /// $A Igor Jul09 Nov11;
    public class Plot2dTestZedgraph
    {

        /// <summary>Performs some tests on 2d plotting</summary>
        /// <param name="AppArguments"></param>
        [STAThread]
        public static void Main1(string[] args)
        {
            Console.WriteLine("Test of Zedgraph graphics.");

            //PlotterZedGraph.ExempleSinePlots();

            // PlotterZedGraph.ExampleLissajous();

            //PlotterZedGraph.ExampleDecorations();

            //PlotterZedGraph.ExampleCurveStylesWithSave("c:/temp/ImageZedGraph.bmp");

          
            //TestZedGraph.TestZedgraph1();

            Console.WriteLine("\r\nBye.\r\n");
        }  // Main1()

    }  // class Plot2dTest






} // namespace igform_console_test
