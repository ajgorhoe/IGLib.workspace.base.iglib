// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

using IG.Lib;

namespace IG.Forms
{

    /// <summary>Contains parameters for GUI notifications.</summary>
    /// <remarks><para>Setting parameters stored in objects of this type: </para>
    /// <para><see cref="SetParameters(string, ReportType?, double?, double?, Color?, Color?, Color?, Color?)"/> sets only those parameters 
    /// that are specified. Calling this function with no arguments does not set anything.</para>
    /// <para><see cref="SetParameters(NotificationParameters)"/> sets all parameters on the current object according to values on the specified
    /// object. If argument is null then all parameters are set to default values (more preciseely, the default parameters template
    /// <see cref="DefaultNotificationParameters"/> is taken).</para>
    /// <para><see cref="SetParameters(NotificationParameters, string, ReportType?, double?, double?, Color?, Color?, Color?, Color?)"/> first sets
    /// all parameters according to the specified parameters object (first argument), and then sets those parameters that are specified by argumentts.
    /// The in fact calls <see cref="SetParameters(NotificationParameters)"/> and then the function for setting individual parameeters.</para></remarks>
    /// $A Igor Mar17;
    public class NotificationParameters
    {


        /// <summary>Initializes the current parameters object to default values.</summary>
        public NotificationParameters() { }

        /// <summary>Initializes the current parameters object according to the specified parameters obect (copy constructor).</summary>
        /// <param name="par">Another <see cref="NotificationParameters"/> object according to which the current object is
        /// initialized (all parameters are copied from that object).
        /// <para>If null then the current object is initialized according to <see cref="DefaultNotificationParameters"/>.</para>,
        /// which in turn is an object initialized by default consttructor. Calling default consttructor is more efficient.</param>
        public NotificationParameters(NotificationParameters par)
        {
            SetParameters(par);
        }

        /// <summary>Initializes the created parameters object to default values, and sets those parameters specified by provided 
        /// arguments to the specified values.
        /// <para>Setting the specified parameter values is prformed by the <see cref="SetParameters(string, ReportType?, double?, double?, double? Color?, Color?, Color?, Color?)"/> 
        /// function.</para></summary>
        public NotificationParameters(string title = null, ReportType? notificationType = null,
            double? showTime = null, double? fadingPortion = null, double? timerInterval = null,
            Color? backgroundColor = null, Color? fadeColor = null, Color? titleColor = null, Color? messageColor = null)
        {
            SetParameters(title: title, notificationType: notificationType, showTime: showTime, fadingPortion: fadingPortion, timerInterval: timerInterval,
                backgroundColor: backgroundColor, fadeColor: fadeColor, titleColor: titleColor, messageColor: messageColor);
        }

        /// <summary>Initializes the current parameters object according to the specified parameters object (<paramref name="par"/> and then sets the specified
        /// parameters.
        /// <para>Initialization according to <paramref name="par"/> is performed by the <see cref="SetParameters(NotificationParameters)"/> function.</para>
        /// <para>Setting the specified parameter values is prformed by the <see cref="SetParameters(string, ReportType?, double?, double?, double? Color?, Color?, Color?, Color?)"/> 
        /// function.</para></summary>
        public NotificationParameters(NotificationParameters par, string title, ReportType? notificationType = null,
            double? showTime = null, double? fadingPortion = null, double ? timerInterval = null,
            Color? backgroundColor = null, Color? fadeColor = null, Color? titleColor = null, Color? messageColor = null)
        {
            SetParameters(par, title: title, notificationType: notificationType, showTime: showTime, fadingPortion: fadingPortion, timerInterval: timerInterval,
                backgroundColor: backgroundColor, fadeColor: fadeColor, titleColor: titleColor, messageColor: messageColor);
        }


