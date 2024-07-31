using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Deepglint.XR.Toolkit.Manager
{
    public delegate void Handler();

    public delegate void Handler<in T1>(T1 param1);

    public delegate void Handler<in T1, in T2>(T1 param1, T2 param2);

    public delegate void Handler<in T1, in T2, in T3>(T1 param1, T2 param2, T3 param3);

    public interface IAnimatable
    {
        void AdvanceTime();
    }

    /**时钟管理器[同一函数多次计时，默认会被后者覆盖,delay小于1会立即执行]*/
    public class TimerManager : MonoBehaviour
    {
        public static readonly List<IAnimatable> TimerList = new();

        private static List<TimerHandler> _pool = new();

        /** 用数组保证按放入顺序执行*/
        private static List<TimerHandler> _handlers = new();

        private static int _currFrame;
        private static float _speed = 1;
        /// <summary>
        /// 游戏自启动运行时间，毫秒
        /// </summary>
        public static long CurrentTime => (long)(Time.time * 1000);
        
        public void Update()
        {
            _currFrame++;
            for (var i = 0; i < _handlers.Count; i++)
            {
                var handler = _handlers[i];
                var t = handler.UserFrame ? _currFrame : CurrentTime;
                if (t >= handler.ExeTime)
                {
                    var method = handler.Method;
                    var args = handler.Args;
                    if (handler.Repeat)
                    {
                        while (t >= handler.ExeTime)
                        {
                            handler.ExeTime += handler.Delay;
                            method.DynamicInvoke(args);
                        }
                    }
                    else
                    {
                        Clear(handler.Method);
                        method.DynamicInvoke(args);
                    }
                }
            }
        }

        public static async Task CustomTask(float duration, CancellationToken cancellationToken = default )
        {
            var newDuration = (float)(duration / _speed);
            await Task.Delay((int)newDuration, cancellationToken);
        }

        private static void Create(bool useFrame, bool repeat, float delay, Delegate method, params object[] args)
        {
            var newDelay = delay / _speed;
            if (method == null) return;

            //如果执行时间小于1，直接执行
            if (newDelay < 1)
            {
                method.DynamicInvoke(args);
                return;
            }

            TimerHandler handler;
            if (_pool.Count > 0)
            {
                handler = _pool[_pool.Count - 1];
                _pool.Remove(handler);
            }
            else
            {
                handler = new TimerHandler();
            }

            handler.UserFrame = useFrame;
            handler.Repeat = repeat;
            handler.Delay = newDelay;
            handler.Method = method;
            handler.Args = args;
            handler.ExeTime = newDelay + (useFrame ? _currFrame : CurrentTime);
            _handlers.Add(handler);
        }


        public static void DoOnce(float delay, Handler method)
        {
            Create(false, false, delay, method);
        }

        /// /// <summary>
        /// 定时执行一次(基于毫秒)
        /// </summary>
        /// <param name="delay">延迟时间(单位毫秒)</param>
        /// <param name="method">结束时的回调方法</param>
        /// <param name="args">回调参数</param>
        public static void DoOnce<T1>(float delay, Handler<T1> method, params object[] args)
        {
            Create(false, false, delay, method, args);
        }

        public static void DoOnce<T1, T2>(float delay, Handler<T1, T2> method, params object[] args)
        {
            Create(false, false, delay, method, args);
        }

        public static void DoOnce<T1, T2, T3>(float delay, Handler<T1, T2, T3> method, params object[] args)
        {
            Create(false, false, delay, method, args);
        }


        public static void DoLoop(float delay, Handler method)
        {
            Create(false, true, delay, method);
        }

        /// /// <summary>
        /// 定时重复执行(基于毫秒)
        /// </summary>
        /// <param name="delay">延迟时间(单位毫秒)</param>
        /// <param name="method">结束时的回调方法</param>
        /// <param name="args">回调参数</param>
        public static void DoLoop<T1>(float delay, Handler<T1> method, params object[] args)
        {
            Create(false, true, delay, method, args);
        }

        public static void DoLoop<T1, T2>(float delay, Handler<T1, T2> method, params object[] args)
        {
            Create(false, true, delay, method, args);
        }

        public static void DoLoop<T1, T2, T3>(float delay, Handler<T1, T2, T3> method, params object[] args)
        {
            Create(false, true, delay, method, args);
        }



        public static void DoFrameOnce(float delay, Handler method)
        {
            Create(true, false, delay, method);
        }

        /// <summary>
        /// 定时执行一次(基于帧率)
        /// </summary>
        /// <param name="delay">延迟时间(单位为帧)</param>
        /// <param name="method">结束时的回调方法</param>
        /// <param name="args">回调参数</param>
        public static void DoFrameOnce<T1>(float delay, Handler<T1> method, params object[] args)
        {
            Create(true, false, delay, method, args);
        }

        public static void DoFrameOnce<T1, T2>(float delay, Handler<T1, T2> method, params object[] args)
        {
            Create(true, false, delay, method, args);
        }

        public static void DoFrameOnce<T1, T2, T3>(float delay, Handler<T1, T2, T3> method, params object[] args)
        {
            Create(true, false, delay, method, args);
        }


        public static void DoFrameLoop(float delay, Handler method)
        {
            Create(true, true, delay, method);
        }

        /// <summary>
        /// 定时重复执行(基于帧率)
        /// </summary>
        /// <param name="delay">延迟时间(单位为帧)</param>
        /// <param name="method">结束时的回调方法</param>
        /// <param name="args">回调参数</param>
        public static void DoFrameLoop<T1>(float delay, Handler<T1> method, params object[] args)
        {
            Create(true, true, delay, method, args);
        }

        public static void DoFrameLoop<T1, T2>(float delay, Handler<T1, T2> method, params object[] args)
        {
            Create(true, true, delay, method, args);
        }

        public static void DoFrameLoop<T1, T2, T3>(float delay, Handler<T1, T2, T3> method, params object[] args)
        {
            Create(true, true, delay, method, args);
        }

        /// <summary>
        /// 清理定时器
        /// </summary>
        /// <param name="method">method为回调函数本身</param>
        public static void ClearTimer(Handler method)
        {
            Clear(method);
        }

        public static void ClearTimer<T1>(Handler<T1> method)
        {
            Clear(method);
        }

        public static void ClearTimer<T1, T2>(Handler<T1, T2> method)
        {
            Clear(method);
        }

        public static void ClearTimer<T1, T2, T3>(Handler<T1, T2, T3> method)
        {
            Clear(method);
        }

        private static void Clear(Delegate method)
        {
            var handler = _handlers.FirstOrDefault(t => t.Method == method);
            if (handler != null)
            {
                _handlers.Remove(handler);
                handler.Clear();
                _pool.Add(handler);
            }
        }

        /// <summary>
        /// 清理所有定时器
        /// </summary>
        public static void ClearAllTimer()
        {
            foreach (var handler in _handlers)
            {
                Clear(handler.Method);
                ClearAllTimer();
                return;
            }
        }


        /**定时处理器*/
        private class TimerHandler
        {
            /**执行间隔*/
            public float Delay;

            /**是否重复执行*/
            public bool Repeat;

            /**是否用帧率*/
            public bool UserFrame;

            /**执行时间*/
            public float ExeTime;

            /**处理方法*/
            public Delegate Method;

            /**参数*/
            public object[] Args;

            /**清理*/
            public void Clear()
            {
                Method = null;
                Args = null;
            }
        }
    }
}