[//]: # "******************************************************************************"
[//]: # "THIS DOCUMENTATION IS BEST VIEWED ONLINE AT https://github.com/meniku/NPBehave"
[//]: # "******************************************************************************"

# NPBehave - An event driven Behavior Tree Library for code based AIs in Unity

![NPBehave Logo](http://labs.nkuebler.de/npbehave/images/np-behave.png)

NPBehave aims to be:

- lightweight, fast & simple
- event driven
- easily extendable
- A framework to define AIs with code, there is no visual editing support

NPBehave builds on the powerful and flexible code based approach to define behavior trees from the [BehaviorLibrary](https://github.com/DeveloperUX/BehaviorLibrary) and mixes in some of the great concepts of [Unreal's behavior trees](https://docs.unrealengine.com/latest/INT/Engine/AI/BehaviorTrees/QuickStart/). Unlike traditional behavior trees, event driven behavior trees do not need to be traversed from the root node again each frame. They stay in their current state and only continue to traverse when they actually need to. This makes them more performant and a lot simpler to use.

In NPBehave you will find most node types from traditional behavior trees, but also some similar to those found in the Unreal engine. Please refer to the [Node Type Reference](#node-type-reference) for the full list. It's fairly easy to add your own custom node types though.

If you don't know anything about behavior trees, it's highly recommended that you gain some theory first, [this Gamasutra article](http://www.gamasutra.com/blogs/ChrisSimpson/20140717/221339/Behavior_trees_for_AI_How_they_work.php) is a good read.

## Installation
Just drop the `NPBehave` folder into your Unity project. There is also an `Examples` subfolder, with some example scenes you may want to check out.

## Example: "Hello World" Behavior Tree
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
behaviorTree = new Root(
	new Sequence(
		new Action(() => Debug.Log("Hello World!")),
		new WaitUntilStopped()
	)
);
... 

```

Up until now there really isn't anything event driven in this tree. Before we can dig into this, you need to understand what Blackboards are.

## Blackboards
In NPBehave, like in Unreal, we got blackboards. You can think about them as beeing the "memory" of your AI. In NPBehave, blackboards are basically dictionaries that can be observed for changes. We mainly use `Service` to store & update values in the blackboards. And we use `BlackboardCondition` or `BlackboardQuery` to observe the blackboard for changes and in turn continue traversing the bahaviour tree. Though you are free to access or modify values of the blackboard everywhere else (you'll also access them often from `Action` nodes).

A blackboard is automatically created when you instantiate a `Root`, but you may also provide another instance with it's constructor (this is particularly useful for [Shared Blackboards](#Shared-Blackboards))

### Example: An event-driven behavior tree
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

A **lower priority node** is a node that is defined after the current node within it's parent [Composite](#node-types). 

The most useful and commonly used stops rules are `SELF`, `IMMEDIATE_RESTART` or `LOWER_PRIORITY_IMMEDIATE_RESTART`. 

Be careful if you're used to Unreal though. In NPBehave `BOTH` and `LOWER_PRIORITY` have a slightly different meaning. `IMMEDIATE_RESTART` actually matches Unreal's `Both` and `LOWER_PRIORITY_IMMEDIATE_RESTART` matches Unreal's `Lower Priority`.

The following stop rules exist:

* `Stops.NONE`: the decorator will only check it's condition once it is started and will never stop any running nodes.
* `Stops.SELF`: the decorator will check it's condition once it is started and if it is met, it will observe the blackboard for changes. Once the condition is no longer met, it will stop itself allowing the parent composite to **proceed with it's next node**.
* `Stops.LOWER_PRIORITY`: the decorator will check it's condition once it is started and if it's not met, it will observe the blackboard for changes. Once the condition is met, it will stop the lower priority node allowing the parent composite to **proceed with it's next node**.
* `Stops.BOTH`: the decorator will stop both: self and lower priority nodes.
* `Stops.LOWER_PRIORITY_IMMEDIATE_RESTART`: the decorator will check it's condition once it is started and if it's not met, it will observe the blackboard for changes. Once the condition is met, it will stop the lower priority node and order the parent composite to **restart the Decorator immediately**. 
* `Stops.IMMEDIATE_RESTART`: the decorator will check it's condition once it is started and if it's not met, it will observe the blackboard for changes. Once the condition is met, it will stop the lower priority node and order the parent composite to **restart the Decorator immediately**. As in `BOTH` it will also stop itself as soon as the condition is no longer met.

## Blackboard Alternatives

In NPBehave you define your behavior tree within a `MonoBehaviour`, as thus it isn't necessary to store everything in the blackboard. If you don't have `BlackboardDecorator` or `BlackboardQuery` with other stop rules than `Stops.NONE`, you probably don't need them to be in the blackboard at all. You can also just make use of plain member variables - it is often the cleaner, faster to write and more performant. It means that you won't make use of the event-drivenness of NPBehave in that case, but it's often not necessary.

If you want to be able to make use of [`stopsOnChange` stops rules](#stops-rules) without using the Blackboard, two alternative ways exist in NPBehave:

1. use a regular [`Condition` decorator](#stops-rules). This decorator has an optional [`stopsOnChange` stops rules](#stops-rules) parameter. When providing any other value than `Stops.NONE`, the condition will frequently check the condition and interrupt the node according to the stops rule when the result of the given `query` function changes. Be aware that this method is not event-driven, it queries every frame (or at the provided interval) and as thus may lead to many queries if you make heavy use of them. However for simple cases it is often is sufficient and much simpler than a combination of a Blackboard-Key, a `Service` and a `BlackboardCondition`.
2. Build your own event-driven `Decorator`s. It's actually pretty easy, just extend from [ObservingDecorator](#implementing-observing-decorators) and override the `isConditionMet()`, `StartObserving()` and `StopObserving()` methods.

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

![NPBehave Debugger](https://github.com/meniku/NPBehave/blob/master/README-Debugger.png)

*Check out the [sample](https://github.com/meniku/NPBehave/blob/master/Examples/Scripts/NPBehaveExampleEnemyAI.cs)*

## Shared Blackboards
You have the option to share blackboards across multiple instances of an AI. This can be useful if you want to implement some kind of swarm behavior. Additionally, you can create blackboard hierarchies, which allows you to combine a shared with a non-shared blackboard. 

You can use `UnityContext.GetSharedBlackboard(name)` to access shared blackboard instances anywhere. 

*Check out the [sample](https://github.com/meniku/NPBehave/blob/master/Examples/Scripts/NPBehaveExampleSwarmEnemyAI.cs)*

## Extending the Library
Please refer to the existing node implementations to find out how to create custom node types, however be sure to at least read the following golden rules before doing so.

### The golden rules

1. **Every call to `DoStop()` must result in a call to `Stopped(result)`**. This is extremely important!: you really need to ensure that Stopped() is called within DoStop(), because NPBehave needs to be able to cancel a running branch at every time *immediately*. This also means that all your child nodes will also call Stopped(), which in turn makes it really easy to write reliable decorators or even composite nodes: Within DoStop() you just call `Stop()` on your active children, they in turn will call of `ChildStopped()` on your node where you then finally put in your Stopped() call. Please have a look at the existing implementations for reference.
2. **Stopped() is the last call you do**, never do modify any state or call anything after calling Stopped. This is because Stopped will immediately continue traversal of the tree on other nodes, which will completley fuckup the state of the behavior tree if you don't take that into account.
3. **Every registered clock or blackboard observer needs to be removed eventually**. Most of the time you unregister your callbacks immediately before you call Stopped(), however there may be exceptions, e.g. the BlackboardCondition keeps observers around up until the parent composite is stopped, it needs to be able to react on blackboard value changes even when the node itself is not active.

### Implementing Tasks
For tasks you extend from the `Task` class and override the `DoStart()` and `DoStop()` methods. In `DoStart()` you start your logic and once you're done, you call `Stopped(bool result)` with the appropriate result. Your node may get cancelled by another node, so be sure to implement `DoStop()`, do proper cleanup and call `Stopped(bool result)` immediately after it.

For a relatively simple example, check the source of the [`Wait Task`](http://github.com/meniku/NPBehave/blob/master/Scripts/Task/Wait.cs).

*As already mentioned in the golden rules section, in NPBehave you have to always call Stopped(bool result) after your node is stopped. So it is currently not supported to have cancel-operations pending over multiple frames and will result in unpredictable behaviour*.

### Implementing Observing Decorators
Writing decorators is a lot more complex than Tasks. However a special base class exists for convenience. It's the `ObservingDecorator`. This class can be used for easy implementation of "conditional" `Decorators` that optionally make use [`stopsOnChange` stops rules](#stops-rules). 

All you have to do is to extend from it `ObservingDecorator` and override the method `bool IsConditionMet()`. If you want to support the `Stops-Rules` you will have to implement `StartObserving()` and `StopObserving()` too. For a simple example, check the source of the [`Condition Decorator`](http://github.com/meniku/NPBehave/blob/master/Scripts/Decorator/Condition.cs).

### Implementing Generic Decorators
For generic decorators you extend from the `Decorator` class and override the `DoStart()`, `DoStop()` and the `DoChildStopped(Node child, bool result)` methods. 

You can start or stop your decorated node by accessing the `Decoratee` property and call `Start()` or `Stop()` on it.

If your decorator receives a `DoStop()` call, it's responsible to stop the `Decoratee` accordingly and in this case will not call `Stopped(bool result)` immediately. Instead it will do that in the `DoChildStopped(Node child, bool result)` method. Be aware that the `DoChildStopped(Node child, bool result)` doesn't necessarily mean that your decorator stopped the decoratee, the decoratee may also stop itself, in which case you don't need to immediately stop the Decoratee (that may be useful if you want to implement things like cooldowns etc). To find out whether your decorator got stopped, you can query it's `IsStopRequested` property.

Check the source of the [`Failer Node`](http://github.com/meniku/NPBehave/blob/master/Scripts/Decorator/Failer.cs) for a very basic implementation or the [`Repeater Node`](http://github.com/meniku/NPBehave/blob/master/Scripts/Decorator/Repeater.cs) for a little more complex one.
 
In addition you can also implement the method `DoParentCompositeStopped()`, which may be called even when your Decorator is inactive. This is useful if you want to do additional cleanup work for listeners you kept active after your Decorator stopped. Check the [`ObservingDecorator`](http://github.com/meniku/NPBehave/blob/master/Scripts/Decorator/ObservingDecorator.cs) for an example.

### Implementing Composites
Composite nodes require a deeper understanding of the library and you usually won't need to implement new ones. If you really need a new Composite, feel free to create a ticket on the [GitHub project](http://github.com/meniku/NPBehave) or [contact](#contact) me and I'll try my best to help you getting through it correctly.

### Node States
Most likely you won't need to access those, but it's still good to know about them:

* ACTIVE: the node is started and not yet stopped.
* STOP_REQUESTED: the node is currently stopping, but has not yet called Stopped() to notify the parent.
* INACTIVE: the node is stopped.

The current state can be retrieved with the `CurrentState` property

### The Clock
You can use the clock in your nodes to register timers or get notified on each frame. Use `RootNode.Clock` to access the clock. Check the [`Wait Task`](http://github.com/meniku/NPBehave/blob/master/Scripts/Task/Wait.cs) for an example on how to register timers on the clock.

By default the behavior tree will be using the global clock privoded by the `UnityContext`. This clock is updated every frame.
There may be scenarious where you want to have more control. For example you may want to throttle or pause updates to a group of AIs. For this reason you can provide your own controlled clock instances to the `Root` node and `Blackboard`, this allows you to precisely control when your behavior trees are updated. Check the [Clock Throttling Example](https://github.com/meniku/NPBehave/blob/master/Examples/Scripts/NPBehaveExampleClockThrottling.cs).


## Node Type Reference

### Root
- **`Root(Node mainNode)`**: run the given `mainNode` endlessly, regardless of it's failure state
- **`Root(Blackboard blackboard, Node mainNode)`**: use the given `blackboard` instead of instantiating one; run the given `mainNode` endlessly, regardless of it's failure state
- **`Root(Blackboard blackboard, Clock clock, Node mainNode)`**: use the given `blackboard` instead of instantiating one; use the given `clock` instead of using the global clock from the `UnityContext`; run the given `mainNode` endlessly, regardless of it's success state

### Composite Nodes

#### Selector
- **`Selector(params Node[] children)`**: Run children sequentially until one succeeds and succeed (succeeds if one of the children succeeds).

#### Composite
- **`Sequence(params Node[] children)`**: Run children sequentially until one fails and fail (succeeds if non of the children fails).

#### Parallel
- **`Parallel(Policy successPolicy, Policy failurePolicy, params Node[] children)`**: Run children in parallel. When `failurePolocity` is `Polociy.ONE`, the Parallel will stop (with failing resi;t) as soon as one of the children fails. When `successPolicy` is `Policy.ONE`, the Parallel will stop (with succeeding result) when of the children fails. If the Parallel doesn't stop because of a `Policy.ONE` it will execute until all of the children are done, then it either succeeds if all children succeeded or fails.

### Task Nodes

#### Action
- **`Action(System.Action action)`**: fire and forget action (always finishes successfully immediately)
- **`Action(System.Func<bool> singleFrameFunc)`**: action which can succeed or fail (return false to fail) 
- **`Action(Func<bool, Result> multiframeFunc)`**: action that can be ticked over multiple frames (return Result.SUCCESS, Result.FAILED or Result.PROGRESS depending on the Action's current state)

#### NavWalkTo (!!!! EXPERIMENTAL !!!!)
- **`NavMoveTo(NavMeshAgent agent, string blackboardKey, float tolerance = 1.0f, bool stopOnTolerance = false, float updateFrequency = 0.1f, float updateVariance = 0.025f)`**: move a NavMeshAgent `agent` to either a transform or vector stored in the given `blackboardKey`. Allows a `tolerance` distance to succeed and optionally will stop once in the tolerance range (`stopOnTolerance`). `updateFrequency` controls how often the target position will be updated and how often the task checks wether it's done.

#### Wait
- **`Wait(float seconds)`**: Wait for given seconds with a random variance of 0.05 * seconds
- **`Wait(float seconds, float randomVariance)`**: Wait for given seconds with given random varaince
- **`Wait(string blackboardKey, float randomVariance = 0f)`**: wait for seconds set as float in given blackboardKey
- **`Wait(System.Func<float> function, float randomVariance = 0f)`**: wait for result of the provided lambda function

#### WaitUntilStopped
- **`WaitUntilStopped(bool sucessWhenStopped = false)`**: just wait until stopped by some other node. It's often used to park at the end of a `Selector`, waiting for any beforehead sibling `BlackboardCondition`, `BlackboardQuery` or `Condition` to become active.

### Decorator Nodes

#### BlackboardCondition
- **`BlackboardCondition(string key, Operator operator, object value, Stops stopsOnChange, Node decoratee)`**: execute the `decoratee` node only if the Blackboard's `key` matches the `op` / `value` condition. If `stopsOnChange` is not NONE, the node will observe the Blackboard for changes and stop execution of running nodes based on the [`stopsOnChange` stops rules](#stops-rules).
- **`BlackboardCondition(string key, Operator operator, Stops stopsOnChange, Node decoratee)`**: execute the `decoratee` node only if the Blackboard's `key` matches the `op` condition (for one operand operators that just check for IS_SET for example). If `stopsOnChange` is not NONE, the node will observe the Blackboard for changes and stop execution of running nodes based on the [`stopsOnChange` stops rules](#stops-rules).

#### BlackboardQuery
- **`BlackboardQuery(string[] keys, Stops stopsOnChange, System.Func<bool> query, Node decoratee)`**: while `BlackboardCondition` allows to check only one key, this one will observe multiple blackboard keys and evaluate the given `query` function as soon as one of the value's changes, allowing you to do arbitrary queries on the blackboard. It will stop running nodes based on the [`stopsOnChange` stops rules](#stops-rules).

#### Condition
- **`Condition(Func<bool> condition, Node decoratee)`**: execute `decoratee` node if the given `condition` returns true
- **`Condition(Func<bool> condition, Stops stopsOnChange, Node decoratee)`**: execute `decoratee` node if the given `condition` returns true. Re-Evaluate the condition every frame and stop running nodes based on the [`stopsOnChange` stops rules](#stops-rules).
- **`Condition(Func<bool> condition, Stops stopsOnChange, float checkInterval, float randomVariance, Node decoratee)`**: execute `decoratee` node if the given condition returns true. Reevaluate the condition at the given `checkInterval` and `randomVariance` and stop running nodes based on the [`stopsOnChange` stops rules](#stops-rules).
	
#### Cooldown
- **`Cooldown(float cooldownTime, Node decoratee)`**: Run `decoratee` immediately, but only if last execution wasn't at least past `cooldownTime`
- **`Cooldown(float cooldownTime, float randomVariation, Node decoratee)`**: Run `decoratee` immediately, but only if last execution wasn't at least past `cooldownTime` with `randomVariation`
- **`Cooldown(float cooldownTime, bool startAfterDecoratee, bool resetOnFailiure, Node decoratee)`**: Run `decoratee` immediately, but only if last execution wasn't at least past `cooldownTime` with `randomVariation`. When `resetOnFailure` is true, the cooldown will be reset if the decorated node fails
- **`Cooldown(float cooldownTime, float randomVariation, bool startAfterDecoratee, bool resetOnFailiure, Node decoratee)`** Run `decoratee` immediately, but only if last execution wasn't at least past `cooldownTime` with `randomVariation`. When `startAfterDecoratee` is true, the cooldown timer will be started after the decoratee finishes instead of when it starts. When `resetOnFailure` is true, the cooldown will be reset if the decorated node fails.

#### Failer
- **`Failer(Node decoratee)`**: always fail, regardless of the `decoratee`'s result.

#### Inverter
- **`Inverter(Node decoratee)`**: if `decoratee` succeeds, the inverter fails and if the `decoratee` fails, the inverter succeeds.

#### Observer
- **`Observer(Action onStart, Action<bool> onStop, Node decoratee)`**: runs the given `onStart` lambda once the `decoratee` starts and the `onStop(bool result)` lambda once the `decoratee` finishes. It's a bit like a special kind of `Service`, as it doesn't interfere in the execution of the `decoratee` directly.

#### Random
- **`Random(float probability, Node decoratee)`**: runs the `decoratee` with the given `probability` chance between 0 and 1.

#### Repeater
- **`Repeater(Node decoratee)`**: repeat the given `decoratee` infinitly, unless it fails
- **`Repeater(int loopCount, Node decoratee)`**: execute the given `decoratee` for `loopCount` times (0 means decoratee would never run). If `decoratee` stops the looping is aborted and the Repeater fails. If all executions of the `decoratee` are successful, the Repeater will succeed.

#### Service
- **`Service(Action service, Node decoratee)`**: run the given `service` function, start the `decoratee` and then run the `service` every tick.
- **`Service(float interval, Action service, Node decoratee)`**: run the given `service` function, start the `decoratee` and then run the `service` at the given `interval`.
- **`Service(float interval, float randomVariation, Action service, Node decoratee)`**: run the given `service` function, start the `decoratee` and then run the `service` at the given `interval` with `randomVariation`.

#### Succeeder
- **`Succeeder(Node decoratee)`**: always succeed, regardless of whether the `decoratee` succeeds or not

#### TimeMax
- **`TimeMax(float limit, bool waitForChildButFailOnLimitReached, Node decoratee)`**: run the given `decoratee`. If the `decoratee` doesn't finish within the `limit`, the execution fails. If `waitForChildButFailOnLimitReached` is true, it will wait for the decoratee to finish but still fail.
- **`TimeMax(float limit, float randomVariation, bool waitForChildButFailOnLimitReached, Node decoratee)`**: run the given `decoratee`. If the `decoratee` doesn't finish within the `limit` and `randomVariation`, the execution fails. If `waitForChildButFailOnLimitReached` is true, it will wait for the decoratee to finish but still fail.

#### TimeMin
- **`TimeMin(float limit, Node decoratee)`**: run the given `decoratee`. If the `decoratee` finishes sucessfully before the `limit` time is reached the decorator will wait until the limit is reached and then stop the execution with the result of the `Decoratee`. If the `decoratee` finishes failing before the `limit` time is reached, the decorator will immediately stop.
- **`TimeMin(float limit, bool waitOnFailure, Node decoratee)`**: run the given `decoratee`. If the `decoratee` finishes sucessful before the `limit` time is reached, the decorator will wait until the limit is reached and then stop the execution with the result of the `Decoratee`. If `waitOnFailure` is true, the `decoratee` will also wait when the decoratee fails.
- **`TimeMin(float limit, float randomVariation, bool waitOnFailure, Node decoratee)`**: run the given `decoratee`. If the `decoratee` finishes sucessful before the `limit` with `randomVariation` time is reached, the decorator will wait until the limit is reached and then stop the execution with the result of the `Decoratee`. If `waitOnFailure` is true, the `decoratee` will also wait when the decoratee fails.

#### WaitForCondition
- **`WaitForCondition(Func<bool> condition, Node decoratee)`**: Delay execution of the `decoratee` node until the `condition` gets true, checking every frame
- **`WaitForCondition(Func<bool> condition, float checkInterval, float randomVariance, Node decoratee)`**: Delay execution of the `decoratee` node until the `condition` gets true, checking with the given `checkInterval` and `randomVariance`

## Video Tutorials

* [Tutorial 01](https://www.youtube.com/watch?v=JZbesN2_-fE): Getting started
* [Tutorial 02](https://www.youtube.com/watch?v=0jKz9WqF24c): Simple Patrolling AI

## Contact

NPBehave was created and is maintained by Nils Kübler (E-Mail: das@nilspferd.net, Skype: disruption@web.de)

### Contributors

* [Nils Kübler](https://github.com/meniku)
* [Xerios/Sam](https://github.com/Xerios)
* [MohHeader](https://github.com/MohHEader)

### Games using NPBehave

* [Ace Of Traze Mobile](http://artwaretists.com/aot)

*If you have built a game or are building a game using NPBehave, I would be glad to have it on this list.  You can submit your game eiter via [contacting me](#contact) or creating a pull request on the [Github page](https://github.com/meniku)*
