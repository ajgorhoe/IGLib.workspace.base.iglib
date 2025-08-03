// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Media;
using System.Reflection;

using IG.Lib;
using IG.Num;



namespace IG.Forms
{
    public partial class TimerControl : UserControl
    {

        /// <summary>Gets a reporter for the current control.</summary>
        public IG.Lib.IReporter Reporter { get { return UtilForms.Reporter; } }

        /// <summary>Constructs a timer control.</summary>
        public TimerControl()
        {
            InitializeComponent();
            // Set the base font:
            this.DisplayFont = dispSwSec.Font;
            // Set correct layout: 
            if (UtilSystem.IsWindowsOs)
            {
                flowOuter.FlowDirection = FlowDirection.TopDown;
            } else if (UtilSystem.IsLinuxOs)
            {
                flowOuter.FlowDirection = FlowDirection.LeftToRight;
            } else
            {
                flowOuter.FlowDirection = FlowDirection.TopDown;
            }
            // Initialize Stopwatch:
            StopwatchResetIt();
            IsSwControlsOpened = false;
            SwShowHoursWhenZero = chkSwDisplayHours.Checked;
            SwShowMilliSeconds = chkSwDisplayMilliseconds.Checked;
            UpdateSwControls();
            // Initialize Count Down:
            CdInitialTimeSpan = new TimeSpan(0, 0, 10 /* minutes */, 0);
            IsCdControlsOpened = false;
            CountdownResetIt();
            CdShowHoursWhenZero = chkCdDisplayHours.Checked;
            CdShowMilliSeconds = chkCdDisplayMillisec.Checked;
            UpdateCdControls();

            //Initialize Clock: 



            // Set the main timer and designate others as not main:
            lblSwTitle.BackColor = ColorBgTitle;  lblSwTitle.ForeColor = ColorFgTitle;
            lblCdTitle.BackColor = ColorBgTitle; lblCdTitle.ForeColor = ColorFgTitle;
            lblClockTitle.BackColor = ColorBgTitle; lblClockTitle.ForeColor = ColorFgTitle;
            IsStopwatchShown = false;
            IsClockShown = true;
            IsCountdownShown = true;
            IsStopwatchMain = true;

            // Start noting wallclock time:
            _clockPreviousWallclockTime = _clockCurrentWalclockTime = WallclockTime;
            // Start the timer events (tu update clock display and so on):
            timer1.Enabled = true;

        }

        private static int _defaultOutputLevel = 0;
        
        /// <summary>Default output level for this class of objects.</summary>
        public static int DefaultOutputLevel
        {
            get { return _defaultOutputLevel; } set { _defaultOutputLevel = value; } }

        private int _outputLevel = DefaultOutputLevel;

        /// <summary>Current output level for the current object.
        /// <para>Latger the value more console output is generated.</para></summary>
        private int OutputLevel {
            get { return _outputLevel;  }
            set { _outputLevel = value; } }


        #region Sounds

        public void LoadDefaultSounds()
        {
            SoundPlayerStart.LoadAsync();
        }



        private int _maxPlayErrors = 5;
        protected int MaxPlayErrors { get { return _maxPlayErrors; } set { _maxPlayErrors = value; } }
        protected int NumPlayErrors { get; set; }

        private void ReportSoundError(string soundDescriptor, Exception ex)
        {
                ++NumPlayErrors;
            if (NumPlayErrors <= MaxPlayErrors)
            {
                string msg = Environment.NewLine +
                    "WARNING: " + soundDescriptor + " could not be played." + Environment.NewLine;
                if (NumPlayErrors == MaxPlayErrors)
                    msg += "Further messages of this kind will be suppressed." + Environment.NewLine;
                msg += "  Details: " + ex.Message + Environment.NewLine;

                // Console.WriteLine(msg);

                Console.WriteLine(Environment.NewLine + Environment.NewLine + "Launching a sound error report...");
                   
                Reporter.ReportError(msg);

                Console.WriteLine("... sound error report done." + Environment.NewLine);
            }
        }


        /// <summary>Plays the sound associated with clicking the start button.</summary>
        public virtual void PlaySoundStart()
        {
            try { SoundPlayerStart.Play(); }
            catch(Exception ex) { ReportSoundError("Start button sound", ex); }
        }

        protected SoundPlayer _soundPlayerStart = null;

        /// <summary>Gets the sound player for playing sound when a start button is  pressed.
        /// Player is pre-laded with the appropriate sound.</summary>
        public virtual SoundPlayer SoundPlayerStart
        {
            get
            {
                if (_soundPlayerStart == null)
                {
                    _soundPlayerStart = new SoundPlayer();
                    Stream stream = Assembly.GetExecutingAssembly().
                        GetManifestResourceStream(
                        "IG.Forms.sounds.timer.signal_gentle_beep_freesound_141121_buttonstartstop.wav");
                    _soundPlayerStart.Stream = stream;
                    _soundPlayerStart.LoadAsync();
                }
                return _soundPlayerStart;
            }
        }



        /// <summary>Plays the sound associated with clicking the stop button.</summary>
        public virtual void PlaySoundStop()
        {
            try { SoundPlayerStop.Play(); }
            catch (Exception ex) { ReportSoundError("Stop button sound", ex); }
        }

        protected SoundPlayer _soundPlayerStop = null;

        /// <summary>Gets the sound player for playing sound when the stop button is  pressed.
        /// Player is pre-laded with the appropriate sound.</summary>
        public virtual SoundPlayer SoundPlayerStop
        {
            get
            {
                if (_soundPlayerStop == null)
                {
                    _soundPlayerStop = new SoundPlayer();
                    Stream stream = Assembly.GetExecutingAssembly().
                        GetManifestResourceStream(
                        "IG.Forms.sounds.timer.signal_gentle_beep_freesound_141121_buttonstartstop.wav");
                    _soundPlayerStop.Stream = stream;
                    _soundPlayerStop.LoadAsync();
                }
                return _soundPlayerStop;
            }
        }



        /// <summary>Plays the sound associated with clicking the reset button.</summary>
        public virtual void PlaySoundReset()
        {
            try { SoundPlayerReset.Play(); }
            catch (Exception ex) { ReportSoundError("Reset button sound", ex); }
        }

        protected SoundPlayer _soundPlayerReset = null;

        /// <summary>Gets the sound player for playing sound when the resett button is  pressed.
        /// Player is pre-laded with the appropriate sound.</summary>
        public virtual SoundPlayer SoundPlayerReset
        {
            get
            {
                if (_soundPlayerReset == null)
                {
                    _soundPlayerReset = new SoundPlayer();
                    Stream stream = Assembly.GetExecutingAssembly().
                        GetManifestResourceStream(
                        "IG.Forms.sounds.timer.signal_end_freesound_191589_buttonreset.wav");
                    _soundPlayerReset.Stream = stream;
                    _soundPlayerReset.LoadAsync();
                }
                return _soundPlayerReset;
            }
        }



        /// <summary>Plays the sound associated with a second tick.</summary>
        public virtual void PlaySoundSecondTick()
        {
            try { SoundPlayerSecondTick.Play(); }
            catch (Exception ex) { ReportSoundError("Second tick sound", ex); }
        }

        protected SoundPlayer _soundPlayerSecondTick = null;

        /// <summary>Gets the sound player for playing sound when another second passes (a second tick).
        /// Player is pre-laded with the appropriate sound.</summary>
        public virtual SoundPlayer SoundPlayerSecondTick
        {
            get
            {
                if (_soundPlayerSecondTick == null)
                {
                    _soundPlayerSecondTick = new SoundPlayer();
                    Stream stream = Assembly.GetExecutingAssembly().
                        GetManifestResourceStream(
                        "IG.Forms.sounds.timer.tick_short_freesound_2076_second.wav");
                    _soundPlayerSecondTick.Stream = stream;
                    _soundPlayerSecondTick.LoadAsync();
                }
                return _soundPlayerSecondTick;
            }
        }


        /// <summary>Plays the sound associated with the whole minute switch.</summary>
        public virtual void PlaySoundMinuteBell()
        {
            try { SoundPlayerMinuteBell.Play(); }
            catch (Exception ex) { ReportSoundError("Minute bell sound", ex); }
        }

        protected SoundPlayer _soundPlayerMinuteBell = null;

        /// <summary>Gets the sound player for playing sound when another minute passes (a minute bell).
        /// Player is pre-laded with the appropriate sound.</summary>
        public virtual SoundPlayer SoundPlayerMinuteBell
        {
            get
            {
                if (_soundPlayerMinuteBell == null)
                {
                    _soundPlayerMinuteBell = new SoundPlayer();
                    Stream stream = Assembly.GetExecutingAssembly().
                        GetManifestResourceStream(
                        "IG.Forms.sounds.timer.tick_short_freesound_2076_second.wav");
                    _soundPlayerMinuteBell.Stream = stream;
                    _soundPlayerMinuteBell.LoadAsync();
                }
                return _soundPlayerMinuteBell;
            }
        }


        /// <summary>Plays the sound associated with the whole minute switch.</summary>
        public virtual void PlaySoundHourBell()
        {
            try { SoundPlayerMinuteBell.Play(); }
            catch (Exception ex) { ReportSoundError("Hour bell sound", ex); }
        }

        protected SoundPlayer _soundPlayerHourBell = null;

        /// <summary>Gets the sound player for playing sound when another hour passes (a hour bell).
        /// Player is pre-laded with the appropriate sound.</summary>
        public virtual SoundPlayer SoundPlayerHourBell
        {
            get
            {
                if (_soundPlayerHourBell == null)
                {
                    _soundPlayerHourBell = new SoundPlayer();
                    Stream stream = Assembly.GetExecutingAssembly().
                        GetManifestResourceStream(
                        "IG.Forms.sounds.timer.tick_short_freesound_2076_second.wav");
                    _soundPlayerHourBell.Stream = stream;
                    _soundPlayerHourBell.LoadAsync();
                }
                return _soundPlayerHourBell;
            }
        }


        /// <summary>Plays the sound associated with a countdown end.</summary>
        public virtual void PlaySoundAlarmCountdown()
        {
            try { SoundPlayerAlarmCountdown.Play(); }
            catch (Exception ex) { ReportSoundError("Countdown alarm sound ", ex); }
        }

        protected SoundPlayer _soundPlayerAlarmCountdown = null;

        /// <summary>Gets the sound player for playing alarm when countdown finishes.
        /// Player is pre-laded with the appropriate sound.</summary>
        public virtual SoundPlayer SoundPlayerAlarmCountdown
        {
            get
            {
                if (_soundPlayerAlarmCountdown == null)
                {
                    _soundPlayerAlarmCountdown = new SoundPlayer();
                    Stream stream = Assembly.GetExecutingAssembly().
                        GetManifestResourceStream(
                        "IG.Forms.sounds.timer.alarm_5_beeps_freesound_221087_alarm.wav");
                    _soundPlayerAlarmCountdown.Stream = stream;
                    _soundPlayerAlarmCountdown.LoadAsync();
                }
                return _soundPlayerAlarmCountdown;
            }
        }


