using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using IG.Lib;
using System.Threading;

namespace IG.Forms
{

    /// <summary>Highly configurable interactive message box with custom buttons, message, and possibly editable
    /// text.</summary>
    /// <remarks>
    /// <para>This message box display an optional title, message, additional text that can be edited (dpeendent on settings) and
    /// optional buttons. Any of these components can be withdrawn (hidden). Control adapts its size with accordance to what is displayed.</para>
    /// <para>Title displays contents of the <see cref="Title"/> string on the top of the message box. If that
    /// property is an empty or null string then the appropriate label is not displayed and control is resized accordingly.</para>
    /// <para>Contents of the <see cref="Message"/> property is displayed below title. If set to null or empty string then the
    /// message box shrinks accordingly because the corresponding label is not visible.</para>
    /// <para></para>
    /// <para></para>
    /// </remarks>
    public partial class DialogControl : UserControl
    {

        /// <summary>Creates a new message box.</summary>
        public DialogControl()
        {
            InitializeComponent();

            // Buttons: 
            try
            {
                foreach (Button btn in PredefinedButtons)
                {
                    btn.Visible = false;
                    if (pnlButtons.Controls.Contains(btn))
                        pnlButtons.Controls.Remove(btn);
                }
            }
            catch { }
            UpdateTextVisibility();
            bool a = IsTextPassword;  // this will update all nexessary in the case we are inserting passwords
        }

        /// <summary>Creates a new message box with specific properties.</summary>
        /// <param name="title">Message box' title.</param>
        /// <param name="message">Message box' message.</param>
        /// <param name="initialText">Initial message box text.</param>
        /// <param name="isTextVisible">Whether message box contains text.</param>
        /// <param name="buttons">Aray of strings that are displayed on message box's buttons.
        /// <para>If any of the buttons is pressed, the <see cref="ButtonResult"/> property contains the corresponding string 
        /// from this argument.</para></param>
        public DialogControl(string title, string message, string initialText, bool isTextVisible, params string[] buttons):
            this()
        {
            this.Title = title;
            this.Message = message;
            this.IsTextVisible = IsTextVisible;
            this.SetText(initialText);
            this.Buttons = buttons;
        }



        #region Data

        private string _title = null;

        private string _message = null;

        private string _text = null;

        /// <summary>Message box' title, displayet on the top.
        /// <para>If null lor empty string then the corresponding controls are not displayed.</para></summary>
        public string Title
        {
            get { return _title; }
            set {
                if (value != _title)
                {
                    _title = value;
                    lblTitle.Visible = pnlTitle.Visible = !string.IsNullOrEmpty(_title);
                    lblTitle.Text = _title;
                }
            }
        }

        /// <summary>Message box' message, display under the title.
        /// <para>If null lor empty string then the corresponding controls are not displayed.</para></summary>
        public string Message
        {
            get { return _message; }
            set {
                if (value != _message)
                {
                    _message = value;
                    lblMessage.Visible = pnlMessage.Visible = !string.IsNullOrEmpty(_message);
                    lblMessage.Text = _message;
                }
            }
        }

        /// <summary>Sets the dialog's text (which is considered its returned value) to the specified string.
        /// <para>This method should always be used internally, while <see cref="Text"/> property's setter is 
        /// intended for use only by external callers.</para>
        /// <para>Reason for the above is that this method does not throw when setting text programatically is not allowed.</para></summary>
        /// <param name="text"></param>
        /// <param name="changeControlTextUnconditionally">If true then text in the text box control is changed unconditionally, 
        /// even if the text box has focus (this must be used when triggered by external callers). If false then tect in the text 
        /// box control is cnaged only if the text box  does not have focus (this should be used internally).</param>
        protected virtual void SetText(string text, bool changeControlTextUnconditionally = false)
        {
            if (text != _text)
            {
                _text = text;
                if (IsTextCausesVisible && !string.IsNullOrEmpty(_text))
                    IsTextVisible = true;
                if (changeControlTextUnconditionally || !txtText.Focused  /* prevent setting when typing (as this changes cursor position) */)
                {
                    txtText.Text = text;
                }
                OnTextChanged();
            }
        }

        /// <summary>Value of the possibly editable text of the message box.
        /// <para>Setter should only be used by external callers; internally SetText() should be used.</para></summary>
        public new string Text
        {
            get {
                if (this.HasTextChangesUnnoticed)
                    SetText(txtText.Text);
                return _text;
            }
            set {
                if (value != _text)
                {
                    if (IsTextSettable)
                    {
                        SetText(value, true /* changeControlTextUnconditionally */);
                    } else
                    {
                        // Text can not be changed programmatically...
                        if (IsTextSetThrows)
                            throw new InvalidOperationException("Invalid attempt: text can not be set on this dialog control.");
                    }
                }
            }
        }

