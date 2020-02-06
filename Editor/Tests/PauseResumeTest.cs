using System.IO;
using NUnit.Framework;

namespace NPBehave
{
    public class PauseResumeTest : Test
    {
        [Test]
        // tests pausing and resuming a very simple behavior tree
        public void SimpleBehaviorTree()
        {
            // building a very simple behavior tree: two tasks in a selector
            this.Timer = new Clock();
            this.Blackboard = new Blackboard(Timer);

            MockTask firstTask = new MockTask(false);
            MockTask secondTask = new MockTask(false);

            Selector selector = new Selector(firstTask, secondTask);
            TestRoot behaviorTree = new TestRoot(Blackboard, Timer, selector);

            // starting the tree
            behaviorTree.Start();

            // first task should be active and second inactive
            Assert.AreEqual(Node.State.ACTIVE, firstTask.CurrentState);
            Assert.AreEqual(Node.State.INACTIVE, secondTask.CurrentState);

            // now pause the tree
            behaviorTree.Pause();

            // the previously active task should be stopped (inactive) now
            Assert.AreEqual(Node.State.INACTIVE, firstTask.CurrentState);
            Assert.AreEqual(Node.State.INACTIVE, secondTask.CurrentState);

            // and the containers above should be in pause mode
            Assert.AreEqual(Node.State.PAUSED, behaviorTree.CurrentState);
            Assert.AreEqual(Node.State.PAUSED, selector.CurrentState);

            // resume the tree again
            behaviorTree.Resume();

            // the first task should be active again and the second inactive
            Assert.AreEqual(Node.State.ACTIVE, firstTask.CurrentState);
            Assert.AreEqual(Node.State.INACTIVE, secondTask.CurrentState);

            // also the containers above should be also active again
            Assert.AreEqual(Node.State.ACTIVE, behaviorTree.CurrentState);
            Assert.AreEqual(Node.State.ACTIVE, selector.CurrentState);

            // stopping the first task and the first task should be inactive and the second active
            firstTask.Stop();
            Assert.AreEqual(Node.State.INACTIVE, firstTask.CurrentState);
            Assert.AreEqual(Node.State.ACTIVE, secondTask.CurrentState);
        }

        [Test]
        // tests pausing and resuming a more complex behavior tree
        public void SlightlyMoreComplexBehaviorTree()
        {
            // building a slighly more complex behavior tree
            this.Timer = new Clock();
            this.Blackboard = new Blackboard(Timer);

            MockTask firstTask = new MockTask(false);
            MockTask secondTask = new MockTask(false);
            MockTask thirdTask = new MockTask(false);

            Selector bottomSelector = new Selector(secondTask, thirdTask);
            Selector topSelector = new Selector(firstTask, bottomSelector);
            TestRoot behaviorTree = new TestRoot(Blackboard, Timer, topSelector);

            // starting the tree
            behaviorTree.Start();

            // first task should be active
            Assert.AreEqual(Node.State.ACTIVE, firstTask.CurrentState);
            Assert.AreEqual(Node.State.INACTIVE, secondTask.CurrentState);
            Assert.AreEqual(Node.State.INACTIVE, thirdTask.CurrentState);

            // now pause the tree
            behaviorTree.Pause();

            // the previously active task should be stopped (inactive) now
            Assert.AreEqual(Node.State.INACTIVE, firstTask.CurrentState);
            Assert.AreEqual(Node.State.INACTIVE, secondTask.CurrentState);
            Assert.AreEqual(Node.State.INACTIVE, secondTask.CurrentState);

            // only the containers that lead to the previously active task should be paused
            Assert.AreEqual(Node.State.PAUSED, behaviorTree.CurrentState);
            Assert.AreEqual(Node.State.PAUSED, topSelector.CurrentState);
            Assert.AreEqual(Node.State.INACTIVE, bottomSelector.CurrentState);

            // resume the tree again
            behaviorTree.Resume();

            // the first task should be active again
            Assert.AreEqual(Node.State.ACTIVE, firstTask.CurrentState);
            Assert.AreEqual(Node.State.INACTIVE, secondTask.CurrentState);
            Assert.AreEqual(Node.State.INACTIVE, thirdTask.CurrentState);

            // also the containers above should be also active again
            Assert.AreEqual(Node.State.ACTIVE, behaviorTree.CurrentState);
            Assert.AreEqual(Node.State.ACTIVE, topSelector.CurrentState);
            Assert.AreEqual(Node.State.INACTIVE, bottomSelector.CurrentState);

            // stopping the first task and the second task should be active
            firstTask.Stop();
            Assert.AreEqual(Node.State.INACTIVE, firstTask.CurrentState);
            Assert.AreEqual(Node.State.ACTIVE, secondTask.CurrentState);
            Assert.AreEqual(Node.State.INACTIVE, thirdTask.CurrentState);

            // pausing the tree and all task should be inactive and the container paused
            behaviorTree.Pause();
            Assert.AreEqual(Node.State.INACTIVE, firstTask.CurrentState);
            Assert.AreEqual(Node.State.INACTIVE, secondTask.CurrentState);
            Assert.AreEqual(Node.State.INACTIVE, thirdTask.CurrentState);
            Assert.AreEqual(Node.State.PAUSED, behaviorTree.CurrentState);
            Assert.AreEqual(Node.State.PAUSED, topSelector.CurrentState);
            Assert.AreEqual(Node.State.PAUSED, bottomSelector.CurrentState);

            // resuming again and the second task should be active again and also the containers
            behaviorTree.Resume();
            Assert.AreEqual(Node.State.INACTIVE, firstTask.CurrentState);
            Assert.AreEqual(Node.State.ACTIVE, secondTask.CurrentState);
            Assert.AreEqual(Node.State.INACTIVE, thirdTask.CurrentState);
            Assert.AreEqual(Node.State.ACTIVE, behaviorTree.CurrentState);
            Assert.AreEqual(Node.State.ACTIVE, topSelector.CurrentState);
            Assert.AreEqual(Node.State.ACTIVE, bottomSelector.CurrentState);
        }
        
