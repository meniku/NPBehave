using NUnit.Framework;
namespace NPBehave
{
#pragma warning disable 618 // deprecation

    public class BlackboardTest
    {
        private Clock clock;
        private Blackboard sut;

        [SetUp]
        public void SetUp()
        {
            this.clock = new Clock();
            this.sut = new Blackboard(clock);
        }

        [Test]
        public void ShouldNotNotifyObservers_WhenNoClockUpdate()
        {
            bool notified = false;
            this.sut.AddObserver("test", ( Blackboard.Type type, object value ) =>
            {
                notified = true;
            });

            this.sut.Set("test", 1f);
            Assert.IsFalse(notified);
        }

        [Test]
        public void ShouldNotifyObservers_WhenClockUpdate()
        {
            bool notified = false;
            this.sut.AddObserver("test", ( Blackboard.Type type, object value ) =>
            {
                notified = true;
            });

            this.sut.Set("test", 1f);
            this.clock.Update(1f);
            Assert.IsTrue(notified);
        }

        [Test]
        public void ShouldNotNotifyObserver_WhenRemovedDuringOtherObserver()
        {
            bool notified = false;
            System.Action<Blackboard.Type, object> obs1 = null;
            System.Action<Blackboard.Type, object> obs2 = null;

            obs1 = ( Blackboard.Type type, object value ) =>
            {
                Assert.IsFalse(notified);
                notified = true;
                this.sut.RemoveObserver("test", obs2);
            };
            obs2 = ( Blackboard.Type type, object value ) =>
            {
                Assert.IsFalse(notified);
                notified = true;
                this.sut.RemoveObserver("test", obs1);
            };
            this.sut.AddObserver("test", obs1);
            this.sut.AddObserver("test", obs2);

            this.sut.Set("test", 1f);
            this.clock.Update(1f);
            Assert.IsTrue(notified);
        }

        [Test]
        public void ShouldAllowToSetToNull_WhenAlreadySertToNull()
        {
            this.sut.Set("test", 1f);
            Assert.AreEqual(this.sut.Get("test"), 1f);
            this.sut.Set("test", null);
            this.sut.Set("test", null);
            Assert.AreEqual(this.sut.Get("test"), null);
            this.sut.Set("test", "something");
            Assert.AreEqual(this.sut.Get("test"), "something");
        }

        [Test]
        public void NewDefaultValuesShouldBeCompatible()
        {
            Assert.AreEqual(this.sut.Get<bool>("not-existing"), this.sut.GetBool("not-existing"));
            Assert.AreEqual(this.sut.Get<int>("not-existing"), this.sut.GetInt("not-existing"));
            //            Assert.AreEqual(this.sut.Get<float>("not-existing"), this.sut.GetFloat("not-existing"));
            Assert.AreEqual(this.sut.Get<UnityEngine.Vector3>("not-existing"), this.sut.GetVector3("not-existing"));
        }


        // check for https://github.com/meniku/NPBehave/issues/17
        [Test]
        public void ShouldListenToEvents_WhenUsingChildBlackboard()
        {
            Blackboard rootBlackboard = new Blackboard(clock);
            Blackboard blackboard = new Blackboard(rootBlackboard, clock);

            // our mock nodes we want to query for status
            MockNode firstChild = new MockNode(false); // false -> fail when aborted
            MockNode secondChild = new MockNode(false);

            // conditions for each subtree that listen the BB for events
            BlackboardCondition firstCondition = new BlackboardCondition("branch1", Operator.IS_EQUAL, true, Stops.IMMEDIATE_RESTART, firstChild);
            BlackboardCondition secondCondition = new BlackboardCondition("branch2", Operator.IS_EQUAL, true, Stops.IMMEDIATE_RESTART, secondChild);

            // set up the tree
            Selector selector = new Selector(firstCondition, secondCondition);
            TestRoot behaviorTree = new TestRoot(blackboard, clock, selector);

            // intially we want to activate branch2
            rootBlackboard.Set("branch2", true);

            // start the tree
            behaviorTree.Start();

            // tick the timer to ensure the blackboard notifies the nodes
            clock.Update(0.1f);

            // verify the second child is running
            Assert.AreEqual(Node.State.INACTIVE, firstChild.CurrentState);
            Assert.AreEqual(Node.State.ACTIVE, secondChild.CurrentState);

            // change keys so the first conditions get true, too
            rootBlackboard.Set("branch1", true);

            // tick the timer to ensure the blackboard notifies the nodes
            clock.Update(0.1f);

            // now we should be in branch1
            Assert.AreEqual(Node.State.ACTIVE, firstChild.CurrentState);
            Assert.AreEqual(Node.State.INACTIVE, secondChild.CurrentState);
        }
    }
}