        /// <summary>Proxy for accessing the base class' Text property, since this property is
        /// redefined in the current class and is given another meaning.</summary>
        public string ControlText
        {
            get { return base.Text; }
            set { base.Text = value; }
        }


        private bool _isTextPassword = false;

        /// <summary>If true then text is treated as password, i.e. for all charactes of the text the same system character is shown.
        /// <para>Both setter and getter update the text box' properties accordingly, so calling any of these ensures that
        /// the mode (password - hidden text / non-password - visible text) is reflecte correctly in behavior.</para></summary>
        public bool IsTextPassword
        {
            get
            {
                if (_isTextPassword)
                {
                    txtText.Multiline = false;
                    txtText.UseSystemPasswordChar = true;
                }
                else
                {
                    txtText.UseSystemPasswordChar = false;
                }
                return _isTextPassword;
            }
            set {
                _isTextPassword = value;
                if (_isTextPassword)
                {
                    txtText.Multiline = false;
                    txtText.UseSystemPasswordChar = true;
                }
                else
                {
                    txtText.UseSystemPasswordChar = false;
                }
                menuShowPasswordShortly.Visible = _isTextPassword;
            }
        }

        private bool _isTextVisible = false;

        /// <summary>Specifies whether text box (displaying contents of the <see cref="Text"/> property) 
        /// is visible or not.</summary>
        public bool IsTextVisible
        {
            get { return _isTextVisible; }
            set {
                if (value != _isTextVisible)
                {
                    _isTextVisible = value;
                    UpdateTextVisibility();

                }
            }
        }

        private bool _textCausesVisible = false;

        /// <summary>If true then <see cref="TextValue"/> property becoming non-null and non-empty string will 
        /// automatically cause the text control containing that text become visible.
        /// <para>Default should be false isn most cases (e.g. when used in a window or when embedded).</para></summary>
        public bool IsTextCausesVisible
        {
            get { return _textCausesVisible; }
            set {
                _textCausesVisible = value;
                UpdateTextVisibility();
            }
        }

        /// <summary>Updates control according to the intended visibility of the text box.</summary>
        protected void UpdateTextVisibility()
        {
            txtText.Visible = IsTextVisible || (IsTextCausesVisible && !string.IsNullOrEmpty(Text));
            menuText.Visible = IsTextVisible;
            menuTextVisible.Checked = txtText.Visible;
        }


        private bool _isTextEditable = true; 

        /// <summary>Specifies whether text can be edited or not.</summary>
        public bool IsTextEditable
        {
            get { return _isTextEditable; }
            set {
                if (value != _isTextEditable)
                {
                    _isTextEditable = value;
                    txtText.ReadOnly = !_isTextEditable;
                    if (_isTextEditable)
                    {
                        txtText.BackColor = Color.White;
                    }
                    else
                    {
                        txtText.BackColor = Color.LightGray;
                    }
                }
            }
        }

        private bool _isTextSettable = true;

        /// <summary>Specifies whether text can be set programatically or not.</summary>
        public bool IsTextSettable
        {
            get { return _isTextSettable; }
            set {
                if (value != _isTextSettable)
                {
                    _isTextSettable = value;
                }
            }
        }

        private bool _isTextSetThrows = true;

        /// <summary>Whether setting text programmatically throws an <see cref="InvalidOperationException"/> in the case
        /// that this is not allowed (i.e., when the <see cref="IsTextSettable"/> property is false).</summary>
        public bool IsTextSetThrows
        {
            get { return _isTextSetThrows; }
            set { _isTextSetThrows = value; }
        } 

        private bool _isTextMultiLine = true;

        /// <summary>Specifies whether text box is multiline.</summary>
        public bool IsTextMultiLine
        {
            get { return _isTextMultiLine; }
            set {
                if (value != _isTextMultiLine)
                {
                    _isTextMultiLine = value;
                    txtText.Multiline = _isTextMultiLine && _isTextPassword;
                }
            }
        }

        /// <summary>Gets or sets the text box width.</summary>
        public int TextBoxWidth
        {
            get { return txtText.Width; }
            set
            {
                if (value < txtText.MinimumSize.Width)
                    txtText.Height = txtText.MinimumSize.Width;
                else
                    txtText.Width = value;
            }
        }

