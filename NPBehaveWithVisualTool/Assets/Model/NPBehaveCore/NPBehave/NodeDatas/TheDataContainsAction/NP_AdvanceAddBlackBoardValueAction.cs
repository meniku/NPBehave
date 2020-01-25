//------------------------------------------------------------
// Author: 烟雨迷离半世殇
// Mail: 1778139321@qq.com
// Data: 2020年1月19日 12:31:47
//------------------------------------------------------------

using System;
using Sirenix.OdinInspector;

namespace ETModel.TheDataContainsAction
{
    /// <summary>
    /// 提前增加黑板数据，用于初始化操作
    /// </summary>
    public class NP_AdvanceAddBlackBoardValue: NP_ClassForStoreAction
    {
        [LabelText("黑板节点相关的数据")]
        public NP_BlackBoardRelationData m_NPBalckBoardRelationData;

        public override Action GetActionToBeDone()
        {
            this.m_Action = this.AdvanceAddBlackBoardValue;
            return this.m_Action;
        }

        public void AdvanceAddBlackBoardValue()
        {
            this.m_NPBalckBoardRelationData.SetBlackBoardValue(Game.Scene.GetComponent<UnitComponent>().Get(this.Unitid)
                    .GetComponent<NP_RuntimeTreeManager>()
                    .GetTreeByRuntimeID(this.RuntimeTreeID)
                    .GetBlackboard());
            //Log.Info("提前加入了黑板数据");
        }
    }
}