        /// <summary>Sets the current notification parameters according to argument of the function.
        /// <para>Those parameters that are not specified are not set.</para></summary>
        /// <param name="title">Title of notification. If not provided then it is set according to notification 
        /// type (<see cref="notificationType"/>), provided that the type is specified (otherwise, it is not set).</param>
        /// <param name="notificationType">Type of notification (such as Info, Warning, Error...).</param>
        /// <param name="showTime">How much time, in seconds, notification is visible.</param>
        /// <param name="fadingPortion">Portion of total display time in which notification fades.</param>
        /// <param name="timerInterval">Interval between timer events being fired. This defines e.g. how frequently background color is changed during fadeout.</param>
        /// <param name="backgroundColor">Notification's background color. If not provided then it is set according to notification
        /// type (<see cref="notificationType"/>), provided that the type is specified (otherwise, it is not set).</param>
        /// <param name="fadeColor">Notification's fade color - color that notification has after fading out.</param>
        /// <param name="titleColor">Color in which title is printed.</param>
        /// <param name="messageColor">Color in which notification's message is printed.</param>
        public virtual void SetParameters(string title = null, ReportType? notificationType = null,
            double? showTime = null, double? fadingPortion = null, double? timerInterval = null,
            Color? backgroundColor = null, Color? fadeColor = null, Color? titleColor = null, Color? messageColor = null)
        {
            if (notificationType != null)
                this.Type = notificationType.Value;
            if (title != null)
                this.Title = title;  // was also set when notification type was set, but explicitly set valu overrides that value
            if (showTime != null)
                this.ShowTime = showTime.Value;
            if (fadingPortion != null)
                this.FadingPortion = fadingPortion.Value;
            if (timerInterval != null)
                this.TimerInterval = timerInterval.Value;
            if (backgroundColor != null)
                this.BackgroundColor = backgroundColor.Value; // was also set when notification type was set, but explicitly set valu overrides that value
            if (fadeColor != null)
                this.FadeColor = fadeColor.Value;
            if (titleColor != null)
                this.TitleColor = titleColor.Value;
                if (messageColor != null)
                    this.MessageColor = messageColor.Value;
        }

        /// <summary>Sets current parameters object accorging to the specified object of type <see cref="NotificationParameters"/>.</summary>
        /// <param name="par">Parameters from which the current object sets (copies) values. 
        /// <para>If nulll (or not specified) then parameter values are re-set to default values. If you don't wannt this behaviour, ues
        /// <see cref="SetParameters(string, ReportType?, double?, double?, double? Color?, Color?, Color?, Color?)"/> instead.</para></param>
        public void SetParameters(NotificationParameters par)
        {
            if (par == null)
            {
                // If argument is null then current parameters are re-set to default values.
                SetParameters(DefaultNotificationParameters);
            }
            else
            {
                SetParameters(title: par.Title, notificationType: par.Type, showTime: par.ShowTime, fadingPortion: par.FadingPortion, timerInterval: par.TimerInterval, 
                    backgroundColor: par.BackgroundColor, fadeColor: par.FadeColor, titleColor: par.TitleColor, messageColor: par.MessageColor);
            }
        }


        /// <summary>Sets the parameters of the current object according to another <see cref="NotificationParameters"/> object plus according
        /// to explicitly set parameters. 
        /// <para>First, parameters are set according to contents of another object of type <see cref="NotificationParameters"/>, and 
        /// then parameters that are explicitly specified are set to specified values.</para>
        /// <para>This makes possible to ues one tamplate object whith which parameeterrs can be re-set to some reference state, while explicitly
        /// specifying those parameters that should be set differently from this.</para></summary>
        /// <param name="par">Parameters object from which all parameters are first copied (and eventually overwritten later according to explicitly set parameters). 
        /// <para>If null then default parameter object <see cref="DefaultNotificationParameters"/> is taken, i.e., parameters are set to default values.</para></param>
        /// <param name="title">Title of notification. 
        /// <para>null means that title is not set here (use empty string "" for no title). In this case, title is still set if type is defined.</para>
        /// <para>Title can not be omitted, the reason is that this function with only <paramref name="par"/> specified must be distinguishable
        /// from <see cref="SetParameters(NotificationParameters)"/>.</para></param>
        /// <param name="notificationType">Type of notification (e.g. error, warning, info...), see <see cref="ReportType"/>.</param>
        /// <param name="showTime">Time span during which notification is visibly displayed (it disappears from display after this time).</param>
        /// <param name="fadingPortion">Portion of time at the end of the display time (<see cref="showTime"/>) during which notification fades out.</param>
        /// <param name="timerInterval">Interval between timer events being fired. This defines e.g. how frequently background color is changed during fadeout.</param>
        /// <param name="backgroundColor">Background color. If null or not specified, then it is not modified, unless the </param>
        /// <param name="fadeColor">Background color after notification fades out (during fadeout, color changes linearly from initial background to this color).</param>
        /// <param name="titleColor">Title color.</param>
        /// <param name="messageColor">Massage color.</param>
        public void SetParameters(NotificationParameters par, string title, ReportType? notificationType = null,
            double? showTime = null, double? fadingPortion = null, double? timerInterval = null, 
            Color? backgroundColor = null, Color? fadeColor = null, Color? titleColor = null, Color? messageColor = null)
        {
            // First, copy all parameters from the specified parameters object (or from template, if this object is null):
            SetParameters(par);
            // Then, set explicitly defined parameters:
            SetParameters(par: par, title: title, notificationType: notificationType,
                showTime: showTime, fadingPortion: fadingPortion, timerInterval: timerInterval,
                backgroundColor: backgroundColor, fadeColor: fadeColor, titleColor: titleColor, messageColor: messageColor);
        }

