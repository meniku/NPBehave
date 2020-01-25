//------------------------------------------------------------
// Author: 烟雨迷离半世殇
// Mail: 1778139321@qq.com
// Data: 2020年1月16日 23:27:45
//------------------------------------------------------------

using System.Collections.Generic;
using UnityEngine;

namespace ETModel
{
    /// <summary>
    /// 行为树初始化缓存组件，缓存一些行为树需要进行的初始化操作（多为黑板数据的添加）
    /// </summary>
    public class NP_InitCacheComponent: Component
    {
        //long为运行时行为树ID，string为字典键，object为对应值
        public Dictionary<long, List<(string, object)>> CacheDatas = new Dictionary<long, List<(string, object)>>();

        /// <summary>
        /// 提前加入要初始化的数据
        /// </summary>
        /// <param name="runtimeId"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void AdvanceAddInitData(long runtimeId, string key, object value)
        {
            if (this.CacheDatas.TryGetValue(runtimeId, out var keyvalueList))
            {
                keyvalueList.Add((key,value));
                return;
            }
            List<(string, object)> temp = new List<(string, object)>();
            temp.Add((key, value));
            CacheDatas.Add(runtimeId, temp);
        }

        /// <summary>
        /// 把缓存的数据给目标行为树
        /// </summary>
        public void AddCacheDatas2RuntimeTree(NP_RuntimeTree npRuntimeTree)
        {
            Log.Info("准备添加缓存数据到行为树");
            if (this.CacheDatas.TryGetValue(npRuntimeTree.Id, out var keyvalueList))
            {
                foreach (var VARIABLE in keyvalueList)
                {
                    npRuntimeTree.GetBlackboard()[VARIABLE.Item1] = VARIABLE.Item2;
                    Log.Info($"已经加入字典键{VARIABLE.Item1},值为{VARIABLE.Item2}");
                }
            }
            Log.Info("添加数据完毕");
        }
    }
}