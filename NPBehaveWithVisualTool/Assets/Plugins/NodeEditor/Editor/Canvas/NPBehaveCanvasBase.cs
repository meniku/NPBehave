//------------------------------------------------------------
// Author: 烟雨迷离半世殇
// Mail: 1778139321@qq.com
// Data: 2020年1月9日 20:02:37
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using ETModel;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using NodeEditorFramework;
using Plugins.NodeEditor.Editor.NPBehaveNodes;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Plugins.NodeEditor.Editor.Canvas
{
    /// <summary>
    /// 行为树基类，继承此类即可自行拓展基于行为树的逻辑图
    /// 必须实现的有以下几点
    /// 1.继承NP_DataSupportorBase的数据基类
    /// 2.自动配置所有结点数据：AddAllNodeData
    /// 3.保存行为树信息为二进制文件：Save
    /// 4.自定义的的额外数据块配置
    /// 需要注意的点
    /// 要在NPBehaveNodes文件夹下面的除了NP_NodeBase之外的所有Node的Node特性的type里加上自定义的Canvas的Type，不然创建不了行为树组件
    /// 推荐的按钮样式：[Button("XXX", 25), GUIColor(0.4f, 0.8f, 1)]
    /// </summary>
    public abstract class NPBehaveCanvasBase: NodeCanvas
    {
        public override string canvasName => Name;
        
        [Title("本Canvas所有数据整理部分")]
        [LabelText("保存文件名"), GUIColor(0.9f, 0.7f, 1)]
        public string Name = "";

        [LabelText("保存路径"), GUIColor(0.1f, 0.7f, 1)]
        [FolderPath]
        public string SavePath;
        
        /// <summary>
        /// 自动配置当前图所有结点
        /// </summary>
        /// <param name="npDataSupportorBase">自定义的继承于NP_DataSupportorBase的数据体</param>
        public virtual void AddAllNodeData(NP_DataSupportorBase npDataSupportorBase)
        {
            this.AutoSetNP_NodeData(npDataSupportorBase);
        }

        /// <summary>
        /// 保存当前所有结点信息为二进制文件
        /// </summary>
        /// <param name="npDataSupportorBase">自定义的继承于NP_DataSupportorBase的数据体</param>
        public virtual void Save(NP_DataSupportorBase npDataSupportorBase)
        {
            using (FileStream file = File.Create($"{SavePath}/{this.Name}.bytes"))
            {
                BsonSerializer.Serialize(new BsonBinaryWriter(file), npDataSupportorBase);
            }

            Debug.Log("保存成功");
        }

        /// <summary>
        /// 测试反序列化
        /// </summary>
        /// <param name="npDataSupportorBase">自定义的继承于NP_DataSupportorBase的数据体</param>
        public virtual T TestDe<T>(T npDataSupportorBase) where T:NP_DataSupportorBase
        {
            byte[] mfile = File.ReadAllBytes($"{SavePath}/{this.Name}.bytes");

            if (mfile.Length == 0) Debug.Log("没有读取到文件");

            try
            {
                npDataSupportorBase = BsonSerializer.Deserialize<T>(mfile);
                return npDataSupportorBase;
            }
            catch (Exception e)
            {
                Debug.Log(e);
                throw;
            }

        }

        /// <summary>
        /// 自动配置所有行为树结点
        /// </summary>
        /// <param name="npDataSupportorBase">自定义的继承于NP_DataSupportorBase的数据体</param>
        private void AutoSetNP_NodeData(NP_DataSupportorBase npDataSupportorBase)
        {
            npDataSupportorBase.mNP_DataSupportorDic.Clear();

            List<NP_NodeBase> tempNode1 = new List<NP_NodeBase>();

            foreach (var VARIABLE in this.nodes)
            {
                if (VARIABLE is NP_NodeBase mNode)
                {
                    tempNode1.Add(mNode);
                }
            }

            tempNode1.Sort((x, y) => -x.position.y.CompareTo(y.position.y));

            foreach (var VARIABLE in tempNode1)
            {
                VARIABLE.NP_GetNodeData().id = IdGenerater.GenerateId();
            }

            npDataSupportorBase.RootId = tempNode1[tempNode1.Count - 1].NP_GetNodeData().id;

            foreach (var VARIABLE1 in tempNode1)
            {
                NP_NodeDataBase mNodeData = VARIABLE1.NP_GetNodeData();
                mNodeData.linkedID.Clear();
                long mNodeDataID = mNodeData.id;
                List<NP_NodeBase> tempNode = new List<NP_NodeBase>();
                foreach (var VARIABLE2 in VARIABLE1.NextNode.connections)
                {
                    tempNode.Add((NP_NodeBase) VARIABLE2.body);
                }

                tempNode.Sort((x, y) => x.position.x.CompareTo(y.position.x));

                foreach (var np_NodeBase in tempNode)
                {
                    mNodeData.linkedID.Add(np_NodeBase.NP_GetNodeData().id);
                }

                //Log.Info($"y:{VARIABLE1.position.y},x:{VARIABLE1.position.x},id:{mNodeDataID}");
                npDataSupportorBase.mNP_DataSupportorDic.Add(mNodeDataID, mNodeData);
            }
        }
        
    }
}