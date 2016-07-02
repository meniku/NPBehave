using NPBehave;

public class NPBtrTestRoot : Root
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

    public NPBtrTestRoot(Blackboard blackboard, Clock timer, Node mainNode) :
        base(blackboard, timer, mainNode)
    {
    }

    override protected void DoStart()
    {
        this.didFinish = false;
        base.DoStart();
    }

    override protected void DoChildStopped(Node node, bool success)
    {
        didFinish = true;
        wasSuccess = success;
        Stopped(success);
    }
}
