using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;
using System;

public class NPBtrWait : NPBtrTask
{
    private float seconds;
    private float randomVariance;

    public NPBtrWait(float seconds, float randomVariance) : base("Wait")
    {
        this.seconds = seconds;
        this.randomVariance = randomVariance;
    }

    public NPBtrWait(float seconds) : base("Wait")
    {
        this.seconds = seconds;
        this.randomVariance = this.seconds * 0.05f;
    }

    protected override void DoStart()
    {
        if (randomVariance >= 0f)
        {
            Clock.AddTimer(seconds, randomVariance, 0, onTimer);
        }
        else
        {
            Clock.AddTimer(seconds, 0, onTimer);
        }
    }

    protected override void DoStop()
    {
        Clock.RemoveTimer(onTimer);
        this.Stopped(false);
    }

    private void onTimer()
    {
        Clock.RemoveTimer(onTimer);
        this.Stopped(true);
    }
}