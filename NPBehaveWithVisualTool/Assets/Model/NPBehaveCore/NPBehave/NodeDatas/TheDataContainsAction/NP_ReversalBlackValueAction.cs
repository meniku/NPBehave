//------------------------------------------------------------
// Author: 烟雨迷离半世殇
// Mail: 1778139321@qq.com
// Data: 2020年1月25日 13:54:24
//------------------------------------------------------------

using NPBehave;
using Sirenix.OdinInspector;
using Action = System.Action;

namespace ETModel.TheDataContainsAction
{
    /// <summary>
    /// 反转黑板值
    /// </summary>
    public class NP_ReversalBlackValueAction : NP_ClassForStoreAction
    {
        [LabelText("黑板节点相关的数据")] public NP_BlackBoardRelationData m_NPBalckBoardRelationData;

        public override Action GetActionToBeDone()
        {
            this.m_Action = this.ReversalBlackValue;
            return this.m_Action;
        }

        public void ReversalBlackValue()
        {
            switch (m_NPBalckBoardRelationData.m_CompareType)
            {
                case CompareType._Bool:
                    m_NPBalckBoardRelationData.theBoolValue = !m_NPBalckBoardRelationData.theBoolValue;
                    break;
                case CompareType._Int:
                    m_NPBalckBoardRelationData.theIntValue = -m_NPBalckBoardRelationData.theIntValue;
                    break;
                case CompareType._Float:
                    m_NPBalckBoardRelationData.theFloatValue = -m_NPBalckBoardRelationData.theFloatValue;
                    break;
            }

            Blackboard blackboard = Game.Scene.GetComponent<UnitComponent>().Get(this.Unitid)
                .GetComponent<NP_RuntimeTreeManager>()
                .GetTreeByRuntimeID(RuntimeTreeID)
                .GetBlackboard();
            m_NPBalckBoardRelationData.SetBlackBoardValue(blackboard);
        }
    }
}