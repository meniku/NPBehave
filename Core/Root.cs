﻿using System.Diagnostics;

namespace NPBehave
{
    public class Root : Decorator
    {
        //private Node mainNode;

        //private Node inProgressNode;

        private Blackboard blackboard;
        public override Blackboard Blackboard
        {
            get
            {
                return blackboard;
            }
        }


        private Clock clock;
        public override Clock Clock
        {
            get
            {
                return clock;
            }
        }

#if UNITY_EDITOR
        public int TotalNumStartCalls = 0;
        public int TotalNumStopCalls = 0;
        public int TotalNumStoppedCalls = 0;
#endif

        public Root(Node mainNode) : base("Root", mainNode)
        {
            this.clock = Context.Clock;
            this.blackboard = new Blackboard(this.clock);
            this.SetRoot(this);
        }
        public Root(Blackboard blackboard, Node mainNode) : base("Root", mainNode)
        {
            this.blackboard = blackboard;
            this.clock = Context.Clock;
            this.SetRoot(this);
        }

        public Root(Blackboard blackboard, Clock clock, Node mainNode) : base("Root", mainNode)
        {
            this.blackboard = blackboard;
            this.clock = clock;
            this.SetRoot(this);
        }

        public override void SetRoot(Root rootNode)
        {
            Debug.Assert( this == rootNode );
            base.SetRoot(rootNode);
        }


        override protected void DoStart()
        {
            this.blackboard.Enable();
            this.Decoratee.Start();
        }

        override protected void DoStop()
        {
            if (this.Decoratee.IsActive)
            {
                this.Decoratee.Stop();
            }
            else
            {
                this.clock.RemoveTimer(this.Decoratee.Start);
            }
        }


        override protected void DoChildStopped(Node node, bool success)
        {
            if (!IsStopRequested)
            {
                // wait one tick, to prevent endless recursions
                this.clock.AddTimer(0, 0, this.Decoratee.Start);
            }
            else
            {
                this.blackboard.Disable();
                Stopped(success);
            }
        }
    }
}
