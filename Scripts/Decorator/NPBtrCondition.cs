using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;
using System;

// TODO: add some kind of "Watch for blackboard values" option
public class NPBtrCondition : NPBtrDecorator
{
    private Func<bool> Condition;

    public NPBtrCondition(Func<bool> condition, NPBtrNode decoratee) : base("Condition", decoratee)
    {
        this.Condition = condition;
    }

    protected override void DoStart()
    {
        if (!Condition.Invoke())
        {
            Stopped(false);
        }
        else
        {
            Decoratee.Start();
        }
    }

    override protected void DoStop()
    {
        Decoratee.Stop();
    }

    protected override void DoChildStopped(NPBtrNode child, bool result)
    {
        Assert.AreNotEqual(this.CurrentState, State.INACTIVE);
        Stopped(result);
    }
}