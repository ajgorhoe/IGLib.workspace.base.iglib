// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using IG.Lib;


namespace IG.Forms
{
    public partial class HashDirControl : UserControl
    {
        public HashDirControl()
        {
            InitializeComponent();
        }

        #region Notifications 

        void ShowNotification(string title, string message, ReportType type = ReportType.Info)
        {
            notificationFrame.ShowNotification(title, message, type);
        }

        #endregion Notifications


        private void txtTitle_TextChanged(object sender, EventArgs e)
        {
            
        }


        ReportType _reportType = ReportType.Info;

        private void btnLaunchNotification_Click(object sender, EventArgs e)
        {

            ShowNotification(txtTitle.Text, txtMessage.Text, _reportType);
            if (_reportType == ReportType.Info)
                _reportType = ReportType.Warning;
            else if (_reportType == ReportType.Warning)
                _reportType = ReportType.Error;
            else
                _reportType = ReportType.Info;
        }
    }
}
