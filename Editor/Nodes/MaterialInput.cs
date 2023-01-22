using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using BNGNode;
using BNGNodeEditor;
using System.Linq;
using System.Text.RegularExpressions;

namespace MaterialNodesGraph
{
    [HelpURL("https://github.com/Warwlock/blender-nodes-subgraph/wiki/Using-Material-Input-Node")]
    public class MaterialInput : Node
    {
        [ContextMenu("Help")]
        public void GetHelp()
        {
            Help.BrowseURL("https://github.com/Warwlock/blender-nodes-subgraph/wiki/Using-Material-Input-Node");
        }

        public string portAddName = "name";
        [Output] public float inputvariable_1;

        public override object GetValue(NodePort port)
        {
            //string a = "";
            //value = a;
            if (port.fieldName != "inputvariable1")
                if (port.fieldName.Split('_').Last() == "vector3")
                    return "?float4(" + port.fieldName.Split('_').First() + ", 0)";
                else
                    return "?" + port.fieldName.Split('_').First();
            else
                return null;
        }
    }

    [CustomNodeEditor(typeof(MaterialInput))]
    public class MaterialInputEditor : NodeEditor
    {
        MaterialInput mi;
        List<NodePort> dynamicPortList;
        Rect buttonRect;
        //int portName = 0;
        public override void OnBodyGUI()
        {
            if (mi == null) mi = target as MaterialInput;
            dynamicPortList = mi.DynamicPorts.ToList();
            serializedObject.Update();
            //NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("outputvariable1"));
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("portAddName"), new GUIContent("Port Name"));
            mi.portAddName = Regex.Replace(mi.portAddName, @"^[^a-zA-Z]|[^a-zA-Z0-9]", "");
            GUILayout.BeginHorizontal();
            Undo.RecordObject(mi, "Enum Change");
            if (GUILayout.Button("   Add Port   "))
            {
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("Select Port Type"), false, null);
                menu.AddSeparator("");
                AddItemForDynamicPortAdder(menu, "Float", "float");
                AddItemForDynamicPortAdder(menu, "Vector 2", "vector2");
                AddItemForDynamicPortAdder(menu, "Vector 3", "vector3");
                AddItemForDynamicPortAdder(menu, "Vector 4", "vector4");
                AddItemForDynamicPortAdder(menu, "Texture", "Texture2D");
                //AddItemForDynamicPortAdder(menu, "Color", typeof(Color));
                //AddItemForDynamicPortAdder(menu, "Boolean", "bool");
                //AddItemForDynamicPortAdder(menu, "String", typeof(string));
                //menu.AddSeparator("");
                //AddItemForDynamicPortAdder(menu, "UV", "uv");
                //AddItemForDynamicPortAdder(menu, "Position", "pos");
                NodeEditorWindow.current.onLateGUI += () => ShowContextMenuAtMouse(menu);
            }
            if (GUILayout.Button("Remove Port"))
            {
                GenericMenu menu = new GenericMenu();
                for (int i = 0; i < dynamicPortList.Count; i++)
                {
                    AddItemForDynamicPortRemover(menu, dynamicPortList[i].fieldName.Split('_').First(), dynamicPortList[i].fieldName);
                }
                if (dynamicPortList.Count == 0)
                    menu.AddItem(new GUIContent("No Port Available"), false, null);
                NodeEditorWindow.current.onLateGUI += () => ShowContextMenuAtMouse(menu);
            }            
            GUILayout.EndHorizontal();
            for (int i = 0; i < dynamicPortList.Count; i++)
            {
                NodeEditorGUILayout.PortField(new GUIContent(dynamicPortList[i].fieldName.Split('_').First() + " (" + dynamicPortList[i].fieldName.Split('_').Last() + ")"), dynamicPortList[i]);
            }
            serializedObject.ApplyModifiedProperties();
        }

        void AddItemForDynamicPortAdder(GenericMenu menu, string menuPath, string portType)
        {
            menu.AddItem(new GUIContent(menuPath), false, AddDynamicPort, portType);
        }
        void AddDynamicPort(object portType)
        {
            if (mi == null) mi = target as MaterialInput;
            dynamicPortList = mi.DynamicPorts.ToList();
            bool isContainSameName = false;
            for (int i = 0; i < dynamicPortList.Count; i++)
            {
                if (mi.portAddName.Split('_').First() == dynamicPortList[i].fieldName.Split('_').First())
                {
                    PopupWindow.Show(buttonRect, new WarningPopup("Port " + mi.portAddName.Split('_').First() + " already exists in Material Input."));
                    isContainSameName = true;
                    break;
                }
                else
                    isContainSameName = false;
            }
            if (!isContainSameName)
            {
                if (mi.portAddName.Split('_').First() == "")
                {
                    PopupWindow.Show(buttonRect, new WarningPopup("No port name assigned."));
                }
                else if (mi.portAddName.Split('_').First().Length < 3)
                {
                    PopupWindow.Show(buttonRect, new WarningPopup("Too short port name. Make it at least 3 character."));
                }
                else
                {
                    mi.AddDynamicOutput(typeof(string), fieldName: mi.portAddName + "_" + portType, connectionType: Node.ConnectionType.Multiple, portType: (string)portType);
                }
            }
        }

        void AddItemForDynamicPortRemover(GenericMenu menu, string menuPath, string portName)
        {
            menu.AddItem(new GUIContent(menuPath), false, RemoveDynamicPort, portName);
        }
        void RemoveDynamicPort(object portName)
        {
            if (mi == null) mi = target as MaterialInput;
            mi.RemoveDynamicPort((string)portName);
        }

        public void ShowContextMenuAtMouse(GenericMenu menu)
        {
            // Display at cursor position
            Rect r = new Rect(Event.current.mousePosition, new Vector2(0, 0));
            menu.DropDown(r);
        }
    }
}