using System.Collections.Generic;

namespace NPBehave.Monogame
{
    public class MonogamePlatform : Platform
    {
        private System.Random Random = new System.Random();

        public override float GenerateRandomFloat()
        {
            return ( float ) Random.NextDouble();
        }
    }

    public class MonogameContext : Context
    {
        static MonogameContext()
        {
            Instance = new MonogameContext();
            Platform = new MonogamePlatform();
        }

        public static void Update( double gameTime )
        {
            Clock.Update( (float) gameTime );
        }
    }
}