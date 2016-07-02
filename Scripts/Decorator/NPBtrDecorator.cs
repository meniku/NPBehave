using UnityEngine;
using System.Collections;

public abstract class NPBtrDecorator : NPBtrContainer
{
    protected NPBtrNode Decoratee;

    public NPBtrDecorator(string name, NPBtrNode decoratee) : base(name)
    {
        this.Decoratee = decoratee;
        this.Decoratee.SetParent(this);
    }

    override public void SetRoot(NPBtrRoot rootNode)
    {
        base.SetRoot(rootNode);
        Decoratee.SetRoot(rootNode);
    }


#if UNITY_EDITOR
    public override NPBtrNode[] DebugChildren
    {
        get
        {
            return new NPBtrNode[] { Decoratee };
        }
    }
#endif

    public override void ParentCompositeStopped(NPBtrComposite composite)
    {
        base.ParentCompositeStopped(composite);
        Decoratee.ParentCompositeStopped(composite);
    }
}
