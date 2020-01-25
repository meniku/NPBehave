using System;
using System.Diagnostics;

namespace NPBehave
{
    public class WaitForCondition: Decorator
    {
        private Func<bool> condition;
        private float checkInterval;
        private float checkVariance;

        public WaitForCondition(Func<bool> condition, float checkInterval, float randomVariance, Node decoratee): base("WaitForCondition", decoratee)
        {
            this.condition = condition;

            this.checkInterval = checkInterval;
            this.checkVariance = randomVariance;

            this.Label = "" + (checkInterval - randomVariance) + "..." + (checkInterval + randomVariance) + "s";
        }

        public WaitForCondition(Func<bool> condition, Node decoratee): base("WaitForCondition", decoratee)
        {
            this.condition = condition;
            this.checkInterval = 0.0f;
            this.checkVariance = 0.0f;
            this.Label = "every tick";
        }

        protected override void DoStart()
        {
            if (!condition.Invoke())
            {
                Clock.AddTimer(checkInterval, checkVariance, -1, checkCondition);
            }
            else
            {
                Decoratee.Start();
            }
        }

        private void checkCondition()
        {
            if (condition.Invoke())
            {
                Clock.RemoveTimer(checkCondition);
                Decoratee.Start();
            }
        }

        override protected void DoStop()
        {
            Clock.RemoveTimer(checkCondition);
            if (Decoratee.IsActive)
            {
                Decoratee.Stop();
            }
            else
            {
                Stopped(false);
            }
        }

        protected override void DoChildStopped(Node child, bool result)
        {
            Debug.Assert(this.CurrentState != State.INACTIVE);
            Stopped(result);
        }
    }
}