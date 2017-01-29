using UnityEngine.Assertions;
using System;

namespace NPBehave
{
    public class Condition : Decorator
    {
        private Func<bool> condition;

        public Condition(Func<bool> condition, Node decoratee) : base("Condition", decoratee)
        {
            this.condition = condition;
        }

        protected override void DoStart()
        {
            if (!condition.Invoke())
            {
                Stopped(false);
            }
            else
            {
                Decoratee.Start();
            }
        }

        override protected void DoStop()
        {
            Decoratee.Stop();
        }

        protected override void DoChildStopped(Node child, bool result)
        {
            Assert.AreNotEqual(this.CurrentState, State.INACTIVE);
            Stopped(result);
        }
    }
}