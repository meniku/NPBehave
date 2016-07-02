using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.Assertions;

public class NPBtrClock
{
    private List<Action> updateObservers = new List<Action>();
    private Dictionary<Action, Timer> timers = new Dictionary<Action, Timer>();
    private HashSet<Action> removeObservers = new HashSet<Action>();
    private HashSet<Action> addObservers = new HashSet<Action>();
    private HashSet<Action> removeTimers = new HashSet<Action>();
    private Dictionary<Action, Timer> addTimers = new Dictionary<Action, Timer>();
    private bool isInUpdate = false;

    class Timer
    {
        public double absoluteTime = 0f;
        public int repeat = 0;
        public bool used = false;
    }

    private double elapsedTime = 0f;

    private List<Timer> timerPool = new List<Timer>();
    private int currentTimerPoolIndex = 0;

    /// <param name="time">time in milliseconds</param>
    /// <param name="repeat">number of times to repeat, set to -1 to repeat until unregistered.</param>
    /// <param name="action">method to invoke</param>
    public void AddTimer(float time, int repeat, Action action)
    {
        AddTimer(time, 0f, repeat, action);
    }

    public void AddTimer(float time, float randomVariance, int repeat, Action action)
    {
        time = time - randomVariance * 0.5f + randomVariance * UnityEngine.Random.value;
        if (!isInUpdate)
        {
            if(this.timers.ContainsKey(action)) 
            {
                Assert.IsTrue(this.timers[action].used);
                this.timers[action].absoluteTime = elapsedTime + time;
                this.timers[action].repeat = repeat;
            }
            else 
            {
                this.timers[action] = getTimerFromPool(elapsedTime + time, repeat);
            }
        }
        else
        {
            if (!this.addTimers.ContainsKey(action))
            {
                this.addTimers[action] = getTimerFromPool(elapsedTime + time, repeat);
            }
            else 
            {
                Assert.IsTrue(this.addTimers[action].used);
                this.addTimers[action].repeat = repeat;
                this.addTimers[action].absoluteTime = elapsedTime + time;
            }
            
            if (this.removeTimers.Contains(action))
            {
                this.removeTimers.Remove(action);
            }
        }
    }

    public void RemoveTimer(Action action)
    {
        if (!isInUpdate)
        {
            if (this.timers.ContainsKey(action))
            {
                timers[action].used = false;
                this.timers.Remove(action);
            }
        }
        else
        {
            if (this.timers.ContainsKey(action))
            {
                this.removeTimers.Add(action);
            }
            if (this.addTimers.ContainsKey(action))
            {
                Assert.IsTrue(this.addTimers[action].used);
                this.addTimers[action].used = false;
                this.addTimers.Remove(action);
            }
        }
    }

    public bool HasTimer(Action action)
    {
        if (!isInUpdate)
        {
            return this.timers.ContainsKey(action);
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
                return this.timers.ContainsKey(action);
            }
        }
    }


    public void AddUpdateObserver(Action action)
    {
        if (!isInUpdate)
        {
            this.updateObservers.Add(action);
        }
        else
        {
            if (!this.updateObservers.Contains(action))
            {
                this.addObservers.Add(action);
            }
            if (this.removeObservers.Contains(action))
            {
                this.removeObservers.Remove(action);
            }
        }
    }

    public void RemoveUpdateObserver(Action action)
    {
        if (!isInUpdate)
        {
            this.updateObservers.Remove(action);
        }
        else
        {
            if (this.updateObservers.Contains(action))
            {
                this.removeObservers.Add(action);
            }
            if (this.addObservers.Contains(action))
            {
                this.addObservers.Remove(action);
            }
        }
    }

    public bool HasUpdateObserver(Action action)
    {
        if (!isInUpdate)
        {
            return this.updateObservers.Contains(action);
        }
        else
        {
            if (this.removeObservers.Contains(action))
            {
                return false;
            }
            else if (this.addObservers.Contains(action))
            {
                return true;
            }
            else
            {
                return this.updateObservers.Contains(action);
            }
        }
    }

    public void Update(float deltaTime)
    {
        this.elapsedTime += deltaTime;

        this.isInUpdate = true;

        foreach (Action action in updateObservers)
        {
            if (!removeObservers.Contains(action))
            {
                action.Invoke();
            }
        }

        Dictionary<Action, Timer>.KeyCollection keys = timers.Keys;
        foreach (Action timer in keys)
        {
            if (this.removeTimers.Contains(timer))
            {
                continue;
            }

            Timer time = timers[timer];
            if (time.absoluteTime <= this.elapsedTime)
            {
                if (time.repeat == 0)
                {
                    RemoveTimer(timer);
                }
                else if (time.repeat >= 0)
                {
                    time.repeat--;
                }
                timer.Invoke();
            }
        }

        foreach (Action action in this.addObservers)
        {
            this.updateObservers.Add(action);
        }
        foreach (Action action in this.removeObservers)
        {
            this.updateObservers.Remove(action);
        }
        foreach (Action action in this.addTimers.Keys)
        {
            if(this.timers.ContainsKey(action))
            {
                Assert.AreNotEqual(this.timers[action], this.addTimers[action]);
                this.timers[action].used = false;
            }
            Assert.IsTrue(this.addTimers[action].used);
            this.timers[action] = this.addTimers[action];
        }
        foreach (Action action in this.removeTimers)
        {
            Assert.IsTrue(this.timers[action].used);
            timers[action].used = false;
            this.timers.Remove(action);
        }
        this.addObservers.Clear();
        this.removeObservers.Clear();
        this.addTimers.Clear();
        this.removeTimers.Clear();

        this.isInUpdate = false;
    }

    public int NumUpdateObservers
    {
        get
        {
            return updateObservers.Count;
        }
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

    private Timer getTimerFromPool(double absoluteTime, int repeat)
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
        timer.absoluteTime = absoluteTime;
        timer.repeat = repeat;
        return timer;
    }

    public int DebugPoolSize
    {
        get {
            return this.timerPool.Count;
        }
    }
}
