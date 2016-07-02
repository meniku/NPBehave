using UnityEngine;
using UnityEditor;
using NUnit.Framework;

public class NPBtrParallelTest : NPBtrTest
{
    // private static NPBtrParallel.Wait[] ALL_WAITING_POLICIES = new NPBtrParallel.Wait[] { NPBtrParallel.Wait.NEVER, NPBtrParallel.Wait.BOTH, NPBtrParallel.Wait.ON_FAILURE, NPBtrParallel.Wait.ON_SUCCESS };
    private static NPBtrParallel.Policy[] ALL_SUCCESS_POLICIES = new NPBtrParallel.Policy[] { NPBtrParallel.Policy.ONE, NPBtrParallel.Policy.ALL };
    private static NPBtrParallel.Policy[] ALL_FAILURE_POLICIES = new NPBtrParallel.Policy[] { NPBtrParallel.Policy.ONE, NPBtrParallel.Policy.ALL };

    [Test]
    public void ShouldFail_WhenSingleChildFails()
    {
        foreach (NPBtrParallel.Policy sucessPolicy in ALL_SUCCESS_POLICIES) foreach (NPBtrParallel.Policy failurePolicy in ALL_FAILURE_POLICIES)
        {
            NPBtrMockNode failingChild = new NPBtrMockNode();
            NPBtrParallel sut = new NPBtrParallel(sucessPolicy, failurePolicy, failingChild);
            NPBtrTestRoot behaviorTree = CreateBehaviorTree(sut);

            behaviorTree.Start();
            Assert.AreEqual(NPBtrNode.State.ACTIVE, sut.CurrentState);

            failingChild.Finish(false);

            Assert.AreEqual(NPBtrNode.State.INACTIVE, sut.CurrentState);
            Assert.IsTrue(behaviorTree.DidFinish);
            Assert.IsFalse(behaviorTree.WasSuccess);
        }
    }

    [Test]
    public void ShouldSucceed_WhenSingleChildSucceeds()
    {
        foreach (NPBtrParallel.Policy sucessPolicy in ALL_SUCCESS_POLICIES) foreach (NPBtrParallel.Policy failurePolicy in ALL_FAILURE_POLICIES)
        {
            NPBtrMockNode succeedingChild = new NPBtrMockNode();
            NPBtrParallel sut = new NPBtrParallel(sucessPolicy, failurePolicy, succeedingChild);
            NPBtrTestRoot behaviorTree = CreateBehaviorTree(sut);

            behaviorTree.Start();
            Assert.AreEqual(NPBtrNode.State.ACTIVE, sut.CurrentState);

            succeedingChild.Finish(true);

            Assert.AreEqual(NPBtrNode.State.INACTIVE, sut.CurrentState);
            Assert.IsTrue(behaviorTree.DidFinish);
            Assert.IsTrue(behaviorTree.WasSuccess);
        }
    }

    [Test]
    public void ShouldFail_WhenStoppedExplicitly()
    {
        foreach (NPBtrParallel.Policy successPolicy in ALL_SUCCESS_POLICIES) foreach (NPBtrParallel.Policy failurePolicy in ALL_FAILURE_POLICIES)
        {
            NPBtrMockNode failingChild = new NPBtrMockNode(false);
            NPBtrParallel sut = new NPBtrParallel(successPolicy, failurePolicy, failingChild);
            NPBtrTestRoot behaviorTree = CreateBehaviorTree(sut);

            behaviorTree.Start();
            Assert.AreEqual(NPBtrNode.State.ACTIVE, sut.CurrentState);

            sut.Stop();

            Assert.AreEqual(NPBtrNode.State.INACTIVE, sut.CurrentState);
            Assert.IsTrue(behaviorTree.DidFinish);
            Assert.IsFalse(behaviorTree.WasSuccess);
        }
    }

    [Test]
    public void ShouldSucceed_WhenStoppedExplicitlyButChildStillFinishesSuccessfully()
    {
        foreach (NPBtrParallel.Policy successPolicy in ALL_SUCCESS_POLICIES) foreach (NPBtrParallel.Policy failurePolicy in ALL_FAILURE_POLICIES)
        {
            NPBtrMockNode succeedingChild = new NPBtrMockNode(true);
            NPBtrParallel sut = new NPBtrParallel(successPolicy, failurePolicy, succeedingChild);
            NPBtrTestRoot behaviorTree = CreateBehaviorTree(sut);

            behaviorTree.Start();
            Assert.AreEqual(NPBtrNode.State.ACTIVE, sut.CurrentState);

            sut.Stop();

            Assert.AreEqual(NPBtrNode.State.INACTIVE, sut.CurrentState);
            Assert.IsTrue(behaviorTree.DidFinish);
            Assert.True(behaviorTree.WasSuccess);
        }
    }

