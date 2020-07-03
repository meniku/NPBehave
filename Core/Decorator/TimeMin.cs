using UnityEngine.Assertions;

namespace NPBehave
{
    public class TimeMin : Decorator
    {
        private float limit = 0.0f;
        private float randomVariation;
        private bool waitOnFailure = false;
        private bool isLimitReached = false;
        private bool isDecorateeDone = false;
        private bool isDecorateeSuccess = false;

        public TimeMin(float limit, Node decoratee) : base("TimeMin", decoratee)
        {
            this.limit = limit;
            this.randomVariation = this.limit * 0.05f;
            this.waitOnFailure = false;
            Assert.IsTrue(limit > 0f, "limit has to be set");
        }

        public TimeMin(float limit, bool waitOnFailure, Node decoratee) : base("TimeMin", decoratee)
        {
            this.limit = limit;
            this.randomVariation = this.limit * 0.05f;
            this.waitOnFailure = waitOnFailure;
            Assert.IsTrue(limit > 0f, "limit has to be set");
        }

        public TimeMin(float limit, float randomVariation, bool waitOnFailure, Node decoratee) : base("TimeMin", decoratee)
        {
            this.limit = limit;
            this.randomVariation = randomVariation;
            this.waitOnFailure = waitOnFailure;
            Assert.IsTrue(limit > 0f, "limit has to be set");
        }

        protected override void DoStart()
        {
            isDecorateeDone = false;
            isDecorateeSuccess = false;
            isLimitReached = false;
            Clock.AddTimer(limit, randomVariation, 0, TimeoutReached);
            Decoratee.Start();
        }

        override protected void DoStop()
        {
            if (Decoratee.IsActive)
            {
                Clock.RemoveTimer(TimeoutReached);
                isLimitReached = true;
                Decoratee.Stop();
            }
            else
            {
                Clock.RemoveTimer(TimeoutReached);
                Stopped(false);
            }
        }

        protected override void DoChildStopped(Node child, bool result)
        {
            isDecorateeDone = true;
            isDecorateeSuccess = result;
            if (isLimitReached || (!result && !waitOnFailure))
            {
                Clock.RemoveTimer(TimeoutReached);
                Stopped(isDecorateeSuccess);
            }
            else
            {
                Assert.IsTrue(Clock.HasTimer(TimeoutReached));
            }
        }

        private void TimeoutReached()
        {
            isLimitReached = true;
            if (isDecorateeDone)
            {
                Stopped(isDecorateeSuccess);
            }
            else
            {
                Assert.IsTrue(Decoratee.IsActive);
            }
        }
    }
}