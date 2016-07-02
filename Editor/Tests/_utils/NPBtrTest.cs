using NPBehave;

public class NPBtrTest
{
    protected Node SUT;
    protected NPBtrTestRoot Root;
    protected Blackboard Blackboard;
    protected Clock Timer;

    protected NPBtrTestRoot CreateBehaviorTree(Node sut)
    {
        this.Timer = new Clock();
        this.Blackboard = new Blackboard(this.Timer);
        this.Root = new NPBtrTestRoot(Blackboard, Timer, sut);
        this.SUT = sut;
        return Root;
    }
}