   [Test]
    public void ShouldStartAllChildrenInParallel()
    {
        NPBtrParallel.Policy successPolicy = NPBtrParallel.Policy.ONE;
        NPBtrParallel.Policy failurePolicy = NPBtrParallel.Policy.ONE;
        NPBtrMockNode firstChild = new NPBtrMockNode();
        NPBtrMockNode secondChild = new NPBtrMockNode();
        NPBtrParallel sut = new NPBtrParallel(successPolicy, failurePolicy, firstChild, secondChild);
        NPBtrTestRoot behaviorTree = CreateBehaviorTree(sut);

        behaviorTree.Start();
        
        Assert.AreEqual(NPBtrNode.State.ACTIVE, sut.CurrentState);
        Assert.AreEqual(NPBtrNode.State.ACTIVE, firstChild.CurrentState);
    }


    [Test]
    public void ShouldSucceed_WhenOneChildSuccessful_AndSuccessPolicyOne()
    {
        NPBtrParallel.Policy successPolicy = NPBtrParallel.Policy.ONE;
        foreach (NPBtrParallel.Policy failurePolicy in ALL_FAILURE_POLICIES)
        {
            NPBtrMockNode firstChild = new NPBtrMockNode();
            NPBtrMockNode secondChild = new NPBtrMockNode(false);
            NPBtrParallel sut = new NPBtrParallel(successPolicy, failurePolicy, firstChild, secondChild);
            NPBtrTestRoot behaviorTree = CreateBehaviorTree(sut);

            behaviorTree.Start();
            firstChild.Finish(true);
            
            Assert.AreEqual(NPBtrNode.State.INACTIVE, firstChild.CurrentState);
            Assert.AreEqual(NPBtrNode.State.INACTIVE, secondChild.CurrentState);
            Assert.IsTrue(behaviorTree.DidFinish);
            Assert.IsTrue(behaviorTree.WasSuccess);
        }
    }

    [Test]
    public void ShouldFail_WhenOneChildFailed_AndFailurePolicyOne()
    {
        NPBtrParallel.Policy failurePolicy = NPBtrParallel.Policy.ONE;
        foreach (NPBtrParallel.Policy successPolicy in ALL_SUCCESS_POLICIES)
        {
            NPBtrMockNode firstChild = new NPBtrMockNode();
            NPBtrMockNode secondChild = new NPBtrMockNode(true);
            NPBtrParallel sut = new NPBtrParallel(successPolicy, failurePolicy, firstChild, secondChild);
            NPBtrTestRoot behaviorTree = CreateBehaviorTree(sut);

            behaviorTree.Start();
            firstChild.Finish(false);
            
            Assert.AreEqual(NPBtrNode.State.INACTIVE, firstChild.CurrentState);
            Assert.AreEqual(NPBtrNode.State.INACTIVE, secondChild.CurrentState);
            Assert.IsTrue(behaviorTree.DidFinish);
            Assert.IsFalse(behaviorTree.WasSuccess);
        }
    }


    [Test]
    public void ShouldSucceed_WhenAllChildrenSuccessful_AndSuccessPolicyAll()
    {
        NPBtrParallel.Policy successPolicy = NPBtrParallel.Policy.ALL;
        foreach (NPBtrParallel.Policy failurePolicy in ALL_FAILURE_POLICIES)
        {
            NPBtrMockNode firstChild = new NPBtrMockNode();
            NPBtrMockNode secondChild = new NPBtrMockNode();
            NPBtrParallel sut = new NPBtrParallel(successPolicy, failurePolicy, firstChild, secondChild);
            NPBtrTestRoot behaviorTree = CreateBehaviorTree(sut);

            behaviorTree.Start();
            firstChild.Finish(true);
            secondChild.Finish(true);
            
            Assert.AreEqual(NPBtrNode.State.INACTIVE, sut.CurrentState);
            Assert.IsTrue(behaviorTree.DidFinish);
            Assert.IsTrue(behaviorTree.WasSuccess);
        }
    }

    [Test]
    public void ShouldFail_WhenNotAllChildrenSuccessful_AndSuccessPolicyAll()
    {
        NPBtrParallel.Policy successPolicy = NPBtrParallel.Policy.ALL;
        foreach (NPBtrParallel.Policy failurePolicy in ALL_FAILURE_POLICIES)
        {
            NPBtrMockNode firstChild = new NPBtrMockNode();
            NPBtrMockNode secondChild = new NPBtrMockNode();
            NPBtrParallel sut = new NPBtrParallel(successPolicy, failurePolicy, firstChild, secondChild);
            NPBtrTestRoot behaviorTree = CreateBehaviorTree(sut);

            behaviorTree.Start();
            firstChild.Finish(true);
            secondChild.Finish(false);
            
            Assert.AreEqual(NPBtrNode.State.INACTIVE, sut.CurrentState);
            Assert.IsTrue(behaviorTree.DidFinish);
            Assert.IsFalse(behaviorTree.WasSuccess);
        }
    }
    
    
    [Test]
    public void ShouldFail_WhenAllChildrenFailed_AndFailPolicyAll()
    {
        NPBtrParallel.Policy failurePolicy = NPBtrParallel.Policy.ALL;
        foreach (NPBtrParallel.Policy successPolicy in ALL_SUCCESS_POLICIES)
        {
            NPBtrMockNode firstChild = new NPBtrMockNode();
            NPBtrMockNode secondChild = new NPBtrMockNode();
            NPBtrParallel sut = new NPBtrParallel(successPolicy, failurePolicy, firstChild, secondChild);
            NPBtrTestRoot behaviorTree = CreateBehaviorTree(sut);

            behaviorTree.Start();
            firstChild.Finish(false);
            secondChild.Finish(false);
            
            Assert.AreEqual(NPBtrNode.State.INACTIVE, sut.CurrentState);
            Assert.IsTrue(behaviorTree.DidFinish);
            Assert.IsFalse(behaviorTree.WasSuccess);
        }
    }

