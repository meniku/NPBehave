using UnityEngine;
using NPBehave;

namespace NPBehave.Examples.ReusableSubtrees
{
    public class EnemyA : MonoBehaviour
    {
        private Root behaviorTree;

        void Start()
        {
            // this enemy is only able to move
            behaviorTree = new Root(

                // create movement behavior from by using our common node factory
                NodeFactory.CreateMoveSubtree("EnemyA")
            );

            behaviorTree.Start();
        }
    }
}
