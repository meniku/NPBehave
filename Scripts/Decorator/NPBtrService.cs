using UnityEngine;
using System.Collections;
using System;

public class NPBtrService : NPBtrDecorator
{
    private Action serviceMethod;

    private float interval = -1.0f;
    private float randomVariation;

    public NPBtrService(float interval, float randomVariation, Action service, NPBtrNode decoratee) : base("Service", decoratee)
    {
        this.serviceMethod = service;
        this.interval = interval;
        this.randomVariation = randomVariation;
    }

    public NPBtrService(float interval, Action service, NPBtrNode decoratee) : base("Service", decoratee)
    {
        this.serviceMethod = service;
        this.interval = interval;
        this.randomVariation = interval * 0.05f;
    }

    public NPBtrService(Action service, NPBtrNode decoratee) : base("Service", decoratee)
    {
        this.serviceMethod = service;
    }

    protected override void DoStart()
    {
        if (this.interval <= 0f)
        {
            this.Clock.AddUpdateObserver(serviceMethod);
            serviceMethod();
        }
        else if (randomVariation <= 0f)
        {
            this.Clock.AddTimer(this.interval, -1, serviceMethod);
            serviceMethod();
        }
        else
        {
            InvokeServiceMethodWithRandomVariation();
        }
        Decoratee.Start();
    }

    override protected void DoStop()
    {
        Decoratee.Stop();
    }

    protected override void DoChildStopped(NPBtrNode child, bool result)
    {
        if (this.interval <= 0f)
        {
            this.Clock.RemoveUpdateObserver(serviceMethod);
        }
        else if (randomVariation <= 0f)
        {
            this.Clock.RemoveTimer(serviceMethod);
        }
        else
        {
            this.Clock.RemoveTimer(InvokeServiceMethodWithRandomVariation);
        }
        Stopped(result);
    }

    private void InvokeServiceMethodWithRandomVariation()
    {
        serviceMethod();
        this.Clock.AddTimer(interval, randomVariation, 0, InvokeServiceMethodWithRandomVariation);
    }
}