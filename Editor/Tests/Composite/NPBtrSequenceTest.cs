using NUnit.Framework;
using NPBehave;

public class NPBtrSequenceTest : NPBtrTest
{
    [Test]
    public void ShouldFail_WhenSingleChildFails()
    {
        NPBtrMockNode failingChild = new NPBtrMockNode();
        Sequence sut = new Sequence(failingChild);
        NPBtrTestRoot behaviorTree = CreateBehaviorTree(sut);

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
        NPBtrMockNode succeedingChild = new NPBtrMockNode();
        Sequence sut = new Sequence(succeedingChild);
        NPBtrTestRoot behaviorTree = CreateBehaviorTree(sut);

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
        NPBtrMockNode failingChild = new NPBtrMockNode(false);
        Sequence sut = new Sequence(failingChild);
        NPBtrTestRoot behaviorTree = CreateBehaviorTree(sut);

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
        NPBtrMockNode succeedingChild = new NPBtrMockNode(true);
        Sequence sut = new Sequence(succeedingChild);
        NPBtrTestRoot behaviorTree = CreateBehaviorTree(sut);

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
        NPBtrMockNode firstChild = new NPBtrMockNode();
        NPBtrMockNode secondChild = new NPBtrMockNode();
        Sequence sut = new Sequence(firstChild, secondChild);
        NPBtrTestRoot behaviorTree = CreateBehaviorTree(sut);

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
        NPBtrMockNode firstChild = new NPBtrMockNode();
        NPBtrMockNode secondChild = new NPBtrMockNode();
        Sequence sut = new Sequence(firstChild, secondChild);
        NPBtrTestRoot behaviorTree = CreateBehaviorTree(sut);

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
        NPBtrMockNode firstChild = new NPBtrMockNode();
        NPBtrMockNode secondChild = new NPBtrMockNode();
        Sequence sut = new Sequence(firstChild, secondChild);
        NPBtrTestRoot behaviorTree = CreateBehaviorTree(sut);

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
        NPBtrMockNode firstChild = new NPBtrMockNode();
        NPBtrMockNode secondChild = new NPBtrMockNode(true);
        Sequence sut = new Sequence(firstChild, secondChild);
        NPBtrTestRoot behaviorTree = CreateBehaviorTree(sut);

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
        NPBtrMockNode firstChild = new NPBtrMockNode();
        NPBtrMockNode secondChild = new NPBtrMockNode(false);
        Sequence sut = new Sequence(firstChild, secondChild);
        NPBtrTestRoot behaviorTree = CreateBehaviorTree(sut);

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
