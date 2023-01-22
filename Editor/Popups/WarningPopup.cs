using UnityEngine;
using UnityEditor;
using BNGNodeEditor;

public class WarningPopup : PopupWindowContent
{
    public string ErrorLabel;
    int index = 0;

    Vector2 position;

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

    public override void OnClose()
    {
        
    }
}