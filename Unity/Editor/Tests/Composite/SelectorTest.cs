using NUnit.Framework;
namespace NPBehave
{

    public class SelectorTest : Test
    {
        [Test]
        public void ShouldFail_WhenSingleChildFails()
        {
            MockNode failingChild = new MockNode();
            Selector sut = new Selector(failingChild);
            TestRoot behaviorTree = CreateBehaviorTree(sut);

            behaviorTree.Start();

            Assert.AreEqual(Node.State.ACTIVE, sut.CurrentState);

            failingChild.Finish(false);

            Assert.AreEqual(Node.State.INACTIVE, sut.CurrentState);
            Assert.IsTrue(behaviorTree.DidFinish);
            Assert.IsFalse(behaviorTree.WasSuccess);
        }

        [Test]
        public void ShouldSucceed_WhenSingleChildSucceeds()
        {
            MockNode succeedingChild = new MockNode();
            Selector sut = new Selector(succeedingChild);
            TestRoot behaviorTree = CreateBehaviorTree(sut);

            behaviorTree.Start();

            Assert.AreEqual(Node.State.ACTIVE, sut.CurrentState);

            succeedingChild.Finish(true);

            Assert.AreEqual(Node.State.INACTIVE, sut.CurrentState);
            Assert.IsTrue(behaviorTree.DidFinish);
            Assert.IsTrue(behaviorTree.WasSuccess);
        }

        [Test]
        public void ShouldFail_WhenStoppedExplicitly()
        {
            MockNode failingChild = new MockNode(false);
            Selector sut = new Selector(failingChild);
            TestRoot behaviorTree = CreateBehaviorTree(sut);

            behaviorTree.Start();

            Assert.AreEqual(Node.State.ACTIVE, sut.CurrentState);

            sut.Stop();

            Assert.AreEqual(Node.State.INACTIVE, sut.CurrentState);
            Assert.IsTrue(behaviorTree.DidFinish);
            Assert.IsFalse(behaviorTree.WasSuccess);
        }

        [Test]
        public void ShouldSucceed_WhenStoppedExplicitlyButChildStillFinishesSuccessfully()
        {
            MockNode succeedingChild = new MockNode(true);
            Selector sut = new Selector(succeedingChild);
            TestRoot behaviorTree = CreateBehaviorTree(sut);

            behaviorTree.Start();

            Assert.AreEqual(Node.State.ACTIVE, sut.CurrentState);

            sut.Stop();

            Assert.AreEqual(Node.State.INACTIVE, sut.CurrentState);
            Assert.IsTrue(behaviorTree.DidFinish);
            Assert.True(behaviorTree.WasSuccess);
        }

        [Test]
        public void ShouldSucceed_WhenFirstChildSuccessful()
        {
            MockNode firstChild = new MockNode();
            MockNode secondChild = new MockNode();
            Selector sut = new Selector(firstChild, secondChild);
            TestRoot behaviorTree = CreateBehaviorTree(sut);

            behaviorTree.Start();

            Assert.AreEqual(Node.State.ACTIVE, sut.CurrentState);
            Assert.AreEqual(Node.State.ACTIVE, firstChild.CurrentState);
            Assert.AreEqual(Node.State.INACTIVE, secondChild.CurrentState);

            firstChild.Finish(true);

            Assert.AreEqual(Node.State.INACTIVE, sut.CurrentState);
            Assert.AreEqual(Node.State.INACTIVE, firstChild.CurrentState);
            Assert.AreEqual(Node.State.INACTIVE, secondChild.CurrentState);
            Assert.IsTrue(behaviorTree.DidFinish);
            Assert.IsTrue(behaviorTree.WasSuccess);
        }

