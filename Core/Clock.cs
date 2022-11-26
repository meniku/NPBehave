using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace NPBehave
{

    public class Clock
    {
        private Dictionary<System.Action, long> timerLookup = new Dictionary<System.Action, long>();
        private SortedDictionary<long, Timer> timers = new SortedDictionary<long, Timer>();
        private HashSet<System.Action> removeTimers = new HashSet<System.Action>();
        private Dictionary<System.Action, AddTimerStruct> addTimers = new Dictionary<System.Action, AddTimerStruct>();
        private bool isInUpdate = false;
        private long timerNum = 0;

        class AddTimerStruct
        {
            public long timerId;
            public Timer timer;
        }

        class Timer
        {
            public double scheduledTime = 0f;
            public int repeat = 0;
            public bool used = false;
			public double delay = 0f;
			public float randomVariance = 0.0f;
            public System.Action action = null;

            public void ScheduleAbsoluteTime(double elapsedTime)
			{
				scheduledTime = elapsedTime + delay - randomVariance * 0.5f + randomVariance * Context.Platform.GenerateRandomFloat();
			}
        }

        private double elapsedTime = 0f;

        private List<Timer> timerPool = new List<Timer>();
        private int currentTimerPoolIndex = 0;

        /// <summary>Register a timer function</summary>
        /// <param name="time">time in milliseconds</param>
        /// <param name="repeat">number of times to repeat, set to -1 to repeat until unregistered.</param>
        /// <param name="action">method to invoke</param>
        public void AddTimer(float time, int repeat, System.Action action)
        {
            AddTimer(time, 0f, repeat, action);
        }

        /// <summary>Register a timer function with random variance</summary>
        /// <param name="time">time in milliseconds</param>
        /// <param name="randomVariance">deviate from time on a random basis</param>
        /// <param name="repeat">number of times to repeat, set to -1 to repeat until unregistered.</param>
        /// <param name="action">method to invoke</param>
        public void AddTimer(float delay, float randomVariance, int repeat, System.Action action)
        {
            Timer timer = null;
            long timerId;

            if (!isInUpdate)
            {
                if (!this.timerLookup.ContainsKey(action))
                {
                    timerId = timerNum;
                    ++timerNum;
                    this.timerLookup[action] = timerId;
                    this.timers[timerId] = getTimerFromPool();
                }
                else
                {
                    timerId = this.timerLookup[action];
                }
				timer = this.timers[timerId];
            }
            else
            {
                if (!this.addTimers.ContainsKey(action))
                {
                    timerId = timerNum;
                    ++timerNum;
                    AddTimerStruct addTimer = new Clock.AddTimerStruct();
                    addTimer.timerId = timerId;
                    addTimer.timer = getTimerFromPool();
                    this.addTimers[action] = addTimer;
                    timer = this.addTimers[action].timer;
                }
                else
                {
                    timer = this.addTimers[ action ].timer;
                }

                if (this.removeTimers.Contains(action))
                {
                    this.removeTimers.Remove(action);
                }
            }

            Debug.Assert( timer.used );
            
			timer.delay = delay;
			timer.randomVariance = randomVariance;
			timer.repeat = repeat;
            timer.action = action;
			timer.ScheduleAbsoluteTime(elapsedTime);
        }

        public void RemoveTimer(System.Action action)
        {
            if (!isInUpdate)
            {
                if (this.timerLookup.ContainsKey(action))
                {
                    long timerId = timerLookup[action];
                    this.timers[timerId].used = false;
                    this.timers.Remove(timerId);
                    this.timerLookup.Remove(action);
                }
            }
            else
            {
                if (this.timerLookup.ContainsKey(action))
                {
                    this.removeTimers.Add(action);
                }
                if (this.addTimers.ContainsKey(action))
                {
                    AddTimerStruct addTimer = this.addTimers[action];
                    Debug.Assert(addTimer.timer.used);
                    addTimer.timer.used = false;
                    this.addTimers.Remove(action);
                }
            }
        }


        public bool HasTimer(System.Action action)
        {
            if (!isInUpdate)
            {
                return this.timerLookup.ContainsKey(action);
            }
            else
            {
                if (this.removeTimers.Contains(action))
                {
                    return false;
                }
                else if (this.addTimers.ContainsKey(action))
                {
                    return true;
                }
                else
                {
                    return this.timerLookup.ContainsKey(action);
                }
            }
        }

        /// <summary>Register a function that is called every frame</summary>
        /// <param name="action">function to invoke</param>
        public void AddUpdateObserver(System.Action action)
        {
            this.AddTimer(0.0f, -1, action);
        }

        public void RemoveUpdateObserver(System.Action action)
        {
            this.RemoveTimer(action);
        }

        public bool HasUpdateObserver(System.Action action)
        {
            return this.HasTimer(action);
        }

        public void Update(float deltaTime)
        {
            this.elapsedTime += deltaTime;

            this.isInUpdate = true;

            SortedDictionary<long, Timer>.KeyCollection keys = this.timers.Keys;
			foreach (long timerId in keys)
            {
                Timer timer = this.timers[timerId];
                Debug.Assert(timer.used);
                if (this.removeTimers.Contains(timer.action) )
                {
                    continue;
                }

                if (timer.scheduledTime <= this.elapsedTime)
                {
                    if (timer.repeat == 0)
                    {
                        RemoveTimer(timer.action);
                    }
                    else if (timer.repeat >= 0)
                    {
                        timer.repeat--;
                    }
                    timer.action.Invoke();
					timer.ScheduleAbsoluteTime(elapsedTime);
                }
            }

            foreach (System.Action action in this.removeTimers)
            {
                long timerId = timerLookup[action];
                Debug.Assert(this.timers[timerId].used);
                this.timers[timerId].used = false;
                this.timers.Remove(timerId);
                this.timerLookup.Remove(action);
            }

            foreach (System.Action action in this.addTimers.Keys)
            {
                AddTimerStruct addTimer = this.addTimers[ action ];

                if (this.timerLookup.ContainsKey(action))
                {
                    Debug.Assert( false );
                }
                long timerId = addTimer.timerId;
                this.timers[addTimer.timerId] = addTimer.timer;
                this.timerLookup[action] = timerId;
                Debug.Assert(this.timers[timerId].used);
            }

            this.addTimers.Clear();
            this.removeTimers.Clear();

            this.isInUpdate = false;
        }

        public int NumTimers
        {
            get
            {
                return timers.Count;
            }
        }

        public double ElapsedTime
        {
            get
            {
                return elapsedTime;
            }
        }

        private Timer getTimerFromPool()
        {
            int i = 0;
            int l = timerPool.Count;
            Timer timer = null;
            while (i < l)
            {
                int timerIndex = (i + currentTimerPoolIndex) % l;
                if (!timerPool[timerIndex].used)
                {
                    currentTimerPoolIndex = timerIndex;
                    timer = timerPool[timerIndex];
                    break;
                }
                i++;
            }

            if (timer == null)
            {
                timer = new Timer();
                currentTimerPoolIndex = 0;
                timerPool.Add(timer);
            }

            timer.used = true;
            return timer;
        }

        public int DebugPoolSize
        {
            get
            {
                return this.timerPool.Count;
            }
        }
    }
}