        /// <summary>Gets or sets the text box height.</summary>
        public int TextBoxHeight
        {
            get { return txtText.Height; }
            set
            {
                if (value < txtText.MinimumSize.Height)
                    txtText.Height = txtText.MinimumSize.Height;
                else
                    txtText.Height = value;
            }
        }


        #region Data.Buttons

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
            get { return ButtonsList.ToArray(); }
            set {
                if (value == null)
                    RemoveAllButtons();
                else if (value.Length == 0)
                    RemoveAllButtons();
                else
                {
                    bool equalToExisting = true;
                    if (value.Length != ButtonsList.Count)
                        equalToExisting = false;
                    else
                    {
                        for (int i = 0; i < value.Length; ++i)
                        {
                            if (value[i] != ButtonsList[i])
                            {
                                equalToExisting = false;
                                break;
                            }
                        }
                    }
                    if (!equalToExisting)
                    {
                        RemoveAllButtons();
                        for (int i = 0; i < value.Length; ++ i)
                        {
                            AddButton(value[i]);
                        }
                    }
                }
            }
        }

        Button[] _predefinedButtons = null;

        /// <summary>List of pre-defined buttons.</summary>
        protected Button[] PredefinedButtons
        {
            get
            {
                if (_predefinedButtons == null)
                {
                    _predefinedButtons = new Button[] { btnOk, btnCancel, btnClose };
                    foreach(Button btn in _predefinedButtons)
                    {
                        btn.Click += ButtonClickHandler;
                        btn.Click += ButtonClickHandlerPredefined;
                    }
                }
                return _predefinedButtons;
            }
        }

        /// <summary>Returns array of predefined button's texts.</summary>
        public IEnumerable<string> PredefinedButtonTexts
        {
            get {
                var sel = from btn in PredefinedButtons
                          select btn.Text;
                return sel.ToArray();
            }
        }

        /// <summary>Returns true if the specified string is a pre-defined button's text (case sensitive).</summary>
        /// <param name="buttonText">String that is checked to be a predefined button text.</param>
        public bool IsPredefinedButtonText(string buttonText)
        {
            return PredefinedButtonTexts.Contains(buttonText);
        }

        /// <summary>Returns the pre-defined button whose text corresponds to the specified text.
        /// <para>If the text does not correspond to any pre-defined button then null is returned.</para></summary>
        /// <param name="buttonText">Butotn's text.</param>
        protected Button GetPredefinedButton(string buttonText)
        {
            foreach(Button btn in PredefinedButtons)
            {
                if (btn.Text == buttonText)
                {
                    return btn;
                }
            }
            return null;
        }


        private bool _isTextChangedOnVaalidationOnly = false;

        /// <summary>If true then the <see cref="TextChanged"/> event is fired only when the text obtained from the text box is validated.
        /// If true then the event fires every time the Text Box control's where text can be edited is fired.</summary>
        /// <remarks>The </remarks>
        public bool IsTextChangedEventOnValidationOnly
        {
            get { return _isTextChangedOnVaalidationOnly; }
            set { _isTextChangedOnVaalidationOnly = value; }
        }

        private bool _isTextChangedUnnoticed = false;

        /// <summary>Indicates whether text has been changed in the text box control but the <see cref="TextChanged"/> event
        /// has not been fired on the current compound control.</summary>
        /// <remarks>This situation appears when <see cref="IsTextChangedEventOnValidationOnly"/> is true, and not each change in 
        /// the text box' textt fire the control's <see cref="TextChanged"/> event.</remarks>
        public bool HasTextChangesUnnoticed
        {
            get { return _isTextChangedUnnoticed; }
            protected set { _isTextChangedUnnoticed = value; }
        }


        private List<string> _buttonTexts = new List<string>();

        protected List<string> ButtonsList { get { return _buttonTexts; } } 

        /// <summary>Returns true if buttons contained in the message box contain a button with the 
        /// specified text (case sensitive), false if not.</summary>
        /// <param name="buttonText">Button text against which button existence is checked.</param>
        public bool ContainsButton(string buttonText)
        {
            return ButtonsList.Contains(buttonText);
        }

        /// <summary>Returns the <see cref="Button"/> contained on the message box whose text corresponds to the 
        /// specified text (case sensitve), or null, if there is no such button.</summary>
        /// <param name="buttonText">Button text by which the corresponding button is searched for.</param>
        protected Button GetButton(string buttonText)
        {
            foreach (Button btn in pnlButtons.Controls)
            {
                if (btn.Text == buttonText)
                    return btn;
            }
            return null;
        }


