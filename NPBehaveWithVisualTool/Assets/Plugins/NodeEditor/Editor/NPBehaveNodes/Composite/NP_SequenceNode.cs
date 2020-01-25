//------------------------------------------------------------
// Author: 烟雨迷离半世殇
// Mail: 1778139321@qq.com
// Data: 2019年8月23日 17:30:16
//------------------------------------------------------------

using ETModel;
using NodeEditorFramework;
using Plugins.NodeEditor.Editor.Canvas;
using Sirenix.OdinInspector;
using UnityEditor;

namespace Plugins.NodeEditor.Editor.NPBehaveNodes
{
    [Node(false, "NPBehave行为树/Composite/Sequence", typeof (NPBehaveCanvas))]
    public class NP_SequenceNode: NP_NodeBase
    {
        /// <summary>
        /// 内部ID
        /// </summary>
        private const string Id = "队列结点";

        /// <summary>
        /// 内部ID
        /// </summary>
        public override string GetID => Id;

        [LabelText("队列结点数据")]
        public NP_SequenceNodeData NP_SequenceNodeData;

        private void OnEnable()
        {
            if (NP_SequenceNodeData == null)
            {
                this.NP_SequenceNodeData = new NP_SequenceNodeData { NodeType = NodeType.Composite };
            }
        }

        public override NP_NodeDataBase NP_GetNodeData()
        {
            return NP_SequenceNodeData;
        }

        public override void NodeGUI()
        {
            EditorGUILayout.TextField(NP_SequenceNodeData.NodeDes);
        }
    }
}