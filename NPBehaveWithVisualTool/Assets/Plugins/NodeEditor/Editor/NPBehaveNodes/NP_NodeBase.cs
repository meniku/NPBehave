//------------------------------------------------------------
// Author: 烟雨迷离半世殇
// Mail: 1778139321@qq.com
// Data: 2019年8月21日 13:11:41
//------------------------------------------------------------

using System.Collections.Generic;
using NodeEditorFramework;
using Plugins.NodeEditor.Editor.Canvas;
using UnityEditor;
using UnityEngine;

namespace Plugins.NodeEditor.Editor.NPBehaveNodes
{
    [Node(false, "NPBehave行为树结点", typeof (NeverbeUsedCanvas))]
    public abstract class NP_NodeBase: Node
    {
        /// <summary>
        /// 内部ID
        /// </summary>
        private const string Id = "行为树节点";

        /// <summary>
        /// 内部ID
        /// </summary>
        public override string GetID => Id;

        public override Vector2 DefaultSize => new Vector2(150, 60);

        [ValueConnectionKnob("NPBehave_PreNode", Direction.In, "NPBehave_PrevNodeDatas", NodeSide.Top, 75)]
        public ValueConnectionKnob PrevNode;

        [ValueConnectionKnob("NPBehave_NextNode", Direction.Out, "NPBehave_NextNodeDatas", NodeSide.Bottom, 75)]
        public ValueConnectionKnob NextNode;
        public virtual void AutoBindAllDelegate()
        {
        }
        
        public override void NodeGUI()
        {
            EditorGUILayout.TextField("不允许使用此结点");
        }
    }
}