namespace ETModel
{
    public static class EventIdType
    {
        public const string RecvHotfixMessage = "RecvHotfixMessage";
        public const string BehaviorTreeRunTreeEvent = "BehaviorTreeRunTreeEvent";
        public const string BehaviorTreeOpenEditor = "BehaviorTreeOpenEditor";
        public const string BehaviorTreeClickNode = "BehaviorTreeClickNode";
        public const string BehaviorTreeAfterChangeNodeType = "BehaviorTreeAfterChangeNodeType";
        public const string BehaviorTreeCreateNode = "BehaviorTreeCreateNode";
        public const string BehaviorTreePropertyDesignerNewCreateClick = "BehaviorTreePropertyDesignerNewCreateClick";
        public const string BehaviorTreeMouseInNode = "BehaviorTreeMouseInNode";
        public const string BehaviorTreeConnectState = "BehaviorTreeConnectState";
        public const string BehaviorTreeReplaceClick = "BehaviorTreeReplaceClick";
        public const string BehaviorTreeRightDesignerDrag = "BehaviorTreeRightDesignerDrag";
        public const string SessionRecvMessage = "SessionRecvMessage";
        public const string NumbericChange = "NumbericChange";
        public const string MessageDeserializeFinish = "MessageDeserializeFinish";
        public const string SceneChange = "SceneChange";
        public const string FrameUpdate = "FrameUpdate";

        public const string TestHotfixSubscribMonoEvent = "TestHotfixSubscribMonoEvent";
        public const string MaxModelEvent = "MaxModelEvent";

        // 检查资源更新
        public const string CheckForUpdateBegin = "CheckForUpdateBegin";
        public const string CheckForUpdateFinish = "CheckForUpdateFinish";

        // 加载UI
        public const string ShowLoadingUI = "ShowLoadingUI";
        public const string CloseLoadingUI = "CloseLoadingUI";
        public const string CreateLoadingUI = "CreateLoadingUI";

        // 显示热更层对话框
        public const string ShowOfflineDialogUI_Model = "ShowOfflineDialogUI_Model";

        // 小地图寻路事件，用来传递目标点
        public const string SmallMapPathFinder = "SmallMapPathFinder";

        //Buff奏效回调
        public const string BuffCallBack = "BuffCallBack";

        //给NPBehave服务端用的事件系统
        public const string CreateCollider = "CreateCollider";

        public const string ChangeHP = "ChangeHP";
        public const string ChangeMP = "ChangeMP";
        
        public const string BattleEvent_PlayEffect = "BattleEvent_PlayEffect";
    }
}