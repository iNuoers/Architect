using System;
using System.Threading;
using System.Collections.Generic;

using APP.CommonLib.Log;

namespace APP.CommonLib.Utils
{
    /// <summary>
    /// 线程池委托
    /// </summary>
    /// <param name="o">对象</param>
    public delegate void ThreadBoxHandler(Object o);

    /// <summary>
    /// 泛型线程池委托
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    /// <param name="o">对象</param>
    public delegate void ThreadBoxHandler<T>(T o);

    /// <summary>
    /// 泛型守护线程委托
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    /// <param name="o">对象</param>
    public delegate void KeeperThreadHandler<T>(T o);

    /// <summary>
    /// 泛型获取对象接口
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    /// <returns>对象</returns>
    public delegate T PopItemMethod<T>();

    /// <summary>
    /// 线程池抽象类
    /// 
    /// 可以通过静态方法来使用默认线程池、
    /// 专用的消费者模式线程池
    /// </summary>
    public abstract class ThreadBox
    {
        #region 容器

        /// <summary>
        /// 线程池容器
        /// </summary>
        private static Dictionary<string, Box> dicBox = new Dictionary<string, Box>();

        /// <summary>
        /// 守护线程容器
        /// </summary>
        private static Dictionary<string, KThread> dicThread = new Dictionary<string, KThread>();

        #endregion

        #region 通用线程池相关方法

        private static Box commonThreadbox = CreateCommonThreadbox();

        private static Box CreateCommonThreadbox()
        {
            commonThreadboxQueue = new Queue<KeyValuePair<object, ThreadBoxHandler>>();
            Box cBox = new Box(1);

            QueueJob<KeyValuePair<object, ThreadBoxHandler>> job = new QueueJob<KeyValuePair<object, ThreadBoxHandler>>();
            job.queue = commonThreadboxQueue;
            job.handler = commonThreadboxHandler;
            job.isStop = true;
            job.sleepTime = 50;
            cBox.Start(job);
            return cBox;
        }

        private static Queue<KeyValuePair<object, ThreadBoxHandler>> commonThreadboxQueue;

        private static void commonThreadboxHandler(KeyValuePair<object, ThreadBoxHandler> kvp)
        {
            kvp.Value(kvp.Key);
        }

        /// <summary>
        /// 在通用线程池内加入待处理的方法
        /// </summary>
        /// <param name="handler">处理方法</param>
        /// <param name="context">上下文对象</param>
        public static void EnqueueHandle(ThreadBoxHandler handler, object context)
        {
            lock (commonThreadboxQueue)
                commonThreadboxQueue.Enqueue(new KeyValuePair<object, ThreadBoxHandler>(context, handler));
        }

        public static void ChangeCommonThreadBoxSize(int threadNum)
        {
            if (threadNum <= 0)
                throw new ArgumentException("thread num error");

            commonThreadbox.ChangeSize(threadNum);
        }

        #endregion

        #region 线程关闭信号

        /// <summary>
        /// 检测线程是否关闭 true是关闭
        /// </summary>
        /// <returns></returns>
        public static bool CheckThreadStop()
        {
            return !Interlocked.Equals(IsStop, 0);
        }

        /// <summary>
        /// 关闭线程池所有线程，而且不可再开启。 用于终止服务
        /// </summary>
        public static void SetStop()
        {
            Interlocked.Add(ref IsStop, 1);
        }

        /// <summary>
        /// 大于0的时候停止
        /// </summary>
        private static int IsStop = 0;

        #endregion

        #region 构造方法

        /// <summary>
        /// 是否存在线程池
        /// </summary>
        /// <param name="threadboxName">线程池名称</param>
        /// <returns></returns>
        public static bool ContainThreadBox(string threadboxName)
        {
            lock (dicBox)
                return dicBox.ContainsKey(threadboxName);
        }

        /// <summary>
        /// 是否存在守护线程
        /// </summary>
        /// <param name="keeperThreadName">守护线程名称</param>
        /// <returns></returns>
        public static bool ContainKeeperThread(string keeperThreadName)
        {
            lock (dicThread)
                return dicThread.ContainsKey(keeperThreadName);
        }

