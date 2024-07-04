using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class CreateMonobehaviourWindow : EditorWindow
{
    const string DEFAULT_NAME = "Monobehaviour";

    static bool _firstFrame;
    static bool _enterClicked;
    static bool _escapeClicked;
    int _numOfScripts = 1;
    readonly static List<string> _inputStrings = new();

    [MenuItem("Assets/Create Monobehaviours %g", priority = 1000)]
    public static void ShowWindow()
    {
        Object selectedObject = Selection.activeObject;

        if (selectedObject == null) return;

        _firstFrame = true;
        _enterClicked = false;
        _escapeClicked = false;

        _inputStrings.Clear();

        // Get existing open window or if none, make a new one:
        CreateMonobehaviourWindow window = (CreateMonobehaviourWindow)GetWindow(typeof(CreateMonobehaviourWindow));
        window.titleContent = new GUIContent("Create Monobehaviours");
        window.Show();
    }

    void OnGUI()
    {
        int uniformPadding = 15;
        RectOffset padding = new(uniformPadding, uniformPadding, uniformPadding, uniformPadding);
        Rect area = new(padding.right, padding.top, position.width - (padding.right + padding.left), position.height - (padding.top + padding.bottom));

        #region Style
        GUIStyle textFieldStyle = GUI.skin.textField;
        textFieldStyle.fontSize = 14;
        textFieldStyle.normal.textColor = Color.white;
        textFieldStyle.alignment = TextAnchor.MiddleCenter;
        textFieldStyle.border = new RectOffset(2, 2, 2, 2);
        textFieldStyle.margin = new RectOffset(0, 0, 5, 5);
        textFieldStyle.padding = new RectOffset(5, 5, 5, 5);

        GUIStyle buttonStyle = GUI.skin.button;
        buttonStyle.fontSize = 18;
        buttonStyle.fontStyle = FontStyle.Bold;
        #endregion

        #region Input
        Event e = Event.current;
        if (e.type is EventType.KeyDown)
        {
            if (e.keyCode is KeyCode.Return)
            {
                _enterClicked = true;
            }
            if (e.keyCode is KeyCode.UpArrow)
            {
                _numOfScripts = Mathf.Clamp(_numOfScripts + 1, 1, 10);
            }
            if (e.keyCode is KeyCode.DownArrow)
            {
                _numOfScripts = Mathf.Clamp(_numOfScripts - 1, 1, 10);
            }
            if (e.keyCode is KeyCode.Escape)
            {
                _escapeClicked = true;
            }
        }
        #endregion

        GUILayout.BeginArea(area);

        Rect rect = GUILayoutUtility.GetRect(40, 25);
        _numOfScripts = EditorGUI.IntSlider(rect, _numOfScripts, 1, 10);

        for (int i = 0; i < _numOfScripts; i++)
        {
            if (_inputStrings.Count - 1 < i)
            {
                _inputStrings.Add(DEFAULT_NAME + (i + 1));
            }

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (i == 0)
                GUI.SetNextControlName("FirstScriptTextField");

            _inputStrings[i] = EditorGUILayout.TextField(_inputStrings[i], GUILayout.Width(350), GUILayout.Height(28));

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        GUILayout.FlexibleSpace();

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        if (GUILayout.Button("Cancel", GUILayout.Width(100), GUILayout.Height(50)) || _escapeClicked)
        {
            Close();
            _escapeClicked = false;
        }

        GUILayout.Space(25);

        if (GUILayout.Button("Create", GUILayout.Width(100), GUILayout.Height(50)) || _enterClicked)
        {
            CreateScripts();
            _enterClicked = false;
        }

        GUILayout.FlexibleSpace();

        GUILayout.EndHorizontal();

        GUILayout.EndArea();

        if (_firstFrame)
        {
            EditorGUI.FocusTextInControl("FirstScriptTextField");
            _firstFrame = false;
        }

        GUI.skin = null;
    }

    void CreateScripts()
    {
        try
        {
            AssetDatabase.StartAssetEditing();

            for (int i = 0; i < _numOfScripts; i++)
            {
                string name = _inputStrings[i];

                if (!string.IsNullOrEmpty(name))
                {
                    CreateMonobehaviourTemplate(name);
                }
                else
                {
                    Debug.LogWarning("Scipts with empty class names can't be created.");
                }
            }
        }
        finally
        {
            AssetDatabase.StopAssetEditing();
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            Close();
        }
    }

    void CreateMonobehaviourTemplate(string scriptName)
    {
        Object selectedObject = Selection.activeObject;

        if (selectedObject == null) return;

        string destinationPath = AssetDatabase.GetAssetPath(selectedObject);

        if (!AssetDatabase.IsValidFolder(destinationPath))
        {
            destinationPath = Path.GetDirectoryName(destinationPath);
        }

        string filePath = destinationPath + "/" + scriptName + ".cs";

        // Ensure the file name is unique
        filePath = AssetDatabase.GenerateUniqueAssetPath(filePath);
        scriptName = Path.GetFileName(filePath);
        scriptName = scriptName.Replace(".cs", "");

        string scriptContent =
@"// Author: #USERNAME#

using UnityEngine;

public class #SCRIPTNAME# : MonoBehaviour
{
        
}";
        scriptContent = scriptContent.Replace("#SCRIPTNAME#", scriptName);
        scriptContent = scriptContent.Replace("#USERNAME#", System.Environment.UserName);

        File.WriteAllText(filePath, scriptContent);
    }
}