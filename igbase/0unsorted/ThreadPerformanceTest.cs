// Copyright (c) Igor Grešovnik (2009), IGLib license; http://www2.arnes.si/~ljc3m2/igor/ioptlib/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Threading;

namespace IG.Lib
{

    /// <summary>Used to measure performance of the currend thread.
    /// A standard unit operation is provided for which number of executions per second is
    /// measured.
    /// Beside number of unit operations per second, the ratio between the CPU time and the clock
    /// time is calculated, which gives the feeling of how much the processor on which the thread
    /// executes is loaded.
    /// Measurements can be performed by specifying the requested number of cycles, but also by
    /// specifying the requested time in seconds.</summary>
    public class ThreadPerformanceTest
    {

        public ThreadPerformanceTest()
        {
            Last.NumCycles = 0;
            Last.CyclesPerSecond = 0.0;
            Last.PerformanceRatio = 0.0;
        }

        /// <summary>Number of inner iterations.
        /// This number is chosen such that there are over 1000 outer cycles per second on medium speed
        /// modern processors.</summary>
        public const int NumInner = 10000;

        /// <summary>A reference number of cycles per second.
        /// This can be used for orientation of how many cycles to request.</summary>
        public const double RefCyclesPerSecond = 1600;

        /// <summary>Minimial number of cycles. Test never executes with less cycles because
        /// of accuracy of results.</summary>
        public const int MinCycles = 20;

        private object _lock = new Object();

        protected object Lock
        { get { return _lock; } }


        /// <summary>Performance data of the last measurement performed.</summary>
        public ThreadPerformanceData Last;

        List<ThreadPerformanceData> _data = new List<ThreadPerformanceData>();

        /// <summary>List of data aboud the performed tests.</summary>
        public List<ThreadPerformanceData> Data
        {
            get { return _data; }
        }

        /// <summary>Stores the last performance data.</summary>
        protected void StoreLast()
        {
            Data.Add(Last);
        }
        

        /// <summary>A standard unit operation that is repeated in order to measure performance.</summary>
        public static void StandardCycle()
        {
            double a, b, c, d;
            int i;
            for (i=0; i < NumInner; ++i)
            {
                a = Math.Exp(0.1);
                b = Math.Sin(a) + Math.Cos(a) + 0.1 * Math.Sqrt(a * a);
                c = Math.Sqrt(a * b) + Math.Cos(a + b);
                d = (a + b) * (b + c) * (c - a) * Math.Sqrt(a * b * c);
            }
        }

        /// <summary>Execute performance test with specified number of standard cycles.
        /// WARNING: 
        /// Usually the TestPerformance should be used which prescribes the approximate time
        /// used for the test.</summary>
        /// <param name="numCycles">Number of standard cycles to be performed.</param>
        /// <param name="data">Output structure that returns the performance data.</param>
        public void TestPerformanceNum(int numCycles, out ThreadPerformanceData data)
        {
            if (numCycles < MinCycles)
                numCycles = MinCycles;
            StopWatch t = new StopWatch();
            int i;
            t.Start();
            for (i = 0; i < numCycles; ++i)
                StandardCycle();
            t.Stop();
            Last.Time = DateTime.Now;
            Last.NumCycles = numCycles;
            Last.CyclesPerSecond = (double)numCycles / t.TotalTime;
            Last.PerformanceRatio = t.TotalCpuTime / t.Time;
            Last.TotalTime = t.TotalTime;
            Last.NumIterations = 0;
            StoreLast();
            data = Last;
        }

        /// <summary>Execute performance test with specified number of standard cycles.
        /// WARNING: 
        /// Usually the TestPerformance should be used which prescribes the approximate time
        /// used for the test.</summary>
        /// <param name="numCycles">Number of standard cycles to be performed performed.</param>
        /// <param name="cyclesPerSec">Returns number of cycles per second, which is the relevant
        /// performance measure for the current thread.</param>
        /// <param name="performanceRatio">Returns ratio between CPU time and clock time spen for the test.
        /// This is a measure of how much the CPU is loaded (smaller the value, more it is loaded).</param>
        public void TestPerformanceNum(int numCycles, out double cyclesPerSec,
            out double performanceRatio)
        {
            ThreadPerformanceData data;
            TestPerformanceNum(numCycles, out data);
            cyclesPerSec = data.CyclesPerSecond;
            performanceRatio = data.PerformanceRatio;
        }