        /// <summary>
        /// 创建一个守护线程定时处理逻辑
        /// </summary>
        /// <param name="keeperThreadName">守护线程名称</param>
        /// <param name="handler">处理方法</param>
        /// <param name="o">相关对象</param>
        /// <param name="milliumsecondSleepTime">方法执行完成的休息时间（毫秒），默认为1000毫秒</param>
        public static void CreateKeeperThread<T>(string keeperThreadName, KeeperThreadHandler<T> handler, T o, int milliumsecondSleepTime = 1000)
        {
            if (keeperThreadName == "" || handler == null)
                throw new ArgumentException();

            KeeperJob<T> j = new KeeperJob<T>();
            j.handler = handler;
            j.obj = o;
            j.sleepTime = milliumsecondSleepTime;
            KThread t = new KThread();
            lock (dicThread)
            {
                if (dicThread.ContainsKey(keeperThreadName))
                    throw new ArgumentException("keeperThreadName exist");
                dicThread.Add(keeperThreadName, t);
            }

            t.Start(j);
        }

        //public static void CreateKeeperThread(string keeperThreadName, KeeperThreadHandler<null> handler, int milliumsecondSleepTime = 1000)
        //{

        //}

        /// <summary>
        /// 创建一个守护线程定时处理逻辑
        /// </summary>
        /// <param name="keeperThreadName">守护线程名称</param>
        /// <param name="handler">处理方法</param>
        /// <param name="o">相关对象</param>
        /// <param name="milliumsecondSleepTime">方法执行完成的休息时间（毫秒），默认为1000毫秒</param>
        public static void CreateKeeperThread(string keeperThreadName, KeeperThreadHandler<object> handler, object o, int milliumsecondSleepTime = 1000)
        {
            if (keeperThreadName == "" || handler == null)
                throw new ArgumentException();

            KeeperJob<object> j = new KeeperJob<object>();
            j.handler = handler;
            j.obj = o;
            j.sleepTime = milliumsecondSleepTime;
            KThread t = new KThread();
            lock (dicThread)
            {
                if (dicThread.ContainsKey(keeperThreadName))
                    throw new ArgumentException("keeperThreadName exist");
                dicThread.Add(keeperThreadName, t);
            }

            t.Start(j);
        }

        public static void WakeupKeeperThread(string KeeperThreadName)
        {
            if (KeeperThreadName == "")
                throw new ArgumentException();

            KThread b = null;
            lock (dicThread)
            {
                if (!dicThread.ContainsKey(KeeperThreadName))
                    return;

                b = dicThread[KeeperThreadName];
            }

            b.Wakeup();
        }

        public static void DeleteKeeperThread(string KeeperThreadName)
        {
            if (KeeperThreadName == "")
                throw new ArgumentException();

            KThread b = null;
            lock (dicThread)
            {
                if (!dicThread.ContainsKey(KeeperThreadName))
                    return;

                b = dicThread[KeeperThreadName];
                dicThread.Remove(KeeperThreadName);
            }

            b.tStop = true;
        }

        public static void CreateQueueConsumerThreadBox<T>(string threadboxName, int threadNum, ThreadBoxHandler<T> handler, PopItemMethod<T> method, int milliumsecondSleepTime = 100, bool isStopOnEmptyQueue = false)
        {
            if (threadboxName == "" || threadNum <= 0 || handler == null || method == null)
                throw new ArgumentException();


            Box b = null;
            lock (dicBox)
            {
                if (dicBox.ContainsKey(threadboxName))
                    throw new ArgumentException("threadboxName exist");
                b = new Box(threadNum);
                dicBox.Add(threadboxName, b);
            }
            MethodJob<T> j = new MethodJob<T>();
            j.isStop = isStopOnEmptyQueue;
            j.sleepTime = milliumsecondSleepTime;
            j.method = method;
            j.handler = handler;
            //b.job = j;
            b.Start(j);
        }

