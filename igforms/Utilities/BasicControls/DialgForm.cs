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
    public partial class DialogForm : Form
    {
        public DialogForm()
        {
            InitializeComponent();

            this.WindowTitle = this.Title;  // window title is also set to title
        }

        public DialogControl MainControl
        {
            get { return messageControl1; } 
        }


        #region DataAndSettings


        /// <summary>Messag box title.</summary>
        public string Title
        {
            get { return MainControl.Title; }
            set {
                MainControl.Title = value;
                this.WindowTitle = value;  // window title is also set to this value
            }
        }

        /// <summary>Message box message text (display below title).</summary>
        public string Message
        {
            get { return MainControl.Message; }
            set { MainControl.Message = value; }
        }

        public new string Text
        {
            get { return MainControl.Text; }
            set { MainControl.Text = value; }
        }

        /// <summary>Gets or sets the window title.
        /// <para>This property is added because the <see cref="Text"/> property, which should be ingerited, is overridden and 
        /// has a different meaning in this form.</para></summary>
        public string WindowTitle
        {
            get { return base.Text; }
            set { base.Text = value; }
        }

        void Test()
        {

            var a = MainControl.ButtonResult;

        }

        /// <summary>If true then text is treated as password, i.e. for all charactes of the text the same system character is shown.
        /// <para>Both setter and getter update the text box' properties accordingly, so calling any of these ensures that
        /// the mode (password - hidden text / non-password - visible text) is reflecte correctly in behavior.</para></summary>
        public bool IsTextPassword
        {
            get { return MainControl.IsTextPassword; }
            set { MainControl.IsTextPassword = value; }
        }

        /// <summary>Whether text box (located below the message) is visible.</summary>
        public bool IsTextVisible
        {
            get { return MainControl.IsTextVisible; }
            set { MainControl.IsTextVisible = value; }
        }

        /// <summary>If true then <see cref="TextValue"/> property becoming non-null and non-empty string will 
        /// automatically cause the text control containing that text become visible.</summary>
        public bool IsTextCausesVisible
        {
            get { return MainControl.IsTextCausesVisible; }
            set { MainControl.IsTextCausesVisible = value; }
        }

        /// <summary>Whether text shown in the text box (located below the message) can be edited.</summary>
        public bool IsTextEditable
        {
            get { return MainControl.IsTextEditable; }
            set { MainControl.IsTextEditable = value; }
        }

        /// <summary>Whether text shown in the text box (located below the message) can be set programatically.</summary>
        public bool IsTextSettable
        {
            get { return MainControl.IsTextSettable; }
            set { MainControl.IsTextSettable = value; }
        }

        /// <summary>Whether setting text programmatically throws an <see cref="InvalidOperationException"/> in the case
        /// that this is not allowed (i.e., when the <see cref="IsTextSettable"/> property is false).</summary>
        public bool IsTextSetThrows
        {
            get { return MainControl.IsTextSetThrows; }
            set { MainControl.IsTextSetThrows = value; }
        }

        /// <summary>Whether the text box (shown below the message) is multline.</summary>
        public bool IsTextMultiLine
        {
            get { return MainControl.IsTextMultiLine; }
            set { MainControl.IsTextMultiLine = value; }
        }

        /// <summary>Whether the text box (shown below the message) is multline.</summary>
        public bool IsTextChangedEventOnValidationOnly
        {
            get { return MainControl.IsTextChangedEventOnValidationOnly; }
            set { MainControl.IsTextChangedEventOnValidationOnly = value; }
        }

        /// <summary>Gets or sets the text box width.</summary>
        public int TextBoxWidth
        {
            get { return MainControl.TextBoxWidth; }
            set { MainControl.TextBoxWidth = value; }
        }

        /// <summary>Gets or sets the text box height.</summary>
        public int TextBoxHeight
        {
            get { return MainControl.TextBoxHeight; }
            set { MainControl.TextBoxHeight = value; }
        }

        /// <summary>Text of the button that was last pressed on the control.</summary>
        public string ButtonResult
        {
            get { return MainControl.ButtonResult; }
        }


        /// <summary>Array of texts that appear on dialog box's buttons.
        /// <para>When set, corresponding buttons are created.</para></summary>
        /// <remarks>Setter also creates buttons that appear visually on the control, and sets their properties
        /// and events.
        /// <para>A small number of buttons with pre-defined text are considered special buttons. These buttons are created in advance,
        /// and any string appearing in this property that correspond to the <see cref="Button.Text"/> property of any of 
        /// these special buttons will not cause creation of a new button, but the existing special button will be associated 
        /// with the text, added to the appropriate panel control, and made visible. This makes possible that special behavior
        /// of these such special buttons is defined in advance (e.g. in this class' definitioon) and that this special behavior
        /// will not be affected by subsequent removals or additions of these buttons.</para></remarks>
        public string[] Buttons
        {
            get { return MainControl.Buttons; }
            set { MainControl.Buttons = value; }
        }

        #endregion DataAndSettings



    }  // class 


}
