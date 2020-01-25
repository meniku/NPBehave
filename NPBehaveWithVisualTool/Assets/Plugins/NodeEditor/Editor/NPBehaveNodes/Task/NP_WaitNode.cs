//------------------------------------------------------------
// Author: 烟雨迷离半世殇
// Mail: 1778139321@qq.com
// Data: 2019年9月13日 20:28:02
//------------------------------------------------------------

using ETModel;
using NodeEditorFramework;
using Plugins.NodeEditor.Editor.Canvas;
using Sirenix.OdinInspector;
using UnityEditor;

namespace Plugins.NodeEditor.Editor.NPBehaveNodes
{
    [Node(false, "NPBehave行为树/Task/Wait", typeof (NPBehaveCanvas))]
    public class NP_WaitNode: NP_NodeBase
    {
        /// <summary>
        /// 内部ID
        /// </summary>
        private const string Id = "等待节点";

        /// <summary>
        /// 内部ID
        /// </summary>
        public override string GetID => Id;

        [LabelText("等待结点数据")]
        public NP_WaitNodeData NP_WaitNodeData;

        private void OnEnable()
        {
            if (NP_WaitNodeData == null)
            {
                this.NP_WaitNodeData = new NP_WaitNodeData { NodeType = NodeType.Task };
            }
        }

        public override NP_NodeDataBase NP_GetNodeData()
        {
            return NP_WaitNodeData;
        }

        public override void NodeGUI()
        {
            EditorGUILayout.TextField(NP_WaitNodeData.NodeDes);
        }
    }
}