        protected string ButtonTextOk
        {
            get { return btnOk.Text; }
            set { btnOk.Text = value; } }
         
        protected string ButtonTextCancel
        {
            get { return btnCancel.Text; }
            set { btnCancel.Text = value; }
        }

        protected string ButtonTextClose
        {
            get { return btnClose.Text; }
            set { btnClose.Text = value; }
        }

        /// <summary>Adds a button with the specified button text to the list of buttons of the current message box.</summary>
        /// <param name="buttonText">Text of the button to be added.
        /// <para>If this is the text of one of the predefined text buttons, then the corresponding pre-defined button is
        /// associated with this. Otherwise, a new button is created.</para>
        /// <para>A button is added to the set of visible buttons that are contained in the buttons panel, and is set visible.</para></param>
        public void AddButton(string buttonText)
        {
            if (string.IsNullOrEmpty(buttonText))
                throw new ArgumentException("Message box' button text is not defined (null or empty string).");
            pnlButtons.Visible = true;
            Button btn = null;
            btn = GetButton(buttonText);
            if (btn != null)
                btn.Visible = true;
            else
            {
                btn = GetPredefinedButton(buttonText);
                if (btn == null)
                {
                    btn = new Button();
                    btn.Text = buttonText;
                    btn.Visible = true;
                    btn.Height = btnOk.Height;
                    btn.Font = btnOk.Font;
                    btn.MinimumSize = btnOk.MinimumSize;
                    btn.AutoSize = true;
                    // Add event handler:
                    btn.Click += ButtonClickHandler;
                }
                btn.Visible = true;
                if (!pnlButtons.Controls.Contains(btn))
                    pnlButtons.Controls.Add(btn);
            }
            if (!ButtonsList.Contains(buttonText))
                ButtonsList.Add(buttonText);
        }


        /// <summary>Removes thebutton with the specified tbutton text from the current message box.</summary>
        /// <param name="buttonText"></param>
        public void RemoveButton(string buttonText)
        {
            if (string.IsNullOrEmpty(buttonText))
                throw new ArgumentException("Can not remove a button: button text not specified (null or empty string).");
            Button btn = GetButton(buttonText);
            if (btn != null)
                pnlButtons.Controls.Remove(btn);
            if (!ButtonsList.Contains(buttonText))
                ButtonsList.Remove(buttonText);
            if (pnlButtons.Controls.Count < 1)
                pnlButtons.Visible = false;
        }

        /// <summary>Removes all buttons from the message box.</summary>
        public void RemoveAllButtons()
        {
            foreach (string buttonText in ButtonsList)
            {
                RemoveButton(buttonText);
            }
        }

        private string _buttonResult = null;

        /// <summary>Text of the last button pressed on the current form. This can be used by creator of the dialog
        /// in order to be informes about which button was pressed by the user.</summary>
        public string ButtonResult
        {
            get { return _buttonResult; }
            protected set {
                if (value != _buttonResult)
                {
                    _buttonResult = value;
                    OnButtonResultChanged();
                }
            }
        }


        #endregion Data.Buttons


        #endregion Data



        #region Events

        /// <summary>This event is fired each time the text is changed on the current control.</summary>
        /// <remarks>If the </remarks>
        public new event EventHandler TextChanged;
        
        /// <summary>Fires the <see cref="TextChanged"/> event.</summary>
        protected void OnTextChanged()
        {
            if (IsTextCausesVisible)
                UpdateTextVisibility();
            if (TextChanged != null)
                TextChanged(this, EventArgs.Empty);
            HasTextChangesUnnoticed = false;  // changes of tect box contents have been noted
        }

        /// <summary>This event is fired each time button result changes, which happens when a button is 
        /// pressed on the dialog.</summary>
        public event EventHandler ButtonResultChanged;

        /// <summary>Fires the <see cref="ButtonResultChanged"/> event.</summary>
        protected void OnButtonResultChanged()
        {
            if (ButtonResultChanged != null)
                ButtonResultChanged(this, EventArgs.Empty);
        }

