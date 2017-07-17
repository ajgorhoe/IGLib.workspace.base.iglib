using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;

namespace IG.Forms
{


    /// <summary>Background worker that can be aborted.
    /// <para>WARNING: Calling <see cref="Abort()"/> method disposes the background worker. Therefore, you should define the 
    /// <see cref="RunWorkerAborted"/> event that re-creates the background worker and sets the event handlers on it.</para></summary>
    public class AbortableBackgroundWorker : BackgroundWorker
    {

        public AbortableBackgroundWorker() : base()
        { _doworkEvents = new DoWorkEventArgs(null); }

        private Thread _workerThread;

        protected DoWorkEventArgs _doworkEvents;

        protected override void OnDoWork(DoWorkEventArgs e)
        {
            _doworkEvents = e;  // Set the event args such that result can be set by the Abort() method.
            _workerThread = Thread.CurrentThread;
            try
            {
                base.OnDoWork(e);
            }
            catch (ThreadAbortException)
            {
                e.Cancel = true; //We must set Cancel property to true!
                Thread.ResetAbort(); //Prevents ThreadAbortException propagation
            }
        }


        /// <summary>Aborts the current job, if a job is currently perrforming. Otherwise, nothing is done.
        /// <para>The <see cref="RunWorkerAborted"/> event is raised, with event arguments sett to the object that was passed to the <see cref="DoWork"/></para>
        /// <para>This method disposes the current background worker. Therefore, <see cref="RunWorkerAborted"/> should be defined that re-creates
        /// tha background worker, sets all the event handlers on it appropriately, and assigns the newly created background worker to the appropriate
        /// variable that was before holding the old one.</para></summary>
        public void Abort()
        {

            if (_workerThread != null)
            {
                _workerThread.Abort();
                _workerThread = null;
            }
        }

        /// <summary>Raises the DoAbort event.</summary>
        protected void OnAbort()
        {
            RunWorkerAborted.Invoke(this, _doworkEvents);
        }

        /// <summary>Event that fires when <see cref="Abort"/> is called on the current object.</summary>
        public event DoWorkEventHandler RunWorkerAborted;

    }


}
