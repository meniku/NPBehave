//------------------------------------------------------------
// Author: 烟雨迷离半世殇
// Mail: 1778139321@qq.com
// Data: 2019年8月23日 17:54:50
//------------------------------------------------------------

using ETModel;
using NodeEditorFramework;
using Plugins.NodeEditor.Editor.Canvas;
using Sirenix.OdinInspector;
using UnityEditor;

namespace Plugins.NodeEditor.Editor.NPBehaveNodes
{
    [Node(false, "NPBehave行为树/Decorator/BlackboardCondition", typeof (NPBehaveCanvas))]
    public class NP_BlackboardConditionNode: NP_NodeBase
    {
        /// <summary>
        /// 内部ID
        /// </summary>
        private const string Id = "黑板条件结点";

        /// <summary>
        /// 内部ID
        /// </summary>
        public override string GetID => Id;

        [LabelText("黑板条件结点数据")]
        public NP_BlackboardConditionNodeData NP_BlackboardConditionNodeData;

        private void OnEnable()
        {
            if (NP_BlackboardConditionNodeData == null)
            {
                this.NP_BlackboardConditionNodeData = new NP_BlackboardConditionNodeData { NodeType = NodeType.Decorator};
            }
        }

        public override NP_NodeDataBase NP_GetNodeData()
        {
            return NP_BlackboardConditionNodeData;
        }

        public override void NodeGUI()
        {
            EditorGUILayout.TextField(NP_BlackboardConditionNodeData.NodeDes);
        }
    }
}