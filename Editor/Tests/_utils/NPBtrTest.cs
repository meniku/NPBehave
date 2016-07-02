using UnityEngine;
using System.Collections;

public class NPBtrTest
{
    protected NPBtrNode SUT;
    protected NPBtrTestRoot Root;
    protected NPBtrBlackboard Blackboard;
    protected NPBtrClock Timer;

    protected NPBtrTestRoot CreateBehaviorTree(NPBtrNode sut)
    {
        this.Timer = new NPBtrClock();
        this.Blackboard = new NPBtrBlackboard(this.Timer);
        this.Root = new NPBtrTestRoot(Blackboard, Timer, sut);
        this.SUT = sut;
        return Root;
    }
}