        /// <summary>Execute performance test with specified number of standard cycles, and
        /// returns the measured number of standard cycles performed per second (which is the
        /// relevant performance measure).
        /// WARNING: 
        /// Usually the TestPerformance should be used which prescribes the approximate time
        /// used for the test.</summary>
        /// <param name="numCycles">Number of standard cycles to be performed.</param>
        public double TestPerformanceNum(int numCycles)
        {
            ThreadPerformanceData data;
            TestPerformanceNum(numCycles, out data);
            return data.CyclesPerSecond;
        }


        /// <summary>Execute performance test with specified measurement duration.</summary>
        /// <param name="requestedTime">Requested (approximate) duration of the test.</param>
        /// <param name="data">Output structure that returns the performance data.</param>
        public void TestPerformance(double testDuration, out ThreadPerformanceData data)
        {
            int numCycles = 0;
            int numIt = 0;
            double cyclesLeft = 0;  // estimated number of cycles yet to be performed in order to reach the requested measurement time
            double prevCycles = 0;  // previous number of cycles
            double timeLeft = testDuration;
            double timePortion = 0.0;
            double CPS = RefCyclesPerSecond;  // current cycles per second
            double cyclesFactor = 0.05;
            if (Last.NumCycles > 0)
            {
                // Measurement has already been performed, so we have an estimation of cycles per second:
                CPS = Last.CyclesPerSecond;
                cyclesFactor = 0.2;  // still we consider first estimation very rough:
            } else
            {
                // We don't have any prior estimation of performcne, so we take the reference number:
                CPS = RefCyclesPerSecond;
                cyclesFactor = 0.05;  // since estimation is very bad
            }
            cyclesLeft = CPS * timeLeft;
            cyclesLeft *= cyclesFactor;
            if (cyclesLeft < MinCycles)
                cyclesLeft = MinCycles;

            StopWatch t = new StopWatch();
            while (timePortion < 0.98)
            {
                ++numIt;
                prevCycles = cyclesLeft;
                //Console.WriteLine("Iteration " + numIt + ": ");
                //Console.WriteLine(" timePortion: " + timePortion + " timeLeft: " + timeLeft);
                //Console.WriteLine(" CPS: " + CPS + " cyclesLeft: " + cyclesLeft);

                t.Start();
                for (int i = 0; i <= cyclesLeft; ++i)
                {
                    StandardCycle();
                    ++numCycles;
                }
                t.Stop();
                if (t.TotalTime == 0)
                    CPS = RefCyclesPerSecond;
                else
                    CPS = numCycles / t.TotalTime;  // improve estimation of cycles per second
                timeLeft = testDuration - t.TotalTime;
                timePortion = 1.0 - (timeLeft / testDuration);  // portion of time spent w.r. prescribed duration
                cyclesLeft = CPS * timeLeft;  // estimation of number of cycles needed to reach the requested duration
                cyclesFactor = 1.0;
                if (timePortion < 0.05)
                    cyclesFactor = 0.1; // since we have a very rough estimation
                else if (timePortion < 0.1)
                    cyclesFactor = 0.2;
                else if (timePortion < 0.5)
                    cyclesFactor = 0.6;
                else if (timePortion < 0.8)
                    cyclesFactor = 0.95;
                cyclesLeft *= cyclesFactor;
                if (cyclesLeft < MinCycles)
                    cyclesLeft = MinCycles;
                if (t.TotalTime <= 0.005)
                    cyclesLeft = prevCycles * 1.5;  // safety: time interval too small to measure
            }
            Last.Time = DateTime.Now;
            Last.NumCycles = numCycles;
            Last.CyclesPerSecond = (double)numCycles / t.TotalTime;
            Last.PerformanceRatio = t.TotalCpuTime / t.Time;
            Last.TotalTime = t.TotalTime;
            Last.NumIterations = numIt;
            StoreLast();
            data = Last;
        }

