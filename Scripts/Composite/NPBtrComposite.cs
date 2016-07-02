using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;

public abstract class NPBtrComposite : NPBtrContainer 
{
    protected NPBtrNode[] Children;
    
    public NPBtrComposite(string name, NPBtrNode[] children) : base(name)
    {
        this.Children = children;
        Assert.IsTrue(children.Length > 0);
        
        foreach(NPBtrNode node in Children) 
        {
            node.SetParent( this );
        }
    }
    
    override public void SetRoot(NPBtrRoot rootNode)
    {
        base.SetRoot(rootNode);
        
        foreach(NPBtrNode node in Children) 
        {
            node.SetRoot(rootNode);
        }
    }
    
       
    #if UNITY_EDITOR
    public override NPBtrNode[] DebugChildren 
    {
        get {
            return this.Children;
        }
    }
    #endif
    
    protected override void Stopped(bool success)
    {
        foreach(NPBtrNode child in Children)
        {
            child.ParentCompositeStopped(this);
        }
        base.Stopped(success);   
    }
    
    public abstract void StopLowerPriorityChildrenForChild(NPBtrNode child, bool immediateRestart);
}
