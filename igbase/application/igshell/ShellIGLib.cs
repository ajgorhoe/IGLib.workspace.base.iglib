// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using IG.Lib;
using IG.Num;
using IG.Neural;
using IG.Script;

namespace IG.Script
{

    /// <summary>Shell application to be installed in script application of the current program.</summary>
    /// $A Igor Jul09 Jan13;
    public class ShellIGLib : ShellApplication<CommandLineApplicationInterpreter>
    {
        
        public ShellIGLib()
            : base()
        {
            DefaultActiveDir = @"../../testdata/scripts";
            OptDir = DefaultActiveDir;

        }

        /// <summary>WARNING: If this reports an error then you should also remove the "shellext.exe"
        /// from AddDefaultAssemblies()!</summary>
        protected AppBase AppShellExt = new AppBase();
        //protected AppBase AppShellExt = new IG.Script.AppShellExt();
        
        /// <summary>Adds assemblies to be automatically referenced by loaded scripts.</summary>
        public override void AddDefaultAssemblies()
        {
            base.AddDefaultAssemblies();
            //ScriptLoaderBase.AddDefaultAssemblies("shellext.exe",null);
        }


    }  // class ShellIGLib

}


