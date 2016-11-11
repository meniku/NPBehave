using UnityEngine;
using System.Collections;
using UnityEngine.Assertions;

namespace NPBehave
{
    public class BlackboardQuery : Decorator
    {
        private string[] keys;
        private System.Func<bool> query;
        private Stops stopsOnChange;

        public BlackboardQuery(string[] keys, Stops stopsOnChange, System.Func<bool> query, Node decoratee) : base("BlackboardCondition", decoratee)
        {
            this.keys = keys;
            this.query = query;
            this.stopsOnChange = stopsOnChange;
        }

        protected override void DoStart()
        {
            if (stopsOnChange != Stops.NONE)
            {
                foreach (string key in this.keys)
                {
                    this.RootNode.Blackboard.AddObserver(key, OnValueChanged);
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
            Assert.AreNotEqual(this.CurrentState, State.INACTIVE);
            if (stopsOnChange == Stops.NONE || stopsOnChange == Stops.SELF)
            {
                foreach (string key in this.keys)
                {
                    this.RootNode.Blackboard.RemoveObserver(key, OnValueChanged);
                }
            }
            Stopped(false);
        }

        override protected void DoParentCompositeStopped(Composite parentComposite)
        {
            foreach (string key in this.keys)
            {
                this.RootNode.Blackboard.RemoveObserver(key, OnValueChanged);
            }
        }

        private void OnValueChanged(Blackboard.Type type, object newValue)
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
                if (stopsOnChange == Stops.LOWER_PRIORITY || stopsOnChange == Stops.BOTH || stopsOnChange == Stops.IMMEDIATE_RESTART || stopsOnChange == Stops.LOWER_PRIORITY_IMMEDIATE_RESTART)
                {
                    // Debug.Log( this.key + " stopped other ");
                    Container parentNode = this.ParentNode;
                    Node childNode = this;
                    while (parentNode != null && !(parentNode is Composite))
                    {
                        childNode = parentNode;
                        parentNode = parentNode.ParentNode;
                    }
                    Assert.IsNotNull(parentNode, "NTBtrStops is only valid when attached to a parent composite");
                    Assert.IsNotNull(childNode);
                    if (parentNode is Parallel)
                    {
                        Assert.IsTrue(stopsOnChange == Stops.IMMEDIATE_RESTART, "On Parallel Nodes all children have the same priority, thus Stops.LOWER_PRIORITY or Stops.BOTH are unsupported in this context!");
                    }

                    ((Composite)parentNode).StopLowerPriorityChildrenForChild(childNode, stopsOnChange == Stops.IMMEDIATE_RESTART || stopsOnChange == Stops.LOWER_PRIORITY_IMMEDIATE_RESTART);
                }
            }
        }

        private bool IsConditionMet()
        {
            return this.query();
        }

        override public string ToString()
        {
            string keys = "";
            foreach (string key in this.keys)
            {
                keys += " " + key;
            }
            return Name + keys;
        }
    }
}