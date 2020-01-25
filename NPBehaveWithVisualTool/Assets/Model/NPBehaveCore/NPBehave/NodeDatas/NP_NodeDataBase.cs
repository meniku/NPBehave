//------------------------------------------------------------
// Author: 烟雨迷离半世殇
// Mail: 1778139321@qq.com
// Data: 2019年8月21日 7:14:44
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using NPBehave;
using Sirenix.OdinInspector;

namespace ETModel
{
    public abstract class NP_NodeDataBase
    {
        /// <summary>
        /// 此结点ID
        /// </summary>
        [LabelText("此结点ID")]
        [HideInEditorMode]
        public long id;

        [LabelText("此结点类型")]
        [HideInEditorMode]
        public NodeType NodeType;

        /// <summary>
        /// 与此结点相连的ID
        /// </summary>
        [HideInEditorMode]
        public List<long> linkedID = new List<long>();

        [LabelText("结点信息描述")]
        public string NodeDes;

        /// <summary>
        /// 获取结点
        /// </summary>
        /// <returns></returns>
        public abstract Node NP_GetNode();

        
        /// <summary>
        /// 创建组合结点
        /// </summary>
        /// <returns></returns>
        public virtual Composite CreateComposite(Node[] nodes)
        {
            return null;
        }

        /// <summary>
        /// 创建装饰结点
        /// </summary>
        /// <param name="UnitId">行为树归属的UnitID</param>
        /// <param name="RuntimeTreeID">运行时归属的行为树ID</param>
        /// <param name="node">所装饰的结点</param>
        /// <returns></returns>
        public virtual Decorator CreateDecoratorNode(long UnitId,long RuntimeTreeID,Node node)
        {
            return null;
        }

        /// <summary>
        /// 创建任务节点
        /// </summary>
        /// <param name="UnitId">行为树归属的UnitID</param>
        /// <param name="RuntimeTreeID">运行时归属的行为树ID</param>
        /// <returns></returns>
        public virtual Task CreateTask(long UnitId,long RuntimeTreeID)
        {
            return null;
        }
    }
}