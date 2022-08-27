using UnityEngine;
using UnityEditor;
using BNGNodeEditor;

public class WarningPopup : PopupWindowContent
{
    public string ErrorLabel;
    int index = 0;

    public WarningPopup(string errorLabel)
    {
        ErrorLabel = errorLabel;
    }

    public override Vector2 GetWindowSize()
    {
        return new Vector2(300, 45);
    }

    public override void OnGUI(Rect rect)
    {
        //GUILayout.Label("Popup Options Example", EditorStyles.boldLabel);
        GUILayout.FlexibleSpace();
        EditorGUILayout.HelpBox(ErrorLabel, MessageType.Error);
        GUILayout.FlexibleSpace();
        OneTimePosition();
    }

    public override void OnOpen()
    {

    }

    void OneTimePosition()
    {
        if (index < 3)
        {
            NodeEditorWindow.current.onLateGUI += () => editorWindow.position = new Rect(GUIUtility.GUIToScreenPoint(Event.current.mousePosition), new Vector2(0, 0));
            index++;
        }
    }

    public override void OnClose()
    {
        
    }
}