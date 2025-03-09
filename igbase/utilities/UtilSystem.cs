// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/


/********************************/
/*                              */
/*    I/O & System Utilities    */
/*                              */
/********************************/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;
using System.Reflection;
using System.Collections;
using System.Globalization;
using System.Resources;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;



namespace IG.Lib
{

    /// <summary>Temporary file stream. Based on a temporary file proveded by the system, which is automatically 
    /// closed when the stream is closed.</summary>
    public class TempFileStream : FileStream
    {
        public TempFileStream()
            : base(Path.GetTempFileName(), FileMode.Create, FileAccess.ReadWrite, FileShare.Read, 4096, FileOptions.DeleteOnClose) { }
        public TempFileStream(FileAccess access)
            : base(Path.GetTempFileName(), FileMode.Create, access, FileShare.Read, 4096, FileOptions.DeleteOnClose) { }
        public TempFileStream(FileAccess access, FileShare share)
            : base(Path.GetTempFileName(), FileMode.Create, access, share, 4096, FileOptions.DeleteOnClose) { }
        public TempFileStream(FileAccess access, FileShare share, int bufferSize)
            : base(Path.GetTempFileName(), FileMode.Create, access, share, bufferSize, FileOptions.DeleteOnClose) { }
    }


    /// <summary>General utilities.</summary>
    /// $A Igor Apr10;
    public static class UtilSystem
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



        #region UserData

        private static string _userName = null;

        private static string _userNameLowerCase = null;

        /// <summary>Whether user name has already been retrieved ans stored.</summary>
        private static bool UserNameNotRetrieved
        {
            get { return (_userName == null || _userNameLowerCase == null); }
        }

        /// <summary>Retrieves and stores information about current user name.</summary>
        private static void RetrieveUserName()
        {
            _userName = System.Environment.UserName;
            _userNameLowerCase = _userName.ToLower();
        }

        /// <summary>Sets name of the current user. This method is provided to enable 
        /// testing code under another user name. Setting to null anihilates effect of previous calls.
        /// <para>After call to this method, user name can be set to null in order to retrieve the true
        /// user logged in for subsequent operations.</para>
        /// <para>Warning: you should use this only exceptionally, e.g. for testing, and only in testing or
        /// demo sections of code.</para></summary>
        /// <param name="username">Name of the user to be set. Null annihilates previous calls and causes
        /// that system provided user name is returned by subsequent queries.</param>
        public static void SetUsername(string username)
        {
            if (string.IsNullOrEmpty(username) && username != null)
            {
                throw new ArgumentException("Empty string provided for user name, only non-empty string or null is accepted.");
            }
            else
            {
                _userName = username;
                _userNameLowerCase = username.ToLower();
                if (!string.IsNullOrEmpty(username))
                {
                    Console.WriteLine();
                    Console.WriteLine();
                    Console.WriteLine("WARNING: ");
                    Console.WriteLine("Name of the current user has been changed to " + username + ".");
                    Console.WriteLine();
                }
            }
        }

        /// <summary>Gets name of the current user.</summary>
        public static string UserName
        {
            get
            {
                if (UserNameNotRetrieved)
                    RetrieveUserName();
                return _userName;
            }
        }

        /// <summary>Gets name of the current user with all letters converted to lower case 
        /// (in order to avoid ambiguities).</summary>
        public static string UserNameLowerCase
        {
            get
            {
                if (UserNameNotRetrieved)
                    RetrieveUserName();
                if (string.IsNullOrEmpty(_userNameLowerCase))
                    if (!string.IsNullOrEmpty(_userName))
                        _userNameLowerCase = _userName.ToLower();
                return _userNameLowerCase;
            }
        }

        /// <summary>Returns true if the current user logged on the computer is Igor, or false otherwise.</summary>
        public static bool IsUserIgor
        {
            get { return (UserNameLowerCase == "igor"); }
        }

        /// <summary>Returns true if the current user logged on the computer is Tadej, or false otherwise.</summary>
        public static bool IsUserTadej
        {
            get
            {
                return (UserNameLowerCase == "tadej" || UserNameLowerCase == "tako78" ||
                UserNameLowerCase == "tadejk");
            }
        }



        #endregion UserData


        #region ComputerInformation

        #region ConmputerID

        // This region includes utilities for obtaining information that can be used to identify
        // the computer on which code is executing.

        /// <summary>Returns name of the computer on which application is running.</summary>
        public static string GetComputerName()
        {
            return Environment.MachineName;
        }

