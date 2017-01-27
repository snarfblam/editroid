using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Collections;

namespace Editroid
{
    static class RenderThread
    {
        static Queue<RenderTask> generalTasks = new Queue<RenderTask>();
        static Queue<RenderTask> priorityTasks = new Queue<RenderTask>();
        static object taskLock = new object();

        static Queue<RenderTask> finishedTasks = new Queue<RenderTask>();
        static object completedLock = new object();



        static BackgroundWorker worker = new BackgroundWorker();

        static RenderThread() {
            worker.WorkerSupportsCancellation = true;
            worker.DoWork += new DoWorkEventHandler(DoWork);
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(OnWorkerFinished);
            worker.ProgressChanged += new ProgressChangedEventHandler(worker_ProgressChanged);
            worker.WorkerReportsProgress = true;
        }

        static void worker_ProgressChanged(object sender, ProgressChangedEventArgs e) {
            lock (completedLock) {
                RenderTask task = finishedTasks.Dequeue();
                task.RaiseTaskComplete();
            }
        }

        static void OnWorkerFinished(object sender, RunWorkerCompletedEventArgs e) {
            
        }

        static void DoWork(object sender, DoWorkEventArgs e) {
            bool tasksRemaining;
            lock (taskLock) {
                tasksRemaining = (generalTasks.Count > 0 || priorityTasks.Count > 0);
            }

            while (tasksRemaining) {
                RenderTask task;
                lock (taskLock) {
                    if (priorityTasks.Count > 0)
                        task = priorityTasks.Dequeue();
                    else
                        task = generalTasks.Dequeue();
                }

                task.DoWork();
                lock (completedLock) {
                    finishedTasks.Enqueue(task);
                }
                worker.ReportProgress(0);

                lock (taskLock) {
                    tasksRemaining = (generalTasks.Count > 0 || priorityTasks.Count > 0);
                }
            }
        }


        /// <summary>This method must only be called from the GUI thread.</summary>
        /// <param name="task"></param>
        /// <param name="priority"></param>
        static public void QueueTask(RenderTask task, bool priority) {
            lock (taskLock) {
                if (priority)
                    priorityTasks.Enqueue(task);
                else
                    generalTasks.Enqueue(task);
            }

            if (!worker.IsBusy)
                worker.RunWorkerAsync();
        }
    }

    public abstract class RenderTask{
        public abstract void DoWork();
        internal void RaiseTaskComplete() {
            if (TaskComplete != null)
                TaskComplete(this, new EventArgs());
        }
        public event EventHandler TaskComplete;
    }
}
