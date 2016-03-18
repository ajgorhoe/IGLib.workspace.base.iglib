// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace IG.Forms
{
    public partial class TimerForm : Form
    {
        public TimerForm()
        {
            InitializeComponent();
            //this.stopWatchControl2.SizeChanged += new System.EventHandler(this.containedFormSizeChanged);
        }


        private void resizeFormToContainedControl()
        {
            Console.WriteLine(Environment.NewLine + "Resizing window according to inner control size." + Environment.NewLine);
            int sizeX = stopWatchControl2.Size.Width;
            int sizeY = stopWatchControl2.Size.Height;
            this.Width = sizeX;
            this.Height = sizeY;
        }


        private void containedFormSizeChanged(object sender, EventArgs e)
        {
            // resizeFormToContainedControl();
        }


        private void StopWatchForm_Load(object sender, EventArgs e)
        {
            // this.AutoSize = true;
            // this.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            // resizeFormToContainedControl();

        }
    }
}
