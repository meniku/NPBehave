using NUnit.Framework;

namespace NPBehave
{
    public class CooldownTest : Test
    {
		[Test]
		public void ShouldSucceedAndSetCooldown_WhenDecorateeSucceeds()
		{
			MockNode child = new MockNode();
			Cooldown sut = new Cooldown(1, child);
			TestRoot behaviorTree = CreateBehaviorTree(sut);

			// start
			behaviorTree.Start();
			Assert.AreEqual(Node.State.ACTIVE, sut.CurrentState);

			// make child suceed
			child.Finish(true);

			// ensure we're stopped
			Assert.AreEqual(Node.State.INACTIVE, sut.CurrentState);

			// start again
			behaviorTree.Start();
			Assert.AreEqual(Node.State.ACTIVE, sut.CurrentState);

			// ensure the child has not been started ( due to cooldown )
			Assert.AreEqual(Node.State.INACTIVE, child.CurrentState);

			// advance clock past cooldown and check that the child has been activated
			behaviorTree.Clock.Update( 2.0f );
			Assert.AreEqual(Node.State.ACTIVE, child.CurrentState);
		}

        [Test]
		public void ShouldFailAndSetCooldown_WhenDecorateeFails()
        {
            MockNode failingChild = new MockNode();
			Cooldown sut = new Cooldown(1, failingChild);
            TestRoot behaviorTree = CreateBehaviorTree(sut);

			// start
            behaviorTree.Start();
            Assert.AreEqual(Node.State.ACTIVE, sut.CurrentState);

			// make child fail
            failingChild.Finish(false);

			// ensure we're stopped
            Assert.AreEqual(Node.State.INACTIVE, sut.CurrentState);

			// start again
			behaviorTree.Start();
			Assert.AreEqual(Node.State.ACTIVE, sut.CurrentState);

			// ensure the child has not been started ( due to cooldown )
			Assert.AreEqual(Node.State.INACTIVE, failingChild.CurrentState);

			// advance clock past cooldown and check that the child has been activated
			behaviorTree.Clock.Update( 2.0f );
			Assert.AreEqual(Node.State.ACTIVE, failingChild.CurrentState);
        }

		[Test]
		public void ShouldFailAndNotSetCooldown_WhenDecorateeFails_DueToParameter()
		{
			MockNode failingChild = new MockNode();
			Cooldown sut = new Cooldown(1, false, true, failingChild);
			TestRoot behaviorTree = CreateBehaviorTree(sut);

			// start
			behaviorTree.Start();
			Assert.AreEqual(Node.State.ACTIVE, sut.CurrentState);

			// make child fail
			failingChild.Finish(false);

			// ensure we're stopped
			Assert.AreEqual(Node.State.INACTIVE, sut.CurrentState);

			// start again
			behaviorTree.Start();
			Assert.AreEqual(Node.State.ACTIVE, sut.CurrentState);

			// ensure the child has been started again ( due to cooldown not active )
			Assert.AreEqual(Node.State.ACTIVE, failingChild.CurrentState);
		}

		[Test]
		public void ShouldFailInsteadOfWait_WhenCooldownActive_DueToParameter()
		{
			MockNode child = new MockNode();
			Cooldown sut = new Cooldown(1, false, false, true, child);
			TestRoot behaviorTree = CreateBehaviorTree(sut);

			// start
			behaviorTree.Start();
			Assert.AreEqual(Node.State.ACTIVE, sut.CurrentState);

			// make child suceed
			child.Finish(true);

			// ensure we're stopped
			Assert.AreEqual(Node.State.INACTIVE, sut.CurrentState);

			// start again
			behaviorTree.Start();

			// ensure that neither this node nor the child has not been started ( due to cooldown )
			Assert.AreEqual(Node.State.INACTIVE, sut.CurrentState);
			Assert.AreEqual(Node.State.INACTIVE, child.CurrentState);

			// advance clock past cooldown, start the tree again and check that we could be activated again
			behaviorTree.Clock.Update( 2.0f );
			behaviorTree.Start();
			Assert.AreEqual(Node.State.ACTIVE, sut.CurrentState);
			Assert.AreEqual(Node.State.ACTIVE, child.CurrentState);
		}

		[Test]
		public void ShouldSucceedAndSetCooldownRightAway_WhenDecorateeSucceeds()
		{
			MockNode child = new MockNode();
			Cooldown sut = new Cooldown(1, child);
			TestRoot behaviorTree = CreateBehaviorTree(sut);

			// start
			behaviorTree.Start();
			Assert.AreEqual(Node.State.ACTIVE, sut.CurrentState);

			// wait 1.5 seconds
			behaviorTree.Clock.Update( 1.5f );

			// make child suceed
			child.Finish(true);

			// ensure we're stopped
			Assert.AreEqual(Node.State.INACTIVE, sut.CurrentState);

			// start again
			behaviorTree.Start();
			Assert.AreEqual(Node.State.ACTIVE, sut.CurrentState);

			// ensure the child has been started as well ( due to cooldown )
			Assert.AreEqual(Node.State.ACTIVE, child.CurrentState);
		}

		[Test]
		public void ShouldSucceedButSetCooldownAfterDecorator_WhenDecorateeSucceeds_DueToParameter()
		{
			MockNode child = new MockNode();
			Cooldown sut = new Cooldown(1, true, false, false, child);
			TestRoot behaviorTree = CreateBehaviorTree(sut);

			// start
			behaviorTree.Start();
			Assert.AreEqual(Node.State.ACTIVE, sut.CurrentState);

			// wait 1.5 seconds 
			behaviorTree.Clock.Update( 1.5f );

			// make child suceed
			child.Finish(true);

			// ensure we're stopped
			Assert.AreEqual(Node.State.INACTIVE, sut.CurrentState);

			// start again
			behaviorTree.Start();
			Assert.AreEqual(Node.State.ACTIVE, sut.CurrentState);

			// ensure the child has not been started ( due to cooldown )
			Assert.AreEqual(Node.State.INACTIVE, child.CurrentState);

			// advance clock to be at 2.0 seconds
			behaviorTree.Clock.Update( 0.5f );

			// ensure the child has not been started ( due to cooldown )
			Assert.AreEqual(Node.State.INACTIVE, child.CurrentState);

			// advance clock to be at 3 seconds
			behaviorTree.Clock.Update( 1.0f );

			// ensure the child has been started 
			Assert.AreEqual(Node.State.ACTIVE, child.CurrentState);
		}
    }
}