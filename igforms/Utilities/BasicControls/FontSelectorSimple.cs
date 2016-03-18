// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace IG.Forms
{


    /// <summary>Simple control that allows font selection.</summary>
    /// <remarks>
    /// <para>Contains a button for font selection and a label with short description of the current font.</para>
    /// <remarks>The font is initially unspecified when control if control is created with default constructor.
    /// <remarks>Font can be queeried, and change of font fires an event.</remarks></remarks>
    /// </remarks>
    public partial class FontSelectorSimple : UserControl
    {
        public FontSelectorSimple()
        {
            
            InitializeComponent();
        }

        public FontSelectorSimple(Font initialFont): this()
        {
            this.Font = Font;
        }

        #region Data

        bool _isFontSelected = false;

        protected bool IsFontSelected
        {
            get { return _isFontSelected; }
            set
            {
                if (value != _isFontSelected)
                {
                    _isFontSelected = value;
                }
            }
        }

        Font _selectedFont = new Font("Times New Roman", 10);

        public Font SelectedFont
        {
            get { return _selectedFont; }
            set
            {
                if (value != _selectedFont || !IsFontSelected)
                {
                    _selectedFont = value;
                    if (!IsFontSelected)
                    {
                        IsFontSelected = true;
                        OnFontSelectedFirstTime(this, new FontEventArgs(value));
                    }
                    OnFontSelected(this, new FontEventArgs(value));
                }
            }
        }




        public string _initialFontLabelText = "< Click to select font! >";

        public string InitialFontLabelText
        {
            get { return _initialFontLabelText; }
            set { _initialFontLabelText = value; }
        }

        protected Color InitialFontLabelForeColor
        { get { return Color.LightBlue; } }

        protected Color NormalFontLabelForeColor
        { get { return Color.Black; } }

        /// <summary>Returns short textual description of the selected forn.</summary>
        public string FontDescriptionShort()
        {
            if (!IsFontSelected)
                return "";
            else
                return FontDesctiptionShort(SelectedFont);
        }

        #endregion Data

        #region Actions

        /// <summary>Returns a short desription of the specified font.</summary>
        /// <remarks>Description contains font size, family name, and descriptors indicating whether it is
        /// bold, italic, underlined, or strikedout, separated by commas. If font has no modifier properties
        /// like bold then hte corresponding discriptor is just empty. Descriptors are like "bold", 
        /// "italic", "underlined", etc.</remarks>
        /// <param name="font">Font whose description is returned.</param>
        /// <returns></returns>
        public static string FontDesctiptionShort(Font font)
        {
            if (font == null)
                return "< undefined >";
            string ret =
                font.SizeInPoints.ToString() + " pt "
                + font.Name;
            if (font.Bold)
                ret += ", bold";
            if (font.Italic)
                ret += ", italic";
            if (font.Underline)
                ret += ", underlined";
            if (font.Strikeout)
                ret += ", strikedout";
            return ret;
        }

        /// <summary>Updates appearance of the control, according to the state of font selection
        /// and the actual font.</summary>
        public void UpdateAppearance()
        {
            // Label defining the selected font:
            if (!IsFontSelected)
            {
                lblFontDescription.ForeColor = InitialFontLabelForeColor;
                lblFontDescription.Text = InitialFontLabelText;
            } else
            {
                lblFontDescription.ForeColor = NormalFontLabelForeColor;
                lblFontDescription.Text = FontDescriptionShort();
            }
        }

        protected void OpenFontDialog()
        {
            // Show the dialog.
            if (SelectedFont != null)
                fontDialog1.Font = SelectedFont;
            DialogResult result = fontDialog1.ShowDialog();
            // See if OK was pressed.
            if (result == DialogResult.OK)
            {
                // Get Font.
                SelectedFont = fontDialog1.Font;
                UpdateAppearance();
            }

        }

        #endregion Actions


        #region Events


        /// <summary>Event that is fired when the selected font changes.</summary>
        public event FontEventHandler FontSelected;

        /// <summary>Event that occurs when the selected font gets defined (i.e., when it is set for the first time).</summary>
        public event FontEventHandler FontSelectedFirstTime;

        /// <summary>Triggers the <see cref="FontSelected"/> event. This occurs whenewer the selected font is changed, 
        /// or when it is set for the first time.</summary>
        protected void OnFontSelected(object sender, FontEventArgs eventArgs)
        {
            UpdateAppearance();
            if (FontSelected != null)
                FontSelected(sender, eventArgs);
        }

        /// <summary>Triggers the <see cref="FontSelectedFirstTime"/> event.
        /// This occurs when font is selected for the first time.</summary>
        protected void OnFontSelectedFirstTime(object sender, FontEventArgs eventArgs)
        {
            if (FontSelectedFirstTime != null)
                FontSelectedFirstTime(sender, eventArgs);
        }

        #endregion Events 




        private void btnOpenDialog_Click(object sender, EventArgs e)
        {
            OpenFontDialog();
        }

        private void lblFontDescription_Click(object sender, EventArgs e)
        {
            OpenFontDialog();
        }
    }



    /// <summary><see cref="EventArgs"/> class that contains font information.</summary>
    public class FontEventArgs : EventArgs
    {
        public FontEventArgs(Font font)
        { this.Font = font; }

        protected Font _font = null;
        public Font Font
        {
            get { return _font; }
            protected set { _font = value; }
        }
    }

    /// <summary><see cref="EventArgs"/> type for font related events, contains a font object.</summary>
    /// <param name="sender">Object that trigered event.</param>
    /// <param name="args">Event argumets that contain the related font object.</param>
    public delegate void FontEventHandler(object sender, FontEventArgs args);




}

