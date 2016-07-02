using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;
using System;

public class NPBtrInverter : NPBtrDecorator
{
    public NPBtrInverter(NPBtrNode decoratee) : base("Inverter", decoratee)
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
        Stopped(!result);
    }
}