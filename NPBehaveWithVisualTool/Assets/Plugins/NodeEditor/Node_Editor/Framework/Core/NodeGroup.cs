﻿using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using NodeEditorFramework.Utilities;
using Sirenix.OdinInspector;

namespace NodeEditorFramework
{
    /// <summary>
    /// A NodeGroup on the canvas that handles node and subgroup pinning and syncing along with functionality to manipulate and customize the group
    /// </summary>
    [Serializable]
    public class NodeGroup
    {
        /// <summary>
        /// Represents selected borders as a flag in order to support corners
        /// </summary>
        [Flags]
        enum BorderSelection
        {
            None = 0,
            Left = 1,
            Right = 2,
            Top = 4,
            Bottom = 8
        };

        public Rect rect;
        public string title;

        public Color color
        {
            get
            {
                return _color;
            }
            set
            {
                _color = value;
                GenerateStyles();
            }
        }

        [SerializeField]
        private Color _color = Color.cyan;

        internal bool isClipped;

        [LabelText("顶部大小（高度）")]
        public int headerHeight = 30;

        [LabelText("字体大小")]
        public int TextSize = 25;

        // Resizing and dragging state for active node group
        private static BorderSelection resizeDir;

        [NonSerialized]
        public List<Node> pinnedNodes = new List<Node>();

        [NonSerialized]
        private List<NodeGroup> pinnedGroups = new List<NodeGroup>();

        // Settings
        private static bool headerFree = true;
        private const int borderWidth = 15;
        private const int minGroupSize = 150;

        // Accessors
        private Rect headerRect
        {
            get
            {
                return new Rect(rect.x, rect.y - (headerFree? headerHeight : 0), rect.width, headerHeight);
            }
        }

        private Rect bodyRect
        {
            get
            {
                return new Rect(rect.x, rect.y + (headerFree? 0 : headerHeight), rect.width, rect.height - (headerFree? 0 : headerHeight));
            }
        }

        internal Rect fullAABBRect
        {
            get
            {
                return new Rect(rect.x, rect.y - (headerFree? headerHeight : 0), rect.width, rect.height + (headerFree? headerHeight : 0));
            }
        }

        /// <summary>
        /// Silently creates a NodeGroup
        /// </summary>
        internal NodeGroup()
        {
        }

        /// <summary>
        /// Creates a new NodeGroup with the specified title at pos and adds it to the current canvas
        /// </summary>
        public NodeGroup(string groupTitle, Vector2 pos)
        {
            title = groupTitle;
            rect = new Rect(pos.x, pos.y, 400, 400);
            GenerateStyles();
            NodeEditor.curNodeCanvas.groups.Add(this);
            UpdateGroupOrder();
        }

        /// <summary>
        /// Deletes this NodeGroup
        /// </summary>
        public void Delete()
        {
            NodeEditor.curNodeCanvas.groups.Remove(this);
        }

        #region Group Functionality

        /// <summary>
        /// Update pinned nodes and subGroups of this NodeGroup
        /// </summary>
        private void UpdatePins()
        {
            pinnedGroups = new List<NodeGroup>();
            foreach (NodeGroup group in NodeEditor.curNodeCanvas.groups)
            {
                // Get all pinned groups -> all groups atleast half in the group
                if (group != this && rect.Contains(group.headerRect.center))
                    pinnedGroups.Add(group);
            }

            pinnedNodes = new List<Node>();
            foreach (Node node in NodeEditor.curNodeCanvas.nodes)
            {
                // Get all pinned nodes -> all nodes atleast half in the group
                if (rect.Contains(node.rect.center))
                    pinnedNodes.Add(node);
            }
        }

