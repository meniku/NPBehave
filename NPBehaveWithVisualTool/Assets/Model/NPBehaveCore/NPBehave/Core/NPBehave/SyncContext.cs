//------------------------------------------------------------
// Author: 烟雨迷离半世殇
// Mail: 1778139321@qq.com
// Data: 2019年8月19日 11:28:12
//------------------------------------------------------------

using System.Collections.Generic;

namespace NPBehave
{
    public class SyncContext
    {
        private static SyncContext _instance;

        public static SyncContext Instance
        {
            get
            {
                return _instance ?? (_instance = new SyncContext());
            }
        }
        
        private Dictionary<string, Blackboard> blackboards = new Dictionary<string, Blackboard>();
        
        private Clock clock = new Clock();

        public Clock GetClock()
        {
            return Instance.clock;
        }
        
        public static Blackboard GetSharedBlackboard(string key)
        {
            if (!Instance.blackboards.ContainsKey(key))
            {
                Instance.blackboards.Add(key, new Blackboard(Instance.clock));
            }

            return Instance.blackboards[key];
        }

        public void Update()
        {
            clock.Update(1f/60f);
        }
        
    }
}