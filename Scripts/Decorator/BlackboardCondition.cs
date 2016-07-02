using UnityEngine;
using System.Collections;
using UnityEngine.Assertions;

namespace NPBehave
{
    public class BlackboardCondition : Decorator
    {
        private string key;
        private object value;
        private Operator op;
        private Stops stopsOnChange;

        public BlackboardCondition(string key, Operator op, object value, Stops stopsOnChange, Node decoratee) : base("BlackboardValue", decoratee)
        {
            this.op = op;
            this.key = key;
            this.value = value;
            this.stopsOnChange = stopsOnChange;
        }
        public BlackboardCondition(string key, Operator op, Stops stopsOnChange, Node decoratee) : base("BlackboardValue", decoratee)
        {
            this.op = op;
            this.key = key;
            this.stopsOnChange = stopsOnChange;
        }

        protected override void DoStart()
        {
            if (stopsOnChange != Stops.NONE)
            {
                this.RootNode.Blackboard.AddObserver(this.key, OnValueChanged);
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
                this.RootNode.Blackboard.RemoveObserver(this.key, OnValueChanged);
            }
            Stopped(false);
        }

        override protected void DoParentCompositeStopped(Composite parentComposite)
        {
            this.RootNode.Blackboard.RemoveObserver(this.key, OnValueChanged);
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
                        Assert.IsTrue(stopsOnChange == Stops.IMMEDIATE_RESTART, "On Parallel Nodes all children have the same priority, thus NPBtrStops.LOWER_PRIORITY or NPBtrStops.BOTH are unsupported in this context!");
                    }

                    ((Composite)parentNode).StopLowerPriorityChildrenForChild(childNode, stopsOnChange == Stops.IMMEDIATE_RESTART || stopsOnChange == Stops.LOWER_PRIORITY_IMMEDIATE_RESTART);
                }
            }
        }

        private bool IsConditionMet()
        {
            if (op == Operator.ON_CHANGE)
            {
                return true;
            }

            if (!this.RootNode.Blackboard.Isset(key))
            {
                return op == Operator.IS_NOT_SET;
            }

            object o = this.RootNode.Blackboard.Get(key);

            switch (this.op)
            {
                case Operator.IS_SET: return true;
                case Operator.IS_EQUAL: return object.Equals(o, value);
                case Operator.IS_NOT_EQUAL: return !object.Equals(o, value);

                case Operator.IS_GREATER_OR_EQUAL:
                    if (o is float)
                    {
                        return (float)o >= (float)this.value;
                    }
                    else if (o is int)
                    {
                        return (int)o >= (int)this.value;
                    }
                    else
                    {
                        Debug.LogError("Type not compareable: " + o.GetType());
                        return false;
                    }

                case Operator.IS_GREATER:
                    if (o is float)
                    {
                        return (float)o > (float)this.value;
                    }
                    else if (o is int)
                    {
                        return (int)o > (int)this.value;
                    }
                    else
                    {
                        Debug.LogError("Type not compareable: " + o.GetType());
                        return false;
                    }

                case Operator.IS_SMALLER_OR_EQUAL:
                    if (o is float)
                    {
                        return (float)o <= (float)this.value;
                    }
                    else if (o is int)
                    {
                        return (int)o <= (int)this.value;
                    }
                    else
                    {
                        Debug.LogError("Type not compareable: " + o.GetType());
                        return false;
                    }

                case Operator.IS_SMALLER:
                    if (o is float)
                    {
                        return (float)o < (float)this.value;
                    }
                    else if (o is int)
                    {
                        return (int)o < (int)this.value;
                    }
                    else
                    {
                        Debug.LogError("Type not compareable: " + o.GetType());
                        return false;
                    }

                default: return false;
            }
        }

        override public string ToString()
        {
            return Name + ":" + this.key;
        }
    }
}