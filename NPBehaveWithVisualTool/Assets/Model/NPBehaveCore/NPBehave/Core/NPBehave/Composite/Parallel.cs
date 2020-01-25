using System.Collections.Generic;
using System.Diagnostics;
using Sirenix.OdinInspector;

namespace NPBehave
{
    public class Parallel: Composite
    {
        public enum Policy
        {
            [LabelText("一个XX就返回XX")]
            ONE,
            [LabelText("全部XX才返回XX")]
            ALL,
        }

        // public enum Wait
        // {
        //     NEVER,
        //     ON_FAILURE,
        //     ON_SUCCESS,
        //     BOTH
        // }

        // private Wait waitForPendingChildrenRule;
        private Policy failurePolicy;
        private Policy successPolicy;
        private int childrenCount = 0;
        private int runningCount = 0;
        private int succeededCount = 0;
        private int failedCount = 0;
        private Dictionary<Node, bool> childrenResults;
        private bool successState;
        private bool childrenAborted;

        public Parallel(Policy successPolicy, Policy failurePolicy, /*Wait waitForPendingChildrenRule,*/ params Node[] children): base("Parallel",
            children)
        {
            this.successPolicy = successPolicy;
            this.failurePolicy = failurePolicy;
            // this.waitForPendingChildrenRule = waitForPendingChildrenRule;
            this.childrenCount = children.Length;
            this.childrenResults = new Dictionary<Node, bool>();
        }

        protected override void DoStart()
        {
            foreach (Node child in Children)
            {
                Debug.Assert(child.CurrentState == State.INACTIVE);
            }

            childrenAborted = false;
            runningCount = 0;
            succeededCount = 0;
            failedCount = 0;
            foreach (Node child in this.Children)
            {
                runningCount++;
                child.Start();
            }
        }

        protected override void DoStop()
        {
            Debug.Assert(runningCount + succeededCount + failedCount == childrenCount);

            foreach (Node child in this.Children)
            {
                if (child.IsActive)
                {
                    child.Stop();
                }
            }
        }

        protected override void DoChildStopped(Node child, bool result)
        {
            runningCount--;
            if (result)
            {
                succeededCount++;
            }
            else
            {
                failedCount++;
            }

            this.childrenResults[child] = result;

            bool allChildrenStarted = runningCount + succeededCount + failedCount == childrenCount;
            if (allChildrenStarted)
            {
                if (runningCount == 0)
                {
                    if (!this.childrenAborted
                    ) // if children got aborted because rule was evaluated previously, we don't want to override the successState 
                    {
                        if (failurePolicy == Policy.ONE && failedCount > 0)
                        {
                            successState = false;
                        }
                        else if (successPolicy == Policy.ONE && succeededCount > 0)
                        {
                            successState = true;
                        }
                        else if (successPolicy == Policy.ALL && succeededCount == childrenCount)
                        {
                            successState = true;
                        }
                        else
                        {
                            successState = false;
                        }
                    }

                    Stopped(successState);
                }
                else if (!this.childrenAborted)
                {
                    Debug.Assert(succeededCount != childrenCount);
                    Debug.Assert(failedCount != childrenCount);

                    if (failurePolicy == Policy.ONE &&
                        failedCount > 0 /* && waitForPendingChildrenRule != Wait.ON_FAILURE && waitForPendingChildrenRule != Wait.BOTH*/)
                    {
                        successState = false;
                        childrenAborted = true;
                    }
                    else if (
                        successPolicy == Policy.ONE &&
                        succeededCount > 0 /* && waitForPendingChildrenRule != Wait.ON_SUCCESS && waitForPendingChildrenRule != Wait.BOTH*/)
                    {
                        successState = true;
                        childrenAborted = true;
                    }

                    if (childrenAborted)
                    {
                        foreach (Node currentChild in this.Children)
                        {
                            if (currentChild.IsActive)
                            {
                                currentChild.Stop();
                            }
                        }
                    }
                }
            }
        }

        public override void StopLowerPriorityChildrenForChild(Node abortForChild, bool immediateRestart)
        {
            if (immediateRestart)
            {
                Debug.Assert(!abortForChild.IsActive);
                if (childrenResults[abortForChild])
                {
                    succeededCount--;
                }
                else
                {
                    failedCount--;
                }

                runningCount++;
                abortForChild.Start();
            }
            else
            {
                throw new Exception(
                    "On Parallel Nodes all children have the same priority, thus the method does nothing if you pass false to 'immediateRestart'!");
            }
        }
    }
}