        /// <summary>
        /// Updates the group order by their sizes for better input handling
        /// </summary>
        private static void UpdateGroupOrder()
        {
            foreach (NodeGroup group in NodeEditor.curNodeCanvas.groups)
                group.UpdatePins();
            //NodeEditor.curNodeCanvas.groups.Sort((x, y) => -x.pinnedGroups.Count.CompareTo(y.pinnedGroups.Count));
            NodeEditor.curNodeCanvas.groups = NodeEditor.curNodeCanvas.groups
                    .OrderByDescending((x) => x.pinnedGroups.Count) // Order by pin hierarchy level
                    .ThenByDescending((x) => x.rect.size.x * x.rect.size.y) // Incase of equal level, prefer smaller groups
                    .ToList();
        }

        #endregion

        #region GUI

        [NonSerialized]
        private GUIStyle backgroundStyle;

        [NonSerialized]
        private GUIStyle altBackgroundStyle;

        [NonSerialized]
        private GUIStyle opBackgroundStyle;

        //		[NonSerialized]
        //		private GUIStyle dragHandleStyle;
        [NonSerialized]
        private GUIStyle headerTitleStyle;

        [NonSerialized]
        private GUIStyle headerTitleEditStyle;

        /// <summary>
        /// Generates all the styles for this node group based of the color
        /// </summary>
        private void GenerateStyles()
        {
            // Transparent background
            Texture2D background = RTEditorGUI.ColorToTex(8, color * new Color(1, 1, 1, 0.4f));
            // lighter, less transparent background
            Texture2D altBackground = RTEditorGUI.ColorToTex(8, color * new Color(1, 1, 1, 0.6f));
            // nearly opaque background
            Texture2D opBackground = RTEditorGUI.ColorToTex(8, color * new Color(1, 1, 1, 0.9f));

            backgroundStyle = new GUIStyle();
            backgroundStyle.normal.background = background;
            backgroundStyle.padding = new RectOffset(10, 10, 5, 5);

            altBackgroundStyle = new GUIStyle();
            altBackgroundStyle.normal.background = altBackground;
            altBackgroundStyle.padding = new RectOffset(10, 10, 10, 10);

            opBackgroundStyle = new GUIStyle();
            opBackgroundStyle.normal.background = opBackground;
            opBackgroundStyle.padding = new RectOffset(10, 10, 5, 5);

            //			dragHandleStyle = new GUIStyle ();
            //			dragHandleStyle.normal.background = background;
            //			//dragHandleStyle.hover.background = altBackground;
            //			dragHandleStyle.padding = new RectOffset (10, 10, 5, 5);

            headerTitleStyle = new GUIStyle();
            headerTitleStyle.fontSize = this.TextSize;
            headerTitleStyle.normal.textColor = Color.white;

            headerTitleEditStyle = new GUIStyle(headerTitleStyle);
            headerTitleEditStyle.normal.background = background;
            headerTitleEditStyle.focused.background = background;
            headerTitleEditStyle.focused.textColor = Color.white;
        }

        /// <summary>
        /// Draws the NodeGroup
        /// </summary>
        public void DrawGroup()
        {
            if (backgroundStyle == null)
                GenerateStyles();
            NodeEditorState state = NodeEditor.curEditorState;
            // Create a rect that is adjusted to the editor zoom
            Rect groupRect = rect;
            groupRect.position += state.zoomPanAdjust + state.panOffset;

            // Resize handles
            //Rect leftSideRect = new Rect(groupRect.x, groupRect.y, borderWidth, groupRect.height);
            //Rect rightSideRect = new Rect(groupRect.x + groupRect.width - borderWidth, groupRect.y, borderWidth, groupRect.height);
            //Rect topSideRect = new Rect(groupRect.x, groupRect.y, groupRect.width, borderWidth);
            //Rect bottomSideRect = new Rect(groupRect.x, groupRect.y + groupRect.height - borderWidth, groupRect.width, borderWidth);

            //GUI.Box(leftSideRect, GUIContent.none, dragHandleStyle);
            //GUI.Box(rightSideRect, GUIContent.none, dragHandleStyle);
            //GUI.Box(topSideRect, GUIContent.none, dragHandleStyle);
            //GUI.Box(bottomSideRect, GUIContent.none, dragHandleStyle);

            if (state.activeGroup == this && state.resizeGroup)
            {
                // Highlight the currently resized border
                Rect handleRect = getBorderRect(rect, NodeGroup.resizeDir);
                handleRect.position += state.zoomPanAdjust + state.panOffset;
                GUI.Box(handleRect, GUIContent.none, opBackgroundStyle);
            }

            // Body
            Rect groupBodyRect = bodyRect;
            groupBodyRect.position += state.zoomPanAdjust + state.panOffset;
            GUI.Box(groupBodyRect, GUIContent.none, backgroundStyle);

            // Header
            Rect groupHeaderRect = headerRect;
            groupHeaderRect.position += state.zoomPanAdjust + state.panOffset + new Vector2(0, -10);

            GUILayout.BeginArea(groupHeaderRect, headerFree? GUIStyle.none : altBackgroundStyle);
            GUILayout.BeginHorizontal();

            GUILayout.Space(8);

            // Header Title

            title = GUILayout.TextField(title, headerTitleEditStyle, GUILayout.MinWidth(40));

            GUILayout.Space(10);

            // Header Color Edit
            color = this._color;

            GUILayout.FlexibleSpace();

            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }

        #endregion

        #region Helpers and Hit Tests

        /// <summary>
        /// Gets a NodeGroup which has it's header under the mouse. If multiple groups are adressed, the smallest is returned.
        /// </summary>
        private static NodeGroup HeaderAtPosition(NodeEditorState state, Vector2 canvasPos)
        {
            if (NodeEditorInputSystem.shouldIgnoreInput(state))
                return null;
            for (int i = state.canvas.groups.Count - 1; i >= 0; i--)
            {
                NodeGroup group = state.canvas.groups[i];
                if (group.headerRect.Contains(canvasPos))
                    return group;
            }

            return null;
        }

        /// <summary>
        /// Gets a NodeGroup under the mouse. If multiple groups are adressed, the group lowest in the pin hierarchy is returned.
        /// </summary>
        private static NodeGroup GroupAtPosition(NodeEditorState state, Vector2 canvasPos)
        {
            if (NodeEditorInputSystem.shouldIgnoreInput(state))
                return null;
            for (int i = state.canvas.groups.Count - 1; i >= 0; i--)
            {
                NodeGroup group = state.canvas.groups[i];
                if (group.headerRect.Contains(canvasPos) || group.rect.Contains(canvasPos))
                    return group;
            }

            return null;
        }

        /// <summary>
        /// Gets a NodeGroup under the mouse that requires input (header or border). If multiple groups are adressed, the group lowest in the pin hierarchy is returned.
        /// </summary>
        private static NodeGroup GroupAtPositionInput(NodeEditorState state, Vector2 canvasPos)
        {
            if (NodeEditorInputSystem.shouldIgnoreInput(state))
                return null;
            for (int i = state.canvas.groups.Count - 1; i >= 0; i--)
            {
                NodeGroup group = state.canvas.groups[i];
                BorderSelection border;
                if (group.headerRect.Contains(canvasPos) || CheckBorderSelection(state, group.rect, canvasPos, out border))
                    return group;
            }

            return null;
        }

        /// <summary>
        /// Returns true if the mouse position is on the border of the focused node and outputs the border as a flag in selection
        /// </summary>
        private static bool CheckBorderSelection(NodeEditorState state, Rect rect, Vector2 canvasPos, out BorderSelection selection)
        {
            selection = 0;
            if (!rect.Contains(canvasPos))
                return false;

            Vector2 min = new Vector2(rect.xMin + borderWidth, rect.yMax - borderWidth);
            Vector2 max = new Vector2(rect.xMax - borderWidth, rect.yMin + borderWidth);

            // Check bordes and mark flags accordingly
            if (canvasPos.x < min.x)
                selection = BorderSelection.Left;
            else if (canvasPos.x > max.x)
                selection = BorderSelection.Right;

            if (canvasPos.y < max.y)
                selection |= BorderSelection.Top;
            else if (canvasPos.y > min.y)
                selection |= BorderSelection.Bottom;

            return selection != BorderSelection.None;
        }