        [Test]
        // the behavior tree should ignore the blackboard condition (with self stop rule) when paused
        public void IgnoreBlackBoardConditionWhenPausedSelf()
        {
            // building a behavior tree with two task and the first has a condition
            this.Timer = new Clock();
            this.Blackboard = new Blackboard(Timer);

            MockTask firstTask = new MockTask(false);
            MockTask secondTask = new MockTask(false);

            BlackboardCondition firstCondition =
                new BlackboardCondition("first", Operator.IS_EQUAL, true, Stops.SELF, firstTask);
            Selector selector = new Selector(firstCondition, secondTask);
            TestRoot behaviorTree = new TestRoot(Blackboard, Timer, selector);

            Blackboard.Set("first", true);

            // start the tree
            behaviorTree.Start();

            // first task should be active
            Assert.AreEqual(Node.State.ACTIVE, firstTask.CurrentState);
            Assert.AreEqual(Node.State.INACTIVE, secondTask.CurrentState);

            // now pause the tree
            behaviorTree.Pause();

            // blackboard condition should be paused now, the observers are unregistered
            // unregister the observers is actually unnecessary because only active nodes can be stopped
            // and restart is only called when there is at least one active child
            // and in pause state is everything inactive
            // but it leads to less unnecessary executed code
            Assert.AreEqual(Node.State.PAUSED, firstCondition.CurrentState);
            Assert.AreEqual(Node.State.INACTIVE, firstTask.CurrentState);
            Assert.AreEqual(Node.State.INACTIVE, secondTask.CurrentState);

            // when changing the condition nothing should happen because it is ignored in pause state
            Blackboard.Set("first", false);
            Timer.Update(0.1f);
            Assert.AreEqual(Node.State.PAUSED, firstCondition.CurrentState);
            Assert.AreEqual(Node.State.INACTIVE, firstTask.CurrentState);
            Assert.AreEqual(Node.State.INACTIVE, secondTask.CurrentState);

            // TODO: this doesn't work here!!! The change isn't notified!
            // when resuming the change should be notified and the second task should be active
            behaviorTree.Resume();
            Timer.Update(0.1f);
            Assert.AreEqual(Node.State.INACTIVE, firstCondition.CurrentState);
            Assert.AreEqual(Node.State.INACTIVE, firstTask.CurrentState);
            Assert.AreEqual(Node.State.ACTIVE, secondTask.CurrentState);
        }