        /// <summary>This eventt is fired each time the dialog button is pressed.</summary>
        /// <remarks>The event has similar role as the <see cref="ButtonResultChanged"/>, however that event does not get fired 
        /// each time this evend does. If the same button is pressed twice in a row, for example, the <see cref="ButtonResultChanged"/>
        /// event is not fired the second time (because the <see cref="ButtonResult"/> remains the same as after the first 
        /// button press and is therefore not changed), but this event does get fired both times. It is important to have separate
        /// events because button press can have important implications (e.g. the dialog goes invisible) even if the  <see cref="ButtonResult"/>
        /// property does not change because of this (e.g. because the same value resulted from previous press on the same button).</remarks>
        public event EventHandler ButtonPressed;
        
        
        /// <summary>Fires the <see cref="ButtonPressed"/> event.</summary>
        protected void OnButtonPressed(object sender)
        {
            Button pressedButton = sender as Button;
            if (pressedButton != null)
            {
                ButtonResult = pressedButton.Text;
            }
            if (ButtonPressed!= null)
            {
                ButtonPressed(this, EventArgs.Empty);
            }
        }



        #endregion Events



        public const double DefaultZoomFactor = 1.2;

        private double _zoomFactor = DefaultZoomFactor;


        /// <summary>Zoom factor, factor by which controls are enlarged or shrinked when zoomed in or out.
        /// <para>Setter takes care that the value is greater than 1. If i</para></summary>
        public double ZoomFactor
        {
            get { return _zoomFactor; }
            set {
                if (value == 1.0 || value <= 0)
                    value = DefaultZoomFactor;
                else if (value <= 1.0)
                    value = 1 / value;
                _zoomFactor = value;
            } }


        /// <summary>Minimal zoom factor.</summary>
        protected readonly double MinZoomFactor = 0.25;

        /// <summary>Maximal zoom factor.</summary>
        protected readonly double MaxZoomFactor = 4;

        /// <summary>Zooms the text box by the specified factor.</summary>
        /// <param name="zoomFactor">Factor by which size of the text box is changed (enlarged or shrinked).
        /// <para>If smaller than <see cref="MinZoomFactor"/> or larger than <see cref="MaxZoomFactor"/> then the corresponding of
       /// these two bounds is taken.</para></param>
        public void ZoomTextBox(double zoomFactor)
        {
            if (zoomFactor < MinZoomFactor)
                zoomFactor = MinZoomFactor;
            else if (zoomFactor > MaxZoomFactor)
                zoomFactor = MaxZoomFactor;
            if (zoomFactor != 1.0)
            {
                txtText.Width = (int)(zoomFactor * (double)txtText.Width);
                txtText.Height = (int)(zoomFactor * (double)txtText.Height);
            }
        }


        /// <summary>Zooms the text box in (enlarges it) by the specified factor.</summary>
        /// <param name="zoomFactor">Factor by which text box is enlarged.
        /// <para>If less shan 1 then <see cref="ZoomFactor"/> is taken. Default is 0 which implies this factor.</para>
        /// <para><see cref="MaxZoomFactor"/> is taken as upper bound.</para></param>
        public void ZoomInTextBox(double zoomFactor = 0)
        {
            if (zoomFactor < 1.0)
                zoomFactor = this.ZoomFactor;
            ZoomTextBox(zoomFactor);
        }


        /// <summary>Zooms the text box out (shrinks it) by the specified factor.</summary>
        /// <param name="zoomFactor">Factor by which text box is enlarged.
        /// <para>If less shan 1 then <see cref="ZoomFactor"/> is taken. Default is 0 which implies this factor.</para>
        /// <para><see cref="MaxZoomFactor"/> is taken as upper bound.</para></param>
        public void ZoomOutTextBox(double zoomFactor = 0)
        {
            if (zoomFactor < 1.0)
                zoomFactor = this.ZoomFactor;
            zoomFactor = 1.0 / zoomFactor;
            ZoomTextBox(zoomFactor);
        }



        protected void CloseForm()
        {
            this.Visible = false;
        }

        /// <summary>General event handler that is executed on button click for all dialog buttons.</summary>
        protected void ButtonClickHandler(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            if (btn!= null)
            {
                ButtonResult = btn.Text;
                OnButtonPressed(sender);
            }
        }


        // Predefined buttons:

        /// <summary>Event handler that is executed on button click for all pre-defined buttons (such as <see cref="ButtonTextOk"/> or <see cref="ButtonTextCancel"/>).
        /// <para>In addition to this, each pre-defined button can also have its own specific handlers added.</para></summary>
        protected void ButtonClickHandlerPredefined(object sender, EventArgs e)
        {

        }


        private void btnOk_Click(object sender, EventArgs e)
        {
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
        }


        private void btnClose_Click(object sender, EventArgs e)
        {
            this.CloseForm();
        }


        // Context menus:

