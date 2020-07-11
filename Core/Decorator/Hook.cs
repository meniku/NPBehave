using System;

namespace NPBehave
{
    public class Hook : Decorator
    {
        private System.Action onStart;

        public Hook( System.Action onStart, Node decoratee) : base("Hook", decoratee)
        {
            this.onStart = onStart;
        }

        protected override void DoStart()
        {
            this.onStart();
            Decoratee.Start();
        }

        override protected void DoStop()
        {
            Decoratee.Stop();
        }

        protected override void DoChildStopped(Node child, bool result)
        {
            Stopped(result);
        }
    }
}