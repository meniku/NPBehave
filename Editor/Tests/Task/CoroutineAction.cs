using NUnit.Framework;

namespace NPBehave
{
    public class CoroutineActionTest
    {
        private int firstStepRunCount;
        private int secondStepRunCount;
        private int thirdStepRunCount;

        [SetUp]
        public void SetUp()
        {
            this.firstStepRunCount = 0;
            this.secondStepRunCount = 0;
            this.thirdStepRunCount = 0;
        }

        private System.Collections.IEnumerator TestCoroutine()
        {
            this.firstStepRunCount++;
            yield return new WaitForSeconds(0.05f);
            this.secondStepRunCount++;
            yield return null;
            this.thirdStepRunCount++;
        }

        [Test]
        public void AllStepsShouldRun_WhenDoesntYieldFailure()
        {
            var timer = new Clock();
            var blackboard = new Blackboard(timer);
            var action = new CoroutineAction(TestCoroutine);
            var tree = new TestRoot(blackboard, timer, action);

            tree.Start(); // First step should run
            timer.Update(0.1f); // Tick - Second step should run
            timer.Update(0.1f); // Tick - Third step should run

            Assert.AreEqual(1, firstStepRunCount);
            Assert.AreEqual(1, secondStepRunCount);
            Assert.AreEqual(1, thirdStepRunCount);
        }

        [Test]
        public void ShouldEndWithSuccess_WhenDoesntYieldFailure()
        {
            var timer = new Clock();
            var blackboard = new Blackboard(timer);
            var action = new CoroutineAction(TestCoroutine);
            var tree = new TestRoot(blackboard, timer, action);

            tree.Start(); // First step should run
            timer.Update(0.1f); // Tick - Second step should run
            timer.Update(0.1f); // Tick - Third step should run

            Assert.AreEqual(1, firstStepRunCount);
            Assert.AreEqual(1, secondStepRunCount);
            Assert.AreEqual(1, thirdStepRunCount);

            Assert.AreEqual(Node.State.INACTIVE, action.CurrentState); // Action should have ended...
            Assert.IsTrue(action.DebugLastResult); // ... with success
        }

        private System.Collections.IEnumerator TestFailCoroutine()
        {
            this.firstStepRunCount++;
            yield return new WaitForSeconds(0.05f);
            this.secondStepRunCount++;
            yield return Action.Result.FAILED;
            this.thirdStepRunCount++;
            yield return null;
        }

        [Test]
        public void StepAfterFailureShouldNotRun_WhenYieldsFailure()
        {
            var timer = new Clock();
            var blackboard = new Blackboard(timer);
            var action = new CoroutineAction(TestFailCoroutine);
            var tree = new TestRoot(blackboard, timer, action);

            tree.Start(); // First step should run
            timer.Update(0.1f); // Tick - Second step should run
            timer.Update(0.1f); // Tick - Third step should run

            Assert.AreEqual(1, firstStepRunCount);
            Assert.AreEqual(1, secondStepRunCount);
            Assert.AreEqual(0, thirdStepRunCount); // Third step should not run as coroutine is failing earlier
        }

        [Test]
        public void ShouldEndWithFailure_WhenYieldsFailure()
        {
            var timer = new Clock();
            var blackboard = new Blackboard(timer);
            var action = new CoroutineAction(TestFailCoroutine);
            var tree = new TestRoot(blackboard, timer, action);

            tree.Start(); // First step should run
            timer.Update(0.1f); // Tick - Second step should run
            timer.Update(0.1f); // Tick - Third step should run

            Assert.AreEqual(Node.State.INACTIVE, action.CurrentState); // Action should have ended...
            Assert.IsFalse(action.DebugLastResult); // ... with failure
        }
    }
}
