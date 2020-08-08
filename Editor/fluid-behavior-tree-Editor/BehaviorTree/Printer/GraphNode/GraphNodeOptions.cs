using UnityEngine;

namespace CleverCrow.Fluid.BTs.Trees.Editors {
    public class GraphNodeOptions {
        public int VerticalConnectorBottomHeight { get; set; }
        public int HorizontalConnectorHeight { get; set; }
        public int VerticalConnectorTopHeight { get; set; }
        public Vector2 Size { get; set; } = new Vector2(50, 100);
    }
}