//------------------------------------------------------------
// Author: 烟雨迷离半世殇
// Mail: 1778139321@qq.com
// Data: 2019年8月21日 8:31:21
//------------------------------------------------------------

using ETModel;

namespace NodeEditorFramework
{
    public abstract partial class Node
    {
        /// <summary>
        /// 获取结点数据
        /// </summary>
        /// <returns></returns>
        public virtual NP_NodeDataBase NP_GetNodeData()
        {
            return null;
        }
    }
}