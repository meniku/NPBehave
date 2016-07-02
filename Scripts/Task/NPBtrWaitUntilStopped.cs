using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;
using System;

public class NPBtrWaitUntilStopped : NPBtrTask
{
    private bool sucessWhenStopped;
    public NPBtrWaitUntilStopped(bool sucessWhenStopped = false) : base("WaitUntilStopped")
    {
        this.sucessWhenStopped = sucessWhenStopped;
    }

    protected override void DoStop()
    {
        this.Stopped(sucessWhenStopped);
    }
}