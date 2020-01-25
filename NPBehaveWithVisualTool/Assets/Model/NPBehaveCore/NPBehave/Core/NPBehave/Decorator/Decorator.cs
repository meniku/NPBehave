namespace NPBehave
{

    public abstract class Decorator : Container
    {
        public Node Decoratee;

        public void SetDecoratorData(Node decoratee)
        {
            this.Decoratee = decoratee;
            this.Decoratee.SetParent(this);
        }
        
        public Decorator(string name, Node decoratee) : base(name)
        {
            this.Decoratee = decoratee;
            this.Decoratee.SetParent(this);
        }

        override public void SetRoot(Root rootNode)
        {
            base.SetRoot(rootNode);
            Decoratee.SetRoot(rootNode);
        }

        public override void ParentCompositeStopped(Composite composite)
        {
            base.ParentCompositeStopped(composite);
            Decoratee.ParentCompositeStopped(composite);
        }
    }
}