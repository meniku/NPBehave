﻿#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
#if UNITY_5_3_OR_NEWER || UNITY_5_3
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
#endif
using System;

namespace NodeEditorFramework.Utilities
{
	[InitializeOnLoad]
	public static class EditorLoadingControl 
	{
#if UNITY_5_3_OR_NEWER || UNITY_5_3
		private static Scene loadedScene;
#else
		private static string loadedScene;
#endif

		private static bool serializationTest = false;
		private static bool playmodeSwitchToEdit = false;
		private static bool toggleLateEnteredPlaymode = false;

		public static Action beforeEnteringPlayMode;
		public static Action justEnteredPlayMode;
		public static Action lateEnteredPlayMode;
		public static Action beforeLeavingPlayMode;
		public static Action justLeftPlayMode;
		public static Action justOpenedNewScene;

		static EditorLoadingControl () 
		{
#if UNITY_2017_2_OR_NEWER
			EditorApplication.playModeStateChanged -= PlayModeStateChanged;
			EditorApplication.playModeStateChanged += PlayModeStateChanged;
#else
			EditorApplication.playmodeStateChanged -= PlaymodeStateChanged;
			EditorApplication.playmodeStateChanged += PlaymodeStateChanged;
#endif
			EditorApplication.update -= Update;
			EditorApplication.update += Update;
#if UNITY_2018_1_OR_NEWER
			EditorApplication.hierarchyChanged -= OnHierarchyChange;
			EditorApplication.hierarchyChanged += OnHierarchyChange;
#else
			EditorApplication.hierarchyWindowChanged -= OnHierarchyChange;
			EditorApplication.hierarchyWindowChanged += OnHierarchyChange;
#endif
		}

		private static void OnHierarchyChange () 
		{ // TODO: OnGUI might be called before this function and migth cause problems. Find a better way to detect scene change!
#if UNITY_5_3_OR_NEWER || UNITY_5_3
			Scene currentScene = EditorSceneManager.GetActiveScene ();
#else
			string currentScene = Application.loadedLevelName;
#endif
			if (loadedScene != currentScene)
			{
				if (justOpenedNewScene != null)
					justOpenedNewScene.Invoke ();
				loadedScene = currentScene;
			}
		}

		// Handles just after switch (non-serialized values lost)
		private static void Update () 
		{
			if (toggleLateEnteredPlaymode)
			{
				toggleLateEnteredPlaymode = false;
				if (lateEnteredPlayMode != null)
					lateEnteredPlayMode.Invoke ();
			}
			serializationTest = true;
		}

#if UNITY_2017_2_OR_NEWER
		private static void PlayModeStateChanged(PlayModeStateChange stateChange)
		{
			PlaymodeStateChanged();
		}
#endif

		private static void PlaymodeStateChanged () 
		{
			//Debug.Log ("Playmode State Change! isPlaying: " + Application.isPlaying + "; Serialized: " + serializationTest);
			if (!Application.isPlaying)
			{ // Edit Mode
				if (playmodeSwitchToEdit)
				{ // After Playmode
					//Debug.Log ("LOAD PLAY MODE Values in Edit Mode!!");
					if (justLeftPlayMode != null)
						justLeftPlayMode.Invoke ();
					playmodeSwitchToEdit = false;
				}
				else 
				{ // Before Playmode
					//Debug.Log ("SAVE EDIT MODE Values before Play Mode!!");
					if (beforeEnteringPlayMode != null)
						beforeEnteringPlayMode.Invoke ();
				}
			}
			else
			{ // Play Mode
				if (serializationTest) 
				{ // Before Leaving Playmode
					//Debug.Log ("SAVE PLAY MODE Values before Edit Mode!!");
					if (beforeLeavingPlayMode != null)
						beforeLeavingPlayMode.Invoke ();
					playmodeSwitchToEdit = true;
				}
				else
				{ // After Entering Playmode
					//Debug.Log ("LOAD EDIT MODE Values in Play Mode!!");
					if (justEnteredPlayMode != null)
						justEnteredPlayMode.Invoke ();
					toggleLateEnteredPlaymode = true;
				}

			}
		}
	}
}
#endif
