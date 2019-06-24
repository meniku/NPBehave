using NUnit.Framework;
namespace NPBehave
{

    public class RandomSelectorTest : Test
    {
        [Test]
        public void ShouldFail_WhenSingleChildFails()
        {
            MockNode failingChild = new MockNode();
            RandomSelector sut = new RandomSelector( failingChild );
            TestRoot behaviorTree = CreateBehaviorTree( sut );

            behaviorTree.Start();

            Assert.AreEqual( Node.State.ACTIVE, sut.CurrentState );

            failingChild.Finish( false );

            Assert.AreEqual( Node.State.INACTIVE, sut.CurrentState );
            Assert.IsTrue( behaviorTree.DidFinish );
            Assert.IsFalse( behaviorTree.WasSuccess );
        }

        [Test]
        public void ShouldSucceed_WhenSingleChildSucceeds()
        {
            MockNode succeedingChild = new MockNode();
            RandomSelector sut = new RandomSelector( succeedingChild );
            TestRoot behaviorTree = CreateBehaviorTree( sut );

            behaviorTree.Start();

            Assert.AreEqual( Node.State.ACTIVE, sut.CurrentState );

            succeedingChild.Finish( true );

            Assert.AreEqual( Node.State.INACTIVE, sut.CurrentState );
            Assert.IsTrue( behaviorTree.DidFinish );
            Assert.IsTrue( behaviorTree.WasSuccess );
        }

        [Test]
        public void ShouldFail_WhenStoppedExplicitly()
        {
            MockNode failingChild = new MockNode( false );
            RandomSelector sut = new RandomSelector( failingChild );
            TestRoot behaviorTree = CreateBehaviorTree( sut );

            behaviorTree.Start();

            Assert.AreEqual( Node.State.ACTIVE, sut.CurrentState );

            sut.Stop();

            Assert.AreEqual( Node.State.INACTIVE, sut.CurrentState );
            Assert.IsTrue( behaviorTree.DidFinish );
            Assert.IsFalse( behaviorTree.WasSuccess );
        }

        [Test]
        public void ShouldSucceed_WhenStoppedExplicitlyButChildStillFinishesSuccessfully()
        {
            MockNode succeedingChild = new MockNode( true );
            RandomSelector sut = new RandomSelector( succeedingChild );
            TestRoot behaviorTree = CreateBehaviorTree( sut );

            behaviorTree.Start();

            Assert.AreEqual( Node.State.ACTIVE, sut.CurrentState );

            sut.Stop();

            Assert.AreEqual( Node.State.INACTIVE, sut.CurrentState );
            Assert.IsTrue( behaviorTree.DidFinish );
            Assert.True( behaviorTree.WasSuccess );
        }

        [Test]
        public void ShouldSucceed_WhenFirstChildSuccessful()
        {
            MockNode firstChild = new MockNode();
            MockNode secondChild = new MockNode();
            RandomSelector sut = new RandomSelector( firstChild, secondChild );
            TestRoot behaviorTree = CreateBehaviorTree( sut );

            behaviorTree.Start();

            MockNode firstActiveChild = sut.DebugGetActiveChild() as MockNode;
            MockNode inactiveChild = firstActiveChild == firstChild ? secondChild : firstChild;
            Assert.IsNotNull( firstActiveChild );

            Assert.AreEqual( Node.State.ACTIVE, sut.CurrentState );
            Assert.AreEqual( Node.State.ACTIVE, firstActiveChild.CurrentState );
            Assert.AreEqual( Node.State.INACTIVE, inactiveChild.CurrentState );

            firstActiveChild.Finish( true );

            Assert.AreEqual( Node.State.INACTIVE, sut.CurrentState );
            Assert.AreEqual( Node.State.INACTIVE, firstChild.CurrentState );
            Assert.AreEqual( Node.State.INACTIVE, secondChild.CurrentState );
            Assert.IsTrue( behaviorTree.DidFinish );
            Assert.IsTrue( behaviorTree.WasSuccess );
        }

