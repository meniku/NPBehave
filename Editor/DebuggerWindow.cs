using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace NPBehave
{
    public class DebuggerWindow : EditorWindow
    {
        private const int nestedPadding = 10;

        public static Transform selectedObject;
        public static Debugger selectedDebugger;

        private Vector2 scrollPosition = Vector2.zero;

        private GUIStyle smallTextStyle, nodeTextStyle, nodeCapsuleGray;
        private GUIStyle nestedBoxStyle;

        private Color defaultColor;

        Dictionary<string, string> nameToTagString;
        Dictionary<Operator, string> operatorToString;

        [MenuItem("Window/NPBehave Debugger")]
        public static void ShowWindow()
        {
            DebuggerWindow window = (DebuggerWindow)EditorWindow.GetWindow(typeof(DebuggerWindow), false, "NPBehave Debugger");
            window.Show();
        }

        public void Init()
        {
            Debug.Log("AWAKE !!");
            operatorToString = new Dictionary<Operator, string>();
            operatorToString[Operator.IS_SET] = "?=";
            operatorToString[Operator.IS_NOT_SET] = "?!=";
            operatorToString[Operator.IS_EQUAL] = "==";
            operatorToString[Operator.IS_NOT_EQUAL] = "!=";
            operatorToString[Operator.IS_GREATER_OR_EQUAL] = ">=";
            operatorToString[Operator.IS_GREATER] = ">";
            operatorToString[Operator.IS_SMALLER_OR_EQUAL] = "<=";
            operatorToString[Operator.IS_SMALLER] = "<";
            operatorToString[Operator.ALWAYS_TRUE] = "ALWAYS_TRUE";

            nameToTagString = new Dictionary<string, string>();
            nameToTagString["Selector"] = "?";
            nameToTagString["Sequence"] = "->";
            // To do add more

            nestedBoxStyle = new GUIStyle();
            nestedBoxStyle.margin = new RectOffset(nestedPadding, 0, 0, 0);

            smallTextStyle = new GUIStyle();
            smallTextStyle.font = EditorStyles.miniFont;

            nodeTextStyle = new GUIStyle(EditorStyles.label);

            nodeCapsuleGray = (GUIStyle)"CapsuleButton";
            nodeCapsuleGray.normal.textColor = Color.white;

            defaultColor = EditorGUIUtility.isProSkin ? Color.white : Color.black;
        }

        public void OnSelectionChange()
        {
            selectedObject = Selection.activeTransform;
            if (selectedObject != null) selectedDebugger = selectedObject.GetComponentInChildren<Debugger>();

            Repaint();
        }

        public void OnGUI()
        {
            if (nameToTagString == null) Init(); // Weird recompile bug fix

            GUI.color = defaultColor;
            GUILayout.Toggle(false, "NPBehave Debugger", GUI.skin.FindStyle("LODLevelNotifyText"));
            GUI.color = Color.white;

            if (!Application.isPlaying)
            {
                EditorGUILayout.HelpBox("Cannot use this utility in Editor Mode", MessageType.Info);
                return;
            }

            var newDebugger = (Debugger)EditorGUILayout.ObjectField("Selected Debugger:", selectedDebugger, typeof(Debugger), true);

            if (newDebugger != selectedDebugger)
            {
                selectedDebugger = newDebugger;
                if (newDebugger != null) selectedObject = selectedDebugger.transform;
            }

            if (selectedObject == null)
            {
                EditorGUILayout.HelpBox("Please select an object", MessageType.Info);
                return;
            }

            if (selectedDebugger == null)
            {
                EditorGUILayout.HelpBox("This object does not contain a debugger component", MessageType.Info);
                return;
            }
            else if (selectedDebugger.BehaviorTree == null)
            {
                EditorGUILayout.HelpBox("BehavorTree is null", MessageType.Info);
                return;
            }

            EditorGUILayout.BeginScrollView(scrollPosition);

            GUILayout.BeginHorizontal();
            DrawBlackboardKeyAndValues(selectedDebugger);
            DrawStats(selectedDebugger);
            GUILayout.EndHorizontal();
            GUILayout.Space(10);

            DrawBehaviourTree(selectedDebugger);
            GUILayout.Space(10);

            EditorGUILayout.EndScrollView();

            Repaint();
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
            Handles.DrawLine(new Vector2(rect.xMin - 5, lastYPos), new Vector2(rect.xMin - 5, rect.yMax - 7));
            Handles.EndGUI();

            depth++;

            if (node is Container)
            {
                EditorGUILayout.BeginVertical(nestedBoxStyle);

                Node[] children = (node as Container).DebugChildren;
                if (children == null)
                {
                    GUILayout.Label("CHILDREN ARE NULL");
                }
                else
                {
                    lastYPos = rect.yMin + 12; // Set new Line position

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

                GUI.color = (node.CurrentState == Node.State.ACTIVE) ? new Color(1f, 1f, 1f, 1f) : new Color(1f, 1f, 1f, 0.3f);

                string tagName;
                GUIStyle tagStyle = nodeCapsuleGray;

                if (node is BlackboardCondition)
                {
                    BlackboardCondition nodeBlackboardCond = node as BlackboardCondition;
                    tagName = nodeBlackboardCond.Key + " " + operatorToString[nodeBlackboardCond.Operator] + " " + nodeBlackboardCond.Value;
                    GUI.backgroundColor = new Color(0.9f, 0.9f, 0.6f);
                }
                else
                {
                    if (node is Composite) GUI.backgroundColor = new Color(0.5f, 0.7f, 1f);
                    if (node is Decorator) GUI.backgroundColor = new Color(0.5f, 0.9f, 0.9f);
                    if (node is Task) GUI.backgroundColor = new Color(0.5f, 0.9f, 0.9f);

                    nameToTagString.TryGetValue(node.Name, out tagName);
                }

                if (string.IsNullOrEmpty(tagName)) tagName = node.Name;

                GUILayout.Label(tagName, tagStyle);


                // Reset background color
                GUI.backgroundColor = Color.white;

                // Draw Label
                if (!string.IsNullOrEmpty(node.Label)) GUILayout.Label("   " + node.Label, (GUIStyle)"ChannelStripAttenuationMarkerSquare");

                GUILayout.FlexibleSpace();

                // Draw Buttons
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

                // Draw Stats
                GUILayout.Label((node.DebugNumStoppedCalls > 0 ? node.DebugLastResult.ToString() : "") + " | " + node.DebugNumStartCalls + " , " + node.DebugNumStopCalls + " , " + node.DebugNumStoppedCalls, smallTextStyle);
            }

            EditorGUILayout.EndHorizontal();

            // Draw the lines
            Rect rect = GUILayoutUtility.GetLastRect();
            Handles.color = (node.CurrentState == Node.State.ACTIVE) ? new Color(0f, 0f, 0f, 1f) : new Color(0f, 0f, 0f, 0.3f);
            Handles.BeginGUI();
            float midY = (rect.yMin + rect.yMax) / 2f;
            Handles.DrawLine(new Vector2(rect.xMin - 5, midY), new Vector2(rect.xMin, midY));
            Handles.EndGUI();
        }
    }
}