        /// <summary>
        /// 创建一个线程池去处理一个队列容器
        /// </summary>
        /// <typeparam name="T">队列容器中对象的类型</typeparam>
        /// <param name="threadboxName">线程池名称</param>
        /// <param name="threadNum">线程池线程数量</param>
        /// <param name="handler">处理方法</param>
        /// <param name="queue">队列容器</param>
        /// <param name="milliumsecondSleepTime">当队列为空时休息时间（毫秒），默认为100毫秒</param>
        /// <param name="isStopOnEmptyQueue">是否当队列为空时才关闭，默认为FALSE</param>
        public static void CreateQueueConsumerThreadBox<T>(string threadboxName, int threadNum, ThreadBoxHandler<T> handler, Queue<T> queue, int milliumsecondSleepTime = 100, bool isStopOnEmptyQueue = false)
        {
            if (threadboxName == "" || threadNum <= 0 || handler == null || queue == null)
                throw new ArgumentException();


            Box b = null;
            lock (dicBox)
            {
                if (dicBox.ContainsKey(threadboxName))
                    throw new ArgumentException("threadboxName exist");
                b = new Box(threadNum);
                dicBox.Add(threadboxName, b);
            }
            QueueJob<T> j = new QueueJob<T>();
            j.isStop = isStopOnEmptyQueue;
            j.sleepTime = milliumsecondSleepTime;
            j.queue = queue;
            j.handler = handler;
            //b.job = j;
            b.Start(j);
        }


        public static void CreateQueueConsumerThreadBox<T>(string threadboxName, int coreThreadNum, int maxThreadNum, ThreadBoxHandler<T> handler, Queue<T> queue, int milliumsecondSleepTime = 100, bool isStopOnEmptyQueue = false)
        {
            if (threadboxName == "" || coreThreadNum <= 0 || maxThreadNum <= 0 || maxThreadNum < coreThreadNum || handler == null || queue == null)
                throw new ArgumentException();


            Box b = null;
            lock (dicBox)
            {
                if (dicBox.ContainsKey(threadboxName))
                    throw new ArgumentException("threadboxName exist");
                b = new Box(coreThreadNum, maxThreadNum);
                dicBox.Add(threadboxName, b);
            }
            QueueJob<T> j = new QueueJob<T>();
            j.isStop = isStopOnEmptyQueue;
            j.sleepTime = milliumsecondSleepTime;
            j.queue = queue;
            j.handler = handler;
            //b.job = j;
            b.Start(j);
        }


        /// <summary>
        /// 创建一个线程池去处理一个集合容器
        /// </summary>
        /// <typeparam name="T">队列容器中对象的类型</typeparam>
        /// <param name="threadboxName">线程池名称</param>
        /// <param name="threadNum">线程池线程数量</param>
        /// <param name="handler">处理方法</param>
        /// <param name="list">集合容器</param>
        /// <param name="milliumsecondSleepTime">当容器为空时休息时间（毫秒），默认为100毫秒</param>
        /// <param name="isStopOnEmptyQueue">是否当队列为空时才关闭，默认为FALSE</param>
        public static void CreateColletionConsumerThreadBox<T>(string threadboxName, int threadNum, ThreadBoxHandler<T> handler, ICollection<T> list, int milliumsecondSleepTime = 100, bool isStopOnEmptyQueue = false)
        {
            if (threadboxName == "" || threadNum <= 0 || handler == null || list == null)
                throw new ArgumentException();


            Box b = null;
            lock (dicBox)
            {
                if (dicBox.ContainsKey(threadboxName))
                    throw new ArgumentException("threadboxName exist");
                b = new Box(threadNum);
                dicBox.Add(threadboxName, b);
            }

            ListJob<T> j = new ListJob<T>();
            j.isStop = isStopOnEmptyQueue;
            j.sleepTime = milliumsecondSleepTime;
            j.list = list;
            j.handler = handler;
            b.Start(j);
        }

