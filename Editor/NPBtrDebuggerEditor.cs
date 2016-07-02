using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(NPBtrDebugger))]
public class NPBtrDebuggerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        if( !target ) 
        {
            return;
        }

        NPBtrDebugger debugger = (NPBtrDebugger)target;

        if( !target || debugger.BehaviorTree == null) 
        {
            return;
        }

        if (debugger.BehaviorTree == null)
        {
            GUILayout.Label("NPBehave Debugger: No Behavor Tree Set");
        }
        else
        {
            EditorGUILayout.LabelField("Blackboard: ", EditorStyles.boldLabel);
            NPBtrBlackboard blackboard = debugger.BehaviorTree.Blackboard;
            List<string> keys = blackboard.Keys;
            foreach (string key in keys)
            {
                GUILayout.Label(" -  " + key + " : " + blackboard.Get(key));
            }
            
            EditorGUILayout.LabelField("Behaviour Tree: ", EditorStyles.boldLabel);
            Traverse(" ", debugger.BehaviorTree);

            EditorUtility.SetDirty(debugger); // ensure we are redrawn every frame

            EditorGUILayout.LabelField("Statistics:", EditorStyles.boldLabel);
            GUILayout.Label(" - Totals (Start|Stop|Stopped):  " + debugger.BehaviorTree.TotalNumStartCalls + "|" + debugger.BehaviorTree.TotalNumStopCalls + "|" + debugger.BehaviorTree.TotalNumStoppedCalls);
            GUILayout.Label(" - Active Timers:  " + debugger.BehaviorTree.Clock.NumTimers);
            GUILayout.Label(" - Timer Pool Size:  " + debugger.BehaviorTree.Clock.DebugPoolSize);
            GUILayout.Label(" - Active Update Observers:  " + debugger.BehaviorTree.Clock.NumUpdateObservers);
            GUILayout.Label(" - Active Blackboard Observers:  " + debugger.BehaviorTree.Blackboard.NumObservers);

        }
    }

    private void Traverse(string prefix, NPBtrNode node)
    {
        Print(prefix, node);

        if (node is NPBtrContainer)
        {
            NPBtrNode[] children = (node as NPBtrContainer).DebugChildren;
            if (children == null)
            {
                GUILayout.Label(prefix + " CHILDREN ARE NULL");
            }
            else
            {
                foreach (NPBtrNode child in children)
                {
                    Traverse(prefix + "  ", child);
                }
            }
        }
    }

    private void Print(string prefix, NPBtrNode node)
    {
        if (node.CurrentState == NPBtrNode.State.ACTIVE)
        {
            string label = prefix + " > " + node + " (" + node.DebugNumStartCalls + "|" + node.DebugNumStopCalls + "|" + node.DebugNumStoppedCalls + (node.DebugNumStoppedCalls > 0 ? "|" + node.DebugLastResult:"") + ")";
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(label);
            if (GUILayout.Button("Stop"))
            {
                node.Stop();
            }
            EditorGUILayout.EndHorizontal();
        }
        else
        {
            string label = prefix + " - " + node + " (" + node.DebugNumStartCalls + "|" + node.DebugNumStopCalls + "|" + node.DebugNumStoppedCalls + (node.DebugNumStoppedCalls > 0 ? "|" + node.DebugLastResult:"") + ")";

            if (node is NPBtrRoot)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(label);
                if (GUILayout.Button("Start"))
                {
                    node.Start();
                }
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                EditorGUILayout.LabelField(label);
            }
        }
    }
}
