# NPBehave - An event driven behaviour tree library for code based AIs in Unity

![NPBehave Logo](http://labs.nkuebler.de/npbehave/images/np-behave.png)

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
    new Service(0.5f, () => { behaviorTree.Blackboard["foo"] = !behaviorTree.Blackboard.Get<bool>("foo"); },
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

## Stops Rules

Some [Decorators](#node-types) such as `BlackboardCondition`, `Condition` or `BlackboardQuery` have a `stopsOnChange` parameter that allows to define stop rules. The parameter allows the `Decorator` to stop the execution of a running subtree within it's parent's [Composite](#node-types). It is your main tool to make power of the event-drivenness in NPBehave. 

A **lower priority node** is a node that is defined after the current node within it's parent's [Composite]. 

The most useful and commonly used stops rules are `SELF`, `IMMEDIATE_RESTART` or `LOWER_PRIORITY_IMMEDIATE_RESTART`. 

Be careful if you're used to Unreal, in NPBehave `BOTH` and `LOWER_PRIORITY` have a slightly different meaning. IMMEDIATE_RESTART matches Unreal's `both` and LOWER_PRIORITY_IMMEDIATE_RESTART matches Unreal's `lower priority`.

The following stop rules exist:

* `Stops.NONE`: the Decorator will only check it's condition once it is started and will never stop any running nodes.
* `Stops.SELF`: the Decorator will check it's condition once it is started and if it is met, it will observe the `Blackboard` for changes. Once the condition is no longer met, it will stop itself allowing the parent Composite to **proceed with it's next node**.
* `Stops.LOWER_PRIORITY`: the Decorator will check it's condition once it is started and if it's not met, it will observe the `Blackboard` for changes. Once the condition is met, it will stop the lower priority node allowing the parent Composite to **proceed with it's next node**.
* `Stops.BOTH`: the Decorator will stop both: self and lower priority nodes.
* `Stops.LOWER_PRIORITY_IMMEDIATE_RESTART`: the Decorator will check it's condition once it is started and if it's not met, it will observe the `Blackboard` for changes. Once the condition is met, it will stop the lower priority node and order the parent Composite to **restart the Decorator immediately**. 
* `Stops.IMMEDIATE_RESTART`: the Decorator will check it's condition once it is started and if it's not met, it will observe the `Blackboard` for changes. Once the condition is met, it will stop the lower priority node and order the parent Composite to **restart the Decorator immediately**. As in `BOTH` it will also stop itself as soon as the condition is no longer met.

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

![NPBehave Debugger](http://labs.nkuebler.de/npbehave/images/npbehave-dbg.png)

*Check out the [sample](https://github.com/meniku/NPBehave/blob/master/Examples/Scripts/NPBehaveExampleEnemyAI.cs)*
*Special thanks to [Xerios](https://github.com/Xerios) for the great improvements*

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
- `Root(Node mainNode)`: run the given `mainNode` endlessly, regardless of it's failure state
- `Root(Blackboard blackboard, Node mainNode)`: use the given `blackboard` instead of instantiating one; run the given `mainNode` endlessly, regardless of it's failure state
- `Root(Blackboard blackboard, Clock clock, Node mainNode)`: use the given `blackboard` instead of instantiating one; use the given `clock` instead of using the global clock from the `UnityContext`; run the given `mainNode` endlessly, regardless of it's success state

### Composite Nodes

#### Selector
- `Selector(params Node[] children)`: Run children sequentially until one succeeds and succeed (succeeds if one of the children succeeds).

#### Composite
- `Sequence(params Node[] children)`: Run children sequentially until one fails and fail (succeeds if non of the children fails).

#### Parallel
- `Parallel(Policy successPolicy, Policy failurePolicy, params Node[] children)`: Run children in parallel. When `failurePolocity` is `Polociy.ONE`, the Parallel will stop (with failing resi;t) as soon as one of the children fails. When `successPolicy` is `Policy.ONE`, the Parallel will stop (with succeeding result) when of the children fails. If the Parallel doesn't stop because of a `Policy.ONE` it will execute until all of the children are done, then it either succeeds if all children succeeded or fails.

### Task Nodes

#### Action
- `Action(System.Action action)`: fire and forget action (always finishes successfully)
- `Action(System.Func<bool> singleFrameFunc)`: action which can succeed or fail (return false to fail) 
- `Action(Func<bool, Result> multiframeFunc)`: action that can be ticked over multiple frames (return Result.SUCCESS, Result.FAILED or Result.PROGRESS depending on the Action's current state)

#### NavWalkTo (!!!! EXPERIMENTAL !!!!)
- `NavMoveTo(NavMeshAgent agent, string blackboardKey, float tolerance = 1.0f, bool stopOnTolerance = false, float updateFrequency = 0.1f, float updateVariance = 0.025f)`: move a NavMeshAgent `agent` to either a transform or vector stored in the given `blackboardKey`. Allows a `tolerance` distance to succeed and optionally will stop once in the tolerance range (`stopOnTolerance`). `updateFrequency` controls how often the target position will be updated and how often the task checks wether it's done.

#### Wait
- `Wait(float seconds)`: Wait for given seconds with a random variance of 0.05 * seconds
- `Wait(float seconds, float randomVariance)``: Wait for given seconds with given random varaince
- `Wait(string blackboardKey, float randomVariance = 0f)`: wait for seconds set as float in given blackboardKey
- `Wait(System.Func<float> function, float randomVariance = 0f)`: wait for result of the provided lambda function

#### WaitUntilStopped
- `WaitUntilStopped(bool sucessWhenStopped = false)`: just wait until stopped by some other node. It's often used to park the behavior at the end of a `Selector`, waiting for any beforehead sibling `BlackboardCondition` to become true.

### Decorator Nodes

#### BlackboardCondition
- `BlackboardCondition(string key, Operator operator, object value, Stops stopsOnChange, Node decoratee)`: execute the `decoratee` node only if the Blackboard's `key` matches the `op` / `value` condition. If `stopsOnChange` is not NONE, the node will observe the Blackboard for changes and stop execution of running nodes based on the [`stopsOnChange` stops rules](#stops-rules).
- `BlackboardCondition(string key, Operator operator, Stops stopsOnChange, Node decoratee)`: execute the `decoratee` node only if the Blackboard's `key` matches the `op` condition (for one operand operators that just check for IS_SET for example). If `stopsOnChange` is not NONE, the node will observe the Blackboard for changes and stop execution of running nodes based on the [`stopsOnChange` stops rules](#stops-rules).

#### BlackboardQuery
- `BlackboardQuery(string[] keys, Stops stopsOnChange, System.Func<bool> query, Node decoratee)`: while `BlackboardCondition` allows to check only one key, this one will observe multiple Blackboard keys and evaluate the given `query` function as soon as one of the value's changes allowing you to do arbitrary queries on the Blackboard. It will stop running nodes based on the [`stopsOnChange` stops rules](#stops-rules).

#### Condition
- `Condition(Func<bool> condition, Node decoratee)`: execute `decoratee` node if the given condition returns true
- `Condition(Func<bool> condition, Stops stopsOnChange, Node decoratee)`: execute `decoratee` node if the given condition returns true. Re-Evaluate the condition every frame and stop running nodes based on the [`stopsOnChange` stops rules](#stops-rules).
- `Condition(Func<bool> condition, Stops stopsOnChange, float checkInterval, float randomVariance, Node decoratee)`: execute `decoratee` node if the given condition returns true. Re-Evaluate the condition at the given `checkInterval` and `randomVariance` and stop running nodes based on the [`stopsOnChange` stops rules](#stops-rules).
	
#### Cooldown
- `Cooldown(float cooldownTime, Node decoratee)`: Run `decoratee` immediately, but only if last execution wasn't at least past `cooldownTime`
- `Cooldown(float cooldownTime, float randomVariation, Node decoratee)`: Run `decoratee` immediately, but only if last execution wasn't at least past `cooldownTime` with `randomVariation``
- `Cooldown(float cooldownTime, bool startAfterDecoratee, bool resetOnFailiure, Node decoratee)`: Run `decoratee` immediately, but only if last execution wasn't at least past `cooldownTime` with `randomVariation`. When `resetOnFailure` is true, the cooldown will be reset if the decorated node fails
- `Cooldown(float cooldownTime, float randomVariation, bool startAfterDecoratee, bool resetOnFailiure, Node decoratee)` Run `decoratee` immediately, but only if last execution wasn't at least past `cooldownTime` with `randomVariation`. When `startAfterDecoratee` is true, the cooldown timer will be started after the decoratee finishes instead of when it starts. When `resetOnFailure` is true, the cooldown will be reset if the decorated node fails

#### Failer
- `Failer(Node decoratee)`: always fail, regardless of wether the `decoratee` fails or not

#### Inverter
- `Inverter(Node decoratee)`: if `decoratee` suceeds, the Inverter fails and if the `decoratee` fails, the Inverter suceeds.

#### Observer
- `Observer(Action onStart, Action<bool> onStop, Node decoratee)`: runs the given `onStop` lambda once the `decoratee` finishes, passing the result as parameter.

#### Random
- `Random(float probability, Node decoratee)`: runs the `decoratee` with the given `probability` chance between 0 and 1.

#### Repeater
- `Repeater(Node decoratee)`: repeat the given `decoratee` infinitly, unless it fails
- `Repeater(int loopCount, Node decoratee)`: execute the given `decoratee` for `loopCount` times (0 means decoratee would never run). If `decoratee` stops the looping is aborted and the Repeater fails. If all executions of the `decoratee` are successful, the Repeater will suceed.

#### Service
- `Service(Action service, Node decoratee)`: run the given `service` function, start the `decoratee` and then run the `service` every tick.
- `Service(float interval, Action service, Node decoratee)`: run the given `service` function, start the `decoratee` and then run the `service` at the given `interval`.
- `Service(float interval, float randomVariation, Action service, Node decoratee)`: run the given `service` function, start the `decoratee` and then run the `service` at the given `interval` with `randomVariation`.

#### Succeeder
- `Succeeder(Node decoratee)`: always suceed, regardless of wether the `decoratee` suceeds or not

#### TimeMax
- `TimeMax(float limit, bool waitForChildButFailOnLimitReached, Node decoratee)`: run the given `decoratee`. If the `decoratee` doesn't finish within the `limit`, the execution fails. If `waitForChildButFailOnLimitReached` is true, it will wait for the decoratee to finish but still fail.
- `TimeMax(float limit, float randomVariation, bool waitForChildButFailOnLimitReached, Node decoratee)`: run the given `decoratee`. If the `decoratee` doesn't finish within the `limit` and `randomVariation`, the execution fails. If `waitForChildButFailOnLimitReached` is true, it will wait for the decoratee to finish but still fail.

#### TimeMin
- TimeMin(float limit, Node decoratee)
- TimeMin(float limit, bool waitOnFailure, Node decoratee)
- TimeMin(float limit, float randomVariation, bool waitOnFailure, Node decoratee)

#### WaitForCondition
- `WaitForCondition(Func<bool> condition, Node decoratee)`: Delay execution of the `decoratee` node until the `condition` gets true, checking every frame
- `WaitForCondition(Func<bool> condition, float checkInterval, float randomVariance, Node decoratee)`: Delay execution of the `decoratee` node until the `condition` gets true, checking with the given `checkInterval` and `randomVariance`

## Video Tutorials

* [Tutorial 01](https://www.youtube.com/watch?v=JZbesN2_-fE): Getting started
* [Tutorial 02](https://www.youtube.com/watch?v=0jKz9WqF24c): Simple Patrolling AI
