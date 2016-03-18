// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;

using IG.Lib;

namespace IG.Forms
{
    public partial class ControlViewerControl : UserControl
    {
        public ControlViewerControl()
        {
            InitializeComponent();


            btnControlPosition.Visible = ShowDummyContents;
        }

        public ControlViewerControl(Control viewedControl): this()
        {
            DisplayedControl = viewedControl;
        }



        private bool _showDummyContents = true;
         

        /// <summary>Whether the dummy contents of the contaier that holds the viewed control is shown.
        /// <para>Showing dummy contents (usually just a button) can serve to verify that layout of the form is 
        /// rendered correctly.</para></summary>
        public bool ShowDummyContents
        {
            get { return _showDummyContents; }
            set {
                if (value != _showDummyContents)
                {
                    _showDummyContents = value;
                    btnControlPosition.Visible = _showDummyContents;
                    btnTest.Visible = _showDummyContents;
                }
            }
        }


        private Control _displayedControl = null;



        /// <summary>Gets or sets the control that is displayed by the current control.</summary>
        public Control DisplayedControl
        {
            get { return _displayedControl; }
            set
            {
                if (_displayedControl != value)
                {
                    if (_displayedControl != null)
                    {
                        // Another control has been previously shown within the container, remove it first:
                        try
                        {
                            pnlControlContainer.Controls.Remove(_displayedControl);
                            _displayedControl = null;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(Environment.NewLine + "Error when trying to remove the old control: "
                                + Environment.NewLine + "  " + ex.Message);
                        }
                    }
                    if (value != null)
                    {
                        // Add the new control to the container:
                        try
                        {
                            ShowDummyContents = false;
                            pnlControlContainer.Controls.Add(value);
                            _displayedControl = value;
                            _displayedControl.BringToFront();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(Environment.NewLine + "Error when trying to add the control to be displayed: "
                                + Environment.NewLine + "  " + ex.Message);
                            throw;
                        }
                    } else
                    {
                        ShowDummyContents = true;
                    }
                }
            }
        }






        public static Control CreateNewControl(Type controlType, bool throwIfIncorrectType)
        {
            if (controlType.IsSubclassOf(typeof(Control)) && !controlType.IsSubclassOf(typeof(Control)))
                return (Control)Activator.CreateInstance(controlType);
            else
            {
                if (throwIfIncorrectType)
                {
                    throw new ArgumentException("Type does not represent System.Windows.Forms.Control: ");
                }
                return null;
            }
        }
        



        public void ShowControl(Type controlType)
        {
            if (controlType == null)
                throw new ArgumentException("Type of Control object is not specified (null reference).");
            if (!controlType.IsSubclassOf(typeof(Control)) || controlType.IsSubclassOf(typeof(Form)))
                throw new ArgumentException("The following type is does not represent a Control or it represents a Form: "
                    + Environment.NewLine + "  " + controlType.FullName);
            Control control = (Control) CreateNewControl(controlType, false);
            if (control == null)
                throw new ArgumentException("Could not instantiate a control of the folllowint");
            this.DisplayedControl = control;
        }

        private void btnCloseControls_Click(object sender, EventArgs e)
        {
            this.pnlControlContainer.Visible = true;
        }

        private void btnZoomOut_Click(object sender, EventArgs e)
        {
            double factor = 1.2;
            if (DisplayedControl != null && DisplayedControl.Visible)
            {
                int width = DisplayedControl.Width;
                int height = DisplayedControl.Height;
                width = (int)((double) width / factor);
                height = (int)((double)height / factor);
                DisplayedControl.Width = width;
                DisplayedControl.Height = height;
            } else if (btnControlPosition.Visible)
            {
                int width = btnControlPosition.Width;
                int height = btnControlPosition.Height;
                width = (int)((double) width / factor);
                height = (int)((double) height / factor);
                btnControlPosition.Width = width;
                btnControlPosition.Height = height;
            }
        }

        private void btnZoomIn_Click(object sender, EventArgs e)
        {
            double factor = 1.2;
            if (DisplayedControl != null && DisplayedControl.Visible)
            {
                int width = DisplayedControl.Width;
                int height = DisplayedControl.Height;
                width = (int)((double) width * factor);
                height = (int)((double) height * factor);
                DisplayedControl.Width = width;
                DisplayedControl.Height = height;
            } else if (btnControlPosition.Visible)
            {
                int width = btnControlPosition.Width;
                int height = btnControlPosition.Height;
                width = (int)((double) width * factor);
                height = (int)((double) height * factor);
                btnControlPosition.Width = width;
                btnControlPosition.Height = height;
            }
        }
    }


}
