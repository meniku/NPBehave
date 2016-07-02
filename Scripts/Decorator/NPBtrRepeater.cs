using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;
using System;

public class NPBtrRepeater : NPBtrDecorator
{
    private int repeatTimes = -1;
    private int numberOfExecutions;
    private bool stopRequested;

    /// <param name="repeatTimes">number of times to repeat, set to -1 to repeat forever, be careful with endless loops!</param>
    /// <param name="decoratee">Decorated Node</param>
    public NPBtrRepeater(int repeatTimes, NPBtrNode decoratee) : base("Repeater", decoratee)
    {
        this.repeatTimes = repeatTimes;
    }

    public NPBtrRepeater(NPBtrNode decoratee) : base("Repeater", decoratee)
    {
    }

    protected override void DoStart()
    {
        if (repeatTimes != 0)
        {
            stopRequested = false;
            numberOfExecutions = 0;
            Decoratee.Start();
        }
        else
        {
            this.Stopped(true);
        }
    }

    override protected void DoStop()
    {
        stopRequested = true;
        Decoratee.Stop();
    }

    protected override void DoChildStopped(NPBtrNode child, bool result)
    {
        if (result)
        {
            if (stopRequested || (repeatTimes > 0 && ++numberOfExecutions >= repeatTimes))
            {
                Stopped(true);
            }
            else
            {
                Decoratee.Start();
            }
        }
        else
        {
            Stopped(false);
        }
    }
}