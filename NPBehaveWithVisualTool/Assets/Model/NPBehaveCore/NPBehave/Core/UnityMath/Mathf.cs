//------------------------------------------------------------
// Author: 烟雨迷离半世殇
// Mail: 1778139321@qq.com
// Data: 2019年8月19日 16:36:16
//------------------------------------------------------------

using System;

namespace NPBehave_Core
{
    public class Mathf
    {
        public static float Random()
        {
            var seed = Guid.NewGuid().GetHashCode();
            Random r = new Random(seed);
            int i = r.Next(0, 100000);
            return (float) i / 100000;
        }
    }
}