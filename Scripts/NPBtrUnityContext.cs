using UnityEngine;
using System.Collections.Generic;

public class NPBtrUnityContext : MonoBehaviour
{
    private static NPBtrUnityContext instance = null;

    private static NPBtrUnityContext GetInstance()
    {
        if (instance == null)
        {
            GameObject gameObject = new GameObject();
            gameObject.name = "~NPBtrContext";
            instance = (NPBtrUnityContext)gameObject.AddComponent(typeof(NPBtrUnityContext));
            gameObject.isStatic = true;
#if !UNITY_EDITOR
            gameObject.hideFlags = HideFlags.HideAndDontSave;
#endif
        }
        return instance;
    }
    
    public static NPBtrClock GetClock()
    {
        return GetInstance().clock;
    }
    
    public static NPBtrBlackboard GetSharedBlackboard(string key)
    {
        NPBtrUnityContext context = GetInstance();
        if(!context.blackboards.ContainsKey(key))
        {
            context.blackboards.Add(key, new NPBtrBlackboard(context.clock));
        }
        return context.blackboards[key];
    }
    
    private Dictionary<string, NPBtrBlackboard> blackboards = new Dictionary<string, NPBtrBlackboard>();

    private NPBtrClock clock = new NPBtrClock();

    void Update()
    {
        clock.Update(Time.deltaTime);
    }
}
