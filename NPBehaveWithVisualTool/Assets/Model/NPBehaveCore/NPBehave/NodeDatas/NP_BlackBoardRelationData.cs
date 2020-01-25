//------------------------------------------------------------
// Author: 烟雨迷离半世殇
// Mail: 1778139321@qq.com
// Data: 2019年9月25日 13:59:03
//------------------------------------------------------------

using NPBehave;
using Sirenix.OdinInspector;

namespace ETModel
{
    /// <summary>
    /// 与黑板节点相关的数据
    /// </summary>
    public class NP_BlackBoardRelationData
    {
        [LabelText("字典键")]
        public string DicKey;

        [LabelText("指定的值类型")]
        public CompareType m_CompareType;

        [ShowIf("m_CompareType", CompareType._String)]
        public string theStringValue;

        [ShowIf("m_CompareType", CompareType._Float)]
        public float theFloatValue;

        [ShowIf("m_CompareType", CompareType._Int)]
        public int theIntValue;

        [ShowIf("m_CompareType", CompareType._Bool)]
        public bool theBoolValue;

        /// <summary>
        /// 自动根据预先设定的值设置值
        /// </summary>
        /// <param name="blackboard">要修改的黑板</param>
        public void SetBlackBoardValue(Blackboard blackboard)
        {
            switch (m_CompareType)
            {
                case CompareType._String:
                    blackboard[DicKey] = this.theStringValue;
                    break;
                case CompareType._Float:
                    blackboard[DicKey] = this.theFloatValue;
                    break;
                case CompareType._Int:
                    blackboard[DicKey] = this.theIntValue;
                    break;
                case CompareType._Bool:
                    blackboard[DicKey] = this.theBoolValue;
                    break;
            }
        }

        /// <summary>
        /// 自动根据传来的值设置值
        /// </summary>
        /// <param name="blackboard">将要改变的黑板值</param>
        /// <param name="compareType">值类型</param>
        /// <param name="value">值</param>
        public void SetBlackBoardValue(Blackboard blackboard, CompareType compareType, object value)
        {
            if (compareType != this.m_CompareType)
            {
                Log.Error("要修改的值与预设类型不符");
                return;
            }

            blackboard[DicKey] = value;
        }
    }
}