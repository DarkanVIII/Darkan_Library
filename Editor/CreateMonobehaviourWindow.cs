using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Darkan.Editor
{
    public class CreateMonobehaviourWindow : EditorWindow
    {
        [System.Serializable]
        class AsmdefJson
        {
            public string rootNamespace;
        }

        const string _defaultName = "Monobehaviour";
        static bool _firstFrame;
        static bool _enterClicked;
        static bool _escapeClicked;
        int _numOfScripts = 1;
        readonly static List<string> _inputStrings = new();

        [MenuItem("Assets/CreateMonobehaviours %m", priority = 1000)]
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
            GUIStyle textFieldStyle = new(GUI.skin.textField)
            {
                fontSize = 14,
                alignment = TextAnchor.MiddleCenter,
                border = new RectOffset(2, 2, 2, 2),
                margin = new RectOffset(0, 0, 5, 5),
                padding = new RectOffset(5, 5, 5, 5),
            };


            GUIStyle buttonStyle = new(GUI.skin.button)
            {
                fontSize = 18,
                fontStyle = FontStyle.Bold
            };
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
                    _inputStrings.Add(_defaultName + (i + 1));
                }

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();

                if (i == 0)
                    GUI.SetNextControlName("FirstScriptTextField");

                _inputStrings[i] = EditorGUILayout.TextField(_inputStrings[i], textFieldStyle, GUILayout.Width(350), GUILayout.Height(28));

                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }

            GUILayout.FlexibleSpace();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Cancel", buttonStyle, GUILayout.Width(100), GUILayout.Height(50)) || _escapeClicked)
            {
                Close();
                _escapeClicked = false;
            }

            GUILayout.Space(25);

            if (GUILayout.Button("Create", buttonStyle, GUILayout.Width(100), GUILayout.Height(50)) || _enterClicked)
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
        }

        void CreateScripts()
        {
            List<string> assetPaths = new();

            try
            {
                AssetDatabase.StartAssetEditing();

                for (int i = 0; i < _numOfScripts; i++)
                {
                    string name = _inputStrings[i];

                    if (!string.IsNullOrEmpty(name))
                    {
                        if (CreateMonobehaviourTemplate(name, out string assetPath))
                            assetPaths.Add(assetPath);
                    }
                    else
                    {
                        Debug.LogWarning("Scripts with empty class names can't be created.");
                    }
                }
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
                AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);

                // open created scrips
                foreach (string assetPath in assetPaths)
                {
                    MonoScript assetToOpen = AssetDatabase.LoadAssetAtPath<MonoScript>(assetPath);

                    if (assetToOpen != null)
                        AssetDatabase.OpenAsset(assetToOpen);
                }

                Close();
            }
        }

        string GetRootNamespaceForPath(string assetPath)
        {
            string absolutePath = Path.GetFullPath(assetPath);
            string directory = Path.GetDirectoryName(absolutePath);

            while (!string.IsNullOrEmpty(directory))
            {
                string[] asmdefFiles = Directory.GetFiles(directory, "*.asmdef", SearchOption.TopDirectoryOnly);

                if (asmdefFiles.Length > 0)
                {
                    string asmdefPath = asmdefFiles[0];
                    string json = File.ReadAllText(asmdefPath);
                    var asmdef = JsonUtility.FromJson<AsmdefJson>(json);
                    return asmdef.rootNamespace ?? "";
                }

                directory = Path.GetDirectoryName(directory); // go up one level
            }

            return ""; // No asmdef found → default Assembly-CSharp
        }

        bool CreateMonobehaviourTemplate(string scriptName, out string assetPath)
        {
            assetPath = null;

            Object selectedObject = Selection.activeObject;

            if (selectedObject == null) return false;

            string destinationPath = AssetDatabase.GetAssetPath(selectedObject);

            if (!AssetDatabase.IsValidFolder(destinationPath))
            {
                destinationPath = Path.GetDirectoryName(destinationPath);
            }

            assetPath = destinationPath + "/" + scriptName + ".cs";

            // Ensure the file name is unique
            assetPath = AssetDatabase.GenerateUniqueAssetPath(assetPath);
            scriptName = Path.GetFileName(assetPath);
            scriptName = scriptName.Replace(".cs", "");
            string rootNamespace = GetRootNamespaceForPath(assetPath);

            if (string.IsNullOrEmpty(rootNamespace))
                rootNamespace = "Game";

            string scriptContent =
@"namespace #ROOTNAMESPACE#
{
    using UnityEngine;

    public class #SCRIPTNAME# : MonoBehaviour
    {
    
    }
}";
            scriptContent = scriptContent.Replace("#SCRIPTNAME#", scriptName);
            scriptContent = scriptContent.Replace("#ROOTNAMESPACE#", rootNamespace);
            //scriptContent = scriptContent.Replace("#USERNAME#", System.Environment.UserName);

            File.WriteAllText(assetPath, scriptContent);

            return true;
        }
    }
}