        // Sound settings for the stopwatch

        private bool _isSwSoundSecondTick = true;

        /// <summary>Whether a tick sound is switched on (launched every second when the stopwatch is running).</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public virtual bool IsSwSoundSecondTick
        {
            get { return _isSwSoundSecondTick; }
            set
            {
                if (value != _isSwSoundSecondTick)
                {
                    _isSwSoundSecondTick = value;
                }
            }
        }


        private bool _isSwSoundMinuteBell = true;

        /// <summary>Whether a minutes bell sound is switched on (launched every minute when the stopwatch is running).</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public virtual bool IsSwSoundMinuteBell
        {
            get { return _isSwSoundMinuteBell; }
            set
            {
                if (value != _isSwSoundMinuteBell)
                {
                    _isSwSoundMinuteBell = value;
                }
            }
        }

        private bool _isSwSoundHourBell = true;

        /// <summary>Whether a hour bell sound is switched on (launched every hour when the stopwatch is running).</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public virtual bool IsSwSoundHourBell
        {
            get { return _isSwSoundHourBell; }
            set
            {
                if (value != _isSwSoundHourBell)
                {
                    _isSwSoundHourBell = value;
                }
            }
        }

        private bool _isSwSoundButtonPressed = true;

        /// <summary>Whether the stopwatch's button sound is switched on (launched every 
        /// time a stopwatch button with some effect is pressed, or the same effect is achieved programatically).</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public virtual bool IsSwSoundButtons
        {
            get { return _isSwSoundButtonPressed; }
            set
            {
                if (value != _isSwSoundButtonPressed)
                {
                    _isSwSoundButtonPressed = value;
                }
            }
        }


        // Sound settings for the countdown:

        private bool _isCdSoundSecondTick = true;

        /// <summary>Whether the countdown's tick sound is switched on (a sound launched every second when 
        /// the countdown is running).</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public virtual bool IsCdSoundSecondTick
        {
            get { return _isCdSoundSecondTick; }
            set
            {
                if (value != _isCdSoundSecondTick)
                {
                    _isCdSoundSecondTick = value;
                }
            }
        }

        private bool _isCdSoundMinuteBell = true;

        /// <summary>Whether the countdown's minute bell sound is switched on (a sound launched every minute when 
        /// the countdown is running).</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public virtual bool IsCdSoundMinuteBell
        {
            get { return _isCdSoundMinuteBell; }
            set
            {
                if (value != _isCdSoundMinuteBell)
                {
                    _isCdSoundMinuteBell = value;
                }
            }
        }

        private bool _isCdSoundHourBell = true;

        /// <summary>Whether the countdown's hpur bell sound is switched on (a sound launched every hour when 
        /// the countdown is running).</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public virtual bool IsCdSoundHourBell
        {
            get { return _isCdSoundHourBell; }
            set
            {
                if (value != _isCdSoundHourBell)
                {
                    _isCdSoundHourBell = value;
                }
            }
        }



        // Sound settings for the clock:

        private bool _isClockSoundSecondTick = true;

        /// <summary>Whether the countdown's tick sound is switched on (a sound launched every second when 
        /// the countdown is running).</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public virtual bool IsClockSoundSecondTick
        {
            get { return _isClockSoundSecondTick; }
            set
            {
                if (value != _isClockSoundSecondTick)
                {
                    _isClockSoundSecondTick = value;
                }
            }
        }

        private bool _isClockSoundMinuteBell = true;

        /// <summary>Whether the countdown's minute bell sound is switched on (a sound launched every minute when 
        /// the countdown is running).</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public virtual bool IsClockSoundMinuteBell
        {
            get { return _isClockSoundMinuteBell; }
            set
            {
                if (value != _isClockSoundMinuteBell)
                {
                    _isClockSoundMinuteBell = value;
                }
            }
        }

        private bool _isClockSoundHourBell = true;

        /// <summary>Whether the countdown's hpur bell sound is switched on (a sound launched every hour when 
        /// the countdown is running).</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public virtual bool IsClockSoundHourBell
        {
            get { return _isClockSoundHourBell; }
            set
            {
                if (value != _isClockSoundHourBell)
                {
                    _isClockSoundHourBell = value;
                }
            }
        }




        #endregion Sounds 

        #region Common

        /// <summary>Indicates whether the timer is currently running or not.
        /// <para>True is returned if either a stopwatch is running, a countdown
        /// is running, or the current time is displayed.</para></summary>
        public bool IsRunning
        {
            get{
                return IsStopwatchRunning || IsCountdownRunning ||
                    IsClockRunning; }
        }

        private int _timerIntervalMs = 5;

        /// <summary>Timer interval, in Milliseconds. </summary>
        protected int TimerIntervalMs
        {
            get { return _timerIntervalMs; }
            set
            {
                if (value > 1)
                {
                    _timerIntervalMs = value;
                    if (value != timer1.Interval)
                        timer1.Interval = value;
                }
            }
        }





        private bool _isSilent = true;

        /// <summary>Flag specifying whether the complete timer is in a silent mode.</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public virtual bool IsSilent
        {
            get { return _isSilent; }
            set
            {
                if (value != _isSilent)
                {
                    _isSilent = value;
                    if (value)
                    {
                        chkSwSilent.Checked = true;
                        chkCdSilent.Checked = true;
                    }
                    else
                    {
                        chkSwSilent.Checked = false;
                        chkCdSilent.Checked = false;
                    }
                }
            }
        }


        /// <summary>Launches an info message.</summary>
        public void ReportInfo(string message)
        {
            new FadingMessage("Info:", message, 6000, 0.4);
        }

        /// <summary>Launches a warning message.</summary>
        public void ReportWarning(string message)
        {
            new FadingMessage("WARNING:", message, 6000, 0.4);
        }

        /// <summary>Launches a warning message.</summary>
        public void ReportError(string message)
        {
            new FadingMessage("ERROR!", message, 6000, 0.4);
        }



        #endregion Common

        


        #region Stopwatch


        #region Stopwatch.Data

        private StopWatch1 _swStopwatch;

        /// <summary>Stopwatch used to measure elapsed time for the displayed stopwatch.</summary>
        protected StopWatch1 SwStopwatch
        {
            get {
                if (_swStopwatch == null)
                    _swStopwatch = new StopWatch1();
                return _swStopwatch;
            }
        }


        /// <summary>Indicates whether the countdown timer is running or not.</summary>
        public bool IsStopwatchRunning
        {
            get { return SwStopwatch.IsRunning; }
        }

        protected bool _isStopwatchShown = true;

        /// <summary>Specifies whether the stopwatch is shown or not.</para></summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool IsStopwatchShown
        {
            get { return _isStopwatchShown; }
            set
            {
                if (value != _isStopwatchShown)
                {
                    if (value == false && IsStopwatchRunning)
                    {
                        // Stopwatch is running, it can not be hidden; revert and show message:
                        ReportWarning("Stopwatch is currently running and it can not be hidden.\n\n"
                            + "In order to hide the stopwatch, stop or reset it first!\n");
                        _isStopwatchShown = false;  // in order for the next (reverting) action to have correct effect
                        IsStopwatchShown = true;
                    } else
                    {
                        _isStopwatchShown = value;
                        if (!_isStopwatchShown)
                        {
                            // Stopwatch will be hidden, check if this is legal and eventually show
                            // another gadget:
                            if (! (IsClockShown || IsCountdownShown))
                            {
                                if (IsCountdownRunning)
                                {
                                    IsCountdownShown = true;
                                    ReportInfo("Countdown timer will be displayed instead of stopwatch.");
                                }
                                else
                                {
                                    IsClockShown = true;
                                    ReportInfo("Clock will be displayed instead of stopwatch.");
                                }
                            }
                            IsStopwatchMain = false;
                        } else
                        {
                            // Stopwatch is displayed:
                            if (!(IsCountdownShown || IsClockShown))
                            {
                                IsStopwatchMain = true;
                            }
                        }
                        flowSwOuter.Visible = _isStopwatchShown;
                        menuSw.Checked = _isStopwatchShown;
                    }
                }
            }
        }

        protected bool _isStopwatchMain = false;

        /// <summary>Specifies whether the stopwatch is the main gadget of the three.</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool IsStopwatchMain
        {
            get { return _isStopwatchMain; }
            set
            {
                if (value != _isStopwatchMain)
                {
                    _isStopwatchMain = value;
                    if (_isStopwatchMain)
                    {
                        if (!IsStopwatchShown)
                            IsStopwatchShown = true;
                        IsCountdownMain = false;
                        IsClockMain = false;
                    } else if (!(IsCountdownMain || IsClockMain))
                    {
                        // Main must be transferred to some other gadget:
                        if (IsCountdownShown)
                            IsCountdownMain = true;
                        else if (IsClockShown)
                        {
                            IsClockMain = true;
                        } else
                        {
                            if (IsStopwatchShown)
                            {
                                ReportWarning("Only the stopwatch is still shown, it willl remain the main gadget.");
                                IsStopwatchMain = true;
                            }
                            else
                            {
                                ReportError("No gadgets are shown.\n" +
                                    "Action will be reverted and stopwatch will remain the main gadget.");
                                IsStopwatchShown = true;
                                IsStopwatchMain = true;
                            }
                        }
                    }
                    if (_isStopwatchMain)
                    {
                        lblSwTitle.BackColor = ColorBgTitleMain;
                        lblSwTitle.ForeColor = ColorFgTitleMain;
                    } else
                    {
                        lblSwTitle.BackColor = ColorBgTitle;
                        lblSwTitle.ForeColor = ColorFgTitle;
                    }
                }
            }
        }


        /// <summary>Gets the current time span measured by the stopwatch.</summary>
        public TimeSpan StopwatchTotalTimeSpan
        { get { return this.SwStopwatch.TotalTimeSpan; } }


        /// <summary>Gets the current time span measured by the stopwatch.</summary>
        public double StopWatchTotalTime
        { get { return this.SwStopwatch.TotalTime; } }

 





        /// <summary>Updates fonts that are dependent on the main display font, according to
        /// this font.</summary>
        public void UpdateDisplayFonts()
        {
            double sizeMilli = DisplayFont.SizeInPoints * MilliSecondsSizeRatio;
            if (sizeMilli < MinDispayFontSizeInPoints && MinDispayFontSizeInPoints <= DisplayFont.SizeInPoints)
                sizeMilli = MinDispayFontSizeInPoints;
            if (sizeMilli < DisplayFont.SizeInPoints)
            {
                FontMilli = new Font(DisplayFont.FontFamily, (float)sizeMilli, DisplayFont.Style, GraphicsUnit.Point);
            }
        }

        Font _fontMilli = null;
        
