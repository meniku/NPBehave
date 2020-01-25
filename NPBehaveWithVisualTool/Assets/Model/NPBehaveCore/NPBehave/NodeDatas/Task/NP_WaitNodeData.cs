//------------------------------------------------------------
// Author: 烟雨迷离半世殇
// Mail: 1778139321@qq.com
// Data: 2019年8月21日 7:13:45
//------------------------------------------------------------

using System.Collections.Generic;
using NPBehave;
using Sirenix.OdinInspector;

namespace ETModel
{
    public class NP_WaitNodeData: NP_NodeDataBase
    {
        [HideInEditorMode]
        public Wait mWaitNode;

        [LabelText("等待时间")]
        public float waitTime;

        public override Task CreateTask(long UnitId, long RuntimeTreeID)
        {
            mWaitNode = new Wait(this.waitTime);
            return mWaitNode;
        }

        public override Node NP_GetNode()
        {
            return this.mWaitNode;
        }
    }
}