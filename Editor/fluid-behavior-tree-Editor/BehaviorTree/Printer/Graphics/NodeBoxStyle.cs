using System.Linq;
using UnityEngine;

namespace CleverCrow.Fluid.BTs.Trees.Editors {
    public class NodeBoxStyle {
        public GUIStyle Style { get; }

        public NodeBoxStyle (Color32 border, Color background) {
            var texture = CreateTexture(19, 19, border);
            texture.SetPixels(1, 1, 17, 17,
                Enumerable.Repeat(background, 17 * 17).ToArray());
            texture.Apply();
            
            Style = new GUIStyle(GUI.skin.box) {
                border = new RectOffset(1, 1, 1, 1),
                normal = {
                    background = texture,
                },
            };
        }

        private static Texture2D CreateTexture (int width, int height, Color color) {
            var texture = new Texture2D(width, height, TextureFormat.ARGB32, false);
            texture.SetPixels(Enumerable.Repeat(color, width * height).ToArray());
            texture.Apply();
            
            return texture;
        }
    }
}