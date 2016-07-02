using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;
using System;

public class NPBtrTimeMax : NPBtrDecorator
{
    private float limit = 0.0f;
    private float randomVariation;
    private bool waitForChildButFailOnLimitReached = false;
    private bool isLimitReached = false;

    public NPBtrTimeMax(float limit, bool waitForChildButFailOnLimitReached, NPBtrNode decoratee) : base("NPBtrTimeMax", decoratee)
    {
        this.limit = limit;
        this.randomVariation = limit * 0.05f;
        this.waitForChildButFailOnLimitReached = waitForChildButFailOnLimitReached;
        Assert.IsTrue(limit > 0f, "limit has to be set");
    }
    
    public NPBtrTimeMax(float limit, float randomVariation, bool waitForChildButFailOnLimitReached, NPBtrNode decoratee) : base("NPBtrTimeMax", decoratee)
    {
        this.limit = limit;
        this.randomVariation = randomVariation;
        this.waitForChildButFailOnLimitReached = waitForChildButFailOnLimitReached;
        Assert.IsTrue(limit > 0f, "limit has to be set");
    }

    protected override void DoStart()
    {
        this.isLimitReached = false;
        Clock.AddTimer(limit, randomVariation, 0, TimeoutReached);
        Decoratee.Start();
    }

    override protected void DoStop()
    {
        Clock.RemoveTimer(TimeoutReached);
        if (Decoratee.IsActive)
        {
            Decoratee.Stop();
        }
        else
        {
            Stopped(false);
        }
    }

    protected override void DoChildStopped(NPBtrNode child, bool result)
    {
        Clock.RemoveTimer(TimeoutReached);
        if(isLimitReached) {
            Stopped(false);
        } else {
            Stopped(result);
        }
    }

    private void TimeoutReached()
    {
        if (!waitForChildButFailOnLimitReached)
        {
            Decoratee.Stop();
        } 
        else
        {
            isLimitReached = true;
            Assert.IsTrue(Decoratee.IsActive);
        }
    }
}