using UnityEngine;
using NPBehave;

public class NPBehaveExampleSwarmEnemyAI : MonoBehaviour
{
    private Blackboard sharedBlackboard;
    private Blackboard ownBlackboard;
    private Root behaviorTree;

    void Start()
    {
        // get the shared blackboard for this kind of ai, this blackboard is shared by all instances
        sharedBlackboard = UnityContext.GetSharedBlackboard("example-swarm-ai");

        // create a new blackboard instance for this ai instance, parenting it to the sharedBlackboard.
        // This way we can also access shared values through the own blackboard.
        ownBlackboard = new Blackboard(sharedBlackboard, UnityContext.GetClock());

        // create the behaviourTree
        behaviorTree = CreateBehaviourTree();

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
        // Tell the behaviour tree to use the provided blackboard instead of creating a new one
        return new Root(ownBlackboard,

            // Update values in the blackboards every 125 milliseconds
            new Service(0.125f, UpdateBlackboards,

                new Selector(

                    // check the 'engaged' blackboard value.
                    // When the condition changes, we want to immediately jump in or out of this path, thus we use IMMEDIATE_RESTART
                    new BlackboardCondition("engaged", Operator.IS_EQUAL, true, Stops.IMMEDIATE_RESTART,

                        // we are currently engaged with the player
                        new Sequence(

                            // set color to 'red'
                            new Action(() => SetColor(Color.red)) { Label = "Change to Red" },

                            // go towards player until we are not engaged anymore
                            new Action((bool _shouldCancel) =>
                            {
                                if (!_shouldCancel)
                                {
                                    MoveTowards(ownBlackboard.Get<Vector3>("playerLocalPos"));
                                    return Action.Result.PROGRESS;
                                }
                                else
                                {
                                    return Action.Result.FAILED;
                                }
                            }){ Label = "Follow" }
                        )
                    ),

                    // park until engaged does change
                    new Selector(

                        // this time we can also use NBtrStops.BOTH, which stops the current branch if the condition changes but will traverse the 
                        // tree further the normal way (in that case, doesn't make a difference at all). 
                        new BlackboardCondition("playerInRange", Operator.IS_EQUAL, true, Stops.BOTH,

                            // player is not in range, mark 'yellow'
                            new Sequence(
                                new Action(() => SetColor(Color.yellow)) { Label = "Change to Yellow" },
                                new WaitUntilStopped()
                            )
                        ),

                        // player is not in range, mark 'gray'
                        new Sequence(
                            new Action(() => SetColor(Color.grey)) { Label = "Change to Gray" },
                            new WaitUntilStopped()
                        )
                    )
                )
            )
        );
    }

    private void UpdateBlackboards()
    {
        Vector3 playerLocalPos = this.transform.InverseTransformPoint(GameObject.FindGameObjectWithTag("Player").transform.position);

        // update all our distances
        ownBlackboard["playerLocalPos"]= playerLocalPos;
        ownBlackboard["playerInRange"]= playerLocalPos.magnitude < 7.5f;

        // if we are not yet engaging the player, but he is in range and there are not yet other 2 guys engaged with him
        if (ownBlackboard.Get<bool>("playerInRange") && !ownBlackboard.Get<bool>("engaged") && sharedBlackboard.Get<int>("numEngagedActors") < 2)
        {
            // increment the shared 'numEngagedActors'
            sharedBlackboard["numEngagedActors"]= sharedBlackboard.Get<int>("numEngagedActors") + 1;

            // set this instance to 'engaged'
            ownBlackboard["engaged"]= true;
        }

        // if we were engaging the player, but he is not in the range anymore
        if (!ownBlackboard.Get<bool>("playerInRange") && ownBlackboard.Get<bool>("engaged"))
        {
            // decrement the shared 'numEngagedActors'
            sharedBlackboard["numEngagedActors"]= sharedBlackboard.Get<int>("numEngagedActors") - 1;

            // set this instance to 'engaged'
            ownBlackboard["engaged"]= false;
        }
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
