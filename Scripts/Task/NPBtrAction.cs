using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;
using System;

public class NPBtrAction : NPBtrTask
{
    public enum Result
    {
        SUCCESS,
        FAILED,
        PROGRESS
    }


    private Func<bool, Result> multiFrameFunc;
    private Action action;

    public NPBtrAction(Action action) : base("Action")
    {
        this.multiFrameFunc = null;
        this.action = action;
    }

    public NPBtrAction(Func<bool, Result> multiframeFunc) : base("Action")
    {
        this.multiFrameFunc = multiframeFunc;
        this.action = null;
    }

    protected override void DoStart()
    {
        if (this.multiFrameFunc != null)
        {
            Result result = this.multiFrameFunc.Invoke(false);
            if (result == Result.PROGRESS)
            {
                this.RootNode.Clock.AddUpdateObserver(OnUpdateFunc);
            }
            else
            {
                this.Stopped(result == Result.SUCCESS);
            }
        }
        else
        {
            this.action.Invoke();
            this.Stopped(true);
        }
    }

    private void OnUpdateFunc()
    {
        Result result = this.multiFrameFunc.Invoke(false);
        if (result != Result.PROGRESS)
        {
            this.RootNode.Clock.RemoveUpdateObserver(OnUpdateFunc);
            this.Stopped(result == Result.SUCCESS);
        }
    }

    protected override void DoStop()
    {
        if (this.multiFrameFunc != null)
        {
            Result result = this.multiFrameFunc.Invoke(true);
            Assert.AreNotEqual(result, Result.PROGRESS, "The Task has to return Result.SUCCESS or Result.FAILED after beeing cancelled!");
            this.RootNode.Clock.RemoveUpdateObserver(OnUpdateFunc);
            this.Stopped(result == Result.SUCCESS);
        }
        else
        {
            Assert.IsTrue(false, "DoStop called for a single frame action on " + this);
        }
    }
}