        /// <summary>
        /// Gets the rect that represents the passed border flag in the passed rect
        /// </summary>
        private static Rect getBorderRect(Rect rect, BorderSelection border)
        {
            Rect borderRect = rect;
            if ((border & BorderSelection.Left) != 0)
                borderRect.xMax = borderRect.xMin + borderWidth;
            else if ((border & BorderSelection.Right) != 0)
                borderRect.xMin = borderRect.xMax - borderWidth;

            if ((border & BorderSelection.Top) != 0)
                borderRect.yMax = borderRect.yMin + borderWidth;
            else if ((border & BorderSelection.Bottom) != 0)
                borderRect.yMin = borderRect.yMax - borderWidth;
            return borderRect;
        }

        #endregion

        #region Input

        /// <summary>
        /// Handles creation of the group in the editor through a context menu item
        /// </summary>
        [ContextEntryAttribute(ContextType.Canvas, "Create Group")]
        private static void CreateGroup(NodeEditorInputInfo info)
        {
            info.SetAsCurrentEnvironment();
            new NodeGroup("Group", NodeEditor.ScreenToCanvasSpace(info.inputPos));
        }

        /// <summary>
        /// Handles the group context click (on the header only)
        /// </summary>
        [EventHandlerAttribute(EventType.MouseDown, -1)] // Before the other context clicks because they won't account for groups
        private static void HandleGroupContextClick(NodeEditorInputInfo inputInfo)
        {
            NodeEditorState state = inputInfo.editorState;
            if (inputInfo.inputEvent.button == 1 && state.focusedNode == null)
            {
                // Right-click NOT on a node
                NodeGroup focusedGroup = HeaderAtPosition(state, NodeEditor.ScreenToCanvasSpace(inputInfo.inputPos));
                if (focusedGroup != null)
                {
                    // Context click for the group. This is static, not dynamic, because it would be useless
                    GenericMenu context = new GenericMenu();
                    context.AddItem(new GUIContent("Delete"), false, () =>
                    {
                        NodeEditor.curNodeCanvas = state.canvas;
                        focusedGroup.Delete();
                    });
                    context.ShowAsContext();
                    inputInfo.inputEvent.Use();
                }
            }
        }

        /// <summary>
        /// Starts a dragging operation for either dragging or resizing (on the header or borders only)
        /// </summary>
        [EventHandlerAttribute(EventType.MouseDown,
            104)] // Priority over hundred to make it call after the GUI, and before Node dragging (110) and window panning (105)
        private static void HandleGroupDraggingStart(NodeEditorInputInfo inputInfo)
        {
            if (GUIUtility.hotControl > 0)
                return; // GUI has control

            NodeEditorState state = inputInfo.editorState;
            if (inputInfo.inputEvent.button == 0 && state.focusedNode == null && state.dragNode == false)
            {
                // Do not interfere with other dragging stuff
                NodeGroup focusedGroup = GroupAtPositionInput(state, NodeEditor.ScreenToCanvasSpace(inputInfo.inputPos));
                if (focusedGroup != null)
                {
                    // Start dragging the focused group
                    UpdateGroupOrder();
                    Vector2 canvasInputPos = NodeEditor.ScreenToCanvasSpace(inputInfo.inputPos);
                    if (CheckBorderSelection(state, focusedGroup.rect, canvasInputPos, out NodeGroup.resizeDir))
                    {
                        // Resizing
                        state.activeGroup = focusedGroup;
                        // Get start drag position
                        Vector2 startSizePos = Vector2.zero;
                        if ((NodeGroup.resizeDir & BorderSelection.Left) != 0)
                            startSizePos.x = focusedGroup.rect.xMin;
                        else if ((NodeGroup.resizeDir & BorderSelection.Right) != 0)
                            startSizePos.x = focusedGroup.rect.xMax;
                        if ((NodeGroup.resizeDir & BorderSelection.Top) != 0)
                            startSizePos.y = focusedGroup.rect.yMin;
                        else if ((NodeGroup.resizeDir & BorderSelection.Bottom) != 0)
                            startSizePos.y = focusedGroup.rect.yMax;
                        // Start the resize drag
                        state.StartDrag("group", inputInfo.inputPos, startSizePos);
                        state.resizeGroup = true;
                        inputInfo.inputEvent.Use();
                    }
                    else if (focusedGroup.headerRect.Contains(canvasInputPos))
                    {
                        // Dragging
                        state.activeGroup = focusedGroup;
                        state.StartDrag("group", inputInfo.inputPos, state.activeGroup.rect.position);
                        state.activeGroup.UpdatePins();
                        inputInfo.inputEvent.Use();
                    }
                }
            }
        }

