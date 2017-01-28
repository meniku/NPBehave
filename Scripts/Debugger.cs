using UnityEngine;
using System.Collections.Generic;

namespace NPBehave
{
    public class Debugger : MonoBehaviour
    {
        public Root BehaviorTree;

        private static Blackboard _customGlobalStats = null;
        public static Blackboard CustomGlobalStats
        {
            get 
            {
                if (_customGlobalStats == null)
                {
                    _customGlobalStats = UnityContext.GetSharedBlackboard("_GlobalStats");;
                }
                return _customGlobalStats;
            }
        }

        private Blackboard _customStats = null;
        public Blackboard CustomStats
        {
            get 
            {
                if (_customStats == null)
                {
                    _customStats = new Blackboard(CustomGlobalStats, UnityContext.GetClock());
                }
                return _customStats;
            }
        }

        public void DebugCounterInc(string key)
        {
            if (!CustomStats.Isset(key))
            {
                CustomStats[key] = 0;
            }
            CustomStats[key] = CustomStats.Get<int>(key) + 1;
        }

        public void DebugCounterDec(string key)
        {
            if (!CustomStats.Isset(key))
            {
                CustomStats[key] = 0;
            }
            CustomStats[key] = CustomStats.Get<int>(key) - 1;
        }

        public static void GlobalDebugCounterInc(string key)
        {
            if (!CustomGlobalStats.Isset(key))
            {
                CustomGlobalStats[key] = 0;
            }
            CustomGlobalStats[key] = CustomGlobalStats.Get<int>(key) + 1;
        }

        public static void GlobalDebugCounterDec(string key)
        {
            if (!CustomGlobalStats.Isset(key))
            {
                CustomGlobalStats[key] = 0;
            }
            CustomGlobalStats[key] = CustomGlobalStats.Get<int>(key) - 1;
        }

    }
}