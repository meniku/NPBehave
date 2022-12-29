using UnityEditor;
using UnityEngine;

namespace CleverCrow.Fluid.BTs.Trees.Editors {
    /// <summary>
    /// Determine if this is a package of the Unity Editor since Unity has no API to determine this
    /// </summary>
    public static class AssetPath {
        private const string PATH_PROJECT = @"Assets\Cabin Icarus\IcFrameWork\IcSkillSystem\NPBehave\Editor\fluid-behavior-tree-Editor";

        private static string _basePath;

        public static string BasePath {
            get {
                if (_basePath != null) return _basePath;

                if (AssetDatabase.IsValidFolder(PATH_PROJECT)) {
                    _basePath = PATH_PROJECT;
                    return _basePath;
                }

                Debug.LogError("Asset root could not be found");

                return null;
            }
        }
    }
}