//------------------------------------------------------------
// Author: 烟雨迷离半世殇
// Mail: 1778139321@qq.com
// Data: 2019年8月21日 8:18:19
//------------------------------------------------------------

using System.Collections.Generic;
using ETModel;
using NodeEditorFramework;
using Plugins.NodeEditor.Editor.Canvas;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using Node = NPBehave.Node;

namespace Plugins.NodeEditor.Editor.NPBehaveNodes
{
    [Node(false, "NPBehave行为树/Task/CommonAction", typeof (NPBehaveCanvas))]
    public class NP_ActionNode: NP_NodeBase
    {
        /// <summary>
        /// 内部ID
        /// </summary>
        private const string Id = "行为节点";

        /// <summary>
        /// 内部ID
        /// </summary>
        public override string GetID => Id;

        [LabelText("行为结点数据")]
        public NP_ActionNodeData NP_ActionNodeData;

        private void OnEnable()
        {
            if (NP_ActionNodeData == null)
            {
                this.NP_ActionNodeData = new NP_ActionNodeData{NodeType = NodeType.Task};
            }
        }

        public override NP_NodeDataBase NP_GetNodeData()
        {
            return NP_ActionNodeData;
        }

        public override void NodeGUI()
        {
            EditorGUILayout.TextField(NP_ActionNodeData.NodeDes);
        }
    }
}