        /// <summary>Font used in timer's displays for milliseconds.</summary>
        protected Font FontMilli
        { get { return _fontMilli; } set { _fontMilli = value; } }

        protected Font _displayFont = null;

        /// <summary>Font used in timerr's displays.</summary>
        protected Font DisplayFont
        {
            get { return _displayFont;  }
            set {
                if (value == null)
                    return;
                else if (value != _displayFont)
                {
                    _displayFont = value;
                    UpdateDisplayFonts();
                    UpdateDisplayAppearance();
                }
            }
        }

        private double _millisecondsSizeRatio = 0.5;

        /// <summary>Ratio between the size of millisecond digits and other digits on the stopwatch.</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public double MilliSecondsSizeRatio
        {
            get { return _millisecondsSizeRatio; }
            set {
                if (value < 0.01)
                    value = 0.01;
                else if (value > 1.0)
                value = 1.0;
                if (value != _millisecondsSizeRatio)
                {
                    _millisecondsSizeRatio = value;
                    UpdateDisplayAppearance();
                }
            }
        }

        private double _minDispayFontSizeInPoints = 2.0;

        /// <summary>Minimal size of display font, in points.</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public double MinDispayFontSizeInPoints
        { get { return _minDispayFontSizeInPoints; }
            protected set { _minDispayFontSizeInPoints = value; } }






        private bool _swShowMilliSeconds = true;

        /// <summary>Whether milliseconds shoulld be shown.</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool SwShowMilliSeconds
        {
            get { return _swShowMilliSeconds; }
            set
            {
                if (value != _swShowMilliSeconds)
                {
                    _swShowMilliSeconds = value;
                    if (this.Visible)
                    {
                        // Make change visible:
                        UpdateDisplayAppearance();
                    }
                    chkSwDisplayMilliseconds.Checked = value;
                }
            }
        }



        /// <summary>Whether hours shoulld be shown when they are zero.</summary>
        private bool _swShowHoursWhenZero = true;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool SwShowHoursWhenZero
        {
            get { return _swShowHoursWhenZero; }
            set
            {
                if (value != _swShowHoursWhenZero)
                {
                    _swShowHoursWhenZero = value;
                    if (this.Visible)
                    {
                        // Make change visible:
                        UpdateDisplayAppearance();
                        UpdateDisplays();
                    }
                    chkSwDisplayHours.Checked = value;
                }
            }
        }




        private Color _colorBgTitleMain = Color.LightBlue;

        private Color _colorBgTitle = Color.Transparent;

        private Color _colorFgTitleMain = Color.Black;

        private Color _colorFgTitle = Color.Black;


        /// <summary>Background color for main timer title.</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color ColorBgTitleMain { get { return _colorBgTitleMain; } set { _colorBgTitleMain = value; } }

        /// <summary>Foreground color for main timer title.</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color ColorFgTitleMain { get { return _colorFgTitleMain; } set { _colorFgTitleMain = value; } }

        /// <summary>Background color for timer title - not main.</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color ColorBgTitle { get { return _colorBgTitle; } set { _colorBgTitle = value; } }

        /// <summary>Foreground color for main timer title.</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color ColorFgTitle { get { return _colorFgTitle; } set { _colorFgTitle = value; } }




        private Color _colorBgWarning = Color.Red;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color ColorBgWarning { get { return Color.Red; }
          set { _colorBgWarning = value; } }


        private Color _colorBgOk = Color.Green;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color ColorBgOk {
            get { return _colorBgOk; }
        set { _colorBgOk = value; } }


        public Color ColorInvisible { get { return Color.Red; } }



        private Color _displayBg = Color.PaleGoldenrod;

        /// <summary>Display background color.</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color DisplayBg
        { get { return _displayBg; } protected set { _displayBg = value; } }

        private Color _displayFg = Color.SeaGreen;

        /// <summary>Display foreground color.</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color DisplayFg
        { get { return _displayFg; } protected set { _displayFg = value; } }



        private string _swStartText = "Start";

        /// <summary>Text that is written on the stopwatch start button.</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string SwStartText {
            get { return _swStartText; }
            set { _swStartText = value; }
        }

        private string _swStopText = "Pause";

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string SwStopText {
            get { return _swStopText; }
            set { _swStopText = value; }
        }

        private string _swResetText = "Reset";

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string SwResetText { get { return _swResetText; }
            protected set { _swResetText = value; }
        }



        private Color _swStartColor = Color.Green;

        /// <summary>Background color for stopwatch start button.</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color SwStartColor
        {
            get { return _swStartColor; }
            protected set { _swStartColor = value; }
        }


        /// <summary>Background color for stopwatch stop button.</summary>
        private Color _swStopColor = Color.LightSalmon;


        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color SwStopColor { get { return _swStopColor; }
            protected set { _swStopColor = value; } }


        private Color _swResetColor = Color.LightSalmon;


