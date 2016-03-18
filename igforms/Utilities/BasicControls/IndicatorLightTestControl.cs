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
    public partial class IndicatorLightTestControl : UserControl
    {
        public IndicatorLightTestControl()
        {
            InitializeComponent();
            comboDirection.Items.Clear();
            comboDirection.Items.Add(FlowDirection.LeftToRight);
            comboDirection.Items.Add(FlowDirection.TopDown);
            comboDirection.Items.Add(FlowDirection.RightToLeft);
            comboDirection.Items.Add(FlowDirection.BottomUp);
            CopyState();
            CopySettings();
        }

        protected IndicatorLight Indicator
        {
            get { return indicatorLight1; } }


        protected void CopyAll()
        {
            CopyState();
            CopySettings();
        }

        /// <summary>Copies indicator light's state to controls of the current control.
        /// <para>By state we just mean state that is meant to change during operation, i.e.
        /// mode of the light (like Off, OK, ..., IsBlinking).</para></summary>
        public void CopyState()
        {
            chkOff.Checked = Indicator.IsOff;
            chkOk.Checked = Indicator.IsOk;
            chkBusy.Checked = Indicator.IsBusy;
            chkError.Checked = Indicator.IsError;
            chkBlinking.Checked = Indicator.IsBlinking;
        }

        /// <summary>Copies indicator light's properties (without the state) to the state of this testing
        /// control.</summary>
        public void CopySettings()
        {
            txtLabelText.Text = Indicator.LabelText;

            chkThrowOnInvalid.Checked = Indicator.ThrowOnInvalidSwitch;
            chkHasOff.Checked = Indicator.HasOff;
            chkHasOk.Checked = Indicator.HasOk;
            chkHasBusy.Checked = Indicator.HasBusy;
            chkHasError.Checked = Indicator.HasError;

            chkOuterBorder.Checked = Indicator.BorderOut;
            chkLabelBorder.Checked = Indicator.BorderLabel;

            numOuterMargin.Value = Indicator.MarginOut;
            numOuterPadding.Value = Indicator.PaddingOut;
            numLabelMargin.Value = Indicator.MarginLabel;
            numLabelPadding.Value = Indicator.PaddingLabel;

            numBlinkTime.Value = (decimal) ((double)Indicator.BlinkIntervalMilliSeconds / 1000.0);
        }




        // EVENTS - STATE

        private void chkOff_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkOff.Checked)
                    Indicator.SetOff();
                else
                    Indicator.UnsetOff();
                CopyAll();
            }
            catch (Exception ex)
            {
                CopyAll();
                UtilForms.Reporter.ReportError("Exception was thrown when trying to change state.", ex);
#if __MonoCS__
#else
                throw;
#endif
            }
        }

        private void chkOk_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkOk.Checked)
                    Indicator.SetOk();
                else
                    Indicator.UnsetOk();
                CopyAll();
            }
            catch (Exception ex)
            {
                CopyAll();
                UtilForms.Reporter.ReportError("Exception was thrown when trying to change state.", ex);
#if __MonoCS__
#else
                throw;
#endif
            }
        }

