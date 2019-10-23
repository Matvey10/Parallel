using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ParallelLaba1
{
    public class TaskManager
    {
        private ConcurrentQueue<Action> _actionsQueue;
        private readonly AutoResetEvent _waitHandler = new AutoResetEvent(false);
        //private readonly Thread _mainThread;

        private List<WorkThread> _idleThreads;
        private List<WorkThread> _workingThreads;

        private object _locker = new object();

        // Объявляем делегат
        public delegate void TaskManagerWorkDoneHandler();
        // Событие, возникающее при выводе денег
        public event TaskManagerWorkDoneHandler TaskManagerWorkDone;

        public TaskManager(int threadsNum)
        {
            _idleThreads = new List<WorkThread>(threadsNum);
            _workingThreads = new List<WorkThread>(threadsNum);

            for (int i = 0; i < threadsNum; i++)
            {
                var wt = new WorkThread();
                wt.WorkDone += SubThreadWorkDoneHandler;
                _idleThreads.Add(wt);
            }
        }
        public List<WorkThread> getThreads()
        {
            return _idleThreads;
        }

        private void SubThreadWorkDoneHandler(WorkThread workThread)
        {
            if (_actionsQueue.TryDequeue(out Action result))
            {
                
                workThread.Do(result);
                return;
            }

            lock (_locker)
            {
                _workingThreads.Remove(workThread);
                _idleThreads.Add(workThread);
                if (_actionsQueue.IsEmpty && _workingThreads.Count == 0)
                {
                    //TaskManagerWorkDone?.Invoke();//добавил цикл по закрытию потоков 
                    foreach (var thrd in _idleThreads)
                    {
                        thrd.Terminate();
                    }
                    TaskManagerWorkDone?.Invoke();
                }

            }
        }

        public void AddTasks (ref ConcurrentQueue<Action> _actionsQueue)
        {
            //lock (_locker)
            //{
                this._actionsQueue = _actionsQueue;
                if (this._actionsQueue.TryDequeue(out Action result))
                {
                    if (_idleThreads.Count > 0)
                    {
                        WorkThread wt = _idleThreads[_idleThreads.Count - 1];
                        _idleThreads.Remove(wt);
                        _workingThreads.Add(wt);
                        wt.Do(result);
                    }
                }
            //}
            
        }
        public void AddTask(Action action)
        {
            lock (_locker)
            {
                if (_idleThreads.Count > 0)
                {
                    WorkThread wt = _idleThreads[_idleThreads.Count - 1];
                    _idleThreads.Remove(wt);
                    _workingThreads.Add(wt);
                    wt.Do(action);
                }
                else
                {
                    _actionsQueue.Enqueue(action);
                }

            }
        }
    }
}