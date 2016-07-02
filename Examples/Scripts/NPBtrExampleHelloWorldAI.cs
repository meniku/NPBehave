using UnityEngine;
using System.Collections;

public class NPBtrExampleHelloWorldAI : MonoBehaviour
{
    private NPBtrRoot behaviorTree;

    void Start()
    {
        behaviorTree = new NPBtrRoot(
            new NPBtrAction(() => Debug.Log("Hello World!"))
        );
        behaviorTree.Start();
    }
}
