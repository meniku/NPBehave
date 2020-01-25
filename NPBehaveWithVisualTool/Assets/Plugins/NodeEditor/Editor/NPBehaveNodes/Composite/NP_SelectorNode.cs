//------------------------------------------------------------
// Author: 烟雨迷离半世殇
// Mail: 1778139321@qq.com
// Data: 2019年8月23日 17:35:02
//------------------------------------------------------------

using ETModel;
using NodeEditorFramework;
using Plugins.NodeEditor.Editor.Canvas;
using Sirenix.OdinInspector;
using UnityEditor;

namespace Plugins.NodeEditor.Editor.NPBehaveNodes
{
    [Node(false, "NPBehave行为树/Composite/Selector", typeof (NPBehaveCanvas))]
    public class NP_SelectorNode: NP_NodeBase
    {
        /// <summary>
        /// 内部ID
        /// </summary>
        private const string Id = "选择结点";

        /// <summary>
        /// 内部ID
        /// </summary>
        public override string GetID => Id;

        [LabelText("Selector结点数据")]
        public NP_SelectorNodeData NP_SelectorNodeData;

        private void OnEnable()
        {
            if (NP_SelectorNodeData == null)
            {
                this.NP_SelectorNodeData = new NP_SelectorNodeData { NodeType = NodeType.Composite};
            }
        }

        public override NP_NodeDataBase NP_GetNodeData()
        {
            return NP_SelectorNodeData;
        }

        public override void NodeGUI()
        {
            EditorGUILayout.TextField(NP_SelectorNodeData.NodeDes);
        }
    }
}