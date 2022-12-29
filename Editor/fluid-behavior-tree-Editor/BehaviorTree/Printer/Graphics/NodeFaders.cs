using UnityEngine;

namespace CleverCrow.Fluid.BTs.Trees.Editors {
    public class NodeFaders {
        public ColorFader BackgroundFader { get; } = new ColorFader(
            new Color(0.65f, 0.65f, 0.65f), new Color(0.39f, 0.78f, 0.39f));
        
        public ColorFader TextFader { get; } = new ColorFader(
            Color.white, Color.black);
        
        public ColorFader MainIconFader { get; } = new ColorFader(
            new Color(1, 1, 1, 0.3f), new Color(1, 1, 1, 1f));

        public void Update (bool active) {
            BackgroundFader.Update(active);
            TextFader.Update(active);
            MainIconFader.Update(active);
        }
    }
}