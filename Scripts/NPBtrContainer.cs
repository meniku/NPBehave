using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;

public abstract class NPBtrContainer : NPBtrNode
{
    public NPBtrContainer(string name) : base(name)
    {
    }

    public void ChildStopped(NPBtrNode child, bool succeeded)
    {
        Assert.AreNotEqual(this.currentState, State.INACTIVE, "The Child " + child.Name + " of Container " + this.Name + " was stopped while the container was inactive. PATH: " + GetPath());
        this.DoChildStopped(child, succeeded);
    }

    protected abstract void DoChildStopped(NPBtrNode child, bool succeeded);

#if UNITY_EDITOR
    public abstract NPBtrNode[] DebugChildren
    {
        get;
    }
#endif
}
