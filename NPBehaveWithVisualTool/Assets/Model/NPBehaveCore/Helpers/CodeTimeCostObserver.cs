//------------------------------------------------------------
// Author: 烟雨迷离半世殇
// Mail: 1778139321@qq.com
// Data: 2019年8月5日 23:39:33
//------------------------------------------------------------

using System.Diagnostics;

namespace ETModel
{
    /// <summary>
    /// 代码片段耗时检测器
    /// </summary>
    public static class CodeTimeCostObserver
    {
        static Stopwatch _stopwatch;

        public static void StartObserve()
        {
            _stopwatch = new Stopwatch();
            _stopwatch.Start();
        }

        public static void StopObserve()
        {
            _stopwatch.Stop();
            Log.Info($"本次观测耗时为：{_stopwatch.Elapsed}");
        }
    }
}