private void chkBusy_CheckedChanged(object sender, EventArgs e)
        {
            try { 
                if (chkBusy.Checked)
                    Indicator.SetBusy();
                else
                    Indicator.UnsetBusy();
                CopyAll();
            }
            catch (Exception ex)
            {
                CopyAll();
                UtilForms.Reporter.ReportError("Exception was thrown when trying to change state.", ex);
#if __MonoCS__
#else
                throw;
#endif
            }
        }

        private void chkError_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkError.Checked)
                    Indicator.SetError();
                else
                    Indicator.UnsetError();
                CopyAll();
            }
            catch (Exception ex)
            {
                CopyAll();
                UtilForms.Reporter.ReportError("Exception was thrown when trying to change state.", ex);
#if __MonoCS__
#else
                throw;
#endif
            }
        }

        private void btnBlinkOnce_Click(object sender, EventArgs e)
        {
            Indicator.Blink(1);
        }

        private void btnBlinkTwice_Click(object sender, EventArgs e)
        {
            Indicator.Blink(2);
        }

        private void chkBlinking_CheckedChanged(object sender, EventArgs e)
        {
            Indicator.IsBlinking = chkBlinking.Checked;
            CopyAll();
        }


        // BEHAVIOR:


        private void chkThrowOnInvalid_CheckedChanged(object sender, EventArgs e)
        {
            Indicator.ThrowOnInvalidSwitch = chkThrowOnInvalid.Checked;
            CopyAll();
        }

        private void chkOuterBorder_CheckedChanged(object sender, EventArgs e)
        {
            Indicator.BorderOut = chkOuterBorder.Checked;
            CopyAll();
        }

        private void chkLabelBorder_CheckedChanged(object sender, EventArgs e)
        {
            Indicator.BorderLabel = chkLabelBorder.Checked;
            CopyAll();
        }

        private void chkHasOff_CheckedChanged(object sender, EventArgs e)
        {
            Indicator.HasOff = chkHasOff.Checked;
            CopyAll();
        }

        private void chkHasOk_CheckedChanged(object sender, EventArgs e)
        {
            Indicator.HasOk = chkHasOk.Checked;
            CopyAll();
        }

        private void chkHasBusy_CheckedChanged(object sender, EventArgs e)
        {
            Indicator.HasBusy = chkHasBusy.Checked;
            CopyAll();
        }

        private void chkHasError_CheckedChanged(object sender, EventArgs e)
        {
            Indicator.HasError = chkHasError.Checked;
            CopyAll();
        }

        private void txtLabelText_Validated(object sender, EventArgs e)
        {
            Indicator.LabelText = txtLabelText.Text;
            CopyAll();
        }

        private void txtLabelText_TextChanged(object sender, EventArgs e)
        {
            Indicator.LabelText = txtLabelText.Text;
        }

        private void btnLabelColor_Click(object sender, EventArgs e)
        {
            DialogResult result = colorDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                Indicator.ColorLabel = colorDialog1.Color;
                CopyAll();
            }
        }

        private void numOuterMargin_ValueChanged(object sender, EventArgs e)
        {
            Indicator.MarginOut = (int)numOuterMargin.Value;
            CopyAll();
        }

        private void numOuterPadding_ValueChanged(object sender, EventArgs e)
        {
            Indicator.PaddingOut = (int) numOuterPadding.Value;
            CopyAll();
        }

        private void numLabelMargin_ValueChanged(object sender, EventArgs e)
        {
            Indicator.MarginLabel = (int) numLabelMargin.Value;
            CopyAll();
        }

        private void numLabelPadding_ValueChanged(object sender, EventArgs e)
        {
            Indicator.PaddingLabel = (int)numLabelPadding.Value;
            CopyAll();
        }

        private void numBlinkTime_ValueChanged(object sender, EventArgs e)
        {
            Indicator.BlinkIntervalMilliSeconds = (int)(numBlinkTime.Value * 1000);
        }

        private void fontSelectorSimple1_FontSelected(object sender, FontEventArgs args)
        {
            Indicator.LabelFont = fontSelectorSimple1.SelectedFont;
        }

        private void btnBlinkYellow3_Click(object sender, EventArgs e)
        {
            Indicator.BlinkSpecial(Color.Yellow, 3);
        }

        private void btnBlinkError2x_Click(object sender, EventArgs e)
        {
            Indicator.BlinkError(2);
        }

        private void comboDirection_SelectedIndexChanged(object sender, EventArgs e)
        {
            FlowDirection direction = (FlowDirection)comboDirection.Items[comboDirection.SelectedIndex];
            Indicator.FlowDirection = direction;
        }

        private void comboDirection_Click(object sender, EventArgs e)
        {
            comboDirection.DroppedDown = true;
        }


        // EVENTS - SETTINGS







    }
}
