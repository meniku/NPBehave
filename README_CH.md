## 前言
NPBahave是GitHub上开源的一个行为树，其代码简洁有力，与Unity耦合较低，[toc]适合拿来做双端行为树。`注意，由于时间关系，原文中的链接这里将不再提供引用。`
## 开源链接
[https://github.com/meniku/NPBehave](https://github.com/meniku/NPBehave)
## 正文
![NPBehave Logo](http://labs.nkuebler.de/npbehave/images/np-behave.png)
NPBehave致力于：

- 轻量，快速，简洁
- 事件驱动
- 易于拓展
- 一个用代码定义AI行为的框架，目前没有可视化编辑器支持（`本人将为其贡献一个`）

NPBehave基于功能强大且灵活的基于代码的方法，从behavior库定义行为树，并混合了虚幻引擎的一些很棒的行为树概念。与传统的行为树不同，事件驱动的行为树不需要每帧从根节点遍历。它们保持当前状态，只有在实际需要时才继续遍历。这使得它们的性能更高，使用起来也更简单。

在NPBehave中，您将发现大多数节点类型来自传统的行为树，但也有一些类似于虚幻引擎中的节点类型。不过，添加您自己的自定义节点类型也相当容易。

### 安装
只需将NPBehave文件夹放入Unity项目中。还有一个Examples子文件夹，其中有一些您可能想要参考的示例场景。

### 例子：“Hello World” 行为树
让我们开始一个例子
```csharp
using NPBehave;

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
当您运行此命令时，您将注意到“Hello World”将被一次又一次地打印出来。这是因为当遍历过树中的最后一个节点时，根节点将重新启动整个树。如果不需要这样，可以添加一个waituntilstop节点，如下所示:
```csharp
// ...
behaviorTree = new Root(
	new Sequence(
		new Action(() => Debug.Log("Hello World!")),
		new WaitUntilStopped()
	)
);
///... 
```
到目前为止，这个行为树中还没有任何事件驱动。在我们深入研究之前，您需要了解黑板（Blackboards）是什么。

### Blackboards（黑板）
在NPBehave中，就像在虚幻引擎中一样，我们有黑板。你可以把它们看作是你的AI的“记忆”。在NPBehave中，黑板是基于可以观察更改的字典。我们主要使用`Service`来存储和更新黑板中的值。我们使用`BlackboardCondition`或`BlackboardQuery`来观察黑板的变化，然后遍历bahaviour树。您也可以在其他任何地方访问或修改黑板的值(您也可以经常从Action节点访问它们)。

当您实例化一个`根（Root）`时，黑板将自动创建，但是您也可以使用它的构造函数提供另一个实例(这对于`共享黑板（Shared Blackboards）`特别有用)

### 例子：一个事件驱动的行为树
这有一个使用黑板的事件驱动的行为树例子
```csharp
/// ...
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
//...
```
这个示例将在每500毫秒交替打印“foo”和“bar”。我们使用一个`服务`装饰器节点在黑板上切换foo boolean值。我们使用BlackboardCondition装饰器节点根据这个boolean值来决定是否执行分支。BlackboardCondition还会根据这个值监视黑板的变化（依据黑板的当前值和我们提供的值做为判断基准），`Stops.IMMEDIATE_RESTART`作用是如果条件不再为真，则当前执行的分支将停止，如果条件再次为真，则立即重新启动。

请注意，您应该将服务放在真正的方法中，而不是使用lambdas，这将使您的树更具可读性。更复杂的行为也是如此。

### 终止原则
一些装饰器(如BlackboardCondition、Condition或BlackboardQuery)有一个stopsOnChange参数，允许定义stop规则。该参数允许装饰器停止其父`组合（Composite）`中正在运行的子树。他是您用来掌控NPBehave中的事件驱动的主要工具。

较`低优先级`的节点是在其父`组合`中的当前节点之后定义的节点。

最有用和最常用的stop规则是SELF、IMMEDIATE_RESTARTt或LOWER_PRIORITY_IMMEDIATE_RESTART。

不过，如果你对虚幻引擎的行为树形成了惯性思维，就要小心了。在NPBehave中，LOWER_PRIORITY和BOTH具有稍微不同的含义。IMMEDIATE_RESTART实际上匹配Unreal的Both，而LOWER_PRIORITY_IMMEDIATE_RESTART匹配Unreal的Lower Priority。

作者提供了如下终止原则

- Stops.NONE：装饰器只会在启动时检查一次它的状态，并且永远不会停止任何正在运行的节点。
- Stops.SELF：装饰器将在启动时检查一次它的条件状态，如果满足，它将继续观察黑板的变化。一旦不再满足该条件，它将终止自身，并让父组合继续处理它的下一个节点。
- Stops.LOWER_PRIORITY：装饰器将在启动时检查它的状态，如果不满足，它将观察黑板的变化。一旦条件满足，它将停止比此结点优先级较低的节点，允许父组合继续处理下一个节点
- Stops.BOTH：装饰器将同时停止:self和优先级较低的节点。
- Stops.LOWER_PRIORITY_IMMEDIATE_RESTART：一旦启动，装饰器将检查它的状态，如果不满足，它将观察黑板的变化。一旦条件满足，它将停止优先级较低的节点，并命令父组合立即重启此装饰器。
- Stops.IMMEDIATE_RESTART：一旦启动，装饰器将检查它的状态，如果不满足，它将观察黑板的变化。一旦条件满足，它将停止优先级较低的节点，并命令父组合立即重启装饰器。正如在这两种情况下，一旦不再满足条件，它也将停止自己。

### 黑板的替代品

在NPBehave中，您在一个MonoBehaviour中定义您的行为树，因为没有必要将所有内容都存储在黑板中。如果没有BlackboardDecorator或BlackboardQuery，则使用其他终止规则而不是Stops.NONE。你可能根本不需要它们出现在黑板上。您还可以使用普通的成员变量——它通常更干净、编写速度更快、性能更好。这意味着在这种情况下，您不会使用NPBehave的事件驱动特性，但这通常是不必要的。

如果你想在不使用黑板的情况下使用stopsOnChange终止规则，NPBehave中存在两种替代方法:

1. 使用常规条件装饰器。这个装饰器有一个可选的stopsOnChange `终止规则`参数。当提供除Stops.NONE之外的任何其他值，且给定查询函数的结果发生更改时，条件将频繁地检查条件并根据stop规则中断节点。请注意，此方法不是事件驱动的，它查询每一帧(或在提供的时间间隔内)，因此如果大量使用它们，可能会导致大量不必要的查询。然而，对于简单的情况，它通常是足够的，并且比Blackboard-Key、Service和BlackboardCondition的组合简单得多。
2. 构建自己的事件驱动的装饰器。实际上非常简单，只需从ObservingDecorator扩展并重写isConditionMet()、startobservice()和stopobservation()方法。

### 节点执行结果
在NPBehave中，节点可以成功也可以失败。与传统的行为树不同，节点执行时没有返回结果。相反，一旦节点执行完成（成功或失败），节点本身将告诉父节点。在创建自己的节点类型时，务必记住这一点。

### 节点类型
在NPBehave中，我们有四种不同的节点类型:

1. 根节点（Root）：根节点只有一个子节点可以启动或停止整个行为树。
2. 组合节点（Composite）：有多个子节点，用于控制它们的哪个子节点被执行。顺序和结果也是由这种节点定义的。
3. 装饰节点（Decorator）：始终`只有一个子节点`，用于修改子节点的结果或在执行子节点时执行其他操作(例如，更新黑板的Service)
4. 任务节点（Task）：这些是做实际工作的整个行为树中的树叶。您最有可能为它们创建自定义类。您可以将操作与lambdas或函数一起使用——对于更复杂的任务，创建任务的新子类通常是更好的选择。如果你这样做了，一定要阅读黄金规则。

### 终止树
如果你的怪物被杀死了，或者你销毁了游戏对象，你应该停止树。你可以在你的脚本中加入如下内容:
```csharp
    // ...
    public void OnDestroy()
    {
        StopBehaviorTree();
    }

    public void StopBehaviorTree()
    {
        if ( behaviorTree != null && behaviorTree.CurrentState == Node.State.ACTIVE )
        {
            behaviorTree.Stop();
        }
    }
    // ...
```
### 运行时Debugger
可以使用调试器组件在运行时在检查器中调试行为树。
![NPBehave Debugger](https://github.com/meniku/NPBehave/blob/master/README-Debugger.png)

### 共享黑板
您可以选择在AI的多个实例之间共享黑板。如果您想实现某种集群行为，这将非常有用。此外，您可以创建黑板层次结构，这允许您将共享黑板与非共享黑板组合起来。
您可以使用UnityContext.GetSharedBlackboard(name)在任何地方访问共享的blackboard实例。

### 拓展库
请参考现有的节点实现了解如何创建自定义节点类型，但是在创建之前至少要阅读以下黄金规则。

#### 黄金法则

1. **每次调用DoStop()都必须导致调用Stopped(result)**。这是非常重要的!您需要确保在DoStop()中调用了Stopped()，因为NPBehave需要能够在运行时立即取消正在运行的分支。这也意味着你所有的子节点也将调用Stopped(),这反过来又使得它很容易编写可靠的decorator甚至composite节点:在DoStop()里你只需要调用active状态下的孩子Stop()函数,他们将轮流执行ChildStopped()。`最终会回溯到上层节点的Stopped()函数！`请查看现有的实现以供参考。
2. **Stopped()是您做的最后一个调用**，在调用Stopped后不要修改任何状态或调用任何东西。这是因为Stopped将立即继续遍历其他节点上的树，如果不考虑这一点，将完全破坏行为树的状态。
3. **每一个注册的时钟或黑板观察者最终都需要删除**。大多数时候你调用Stopped()之前立刻注销你的回调函数,不过可能会有例外,比如BlackboardCondition使观察者处于警惕状态直到父组合结点终止,它需要能够对黑板上值改变及时作出反应，即使节点本身并不活跃。

### 实现任务
对于任务，可以从任务类扩展并覆盖DoStart()和DoStop()方法。在DoStart()中，您启动您的逻辑，一旦您完成了，您将使用适当的结果调用Stopped(bool result)。您的节点可能被另一个节点取消，因此请确保实现DoStop()，进行适当的清理并在它之后立即调用Stopped(bool result)。
对于一个相对简单的示例，请查看Wait Task.cs。
正如黄金规则部分已经提到的，在NPBehave中，您必须在节点停止之后始终调用Stopped(bool result)。因此，目前不支持在多个帧上挂起取消操作，这将导致不可预测的行为。

### 实现观察装饰器
编写装饰器要比编写任务复杂得多。然而，为了方便起见，存在一个特殊的基类。ObservingDecorator。这个类可用于简单地实现“条件”装饰器，这些装饰器可选地使用stopsOnChange 终止规则。
您所要做的就是从它ObservingDecorator扩展并覆盖bool IsConditionMet()方法。如果希望支持stop - rules，还必须实现startobservice()和stopobserve()。对于一个简单的示例，请查看Condition Decorator.cs

### 实现常规装饰器
对于常规装饰器，可以从Decorator.cs扩展并覆盖DoStart()、DoStop()和DoChildStopped(Node child, bool result)方法。
您可以通过访问Decoratee属性启动或停止已装饰节点，并在其上调用start()或stop()。
如果您的decorator接收到DoStop()调用，它将负责相应地停止Decoratee，并且在这种情况下不会立即调用Stopped(bool result)。相反，它将在DoChildStopped(Node child, bool result)方法中执行该操作。请注意，DoChildStopped(Node child, bool result)并不一定意味着您的decorator停止了decoratee, decoratee本身也可能停止，在这种情况下，您不需要立即停止decoratee(如果您想实现诸如冷却等功能，这可能很有用)。要查明装饰器是否被停止，可以查询它的isstoprequired属性。
对于非常基本的实现，请查看Failer Node.cs;对于稍微复杂一点的实现，请查看Repeater Node.cs。
此外，您还可以实现DoParentCompositeStopped()方法，即使您的装饰器处于非活动状态，也可以调用该方法。如果您想为在装饰器stopped后仍保持活动的侦听器执行额外的清理工作，这是非常有用的。以ObservingDecorator为例。

### 实现组合
组合节点需要对库有更深入的理解，通常不需要实现新的节点。如果您真的需要一个新的组合，请在GitHub项目上创建一个票据，或者与我联系，我将尽力帮助您正确地完成它。

### 结点状态
很有可能你不需要访问它们，但了解它们仍然是件好事:

- ACTIVE:节点已启动，但尚未停止。
- STOP_REQUESTED:节点当前正在停止，但尚未调用Stopped()来通知父节点。
- INACTIVE:节点已停止。

可以使用CurrentState属性检索当前状态

### 时钟
您可以使用节点中的时钟注册计时器，或者在每一帧上得到通知。使用RootNode.Clock访问时钟。查看`Wait Task.cs`以获得关于如何在时钟上注册计时器的示例。
默认情况下，行为树将使用UnityContext指定的全局时钟。这个时钟每一帧都更新一次。在某些情况下，你可能想要拥有更多的控制权。例如，您可能想要限制或暂停对一组AI的更新。由于这个原因，您可以向根节点和Blackboard提供自己的受控时钟实例，这允许您精确地控制何时更新行为树。查看 Clock Throttling .cs。

## 结点类型汇总
### Root

- Root(Node mainNode):无休止地运行mainNode，不论任何情况
- Root(Blackboard Blackboard, Node mainNode):使用给定的黑板，而不是实例化一个;无休止地运行给定的mainNode，不论任何情况
- Root(Blackboard blackboard, Clock clock, Node mainNode):使用给定的黑板而不是实例化一个;使用给定的时钟，而不是使用UnityContext中的全局时钟;无休止地运行给定的mainNode，不论任何情况

### 组合结点
#### Selector
- Selector(params Node[] children):按顺序运行子元素，直到其中一个子元素成功(如果其中一个子元素成功，则成功)。
#### Sequence
- Sequence(params Node[] children):按顺序运行子节点，直到其中一个失败(如果所有子节点都没有失败，则成功)。
#### Parallel
- Parallel(Policy successPolicy, Policy failurePolicy, params Node[] children): 并行运行子节点。
- 当failurePolocity为Polociy.ONE。当其中一个孩子失败时，并行就会停止，返回失败。
- 当successPolicy为Policy.ONE。当其中一个孩子失败时，并行将停止，返回成功。
- 如果并行没有因为Policy.ONE而停止。它会一直执行，直到所有的子节点都完成，然后如果所有的子节点都成功或者失败，它就会返回成功。
#### RandomSelector
- RandomSelector(params Node[] children):按随机顺序运行子进程，直到其中一个子进程成功(如果其中一个子进程成功，则成功)。注意，对于打断规则，最初的顺序定义了优先级。
#### RandomSequence
- RandomSequence(params Node[] children):以随机顺序运行子节点，直到其中一个失败(如果没有子节点失败，则成功)。注意，对于打断规则，最初的顺序定义了优先级。
### 任务结点
#### Action

- Action(System.Action action):(总是立即成功完成)
- Action(System.Func<bool> singleFrameFunc): 可以成功或失败的操作(返回false to fail)
- Action(Func<bool, Result> multiframeFunc):可以在多个帧上执行的操作(
Result.BLOCKED——你的行动还没有准备好
Result.PROGRESS——当你忙着这个行为的时候，
Result.SUCCESS或Result.FAILED——成功或失败)。
- Action(Func<Request, Result> multiframeFunc2): 与上面类似，但是Request会给你一个状态信息:
Request.START表示它是您的操作或返回结果的第一个标记或者是Result.BLOCKED最后一个标记。
Request.UPDATE表示您最后一次返回Request.PROGRESS;
Request.CANCEL意味着您需要取消操作并返回结果。成功或者Result.FAILED。

#### Wait

- Wait(float seconds): 等待给定的秒，随机误差为0.05 *秒
- Wait(float seconds, float randomVariance): 用给定的随机变量等待给定的秒数
- Wait(string blackboardKey, float randomVariance = 0f): 
- Wait(System.Func<float> function, float randomVariance = 0f): 等待在给定的blackboardKey中设置为float的秒数
	
#### WaitUntilStopped

- WaitUntilStopped(bool sucessWhenStopped = false):等待被其他节点停止。它通常用于Selector的末尾，等待任何before头的同级BlackboardCondition、BlackboardQuery或Condition变为活动状态。

### 装饰器结点
#### BlackboardCondition
- BlackboardCondition(string key, Operator operator, object value, Stops stopsOnChange, Node decoratee): 只有当黑板的键匹配op / value条件时，才执行decoratee节点。如果stopsOnChange不是NONE，则节点将根据stopsOnChange stop规则观察黑板上的变化并停止运行节点的执行。
- BlackboardCondition(string key, Operator operator, Stops stopsOnChange, Node decoratee): 只有当黑板的键与op条件匹配时才执行decoratee节点(例如，对于一个只检查IS_SET的操作数操作符)。如果stopsOnChange不是NONE，则节点将根据stopsOnChange stop规则观察黑板上的变化并停止运行节点的执行。
#### BlackboardQuery
- BlackboardQuery(string[] keys, Stops stopsOnChange, System.Func<bool> query, Node decoratee):BlackboardCondition只允许检查一个键，而这个将观察多个黑板键，并在其中一个值发生变化时立即计算给定的查询函数，从而允许您在黑板上执行任意查询。它将根据stopsOnChange stop规则停止运行节点。
#### Condition
- Condition(Func<bool> condition, Node decoratee): 如果给定条件返回true，则执行decoratee节点
- Condition(Func<bool> condition, Stops stopsOnChange, Node decoratee): 如果给定条件返回true，则执行decoratee节点。根据stopsOnChange stop规则重新评估每个帧的条件并停止运行节点。
- Condition(Func<bool> condition, Stops stopsOnChange, float checkInterval, float randomVariance, Node decoratee): 如果给定条件返回true，则执行decoratee节点。在给定的校验间隔和随机方差处重新评估条件，并根据stopsOnChange stop规则停止运行节点。
#### Cooldown
- Cooldown(float cooldownTime, Node decoratee):立即运行decoratee，但前提是最后一次执行至少没有超过cooldownTime
- Cooldown(float cooldownTime, float randomVariation, Node decoratee): 立即运行decoratee，但前提是最后一次执行至少没有超过使用randomVariation进行的cooldownTime
- Cooldown(float cooldownTime, bool startAfterDecoratee, bool resetOnFailiure, Node decoratee):  立即运行decoratee，但前提是最后一次执行至少没有超过使用randomVariation进行的cooldownTime，当resetOnFailure为真时，如果修饰节点失败，则重置冷却时间
- Cooldown(float cooldownTime, float randomVariation, bool startAfterDecoratee, bool resetOnFailiure, Node decoratee)  立即运行decoratee，但前提是最后一次执行至少没有超过使用randomVariation进行的cooldownTime，当startAfterDecoratee为true时，将在decoratee完成后而不是启动时启动冷却计时器。当resetOnFailure为真时，如果修饰节点失败，则重置冷却时间。
#### Failer
- Failer(Node decoratee): 总是失败，不管装饰者的结果如何。
#### Inverter
- Inverter(Node decoratee): 如果decoratee成功，则逆变器失败;如果decoratee失败，则逆变器成功。
#### Observer
- Observer(Action onStart, Action<bool> onStop, Node decoratee): 一旦decoratee启动，运行给定的onStart lambda;一旦decoratee结束，运行onStop(bool result) lambda。它有点像一种特殊的服务，因为它不会直接干扰decoratee的执行。
#### Random
- Random(float probability, Node decoratee): 以给定的概率，0到1运行decoratee。
#### Repeater
- Repeater(Node decoratee): 无限重复给定的装饰，除非失败
- Repeater(int loopCount, Node decoratee): 执行给定的decoratee循环次数(0表示decoratee永远不会运行)。如果decoratee停止，循环将中止，并且中继器失败。如果decoratee的所有执行都成功，那么中继器将会成功。
#### Service
- Service(Action service, Node decoratee): 运行给定的服务函数，启动decoratee，然后每次运行服务。
- Service(float interval, Action service, Node decoratee): 运行给定的服务函数，启动decoratee，然后按给定的间隔运行服务。
- Service(float interval, float randomVariation, Action service, Node decoratee): 运行给定的服务函数，启动decoratee，然后在给定的时间间隔内以随机变量的方式运行服务。
#### Succeeder
- Succeeder(Node decoratee): 永远要成功，不管装饰器是否成功
#### TimeMax
- TimeMax(float limit, bool waitForChildButFailOnLimitReached, Node decoratee): 运行给定的decoratee。如果decoratee没有在限制时间内完成，则执行将失败。如果waitforchildbutfailonlimitarrived为true，它将等待decoratee完成，但仍然失败。
- TimeMax(float limit, float randomVariation, bool waitForChildButFailOnLimitReached, Node decoratee):运行给定的decoratee。如果decoratee没有在限制和随机变化范围内完成，则执行将失败。如果waitforchildbutfailonlimitarrived为true，它将等待decoratee完成，但仍然失败。
#### TimeMin
- TimeMin(float limit, Node decoratee): 运行给定的decoratee。如果decoratee在达到限制时间之前成功完成，decorator将等待直到达到限制，然后根据decoratee的结果停止执行。如果被装饰者在达到限制时间之前失败，装饰者将立即停止。
- TimeMin(float limit, bool waitOnFailure, Node decoratee): 运行给定的decoratee。如果decoratee在达到限制时间之前成功完成，decorator将等待直到达到限制，然后根据decoratee的结果停止执行。如果waitOnFailure为真，那么当decoratee失败时，decoratee也将等待。
- TimeMin(float limit, float randomVariation, bool waitOnFailure, Node decoratee): 运行给定的decoratee。如果decoratee在达到随机变化时间限制之前成功完成，decorator将等待直到达到限制，然后根据decoratee的结果停止执行。如果waitOnFailure为真，那么当decoratee失败时，decoratee也将等待。
#### WaitForCondition
- WaitForCondition(Func<bool> condition, Node decoratee): 延迟decoratee节点的执行，直到条件为真，检查每一帧
- WaitForCondition(Func<bool> condition, float checkInterval, float randomVariance, Node decoratee): 延迟decoratee节点的执行，直到条件为真，使用给定的checkInterval和randomVariance进行检查

## 后记
本文档仅供参考，一切以代码为准！

