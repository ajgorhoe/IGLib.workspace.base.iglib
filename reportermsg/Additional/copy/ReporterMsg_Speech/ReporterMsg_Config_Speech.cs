using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using IG.ReporterMsg;

namespace IG.ReporterMsgForms
{

    /// <summary>This class is extension of ReporterConf that enables reporting via speech.</summary>
    /// <remarks>Do not use the visual builder for modifying this class! Only modify the class manually.
    /// This class should only contain overrides of certain methods such that speech is activated!</remarks>
    public class ReporterConfSpeech : ReporterConf
    {

        /// <summary>Constructor.</summary>
        /// <param name="rf">Reporter that is used for reporting errors that appear in this form.</param>
        public ReporterConfSpeech(Reporter rf) : base(rf)
        {
            // Make the speech settings subpanel visible:
            SpeechVisible = true;
        }


        /// <summary>Sets the current reporter of this calss according to the reporter type.</summary>
        /// <param name="type">Specifies what type of reporter should be set.</param>
        protected override void SetReporter(enReporterType type)
        {
            switch (type)
            {
                case enReporterType.Basic:
                    _reporter = Reporter.Global;
                    break;
                case enReporterType.Console:
                    _reporter = ReporterConsole.Global;
                    break;
                case enReporterType.Msgbox:
                    _reporter = ReporterMsgbox.Global;
                    break;
                case enReporterType.ConsoleMsgbox:
                    // Set Reporter with speech capabilities here:
                    _reporter = ReporterConsoleMsgboxSpeech.Global;
                    break;
                default:
                    // Set Reporter with speech capabilities here:
                    _reporter = ReporterConsoleMsgboxSpeech.Global;
                    break;
            }
        }


    }



}
