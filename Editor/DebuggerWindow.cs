
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

        private GUIStyle smallTextStyle, nodeCapsuleGray, nodeCapsuleFailed, nodeCapsuleStopRequested;
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
//            Debug.Log("AWAKE !!");
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

//            nodeTextStyle = new GUIStyle(EditorStyles.label);

            nodeCapsuleGray = (GUIStyle)"helpbox";
            nodeCapsuleGray.normal.textColor = Color.black;

            nodeCapsuleFailed = new GUIStyle(nodeCapsuleGray);
            nodeCapsuleFailed.normal.textColor = Color.red;
            nodeCapsuleStopRequested = new GUIStyle(nodeCapsuleGray);
            nodeCapsuleStopRequested.normal.textColor = new Color(0.7f, 0.7f, 0.0f);

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

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            GUILayout.BeginHorizontal();
            DrawBlackboardKeyAndValues("Blackboard:", selectedDebugger.BehaviorTree.Blackboard);
            if (selectedDebugger.CustomStats.Keys.Count > 0)
            {
                DrawBlackboardKeyAndValues("Custom Stats:", selectedDebugger.CustomStats);
            }
            DrawStats(selectedDebugger);
            GUILayout.EndHorizontal();
            GUILayout.Space(10);

            if (Time.timeScale <= 2.0f)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("TimeScale: ");
                Time.timeScale = EditorGUILayout.Slider(Time.timeScale, 0.0f, 2.0f);
                GUILayout.EndHorizontal();
            }

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

        private void DrawBlackboardKeyAndValues(string label, Blackboard blackboard)
        {
            EditorGUILayout.BeginVertical();
            {
                GUILayout.Label(label, EditorStyles.boldLabel);

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                {
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
            bool decorator = node is Decorator && !(node is Root);
            bool parentIsDecorator = (node.ParentNode is Decorator);
            GUI.color = (node.CurrentState == Node.State.ACTIVE) ? new Color(1f, 1f, 1f, 1f) : new Color(1f, 1f, 1f, 0.3f);

            if (!parentIsDecorator)
            {
                DrawSpacing();
            }

            bool drawConnected = !decorator || (decorator && ((Container)node).Collapse);
            DrawNode(node, depth, drawConnected);

            Rect rect = GUILayoutUtility.GetLastRect();

            // Set intial line position
            if (firstNode)
            {
                lastYPos = rect.yMin;
            }

            // Draw the lines
            Handles.BeginGUI();

            // Container collapsing
            Container container = node as Container;
            Rect interactionRect = new Rect(rect);
            interactionRect.width = 100;
            interactionRect.y += 8;
            if (container != null && Event.current.type == EventType.MouseUp && Event.current.button == 0 && interactionRect.Contains(Event.current.mousePosition))
            {
                container.Collapse = !container.Collapse;
                Event.current.Use();
            }

            Handles.color = new Color(0f, 0f, 0f, 1f);
            if (!decorator)
            {
                Handles.DrawLine(new Vector2(rect.xMin - 5, lastYPos + 4), new Vector2(rect.xMin - 5, rect.yMax - 4));
            }
            else
            {
                Handles.DrawLine(new Vector2(rect.xMin - 5, lastYPos + 4), new Vector2(rect.xMin - 5, rect.yMax + 6));
            }
            Handles.EndGUI();

            if(decorator) depth++;

            if (node is Container && !((Container)node).Collapse)
            {
                if(!decorator) EditorGUILayout.BeginVertical(nestedBoxStyle);

                Node[] children = (node as Container).DebugChildren;
                if (children == null)
                {
                    GUILayout.Label("CHILDREN ARE NULL");
                }
                else
                {
                    lastYPos = rect.yMin + 16; // Set new Line position
                    
                    for (int i = 0; i < children.Length; i++)
                    {
                        DrawNodeTree(children[i], depth, i == 0, lastYPos);
                    }
                }

                if(!decorator) EditorGUILayout.EndVertical();
            }

        }

        private void DrawSpacing()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.EndHorizontal();
        }

        private void DrawNode(Node node, int depth, bool connected)
        {
            float tStopRequested = Mathf.Lerp(0.85f, 0.25f, 2.0f * (Time.time - node.DebugLastStopRequestAt));
            float tStopped = Mathf.Lerp(0.85f, 0.25f, 2.0f * (Time.time - node.DebugLastStoppedAt));
            bool inactive = node.CurrentState != Node.State.ACTIVE;
            float alpha = inactive ? Mathf.Max(0.35f, Mathf.Pow(tStopped, 1.5f)) : 1.0f;
            bool failed = (tStopped > 0.25f && tStopped < 1.0f && !node.DebugLastResult && inactive);
            bool stopRequested = (tStopRequested > 0.25f && tStopRequested < 1.0f && inactive);

            EditorGUILayout.BeginHorizontal();
            {
                GUI.color = new Color(1f, 1f, 1f, alpha);

                string tagName;
                GUIStyle tagStyle = stopRequested ? nodeCapsuleStopRequested : (failed ? nodeCapsuleFailed : nodeCapsuleGray);

                bool drawLabel = !string.IsNullOrEmpty(node.Label);
                string label = node.Label;

                if (node is BlackboardCondition)
                {
                    BlackboardCondition nodeBlackboardCond = node as BlackboardCondition;
                    tagName = nodeBlackboardCond.Key + " " + operatorToString[nodeBlackboardCond.Operator] + " " + nodeBlackboardCond.Value;
                    GUI.backgroundColor = new Color(0.9f, 0.9f, 0.6f);
                }
                else
                {
                    if (node is Composite) GUI.backgroundColor = new Color(0.3f, 1f, 0.1f);
                    if (node is Decorator) GUI.backgroundColor = new Color(0.3f, 1f, 1f);
                    if (node is Task) GUI.backgroundColor = new Color(0.5f, 0.1f, 0.5f);
                    if (node is ObservingDecorator) GUI.backgroundColor = new Color(0.9f, 0.9f, 0.6f);

                    nameToTagString.TryGetValue(node.Name, out tagName);
                }

                if ((node is Container) && ((Container)node).Collapse)
                {
                    if (!drawLabel)
                    {
                        drawLabel = true;
                        label = tagName;
                    }
                    tagName = "...";
                    GUI.backgroundColor = new Color(0.4f, 0.4f, 0.4f);
                }

                if (string.IsNullOrEmpty(tagName)) tagName = node.Name;

                if (!drawLabel) {
                    GUILayout.Label(tagName, tagStyle);
                } else {
                    GUILayout.Label("("+tagName+") " + label, tagStyle);
                    // Reset background color
                    GUI.backgroundColor = Color.white;
                }

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
            if (connected)
            {
                Rect rect = GUILayoutUtility.GetLastRect();

                Handles.color = new Color(0f, 0f, 0f, 1f);
                Handles.BeginGUI();
                float midY = 4 + (rect.yMin + rect.yMax) / 2f;
                Handles.DrawLine(new Vector2(rect.xMin - 5, midY), new Vector2(rect.xMin, midY));
                Handles.EndGUI();
            }
        }
    }
}
