//------------------------------------------------------------
// Author: 烟雨迷离半世殇
// Mail: 1778139321@qq.com
// Data: 2020年1月19日 10:56:54
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace ETModel.TheDataContainsAction
{
    /// <summary>
    /// 用于初始化行为树的结点,去执行那些只会执行一次的初始化操作
    /// 注意，里面的所有action只能是Action()，不能是Func这种
    /// 并且默认只能是加给自己的Buff
    /// </summary>
    public class NP_InitTreeAction: NP_ClassForStoreAction
    {
        [LabelText("初始化行为节点集合")]
        public List<NP_ClassForStoreAction> NpClassForStoreActions = new List<NP_ClassForStoreAction>();

        public override Action GetActionToBeDone()
        {
            Game.Scene.GetComponent<UnitComponent>().Get(this.Unitid).GetComponent<NP_InitCacheComponent>()
                    .AdvanceAddInitData(this.RuntimeTreeID, "InitTree", true);

            foreach (var VARIABLE in NpClassForStoreActions)
            {
                VARIABLE.Unitid = this.Unitid;
                VARIABLE.RuntimeTreeID = this.RuntimeTreeID;
            }

            this.m_Action = this.DoInit;
            return this.m_Action;
        }

        public void DoInit()
        {
            Log.Info("准备执行初始化的行为操作");
            foreach (var VARIABLE in NpClassForStoreActions)
            {
                VARIABLE.GetActionToBeDone().Invoke();
            }
        }
    }
}