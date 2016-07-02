using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Assertions;

public class NPBtrRoot : NPBtrDecorator
{
    private NPBtrNode mainNode;

    //private NPBtrNode inProgressNode;

    private NPBtrBlackboard blackboard;
    public override NPBtrBlackboard Blackboard
    {
        get
        {
            return blackboard;
        }
    }


    private NPBtrClock clock;
    public override NPBtrClock Clock
    {
        get
        {
            return clock;
        }
    }

#if UNITY_EDITOR
    public int TotalNumStartCalls = 0;
    public int TotalNumStopCalls = 0;
    public int TotalNumStoppedCalls = 0;
#endif

    public NPBtrRoot(NPBtrNode mainNode) : base("Root", mainNode)
    {
        this.mainNode = mainNode;
        this.clock = NPBtrUnityContext.GetClock();
        this.blackboard = new NPBtrBlackboard(this.clock);
        this.SetRoot(this);
    }
    public NPBtrRoot(NPBtrBlackboard blackboard, NPBtrNode mainNode) : base("Root", mainNode)
    {
        this.blackboard = blackboard;
        this.mainNode = mainNode;
        this.clock = NPBtrUnityContext.GetClock();
        this.SetRoot(this);
    }

    public NPBtrRoot(NPBtrBlackboard blackboard, NPBtrClock clock, NPBtrNode mainNode) : base("Root", mainNode)
    {
        this.blackboard = blackboard;
        this.mainNode = mainNode;
        this.clock = clock;
        this.SetRoot(this);
    }

    public override void SetRoot(NPBtrRoot rootNode)
    {
        Assert.AreEqual(this, rootNode);
        base.SetRoot(rootNode);
        this.mainNode.SetRoot(rootNode);
    }


    override protected void DoStart()
    {
        this.blackboard.Enable();
        this.mainNode.Start();
    }

    override protected void DoStop()
    {
        this.mainNode.Stop();
    }


    override protected void DoChildStopped(NPBtrNode node, bool success)
    {
        if (!IsStopRequested)
        {
            // wait one tick, to prevent endless recursions
            this.clock.AddTimer(0, 0, this.mainNode.Start);
            // this.mainNode.Start();
        }
        else
        {
            this.blackboard.Disable();
            Stopped(success);
        }
    }
}
