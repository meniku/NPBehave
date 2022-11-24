using UnityEngine;
using System.Collections.Generic;

namespace NPBehave.Unity
{
    public class UnityPlatform : Platform
    {
        public override float GenerateRandomFloat()
        {
            return UnityEngine.Random.value;
        }
    }

    public class UnityContext : Context
    {
        static UnityContext()
        {
            Instance = new UnityContext();
            Platform = new UnityPlatform();
        }

        public void Update(double gameTime)
        {
            Clock.Update( ( float ) gameTime );
        }
    }
}