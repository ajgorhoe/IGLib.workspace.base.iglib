'============================================================================
'ZedGraph demo code
'The code contained in this file (only) is released into the public domain, so you
'can copy it into your project without any license encumbrance.  Note that
'the actual ZedGraph library code is licensed under the LGPL, which is not
'public domain.
'
'This file is distributed in the hope that it will be useful,
'but WITHOUT ANY WARRANTY; without even the implied warranty of
'MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
'=============================================================================
Imports ZedGraph

Public Class Form1

   Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
      Dim myPane As GraphPane = zg1.GraphPane

      ' Set the titles and axis labels
      myPane.Title.Text = "Demonstration of Dual Y Graph"
      myPane.XAxis.Title.Text = "Time, Days"
      myPane.YAxis.Title.Text = "Parameter A"
      myPane.Y2Axis.Title.Text = "Parameter B"

      ' Make up some data points based on the Sine function
      Dim list As New PointPairList
      Dim list2 As New PointPairList
      Dim i As Integer, x As Double, y As Double, y2 As Double

      For i = 0 To 35
         x = i * 5.0
         y = Math.Sin(i * Math.PI / 15.0) * 16.0
         y2 = y * 13.5
         list.Add(x, y)
         list2.Add(x, y2)
      Next i

      ' Generate a red curve with diamond symbols, and "Alpha" in the legend
      Dim myCurve As LineItem
      myCurve = myPane.AddCurve("Alpha", list, Color.Red, SymbolType.Diamond)
      ' Fill the symbols with white
      myCurve.Symbol.Fill = New Fill(Color.White)

      ' Generate a blue curve with circle symbols, and "Beta" in the legend
      myCurve = myPane.AddCurve("Beta", list2, Color.Blue, SymbolType.Circle)
      ' Fill the symbols with white
      myCurve.Symbol.Fill = New Fill(Color.White)
      ' Associate this curve with the Y2 axis
      myCurve.IsY2Axis = True

      ' Show the x axis grid
      myPane.XAxis.MajorGrid.IsVisible = True

      ' Make the Y axis scale red
      myPane.YAxis.Scale.FontSpec.FontColor = Color.Red
      myPane.YAxis.Title.FontSpec.FontColor = Color.Red
      ' turn off the opposite tics so the Y tics don't show up on the Y2 axis
      myPane.YAxis.MajorTic.IsOpposite = False
      myPane.YAxis.MinorTic.IsOpposite = False
      ' Don't display the Y zero line
      myPane.YAxis.MajorGrid.IsZeroLine = False
      ' Align the Y axis labels so they are flush to the axis
      myPane.YAxis.Scale.Align = AlignP.Inside
      ' Manually set the axis range
      myPane.YAxis.Scale.Min = -30
      myPane.YAxis.Scale.Max = 30

      ' Enable the Y2 axis display
      myPane.Y2Axis.IsVisible = True
      ' Make the Y2 axis scale blue
      myPane.Y2Axis.Scale.FontSpec.FontColor = Color.Blue
      myPane.Y2Axis.Title.FontSpec.FontColor = Color.Blue
      ' turn off the opposite tics so the Y2 tics don't show up on the Y axis
      myPane.Y2Axis.MajorTic.IsOpposite = False
      myPane.Y2Axis.MinorTic.IsOpposite = False
      ' Display the Y2 axis grid lines
      myPane.Y2Axis.MajorGrid.IsVisible = True
      ' Align the Y2 axis labels so they are flush to the axis
      myPane.Y2Axis.Scale.Align = AlignP.Inside

      ' Fill the axis background with a gradient
      myPane.Chart.Fill = New Fill(Color.White, Color.LightGray, 45.0F)

      ' Add a text box with instructions
      Dim text As New TextObj( _
   "Zoom: left mouse & drag" & Chr(10) & "Pan: middle mouse & drag" & Chr(10) & "Context Menu: right mouse", _
   0.05F, 0.95F, CoordType.ChartFraction, AlignH.Left, AlignV.Bottom)
      text.FontSpec.StringAlignment = StringAlignment.Near
      myPane.GraphObjList.Add(text)

      ' Enable scrollbars if needed
      zg1.IsShowHScrollBar = True
      zg1.IsShowVScrollBar = True
      zg1.IsAutoScrollRange = True
      zg1.IsScrollY2 = True

      zg1.IsShowPointValues = True

      ' Size the control to fit the window
      SetSize()

      ' Tell ZedGraph to calculate the axis ranges
      ' Note that you MUST call this after enabling IsAutoScrollRange, since AxisChange() sets
      ' up the proper scrolling parameters
      zg1.AxisChange()
      ' Make sure the Graph gets redrawn
      zg1.Invalidate()
   End Sub

   ' On resize action, resize the ZedGraphControl to fill most of the Form, with a small
   ' margin around the outside
   Private Sub Form1_Resize(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Resize
      SetSize()
   End Sub

   Private Sub SetSize()
      Dim loc As New Point(10, 10)
      zg1.Location = loc
      ' Leave a small margin around the outside of the control
      Dim size As New Size(Me.ClientRectangle.Width - 20, Me.ClientRectangle.Height - 20)
      zg1.Size = size
   End Sub

   'Display customized tooltips when the mouse hovers over a point
   Private Function MyPointValueEvent(ByVal control As ZedGraphControl, _
           ByVal pane As GraphPane, ByVal curve As CurveItem, _
           ByVal iPt As Integer) As String Handles zg1.PointValueEvent

      ' Get the PointPair that is under the mouse
      Dim pt As PointPair = curve(iPt)

      Return curve.Label.Text + " is " + pt.Y.ToString("f2") + " units at " + pt.X.ToString("f1") + " days"
   End Function

   ' Customize the context menu by adding a new item to the end of the menu
   Private Sub MyContextMenuBuilder(ByVal control As ZedGraphControl, _
              ByVal menu As ContextMenuStrip, ByVal mousePt As Point, _
              ByVal objState As ZedGraphControl.ContextMenuObjectState) _
           Handles zg1.ContextMenuBuilder
      Dim item As New ToolStripMenuItem
      item.Name = "add-beta"
      item.Tag = "add-beta"
      item.Text = "Add a new Beta Point"
      AddHandler item.Click, AddressOf Me.AddBetaPoint

      menu.Items.Add(item)
   End Sub

   ' Handle the "Add New Beta Point" context menu item.  This finds the curve with
   ' the CurveItem.Label = "Beta", and adds a new point to it.
   Private Sub AddBetaPoint(ByVal sender As Object, ByVal args As EventArgs)
      ' Get a reference to the "Beta" curve PointPairList
      Dim x As Double, y As Double

      Dim ip As IPointListEdit = zg1.GraphPane.CurveList("Beta").Points

      If (Not IsNothing(ip)) Then
         x = ip.Count * 5.0
         y = Math.Sin(ip.Count * Math.PI / 15.0) * 16.0 * 13.5
         ip.Add(x, y)
         zg1.AxisChange()
         zg1.Refresh()
      End If
   End Sub

   Private Sub zg1_ZoomEvent(ByVal control As ZedGraphControl, ByVal oldState As ZoomState, _
           ByVal newState As ZoomState) Handles zg1.ZoomEvent
      'Here we get notification everytime the user zooms
   End Sub
End Class
