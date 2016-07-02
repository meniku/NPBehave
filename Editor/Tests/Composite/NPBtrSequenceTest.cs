using UnityEngine;
using UnityEditor;
using NUnit.Framework;

public class NPBtrSequenceTest : NPBtrTest
{
    [Test]
    public void ShouldFail_WhenSingleChildFails()
    {
        NPBtrMockNode failingChild = new NPBtrMockNode();
        NPBtrSequence sut = new NPBtrSequence(failingChild);
        NPBtrTestRoot behaviorTree = CreateBehaviorTree(sut);

        behaviorTree.Start();
        
        Assert.AreEqual(NPBtrNode.State.ACTIVE, sut.CurrentState);

        failingChild.Finish(false);

        Assert.AreEqual(NPBtrNode.State.INACTIVE, sut.CurrentState);
        Assert.IsTrue(behaviorTree.DidFinish);
        Assert.IsFalse(behaviorTree.WasSuccess);
    }

    [Test]
    public void ShouldSucceed_WhenSingleChildSucceeds()
    {
        NPBtrMockNode succeedingChild = new NPBtrMockNode();
        NPBtrSequence sut = new NPBtrSequence(succeedingChild);
        NPBtrTestRoot behaviorTree = CreateBehaviorTree(sut);

        behaviorTree.Start();
        
        Assert.AreEqual(NPBtrNode.State.ACTIVE, sut.CurrentState);

        succeedingChild.Finish(true);

        Assert.AreEqual(NPBtrNode.State.INACTIVE, sut.CurrentState);
        Assert.IsTrue(behaviorTree.DidFinish);
        Assert.IsTrue(behaviorTree.WasSuccess);
    }

    [Test]
    public void ShouldFail_WhenStoppedExplicitly()
    {
        NPBtrMockNode failingChild = new NPBtrMockNode(false);
        NPBtrSequence sut = new NPBtrSequence(failingChild);
        NPBtrTestRoot behaviorTree = CreateBehaviorTree(sut);

        behaviorTree.Start();
        
        Assert.AreEqual(NPBtrNode.State.ACTIVE, sut.CurrentState);

        sut.Stop();

        Assert.AreEqual(NPBtrNode.State.INACTIVE, sut.CurrentState);
        Assert.IsTrue(behaviorTree.DidFinish);
        Assert.IsFalse(behaviorTree.WasSuccess);
    }

    [Test]
    public void ShouldSucceed_WhenStoppedExplicitlyButChildStillFinishesSuccessfully()
    {
        NPBtrMockNode succeedingChild = new NPBtrMockNode(true);
        NPBtrSequence sut = new NPBtrSequence(succeedingChild);
        NPBtrTestRoot behaviorTree = CreateBehaviorTree(sut);

        behaviorTree.Start();
        
        Assert.AreEqual(NPBtrNode.State.ACTIVE, sut.CurrentState);

        sut.Stop();

        Assert.AreEqual(NPBtrNode.State.INACTIVE, sut.CurrentState);
        Assert.IsTrue(behaviorTree.DidFinish);
        Assert.True(behaviorTree.WasSuccess);
    }

    [Test]
    public void ShouldFail_WhenFirstChildFails()
    {
        NPBtrMockNode firstChild = new NPBtrMockNode();
        NPBtrMockNode secondChild = new NPBtrMockNode();
        NPBtrSequence sut = new NPBtrSequence(firstChild, secondChild);
        NPBtrTestRoot behaviorTree = CreateBehaviorTree(sut);

        behaviorTree.Start();
        
        Assert.AreEqual(NPBtrNode.State.ACTIVE, sut.CurrentState);
        Assert.AreEqual(NPBtrNode.State.ACTIVE, firstChild.CurrentState);
        Assert.AreEqual(NPBtrNode.State.INACTIVE, secondChild.CurrentState);

        firstChild.Finish(false);
        
        Assert.AreEqual(NPBtrNode.State.INACTIVE, sut.CurrentState);
        Assert.AreEqual(NPBtrNode.State.INACTIVE, firstChild.CurrentState);
        Assert.AreEqual(NPBtrNode.State.INACTIVE, secondChild.CurrentState);
        Assert.IsTrue(behaviorTree.DidFinish);
        Assert.IsFalse(behaviorTree.WasSuccess);
    }

