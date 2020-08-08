using NPBehave;
using UnityEditor;
using UnityEngine;

namespace CleverCrow.Fluid.BTs.Trees.Editors {
    public class BehaviorTreeWindow : EditorWindow {
        private BehaviorTreePrinter _printer;
        private string _name;

        public static void ShowTree (Root tree,string label) {
            var window = GetWindow<BehaviorTreeWindow>(false);
            window.titleContent = new GUIContent($"Behavior Tree: {label}");
            window.SetTree(tree, label);
        }

        private void SetTree(Root tree, string label) {
            _printer?.Unbind();
            _printer = new BehaviorTreePrinter(tree, position.size);
            _name = label;
        }

        private void OnGUI () {
            if (!Application.isPlaying) {
                ClearView();
            }
            
            GUILayout.Label($"Behavior Tree: {_name}", EditorStyles.boldLabel);
            _printer?.Print(position.size);
        }

        private void ClearView () {
            _name = null;
            _printer = null;
        }

        private void Update () {
            if (Application.isPlaying) {
                Repaint();
            }
        }
    }
}
