//------------------------------------------------------------
// Author: 烟雨迷离半世殇
// Mail: 1778139321@qq.com
// Data: 2019年8月21日 7:10:51
//------------------------------------------------------------

using System.Collections.Generic;
using NPBehave;
using Sirenix.OdinInspector;

namespace ETModel
{
    public class NP_ParallelNodeData: NP_NodeDataBase
    {
        [HideInEditorMode]
        public Parallel mParallelNode;

        [LabelText("成功政策")]
        public Parallel.Policy SuccessPolicy;

        [LabelText("失败政策")]
        public Parallel.Policy FailurePolicy;
        
        public override Composite CreateComposite(Node[] nodes)
        {
            this.mParallelNode = new Parallel(SuccessPolicy, FailurePolicy, nodes);
            return mParallelNode;
        }

        public override Node NP_GetNode()
        {
            return this.mParallelNode;
        }
    }
}