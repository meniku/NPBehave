using UnityEngine;
using NPBehave;

namespace NPBehave.Examples.ReusableSubtrees
{
    public class EnemyB : MonoBehaviour
    {
        private Root behaviorTree;

        void Start()
        {
            // this enemy is only able to move
            behaviorTree = new Root(
                new Sequence(

                    // create movement behavior from by using our common node factory
                    NodeFactory.CreateMoveSubtree("EnemyB"),

                    // also add some custom behavior
                    new Action(() => Debug.Log("EnemyB attacking!"))
                )
            );
            behaviorTree.Start();
        }
    }
}