        /// <summary>Execute performance test with specified measurement duration.</summary>
        /// <param name="requestedTime">Requested (approximate) duration of the test.</param>
        /// <param name="cyclesPerSec">Returns number of cycles per second, which is the relevant
        /// performance measure for the current thread.</param>
        /// <param name="performanceRatio">Returns ratio between CPU time and clock time spen for the test.
        /// This is a measure of how much the CPU is loaded (smaller the value, more it is loaded).</param>
        public void TestPerformance(double testDuration, out double cyclesPerSec,
            out double performanceRatio)
        {
            ThreadPerformanceData data;
            TestPerformance(testDuration, out data);
            cyclesPerSec = data.CyclesPerSecond;
            performanceRatio = data.PerformanceRatio;
        }

        /// <summary>Execute performance test with specified measurement duration, and
        /// returns the measured number of standard cycles performed per second (which is the
        /// relevant performance measure).</summary>
        /// <param name="requestedTime">Requested (approximate) duration of the test.</param>
        public double TestPerformance(double testDuration)
        {
            ThreadPerformanceData data;
            TestPerformance(testDuration, out data);
            return data.CyclesPerSecond;
        }


        protected static bool _performLoad = false;

        protected static int _numLoadThreads = 0;

        /// <summary>Returns the current number of alive loading threads performing full CPU load.</summary>
        public static int NumLoadThreds
        {
            get { return _numLoadThreads; }
            protected set { _numLoadThreads = value; }
        }

        /// <summary>Specifies whether load should be performed. 
        /// Setting this to false makes eventual threads to exit.</summary>
        public static bool PerformLoad
        {
            get { return _performLoad; }
            set {
                _performLoad = value;
                // Suspend execution a little bit; this guarantees that in this time
                // nobody from the current thread will set PerformLoad to true again, 
                // which makes possible for the threads to see the change.
                if (value == false)
                    Thread.Sleep(500);
            }
        }

        /// <summary>Starts the specified number of new load threads, each of which causes a full load
        /// to a CPU core until it stops.
        /// IMPORTANT:
        /// The load threads are switched off by setting ThreadPerformanceTest.PerformLoad to false.
        /// It can happen that this must be repeated several times to take effect.</summary>
        /// <param name="numThreads"></param>
        public static void CreateThreadLoads(int numThreads)
        {
            PerformLoad = true;
            for (int i = 0; i < numThreads; ++i)
            {
                Thread t = new Thread(ThreadLoadFunction);
                t.IsBackground = true;
                t.Start();
            }
        }

        /// <summary>Performed by load threads.</summary>
        protected static void ThreadLoadFunction()
        {
            Console.WriteLine("Load thread started, ID = " + Thread.CurrentThread.ManagedThreadId);
            ++NumLoadThreds;
            double a = 0.1, b = 0.2, c = 0.1;
            int numit = 0;
            int numprint = 100000;
            
            //TODO: 
            // Check why load thread do not stop to perform when PerformLoad is set to false!

            while (PerformLoad)
            {
                if (!PerformLoad)
                {
                    --NumLoadThreds;
                    Console.WriteLine("Load thread stopped, ID = " + Thread.CurrentThread.ManagedThreadId
                        + "; load threads left: " + NumLoadThreds);
                    return;
                }
                ++numit;
                if (numit == numprint)
                {
                    numit = 0;
                    //Console.WriteLine("Load thread " + Thread.CurrentThread.ManagedThreadId
                    //    + ", PerformLoad = " + PerformLoad);
                }
                a = Math.Cos(b + c);
                c = Math.Sqrt(a + b);
                b = Math.Sin(0.1 + b);
                a = Math.Sqrt(a + b + c);
                b = Math.Cos(a + 2 * c);
            }
            
        }


