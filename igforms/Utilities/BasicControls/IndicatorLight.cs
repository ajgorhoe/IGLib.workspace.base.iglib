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
    public partial class IndicatorLight : UserControl
    {

        public IndicatorLight()
        {
            InitializeComponent();
            lblText.Text = this.LabelText;
            lblText.Visible = !string.IsNullOrEmpty(this.LabelText);
            if (HasOff && IsOff)
            {
                ClearAll();
                IsOff = true;
            } else if (HasOk && IsOk)
            {
                ClearAll();
                IsOk = true;
            } else if (HasBusy && IsBusy)
            {
                ClearAll();
                IsBusy = true;
            } else if (HasError && IsError)
            {
                ClearAll();
                IsError = true;
            } else
            {
                ClearAll();
                if (HasOff)
                    IsOff = true;
                else if (HasOk)
                    IsOk = true;
                else if (HasBusy)
                    IsBusy = true;
                else if (HasError)
                    IsError = true;
            }
            LightColor = ActiveColor;
            timer1.Interval = BlinkIntervalMilliSeconds;
            if (IsBlinking)
            {
                _isBlinking = false;
                IsBlinking = true;
            }
            // Ensure that other properties (which affect appearance) take effect:
            BorderLabel = BorderLabel;
            BorderOut = BorderOut;
            MarginLabel = MarginLabel;
            PaddingLabel = PaddingLabel;
            MarginOut = MarginOut;
            PaddingOut = PaddingOut;
            ColorLabel = ColorLabel;
        }

        public void SetProperties()
        {

        }


        #region State

        private Color _lightColor = DefaultColorOff;

        protected Color LightColor
        {
            get { return _lightColor; }
            set {
                _lightColor = value;
                pnlLight.BackColor = value;
            }
        }

        /// <summary>Auxiliary, clears all flags withoud side effects.</summary>
        protected void ClearAll()
        {
            _isOff = false;
            _isOk = false;
            _isBusy = false;
            _isError = false;
        }

        /// <summary>Auxiliary, returns a flag indicating whether any mode is active.</summary>
        protected bool IsAny()
        {
            return (_isOff || _isOk || _isBusy || _isError);
        }

        /// <summary>Switch on the first available indicator light if none is currently switched on.
        /// <para>This helps ensure that at least one indicator is on when availability of different
        /// types of lights changes (and the indicator that was on becomes unavailable).</para></summary>
        protected void SetFirstAvailable()
        {
            if (!IsAny())
            {
                if (HasOff)
                    IsOff = true;
                else if (HasOk)
                    IsOk = true;
                else if (HasBusy)
                    IsBusy = true;
                else if (HasError)
                    IsError = true;
            }
        }

        /// <summary>Gets current active color for the light indicator.</summary>
        protected Color ActiveColor
        {
            get
            {
                if (_isOff)
                    return ColorOff;
                else if (_isOk)
                    return ColorOk;
                else if (_isBusy)
                    return ColorBusy;
                else if (_isError)
                    return ColorError;
                throw new InvalidOperationException("Could not determine active color, all modes are off.");
            }
        }


        protected bool _isOff = true;

        protected bool _isOk = false;

        protected bool _isBusy = false;

        protected bool _isError = false;


        /// <summary>Sets indicator to off, returns true if successful and false if not
        /// (which can be vecause the indictor does not have this state).</summary>
        public bool SetOff()
        {
            if (!HasOff && ThrowOnInvalidSwitch)
                throw new InvalidOperationException("The current indicator does not have the 'Off' state.");
            IsOff = true;
            return HasOff && IsOff;
        }

        /// <summary>Unsets the Off state, if set. Switches to a suitable other state available.</summary>
        /// <returns>True if successful and some other state was activated, false if not successful or if
        /// this state was not set when called.</returns>
        public bool UnsetOff()
        {
            if (!IsOff)
                return false;
            if (HasBusy)
                SetBusy();
            else if (HasError)
                SetError();
            else if (HasOk)
                SetOk();
            return IsAny();
        }

        /// <summary>Sets indicator to OK, returns true if successful and false if not
        /// (which can be vecause the indictor does not have this state).</summary>
        public bool SetOk()
        {
            if (!HasOk && ThrowOnInvalidSwitch)
                throw new InvalidOperationException("The current indicator does not have the 'OK' state.");
            IsOk = true;
            return HasOk && IsOk;
        }

        /// <summary>Unsets the OK state, if set. Switches to a suitable other state available.</summary>
        /// <returns>True if successful and some other state was activated, false if not successful or if
        /// this state was not set when called.</returns>
        public bool UnsetOk()
        {
            if (!IsOk)
                return false;
            if (HasOff)
                SetOff();
            else if (HasError)
                SetError();
            else if (HasBusy)
                SetBusy();
            return IsAny();
        }

        /// <summary>Sets indicator to Busy, returns true if successful and false if not
        /// (which can be vecause the indictor does not have this state).</summary>
        public bool SetBusy()
        {
            if (!HasBusy && ThrowOnInvalidSwitch)
                throw new InvalidOperationException("The current indicator does not have the 'Busy' state.");
            IsBusy = true;
            return HasBusy && IsBusy;
        }

        /// <summary>Unsets the Busy state, if set. Switches to a suitable other state available.</summary>
        /// <returns>True if successful and some other state was activated, false if not successful or if
        /// this state was not set when called.</returns>
        public bool UnsetBusy()
        {
            if (!IsBusy)
                return false;
            if (HasOff)
                SetOff();
            else if (HasError)
                SetError();
            else if (HasOk)
                SetOk();
            return IsAny();
        }

        /// <summary>Sets indicator to Error, returns true if successful and false if not
        /// (which can be vecause the indictor does not have this state).</summary>
        public bool SetError()
        {
            if (!HasError && ThrowOnInvalidSwitch)
                throw new InvalidOperationException("The current indicator does not have the 'Error' state.");
            IsError = true;
            return HasError && IsError;
        }


        /// <summary>Unsets the Error state, if set. Switches to a suitable other state available.</summary>
        /// <returns>True if successful and some other state was activated, false if not successful or if
        /// this state was not set when called.</returns>
        public bool UnsetError()
        {
            if (!IsError)
                return false;
            if (HasOff)
                SetOff();
            else if (HasBusy)
                SetBusy();
            else if (HasOk)
                SetOk();
            return IsAny();
        }


        /// <summary>Flag indicating whether the indicator is off.</summary>
        public bool IsOff
        {
            get { return _isOff; }
            protected set
            {
                if (value != _isOff)
                {
                    if (value == true)
                    {
                        if (HasOff)
                        {
                            ClearAll();
                            _isOff = true;
                            OnStateChanged();
                        }
                    } else
                    {
                        ClearAll();
                        IsOk = true;  // this will trigger OnStateChanged event.
                    }
                }
                if (!IsAny())
                    SetFirstAvailable();
            }
        }

        /// <summary>Flag indicating whether the indicator state is OK.</summary>
        public bool IsOk
        {
            get { return _isOk; }
            protected set
            {
                if (value != _isOk)
                {
                    if (value == true)
                    {
                        if (HasOk)
                        {
                            ClearAll();
                            _isOk = true;
                            OnStateChanged();
                        }
                    }
                    else
                    {
                        ClearAll();
                        IsOff = true;  // this will trigger OnStateChanged event.
                    }
                }
                if (!IsAny())
                    SetFirstAvailable();
            }
        }

        /// <summary>Flag indicating whether the indicator state is Busy.</summary>
        public bool IsBusy
        {
            get { return _isBusy; }
            protected set
            {
                if (value != _isBusy)
                {
                    if (value == true)
                    {
                        if (HasBusy)
                        {
                            ClearAll();
                            _isBusy = true;
                            OnStateChanged();
                        }
                    }
                    else
                    {
                        ClearAll();
                        IsOk = true;  // this will trigger OnStateChanged event.
                    }
                }
                if (!IsAny())
                    SetFirstAvailable();
            }
        }

        /// <summary>Flag indicating whether the indicator state is Busy.</summary>
        public bool IsError
        {
            get { return _isError; }
            protected set
            {
                if (value != _isError)
                {
                    if (value == true)
                    {
                        if (HasError)
                        {
                            ClearAll();
                            _isError = true;
                            OnStateChanged();
                        }
                    }
                    else
                    {
                        ClearAll();
                        IsOk = true;  // this will trigger OnStateChanged event.
                    }
                }
                if (!IsAny())
                    SetFirstAvailable();
            }
        }



        private int _numRemainingBlinks = 0;

        public int NumRemainingBlinks
        { get { return _numRemainingBlinks; } protected set { _numRemainingBlinks = value; } }

        protected int _numRemainingSpecialBlinks = 0;

        public int NumRemainingSpecialBlinks
        {
            get { return _numRemainingSpecialBlinks; }
            protected set { _numRemainingSpecialBlinks = value; }
        }

        protected Color _specialBlinkColor = Color.Yellow;

        public Color SpecialBlinkColor
        { get { return _specialBlinkColor;  } protected set { _specialBlinkColor = value; } }

        private bool _blinkOn = false;

        /// <summary>Specifies, when the indicator light is blinking, whether the blinking color is currently
        /// on or it is off. This flag is toggled by the timer when blinking is on, and the consequence is
        /// that the light color changes.</summary>
        protected bool IsBlinkOn
        {
            get { return _blinkOn; }
            set
            {
                _blinkOn = value;
                if (value == true)
                {
                    if (NumRemainingSpecialBlinks > 0)
                        LightColor = SpecialBlinkColor;
                    else
                        LightColor = ActiveColor;
                }
                else
                    LightColor = ColorOff;
            } }



        bool _isBlinking = false;

        /// <summary>Gets or sets the flag indicating whetherr the indicator light is continuously blinking
        /// in its active color.</summary>
        public bool IsBlinking
        {
            get { return _isBlinking; }
            set {
                if (value != _isBlinking)
                {
                    _isBlinking = value;
                    if (value)
                        StartBlinking();
                    else
                        StopBlinking();
                }
            }
        }

        /// <summary>Starts blinking the indicator light.</summary>
        protected void StartBlinking()
        {
            
            IsBlinkOn = false;
            timer1.Interval = BlinkIntervalMilliSeconds;
            timer1.Start();
        }


        /// <summary>Stops blinking the indicator light.</summary>
        protected void StopBlinking()
        {
            if (NumRemainingBlinks <= 0)
            {
                timer1.Stop();
                IsBlinkOn = true;
            }
        }

        /// <summary>Blninks indicator light in the currently active color for the specified number of times.</summary>
        /// <param name="numTimes">The number of times indicator blinks, default is 2.</param>
        public void Blink(int numTimes = 2)
        {
            NumRemainingBlinks = numTimes;
            StartBlinking();
        }


        /// <summary>Blinks the indicator link in chosen color, for a specified number of times.</summary>
        /// <param name="blinkColor">Color in which light blinks.</param>
        /// <param name="numTimes">Number of times the indictor blinks. Default is 2.</param>
        public void BlinkSpecial(Color blinkColor, int numTimes = 2)
        {
            NumRemainingSpecialBlinks = numTimes;
            SpecialBlinkColor = blinkColor;
            StartBlinking();
        }

        /// <summary>Blinks the indicator light in the color indicating teh OK state, for the 
        /// specified numberr of times. This is done independently of the current active color of 
        /// the indicator light.</summary>
        /// <param name="numTimes">Number of time the light blinks. Default is 2.</param>
        public void BlinkOk(int numTimes = 2)
        {
            BlinkSpecial(ColorOk, numTimes);
        }

        /// <summary>Blinks the indicator light in the color indicating teh Busy state, for the 
        /// specified numberr of times. This is done independently of the current active color of 
        /// the indicator light.</summary>
        /// <param name="numTimes">Number of time the light blinks. Default is 2.</param>
        public void BlinkBusy(int numTimes = 2)
        {
            BlinkSpecial(ColorBusy, numTimes);
        }

        /// <summary>Blinks the indicator light in the color indicating teh Error state, for the 
        /// specified numberr of times. This is done independently of the current active color of 
        /// the indicator light.</summary>
        /// <param name="numTimes">Number of time the light blinks. Default is 2.</param>
        public void BlinkError(int numTimes = 2)
        {
            BlinkSpecial(ColorError, numTimes);
        }
        

        #endregion State


        #region Behavior

        private int _blinkIntervalMilliseconds = DefaultBlinkIntervalMilliSeconds;

        public int BlinkIntervalMilliSeconds
        { get { return _blinkIntervalMilliseconds; } set { _blinkIntervalMilliseconds = value; } }

        private bool _throwOnInvalidSwitch = false;

        private bool _hasOff = true;

        private bool _hasOk = true;

        private bool _hasBusy = true;

        private bool _hasError = true;

        /// <summary>Gets or sets a flag indicating whether exception is thrown when an invalid switch
        /// is attempted (i.e. one that is not available).</summary>
        public bool ThrowOnInvalidSwitch
        { get { return _throwOnInvalidSwitch; } set { _throwOnInvalidSwitch = value; } }

        /// <summary>Gets or sets a flag indicating whether the indicator light has an Off state.</summary>
        public bool HasOff { get { return _hasOff; }
            set {
                _hasOff = value;
                if (value == false && IsOff)
                    IsOff = false;
            } }

        /// <summary>Gets or sets a flag indicating whether the indicator light has an OK state.</summary>
        public bool HasOk { get { return _hasOk; }
            set {
                _hasOk = value;
                if (value == false && IsOk)
                    IsOk = false;
            } }

        /// <summary>Gets or sets a flag indicating whether the indicator light has a Busy state.</summary>
        public bool HasBusy { get { return _hasBusy; }
            set {
                _hasBusy = value;
                if (value == false && IsBusy)
                    IsBusy = false;
            } }

        /// <summary>Gets or sets a flag indicating whether the indicator light has an Error state.</summary>
        public bool HasError { get { return _hasError; }
            set {
                _hasError = value;
                if (value == false && IsError)
                    IsError = false;
            } }



        #endregion Behavior


        #region AppearanceAndLayout

        private FlowDirection _direction = FlowDirection.LeftToRight;

        /// <summary>Directional arrangemnt of the contained light (a pannel) and label within 
        /// the containing outer flow layout panel.</summary>
        public FlowDirection FlowDirection
        {
            get { return _direction; }
            set { _direction = value;
                _direction = value;
                flowOuter.FlowDirection = value;
            }
        }


        private string _labelText = null;

        /// <summary>Text on the label besid the light (can be null).</summary>
        public string LabelText
        { get { return _labelText; }
            set
            {
                if (value != _labelText)
                {
                    _labelText = value;
                    lblText.Text = value;
                    lblText.Visible = !string.IsNullOrEmpty(value);
                }
            }
        }

        private Font _labelFont = null;

        public Font LabelFont
        {
            get { return _labelFont; }
            set
            {
                _labelFont = value;
                lblText.Font = value;
            }
        }


        private Color _colorLabel = DefaultColorLabel;

        private Color _colorLightOff = DefaultColorOff;

        private Color _colorLightOk = DefaultColorOk;

        private Color _colorLightBusy = DefaultColorBusy;

        private Color _colorLightError = DefaultColorError;


        public Color ColorLabel
        { get { return _colorLabel; }
            set {
                _colorLabel = value;
                lblText.ForeColor = value;
            } }

        public Color ColorOff
        { get { return _colorLightOff; } protected set { _colorLightOff = value; } }

        public Color ColorOk
        { get { return _colorLightOk; } protected set { _colorLightOk = value; } }

        public Color ColorError
        { get { return _colorLightError; } protected set { _colorLightError = value; } }

        public Color ColorBusy
        { get { return _colorLightBusy; } protected set { _colorLightBusy = value; } }


        private bool _borderOut = DefaultBorderOut;

        private bool _borderLabel = DefaultBorderLabel;

        private int _marginOut = DefaultMarginOut;

        private int _paddingOut = DefaultPaddingOut;

        private int _marginLabel = DefaultMarginLabel;

        private int _paddingLabel = DefaultPaddingLabel;

        /// <summary>Whether there is a border around control (more precisely, around its outer panel).</summary>
        public bool BorderOut
        { get { return _borderOut; } set {
                _borderOut = value;
                if (value)
                    flowOuter.BorderStyle = BorderStyle.FixedSingle;
                else
                    flowOuter.BorderStyle = BorderStyle.None;
            }
        }

        /// <summary>Whether label has a border.</summary>
        public bool BorderLabel
        { get { return _borderLabel; } set {
                _borderLabel = value;
                if (value)
                    lblText.BorderStyle = BorderStyle.FixedSingle;
                else
                    lblText.BorderStyle = BorderStyle.None;
            } }

        /// <summary>Margin of teh outer panel.</summary>
        public int MarginOut
        { get { return _marginOut; }
            set {
                _marginOut = value;
                flowOuter.Margin = new Padding(value);
            } }

        /// <summary>Padding of the outer panel.</summary>
        public int PaddingOut
        { get { return _paddingOut; }
            set {
                _paddingOut = value;
                flowOuter.Padding = new Padding(value);
            } }

        /// <summary>Margin of the label.</summary>
        public int MarginLabel
        { get { return _marginLabel; } set {
                _marginLabel = value;
                lblText.Margin = new Padding(value);
            } }

        /// <summary>Padding of the label.</summary>
        public int PaddingLabel
        { get { return _paddingLabel; }
            set { _paddingLabel = value;
                lblText.Padding = new Padding(value);
            } }


        #endregion AppearanceAndLayout


        #region Events

        /// <summary>Event that is fired whenever the selected file changes.</summary>
        public event EventHandler StateChanged;

        /// <summary>This method is executed when state changes. 
        /// It fires the <see cref="StateChanged"/> event.</summary>
        protected void OnStateChanged()
        {
            LightColor = ActiveColor;
            if (StateChanged != null)
                StateChanged(this, new EventArgs());
        }



        #endregion Events


        #region StaticSettings


        private static int _defaultBlinkIntervalMilliseconds = 500;

        public static int DefaultBlinkIntervalMilliSeconds
        { get { return _defaultBlinkIntervalMilliseconds; } set { _defaultBlinkIntervalMilliseconds = value; } }

        private static Color _defaultColorLabel = Color.Black;

        private static Color _defaultColorLightOff = Color.DarkSlateGray;  // Color.Gray;

        private static Color _defaultColorLightOk = Color.LimeGreen;

        private static Color _defaultColorLightBusy = Color.DarkRed;

        private static Color _defaultColorLightError = Color.Red;


        public static Color DefaultColorLabel
        { get { return _defaultColorLabel; } protected set { _defaultColorLabel = value; } }

        public static Color DefaultColorOff
        { get { return _defaultColorLightOff; }  protected set { _defaultColorLightOff = value; } }

        public static Color DefaultColorOk
        { get { return _defaultColorLightOk; }  protected set { _defaultColorLightOk = value; } }

        public static Color DefaultColorError
        { get { return _defaultColorLightError; }  protected set { _defaultColorLightError = value; } }

        public static Color DefaultColorBusy
        { get { return _defaultColorLightBusy; }  protected set { _defaultColorLightBusy = value; } }


        private static bool _defaultBorderOut = false;

        private static bool _defaultBorderLabel = false;

        private static int _defaultMarginOut = 2;

        private static int _defaultPaddingOut = 2;

        private static int _defaultMarginLabel = 2;

        private static int _defaultPaddingLabel = 2;

        public static bool DefaultBorderOut
        { get { return _defaultBorderOut; } set { _defaultBorderOut = value; } }

        public static bool DefaultBorderLabel
        { get { return _defaultBorderLabel; } set { _defaultBorderLabel = value; } }

        public static int DefaultMarginOut
        { get { return _defaultMarginOut; } set { _defaultMarginOut = value; } }

        public static int DefaultPaddingOut
        { get { return _defaultPaddingOut; } set { _defaultPaddingOut = value; } }

        public static int DefaultMarginLabel
        { get { return _defaultMarginLabel; } set { _defaultMarginLabel = value; } }

        public static int DefaultPaddingLabel
        { get { return _defaultPaddingLabel; } set { _defaultPaddingLabel = value; } }





        #endregion StaticSettings


        private void timer1_Tick(object sender, EventArgs e)
        {
            if (IsBlinkOn)
            {
                IsBlinkOn = false;

                if (NumRemainingBlinks > 0)
                    --NumRemainingBlinks;
                if (NumRemainingSpecialBlinks > 0)
                    --NumRemainingSpecialBlinks;
            } else
            {
                IsBlinkOn = true;
            }
            if (NumRemainingSpecialBlinks <= 0 && NumRemainingBlinks <= 0 && 
                !IsBlinking)
            {
                IsBlinkOn = true;
                timer1.Stop();
            }
            timer1.Interval = BlinkIntervalMilliSeconds;
        }
    }
}
