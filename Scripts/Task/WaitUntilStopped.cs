namespace NPBehave
{
    public class WaitUntilStopped : Task
    {
        private bool sucessWhenStopped;
        public WaitUntilStopped(bool sucessWhenStopped = false) : base("WaitUntilStopped")
        {
            this.sucessWhenStopped = sucessWhenStopped;
        }

        protected override void DoStop()
        {
            this.Stopped(sucessWhenStopped);
        }
    }
}