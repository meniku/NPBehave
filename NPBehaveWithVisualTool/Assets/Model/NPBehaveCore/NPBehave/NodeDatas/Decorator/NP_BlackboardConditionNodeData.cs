//------------------------------------------------------------
// Author: 烟雨迷离半世殇
// Mail: 1778139321@qq.com
// Data: 2019年8月21日 7:12:13
//------------------------------------------------------------

using System.Collections.Generic;
using NPBehave;
using Sirenix.OdinInspector;

namespace ETModel
{
    public enum CompareType
    {
        _String,
        _Float,
        _Int,
        _Bool
    }

    public class NP_BlackboardConditionNodeData: NP_NodeDataBase
    {
        [HideInEditorMode]
        public BlackboardCondition mBlackboardConditionNode;

        [LabelText("运算符号")]
        public Operator mOpe;

        [LabelText("终止条件")]
        public Stops stop;

        [LabelText("黑板节点相关的数据")]
        public NP_BlackBoardRelationData m_NPBalckBoardRelationData;

        public override Decorator CreateDecoratorNode(long UnitId, long RuntimeTreeID, Node node)
        {
            switch (m_NPBalckBoardRelationData.m_CompareType)
            {
                case CompareType._String:
                    this.mBlackboardConditionNode = new BlackboardCondition(m_NPBalckBoardRelationData.DicKey, this.mOpe,
                        this.m_NPBalckBoardRelationData.theStringValue, this.stop, node);
                    break;
                case CompareType._Float:
                    this.mBlackboardConditionNode = new BlackboardCondition(m_NPBalckBoardRelationData.DicKey, this.mOpe,
                        this.m_NPBalckBoardRelationData.theFloatValue, this.stop, node);
                    break;
                case CompareType._Int:
                    this.mBlackboardConditionNode = new BlackboardCondition(m_NPBalckBoardRelationData.DicKey, this.mOpe,
                        this.m_NPBalckBoardRelationData.theIntValue, this.stop, node);
                    break;
                case CompareType._Bool:
                    this.mBlackboardConditionNode = new BlackboardCondition(m_NPBalckBoardRelationData.DicKey, this.mOpe,
                        this.m_NPBalckBoardRelationData.theBoolValue, this.stop, node);
                    break;
            }

            //此处的value参数可以随便设，因为我们在游戏中这个value是需要动态改变的
            return mBlackboardConditionNode;
        }

        public override Node NP_GetNode()
        {
            return this.mBlackboardConditionNode;
        }
    }
}