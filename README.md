# NPBehave - An event driven behaviour tree library for code based AIs in Unity
NPBehave aims to be:

- lightweight, fast & simple
- event driven
- easily extendable
- A framework to define AIs with code, there is no visual editing support

NPBehave builds on the powerful and flexible code based approach to define behavior trees from the [BehaviorLibrary](https://github.com/DeveloperUX/BehaviorLibrary) and mixes in some of the great concepts of [Unreal's behaviour trees](https://docs.unrealengine.com/latest/INT/Engine/AI/BehaviorTrees/QuickStart/). Unlike traditional behaviour trees, event driven behaviour trees do not need to be traversed from the root node again each frame. They stay in their current state and only continue to traverse when they actually need to. This makes them more performant and a lot simpler to use.

In NPBehave you will find most node types from traditional behavior trees, but also some similar to those found in the Unreal engine. Please refer to the [Node Type Reference](#node-type-reference) for the full list. It's fairly easy to add your own custom node types though.

If you don't know anything about behavior trees, it's highly recommended that you gain some theory first, [this Gamasutra article](http://www.gamasutra.com/blogs/ChrisSimpson/20140717/221339/Behavior_trees_for_AI_How_they_work.php) is a good read.

## Installation
Just drop the `NPBehave` folder into your Unity project. There is also an `Examples` subfolder, with some example scenes you may want to check out.

## Example: "Hello World" Behaviour Tree
Let's start with an example:

```
using namespace NPBehave;

public class HelloWorld : MonoBehaviour
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
```

*[Full sample ](https://github.com/meniku/NPBehave/blob/master/Examples/Scripts/NPBehaveExampleHelloWorldAI.cs)*

When you run this, you'll notice that "Hello World" will be printed over and over again. This is because the `Root` node will restart the whole tree once traversal bypasses the last node in the tree. If you don't want this, you might add a `WaitUntilStopped` node, like so:

```
...
behaviourTree = new Root(
	new Sequence(
		new Action(() => Debug.Log("Hello World!")),
		new WaitUntilStopped()
	)
);
... 

```

Up until now there really isn't anything event driven in this tree. Before we can dig into this, you need to understand what blackboards are.

## Blackboards
In NPBehave, like in Unreal, we got blackboards. You can think about them as beeing the "memory" of your AI. In NPBehave, blackboards are basically dictionaries that can be observed for changes. We mainly use `Service` to store & update values in the blackboards. And we use `BlackboardCondition` to observe the blackboard for changes and in turn continue traversing the bahaviour tree. Though you are free to access or modify values of the blackboard everywhere else (you'll also access them often from `Action` nodes).

A blackboard is automatically created when you instantiate a `Root`, but you may also provide another instance with it's constructor (this is particulary useful for [Shared Blackboards](#Shared-Blackboards))

## Example: An event-driven behavior tree
Here's a simple example that uses the blackboard for event-driven behavior:

```
...
behaviorTree = new Root(
    new Service(0.5f, () => { behaviorTree.Blackboard.Set("foo", !behaviorTree.Blackboard.GetBool("foo")); },
        new Selector(
        
            new BlackboardCondition("foo", Operator.IS_EQUAL, true, Stops.IMMEDIATE_RESTART,
                new Sequence(
                    new Action(() => Debug.Log("foo")),
                    new WaitUntilStopped()
                )
            ),

            new Sequence(
                new Action(() => Debug.Log("bar")),
                new WaitUntilStopped()
            )
        )
    )
);
behaviorTree.Start();
...

```
*[Full sample](https://github.com/meniku/NPBehave/blob/master/Examples/Scripts/NPBehaveExampleHelloBlackboardsAI.cs)*
| *[More sophisticated example](https://github.com/meniku/NPBehave/blob/master/Examples/Scripts/NPBehaveExampleEnemyAI.cs)*

This sample will swap between printing "foo" and "bar" every 500 milliseconds. We use a `Service` decorator node to toggle the `foo` boolean value in the blackboard. We use a `BlackboardCondition` decorator node to decide based on this flag whether the branch gets executed or not. The `BlackboardCondition` also watches the blackboard for changes based on this value and as we provided `Stops.IMMEDIATE_RESTART` the currently executed branch will be stopped if the condition no longer is true, also if it becomes true again, it will be restarted immediately.

*Please note that you should put services in real methods instead of using lambdas, this will make your trees more readable. Same is true for larger actions.*

## Node execution results
In NPBehave a node can either `succeed` or `fail`. Unlike traditional behavior trees, there is no result while a node is executing. Instead the node will itself tell the parent node once it is finished. This is important to keep in mind when you [create your own node types](#extending-the-library).

## Node Types
In NPBehave we have four different node types:

- **the root node**: The root has one single child and is used to start or stop the whole behavior tree.
- **composite nodes**: these have multiple children and are used to control which of their children are executed. Also the order and result is defined by this kind of node.
- **decorator nodes**: these nodes have always exactly one child and are used to either modify the result of the child or do something else while executing the child (e.g. a service updating the blackboard)
- **task nodes**: these are the leafs of the tree doing the actual work. These are the ones you most likely would create custom classes for. You can use the `Action` with lambdas or functions - For more complicated tasks, it is often a better option to create a new subclass of `Task`. Be sure to read the [the golden rules](#the-golden-rules) if you do so.

## The Debugger
You can use the `Debugger` component to debug the behavior trees at runtime in the inspector. 

*Check out the [sample](https://github.com/meniku/NPBehave/blob/master/Examples/Scripts/NPBehaveExampleEnemyAI.cs)*

## Shared Blackboards
You have the option to share blackboards across multiple instances of an AI. This can be useful if you want to implement some kind of swarm behavior. Additionally, you can create blackboard hierarchies, which allows you to combine a shared with a non-shared blackboard. 

You can use `UnityContext.GetSharedBlackboard(name)` to access shared blackboard instances anywhere. 

*Check out the [sample](https://github.com/meniku/NPBehave/blob/master/Examples/Scripts/NPBehaveExampleSwarmEnemyAI.cs)*

## Extending the Library
Please refer to the existing node implementations to find out how to create custom node types, however be sure to at least read the following golden rules before doing so.

### The golden rules

1. **Every call to `DoStop()` must result in a call to `Stopped(result)`**. This is extremly important!: you really need to ensure that Stopped() is called within DoStop(), because NPBehave needs to be able to cancel a running branch at every time *immediately*. This also means that all your child nodes will also call Stopped(), which in turn makes it really easy to write reliable decorators or even composite nodes: Within DoStop() you just call `Stop()` on your active children, they in turn will call of `ChildStopped()` on your node where you then finally put in your Stopped() call. Please have a look at the existing implementations for reference.
2. **Stopped() is the last call you do**, never do modify any state or call anything after calling Stopped. This is because Stopped will immediately continue traversal of the tree on other nodes, which will completly fuckup the state of the behavior tree if you don't take that into account.
3. **Every registered clock or blackboard observer needs to be removed eventually**. Most of the time you unregister your callbacks immediately before you call Stopped(), however there may be exceptions, e.g. the BlackboardCondition keeps observers around up until the parent composite is stopped, it needs to be able to react on blackboard value changes even when the node itself is not active.

### Node States
Most likely you won't need to access those, but it's still good to know about them:

* ACTIVE: the node is started and not yet stopped.
* STOP_REQUESTED: the node is currently stopping, but has not yet called Stopped() to notify the parent.
* INACTIVE: the node is stopped.

The current state can be retrieved with the `CurrentState` property

### The Clock

You can use the clock in your nodes to register timers or get notified on each frame. Use `RootNode.Clock` to access the clock. 

## Node Type Reference

### Root
- Root(Node mainNode) 
- Root(Blackboard blackboard, Node mainNode)
- Root(Blackboard blackboard, Clock clock, Node mainNode)

### Composite Nodes

#### Selector
- Selector(params Node[] children)

#### Composite
- Sequence(params Node[] children) 

#### Parallel
- Parallel(Policy successPolicy, Policy failurePolicy, params Node[] children) 

### Task Nodes

#### Action
- Action(Action action)
- Action(Func<bool, Result> multiframeFunc)

#### NavWalkTo (!!!! EXPERIMENTAL !!!!)
- NavMoveTo(NavMeshAgent agent, string blackboardKey, float tolerance = 1.0f, bool stopOnTolerance = false, float updateFrequency = 0.1f, float updateVariance = 0.025f)

#### Wait
- Wait(float seconds)
- Wait(float seconds, float randomVariance)

#### WaitUntilStopped
- WaitUntilStopped(bool sucessWhenStopped = false)

### Decorator Nodes

#### BlackboardCondition
- BlackboardCondition(string key, Operator op, object value, Stops stopsOnChange, Node decoratee)
- BlackboardCondition(string key, Operator op, Stops stopsOnChange, Node decoratee)

#### BlackboardQuery
- BlackboardQuery(string[] observedKeys, Stops stopsOnChange, System.Func<bool> query, Node decoratee

#### Condition
- Condition(Func<bool> condition, Node decoratee)

#### Cooldown
- Cooldown(float cooldownTime, Node decoratee)
- Cooldown(float cooldownTime, float randomVariation, Node decoratee)
- Cooldown(float cooldownTime, bool startAfterDecoratee, bool resetOnFailiure, Node decoratee)
- Cooldown(float cooldownTime, float randomVariation, bool startAfterDecoratee, bool resetOnFailiure, Node decoratee)

#### Failer
- Failer(Node decoratee)

#### Inverter
- Inverter(Node decoratee)

#### Observer
- Observer(Action onStart, Action<bool> onStop, Node decoratee)

#### Random
- Random(float probability, Node decoratee)

#### Repeater
- Repeater(Node decoratee)
- Repeater(int repeatTimes, Node decoratee) 

#### Service
- Service(Action service, Node decoratee)
- Service(float interval, Action service, Node decoratee)
- Service(float interval, float randomVariation, Action service, Node decoratee)

#### Succeeder
- Succeeder(Node decoratee)

#### TimeMax
- TimeMax(float limit, bool waitForChildButFailOnLimitReached, Node decoratee)
- TimeMax(float limit, float randomVariation, bool waitForChildButFailOnLimitReached, Node decoratee)

#### TimeMin
- TimeMin(float limit, Node decoratee)
- TimeMin(float limit, bool waitOnFailure, Node decoratee)
- TimeMin(float limit, float randomVariation, bool waitOnFailure, Node decoratee)
