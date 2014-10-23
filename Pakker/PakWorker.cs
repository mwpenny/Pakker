//PakWorker.cs

using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Pakker
{
    /// <summary>
    /// A wrapper for the <see cref="BackgroundWorker"/> class that allows
    /// asynchronous reading and writing of packed archives.
    /// </summary>
    public class PakWorker
    {
        private static bool IOLocked; //Flag to lock IO operations when packing/unpacking
        public static bool isIOLocked() { return IOLocked; }
        public bool isCancelling() { return worker.CancellationPending; }

        public delegate void ProgressCallback(int progress); //Delegate for progress bar update
        public delegate bool CancelCallback(); //Delegate for checking if operations have been cancelled
        public delegate void PakWorkerWork(CancelCallback cancelled, ProgressCallback progress); //For general archive methods
        public delegate void PakWorkerArgWork(object arg, CancelCallback cancelled, ProgressCallback progress); //For methods requiring an argument
        public delegate void DoneCallback(bool success); //For when the worker finishes

        private BackgroundWorker worker;
        private object arg;
        private ToolStripProgressBar pb;
        private DoneCallback callback;

        /// <param name="pb">The <see cref="ToolStripProgressBar"/> to use for progress updating.</param>
        /// <param name="callback">The function to execute after the <see cref="BackgroundWorker"/> completes.</param>
        public PakWorker(ToolStripProgressBar pb, DoneCallback callback)
        {
            this.pb = pb;
            this.callback = callback;
            IOLocked = false;
            worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;

            //Add event handlers
            worker.DoWork += new DoWorkEventHandler(worker_DoWork);
            worker.ProgressChanged += new ProgressChangedEventHandler(worker_ProgressChanged);
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
        }

        /// <summary>
        /// Runs a <see cref="PakWorkerWork"/> method asynchronously.
        /// </summary>
        /// <param name="workMethod">The <see cref="PakWorkerWork"/> method to run.</param>
        public void runMethodAsync(PakWorkerWork workMethod)
        {
            startWork(workMethod);
        }

        /// <summary>
        /// Runs a <see cref="PakWorkerArgWork"/> method asynchronously.
        /// </summary>
        /// <param name="workMethod">The <see cref="PakWorkerArgWork"/> method to run.</param>
        /// <param name="arg">The argument to pass to the <see cref="PakWorkerArgWork"/> method.</param>
        public void runArgMethodAsync(PakWorkerArgWork workMethod, object arg)
        {
            this.arg = arg;
            startWork(workMethod);
        }

        private void startWork(object workMethod)
        {
            IOLocked = true;
            worker.RunWorkerAsync(workMethod);
        }

        /// <summary>
        /// Calculates a percentage using <see cref="num1"/> of <see cref="num2"/>.
        /// </summary>
        /// <param name="num1">The number of steps elapsed.</param>
        /// <param name="num2">The total number of steps.</param>
        /// <returns></returns>
        public static int calcPercent(int num1, int num2)
        {
            double p = num1 / (double)num2;
            return (int)(p * 100);
        }

        /// <summary>
        /// Cancels the <see cref="BackgroundWorker"/>
        /// </summary>
        public void cancel()
        {
            if (worker.IsBusy)
                worker.CancelAsync();
        }

        private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.pb.Value = e.ProgressPercentage;
        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            IOLocked = false;
            if (e.Error != null) //Magic exception catchall
                MessageBox.Show("Error! " + e.Error.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            this.pb.Value = 0;
            this.callback(e.Error == null);
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = (BackgroundWorker)sender;
            
            //The method to run takes an argument
            if (this.arg != null)
            {
                ((PakWorkerArgWork)e.Argument)(this.arg, isCancelling, worker.ReportProgress);
                this.arg = null;
            }
            else
                ((PakWorkerWork)e.Argument)(isCancelling, worker.ReportProgress);
        }
    }
}
