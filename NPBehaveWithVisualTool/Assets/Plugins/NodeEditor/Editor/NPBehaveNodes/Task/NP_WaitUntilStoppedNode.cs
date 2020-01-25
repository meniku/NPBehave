//------------------------------------------------------------
// Author: 烟雨迷离半世殇
// Mail: 1778139321@qq.com
// Data: 2019年8月26日 18:12:50
//------------------------------------------------------------

using ETModel;
using NodeEditorFramework;
using Plugins.NodeEditor.Editor.Canvas;
using Sirenix.OdinInspector;
using UnityEditor;

namespace Plugins.NodeEditor.Editor.NPBehaveNodes
{
    [Node(false, "NPBehave行为树/Task/WaitUntilStopped", typeof (NPBehaveCanvas))]
    public class NP_WaitUntilStoppedNode:NP_NodeBase
    {
        /// <summary>
        /// 内部ID
        /// </summary>
        private const string Id = "一直等待，直到Stopped";

        /// <summary>
        /// 内部ID
        /// </summary>
        public override string GetID => Id;

        [LabelText("结点数据")]
        public NP_WaitUntilStoppedData NpWaitUntilStoppedData;

        private void OnEnable()
        {
            if (NpWaitUntilStoppedData == null)
            {
                this.NpWaitUntilStoppedData = new NP_WaitUntilStoppedData{NodeType = NodeType.Task};
            }
        }

        public override NP_NodeDataBase NP_GetNodeData()
        {
            return NpWaitUntilStoppedData;
        }

        public override void NodeGUI()
        {
            EditorGUILayout.TextField(NpWaitUntilStoppedData.NodeDes);
        }
    }
}