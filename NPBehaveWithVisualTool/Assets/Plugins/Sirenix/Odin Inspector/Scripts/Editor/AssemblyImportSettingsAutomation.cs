//-----------------------------------------------------------------------
// <copyright file="AssemblyImportSettingsAutomation.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY_EDITOR && UNITY_5_6_OR_NEWER

namespace Sirenix.OdinInspector.Editor
{
    using System.Collections.Generic;
    using Sirenix.Serialization.Utilities.Editor;
    using Sirenix.Utilities;
    using UnityEditor;
    using UnityEditor.Build;

#if UNITY_2018_1_OR_NEWER
    using UnityEditor.Build.Reporting;
#endif

    public class AssemblyImportSettingsAutomation :
#if UNITY_2018_1_OR_NEWER
        IPreprocessBuildWithReport
#else
        IPreprocessBuild
#endif
    {

        public int callbackOrder { get { return -1500; } }

        private static void ConfigureImportSettings()
        {
            if (EditorOnlyModeConfig.Instance.IsEditorOnlyModeEnabled() || ImportSettingsConfig.Instance.AutomateBeforeBuild == false)
            {
                return;
            }

            var assemblyDir = SirenixAssetPaths.SirenixAssembliesPath;
            var aotDir = assemblyDir + "NoEmitAndNoEditor/";
            var jitDir = assemblyDir + "NoEditor/";
            var aotAssemblies = new List<string>();
            var jitAssemblies = new List<string>();
            var paths = AssetDatabase.GetAllAssetPaths();
            for (int i = 0; i < paths.Length; i++)
            {
                var p = paths[i];
                if (p.StartsWith(assemblyDir))
                {
                    if (!p.EndsWith(".dll", System.StringComparison.InvariantCultureIgnoreCase)) continue;
                    if (p.StartsWith(aotDir))
                    {
                        aotAssemblies.Add(p);
                    }
                    else if (p.StartsWith(jitDir))
                    {
                        jitAssemblies.Add(p);
                    }
                }
            }

            AssetDatabase.StartAssetEditing();
            try
            {
                var platform = EditorUserBuildSettings.activeBuildTarget;

                if (AssemblyImportSettingsUtilities.IsJITSupported(
                    platform,
                    AssemblyImportSettingsUtilities.GetCurrentScriptingBackend(),
                    AssemblyImportSettingsUtilities.GetCurrentApiCompatibilityLevel()))
                {
                    ApplyImportSettings(platform, aotAssemblies.ToArray(), OdinAssemblyImportSettings.ExcludeFromAll);
                    ApplyImportSettings(platform, jitAssemblies.ToArray(), OdinAssemblyImportSettings.IncludeInBuildOnly);
                }
                else
                {
                    ApplyImportSettings(platform, aotAssemblies.ToArray(), OdinAssemblyImportSettings.IncludeInBuildOnly);
                    ApplyImportSettings(platform, jitAssemblies.ToArray(), OdinAssemblyImportSettings.ExcludeFromAll);
                }
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
            }
        }

        private static void ApplyImportSettings(BuildTarget platform, string[] assemblyPaths, OdinAssemblyImportSettings importSettings)
        {
            for (int i = 0; i < assemblyPaths.Length; i++)
            {
                AssemblyImportSettingsUtilities.SetAssemblyImportSettings(platform, assemblyPaths[i], importSettings);
            }
        }

#if UNITY_2018_1_OR_NEWER

        void IPreprocessBuildWithReport.OnPreprocessBuild(BuildReport report)
        {
            ConfigureImportSettings();
        }

#else

        void IPreprocessBuild.OnPreprocessBuild(BuildTarget target, string path)
        {
            ConfigureImportSettings();
        }

#endif
    }
}

#endif // UNITY_EDITOR && UNITY_5_6_OR_NEWER