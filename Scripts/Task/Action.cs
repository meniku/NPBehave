using UnityEngine.Assertions;

namespace NPBehave
{
    public class Action : Task
    {
        public enum Result
        {
            SUCCESS,
            FAILED,
            PROGRESS
        }


        private System.Func<bool> singleFrameFunc = null;
        private System.Func<bool, Result> multiFrameFunc = null;
        private System.Action action = null;

        public Action(System.Action action) : base("Action")
        {
            this.action = action;
        }

        public Action(System.Func<bool, Result> multiframeFunc) : base("Action")
        {
            this.multiFrameFunc = multiframeFunc;
        }

        public Action(System.Func<bool> singleFrameFunc) : base("Action")
        {
            this.singleFrameFunc = singleFrameFunc;
        }

        protected override void DoStart()
        {
            if (this.action != null)
            {
                this.action.Invoke();
                this.Stopped(true);
            }
            else if (this.multiFrameFunc != null)
            {
                Result result = this.multiFrameFunc.Invoke(false);
                if (result == Result.PROGRESS)
                {
                    this.RootNode.Clock.AddUpdateObserver(OnUpdateFunc);
                }
                else
                {
                    this.Stopped(result == Result.SUCCESS);
                }
            }
            else if (this.singleFrameFunc != null)
            {
                this.Stopped(this.singleFrameFunc.Invoke());
            }
        }

        private void OnUpdateFunc()
        {
            Result result = this.multiFrameFunc.Invoke(false);
            if (result != Result.PROGRESS)
            {
                this.RootNode.Clock.RemoveUpdateObserver(OnUpdateFunc);
                this.Stopped(result == Result.SUCCESS);
            }
        }

        protected override void DoStop()
        {
            if (this.multiFrameFunc != null)
            {
                Result result = this.multiFrameFunc.Invoke(true);
                Assert.AreNotEqual(result, Result.PROGRESS, "The Task has to return Result.SUCCESS or Result.FAILED after beeing cancelled!");
                this.RootNode.Clock.RemoveUpdateObserver(OnUpdateFunc);
                this.Stopped(result == Result.SUCCESS);
            }
            else
            {
                // Assert.IsTrue(false, "DoStop called for a single frame action on " + this);
                Assert.IsTrue(false, "DoStop called for a single frame action");
            }
        }
    }
}