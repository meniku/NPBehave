using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;
using System;

public class NPBtrSucceeder : NPBtrDecorator
{
    public NPBtrSucceeder(NPBtrNode decoratee) : base("Succeeder", decoratee)
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