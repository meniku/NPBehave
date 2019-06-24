using NUnit.Framework;
namespace NPBehave
{

    public class ParallelTest : Test
    {
        // private static Parallel.Wait[] ALL_WAITING_POLICIES = new Parallel.Wait[] { Parallel.Wait.NEVER, Parallel.Wait.BOTH, Parallel.Wait.ON_FAILURE, Parallel.Wait.ON_SUCCESS };
        private static Parallel.Policy[] ALL_SUCCESS_POLICIES = new Parallel.Policy[] { Parallel.Policy.ONE, Parallel.Policy.ALL };
        private static Parallel.Policy[] ALL_FAILURE_POLICIES = new Parallel.Policy[] { Parallel.Policy.ONE, Parallel.Policy.ALL };

        [Test]
        public void ShouldFail_WhenSingleChildFails()
        {
            foreach (Parallel.Policy sucessPolicy in ALL_SUCCESS_POLICIES)
                foreach (Parallel.Policy failurePolicy in ALL_FAILURE_POLICIES)
                {
                    MockNode failingChild = new MockNode();
                    Parallel sut = new Parallel(sucessPolicy, failurePolicy, failingChild);
                    TestRoot behaviorTree = CreateBehaviorTree(sut);

                    behaviorTree.Start();
                    Assert.AreEqual(Node.State.ACTIVE, sut.CurrentState);

                    failingChild.Finish(false);

                    Assert.AreEqual(Node.State.INACTIVE, sut.CurrentState);
                    Assert.IsTrue(behaviorTree.DidFinish);
                    Assert.IsFalse(behaviorTree.WasSuccess);
                }
        }

        [Test]
        public void ShouldSucceed_WhenSingleChildSucceeds()
        {
            foreach (Parallel.Policy sucessPolicy in ALL_SUCCESS_POLICIES)
                foreach (Parallel.Policy failurePolicy in ALL_FAILURE_POLICIES)
                {
                    MockNode succeedingChild = new MockNode();
                    Parallel sut = new Parallel(sucessPolicy, failurePolicy, succeedingChild);
                    TestRoot behaviorTree = CreateBehaviorTree(sut);

                    behaviorTree.Start();
                    Assert.AreEqual(Node.State.ACTIVE, sut.CurrentState);

                    succeedingChild.Finish(true);

                    Assert.AreEqual(Node.State.INACTIVE, sut.CurrentState);
                    Assert.IsTrue(behaviorTree.DidFinish);
                    Assert.IsTrue(behaviorTree.WasSuccess);
                }
        }

        [Test]
        public void ShouldFail_WhenStoppedExplicitly()
        {
            foreach (Parallel.Policy successPolicy in ALL_SUCCESS_POLICIES)
                foreach (Parallel.Policy failurePolicy in ALL_FAILURE_POLICIES)
                {
                    MockNode failingChild = new MockNode(false);
                    Parallel sut = new Parallel(successPolicy, failurePolicy, failingChild);
                    TestRoot behaviorTree = CreateBehaviorTree(sut);

                    behaviorTree.Start();
                    Assert.AreEqual(Node.State.ACTIVE, sut.CurrentState);

                    sut.Stop();

                    Assert.AreEqual(Node.State.INACTIVE, sut.CurrentState);
                    Assert.IsTrue(behaviorTree.DidFinish);
                    Assert.IsFalse(behaviorTree.WasSuccess);
                }
        }

        [Test]
        public void ShouldSucceed_WhenStoppedExplicitlyButChildStillFinishesSuccessfully()
        {
            foreach (Parallel.Policy successPolicy in ALL_SUCCESS_POLICIES)
                foreach (Parallel.Policy failurePolicy in ALL_FAILURE_POLICIES)
                {
                    MockNode succeedingChild = new MockNode(true);
                    Parallel sut = new Parallel(successPolicy, failurePolicy, succeedingChild);
                    TestRoot behaviorTree = CreateBehaviorTree(sut);

                    behaviorTree.Start();
                    Assert.AreEqual(Node.State.ACTIVE, sut.CurrentState);

                    sut.Stop();

                    Assert.AreEqual(Node.State.INACTIVE, sut.CurrentState);
                    Assert.IsTrue(behaviorTree.DidFinish);
                    Assert.True(behaviorTree.WasSuccess);
                }
        }

