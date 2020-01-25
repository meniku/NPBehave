//------------------------------------------------------------
// Author: 烟雨迷离半世殇
// Mail: 1778139321@qq.com
// Data: 2019年8月20日 8:11:55
//------------------------------------------------------------

using System;
using NodeEditorFramework;
using UnityEngine;
using Node = NPBehave.Node;

namespace Plugins.NodeEditor.Editor.NPBehaveNodes
{
    public class NPBehave_PreNode: ValueConnectionType //: IConnectionTypeDeclaration
    {
        public override string Identifier => "NPBehave_PrevNodeDatas";

        public override Type Type => typeof (Node);

        public override Color Color => Color.yellow;
    }

    public class NPBehave_NextNode: ValueConnectionType // : IConnectionTypeDeclaration
    {
        public override string Identifier => "NPBehave_NextNodeDatas";

        public override Type Type => typeof (Node);

        public override Color Color => Color.cyan;
    }
}