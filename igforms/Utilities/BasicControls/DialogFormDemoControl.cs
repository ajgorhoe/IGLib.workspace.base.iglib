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
    public partial class DialogFormDemoControl : UserControl
    {
        public DialogFormDemoControl()
        {
            InitializeComponent();

            CopyDataToDialogs();
            CopyButtonDataToDialogs();

            // Set up embedded dialog control's events: 
            dialogControl1.TextChanged += (sender, eventArgs) => {
                string currentText = dialogControl1.Text;
                if (!IsDialogFormActive)
                {
                    indicatorTextChanged.BlinkSpecial(Color.Yellow);
                }
                UpdateResults();
                //if (!txtText.Focused && currentText != txtText.Text)  // update back the text field
                //    txtText.Text = currentText;
            };
            dialogControl1.ButtonResultChanged += (sender, eventArgs) => {
                indicatorButtonResultChanged.BlinkSpecial(Color.LightBlue);
                UpdateResults();
            };
            dialogControl1.ButtonPressed += (sender, eventArgs) => {
                indicatorButtonPressed.BlinkSpecial(Color.LightBlue);
                UpdateResults();
            };


            UpdateResults();
        }

        /// <summary>Creates a new testing dialog window.</summary>
        public void CreateNewDialogWindow()
        {
            if (dialogForm1 != null)
            {
                dialogForm1.Close();
            }
            dialogForm1 = new DialogForm();

            // Set up fomr's events:
            dialogForm1.MainControl.TextChanged += (sender, eventArgs) => {
                string currentText = dialogControl1.Text;
                indicatorTextChanged.BlinkError();
                UpdateResults();
                //if (!txtText.Focused && currentText != txtText.Text)  // update back the text field
                //    txtText.Text = currentText;
            };
            dialogForm1.MainControl.ButtonResultChanged += (sender, eventArgs) => {
                indicatorButtonResultChanged.BlinkOk();
                UpdateResults();
            };
            dialogForm1.MainControl.ButtonPressed += (sender, eventArgs) => {
                indicatorButtonPressed.BlinkSpecial(Color.Yellow);
                UpdateResults();
            };

            CopyDataToDialogs();
            CopyButtonDataToDialogs();
            dialogForm1.Show();
            UpdateResults();
        }


        /// <summary>Returns true if the dialog form is active, i. e. if it is located, visible, and not yet disposed.</summary>
        /// <remarks>This is used e.g. in cases where functions of the dialog form can be taken over by the embedded dialog 
        /// control when the former is not active (such as events, for example).</remarks>
        public bool IsDialogFormActive
        {
            get {
                return (dialogForm1 != null && dialogForm1.Visible && !dialogForm1.IsDisposed);
            }
        }

        /// <summary>Updates controls that stow dialog's results.</summary>
        protected void UpdateResults()
        {
            string resultButton = null;
            string resultText = null;
            if (dialogForm1 != null && dialogForm1.Visible && !dialogForm1.IsDisposed)
            {
                resultButton = dialogForm1.ButtonResult;
                resultText = dialogForm1.Text;
            }
            else
            {
                if (dialogControl1.IsDisposed)
                    throw new InvalidOperationException("Embedded dialog control is DISPOSED!");
                resultButton = dialogControl1.ButtonResult;
                resultText = dialogControl1.Text;
            }
            lblResultButton.Text = resultButton;
            lblResultText.Text = resultText;
            if (!txtText.Focused && resultText != txtText.Text)  // update back the text field
                txtText.Text = resultText;
        }


        /// <summary>Applies all data and settings forom the current control to the embedded and window test dialogs.
        /// <para>This does not copy the buttons data, which is performed specially by the <see cref="CopyButtonDataToDialogs"/> 
        /// function.</para></summary>
        public void CopyDataToDialogs() 
        {
            indicatorSettingsApplied.BlinkOk(1);

            // Copy data and settings to embedded control:
            dialogControl1.IsTextVisible = chkTextVisible.Checked;
            dialogControl1.IsTextCausesVisible = chkTextCausesVisible.Checked;
            dialogControl1.IsTextEditable = chkTextEditable.Checked;
            dialogControl1.IsTextMultiLine = chkTextMultiLine.Checked;
            dialogControl1.IsTextSettable = chkTextSettable.Checked;
            dialogControl1.IsTextSetThrows = chkTextSetExceptions.Checked;
            dialogControl1.IsTextChangedEventOnValidationOnly = !chkTextImmediate.Checked;
            dialogControl1.IsTextPassword = chkTextPassword.Checked;

            dialogControl1.Title = txtTitle.Text;
            dialogControl1.Message = txtMessage.Text;
            dialogControl1.Text = txtText.Text;

            dialogControl1.TextBoxWidth = (int)numTextBoxWidth.Value;
            dialogControl1.TextBoxHeight = (int)numTextBoxHeight.Value;

            if (dialogForm1 != null && !dialogForm1.IsDisposed)
            {
                // Copy data and settings to the window dialog:
                dialogForm1.IsTextVisible = chkTextVisible.Checked;
                dialogForm1.IsTextCausesVisible = chkTextCausesVisible.Checked;
                dialogForm1.IsTextEditable = chkTextEditable.Checked;
                dialogForm1.IsTextMultiLine = chkTextMultiLine.Checked;
                dialogForm1.IsTextSettable = chkTextSettable.Checked;
                dialogForm1.IsTextSetThrows = chkTextSetExceptions.Checked;
                dialogForm1.IsTextChangedEventOnValidationOnly = !chkTextImmediate.Checked;
                dialogForm1.IsTextPassword = chkTextPassword.Checked;

                dialogForm1.Title = txtTitle.Text;
                dialogForm1.Message = txtMessage.Text;
                dialogForm1.Text = txtText.Text;

                dialogForm1.TextBoxWidth = (int)numTextBoxWidth.Value;
                dialogForm1.TextBoxHeight = (int)numTextBoxHeight.Value;
            }
        }

        /// <summary>Applies all data and settings forom the current control to the embedded and window test dialogs.
        /// <para>This does not copy the buttons data, which is performed specially by the <see cref="CopyButtonDataToDialogs"/> 
        /// function.</para></summary>
        public void CopyButtonDataToDialogs()
        {
            indicatorSettingsApplied.BlinkBusy(3);
            string buttonsString = textBotxtButtons.Text;
            string[] buttonTexts = buttonsString.Split('\n');
            for (int i = 0; i < buttonTexts.Length; ++i)
            {
                buttonTexts[i] = buttonTexts[i].Trim();
                if (string.IsNullOrEmpty(buttonTexts[i]))
                    buttonTexts[i] = "<< ERROR: empty string >>";
            }
            dialogControl1.Buttons = buttonTexts;
            if (dialogForm1 != null)
                dialogForm1.Buttons = buttonTexts;
        }



        private void txtTitle_TextChanged(object sender, EventArgs e)
        {
            if (chkImmediateText.Checked)
                CopyDataToDialogs();
        }

        private void txtTitle_Validated(object sender, EventArgs e)
        {
            CopyDataToDialogs();
        }

        private void txtMessage_TextChanged(object sender, EventArgs e)
        {
            if (chkImmediateText.Checked)
                CopyDataToDialogs();
        }

        private void txtMessage_Validated(object sender, EventArgs e)
        {
            CopyDataToDialogs();
        }


        private void txtText_TextChanged(object sender, EventArgs e)
        {
            if (/* txtText.Focused && */ chkImmediateText.Checked)
                CopyDataToDialogs();
        }


        private void txtText_Validated(object sender, EventArgs e)
        {
            CopyDataToDialogs();
        }

        private void textBotxtButtons_TextChanged(object sender, EventArgs e)
        {
            // CopyButtonDataToDialogs();
        }

        private void textBotxtButtons_Validated(object sender, EventArgs e)
        {
            CopyButtonDataToDialogs();
        }

        private void chkImmediateText_CheckedChanged(object sender, EventArgs e)
        {  }

        private void btnLaunchDialog_Click(object sender, EventArgs e)
        {
            CreateNewDialogWindow();
            CopyDataToDialogs();
            UpdateResults();
        }

        private void btnRefreshDialog_Click(object sender, EventArgs e)
        {
            if (dialogForm1 == null)
                CreateNewDialogWindow();
            if (dialogControl1.IsDisposed)
                CreateNewDialogWindow();
            if (!dialogForm1.Visible)
                dialogForm1.Visible = true;
            dialogForm1.Show();
            // dialogForm1.TopMost = true;
            dialogForm1.Focus();
            CopyDataToDialogs();
            CopyButtonDataToDialogs();
            UpdateResults();
        }

        private void btnRefreshText_Click(object sender, EventArgs e)
        {
            UpdateResults();
        }

        private void chkTextVisible_CheckedChanged(object sender, EventArgs e)
        {
            CopyDataToDialogs();
        }

        private void chkTextCausesVisible_CheckedChanged(object sender, EventArgs e)
        {
            CopyDataToDialogs();
        }

        private void chkTextEditable_CheckedChanged(object sender, EventArgs e)
        {
            CopyDataToDialogs();
        }

        private void chkTextSettable_CheckedChanged(object sender, EventArgs e)
        {
            CopyDataToDialogs();
        }

        private void chkTextSetExceptions_CheckedChanged(object sender, EventArgs e)
        {
            CopyDataToDialogs();
        }

        private void chkTextMultiLine_CheckedChanged(object sender, EventArgs e)
        {
            CopyDataToDialogs();
        }

        private void chkTextImmediate_CheckedChanged(object sender, EventArgs e)
        {
            CopyDataToDialogs();
        }

        private void chkTextPassword_CheckedChanged(object sender, EventArgs e)
        {
            CopyDataToDialogs();
        }

        private void numTextBoxWidth_Validated(object sender, EventArgs e)
        {
            CopyDataToDialogs();
        }

        private void numTextBoxHeight_Validated(object sender, EventArgs e)
        {
            CopyDataToDialogs();
        }


    }
}
