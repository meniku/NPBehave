//------------------------------------------------------------
// Author: 烟雨迷离半世殇
// Mail: 1778139321@qq.com
// Data: 2019年8月20日 7:55:05
//------------------------------------------------------------

using System.Collections.Generic;
using System.IO;
using ETModel;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using NodeEditorFramework;
using NPBehave;
using Plugins.NodeEditor.Editor.NPBehaveNodes;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Networking.Types;
using Node = NPBehave.Node;

namespace Plugins.NodeEditor.Editor.Canvas
{
    [NodeCanvasType("NP行为树Canvas")]
    public class NPBehaveCanvas: NPBehaveCanvasBase
    {
        [LabelText("此行为树数据载体")]
        public NP_DataSupportorBase MNpDataSupportor = new NP_DataSupportorBase();

        [LabelText("反序列化测试")]
        public NP_DataSupportorBase MNpDataSupportor1 = new NP_DataSupportorBase();

        [Button("自动配置所有结点数据", 25), GUIColor(0.4f, 0.8f, 1)]
        public void AddAllNodeData()
        {
            base.AddAllNodeData(MNpDataSupportor);
        }

        [Button("保存行为树信息为二进制文件", 25), GUIColor(0.4f, 0.8f, 1)]
        public void Save()
        {
            base.Save(MNpDataSupportor);
        }

        [Button("测试反序列化", 25), GUIColor(0.4f, 0.8f, 1)]
        public void TestDe()
        {
            MNpDataSupportor1 = base.TestDe(MNpDataSupportor1);
        }
    }
}