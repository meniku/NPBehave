using UnityEngine;
using System.Collections.Generic;

namespace NPBehave.Unity
{
    public class UnityContextProvider : MonoBehaviour
    {
        UnityContext unityContext = new UnityContext();

        void Update()
        {
            unityContext.Update( Time.deltaTime );
        }
    }
}