        /// <summary>
        /// Updates the dragging operation for either dragging or resizing
        /// </summary>
        [EventHandlerAttribute(EventType.MouseDrag)]
        private static void HandleGroupDragging(NodeEditorInputInfo inputInfo)
        {
            NodeEditorState state = inputInfo.editorState;
            NodeGroup active = state.activeGroup;
            if (active != null)
            {
                // Handle dragging and resizing of active group
                if (state.dragUserID != "group")
                {
                    state.activeGroup = null;
                    state.resizeGroup = false;
                    return;
                }

                // Update drag operation
                Vector2 dragChange = state.UpdateDrag("group", inputInfo.inputPos);
                Vector2 newSizePos = state.dragObjectPos;
                if (state.resizeGroup)
                {
                    // Resizing -> Apply drag to selected borders while keeping a minimum size
                    // Note: Binary operator and !=0 checks of the flag is enabled, but not necessarily the only flag (in which case you would use ==)
                    Rect r = active.rect;
                    if ((NodeGroup.resizeDir & BorderSelection.Left) != 0)
                        active.rect.xMin = r.xMax - Math.Max(minGroupSize, r.xMax - newSizePos.x);
                    else if ((NodeGroup.resizeDir & BorderSelection.Right) != 0)
                        active.rect.xMax = r.xMin + Math.Max(minGroupSize, newSizePos.x - r.xMin);

                    if ((NodeGroup.resizeDir & BorderSelection.Top) != 0)
                        active.rect.yMin = r.yMax - Math.Max(minGroupSize, r.yMax - newSizePos.y);
                    else if ((NodeGroup.resizeDir & BorderSelection.Bottom) != 0)
                        active.rect.yMax = r.yMin + Math.Max(minGroupSize, newSizePos.y - r.yMin);
                }
                else
                {
                    // Dragging -> Apply drag to body and pinned nodes
                    active.rect.position = newSizePos;
                    foreach (Node pinnedNode in active.pinnedNodes)
                        pinnedNode.position += dragChange;
                    foreach (NodeGroup pinnedGroup in active.pinnedGroups)
                        pinnedGroup.rect.position += dragChange;
                }

                inputInfo.inputEvent.Use();
                NodeEditor.RepaintClients();
            }
        }

        /// <summary>
        /// Ends the dragging operation for either dragging or resizing
        /// </summary>
        [EventHandlerAttribute(EventType.MouseDown)]
        [EventHandlerAttribute(EventType.MouseUp)]
        private static void HandleDraggingEnd(NodeEditorInputInfo inputInfo)
        {
            if (inputInfo.editorState.dragUserID == "group")
            {
                //				if (inputInfo.editorState.activeGroup != null )
                //					inputInfo.editorState.activeGroup.UpdatePins ();
                inputInfo.editorState.EndDrag("group");
                NodeEditor.RepaintClients();
            }

            UpdateGroupOrder();
            inputInfo.editorState.activeGroup = null;
            inputInfo.editorState.resizeGroup = false;
        }

        #endregion
    }
}