        [Test]
        public void ServiceInactiveWhenPause()
        {
            // I am not really sure if the requirement is rigth that the service should be really inactive when paused.
            // But if the blackboard condition was set only in a service, the problem above with the
            // blackboard condition would be solved, because the blackboard wouldn't be updated in pause state

            // building a behavior tree with only one service and a task
            this.Timer = new Clock();
            this.Blackboard = new Blackboard(Timer);

            MockTask task = new MockTask(false);

            int serviceRunCount = 0;
            Service service = new Service(() => serviceRunCount++, task);
            TestRoot behaviorTree = new TestRoot(Blackboard, Timer, service);

            behaviorTree.Start();

            // the service run count should be one
            Assert.AreEqual(1, serviceRunCount);

            // after update it should be two
            Timer.Update(0.1f);
            Assert.AreEqual(2, serviceRunCount);

            // pause the tree
            behaviorTree.Pause();
            Timer.Update(0.1f);
            // service method shouldn't be called. It should still be two.
            Assert.AreEqual(2, serviceRunCount);

            // after resuming the service should be active again and the counter should be three
            // because when resuming the method is called
            behaviorTree.Resume();
            Assert.AreEqual(3, serviceRunCount);
        }

        [Test]
        public void IgnoreWaitForConditionWhenPause()
        {
            this.Timer = new Clock();
            this.Blackboard = new Blackboard(Timer);

            int condition = 0;
            MockTask task = new MockTask(false);
            WaitForCondition waitForCondition = new WaitForCondition(() => condition != 0, task);

            TestRoot behaviorTree = new TestRoot(Blackboard, Timer, waitForCondition);

            behaviorTree.Start();
            Assert.AreEqual(Node.State.ACTIVE, behaviorTree.CurrentState);
            Assert.AreEqual(Node.State.ACTIVE, waitForCondition.CurrentState);
            Assert.AreEqual(Node.State.INACTIVE, task.CurrentState);

            // when pausing and the condition is met, it should be ignored
            behaviorTree.Pause();
            condition = 1;
            Timer.Update(0.1f);
            Assert.AreEqual(Node.State.PAUSED, behaviorTree.CurrentState);
            Assert.AreEqual(Node.State.PAUSED, waitForCondition.CurrentState);
            Assert.AreEqual(Node.State.INACTIVE, task.CurrentState);

            // when resuming and the condition is met the task should be active
            behaviorTree.Resume();
            Timer.Update(0.1f);
            Assert.AreEqual(Node.State.ACTIVE, behaviorTree.CurrentState);
            Assert.AreEqual(Node.State.ACTIVE, waitForCondition.CurrentState);
            Assert.AreEqual(Node.State.ACTIVE, task.CurrentState);
        }

        // Testing different composite types behavior when pausing and the blackboard condition with Stops.IMMEDIATE_RESTART is changed.
        // The change of the condition should be ignored.
        [Test]
        public void IgnoreBlackboardConditionWhenPausedImmediateRestartSelector()
        {
            this.Timer = new Clock();
            this.Blackboard = new Blackboard(Timer);

            MockTask firstTask = new MockTask(false);
            MockTask secondTask = new MockTask(false);

            BlackboardCondition condition =
                new BlackboardCondition("first", Operator.IS_EQUAL, true, Stops.IMMEDIATE_RESTART, firstTask);
            Selector selector = new Selector(condition, secondTask);
            TestRoot behaviorTree = new TestRoot(Blackboard, Timer, selector);

            // set the condition for the first task to false so that the second task should be active
            Blackboard.Set("first", false);
            behaviorTree.Start();
            Assert.AreEqual(Node.State.INACTIVE, condition.CurrentState);
            Assert.AreEqual(Node.State.INACTIVE, firstTask.CurrentState);
            Assert.AreEqual(Node.State.ACTIVE, secondTask.CurrentState);
            
            // when pausing the tree and setting the blackboard condition to true nothing should happens
            behaviorTree.Pause();
            Blackboard.Set("first", true);
            Timer.Update(0.1f);
            Assert.AreEqual(Node.State.INACTIVE, condition.CurrentState);
            Assert.AreEqual(Node.State.INACTIVE, firstTask.CurrentState);
            Assert.AreEqual(Node.State.INACTIVE, secondTask.CurrentState);
            
            // now resume the tree and the first task should be the active one because of the true blackboard condition
            behaviorTree.Resume();
            Timer.Update(0.1f);
            Assert.AreEqual(Node.State.ACTIVE, condition.CurrentState);
            Assert.AreEqual(Node.State.ACTIVE, firstTask.CurrentState);
            Assert.AreEqual(Node.State.INACTIVE, secondTask.CurrentState);
        }
        
