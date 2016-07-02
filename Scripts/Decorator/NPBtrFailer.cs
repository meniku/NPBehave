using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;
using System;

public class NPBtrFailer : NPBtrDecorator
{
    public NPBtrFailer(NPBtrNode decoratee) : base("Failer", decoratee)
    {
    }

    protected override void DoStart()
    {
        Decoratee.Start();
    }

    override protected void DoStop()
    {
        Decoratee.Stop();
    }

    protected override void DoChildStopped(NPBtrNode child, bool result)
    {
        Stopped(true);
    }
}