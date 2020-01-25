//------------------------------------------------------------
// Author: 烟雨迷离半世殇
// Mail: 1778139321@qq.com
// Data: 2019年8月21日 7:11:32
//------------------------------------------------------------

using MongoDB.Bson.Serialization.Attributes;
using NPBehave;
using Sirenix.OdinInspector;

namespace ETModel
{
    public class NP_SequenceNodeData: NP_NodeDataBase
    {
        [HideInEditorMode]
        public Sequence mSequenceNode;

        public override Node NP_GetNode()
        {
            return this.mSequenceNode;
        }

        public override Composite CreateComposite(Node[] nodes)
        {
            mSequenceNode = new Sequence(nodes);
            return mSequenceNode;
        }
    }
}