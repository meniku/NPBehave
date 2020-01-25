//------------------------------------------------------------
// Author: 烟雨迷离半世殇
// Mail: 1778139321@qq.com
// Data: 2019年8月26日 18:08:42
//------------------------------------------------------------

using NPBehave;
using Sirenix.OdinInspector;

namespace ETModel
{
    public class NP_WaitUntilStoppedData: NP_NodeDataBase
    {
        [HideInEditorMode]
        public WaitUntilStopped MWaitUntilStopped;

        public override Node NP_GetNode()
        {
            return this.MWaitUntilStopped;
        }

        public override Task CreateTask(long UnitId, long RuntimeTreeID)
        {
            MWaitUntilStopped = new WaitUntilStopped();
            return MWaitUntilStopped;
        }
    }
}