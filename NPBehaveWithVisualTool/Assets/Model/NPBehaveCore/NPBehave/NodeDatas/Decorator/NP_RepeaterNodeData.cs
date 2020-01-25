//------------------------------------------------------------
// Author: 烟雨迷离半世殇
// Mail: 1778139321@qq.com
// Data: 2019年9月26日 21:28:13
//------------------------------------------------------------

using NPBehave;
using Sirenix.OdinInspector;

namespace ETModel
{
    public class NP_RepeaterNodeData:NP_NodeDataBase
    {
        [HideInEditorMode]
        public Repeater m_Repeater;
        
        public override Node NP_GetNode()
        {
            return this.m_Repeater;
        }

        public override Decorator CreateDecoratorNode(long UnitId, long RuntimeTreeID, Node node)
        {
            this.m_Repeater = new Repeater(node);
            return this.m_Repeater;
        }
    }
}