        public override string ToString()
        {
            lock(_lock)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Performance testes: ");
                sb.AppendLine("  InnerCycles: " + NumInner 
                    + ", MinCycles: " + MinCycles 
                    + ", RefCyclesPerSecond: " + RefCyclesPerSecond);
                if (Data.Count < 1)
                    sb.AppendLine("  Last: " + Last.ToString());
                for (int i = 0; i < Data.Count; ++i)
                    sb.AppendLine("  " + Data[i]);
                return sb.ToString();
            }
        }


        /// <summary>Examples and tests for this class.</summary>
        public static void Example()
        {

            Console.WriteLine("\n\nThreadPerformanceTest.Example():\n");
            ThreadPerformanceTest test = new ThreadPerformanceTest();
            int numCycles = 1000;;
            Console.WriteLine("\nInitial state of performance test: \n"
                + test.ToString());

            numCycles = 20;
            test.TestPerformanceNum(numCycles);
            Console.WriteLine("\nState after measuring " + numCycles + " cycles: \n"
                + test.ToString());

            numCycles = 100;
            test.TestPerformanceNum(numCycles);
            Console.WriteLine("\nState after measuring " + numCycles + " cycles: \n"
                + test.ToString());

            numCycles = 1000;
            test.TestPerformanceNum(numCycles);
            Console.WriteLine("\nState after measuring " + numCycles + " cycles: \n"
                + test.ToString());

            Console.WriteLine("\n\n\nPerformance measured over requested time period: \n");
            test = new ThreadPerformanceTest();
            double time;

            time = 0.01;
            test = new ThreadPerformanceTest();
            Console.WriteLine("\nRequested duration time: " + time + " s." );
            Console.WriteLine("Performing next test...");
            test.TestPerformance(time);
            Console.WriteLine("Performing next test...");
            test.TestPerformance(time);
            Console.WriteLine("Performing next test...");
            test.TestPerformance(time);
            Console.WriteLine("State after measuring: \n"
                + test.ToString());


            time = 0.1;
            test = new ThreadPerformanceTest();
            Console.WriteLine("\nRequested duration time: " + time + " s.");
            Console.WriteLine("Performing next test...");
            test.TestPerformance(time);
            Console.WriteLine("Performing next test...");
            test.TestPerformance(time);
            Console.WriteLine("Performing next test...");
            test.TestPerformance(time);
            Console.WriteLine("State after measuring: \n"
                + test.ToString());

            time = 0.5;
            test = new ThreadPerformanceTest();
            Console.WriteLine("\nRequested duration time: " + time + " s.");
            Console.WriteLine("Performing next test...");
            test.TestPerformance(time);
            Console.WriteLine("Performing next test...");
            test.TestPerformance(time);
            Console.WriteLine("Performing next test...");
            test.TestPerformance(time);
            Console.WriteLine("State after measuring: \n"
                + test.ToString());

            time = 1;
            test = new ThreadPerformanceTest();
            Console.WriteLine("\nRequested duration time: " + time + " s.");
            Console.WriteLine("Performing next test...");
            test.TestPerformance(time);
            Console.WriteLine("Performing next test...");
            test.TestPerformance(time);
            Console.WriteLine("Performing next test...");
            test.TestPerformance(time);
            Console.WriteLine("State after measuring: \n"
                + test.ToString());


            Console.WriteLine("\n\nPerformance tests with loaded CPU: ");
            time = 1;  // testing time

            Console.WriteLine("\nTesting...");
            test = new ThreadPerformanceTest();
            test.TestPerformance(time);
            Console.WriteLine("Performance results, number of loading threads = " 
                + ThreadPerformanceTest.NumLoadThreds + ": \n" + test.ToString());

            Console.WriteLine("\nTesting...");
            ThreadPerformanceTest.CreateThreadLoads(3);
            test = new ThreadPerformanceTest();
            test.TestPerformance(time);
            Console.WriteLine("\nPerformance results, number of loading threads = "
                + ThreadPerformanceTest.NumLoadThreds + ": \n" + test.ToString());

            Console.WriteLine("\nTesting...");
            ThreadPerformanceTest.CreateThreadLoads(3);
            test = new ThreadPerformanceTest();
            test.TestPerformance(time);
            Console.WriteLine("Performance results, number of loading threads = "
                + ThreadPerformanceTest.NumLoadThreds + ": \n" + test.ToString());

            // Now switch off loading threads:
            ThreadPerformanceTest.PerformLoad = false;
            Console.WriteLine("PerformLoad: " + ThreadPerformanceTest.PerformLoad);
            Console.WriteLine("\nTesting...");
            test = new ThreadPerformanceTest();
            test.TestPerformance(time);
            Console.WriteLine("Performance results, number of loading threads = "
                + ThreadPerformanceTest.NumLoadThreds + ": \n" + test.ToString());

            Console.WriteLine("\nNumber of loading threads: " + ThreadPerformanceTest.NumLoadThreds);
            Console.WriteLine("PerformLoad: " + ThreadPerformanceTest.PerformLoad);
            Thread.Sleep(2000);
            Console.WriteLine("\nNumber of loading threads: " + ThreadPerformanceTest.NumLoadThreds);
            Console.WriteLine("PerformLoad: " + ThreadPerformanceTest.PerformLoad);
            Thread.Sleep(2000);
            Console.WriteLine("\nNumber of loading threads: " + ThreadPerformanceTest.NumLoadThreds);
            Console.WriteLine("PerformLoad: " + ThreadPerformanceTest.PerformLoad);
            Thread.Sleep(2000);
            Console.WriteLine("\nNumber of loading threads: " + ThreadPerformanceTest.NumLoadThreds);
            Console.WriteLine("\nTesting...");
            Console.WriteLine("PerformLoad: " + ThreadPerformanceTest.PerformLoad);
            test = new ThreadPerformanceTest();
            test.TestPerformance(time);
            Console.WriteLine("Performance results, number of loading threads = "
                + ThreadPerformanceTest.NumLoadThreds + ": \n" + test.ToString());

        }

    }  // class ThreadPerformanceTest


    /// <summary>Stores results of performance test.</summary>
    public struct ThreadPerformanceData
    {
        /// <summary>Time at which data has been acquired.</summary>
        public DateTime Time;

        /// <summary>Number of standard cycles executed in the test.</summary>
        public double NumCycles;

        /// <summary>Number of standard cycles per second - the ultimate performance measure.
        /// Standard cycle is executed by ThreadPerformanceTest.StandardCycle().</summary>
        public double CyclesPerSecond;

        /// <summary>Ratio between the CPU time and wall clock time spent for the test.
        /// Indicates how much the CPU is loaded (the smaller the value, teh more it is loaded).
        /// Should be between 0 and 1.</summary>
        public double PerformanceRatio;

        /// <summary>Total wallclock time spent for the test.</summary>
        public double TotalTime;

        /// <summary>Number of iterations necessary to hit the requested time interval.</summary>
        public int NumIterations;


        /// <summary>Copies data from another ThreadPerformanceData structure.</summary>
        /// <param name="data"></param>
        public void CopyFrom(ThreadPerformanceData data)
        {
            this.Time = data.Time;
            this.NumCycles = data.NumCycles;
            this.CyclesPerSecond = data.CyclesPerSecond;
            this.PerformanceRatio = data.PerformanceRatio;
            this.TotalTime = data.TotalTime;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append('[');
            sb.Append(Time.ToString() + ": ");
            sb.Append("Cycles: " + NumCycles + " ");
            sb.Append("kC/s: " + CyclesPerSecond / 1000.0 + " ");
            sb.Append("R: " + PerformanceRatio + " ");
            sb.Append("t: " + TotalTime + "");
            sb.Append(']');
            return sb.ToString();
        }
    }  // struct ThreadPerformanceData



}
