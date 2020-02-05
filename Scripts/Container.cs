using System.Collections.Generic;
using UnityEngine.Assertions;

namespace NPBehave
{
    public abstract class Container : Node
    {
        protected Node[] Children;
        private readonly List<Node> pausedChildren = new List<Node>();
        
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
            
            if (currentState != State.PAUSED)
            {
                this.DoChildStopped(child, succeeded);
            }
        }

        override public void Pause()
        {
            if (!IsActive)
                return;

            currentState = State.PAUSED;
            
            foreach (Node child in Children)
            {
                if (child.IsActive)
                {
                    child.Pause();
                    this.pausedChildren.Add(child);
                }
            }
        }

        override public void Resume()
        {
            if (currentState != State.PAUSED)
                return;

            currentState = State.ACTIVE;
            
            foreach (Node child in pausedChildren)
            {
                child.Resume();
            }
            this.pausedChildren.Clear();
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