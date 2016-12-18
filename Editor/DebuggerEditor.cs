using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace NPBehave
{
    [CustomEditor(typeof(Debugger))]
    public class DebuggerEditor : Editor
    {
        private const int nestedPadding = 10;

        private GUIStyle smallTextStyle, nodeTextStyle;
        private GUIStyle nestedBoxStyle;

        private Color defaultColor;

        public void OnEnable()
        {
            nestedBoxStyle = new GUIStyle();
            nestedBoxStyle.margin = new RectOffset(nestedPadding, 0, 0, 0);

            smallTextStyle = new GUIStyle();
            smallTextStyle.font = EditorStyles.miniFont;

            nodeTextStyle = new GUIStyle(EditorStyles.label);

            defaultColor = EditorGUIUtility.isProSkin ? Color.white : Color.black;
        }

        public override void OnInspectorGUI()
        {
            GUI.color = defaultColor;
            GUILayout.Toggle(false, "NPBehave Debugger", GUI.skin.FindStyle("LODLevelNotifyText"));
            GUI.color = Color.white;

            if (!target)
            {
                return;
            }

            Debugger debugger = (Debugger)target;

            if (!target || debugger.BehaviorTree == null)
            {
                return;
            }

            if (debugger.BehaviorTree == null)
            {
                GUILayout.Label("NPBehave Debugger: No Behavor Tree Set");
            }
            else
            {
                GUIStyle boxStyle = EditorStyles.helpBox;

                GUILayout.BeginHorizontal();
                DrawBlackboardKeyAndValues(debugger);
                DrawStats(debugger);
                GUILayout.EndHorizontal();
                GUILayout.Space(10);

                DrawBehaviourTree(debugger);
                GUILayout.Space(10);

                EditorUtility.SetDirty(debugger); // ensure we are redrawn every frame
            }
        }

        private void DrawStats(Debugger debugger)
        {
            EditorGUILayout.BeginVertical();
            {
                GUILayout.Label("Stats:", EditorStyles.boldLabel);

                Root behaviorTree = debugger.BehaviorTree;

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                {
                    DrawKeyValue("Total Starts:", behaviorTree.TotalNumStartCalls.ToString());
                    DrawKeyValue("Total Stops:", behaviorTree.TotalNumStopCalls.ToString());
                    DrawKeyValue("Total Stopped:", behaviorTree.TotalNumStoppedCalls.ToString());
                    DrawKeyValue("Active Timers:  ", behaviorTree.Clock.NumTimers.ToString());
                    DrawKeyValue("Timer Pool Size:  ", behaviorTree.Clock.DebugPoolSize.ToString());
                    DrawKeyValue("Active Update Observers:  ", behaviorTree.Clock.NumUpdateObservers.ToString());
                    DrawKeyValue("Active Blackboard Observers:  ", behaviorTree.Blackboard.NumObservers.ToString());
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawBlackboardKeyAndValues(Debugger debugger)
        {
            EditorGUILayout.BeginVertical();
            {
                GUILayout.Label("Blackboard:", EditorStyles.boldLabel);

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                {
                    Blackboard blackboard = debugger.BehaviorTree.Blackboard;
                    List<string> keys = blackboard.Keys;
                    foreach (string key in keys)
                    {
                        DrawKeyValue(key, blackboard.Get(key).ToString());
                    }
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawKeyValue(string key, string value)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(key, smallTextStyle);
            GUILayout.FlexibleSpace();
            GUILayout.Label(value, smallTextStyle);
            EditorGUILayout.EndHorizontal();
        }

        private void DrawBehaviourTree(Debugger debugger)
        {
            EditorGUILayout.BeginVertical();
            {
                GUILayout.Label("Behaviour Tree:", EditorStyles.boldLabel);

                EditorGUILayout.BeginVertical(nestedBoxStyle);
                DrawNodeTree(debugger.BehaviorTree, 0);
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawNodeTree(Node node, int depth = 0, bool firstNode = true, float lastYPos = 0f)
        {
            GUI.color = (node.CurrentState == Node.State.ACTIVE) ? new Color(1f, 1f, 1f, 1f) : new Color(1f, 1f, 1f, 0.3f);

            DrawNode(node, depth);

            Rect rect = GUILayoutUtility.GetLastRect();

            // Set intial line position
            if (firstNode) lastYPos = rect.yMin;

            // Draw the lines
            Handles.BeginGUI();
            Handles.color = (node.CurrentState == Node.State.ACTIVE) ? new Color(0f, 0f, 0f, 1f) : new Color(0f, 0f, 0f, 0.15f);
            Handles.DrawLine(new Vector2(rect.xMin + 3, lastYPos + 3), new Vector2(rect.xMin + 3, rect.yMax - 5));
            Handles.EndGUI();

            depth++;

            if (node is Container)
            {
                GUI.color = (node.CurrentState == Node.State.ACTIVE) ? new Color(1f,1f,1f,1f) : new Color(1f, 1f, 1f, 0.3f);

                EditorGUILayout.BeginVertical(nestedBoxStyle);
                
                Node[] children = (node as Container).DebugChildren;
                if (children == null)
                {
                    GUILayout.Label("CHILDREN ARE NULL");
                }
                else
                {
                    lastYPos = rect.yMin + 10; // Set new Line position

                    for (int i = 0; i < children.Length; i++)
                    {
                        DrawNodeTree(children[i], depth, i == 0, lastYPos);
                    }
                }

                EditorGUILayout.EndVertical();
            }

        }

        private void DrawNode(Node node, int depth)
        {
            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Label("-" + node.ToString(), nodeTextStyle);


                if (node.CurrentState == Node.State.ACTIVE)
                {
                    if (GUILayout.Button("stop", EditorStyles.miniButton))
                    {
                        node.Stop();
                    }
                }
                else if (node is Root)
                {
                    GUI.color = new Color(1f, 1f, 1f, 1f);
                    if (GUILayout.Button("start", EditorStyles.miniButton))
                    {
                        node.Start();
                    }
                    GUI.color = new Color(1f, 1f, 1f, 0.3f);
                }

                GUILayout.FlexibleSpace();
                GUILayout.Label((node.DebugNumStoppedCalls > 0 ? node.DebugLastResult.ToString() : "") + " | "+ node.DebugNumStartCalls + " , " + node.DebugNumStopCalls + " , " + node.DebugNumStoppedCalls, smallTextStyle);
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}
