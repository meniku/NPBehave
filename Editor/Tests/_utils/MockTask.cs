namespace NPBehave
{
    public class MockTask : Task
    {
        private bool suceedsOnExplicitStop;
        
        public MockTask(bool suceedsOnExplicitStop) : base("MockTask")
        {
            this.suceedsOnExplicitStop = suceedsOnExplicitStop;
        }

        protected override void DoStop()
        {
            this.Stopped(suceedsOnExplicitStop);
        }

        public void Finish(bool success)
        {
            this.Stopped(success);
        }
    }
}