        [Test]
        public void IgnoreBlackboardConditionWhenPausedImmediateRestartSequence()
        {
            this.Timer = new Clock();
            this.Blackboard = new Blackboard(Timer);

            MockTask firstTask = new MockTask(true);
            MockTask secondTask = new MockTask(true);

            BlackboardCondition condition =
                new BlackboardCondition("first", Operator.IS_EQUAL, true, Stops.IMMEDIATE_RESTART, firstTask);
            Sequence sequence = new Sequence(condition, secondTask);
            TestRoot behaviorTree = new TestRoot(Blackboard, Timer, sequence);

            // set the condition for the first task to true so that the first task should be active
            Blackboard.Set("first", true);
            behaviorTree.Start();
            // update timer so the first task is active
            Timer.Update(0.1f);
            firstTask.Finish(true);
            Blackboard.Set("first", false);
            Assert.AreEqual(Node.State.INACTIVE, condition.CurrentState);
            Assert.AreEqual(Node.State.INACTIVE, firstTask.CurrentState);
            Assert.AreEqual(Node.State.ACTIVE, secondTask.CurrentState);
            
            // when pausing the tree and setting the blackboard condition to true nothing should happens
            behaviorTree.Pause();
            Blackboard.Set("first", true);
            Timer.Update(0.1f);
            Assert.AreEqual(Node.State.INACTIVE, condition.CurrentState);
            Assert.AreEqual(Node.State.INACTIVE, firstTask.CurrentState);
            Assert.AreEqual(Node.State.INACTIVE, secondTask.CurrentState);
            
            // now resume the tree and the first task should be the active one because of the true blackboard condition
            behaviorTree.Resume();
            Timer.Update(0.1f);
            Assert.AreEqual(Node.State.ACTIVE, condition.CurrentState);
            Assert.AreEqual(Node.State.ACTIVE, firstTask.CurrentState);
            Assert.AreEqual(Node.State.INACTIVE, secondTask.CurrentState);
        }
        
        [Test]
        public void IgnoreBlackboardConditionWhenPausedImmediateRestartParallel()
        {
            this.Timer = new Clock();
            this.Blackboard = new Blackboard(Timer);

            MockTask firstTask = new MockTask(false);
            MockTask secondTask = new MockTask(false);

            BlackboardCondition condition =
                new BlackboardCondition("first", Operator.IS_EQUAL, true, Stops.IMMEDIATE_RESTART, firstTask);
            Parallel parallel = new Parallel(Parallel.Policy.ALL, Parallel.Policy.ALL, condition, secondTask);
            TestRoot behaviorTree = new TestRoot(Blackboard, Timer, parallel);

            // set the condition for the first task to false so that the second task should be active
            Blackboard.Set("first", false);
            behaviorTree.Start();
            Assert.AreEqual(Node.State.INACTIVE, condition.CurrentState);
            Assert.AreEqual(Node.State.INACTIVE, firstTask.CurrentState);
            Assert.AreEqual(Node.State.ACTIVE, secondTask.CurrentState);
            
            // when pausing the tree and setting the blackboard condition to true nothing should happens
            behaviorTree.Pause();
            Blackboard.Set("first", true);
            Timer.Update(0.1f);
            Assert.AreEqual(Node.State.INACTIVE, condition.CurrentState);
            Assert.AreEqual(Node.State.INACTIVE, firstTask.CurrentState);
            Assert.AreEqual(Node.State.INACTIVE, secondTask.CurrentState);
            
            // now resume the tree and the first task should be the active one because of the true blackboard condition
            behaviorTree.Resume();
            Timer.Update(0.1f);
            Assert.AreEqual(Node.State.ACTIVE, condition.CurrentState);
            Assert.AreEqual(Node.State.ACTIVE, firstTask.CurrentState);
            Assert.AreEqual(Node.State.INACTIVE, secondTask.CurrentState);
        }
    }
}