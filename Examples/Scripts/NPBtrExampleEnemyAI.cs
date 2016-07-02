using UnityEngine;
using System.Collections;

public class NPBtrExampleEnemyAI : MonoBehaviour
{
    private NPBtrBlackboard blackboard;
    private NPBtrRoot behaviorTree;

    void Start()
    {
        // create our behaviour tree and get it's blackboard
        behaviorTree = CreateBehaviourTree();
        blackboard = behaviorTree.Blackboard;

        // attach the debugger component if executed in editor (helps to debug in the inspector) 
#if UNITY_EDITOR
        NPBtrDebugger debugger = (NPBtrDebugger)this.gameObject.AddComponent(typeof(NPBtrDebugger));
        debugger.BehaviorTree = behaviorTree;
#endif

        // start the behaviour tree
        behaviorTree.Start();
    }

    private NPBtrRoot CreateBehaviourTree()
    {
        // we always need a root node
        return new NPBtrRoot(

            // kick up our service to update the "playerDistance" and "playerLocalPos" Blackboard values every 125 milliseconds
            new NPBtrService(0.125f, UpdatePlayerDistance,

                new NPBtrSelector(

                    // check the 'playerDistance' blackboard value.
                    // When the condition changes, we want to immediately jump in or out of this path, thus we use IMMEDIATE_RESTART
                    new NPBtrBlackboardValue("playerDistance", NPBtrOperator.IS_SMALLER, 7.5f, NPBtrStops.IMMEDIATE_RESTART,

                        // the player is in our range of 7.5f
                        new NPBtrSequence(

                            // set color to 'red'
                            new NPBtrAction(() => SetColor(Color.red)),

                            // go towards player until playerDistance is greater than 7.5 ( in that case, _shouldCancel will get true )
                            new NPBtrAction((bool _shouldCancel) =>
                            {
                                if (!_shouldCancel)
                                {
                                    MoveTowards(blackboard.GetVector3("playerLocalPos"));
                                    return NPBtrAction.Result.PROGRESS;
                                }
                                else
                                {
                                    return NPBtrAction.Result.FAILED;
                                }
                            })
                        )
                    ),

                    // park until playerDistance does change
                    new NPBtrSequence(
                        new NPBtrAction(() => SetColor(Color.grey)),
                        new NPBtrWaitUntilStopped()
                    )
                )
            )
        );
    }

    private void UpdatePlayerDistance()
    {
        Vector3 playerLocalPos = this.transform.InverseTransformPoint(GameObject.FindGameObjectWithTag("Player").transform.position);
        behaviorTree.Blackboard.Set("playerLocalPos", playerLocalPos);
        behaviorTree.Blackboard.Set("playerDistance", playerLocalPos.magnitude);
    }

    private void MoveTowards(Vector3 localPosition)
    {
        transform.localPosition += localPosition * 0.01f;
    }

    private void SetColor(Color color)
    {
        GetComponent<MeshRenderer>().material.SetColor("_Color", color);
    }
}
