using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;
using System;

public class NPBtrRandom : NPBtrDecorator
{
    private float probability;

    public NPBtrRandom(float probability, NPBtrNode decoratee) : base("Random", decoratee)
    {
        this.probability = probability;
    }

    protected override void DoStart()
    {
        if (UnityEngine.Random.value <= this.probability)
        {
            Decoratee.Start();
        }
        else
        {
            Stopped(false);
        }
    }

    override protected void DoStop()
    {
        Decoratee.Stop();
    }

    protected override void DoChildStopped(NPBtrNode child, bool result)
    {
        Stopped(result);
    }
}