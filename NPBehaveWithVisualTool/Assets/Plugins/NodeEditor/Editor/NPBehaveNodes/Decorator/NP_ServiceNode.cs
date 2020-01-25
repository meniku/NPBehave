//------------------------------------------------------------
// Author: 烟雨迷离半世殇
// Mail: 1778139321@qq.com
// Data: 2019年8月22日 20:30:01
//------------------------------------------------------------

using ETModel;
using NodeEditorFramework;
using Plugins.NodeEditor.Editor.Canvas;
using Sirenix.OdinInspector;
using UnityEditor;

namespace Plugins.NodeEditor.Editor.NPBehaveNodes
{
    [Node(false, "NPBehave行为树/Decorator/Service", typeof (NPBehaveCanvas))]
    public class NP_ServiceNode: NP_NodeBase
    {
        /// <summary>
        /// 内部ID
        /// </summary>
        private const string Id = "服务结点";

        /// <summary>
        /// 内部ID
        /// </summary>
        public override string GetID => Id;

        [LabelText("服务结点数据")]
        public NP_ServiceNodeData NP_ServiceNodeData;

        private void OnEnable()
        {
            if (NP_ServiceNodeData == null)
            {
                this.NP_ServiceNodeData = new NP_ServiceNodeData { NodeType = NodeType.Decorator};
            }

        }

        public override NP_NodeDataBase NP_GetNodeData()
        {
            return NP_ServiceNodeData;
        }

        public override void NodeGUI()
        {
            EditorGUILayout.TextField(NP_ServiceNodeData.NodeDes);
        }
    }
}