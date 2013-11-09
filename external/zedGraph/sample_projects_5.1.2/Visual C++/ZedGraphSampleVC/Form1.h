#pragma once


namespace ZedGraphSampleVC
{

	using namespace System;
	using namespace System::ComponentModel;
	using namespace System::Collections;
	using namespace System::Windows::Forms;
	using namespace System::Data;
	using namespace System::Drawing;
	using namespace ZedGraph;

	/// <summary>
	/// Summary for Form1
	///
	/// WARNING: If you change the name of this class, you will need to change the
	///          'Resource File Name' property for the managed resource compiler tool
	///          associated with all .resx files this class depends on.  Otherwise,
	///          the designers will not be able to interact properly with localized
	///          resources associated with this form.
	/// </summary>
	public ref class Form1 : public System::Windows::Forms::Form
	{
	public:
		Form1(void)
		{
			InitializeComponent();
		}

	private: 
		void SetSize()
		{
			zg1->Location = Point( 10, 10 );
			// Leave a small margin around the outside of the control
			zg1->Size = System::Drawing::Size( ClientRectangle.Width - 20,
					ClientRectangle.Height - 20 );
		}

		System::Void Form1_Load(System::Object^  sender, System::EventArgs^  e)
		{
			CreateGraph( zg1 );
			SetSize();
		}

		System::Void Form1_Resize(System::Object^  sender, System::EventArgs^  e)
		{
			SetSize();
		}

		void CreateGraph( ZedGraphControl ^z1 )
		{
			// Get a reference to the GraphPane instance in the ZedGraphControl
			GraphPane ^myPane = z1->GraphPane;

			// Set the titles and axis labels
			myPane->Title->Text = L"Demonstration of Dual Y Graph";
			myPane->XAxis->Title->Text = L"Time, Days";
			myPane->YAxis->Title->Text = L"Parameter A";
			myPane->Y2Axis->Title->Text = L"Parameter B";

			// Make up some data points based on the Sine function
			PointPairList ^list = gcnew PointPairList();
			PointPairList ^list2 = gcnew PointPairList();
			for ( int i = 0; i < 36; i++ )
			{
				double x = (double)i * 5.0;
				double y = Math::Sin( (double)i * Math::PI / 15.0 ) * 16.0;
				double y2 = y * 13.5;
				list->Add( x, y );
				list2->Add( x, y2 );
			}

			// Generate a red curve with diamond symbols, and "Alpha" in the legend
			LineItem ^myCurve = myPane->AddCurve( L"Alpha",
				list, Color::Red, SymbolType::Diamond );
			// Fill the symbols with white
			myCurve->Symbol->Fill = gcnew Fill( Color::White );

			// Generate a blue curve with circle symbols, and "Beta" in the legend
			myCurve = myPane->AddCurve( L"Beta",
				list2, Color::Blue, SymbolType::Circle );
			// Fill the symbols with white
			myCurve->Symbol->Fill = gcnew Fill( Color::White );
			// Associate this curve with the Y2 axis
			myCurve->IsY2Axis = true;

			// Show the x axis grid
			myPane->XAxis->MajorGrid->IsVisible = true;

			// Make the Y axis scale red
			myPane->YAxis->Scale->FontSpec->FontColor = Color::Red;
			myPane->YAxis->Title->FontSpec->FontColor = Color::Red;
			// turn off the opposite tics so the Y tics don't show up on the Y2 axis
			myPane->YAxis->MajorTic->IsOpposite = false;
			myPane->YAxis->MinorTic->IsOpposite = false;
			// Don't display the Y zero line
			myPane->YAxis->MajorGrid->IsZeroLine = false;
			// Align the Y axis labels so they are flush to the axis
			myPane->YAxis->Scale->Align = AlignP::Inside;
			// Manually set the axis range
			myPane->YAxis->Scale->Min = -30;
			myPane->YAxis->Scale->Max = 30;

			// Enable the Y2 axis display
			myPane->Y2Axis->IsVisible = true;
			// Make the Y2 axis scale blue
			myPane->Y2Axis->Scale->FontSpec->FontColor = Color::Blue;
			myPane->Y2Axis->Title->FontSpec->FontColor = Color::Blue;
			// turn off the opposite tics so the Y2 tics don't show up on the Y axis
			myPane->Y2Axis->MajorTic->IsOpposite = false;
			myPane->Y2Axis->MinorTic->IsOpposite = false;
			// Display the Y2 axis grid lines
			myPane->Y2Axis->MajorGrid->IsVisible = true;
			// Align the Y2 axis labels so they are flush to the axis
			myPane->Y2Axis->Scale->Align = AlignP::Inside;

			// Fill the axis background with a gradient
			myPane->Chart->Fill = gcnew Fill( Color::White, Color::LightGray, 45.0f );

			// Add a text box with instructions
			TextObj ^text = gcnew TextObj(
				L"Zoom: left mouse & drag\nPan: middle mouse & drag\nContext Menu: right mouse",
				0.05f, 0.95f, CoordType::ChartFraction, AlignH::Left, AlignV::Bottom );
			text->FontSpec->StringAlignment = StringAlignment::Near;
			myPane->GraphObjList->Add( text );

			// Enable scrollbars if needed
			z1->IsShowHScrollBar = true;
			z1->IsShowVScrollBar = true;
			z1->IsAutoScrollRange = true;
			z1->IsScrollY2 = true;

			// OPTIONAL: Show tooltips when the mouse hovers over a point
			z1->IsShowPointValues = true;
			z1->PointValueEvent += gcnew ZedGraphControl::PointValueHandler( this,
					&Form1::MyPointValueHandler );

			// OPTIONAL: Add a custom context menu item
			z1->ContextMenuBuilder += gcnew
					ZedGraphControl::ContextMenuBuilderEventHandler(
					this, &Form1::MyContextMenuBuilder );

			// OPTIONAL: Handle the Zoom Event
			z1->ZoomEvent += gcnew ZedGraphControl::ZoomEventHandler( this,
						&Form1::MyZoomEvent );

			// Size the control to fit the window
			SetSize();

			// Tell ZedGraph to calculate the axis ranges
			// Note that you MUST call this after enabling IsAutoScrollRange, since AxisChange() sets
			// up the proper scrolling parameters
			z1->AxisChange();
			// Make sure the Graph gets redrawn
			z1->Invalidate();
		}

		/// <summary>
		/// Display customized tooltips when the mouse hovers over a point
		/// </summary>
		System::String ^MyPointValueHandler( ZedGraphControl ^control, GraphPane ^pane,
						CurveItem ^curve, int iPt )
		{
			// Get the PointPair that is under the mouse
			PointPair pt = curve[iPt];

			return curve->Label->Text + " is " + pt.Y.ToString( "f2" ) + " units at " + pt.X.ToString( "f1" ) + " days";
		}

		/// <summary>
		/// Customize the context menu by adding a new item to the end of the menu
		/// </summary>
		void MyContextMenuBuilder( ZedGraphControl ^control,
					System::Windows::Forms::ContextMenuStrip ^menuStrip,
					Point mousePt,
					ZedGraphControl::ContextMenuObjectState objState )
		{
			ToolStripMenuItem ^item = gcnew ToolStripMenuItem();
			item->Name = L"add-beta";
			item->Tag = L"add-beta";
			item->Text = L"Add a new Beta Point";
			item->Click += gcnew System::EventHandler( this, &Form1::AddBetaPoint );

			menuStrip->Items->Add( item );
		}

		/// <summary>
		/// Handle the "Add New Beta Point" context menu item.  This finds the curve with
		/// the CurveItem.Label = "Beta", and adds a new point to it.
		/// </summary>
		void AddBetaPoint( System::Object ^sender, EventArgs ^args )
		{
			// Get a reference to the "Beta" curve IPointListEdit
			IPointListEdit ^ip = (IPointListEdit^) zg1->GraphPane->CurveList[L"Beta"]->Points;
			if ( ip != nullptr )
			{
				double x = ip->Count * 5.0;
				double y = Math::Sin( ip->Count * Math::PI / 15.0 ) * 16.0 * 13.5;
				ip->Add( x, y );
				zg1->AxisChange();
				zg1->Refresh();
			}
		}

		// Respond to a Zoom Event
		void MyZoomEvent( ZedGraphControl ^control, ZoomState ^oldState,
					ZoomState ^newState )
		{
			// Here we get notification everytime the user zooms
		}

	protected:
		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		~Form1()
		{
			if (components)
			{
				delete components;
			}
		}
	private: ZedGraph::ZedGraphControl^  zg1;
	protected: 

	protected: 

	protected: 

	protected: 
	private: System::ComponentModel::IContainer^  components;

	private:
		/// <summary>
		/// Required designer variable.
		/// </summary>


#pragma region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		void InitializeComponent(void)
		{
			this->components = (gcnew System::ComponentModel::Container());
			this->zg1 = (gcnew ZedGraph::ZedGraphControl());
			this->SuspendLayout();
			// 
			// zg1
			// 
			this->zg1->Location = System::Drawing::Point(12, 12);
			this->zg1->Name = L"zg1";
			this->zg1->ScrollMaxX = 0;
			this->zg1->ScrollMaxY = 0;
			this->zg1->ScrollMaxY2 = 0;
			this->zg1->ScrollMinX = 0;
			this->zg1->ScrollMinY = 0;
			this->zg1->ScrollMinY2 = 0;
			this->zg1->Size = System::Drawing::Size(513, 392);
			this->zg1->TabIndex = 0;
			// 
			// Form1
			// 
			this->AutoScaleDimensions = System::Drawing::SizeF(6, 13);
			this->AutoScaleMode = System::Windows::Forms::AutoScaleMode::Font;
			this->ClientSize = System::Drawing::Size(537, 416);
			this->Controls->Add(this->zg1);
			this->Name = L"Form1";
			this->Text = L"Form1";
			this->Resize += gcnew System::EventHandler(this, &Form1::Form1_Resize);
			this->Load += gcnew System::EventHandler(this, &Form1::Form1_Load);
			this->ResumeLayout(false);

		}
#pragma endregion
	};
}

