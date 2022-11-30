namespace NPBehave
{
    public abstract class Task : Node
    {
        public Task(string name) : base(name)
        {
        }

        public override void Pause()
        {
            Stop();
        }

        public override void Resume()
        {
            Start();
        }
    }
}