        [Test]
        public void ShouldProcceedToSecondChild_WhenFirstChildFailed()
        {
            MockNode firstChild = new MockNode();
            MockNode secondChild = new MockNode();
            RandomSelector sut = new RandomSelector( firstChild, secondChild );
            TestRoot behaviorTree = CreateBehaviorTree( sut );

            behaviorTree.Start();

            MockNode firstActiveChild = sut.DebugGetActiveChild() as MockNode;
            MockNode secondActiveChild = firstActiveChild == firstChild ? secondChild : firstChild;
            Assert.IsNotNull( firstActiveChild );

            Assert.AreEqual( Node.State.ACTIVE, sut.CurrentState );
            Assert.AreEqual( Node.State.ACTIVE, firstActiveChild.CurrentState );
            Assert.AreEqual( Node.State.INACTIVE, secondActiveChild.CurrentState );

            firstActiveChild.Finish( false );

            Assert.AreEqual( Node.State.ACTIVE, sut.CurrentState );
            Assert.AreEqual( Node.State.INACTIVE, firstActiveChild.CurrentState );
            Assert.AreEqual( Node.State.ACTIVE, secondActiveChild.CurrentState );

            secondActiveChild.Finish( false );

            Assert.AreEqual( Node.State.INACTIVE, sut.CurrentState );
            Assert.AreEqual( Node.State.INACTIVE, firstActiveChild.CurrentState );
            Assert.AreEqual( Node.State.INACTIVE, secondActiveChild.CurrentState );
            Assert.IsTrue( behaviorTree.DidFinish );
            Assert.IsFalse( behaviorTree.WasSuccess );
        }

        [Test]
        public void StopLowerPriorityChildrenForChild_WithoutImmediateRestart_ShouldCancelSecondChild()
        {
            MockNode firstChild = new MockNode();
            MockNode secondChild = new MockNode();
            RandomSelector sut = new RandomSelector( firstChild, secondChild );
            TestRoot behaviorTree = CreateBehaviorTree( sut );

            // TODO: will we keep the priority or will we switch to the priority defined by the randomized children?
            RandomSelector.DebugSetSeed( 2 );

            behaviorTree.Start();
            firstChild.Finish( false );

            Assert.AreEqual( Node.State.ACTIVE, sut.CurrentState );
            Assert.AreEqual( Node.State.INACTIVE, firstChild.CurrentState );
            Assert.AreEqual( Node.State.ACTIVE, secondChild.CurrentState );

            sut.StopLowerPriorityChildrenForChild( firstChild, false );

            Assert.AreEqual( Node.State.INACTIVE, sut.CurrentState );
            Assert.AreEqual( Node.State.INACTIVE, firstChild.CurrentState );
            Assert.AreEqual( Node.State.INACTIVE, secondChild.CurrentState );
            Assert.IsTrue( behaviorTree.DidFinish );
            Assert.IsFalse( behaviorTree.WasSuccess );
        }

        [Test]
        public void StopLowerPriorityChildrenForChild_WithImmediateRestart_ShouldRestartFirstChild_WhenSecondChildFails()
        {
            MockNode firstChild = new MockNode();
            MockNode secondChild = new MockNode( false );
            RandomSelector sut = new RandomSelector( firstChild, secondChild );
            TestRoot behaviorTree = CreateBehaviorTree( sut );

            // TODO: will we keep the priority or will we switch to the priority defined by the randomized children?
            RandomSelector.DebugSetSeed( 2 );

            behaviorTree.Start();
            firstChild.Finish( false );

            Assert.AreEqual( Node.State.ACTIVE, sut.CurrentState );
            Assert.AreEqual( Node.State.INACTIVE, firstChild.CurrentState );
            Assert.AreEqual( Node.State.ACTIVE, secondChild.CurrentState );

            sut.StopLowerPriorityChildrenForChild( firstChild, true );

            Assert.AreEqual( Node.State.ACTIVE, sut.CurrentState );
            Assert.AreEqual( Node.State.ACTIVE, firstChild.CurrentState );
            Assert.AreEqual( Node.State.INACTIVE, secondChild.CurrentState );
            Assert.IsFalse( behaviorTree.DidFinish );
        }

        [Test]
        public void StopLowerPriorityChildrenForChild_WithImmediateRestart_ShouldNotRestartFirstChild_WhenSecondChildSucceeds()
        {
            MockNode firstChild = new MockNode();
            MockNode secondChild = new MockNode( true );
            RandomSelector sut = new RandomSelector( firstChild, secondChild );
            TestRoot behaviorTree = CreateBehaviorTree( sut );

            // TODO: will we keep the priority or will we switch to the priority defined by the randomized children?
            RandomSelector.DebugSetSeed( 2 );

            behaviorTree.Start();
            firstChild.Finish( false );

            Assert.AreEqual( Node.State.ACTIVE, sut.CurrentState );
            Assert.AreEqual( Node.State.INACTIVE, firstChild.CurrentState );
            Assert.AreEqual( Node.State.ACTIVE, secondChild.CurrentState );

            sut.StopLowerPriorityChildrenForChild( firstChild, true );

            Assert.AreEqual( Node.State.INACTIVE, sut.CurrentState );
            Assert.AreEqual( Node.State.INACTIVE, firstChild.CurrentState );
            Assert.AreEqual( Node.State.INACTIVE, secondChild.CurrentState );
            Assert.IsTrue( behaviorTree.DidFinish );
            Assert.IsTrue( behaviorTree.WasSuccess );
        }
    }
}