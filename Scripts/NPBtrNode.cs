using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;

public abstract class NPBtrNode
{
    public enum State
    {
        INACTIVE,
        ACTIVE,
        STOP_REQUESTED,
    }

    protected State currentState = State.INACTIVE;

    public State CurrentState
    {
        get { return currentState; }
    }

    public NPBtrRoot RootNode;

    private NPBtrContainer parentNode;
    public NPBtrContainer ParentNode
    {
        get
        {
            return parentNode;
        }
    }

    private string name;

    public string Name
    {
        get
        {
            return name;
        }
    }

    public virtual NPBtrBlackboard Blackboard
    {
        get
        {
            return RootNode.Blackboard;
        }
    }

    public virtual NPBtrClock Clock
    {
        get
        {
            return RootNode.Clock;
        }
    }

    public bool IsStopRequested
    {
        get
        {
            return this.currentState == State.STOP_REQUESTED;
        }
    }

    public bool IsActive
    {
        get
        {
            return this.currentState == State.ACTIVE;
        }
    }


    public NPBtrNode(string name)
    {
        this.name = name;
    }

    public virtual void SetRoot(NPBtrRoot rootNode)
    {
        this.RootNode = rootNode;
    }

    public void SetParent(NPBtrContainer parent)
    {
        this.parentNode = parent;
    }

#if UNITY_EDITOR
    public int DebugNumStartCalls = 0;
    public int DebugNumStopCalls = 0;
    public int DebugNumStoppedCalls = 0;
    public bool DebugLastResult = false;
#endif

    public void Start()
    {
        Assert.AreEqual(this.currentState, State.INACTIVE, "can only start inactive nodes, tried to start: " + this.Name+"! PATH: " + GetPath());

#if UNITY_EDITOR
        RootNode.TotalNumStartCalls++;
        this.DebugNumStartCalls++;
#endif
        this.currentState = State.ACTIVE;
        DoStart();
    }

    public void Stop()
    {
        Assert.AreEqual(this.currentState, State.ACTIVE, "can only stop active nodes, tried to stop " + this.Name +"! PATH: " + GetPath());
        this.currentState = State.STOP_REQUESTED;
#if UNITY_EDITOR
        RootNode.TotalNumStopCalls++;
        this.DebugNumStopCalls++;
#endif
        DoStop();
    }

    protected virtual void DoStart()
    {

    }

    protected virtual void DoStop()
    {

    }


    /// THIS ABSOLUTLY HAS TO BE THE LAST CALL IN YOUR FUNCTION, NEVER MODIFY
    /// ANY STATE AFTER CALLING Stopped !!!!
    protected virtual void Stopped(bool success)
    {
        Assert.AreNotEqual(this.currentState, State.INACTIVE, "The Node " + this + " called 'Stopped' while in state INACTIVE, something is wrong! PATH: " + GetPath());
        this.currentState = State.INACTIVE;
#if UNITY_EDITOR
        RootNode.TotalNumStoppedCalls++;
        this.DebugNumStoppedCalls++;
        DebugLastResult = success;
#endif
        if (this.ParentNode != null)
        {
            this.ParentNode.ChildStopped(this, success);
        }
    }

    public virtual void ParentCompositeStopped(NPBtrComposite composite)
    {
        DoParentCompositeStopped(composite);
    }

    /// THIS IS CALLED WHILE YOU ARE INACTIVE, IT's MEANT FOR DECORATORS TO REMOVE ANY PENDING
    /// OBSERVERS
    protected virtual void DoParentCompositeStopped(NPBtrComposite composite)
    {
        /// be careful with this!
    }

    // public NPBtrComposite ParentComposite
    // {
    //     get
    //     {
    //         if (ParentNode != null && !(ParentNode is NPBtrComposite))
    //         {
    //             return ParentNode.ParentComposite;
    //         }
    //         else
    //         {
    //             return ParentNode as NPBtrComposite;
    //         }
    //     }
    // }

    override public string ToString()
    {
        return this.Name;
    }
    
    protected string GetPath()
    {
        if(ParentNode != null) 
        {
            return ParentNode.GetPath() + "/" + Name;
        } 
        else 
        {
            return Name;
        } 
    } 
}
