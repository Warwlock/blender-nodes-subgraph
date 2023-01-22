using UnityEngine;
using UnityEditor;
using System.Text;
using BNGNodeEditor;

public class GeneralNodePopup : PopupWindowContent
{
    public string EnumValue = "";
    public Vector2 windowSize;
    string[] enumNames;
    Color firstColor;
    float y;
    int index = 0;

    public bool isButtonPressed = false;

    Vector2 position;

    public GeneralNodePopup(Vector2 windowSize, string[] enumNames = null, string defaultEnum = "")
    {
        EnumValue = defaultEnum;
        this.enumNames = enumNames;
        this.windowSize = windowSize;
    }

    public override Vector2 GetWindowSize()
    {
        return windowSize;
    }

    public override void OnGUI(Rect rect)
    {
        GUILayout.FlexibleSpace();
        GUILayout.BeginHorizontal();
        firstColor = GUI.color;
        for (int i = 0; i < enumNames.Length; i++)
        {
            if(i == 0)
            {
                GUILayout.BeginVertical();
            }
            if (enumNames[i] == "_a" || enumNames[i] == "_b" || enumNames[i] == "_c" || enumNames[i] == "_d" || enumNames[i] == "_e")
            {
                GUILayout.EndVertical();
                if (windowSize.y < y)
                    windowSize.y = y;
                y = 0;
                GUILayout.BeginVertical();
                continue;
            }
            if (enumNames[i] == "_" || enumNames[i] == "__" || enumNames[i] == "___" || enumNames[i] == "____" || enumNames[i] == "_____")
            {
                HorizontalLine();
                y += 10;
                continue;
            }
            if(enumNames[i][0] == '_')
            {
                GUI.color = Color.gray * 1f;
                GUILayout.Label(enumNames[i].TrimStart('_'), EditorStyles.label);
                GUI.color = firstColor;
                GUILayout.Space(3);
                HorizontalLine();
                y += 10;
                continue;
            }
            GUIStyle buttonStyle = EditorStyles.toolbarButton;
            buttonStyle.alignment = TextAnchor.MiddleLeft;
            if (GUILayout.Button(AddSpacesToSentence(enumNames[i]), buttonStyle))
            {                
                EnumValue = enumNames[i];
                isButtonPressed = true;
                editorWindow.Close();
            }
            y += 10;
        }
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
        GUILayout.FlexibleSpace();
        if (windowSize.y < y)
            windowSize.y = y;
        y = 0;
        OneTimePosition();
        NodeEditorWindow.RepaintAll();
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

    string AddSpacesToSentence(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return "";
        StringBuilder newText = new StringBuilder(text.Length * 2);
        newText.Append(text[0]);
        for (int i = 1; i < text.Length; i++)
        {
            if (char.IsUpper(text[i]) && text[i - 1] != ' ')
                newText.Append(' ');
            newText.Append(text[i]);
        }
        return newText.ToString();
    }

    public override void OnOpen()
    {
        
    }

    void OneTimePosition()
    {
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