        private void menuCopyMessage_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(lblMessage.Text, TextDataFormat.Text);
        }

        private void menuCopyTitleAndMessage_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(lblTitle.Text + Environment.NewLine + Environment.NewLine + lblMessage.Text + Environment.NewLine, 
                TextDataFormat.Text);
        }

        private void menuClose_Click(object sender, EventArgs e)
        {
            this.CloseForm();
        }

        private void menuCopyText_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(txtText.Text, TextDataFormat.Text);
        }

        private void menuZoomInTextBox_Click(object sender, EventArgs e)
        {
            ZoomInTextBox();
        }

        private void menuZoomOutTextBox_Click(object sender, EventArgs e)
        {
            ZoomOutTextBox();
        }

        private void menuZoomInText_Click(object sender, EventArgs e)
        {
            ZoomInTextBox();
        }

        private void menuZoomOutText_Click(object sender, EventArgs e)
        {
            ZoomOutTextBox();
        }

        private void menuTextVisible_CheckedChanged(object sender, EventArgs e)
        {
            this.txtText.Visible = menuTextVisible.Checked;
        }

        /// <summary>Returns a string describing state of the current message box.</summary>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if (Title == null)
                sb.AppendLine("Title is null.");
            else
                sb.AppendLine("Title: \"" + Title + "\"");
            if (Message == null)
                sb.AppendLine("Message is null.");
            else if (Message.Length == 0)
                sb.AppendLine("Message: \"\"");
            else
                sb.AppendLine("Message: " + Environment.NewLine + "\"" + Message + "\".");
            sb.AppendLine();
            if (Text == null)
                sb.AppendLine("Text is null.");
            else if (Text.Length == 0)
                sb.AppendLine("Text is an empty string.");
            else
                sb.AppendLine("Text length: " + Text.Length + ".");
            if (IsTextVisible)
                sb.AppendLine("Text box is VISIBLE.");
            else
            {
                sb.AppendLine("Text box is NOT visible.");
                if (IsTextCausesVisible)
                {
                    sb.AppendLine("  Nonempty text will make text box visible.");
                }
            }
            sb.AppendLine("  Actual visibility of text box: " + txtText.Visible);
            if (IsTextEditable)
                sb.AppendLine("Text is EDITABLE.");
            else
                sb.AppendLine("Text can NOT be edited.");
            if (IsTextSettable)
                sb.AppendLine("Text is SETTABLE programatically.");
            else
            {
                sb.AppendLine("Text can NOT be set programatically.");
                if (IsTextSetThrows)
                    sb.AppendLine("  Attempt to change text programmatically will THROW exception.");
                else
                    sb.AppendLine("  Attempt to change text programmatically will have no effect.");
            }
            if (IsTextMultiLine)
                sb.AppendLine("Text box is multi line.");
            else
                sb.AppendLine("Text box is single line.");
            sb.AppendLine();
            if (ButtonsList.Count < 1)
                sb.AppendLine("There are no Buttons.");
            else
            {
                sb.Append("Buttons: ");
                int numButtons = ButtonsList.Count;
                for (int i = 0; i < numButtons; ++i)
                {
                    sb.Append("\"" + ButtonsList[i] + "\"");
                    if (i < numButtons - 1)
                        sb.Append(",");
                }
                sb.AppendLine(); // bor buttons
            }
            return sb.ToString();
            
        }

        /// <summary>Launches a fading message that summarizes the state of the current message box.</summary>
        private void menuSummary_Click(object sender, EventArgs e)
        {
            FadingMessage msg = new FadingMessage("Message Box Summary", 
                "State of the message box: " + Environment.NewLine + this.ToString(), 
                4000, 0.3, false /* doLaunch */);
            msg.Launch(false);
        }


        private void txtText_Validated(object sender, EventArgs e)
        {
            this.SetText(this.txtText.Text);
        }

        private void txtText_TextChanged(object sender, EventArgs e)
        {
            if (IsTextChangedEventOnValidationOnly)
            {
                HasTextChangesUnnoticed = true;  // note that text has changed
            } else
            {
                this.SetText(this.txtText.Text); ;  // fire the event. 
            }
        }

        private void menuShowPasswordShortly_Click(object sender, EventArgs e)
        {
            if (IsTextPassword)
            {
                try
                {
                    // temporarily show the password characters:
                    txtText.UseSystemPasswordChar = false;
                    Thread.Sleep(800);
                }
                finally
                {
                    bool a = IsTextPassword;  // this will restore the state and hide password text
                }
            }
        }
    }
}
