
using UnityEngine;

namespace ETModel
{
    public static class UnitFactory
    {

        /// <summary>
        /// 用于NPBehave测试
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Unit NPBehaveTestCreate()
        {
            UnitComponent unitComponent = Game.Scene.GetComponent<UnitComponent>();
            Unit unit = ComponentFactory.Create<Unit>();
            unit.AddComponent<NP_RuntimeTreeManager>();
            unitComponent.Add(unit);
            return unit;
        }
    }
}