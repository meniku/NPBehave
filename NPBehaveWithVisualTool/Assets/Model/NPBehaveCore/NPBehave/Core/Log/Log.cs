//------------------------------------------------------------
// Author: 烟雨迷离半世殇
// Mail: 1778139321@qq.com
// Data: 2019年8月19日 16:37:38
//------------------------------------------------------------

using System;

#if UNITY_EDITOR
    using UnityEngine;
#endif

namespace NPBehave_Core
{
    public static class Log
    {
        public static void Info(string content)
        {
#if UNITY_EDITOR
            UnityEngine.Debug.Log(content);
#else
            Console.WriteLine("信息：" + content);
#endif
        }

        public static void Error(string content)
        {
#if UNITY_EDITOR
            UnityEngine.Debug.LogError(content);
#else
            Console.WriteLine("错误：" + content);
#endif
        }
    }
}