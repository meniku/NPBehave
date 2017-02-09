using System;

namespace NPBehave
{
    public class Observer : Decorator
    {
        private System.Action onStart;
        private System.Action<bool> onStop;

        public Observer(System.Action onStart, System.Action<bool> onStop, Node decoratee) : base("Observer", decoratee)
        {
            this.onStart = onStart;
            this.onStop = onStop;
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
            this.onStop(result);
            Stopped(result);
        }
    }
}