using UnityEngine;
using System.Collections;

public class NPBtrTestRoot : NPBtrRoot
{
    private bool didFinish = false;
    private bool wasSuccess = false;

    public bool DidFinish
    {
        get { return didFinish; }
    }

    public bool WasSuccess
    {
        get { return wasSuccess; }
    }

    public NPBtrTestRoot(NPBtrBlackboard blackboard, NPBtrClock timer, NPBtrNode mainNode) :
        base(blackboard, timer, mainNode)
    {
    }

    override protected void DoStart()
    {
        this.didFinish = false;
        base.DoStart();
    }

    override protected void DoChildStopped(NPBtrNode node, bool success)
    {
        didFinish = true;
        wasSuccess = success;
        Stopped(success);
    }
}
