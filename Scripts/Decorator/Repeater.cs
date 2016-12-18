namespace NPBehave
{
    public class Repeater : Decorator
    {
        private int loopCount = -1;
        private int currentLoop;
        private bool stopRequested;

        /// <param name="loopCount">number of times to execute the decoratee. Set to -1 to repeat forever, be careful with endless loops!</param>
        /// <param name="decoratee">Decorated Node</param>
        public Repeater(int loopCount, Node decoratee) : base("Repeater", decoratee)
        {
            this.loopCount = loopCount;
        }

        /// <param name="decoratee">Decorated Node, repeated forever</param>
        public Repeater(Node decoratee) : base("Repeater", decoratee)
        {
        }

        protected override void DoStart()
        {
            if (loopCount != 0)
            {
                stopRequested = false;
                currentLoop = 0;
                Decoratee.Start();
            }
            else
            {
                this.Stopped(true);
            }
        }

        override protected void DoStop()
        {
            this.Clock.RemoveTimer(restartDecoratee);
            stopRequested = true;
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
            if (result)
            {
                if (stopRequested || (loopCount > 0 && ++currentLoop >= loopCount))
                {
                    Stopped(true);
                }
                else
                {
                    this.Clock.AddTimer(0, 0, restartDecoratee);
                }
            }
            else
            {
                Stopped(false);
            }
        }

        protected void restartDecoratee()
        {
            Decoratee.Start();
        }
    }
}