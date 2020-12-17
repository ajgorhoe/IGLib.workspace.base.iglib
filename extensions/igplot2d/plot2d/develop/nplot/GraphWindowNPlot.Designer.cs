
//using System;
//using System.Drawing;
//// Copyright (c) Igor Grešovnik (2009), IGLib license; http://www2.arnes.si/~ljc3m2/igor/ioptlib/

//using System.Drawing.Drawing2D;
//using System.Collections;
//using System.ComponentModel;
//using System.Windows.Forms;
//using System.Drawing.Printing;
//using System.Data;
//using System.IO;
//using System.Collections.Generic;
//using System.Reflection;

//using NPlot;


//namespace IG.Plot2d
//{
    
//    /// <summary>Stand-alone graph window using NPlot as plotting engine.
//    /// For layout, GraphWindowTemplate class is inherited.</summary>
//    /// $A Igor Sept09;
//    partial class GraphWindowNPlot
//    {


//        /// <summary>Overridden ResumeLayout() that enables switching on or off the original ResumeLayout()
//        /// dependent on the value of the ResumeLayoutInInitializeComponent flag.</summary>
//        protected new void ResumeLayout()
//        {
//            System.Console.WriteLine();
//            System.Console.WriteLine("Within GraphWindowNPlot:");
//            base.ResumeLayout();
//        }



//        /// <summary>Required method for Designer support.</summary>
//        protected override void InitializeComponent()
//        {
//            bool resumeLayout = ResumeLayoutInInitializeComponent; // store flag value
//            ResumeLayoutInInitializeComponent = false;  // prevent resuming layout in base class' InitializeComponent()
//            base.InitializeComponent();  
//            ResumeLayoutInInitializeComponent = resumeLayout; // restore the original value

//            // Create a new graph panel and replace that of the parent class:
//            this.pnlGraphNPlot = new NPlot.Windows.PlotSurface2D();
//            // Copy properties from the original graph panel defined in the parent class:
//            this.pnlGraphNPlot.Location = new System.Drawing.Point(pnlGraph.Location.X,pnlGraph.Location.Y);
//            this.pnlGraphNPlot.Name = "pnlGraphNPlot";
//            this.pnlGraphNPlot.Size = new System.Drawing.Size(pnlGraph.Size.Width, pnlGraph.Size.Height);
//            this.pnlGraphNPlot.TabIndex = pnlGraph.TabIndex;
//            // Finally, replace the old graph panel from parent class with the NPlot panel:
//            List<System.Windows.Forms.Control> lst = new List<Control>();
//            for (int i = 0; i<this.Controls.Count; ++i)
//            {
//                // Copy current controls of the window to a list, but replace the original graph panel
//                // with a NPLot graph control:
//                if (this.Controls[i]!=pnlGraph)
//                    lst.Add(this.Controls[i]);
//                else
//                    lst.Add(pnlGraphNPlot);
//            }
//            this.Controls.Clear();
//            this.Controls.AddRange(lst.ToArray());
            
//            // Now, resume layout if the flag specifies so:
//            if (resumeLayout)
//                this.ResumeLayout(false);
//        }  // InitializeComponent()


//        protected NPlot.Windows.PlotSurface2D pnlGraphNPlot;

//    }

//}