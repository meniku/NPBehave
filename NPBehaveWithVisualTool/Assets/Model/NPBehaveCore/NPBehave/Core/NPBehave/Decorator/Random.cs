using NPBehave_Core;

namespace NPBehave
{
    public class Random : Decorator
    {
        private float probability;

        public Random(float probability, Node decoratee) : base("Random", decoratee)
        {
            this.probability = probability;
        }

        protected override void DoStart()
        {
            if (Mathf.Random() <= this.probability)
            {
                Decoratee.Start();
            }
            else
            {
                Stopped(false);
            }
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