        /// <summary>Background color for stopwatch reset button.</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color SwResetColor { get { return _swStopColor; }
            protected set { _swResetColor = value; } }


        private Color _controlsFgClosed = Color.Black;



        /// <summary>Foreground color for controls opener when controls are closed.</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color ControlsFgClosed { get { return _controlsFgClosed; }
            protected set { _controlsFgClosed = value; } }

        private Color _controlsFgOpen = Color.Blue;

        /// <summary>Foreground color for controls opener when controls are opened.</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color ControlsFgOpen { get { return _controlsFgOpen; }
            protected set { _controlsFgOpen = value; } }

        private string _controlsClosedText = "▼";

        /// <summary>Tect for controls opener when controls are closed.</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string ControlsClosedText { get { return _controlsClosedText; }
            protected set { _controlsClosedText = value; } }

        private string _ontrolsOpenText = "▲";

        /// <summary>Tect for controls opener when controls are opened.</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string ControlsOpenText { get { return _ontrolsOpenText; }
            protected set { _ontrolsOpenText = value; } }


        private bool _swControlsOpen = false;

        /// <summary>Wheether controls are opened or not.</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool IsSwControlsOpened
        {
            get { return _swControlsOpen; }
            protected set {
                if (value != _swControlsOpen)
                {
                    _swControlsOpen = value;
                    UpdateSwControls();
                }
            }
        }





        #endregion Stopwatch.Data


        #region Stopwatch.Actions

        /// <summary>Updates timer's displays.</summary>
        protected virtual void UpdateDisplays()
        {
            UpdateSwDisplay();
            UpdateCdDisplay();
            UpdateClockDisplay();
        }

        /// <summary>Refreshes stopwatch's display according to elapsed time.</summary>
        protected virtual void UpdateSwDisplay()
        {
            TimeSpan elapsedTime = StopwatchTotalTimeSpan; // SwStopwatch.TotalTimeSpan;
            int wholeHours = (int)M.floor(elapsedTime.Hours);
            dispSwHours.Text = (wholeHours).ToString("00");
            dispSwHoursSeparator.Text = ":";
            if (wholeHours == 0 && !SwShowHoursWhenZero)
            {
                dispSwHours.Visible = false;
                dispSwHoursSeparator.Visible = false;
            } else
            {
                dispSwHours.Visible = true;
                dispSwHoursSeparator.Visible = true;
            }
            dispSwMin.Text = elapsedTime.Minutes.ToString("00");
            dispSwMinSeparator.Text = ":";
            dispSwSec.Text = elapsedTime.Seconds.ToString("00");
            dispSwSecDecimalPoint.Text = ".";
            dispSwMilliSec.Text = elapsedTime.Milliseconds.ToString("000");
        }


        /// <summary>Updates stopwatch controls according to internal properties.</summary>
        protected virtual void UpdateSwControls()
        {
            if (IsSwControlsOpened)
            {
                btnSwControls.Text = ControlsOpenText;
                btnSwControls.ForeColor = ControlsFgOpen;
                boxSwControls.Visible = true;
            } else
            {
                btnSwControls.Text = ControlsClosedText;
                btnSwControls.ForeColor = ControlsFgClosed;
                boxSwControls.Visible = false;
            }
        }
        

        /// <summary>Updates fonts on the timer's displays.</summary>
        protected virtual void UpdateDisplayAppearance()
        {
            Font dispFont = DisplayFont;

            // Update controls containning inormation dependent on the display font:
            this.fontSelectorSw.SelectedFont = dispFont;
            this.fontSelectorCd.SelectedFont = dispFont;
            this.numSwFontSize.Value = (decimal)dispFont.SizeInPoints;
            this.numCdFontSize.Value = (decimal)dispFont.SizeInPoints;

            // Update appearance of the stopwatch display:
            flowSwDisplay.BackColor = DisplayBg;
            dispSwHours.Font = dispFont;
            dispSwHoursSeparator.Font = dispFont;
            dispSwMin.Font = dispFont;
            dispSwMinSeparator.Font = dispFont;
            dispSwSec.Font = dispFont;
            dispSwSecDecimalPoint.Font = FontMilli;
            dispSwMilliSec.Font = FontMilli;
            dispSwHours.ForeColor = DisplayFg;
            dispSwHoursSeparator.ForeColor = DisplayFg;
            dispSwMin.ForeColor = DisplayFg;
            dispSwMinSeparator.ForeColor = DisplayFg;
            dispSwSec.ForeColor = DisplayFg;
            dispSwSecDecimalPoint.ForeColor = DisplayFg;
            dispSwMilliSec.ForeColor = DisplayFg;

            // Update appearance of the countdown display:
            //flowCdDisplay.BackColor = DisplayBg;   // TODO!!

            dispCdHours.Font = dispFont;
            dispCdHoursSeparator.Font = dispFont;
            dispCdMin.Font = dispFont;
            dispCdMinSeparator.Font = dispFont;
            dispCdSec.Font = dispFont;
            dispCdSecDecimalPoint.Font = FontMilli;
            dispCdMilliSec.Font = FontMilli;
            dispCdHours.ForeColor = DisplayFg;
            dispCdHoursSeparator.ForeColor = DisplayFg;
            dispCdMin.ForeColor = DisplayFg;
            dispCdMinSeparator.ForeColor = DisplayFg;
            dispCdSec.ForeColor = DisplayFg;
            dispCdSecDecimalPoint.ForeColor = DisplayFg;
            dispCdMilliSec.ForeColor = DisplayFg;


            // Whether milliseconds are shown on the stopwatch:
            if (SwShowMilliSeconds)
            {
                dispSwMilliSec.Visible = true;
                dispSwSecDecimalPoint.Visible = true;
            }
            else
            {
                dispSwMilliSec.Visible = false;
                dispSwSecDecimalPoint.Visible = false;
            }

            // Whether milliseconds are shown on the countdown:
            if (CdShowMilliSeconds)
            {
                dispCdMilliSec.Visible = true;
                dispCdSecDecimalPoint.Visible = true;
            }
            else
            {
                dispCdMilliSec.Visible = false;
                dispCdSecDecimalPoint.Visible = false;
            }


        }


        /// <summary>Updates appearance of the stopwatch buttons.</summary>
        protected virtual void UpdateSwButtonsAppearance()
        {
            if (IsStopwatchRunning)
            {
                btnSwStartStop.BackColor = SwStopColor;
                btnSwStartStop.Text = SwStopText;
            }
            else
            {
                btnSwStartStop.BackColor = SwStartColor;
                btnSwStartStop.Text = SwStartText;
            }
            btnSwReset.Text = SwResetText;
            btnSwReset.BackColor = SwResetColor;
            UpdateSwDisplay();
        }


        #endregion Actions


        #region StopWatch.EventHandling


        // private bool _timerHandlerBusy = false;


        protected TimeSpan _swPreviousSpan;
        protected TimeSpan _swCurrentSpan;
        protected TimeSpan _cdPreviousRemainingSpan;
        protected TimeSpan _cdCurrentRemainingSpan;

        protected DateTime _clockPreviousWallclockTime;
        protected DateTime _clockCurrentWalclockTime;

        /// <summary>Timer event handlesr, executed on every tick of thr timer.</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                if (IsClockRunning)
                {
                    // Register clock timer event; This wil update display and raise event that can
                    // be consumed by users of this timer:
                    OnClockTimerTick();
                    _clockPreviousWallclockTime = _clockCurrentWalclockTime;
                    _clockCurrentWalclockTime = WallclockTime;  // set by OnClockTimerTick() -> UpdateClockDisplay()
                    // If there was a whole seconds switch (or whole minutes or whole hours), raise the 
                    // appropriate event:
                    if (_clockCurrentWalclockTime.Second != _clockPreviousWallclockTime.Second)
                    {
                        OnClockSecondTick();
                        if (_clockCurrentWalclockTime.Minute != _clockPreviousWallclockTime.Minute)
                            OnClockMinuteBell();
                        if (_clockCurrentWalclockTime.Hour != _clockPreviousWallclockTime.Hour)
                            OnClockHourBell();
                    }
                }
                if (IsStopwatchRunning)
                {
                    // Register stopwatch timer event; This wil update display and raise event that can
                    // be consumed by users of this timer:
                    OnStopwatchTimerTick();
                    _swPreviousSpan = _swCurrentSpan;
                    _swCurrentSpan = StopwatchTotalTimeSpan;
                    // If there was a whole seconds switch (or whole minutes or whole hours), raise the 
                    // appropriate alarm:
                    double wholeSecondsPrevious = (int) Math.Floor(_swPreviousSpan.TotalSeconds);
                    double wholeSecondsCurrent = (int) Math.Floor(_swCurrentSpan.TotalSeconds);
                    if (wholeSecondsCurrent != wholeSecondsPrevious)
                    {
                        OnStopwatchSecondTick();
                        if (_swCurrentSpan.Minutes != _swCurrentSpan.Minutes)
                            OnStopwatchMinuteBell();
                        if (_swCurrentSpan.Hours != _swCurrentSpan.Hours)
                            OnStopwatchHourBell();
                    }
                }
                if (IsCountdownRunning)
                {
                    // Register stopwatch timer event; This wil update display and raise event that can
                    // be consumed by users of this timer:
                    OnCountdownTimerTick();
                    UpdateCdDisplay();
                    _cdPreviousRemainingSpan = _cdCurrentRemainingSpan;
                    _cdCurrentRemainingSpan = CountdownRemainingTimeSpan;
                    if (_cdCurrentRemainingSpan.Ticks <= 0)
                    {
                        // Countdown has finished, stop its stopwatch and raise
                        // the appropriate event:
                        CdStopwatch.Stop();
                        OnCountdownFinished();
                    } else
                    {
                        // If there was a whole seconds switch (or whole minutes or whole hours), raise the 
                        // appropriate alarm:
                        double wholeSecondsPrevious = (int)Math.Floor(_cdPreviousRemainingSpan.TotalSeconds);
                        double wholeSecondsCurrent = (int)Math.Floor(_cdCurrentRemainingSpan.TotalSeconds);
                        if (wholeSecondsCurrent != wholeSecondsPrevious)
                        {
                            OnCountdownSecondTick();
                            if (_cdCurrentRemainingSpan.Minutes != _cdCurrentRemainingSpan.Minutes)
                                OnCountdownMinuteBell();
                            if (_cdCurrentRemainingSpan.Hours != _cdCurrentRemainingSpan.Hours)
                                OnCountdownHourBell();
                        }
                    }
                }
            }
            catch (Exception ex) {
                ++NumTimerErrorReports;
                if (NumTimerErrorReports <= MaxTimerErrorReports)
                {
                    string message = Environment.NewLine + "Internal timer's tick event was not processed correctly." + Environment.NewLine
                            + "  Details: " + Environment.NewLine + ex.Message + Environment.NewLine;
                    if (OutputLevel >= 1)
                        Console.WriteLine(Environment.NewLine + "ERRROR: internal timer's tick event was not processed correctly." + Environment.NewLine
                            + "  Details: " + ex.Message);
                    if (NumTimerErrorReports == MaxTimerErrorReports)
                    {
                        message += Environment.NewLine + "Further messages of this kind will be omitted." + Environment.NewLine;
                        Console.WriteLine("Further messages of this kind will be omitted.");
                    }
                    Console.WriteLine(Environment.NewLine);
                    ReportError(message);
                }
                throw; }
            finally {  }
        }


        /// <summary>Default maximal number of times a report about timer tick exception is launched.</summary>
        public static int DefaultMaxNumTimerErrorReports = 10;

        private int _maxTimerErrorReports = DefaultMaxNumTimerErrorReports;

        /// <summary>Macimal number of times a report on exception thrown within the timer tick event can be launched.</summary>
        protected int MaxTimerErrorReports
        {
            get { return _maxTimerErrorReports; }
            set { _maxTimerErrorReports = value; }
        }


        private int _numTimerErrorReports = 0;

        /// <summary>Number of times a report on exception thrown within the timer tick event has already been launched.</summary>
        protected int NumTimerErrorReports { get { return _numTimerErrorReports; }
            set { _numTimerErrorReports = value; }
        }

        

        private void StopWatchControl_Load(object sender, EventArgs e)
        {
            UpdateSwControls();
        }


        /// <summary>Toggles opening/closing stopwatch's controls panel.</summary>
        private void btnSwControls_Click(object sender, EventArgs e)
        {
            if (IsSwControlsOpened)
            {
                IsSwControlsOpened = false;
            }
            else
            {
                IsSwControlsOpened = true;
            }
        }


        /// <summary>Toggles silent mode.</summary>
        private void chkSwSilent_CheckedChanged(object sender, EventArgs e)
        {
            if (chkSwSilent.Checked == true)
                IsSilent = true;
            else
                IsSilent = false;
        }


        /// <summary>Start / stop button pressed.</summary>
        private void btnSwStartStop_Click(object sender, EventArgs e)
        {
            StopwatchStartOrStop();
        }


        private void btnSwReset_Click(object sender, EventArgs e)
        {
            StopwatchResetIt();
        }



        /// <summary>Fired on size change of the outer panel of the control. Currently not used, but this event
        /// can be used fo triger the appropriate actions (e.g. adjust the size) of containing controls. Now 
        /// containing controls are adjusted automatically (by appropriate property setting).</summary>
        private void flowPanelOuter_SizeChanged(object sender, EventArgs e)
        {
            //if (true)
            //{
            //    Console.WriteLine(Environment.NewLine
            //        + "StopWatchControl: size of the outer panel was changed, width = " + flowPanelOuter.Width
            //        + ", height = " + flowPanelOuter.Height + Environment.NewLine);
            //}
        }
        

        /// <summary>Trigered by font selector control when a new font was selected in it.</summary>
        private void fontSelectorSw_FontSelected(object sender, FontEventArgs args)
        {
            Font selectedFont = null;
            if (args != null)
                selectedFont = args.Font;
            if (selectedFont != null)
                DisplayFont = selectedFont;
        }


        /// <summary>Not used.</summary>
        private void fontSelector_Load(object sender, EventArgs e)
        {

        }


        /// <summary>When font is selected by the font selector, it sets the display 
        /// font in this control to the selected font.</summary>
        /// <param name="sender">Control that raised the event.</param>
        /// <param name="args">Contains the selected font.</param>
        private void fontSelector_FontSelected(object sender, FontEventArgs args)
        {
            if (args != null)
                if (args.Font != null)
                    this.DisplayFont = args.Font;
        }


        /// <summary>Changes size of the display font.</summary>
        /// <param name="sizeInPoints">New size of the display font, in points.</param>
        public void SetDisplayFontSize(double sizeInPoints)
        {
            SetDisplayFontSize((decimal) sizeInPoints);
        }

        /// <summary>Changes size of the display font.</summary>
        /// <param name="sizeinPointsDecimal">New size of the display font, in points.</param>
        public void SetDisplayFontSize(decimal sizeInPointsDecimal)
        {
            if (sizeInPointsDecimal < (decimal) MinDispayFontSizeInPoints)
                sizeInPointsDecimal = (decimal) MinDispayFontSizeInPoints;
            Font prewFont = DisplayFont;
            if (prewFont != null)
                if (sizeInPointsDecimal != (decimal) DisplayFont.SizeInPoints)
                {
                    Font newFont = new Font(prewFont.FontFamily, (float) sizeInPointsDecimal,
                        prewFont.Style, GraphicsUnit.Point);
                    DisplayFont = newFont;
                }
        }

        private void numFontSize_ValueChanged(object sender, EventArgs e)
        {
            SetDisplayFontSize(numSwFontSize.Value);

            //decimal sizeInPointsDecimal = numSwFontSize.Value;
            //if (sizeInPointsDecimal < (decimal)MinDispayFontSizeInPoints)
            //    sizeInPointsDecimal = (decimal)MinDispayFontSizeInPoints;
            //Font prewFont = DisplayFont;
            //if (prewFont != null)
            //    if (sizeInPointsDecimal != (decimal)DisplayFont.SizeInPoints)
            //    {
            //        Font newFont = new Font(prewFont.FontFamily, (float)numSwFontSize.Value,
            //            prewFont.Style, GraphicsUnit.Point);
            //        DisplayFont = newFont;
            //    }

        }

        /// <summary>Selects the contents for easy changing by typing.</summary>
        private void numSwFontSize_Enter(object sender, EventArgs e)
        {
            numSwFontSize.Select(0, 100);
        }

        /// <summary>Selects the contents for easy changing by typing.</summary>
        private void numSwFontSize_Click(object sender, EventArgs e)
        {
            numSwFontSize.Select(0, 100);
        }


        private void chkSwDisplayMilliseconds_CheckedChanged(object sender, EventArgs e)
        {
            SwShowMilliSeconds = chkSwDisplayMilliseconds.Checked;
        }

        private void chkSwDisplayHours_CheckedChanged(object sender, EventArgs e)
        {
            SwShowHoursWhenZero = chkSwDisplayHours.Checked;
        }



        #endregion StopWatch.EventHandling


        #region StopWatch.Events


        /// <summary>Event that is fired on each stopwatch's timer tick (very often, in the range of milliseconds).
        /// <para>This event is usually useful for debugging or for very fine control.</para></summary>
        public event EventHandler StopwatchTimerTick;

        /// <summary>Called when the timer event fires  (very often, in the range of milliseconds). 
        /// Raises the <see cref="StopwatchTimerTick"/> event and updates the display.
        /// <para>Sounds can not be assigned to this event.</para></summary>
        protected virtual void OnStopwatchTimerTick()
        {
            UpdateSwDisplay();
            if (StopwatchTimerTick != null)
                StopwatchTimerTick(this, new EventArgs());
        }


        /// <summary>Starts (if it is not running) or stops (if it is running) the stopwatch.</summary>
        public virtual void StopwatchStartOrStop()
        {
            if (SwStopwatch.IsRunning)
            {
                StopwatchStop();
            }
            else
            {
                StopwatchStart();
            }
        }


        /// <summary>Event that is fired when the stopwatch starts.</summary>
        public event EventHandler StopwatchStarted;


        /// <summary>Starts the stopwatch.
        /// <para>Its internal stopwatch starts running and the <see cref="StopwatchStarted"/> event is raised.</para>
        /// <para>Eventually the appropriate sound is played (if not <see cref="IsSilent"/> and if <see cref="IsSwSoundButtons"/>
        /// <para>If the stopwatch is already runninng or it has already finished, call to this function has no effect.</para></summary>
        public virtual void StopwatchStart()
        {
            if (!IsStopwatchRunning)
            {
                SwStopwatch.Start();
                timer1.Enabled = true;
                if (!IsSilent && IsSwSoundButtons)
                    PlaySoundStart();
                UpdateSwButtonsAppearance();
                if (StopwatchStarted != null)
                    StopwatchStarted(this, new EventArgs());
            }
        }


        /// <summary>Event that is fired when the stopwatch stops.</summary>
        public event EventHandler StopwatchStopped;

        /// <summary>Stops the stopwatch timer. 
        /// <para>Its internal stopwatch stops running and the <see cref="StopwatchStopped"/> event is raised.</para>
        /// <para>Eventually the appropriate sound is played (if not <see cref="IsSilent"/> and if <see cref="IsSwSoundButtons"/>)</para>
        /// <para>If the stopwatch is paused or it has already finished, call to this function has no effect.</para></summary>
        public virtual void StopwatchStop()
        {
            if (IsStopwatchRunning)
            {
                SwStopwatch.Stop();
                if (!IsRunning)
                    timer1.Enabled = false;  // stop triggering timer events
                if (!IsSilent && IsSwSoundButtons)
                    PlaySoundStop();
                UpdateSwButtonsAppearance();
                if (StopwatchStopped != null)
                    StopwatchStopped(this, new EventArgs());
            }
        }



        /// <summary>Event that is raised when the counter is reset.</summary>
        public event EventHandler StopwatchReset;


        /// <summary>Resets the stopwatch.
        /// <para>Its internal stopwatch stops running (if it runs) and the <see cref="StopwatchReset"/> event is raised.</para>
        /// <para>Eventually the appropriate sound is played (if not <see cref="IsSilent"/> and if <see cref="IsSwSoundButtons"/>)</para></summary>
        protected virtual void StopwatchResetIt()
        {

            if (SwStopwatch.IsRunning)
                SwStopwatch.Stop();
            SwStopwatch.Reset();
            if (!IsRunning)
                timer1.Enabled = false;
            UpdateSwDisplay();
            UpdateSwButtonsAppearance();
            if (StopwatchReset != null)
                StopwatchReset(this, new EventArgs());
        }




        /// <summary>Event that is fired when second count changes to a whole value (seconds tick).</summary>
        public event EventHandler StopwatchSecondTick;

        /// <summary>Called when the stopwatch seconds count reaches a whole value (seconds tick). 
        /// Raises the <see cref="StopwatchSecondTick"/> event and eventually plays the
        /// appropriate "second tick" sound (if not <see cref="IsSilent"/> and if <see cref="IsSwSoundSecondTick"/>).</summary>
        protected virtual void OnStopwatchSecondTick()
        {
            if (!IsSilent && IsSwSoundSecondTick && !IsClockRunning)
            {
                PlaySoundSecondTick();
            }
            if (StopwatchSecondTick != null)
                StopwatchSecondTick(this, new EventArgs());
        }


        /// <summary>Event that is fired when minutes count changes to a whole value (minutes signal).</summary>
        public event EventHandler StopwatchMinuteBell;

        /// <summary>Called when the stopwatch minutes count reaches a whole value (minutes signal). 
        /// Raises the <see cref="StopwatchMinuteBell"/> event and eventually plays the
        /// appropriate "minutes bell" sound (if not <see cref="IsSilent"/> and if <see cref="IsSwSoundMinuteBell"/>).</summary>
        protected virtual void OnStopwatchMinuteBell()
        {
            if (!IsSilent && IsSwSoundMinuteBell)
            {
                PlaySoundMinuteBell();
            }
            if (StopwatchMinuteBell != null)
                StopwatchMinuteBell(this, new EventArgs());
        }

        /// <summary>Event that is fired when minutes count changes to a whole value (minute switch signal).</summary>
        public event EventHandler StopwatchHourBell;

        /// <summary>Called when the stopwatch hours count reaches a whole value (hour switch signal). 
        /// Raises the <see cref="StopwatchHourBell"/> event and eventually plays the
        /// appropriate "hours bell" sound (if not <see cref="IsSilent"/> and if <see cref="IsSwSoundHourBell"/>).</summary>
        protected virtual void OnStopwatchHourBell()
        {
            if (!IsSilent && IsSwSoundHourBell)
            {
                PlaySoundHourBell();
            }
            if (StopwatchHourBell != null)
                StopwatchHourBell(this, new EventArgs());
        }



        #endregion StopWatch.Events 



        #endregion Stopwatch

        


        #region Cowntdown


        #region Countdown.Data


        private StopWatch1 _cdStopwatch;

        /// <summary>Stopwatch used to measure elapsed time for the displayed cowntdown.</summary>
        protected StopWatch1 CdStopwatch
        {
            get {
                if (_cdStopwatch == null)
                    _cdStopwatch = new StopWatch1();
                return _cdStopwatch;
            }
        }

        /// <summary>Indicates whether the countdown timer is running or not.</summary>
        public bool IsCountdownRunning
        {
            get { return CdStopwatch.IsRunning; }
        }


        protected bool _isCountdownShown = true;

        /// <summary>Specifies whether the countdown timer is shown or not.</para></summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool IsCountdownShown
        {
            get { return _isCountdownShown; }
            set
            {
                if (value != _isCountdownShown)
                {
                    if (value == false && IsCountdownRunning)
                    {
                        // Countdown timer is running, it can not be hidden; revert and show message:
                        ReportWarning("Countdown timer is currently running and it can not be hidden.\n\n" 
                            + "In order to hide the countdown timer, stop or reset it first!\n");
                        _isCountdownShown = false;  // in order for the next (reverting) action to have correct effect
                        IsCountdownShown = true;
                    }
                    else
                    {
                        _isCountdownShown = value;
                        if (!_isCountdownShown)
                        {
                            // Countdown timer will be hidden, check if this is legal and eventually show
                            // another gadget:
                            if (!(IsClockShown || IsStopwatchShown))
                            {
                                if (IsStopwatchRunning)
                                {
                                    IsStopwatchShown = true;
                                    ReportInfo("Stopwatch will be displayed instead of countdown timer.");
                                } else
                                {
                                    IsClockShown = true;
                                    ReportInfo("Clock will be displayed instead of countdown timer.");
                                }
                            }
                            IsCountdownMain = false;
                        }
                        else
                        {
                            // Countdown is displayed:
                            if (!(IsStopwatchShown || IsClockShown))
                            {
                                IsStopwatchMain = true;
                            }
                        }
                        flowCdOuter.Visible = _isCountdownShown;
                        menuCd.Checked = _isCountdownShown;
                    }
                }
            }
        }

        protected bool _isCountdownhMain = false;

        /// <summary>Specifies whether the countdown timer is the main gadget of the three.</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool IsCountdownMain
        {
            get { return _isCountdownhMain; }
            set
            {
                if (value != _isCountdownhMain)
                {
                    _isCountdownhMain = value;
                    if (_isCountdownhMain)
                    {
                        if (!IsCountdownShown)
                            IsCountdownShown = true;
                        IsStopwatchMain = false;
                        IsClockMain = false;
                    }
                    else if (!(IsStopwatchMain || IsClockMain))
                    {
                        // Main must be transferred to some other gadget:
                        if (IsStopwatchShown)
                            IsStopwatchMain = true;
                        else if (IsClockShown)
                        {
                            IsClockMain = true;
                        }
                        else
                        {
                            if (IsCountdownShown)
                            {
                                ReportWarning("Only the countdown timer is still displayed, it willl remain the main gadget.");
                                IsCountdownMain = true;
                            } else
                            {
                                ReportError("No gadgets are shown.\n" +
                                    "Action will be reverted and countdown timer will remain the main gadget.");
                                IsCountdownShown = true;
                                IsCountdownMain = true;
                            }
                        }
                    }
                    if (_isCountdownhMain)
                    {
                        lblCdTitle.BackColor = ColorBgTitleMain;
                        lblCdTitle.ForeColor = ColorFgTitleMain;
                    }
                    else
                    {
                        lblCdTitle.BackColor = ColorBgTitle;
                        lblCdTitle.ForeColor = ColorFgTitle;
                    }
                }
            }
        }



        //private bool _playSystemSoundOnCountdownFinished = true;

        ///// <summary>Whether a system sound is played whwn countdown finishes.</summary>
        //public bool PlaySystemSoundOnCountdownFinished
        //{
        //    get { return _playSystemSoundOnCountdownFinished; }
        //    set { _playSystemSoundOnCountdownFinished = value; }
        //}


        /// <summary>Gets the current total time span measured by the countdown's stopwatch.</summary>
        public TimeSpan CountdownTotalTimeSpan
        { get {
                TimeSpan ret = this.CdStopwatch.TotalTimeSpan;
                if (ret > CdInitialTimeSpan)
                    ret = CdInitialTimeSpan;
                return ret;
            } }


        /// <summary>Gets the current time span measured by the countdown's stopwatch.</summary>
        public double CountdownTotalTime
        { get { return CountdownTotalTimeSpan.TotalSeconds; } }


        /// <summary>Gets Countdown's remaining time span.</summary>
        /// <returns></returns>
        public TimeSpan CountdownRemainingTimeSpan
        {
            get {
            TimeSpan ret = CdInitialTimeSpan - CdStopwatch.TotalTimeSpan;
            if (ret.Ticks < 0)
                ret = new TimeSpan(0);
            return ret;
            }
        }

        /// <summary>Returns Countdown's remaining time in seconds.</summary>
        /// <returns></returns>
        public double CountdownRemainingTime
        { get { return CountdownRemainingTimeSpan.TotalSeconds; } }


        

        private int _cdInitialHours = 0;

        /// <summary>Number of hours initially set in countdown.</summary>
        protected virtual int CdInitialHours
        {
            get { return _cdInitialHours; }
            set
            {
                if (value != _cdInitialHours)
                {
                    _cdInitialHours = value;
                    UpdateCdInitialTime();
                }
            }
        }

        private int _cdInitialMinutes = 0;

        /// <summary>Number of minutes initially set in countdown.</summary>
        protected virtual int CdInitialMinutes
        {
            get { return _cdInitialMinutes; }
            set
            {
                if (value != _cdInitialMinutes)
                {
                    _cdInitialMinutes = value;
                    UpdateCdInitialTime();
                }
            }
        }


        private int _cdInitialSecods = 0;

        /// <summary>Number of seconds initially set in countdown.</summary>
        protected virtual int CdInitialSeconds
        {
            get { return _cdInitialSecods; }
            set
            {
                if (value != _cdInitialSecods)
                {
                    _cdInitialSecods = value;
                    UpdateCdInitialTime();
                }
            }
        }


        private int _cdInitialMs = 0;

        /// <summary>Number of milliseconds initially set in countdown.</summary>
        protected virtual int CdInitialMilliseconds
        {
            get { return _cdInitialMs; }
            set
            {
                if (value != _cdInitialMs)
                {
                    _cdInitialMs = value;
                    UpdateCdInitialTime();
                }
            }
        }


        private TimeSpan _cdInitialTimeSpan = new TimeSpan(0, 0, 10, 0);

        /// <summary>Initial time span set in countdown.</summary>
        protected virtual TimeSpan CdInitialTimeSpan
        {
            get { return _cdInitialTimeSpan; }
            set
            {
                if (value != _cdInitialTimeSpan)
                {
                    _cdInitialTimeSpan = value;
                    UpdateCdInitialTime();
                }
            }
        }

        /// <summary>Updates initial <see cref="TimeSpan"/> of the countdown in such a way
        /// that it corresponds to its initial hours, minutes, seconds, and miilliseconds.</summary>
        protected virtual void UpdateCdInitialTimeSpan()
        {
            CdInitialTimeSpan = new TimeSpan(0, CdInitialHours, CdInitialMinutes,
                CdInitialSeconds, CdInitialMilliseconds);
        }


        /// <summary>Updates initial hours, minutes, seconds, and milliseconds of the countdown in such a way
        /// that they correspond to its intial <see cref="TimeSpan"/>.</summary>
        protected virtual void UpdateCdInitialTime()
        {
            TimeSpan ts = CdInitialTimeSpan;
            CdInitialHours = (int)M.floor(ts.TotalHours);
            CdInitialMinutes = ts.Minutes;
            CdInitialSeconds = ts.Seconds;
            CdInitialMilliseconds = ts.Milliseconds;
        }


        private bool _cdShowMilliSeconds = true;

        /// <summary>Whether milliseconds shoulld be shown on countdown.</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool CdShowMilliSeconds
        {
            get { return _cdShowMilliSeconds; }
            set
            {
                if (value != _cdShowMilliSeconds)
                {
                    _cdShowMilliSeconds = value;
                    if (this.Visible)
                    {
                        // Make change visible:
                        UpdateDisplayAppearance();
                    }
                    chkCdDisplayMillisec.Checked = value;
                }
            }
        }



        /// <summary>Whether hours shoulld be shown on countdown when they are zero.</summary>
        private bool _cdShowHoursWhenZero = true;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool CdShowHoursWhenZero
        {
            get { return _cdShowHoursWhenZero; }
            set
            {
                if (value != _cdShowHoursWhenZero)
                {
                    _cdShowHoursWhenZero = value;
                    if (this.Visible)
                    {
                        // Make change visible:
                        UpdateDisplayAppearance();
                        UpdateDisplays();
                    }
                    chkCdDisplayHours.Checked = value;
                }
            }
        }



        private bool _isCdSoundButtonPressed = true;

        /// <summary>Whether the countdown's button sound is switched on (a sound launched every time a countdown 
        /// button with some effectt is pressed, or the same effect is achieved programatically).</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public virtual bool IsCdSoundButtons
        {
            get { return _isCdSoundButtonPressed; }
            set
            {
                if (value != _isCdSoundButtonPressed)
                {
                    _isCdSoundButtonPressed = value;
                }
            }
        }

        private bool _isCdSoundAlarm = true;

        /// <summary>Whether a countdown's alarm sound is switched on (launched when the countdown is finished,
        /// i.e. the countdown reachhes zero).</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public virtual bool IsCdSoundAlarm
        {
            get { return _isCdSoundAlarm; }
            set
            {
                if (value != _isCdSoundAlarm)
                {
                    _isCdSoundAlarm = value;
                }
            }
        }




        private string _cdStartText = "Start";

        /// <summary>Text that is written on the stopwatch start button.</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string CdStartText
        {
            get { return _cdStartText; }
            set { _cdStartText = value; }
        }

        private string _cdStopText = "Pause";

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string CdStopText
        {
            get { return _cdStopText; }
            set { _cdStopText = value; }
        }

        private string _cdResetText = "Reset";

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string CdResetText
        {
            get { return _cdResetText; }
            protected set { _cdResetText = value; }
        }

        private Color _cdStartColor = Color.Green;

        /// <summary>Background color for stopwatch start button.</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color CdStartColor
        {
            get { return _cdStartColor; }
            protected set { _cdStartColor = value; }
        }


        /// <summary>Background color for stopwatch stop button.</summary>
        private Color _cdStopColor = Color.LightSalmon;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color CdStopColor
        {
            get { return _cdStopColor; }
            protected set { _cdStopColor = value; }
        }

        /// <summary>Background color for countdown reset button.</summary>
        private Color _cdResetColor = Color.LightSalmon;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color CdResetColor
        {
            get { return _cdResetColor; }
            protected set { _cdResetColor = value; }
        }








        #endregion Cowntdown.Data



        #region Countdown.Actions

        

        /// <summary>Refreshes countdown's display according to elapsed time.</summary>
        protected virtual void UpdateCdDisplay()
        {
            TimeSpan remainingTime = CountdownRemainingTimeSpan; 
            int wholeHours = (int)M.floor(remainingTime.Hours);
            dispCdHours.Text = (wholeHours).ToString("00");
            dispCdHoursSeparator.Text = ":";
            if (wholeHours == 0 && !CdShowHoursWhenZero)
            {
                dispCdHours.Visible = false;
                dispCdHoursSeparator.Visible = false;
            }
            else
            {
                dispCdHours.Visible = true;
                dispCdHoursSeparator.Visible = true;
            }
            dispCdMin.Text = remainingTime.Minutes.ToString("00");
            dispCdMinSeparator.Text = ":";
            dispCdSec.Text = remainingTime.Seconds.ToString("00");
            dispCdSecDecimalPoint.Text = ".";
            dispCdMilliSec.Text = remainingTime.Milliseconds.ToString("000");
        }


        /// <summary>Updates countdown controls according to internal properties.</summary>
        protected virtual void UpdateCdControls()
        {
            if (IsCdControlsOpened)
            {
                btnCdControls.Text = ControlsOpenText;
                btnCdControls.ForeColor = ControlsFgOpen;
                boxCdControls.Visible = true;
            }
            else
            {
                btnCdControls.Text = ControlsClosedText;
                btnCdControls.ForeColor = ControlsFgClosed;
                boxCdControls.Visible = false;
            }
        }


        /// <summary>Updates appearance of the stopwatch buttons.</summary>
        protected virtual void UpdateCdButtonsAppearance()
        {
            if (IsCountdownRunning && CountdownRemainingTime > 0)
            {
                btnCdStartStop.BackColor = CdStopColor;
                btnCdStartStop.Text = CdStopText;
                 
            } else
            {
                btnCdStartStop.BackColor = CdStartColor;
                btnCdStartStop.Text = CdStartText;
            }
            btnCdReset.Text = CdResetText;
            btnCdReset.BackColor = CdResetColor;
            UpdateCdDisplay();
        }





        #endregion Cowntdown.Actions


        #region Cownddownn.EventHandling


        private bool _cdControlsOpen = false;

        /// <summary>Wheether controls are opened or not.</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool IsCdControlsOpened
        {
            get { return _cdControlsOpen; }
            protected set
            {
                if (value != _cdControlsOpen)
                {
                    _cdControlsOpen = value;
                    UpdateCdControls();
                }
            }
        }


        /// <summary>Toggles between open / closed control panel for the countdown.</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCdControls_Click(object sender, EventArgs e)
        {
            if (IsCdControlsOpened)
            {
                IsCdControlsOpened = false;
            }
            else
            {
                IsCdControlsOpened = true; 
            }
        }


        /// <summary>Toggles silent mode.</summary>
        private void chkCdSilent_CheckedChanged(object sender, EventArgs e)
        {
            if (chkCdSilent.Checked)
                IsSilent = true;
            else
                IsSilent = false;
        }


        private void fontSelectorCd_FontSelected(object sender, FontEventArgs args)
        {
            Font fn = null;
            if (args != null)
                fn = args.Font;
            if (fn != null)
                DisplayFont = fn;
        }



        private void btnCdStartStop_Click(object sender, EventArgs e)
        {
            CountdownStartOrStop();
        }

        private void btnCdReset_Click(object sender, EventArgs e)
        {
            CountdownResetIt();
        }

        private void chkCdDisplayMilliseconds_CheckedChanged(object sender, EventArgs e)
        {
            CdShowMilliSeconds = chkCdDisplayMillisec.Checked;
        }

        private void chkCdDisplayHours_CheckedChanged(object sender, EventArgs e)
        {
            CdShowHoursWhenZero = chkCdDisplayHours.Checked;
        }

        private void numCdFontSize_ValueChanged(object sender, EventArgs e)
        {
            SetDisplayFontSize(numCdFontSize.Value);
        }

        private void numCdFontSize_Enter(object sender, EventArgs e)
        {
            numCdFontSize.Select(0, 100);
        }

        private void numCdFontSize_Click(object sender, EventArgs e)
        {
            numCdFontSize.Select(0, 100);
        }



        #endregion Contdown.Eventhandling



        #region CountDown.Events


        /// <summary>Event that is fired on each countdown's timer tick (very often, in the range of milliseconds).
        /// <para>This event is usually usef for debugging or for very fine control.</para></summary>
        public event EventHandler CountdownTimerTick;

        /// <summary>Called when the timer event fires  (very often, in the range of milliseconds). 
        /// Raises the <see cref="CountdownTimerTick"/> event and updates the display.
        /// <para>Sounds can not be assigned to this event.</para></summary>
        protected virtual void OnCountdownTimerTick()
        {
            UpdateCdDisplay();
            if (CountdownTimerTick != null)
                CountdownTimerTick(this, new EventArgs());
        }


        /// <summary>Starts (if it is not running) or stops (if it is running) the countdown.</summary>
        public virtual void CountdownStartOrStop()
        {
            if (CdStopwatch.IsRunning)
            {
                CountdownStop();
            }
            else
            {
                CountdownStart();
            }
        }


        /// <summary>Event that is fired when the countdown starts.</summary>
        public event EventHandler CountdownStarted;

        /// <summary>Starts the countdown timer.
        /// <para>Its internal stopwatch starts running and the <see cref="CountdownStarted"/> event is raised.</para>
        /// <para>Eventually the appropriate sound is played (if not <see cref="IsSilent"/> and if <see cref="IsCdSoundButtons"/>
        /// <para>If the countdown is already runninng or it has already finished, call to this function has no effect.</para></summary>
        public virtual void CountdownStart()
        {
            if (!IsCountdownRunning && CountdownRemainingTime > 0)
            {
                CdStopwatch.Start();
                timer1.Enabled = true;
                if (!IsSilent && IsCdSoundButtons)
                    PlaySoundStart();
                UpdateCdButtonsAppearance();
                if (CountdownStarted != null)
                    CountdownStarted(this, new EventArgs());
            }
        }


        /// <summary>Event that is fired when the countdown stops.</summary>
        public event EventHandler CountdownStopped;

        /// <summary>Stops the countdown timer. 
        /// <para>Its internal stopwatch stops running and the <see cref="CountdownStopped"/> event is raised.</para>
        /// <para>Eventually the appropriate sound is played (if not <see cref="IsSilent"/> and if <see cref="IsCdSoundButtons"/>)</para>
        /// <para>If the countdown is paused or it has already finished, call to this function has no effect.</para></summary>
        public virtual void CountdownStop()
        {
            if (IsCountdownRunning && CountdownRemainingTime > 0)
            {
                CdStopwatch.Stop();
                if (!IsRunning)
                    timer1.Enabled = false;  // stop triggering timer events
                if (!IsSilent && IsCdSoundButtons)
                    PlaySoundStop();
               UpdateCdButtonsAppearance();
                if (CountdownStopped != null)
                    CountdownStopped(this, new EventArgs());
             }
        }



        /// <summary>Event that is raised when the counter is reset.</summary>
        public event EventHandler CountdownReset;


        /// <summary>Resets the countdown.
        /// <para>Its internal stopwatch stops running (if it runs) and the <see cref="CountdownReset"/> event is raised.</para>
        /// <para>Eventually the appropriate sound is played (if not <see cref="IsSilent"/> and if <see cref="IsCdSoundButtons"/>)</para></summary>
        protected virtual void CountdownResetIt()
        {

            if (CdStopwatch.IsRunning)
                CdStopwatch.Stop();
            CdStopwatch.Reset();
            if (!IsRunning)
                timer1.Enabled = false;
            UpdateCdDisplay();
            UpdateCdButtonsAppearance();
            if (CountdownReset != null)
                CountdownReset(this, new EventArgs());
        }



        /// <summary>Event that is fired when the countdown reaches 0.</summary>
        public event EventHandler CountdownFinished;

        /// <summary>Called when the countdown reaches 0 and stops counting down. 
        /// Raises the <see cref="CountdownFinished"/> event and eventually plays the
        /// appropriate alarm (if not <see cref="IsSilent"/> and if <see cref="IsCdSoundAlarm"/>).</summary>
        protected virtual void OnCountdownFinished()
        {
            UpdateCdButtonsAppearance();
            UpdateDisplayAppearance();
            if (CountdownFinished != null)
                CountdownFinished(this, new EventArgs());
            if (!IsSilent && IsCdSoundAlarm)
            {
                PlaySoundAlarmCountdown();
            }
        }


        /// <summary>Event that is fired when second count changes to a whole value (seconds tick).</summary>
        public event EventHandler CountdownSecondTick;

        /// <summary>Called when the countdown seconds count reaches a whole value (seconds tick). 
        /// Raises the <see cref="CountdownSecondTick"/> event and eventually plays the
        /// appropriate "second tick" sound (if not <see cref="IsSilent"/> and if <see cref="IsCdSoundSecondTick"/>).</summary>
        protected virtual void OnCountdownSecondTick()
        {
            if (!IsSilent && IsCdSoundSecondTick && !IsStopwatchRunning && !IsClockRunning)
            {
                PlaySoundSecondTick();
            }
            if (CountdownSecondTick != null)
                CountdownSecondTick(this, new EventArgs());
        }


        /// <summary>Event that is fired when minutes count changes to a whole value (minutes signal).</summary>
        public event EventHandler CountdownMinuteBell;

        /// <summary>Called when the countdown minutes count reaches a whole value (minutes signal). 
        /// Raises the <see cref="CountdownMinuteBell"/> event and eventually plays the
        /// appropriate "minutes bell" sound (if not <see cref="IsSilent"/> and if <see cref="IsCdSoundMinuteBell"/>).</summary>
        protected virtual void OnCountdownMinuteBell()
        {
            if (!IsSilent && IsCdSoundMinuteBell)
            {
                PlaySoundMinuteBell();
            }
            if (CountdownMinuteBell != null)
                CountdownMinuteBell(this, new EventArgs());
        }

        /// <summary>Event that is fired when gours count changes to a whole value (hours signal).</summary>
        public event EventHandler CountdownHourBell;

        /// <summary>Called when the countdown hours count reaches a whole value (hours signal). 
        /// Raises the <see cref="CountdownHourBell"/> event and eventually plays the
        /// appropriate "hours bell" sound (if not <see cref="IsSilent"/> and if <see cref="IsCdSoundHourBell"/>).</summary>
        protected virtual void OnCountdownHourBell()
        {
            if (!IsSilent && IsCdSoundHourBell)
            {
                PlaySoundHourBell();
            }
            if (CountdownHourBell != null)
                CountdownHourBell(this, new EventArgs());
        }




        #endregion Countdown.Events


        #endregion Cowntdown

        


        #region Clock






        #region Clock.Data


        DateTime _lastWallclockTime = DateTime.Now;

        /// <summary>Wallclock time as it was last looked up.</summary>
        DateTime WallclockTime
        {
            get
            {
                _lastWallclockTime = DateTime.Now;
                return _lastWallclockTime;
            }
        }



        protected bool _isClockRunnng = true;

        /// <summary>Whether the clock is currently running or not.</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool IsClockRunning
        { get { return _isClockRunnng; }
            protected set {
                if (value != _isClockRunnng)
                {
                    if (value == false)
                    {
                        ReportError("Sorry, we can not stop the clock for you. The Universe will continue to move.");
                    }
                    else
                    {
                        _isClockRunnng = value;
                        ReportInfo("The time still passes by. The Universe moves on.");
                    }
                }
            } }



        protected bool _isClockShown = true;

        /// <summary>Specifies whether the clock is shown or not.</para></summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool IsClockShown
        {
            get { return _isClockShown; }
            set
            {
                if (value != _isClockShown)
                {
                    {
                        _isClockShown = value;
                        if (!_isClockShown)
                        {
                            // Clock will be hidden, check if this is legal and eventually show
                            // another gadget:
                            if (!(IsCountdownShown || IsStopwatchShown))
                            {
                                IsClockRunning = true;
                                if (IsStopwatchRunning)
                                {
                                    IsStopwatchShown = true;
                                    ReportInfo("Stopwatch will be displayed instead of clock.");
                                } else
                                {
                                    IsCountdownShown = true;
                                    ReportInfo("Countdown timer will be displayed instead of clock.");
                                }
                            }
                            IsClockMain = false;
                        }  else
                        {
                            // Clock will be displayed:
                            if (!(IsStopwatchShown || IsCountdownShown))
                            {
                                IsClockMain = true;
                            }
                        }
                        flowClockOuter.Visible = _isClockShown;
                        menuClock.Checked = _isClockShown;
                    }
                }
            }
        }

        protected bool _isClockMain = false;

        /// <summary>Specifies whether the clock is the main gadget of the three.</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool IsClockMain
        {
            get { return _isClockMain; }
            set
            {
                if (value != _isClockMain)
                {
                    _isClockMain = value;
                    if (_isClockMain)
                    {
                        if (!IsClockShown)
                            IsClockShown = true;
                        IsStopwatchMain = false;
                        IsCountdownMain = false;
                    }
                    else if (!(IsStopwatchMain || IsCountdownMain))
                    {
                        // Main must be transferred to some other gadget:
                        if (IsStopwatchShown)
                        {
                            IsStopwatchMain = true;
                        } else if (IsCountdownShown)
                        {
                            IsCountdownMain = true;
                        }
                        else
                        {
                            if (IsClockShown)
                            {
                                ReportWarning("Only the clock is still displayed, it willl remain the main gadget.");
                                IsClockMain = true;
                            } else
                            {
                                ReportError("No gadgets are shown.\n" +
                                    "Action will be reverted and clock will remain the main gadget.");
                                IsClockShown = true;
                                IsClockMain = true;
                            }
                        }
                    }
                    if (_isClockMain)
                    {
                        lblClockTitle.BackColor = ColorBgTitleMain;
                        lblClockTitle.ForeColor = ColorFgTitleMain;
                    }
                    else
                    {
                        lblClockTitle.BackColor = ColorBgTitle;
                        lblClockTitle.ForeColor = ColorFgTitle;
                    }
                }
            }
        }





        #endregion Clock.Actions



        #region Clock.Actions



        /// <summary>Updates clock controls according to internal properties.</summary>
        protected virtual void UpdateClockControls()
        {
            //if (IsClockControlsOpened)
            //{
            //    btnClockControls.Text = ControlsOpenText;
            //    btnClockControls.ForeColor = ControlsFgOpen;
            //    boxClockControls.Visible = true;
            //}
            //else
            //{
            //    btnClockControls.Text = ControlsClosedText;
            //    btnClockControls.ForeColor = ControlsFgClosed;
            //    boxClock Controls.Visible = false;
            //}
        }


        /// <summary>Updates appearance of the clock buttons.</summary>
        protected virtual void UpdateClockButtonsAppearance()
        {
            //btnClockReset.Text = ClockResetText;
            //btnCdReset.BackColor = ClockResetColor;
            //UpdateClockDisplay();
        }
        

        /// <summary>Refreshes Clock's display according to the current time.</summary>
        protected virtual void UpdateClockDisplay()
        {
            DateTime currentTime = WallclockTime;
            int wholeHours = currentTime.Hour;
            dispClockHours.Text = (wholeHours).ToString("00");
            dispClockHoursSeparator.Text = ":";
            dispClockMin.Text = currentTime.Minute.ToString("00");
            dispClockMinSeparator.Text = ":";
            dispClockSec.Text = currentTime.Second.ToString("00");
            dispClockSecDecimalPoint.Text = ".";
            dispClockMilliSec.Text = currentTime.Millisecond.ToString("00");
        }



        #endregion Clock.Events



        #region Clock.Events



        /// <summary>Event that is fired on each clock's timer tick (very often, in the range of milliseconds).
        /// <para>This event is usually usef for debugging or for very fine control.</para></summary>
        public event EventHandler ClockTimerTick;

        /// <summary>Called when the timer event fires  (very often, in the range of milliseconds). 
        /// Raises the <see cref="ClockTimerTick"/> event and updates the display.
        /// <para>Sounds can not be assigned to this event.</para></summary>
        protected virtual void OnClockTimerTick()
        {
            UpdateClockDisplay();
            if (ClockTimerTick != null)
                ClockTimerTick(this, new EventArgs());
        }


        ///// <summary>Starts (if it is not running) or stops (if it is running) the clock.</summary>
        //public virtual void ClockStartOrStop()
        //{
        //    if (ClockStopwatch.IsRunning)
        //    {
        //        ClockStop();
        //    }
        //    else
        //    {
        //        ClockStart();
        //    }
        //}


        ///// <summary>Event that is fired when the clock starts.</summary>
        //public event EventHandler ClockStarted;

        ///// <summary>Starts the clock timer.
        ///// <para>Its internal stopwatch starts running and the <see cref="ClockStarted"/> event is raised.</para>
        ///// <para>Eventually the appropriate sound is played (if not <see cref="IsSilent"/> and if <see cref="IsClockSoundButtons"/>
        ///// <para>If the clock is already runninng or it has already finished, call to this function has no effect.</para></summary>
        //public virtual void ClockStart()
        //{
        //    if (!IsClockRunning && ClockRemainingTime > 0)
        //    {
        //        ClockStopwatch.Start();
        //        timer1.Enabled = true;
        //        if (!IsSilent && IsClockSoundButtons)
        //            PlaySoundStart();
        //        UpdateClockButtonsAppearance();
        //        if (ClockStarted != null)
        //            ClockStarted(this, new EventArgs());
        //    }
        //}


        ///// <summary>Event that is fired when the clock stops.</summary>
        //public event EventHandler ClockStopped;

        ///// <summary>Stops the clock timer. 
        ///// <para>Its internal stopwatch stops running and the <see cref="ClockStopped"/> event is raised.</para>
        ///// <para>Eventually the appropriate sound is played (if not <see cref="IsSilent"/> and if <see cref="IsClockSoundButtons"/>)</para>
        ///// <para>If the clock is paused or it has already finished, call to this function has no effect.</para></summary>
        //public virtual void ClockStop()
        //{
        //    if (IsClockRunning && ClockRemainingTime > 0)
        //    {
        //        ClockStopwatch.Stop();
        //        if (!IsRunning)
        //            timer1.Enabled = false;  // stop triggering timer events
        //        if (!IsSilent && IsClockSoundButtons)
        //            PlaySoundStop();
        //        UpdateClockButtonsAppearance();
        //        if (ClockStopped != null)
        //            ClockStopped(this, new EventArgs());
        //    }
        //}



        ///// <summary>Event that is raised when the counter is reset.</summary>
        //public event EventHandler ClockReset;


        ///// <summary>Resets the clock.
        ///// <para>Its internal stopwatch stops running (if it runs) and the <see cref="ClockReset"/> event is raised.</para>
        ///// <para>Eventually the appropriate sound is played (if not <see cref="IsSilent"/> and if <see cref="IsClockSoundButtons"/>)</para></summary>
        //protected virtual void ClockResetIt()
        //{

        //    if (ClockStopwatch.IsRunning)
        //        ClockStopwatch.Stop();
        //    ClockStopwatch.Reset();
        //    if (!IsRunning)
        //        timer1.Enabled = false;
        //    UpdateClockDisplay();
        //    UpdateClockButtonsAppearance();
        //    if (ClockReset != null)
        //        ClockReset(this, new EventArgs());
        //}



        ///// <summary>Event that is fired when the clock reaches 0.</summary>
        //public event EventHandler ClockFinished;

        ///// <summary>Called when the clock reaches 0 and stops counting down. 
        ///// Raises the <see cref="ClockFinished"/> event and eventually plays the
        ///// appropriate alarm (if not <see cref="IsSilent"/> and if <see cref="IsClockSoundAlarm"/>).</summary>
        //protected virtual void OnClockFinished()
        //{
        //    UpdateClockButtonsAppearance();
        //    UpdateDisplayAppearance();
        //    if (ClockFinished != null)
        //        ClockFinished(this, new EventArgs());
        //    if (!IsSilent && IsClockSoundAlarm)
        //    {
        //        PlaySoundAlarmClock();
        //    }
        //}


        /// <summary>Event that is fired when second count changes to a whole value (seconds tick).</summary>
        public event EventHandler ClockSecondTick;

        /// <summary>Called when the clock seconds count reaches a whole value (seconds tick). 
        /// Raises the <see cref="ClockSecondTick"/> event and eventually plays the
        /// appropriate "second tick" sound (if not <see cref="IsSilent"/> and if <see cref="IsClockSoundSecondTick"/>).</summary>
        protected virtual void OnClockSecondTick()
        {
            if (!IsSilent && IsClockSoundSecondTick && !IsStopwatchRunning && !IsClockRunning)
            {
                PlaySoundSecondTick();
            }
            if (ClockSecondTick != null)
                ClockSecondTick(this, new EventArgs());
        }


        /// <summary>Event that is fired when minutes count changes to a whole value (minutes signal).</summary>
        public event EventHandler ClockMinuteBell;

        /// <summary>Called when the clock minutes count reaches a whole value (minutes signal). 
        /// Raises the <see cref="ClockMinuteBell"/> event and eventually plays the
        /// appropriate "minutes bell" sound (if not <see cref="IsSilent"/> and if <see cref="IsClockSoundMinuteBell"/>).</summary>
        protected virtual void OnClockMinuteBell()
        {
            if (!IsSilent && IsClockSoundMinuteBell)
            {
                PlaySoundMinuteBell();
            }
            if (ClockMinuteBell != null)
                ClockMinuteBell(this, new EventArgs());
        }

        /// <summary>Event that is fired when gours count changes to a whole value (hours signal).</summary>
        public event EventHandler ClockHourBell;

        /// <summary>Called when the clock hours count reaches a whole value (hours signal). 
        /// Raises the <see cref="ClockHourBell"/> event and eventually plays the
        /// appropriate "hours bell" sound (if not <see cref="IsSilent"/> and if <see cref="IsClockSoundHourBell"/>).</summary>
        protected virtual void OnClockHourBell()
        {
            if (!IsSilent && IsClockSoundHourBell)
            {
                PlaySoundHourBell();
            }
            if (ClockHourBell != null)
                ClockHourBell(this, new EventArgs());
        }



        #endregion Clock



        #endregion Clock




        /// <summary>Tries to adapt control settings in such a way that crucial controls are visible in the Linux version compiled 
        /// with Mono.
        /// <para>Problem is that Mono's implementation of WinForms is not absolutely complete reproduction of oriiginal WinForms.</para></summary>
        void AdaptForLinux()
        {
            btnDispCdTop.Visible = true;
            btnDispCdBottom.Visible = true;
            btnDispCdLeft.Visible = true;
            btnDispCdRight.Visible = true;

            flowOuter.FlowDirection = FlowDirection.TopDown;
            flowCdOuter.FlowDirection = FlowDirection.LeftToRight;
            flowCdButtons.FlowDirection = FlowDirection.TopDown;

            flowCdDisplay.FlowDirection = FlowDirection.TopDown;

            flowCdDisplayTop.FlowDirection = FlowDirection.LeftToRight;

            // $$
            flowCdDisplaySeconds.FlowDirection = FlowDirection.LeftToRight;
            flowCdDisplayMillisec.FlowDirection = FlowDirection.BottomUp;


            flowCdDisplayTop.MinimumSize = new Size(327, 80);  // 327, 58
            flowCdDisplay.MinimumSize = new Size(400, 120);


            flowCdOuter.MinimumSize = new Size(400, 500);  // 369, 401 
            boxCdControls.MinimumSize = new Size(355, 138);  // 355, 138

        }



        void AdaptForWindows()
        {
            btnDispCdTop.Visible = false;
            btnDispCdBottom.Visible = false;
            btnDispCdLeft.Visible = false;
            btnDispCdRight.Visible = false;
        }





        private void menuLinux_Click(object sender, EventArgs e)
        {
            AdaptForLinux();
        }

        private void menuWindows_Click(object sender, EventArgs e)
        {
            AdaptForWindows();
        }

        private void menuMain_Opening(object sender, CancelEventArgs e)
        {

        }



        /// <summary>Stopwatch becomes the main gadget.</summary>
        private void menuSw_DoubleClick(object sender, EventArgs e)
        {
            menuSwMain_Click(sender, e);
        }

        /// <summary>Hides / shows stopwatch.</summary>
        private void menuSw_CheckedChanged(object sender, EventArgs e)
        {
            if (menuSw.Checked)
                IsStopwatchShown = true;
            else
                IsStopwatchShown = false;
        }

        private void menuSwMain_Click(object sender, EventArgs e)
        {
            IsStopwatchMain = true;
        }

        private void menuSwShow_Click(object sender, EventArgs e)
        {
            if (IsStopwatchShown)
                IsStopwatchShown = false;
            else
                IsStopwatchShown = true;
        }

        


        private void menuCd_DoubleClick(object sender, EventArgs e)
        {
            menuCdMain_Click(sender, e);
        }

        private void menuCd_CheckedChanged(object sender, EventArgs e)
        {
            if (menuCd.Checked)
                IsCountdownShown = true;
            else
                IsCountdownShown = false;
        }

        private void menuCdMain_Click(object sender, EventArgs e)
        {
            IsCountdownMain = true;
        }

        private void menuCdShow_Click(object sender, EventArgs e)
        {
            if (IsCountdownShown)
                IsCountdownShown = false;
            else
                IsCountdownShown = true;
        }





        private void menuClock_DoubleClick(object sender, EventArgs e)
        {
            menuClockMain_Click(sender, e);
        }

        private void menuClock_CheckedChanged(object sender, EventArgs e)
        {
            if (menuClock.Checked)
                IsClockShown = true;
            else
                IsClockShown = false;
        }

        private void menuClockMain_Click(object sender, EventArgs e)
        {
            IsClockMain = true;
        }

        private void menuClockShow_Click(object sender, EventArgs e)
        {
            if (IsClockShown)
                IsClockShown = false;
            else
                IsClockShown = true;

        }



    }
}
 