    [Test]
    public void ShouldProcceedToSecondChild_WhenFirstChildSucceeded()
    {
        NPBtrMockNode firstChild = new NPBtrMockNode();
        NPBtrMockNode secondChild = new NPBtrMockNode();
        NPBtrSequence sut = new NPBtrSequence(firstChild, secondChild);
        NPBtrTestRoot behaviorTree = CreateBehaviorTree(sut);

        behaviorTree.Start();
        
        Assert.AreEqual(NPBtrNode.State.ACTIVE, sut.CurrentState);
        Assert.AreEqual(NPBtrNode.State.ACTIVE, firstChild.CurrentState);
        Assert.AreEqual(NPBtrNode.State.INACTIVE, secondChild.CurrentState);

        firstChild.Finish(true);
        
        Assert.AreEqual(NPBtrNode.State.ACTIVE, sut.CurrentState);
        Assert.AreEqual(NPBtrNode.State.INACTIVE, firstChild.CurrentState);
        Assert.AreEqual(NPBtrNode.State.ACTIVE, secondChild.CurrentState);

        secondChild.Finish(false);
        
        Assert.AreEqual(NPBtrNode.State.INACTIVE, sut.CurrentState);
        Assert.AreEqual(NPBtrNode.State.INACTIVE, firstChild.CurrentState);
        Assert.AreEqual(NPBtrNode.State.INACTIVE, secondChild.CurrentState);
        Assert.IsTrue(behaviorTree.DidFinish);
        Assert.IsFalse(behaviorTree.WasSuccess);
    }

    [Test]
    public void StopLowerPriorityChildrenForChild_WithoutImmediateRestart_ShouldCancelSecondChild()
    {
        NPBtrMockNode firstChild = new NPBtrMockNode();
        NPBtrMockNode secondChild = new NPBtrMockNode();
        NPBtrSequence sut = new NPBtrSequence(firstChild, secondChild);
        NPBtrTestRoot behaviorTree = CreateBehaviorTree(sut);

        behaviorTree.Start();

        firstChild.Finish(true);
        
        Assert.AreEqual(NPBtrNode.State.ACTIVE, sut.CurrentState);
        Assert.AreEqual(NPBtrNode.State.INACTIVE, firstChild.CurrentState);
        Assert.AreEqual(NPBtrNode.State.ACTIVE, secondChild.CurrentState);

        sut.StopLowerPriorityChildrenForChild(firstChild, false);
        
        Assert.AreEqual(NPBtrNode.State.INACTIVE, sut.CurrentState);
        Assert.AreEqual(NPBtrNode.State.INACTIVE, firstChild.CurrentState);
        Assert.AreEqual(NPBtrNode.State.INACTIVE, secondChild.CurrentState);
        Assert.IsTrue(behaviorTree.DidFinish);
        Assert.IsFalse(behaviorTree.WasSuccess);
    }

    [Test]
    public void StopLowerPriorityChildrenForChild_WithImmediateRestart_ShouldRestartFirstChild_WhenSecondChildSucceeds()
    {
        NPBtrMockNode firstChild = new NPBtrMockNode();
        NPBtrMockNode secondChild = new NPBtrMockNode(true);
        NPBtrSequence sut = new NPBtrSequence(firstChild, secondChild);
        NPBtrTestRoot behaviorTree = CreateBehaviorTree(sut);

        behaviorTree.Start();
        firstChild.Finish(true);
        
        Assert.AreEqual(NPBtrNode.State.ACTIVE, sut.CurrentState);
        Assert.AreEqual(NPBtrNode.State.INACTIVE, firstChild.CurrentState);
        Assert.AreEqual(NPBtrNode.State.ACTIVE, secondChild.CurrentState);

        sut.StopLowerPriorityChildrenForChild(firstChild, true);
        
        Assert.AreEqual(NPBtrNode.State.ACTIVE, sut.CurrentState);
        Assert.AreEqual(NPBtrNode.State.ACTIVE, firstChild.CurrentState);
        Assert.AreEqual(NPBtrNode.State.INACTIVE, secondChild.CurrentState);
        Assert.IsFalse(behaviorTree.DidFinish);
    }

    [Test]
    public void StopLowerPriorityChildrenForChild_WithImmediateRestart_ShouldNotRestartFirstChild_WhenSecondChildFails()
    {
        NPBtrMockNode firstChild = new NPBtrMockNode();
        NPBtrMockNode secondChild = new NPBtrMockNode(false);
        NPBtrSequence sut = new NPBtrSequence(firstChild, secondChild);
        NPBtrTestRoot behaviorTree = CreateBehaviorTree(sut);

        behaviorTree.Start();

        firstChild.Finish(true);
        
        Assert.AreEqual(NPBtrNode.State.ACTIVE, sut.CurrentState);
        Assert.AreEqual(NPBtrNode.State.INACTIVE, firstChild.CurrentState);
        Assert.AreEqual(NPBtrNode.State.ACTIVE, secondChild.CurrentState);

        sut.StopLowerPriorityChildrenForChild(firstChild, true);
        
        Assert.AreEqual(NPBtrNode.State.INACTIVE, sut.CurrentState);
        Assert.AreEqual(NPBtrNode.State.INACTIVE, firstChild.CurrentState);
        Assert.AreEqual(NPBtrNode.State.INACTIVE, secondChild.CurrentState);
        Assert.IsTrue(behaviorTree.DidFinish);
        Assert.IsFalse(behaviorTree.WasSuccess);
    }
}
