// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/


/***************************/
/*                         */
/*    Network Utilities    */
/*                         */
/***************************/


using System.IO;
using IG.Lib;


/*
Port scanners:
   Simple multithreading port scanner: https://github.com/munirusman/Simple-TCP-Port-Scanner/tree/master/PortScanner
   More complex multithreading TCP port scanner: https://github.com/PhilipMur/C-Sharp-Multi-Threaded-Port-Scanner
   Using Threatpool (CodePjroject): https://www.codeproject.com/Articles/199016/Simple-TCP-Port-Scanner
 wget command implementations:
   wget command examples: https://www.rosehosting.com/blog/wget-command-examples/
 */

namespace IG.Net
{



    /// <summary>Networking and web utilities.</summary>
    /// $A Igor Apr18;
    public static class UtilNet
    {


        private static object _lockStatic = null;

        /// <summary>Locking object for static methods and properties of this class.</summary>
        /// <remarks>Read-only, safely provided on demand (using <see cref="Util.LockGlobal"/> when 
        /// initializing the first time when referenced).</remarks>
        public static object LockStatic
        {

            get
            {
                if (_lockStatic == null)
                {
                    lock (Util.LockGlobal)
                    {
                        if (_lockStatic == null)
                            _lockStatic = new object();
                    }
                }
                return _lockStatic;
            }
        }




        #region WebGet



        #endregion WebGet

        

        #region PortScanner



        #endregion PortScanner





    } // class UtilSystem

}