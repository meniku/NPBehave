using UnityEngine;
using System.Collections;

public class NPBtrExampleHelloBlackboardsAI : MonoBehaviour
{
    private NPBtrRoot behaviorTree;

    void Start()
    {
        behaviorTree = new NPBtrRoot(

            // toggle the 'toggled' blackboard boolean flag around every 500 milliseconds
            new NPBtrService(0.5f, () => { behaviorTree.Blackboard.Set("foo", !behaviorTree.Blackboard.GetBool("foo")); },

                new NPBtrSelector(

                    // Check the 'toggled' flag. NPBtrStops.IMMEDIATE_RESTART means that the Blackboard will be observed for changes 
                    // while this or any lower priority branches are executed. If the value changes, the corresponding branch will be
                    // stopped and it will be immediately jump to the branch that now matches the condition.
                    new NPBtrBlackboardValue("foo", NPBtrOperator.IS_EQUAL, true, NPBtrStops.IMMEDIATE_RESTART,

                        // when 'toggled' is true, this branch will get executed.
                        new NPBtrSequence(

                            // print out a message ...
                            new NPBtrAction(() => Debug.Log("foo")),

                            // ... and stay here until the `NPBtrBlackboardValue`-node stops us because the toggled flag went false.
                            new NPBtrWaitUntilStopped()
                        )
                    ),

                    // when 'toggled' is false, we'll eventually land here
                    new NPBtrSequence(
                        new NPBtrAction(() => Debug.Log("bar")),
                        new NPBtrWaitUntilStopped()
                    )
                )
            )
        );
        behaviorTree.Start();
    }
}