        /// <summary>Returns the MAC address of the network interface card with maximal speed.
        /// <para>The returned string represents hexadecimal MAC address without ':', '-' or other punctation marks.</para></summary>
        public static string GetMacAddressFastest()
        {
            const int minOutputLevel = 6;
            const int minMacAddressLength = 12;
            string macAddress = "";
            long maxSpeed = -1;
            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (Util.OutputLevel >= minOutputLevel)
                    Console.WriteLine("Found MAC Address: " + nic.GetPhysicalAddress().ToString() + "; Type: " + nic.NetworkInterfaceType
                        + "; Speed: " + nic.Speed);
                string tempMac = nic.GetPhysicalAddress().ToString();
                if (nic.Speed > maxSpeed && !String.IsNullOrEmpty(tempMac) && tempMac.Length >= minMacAddressLength)
                {
                    if (Util.OutputLevel >= minOutputLevel)
                        Console.WriteLine("New Max Speed = " + nic.Speed + ", MAC: " + tempMac);
                    maxSpeed = nic.Speed;
                    macAddress = tempMac;
                }
            }
            if (Util.OutputLevel >= minOutputLevel)
                Console.WriteLine("Returned MAC address: " + macAddress);
            return macAddress;
        }

        #endregion ConmputerID

        /// <summary>Returns the current local IP address of computer.</summary>
        public static string GetIpAddressLocal()
        {
            IPHostEntry host;
            string localIP = "?";
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily.ToString() == "InterNetwork")
                {
                    localIP = ip.ToString();
                }
            }
            return localIP;
        }

        /// <summary>Returns the domain name associated with the current user.</summary>
        /// <returns></returns>
        public static string GetUserDomainName()
        {
            return Environment.UserDomainName;
        }

        /// <summary>Returns the network domain name associated with the current user.</summary>
        /// <returns></returns>
        public static string GetDomainName()
        {
            Console.WriteLine(Environment.NewLine + Environment.NewLine
                + "WARNING: " + Environment.NewLine
                + "Domain name does not work correctly - it returns user domain."
                + Environment.NewLine + Environment.NewLine);
            return Environment.UserDomainName;
        }


        /// <summary>Returns a string containing basic system information - name of the current user,
        /// computer name, domain name, IP address, MAC address, and runtime version.</summary>
        /// <returns></returns>
        public static string GetSystemInfoString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Current user:    " + UserName);
            sb.AppendLine("Computer name:   " + GetComputerName());
            sb.AppendLine("Domain name:     " + GetDomainName());
            sb.AppendLine("IP address:      " + GetIpAddressLocal());
            sb.AppendLine("MAC address:     " + GetMacAddressFastest());
            sb.AppendLine("Runtime version: " + GetRuntimeVersionString());
            return sb.ToString();
        }

        #endregion ComputerInformation


        #region OperatingSystem

        private static bool _isOsDetected = false;

        private static bool _isWindowsOs = false;

        private static bool _isLinuxOs = false;

        private static bool _isMacOs = false;

        /// <summary>Detects operating system.</summary>
        private static void DetectOs()
        {
            lock (LockStatic)
            {
                OperatingSystem os = Environment.OSVersion;
                string platformStringLowercase = os.Platform.ToString().ToLower();
                string osVersionStringLowercase = os.VersionString.ToLower();
                if (osVersionStringLowercase.Contains("win"))
                    _isWindowsOs = true;
                else
                    _isWindowsOs = false;
                if (osVersionStringLowercase.Contains("linux"))
                    _isLinuxOs = true;
                else
                    _isLinuxOs = false;
                if (osVersionStringLowercase.Contains("macos"))
                    _isMacOs = true;
                else
                    _isMacOs = false;
            }
        }

        /// <summary>Returns true if the operating system is a Windows variant, false otherwise.</summary>
        public static bool IsWindowsOs
        {
            get
            {
                if (!_isOsDetected)
                    DetectOs();
                return _isWindowsOs;
            }
        }

        /// <summary>Returns true if the operating system is a Linux variant, false otherwise.</summary>
        public static bool IsLinuxOs
        {
            get
            {
                if (!_isOsDetected)
                    DetectOs();
                return _isLinuxOs;
            }
        }

        /// <summary>Returns true if the operating system is a MacOs variant, false otherwise.</summary>
        public static bool IsMaxOs
        {
            get
            {
                if (!_isOsDetected)
                    DetectOs();
                return _isMacOs;
            }
        }

        #endregion OperatingSystem


        #region Runtime

        /// <summary>Gets the version of the runtime on which the current application executes.</summary>
        public static string GetRuntimeVersionString()
        {
            return System.Environment.Version.ToString();
        }

        #endregion Runtime


        #region SystemExecution


        /// <summary>Executes system command with arguments synchronously (blocks until the 
        /// process that is created exits).</summary>
        /// <param name="command">Command string, usually a path to executable or other type of command.</param>
        /// <param name="args">Arguments to system command.</param>
        public static Process ExecuteSystemCommand(string command, params string[] args)
        {
            return ExecuteSystemCommand(null /* workingDirectory */, false /* asynchronous */, command, args);
        }


        /// <summary>Executes system command with arguments asynchronously (returns immediately and does not
        /// wait for the process to complete).</summary>
        /// <param name="command">Command string, usually a path to executable or other type of command.</param>
        /// <param name="args">Arguments to system command.</param>
        public static Process ExecuteSystemCommandAsync(string command, params string[] args)
        {
            return ExecuteSystemCommand(null /* workingDirectory */, true /* asynchronous */, command, args);
        }

        /// <summary>Executes system command with arguments.</summary>
        /// <param name="asynchronous">If true then the system command is executed asynchronously (the method returns immediately,
        /// and the caller can wait for the executed process by calling WaitForExit() on the returned object).
        /// If false then method blocks until the process completes.</param>
        /// <param name="command">Command string, usually a path to executable or other type of command.</param>
        /// <param name="args">Arguments to system command.</param>
        /// <returns><see cref="Process"/> object that can be used to access and manimulate the process that has
        /// been executed by this method.</returns>
        public static Process ExecuteSystemCommand(bool asynchronous, string command, params string[] args)
        {
            return ExecuteSystemCommand(null /* workingDirectory */, asynchronous, command, args);
        }

        /// <summary>Executes system command with arguments.</summary>
        /// <param name="workingDirectory">Working directory of the process that is run. If null or empty string then working
        /// sirectory is not specified (the current working directory is used).</param>
        /// <param name="asynchronous">If true then the system command is executed asynchronously (the method returns immediately,
        /// and the caller can wait for the executed process by calling WaitForExit() on the returned object).
        /// If false then method blocks until the process completes.</param>
        /// <param name="command">Command string, usually a path to executable or other type of command.</param>
        /// <param name="args">Arguments to system command.</param>
        /// <returns><see cref="Process"/> object that can be used to access and manimulate the process that has
        /// been executed by this method.</returns>
        public static Process ExecuteSystemCommand(string workingDirectory, bool asynchronous,
            string command, params string[] args)
        {
            return ExecuteSystemCommand(workingDirectory, asynchronous, false /* useShell */, false /* createNoWindow */,
                null /* redirectedOutputPath */, false /* redirectStandardOutput */,
                command, args);
        }

        /// <summary>Executes system command with arguments.</summary>
        /// <param name="workingDirectory">Working directory of the process that is run. If null or empty string then working
        /// sirectory is not specified (the current working directory is used).</param>
        /// <param name="asynchronous">If true then the system command is executed asynchronously (the method returns immediately,
        /// and the caller can wait for the executed process by calling WaitForExit() on the returned object).
        /// If false then method blocks until the process completes.</param>
        /// <param name="useShell">Whether to use the command shell (open in a new window) for execution.</param>
        /// <param name="command">Command string, usually a path to executable or other type of command.</param>
        /// <param name="args">Arguments to system command.</param>
        /// <returns><see cref="Process"/> object that can be used to access and manimulate the process that has
        /// been executed by this method.</returns>
        public static Process ExecuteSystemCommand(string workingDirectory, bool asynchronous, bool useShell,
            string command, params string[] args)
        {
            return ExecuteSystemCommand(workingDirectory, asynchronous, useShell, false /* createNoWindow */,
                null /* redirectedOutputPath */, false /* redirectStandardOutput */,
                command, args);
        }

        /// <summary>Executes system command with arguments.</summary>
        /// <param name="workingDirectory">Working directory of the process that is run. If null or empty string then working
        /// sirectory is not specified (the current working directory is used).</param>
        /// <param name="asynchronous">If true then the system command is executed asynchronously (the method returns immediately,
        /// and the caller can wait for the executed process by calling WaitForExit() on the returned object).
        /// If false then method blocks until the process completes.</param>
        /// <param name="useShell">Whether to use the command shell (open in a new window) for execution.</param>
        /// <param name="createNoWindow">If true then window is not created.</param>
        /// <param name="redirectedOutputPath">Path to the file where standard output is redirected (null means that
        /// output is not redirected to a file).</param>
        /// <param name="redirectStandardOutput">Whether standard output is redirected (in this case, there will be no output
        /// to the console). Enables suppressing output to console without redirecting it to a specified file.</param>
        /// <param name="command">Command string, usually a path to executable or other type of command.</param>
        /// <param name="args">Arguments to system command.</param>
        /// <returns><see cref="Process"/> object that can be used to access and manimulate the process that has
        /// been executed by this method.</returns>
        public static Process ExecuteSystemCommand(string workingDirectory, bool asynchronous, bool useShell,
            bool createNoWindow, string redirectedOutputPath, bool redirectStandardOutput,
            string command, params string[] args)
        {
            if (UtilSystem.IsWindowsOs)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("/c ");
                sb.Append(command);
                if (args != null)
                {
                    for (int i = 0; i < args.Length; ++i)
                    {
                        sb.Append(" ");
                        sb.Append(args[i]);
                    }
                }
                command = "cmd";
                args = new string[] { sb.ToString() };
            }
            System.Diagnostics.Process process = null;
            string argString = null;
            if (args != null || !string.IsNullOrEmpty(redirectedOutputPath))
            {
                StringBuilder sbArgs = new StringBuilder();
                for (int i = 0; i < args.Length; ++i)
                {
                    sbArgs.Append(args[i]);
                    if (i < args.Length - 1)
                        sbArgs.Append(" ");
                }
                if (!string.IsNullOrEmpty(redirectedOutputPath))
                    sbArgs.Append(" > " + redirectedOutputPath);
                argString = sbArgs.ToString();
            }
            //Console.WriteLine("Starting process by system shell...");
            //Console.WriteLine("Command:   \"" + command + "\"");
            //Console.WriteLine("Arguments: \"" + argString + "\"");
            //bool easierWay = false;
            //if (easierWay)
            //{
            //    if (string.IsNullOrEmpty(argString))
            //        process = System.Diagnostics.Process.Start(command);
            //    else
            //        process = System.Diagnostics.Process.Start(command + " " + argString);
            //}
            //else
            {
                //Create process
                process = new System.Diagnostics.Process();
                process.StartInfo.FileName = command; // path and file name of command to run
                process.StartInfo.Arguments = argString; // parameters to pass to program
                process.StartInfo.UseShellExecute = useShell;  // whether to use OS shell to start the process
                process.StartInfo.CreateNoWindow = createNoWindow; // 
                process.StartInfo.RedirectStandardOutput = redirectStandardOutput;
                // process.StartInfo.RedirectStandardOutput = true; // in this way you can read output stream by string strOutput = pProcess.StandardOutput.ReadToEnd();
                if (!string.IsNullOrEmpty(workingDirectory))
                {
                    StandardizeDirectoryPath(ref workingDirectory);
                    process.StartInfo.WorkingDirectory = workingDirectory;
                }

                // Remark for commented code below: It seems that priority can only be set for the process that runs.
                //// Set priority to that of the current process:
                //ProcessPriorityClass priorityClass = Process.GetCurrentProcess().PriorityClass;
                //process.PriorityClass = priorityClass;

                //Start the process:

                process.Start();

                ////Get program output
                //string strOutput = process.StandardOutput.ReadToEnd();
                //Wait for process to finish
                if (!asynchronous)
                    process.WaitForExit();
            }
            return process;
        }


        /// <summary>Opens the specified file in the default system's application for handling this kind of prcesses, 
        /// and returns the process that opened the file.</summary>
        /// <param name="filePath">Path to the file to be opened.</param>
        /// <returns>Process object for the eprocess where the file is opened.</returns>
        public static Process OpenFileInDefaultApplication(string filePath)
        {
            return Process.Start(filePath);
        }

        /// <summary>Opens the specified file in the system's default browser.</summary>
        /// <param name="filePath">Path of the file to be opened.</param>
        /// <returns>Process object for the eprocess where the file is opened.</returns>
        public static Process OpenFileInDefaultBrowser(string filePath)
        {
            return Process.Start("file:///" + filePath);
        }

        /// <summary>Opens the specified URL (Unique Resource locator, e.g. a web address) in the default browser.</summary>
        /// <url>Adress of page to be shown.</url>
        /// <returns>Process object for the eprocess where the file is opened.</returns>
        public static Process OpenUrlInDefaultBrowser(string url)
        {
            return Process.Start(url);
        }


        #endregion SystemExecution


        #region CurrentProcess

        ///// <summary>Returns name of the executable file (with extension) for the current process.</summary>
        //public static string GetCurrentProcessExecutable()
        //{
        //    return System.AppDomain.CurrentDomain.FriendlyName;
        //}

        /// <summary>Returns name of the executable file (with extension) for the current process.</summary>
        public static string GetCurrentProcessExecutableName()
        {
            return Path.GetFileName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
        }

        /// <summary>Returns the absolute path of the executable file (with extension) for the current process.</summary>
        public static string GetCurrentProcessExecutablePath()
        {
            return System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
        }

        /// <summary>Returns name of the executable file (with extension) for the current process.</summary>
        public static string GetCurrentProcessExecutableWithoutExtension()
        {
            return System.Diagnostics.Process.GetCurrentProcess().ProcessName;
        }



        #endregion CurrentProcess


        #region Processes

        // Operations on processes:

        /// <summary>Gets all runnning processes, and puts them to the specified list.</summary>
        /// <param name="processList">List on which the processes are put. If nul then it is
        /// allocated first. If not empty then it is cleared firts.</param>
        public static void GetAllProcesses(ref List<Process> processList)
        {
            if (processList == null)
                processList = new List<Process>();
            if (processList.Count > 0)
                processList.Clear();
            Process[] processes = Process.GetProcesses();
            foreach (Process proc in processes)
            {
                if (proc != null)
                    processList.Add(proc);
            }
        }


        /// <summary>Gets all processes with the specified name, and puts them to tehe specified list.</summary>
        /// <param name="processName">Name of the processes to be put on the list. Name is case sensitive.</param>
        /// <param name="processList">List on which the processes matching the name are put. If nul then it is
        /// allocated first. If not empty then it is cleared firts.</param>
        public static void GetProcesses(string processName, ref List<Process> processList)
        {
            if (string.IsNullOrEmpty(processName))
                throw new ArgumentException("Process name is not specified (null or empty string).");
            if (processList == null)
                processList = new List<Process>();
            if (processList.Count > 0)
                processList.Clear();
            Process[] processes = Process.GetProcessesByName(processName);
            foreach (Process proc in processes)
            {
                if (proc != null)
                    if (proc.ProcessName == processName)
                        processList.Add(proc);
            }
        }

        /// <summary>Gets all processes with the specified name, and puts them to tehe specified list.</summary>
        /// <param name="processName">Name of the processes to be put on the list.</param>
        /// <param name="caseSensitive">Whether name is case sensitive.</param>
        /// <param name="processList">List on which the processes matching the name are put. If nul then it is
        /// allocated first. If not empty then it is cleared firts.</param>
        public static void GetProcesses(string processName, bool caseSensitive, ref List<Process> processList)
        {
            if (caseSensitive)
            {
                GetProcesses(processName, ref processList);
                return;
            }
            if (string.IsNullOrEmpty(processName))
                throw new ArgumentException("Process name is not specified (null or empty string).");
            if (processList == null)
                processList = new List<Process>();
            if (processList.Count > 0)
                processList.Clear();
            // ! caseSensitive:
            Process[] processes = Process.GetProcesses();
            processName = processName.ToLower();
            foreach (Process proc in processes)
            {
                if (proc != null)
                {
                    if (proc.ProcessName.ToLower() == processName)
                        processList.Add(proc);
                }
            }
        }

        /// <summary>Gets all processes with the specified name, and puts them to tehe specified list.</summary>
        /// <param name="processName">Name of the processes to be put on the list.</param>
        /// <param name="caseSensitive">Whether name is case sensitive.</param>
        /// <param name="isFullString">Whether the <paramref name="processName"/> is a full process name
        /// (if false, it can be only its substring).</param>
        /// <param name="processList">List on which the processes matching the name are put. If nul then it is
        /// allocated first. If not empty then it is cleared firts.</param>
        public static void GetProcesses(string processName, bool caseSensitive, bool isFullString, ref List<Process> processList)
        {
            if (isFullString)
            {
                GetProcesses(processName, caseSensitive, ref processList);
                return;
            }
            if (string.IsNullOrEmpty(processName))
                throw new ArgumentException("Process name is not specified (null or empty string).");
            if (processList == null)
                processList = new List<Process>();
            if (processList.Count > 0)
                processList.Clear();
            // ! isFullString:
            Process[] processes = Process.GetProcesses();
            if (!caseSensitive)
                processName = processName.ToLower();
            foreach (Process proc in processes)
            {
                if (proc != null)
                {
                    string currentName = proc.ProcessName;
                    if (!string.IsNullOrEmpty(currentName))
                    {
                        if (!caseSensitive)
                            currentName = currentName.ToLower();
                        if (currentName.Contains(processName))
                            processList.Add(proc);
                    }
                }
            }
        }


        /// <summary>Returns true if at least one process with the specified name is running, false otherwise.</summary>
        /// <param name="processName">Name of the process for which we check whether it is running. Name is case sensitive.</param>
        /// <returns>True if at least one process with the specified name is running, false otherwise.</returns>
        public static bool IsProcessRunning(string processName)
        {
            if (string.IsNullOrEmpty(processName))
                throw new ArgumentException("Process name is not specified (null or empty string).");
            Process[] processes = Process.GetProcessesByName(processName);
            foreach (Process proc in processes)
            {
                if (proc != null)
                    if (proc.ProcessName == processName)
                        return true;
            }
            return false;
        }

        /// <summary>Returns true if at least one process with the specified name is running, false otherwise.</summary>
        /// <param name="processName">Name of the process for which we check whether it is running.</param>
        /// <param name="caseSensitive">Whether process name is case sensitive.</param>
        /// <returns>True if at least one process with the specified name is running, false otherwise.</returns>
        public static bool IsProcessRunning(string processName, bool caseSensitive)
        {
            if (caseSensitive)
            {
                return IsProcessRunning(processName);
            }
            if (string.IsNullOrEmpty(processName))
                throw new ArgumentException("Process name is not specified (null or empty string).");
            // ! caseSensitive:
            Process[] processes = Process.GetProcesses();
            processName = processName.ToLower();
            foreach (Process proc in processes)
            {
                if (proc != null)
                {
                    if (proc.ProcessName.ToLower() == processName)
                        return true;
                }
            }
            return false;
        }

        /// <summary>Returns true if at least one process with the specified name is running, false otherwise.</summary>
        /// <param name="processName">Name of the process for which we check whether it is running.</param>
        /// <param name="caseSensitive">Whether process name is case sensitive.</param>
        /// <param name="isFullString">Whether the <paramref name="processName"/> is a full process name
        /// (if false, it can be only its substring).</param>
        /// <returns>True if at least one process with the specified name is running, false otherwise.</returns>
        public static bool IsProcessRunning(string processName, bool isFullString, bool caseSensitive)
        {
            if (isFullString)
            {
                return IsProcessRunning(processName, caseSensitive);
            }
            if (string.IsNullOrEmpty(processName))
                throw new ArgumentException("Process name is not specified (null or empty string).");
            // ! isFullString:
            Process[] processes = Process.GetProcesses();
            if (!caseSensitive)
                processName = processName.ToLower();
            foreach (Process proc in processes)
            {
                if (proc != null)
                {
                    string currentName = proc.ProcessName;
                    if (!string.IsNullOrEmpty(currentName))
                    {
                        if (!caseSensitive)
                            currentName = currentName.ToLower();
                        if (currentName.Contains(processName))
                            return true;
                    }
                }
            }
            return false;
        }


        /// <summary>Kills the first running process found that has the specified process name.
        /// <para>Nothing happens if there are no such processes running. Exceptions are cought and
        /// eventually reported on console if the <see cref="Util.OutputLevel"/> >= 1.</para></summary>
        /// <param name="processName">Name of the process to be killed. Name is case sensitive.</param>
        public static void KillFirstProcess(string processName)
        {
            if (string.IsNullOrEmpty(processName))
                throw new ArgumentException("Process name is not specified (null or empty string).");
            Process[] processes = Process.GetProcessesByName(processName);
            foreach (Process proc in processes)
            {
                if (proc != null)
                    if (proc.ProcessName == processName)
                    {
                        try
                        {
                            proc.Kill();
                        }
                        catch (Exception ex)
                        {
                            if (Util.OutputLevel >= 1)
                            {
                                Console.WriteLine(Environment.NewLine +
                                    "Can not kill process " + proc.ProcessName + ": " + Environment.NewLine
                                    + "  " + ex.Message + Environment.NewLine);
                            }
                        }
                        return;
                    }
            }
        }

        /// <summary>Kills the first running process found that has the specified process name.
        /// <para>Nothing happens if there are no such processes running. Exceptions are cought and
        /// eventually reported on console if the <see cref="Util.OutputLevel"/> >= 1.</para></summary>
        /// <param name="processName">Name of the process for which we check whether it is running.</param>
        /// <param name="caseSensitive">Whether process name is case sensitive.</param>
        public static void KillFirstProcess(string processName, bool caseSensitive)
        {
            if (caseSensitive)
            {
                KillFirstProcess(processName);
                return;
            }
            if (string.IsNullOrEmpty(processName))
                throw new ArgumentException("Process name is not specified (null or empty string).");
            // ! caseSensitive:
            Process[] processes = Process.GetProcesses();
            processName = processName.ToLower();
            foreach (Process proc in processes)
            {
                if (proc != null)
                {
                    if (proc.ProcessName.ToLower() == processName)
                    {
                        try
                        {
                            proc.Kill();
                        }
                        catch (Exception ex)
                        {
                            if (Util.OutputLevel >= 1)
                            {
                                Console.WriteLine(Environment.NewLine +
                                    "Can not kill process " + proc.ProcessName + ": " + Environment.NewLine
                                    + "  " + ex.Message + Environment.NewLine);
                            }
                        }
                        return;
                    }
                }
            }
        }

        /// <summary>Kills the first running process found that has the specified process name.
        /// <para>Nothing happens if there are no such processes running. Exceptions are cought and
        /// eventually reported on console if the <see cref="Util.OutputLevel"/> >= 1.</para></summary>
        /// <param name="processName">Name of the process for which we check whether it is running.</param>
        /// <param name="caseSensitive">Whether process name is case sensitive.</param>
        /// <param name="isFullString">Whether the <paramref name="processName"/> is a full process name
        /// (if false, it can be only its substring).</param>
        public static void KillFirstProcess(string processName, bool caseSensitive, bool isFullString)
        {
            if (isFullString)
            {
                KillFirstProcess(processName, caseSensitive);
                return;
            }
            if (string.IsNullOrEmpty(processName))
                throw new ArgumentException("Process name is not specified (null or empty string).");
            // ! caseSensitive:
            Process[] processes = Process.GetProcesses();
            if (!caseSensitive)
                processName = processName.ToLower();
            foreach (Process proc in processes)
            {
                if (proc != null)
                {
                    string currentName = proc.ProcessName;
                    if (!string.IsNullOrEmpty(currentName))
                    {
                        if (!caseSensitive)
                            currentName = currentName.ToLower();
                        if (currentName.Contains(processName))
                        {
                            try
                            {
                                proc.Kill();
                            }
                            catch (Exception ex)
                            {
                                if (Util.OutputLevel >= 1)
                                {
                                    Console.WriteLine(Environment.NewLine +
                                        "Can not kill process " + proc.ProcessName + ": " + Environment.NewLine
                                        + "  " + ex.Message + Environment.NewLine);
                                }
                            }
                            return;
                        }
                    }
                }
            }
        }


        /// <summary>Kills all running process have the specified process name.
        /// <para>Nothing happens if there are no such processes running. Exceptions are cought and
        /// eventually reported on console if the <see cref="Util.OutputLevel"/> >= 1.</para></summary>
        /// <param name="processName">Name of the process to be killed. Name is case sensitive.</param>
        public static void KillAllProcesses(string processName)
        {
            if (string.IsNullOrEmpty(processName))
                throw new ArgumentException("Process name is not specified (null or empty string).");
            Process[] processes = Process.GetProcessesByName(processName);
            foreach (Process proc in processes)
            {
                if (proc != null)
                    if (proc.ProcessName == processName)
                    {
                        try
                        {
                            proc.Kill();
                        }
                        catch (Exception ex)
                        {
                            if (Util.OutputLevel >= 1)
                            {
                                Console.WriteLine(Environment.NewLine +
                                    "Can not kill process " + proc.ProcessName + ": " + Environment.NewLine
                                    + "  " + ex.Message + Environment.NewLine);
                            }
                        }
                    }
            }
        }

        /// <summary>Kills all running processes that have the specified process name.
        /// <para>Nothing happens if there are no such processes running. Exceptions are cought and
        /// eventually reported on console if the <see cref="Util.OutputLevel"/> >= 1.</para></summary>
        /// <param name="processName">Name of the process for which we check whether it is running.</param>
        /// <param name="caseSensitive">Whether process name is case sensitive.</param>
        public static void KillAllProcesses(string processName, bool caseSensitive)
        {
            if (caseSensitive)
            {
                KillAllProcesses(processName);
                return;
            }
            if (string.IsNullOrEmpty(processName))
                throw new ArgumentException("Process name is not specified (null or empty string).");
            // ! caseSensitive:
            Process[] processes = Process.GetProcesses();
            processName = processName.ToLower();
            foreach (Process proc in processes)
            {
                if (proc != null)
                {
                    if (proc.ProcessName.ToLower() == processName)
                    {
                        try
                        {
                            proc.Kill();
                        }
                        catch (Exception ex)
                        {
                            if (Util.OutputLevel >= 1)
                            {
                                Console.WriteLine(Environment.NewLine +
                                    "Can not kill process " + proc.ProcessName + ": " + Environment.NewLine
                                    + "  " + ex.Message + Environment.NewLine);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>Kills all running processes that have the specified process name.
        /// <para>Nothing happens if there are no such processes running. Exceptions are cought and
        /// eventually reported on console if the <see cref="Util.OutputLevel"/> >= 1.</para></summary>
        /// <param name="processName">Name of the process for which we check whether it is running.</param>
        /// <param name="caseSensitive">Whether process name is case sensitive.</param>
        /// <param name="isFullString">Whether the <paramref name="processName"/> is a full process name
        /// (if false, it can be only its substring).</param>
        public static void KillAllProcesses(string processName, bool isFullString, bool caseSensitive)
        {
            if (isFullString)
            {
                KillAllProcesses(processName, caseSensitive);
                return;
            }
            if (string.IsNullOrEmpty(processName))
                throw new ArgumentException("Process name is not specified (null or empty string).");
            // ! isFullString:
            Process[] processes = Process.GetProcesses();
            processName = processName.ToLower();
            if (!caseSensitive)
                processName = processName.ToLower();
            foreach (Process proc in processes)
            {
                if (proc != null)
                {
                    string currentName = proc.ProcessName;
                    if (!string.IsNullOrEmpty(currentName))
                    {
                        if (!caseSensitive)
                            currentName = currentName.ToLower();
                        //Console.WriteLine("Process name: " + currentName);
                        if (currentName.Contains(processName))
                        {
                            try
                            {
                                proc.Kill();
                            }
                            catch (Exception ex)
                            {
                                if (Util.OutputLevel >= 1)
                                {
                                    Console.WriteLine(Environment.NewLine +
                                        "Can not kill process " + proc.ProcessName + ": " + Environment.NewLine
                                        + "  " + ex.Message + Environment.NewLine);
                                }
                            }
                        }
                    }
                }
            }
        }


        #region Processes.Applications

        // Operations on applications. Applications are regarded processes that have a 
        // visible Main Window Title.


        /// <summary>Gets all runnning applications, and puts them to the specified list.
        /// <para>Applications are regarded all processes with visible main window and window title defined.</para></summary>
        /// <param name="processList">List on which the processes are put. If nul then it is
        /// allocated first. If not empty then it is cleared firts.</param>
        public static void GetAllApplications(ref List<Process> processList)
        {
            if (processList == null)
                processList = new List<Process>();
            if (processList.Count > 0)
                processList.Clear();
            Process[] processes = Process.GetProcesses();
            foreach (Process proc in processes)
            {
                if (proc != null)
                {
                    if (!string.IsNullOrEmpty(proc.MainWindowTitle))
                        processList.Add(proc);
                }
            }
        }


        /// <summary>Gets all applications with the specified process name, and puts them to the specified list.
        /// <para>Applications are regarded all processes with visible main window and window title defined.</para></summary>
        /// <param name="processName">Name of the processes to be put on the list. Name is case sensitive.</param>
        /// <param name="processList">List on which the application processes matching the name are put. If nul then it is
        /// allocated first. If not empty then it is cleared firts.</param>
        public static void GetApplications(string processName, ref List<Process> processList)
        {
            if (string.IsNullOrEmpty(processName))
                throw new ArgumentException("Process name is not specified (null or empty string).");
            if (processList == null)
                processList = new List<Process>();
            if (processList.Count > 0)
                processList.Clear();
            Process[] processes = Process.GetProcessesByName(processName);
            foreach (Process proc in processes)
            {
                if (proc != null)
                    if (!string.IsNullOrEmpty(proc.MainWindowTitle) && proc.ProcessName == processName)
                        processList.Add(proc);
            }
        }

        /// <summary>Gets all applications with the specified proces name, and puts them to tehe specified list.
        /// <para>Applications are regarded all processes with visible main window and window title defined.</para></summary>
        /// <param name="processName">Name of the processes to be put on the list.</param>
        /// <param name="caseSensitive">Whether name is case sensitive.</param>
        /// <param name="processList">List on which the application processes matching the name are put. If nul then it is
        /// allocated first. If not empty then it is cleared firts.</param>
        public static void GetApplications(string processName, bool caseSensitive, ref List<Process> processList)
        {
            if (caseSensitive)
            {
                GetApplications(processName, ref processList);
                return;
            }
            if (string.IsNullOrEmpty(processName))
                throw new ArgumentException("Process name is not specified (null or empty string).");
            if (processList == null)
                processList = new List<Process>();
            if (processList.Count > 0)
                processList.Clear();
            {
                // ! caseSensitive:
                Process[] processes = Process.GetProcesses();
                processName = processName.ToLower();
                foreach (Process proc in processes)
                {
                    if (proc != null)
                    {
                        if (!string.IsNullOrEmpty(proc.MainWindowTitle) && proc.ProcessName.ToLower() == processName)
                            processList.Add(proc);
                    }
                }
            }
        }

        /// <summary>Gets all applications with the specified proces name, and puts them to tehe specified list.
        /// <para>Applications are regarded all processes with visible main window and window title defined.</para></summary>
        /// <param name="processName">Name of the processes to be put on the list.</param>
        /// <param name="caseSensitive">Whether name is case sensitive.</param>
        /// <param name="isFullString">Whether the <paramref name="processName"/> is a full name
        /// (if false, it can be only a substring of the process name).</param>
        /// <param name="processList">List on which the application processes matching the name are put. If nul then it is
        /// allocated first. If not empty then it is cleared firts.</param>
        public static void GetApplications(string processName, bool caseSensitive, bool isFullString, ref List<Process> processList)
        {
            if (isFullString)
            {
                GetApplications(processName, caseSensitive, ref processList);
                return;
            }
            if (string.IsNullOrEmpty(processName))
                throw new ArgumentException("Process name is not specified (null or empty string).");
            if (processList == null)
                processList = new List<Process>();
            if (processList.Count > 0)
                processList.Clear();
            // ! isFullString:
            Process[] processes = Process.GetProcesses();
            if (!caseSensitive)
                processName = processName.ToLower();
            foreach (Process proc in processes)
            {
                if (proc != null)
                {
                    string currentName = proc.ProcessName;
                    if (!string.IsNullOrEmpty(currentName))
                    {
                        if (!caseSensitive)
                            currentName = currentName.ToLower();
                        if (!string.IsNullOrEmpty(proc.MainWindowTitle) && currentName.Contains(processName))
                            processList.Add(proc);
                    }
                }
            }
        }


        /// <summary>Returns true if at least one application with the specified process name is running, false otherwise.
        /// <para>Applications are regarded all processes with visible main window and window title defined.</para></summary>
        /// <param name="processName">Name of the process for which we check whether it is running. Name is case sensitive.</param>
        /// <returns>True if at least one process with the specified name is running, false otherwise.</returns>
        public static bool IsApplicationRunning(string processName)
        {
            if (string.IsNullOrEmpty(processName))
                throw new ArgumentException("Process name is not specified (null or empty string).");
            Process[] processes = Process.GetProcessesByName(processName);
            foreach (Process proc in processes)
            {
                if (proc != null)
                    if (!string.IsNullOrEmpty(proc.MainWindowTitle) && proc.ProcessName == processName)
                        return true;
            }
            return false;
        }

        /// <summary>Returns true if at least one application with the specified process name is running, false otherwise.
        /// <para>Applications are regarded all processes with visible main window and window title defined.</para></summary>
        /// <param name="processName">Name of the process for which we check whether it is running.</param>
        /// <param name="caseSensitive">Whether process name is case sensitive.</param>
        /// <returns>True if at least one process with the specified name is running, false otherwise.</returns>
        public static bool IsApplicationRunning(string processName, bool caseSensitive)
        {
            if (caseSensitive)
            {
                return IsApplicationRunning(processName);
            }
            if (string.IsNullOrEmpty(processName))
                throw new ArgumentException("Process name is not specified (null or empty string).");
            // ! caseSensitive:
            Process[] processes = Process.GetProcesses();
            processName = processName.ToLower();
            foreach (Process proc in processes)
            {
                if (proc != null)
                {
                    if (!string.IsNullOrEmpty(proc.MainWindowTitle) && proc.ProcessName.ToLower() == processName)
                        return true;
                }
            }
            return false;
        }

        /// <summary>Returns true if at least one application with the specified process name is running, false otherwise.
        /// <para>Applications are regarded all processes with visible main window and window title defined.</para></summary>
        /// <param name="processName">Name of the process for which we check whether it is running.</param>
        /// <param name="caseSensitive">Whether process name is case sensitive.</param>
        /// <param name="isFullString">Whether the <paramref name="processName"/> is a full name
        /// (if false, it can be only a substring of the process name).</param>
        /// <returns>True if at least one process with the specified name is running, false otherwise.</returns>
        public static bool IsApplicationRunning(string processName, bool caseSensitive, bool isFullString)
        {
            if (isFullString)
            {
                return IsApplicationRunning(processName, caseSensitive);
            }
            if (string.IsNullOrEmpty(processName))
                throw new ArgumentException("Process name is not specified (null or empty string).");
            // ! isFullString:
            Process[] processes = Process.GetProcesses();
            if (!caseSensitive)
                processName = processName.ToLower();
            foreach (Process proc in processes)
            {
                if (proc != null)
                {
                    string currentName = proc.ProcessName;
                    if (!string.IsNullOrEmpty(currentName))
                    {
                        if (!caseSensitive)
                            currentName = currentName.ToLower();
                        if (!string.IsNullOrEmpty(proc.MainWindowTitle) && currentName.Contains(processName))
                            return true;
                    }
                }
            }
            return false;
        }


        /// <summary>Kills the first running application found that has the specified process name.
        /// <para>Applications are regarded all processes with visible main window and window title defined.</para>
        /// <para>Nothing happens if there are no such processes running. Exceptions are cought and
        /// eventually reported on console if the <see cref="Util.OutputLevel"/> >= 1.</para></summary>
        /// <param name="processName">Name of the process to be killed. Name is case sensitive.</param>
        public static void KillFirstApplication(string processName)
        {
            if (string.IsNullOrEmpty(processName))
                throw new ArgumentException("Process name is not specified (null or empty string).");
            Process[] processes = Process.GetProcessesByName(processName);
            foreach (Process proc in processes)
            {
                if (proc != null)
                    if (!string.IsNullOrEmpty(proc.MainWindowTitle) && proc.ProcessName == processName)
                    {
                        try
                        {
                            proc.Kill();
                        }
                        catch (Exception ex)
                        {
                            if (Util.OutputLevel >= 1)
                            {
                                Console.WriteLine(Environment.NewLine +
                                    "Can not kill process " + proc.ProcessName + ": " + Environment.NewLine
                                    + "  " + ex.Message + Environment.NewLine);
                            }
                        }
                        return;
                    }
            }
        }

        /// <summary>Kills the first running application found that has the specified process name.
        /// <para>Applications are regarded all processes with visible main window and window title defined.</para>
        /// <para>Nothing happens if there are no such processes running. Exceptions are cought and
        /// eventually reported on console if the <see cref="Util.OutputLevel"/> >= 1.</para></summary>
        /// <param name="processName">Name of the process for which we check whether it is running.</param>
        /// <param name="caseSensitive">Whether process name is case sensitive.</param>
        public static void KillFirstApplication(string processName, bool caseSensitive)
        {
            if (caseSensitive)
            {
                KillFirstApplication(processName);
                return;
            }
            if (string.IsNullOrEmpty(processName))
                throw new ArgumentException("Process name is not specified (null or empty string).");
            // ! caseSensitive:
            Process[] processes = Process.GetProcesses();
            processName = processName.ToLower();
            foreach (Process proc in processes)
            {
                if (proc != null)
                {
                    if (!string.IsNullOrEmpty(proc.MainWindowTitle) && proc.ProcessName.ToLower() == processName)
                    {
                        try
                        {
                            proc.Kill();
                        }
                        catch (Exception ex)
                        {
                            if (Util.OutputLevel >= 1)
                            {
                                Console.WriteLine(Environment.NewLine +
                                    "Can not kill process " + proc.ProcessName + ": " + Environment.NewLine
                                    + "  " + ex.Message + Environment.NewLine);
                            }
                        }
                        return;
                    }
                }
            }
        }

        /// <summary>Kills the first running application found that has the specified process name.
        /// <para>Applications are regarded all processes with visible main window and window title defined.</para>
        /// <para>Nothing happens if there are no such processes running. Exceptions are cought and
        /// eventually reported on console if the <see cref="Util.OutputLevel"/> >= 1.</para></summary>
        /// <param name="processName">Name of the process for which we check whether it is running.</param>
        /// <param name="caseSensitive">Whether process name is case sensitive.</param>
        /// <param name="isFullString">Whether the <paramref name="processName"/> is a full name
        /// (if false, it can be only a substring of the process name).</param>
        public static void KillFirstApplication(string processName, bool caseSensitive, bool isFullString)
        {
            if (isFullString)
            {
                KillFirstApplication(processName, caseSensitive);
                return;
            }
            if (string.IsNullOrEmpty(processName))
                throw new ArgumentException("Process name is not specified (null or empty string).");
            // ! isFullString:
            Process[] processes = Process.GetProcesses();
            if (!caseSensitive)
                processName = processName.ToLower();
            foreach (Process proc in processes)
            {
                if (proc != null)
                {
                    string currentName = proc.ProcessName;
                    if (!string.IsNullOrEmpty(currentName))
                    {
                        if (!caseSensitive)
                            currentName = currentName.ToLower();
                        if (!string.IsNullOrEmpty(proc.MainWindowTitle) && currentName.Contains(processName))
                        {
                            try
                            {
                                proc.Kill();
                            }
                            catch (Exception ex)
                            {
                                if (Util.OutputLevel >= 1)
                                {
                                    Console.WriteLine(Environment.NewLine +
                                        "Can not kill process " + proc.ProcessName + ": " + Environment.NewLine
                                        + "  " + ex.Message + Environment.NewLine);
                                }
                            }
                            return;
                        }
                    }
                }
            }
        }


        /// <summary>Kills the all running applications that has the specified process name.
        /// <para>Applications are regarded all processes with visible main window and window title defined.</para>
        /// <para>Nothing happens if there are no such processes running. Exceptions are cought and
        /// eventually reported on console if the <see cref="Util.OutputLevel"/> >= 1.</para></summary>
        /// <param name="processName">Name of the process to be killed. Name is case sensitive.</param>
        public static void KillAllApplications(string processName)
        {
            if (string.IsNullOrEmpty(processName))
                throw new ArgumentException("Process name is not specified (null or empty string).");
            Process[] processes = Process.GetProcessesByName(processName);
            foreach (Process proc in processes)
            {
                if (proc != null)
                    if (!string.IsNullOrEmpty(proc.MainWindowTitle) && proc.ProcessName == processName)
                    {
                        try
                        {
                            proc.Kill();
                        }
                        catch (Exception ex)
                        {
                            if (Util.OutputLevel >= 1)
                            {
                                Console.WriteLine(Environment.NewLine +
                                    "Can not kill process " + proc.ProcessName + ": " + Environment.NewLine
                                    + "  " + ex.Message + Environment.NewLine);
                            }
                        }
                    }
            }
        }

        /// <summary>Kills all running applications that have the specified process name.
        /// <para>Applications are regarded all processes with visible main window and window title defined.</para>
        /// <para>Nothing happens if there are no such processes running. Exceptions are cought and
        /// eventually reported on console if the <see cref="Util.OutputLevel"/> >= 1.</para></summary>
        /// <param name="processName">Name of the process for which we check whether it is running.</param>
        /// <param name="caseSensitive">Whether process name is case sensitive.</param>
        public static void KillAllApplications(string processName, bool caseSensitive)
        {
            if (caseSensitive)
            {
                KillAllApplications(processName);
                return;
            }
            if (string.IsNullOrEmpty(processName))
                throw new ArgumentException("Process name is not specified (null or empty string).");
            // ! caseSensitive:
            Process[] processes = Process.GetProcesses();
            processName = processName.ToLower();
            foreach (Process proc in processes)
            {
                if (proc != null)
                {
                    if (!string.IsNullOrEmpty(proc.MainWindowTitle) && proc.ProcessName.ToLower() == processName)
                    {
                        try
                        {
                            proc.Kill();
                        }
                        catch (Exception ex)
                        {
                            if (Util.OutputLevel >= 1)
                            {
                                Console.WriteLine(Environment.NewLine +
                                    "Can not kill process " + proc.ProcessName + ": " + Environment.NewLine
                                    + "  " + ex.Message + Environment.NewLine);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>Kills all running applications that have the specified process name.
        /// <para>Applications are regarded all processes with visible main window and window title defined.</para>
        /// <para>Nothing happens if there are no such processes running. Exceptions are cought and
        /// eventually reported on console if the <see cref="Util.OutputLevel"/> >= 1.</para></summary>
        /// <param name="processName">Name of the process for which we check whether it is running.</param>
        /// <param name="caseSensitive">Whether process name is case sensitive.</param>
        /// <param name="isFullString">Whether the <paramref name="processName"/> is a full name
        /// (if false, it can be only a substring of the process name).</param>
        public static void KillAllApplications(string processName, bool caseSensitive, bool isFullString)
        {
            if (isFullString)
            {
                KillAllApplications(processName, caseSensitive);
                return;
            }
            if (string.IsNullOrEmpty(processName))
                throw new ArgumentException("Process name is not specified (null or empty string).");
            // ! isFullString:
            Process[] processes = Process.GetProcesses();
            if (!caseSensitive)
                processName = processName.ToLower();
            foreach (Process proc in processes)
            {
                if (proc != null)
                {
                    string currentName = proc.ProcessName;
                    // Console.WriteLine("Process: " + currentName);
                    if (!string.IsNullOrEmpty(currentName))
                    {
                        if (!caseSensitive)
                            currentName = currentName.ToLower();
                        if (!string.IsNullOrEmpty(proc.MainWindowTitle) && currentName.Contains(processName))
                        {
                            try
                            {
                                proc.Kill();
                            }
                            catch (Exception ex)
                            {
                                if (Util.OutputLevel >= 1)
                                {
                                    Console.WriteLine(Environment.NewLine +
                                        "Can not kill process " + proc.ProcessName + ": " + Environment.NewLine
                                        + "  " + ex.Message + Environment.NewLine);
                                }
                            }
                        }
                    }
                }
            }
        }

        #endregion Processes.Applications


        #region Processes.ApplicationsByWindowTitle

        // Operations on applications tccording to their window title. Applications are regarded processes that have a 
        // visible Main Window Title.


        /// <summary>Gets all applications with the specified main window title, and puts them to the specified list.
        /// <para>Applications are regarded all processes with visible main window and window title defined.</para></summary>
        /// <param name="mainWindowTitle">Applications' main window title. Name is case sensitive.</param>
        /// <param name="processList">List on which the application processes matching the name are put. If nul then it is
        /// allocated first. If not empty then it is cleared firts.</param>
        public static void GetApplicationsByWindowTitle(string mainWindowTitle, ref List<Process> processList)
        {
            if (string.IsNullOrEmpty(mainWindowTitle))
                throw new ArgumentException("Process name is not specified (null or empty string).");
            if (processList == null)
                processList = new List<Process>();
            if (processList.Count > 0)
                processList.Clear();
            Process[] processes = Process.GetProcessesByName(mainWindowTitle);
            foreach (Process proc in processes)
            {
                if (proc != null)
                    if (!string.IsNullOrEmpty(proc.MainWindowTitle) && proc.MainWindowTitle == mainWindowTitle)
                        processList.Add(proc);
            }
        }

        /// <summary>Gets all applications with the specified main window title, and puts them to tehe specified list.
        /// <para>Applications are regarded all processes with visible main window and window title defined.</para></summary>
        /// <param name="mainWindowTitle">Applications' main window title.</param>
        /// <param name="caseSensitive">Whether name is case sensitive.</param>
        /// <param name="processList">List on which the application processes matching the name are put. If nul then it is
        /// allocated first. If not empty then it is cleared firts.</param>
        public static void GetApplicationsByWindowTitle(string mainWindowTitle, bool caseSensitive, ref List<Process> processList)
        {
            if (caseSensitive)
            {
                GetApplicationsByWindowTitle(mainWindowTitle, ref processList);
                return;
            }
            if (string.IsNullOrEmpty(mainWindowTitle))
                throw new ArgumentException("Process name is not specified (null or empty string).");
            if (processList == null)
                processList = new List<Process>();
            if (processList.Count > 0)
                processList.Clear();
            {
                // ! caseSensitive:
                Process[] processes = Process.GetProcesses();
                mainWindowTitle = mainWindowTitle.ToLower();
                foreach (Process proc in processes)
                {
                    if (proc != null)
                    {
                        if (!string.IsNullOrEmpty(proc.MainWindowTitle) && proc.MainWindowTitle.ToLower() == mainWindowTitle)
                            processList.Add(proc);
                    }
                }
            }
        }

        /// <summary>Gets all applications with the specified main window title, and puts them to tehe specified list.
        /// <para>Applications are regarded all processes with visible main window and window title defined.</para></summary>
        /// <param name="mainWindowTitle">Applications' main window title.</param>
        /// <param name="caseSensitive">Whether name is case sensitive.</param>
        /// <param name="isFullString">Whether the <paramref name="mainWindowTitle"/> is a full window title
        /// (if false, it can be only its substring).</param>
        /// <param name="processList">List on which the application processes matching the name are put. If nul then it is
        /// allocated first. If not empty then it is cleared firts.</param>
        public static void GetApplicationsByWindowTitle(string mainWindowTitle, bool caseSensitive, bool isFullString, ref List<Process> processList)
        {
            if (isFullString)
            {
                GetApplicationsByWindowTitle(mainWindowTitle, caseSensitive, ref processList);
                return;
            }
            if (string.IsNullOrEmpty(mainWindowTitle))
                throw new ArgumentException("Application's main window title is not specified (null or empty string).");
            if (processList == null)
                processList = new List<Process>();
            if (processList.Count > 0)
                processList.Clear();
            // ! isFullString:
            Process[] processes = Process.GetProcesses();
            if (!caseSensitive)
                mainWindowTitle = mainWindowTitle.ToLower();
            foreach (Process proc in processes)
            {
                if (proc != null)
                {
                    string currentTitle = proc.MainWindowTitle;
                    if (!string.IsNullOrEmpty(currentTitle))
                    {
                        if (!caseSensitive)
                            currentTitle = currentTitle.ToLower();
                        if (currentTitle.Contains(mainWindowTitle))
                        {
                            processList.Add(proc);
                        }
                    }
                }
            }
        }

        /// <summary>Returns true if at least one application with the specified main window title is running, false otherwise.
        /// <para>Applications are regarded all processes with visible main window and window title defined.</para></summary>
        /// <param name="mainWindowTitle">Application's main window title. Name is case sensitive.</param>
        /// <returns>True if at least one process with the specified name is running, false otherwise.</returns>
        public static bool IsApplicationRunningByWindowTitle(string mainWindowTitle)
        {
            if (string.IsNullOrEmpty(mainWindowTitle))
                throw new ArgumentException("Process name is not specified (null or empty string).");
            Process[] processes = Process.GetProcessesByName(mainWindowTitle);
            foreach (Process proc in processes)
            {
                if (proc != null)
                    if (!string.IsNullOrEmpty(proc.MainWindowTitle) && proc.MainWindowTitle == mainWindowTitle)
                        return true;
            }
            return false;
        }

        /// <summary>Returns true if at least one application with the specified main window title is running, false otherwise.
        /// <para>Applications are regarded all processes with visible main window and window title defined.</para></summary>
        /// <param name="mainWindowTitle">Application's main window title.</param>
        /// <param name="caseSensitive">Whether process name is case sensitive.</param>
        /// <returns>True if at least one process with the specified name is running, false otherwise.</returns>
        public static bool IsApplicationRunningByWindowTitle(string mainWindowTitle, bool caseSensitive)
        {
            if (caseSensitive)
            {
                return IsApplicationRunningByWindowTitle(mainWindowTitle);
            }
            if (string.IsNullOrEmpty(mainWindowTitle))
                throw new ArgumentException("Process name is not specified (null or empty string).");
            // ! caseSensitive:
            Process[] processes = Process.GetProcesses();
            mainWindowTitle = mainWindowTitle.ToLower();
            foreach (Process proc in processes)
            {
                if (proc != null)
                {
                    if (!string.IsNullOrEmpty(proc.MainWindowTitle) && proc.MainWindowTitle.ToLower() == mainWindowTitle)
                        return true;
                }
            }
            return false;
        }

        /// <summary>Returns true if at least one application with the specified main window title is running, false otherwise.
        /// <para>Applications are regarded all processes with visible main window and window title defined.</para></summary>
        /// <param name="mainWindowTitle">Application's main window title.</param>
        /// <param name="caseSensitive">Whether process name is case sensitive.</param>
        /// <param name="isFullString">Whether the name is full name.</param>
        /// <returns>True if at least one process with the specified name is running, false otherwise.</returns>
        public static bool IsApplicationRunningByWindowTitle(string mainWindowTitle, bool caseSensitive, bool isFullString)
        {
            if (isFullString)
            {
                return IsApplicationRunningByWindowTitle(mainWindowTitle, caseSensitive);
            }
            if (string.IsNullOrEmpty(mainWindowTitle))
                throw new ArgumentException("Application's main window title is not specified (null or empty string).");
            // ! isFullString:
            Process[] processes = Process.GetProcesses();
            if (!caseSensitive)
                mainWindowTitle = mainWindowTitle.ToLower();
            foreach (Process proc in processes)
            {
                if (proc != null)
                {
                    string currentTitle = proc.MainWindowTitle;
                    if (!string.IsNullOrEmpty(currentTitle))
                    {
                        if (!caseSensitive)
                            currentTitle = currentTitle.ToLower();
                        if (currentTitle.Contains(mainWindowTitle))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }


        /// <summary>Kills the first running application found that has the specified main window title.
        /// <para>Applications are regarded all processes with visible main window and window title defined.</para>
        /// <para>Nothing happens if there are no such processes running. Exceptions are cought and
        /// eventually reported on console if the <see cref="Util.OutputLevel"/> >= 1.</para></summary>
        /// <param name="mainWindowTitle">Application's main window title. Name is case sensitive.</param>
        public static void KillFirstApplicationByWindowTitle(string mainWindowTitle)
        {
            if (string.IsNullOrEmpty(mainWindowTitle))
                throw new ArgumentException("Process name is not specified (null or empty string).");
            Process[] processes = Process.GetProcessesByName(mainWindowTitle);
            foreach (Process proc in processes)
            {
                if (proc != null)
                    if (!string.IsNullOrEmpty(proc.MainWindowTitle) && proc.MainWindowTitle == mainWindowTitle)
                    {
                        try
                        {
                            proc.Kill();
                        }
                        catch (Exception ex)
                        {
                            if (Util.OutputLevel >= 1)
                            {
                                Console.WriteLine(Environment.NewLine +
                                    "Can not kill process " + proc.ProcessName + ": " + Environment.NewLine
                                    + "  " + ex.Message + Environment.NewLine);
                            }
                        }
                        return;
                    }
            }
        }

        /// <summary>Kills the first running application found that has the specified main window title.
        /// <para>Applications are regarded all processes with visible main window and window title defined.</para>
        /// <para>Nothing happens if there are no such processes running. Exceptions are cought and
        /// eventually reported on console if the <see cref="Util.OutputLevel"/> >= 1.</para></summary>
        /// <param name="mainWindowTitle">Application's main window title.</param>
        /// <param name="caseSensitive">Whether process name is case sensitive.</param>
        public static void KillFirstApplicationByWindowTitle(string mainWindowTitle, bool caseSensitive)
        {
            if (caseSensitive)
            {
                KillFirstApplicationByWindowTitle(mainWindowTitle);
                return;
            }
            if (string.IsNullOrEmpty(mainWindowTitle))
                throw new ArgumentException("Process name is not specified (null or empty string).");
            // ! caseSensitive:
            Process[] processes = Process.GetProcesses();
            mainWindowTitle = mainWindowTitle.ToLower();
            foreach (Process proc in processes)
            {
                if (proc != null)
                {
                    if (!string.IsNullOrEmpty(proc.MainWindowTitle) && proc.MainWindowTitle.ToLower() == mainWindowTitle)
                    {
                        try
                        {
                            proc.Kill();
                        }
                        catch (Exception ex)
                        {
                            if (Util.OutputLevel >= 1)
                            {
                                Console.WriteLine(Environment.NewLine +
                                    "Can not kill process " + proc.ProcessName + ": " + Environment.NewLine
                                    + "  " + ex.Message + Environment.NewLine);
                            }
                        }
                        return;
                    }
                }
            }
        }

        /// <summary>Kills the first running application found that has the specified main window title.
        /// <para>Applications are regarded all processes with visible main window and window title defined.</para>
        /// <para>Nothing happens if there are no such processes running. Exceptions are cought and
        /// eventually reported on console if the <see cref="Util.OutputLevel"/> >= 1.</para></summary>
        /// <param name="mainWindowTitle">Application's main window title.</param>
        /// <param name="caseSensitive">Whether process name is case sensitive.</param>
        /// <param name="isFullString">Whether the name is full name.</param>
        public static void KillFirstApplicationByWindowTitle(string mainWindowTitle, bool caseSensitive, bool isFullString)
        {
            if (isFullString)
            {
                KillFirstApplicationByWindowTitle(mainWindowTitle, caseSensitive);
                return;
            }
            if (string.IsNullOrEmpty(mainWindowTitle))
                throw new ArgumentException("Process name is not specified (null or empty string).");
            // ! isFullString:
            Process[] processes = Process.GetProcesses();
            if (!caseSensitive)
                mainWindowTitle = mainWindowTitle.ToLower();
            foreach (Process proc in processes)
            {
                if (proc != null)
                {
                    string currentTitle = proc.MainWindowTitle;
                    if (!string.IsNullOrEmpty(currentTitle))
                    {
                        if (!caseSensitive)
                            currentTitle = currentTitle.ToLower();
                        if (currentTitle.Contains(mainWindowTitle))
                        {
                            try
                            {
                                proc.Kill();
                            }
                            catch (Exception ex)
                            {
                                if (Util.OutputLevel >= 1)
                                {
                                    Console.WriteLine(Environment.NewLine +
                                        "Can not kill process " + proc.ProcessName + ": " + Environment.NewLine
                                        + "  " + ex.Message + Environment.NewLine);
                                }
                            }
                            return;
                        }
                    }
                }
            }
        }


        /// <summary>Kills the all running applications that has the specified main window title.
        /// <para>Applications are regarded all processes with visible main window and window title defined.</para>
        /// <para>Nothing happens if there are no such processes running. Exceptions are cought and
        /// eventually reported on console if the <see cref="Util.OutputLevel"/> >= 1.</para></summary>
        /// <param name="mainWindowTitle">Application's main window title. Name is case sensitive.</param>
        public static void KillAllApplicationsByWindowTitle(string mainWindowTitle)
        {
            if (string.IsNullOrEmpty(mainWindowTitle))
                throw new ArgumentException("Process name is not specified (null or empty string).");
            Process[] processes = Process.GetProcessesByName(mainWindowTitle);
            foreach (Process proc in processes)
            {
                if (proc != null)
                    if (!string.IsNullOrEmpty(proc.MainWindowTitle) && proc.MainWindowTitle == mainWindowTitle)
                    {
                        try
                        {
                            proc.Kill();
                        }
                        catch (Exception ex)
                        {
                            if (Util.OutputLevel >= 1)
                            {
                                Console.WriteLine(Environment.NewLine +
                                    "Can not kill process " + proc.ProcessName + ": " + Environment.NewLine
                                    + "  " + ex.Message + Environment.NewLine);
                            }
                        }
                    }
            }
        }

        /// <summary>Kills all running applications that have the specified main window title.
        /// <para>Applications are regarded all processes with visible main window and window title defined.</para>
        /// <para>Nothing happens if there are no such processes running. Exceptions are cought and
        /// eventually reported on console if the <see cref="Util.OutputLevel"/> >= 1.</para></summary>
        /// <param name="mainWindowTitle">Application's main window title.</param>
        /// <param name="caseSensitive">Whether process name is case sensitive.</param>
        public static void KillAllApplicationsByWindowTitle(string mainWindowTitle, bool caseSensitive)
        {
            if (caseSensitive)
            {
                KillAllApplicationsByWindowTitle(mainWindowTitle);
                return;
            }
            if (string.IsNullOrEmpty(mainWindowTitle))
                throw new ArgumentException("Process name is not specified (null or empty string).");
            // ! caseSensitive:
            Process[] processes = Process.GetProcesses();
            mainWindowTitle = mainWindowTitle.ToLower();
            foreach (Process proc in processes)
            {
                if (proc != null)
                {
                    if (!string.IsNullOrEmpty(proc.MainWindowTitle) && proc.MainWindowTitle.ToLower() == mainWindowTitle)
                    {
                        try
                        {
                            proc.Kill();
                        }
                        catch (Exception ex)
                        {
                            if (Util.OutputLevel >= 1)
                            {
                                Console.WriteLine(Environment.NewLine +
                                    "Can not kill process " + proc.ProcessName + ": " + Environment.NewLine
                                    + "  " + ex.Message + Environment.NewLine);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>Kills all running applications that have the specified main window title.
        /// <para>Applications are regarded all processes with visible main window and window title defined.</para>
        /// <para>Nothing happens if there are no such processes running. Exceptions are cought and
        /// eventually reported on console if the <see cref="Util.OutputLevel"/> >= 1.</para></summary>
        /// <param name="mainWindowTitle">Application's main window title.</param>
        /// <param name="caseSensitive">Whether process name is case sensitive.</param>
        /// <param name="isFullString">Whether the <paramref name="mainWindowTitle"/> is a full window title
        /// (if false, it can be only its substring).</param>
        public static void KillAllApplicationsByWindowTitle(string mainWindowTitle, bool caseSensitive, bool isFullString)
        {
            if (isFullString)
            {
                KillAllApplicationsByWindowTitle(mainWindowTitle, caseSensitive);
                return;
            }
            if (string.IsNullOrEmpty(mainWindowTitle))
                throw new ArgumentException("Process name is not specified (null or empty string).");
            // ! isFullString:
            Process[] processes = Process.GetProcesses();
            if (!caseSensitive)
                mainWindowTitle = mainWindowTitle.ToLower();
            foreach (Process proc in processes)
            {
                if (proc != null)
                {
                    string currentTitle = proc.MainWindowTitle;
                    if (!string.IsNullOrEmpty(currentTitle))
                    {
                        if (!caseSensitive)
                            currentTitle = currentTitle.ToLower();
                        if (currentTitle.Contains(mainWindowTitle))
                        {
                            try
                            {
                                proc.Kill();
                            }
                            catch (Exception ex)
                            {
                                if (Util.OutputLevel >= 1)
                                {
                                    Console.WriteLine(Environment.NewLine +
                                        "Can not kill process " + proc.ProcessName + ": " + Environment.NewLine
                                        + "  " + ex.Message + Environment.NewLine);
                                }
                            }
                        }
                    }
                }
            }
        }


        #endregion Processes.ApplicationsByWindowTitle



        #endregion Processes


        #region ThreadPriority


        /// <summary>Converts the specified <see cref="ProcessPriorityClass"/> enum value to the approcimately equivalent
        /// <see cref="ThreadPriority"/> value and returns it.</summary>
        /// <param name="processPriority">Process priority value to be converted to thread priority.</param>
        public static ThreadPriority ProcessToThreadPriority(ProcessPriorityClass processPriority)
        {
            switch (processPriority)
            {
                case ProcessPriorityClass.Idle:
                    return ThreadPriority.Lowest;
                case ProcessPriorityClass.BelowNormal:
                    return ThreadPriority.BelowNormal;
                case ProcessPriorityClass.Normal:
                    return ThreadPriority.Normal;
                case ProcessPriorityClass.AboveNormal:
                    return ThreadPriority.AboveNormal;
                case ProcessPriorityClass.High:
                    return ThreadPriority.Highest;
                case ProcessPriorityClass.RealTime:
                    return ThreadPriority.Highest;
                default:
                    return ThreadPriority.Normal;
            }
        }


        /// <summary>Converts the specified <see cref="ThreadPriority"/> enum value to the approcimately equivalent
        /// <see cref="ProcessPriorityClass"/> value and returns it.</summary>
        /// <param name="threadPriority">Thread priority value to be converted to thread priority.</param>
        public static ProcessPriorityClass ThreadToProcessPriority(ThreadPriority threadPriority)
        {
            switch (threadPriority)
            {
                case ThreadPriority.Lowest:
                    return ProcessPriorityClass.Idle;
                case ThreadPriority.BelowNormal:
                    return ProcessPriorityClass.BelowNormal;
                case ThreadPriority.Normal:
                    return ProcessPriorityClass.Normal;
                case ThreadPriority.AboveNormal:
                    return ProcessPriorityClass.AboveNormal;
                case ThreadPriority.Highest:
                    return ProcessPriorityClass.RealTime;
                default:
                    return ProcessPriorityClass.Normal;
            }
        }


        private static bool _dynamicThreadPriority = true;

        /// <summary>Whether the <see cref="ThreadPriority"/> property should be obtained dynamically from the 
        /// process priority each time its getter is called, or the value that is set should be used until it
        /// is not changed explicitly.
        /// <para>Default is true.</para>
        /// <para>By setting the <see cref="ThreadPriority"/> property, this flag is automatically set to false.</para></summary>
        public static bool DynamicThreadPriority
        {
            get { lock (LockStatic) { return _dynamicThreadPriority; } }
            set { lock (LockStatic) { _dynamicThreadPriority = value; } }
        }


        private static ThreadPriority _threadPriority = ProcessToThreadPriority(Process.GetCurrentProcess().PriorityClass);

        /// <summary>Global thread priority.
        /// <para>Gets or sets priority that should be given to the newly created threads that
        /// use this instrument.</para>
        /// <para>If the <see cref="DynamicThreadPriority"/> property is set to true then each time the getter
        /// is called, the value of the property will be obtained anew from the process priority class. Otherwise, 
        /// the value that has been set last is used.</para>
        /// <para>Setting the property value will automatically set the <see cref="DynamicThreadPriority"/> to false.</para></summary>
        public static ThreadPriority ThreadPriority
        {
            get
            {
                ThreadPriority ret;
                bool priorityChanged = false;
                lock (LockStatic)
                {
                    if (_dynamicThreadPriority)
                    {
                        ThreadPriority priority = GetThreadPriorityFromProcess();
                        if (priority != _threadPriority)
                        {
                            priorityChanged = true;
                            _threadPriority = priority;
                        }
                    }
                    ret = _threadPriority;
                }
                if (priorityChanged)
                    OnThreadPriorityChange();
                return ret;
            }
            set
            {
                bool priorityChanged = false;
                lock (LockStatic)
                {
                    if (value != _threadPriority)
                        priorityChanged = true;
                    _dynamicThreadPriority = false;
                    _threadPriority = value;
                }
                if (priorityChanged)
                {
                    OnThreadPriorityChange();
                }
            }
        }


        /// <summary>Returns the thread priority value that is equivallent to the current process' priority class.</summary>
        public static ThreadPriority GetThreadPriorityFromProcess()
        {
            return ProcessToThreadPriority(Process.GetCurrentProcess().PriorityClass);
        }

        /// <summary>Updates the global thread priority (the <see cref="UtilSystem.ThreadPriority"/> property ) 
        /// in such a way that it is the same as the current process priority.
        /// <para>If the priority is changed by this call then the event handlers are also executed
        /// (the delegate <see cref="UtilSystem.OnThreadPriorityChange"/> is called).</para></summary>
        public static void UpdateThreadPriorityFromProcess()
        {
            ThreadPriority priority = GetThreadPriorityFromProcess();
            bool priorityChanged = false;
            lock (LockStatic)
            {
                if (priority != _threadPriority)
                {
                    priorityChanged = true;
                    _threadPriority = priority;
                }
            }
            if (priorityChanged)
                OnThreadPriorityChange();
        }


        private static ThreadStart _onThreadPriorityChange;

        /// <summary>This delegate is called when the global thread priority changes (property <see cref="UtilSystem.ThreadPriority"/>),
        /// but can also be called manually.</summary>
        public static void OnThreadPriorityChange()
        {
            ThreadStart onChange;
            lock (LockStatic)
            {
                onChange = _onThreadPriorityChange;
            }
            if (onChange != null)
                onChange();
        }

        /// <summary>Adds the specified method that is executed when the global thread priority changes.</summary>
        /// <param name="onPriorityChangeMethod">Method that is added.</param>
        public static void AddOnThreadPriorityChange(ThreadStart onPriorityChangeMethod)
        {
            lock (LockStatic)
            {
                _onThreadPriorityChange += onPriorityChangeMethod;
            }
        }

        /// <summary>Removes the specified method that is executed when the global thread priority changes.</summary>
        /// <param name="onPriorityChangeMethod">Method that is removed.</param>
        public static void RemoveOnThreadPriorityChange(ThreadStart onPriorityChangeMethod)
        {
            lock (LockStatic)
            {
                try
                {
                    _onThreadPriorityChange -= onPriorityChangeMethod;
                }
                catch { }
            }
        }



        #endregion ThreadPriority


        #region Assemblies

        // See also:
        // Assembly class: 

        // Best practices of assembly loading: https://msdn.microsoft.com/en-us/library/dd153782%28v=vs.110%29.aspx

        // RESOLVE assemblies that should be loaded (either when referencing dependencies or when dynamically loading):
        // Define AppDomain.AssemblyResolve event handler in order to resolve assemblies that could not be loaded, 
        // which can happen when a type could not be instantiated (by doing it on a wrong assembly) or when dependency
        // assemblies are not in the probing path.
        // Resolving Assembly Loads: https://msdn.microsoft.com/en-us/library/ff527268%28v=vs.110%29.aspx
        // AppDomain.AssemblyResolve Event (warning: example is useless!): https://msdn.microsoft.com/en-us/library/system.appdomain.assemblyresolve%28v=vs.110%29.aspx 
        // ResolveEventArgs.RequestingAssembly (good explanation!): https://msdn.microsoft.com/en-us/library/system.resolveeventargs.requestingassembly%28v=vs.110%29.aspx
        //   Very good article on AssemblyResolve:
        //     C# in a nutshell - see under Resolving and Loading Assemblies, and under AssemblyResolve:
        //     https://books.google.lu/books?id=_Y0rWd-Q2xkC&pg=PA548&lpg=PA548&dq=how+to+get+stream+of+resource+within+resource+file&source=bl&ots=R9FwU6WG-j&sig=ESi1A_dkYpM69Xbc2c6N0PzeE0M&hl=en&sa=X&redir_esc=y#v=onepage&q=how%20to%20get%20stream%20of%20resource%20within%20resource%20file&f=false

        // General on loading assemblies:
        // How the Runtime Locates Assemblies: https://msdn.microsoft.com/en-us/library/yx7xezcf%28v=vs.110%29.aspx

        // Other:
        // Using application domains: https://msdn.microsoft.com/en-us/library/yb506139%28v=vs.110%29.aspx
        // 



        private static volatile Assembly _executingAssembly = null;

        private static volatile Assembly _iglibAssembly = null;

        /// <summary>Returns assembly of the current executable, obtained by <see cref="Assembly.GetEntryAssembly()"/>.</summary>
        public static Assembly ExecutableAssembly
        {
            get
            {
                if (_executingAssembly == null)
                {
                    lock (LockStatic)
                    {
                        if (_executingAssembly == null)
                        {
                            _executingAssembly = Assembly.GetEntryAssembly();
                        }
                    }
                }
                return _executingAssembly;
            }
        }

        /// <summary>Returns assembly of the IGLib base assembly.</summary>
        public static Assembly IglibAssembly
        {
            get
            {
                if (_iglibAssembly == null)
                {
                    lock (LockStatic)
                    {
                        if (_iglibAssembly == null)
                        {
                            // _iglibAssembly = Assembly.GetExecutingAssembly();  // slower, not used
                            _iglibAssembly = typeof(IG.Lib.UtilSystem).Assembly;  // since this type is defined in IGLib assembly
                        }
                    }
                }
                return _iglibAssembly;
            }
        }
        
        
        /// <summary>Finnds and returns assembly specified by file name.</summary>
        /// <param name="assemblyName">Name of the assembly file.</param>
        /// <param name="caseSensitive">Whether names are case sensitive.</param>
        /// <param name="loadIfNecessary">Whether assembly can be loaded.</param>
        /// <param name="byFileName">Whether assemblies are searched by file name.</param>
        public static Assembly GetAssemblyByName(string assemblyName, bool caseSensitive = false, bool loadIfNecessary = true, bool byFileName = false)
        {
            return GetAssemblyByNameOrFileName(assemblyName, caseSensitive, loadIfNecessary, true /* byName  */, false /* byFileName  */);
        }
        
        /// <summary>Finnds and returns assembly specified by file name.</summary>
        /// <param name="assemblyName">Name of the assembly file.</param>
        /// <param name="caseSensitive">Whether names are case sensitive.</param>
        /// <param name="loadIfNecessary">Whether assembly can be loaded.</param>
        public static Assembly GetAssemblyByFileName(string assemblyName, bool caseSensitive = false, bool loadIfNecessary = true)
        {
            return GetAssemblyByNameOrFileName(assemblyName, caseSensitive, loadIfNecessary, false /* byName  */, true /* byFileName  */);
        }

        /// <summary>Finnds and returns assembly specified by name.</summary>
        /// <param name="assemblyName">Name of the assembly.</param>
        /// <param name="caseSensitive">Whether names are case sensitive.</param>
        /// <param name="loadIfNecessary">Whether assembly can be loaded.</param>
        /// <param name="byName">Whether assembly is searched for by name rather than by its filenmae.</param>
        /// <param name="byFileName">If true then assembly is searched for by file name.</param>
        public static Assembly GetAssemblyByNameOrFileName(string assemblyName, bool caseSensitive = false, bool loadIfNecessary = true,
            bool byName = true, bool byFileName = true)
        {
            Assembly ret = null;
            if (caseSensitive)
            {
                ret = AppDomain.CurrentDomain.GetAssemblies().
                    SingleOrDefault(assembly => (byName && assembly.GetName().Name == assemblyName) || (byFileName && UtilSystem.GetAssemblyFileName(assembly) == assemblyName) );
            }
            else
            {
                ret = AppDomain.CurrentDomain.GetAssemblies().
                    SingleOrDefault(assembly => (byName && assembly.GetName().Name.ToLower() == assemblyName.ToLower()) 
                        || (byFileName && UtilSystem.GetAssemblyFileName(assembly).ToLower() == assemblyName.ToLower()));
            }
            if (ret == null)
            {

            }
            if (ret == null)
            {

                Assembly[] loadedAssemblies = GetLoadedAssemblies();
                foreach (Assembly assembly in loadedAssemblies)
                {
                    if (assembly != null)
                    {
                        string name = assembly.GetName().Name;
                        string fileName = UtilSystem.GetAssemblyFileName(assembly);
                        if (caseSensitive)
                        {
                            if ( (byName && name == assemblyName ) || (byFileName && fileName == assemblyName) )
                                return assembly;
                        }
                        else
                        {
                            if ((byName && name.ToLower() == assemblyName.ToLower()) || (byFileName && fileName.ToLower() == assemblyName.ToLower()) )
                                return assembly;
                        }
                    }
                }

                Assembly[] referencedAssemblies = GetReferencedAssembliesRecursive();
                if (referencedAssemblies != null)
                {
                    foreach (Assembly assembly in referencedAssemblies)
                    {
                        if (assembly != null)
                        {
                            string name = assembly.GetName().Name;
                            string fileName = UtilSystem.GetAssemblyFileName(assembly);
                            if (caseSensitive)
                            {
                                if ((byName && name == assemblyName) || (byFileName && fileName == assemblyName) )
                                    return assembly;
                            }
                            else
                            {
                                if ((byName && name.ToLower() == assemblyName.ToLower()) || (byFileName && fileName.ToLower() == assemblyName.ToLower()) )
                                    return assembly;
                            }
                        }
                    }
                }
            }
            return ret;
        }

        /// <summary>Returns a list of all currently loaded assemblies in the applicattion.</summary>
        public static Assembly[] GetLoadedAssemblies()
        {
            Assembly[] loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            return loadedAssemblies;
        }


        /// <summary>Adds all loaded assembly from the current application domain to the specified list, and returns the list.</summary>
        /// <param name="assemblyList">List on which loaded assemblies are loaded. If null then the list is created internally
        /// (inboth cases the list is returned).</param>
        /// <param name="clearBefore">Spwcifies whether the list should be cleared fitst in order to remove any existent items on the list.</param>
        /// <returns>List onto which the loaded assemblies were added. This makes the method usable even if the <paramref name="assemblyList"/>
        /// is null, in which case the list is allocated within the method.</returns>
        public static IList<Assembly> GetLoadedAssemblies(IList<Assembly> assemblyList, bool clearBefore = true)
        {
            if (assemblyList == null)
                assemblyList = new List<Assembly>();
            else if (clearBefore)
                assemblyList.Clear();
            Assembly[] loadedAssemblies = GetLoadedAssemblies();
            foreach (Assembly asm in loadedAssemblies)
                assemblyList.Add(asm);
            return assemblyList;
        }



        #region Assembly.ReferencedAssemblies


        // TODO: GetReferencedAssemblies() class of functions can return assemblies that are not null but that can 
        // not be worked on because their dependencies are not loaded. The following type of exception is thrown if a
        // type is checked in such an assembly by the assembly.GetType(formClassName2, false):
        //   Cannot resolve dependency to assembly 'System.Drawing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a' because it has not been preloaded. When using the ReflectionOnly APIs, dependent assemblies must be pre-loaded or loaded on demand 
        // This error can be reproduced e.g. by 
        //   Internal IG.Script.AppExtBase FormDemo OpenForm IG.Forms.TestForm
        // if the first check iwthin the corresponding function is set to GetReferencedAssemblies() instead of 
        // GetLoadedAssemblies().
        // Check what can be done in this respect. Sometimes one would also like to check all referenced fo a given
        // class that could then be instantiated.



        /// <summary>Assemblies directly referenced by the current executale assembly.
        /// Calculated only once, lazy evaluation.</summary>
        private static volatile Assembly[] _referencedAssembliesDirectWithoutGac;

        /// <summary>Assemblies directly or indirectly referenced by the current executale assembly.
        /// Calculated only once, lazy evaluation.</summary>
        private static volatile Assembly[] _referencedAssembliesRecursiveWithoutGac;

        /// <summary>Assemblies directly referenced by the current executale assembly.
        /// Assemblies from global assembly cache are also included.
        /// Calculated only once, lazy evaluation.</summary>
        private static volatile Assembly[] _referencedAssembliesDirect;

        /// <summary>Assemblies directly or indirectly referenced by the current executale assembly.
        /// Calculated only once, lazy evaluation.</summary>
        private static volatile Assembly[] _referencedAssembliesRecursive;

        /// <summary>Returns an array containing the executable assembly and all its DIRECTLY referenced assemblies.
        /// Assemblies from the Global Assembly Cache (GAC) are NOT included.</summary>
        /// <remarks>Array of assemblies is calculated only once (the first time it is needed) and is stored internally for 
        /// to speed up further uses.
        /// <para>See:</para>
        /// <para>http://stackoverflow.com/questions/383686/how-do-you-loop-through-currently-loaded-assemblies </para>
        /// <para>Assembly names, http://msdn.microsoft.com/en-us/library/k8xx4k69%28v=vs.110%29.aspx </para></remarks>
        public static Assembly[] GetReferencedAssembliesWithoutGac()
        {
            if (_referencedAssembliesDirectWithoutGac == null)
            {
                lock (LockStatic)
                {
                    if (_referencedAssembliesDirectWithoutGac == null)
                    {
                        Dictionary<string, Assembly> assemblydict = GetReferencedAssemblies(ExecutableAssembly,
                            ignoreGac: true, recursive: false);
                        List<Assembly> assemblies = new List<Assembly>();
                        assemblies.Add(ExecutableAssembly);
                        foreach (Assembly assembly in assemblydict.Values)
                        {
                            if (assembly != null)
                                assemblies.Add(assembly);
                        }
                        _referencedAssembliesDirectWithoutGac = assemblies.ToArray();
                    }
                }
            }
            return _referencedAssembliesDirectWithoutGac;
        }



        /// <summary>Returns an array containing the executable assembly and all its referenced assemblies (directly or indirectly).
        /// Assemblies from the Global Assembly Cache (GAC) are NOT included.</summary>
        /// <remarks>Array of assemblies is calculated only once (the first time it is needed) and is stored internally for 
        /// to speed up further uses.
        /// <para>See:</para>
        /// <para>http://stackoverflow.com/questions/383686/how-do-you-loop-through-currently-loaded-assemblies </para>
        /// <para>Assembly names, http://msdn.microsoft.com/en-us/library/k8xx4k69%28v=vs.110%29.aspx </para></remarks>
        public static Assembly[] GetReferencedAssembliesRecursiveWithoutGac()
        {
            if (_referencedAssembliesRecursiveWithoutGac == null)
            {
                lock (LockStatic)
                {
                    if (_referencedAssembliesRecursiveWithoutGac == null)
                    {
                        Dictionary<string, Assembly> assemblydict = GetReferencedAssemblies(ExecutableAssembly,
                            ignoreGac: true, recursive: true);
                        List<Assembly> assemblies = new List<Assembly>();
                        assemblies.Add(ExecutableAssembly);
                        foreach (Assembly assembly in assemblydict.Values)
                        {
                            if (assembly != null)
                                assemblies.Add(assembly);
                        }
                        _referencedAssembliesRecursiveWithoutGac = assemblies.ToArray();
                    }
                }
            }
            return _referencedAssembliesRecursiveWithoutGac;
        }

        /// <summary>Adds the executable assembly and all its referenced assemblies (directly or indirectly) without those 
        /// from GAC to the specified list, and returns the list.</summary>
        /// <param name="assemblyList">List on which referenced assemblies are loaded. If null then the list is created internally
        /// (inboth cases the list is returned).</param>
        /// <param name="clearBefore">Spwcifies whether the list should be cleared fitst in order to remove any existent items on the list.</param>
        /// <returns>List onto which the loaded assemblies were added. This makes the method usable even if the <paramref name="assemblyList"/>
        /// is null, in which case the list is allocated within the method.</returns>
        public static IList<Assembly> GetReferencedAssembliesWithoutGac(IList<Assembly> assemblyList, bool clearBefore = true)
        {
            if (assemblyList == null)
                assemblyList = new List<Assembly>();
            else if (clearBefore)
                assemblyList.Clear();
            Assembly[] loadedAssemblies = GetReferencedAssembliesWithoutGac();
            foreach (Assembly asm in loadedAssemblies)
                assemblyList.Add(asm);
            return assemblyList;
        }


        /// <summary>Returns an array containing the executable assembly and all its DIRECTLY referenced assemblies.
        /// Assemblies from the Global Assembly Cache (GAC) are also included.</summary>
        /// <remarks>Array of assemblies is calculated only once (the first time it is needed) and is stored internally 
        /// to speed up further uses.
        /// <para>See:</para>
        /// <para>http://stackoverflow.com/questions/383686/how-do-you-loop-through-currently-loaded-assemblies </para>
        /// <para>Assembly names, http://msdn.microsoft.com/en-us/library/k8xx4k69%28v=vs.110%29.aspx </para></remarks>
        public static Assembly[] GetReferencedAssemblies()
        {
            if (_referencedAssembliesDirect == null)
            {
                lock (LockStatic)
                {
                    if (_referencedAssembliesDirect == null)
                    {
                        Dictionary<string, Assembly> assemblydict = GetReferencedAssemblies(ExecutableAssembly,
                            ignoreGac: false, recursive: false);
                        List<Assembly> assemblies = new List<Assembly>();
                        assemblies.Add(ExecutableAssembly);
                        foreach (Assembly assembly in assemblydict.Values)
                        {
                            if (assembly != null)
                                assemblies.Add(assembly);
                        }
                        _referencedAssembliesDirect = assemblies.ToArray();
                    }
                }
            }
            return _referencedAssembliesDirect;
        }

        /// <summary>Adds the executable assembly and all its DIRECTLY referenced assemblies without those from GAC to the specified list, and returns the list.</summary>
        /// <param name="assemblyList">List on which referenced assemblies are loaded. If null then the list is created internally
        /// (inboth cases the list is returned).</param>
        /// <param name="clearBefore">Spwcifies whether the list should be cleared fitst in order to remove any existent items on the list.</param>
        /// <returns>List onto which the loaded assemblies were added. This makes the method usable even if the <paramref name="assemblyList"/>
        /// is null, in which case the list is allocated within the method.</returns>
        public static IList<Assembly> GetReferencedAssemblies(IList<Assembly> assemblyList, bool clearBefore = true)
        {
            if (assemblyList == null)
                assemblyList = new List<Assembly>();
            else if (clearBefore)
                assemblyList.Clear();
            Assembly[] loadedAssemblies = GetReferencedAssemblies();
            foreach (Assembly asm in loadedAssemblies)
                assemblyList.Add(asm);
            return assemblyList;
        }


        /// <summary>Returns an array containing the executable assembly and all its referenced assemblies (directly or indirectly), 
        /// which roughly coincides with all assemblies that can be potentially used by the current application.
        /// Assemblies from the Global Assembly Cache (GAC) are also included.</summary>
        /// <remarks>Array of assemblies is calculated only once (the first time it is needed) and is stored internally 
        /// to speed up further uses.
        /// <para>See:</para>
        /// <para> http://stackoverflow.com/questions/383686/how-do-you-loop-through-currently-loaded-assemblies </para>
        /// <para>Assembly names, http://msdn.microsoft.com/en-us/library/k8xx4k69%28v=vs.110%29.aspx </para></remarks>
        public static Assembly[] GetReferencedAssembliesRecursive()
        {
            if (_referencedAssembliesRecursive == null)
            {
                lock (LockStatic)
                {
                    if (_referencedAssembliesRecursive == null)
                    {
                        Dictionary<string, Assembly> assemblydict = GetReferencedAssemblies(ExecutableAssembly,
                            ignoreGac: false, recursive: true);
                        List<Assembly> assemblies = new List<Assembly>();
                        assemblies.Add(ExecutableAssembly);
                        foreach (Assembly assembly in assemblydict.Values)
                        {
                            if (assembly != null)
                                assemblies.Add(assembly);
                        }
                        _referencedAssembliesRecursive = assemblies.ToArray();
                    }
                }
            }
            return _referencedAssembliesRecursive;
        }

        /// <summary>Adds the executable assembly and all its referenced (directly or indirectly) assemblies to the specified list, 
        /// and returns the list. This roughly coincides with all assemblies that can be potentially used by the current application.</summary>
        /// <param name="assemblyList">List on which referenced assemblies are loaded. If null then the list is created internally
        /// (inboth cases the list is returned).</param>
        /// <param name="clearBefore">Spwcifies whether the list should be cleared fitst in order to remove any existent items on the list.</param>
        /// <returns>List onto which the loaded assemblies were added. This makes the method usable even if the <paramref name="assemblyList"/>
        /// is null, in which case the list is allocated within the method.</returns>
        public static IList<Assembly> GetReferencedAssembliesRecursive(IList<Assembly> assemblyList, bool clearBefore = true)
        {
            if (assemblyList == null)
                assemblyList = new List<Assembly>();
            else if (clearBefore)
                assemblyList.Clear();
            Assembly[] loadedAssemblies = GetReferencedAssembliesRecursive();
            foreach (Assembly asm in loadedAssemblies)
                assemblyList.Add(asm);
            return assemblyList;
        }




        public class MissingAssembly
        {
            public MissingAssembly(string missingAssemblyName, string missingAssemblyNameParent)
            {
                MissingAssemblyName = missingAssemblyName;
                MissingAssemblyNameParent = missingAssemblyNameParent;
            }
            public string MissingAssemblyName { get; set; }
            public string MissingAssemblyNameParent { get; set; }
        }

        private static Dictionary<string, Assembly> _dependentAssemblyList;
        private static List<MissingAssembly> _missingAssemblyList;


        /// <summary>Get assemblies referenced by the specified assembly. 
        /// Not recursive.</summary>
        /// <param name="assembly">Assembly whose referenced assemblies are obtained.</param>
        public static List<string> GetReferencedAssembliesFlat(Assembly assembly)
        {
            lock (LockStatic)
            {
                var results = assembly.GetReferencedAssemblies();
                return results.Select(o => o.FullName).OrderBy(o => o).ToList();
            }
        }

        /// <summary>Creates and returns a dictionary containing all assemblies referenced (directly or indirectly)
        /// by the specified assembly. Recursive.</summary>
        /// <param name="assembly">Assembly whose referenced assemblies are obtained recursively.</param>
        /// <param name="ignoreGac">Whether to ignore assemblies in GAC.</param>
        /// <param name="recursive">Whetherr to get a listt of assemblies recursively.</param>
        public static Dictionary<string, Assembly> GetReferencedAssemblies(Assembly assembly, 
            bool ignoreGac = true, bool recursive = true)
        {
            lock (LockStatic)
            {
                _dependentAssemblyList = new Dictionary<string, Assembly>();
                _missingAssemblyList = new List<MissingAssembly>();

                InternalGetReferencedAssembliesRecursive(assembly, recursive);

                if (ignoreGac)
                {
                    // Only include assemblies that we wrote ourselves (ignore ones from GAC).
                    var keysToRemove = _dependentAssemblyList.Values.Where(
                        o => o.GlobalAssemblyCache == true).ToList();
                    foreach (var k in keysToRemove)
                    {
                        _dependentAssemblyList.Remove(SimpleAssemblyName(k.FullName));
                    }
                }

                return _dependentAssemblyList;
            }
        }

        /// <summary>Get missing assemblies - those referenced assemblies that could not be loaded.</summary>
        public static List<MissingAssembly> GetMissingAssemblies(Assembly assembly, bool recursive = true)
        {
            lock (LockStatic)
            {
                _dependentAssemblyList = new Dictionary<string, Assembly>();
                _missingAssemblyList = new List<MissingAssembly>();
                InternalGetReferencedAssembliesRecursive(assembly, recursive);
                return _missingAssemblyList;
            }
        }

        /// <summary>Internal recursive method to get all referenced assemblies, and all dependent assemblies of dependent 
        /// assemblies, etc.</summary>
        private static void InternalGetReferencedAssembliesRecursive(Assembly assembly, bool recursive = true)
        {
            lock (LockStatic)
            {
                // Load assemblies with newest versions first. Omitting the ordering results in false positives on
                // _missingAssemblyList.
                var referencedAssemblies = assembly.GetReferencedAssemblies()
                    .OrderByDescending(o => o.Version);

                foreach (var r in referencedAssemblies)
                {
                    if (String.IsNullOrEmpty(assembly.FullName))
                    {
                        continue;
                    }

                    if (!_dependentAssemblyList.ContainsKey(SimpleAssemblyName(r.FullName)))
                    {
                        try
                        {
                            Assembly a = Assembly.ReflectionOnlyLoad(r.FullName); // loads the assemblies in a separate AppDomain, not interfering with the JIT process
                            _dependentAssemblyList[SimpleAssemblyName(a.FullName)] = a;
                            if (recursive)
                            {
                                InternalGetReferencedAssembliesRecursive(a, true);
                            }
                        }
                        catch (Exception)
                        {
                            _missingAssemblyList.Add(new MissingAssembly(r.FullName.Split(',')[0], SimpleAssemblyName(assembly.FullName)));
                        }
                    }
                }
            }
        }

        /// <summary>Returns a simple assembly name that corresponds to the specified full
        /// name of the assembly.
        /// <para>Simple name is only the assemblyname, not including assembly version, pulbic key token,
        /// or culture.</para></summary>
        /// <param name="fullName"></param>
        /// <returns></returns>
        public static string SimpleAssemblyName(string fullName)
        {
            return fullName.Split(',')[0];
        }


        #endregion Assembly.ReferencedAssemblies


        #region Assembly.Load

        private static bool _assemblyResolveHandlerAdded = false;

        /// <summary>Whether this class' event handler for resolving assembly loading is registered or not.</summary>
        /// <returns></returns>
        public static bool IsRegisteredAssemblyResolveHandler
        { get { lock (LockStatic) { return _assemblyResolveHandlerAdded; } } }

        /// <summary>Registers tis class' event handler for resolving assembly load failures.</summary>
        public static void RegisterAssemblyResolveHandler()
        {
            lock(LockStatic)
            {
                if (!_assemblyResolveHandlerAdded)
                {
                    AppDomain currentDomain = AppDomain.CurrentDomain;
                    currentDomain.AssemblyResolve += AssemblyResolveEventHandler; // new ResolveEventHandler(AssemblyResolveEventHandler);
                    _assemblyResolveHandlerAdded = true;
                }
            }
        }

        /// <summary>Unregisters tis class' event handler for resolving assembly load failures.</summary>
        public static void UnregisterAssemblyResolveHandler()
        {
            lock (LockStatic)
            {
                if (!_assemblyResolveHandlerAdded)
                {
                    _assemblyResolveHandlerAdded = false;
                    try
                    {
                        AppDomain currentDomain = AppDomain.CurrentDomain;
                        currentDomain.AssemblyResolve -= AssemblyResolveEventHandler;
                    }
                    catch { }
                }
            }
        }

        /// <summary>Provides identity for assembly resolution attempts.
        /// <para>Using objects of this class, the szstem can establish while the same kind of assembly resolution has
        /// already occurred in the past, and thus skip the attempt.</para></summary>
        public class ResolutionIdentity: IEquatable<ResolutionIdentity>
        {
            private ResolutionIdentity() { }

            public ResolutionIdentity(string assembly, string requestingAssembly) 
            { AssemblyName = assembly; RequestingAssemblyName = requestingAssembly; }

            public string AssemblyName { get; set; }

            public string RequestingAssemblyName { get; set; }

            public bool Equals(ResolutionIdentity id)
            {
                if (id == null)
                    return false;
                else
                    return AssemblyName == id.AssemblyName && RequestingAssemblyName == id.RequestingAssemblyName;
            }
        }

        private static List<ResolutionIdentity> _failedResolves = new List<ResolutionIdentity>();


        private static bool ResolveAlreadyFailed(ResolutionIdentity resId)
        { lock(LockStatic) { return _failedResolves.Contains(resId); } }

        private static void AddResolveFailed(ResolutionIdentity resId)
        { lock (LockStatic) { if (!ResolveAlreadyFailed(resId)) _failedResolves.Add(resId); } }

        private static void ClearResolveFailed(ResolutionIdentity resId)
        { lock (LockStatic) { _failedResolves.Clear(); } }

        private static List<string> _assemblyResolutionPaths = new List<string>();

        /// <summary>Adds a new directory where assemblies to be loaded are looked for.</summary>
        /// <param name="path">Path of the directory.</param>
        public static void AddAssemblyResolutionPath(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("Assembly search path to be added is not specified (null or empty string).");
            lock (LockStatic)
            {
                if (!_assemblyResolutionPaths.Contains(path))
                    _assemblyResolutionPaths.Add(path);
            }
        }

        /// <summary>Removes the specified directory where assemblies to be loaded are looked for.
        /// <para>Path string must be exactly the same as when the path was added to the list of locations.</para></summary>
        /// <param name="path">Path of the directory.</param>
        public static void RemoveAssemblyResolutionPath(string path)
        {
            lock (LockStatic)
            {
                if (_assemblyResolutionPaths.Contains(path))
                    _assemblyResolutionPaths.Remove(path);
            }
        }

        public static string[] AssemblyResolutionPaths
        { get { lock (LockStatic) { return _assemblyResolutionPaths.ToArray(); } } }


        /// <summary>Event handler that tries to resolve assembly load.</summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private static Assembly AssemblyResolveEventHandler(object sender, ResolveEventArgs args)
        {
            
            Assembly ret = null;
            string fullName = args.Name;
            AssemblyName name = new AssemblyName();
            Console.WriteLine(Environment.NewLine + Environment.NewLine + "IGLib Assembly Resolve handler called.... " + Environment.NewLine
                + "  assembly: " + fullName);
            Console.WriteLine("  Assembly name: " + name.Name + Environment.NewLine
                + "    version: " + name.Version + Environment.NewLine
                + "    codebase: "  + name.CodeBase);
            Assembly requestingAssmbly = args.RequestingAssembly;
            string requestingAssmblyFullName = null;
            if (requestingAssmbly != null)
                requestingAssmblyFullName = requestingAssmbly.FullName;
            if (requestingAssmblyFullName == null)
                Console.WriteLine("Requesting assembly is not known.");
            else
                Console.WriteLine("RequestingAssmbly: " + requestingAssmblyFullName);
            ResolutionIdentity resId = new ResolutionIdentity(fullName, requestingAssmblyFullName);
            if (ResolveAlreadyFailed(resId))
            {
                Console.WriteLine(Environment.NewLine + "Resolution skipped." + Environment.NewLine + 
                  "Resolution has already failed for the following combination: " + Environment.NewLine +
                  "  loading assembly: " + fullName + Environment.NewLine + 
                  "  requesting assembly: " + requestingAssmblyFullName + Environment.NewLine);
            }

            try
            {
                string assemblyFilename = name.Name + ".dll";
                if (ret == null && requestingAssmbly != null)
                {
                    // Try to find the assembly in the same direectory where the requesting assembly is located:
                    string reqLocation = requestingAssmbly.Location;
                    if (!string.IsNullOrEmpty(reqLocation))
                    {
                        string path = Path.GetDirectoryName(reqLocation);
                        if (!string.IsNullOrEmpty(reqLocation))
                        {
                            string filePath = Path.Combine(path, assemblyFilename);
                            Console.WriteLine("Trying to load assembly from the directory of requesting assembly..." + Environment.NewLine
                                + "  Tested path: " + filePath);
                            if (File.Exists(filePath))
                            {
                                try
                                {
                                    ret = Assembly.LoadFrom(filePath);
                                    if (ret != null)
                                        Console.WriteLine("  ... loading successful.");
                                    else
                                        Console.WriteLine("  ... loading unsuccessful.");
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(Environment.NewLine + "Exception was thrown when trying to resolve assembly load. " + Environment.NewLine
                                        + "  Message: " + ex.Message + Environment.NewLine
                                        + "  Loar path: " + filePath + ". ");
                                }
                            }
                        }
                    }
                }
                if (ret == null)
                {
                    // Try to find and load the assembly from one of the user specified search paths:
                    string[] paths = AssemblyResolutionPaths;
                    foreach (string path in paths)
                    {
                        if (ret == null)
                        {
                            string filePath = Path.Combine(path, assemblyFilename);
                            Console.WriteLine("Trying to load assembly from the user specified directory..." + Environment.NewLine
                                + "  Tested path: " + filePath);
                            if (File.Exists(filePath))
                            {
                                try
                                {
                                    ret = Assembly.LoadFrom(filePath);
                                    if (ret != null)
                                        Console.WriteLine("  ... loading successful.");
                                    else
                                        Console.WriteLine("  ... loading unsuccessful.");
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(Environment.NewLine + "Exception was thrown when trying to resolve assembly load. " + Environment.NewLine
                                        + "  Message: " + ex.Message + Environment.NewLine
                                        + "  Loar path: " + filePath + ". ");
                                }
                            }
                        }
                    }
                }

            }
            catch(Exception ex)
            {
                Console.WriteLine(Environment.NewLine + "Exception was thrown when trying to resolve assembly load. " + Environment.NewLine
                    + "  Message: " + ex.Message);
            }

            if (ret == null)
            {
                Console.WriteLine("Assembly resolution FAILED." + Environment.NewLine);
                AddResolveFailed(resId);
            } else
            {
                Console.WriteLine("Assembly resolution successful." + Environment.NewLine);
            }
            return ret;
        }



        /// <summary>Loads the specified assembly.</summary>
         /// <param name="fullName">Name of the assembly to be loaded.</param>
        /// <param name="outputLevel">The level of output. If greater than 0 then some information 
        /// abot loading the assembly and assembly itself is printed.</param>
        /// <param name="reflectionOnly">If true then assembly is loaded for inspection only, i.e. 
        /// out of any applicatin domain, and can not be executed.</param>
        /// <returns>Loaded assembly, or null if the assembly could not be loaded.</returns>
        public static Assembly LoadAssembly(string fullName, int outputLevel = 2, bool reflectionOnly = false)
        {
            Assembly ret = null;
            if (outputLevel >= 1)
            {
                if (reflectionOnly)
                    Console.WriteLine(Environment.NewLine + "Loading assembly (for reflection only): " + fullName + " ...");
                else
                    Console.WriteLine(Environment.NewLine + "Loading assembly " + fullName + " ...");
            }
            if (reflectionOnly)
            {
                // Load assembly for reflection only (out of any app domain, so it can not be executed):
                if (ret == null)
                {
                    try
                    {
                        ret = Assembly.Load(fullName);
                        if (ret == null)
                            throw new InvalidOperationException("Could not load the following assembly: " + fullName);
                        if (outputLevel >= 2)
                            Console.WriteLine("Loaded by Assembly.Load(fullName).");
                    }
                    catch (Exception ex)
                    {
                        if (outputLevel >= 2)
                            Console.WriteLine("ERROR in Assembly.Load(fullName): " + Environment.NewLine + "  " + ex.Message);
                    }
                }
            }
            else
            {
                // Load assembly into current AppDomain, so that it can be executed:
                if (ret == null)
                {
                    try
                    {
                        ret = Assembly.Load(fullName);
                        if (ret == null)
                            throw new InvalidOperationException("Could not load the following assembly: " + fullName);
                        if (outputLevel >= 2)
                            Console.WriteLine("Loaded by Assembly.Load(fullName).");
                    }
                    catch (Exception ex)
                    {
                        if (outputLevel >= 2)
                            Console.WriteLine("ERROR in Assembly.Load(fullName): " + Environment.NewLine + "  " + ex.Message);
                    }
                }

                if (ret == null)
                {
                    try
                    {
                        ret = Assembly.LoadFrom(fullName);
                        if (ret == null)
                            throw new InvalidOperationException("Could not load the following assembly: " + fullName);
                        if (outputLevel >= 2)
                            Console.WriteLine("Loaded by Assembly.LoadFrom(fullName).");
                    }
                    catch (Exception ex)
                    {
                        if (outputLevel >= 2)
                            Console.WriteLine("ERROR in Assembly.LoadFrom(fullName): " + Environment.NewLine + "  " + ex.Message);
                    }
                }

                if (ret == null)
                {
                    try
                    {
                        ret = Assembly.LoadFile(fullName);
                        if (ret == null)
                            throw new InvalidOperationException("Could not load the following assembly: " + fullName);
                        if (outputLevel >= 2)
                            Console.WriteLine("Loaded by Assembly.LoadFile(fullName).");
                    }
                    catch (Exception ex)
                    {
                        if (outputLevel >= 2)
                            Console.WriteLine("ERROR in Assembly.LoadFile(fullName): " + Environment.NewLine + "  " + ex.Message);
                    }
                }

            }
            if (outputLevel >= 1)
            {
                if (ret != null)
                {
                    Console.WriteLine("  ... done. " + Environment.NewLine);
                    if (outputLevel >= 2)
                    {
                        int infoLevel = outputLevel;
                        int versionLevel = outputLevel + 1;
                        Console.WriteLine(Environment.NewLine + "Loaded assembly: " + Environment.NewLine 
                            + GetAssemblyInfo(ret, infoLevel, versionLevel)
                            + Environment.NewLine);
                    }
                }
                else
                    Console.WriteLine("  ... loading FALED for " + fullName);
            }
            return ret;
        }



        /// <summary>Loads all the assemblies specified by arguments, and returns the array of loaded assemblies.</summary>
        /// <param name="fullNames">Names of the assemblies to be loaded.</param>
        /// <param name="outputLevel">The level of output. If greater than 0 then some information 
        /// abot loading the assembly and assembly itself is printed.</param>
        /// <param name="reflectionOnly">If true then assembly is loaded for inspection only, i.e. 
        /// out of any applicatin domain, and can not be executed.</param>
        /// <returns>Array of loaded assemblies.</returns>
        public static Assembly[] LoadAssemblies(string[] fullNames, int outputLevel = 2, bool reflectionOnly = false)
        {
            List<Assembly> assemblies = new List<Assembly>();
            foreach (string assemblyName in fullNames)
            {
                assemblies.Add(LoadAssembly(assemblyName));
            }
            return assemblies.ToArray();
        }


        #endregion Assembly.Load


        #region Assemblies.General

        /// <summary>Returns the simple name of the specified assembly.
        /// <para>This method is provided, among the others, in order to standardise how assemblies are referred to
        /// within the IGLib library when version, culture and other specifics do not need to be differentiated and 
        /// it only matters that logically the same assemblies are identified by some a common identifier.</para></summary>
        /// <param name="assembly">Assembly whose name is returned.</param>
        public static string GetAssemblyName(Assembly assembly)
        {
            return assembly.GetName().Name;
        }

        /// <summary>Returns file name of the specified assembly.</summary>
        /// <param name="assembly">Assembly whose file name is returned.</param>
        public static string GetAssemblyFileName(Assembly assembly)
        {
            //string path = assembly.CodeBase;
            string path = assembly.Location;
            return Path.GetFileName(path);
        }

        /// <summary>Returns the directory containing the specified assembly.</summary>
        /// <param name="assembly">Assembly whose directory is returned.</param>
        public static string GetAssemblyDirectory(Assembly assembly)
        {
            //string path = assembly.CodeBase;
            string path = assembly.Location;
            return Path.GetDirectoryName(path);
        }

        /// <summary>Returns assembly name of the specified assembly.</summary>
        /// <param name="assembly">Assembly whose name is returned.</param>
        public static string GetAssemblyAssemblyName(Assembly assembly)
        {
            return assembly.GetName().Name;
        }

        /// <summary>Returns assembly name of the specified assembly.</summary>
        /// <param name="assembly">Assembly whose name is returned.</param>
        public static string GetAssemblyAssemblyFullName(Assembly assembly)
        {
            return assembly.GetName().FullName;
        }

        /// <summary>Returns version (from the file info) of the specified assembly.</summary>
        /// <param name="numLevels">Nmber of levels included in the returned version string.</param>
        /// <param name="assembly">Assembly whose version is returned.</param>
        public static string GetAssemblyVersion(Assembly assembly, int numLevels = 2)
        {
            if (numLevels >= 4)
                return assembly.GetName().Version.ToString();
            else
            {
                string ret = assembly.GetName().Version.Major.ToString();
                if (numLevels >= 2)
                {
                    ret += "." + assembly.GetName().Version.Minor.ToString();
                }
                if (numLevels >= 3)
                {
                    ret += "." + assembly.GetName().Version.MajorRevision.ToString();
                }
                return ret;
            }
        }

        /// <summary>Returns descriptive title of the specified assembly (from the AssemblyInfo file).</summary>
        /// <param name="assembly">Assembly whose title is returned.</param>
        public static string GetAssemblyTitle(Assembly assembly)
        {
            FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            return versionInfo.FileDescription;
        }

        /// <summary>Returns description of the specified assembly (from assembly info).</summary>
        /// <param name="assembly">Assembly whose description is returned.</param>
        public static string GetAssemblyDescription(Assembly assembly)
        {
            FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            return versionInfo.Comments;
        }

        /// <summary>Returns company attribute of the specified assembly.</summary>
        /// <param name="assembly">Assembly whose company is returned.</param>
        public static string GetAssemblyCompany(Assembly assembly)
        {
            FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            return versionInfo.CompanyName;
        }

        /// <summary>Returns copyright information of the specified assembly.</summary>
        /// <param name="assembly">Assembly whose copyright information is returned.</param>
        public static string GetAssemblyCopyrightInfo(Assembly assembly)
        {
            FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            return versionInfo.LegalCopyright;
        }

        
        /// <summary>Returns a (possibly multiline) string containing basic information about the specified assembly, 
        /// such as file name, directory, assembly name, and version.</summary>
        /// <param name="infoLevel">Level of information put into the string:
        /// <para>  1: assembly name and version, executable file.</para>
        /// <para>  2: executable directory, title, description.</para>
        /// <para>  3: creator, copyright info.</para></param>
        /// <param name="versionLevel">Level version nformation included. By default (value 0), one level more than <paramref name="infoLevel"/>.</param>
        /// <param name="assembly">Assembly whose information in readable form is returned.</param>
        public static string GetAssemblyInfo(Assembly assembly, int infoLevel = 3, int versionLevel = 0)
        {
            if (versionLevel <= 0)
                versionLevel = infoLevel + 1;
            StringBuilder sb = new StringBuilder();
            if (infoLevel >= 1)
            {
                sb.AppendLine("Assembly: " + assembly.GetName().Name + ", version " + GetAssemblyVersion(assembly, versionLevel));
                sb.AppendLine("Full name: " + assembly.GetName().FullName);
                if (assembly == ExecutableAssembly)
                    sb.AppendLine("Executable name: " + GetAssemblyFileName(assembly));
                else
                    sb.AppendLine("File name: " + GetAssemblyFileName(assembly));
            }
            if (infoLevel >= 2)
            {
                if (assembly == ExecutableAssembly)
                    sb.AppendLine("Executable directory: " + GetAssemblyDirectory(assembly));
                else
                    sb.AppendLine("Directory: " + GetAssemblyDirectory(assembly));
                string title = GetAssemblyTitle(assembly);
                string description = GetAssemblyDescription(assembly);
                if (!string.IsNullOrEmpty(title) || !string.IsNullOrEmpty(description))
                {
                    sb.Append("Description: ");
                    if (!string.IsNullOrEmpty(title))
                    {
                        sb.Append(title);
                    }
                    if (!string.IsNullOrEmpty(description))
                    {
                        if (!string.IsNullOrEmpty(title))
                        {
                            sb.Append(" - ");
                            sb.Append(description);
                        }
                    }
                    sb.AppendLine();
                }
            }
            if (infoLevel >= 3)
            {
                string companyName = GetAssemblyCompany(assembly);
                if (!string.IsNullOrEmpty(companyName))
                {
                    sb.AppendLine("Produced by: " + companyName);
                }
                string copyrightInfo = GetAssemblyCopyrightInfo(assembly);
                if (!string.IsNullOrEmpty(copyrightInfo))
                {
                    // sb.AppendLine("Copyright information: ");
                    sb.AppendLine(copyrightInfo);
                }
            }
            return sb.ToString();
        }


        #endregion Assemblies.General


        #region Assemblies.Executable

        /// <summary>Returns file name of the current executable.</summary>
        public static string GetExecutableFileName()
        {
            string executableFilePath = Process.GetCurrentProcess().MainModule.FileName;
            return Path.GetFileName(executableFilePath);
            //return Assembly.GetExecutingAssembly().GetName().
        }

        /// <summary>Returns the directory containing the executable that started the current
        /// application.</summary>
        public static string GetExecutableDirectory()
        {
            // return Application.StartupPath;
            // string executableFilePath = Assembly.GetEntryAssembly().Location;
            string executableFilePath = Process.GetCurrentProcess().MainModule.FileName;
            return Path.GetDirectoryName(executableFilePath);
        }

        /// <summary>Returns assembly name of the current executable.</summary>
        public static string GetExecutableAssemblyName()
        {
            return GetAssemblyName(ExecutableAssembly);
        }

        /// <summary>Returns version (from the file info) of the current executable.</summary>
        /// <param name="numLevels">Nmber of levels included in the returned version string.</param>
        public static string GetExecutableVersion(int numLevels = 2)
        {
            return GetAssemblyVersion(ExecutableAssembly, numLevels);
        }

        /// <summary>Returns descriptive title of the current executable (from the AssemblyInfo file).</summary>
        public static string GetExecutableTitle()
        {
            return GetAssemblyTitle(ExecutableAssembly);
        }

        /// <summary>Returns description of the current executable (from the AssemblyInfo file).</summary>
        public static string GetExecutableDescription()
        {
            return GetAssemblyDescription(ExecutableAssembly);
        }

        /// <summary>Returns company attribute of the currentt executable.</summary>
        public static string GetExecutableCompany()
        {
            return GetAssemblyCompany(ExecutableAssembly);
        }

        /// <summary>Returns copyright information of the current executable.</summary>
        public static string GetExecutableCopyrightInfo()
        {
            return GetAssemblyCopyrightInfo(ExecutableAssembly);
        }

        /// <summary>Returns a (possibly multiline) string containing basic information about the current executable, 
        /// such as executable file name and directory.</summary>
        /// <param name="infoLevel">Level of information put into the string:
        /// <para>  1: assembly name and version, executable file.</para>
        /// <para>  2: executable directory, title, description.</para>
        /// <para>  3: creator, copyright info.</para></param>
        /// <param name="versionLevel">Level version nformation included. By default (vlue 0), one level more than <paramref name="infoLevel"/>.</param>
        public static string GetExecutableInfo(int infoLevel = 3, int versionLevel = 0)
        {
            return GetAssemblyInfo(ExecutableAssembly, infoLevel, versionLevel);
        }

        #endregion Assemblies.Executable


        #region Assemblies.IGLib

        /// <summary>Returns file name of the IGLib assembly.</summary>
        public static string GetIglibFileName()
        {
            return GetAssemblyFileName(IglibAssembly);
        }

        /// <summary>Returns the directory containing the IGLib assembly.</summary>
        public static string GetIglibDirectory()
        {
            return GetAssemblyDirectory(IglibAssembly);
        }

        /// <summary>Returns assembly name of the IGLib assembly.</summary>
        public static string GetIglibAssemblyName()
        {
            return GetAssemblyName(IglibAssembly);
        }

        /// <summary>Returns version (from the file info) of the IGLib assembly.</summary>
        /// <param name="numLevels">Nmber of levels included in the returned version string.</param>
        public static string GetIglibVersion(int numLevels = 2)
        {
            return GetAssemblyVersion(IglibAssembly, numLevels);
        }

        /// <summary>Returns descriptive title of the IGLib assembly (from the AssemblyInfo file).</summary>
        public static string GetIglibTitle()
        {
            return GetAssemblyTitle(IglibAssembly);
        }

        /// <summary>Returns description of the IGLib assembly (from assembly info).</summary>
        public static string GetIglibDescription()
        {
            return GetAssemblyDescription(IglibAssembly);
        }

        /// <summary>Returns company attribute of the IGLib assembly.</summary>
        public static string GetIglibCompany()
        {
            return GetAssemblyCompany(IglibAssembly);
        }

        /// <summary>Returns copyright information of the IGLib assembly.</summary>
        public static string GetIglibCopyrightInfo()
        {
            return GetAssemblyCopyrightInfo(IglibAssembly);
        }

        /// <summary>Returns a (possibly multiline) string containing basic information about the IGLib base library, 
        /// such as file name, directory, assembly name, and version.</summary>
        /// <param name="infoLevel">Level of information put into the string:
        /// <para>  1: assembly name and version, executable file.</para>
        /// <para>  2: executable directory, title, description.</para>
        /// <para>  3: creator, copyright info.</para></param>
        /// <param name="versionLevel">Level version nformation included. By default (value 0), one level more than <paramref name="infoLevel"/>.</param>
        public static string GetIglibInfo(int infoLevel = 3, int versionLevel = 0)
        {
            return GetAssemblyInfo(IglibAssembly, infoLevel, versionLevel);
        }


        #endregion Assemblies.IGLib


        /// <summary>Returns a (possibly multiline) string containing basic information about the current application, 
        /// such as file name, directory, assembly name, and version. Information about IGLib can be included, too.</summary>
        /// <param name="infoLevel">Level of information put into the string:
        /// <para>  1: assembly name and version, executable file.</para>
        /// <para>  2: executable directory.</para></param>
        /// <param name="includeIglibInfo">Whether info about IGLib is included. If false then only application info is included.</param>
        /// <param name="versionLevel">Level version nformation included. 2 by default, 0: one level more than <paramref name="infoLevel"/>.</param>
        /// <param name="additionalAssemblies">Additional assemblies that should be described in the returned info string.</param>
        public static string GetApplicationInfo(int infoLevel = 3, bool includeIglibInfo = true, int versionLevel = 2, IList<Assembly> additionalAssemblies = null)
        {

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Application information:");
            sb.AppendLine(GetExecutableInfo(infoLevel, versionLevel));
            if (includeIglibInfo)
            {
                sb.AppendLine("Base library (IGLib):");
                sb.AppendLine(GetIglibInfo(infoLevel, versionLevel));
            }
            if (infoLevel >= 4)
            {
                Assembly[] loadedAssemblies = GetLoadedAssemblies();
                sb.AppendLine("Currently loaded assemblies: ");
                foreach (Assembly assembly in loadedAssemblies)
                {
                    sb.AppendLine("  " + assembly.GetName().FullName);
                }
                sb.AppendLine();
                if (infoLevel >= 5)
                {
                    Assembly[] referencedAssemblies = GetReferencedAssemblies();
                    sb.AppendLine("Referenced assemblies: ");
                    foreach (Assembly assembly in referencedAssemblies)
                    {
                        if (assembly != null)
                            sb.AppendLine("  " + assembly.GetName().FullName);
                    }
                    sb.AppendLine();
                    if (infoLevel >= 6)
                    {
                        referencedAssemblies = GetReferencedAssembliesRecursive();
                        sb.AppendLine("All referenced assemblies (recursive): ");
                        foreach (Assembly assembly in referencedAssemblies)
                        {
                            if (assembly != null)
                                sb.AppendLine("  " + assembly.GetName().FullName);
                        }
                        sb.AppendLine();
                    }
                }
            }
            if (additionalAssemblies != null)
            {
                if (additionalAssemblies.Count > 0)
                {
                    sb.AppendLine();
                    sb.AppendLine("Other important assemblies: ");
                    sb.AppendLine();
                    foreach (Assembly assembly in additionalAssemblies)
                    {
                        if (assembly != null)
                            sb.AppendLine(GetAssemblyInfo(assembly, infoLevel, versionLevel));
                    }
                }
            }
            return sb.ToString();
        }


        #endregion Assemblies


        #region AssemblyTypes

        /// <summary>Finds a type with the specified class name and returns it.
        /// <para>The type is searched for in the executable assembly, the current assembly, all loaded assemblies and in the 
        /// referenced assemblies of the executable assembly.</para>
        /// <para>If the type can not be found then 0 is returned (no exceptions thrown).</para></summary>
        /// <param name="className">Name of the type to be obtained. It must include namespace unless the namespace is specified
        /// separately by the <paramref name="nameSpace"/> parameter.</param>
        /// <param name="nameSpace">If provided then this parameter is prepended to the class name as (. separated) namespace.
        /// <para>If thee <paramref>tryWithoutNamespace</paramref> parameter is true then type name is also tried without the
        /// namespace, but this is done after the name prepended with a namespace is tried (when namespace is specified).</para></param>
        /// <param name="ignoreCase">If true then type name is case insensitive; default is false (case sensitive names).</param>
        /// <param name="tryWithoutNamespace">If true then search for a type without namespace is attempted, too, even if the 
        /// namespace is specified.</param>
        public static Type GetType(string className, string nameSpace = null,
            bool ignoreCase = true, bool tryWithoutNamespace = true)
        {
            if (string.IsNullOrEmpty(className))
                throw new ArgumentException("Class name is not specified (null or empty string).");
            Type ret = null;
            if (!string.IsNullOrEmpty(nameSpace))
                ret = GetType(nameSpace + "." + className, ignoreCase);
            if (ret == null && (tryWithoutNamespace || !string.IsNullOrEmpty(nameSpace)))
                ret = GetType(className, ignoreCase);
            return ret;
        }


        /// <summary>Finds a type with the specified class name and returns it.
        /// <para>The type is searched for in the executable assembly, the current assembly, all loaded assemblies and in the 
        /// referenced assemblies of the executable assembly.</para>
        /// <para>If the type can not be found then 0 is returned (no exceptions thrown).</para></summary>
        /// <param name="className">Full name of the type to be obtaines (must include namespace).</param>
        /// <param name="ignoreCase">If true then type name is case insensitive; default is false (case sensitive names).</param>
        public static Type GetType(string className, bool ignoreCase = false)
        {
            if (string.IsNullOrEmpty(className))
                throw new ArgumentException("Class name is not specified (null or empty string).");
            Type ret = null;
            // Check for the control type in the executable assembly:
            ret = Assembly.GetEntryAssembly().GetType(className, false /* throwOnError */, ignoreCase);
            if (ret == null)
            {
                // Type not found in the entry assembly, try to locate it in the currently executing assembly:
                ret = Assembly.GetExecutingAssembly().GetType(className, false /* throwOnError */, ignoreCase);
                if (ret == null)
                {
                    // Still not found, check all loaded assemblies:
                    Assembly[] assemblies = UtilSystem.GetLoadedAssemblies();
                    int numAssemblies = 0;
                    if (assemblies != null)
                        numAssemblies = assemblies.Length;
                    for (int whichAsm = 0; whichAsm < numAssemblies && ret == null; ++whichAsm)
                    {
                        Assembly asm = assemblies[whichAsm];
                        try
                        {
                            ret = asm.GetType(className, false /* throwOnError */, ignoreCase);
                        }
                        catch (Exception ex)
                        {
                            // There should be no exception here, therefore we output something to console:
                            Console.WriteLine(Environment.NewLine + "Exception thrown when trying to locate the form type "
                                + className + Environment.NewLine + "  in assembly \"" + asm.FullName + "\". Details: "
                                + Environment.NewLine + ex.Message);
                        }
                    }
                    if (ret == null)
                    {
                        // The type is not even found in loaded assemblies, try also the referenced assemblies:
                        assemblies = UtilSystem.GetReferencedAssemblies();
                        numAssemblies = 0;
                        if (assemblies != null)
                            numAssemblies = assemblies.Length;
                        for (int whichAsm = 0; whichAsm < numAssemblies && ret == null; ++whichAsm)
                        {
                            Assembly asm = assemblies[whichAsm];
                            try
                            {
                                ret = asm.GetType(className, false /* throwOnError */, ignoreCase);
                            }
                            catch (Exception ex)
                            {
                                // There should be no exception here, therefore we output something to console:
                                Console.WriteLine(Environment.NewLine + "Exception thrown when trying to locate the form type "
                                    + className + Environment.NewLine + "  in assembly \"" + asm.FullName + "\". Details: "
                                    + Environment.NewLine + ex.Message);
                            }
                        }
                    }
                }
            }
            return ret;
        }



        /// <summary>Instantiates an object of the specified type and performs checks on type inheritance if this is 
        /// specified by parameters.
        /// <para>Object type must have an argument-less constructor, otherwise exception is thrown.</para></summary>
        /// <exception cref="InvalidCastException">Thrown if object type does not correspond to the inheritance requirements specified by parameters.</exception>
        /// <exception cref="ArgumentException"> Thrown if <paramref name="objectType"/> is null.</exception>
        /// <param name="objectType">Type of the object to be instantiated.</param>
        /// <param name="superClass">If specified (not null) then the <paramref name="objectType"/> must inherit from this type.</param>
        /// <param name="notSuperClass">If specified (not null) then the <paramref name="objectType"/> must NOT inherit from this type.</param>
        /// <returns>Newly created object of the specified type.</returns>
        public static object InstantiateObject(Type objectType, Type superClass = null, Type notSuperClass = null)
        {
            if (objectType == null)
                throw new ArgumentException("Type of the object to be created is not specified (null argument).");
            else
            {
                if (superClass != null)
                    if (!objectType.IsSubclassOf(superClass))
                    {
                        throw new InvalidCastException("Type " + objectType.FullName + " is not subclass of " + superClass.FullName + 
                            " while it is supposed to be.");
                    }
                if (notSuperClass != null)
                    if (objectType.IsSubclassOf(notSuperClass))
                    {
                        throw new InvalidCastException("Type " + objectType.FullName + " is a subclass of " + superClass.FullName + 
                            " while it is not supposed to be.");
                    }
            }
            return System.Activator.CreateInstance(objectType);
        }


        #endregion AssemblyTypes





        #region AssemblyResources

        // ASSEMBLY RESOURCES:
        // There are two type of resources, embedded file resources, and resources that are compiled from .resx files.
        // Good starting point (check under Remarks):
        // https://msdn.microsoft.com/en-us/library/system.reflection.assembly.getmanifestresourcenames.aspx
        //    Good article on resources: in C# in a Nutshell
        //      https://books.google.lu/books?id=_Y0rWd-Q2xkC&pg=PA548&lpg=PA548&dq=how+to+get+stream+of+resource+within+resource+file&source=bl&ots=R9FwU6WG-j&sig=ESi1A_dkYpM69Xbc2c6N0PzeE0M&hl=en&sa=X&redir_esc=y#v=onepage&q=how%20to%20get%20stream%20of%20resource%20within%20resource%20file&f=false
        // EMBEDDED resources:
        // Put files in directories within projects, include them in the project, then under Properties (right-click)
        // in Solution Explorer) set Build Action to "Embedded Resource". Directory structure (as seen in Solution 
        // Explorer) is reflected in resouce name (a kind of path) as returned by Assembly.GetManifestResourceNames()
        // and as passed to Assembly.GetManifestResourceStream(): "<namespace>.<dir1>.<dir2>.<filename>" where namespace
        // is default namespace of the assembly, dir1.dir2 corresponds to relative directory path within the project, 
        // (e.g., file was contained in directory dir1/dir2) and filename is original file name, including extension.
        //  - To list: names = Assembly.GetManifestResourceNames()
        //  - To access: stream = Assembly.GetManifestResourceStream(name[i]), 
        //      then use tools like Image.FromStream(stream), various StreamReader-s,  etc.
        // .RESX resources:
        // Include the via resource editor - double click on .resx file to open
        //  - Included through .resx files (XML).
        //  - Strings included directly, other usually by relative links to files
        //  - Compiled into a .resources binary file included in assembly
        //  - Class generated automatically with access properties (<filename>.Designer.cs) (returning objects for non-string data)
        //  - Default is IG.Forms.Properties.Resources.resources (in Properties directory, together with AssemblyInfo.cs)
        //  - Naming similar as for embedded resources, but with .resources extension, contained resources without extension
        // Accessed either through ResourceManager (or ResourceReader), or throug accessing autometically generated class (strongly typed).
        //   - To list resources from .resx file:
        //       stream = assembly.GetManifestResourceStream("<resFileName>.resources");
        //       reader = new ResourceReader(stream);
        //       IDictionaryEnumerator dict = reader.GetEnumerator();
        //       while (dict.MoveNext()) { var a = dict.Key. var b = dict.Value;  }
        //   - Access to .resx-contained resources throgh resouce manager:
        //    - mgr = new ResourceManager (baseName, assmebly)  ; baseName = name of resource file without extension 
        //    - img = (img) mgr.GetObject(name)    for strings: mgr.GetString(name).  name is file name without extension.
        //   - Access through generated resource classs properties:
        //    - class is <ResourceFileName>.Designer.cs
        // 
        // Advantages of embedded resources:
        //   - normal files in the project, can be edited by usual means
        // Advantages of .resx files:
        //   - easy programatic access through strongly typed members of generated resource class


        private static char _resourceSeparator = ':';

        private static string _resourceSeparatorString = null;

        private static char _assemblySeparator = ';';

        private static string _assemblySeparatorString = null;

        /// <summary>Gets the character that is used to separate resource from the resource file (.resources).</summary>
        public static char ResourceSeparator
        { get { return _resourceSeparator; } }

        /// <summary>Gets the string that is used to separate resource from the resource file (.resources).
        /// <para>The separator is originally defined as character, this method just returns the string 
        /// that contains only that character.</para></summary>
        public static string ResourceSeparatorString
        {
            get
            {
                if (_resourceSeparatorString == null)
                {
                    lock (Util.LockGlobal)
                    {
                        _resourceSeparatorString = new string(new char[] { _resourceSeparator });
                    }
                }
                return _resourceSeparatorString;
            }
        }

        /// <summary>Gets the character that is used to separate resource from the assembly.</summary>
        public static char AssemblySeparator
        { get { return _assemblySeparator; } }

        /// <summary>Gets the string that is used to separate resource from the assembly.
        /// <para>The separator is originally defined as character, this method just returns the string 
        /// that contains only that character.</para></summary>
        public static string AssemblySeparatorString
        {
            get
            {
                if (_assemblySeparatorString == null)
                {
                    lock (Util.LockGlobal)
                    {
                        _assemblySeparatorString = new string(new char[] { _assemblySeparator });
                    }
                }
                return _assemblySeparatorString;
            }
        }

        // resource resource

        #region Resources.PathManipulations

        /// <summary>Returns a flag indicating whether the specified resource path represents a resource included
        /// through a resource file (extension .resx, which compiles to file with extension .resourced).</summary>
        /// <param name="resourcePath">Full path to the resource. If this is a resource that was included via
        /// resource file (.resx), this consists of the path of the resources file (extension .resources) and name
        /// of the resource without extension.</param>
        public static bool IsPathResxResource(string resourcePath)
        {
            bool ret = false;
            if (!string.IsNullOrEmpty(resourcePath))
            {
                ret = resourcePath.Contains(":");
                if (ret)
                {
                    ret = false;
                    string[] splitPath = resourcePath.Split(':');
                    if (splitPath != null)
                        if (splitPath.Length >= 2)
                        {
                            ret = splitPath[splitPath.Length - 2].EndsWith(".resources");
                        }
                }
            }
            return ret;
        }

        /// <summary>Returns a flag indicating whether the specified resource path represents a compiled resource file that
        /// was compiled via the XML resource file (extension .resx, which compiles to file with extension .resourced).</summary>
        /// <param name="resourcePath">Full path to the resource or resource file. If this is a resource that was included via
        /// resource file (.resx), this consists of the path of the resources file (extension .resources) and name
        /// of the resource without extension.</param>
        public static bool IsPathResxResourceFile(string resourcePath)
        {
            bool ret = false;
            if (!string.IsNullOrEmpty(resourcePath))
            {
                ret = resourcePath.EndsWith(".resources");
                if (ret)
                {
                    ret = !resourcePath.Contains(":");
                }
            }
            return ret;
        }

        /// <summary>Returns a flag indicating whether the specified resource path represents an embedded resource (a resource
        /// that is compiled into assembly as a stand-alone file).</summary>
        /// <param name="resourcePath">Full path to the resource or resource file. If this is a resource that was included via
        /// resource file (.resx), this consists of the path of the resources file (extension .resources) and name
        /// of the resource without extension.</param>
        public static bool IsPathEmbeddedResource(string resourcePath)
        {
            bool ret = false;
            if (!string.IsNullOrEmpty(resourcePath))
            {
                ret = (!resourcePath.EndsWith(".resources") && !resourcePath.Contains(":"));
            }
            return ret;
        }




        /// <summary>Returns name of the resource with the specified path that was compiled to assembly
        /// throuwh a .resx file (XML resource file).
        /// <para>If the <paramref name="resourcePath"/> does not represent such a resource then null is returned.</para></summary>
        /// <param name="resourcePath">Full path to the resource or resource file. If this is a resource that was included via
        /// resource file (.resx), this consists of the path of the resources file (extension .resources) and name
        /// of the resource without extension.</param>
        public static string GetResxResourceName(string resourcePath)
        {
            string ret = null;
            if (IsPathResxResource(resourcePath))
            {
                string[] splitPath = resourcePath.Split(':');
                if (splitPath != null)
                    if (splitPath.Length >= 2)
                    {
                        ret = splitPath[splitPath.Length - 1];
                    }
            }
            return ret;
        }


        /// <summary>Returns path of the resource file, given the specified path of a resource (or its resource file) that was compiled to assembly
        /// through a .resx file (XML resource file).
        /// <para>If the <paramref name="resourcePath"/> does not represent such a resource then null is returned.</para></summary>
        /// <param name="resourcePath">Full path to the resource or resource file. If this is a resource that was included via
        /// resource file (.resx), this consists of the path of the resources file (extension .resources) and name
        /// of the resource without extension.
        /// <para>IMPORTANT: For this method, parameter can either be a path to the resource (consisting of resource file path and resource name 
        /// separated by colon) or it can just be a path to the resource.</para></param>
        public static string GetResxResourceFilePath(string resourcePath)
        {
            string ret = null;
            if (IsPathResxResource(resourcePath))
            {
                // The specified path is full path of the resource:
                string[] splitPath = resourcePath.Split(':');
                if (splitPath != null)
                    if (splitPath.Length >= 2)
                    {
                        ret = splitPath[0];
                    }
            }
            else
            {
                // Tehe specified path is actually path to the resource file, therefore we just return it:
                if (IsPathResxResourceFile(resourcePath))
                    return resourcePath;
            }
            return ret;
        }

        /// <summary>Returns name of the embedded resource (i.e., its filename) with the specified path.
        /// <para>If the <paramref name="resourcePath"/> does not represent such a resource then null is returned.</para>
        /// <para>WARNING: This method is not reliable and can only be ursed for informative purposes! For example, it does 
        /// not return the correct path if the embedded resource file does not have an extension.</para>summary>
        /// <param name="resourcePath">Full path to the resource or resource file. If this is a resource that was included via
        /// resource file (.resx), this consists of the path of the resources file (extension .resources) and name
        /// of the resource without extension.</param>
        public static string GetEmbeddedResourceName(string resourcePath)
        {
            string ret = null;
            if (IsPathEmbeddedResource(resourcePath))
            {
                string lastPart = Path.GetExtension(resourcePath);
                if (string.IsNullOrEmpty(lastPart))
                    ret = resourcePath;
                else
                {
                    string firstPart = Path.GetExtension(Path.GetFileNameWithoutExtension(resourcePath));
                    if (string.IsNullOrEmpty(firstPart))
                    {
                        ret = lastPart;
                    }
                    else
                    {
                        ret = firstPart + "." + lastPart;
                    }
                }
            }
            return ret;
        }


        /// <summary>Returns path of the embedded resource' directory for a resource with the specified path.
        /// <para>If the <paramref name="resourcePath"/> does not represent such a resource then null is returned.</para>
        /// <para>WARNING: This method is not reliable and can only be ursed for informative purposes! For example, it does 
        /// not return the correct path if the embedded resource file does not have an extension.</para></summary>
        /// <param name="resourcePath">Full path to the resource or resource file. If this is a resource that was included via
        /// resource file (.resx), this consists of the path of the resources file (extension .resources) and name
        /// of the resource without extension.</param>
        public static string GetEmbeddedResourceDirectory(string resourcePath)
        {
            string ret = null;
            if (IsPathEmbeddedResource(resourcePath))
            {
                string firstPart = Path.GetFileNameWithoutExtension(resourcePath);
                string firstFirstPart = Path.GetFileNameWithoutExtension(firstPart);
                if (string.IsNullOrEmpty(firstFirstPart))
                {
                    ret = firstPart;
                }
                else
                {
                    ret = firstFirstPart;
                }
            }
            return ret;
        }


        #endregion Resources.PathManipulations



        /// <summary>Returns a stream containing the embedded file resource included in the specified assembly.</summary>
        /// <param name="assembly">Assembly from which the resource is obtained.</param>
        /// <param name="resourceName">Name of the embedded file resource (i.e., its complete path, consisting of the
        /// default namespace of the assembly, dot separated directory path, and resource file name).</param>
        /// <returns>Open readable stream (which must be closed upon use) containing the resource.</returns>
        public static Stream AssemblyEmbeddedFileResourceStream(Assembly assembly, string resourceName)
        {
            if (assembly == null)
                throw new ArgumentException("Assembly not specified (null reference).");
            return assembly.GetManifestResourceStream(resourceName);
        }

        /// <summary>Returns a stream containing the embedded file resource included in the currently executing assembly.</summary>
        /// <param name="resourceName">Name of the embedded file resource (i.e., its complete path, consisting of the
        /// default namespace of the assembly, dot separated directory path, and resource file name).</param>
        /// <returns>Open readable stream (which must be closed upon use) containing the resource.</returns>
        public static Stream AssemblyEmbeddedFileResourceStream(string resourceName)
        {
            return AssemblyEmbeddedFileResourceStream(Assembly.GetExecutingAssembly(), resourceName);
        }


        /// <summary>The same as <see cref="GetAssemblyResourceFiles(Assembly, IList{string}, bool,)"/>, except
        /// that list of resource files is passed by references and a newly created list is assigned to the argument
        /// when a null valued variable is passed.</summary>
        public static IList<string> GetAssemblyResourceFiles(Assembly assembly, ref IList<string>
            resourceNames, bool clearBefore = true)
        {
            if (resourceNames == null)
                resourceNames = new List<string>();
            return GetAssemblyResourceFiles(assembly, ref resourceNames, clearBefore);
        }

        /// <summary>Returns names of the resource files (generated from .resx files, with .resources extensions) in the 
        /// specified assemnly and stores them on the provided list.</summary>
        /// <param name="assembly">Assembly that is searched for contained resources.</param>
        /// <param name="resourceNames">A <see cref="IList"/> list object where the names of lcated resources are stored.
        /// <para>If the list is not provided (i.e.,  it is null) then it is created by the function and returned.</para></param>
        /// <param name="clearBefore">If true then the provided list is cleared before resource names are loaded on it.</param>
        /// <returns>Reference of the list where names of the located resources are stored. This makes the method usable for
        /// the case where the caller does not provide the list, in which case it is created anew.</returns>
        public static IList<string> GetAssemblyResourceFiles(Assembly assembly, IList<string>
            resourceNames = null, bool clearBefore = true)
        {
            if (resourceNames == null)
            {
                resourceNames = new List<string>();
            }
            else
            {
                if (clearBefore)
                    resourceNames.Clear();
            }
            string[] embeddedFileResources = assembly.GetManifestResourceNames();
            if (embeddedFileResources != null)
            {
                int num = embeddedFileResources.Length;
                for (int i = 0; i < num; ++i)
                {
                    if (embeddedFileResources[i].EndsWith(".resources"))
                        resourceNames.Add(embeddedFileResources[i]);
                }
            }
            return resourceNames;
        }




        /// <summary>The same as <see cref="GetAssemblyEmbeddedFileResources(Assembly, IList{string}, bool, bool)"/>, except
        /// that list of resources is passed by references and a newly created list is assigned to the argument
        /// when a null valued variable is passed.</summary>
        public static IList<string> GetAssemblyEmbeddedFileResources(Assembly assembly, ref IList<string>
            resourceNames, bool clearBefore = true, bool includeResourceFiles = false)
        {
            if (resourceNames == null)
                resourceNames = new List<string>();
            return GetAssemblyEmbeddedFileResources(assembly, ref resourceNames, clearBefore, includeResourceFiles);
        }

        /// <summary>Returns names of the embedded resources in the specified assemnly and stores them on the provided 
        /// collection.</summary>
        /// <remarks>In order to get a list of resource files (compiled from .resx files), call the 
        /// <see cref="GetAssemblyResourceFiles(Assembly, IList{string}, bool)"/> method.</remarks>
        /// <param name="assembly">Assembly that is searched for contained resources.</param>
        /// <param name="resourceNames">A <see cref="IList"/> list object where the names of lcated resources are stored.
        /// <para>If the list is not provided (i.e.,  it is null) then it is created by the function and returned.</para></param>
        /// <param name="clearBefore">If true then the provided list is cleared before resource names are loaded on it.</param>
        /// <param name="includeResourceFiles">If true then resource files will also be included on the list. Default is false.</param>
        /// <returns>Reference of the list where names of the located resources are stored. This makes the method usable for
        /// the case where the caller does not provide the list, in which case it is created anew.</returns>
        public static IList<string> GetAssemblyEmbeddedFileResources(Assembly assembly, IList<string>
            resourceNames = null, bool clearBefore = true, bool includeResourceFiles = false)
        {
            if (resourceNames == null)
            {
                resourceNames = new List<string>();
            }
            else
            {
                if (clearBefore)
                    resourceNames.Clear();
            }
            string[] embeddedFileResources = assembly.GetManifestResourceNames();
            if (embeddedFileResources != null)
            {
                int num = embeddedFileResources.Length;
                for (int i = 0; i < num; ++i)
                {
                    if (includeResourceFiles)
                    {
                        resourceNames.Add(embeddedFileResources[i]);
                    }
                    else
                    {
                        if (!embeddedFileResources[i].EndsWith(".resources"))
                            resourceNames.Add(embeddedFileResources[i]);
                    }
                }
            }
            return resourceNames;
        }



        /// <summary>The same as <see cref="GetAssemblyResxResources(Assembly, IList{string}, bool)"/>, except
        /// that list of resources is passed by references and a newly created list is assigned to the argument
        /// when a null valued variable is passed.</summary>
        public static IList<string> GetAssemblyResxResources(Assembly assembly, ref IList<string>
            resourceNames, bool clearBefore = true)
        {
            if (resourceNames == null)
                resourceNames = new List<string>();
            return GetAssemblyResxResources(assembly, ref resourceNames, clearBefore);
        }

        /// <summary>Returns names of the resources included in the specified assemnly through a .resx file and 
        /// stores them on the provided 
        /// collection.</summary>
        /// <remarks>See: http://stackoverflow.com/questions/2041000/loop-through-all-the-resources-in-a-resx-file </remarks>
        /// <param name="assembly">Assembly that is searched for contained resources.</param>
        /// <param name="resourceNames">A <see cref="IList"/> list object where the names of lcated resources are stored.
        /// <para>If the list is not provided (i.e.,  it is null) then it is created by the function and returned.</para></param>
        /// <param name="clearBefore">If true then the provided list is cleared before resource names are loaded on it.</param>
        /// <returns>Reference of the list where names of the located resources are stored. This makes the method usable for
        /// the case where the caller does not provide the list, in which case it is created anew.</returns>
        public static IList<string> GetAssemblyResxResources(Assembly assembly, IList<string>
            resourceNames = null, bool clearBefore = true)
        {
            if (resourceNames == null)
            {
                resourceNames = new List<string>();
            }
            else
            {
                if (clearBefore)
                    resourceNames.Clear();
            }
            var resNames = assembly.GetManifestResourceNames();
            var resourceFileNames = resNames.Select(p => p).Where(p => p.EndsWith(".resources"));
            foreach (string resFilename in resourceFileNames)
            {
                var stream = assembly.GetManifestResourceStream(resFilename);
                var resReader = new ResourceReader(stream);
                IDictionaryEnumerator dict = resReader.GetEnumerator();
                //int num = 0;
                while (dict.MoveNext())
                {
                    //num++;
                    //Console.WriteLine("    {0:00}: {1} = {2}", num, dict.Key, dict.Value);
                    resourceNames.Add(resFilename + ":" + dict.Key);

                }
                resReader.Close();
            }

            return resourceNames;
        }



        /// <summary>Returns a string containing informatio nabout resources contained in the specified assembly, such as 
        /// images, sounds, strings, and various files. Information returned is on embedded resources (i.e., files that are 
        /// compiled into assembly and are accessed by path in a directory-like structure and full file names, eventually 
        /// with extension) and resources that are compiled into .resix files and are thus accessed by a fully qualified 
        /// name of a property in a Resources class.</summary>
        /// <param name="assembly">Assembly that is searched for resources. You can find application-related assemblies
        /// by one of the corresponding methods in the <see cref="UtilSystem"/> class.</param>
        /// <param name="includeEmbedded">Specifies whether embedded resources are included in the report.</param>
        /// <param name="includeResx">Specifies whether rerources from .resources files (compiled through .resx filles) are included in the report.</param>
        /// <param name="outputLevel">Level of output that is output to console. If 0 then  no output is produced.
        /// <para>Currently only 0 (no output at all) and 1 or more (complete output) are supported.</para></param>
        /// <returns>A string containing names of all located resources, each contained in a separate line.</returns>
        public static string GetAssemblyResourcesInfo(Assembly assembly, bool includeEmbedded = true, bool includeResx = true, int outputLevel = 1)
        {
            StringBuilder sbOutput = new StringBuilder();
            StringBuilder sbReturned = new StringBuilder();
            int versionLevel = 3;
            sbOutput.AppendLine("Assembly: " + assembly.GetName().Name + ", version " + GetAssemblyVersion(assembly, versionLevel));
            sbOutput.AppendLine("Full name: " + assembly.GetName().FullName);
            if (assembly == ExecutableAssembly)
                sbOutput.AppendLine("Executable name: " + GetAssemblyFileName(assembly));
            else
                sbOutput.AppendLine("File name: " + GetAssemblyFileName(assembly));
            sbOutput.AppendLine("Resource information:");


            List<string> embeddedFileResources = new List<string>();
            List<string> resxResources = new List<string>();
            try
            {
                // Obtain the embedded resources:
                GetAssemblyEmbeddedFileResources(assembly, embeddedFileResources, true /*  clearBefoe */, false /* includeresourceFiles */);
            }
            catch (Exception ex)
            {
                Console.WriteLine(Environment.NewLine + "ERROR when acquiring embedded resources: " + Environment.NewLine
                    + "  " + ex.Message + Environment.NewLine);
            }
            try
            {
                // Obtain the resources compiled into assembly through a .resx file:
                GetAssemblyResxResources(assembly, resxResources);
            }
            catch (Exception ex)
            {
                Console.WriteLine(Environment.NewLine + "ERROR when acuiring resources included through a .resx file: " + Environment.NewLine
                    + "  " + ex.Message + Environment.NewLine);
            }
            // Include resources in the report:
            if (includeEmbedded)
            {
                sbOutput.AppendLine("----");
                sbOutput.AppendLine("File resources embedded in assembly: ");
                if (embeddedFileResources.Count == 0)
                    sbOutput.AppendLine("  No such resources.  (null array)");
                else
                {
                    int numRes = embeddedFileResources.Count;
                    for (int i = 0; i < numRes; ++i)
                    {
                        sbOutput.AppendLine("  " + (i + 1).ToString("00") + ": " + embeddedFileResources[i]);
                        sbReturned.AppendLine(embeddedFileResources[i]);
                    }
                }
            }
            if (includeResx)
            {
                sbOutput.AppendLine("----");
                sbOutput.AppendLine("Resources in the resx files:");
                if (resxResources.Count == 0)
                    sbOutput.AppendLine("  No such resources.  (null array)");
                else
                {
                    int numRes = resxResources.Count;
                    string currentResourceFilename = null;
                    string currentResourcename = null;
                    int whichWithinFile = 0;
                    for (int i = 0; i < numRes; ++i)
                    {
                        try
                        {
                            string[] parsed = resxResources[i].Split(':');
                            if (parsed.Length < 2)
                                throw new InvalidDataException("Could not split .resx-included resource to file name and resource name.");
                            if (parsed[0] != currentResourceFilename)
                            {
                                currentResourceFilename = parsed[0];
                                sbOutput.AppendLine("In " + currentResourceFilename + ":");
                                whichWithinFile = 0;
                            }
                            ++whichWithinFile;
                            currentResourcename = parsed[1];
                            sbOutput.AppendLine("  " + whichWithinFile.ToString("00") + ": " + currentResourcename);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(Environment.NewLine + "ERROR in parsing resource stiring: " + resxResources[i] + Environment.NewLine);
                            throw;
                        }

                        // sbOutput.AppendLine("  " + (i + 1).ToString("00") + ": " + resxResources[i]);
                        sbReturned.AppendLine(resxResources[i]);
                    }
                }
            }
            if (outputLevel > 0)
                Console.WriteLine(sbOutput.ToString());

            if (outputLevel >= 5)
            {
                Console.WriteLine(Environment.NewLine + Environment.NewLine + "================ additional info & tests:" + Environment.NewLine);
                Console.WriteLine("Assembly simple name: " + assembly.GetName().Name);
                Console.WriteLine("Assembly path: " + assembly.Location);
                try
                {
                    //Console.WriteLine(Environment.NewLine + "Types in the assembly: ");
                    //var types = assembly.GetTypes().Select(t => t.FullName);
                    //foreach (string typeName in types)
                    //    Console.WriteLine("  " + typeName);

                    Console.WriteLine(Environment.NewLine + "All nmespaces used in the assembly:");
                    var namespaces = assembly.GetTypes().Select(t => t.Namespace).Distinct();
                    foreach (string ns in namespaces)
                        Console.WriteLine("  " + ns);

                    Console.WriteLine(Environment.NewLine + "Detailed information on resources:" + Environment.NewLine);
                    var resNames = assembly.GetManifestResourceNames();

                    Console.WriteLine(Environment.NewLine + ">>>> Detailed resource information on reources:");
                    foreach (string resourceName in resNames)
                    {
                        Console.WriteLine("Resource: " + resourceName);
                        ManifestResourceInfo resInfo = assembly.GetManifestResourceInfo(resourceName);
                        Console.WriteLine("  File name:           " + resInfo.FileName);
                        Console.WriteLine("  Resource location:   " + resInfo.ResourceLocation);
                        Console.WriteLine("  Referenced assembly: " + resInfo.ReferencedAssembly);
                    }

                    Console.WriteLine(Environment.NewLine + ">>>> Resource files included in assembly (.resources):");
                    var resourceFileNames = resNames.Select(p => p).Where(p => p.EndsWith(".resources"));
                    foreach (string resFilename in resourceFileNames)
                    {
                        Console.WriteLine("  " + resFilename);
                        string resFilenameWithoutExtension = Path.GetFileNameWithoutExtension(resFilename);
                        var stream = assembly.GetManifestResourceStream(resFilename);
                        var resReader = new ResourceReader(stream);
                        IDictionaryEnumerator dict = resReader.GetEnumerator();
                        int num = 0;
                        while (dict.MoveNext())
                        {
                            num++;
                            Console.WriteLine("    {0:00}: {1} = {2}", num, dict.Key, dict.Value);
                        }
                        resReader.Close();
                    }
                    Console.WriteLine();

                }
                catch (Exception ex)
                {
                    Console.WriteLine(Environment.NewLine + "ERROR: " + ex.Message + Environment.NewLine);
                }


            }

            return sbReturned.ToString();
        }

        #endregion AssemblyResources


        #region ResourceOrFileTypes

        // For supported types of images, see:
        // Bitmap constructor, https://msdn.microsoft.com/en-us/library/0cbhe98f%28v=vs.110%29.aspx 
        // Types of bitmaps: https://msdn.microsoft.com/en-us/library/at62haz6%28v=vs.110%29.aspx
        // Bitmap constructor with stream argument: https://msdn.microsoft.com/en-us/library/z7ha67kw%28v=vs.110%29.aspx 

        // ImageFormat Class: https://msdn.microsoft.com/en-us/library/system.drawing.imaging.imageformat%28v=vs.110%29.aspx

        // File Extensions:



        private static string[] _bitmapExtensionsDotNetDefault = { "BMP", "GIF", "EXIF", "JPG", "PNG", "TIFF" };


        private static List<string> _imageExtensionsDotNet = new List<string>(_bitmapExtensionsDotNetDefault);

        /// <summary>Returns true if the speicified file extesion (without a '.') represents one of the standard .NET recognized 
        /// bitmap file extensions; false if not.</summary>
        /// <param name="extension">File extension that is checked; should not contain '.', case does not matter.</param>
        /// <returns></returns>
        public static bool IsBitmapExtensionDotNet(string extension)
        {
            return _bitmapExtensionsDotNetDefault.Contains(extension.ToUpper());
        }



        private static string[] _soundExtensionsDotNetDefault = { "WAV" };


        private static List<string> _soundExtensionsDotNet = new List<string>(_soundExtensionsDotNetDefault);

        /// <summary>Returns true if the speicified file extesion (without a '.') represents one of the standard .NET recognized 
        /// sound file extensions; false if not.</summary>
        /// <param name="extension">File extension that is checked; should not contain '.', case does not matter.</param>
        /// <returns></returns>
        public static bool IsSoundExtensionDotNet(string extension)
        {
            return _soundExtensionsDotNetDefault.Contains(extension.ToUpper());
        }






        #endregion ResourceOrFileTypes


        #region Files

        /// <summary>Minimal number of checked bytes when determining whether a file is a text file.</summary>
        const int MinNumCheckedIsTextFile = 100;


        /// <summary>Returns true if the file with the specified path is existent and readable by the current process,
        /// false otherwise.
        /// <para>False is returned regardless of reasons why file is not readable for the process (permissions, existence,
        /// drive errors, etc.)</para></summary>
        /// <param name="filePath">Path to the file that is check for readibility. It can be path of a non-existent file, in which
        /// case false is returned.</param>
        /// <returns>True if the file at the specified path is actually readable by the current process, false otherwise.</returns>
        public static bool IsReadableFile(string filePath)
        {
            try
            {
                using (FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
                {
                    return true; 
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>Returns true if the file with the specified path is writable by the current process,
        /// false otherwise.
        /// <para>False is returned regardless of reasons why file is not writable for the process (permissions,
        /// drive errors, etc.)</para>
        /// <para>Parameter <paramref name="onlyExistent"/> controls whether file must exist in order to return true (default false).</para></summary>
        /// <param name="filePath">Path to the file that is check for writability. It can be path of a non-existent file.</param>
        /// <param name="onlyExistent">If true then function will automatically return false for non-existent files. 
        /// If false (which is default) then true will also be returned for non-existent files that the process can open
        /// for writing.
        /// <para>Remark: If true then the function will not create a file if it does not exist, because it will first test 
        /// for existence and will not attempt to open a file if it does not exist.</para></param>
        /// <returns>True if the file at the specified path is actually writable by the current process, false otherwise.</returns>
        public static bool IsWritableFile(string filePath, bool onlyExistent = false)
        {
            if (onlyExistent && !File.Exists(filePath))
            { return false; }
            try
            {
                using (FileStream stream = File.Open(filePath, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>Returns true if the file with the specified path is both readable and writable by the current process,
        /// false otherwise.
        /// <para>False is returned regardless of reasons why file is not readable or writable for the process (permissions,
        /// drive errors, etc.)</para>
        /// <para>Parameter <paramref name="onlyExistent"/> controls whether file must exist in order to return true (default false).</para></summary>
        /// <param name="filePath">Path to the file that is check for writability. It can be path of a non-existent file.</param>
        /// <param name="onlyExistent">If true then function will automatically return false for non-existent files. 
        /// If false (which is default) then true will also be returned for non-existent files that the process can open
        /// for writing and reading.
        /// <para>Remark: If true then the function will not create a file if it does not exist, because it will first test 
        /// for existence and will not attempt to open a file if it does not exist.</para></param>
        /// <returns>True if the file at the specified path is actually writable by the current process, false otherwise.</returns>
        public static bool IsReadableAndWritableFile(string filePath, bool onlyExistent = false)
        {
            if (onlyExistent && !File.Exists(filePath))
            { return false; }
            try
            {
                using (FileStream stream = File.Open(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }



        /// <summary>Detects if the specified file is a text file and detects the encoding.</summary>
        /// <param name="filePath">The file name.</param>
        /// <returns> true if the specified file is a text file text.</returns>
        public static bool IsTextFile(string filePath)
        {
            Encoding encoding = null;
            return IsTextFile(filePath, 0 /* numChecked */, out encoding);
        }


        /// <summary>Detects if the specified file is a text file and detects the encoding.</summary>
        /// <param name="filePath">The file name.</param>
        /// <param name="numChecked">The max. number of bytes to use for testing (if 0 then complete file is used).</param>
        /// <returns> true if the specified file is a text file text.</returns>
        public static bool IsTextFile(string filePath, int numChecked)
        {
            Encoding encoding = null;
            return IsTextFile(filePath, numChecked, out encoding);
        }

        /// <summary>Detects if the specified file is a text file and detects the encoding.</summary>
        /// <param name="filePath">The file name.</param>
        /// <param name="encoding">The detected encoding. </param>
        /// <returns> true if the specified file is a text file text.</returns>
        public static bool IsTextFile(string filePath, out Encoding encoding)
        {
            return IsTextFile(filePath, 0 /* checkedSize */, out encoding);
        }

        /// <summary>Detects if the specified file is a text file and detects the encoding.</summary>
        /// <param name="filePath">The file name.</param>
        /// <param name="numChecked">The max. number of bytes to use for testing (if 0 then complete file is used).</param>
        /// <param name="encoding">The detected encoding. </param>
        /// <returns> true if the specified file is a text file text.</returns>
        /// <remarks><para>Source:</para>
        /// <para>http://stackoverflow.com/questions/910873/how-can-i-determine-if-a-file-is-binary-or-text-in-c</para>
        /// <para>To detect file encoding, see Rick Strahl's blog:</para>
        /// <para>http://www.west-wind.com/weblog/posts/2007/Nov/28/Detecting-Text-Encoding-for-StreamReader</para></remarks>
        public static bool IsTextFile(string filePath, int numChecked, out Encoding encoding)
        {
            if (numChecked < MinNumCheckedIsTextFile)
                numChecked = MinNumCheckedIsTextFile;
            using (var fileStream = File.OpenRead(filePath))
            {
                var rawData = new byte[numChecked];
                var text = new char[numChecked];
                var isText = true;

                // Read raw bytes
                var rawLength = fileStream.Read(rawData, 0, rawData.Length);
                fileStream.Seek(0, SeekOrigin.Begin);

                // Detect encoding correctly (from Rick Strahl's blog)
                // http://www.west-wind.com/weblog/posts/2007/Nov/28/Detecting-Text-Encoding-for-StreamReader
                if (rawData[0] == 0xef && rawData[1] == 0xbb && rawData[2] == 0xbf)
                {
                    encoding = Encoding.UTF8;
                }
                else if (rawData[0] == 0xfe && rawData[1] == 0xff)
                {
                    encoding = Encoding.Unicode;
                }
                else if (rawData[0] == 0 && rawData[1] == 0 && rawData[2] == 0xfe && rawData[3] == 0xff)
                {
                    encoding = Encoding.UTF32;
                }
                else if (rawData[0] == 0x2b && rawData[1] == 0x2f && rawData[2] == 0x76)
                {
                    encoding = Encoding.UTF7;
                }
                else
                {
                    encoding = Encoding.Default;
                }

                // Read text and detect the encoding
                using (var streamReader = new StreamReader(fileStream))
                {
                    streamReader.Read(text, 0, text.Length);
                }

                using (var memoryStream = new MemoryStream())
                {
                    using (var streamWriter = new StreamWriter(memoryStream, encoding))
                    {
                        // Write the text to a buffer
                        streamWriter.Write(text);
                        streamWriter.Flush();

                        // Get the buffer from the memory stream for comparision
                        var memoryBuffer = memoryStream.GetBuffer();

                        // Compare only bytes read
                        for (var i = 0; i < rawLength && isText; i++)
                        {
                            isText = rawData[i] == memoryBuffer[i];
                        }
                    }
                }

                return isText;
            }
        }

        #endregion Files


        #region Paths

        const string WorkspaceDirectoryEnvironmentVar = "WORKSPACE";

        const string WorkspaceProjectsDirectoryEnvironmentVar = "WORKSPACEPROJECTS";

        const string WorkspaceProjectsDirName = "workspaceprojects";

        /// <summary>Returns the workspace directory, which is primarily located through the
        /// environment variable contained in <see cref="WorkspaceDirectoryEnvironmentVar"/> (usually named WORKSPACE).
        /// <para>If the workspace directory can not be located then nulll is returned.</para>
        /// <para>Workspace directory is base directory for code development.</para></summary>
        public static string GetWorkspaceDirectoryPath()
        {
            string ret = System.Environment.GetEnvironmentVariable(WorkspaceDirectoryEnvironmentVar);
            if (string.IsNullOrEmpty(ret))
            {
                if (IsUserIgor)
                {
                    string trial = "c:/users1/workspace";
                    RepairDirectoryPath(ref trial);
                    if (Directory.Exists(trial))
                        return trial;
                }
            }
            string[] trialstrings = { "c:/users/workspace", "d:/users/workspace", 
                                          "c:/users1/workspace", "d:/users1/workspace" };
            foreach (string str in trialstrings)
            {
                string trial = str;
                RepairDirectoryPath(ref trial);
                if (Directory.Exists(trial))
                    return trial;
            }
            if (!string.IsNullOrEmpty(ret)) if (!Directory.Exists(ret))
                    ret = null;
            return ret;
        }

        /// <summary>Returns a complete path of a file or directory specified by a relative path 
        /// to the workspace directory, or null if the workspace directory can not be located.
        /// <para>The workspace directory is obtained by calling the <see cref="GetWorkspaceDirectoryPath"/> method.</para></summary>
        /// <param name="relativePath">Path of the file or directory relative to the workspace directory.
        /// <para>If this argument represents an absolute path then the argument itself is returned.</para></param>
        /// <returns></returns>
        public static string GetWorkspacePath(string relativePath)
        {
            if (string.IsNullOrEmpty(relativePath))
                return null;
            else if (Path.IsPathRooted(relativePath))
                return relativePath;
            else
            {
                string basePath = GetWorkspaceDirectoryPath();
                if (!string.IsNullOrEmpty(basePath))
                    return Path.Combine(basePath, relativePath);
                else
                    return null;
            }
        }


        /// <summary>Returns the workspace projects directory, which is primarily located through the workspace
        /// directory (returned by the <see cref="GetWorkspaceDirectoryPath"/> method) as a directory located in
        /// the same directory and named <see cref="WorkspaceProjectsDirName"/>.
        /// <para>Workspace projects directory is base directory for various project data.</para></summary>
        public static string GetWorkspaceProjectsDirectoryPath()
        {
            string ret = System.Environment.GetEnvironmentVariable(WorkspaceProjectsDirectoryEnvironmentVar);
            if (string.IsNullOrEmpty(ret))
            {
                // The environment variable containing the workspaceprojects directory is not specified,
                // try to obtain the directory from the eventually known path workspace path:
                string workspaceDir = GetWorkspaceDirectoryPath();
                if (!string.IsNullOrEmpty(workspaceDir))
                {
                    string parent = Directory.GetParent(workspaceDir).FullName;
                    if (!string.IsNullOrEmpty(parent))
                    {
                        ret = Path.Combine(parent, WorkspaceProjectsDirName);
                        if (!Directory.Exists(ret))
                            ret = null;
                    }
                }
            }
            if (string.IsNullOrEmpty(ret))
            {
                if (IsUserIgor)
                {
                    string trial = "d:/users/workspaceprojects";
                    RepairDirectoryPath(ref trial);
                    if (Directory.Exists(trial))
                        ret = trial;
                }
            }
            if (!string.IsNullOrEmpty(ret)) if (!Directory.Exists(ret))
                    ret = null;
            return ret;
        }

        /// <summary>Returns a complete path of a file or directory specified by a relative path 
        /// to the workspace projects directory, or null if the workspace  projects directory can not be located.
        /// <para>The workspace projects directory is obtained by calling the <see cref="GetWorkspaceDirectoryPath"/> method.</para></summary>
        /// <param name="relativePath">Path of the file or directory relative to the workspace projects directory.
        /// <para>If this argument represents an absolute path then the argument itself is returned.</para></param>
        /// <returns></returns>
        public static string GetWorkspaceProjectsPath(string relativePath)
        {
            if (string.IsNullOrEmpty(relativePath))
                return null;
            else if (Path.IsPathRooted(relativePath))
                return relativePath;
            else
            {
                string basePath = GetWorkspaceProjectsDirectoryPath();
                if (!string.IsNullOrEmpty(basePath))
                    return Path.Combine(basePath, relativePath);
                else
                    return null;
            }
        }


        // IMPORTANT:
        // Agreement is that directory paths in standard form DO NOT end with a directory separator character.

        /// <summary>Repairs the specified directory path, if applicable, and returns the repaired directory path.
        /// <para>If the specified path ends with directory separator then the last character is removed.</para></summary>
        /// <param name="directoryPath">Directory path to be repaired.</param>
        /// <returns>The repaired directory path, or the original string if no reparations were necessary.</returns>
        static string GetRepairedDirectoryPath(string directoryPath)
        {
            string ret = directoryPath;
            RepairDirectoryPath(ref ret);
            return ret;
        }

        /// <summary>Repairs the specified directory path, if applicable.
        /// <para>If the specified path ends with directory separator then the last character is removed.</para></summary>
        /// <param name="directoryPath">Directory path to be repaired. Eventually repaired path is stored back to this variable.</param>
        static void RepairDirectoryPath(ref string directoryPath)
        {
            if (string.IsNullOrEmpty(directoryPath))
                throw new ArgumentException("Directory path is not specified (null or empty string).");
            if (directoryPath.Length > 0)
            {
                // Replace alternative character separators with system character separators:
                if (directoryPath.Contains(Path.AltDirectorySeparatorChar))
                    directoryPath = directoryPath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
                // Take care that the last character is NOT path separator:
                char lastChar = directoryPath[directoryPath.Length - 1];
                while (directoryPath.Length > 1 &&
                    (lastChar == Path.DirectorySeparatorChar || lastChar == Path.AltDirectorySeparatorChar))
                {
                    directoryPath = directoryPath.Substring(0, directoryPath.Length - 1);
                    lastChar = directoryPath[directoryPath.Length - 1];
                }
            }
        }


        /// <summary>Returns the specified directory path in standard form (absolute path, system's path separator, ended with path separator).
        /// <para>This method should always result in the same string for the same directory (regardles of the original form),
        /// therefore it is useful e.g. for naming a mutex used for locking access to a directory.</para></summary>
        /// <param name="directoryPath">Path to the directory, which can be specified in any acceptale form.</param>
        /// <returns>Standard form of the specified directory path: absolute path, with system's path separators, ending with path separator.</returns>
        public static string GetStandardizedDirectoryPath(string directoryPath)
        {
            string ret = directoryPath;
            StandardizeDirectoryPath(ref ret);
            return ret;
        }

        /// <summary>Converts the specified path to standard form (absolute path, system's path separator, ended with path separator).
        /// <para>This method should always result in the same string for the same directory (regardles of the original form),
        /// therefore it is useful e.g. for naming a mutex used for locking access to a directory.</para></summary>
        /// <param name="directoryPath">Path to the directory, which can be specified in any acceptale form.</param>
        /// <remarks>Result of this method is directory path that DOES NOT END WITH DIRECTORY SEPARATOR.
        /// <para>If the original path string on Windows OS is "d:" and the current directory is on the d: drive, then the result
        /// will be "d:" and not the current directory (this would be returned by the <see cref="Path.GetFullPath"/>).</para></remarks>
        public static void StandardizeDirectoryPath(ref string directoryPath)
        {
            if (string.IsNullOrEmpty(directoryPath))
                throw new ArgumentException("Directory path is not specified (null or empty string).");
            if (directoryPath.Length >= 2)
            {
                char lastChar = directoryPath[directoryPath.Length - 1];
                if (lastChar == ':')
                    directoryPath = directoryPath + Path.DirectorySeparatorChar;
            }
            directoryPath = Path.GetFullPath(directoryPath);
            //RepairDirectoryPath(ref directoryPath);
            if (directoryPath.Length > 1)
            {
                char lastChar = directoryPath[directoryPath.Length - 1];
                // char preLastChar = directoryPath[directoryPath.Length - 2];
                while (directoryPath.Length > 1
                    && (lastChar == Path.DirectorySeparatorChar || lastChar == Path.AltDirectorySeparatorChar))
                {
                    directoryPath = directoryPath.Substring(0, directoryPath.Length - 1);
                    lastChar = directoryPath[directoryPath.Length - 1];
                }
            }
        }

        /// <summary>Changes directory to the specified directory.</summary>
        /// <para>This method overcomes problems with the fact that calling <see cref="Directory.SetCurrentDirectory"/> 
        /// e.g. on "d:" (on Windows OS) when current directory is on the disk d: will not change the current directory
        /// (a backslash must be added to do so).</para>
        /// <param name="directoryPath">Path of the directory specified, can be a relaitve path.</param>
        public static void SetCurrentDirectory(string directoryPath)
        {
            if (!string.IsNullOrEmpty(directoryPath))
            {
                StandardizeDirectoryPath(ref directoryPath);
                if (directoryPath.Length > 1)
                {
                    char lastCharacter = directoryPath[directoryPath.Length - 1];
                    if (lastCharacter == ':')
                        directoryPath = directoryPath + Path.DirectorySeparatorChar;
                }
                Directory.SetCurrentDirectory(directoryPath);
            }
        }

        #endregion Paths


        #region RelativePathAbsolutePath

        /// <summary>Returns the absolute path of the specified path (which can be relative or absolute or
        /// whatever legal form).</summary>
        /// <param name="path">Path whose absolute path is returned.</param>
        public static string GetAbsolutePath(string path)
        {
            return GetStandardizedDirectoryPath(path);
        }

        /// <summary>Calculates and returns relativa path from one path to another.
        /// WARNING: 
        /// First path (with respect to this relative path is calculated) must be a directory path!</summary>
        /// <param name="fromPath">Path of the directory with respect to which the full path is calculated.
        /// It must be a directory path, file paths are not good.</param>
        /// <param name="toPath">Path for which relative path is calculated.</param>
        /// <returns>Relative path of the second argument with respect to the first argument.</returns>
        public static string GetRelativePath(string fromPath, string toPath)
        {
            //string fromFull = Path.Combine(Environment.CurrentDirectory, fromPath);
            //string toFull = Path.Combine(Environment.CurrentDirectory, toPath);
            string fromFull = Path.GetFullPath(fromPath);
            string toFull = Path.GetFullPath(toPath);


            List<string> fromParts = new List<string>(
                  fromFull.Split(Path.DirectorySeparatorChar));
            List<string> toParts =
                  new List<string>(toFull.Split(Path.DirectorySeparatorChar));

            fromParts.RemoveAll(string.IsNullOrEmpty);
            toParts.RemoveAll(string.IsNullOrEmpty);
            // Remove all the same parts in front
            bool areRelative = false;
            while (fromParts.Count > 0 && toParts.Count > 0 &&
                  StringComparer.OrdinalIgnoreCase.Compare(fromParts[0], toParts[0]) == 0)
            {
                fromParts.RemoveAt(0);
                toParts.RemoveAt(0);

                areRelative = true;
            }
            if (!areRelative)
                return toPath;
            else
            {
                // Number of remaining fromParts is number of parent dirs
                StringBuilder ret = new StringBuilder();
                for (int i = 0; i < fromParts.Count; i++)
                {
                    if (ret.Length > 0)
                        ret.Append(Path.DirectorySeparatorChar);

                    ret.Append("..");
                }
                // And the remainder of toParts
                foreach (string part in toParts)
                {
                    if (ret.Length > 0)
                        ret.Append(Path.DirectorySeparatorChar);

                    ret.Append(part);
                }
                if (ret.Length == 0)
                    return "." + Path.DirectorySeparatorChar;
                return ret.ToString();
            }
        }

        /// <summary>Tests calculation</summary>
        public static void ExampleRelativePath()
        {
            Console.WriteLine();
            Console.WriteLine("Test of calculation of relative paths: ");
            string pathFrom, pathTo, relativePath;
            pathFrom = @"c:\users\";
            pathTo = @"c:\users\inverse\fileop.lib";
            relativePath = GetRelativePath(pathFrom, pathTo);
            Console.WriteLine("Base:   " + pathFrom);
            Console.WriteLine("Target: " + pathTo);
            Console.WriteLine("Rel.:   " + relativePath);
            Console.WriteLine();
            pathFrom = @"users\";
            pathTo = @"users\inverse\fileop.lib";
            relativePath = GetRelativePath(pathFrom, pathTo);
            Console.WriteLine("Base:   " + pathFrom);
            Console.WriteLine("Target: " + pathTo);
            Console.WriteLine("Rel.:   " + relativePath);
            Console.WriteLine();
            pathFrom = @"c:\users\a\b\c";
            pathTo = @"c:\users\a\xxx";
            relativePath = GetRelativePath(pathFrom, pathTo);
            Console.WriteLine("Base:   " + pathFrom);
            Console.WriteLine("Target: " + pathTo);
            Console.WriteLine("Rel.:   " + relativePath);
            Console.WriteLine();
            pathFrom = @"c:\users\a\b\c\";
            pathTo = @"c:\users\a\xxx\yyy\zzz\file1.xml";
            relativePath = GetRelativePath(pathFrom, pathTo);
            Console.WriteLine("Base:   " + pathFrom);
            Console.WriteLine("Target: " + pathTo);
            Console.WriteLine("Rel.:   " + relativePath);
            Console.WriteLine();
            pathFrom = @"c:\users\a\b\c\my.xml";
            pathTo = @"c:\users\a\xxx\";
            relativePath = GetRelativePath(pathFrom, pathTo);
            Console.WriteLine("Base:   " + pathFrom);
            Console.WriteLine("Target: " + pathTo);
            Console.WriteLine("Rel.:   " + relativePath);
            Console.WriteLine();
            pathFrom = @"/a/b/c";
            pathTo = @"/a/b/x/y/c.tcl";
            relativePath = GetRelativePath(pathFrom, pathTo);
            Console.WriteLine("Base:   " + pathFrom);
            Console.WriteLine("Target: " + pathTo);
            Console.WriteLine("Rel.:   " + relativePath);
            Console.WriteLine();
        }

        ///// <summary>Creates and returns a relative path from one file or folder to another. </summary>
        ///// <param name="fromDirectory">
        ///// Contains the directory that defines the
        ///// start of the relative path.
        ///// </param>
        ///// <param name="toPath">
        ///// Contains the path that defines the
        ///// endpoint of the relative path.
        ///// </param>
        ///// <returns>
        ///// The relative path from the start
        ///// directory to the end path.
        ///// </returns>
        ///// <exception cref="ArgumentNullException"></exception>
        //public static string RelativePathTo( string fromDirectory, string toPath)
        //{
        //    if (fromDirectory == null)
        //        throw new ArgumentNullException("fromDirectory");
        //    if (toPath == null)
        //        throw new ArgumentNullException("toPath");
        //    bool isRooted = Path.IsPathRooted(fromDirectory)
        //        && Path.IsPathRooted(toPath);
        //    if (isRooted)
        //    {
        //        bool isDifferentRoot = string.Compare(
        //            Path.GetPathRoot(fromDirectory),
        //            Path.GetPathRoot(toPath), true) != 0;
        //        if (isDifferentRoot)
        //            return toPath;
        //    }
        //    StringCollection relativePath = new StringCollection();
        //    string[] fromDirectories = fromDirectory.Split(
        //        Path.DirectorySeparatorChar);
        //    string[] toDirectories = toPath.Split(
        //        Path.DirectorySeparatorChar);
        //    int length = Math.Min(
        //        fromDirectories.Length,
        //        toDirectories.Length);
        //    int lastCommonRoot = -1;
        //    // find common root
        //    for (int x = 0; x < length; x++)
        //    {
        //        if (string.Compare(fromDirectories[x],
        //            toDirectories[x], true) != 0)
        //            break;
        //        lastCommonRoot = x;
        //    }
        //    if (lastCommonRoot == -1)
        //        return toPath;
        //    // add relative folders in from path
        //    for (int x = lastCommonRoot + 1; x < fromDirectories.Length; x++)
        //        if (fromDirectories[x].Length > 0)
        //            relativePath.Add("..");
        //    // add to folders to path
        //    for (int x = lastCommonRoot + 1; x < toDirectories.Length; x++)
        //        relativePath.Add(toDirectories[x]);
        //    // create relative path
        //    string[] relativeParts = new string[relativePath.Count];
        //    relativePath.CopyTo(relativeParts, 0);
        //    string newPath = string.Join(
        //        Path.DirectorySeparatorChar.ToString(),
        //        relativeParts);
        //    return newPath;
        //}

        #endregion RelativePathAbsolutePath


        #region ListDirectory



        /// <summary>Provides a convenient array of strings containing only null.
        /// <para>WARNING: Do not change contents of this array!</para></summary>
        private static readonly string[] _searcPatternsNone = { null };


        /// <summary>Recursively lists files and directories within the specified directories, and stores their paths 
        /// in the specified list, ordered in a tree-like fashion (any directory is processed to all depths before
        /// another same level directory is processed).
        /// <para>Files and directories can be listed in a single or in several root directories.</para>
        /// <para>Only files or only directories can be listed, and a list of matching pattern such as {"*.txt", "*.dll"} 
        /// can be specified.</para></summary>
        /// <remarks><para>This function is not recursive.</para></remarks>
        /// <param name="directoryPath">Path within which files and/or directories will be listed. 
        /// <para>If not specified then it is ignored, and files/directories will not be searched for in this path (but they 
        /// can still be searched for in directories contained in <paramref name="pathList"/>) if <paramref name="includeList"/> is true.</para></param>
        /// <param name="pathList">List where matching paths are stored.<para>If null then it is created anew.</para>
        /// <para>If already populated and <paramref name="includeList"/> is true then files/directories will also be listed in
        /// the directories whose paths are included in the list before the call. This is regardless of the value of the 
        /// <paramref name="clearOnBeginning"/> flag.</para>
        /// <para>If already populated and <paramref name="clearOnBeginning"/> is false then discovered files will be added to existig ones.</para></param>
        /// <param name="auxList">Auxiliary list provided for the method to store its working data. If null then the method allocates one.</param>
        /// <param name="numLevels">Number of levels of subdirectories in which files/directories are listed.
        /// <para>If less than 0 then unlimited number of levels will be searched for.</para>
        /// <para>If 0 then no files/directories will be searched for.</para>
        /// <para>If 1 then only those files/directories are listed that are contained directly in the <paramref name="directoryPath"/>
        /// directory (and eventually in directories contained in the directories contained in <paramref name="pathList"/> prior to the
        /// method call, if <paramref name="includeList"/></para> is true). If 2 then also files/directories in the first level 
        /// directories are listed, if 3 then also files/directories in the second level directories will be listed, etc.</param>
        /// <param name="includeList">If true then search for files/directories is also performed in directories that were contained
        /// in teh <paramref name="pathList"/> just before the method was called (beside the <paramref name="directoryPath"/>).</param>
        /// <param name="clearOnBeginning">If true then the list of files (parameter <paramref name="pathList"/>) is cleared before
        /// any discovered files or directories are added to the list. If false then discovered files/directories are just added
        /// to the existing paths.</param>
        /// <param name="RelativePaths">If true then all paths that are put to the list are converted to relative path with respect
        /// to the current directory. Relative paths with respect to any other directory are not implemented.</param>
        /// <param name="listDirectories">If true then directory paths are also listed (which is default), otherwise directories are omitted.</param>
        /// <param name="listFiles">If true then file paths are also listed (which is default), otherwise files are omitted.</param>
        /// <param name="searchPatterns">Eventual list of search patterns according to which files are searched for.
        /// <para>WARNING: search patterns do not apply to directories.</para></param>
        /// <returns>The number of discovered matching paths that were added to the list.</returns>
        public static int ListFilesRecursively(string directoryPath, ref List<string> pathList, List<string> auxList,
            int numLevels = 0, bool includeList = false, bool clearOnBeginning = true,
            bool RelativePaths = false, bool listDirectories = true, bool listFiles = true, IList<string> searchPatterns = null)
        {
            if (pathList == null)
                pathList = new List<string>();
            if (auxList == null)
                auxList = new List<string>();
            // Mark position on aux. list:
            int currentIndex = auxList.Count;  // current index of file treated
            bool searchByPatterns = false;  // whether files/directories are searched by a pattern
            if (searchPatterns != null)
                if (searchPatterns.Count > 0)
                {
                    if (searchPatterns != _searcPatternsNone)
                        searchByPatterns = true;
                }
            if (!searchByPatterns)
                searchPatterns = _searcPatternsNone;  // just in order to unify searching by pattens and without

            int numAdded = 0;
            // Number of added files and directories:
            // Prepare a list of directories that will be listed at the initial level.
            // This includes the specified directory (when specified), but can also includes directories
            // that are already on the file list:
            if (!string.IsNullOrEmpty(directoryPath))
            {
                if (Directory.Exists(directoryPath))
                    auxList.Add(directoryPath);
                else
                    throw new ArgumentException("Directory does not exist: " + directoryPath + ".");
                directoryPath = null;  // we will not search for this in further iterations
            }
            if (includeList)
            {
                // also the directories that are already on the list will be listed:
                foreach (string path in pathList)
                {
                    if (!string.IsNullOrEmpty(path))
                    {
                        if (Directory.Exists(path))
                            auxList.Add(path);
                    }
                }
                includeList = false;  // prevent including file form the list in subsequent iterations
            }
            if (clearOnBeginning)
            {
                pathList.Clear();
                clearOnBeginning = false;
            }
            int level = 0;
            if (level < numLevels || numLevels < 0)
            {
                ++level;
                // Go through unworked directories and list contents:
                int current = currentIndex;
                int next = auxList.Count;
                for (int i = current; i < next; ++i)
                {
                    string workedDir = auxList[i];
                    if (!string.IsNullOrEmpty(workedDir) && Directory.Exists(workedDir))
                    {
                        if (listFiles)
                        {
                            string[] filePaths = null;
                            foreach (string searchPattern in searchPatterns)
                            {
                                if (searchByPatterns)
                                    filePaths = Directory.GetFiles(workedDir, searchPattern);
                                else
                                    filePaths = Directory.GetFiles(workedDir);
                                foreach (string path in filePaths)
                                {
                                    string added = path;
                                    if (RelativePaths)
                                        added = UtilSystem.GetRelativePath(".", path);
                                    pathList.Add(added);
                                    ++numAdded;
                                }
                            }
                        }
                        bool nextLevel = (level < numLevels || numLevels < 0); // whether we will go to next level
                        if (nextLevel || (listDirectories)) // && !searchByPatterns))
                        {
                            // We also need a list of all directories contained in the worked dir (either for the 
                            // next level or to add them to the list of paths discovered):
                            string[] dirPaths = Directory.GetDirectories(workedDir);
                            foreach (string path in dirPaths)
                            {
                                if (listDirectories) // && !searchByPatterns)  // we need to add the directory to the list
                                {
                                    string added = path;
                                    if (RelativePaths)
                                        added = UtilSystem.GetRelativePath(".", path);
                                    pathList.Add(added);
                                    ++numAdded;
                                }
                                // Recursively list contained dirextories and files:
                                if (nextLevel)
                                {

                                    numAdded += ListFilesRecursively(path /* directory listed recursively */,
                                        ref pathList, auxList, numLevels - 1 /* The remaining numver of levels */,
                                        false /* includeList; only one directory is now searched for */ ,
                                        false /* clearOnBeginning; recursive calls must preserve what has already been discovered */ ,
                                        RelativePaths, listDirectories, listFiles, searchPatterns);
                                }

                            }
                        }
                    }
                }
                currentIndex = next;
            }
            // Auxliliary list will only be cleared after the operation:
            if (clearOnBeginning)
                auxList.Clear();
            return numAdded;
        }


        /// <summary>Recursively (ordered by levels) lists files and directories within the specified directories, and 
        /// stores their paths in the specified list.
        /// <para>Listing is done in a level order, meaning that path at the first level are listed first, then paths at
        /// the second level (contained in the first level directories), etc.</para>
        /// <para>Files and directories can be listed in a single directory or in several directories.</para>
        /// <para>Only files or only directories can be listed, and a list of matching pattern such as {"*.txt", "*.dll"} 
        /// can be specified.</para></summary>
        /// <remarks><para>This function is not recursive.</para></remarks>
        /// <param name="directoryPath">Path within which files and/or directories will be listed. 
        /// <para>If not specified then it is ignored, and files/directories will not be searched for in this path (but they 
        /// can still be searched for in directories contained in <paramref name="pathList"/>) if <paramref name="includeList"/> is true.</para></param>
        /// <param name="pathList">List where matching paths are stored.<para>If null then it is created anew.</para>
        /// <para>If already populated and <paramref name="includeList"/> is true then files/directories will also be listed in
        /// the directories whose paths are included in the list before the call. This is regardless of the value of the 
        /// <paramref name="clearOnBeginning"/> flag.</para>
        /// <para>If already populated and <paramref name="clearOnBeginning"/> is false then discovered files will be added to existig ones.</para></param>
        /// <param name="auxList">Auxiliary list provided for the method to store its working data. If null then the method allocates one.</param>
        /// <param name="numLevels">Number of levels of subdirectories in which files/directories are listed.
        /// <para>If less than 0 then unlimited number of levels will be searched for.</para>
        /// <para>If 0 then no files/directories will be searched for.</para>
        /// <para>If 1 then only those files/directories are listed that are contained directly in the <paramref name="directoryPath"/>
        /// directory (and eventually in directories contained in the directories contained in <paramref name="pathList"/> prior to the
        /// method call, if <paramref name="includeList"/></para> is true). If 2 then also files/directories in the first level 
        /// directories are listed, if 3 then also files/directories in the second level directories will be listed, etc.</param>
        /// <param name="includeList">If true then search for files/directories is also performed in directories that were contained
        /// in teh <paramref name="pathList"/> just before the method was called (beside the <paramref name="directoryPath"/>).</param>
        /// <param name="clearOnBeginning">If true then the list of files (parameter <paramref name="pathList"/>) is cleared before
        /// any discovered files or directories are added to the list. If false then discovered files/directories are just added
        /// to the existing paths.</param>
        /// <param name="RelativePaths">If true then all paths that are put to the list are converted to relative path with respect
        /// to the current directory. Relative paths with respect to any other directory are not implemented.</param>
        /// <param name="listDirectories">If true then directory paths are also listed (which is default), otherwise directories are omitted.</param>
        /// <param name="listFiles">If true then file paths are also listed (which is default), otherwise files are omitted.</param>
        /// <param name="searchPatterns">Eventual list of search patterns according to which files or directories are searched for.
        /// <para>WARNING: search patterns do not apply to directories.</para></param>
        /// <returns>The number of discovered matching paths that were added to the list.</returns>
        public static int ListFilesByLevels(string directoryPath, ref List<string> pathList, List<string> auxList, 
            int numLevels = 0, bool includeList = false, bool clearOnBeginning = true,
            bool RelativePaths = false, bool listDirectories = true, bool listFiles = true, IList<string> searchPatterns = null)
        {
            if (pathList == null)
                pathList = new List<string>();
            if (auxList == null)
                auxList = new List<string>();
            // Mark position on aux. list:
            int currentIndex = auxList.Count;  // current index of file treated
            bool searchByPatterns = false;  // whether files/directories are searched by a pattern
            if (searchPatterns != null)
                if (searchPatterns.Count > 0)
                {
                    if (searchPatterns != _searcPatternsNone)
                        searchByPatterns = true;
                    searchByPatterns = true;
                }
            if (!searchByPatterns)
                searchPatterns = _searcPatternsNone;  // just in order to unify searching by pattens and without

            int numAdded = 0;
            // Number of added files and directories:
            // Prepare a list of directories that will be listed at the initial level.
            // This includes the specified directory (when specified), but can also includes directories
            // that are already on the file list:
            if (! string.IsNullOrEmpty(directoryPath))
            {
                if (Directory.Exists(directoryPath))
                    auxList.Add(directoryPath);
                else
                    throw new ArgumentException("Directory does not exist: " + directoryPath + ".");
                directoryPath = null;  // we will not search for this in further iterations
            }
            if (includeList)
            {
                // also the directories that are already on the list will be listed:
                foreach( string path in pathList)
                {
                    if (!string.IsNullOrEmpty(path))
                    {
                        if (Directory.Exists(path))
                            auxList.Add(path);
                    }
                }
                includeList = false;  // prevent including file form the list in subsequent iterations
            }
            if (clearOnBeginning)
            {
                pathList.Clear();
                clearOnBeginning = false;
            }
            int level = 0;
            while (level < numLevels || numLevels < 1)
            {
                ++level;
                // Go through unworked directories and list contents:
                int current = currentIndex;
                int next = auxList.Count;
                if (current == next)
                    break;  // we have worked all the directories available, exit  the while loop
                for (int i = current; i < next; ++i)
                {
                    string workedDir = auxList[i];
                    if (!string.IsNullOrEmpty(workedDir) && Directory.Exists(workedDir))
                    {
                        if (listFiles)
                        {
                            string[] filePaths = null;
                            foreach (string searchPattern in searchPatterns)
                            {
                                if (searchByPatterns)
                                    filePaths = Directory.GetFiles(workedDir, searchPattern);
                                else
                                    filePaths = Directory.GetFiles(workedDir);
                                foreach (string path in filePaths)
                                {
                                    string added = path;
                                    if (RelativePaths)
                                        added = UtilSystem.GetRelativePath(".", path);
                                    pathList.Add(added);
                                    ++numAdded;
                                }
                            }
                        }
                        bool nextLevel = (level < numLevels || numLevels < 0); // whether we will go to next level
                        if (nextLevel || (listDirectories)) // && ! searchByPatterns))
                        {
                            // We also need a list of all directories contained in the worked dir (either for the 
                            // next level or to add them to the list of paths discovered):
                            string[] dirPaths = Directory.GetDirectories(workedDir);
                            foreach (string path in dirPaths)
                            {
                                if (nextLevel)
                                    auxList.Add(path);  // we will need the directory path for the next level
                                if (listDirectories) //  && !searchByPatterns)  // we need to add the directory to the list
                                {
                                    string added = path;
                                    if (RelativePaths)
                                        added = UtilSystem.GetRelativePath(".", path);
                                    pathList.Add(added);
                                    ++numAdded;
                                }
                            }
                        }
                    }
                }
                currentIndex = next;
            }
            // Auxliliary list will only be cleared after the operation:
            if (clearOnBeginning)
                auxList.Clear();
            return numAdded;
        }



        #endregion ListDirectory

        #region CopyDirectory

        // See also: 
        // http://stackoverflow.com/questions/58744/best-way-to-copy-the-entire-contents-of-a-directory-in-c


        /// <summary>Recursively copies contents of the source directory to the target directory.</summary>
        /// <param name="sourceDirectoryPath">Path to the source directory.</param>
        /// <param name="targetDirectoryPath">Path to the target directory.</param>
        public static void CopyDirectory(string sourceDirectoryPath, string targetDirectoryPath)
        {
            DirectoryInfo diSource = new DirectoryInfo(sourceDirectoryPath);
            DirectoryInfo diTarget = new DirectoryInfo(targetDirectoryPath);
            CopyRecursive(diSource, diTarget);
        }


        /// <summary>Recursively copies contents of the source directory to the target directory.</summary>
        /// <param name="source">Source directory.</param>
        /// <param name="target">Target directory.</param>
        public static void CopyRecursive(DirectoryInfo source, DirectoryInfo target)
        {
            // Check if the target directory exists, if not, create it.
            if (!Directory.Exists(target.FullName))
            {
                Directory.CreateDirectory(target.FullName);
            }
            // Copy each file into it's new directory.
            FileInfo[] containedFiles = source.GetFiles();
            foreach (FileInfo file in containedFiles)
            {
                if (Util.OutputLevel > 1)
                {
                    Console.WriteLine(@"Copying {0} to {1}", target.FullName, file.Name);
                }
                file.CopyTo(Path.Combine(target.FullName /* ToString() */, file.Name), true);
            }
            // Copy each subdirectory using recursion.
            DirectoryInfo[] subDirs = source.GetDirectories();
            foreach (DirectoryInfo subDir in subDirs)
            {
                if (Util.OutputLevel > 1)
                {
                    Console.WriteLine(">> Copying directory " + subDir.FullName + "...");
                }
                DirectoryInfo nextTargetSubDir =
                    target.CreateSubdirectory(subDir.Name);
                CopyRecursive(subDir, nextTargetSubDir);
            }
        }



        /// <summary>Recursively copies contents of the source directory to the target directory.
        /// <para>Fail safe variant, does not throw exceptions and does not break execution when any
        /// individual basic operation fails.</para></summary>
        /// <param name="sourceDirectoryPath">Path to the source directory.</param>
        /// <param name="targetDirectoryPath">Path to the target directory.</param>
        /// <param name="numErrors">Variable where number of exceptions cathced during the call is written.</param>
        /// <param name="errorsString">String variable where information about all errors that occurred during operation
        /// is stored.</param>
        /// <remarks>This is a safe variant of the method. Basic operations are embedded in try/catch blocks.
        /// If any individual basic operation fails (such as copying a single file or creating a target subdirectory)
        /// then copying of the remaining directory structure is not interrupted.
        /// <para>It should be well considered whether this variant or the plain variant (which fails and throws exception 
        /// when an individual operation fails) should be used. Problem with this version is that it does not throw 
        /// wxception if a part of directory structure could not be copied, so the caller will not know that.
        /// Use of this variant could be beneficial when it is likely that copying of some special parts (such as hidden
        /// or system files) is likely to fail but this is not relefant for function of the copied directory.</para>
        /// <para>If <see cref="Util.OutputLevel"/>>0 then all errors are reporter on console.</para></remarks>
        public static void CopyDirectorySafe(string sourceDirectoryPath, string targetDirectoryPath,
            out int numErrors, out string errorsString)
        {
            string erStr;
            numErrors = 0;
            errorsString = null;
            try
            {
                DirectoryInfo diSource = new DirectoryInfo(sourceDirectoryPath);
                DirectoryInfo diTarget = new DirectoryInfo(targetDirectoryPath);
                CopyRecursiveSafe(diSource, diTarget, ref numErrors, ref errorsString);
            }
            catch (Exception ex)
            {
                ++numErrors;
                erStr = Environment.NewLine
                    + "ERROR while copying directories: " + ex.Message
                        + Environment.NewLine + "  source: " + sourceDirectoryPath
                        + Environment.NewLine + "  target: " + targetDirectoryPath
                    + Environment.NewLine;
                errorsString += erStr;
                if (Util.OutputLevel > 0)
                {
                    Console.Write(erStr);
                }
            }
        }


        /// <summary>Recursively copies contents of the source directory to the target directory.
        /// <para>Fail safe variant, does not throw exceptions and does not break execution when any
        /// individual basic operation fails.</para></summary>
        /// <param name="source">Source directory.</param>
        /// <param name="target">Target directory.</param>
        /// <param name="numErrors">Variable where number of exceptions cathced during the call is written.
        /// Value of the variable increments for each exception catched.</param>
        /// <param name="errorsString">String variable where information about all errors that occurred during operation
        /// is stored. For each exception catched, a short error report is appended to its value.</param>
        /// <remarks>This is a safe variant of the method. Basic operations are embedded in try/catch blocks.
        /// If any individual basic operation fails (such as copying a single file or creating a target subdirectory)
        /// then copying of the remaining directory structure is not interrupted.
        /// <para>It should be well considered whether this variant or the plain variant (which fails and throws exception 
        /// when an individual operation fails) should be used. Problem with this version is that it does not throw 
        /// wxception if a part of directory structure could not be copied, so the caller will not know that.
        /// Use of this variant could be beneficial when it is likely that copying of some special parts (such as hidden
        /// or system files) is likely to fail but this is not relefant for function of the copied directory.</para>
        /// <para>If <see cref="Util.OutputLevel"/>>0 then all errors are reporter on console.</para></remarks>
        public static void CopyRecursiveSafe(DirectoryInfo source, DirectoryInfo target,
            ref int numErrors, ref string errorsString)
        {
            string erStr;
            // Check if the target directory exists, if not, create it.
            if (!Directory.Exists(target.FullName))
            {
                try
                {
                    Directory.CreateDirectory(target.FullName);
                }
                catch (Exception ex)
                {
                    ++numErrors;
                    erStr = Environment.NewLine
                        + "ERROR while copying directories. "
                            + Environment.NewLine + "  message: " + ex.Message
                            + Environment.NewLine + "  could not create directory: " + target.FullName
                        + Environment.NewLine;
                    errorsString += erStr;
                    if (Util.OutputLevel > 0)
                    {
                        Console.Write(erStr);
                    }
                }
            }
            try
            {
                // Copy each file into it's new directory.
                FileInfo[] containedFiles = source.GetFiles();
                foreach (FileInfo file in containedFiles)
                {
                    if (Util.OutputLevel > 1)
                    {
                        Console.WriteLine(@"Copying {0} to {1}", target.FullName, file.Name);
                    }
                    try
                    {
                        file.CopyTo(Path.Combine(target.FullName /* ToString() */, file.Name), true);
                    }
                    catch (Exception ex)
                    {
                        ++numErrors;
                        erStr = Environment.NewLine
                            + "ERROR while copying directories. "
                                + Environment.NewLine + "  message: " + ex.Message
                                + Environment.NewLine + "  could not copy file: " + file.FullName
                            + Environment.NewLine;
                        errorsString += erStr;
                        if (Util.OutputLevel > 0)
                        {
                            Console.Write(erStr);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ++numErrors;
                erStr = Environment.NewLine
                    + "ERROR while copying directories. "
                        + Environment.NewLine + "  message: " + ex.Message
                        + Environment.NewLine + "  could not copy files from: " + source.FullName
                    + Environment.NewLine;
                errorsString += erStr;
                if (Util.OutputLevel > 0)
                {
                    Console.Write(erStr);
                }
            }
            try
            {
                // Copy each subdirectory using recursion.
                DirectoryInfo[] subDirs = source.GetDirectories();
                foreach (DirectoryInfo subDir in subDirs)
                {
                    if (Util.OutputLevel > 1)
                    {
                        Console.WriteLine(">> Copying directory " + subDir.FullName + "...");
                    }
                    try
                    {
                        DirectoryInfo nextTargetSubDir = null;
                        try
                        {
                            nextTargetSubDir =
                                target.CreateSubdirectory(subDir.Name);
                        }
                        catch (Exception ex)
                        {
                            ++numErrors;
                            erStr = Environment.NewLine
                                + "ERROR while copying directories. "
                                    + Environment.NewLine + "  message: " + ex.Message
                                    + Environment.NewLine + "  could not create copy of directory: " + subDir.FullName
                                + Environment.NewLine;
                            errorsString += erStr;
                            if (Util.OutputLevel > 0)
                            {
                                Console.Write(erStr);
                            }
                        }
                        CopyRecursiveSafe(subDir, nextTargetSubDir, ref numErrors, ref errorsString);
                    }
                    catch (Exception ex)
                    {
                        ++numErrors;
                        erStr = Environment.NewLine
                            + "ERROR while copying directories. "
                                + Environment.NewLine + "  message: " + ex.Message
                                + Environment.NewLine + "  could not copy subdirectorty: " + subDir.FullName
                            + Environment.NewLine;
                        errorsString += erStr;
                        if (Util.OutputLevel > 0)
                        {
                            Console.Write(erStr);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (Util.OutputLevel > 0)
                {
                    ++numErrors;
                    erStr = Environment.NewLine
                        + "ERROR while copying directories. "
                        + Environment.NewLine + "  message: " + ex.Message
                        + Environment.NewLine + "  could not copy subdirectories from: " + source.FullName
                        + Environment.NewLine;
                    errorsString += erStr;
                    if (Util.OutputLevel > 0)
                    {
                        Console.Write(erStr);
                    }
                }
            }
        }



        /// <summary>Creates a test directory structure for testing operatios such as recursive copying.
        /// Returns path of the created directory structure, or null if something is wrong.</summary>
        /// <param name="baseDirPath">Path of the base directory within which the structure is created.</param>
        /// <param name="rootDirName">Name of the root directoy of the created directory structure.</param>
        /// <returns>Path of the root directory of the created test directory structure, or null if 
        /// the directory structure could not be created for some reason.</returns>
        public static string CreateTestdirectoryStructure(string baseDirPath, string rootDirName)
        {
            if (string.IsNullOrEmpty(baseDirPath))
                throw new ArgumentException("Base directory path not specified (null or empty string).");
            if (string.IsNullOrEmpty(rootDirName))
                throw new ArgumentException("Name of the root directory of the test directory structure is not specified (null or empty string).");
            if (!Directory.Exists(baseDirPath))
                throw new ArgumentException("Base directory for creating a test directory structure does not exist. "
                    + Environment.NewLine + "  directory path: " + baseDirPath);
            string rootDirPath = Path.Combine(baseDirPath, rootDirName);
            if (!Directory.Exists(rootDirPath))
            {
                Directory.CreateDirectory(rootDirPath);
            }
            string dirPath = rootDirPath;
            string filePath = Path.Combine(dirPath, "file1.txt");
            File.WriteAllText(filePath, "This is the file " + filePath);
            filePath = Path.Combine(dirPath, "file2.txt");
            File.WriteAllText(filePath, "This is the file " + filePath);

            dirPath = Path.Combine(rootDirPath, "dir1");
            Directory.CreateDirectory(dirPath);
            filePath = Path.Combine(dirPath, "file1.1.txt");
            File.WriteAllText(filePath, "This is the file " + filePath);
            filePath = Path.Combine(dirPath, "file1.2.txt");
            File.WriteAllText(filePath, "This is the file " + filePath);

            dirPath = Path.Combine(dirPath, "dir1.1");
            Directory.CreateDirectory(dirPath);
            filePath = Path.Combine(dirPath, "file1.1.1.txt");
            File.WriteAllText(filePath, "This is the file " + filePath);
            filePath = Path.Combine(dirPath, "file1.1.2.txt");
            File.WriteAllText(filePath, "This is the file " + filePath);
            filePath = Path.Combine(dirPath, "file1.1.3.txt");
            File.WriteAllText(filePath, "This is the file " + filePath);

            dirPath = Path.Combine(rootDirPath, "dir2");
            Directory.CreateDirectory(dirPath);
            filePath = Path.Combine(dirPath, "file2.1.txt");
            File.WriteAllText(filePath, "This is the file " + filePath);
            filePath = Path.Combine(dirPath, "file2.2.txt");
            File.WriteAllText(filePath, "This is the file " + filePath);
            return rootDirPath;
        }


        /// <summary>Examp</summary>
        public static void ExampleCopyDir()
        {
            int storedOutputLevel = Util.OutputLevel;
            Util.OutputLevel = 3;
            try
            {

                string baseDirPath = @"c:/temp/";
                if (!Directory.Exists(baseDirPath))
                {
                    Console.WriteLine();
                    Console.WriteLine("The directory in which test directory structure will be created does not exist. "
                        + Environment.NewLine + "  Dir. path: " + baseDirPath);
                    Console.WriteLine("Insert a new directory where test directory structure is created!");
                    Console.WriteLine();
                    Console.Write("Directory path: ");
                    baseDirPath = Console.ReadLine();
                    if (!Directory.Exists(baseDirPath))
                        baseDirPath = Path.GetTempPath();
                    if (!Directory.Exists(baseDirPath))
                    {
                        Console.WriteLine();
                        Console.WriteLine("The inserted directory does not exist, exiting example!");
                        Console.WriteLine("Inserted dir.: " + baseDirPath);
                        Console.WriteLine();
                        return;
                    }
                    Console.WriteLine("Base directory set to " + baseDirPath);
                    Console.WriteLine();
                }
                string sourceDirPath = CreateTestdirectoryStructure(baseDirPath, "dirSource/");
                string targetdirPath = Path.Combine(baseDirPath, "dirTarget/");
                Console.WriteLine();
                Console.WriteLine("Recursively copying directories, from "
                    + Environment.NewLine + "  " + sourceDirPath
                    + Environment.NewLine + "to "
                    + Environment.NewLine + "  " + targetdirPath
                    + Environment.NewLine + "...");
                StopWatch1 timer = new StopWatch1();
                timer.Start();
                CopyDirectory(sourceDirPath, targetdirPath);
                timer.Stop();
                Console.WriteLine("... done in " + timer.Time + " s.");
                Console.WriteLine();
            }
            catch (Exception) { throw; }
            finally
            {
                Util.OutputLevel = storedOutputLevel;
            }
        }

        #endregion CopyDirectory


        #region SerializationBinary

        #region SerializationBinary.TypeSafe

        /* IMPORTANT - BINARY SERIALIZATION
        BinaryFormatter class and the associated IFormatter were made obsolete in more recent frameworks
        due to potential security issues, and are not suported since NET 8.
        In legacy IGLib, to enable some legacy software, the project references a NuiGet package that still
        provides the functionality.
        The package is referenced when the target framework is .NET 8 or later. Beside this, compiler warnings
        are issuer in the code that uses these technologies, and informative messages are written to console
        for a limited number of times. These warnigs can be swotched on or off via the
        LanchConsoleWarningsOnBinarFormatterUse property. */

        /// <summary>Specifies whether informational messages are written to console when the BinaryFormatter
        /// is used.</summary>
        public static bool DoLaunchConsoleWarningsOnBinarFormatterUse { get; set; } = true;

        public static int MaxLaunchedBinayFormattingConsoleWarnings { get; set; } = 0;

        private static int NumLaunchedBinaryFormattingConsoleWarinings { get; set; } = 0;

        private static void LaunchBinaryFormattingConsoleWarining()
        {
            try
            {

                if (DoLaunchConsoleWarningsOnBinarFormatterUse &&
                    NumLaunchedBinaryFormattingConsoleWarinings < MaxLaunchedBinayFormattingConsoleWarnings)
                {
#if NET8_0_OR_GREATER
                    Console.WriteLine("\n\nNOTICE: Using BinaryFormatter from System.Runtime.Serialization.Formatters package.\n");
#else
                    Console.WriteLine("\n\nNOTICE: Using built-in BinaryFormatter.\n");
#endif
                }
            }
            finally {
                ++ NumLaunchedBinaryFormattingConsoleWarinings;
            }
        }

        // Disable the warning / error due to use of IFormatter and BinaryFormatter:
#pragma warning disable SYSLIB0011


        /// <summary>Save the specified serialized object in binary form to the specified file.</summary>
        /// <param name="objectToSave">Object to be saved.</param>
        /// <param name="fileName">File name to save the object into.</param>
        /// <typeparam name="ObjectType">Type of the object to be saved.</typeparam>
        /// <remarks><para>The object is saved using .NET serialization (binary formatter is used).</para></remarks>
        public static void SaveBinary<ObjectType>(ObjectType objectToSave, string fileName)
        {
            FileStream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None);
            SaveBinary(objectToSave, stream);
            stream.Close();
        }

        /// <summary>Save the specified serialized object in binary form to the specified stream.</summary>
        /// <param name="objectToSave">Object to be saved.</param>
        /// <param name="stream">Stream to save the object into.</param>
        /// <typeparam name="ObjectType">Type of the object to be saved.</typeparam>
        /// <remarks><para>The object is saved using .NET serialization (binary formatter is used).</para></remarks>
        public static void SaveBinary<ObjectType>(ObjectType objectToSave, Stream stream)
        {
            LaunchBinaryFormattingConsoleWarining();
            IFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, objectToSave);
        }

        /// <summary>Loads the from specified file.</summary>
        /// <param name="fileName">File name to load network from.</param>
        /// <remarks><para>Neural network is loaded from file using .NET serialization (binary formater is used).</para></remarks>
        public static ObjectType LoadBinary<ObjectType>(string fileName)
        {
            FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            ObjectType network = LoadBinary<ObjectType>(stream);
            stream.Close();
            return network;
        }

        /// <summary>Load network from specified file.</summary>
        /// <param name="stream">Stream to load network from.</param>
        /// <returns>Returns instance of the <typeparamref name="ObjectType"/> class with all properties initialized from file.</returns>
        /// <typeparam name="ObjectType">Type of the object to be saved.</typeparam>
        /// <remarks><para>Neural network is loaded from file using .NET serialization (binary formater is used).</para></remarks>
        public static ObjectType LoadBinary<ObjectType>(Stream stream)
        {
            LaunchBinaryFormattingConsoleWarining();
            IFormatter formatter = new BinaryFormatter();
            ObjectType network = (ObjectType)formatter.Deserialize(stream);
            return network;
        }

        // Re-enable the warning / error due to use of IFormatter and BinaryFormatter:
#pragma warning restore SYSLIB0011

        #endregion SerializationBinary.TypeSafe


        /// <summary>Save the specified serialized object in binary form to the specified file.</summary>
        /// <param name="objectToSave">Object to be saved.</param>
        /// <param name="fileName">File name to save the object into.</param>
        /// <remarks><para>The object is saved using .NET serialization (binary formatter is used).</para></remarks>
        public static void SaveBinary(object objectToSave, string fileName)
        {
            SaveBinary<object>(objectToSave, fileName);
        }

        /// <summary>Save the specified serialized object in binary form to the specified stream.</summary>
        /// <param name="objectToSave">Object to be saved.</param>
        /// <param name="stream">Stream to save the object into.</param>
        /// <remarks><para>The object is saved using .NET serialization (binary formatter is used).</para></remarks>
        public static void SaveBinary(object objectToSave, Stream stream)
        {
            SaveBinary<object>(objectToSave, stream);
        }

        /// <summary>Loads the from specified file.</summary>
        /// <param name="fileName">File name to load network from.</param>
        /// <returns>Returns instance of Network class with all properties initialized from file.</returns>
        /// <remarks><para>Neural network is loaded from file using .NET serialization (binary formater is used).</para></remarks>
        public static object LoadBinary(string fileName)
        {
            return LoadBinary<object>(fileName);
        }

        /// <summary>Load network from specified file.</summary>
        /// <param name="stream">Stream to load network from.</param>
        /// <returns>Returns instance of Network class with all properties initialized from file.</returns>
        /// <remarks><para>Neural network is loaded from file using .NET serialization (binary formater is used).</para></remarks>
        public static object LoadBinary(Stream stream)
        {
            return LoadBinary<object>(stream);
        }


        #endregion SerializationBinary



    } // class UtilSystem

}