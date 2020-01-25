//------------------------------------------------------------
// Author: 烟雨迷离半世殇
// Mail: 1778139321@qq.com
// Data: 2019年8月22日 20:32:18
//------------------------------------------------------------

using ETModel.TheDataContainsAction;
using NPBehave;
using Sirenix.OdinInspector;
using Action = System.Action;

namespace ETModel
{
    public class NP_ServiceNodeData: NP_NodeDataBase
    {
        [HideInEditorMode]
        public Service m_Service;

        [LabelText("委托执行时间间隔")]
        public float interval;

        public NP_ClassForStoreAction MNpClassForStoreAction;

        public override Node NP_GetNode()
        {
            return this.m_Service;
        }

        public override Decorator CreateDecoratorNode(long UnitId, long RuntimeTreeID, Node node)
        {
            this.MNpClassForStoreAction.Unitid = UnitId;
            this.MNpClassForStoreAction.RuntimeTreeID = RuntimeTreeID;
            this.m_Service = new Service(interval, MNpClassForStoreAction.GetActionToBeDone(), node);
            return this.m_Service;
        }
    }
}