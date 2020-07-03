using NUnit.Framework;
namespace NPBehave
{

    public class SequenceTest : Test
    {
        [Test]
        public void ShouldFail_WhenSingleChildFails()
        {
            MockNode failingChild = new MockNode();
            Sequence sut = new Sequence(failingChild);
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
            Sequence sut = new Sequence(succeedingChild);
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
            Sequence sut = new Sequence(failingChild);
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
            Sequence sut = new Sequence(succeedingChild);
            TestRoot behaviorTree = CreateBehaviorTree(sut);

            behaviorTree.Start();

            Assert.AreEqual(Node.State.ACTIVE, sut.CurrentState);

            sut.Stop();

            Assert.AreEqual(Node.State.INACTIVE, sut.CurrentState);
            Assert.IsTrue(behaviorTree.DidFinish);
            Assert.True(behaviorTree.WasSuccess);
        }

        [Test]
        public void ShouldFail_WhenFirstChildFails()
        {
            MockNode firstChild = new MockNode();
            MockNode secondChild = new MockNode();
            Sequence sut = new Sequence(firstChild, secondChild);
            TestRoot behaviorTree = CreateBehaviorTree(sut);

            behaviorTree.Start();

            Assert.AreEqual(Node.State.ACTIVE, sut.CurrentState);
            Assert.AreEqual(Node.State.ACTIVE, firstChild.CurrentState);
            Assert.AreEqual(Node.State.INACTIVE, secondChild.CurrentState);

            firstChild.Finish(false);

            Assert.AreEqual(Node.State.INACTIVE, sut.CurrentState);
            Assert.AreEqual(Node.State.INACTIVE, firstChild.CurrentState);
            Assert.AreEqual(Node.State.INACTIVE, secondChild.CurrentState);
            Assert.IsTrue(behaviorTree.DidFinish);
            Assert.IsFalse(behaviorTree.WasSuccess);
        }

        [Test]
        public void ShouldProcceedToSecondChild_WhenFirstChildSucceeded()
        {
            MockNode firstChild = new MockNode();
            MockNode secondChild = new MockNode();
            Sequence sut = new Sequence(firstChild, secondChild);
            TestRoot behaviorTree = CreateBehaviorTree(sut);

            behaviorTree.Start();

            Assert.AreEqual(Node.State.ACTIVE, sut.CurrentState);
            Assert.AreEqual(Node.State.ACTIVE, firstChild.CurrentState);
            Assert.AreEqual(Node.State.INACTIVE, secondChild.CurrentState);

            firstChild.Finish(true);

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
            Sequence sut = new Sequence(firstChild, secondChild);
            TestRoot behaviorTree = CreateBehaviorTree(sut);

            behaviorTree.Start();

            firstChild.Finish(true);

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
        public void StopLowerPriorityChildrenForChild_WithImmediateRestart_ShouldRestartFirstChild_WhenSecondChildSucceeds()
        {
            MockNode firstChild = new MockNode();
            MockNode secondChild = new MockNode(true);
            Sequence sut = new Sequence(firstChild, secondChild);
            TestRoot behaviorTree = CreateBehaviorTree(sut);

            behaviorTree.Start();
            firstChild.Finish(true);

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
        public void StopLowerPriorityChildrenForChild_WithImmediateRestart_ShouldNotRestartFirstChild_WhenSecondChildFails()
        {
            MockNode firstChild = new MockNode();
            MockNode secondChild = new MockNode(false);
            Sequence sut = new Sequence(firstChild, secondChild);
            TestRoot behaviorTree = CreateBehaviorTree(sut);

            behaviorTree.Start();

            firstChild.Finish(true);

            Assert.AreEqual(Node.State.ACTIVE, sut.CurrentState);
            Assert.AreEqual(Node.State.INACTIVE, firstChild.CurrentState);
            Assert.AreEqual(Node.State.ACTIVE, secondChild.CurrentState);

            sut.StopLowerPriorityChildrenForChild(firstChild, true);

            Assert.AreEqual(Node.State.INACTIVE, sut.CurrentState);
            Assert.AreEqual(Node.State.INACTIVE, firstChild.CurrentState);
            Assert.AreEqual(Node.State.INACTIVE, secondChild.CurrentState);
            Assert.IsTrue(behaviorTree.DidFinish);
            Assert.IsFalse(behaviorTree.WasSuccess);
        }
    }
}