        /// <summary>Returns default notification title according to type of notification.</summary>
        /// <param name="type">Type of notification.</param>
        /// <returns></returns>
        public static string GetDefaultTitle(ReportType type)
        {
            switch (type)
            {
                case ReportType.Info:
                    return DefaultInfoTitle;
                case ReportType.Warning:
                    return DefaultWarningTitle;
                case ReportType.Error:
                    return DefaultErrorTitle;
                default:
                    return DefaultTitle;
            }
        }

        /// <summary>Returns default notification's background color according to type of notification.</summary>
        /// <param name="notificationType">Type of notification.</param>
        public static Color GetDefaultColor(ReportType notificationType)
        {
            switch (notificationType)
            {
                case ReportType.Info:
                    return DefaultInfoColor;
                case ReportType.Warning:
                    return DefaultWarningColor;
                case ReportType.Error:
                    return DefaultErrorColor;
                default:
                    return DefaultBackgroundColor;
            }
        }


        /// <summary>Static constructor.
        /// <para>Sets the static variable holding the default object used as template for other objects of type <see cref="NotificationParameters"/>.</para>
        /// This template object is used for <see cref="SetParameters(NotificationParameters)"/> when argument is null.</summary>
        static NotificationParameters()
        {
            _defaultNotificationParameters = new NotificationParameters();
            _defaultNotificationParameters.SetParameters();
        }


        /// <summary>Template <see cref="NotificationParameters"/> object having all parameters set to default values, set by 
        /// static constructor.</summary>
        static readonly NotificationParameters _defaultNotificationParameters = null;

        /// <summary>Template <see cref="NotificationParameters"/> object having all parameters set to default values.</summary>
        public static NotificationParameters DefaultNotificationParameters
        {
            get { return _defaultNotificationParameters; }
        }


        public static double DefaultShowTime { get; set; } = 3.0;

        public static double DefaultTimerInterval { get; set; } = 0.01;

        public static double DefaultFadingPortion { get; set; } = 0.3;

        public static ReportType DefaultType { get; set; } = ReportType.Info;

        public static Color DefaultTitleColor { get; set; } = Color.Blue;

        public static Color DefaultMessageColor { get; set; } = Color.Black;

        public static Color DefaultBackgroundColor { get; set; } = Color.LightYellow;

        public static Color DefaultFadeColor { get; set; } = Color.Gray;

        public static color DefaultInfoColor { get;  set; } = DefaultBackgroundColor;

        public static color DefaultWarningColor { get;  set; } = Color.Orange;

        public static color DefaultErrorColor { get; set; } = Color.Red;

        public static string DefaultTitle = "Notification";

        public static string DefaultInfoTitle = "Info";

        public static string DefaultWarningTitle = "Warning";

        public static string DefaultErrorTitle = "Error";



        /// <summary>Returns notification title that corresponds to the specified type of notification.</summary>
        /// <param name="type">Type of notification for which the corresponding title is returned.</param>
        public  string GetTitle(ReportType type)
        {
            switch (type)
            {
                case ReportType.Info:
                    return InfoTitle;
                case ReportType.Warning:
                    return WarningTitle;
                case ReportType.Error:
                    return ErrorTitle;
                default:
                    return OtherTitle;
            }
        }

        /// <summary>Returns notification's background color that corresponds to the specified  type of notification.</summary>
        /// <param name="notificationType">Type of notification for which the corresponding background color is returned.</param>
        public Color GetColor(ReportType notificationType)
        {
            switch (notificationType)
            {
                case ReportType.Info:
                    return InfoColor;
                case ReportType.Warning:
                    return WarningColor;
                case ReportType.Error:
                    return ErrorColor;
                default:
                    return OtherBackgroundColor;
            }
        }

        /// <summary>Notification background color corresponding to notification type <see cref="ReportType.Info"/>. Used when color is not specified.</summary>
        public color InfoColor { get; set; } = DefaultInfoColor;

        /// <summary>Notification background color corresponding to notification type <see cref="ReportType.Info"/>. Used when color is not specified.</summary>
        public Color WarningColor { get; set; } = DefaultWarningColor;

        /// <summary>Notification background color corresponding to notification type <see cref="ReportType.Error"/>. Used when color is not specified.</summary>
        public Color ErrorColor { get; set; } = DefaultErrorColor;

