//------------------------------------------------------------
// Author: 烟雨迷离半世殇
// Mail: 1778139321@qq.com
// Data: 2020年1月10日 20:23:42
//------------------------------------------------------------

using System;
using ETModel;
using NodeEditorFramework;

namespace Plugins.NodeEditor.Node_Editor.Default
{
    [CannotShowInToolBarCanvasType]
    [NodeCanvasType("默认Canvas")]
    public class DefaultCanvas: NodeCanvas
    {
        public override string canvasName => Name;
        
        public string Name = "默认Canvas";
        
        protected override void OnCreate()
        {
            Log.Error("当你来到这个默认Canvas界面，证明你可能遇到了问题，因为这个界面默认是不使用的，所以请先尝试在当前这个编辑器界面Load你想打开的Canvas吧！");
        }
    }
}