// Copyright (c) Igor Grešovnik (2009), IGLib license; http://www2.arnes.si/~ljc3m2/igor/ioptlib/


// APPLICATION MESSAGE REPORTERS

// This is a temporary solution and should be changed!

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace IG.Lib
{

    //public class Rep
    //{

    //    public static object lockobj = new object();

    //    static Reporter _R = null;

    //    /// <summary>Returns the curent global Application reporter.</summary>
    //    public static Reporter R
    //    {
    //        get
    //        {
    //            if (_R != null)
    //                return _R;
    //            else return ReporterConsoleApp;
    //        }
    //    }

    //    /// <summary>Returns a properly initialized global reporter for applications without a console.</summary>
    //    public static Reporter ReporterBasic
    //    {
    //        get
    //        {
    //            if (!ReporterConsole.GlobalInitialized)
    //            {
    //                Reporter r = Reporter.Global;
    //                r.ReadAppSettings("Basic");
    //                // TODO: 
    //                // Settings below are just for testing. Comment or rehsape this section later!
    //                // Note that reporter settings for the global reporter are already read from application settings
    //                // for the default and "General" groups.
    //                r.UseTextLogger = r.UseTextWriter = true;
    //                r.ReportingLevel = ReportLevel.Off;
    //                r.LoggingLevel = ReportLevel.Info;
    //                r.TracingLevel = ReportLevel.Warning;
    //            }
    //            return ReporterConsole.Global;
    //        }
    //    }

    //    /// <summary>Returns a properly initialized global reporter for console application.</summary>
    //    public static Reporter ReporterConsoleApp
    //    {
    //        get
    //        {
    //            if (!ReporterConsole.GlobalInitialized)
    //            {
    //                ReporterConsole r = ReporterConsole.Global;
    //                r.ReadAppSettings("Console");
    //                // TODO: 
    //                // Settings below are just for testing. Comment or rehsape this section later!
    //                // Note that reporter settings for the global reporter are already read from application settings
    //                // for the default and "General" groups.
    //                r.UseTextLogger = r.UseTextWriter = r.UseConsole = true;
    //                r.ReportingLevel = ReportLevel.Warning;
    //                r.LoggingLevel = ReportLevel.Info;
    //                r.TracingLevel = ReportLevel.Warning;
    //            }
    //            return ReporterConsole.Global;
    //        }
    //    }


    //}




}
