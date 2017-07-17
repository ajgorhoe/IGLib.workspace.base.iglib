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

    /// <summary>A window form to calculate various checksums of selected files or inserted text.
    /// <para>This is only a window container for <see cref="HashForm"/>.</para></summary>
    /// <remarks><para>See:</para>
    /// <para>http://www2.arnes.si/~ljc3m2/igor/software/IGLibShellApp/HashForm.html#hashform </para></remarks>
    /// $A Igor Aug 08;
    public partial class HashForm : Form
    {
        public HashForm()
        {
            InitializeComponent();
        }
    }
}
