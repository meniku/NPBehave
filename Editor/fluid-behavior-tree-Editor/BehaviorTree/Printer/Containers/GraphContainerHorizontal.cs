using System.Collections.Generic;

namespace CleverCrow.Fluid.BTs.Trees.Editors {
    public class GraphContainerHorizontal : IGraphContainer {
        protected readonly List<IGraphBox> _childContainers = new List<IGraphBox>();
        
        public float LocalPositionX { get; private set; }
        public float LocalPositionY { get; private set; }
        public float GlobalPositionX { get; private set; }
        public float GlobalPositionY { get; private set; }
        public float Width { get; protected set; }
        public float Height { get; protected set; }
        public float PaddingX { get; }
        public float PaddingY { get; }
        public List<IGraphBox> ChildContainers => _childContainers;

        public void SetLocalPosition (float x, float y) {
            LocalPositionX = x;
            LocalPositionY = y;
        }

        public virtual void AddBox (IGraphBox child) {
            CalculateChild(child);
            _childContainers.Add(child);
        }

        private void CalculateChild (IGraphBox child) {
            child.SetLocalPosition(Width, 0);
            child.AddGlobalPosition(GlobalPositionX + child.LocalPositionX, GlobalPositionY + child.LocalPositionY);

            Width += child.Width;
            if (child.Height > Height) Height = child.Height;
        }

        public void SetSize (float width, float height) {
            throw new System.NotImplementedException();
        }

        public void SetGlobalPosition (float x, float y) {
            GlobalPositionX = x;
            GlobalPositionY = y;
        }

        public void AddGlobalPosition (float x, float y) {
            GlobalPositionX += x;
            GlobalPositionY += y;
            
            foreach (var child in ChildContainers) {
                child.AddGlobalPosition(x, y);
            }
        }

        public void SetPadding (float x, float y) {
            throw new System.NotImplementedException();
        }

        public override string ToString () {
            return
                $"Size: {Width}, {Height}; Local: {LocalPositionX}, {LocalPositionY}; Global: {GlobalPositionX}, {GlobalPositionY};";
        }

        public virtual void CenterAlignChildren () {
            foreach (var child in _childContainers) {
                child.CenterAlignChildren();
            }
        }

        public bool SkipCentering { get; }
    }
}