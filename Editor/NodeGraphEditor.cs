using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace BNGNodeEditor {
    /// <summary> Base class to derive custom Node Graph editors from. Use this to override how graphs are drawn in the editor. </summary>
    [CustomNodeGraphEditor(typeof(BNGNode.NodeGraph))]
    public class NodeGraphEditor : BNGNodeEditor.Internal.NodeEditorBase<NodeGraphEditor, NodeGraphEditor.CustomNodeGraphEditorAttribute, BNGNode.NodeGraph> {
        [Obsolete("Use window.position instead")]
        public Rect position { get { return window.position; } set { window.position = value; } }
        /// <summary> Are we currently renaming a node? </summary>
        protected bool isRenaming;

        public virtual void OnGUI() { }

        /// <summary> Called when opened by NodeEditorWindow </summary>
        public virtual void OnOpen() { }
        
        /// <summary> Called when NodeEditorWindow gains focus </summary>
        public virtual void OnWindowFocus() { }

        /// <summary> Called when NodeEditorWindow loses focus </summary>
        public virtual void OnWindowFocusLost() { }

        public virtual Texture2D GetGridTexture() {
            return NodeEditorPreferences.GetSettings().gridTexture;
        }

        public virtual Texture2D GetSecondaryGridTexture() {
            return NodeEditorPreferences.GetSettings().crossTexture;
        }

        /// <summary> Return default settings for this graph type. This is the settings the user will load if no previous settings have been saved. </summary>
        public virtual NodeEditorPreferences.Settings GetDefaultPreferences() {
            return new NodeEditorPreferences.Settings();
        }

        /// <summary> Returns context node menu path. Null or empty strings for hidden nodes. </summary>
        public virtual string GetNodeMenuName(Type type) {
            //Check if type has the CreateNodeMenuAttribute
            BNGNode.Node.CreateNodeMenuAttribute attrib;
            if (NodeEditorUtilities.GetAttrib(type, out attrib)) // Return custom path
                return attrib.menuName;
            else // Return generated path
                return NodeEditorUtilities.NodeDefaultPath(type);
        }

        /// <summary> The order by which the menu items are displayed. </summary>
        public virtual int GetNodeMenuOrder(Type type) {
            //Check if type has the CreateNodeMenuAttribute
            BNGNode.Node.CreateNodeMenuAttribute attrib;
            if (NodeEditorUtilities.GetAttrib(type, out attrib)) // Return custom path
                return attrib.order;
            else
                return 0;
        }

        public virtual void OpenNewNodeMenu(Vector2 Position = new Vector2())
        {
            Vector2 pos = NodeEditorWindow.current.WindowToGridPosition(Position);
            var nodeTypes = NodeEditorReflection.nodeTypes.OrderBy(type => GetNodeMenuOrder(type)).ToArray();
            bool addNode = true;

            bool isInput = false;
            bool isOutput = false;
            BNGNode.Node[] gNodes = target.nodes.ToArray();
            for (int t = 0; t < gNodes.Length; t++)
            {
                if (gNodes[t].GetType() == typeof(MaterialNodesGraph.MaterialInput))
                    isInput = true;
                if (gNodes[t].GetType() == typeof(MaterialNodesGraph.MaterialOutput))
                    isOutput = true;
            }
            string[] enumNames = new string[nodeTypes.Length];

            for (int i = 0; i < nodeTypes.Length; i++)
            {
                Type type = nodeTypes[i];
                addNode = true;

                //Get node context menu path
                string path = GetNodeMenuName(type);
                if (path == "Material Nodes Graph/Material Input" && isInput) addNode = false;
                if (path == "Material Nodes Graph/Material Output" && isOutput) addNode = false;
                if (!NodeEditorPreferences.GetSettings().dMode)
                    if (path == "Color/RGBCurves (Beta)") addNode = false;
                if (string.IsNullOrEmpty(path)) continue;

                // Check if user is allowed to add more of given node type

                // Add node entry to context menu
                if (path == "Blender Nodes Graph/Material Input") path = "Material Input";
                if (path == "Blender Nodes Graph/Material Output") path = "Material Output";
                //if (disallowed) menu.AddItem(new GUIContent(path), false, null);
                if (addNode) enumNames[i] = path;
                /*if (addNode) menu.AddItem(new GUIContent(path), false, () =>
                {
                    XNode.Node node = CreateNode(type, pos);
                    NodeEditorWindow.current.AutoConnect(node);
                });*/
            }
            NewNodePopup nodePopup = new NewNodePopup(new Vector2(200, 260), enumNames);
            nodePopup.OnCloseEvent += () =>
            {
                if (nodePopup.EnumValue != -1)
                {
                    Type type = nodeTypes[nodePopup.EnumValue];
                    BNGNode.Node node = CreateNode(type, pos);
                    Selection.activeObject = null;
                    Selection.activeObject = node;
                    (NodeEditorWindow.current.graph as MaterialNodesGraph.MaterialNodeGraph).SetSubAssetHideFlags(HideFlags.HideInHierarchy);
                    NodeEditorWindow.current.AutoConnect(node);
                }
            };
            PopupWindow.Show(new Rect(Position, Vector2.zero), nodePopup);
        }

        /// <summary> Add items for the context menu when right-clicking this node. Override to add custom menu items. </summary>
        public virtual void AddContextMenuItems(GenericMenu menu) {
            Vector2 pos = NodeEditorWindow.current.WindowToGridPosition(Event.current.mousePosition);
            var nodeTypes = NodeEditorReflection.nodeTypes.OrderBy(type => GetNodeMenuOrder(type)).ToArray();
            for (int i = 0; i < nodeTypes.Length; i++) {
                Type type = nodeTypes[i];

                //Get node context menu path
                string path = GetNodeMenuName(type);
                if (string.IsNullOrEmpty(path)) continue;

                // Check if user is allowed to add more of given node type
                BNGNode.Node.DisallowMultipleNodesAttribute disallowAttrib;
                bool disallowed = false;
                if (NodeEditorUtilities.GetAttrib(type, out disallowAttrib)) {
                    int typeCount = target.nodes.Count(x => x.GetType() == type);
                    if (typeCount >= disallowAttrib.max) disallowed = true;
                }

                // Add node entry to context menu
                if (disallowed) menu.AddItem(new GUIContent(path), false, null);
                else menu.AddItem(new GUIContent(path), false, () => {
                    BNGNode.Node node = CreateNode(type, pos);
                    NodeEditorWindow.current.AutoConnect(node);
                });
            }
            menu.AddSeparator("");
            if (NodeEditorWindow.copyBuffer != null && NodeEditorWindow.copyBuffer.Length > 0) menu.AddItem(new GUIContent("Paste"), false, () => NodeEditorWindow.current.PasteNodes(pos));
            else menu.AddDisabledItem(new GUIContent("Paste"));
            menu.AddItem(new GUIContent("Preferences"), false, () => NodeEditorReflection.OpenPreferences());
            menu.AddCustomContextMenuItems(target);
        }

        /// <summary> Returned gradient is used to color noodles </summary>
        /// <param name="output"> The output this noodle comes from. Never null. </param>
        /// <param name="input"> The output this noodle comes from. Can be null if we are dragging the noodle. </param>
        public virtual Gradient GetNoodleGradient(BNGNode.NodePort output, BNGNode.NodePort input) {
            Gradient grad = new Gradient();

            // If dragging the noodle, draw solid, slightly transparent
            if (input == null) {
                Color a = GetTypeColor(output.nodePortType);
                grad.SetKeys(
                    new GradientColorKey[] { new GradientColorKey(a, 0f) },
                    new GradientAlphaKey[] { new GradientAlphaKey(0.6f, 0f) }
                );
            }
            // If normal, draw gradient fading from one input color to the other
            else {
                Color a = GetTypeColor(output.nodePortType);
                Color b = GetTypeColor(input.nodePortType);
                // If any port is hovered, tint white
                if (window.hoveredPort == output || window.hoveredPort == input) {
                    a = Color.Lerp(a, Color.white, 0.8f);
                    b = Color.Lerp(b, Color.white, 0.8f);
                }
                grad.SetKeys(
                    new GradientColorKey[] { new GradientColorKey(a, 0f), new GradientColorKey(b, 1f) },
                    new GradientAlphaKey[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(1f, 1f) }
                );
            }
            return grad;
        }

        /// <summary> Returned float is used for noodle thickness </summary>
        /// <param name="output"> The output this noodle comes from. Never null. </param>
        /// <param name="input"> The output this noodle comes from. Can be null if we are dragging the noodle. </param>
        public virtual float GetNoodleThickness(BNGNode.NodePort output, BNGNode.NodePort input) {
            return 5f;
        }

        public virtual NoodlePath GetNoodlePath(BNGNode.NodePort output, BNGNode.NodePort input) {
            return NodeEditorPreferences.GetSettings().noodlePath;
        }

        public virtual NoodleStroke GetNoodleStroke(BNGNode.NodePort output, BNGNode.NodePort input) {
            return NodeEditorPreferences.GetSettings().noodleStroke;
        }

        /// <summary> Returned color is used to color ports </summary>
        public virtual Color GetPortColor(BNGNode.NodePort port) {
            return GetTypeColor(port.nodePortType);
        }

        /// <summary> Returns generated color for a type. This color is editable in preferences </summary>
        public virtual Color GetTypeColor(string type) {
            Color newCol = Color.white;
            if (!NodeEditorPreferences.GetSettings().useOneColor)
            {
                if (type == "float")
                    newCol = Color.gray;
                if (type == "vector2")
                    newCol = new Color(0, 0.6f, 0.1f, 1);//green
                if (type == "vector3")
                    newCol = new Color(0.35f, 0.35f, 0.75f, 1);//blue
                if (type == "vector4")
                    newCol = new Color(0.7f, 0.7f, 0.03f, 1);//yellow
                if (type == "Texture2D")
                    newCol = new Color(0.8f, 0.3f, 0.3f, 1);
            }
            else
                newCol = Color.gray;
            return newCol;
        }

        /// <summary> Override to display custom tooltips </summary>
        public virtual string GetPortTooltip(BNGNode.NodePort port) {
            string portType = port.nodePortType;
            string tooltip = "";
            tooltip = portType;//.PrettyName();
            /*if (port.IsOutput) {
                object obj = port.node.GetValue(port);
                tooltip += " = " + (obj != null ? obj.ToString() : "null");
            }*/
            return tooltip;
        }

        /// <summary> Deal with objects dropped into the graph through DragAndDrop </summary>
        public virtual void OnDropObjects(UnityEngine.Object[] objects) {
            if(objects.Length == 1)
            {
                //Debug.Log(objects[0].GetType());
                if (!NodeEditorWindow.current.IsHoveringNode)
                {
                    if (objects[0].GetType() == typeof(Texture2D) && !NodeEditorWindow.current.IsHoveringNode)
                    {
                        BNGNode.Node textureNode = CreateNode(typeof(MaterialNodesGraph.ImageTextureNode), NodeEditorWindow.current.WindowToGridPosition(Event.current.mousePosition));
                        (textureNode as MaterialNodesGraph.ImageTextureNode).ImageTexture = objects[0] as Texture2D;
                        Selection.activeObject = null;
                        (NodeEditorWindow.current.graph as MaterialNodesGraph.MaterialNodeGraph).SetSubAssetHideFlags(HideFlags.HideInHierarchy);
                        Selection.activeObject = textureNode;
                        NodeEditorWindow.current.Save();
                    }
                    if (objects[0].GetType() == typeof(TextAsset) && Path.GetExtension(AssetDatabase.GetAssetPath(objects[0])) == ".json")
                    {
                        Vector2 pos = NodeEditorWindow.current.WindowToGridPosition(Event.current.mousePosition);
                        NodeJSONImporter.ImportJson(this, true, AssetDatabase.GetAssetPath(objects[0]), pos);
                        NodeEditorWindow.current.Save();
                    }
                    if (objects[0].GetType() == typeof(TextAsset) && Path.GetExtension(AssetDatabase.GetAssetPath(objects[0])) == ".hlsl")
                    {
                        BNGNode.Node customNode = CreateNode(typeof(MaterialNodesGraph.CustomFunctionNode), NodeEditorWindow.current.WindowToGridPosition(Event.current.mousePosition));
                        (customNode as MaterialNodesGraph.CustomFunctionNode).SourceFile = objects[0] as TextAsset;
                        Selection.activeObject = null;
                        (NodeEditorWindow.current.graph as MaterialNodesGraph.MaterialNodeGraph).SetSubAssetHideFlags(HideFlags.HideInHierarchy);
                        Selection.activeObject = customNode;
                        NodeEditorWindow.current.Save();
                    }
                }
                else if (NodeEditorWindow.current.hoveredNode.GetType() == typeof(MaterialNodesGraph.ImageTextureNode))
                {
                    BNGNode.Node textureNode = NodeEditorWindow.current.hoveredNode;
                    if (objects[0].GetType() == typeof(Texture2D))
                    {
                        Undo.RecordObject(textureNode, "Texture Change");
                        (textureNode as MaterialNodesGraph.ImageTextureNode).ImageTexture = objects[0] as Texture2D;
                        NodeEditorWindow.current.Save();
                    }
                }
                else if (NodeEditorWindow.current.hoveredNode.GetType() == typeof(MaterialNodesGraph.CustomFunctionNode))
                {
                    BNGNode.Node customNode = NodeEditorWindow.current.hoveredNode;
                    if (objects[0].GetType() == typeof(TextAsset) && Path.GetExtension(AssetDatabase.GetAssetPath(objects[0])) == ".hlsl")
                    {
                        Undo.RecordObject(customNode, "Text Asset Change");
                        (customNode as MaterialNodesGraph.CustomFunctionNode).SourceFile = objects[0] as TextAsset;
                        NodeEditorWindow.current.Save();
                    }
                }
            }
            //if (GetType() != typeof(NodeGraphEditor)) Debug.Log("No OnDropObjects override defined for " + GetType());
        }

        /// <summary> Create a node and save it in the graph asset </summary>
        public virtual BNGNode.Node CreateNode(Type type, Vector2 position) {
            Undo.RecordObject(target, "Create Node");
            BNGNode.Node node = target.AddNode(type);
            Undo.RegisterCreatedObjectUndo(node, "Create Node");
            node.position = position;
            if (node.name == null || node.name.Trim() == "") node.name = NodeEditorUtilities.NodeDefaultName(type);
            if (!string.IsNullOrEmpty(AssetDatabase.GetAssetPath(target))) AssetDatabase.AddObjectToAsset(node, target);
            if (NodeEditorPreferences.GetSettings().autoSave) AssetDatabase.SaveAssets();
            NodeEditorWindow.RepaintAll();
            return node;
        }

        /// <summary> Creates a copy of the original node in the graph </summary>
        public virtual BNGNode.Node CopyNode(BNGNode.Node original) {
            Undo.RecordObject(target, "Duplicate Node");
            BNGNode.Node node = target.CopyNode(original);
            Undo.RegisterCreatedObjectUndo(node, "Duplicate Node");
            node.name = original.name;
            AssetDatabase.AddObjectToAsset(node, target);
            if (NodeEditorPreferences.GetSettings().autoSave) AssetDatabase.SaveAssets();
            return node;
        }

        /// <summary> Return false for nodes that can't be removed </summary>
        public virtual bool CanRemove(BNGNode.Node node) {
            // Check graph attributes to see if this node is required
            Type graphType = target.GetType();
            BNGNode.NodeGraph.RequireNodeAttribute[] attribs = Array.ConvertAll(
                graphType.GetCustomAttributes(typeof(BNGNode.NodeGraph.RequireNodeAttribute), true), x => x as BNGNode.NodeGraph.RequireNodeAttribute);
            if (attribs.Any(x => x.Requires(node.GetType()))) {
                if (target.nodes.Count(x => x.GetType() == node.GetType()) <= 1) {
                    return false;
                }
            }
            return true;
        }

        /// <summary> Safely remove a node and all its connections. </summary>
        public virtual void RemoveNode(BNGNode.Node node) {
            if (!CanRemove(node)) return;

            // Remove the node
            Undo.RecordObject(node, "Delete Node");
            Undo.RecordObject(target, "Delete Node");
            foreach (var port in node.Ports)
                foreach (var conn in port.GetConnections())
                    Undo.RecordObject(conn.node, "Delete Node");
            target.RemoveNode(node);
            Undo.DestroyObjectImmediate(node);
            if (NodeEditorPreferences.GetSettings().autoSave) AssetDatabase.SaveAssets();
        }

        [AttributeUsage(AttributeTargets.Class)]
        public class CustomNodeGraphEditorAttribute : Attribute,
        BNGNodeEditor.Internal.NodeEditorBase<NodeGraphEditor, NodeGraphEditor.CustomNodeGraphEditorAttribute, BNGNode.NodeGraph>.INodeEditorAttrib {
            private Type inspectedType;
            public string editorPrefsKey;
            /// <summary> Tells a NodeGraphEditor which Graph type it is an editor for </summary>
            /// <param name="inspectedType">Type that this editor can edit</param>
            /// <param name="editorPrefsKey">Define unique key for unique layout settings instance</param>
            public CustomNodeGraphEditorAttribute(Type inspectedType, string editorPrefsKey = "xNode.Settings") {
                this.inspectedType = inspectedType;
                this.editorPrefsKey = editorPrefsKey;
            }

            public Type GetInspectedType() {
                return inspectedType;
            }
        }
    }
}
