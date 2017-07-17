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
    public partial class NotificationFrame : UserControl
    {
        public NotificationFrame()
        {
            InitializeComponent();
        }

        public void ShowNotification(string title, string message, ReportType? notificationtype = null)
        {
            if (notificationtype == null)
                notificationtype = NotificationParameters.DefaultType;
        }

    }
}
