using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System;
using BNGNodeEditor;

public class NewNodePopup : PopupWindowContent
{
    public int EnumValue = -1;
    public Vector2 windowSize;
    string[] enumNames;
    bool[] foldouts;
    Color firstColor;
    float y;
    List<string> firstNames;
    bool isSearch;
    string searchText;

    int index = 0;
    Vector2 position;

    public NewNodePopup(Vector2 windowSize, string[] enumNames = null, string defaultEnum = "")
    {
        //EnumValue = defaultEnum;
        this.enumNames = enumNames;
        this.windowSize = windowSize;
    }

    public override Vector2 GetWindowSize()
    {
        return windowSize;
    }

    public override void OnGUI(Rect rect)
    {        
        firstColor = GUI.color;
        GUIStyle buttonStyle = EditorStyles.toolbarButton;
        buttonStyle.alignment = TextAnchor.MiddleLeft;
        if (!isSearch)
        {
            GUILayout.FlexibleSpace();
            GUI.color = Color.gray * 1f;
            GUILayout.Label("Add", EditorStyles.label);
            GUI.color = firstColor;
            GUILayout.Space(3);
            y += 21;
            HorizontalLine();
            if (GUILayout.Button("Search", EditorStyles.toolbarSearchField))
                isSearch = true;

            y += 21;
            HorizontalLine();
        }
        else
        {
            GUI.SetNextControlName("Seraching");
            searchText = GUILayout.TextField(searchText);
            EditorGUI.FocusTextInControl("Seraching");
            y = 160;
        }

        if (isSearch)
        {
            for (int i = 0; i < enumNames.Length; i++)
            {
                if (enumNames[i] != null && searchText != null && searchText != "")
                {
                    if (enumNames[i].Split('/').Last().IndexOf(searchText, StringComparison.CurrentCultureIgnoreCase) != -1)
                    {
                        if (GUILayout.Button("    " + enumNames[i].Split('/').Last(), buttonStyle))
                        {
                            EnumValue = i;
                            editorWindow.Close();
                        }
                        if(Event.current.keyCode == KeyCode.Return)
                        {
                            EnumValue = i;
                            editorWindow.Close();
                        }
                    }
                }
                if (enumNames[i] != null && (searchText == null || searchText == ""))
                {
                    if (GUILayout.Button("    " + enumNames[i].Split('/').Last(), buttonStyle))
                    {
                        EnumValue = i;
                        editorWindow.Close();
                    }
                }
            }
        }

        //GUILayout.Button("Heyyoo", EditorStyles.foldoutHeader);
        /*if (GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
            Debug.Log("MOUSE");*/
        if (!isSearch)
        {
            for (int i = 0; i < firstNames.Count(); i++)
            {
                foldouts[i] = EditorGUILayout.BeginFoldoutHeaderGroup(foldouts[i], firstNames[i]);
                if (foldouts[i])
                {
                    for (int t = 0; t < foldouts.Count(); t++)
                    {
                        if (t != i)
                            foldouts[t] = false;
                    }
                    for (int t = 0; t < enumNames.Length; t++)
                    {
                        if (enumNames[t] != null)
                        {
                            if (firstNames[i] == enumNames[t].Split('/').First())
                            {
                                if (GUILayout.Button("      " + enumNames[t].Split('/').Last(), buttonStyle))
                                {
                                    EnumValue = t;
                                    editorWindow.Close();
                                }
                                y += 20;
                            }
                        }
                    }
                    OneTimePosition();
                }
                EditorGUILayout.EndFoldoutHeaderGroup();
                /*else if(nodePopup.editorWindow.o)
                    nodePopup.editorWindow.Close();*/
                y += 21;
            }
        }
                
        windowSize.y = y;
        y = 0;
        GUILayout.FlexibleSpace();
        OneTimePosition();
    }

    void HorizontalLine()
    {
        GUIStyle horizontalLine;
        horizontalLine = new GUIStyle();
        horizontalLine.normal.background = EditorGUIUtility.whiteTexture;
        Colorize(horizontalLine.normal.background, Color.gray * 0.7f);
        horizontalLine.margin = new RectOffset(0, 0, 2, 2);
        horizontalLine.fixedHeight = 1;
        GUILayout.Box(GUIContent.none, horizontalLine);
        Colorize(horizontalLine.normal.background, Color.white);
    }

    void Colorize(Texture2D tex, Color color)
    {
        Color[] pixels = tex.GetPixels();
        for (int i = 0; i < pixels.Length; i++) pixels[i] = color;
        tex.SetPixels(pixels);
        tex.Apply();
    }

    public override void OnOpen()
    {
        EnumValue = -1;
        firstNames = new List<string>();
        for (int i = 0; i < enumNames.Length; i++)
        {
            if (enumNames[i] == null)
                continue;
            firstNames.Add(enumNames[i].Split('/').First());
        }
        firstNames = firstNames.Distinct().ToList();
        foldouts = new bool[firstNames.Count()];
    }

    public void OneTimePosition()
    {
        /*if (index < 3)
        {
            NodeEditorWindow.current.onLateGUI += () =>
            {
                editorWindow.position = new Rect(GUIUtility.GUIToScreenPoint(
                    new Vector2(Event.current.mousePosition.x, Event.current.mousePosition.y)),
                    new Vector2(0, 0));
                position = Event.current.mousePosition;
            };
            index++;
        }*/
        if (index < 2)
        {
            NodeEditorWindow.current.onLateGUI += () =>
            {
                editorWindow.position = new Rect(GUIUtility.GUIToScreenPoint(
                    new Vector2(Event.current.mousePosition.x, Event.current.mousePosition.y)),
                    new Vector2(0, 0));
                position = Event.current.mousePosition;
                position = GUIUtility.GUIToScreenPoint(position);
                editorWindow.position = new Rect(position, new Vector2());
                index++;
            };
        }
        editorWindow.position = new Rect(position, new Vector2());
        NodeEditorWindow.RepaintAll();
    }

    public event System.Action OnCloseEvent;
    public override void OnClose()
    {
        if (OnCloseEvent != null)
        {
            OnCloseEvent();
            OnCloseEvent = null;
        }
    }
}