        [Test]
        public void ShouldStartAllChildrenInParallel()
        {
            Parallel.Policy successPolicy = Parallel.Policy.ONE;
            Parallel.Policy failurePolicy = Parallel.Policy.ONE;
            MockNode firstChild = new MockNode();
            MockNode secondChild = new MockNode();
            Parallel sut = new Parallel(successPolicy, failurePolicy, firstChild, secondChild);
            TestRoot behaviorTree = CreateBehaviorTree(sut);

            behaviorTree.Start();

            Assert.AreEqual(Node.State.ACTIVE, sut.CurrentState);
            Assert.AreEqual(Node.State.ACTIVE, firstChild.CurrentState);
        }


        [Test]
        public void ShouldSucceed_WhenOneChildSuccessful_AndSuccessPolicyOne()
        {
            Parallel.Policy successPolicy = Parallel.Policy.ONE;
            foreach (Parallel.Policy failurePolicy in ALL_FAILURE_POLICIES)
            {
                MockNode firstChild = new MockNode();
                MockNode secondChild = new MockNode(false);
                Parallel sut = new Parallel(successPolicy, failurePolicy, firstChild, secondChild);
                TestRoot behaviorTree = CreateBehaviorTree(sut);

                behaviorTree.Start();
                firstChild.Finish(true);

                Assert.AreEqual(Node.State.INACTIVE, firstChild.CurrentState);
                Assert.AreEqual(Node.State.INACTIVE, secondChild.CurrentState);
                Assert.IsTrue(behaviorTree.DidFinish);
                Assert.IsTrue(behaviorTree.WasSuccess);
            }
        }

        [Test]
        public void ShouldFail_WhenOneChildFailed_AndFailurePolicyOne()
        {
            Parallel.Policy failurePolicy = Parallel.Policy.ONE;
            foreach (Parallel.Policy successPolicy in ALL_SUCCESS_POLICIES)
            {
                MockNode firstChild = new MockNode();
                MockNode secondChild = new MockNode(true);
                Parallel sut = new Parallel(successPolicy, failurePolicy, firstChild, secondChild);
                TestRoot behaviorTree = CreateBehaviorTree(sut);

                behaviorTree.Start();
                firstChild.Finish(false);

                Assert.AreEqual(Node.State.INACTIVE, firstChild.CurrentState);
                Assert.AreEqual(Node.State.INACTIVE, secondChild.CurrentState);
                Assert.IsTrue(behaviorTree.DidFinish);
                Assert.IsFalse(behaviorTree.WasSuccess);
            }
        }


        [Test]
        public void ShouldSucceed_WhenAllChildrenSuccessful_AndSuccessPolicyAll()
        {
            Parallel.Policy successPolicy = Parallel.Policy.ALL;
            foreach (Parallel.Policy failurePolicy in ALL_FAILURE_POLICIES)
            {
                MockNode firstChild = new MockNode();
                MockNode secondChild = new MockNode();
                Parallel sut = new Parallel(successPolicy, failurePolicy, firstChild, secondChild);
                TestRoot behaviorTree = CreateBehaviorTree(sut);

                behaviorTree.Start();
                firstChild.Finish(true);
                secondChild.Finish(true);

                Assert.AreEqual(Node.State.INACTIVE, sut.CurrentState);
                Assert.IsTrue(behaviorTree.DidFinish);
                Assert.IsTrue(behaviorTree.WasSuccess);
            }
        }

        [Test]
        public void ShouldFail_WhenNotAllChildrenSuccessful_AndSuccessPolicyAll()
        {
            Parallel.Policy successPolicy = Parallel.Policy.ALL;
            foreach (Parallel.Policy failurePolicy in ALL_FAILURE_POLICIES)
            {
                MockNode firstChild = new MockNode();
                MockNode secondChild = new MockNode();
                Parallel sut = new Parallel(successPolicy, failurePolicy, firstChild, secondChild);
                TestRoot behaviorTree = CreateBehaviorTree(sut);

                behaviorTree.Start();
                firstChild.Finish(true);
                secondChild.Finish(false);

                Assert.AreEqual(Node.State.INACTIVE, sut.CurrentState);
                Assert.IsTrue(behaviorTree.DidFinish);
                Assert.IsFalse(behaviorTree.WasSuccess);
            }
        }


