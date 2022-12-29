using System.Collections.Generic;

namespace CleverCrow.Fluid.BTs.Trees.Editors {
    public interface IGraphBox {
        List<IGraphBox> ChildContainers { get; }

        float LocalPositionX { get; }
        float LocalPositionY { get; }
        float GlobalPositionX { get; }
        float GlobalPositionY { get; }

        float Width { get; }
        float Height { get; }
        float PaddingX { get; }
        float PaddingY { get; }

        void SetSize (float width, float height);
        void SetLocalPosition (float x, float y);
        void AddGlobalPosition (float x, float y);
        void SetPadding (float x, float y);
        void CenterAlignChildren ();

        bool SkipCentering { get; }
    }
}