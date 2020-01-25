//------------------------------------------------------------
// Author: 烟雨迷离半世殇
// Mail: 1778139321@qq.com
// Data: 2019年8月22日 21:13:00
//------------------------------------------------------------

using System;
using Sirenix.OdinInspector;

namespace ETModel.TheDataContainsAction
{
    public class NP_LogAction:NP_ClassForStoreAction
    {
        [LabelText("打印信息")]
        public string LogInfo;
        
        public override Action GetActionToBeDone()
        {
            this.m_Action = this.TestLog;
            return this.m_Action;
        }

        public void TestLog()
        {
            Log.Info(LogInfo);
        }
    }
}