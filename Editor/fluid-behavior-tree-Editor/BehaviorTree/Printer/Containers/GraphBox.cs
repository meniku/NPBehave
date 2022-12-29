using System.Collections.Generic;

namespace CleverCrow.Fluid.BTs.Trees.Editors {
    public class GraphBox : IGraphBox {
        public List<IGraphBox> ChildContainers { get; } = new List<IGraphBox>();
        public bool SkipCentering { get; set; }

        public float LocalPositionX { get; private set; }
        public float LocalPositionY { get; private set; }
        
        public float GlobalPositionX { get; private set; }
        public float GlobalPositionY { get; private set; }
        
        public float Width { get; private set; }
        public float Height { get; private set; }
        
        public float PaddingX { get; private set; }
        public float PaddingY { get; private set; }

        public void SetSize (float width, float height) {
            Width = width;
            Height = height;
        }

        public void SetPadding (float x, float y) {
            Width += x;
            Height += y;

            PaddingX = x;
            PaddingY = y;
        }

        public void CenterAlignChildren () {
        }
        
        public void SetLocalPosition (float x, float y) {
            LocalPositionX = x;
            LocalPositionY = y;
        }

        public void AddGlobalPosition (float x, float y) {
            GlobalPositionX += x;
            GlobalPositionY += y;
        }
    }
}