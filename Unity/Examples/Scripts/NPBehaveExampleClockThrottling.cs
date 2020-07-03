using UnityEngine;
using NPBehave;

/// <summary>
/// This example shows how you can use use clock instances to have complete control over how your tree receives updates.
/// This allows you for example to throttle updates to AI instances that are far away from the player.
/// You can also share clock instances by multiple trees if you like.
/// </summary>
public class NPBehaveExampleClockThrottling : MonoBehaviour
{
    // tweak this value to control how often your tree is ticked
    public float updateFrequency = 1.0f; // 1.0f = every second

    private Clock myThrottledClock;
    private Root behaviorTree;
    private float accumulator = 0.0f;

    void Start()
    {
        Node mainTree = new Service(() => { Debug.Log("Test"); },
            new WaitUntilStopped()
        );
        myThrottledClock = new Clock();
        behaviorTree = new Root(new Blackboard(myThrottledClock), myThrottledClock, mainTree);
        behaviorTree.Start();
    }

    void Update()
    {
        accumulator += Time.deltaTime;
        if (accumulator > updateFrequency)
        {
            accumulator -= updateFrequency;
            myThrottledClock.Update(updateFrequency);
        }
    }
}
