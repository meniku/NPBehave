using UnityEngine;
using System.Collections;
using UnityEngine.Assertions;

public class NPBtrBlackboardValue : NPBtrDecorator
{
    private string key;
    private object value;
    private NPBtrOperator op;
    private NPBtrStops stopsOnChange;

    public NPBtrBlackboardValue(string key, NPBtrOperator op, object value, NPBtrStops stopsOnChange, NPBtrNode decoratee) : base("BlackboardValue", decoratee)
    {
        this.op = op;
        this.key = key;
        this.value = value;
        this.stopsOnChange = stopsOnChange;
    }
    public NPBtrBlackboardValue(string key, NPBtrOperator op, NPBtrStops stopsOnChange, NPBtrNode decoratee) : base("BlackboardValue", decoratee)
    {
        this.op = op;
        this.key = key;
        this.stopsOnChange = stopsOnChange;
    }

    protected override void DoStart()
    {
        if( stopsOnChange != NPBtrStops.NONE )
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

    protected override void DoChildStopped(NPBtrNode child, bool result)
    {
        Assert.AreNotEqual(this.CurrentState, State.INACTIVE);
        if (stopsOnChange == NPBtrStops.NONE || stopsOnChange == NPBtrStops.SELF)
        {
            this.RootNode.Blackboard.RemoveObserver(this.key, OnValueChanged);
        }
        Stopped(false);
    }

    override protected void DoParentCompositeStopped(NPBtrComposite parentComposite)
    {
        this.RootNode.Blackboard.RemoveObserver(this.key, OnValueChanged);
    }

    private void OnValueChanged(NPBtrBlackboard.Type type, object newValue)
    {
        if (IsActive && !IsConditionMet())
        {
            if (stopsOnChange == NPBtrStops.SELF || stopsOnChange == NPBtrStops.BOTH || stopsOnChange == NPBtrStops.IMMEDIATE_RESTART)
            {
                // Debug.Log( this.key + " stopped self ");
                this.Stop();
            }
        }
        else if (!IsActive && IsConditionMet())
        {
            if (stopsOnChange == NPBtrStops.LOWER_PRIORITY || stopsOnChange == NPBtrStops.BOTH || stopsOnChange == NPBtrStops.IMMEDIATE_RESTART || stopsOnChange == NPBtrStops.LOWER_PRIORITY_IMMEDIATE_RESTART)
            {
                // Debug.Log( this.key + " stopped other ");
                NPBtrContainer parentNode = this.ParentNode;
                NPBtrNode childNode = this;
                while(parentNode != null && !(parentNode is NPBtrComposite))
                {
                    childNode = parentNode;
                    parentNode = parentNode.ParentNode;
                }
                Assert.IsNotNull(parentNode, "NTBtrStops is only valid when attached to a parent composite");
                Assert.IsNotNull(childNode);
                if(parentNode is NPBtrParallel)
                {
                    Assert.IsTrue(stopsOnChange == NPBtrStops.IMMEDIATE_RESTART, "On Parallel Nodes all children have the same priority, thus NPBtrStops.LOWER_PRIORITY or NPBtrStops.BOTH are unsupported in this context!");
                }
                
                ((NPBtrComposite)parentNode).StopLowerPriorityChildrenForChild(childNode, stopsOnChange == NPBtrStops.IMMEDIATE_RESTART || stopsOnChange == NPBtrStops.LOWER_PRIORITY_IMMEDIATE_RESTART);
            }
        }
    }

    private bool IsConditionMet()
    {
        if (op == NPBtrOperator.ON_CHANGE)
        {
            return true;
        }

        if (!this.RootNode.Blackboard.Isset(key))
        {
            return op == NPBtrOperator.IS_NOT_SET;
        }

        object o = this.RootNode.Blackboard.Get(key);

        switch (this.op)
        {
            case NPBtrOperator.IS_SET: return true;
            case NPBtrOperator.IS_EQUAL: return object.Equals(o, value);
            case NPBtrOperator.IS_NOT_EQUAL: return !object.Equals(o, value);

            case NPBtrOperator.IS_GREATER_OR_EQUAL:
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

            case NPBtrOperator.IS_GREATER:
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

            case NPBtrOperator.IS_SMALLER_OR_EQUAL:
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

            case NPBtrOperator.IS_SMALLER:
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
        return Name + ":"+this.key;
    }
}