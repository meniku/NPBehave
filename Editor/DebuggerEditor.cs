using System.Collections;
using System.Collections.Generic;
using CleverCrow.Fluid.BTs.Trees.Editors;
using UnityEditor;
using UnityEngine;

namespace NPBehave
{
    [CustomEditor(typeof(Debugger))]
    public class DebuggerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var debugger = (Debugger) target;
            
            GUILayout.Label($"NPBehave Debugger {debugger.Label}", EditorStyles.centeredGreyMiniLabel);

            if (GUILayout.Button("Open Debugger"))
            {
                DebuggerWindow.selectedDebugger = debugger;
                DebuggerWindow.selectedObject = DebuggerWindow.selectedDebugger.transform;
                DebuggerWindow.ShowWindow();
            }

            if (GUILayout.Button("Open Tree Window"))
            {
                BehaviorTreeWindow.ShowTree(debugger.BehaviorTree, debugger.Label);
            }
        }
    }
}