        /// <summary>
        /// 创建一个线程池去处理一个调用队列容器
        /// </summary>
        /// <typeparam name="T">队列容器中对象的类型</typeparam>
        /// <param name="threadboxName">线程池名称</param>
        /// <param name="threadNum">线程池线程数量</param>
        /// <param name="queue">调用队列容器</param>
        /// <param name="milliumsecondSleepTime">当队列为空时休息时间（毫秒），默认为100毫秒</param>
        /// <param name="isStopOnEmptyQueue">是否当队列为空时才关闭，默认为FALSE</param>
        public static void CreateQueueInvokeThreadBox<T>(string threadboxName, int threadNum, Queue<KeyValuePair<ThreadBoxHandler<T>, T>> queue, int milliumsecondSleepTime = 100, bool isStopOnEmptyQueue = false)
        {
            if (threadboxName == "" || threadNum <= 0 || queue == null)
                throw new ArgumentException();

            Box b = null;
            lock (dicBox)
            {
                if (dicBox.ContainsKey(threadboxName))
                    throw new ArgumentException("threadboxName exist");
                b = new Box(threadNum);
                dicBox.Add(threadboxName, b);
            }

            QueueJob<KeyValuePair<ThreadBoxHandler<T>, T>> job = new QueueJob<KeyValuePair<ThreadBoxHandler<T>, T>>();
            job.isStop = isStopOnEmptyQueue;
            job.sleepTime = milliumsecondSleepTime;
            job.queue = queue;
            job.handler = InvokeThreadboxHandler;
            b.Start(job);
        }

        private static void InvokeThreadboxHandler<T>(KeyValuePair<ThreadBoxHandler<T>, T> kvp)
        {
            kvp.Key(kvp.Value);
        }

        /// <summary>
        /// 修改线程池的线程数量
        /// </summary>
        /// <param name="threadboxName">线程池名称</param>
        /// <param name="threadNum">线程数量</param>
        public static void changeThreadBoxSize(string threadboxName, int threadNum)
        {
            if (threadboxName == "" || threadNum <= 0)
                throw new ArgumentException();

            Box b = null;

            lock (dicBox)
            {
                if (!dicBox.ContainsKey(threadboxName))
                    throw new ArgumentException("threadboxName not exist");
                b = dicBox[threadboxName];
            }

            b.ChangeSize(threadNum);
        }

        public static int getThreadBoxSize(string threadboxName)
        {
            if (threadboxName == "")
                throw new ArgumentException();

            Box b = null;

            lock (dicBox)
            {
                if (!dicBox.ContainsKey(threadboxName))
                    throw new ArgumentException("threadboxName not exist");
                b = dicBox[threadboxName];
            }

            return b.coreThreadNum;
        }

        public static void DeleteThreadBox(string threadboxName)
        {
            if (threadboxName == "")
                throw new ArgumentException();

            Box b = null;
            lock (dicBox)
            {
                if (!dicBox.ContainsKey(threadboxName))
                    return;

                b = dicBox[threadboxName];

                dicBox.Remove(threadboxName);
            }

            b.Close();
        }
        #endregion

        #region 线程池实现及作业类

        class KThread : IDisposable
        {
            private Thread t;
            private AutoResetEvent e;
            public void Start(Job j)
            {
                e = new AutoResetEvent(false);
                job = j;
                t = new Thread(HandleInvoke);
                t.IsBackground = true;
                t.Start();
            }
            private Job job { get; set; }
            private void HandleInvoke(object o)
            {
                while (true)
                    try
                    {
                        UpdateStopStatus();
                        while (!StopStatus)
                        {
                            try
                            {
                                job.Invoke();
                            }
                            catch (Exception ex)
                            {
                                //// **** 如果要记日志，就在这里记录
                                Logger.WriteLog("exception at threadbox", LogLevel.Error, ex);
                                Console.WriteLine(ex);
                            }
                            UpdateStopStatus();
                            if (StopStatus)
                                break;
                            if (job.canSleep)
                                e.WaitOne(job.sleepTime);
                            e.Reset();
                            //Thread.Sleep(job.sleepTime);
                        }
                        job = null;
                        Console.WriteLine("Thread Close");
                        return;
                    }
                    catch
                    {

                    }
            }
            private void UpdateStopStatus()
            {
                bool AllEndSignal = !Interlocked.Equals(ThreadBox.IsStop, 0);


                StopStatus = (!Interlocked.Equals(ThreadBox.IsStop, 0) && !(!job.canStop && job.isStop)) || tStop;
            }
            bool StopStatus = false;
            public bool tStop = false;

