using System.Collections.Generic;
using NPBehave;
using UnityEngine.Events;

namespace CleverCrow.Fluid.BTs.Trees.Editors {
    public class VisualTask {
        private readonly List<VisualTask> _children = new List<VisualTask>();
        private readonly NodePrintController _printer;
        private bool _taskActive;

        public Node Task { get; }
        private UnityEvent _event;
        public IReadOnlyList<VisualTask> Children => _children;
        
        public float Width { get; } = 70;
        public float Height { get; private set; } = 50;
        
        public IGraphBox Box { get; private set; }
        public IGraphBox Divider { get; private set; }
        public float DividerLeftOffset { get; private set; }

        public VisualTask (Node task, IGraphContainer parentContainer) {
            Task = task;
            _event = new UnityEvent();
            Task.OnStart += _event.Invoke;
            _taskActive = task.IsActive;
            BindTask();
            
            var container = new GraphContainerVertical();

            AddBox(container);

            if (task is Container conta) {
                var childContainer = new GraphContainerHorizontal();
                foreach (var child in conta.DebugChildren) {
                    _children.Add(new VisualTask(child, childContainer));
                }
                
                AddDivider(container, childContainer);
                container.AddBox(childContainer);
            }

            parentContainer.AddBox(container);
            
            _printer = new NodePrintController(this);
        }

        private void BindTask () {
            _event.AddListener(UpdateTaskActiveStatus);
        }

        public void RecursiveTaskUnbind () {
            _event.RemoveListener(UpdateTaskActiveStatus);
            
            foreach (var child in _children) {
                child.RecursiveTaskUnbind();
            }
        }

        private void UpdateTaskActiveStatus () {
            _taskActive = true;
        }

        private void AddDivider (IGraphContainer parent, IGraphContainer children) {
            Divider = new GraphBox {
                SkipCentering = true,
            };

            DividerLeftOffset = children.ChildContainers[0].Width / 2;
            var dividerRightOffset = children.ChildContainers[children.ChildContainers.Count - 1].Width / 2;
            var width = children.Width - DividerLeftOffset - dividerRightOffset;

            Divider.SetSize(width, 1);

            parent.AddBox(Divider);
        }

        private void AddBox (IGraphContainer parent) {
            Box = new GraphBox();
            if (!string.IsNullOrWhiteSpace(Task.Label))
            {
                Height += 20;
            }
            Box.SetSize(Width, Height);
            Box.SetPadding(10, 10);
            parent.AddBox(Box);
        }

        public void Print () {
            _printer.Print(Task.IsActive | _taskActive);
            _taskActive = false;

            foreach (var child in _children) {
                child.Print();
            }
        }
    }
}
