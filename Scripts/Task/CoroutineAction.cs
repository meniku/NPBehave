using System.Collections;

namespace NPBehave
{
    public struct WaitForSeconds
    {
        public readonly float Seconds;
        public WaitForSeconds(float seconds)
        {
            this.Seconds = seconds;
        }
    }

    public class CoroutineAction : Task
    {
        public System.Func<IEnumerator> action;
        private IEnumerator actionCoroutine;

        public CoroutineAction(System.Func<IEnumerator> action) : base("Action")
        {
            this.action = action;
        }

        private void Progress()
        {
            if (actionCoroutine.Current is Action.Result.FAILED)
            {
                Stopped(false);
            }
            else if (actionCoroutine.Current is WaitForSeconds w)
            {
                if (actionCoroutine.MoveNext())
                {
                    Clock.AddTimer(w.Seconds, 0, Progress);
                }
                else
                {
                    Stopped(true);
                }
            }
            else if (actionCoroutine.Current == null)
            {
                if (actionCoroutine.MoveNext())
                {
                    Clock.AddTimer(0, 0, Progress);
                }
                else
                {
                    Stopped(true);
                }
            }
            else
            {
                throw new System.Exception("unsupported coroutine return value");
            }
        }

        protected override void DoStart()
        {
            this.actionCoroutine = action();
            Progress();
        }

        protected override void DoStop()
        {
        }
    }
}
