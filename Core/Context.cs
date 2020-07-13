using System.Collections.Generic;

namespace NPBehave
{
    public abstract class Platform
    {
        public abstract float GenerateRandomFloat();
    }

    public abstract class Context
    {
        internal static Context Instance = null;

        private static Context GetInstance()
        {
            return Instance;
        }

        public static Platform Platform { get; internal set; }
        public static Clock Clock = new Clock();

        public static Blackboard GetSharedBlackboard( string key )
        {
            Context context = GetInstance();
            if ( !context.Blackboards.ContainsKey( key ) )
            {
                context.Blackboards.Add( key, new Blackboard( Clock ) );
            }
            return context.Blackboards[ key ];
        }

        protected Dictionary<string, Blackboard> Blackboards = new Dictionary<string, Blackboard>();
    }
}