using UnityEngine;
using NPBehave;

public class NPBehaveExampleNavMoveToEnemy : MonoBehaviour
{
    private Blackboard blackboard;
    private Root behaviorTree;

    public UnityEngine.AI.NavMeshAgent Agent;

    void Start()
    {
        // create our behaviour tree and get it's blackboard
        behaviorTree = CreateBehaviourTree();
        blackboard = behaviorTree.Blackboard;

        // attach the debugger component if executed in editor (helps to debug in the inspector) 
#if UNITY_EDITOR
        Debugger debugger = (Debugger)this.gameObject.AddComponent(typeof(Debugger));
        debugger.BehaviorTree = behaviorTree;
#endif

        // start the behaviour tree
        behaviorTree.Start();
    }

    private Root CreateBehaviourTree()
    {
        // we always need a root node
        return new Root(

            // kick up our service to update the "playerDistance" and "playerPos" Blackboard values every 125 milliseconds
            new Service(0.125f, UpdateBlackboard,

                new Selector(

                    // check the 'playerDistance' blackboard value.
                    // When the condition changes, we want to immediately jump in or out of this path, thus we use IMMEDIATE_RESTART
                    new BlackboardCondition("playerDistance", Operator.IS_SMALLER, 7.5f, Stops.IMMEDIATE_RESTART,

                        // the player is in our range of 7.5f
                        new Sequence(

                            // set color to 'red'
                            new Action(() => SetColor(Color.yellow)) { Label = "Change to Yellow" },

                            // go towards player until playerDistance is greater than 7.5f, but stop once within 1.5f
                            new NavMoveTo(Agent, "playerPos", 2.0f, true) { Label = "Follow" },

                            // stop when reached the player position
                            new Action(() => SetColor(Color.red)) { Label = "Change to Red" },

                            // stop only when player distance is further away again
                            new Wait(0.5f)
                        )

                    ),

                    // park until playerDistance does change
                    new Sequence(
                        new Action(() => SetColor(Color.grey)) { Label = "Change to Gray" },
                        new WaitUntilStopped()
                    )
                )
            )
        );
    }

    private void UpdateBlackboard()
    {
        Transform playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        Vector3 playerPos = playerTransform.position;
        behaviorTree.Blackboard["playerPos"] = playerPos;
        behaviorTree.Blackboard["playerDistance"] = (playerPos - transform.position).magnitude;
    }

    private void MoveTowards(Vector3 localPosition)
    {
        transform.localPosition += localPosition * 0.5f * Time.deltaTime;
    }

    private void SetColor(Color color)
    {
        GetComponent<MeshRenderer>().material.SetColor("_Color", color);
    }
}
