using NPBehave;

public class NPBtrMockNode : Node
{
    private bool succedsOnExplictStop;

    public NPBtrMockNode(bool succedsOnExplictStop = false) : base("MockNode")
    {
        this.succedsOnExplictStop = succedsOnExplictStop;
    }

    override protected void DoStop()
    {
        this.Stopped(succedsOnExplictStop);
    }

    public void Finish(bool success)
    {
        this.Stopped(success);
    }
}
