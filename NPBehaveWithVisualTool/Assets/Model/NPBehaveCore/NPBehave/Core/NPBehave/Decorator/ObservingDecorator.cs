using System.Diagnostics;

namespace NPBehave
{
    public abstract class ObservingDecorator: Decorator
    {
        protected Stops stopsOnChange;
        private bool isObserving;

        public ObservingDecorator(string name, Stops stopsOnChange, Node decoratee): base(name, decoratee)
        {
            this.stopsOnChange = stopsOnChange;
            this.isObserving = false;
        }

        protected override void DoStart()
        {
            if (stopsOnChange != Stops.NONE)
            {
                if (!isObserving)
                {
                    isObserving = true;
                    StartObserving();
                }
            }

            if (!IsConditionMet())
            {
                Stopped(false);
            }
            else
            {
                Decoratee.Start();
            }
        }

        override protected void DoStop()
        {
            Decoratee.Stop();
        }

        protected override void DoChildStopped(Node child, bool result)
        {
            Debug.Assert(this.CurrentState != State.INACTIVE);
            if (stopsOnChange == Stops.NONE || stopsOnChange == Stops.SELF)
            {
                if (isObserving)
                {
                    isObserving = false;
                    StopObserving();
                }
            }

            Stopped(result);
        }

        override protected void DoParentCompositeStopped(Composite parentComposite)
        {
            if (isObserving)
            {
                isObserving = false;
                StopObserving();
            }
        }

        protected void Evaluate()
        {
            if (IsActive && !IsConditionMet())
            {
                if (stopsOnChange == Stops.SELF || stopsOnChange == Stops.BOTH || stopsOnChange == Stops.IMMEDIATE_RESTART)
                {
                    // Debug.Log( this.key + " stopped self ");
                    this.Stop();
                }
            }
            else if (!IsActive && IsConditionMet())
            {
                if (stopsOnChange == Stops.LOWER_PRIORITY || stopsOnChange == Stops.BOTH || stopsOnChange == Stops.IMMEDIATE_RESTART ||
                    stopsOnChange == Stops.LOWER_PRIORITY_IMMEDIATE_RESTART)
                {
                    // Debug.Log( this.key + " stopped other ");
                    Container parentNode = this.ParentNode;
                    Node childNode = this;
                    while (parentNode != null && !(parentNode is Composite))
                    {
                        childNode = parentNode;
                        parentNode = parentNode.ParentNode;
                    }

                    Debug.Assert(parentNode != null, "NTBtrStops is only valid when attached to a parent composite");
                    Debug.Assert(childNode != null);
                    if (parentNode is Parallel)
                    {
                        Debug.Assert(stopsOnChange == Stops.IMMEDIATE_RESTART,
                            "On Parallel Nodes all children have the same priority, thus Stops.LOWER_PRIORITY or Stops.BOTH are unsupported in this context!");
                    }

                    if (stopsOnChange == Stops.IMMEDIATE_RESTART || stopsOnChange == Stops.LOWER_PRIORITY_IMMEDIATE_RESTART)
                    {
                        if (isObserving)
                        {
                            isObserving = false;
                            StopObserving();
                        }
                    }

                    ((Composite) parentNode).StopLowerPriorityChildrenForChild(childNode,
                        stopsOnChange == Stops.IMMEDIATE_RESTART || stopsOnChange == Stops.LOWER_PRIORITY_IMMEDIATE_RESTART);
                }
            }
        }

        protected abstract void StartObserving();

        protected abstract void StopObserving();

        protected abstract bool IsConditionMet();
    }
}