    [Test]
    public void ShouldFail_WhenNotAllChildrenFailed_AndFailPolicyAll_AndSucessPolicyAll()
    {
        NPBtrParallel.Policy failurePolicy = NPBtrParallel.Policy.ALL;
        NPBtrParallel.Policy successPolicy = NPBtrParallel.Policy.ALL;
        NPBtrMockNode firstChild = new NPBtrMockNode();
        NPBtrMockNode secondChild = new NPBtrMockNode();
        NPBtrParallel sut = new NPBtrParallel(successPolicy, failurePolicy, firstChild, secondChild);
        NPBtrTestRoot behaviorTree = CreateBehaviorTree(sut);

        behaviorTree.Start();
        firstChild.Finish(true);
        secondChild.Finish(false);
        
        Assert.AreEqual(NPBtrNode.State.INACTIVE, sut.CurrentState);
        Assert.IsTrue(behaviorTree.DidFinish);
        Assert.IsFalse(behaviorTree.WasSuccess);
    }
    
    [Test]
    public void ShouldSucceed_WhenNotAllChildrenFailed_AndFailPolicyAll_AndSucessPolicyOne()
    {
        NPBtrParallel.Policy failurePolicy = NPBtrParallel.Policy.ALL;
        NPBtrParallel.Policy successPolicy = NPBtrParallel.Policy.ONE;
        NPBtrMockNode firstChild = new NPBtrMockNode();
        NPBtrMockNode secondChild = new NPBtrMockNode();
        NPBtrParallel sut = new NPBtrParallel(successPolicy, failurePolicy, firstChild, secondChild);
        NPBtrTestRoot behaviorTree = CreateBehaviorTree(sut);

        behaviorTree.Start();
        firstChild.Finish(false);
        secondChild.Finish(true);
        
        Assert.AreEqual(NPBtrNode.State.INACTIVE, sut.CurrentState);
        Assert.IsTrue(behaviorTree.DidFinish);
        Assert.IsTrue(behaviorTree.WasSuccess);
    }

    [Test]
    public void StopLowerPriorityChildrenForChild_WithImmediateRestart_ShouldRestartChild()
    {
        NPBtrParallel.Policy failurePolicy = NPBtrParallel.Policy.ALL;
        NPBtrParallel.Policy successPolicy = NPBtrParallel.Policy.ALL;
        
        NPBtrMockNode firstChild = new NPBtrMockNode();
        NPBtrMockNode secondChild = new NPBtrMockNode();
        NPBtrParallel sut = new NPBtrParallel(successPolicy, failurePolicy, firstChild, secondChild);
        NPBtrTestRoot behaviorTree = CreateBehaviorTree(sut);

        behaviorTree.Start();
        firstChild.Finish(false);
        sut.StopLowerPriorityChildrenForChild(firstChild, true);
        
        Assert.AreEqual(NPBtrNode.State.ACTIVE, sut.CurrentState);
        Assert.AreEqual(NPBtrNode.State.ACTIVE, firstChild.CurrentState);
        Assert.AreEqual(NPBtrNode.State.ACTIVE, secondChild.CurrentState);
    }
    
    [Test]
    [ExpectedException( typeof( NPBtrException ) )]
    public void StopLowerPriorityChildrenForChild_WithoutImmediateRestart_ShouldThrowError()
    {
        NPBtrParallel.Policy failurePolicy = NPBtrParallel.Policy.ALL;
        NPBtrParallel.Policy successPolicy = NPBtrParallel.Policy.ALL;
        
        NPBtrMockNode firstChild = new NPBtrMockNode();
        NPBtrMockNode secondChild = new NPBtrMockNode();
        NPBtrParallel sut = new NPBtrParallel(successPolicy, failurePolicy, firstChild, secondChild);
        NPBtrTestRoot behaviorTree = CreateBehaviorTree(sut);

        behaviorTree.Start();
        firstChild.Finish(false);
        sut.StopLowerPriorityChildrenForChild(firstChild, false);
        
    }
}