        [Test]
        public void ShouldFail_WhenAllChildrenFailed_AndFailPolicyAll()
        {
            Parallel.Policy failurePolicy = Parallel.Policy.ALL;
            foreach (Parallel.Policy successPolicy in ALL_SUCCESS_POLICIES)
            {
                MockNode firstChild = new MockNode();
                MockNode secondChild = new MockNode();
                Parallel sut = new Parallel(successPolicy, failurePolicy, firstChild, secondChild);
                TestRoot behaviorTree = CreateBehaviorTree(sut);

                behaviorTree.Start();
                firstChild.Finish(false);
                secondChild.Finish(false);

                Assert.AreEqual(Node.State.INACTIVE, sut.CurrentState);
                Assert.IsTrue(behaviorTree.DidFinish);
                Assert.IsFalse(behaviorTree.WasSuccess);
            }
        }

        [Test]
        public void ShouldFail_WhenNotAllChildrenFailed_AndFailPolicyAll_AndSucessPolicyAll()
        {
            Parallel.Policy failurePolicy = Parallel.Policy.ALL;
            Parallel.Policy successPolicy = Parallel.Policy.ALL;
            MockNode firstChild = new MockNode();
            MockNode secondChild = new MockNode();
            Parallel sut = new Parallel(successPolicy, failurePolicy, firstChild, secondChild);
            TestRoot behaviorTree = CreateBehaviorTree(sut);

            behaviorTree.Start();
            firstChild.Finish(true);
            secondChild.Finish(false);

            Assert.AreEqual(Node.State.INACTIVE, sut.CurrentState);
            Assert.IsTrue(behaviorTree.DidFinish);
            Assert.IsFalse(behaviorTree.WasSuccess);
        }

        [Test]
        public void ShouldSucceed_WhenNotAllChildrenFailed_AndFailPolicyAll_AndSucessPolicyOne()
        {
            Parallel.Policy failurePolicy = Parallel.Policy.ALL;
            Parallel.Policy successPolicy = Parallel.Policy.ONE;
            MockNode firstChild = new MockNode();
            MockNode secondChild = new MockNode();
            Parallel sut = new Parallel(successPolicy, failurePolicy, firstChild, secondChild);
            TestRoot behaviorTree = CreateBehaviorTree(sut);

            behaviorTree.Start();
            firstChild.Finish(false);
            secondChild.Finish(true);

            Assert.AreEqual(Node.State.INACTIVE, sut.CurrentState);
            Assert.IsTrue(behaviorTree.DidFinish);
            Assert.IsTrue(behaviorTree.WasSuccess);
        }

        [Test]
        public void StopLowerPriorityChildrenForChild_WithImmediateRestart_ShouldRestartChild()
        {
            Parallel.Policy failurePolicy = Parallel.Policy.ALL;
            Parallel.Policy successPolicy = Parallel.Policy.ALL;

            MockNode firstChild = new MockNode();
            MockNode secondChild = new MockNode();
            Parallel sut = new Parallel(successPolicy, failurePolicy, firstChild, secondChild);
            TestRoot behaviorTree = CreateBehaviorTree(sut);

            behaviorTree.Start();
            firstChild.Finish(false);
            sut.StopLowerPriorityChildrenForChild(firstChild, true);

            Assert.AreEqual(Node.State.ACTIVE, sut.CurrentState);
            Assert.AreEqual(Node.State.ACTIVE, firstChild.CurrentState);
            Assert.AreEqual(Node.State.ACTIVE, secondChild.CurrentState);
        }

        [Test]
        public void StopLowerPriorityChildrenForChild_WithoutImmediateRestart_ShouldThrowError()
        {
            Parallel.Policy failurePolicy = Parallel.Policy.ALL;
            Parallel.Policy successPolicy = Parallel.Policy.ALL;

            MockNode firstChild = new MockNode();
            MockNode secondChild = new MockNode();
            Parallel sut = new Parallel(successPolicy, failurePolicy, firstChild, secondChild);
            TestRoot behaviorTree = CreateBehaviorTree(sut);

            behaviorTree.Start();
            firstChild.Finish(false);
            bool bExceptionCought = false;
            try
            {
                
                sut.StopLowerPriorityChildrenForChild(firstChild, false);
            }
            catch(Exception)
            {
                bExceptionCought = true;
            }
            Assert.AreEqual( bExceptionCought, true );
        }
    }
}