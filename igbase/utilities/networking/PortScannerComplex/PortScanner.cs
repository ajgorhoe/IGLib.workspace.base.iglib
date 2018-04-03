using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace IG.Net
{

    /// <summary>A TCP port scanner, multi-threaded.
    /// <para>Basic idea taken from this project: https://github.com/PhilipMur/C-Sharp-Multi-Threaded-Port-Scanner
    /// - by Philip Murray (MIT license, see LICENSE.txt).</para></summary>
    /// $A Philip_Murray xx; Igor Apr18;
    class PortScanner2
    {

        private string host;
        private PortList2 portList;
        private bool turnOff = true;
        private int count = 0;
        public int tcpTimeout;

        private class isTcpPortOpen
        {
            public TcpClient MainClient { get; set; }
            public bool tcpOpen { get; set; }
        }


        public PortScanner2(string host, int portStart, int portStop, int timeout)
        {
            this.host = host;
            portList = new PortList2(portStart, portStop);
            tcpTimeout = timeout;

        }

        public void start(int threadCounter)
        {
            for (int i = 0; i < threadCounter; i++)
            {

                Thread thread1 = new Thread(new ThreadStart(RunScanTcp));
                thread1.Start();

            }

        }

        public void RunScanTcp()
        {

            int port;

            //while there are more ports to scan 
            while ((port = portList.NextPort()) != -1)
            {
                count = port;

                Thread.Sleep(1); //lets be a good citizen to the cpu

                Console.Title = "Current Port Count : " + count.ToString();

                try
                {

                    Connect(host, port, tcpTimeout);

                }
                catch
                {
                    continue;
                }

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine();
                Console.WriteLine("TCP Port {0} is open ", port);
                try
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    //grabs the banner / header info etc..
                    Console.WriteLine(BannerGrab(host, port, tcpTimeout));


                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Could not retrieve the Banner ::Original Error = " + ex.Message);
                    Console.ResetColor();
                }
                Console.ForegroundColor = ConsoleColor.Green;
                string webpageTitle = GetPageTitle("http://" + host + ":" + port.ToString());

                if (!string.IsNullOrWhiteSpace(webpageTitle))
                {
                    //this gets the html title of the webpage
                    Console.WriteLine("Webpage Title = " + webpageTitle + "Found @ :: " + "http://" + host + ":" + port.ToString());

                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    Console.WriteLine("Maybe A Login popup or a Service Login Found @ :: " + host + ":" + port.ToString());
                    Console.ResetColor();
                }


                Console.ResetColor();

            }


            if (turnOff == true)
            {

                turnOff = false;
                Console.WriteLine();
                Console.WriteLine("Scan Complete !!!");

                Console.ReadKey();

            }

        }
        //method for returning tcp client connected or not connected
        public TcpClient Connect(string hostName, int port, int timeout)
        {
            var newClient = new TcpClient();

            var state = new isTcpPortOpen
            {
                MainClient = newClient,
                tcpOpen = true
            };

            IAsyncResult ar = newClient.BeginConnect(hostName, port, AsyncCallback, state);
            state.tcpOpen = ar.AsyncWaitHandle.WaitOne(timeout, false);

            if (state.tcpOpen == false || newClient.Connected == false)
            {
                throw new Exception();

            }
            return newClient;
        }

        //method for Grabbing a webpage banner / header information
        public string BannerGrab(string hostName, int port, int timeout)
        {
            var newClient = new TcpClient(hostName, port);


            newClient.SendTimeout = timeout;
            newClient.ReceiveTimeout = timeout;
            NetworkStream ns = newClient.GetStream();
            StreamWriter sw = new StreamWriter(ns);

            //sw.Write("GET / HTTP/1.1\r\n\r\n");

            sw.Write("HEAD / HTTP/1.1\r\n\r\n"
                + "Connection: Closernrn");

            sw.Flush();

            byte[] bytes = new byte[2048];
            int bytesRead = ns.Read(bytes, 0, bytes.Length);
            string response = Encoding.ASCII.GetString(bytes, 0, bytesRead);

            return response;
        }


        //async callback for tcp clients
        void AsyncCallback(IAsyncResult asyncResult)
        {
            var state = (isTcpPortOpen)asyncResult.AsyncState;
            TcpClient client = state.MainClient;

            try
            {
                client.EndConnect(asyncResult);
            }
            catch
            {
                return;
            }

            if (client.Connected && state.tcpOpen)
            {
                return;
            }

            client.Close();
        }

        static string GetPageTitle(string link)
        {
            try
            {

                WebClient x = new WebClient();
                string sourcedata = x.DownloadString(link);
                string getValueTitle = Regex.Match(sourcedata, @"\<title\b[^>]*\>\s*(?<Title>[\s\S]*?)\</title\>", RegexOptions.IgnoreCase).Groups["Title"].Value;

                return getValueTitle;

            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Could not connect. Error:" + ex.Message);
                Console.ResetColor();
                return "";
            }


        }  // GetPageTitle(link)







        /// <summary>
        /// A Console type Multi Port TCP Scanner
        /// Author : Philip Murray
        /// </summary>
        public static void Run(string[] args)
        {
            string host;
            int portStart;
            int portStop;
            int Threads;
            int timeout;

            youGotItWrong: //goto: Start Again

            //this is for the user to select a host ip
            string ip;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Enter Host I.P or Domain example: (127.0.0.1) or (localhost) etc..");
            Console.ResetColor();
            Console.Write("Enter Host I.P or Domain : ");
            ip = Console.ReadLine();
            Console.WriteLine();
            host = ip;

            //this is for the user to select the start port
            string startPort;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Min Start Port : 0");
            Console.ResetColor();
            Console.Write("Enter A start Port : ");
            startPort = Console.ReadLine();
            Console.WriteLine();

            //THIS CHECKS TO SEE IF IT THE START PORT CAN BE PARSED OUT
            int number;
            bool resultStart = int.TryParse(startPort, out number);

            if (resultStart)
            {
                portStart = int.Parse(startPort);
            }

            else
            {
                Console.WriteLine("Try Again NOOOB!!");
                goto youGotItWrong;
                // return;
            }


            //this is for the end port
            string endPort;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Max End Port : 65535");
            Console.ResetColor();
            Console.Write("Enter A End Port : ");
            endPort = Console.ReadLine();
            Console.WriteLine();


            //THIS CHECKS TO SEE IF IT THE END PORT CAN BE PARSED OUT
            int number2;
            bool resultEnd = int.TryParse(endPort, out number2);

            if (resultEnd)
            {
                portStop = int.Parse(endPort);
            }

            else
            {
                Console.WriteLine("Try Again NOOOB!!");

                goto youGotItWrong;
                // return;
            }

            //this is how many threads will be started
            string threadsToRun;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Normal Thread amount is between 1 - 50 (less threads = higher accuracy)");
            Console.ResetColor();
            Console.Write("Enter How Many Threads To Run : ");
            threadsToRun = Console.ReadLine();
            Console.WriteLine();


            //THIS CHECKS TO SEE IF IT THE END PORT CAN BE PARSED OUT
            int number3;
            bool resultThreads = int.TryParse(threadsToRun, out number3);

            if (resultThreads)
            {
                Threads = int.Parse(threadsToRun);
            }

            else
            {
                Console.WriteLine("Try Again NOOOB!!");

                goto youGotItWrong;

                // return;
            }

            //this is how many threads will be started 
            string tcpTimeout;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Normal Timeout amount is between 1 - 10 secs ( 1 = 1 second)(higher timeout = higher accuracy)");
            Console.ResetColor();
            Console.Write("Enter Timeout : ");
            tcpTimeout = Console.ReadLine();
            Console.WriteLine();

            //THIS CHECKS TO SEE IF IT THE timeout CAN BE PARSED OUT
            int number4;
            bool resultTimeout = int.TryParse(tcpTimeout, out number4);

            if (resultTimeout)
            {
                timeout = int.Parse(tcpTimeout) * 1000;

            }

            else
            {
                Console.WriteLine("Try Again NOOOB!!");

                goto youGotItWrong;
                //  return;
            }

            try
            {

                host = ip;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }

            if (resultStart == true && resultEnd == true)
            {
                try
                {

                    portStart = int.Parse(startPort);
                    portStop = int.Parse(endPort);

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return;
                }

            }
            if (resultThreads == true)
            {
                try
                {

                    Threads = int.Parse(threadsToRun);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);

                    return;
                }
            }

            PortScanner2 ps = new PortScanner2(host, portStart, portStop, timeout);
            ps.start(Threads);

        }  // Main

        public class PortList2
        {
            private int start;
            private int stop;
            private int ports;

            public PortList2(int starts, int stops)
            {
                start = starts;
                stop = stops;
                ports = start;
            }

            public bool MorePorts()
            {
                return (stop - ports) >= 0;
            }
            public int NextPort()
            {
                if (MorePorts())
                {
                    return ports++;
                }
                return -1;
            }
        }

    }  // Class PortScanner2
}
