using UnityEngine;
using NPBehave;

public class NPBehaveExampleHelloWorldAI : MonoBehaviour
{
    private Root behaviorTree;

    void Start()
    {
        behaviorTree = new Root(
            new Action(() => Debug.Log("Hello World!"))
        );
        behaviorTree.Start();
    }
}
