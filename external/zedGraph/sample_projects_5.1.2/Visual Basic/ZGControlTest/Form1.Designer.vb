<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso components IsNot Nothing Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
      Me.components = New System.ComponentModel.Container
      Me.zg1 = New ZedGraph.ZedGraphControl
      Me.SuspendLayout()
      '
      'zg1
      '
      Me.zg1.EditButtons = System.Windows.Forms.MouseButtons.Left
      Me.zg1.EditModifierKeys = CType((System.Windows.Forms.Keys.Alt Or System.Windows.Forms.Keys.None), System.Windows.Forms.Keys)
      Me.zg1.IsAutoScrollRange = False
      Me.zg1.IsEnableHEdit = False
      Me.zg1.IsEnableHPan = True
      Me.zg1.IsEnableHZoom = True
      Me.zg1.IsEnableVEdit = False
      Me.zg1.IsEnableVPan = True
      Me.zg1.IsEnableVZoom = True
      Me.zg1.IsPrintFillPage = True
      Me.zg1.IsPrintKeepAspectRatio = True
      Me.zg1.IsScrollY2 = False
      Me.zg1.IsShowContextMenu = True
      Me.zg1.IsShowCopyMessage = True
      Me.zg1.IsShowCursorValues = False
      Me.zg1.IsShowHScrollBar = False
      Me.zg1.IsShowPointValues = False
      Me.zg1.IsShowVScrollBar = False
      Me.zg1.IsSynchronizeXAxes = False
      Me.zg1.IsSynchronizeYAxes = False
      Me.zg1.IsZoomOnMouseCenter = False
      Me.zg1.LinkButtons = System.Windows.Forms.MouseButtons.Left
      Me.zg1.LinkModifierKeys = CType((System.Windows.Forms.Keys.Alt Or System.Windows.Forms.Keys.None), System.Windows.Forms.Keys)
      Me.zg1.Location = New System.Drawing.Point(12, 12)
      Me.zg1.Name = "zg1"
      Me.zg1.PanButtons = System.Windows.Forms.MouseButtons.Left
      Me.zg1.PanButtons2 = System.Windows.Forms.MouseButtons.Middle
      Me.zg1.PanModifierKeys = CType((System.Windows.Forms.Keys.Shift Or System.Windows.Forms.Keys.None), System.Windows.Forms.Keys)
      Me.zg1.PanModifierKeys2 = System.Windows.Forms.Keys.None
      Me.zg1.PointDateFormat = "g"
      Me.zg1.PointValueFormat = "G"
      Me.zg1.ScrollMaxX = 0
      Me.zg1.ScrollMaxY = 0
      Me.zg1.ScrollMaxY2 = 0
      Me.zg1.ScrollMinX = 0
      Me.zg1.ScrollMinY = 0
      Me.zg1.ScrollMinY2 = 0
      Me.zg1.Size = New System.Drawing.Size(493, 337)
      Me.zg1.TabIndex = 0
      Me.zg1.ZoomButtons = System.Windows.Forms.MouseButtons.Left
      Me.zg1.ZoomButtons2 = System.Windows.Forms.MouseButtons.None
      Me.zg1.ZoomModifierKeys = System.Windows.Forms.Keys.None
      Me.zg1.ZoomModifierKeys2 = System.Windows.Forms.Keys.None
      Me.zg1.ZoomStepFraction = 0.1
      '
      'Form1
      '
      Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
      Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
      Me.ClientSize = New System.Drawing.Size(517, 361)
      Me.Controls.Add(Me.zg1)
      Me.Name = "Form1"
      Me.Text = "Form1"
      Me.ResumeLayout(False)

   End Sub
   Friend WithEvents zg1 As ZedGraph.ZedGraphControl

End Class
