using UnityEngine.Assertions;

namespace NPBehave
{
    public abstract class Container : Node
    {
        private bool collapse = false;
        public bool Collapse
        {
            get
            {
                return collapse;
            }
            set
            {
                collapse = value;
            }
        }

        public Container(string name) : base(name)
        {
        }

        public void ChildStopped(Node child, bool succeeded)
        {
            // Assert.AreNotEqual(this.currentState, State.INACTIVE, "The Child " + child.Name + " of Container " + this.Name + " was stopped while the container was inactive. PATH: " + GetPath());
            Assert.AreNotEqual(this.currentState, State.INACTIVE, "A Child of a Container was stopped while the container was inactive.");
            this.DoChildStopped(child, succeeded);
        }

        protected abstract void DoChildStopped(Node child, bool succeeded);

#if UNITY_EDITOR
        public abstract Node[] DebugChildren
        {
            get;
        }
#endif
    }
}