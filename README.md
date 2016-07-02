# NPBehave - An event driven behaviour tree library for Unity 3D
NPBehave brings event driven behavior trees to Unity. 

It aims to be:

- lightweight, fast & simple
- event driven
- easily extendable
- A framework to define AIs with code, there is no visual editing support right now

NPBehave builds on the powerful and flexible code driven approach from the [C# BehaviorLibrary](https://github.com/DeveloperUX/BehaviorLibrary) and mixes in some of the great concepts of [Unreal's behaviour trees](https://docs.unrealengine.com/latest/INT/Engine/AI/BehaviorTrees/QuickStart/). Unlike traditional behaviour trees, event driven behaviour trees do not need to be traversed from the root node again each frame. They stay in their current state and only continue to traverse when they actually need to. This makes them more performant and a lot simpler to use.

In NPBehave you will find most node types from traditional behavior trees, but also some similar to those found in the Unreal engine. Please refer to the [Node Type Reference](#Node-Type-Reference) for the full list. It's fairly easy to add your own custom node types though.

If you don't know anything about behavior trees, it's highly recommended that you gain some theory first, [this Gamasutra article](http://www.gamasutra.com/blogs/ChrisSimpson/20140717/221339/Behavior_trees_for_AI_How_they_work.php) is a good read.

## Installation
Just drop the `NPBehave` folder into your unity project. There is also an `Examples` subfolder, with some example scenes you may want to check out.

## Example: "Hello World" Behaviour Tree
Let's start with an example:

```
public class HelloWorld : MonoBehaviour
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
```

*[Full sample ](https://github.com/meniku/NPBehave/blob/master/Examples/Scripts/NPBtrExampleHelloWorldAI.cs)*

When you run this, you'll notice that "Hello World" will be printed over and over again. This is because the `NPBtrRoot` node will restart the whole tree once traversal bypasses the last node in the tree. If you don't want this, you might add a `NPBtrWaitUntilStopped` node, like so:

```
...
behaviourTree = new NPBtrRoot(
	new NPBtrSequence(
		new NPBtrAction(() => Debug.Log("Hello World!")),
		new NBtrWaitUntilStops()
	)
);
... 

```

Up until now there really isn't anything event driven in this tree. Before we can dig into this, you need to understand what blackboards are.

## Blackboards
In NPBehave, like in Unreal, we got blackboards. You can think about them as beeing the "Brain" of your AI. In NPBehave, blackboards are basically dictionaries that can be observed for changes. We mainly use `NPBtrService` to store & update values in the blackboards. And we use `NPBtrBlackboardValue` to observe the blackboard for changes and in turn continue traversing the bahaviour tree. Though you are free to access or modify values of the blackboard everywhere else (you'll also access them often from `NPBtrAction` nodes).

A blackboard is automatically created when you instantiate a `NPBtrRoot`, but you may also provide another instance with it's constructor (this is particulary useful for [Shared Blackboards](#Shared-Blackboards))

## Example: An event-driven behavior tree
Here's a simple example that uses the blackboard for event-driven behavior:

```
...
behaviorTree = new NPBtrRoot(
    new NPBtrService(0.5f, () => { behaviorTree.Blackboard.Set("foo", !behaviorTree.Blackboard.GetBool("foo")); },
        new NPBtrSelector(
        
            new NPBtrBlackboardValue("foo", NPBtrOperator.IS_EQUAL, true, NPBtrStops.IMMEDIATE_RESTART,
                new NPBtrSequence(
                    new NPBtrAction(() => Debug.Log("foo")),
                    new NPBtrWaitUntilStopped()
                )
            ),

            new NPBtrSequence(
                new NPBtrAction(() => Debug.Log("bar")),
                new NPBtrWaitUntilStopped()
            )
        )
    )
);
behaviorTree.Start();
...

```
*[Full sample](https://github.com/meniku/NPBehave/blob/master/Examples/Scripts/NPBtrExampleHelloBlackboardsAI.cs)*
| *[More sophisticated example](https://github.com/meniku/NPBehave/blob/master/Examples/Scripts/NPBtrExampleEnemyAI.cs)*

This sample will swap between printing "foo" and "bar" every 500 milliseconds. We use a `NPBtrService` decorator node to toggle the `foo` boolean value in the blackboard. We use a `NPBtrBlackboardValue` decorator node to decide based on this flag whether the branch gets executed or not. The `NPBtrBlackboardValue` also watches the blackboard for changes based on this value and as we provided `NPBtrStops.IMMEDIATE_RESTART` the currently executed branch will be stopped if the condition no longer is true, also if it becomes true again, it will be restarted immediately.

*Please note that you should put services in real methods instead of using lambdas, this will make your trees more readable. Same is true for larger actions.*

## Node execution results
In NPBehave a node can either `succeed` or `fail`. Unlike traditional behavior trees, there is no result while a node is executing. Instead the node will itself tell the parent node once it is finished. This is important to keep in mind when you [create your own node types](#Extending-the-Library).

## Node Types
In NPBehave we have four different node types:

- **the root node**: The root has one single child and is used to start or stop the whole behavior tree.
- **composite nodes**: these have multiple children and are used to control which of their children are executed. Also the order and result is defined by this kind of node.
- **decorator nodes**: these nodes have always exactly one child and are used to either modify the result of the child or do something else while executing the child (e.g. a service updating the blackboard)
- **task nodes**: these are the leafs of the tree doing the actual work. These are the ones you most likely would create custom classes for. You can use the `NPBtrAction` with lambdas or functions - For more sophisticated tasks, extending `NPBtrTask` by yourself may be a better option. Be sure to read the [the golden rules](#The-golden-rules) if you do so.

## The Debugger
You can use the `NPBtrDebugger` component to debug the behavior trees at runtime in the inspector. 

*Check out the *[sample](https://github.com/meniku/NPBehave/blob/master/Examples/Scripts/NPBtrExampleEnemyAI.cs)*.*

## Shared Blackboards
You have the option to share blackboards across multiple instances of an AI. This can be useful if you want to implement some kind of swarm behavior. Additionally, you can create blackboard hierarchies, which allows you to combine a shared with a non-shared blackboard. 

You can use `NPBtrUnityContext.GetSharedBlackboard(name)` to access shared blackboard instances anywhere. 

*Check out the *[sample](https://github.com/meniku/NPBehave/blob/master/Examples/Scripts/NPBtrExampleSwarmEnemyAI.cs)*.*

## Extending the Library
Please refer to the existing node implememtations to find out how to create custom node types, however be sure to at least read the following golden rules before doing so.

### The golden rules

1. **Every call to `DoStop()` must result in a call to `Stopped(result)`**. This is extremly important!: you really need to ensure that Stopped() is called within DoStop(), because NPBehave needs to be able to cancel a running branch at every time *immediately*. This also means that all your child nodes will also call Stopped(), which in turn makes it really easy to write reliable decorators or even composite nodes: Within DoStop() you just call `Stop()` on your active children, they in turn will call of `ChildStopped()` on your node where you then finally put in your Stopped() call. Please have a look at the existing implementations for reference.
2. **Every registered Clock or Blackboard observer needs to be removed eventually**. Most of the time you unregister your callbacks at the same location you also call Stopped(), however there may be exceptions to this rule (e.g. the NPBtrBlackboardValue keeps observers around up until the parent composite is stopped, it needs to be able to react on blackboard value changes even when the node itself is not active) 

### Node States
Most likely you won't need to access those, but it's still good to know about them:

* ACTIVE: the node is started and not yet stopped.
* STOP_REQUESTED: the node is currently stopping, but has not yet called Stopped() to notify the parent.
* INACTIVE: the node is stopped.

The current state can be retrieved with the `CurrentState` property

### The Clock

When the blackboard is the brain of the behavior tree, the `clock` is the heart. However there is only one single instance, so it's more like a big shared heart for all of your behaviours.

You can use the clock in your nodes to register timers or get notified on each frame. Use `RootNode.Clock` to access the clock. 

## Node Type Reference

*more detailed information coming soon*

### Root
- NPBtrRoot(NPBtrNode mainNode) 
- NPBtrRoot(NPBtrBlackboard blackboard, NPBtrNode mainNode)
- NPBtrRoot(NPBtrBlackboard blackboard, NPBtrClock clock, NPBtrNode mainNode)

### Composite Nodes

#### Selector
- NPBtrSelector(params NPBtrNode[] children)

#### Composite
- NPBtrSequence(params NPBtrNode[] children) 

#### Parallel
- NPBtrParallel(Policy successPolicy, Policy failurePolicy, params NPBtrNode[] children) 

### Task Nodes

#### Action
- NPBtrAction(Action action)
- NPBtrAction(Func<bool, Result> multiframeFunc)

#### Wait
- NPBtrWait(float seconds)
- NPBtrWait(float seconds, float randomVariance)

#### WaitUntilStopped
- NPBtrWaitUntilStopped(bool sucessWhenStopped = false)

### Decorator Nodes

#### BlackboardValue
- NPBtrBlackboardValue(string key, NPBtrOperator op, object value, NPBtrStops stopsOnChange, NPBtrNode decoratee)
- NPBtrBlackboardValue(string key, NPBtrOperator op, NPBtrStops stopsOnChange, NPBtrNode decoratee)

#### Condition
- NPBtrCondition(Func<bool> condition, NPBtrNode decoratee)

#### Cooldown
- NPBtrCooldown(float cooldownTime, NPBtrNode decoratee)
- NPBtrCooldown(float cooldownTime, float randomVariation, NPBtrNode decoratee)
- NPBtrCooldown(float cooldownTime, bool startAfterDecoratee, bool resetOnFailiure, NPBtrNode decoratee)
- NPBtrCooldown(float cooldownTime, float randomVariation, bool startAfterDecoratee, bool resetOnFailiure, NPBtrNode decoratee)

#### Failer
- NPBtrFailer(NPBtrNode decoratee)

#### Inverter
- NPBtrInverter(NPBtrNode decoratee)

#### Observer
- NPBtrObserver(Action onStart, Action<bool> onStop, NPBtrNode decoratee)

#### Random
- NPBtrRandom(float probability, NPBtrNode decoratee)

#### Repeater
- NPBtrRepeater(NPBtrNode decoratee)
- NPBtrRepeater(int repeatTimes, NPBtrNode decoratee) 

#### Service
- NPBtrService(Action service, NPBtrNode decoratee)
- NPBtrService(float interval, Action service, NPBtrNode decoratee)
- NPBtrService(float interval, float randomVariation, Action service, NPBtrNode decoratee)

#### Succeeder
- NPBtrSucceeder(NPBtrNode decoratee)

#### TimeMax
- NPBtrTimeMax(float limit, bool waitForChildButFailOnLimitReached, NPBtrNode decoratee)
- NPBtrTimeMax(float limit, float randomVariation, bool waitForChildButFailOnLimitReached, NPBtrNode decoratee)

#### TimeMin
- NPBtrTimeMin(float limit, NPBtrNode decoratee)
- NPBtrTimeMin(float limit, bool waitOnFailure, NPBtrNode decoratee)
- NPBtrTimeMin(float limit, float randomVariation, bool waitOnFailure, NPBtrNode decoratee)