        /// <summary>Notification background color corresponding to undefined and other notification types. Used when color is not specified.</summary>
        public Color OtherBackgroundColor { get; set; } = DefaultBackgroundColor;


        /// <summary>Notification title corresponding to notification type <see cref="ReportType.Info"/>. Used when color is not specified.</summary>
        public string InfoTitle { get; set; } = DefaultInfoTitle;

        /// <summary>Notification title corresponding to notification type <see cref="ReportType.Warning"/>. Used when color is not specified.</summary>
        public string WarningTitle { get; set; } = DefaultWarningTitle;

        /// <summary>Notification title corresponding to notification type <see cref="ReportType.Error"/>. Used when color is not specified.</summary>
        public string ErrorTitle { get; set; } = DefaultErrorTitle;

        /// <summary>Notification title corresponding to undefined and other notification types. Used when color is not specified.</summary>
        public string OtherTitle { get; set; } = DefaultTitle;


        /// <summary>Message title. 
        /// <para>Title is included in parameters because it can be set automatically when not specified.</para>
        /// <para>If null title is provided in constructor or <see cref="SetParameters(NotificationParameters)"/> then title is set automatically.</para>
        /// <para>If empty string is provided then it is considered that there is no title.</para></summary>
        public virtual string Title { get; set; } = DefaultTitle;

        ReportType _type = DefaultType;

        /// <summary>Notification type (such as info, warning, errror...),  see <see cref="ReportType"/>.
        /// <para>This nfluences how some parameters are set when not provided, such as <see cref="BackgroundColor"/> or <see cref="Title"/>.</para></summary>
        public virtual ReportType Type { get { return _type; }
            set {
                _type = value;
                // When type is set, we also set parameters that have default values corresponding to different types;
                // These parameter values can still be overridden later by explicitly setting them:
                Title = GetTitle(value);
                BackgroundColor = GetColor(value);
            }
        }

        /// <summary>Time span, in seconds, for which notification is displayed.</summary>
        public virtual double ShowTime { get; set; } = DefaultShowTime;

        /// <summary>Time nterval, in seconds, between events fired by timer. This is e.g. the interval betwen changes in background color during fading.</summary>
        public virtual double TimerInterval { get; set; } = DefaultTimerInterval;

        /// <summary>Portion of the end of <see cref="ShowTime"/> during which notification fades out.</summary>
        public virtual double FadingPortion { get; set; } = DefaultFadingPortion;

        /// <summary>Backhround color of notification.</summary>
        public virtual Color BackgroundColor { get; set; } = DefaultBackgroundColor;

        /// <summary>Final background color of notification after fading out at the end of its display time.</summary>
        public virtual Color FadeColor { get; set; } = DefaultFadeColor;

        /// <summary>Color of notification's title.</summary>
        public virtual Color TitleColor { get; set; } = DefaultTitleColor;

        /// <summary>Color of notification's message.</summary>
        public virtual Color MessageColor { get; set; } = DefaultMessageColor;


        // Dependent properties:

        /// <summary>Time span, in milliseconds, for which notification is displayed.
        /// <para>This is dependent (calculated) property, but it has a setter.</para></summary>
        public int ShowTmeMilliseconds { get { return (int) (ShowTime * 1000); } set { ShowTime = (int) (value / 1000); } }

        /// <summary>Time nterval, in milliseconds, between events fired by timer. This is e.g. the interval betwen changes in background color during fading.
        /// <para>This is dependent (calculated) property, but it has a setter.</para></summary></summary>
        public int TimerIntervalMilliseconds { get { return (int) (TimerInterval * 1000); } set { TimerInterval = (int) (value / 1000); } }

        /// <summary>Time interval, in seconds, of the last part of show time during which notification background fades out.
        /// <para>This is dependent (calculated) property.</para></summary>
        public double FadingTime { get { return FadingPortion * ShowTime; } }

        /// <summary>Time interval, in milliseconds, of the last part of show time during which notification bacground fades out.
        /// <para>This is dependent (calculated) property.</para></summary>
        public int FadingTimeMilliseconds { get { return (int)(FadingTime * 1000); } } 

        /// <summary>Time interval in the first part of show time, in seconds, during which notification background is not fadin out (color is constant).
        /// <para>This is dependent (calculated) property.</para></summary>
        public double NonFadingTime { get { return ShowTime - FadingTime; }  }

        /// <summary>Time interval in the first part of show time, in milliseconds, during which notification background is not fadin out (color is constant).
        /// <para>This is dependent (calculated) property.</para></summary>
        public int NonFadingTimeMilliseconds { get { return (int)(NonFadingTime * 1000); } }



    }  // class NotificationParameters



}