        [Test]
        public void ShouldProcceedToSecondChild_WhenFirstChildFailed()
        {
            MockNode firstChild = new MockNode();
            MockNode secondChild = new MockNode();
            Selector sut = new Selector(firstChild, secondChild);
            TestRoot behaviorTree = CreateBehaviorTree(sut);

            behaviorTree.Start();

            Assert.AreEqual(Node.State.ACTIVE, sut.CurrentState);
            Assert.AreEqual(Node.State.ACTIVE, firstChild.CurrentState);
            Assert.AreEqual(Node.State.INACTIVE, secondChild.CurrentState);

            firstChild.Finish(false);

            Assert.AreEqual(Node.State.ACTIVE, sut.CurrentState);
            Assert.AreEqual(Node.State.INACTIVE, firstChild.CurrentState);
            Assert.AreEqual(Node.State.ACTIVE, secondChild.CurrentState);

            secondChild.Finish(false);

            Assert.AreEqual(Node.State.INACTIVE, sut.CurrentState);
            Assert.AreEqual(Node.State.INACTIVE, firstChild.CurrentState);
            Assert.AreEqual(Node.State.INACTIVE, secondChild.CurrentState);
            Assert.IsTrue(behaviorTree.DidFinish);
            Assert.IsFalse(behaviorTree.WasSuccess);
        }

        [Test]
        public void StopLowerPriorityChildrenForChild_WithoutImmediateRestart_ShouldCancelSecondChild()
        {
            MockNode firstChild = new MockNode();
            MockNode secondChild = new MockNode();
            Selector sut = new Selector(firstChild, secondChild);
            TestRoot behaviorTree = CreateBehaviorTree(sut);

            behaviorTree.Start();
            firstChild.Finish(false);

            Assert.AreEqual(Node.State.ACTIVE, sut.CurrentState);
            Assert.AreEqual(Node.State.INACTIVE, firstChild.CurrentState);
            Assert.AreEqual(Node.State.ACTIVE, secondChild.CurrentState);

            sut.StopLowerPriorityChildrenForChild(firstChild, false);

            Assert.AreEqual(Node.State.INACTIVE, sut.CurrentState);
            Assert.AreEqual(Node.State.INACTIVE, firstChild.CurrentState);
            Assert.AreEqual(Node.State.INACTIVE, secondChild.CurrentState);
            Assert.IsTrue(behaviorTree.DidFinish);
            Assert.IsFalse(behaviorTree.WasSuccess);
        }

        [Test]
        public void StopLowerPriorityChildrenForChild_WithImmediateRestart_ShouldRestartFirstChild_WhenSecondChildFails()
        {
            MockNode firstChild = new MockNode();
            MockNode secondChild = new MockNode(false);
            Selector sut = new Selector(firstChild, secondChild);
            TestRoot behaviorTree = CreateBehaviorTree(sut);

            behaviorTree.Start();
            firstChild.Finish(false);

            Assert.AreEqual(Node.State.ACTIVE, sut.CurrentState);
            Assert.AreEqual(Node.State.INACTIVE, firstChild.CurrentState);
            Assert.AreEqual(Node.State.ACTIVE, secondChild.CurrentState);

            sut.StopLowerPriorityChildrenForChild(firstChild, true);

            Assert.AreEqual(Node.State.ACTIVE, sut.CurrentState);
            Assert.AreEqual(Node.State.ACTIVE, firstChild.CurrentState);
            Assert.AreEqual(Node.State.INACTIVE, secondChild.CurrentState);
            Assert.IsFalse(behaviorTree.DidFinish);
        }

        [Test]
        public void StopLowerPriorityChildrenForChild_WithImmediateRestart_ShouldNotRestartFirstChild_WhenSecondChildSucceeds()
        {
            MockNode firstChild = new MockNode();
            MockNode secondChild = new MockNode(true);
            Selector sut = new Selector(firstChild, secondChild);
            TestRoot behaviorTree = CreateBehaviorTree(sut);

            behaviorTree.Start();
            firstChild.Finish(false);

            Assert.AreEqual(Node.State.ACTIVE, sut.CurrentState);
            Assert.AreEqual(Node.State.INACTIVE, firstChild.CurrentState);
            Assert.AreEqual(Node.State.ACTIVE, secondChild.CurrentState);

            sut.StopLowerPriorityChildrenForChild(firstChild, true);

            Assert.AreEqual(Node.State.INACTIVE, sut.CurrentState);
            Assert.AreEqual(Node.State.INACTIVE, firstChild.CurrentState);
            Assert.AreEqual(Node.State.INACTIVE, secondChild.CurrentState);
            Assert.IsTrue(behaviorTree.DidFinish);
            Assert.IsTrue(behaviorTree.WasSuccess);
        }
    }
}