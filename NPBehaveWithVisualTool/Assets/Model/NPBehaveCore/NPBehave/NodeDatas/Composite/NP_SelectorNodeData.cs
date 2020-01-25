//------------------------------------------------------------
// Author: 烟雨迷离半世殇
// Mail: 1778139321@qq.com
// Data: 2019年8月21日 7:11:12
//------------------------------------------------------------

using MongoDB.Bson.Serialization.Attributes;
using NPBehave;
using Sirenix.OdinInspector;

namespace ETModel
{
    [BsonIgnoreExtraElements]
    public class NP_SelectorNodeData:NP_NodeDataBase
    {
        [HideInEditorMode]
        public Selector mSelectorNode;

        public override Composite CreateComposite(Node[] nodes)
        {
            mSelectorNode = new Selector(nodes);
            return mSelectorNode;
        }

        public override Node NP_GetNode()
        {
            return this.mSelectorNode;
        }
    }
}