using NPBehave;
using UnityEngine;

namespace CleverCrow.Fluid.BTs.Trees.Editors {
    public class BehaviorTreePrinter {
        private const float SCROLL_PADDING = 40;

        private readonly VisualTask _root;
        private readonly Rect _containerSize;

        private Vector2 _scrollPosition;
        
        public static StatusIcons StatusIcons { get; private set; }
        public static GuiStyleCollection SharedStyles { get; private set; }


        public BehaviorTreePrinter (Root tree, Vector2 windowSize) {
            StatusIcons = new StatusIcons();
            SharedStyles = new GuiStyleCollection();
            
            var container = new GraphContainerVertical();
            container.SetGlobalPosition(SCROLL_PADDING, SCROLL_PADDING);
            _root = new VisualTask(tree, container);
            container.CenterAlignChildren();
            
            _containerSize = new Rect(0, 0, 
                container.Width + SCROLL_PADDING * 2, 
                container.Height + SCROLL_PADDING * 2);

            CenterScrollView(windowSize, container);
        }

        private void CenterScrollView (Vector2 windowSize, GraphContainerVertical container) {
            var scrollOverflow = container.Width + SCROLL_PADDING * 2 - windowSize.x;
            var centerViewPosition = scrollOverflow / 2;
            _scrollPosition.x = centerViewPosition;
        }

        public void Print (Vector2 windowSize) {
            _scrollPosition = GUI.BeginScrollView(
                new Rect(0, 0, windowSize.x, windowSize.y), 
                _scrollPosition, 
                _containerSize);
            _root.Print();
            GUI.EndScrollView();
        }

        public void Unbind () {
            _root.RecursiveTaskUnbind();
        }
    }
}