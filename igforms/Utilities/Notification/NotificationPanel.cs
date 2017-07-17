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

    /// <summary>Control containing a notification. Can be used in different scanarios - as static notification panel in otehr controls, 
    /// as a single notification box in controls where multiple notifications can be shown at once (via <see cref="NotificationFrame"/> control)
    /// or as part of standalone (<see cref="NotificationWindw"/>), which is similar to <see cref="FadingMessage"/>.</summary>
    public partial class NotificationPanel : UserControl
    {
        public NotificationPanel()
        {
            InitializeComponent();

        }


        #region NotificationParameters


        /// <summary>Stores internal parameters of the current object to the specified parameters object.</summary>
        /// <param name="par">Parameters object to which parameters are stored.</param>
        public void StoreParameters(NotificationParameters par)
        {
            if (par == null)
                throw new ArgumentException("Can not store internal parameters: parameters object not specified (null reference).");
            //par.SetParameters(title: this.Title, notificationType: this.Type,
            //    showTime: this.ShowTime, fadingPortion: this.FadingPortion,
            //    backgroundColor: this.BackgroundColor, fadeColor: this.FadeColor, titleColor: this.TitleColor, messageColor: this.MessageColor);
            par.SetParameters(this.Parameters);
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
            // Warning: It is very important that type is set before title and backgroundColor! In this way, eventual explicitly 
            // provided values will override those that were set by setting the title.
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


        /// <summary>Sets parameters of the current control accorging to the specified object of type <see cref="NotificationParameters"/>.</summary>
        /// <param name="par">Parameters from which the current object sets (copies) values. 
        /// <para>If nulll (or not specified) then parameter values are re-set to default values. If you don't wannt this behaviour, ues
        /// <see cref="SetParameters(string, ReportType?, double?, double?, double? Color?, Color?, Color?, Color?)"/> instead.</para></param>
        public void SetParameters(NotificationParameters par)
        {
            if (par == null)
            {
                // If argument is null then current parameters are re-set to default values.
                SetParameters(NotificationParameters.DefaultNotificationParameters);
            }
            else
            {
                // Remark: the function ooverload below is used in order that setters of individual parameters are executed, because these
                // setters may not only set values but have other side effect, which must be produced when copying parameters from parameters object:
                SetParameters(title: par.Title, notificationType: par.Type, showTime: par.ShowTime, fadingPortion: par.FadingPortion, timerInterval: par.TimerInterval,
                    backgroundColor: par.BackgroundColor, fadeColor: par.FadeColor, titleColor: par.TitleColor, messageColor: par.MessageColor);
            }
        }


        /// <summary>Sets the parameters of the current control according to the specified <see cref="NotificationParameters"/> object plus according
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


        private NotificationParameters _parameters = new NotificationParameters(NotificationParameters.DefaultNotificationParameters);

        /// <summary>Internal parameters of the current <see cref="NotificationPanel"/>.
        /// <para>This object leverages functionality of <see cref="NotificationParameters"/> class in setting internal parameters.</para>
        /// <para>Its parameters are initially set to default values via template <see cref="NotificationParameters.DefaultNotificationParameters"/>.</para>
        /// <para>This object is used as primary storage for parameters. Individual properties reference properties of this object and are used 
        /// in order to set parameters in GUI builders.</para></summary>
        protected NotificationParameters Parameters { get { return _parameters; }  }


        /// <summary>Message title. 
        /// <para>Title is included in parameters because it can be set automatically when not specified.</para>
        /// <para>If null title is provided in constructor or <see cref="SetParameters(NotificationParameters)"/> then title is set automatically.</para>
        /// <para>If empty string is provided then it is considered that there is no title.</para></summary>
        public virtual string Title { get { return Parameters.Title; } 
            set
            {
                if (InvokeRequired)
                {
                    Invoke((MethodInvoker)delegate
                    {
                        this.Title = value;
                    });
                }
                else
                {
                    lblTitle.Text = value;
                    Parameters.Title = value;
                }
            }
        }


        /// <summary>Notification type (such as info, warning, errror...),  see <see cref="ReportType"/>.
        /// <para>This nfluences how some parameters are set when not provided, such as <see cref="BackgroundColor"/> or <see cref="Title"/>.</para></summary>
        public virtual ReportType Type { get { return Parameters.Type; } 
            set
            {
                if (InvokeRequired)
                {
                    Invoke((MethodInvoker)delegate
                    {
                        this.Type = value;
                    });
                }
                else
                {
                    Parameters.Type = value;
                    // Set possibly dependent properties:
                    this.Title = Parameters.Title;
                    this.BackgroundColor = Parameters.BackgroundColor;
                }
            }
        }


        /// <summary>Time span, in seconds, for which notification is displayed.</summary>
        public virtual double ShowTime
        {
            get { return Parameters.ShowTime; }
            set
            {
                if (this.InvokeRequired)
                {
                    Invoke((MethodInvoker)delegate
                    {
                        this.ShowTime = value;
                    });
                } else
                {
                    Parameters.ShowTime = value;
                }
            }
        }


        /// <summary>Portion of the end of <see cref="ShowTime"/> during which notification fades out.</summary>
        public virtual double FadingPortion
        {
            get { return Parameters.FadingPortion; }
            set
            {
                if (this.InvokeRequired)
                {
                    Invoke((MethodInvoker)delegate
                    {
                        this.FadingPortion = value;
                    });
                } else
                {
                    Parameters.FadingPortion = value;
                }
            }
        }

        /// <summary>Interval of events fired by timer. This is e.g. the interval betwen changes in background color during fading..</summary>
        public virtual double TimerInterval { get { return Parameters.TimerInterval; }  
            set
            {
                if (this.InvokeRequired)
                {
                    Invoke((MethodInvoker)delegate
                    {
                        this.TimerInterval = value;
                    });
                }
                else
                {
                    Parameters.TimerInterval = value;
                }
            }
        }


        /// <summary>Backhround color of notification.</summary>
        public virtual Color BackgroundColor { get { return Parameters.BackgroundColor; } 
            set
            {
                if (this.InvokeRequired)
                {
                    Invoke((MethodInvoker)delegate
                    {
                        this.BackgroundColor = value;
                    });
                }
                else
                {
                        this.BackColor = value;
                        lblTitle.BackColor = value;
                        txtMessage.BackColor = value;
                        Parameters.BackgroundColor = value;
                }
            }
        }


        /// <summary>Final background color of notification after fading out at the end of its display time.</summary>
        public virtual Color FadeColor { get { return Parameters.FadeColor; } 
            set
            {
                if (this.InvokeRequired)
                {
                    Invoke((MethodInvoker)delegate
                    {
                        this.FadeColor = value;
                    });
                }
                else
                {
                    Parameters.FadeColor = value;
                }
            }
        }

        /// <summary>Color of notification's title.</summary>
        public virtual Color TitleColor { get { return Parameters.TitleColor; }
            set {
                if (this.InvokeRequired)
                {
                   Invoke((MethodInvoker)delegate
                   {
                       this.TitleColor = value;
                   });
                }
                else
                {
                    lblTitle.ForeColor = value;
                    Parameters.TitleColor = value;
                }
            }
        }

        //Color _messageColor = NotificationParameters.DefaultMessageColor;

        /// <summary>Color of notification's message.</summary>
        public virtual Color MessageColor { get { return Parameters.MessageColor; } set { Parameters.MessageColor = value; } }



        // Corresponding values for properties that by default depend on notification type (but this can be overridden
        // by setting these properties explicitly); these properties are tightly bound to corresponding properties on Parameters:

        /// <summary>Notification background color corresponding to notification type <see cref="ReportType.Info"/>. Used when color is not specified.</summary>
        public color InfoColor { get { return Parameters.InfoColor; } set { Parameters.InfoColor = value; } }

        /// <summary>Notification background color corresponding to notification type <see cref="ReportType.Info"/>. Used when color is not specified.</summary>
        public Color WarningColor { get { return Parameters.WarningColor; } set { Parameters.WarningColor = value; } }

        /// <summary>Notification background color corresponding to notification type <see cref="ReportType.Error"/>. Used when color is not specified.</summary>
        public Color ErrorColor { get { return Parameters.ErrorColor; } set { Parameters.ErrorColor = value; } }

        /// <summary>Notification background color corresponding to undefined and other notification types. Used when color is not specified.</summary>
        public Color OtherBackgroundColor { get { return Parameters.OtherBackgroundColor; } set { Parameters.OtherBackgroundColor = value; } }


        /// <summary>Notification title corresponding to notification type <see cref="ReportType.Info"/>. Used when color is not specified.</summary>
        public string InfoTitle { get { return Parameters.InfoTitle; } set { Parameters.InfoTitle = value; } }

        /// <summary>Notification title corresponding to notification type <see cref="ReportType.Warning"/>. Used when color is not specified.</summary>
        public string WarningTitle { get { return Parameters.WarningTitle; } set { Parameters.WarningTitle = value; } }

        /// <summary>Notification title corresponding to notification type <see cref="ReportType.Error"/>. Used when color is not specified.</summary>
        public string ErrorTitle { get { return Parameters.ErrorTitle; } set { Parameters.ErrorTitle = value; } }

        /// <summary>Notification title corresponding to undefined and other notification types. Used when color is not specified.</summary>
        public string OtherTitle { get { return Parameters.OtherTitle; } set { Parameters.OtherTitle = value; } }


        // Dependent properties:

        public int ShowTmeMilliseconds { get { return (int)(ShowTime * 1000); } set { ShowTime = (int)(value / 1000); } }

        public int TimerIntervalMilliseconds { get { return (int)(TimerInterval * 1000); } set { TimerInterval = (int)(value / 1000); } }

        public double FadingTime { get { return FadingPortion * ShowTime; } set { FadingPortion = value / ShowTime; } }

        public int FadingTimeMilliseconds { get { return (int)(FadingTime * 1000); } }

        public double NonFadingTime { get { return ShowTime - FadingTime; } set { FadingTime = ShowTime - value; } }

        public int NonFadingTimeMilliseconds { get { return (int)(NonFadingTime * 1000); } }


        // Other properties - not handled by NotificationProperties class:

        string _message = "<< Message. >>";

        public string Message
        {
            get { return _message; }
            set
            {
                if (this.InvokeRequired)
                {
                    Invoke((MethodInvoker)delegate
                    {
                        this.Message = value;
                    });
                }
                else
                {
                    this.txtMessage.Text = value;
                    this._message = value;
                }
            }
        }



        #endregion NotificationParameters


        #region Operation 


        #endregion Operation


        private void txtMessage_TextChanged(object sender, EventArgs e)
        {
            // Calculaye the necessary heihgt of the text window
            const int padding = 3; // amount of padding to add
            int numLines = this.txtMessage.GetLineFromCharIndex(this.txtMessage.TextLength) + 1;  // get number of lines, also accounts for possibility that lines are wrapped if width is not sufficient
            int border = this.txtMessage.Height - this.txtMessage.ClientSize.Height;  // get border thickness
            // Calculate the necessary height (height of one line * number of lines + spacing):
            int intendedHeight = this.txtMessage.Font.Height * numLines + padding + border;
            // take into account eventual maximal height:
            // Set height:
            this.txtMessage.Height = intendedHeight;

        }

        private void lblTitle_Click(object sender, EventArgs e)
        {

        }
    }
}
