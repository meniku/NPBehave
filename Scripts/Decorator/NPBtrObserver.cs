using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;
using System;

public class NPBtrObserver : NPBtrDecorator
{
    private Action onStart;
    private Action<bool> onStop;
    
    public NPBtrObserver(Action onStart, Action<bool> onStop, NPBtrNode decoratee) : base("Observer", decoratee)
    {
        this.onStart = onStart;
        this.onStop = onStop;
    }

    protected override void DoStart()
    {
        this.onStart();
        Decoratee.Start();
    }

    override protected void DoStop()
    {
        Decoratee.Stop();
    }

    protected override void DoChildStopped(NPBtrNode child, bool result)
    {
        this.onStop(result);
        Stopped(true);
    }
}