            public void Dispose()
            {
                t = null;
            }

            public void Wakeup()
            {
                e.Set();
            }
        }

        /// <summary>
        /// 线程池
        /// </summary>
        class Box
        {
            private Job job { get; set; }

            List<KThread> list = new List<KThread>();

            public int coreThreadNum = 0;
            public int maxThreadNum = 0;

            public Box(int threadNum)
                : this(threadNum, threadNum)
            { }

            public Box(int threadNum, int mThreadNum)
            {
                coreThreadNum = threadNum;
                maxThreadNum = mThreadNum;

                for (int i = 0; i < coreThreadNum; i++)
                {
                    KThread t = new KThread();
                    list.Add(t);
                }
            }

            public void Start(Job j)
            {
                job = j;
                foreach (KThread t in list)
                    t.Start(j);
            }

            public void ChangeSize(int threadNum)
            {
                bool isNew = false;
                lock (list)
                {
                    int num = list.Count;
                    isNew = num > threadNum;
                    if (isNew)
                    {
                        for (int i = 0; i < num - threadNum; i++)
                        {
                            KThread t = list[0];
                            t.tStop = true;
                            t.Dispose();
                            list.Remove(t);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < threadNum - num; i++)
                        {
                            KThread t = new KThread();
                            list.Add(t);
                            t.Start(job);
                        }
                    }
                }
            }

            public void Close()
            {
                ChangeSize(0);
                list = null;
                job = null;
            }
        }

        abstract class Job
        {
            public Job()
            {
                canSleep = true;
                canStop = true;
                isStop = false;
                sleepTime = 0;
            }

            /// <summary>
            /// 为true时可以休息
            /// </summary>
            public bool canSleep { get; set; }
            /// <summary>
            /// 为true时可以停止
            /// </summary>
            public bool canStop { get; set; }
            public bool isStop { get; set; }
            public int sleepTime { get; set; }
            public virtual void Invoke()
            {
            }
        }

        class MethodJob<T> : Job
        {
            public ThreadBoxHandler<T> handler { get; set; }
            public PopItemMethod<T> method;
            public override void Invoke()
            {
                T t = default(T);
                t = method();
                canStop = canSleep = t == null;
                if (t != null)
                    handler(t);
            }
        }

        class QueueJob<T> : Job
        {
            public ThreadBoxHandler<T> handler { get; set; }
            public Queue<T> queue;
            public override void Invoke()
            {
                T t = default(T);
                int count = 0;


                lock (queue)
                {
                    count = queue.Count;
                    if (count > 0)
                        t = queue.Dequeue();
                }
                canStop = canSleep = count == 0;
                if (count != 0)
                    handler(t);

            }
        }

        class ListJob<T> : Job
        {
            public ThreadBoxHandler<T> handler { get; set; }
            public ICollection<T> list;
            public override void Invoke()
            {
                T t = default(T);
                int count = 0;
                lock (list)
                {
                    count = list.Count;
                    if (count > 0)
                    {
                        IEnumerator<T> enu = list.GetEnumerator();
                        enu.MoveNext();
                        t = enu.Current;
                        list.Remove(t);
                    }
                }
                canStop = canSleep = count == 0;
                if (count != 0)
                    handler(t);

            }
        }

        class KeeperJob<T> : Job
        {
            public KeeperThreadHandler<T> handler { get; set; }
            public T obj { get; set; }
            public override void Invoke()
            {
                handler(obj);
            }
        }

        #endregion
    }
}