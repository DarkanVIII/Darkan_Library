namespace Game
{
    using System.Collections.Generic;
    using System.IO;
    using UnityEditor;
    using UnityEditor.SceneManagement;
    using UnityEditor.Toolbars;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    public class MainToolbarExtensions : MonoBehaviour
    {
        #region Time Scale Dropdown

        const string _timeScaleDropdownPath = "Darkan/Time Scale Dropdown";

        [MainToolbarElement(_timeScaleDropdownPath, defaultDockPosition = MainToolbarDockPosition.Middle)]
        public static MainToolbarElement CreateTimeScaleSlider()
        {
            var icon = EditorGUIUtility.IconContent("UnityEditor.AnimationWindow").image as Texture2D;
            MainToolbarContent content = new("Time x", icon, "Set Time Scale");
            MainToolbarSlider slider = new(content, Time.timeScale, 0f, 5f, OnTimeScaleChanged)
            {
                populateContextMenu = (menu) =>
                {
                    menu.InsertAction(menu.MenuItems().Count, "Reset to 1", _ =>
                    {
                        Time.timeScale = 1;
                        MainToolbar.Refresh(_timeScaleDropdownPath);
                    });
                }
            };

            return slider;
        }

        static void OnTimeScaleChanged(float value)
        {
            Time.timeScale = value;
        }

        #endregion

        #region Build Scene Changer

        const string _buildSceneChangerDropdownPath = "Darkan/Build Scene Changer Dropdown";

        [MainToolbarElement(_buildSceneChangerDropdownPath, defaultDockPosition = MainToolbarDockPosition.Middle)]
        public static MainToolbarElement CreateBuildSceneChangerDropdown()
        {
            string activeSceneName;

            if (Application.isPlaying)
                activeSceneName = SceneManager.GetActiveScene().name;
            else
                activeSceneName = EditorSceneManager.GetActiveScene().name;
            if (activeSceneName.Length == 0)
                activeSceneName = "Untitled";

            var icon = EditorGUIUtility.IconContent("UnityLogo").image as Texture2D;
            var content = new MainToolbarContent(activeSceneName, icon, "Select active scene");

            return new MainToolbarDropdown(content, ShowDropdownMenu);

            static void ShowDropdownMenu(Rect dropDownRect)
            {
                var menu = new GenericMenu();

                if (EditorBuildSettings.scenes.Length == 0)
                {
                    menu.AddDisabledItem(new GUIContent("No Scenes in Project"));
                }
                else
                {
                    foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
                    {
                        string sceneName = Path.GetFileNameWithoutExtension(scene.path);
                        string label = sceneName;

                        if (GetActiveSceneName() == sceneName)
                            label += " (Active)";

                        menu.AddItem(new GUIContent(label), false, () =>
                        {
                            SwitchScene(scene.path);
                        });
                    }
                }

                menu.DropDown(dropDownRect);
            }

            static void SwitchScene(string scenePath)
            {
                if (Application.isPlaying)
                {
                    string sceneName = Path.GetFileNameWithoutExtension(scenePath);

                    if (Application.CanStreamedLevelBeLoaded(sceneName))
                        SceneManager.LoadScene(sceneName);
                    else
                        Debug.LogError($"Scene '{sceneName}' is not in the Build Settings.");
                }
                else
                {
                    if (File.Exists(scenePath))
                    {
                        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                            EditorSceneManager.OpenScene(scenePath);
                    }
                    else
                    {
                        Debug.LogError($"Scene at path '{scenePath}' does not exist.");
                    }
                }

                MainToolbar.Refresh(_buildSceneChangerDropdownPath);
            }
        }

        #endregion


        #region Scene Changer

        const string _sceneChangerDropdownPath = "Darkan/Scene Changer Dropdown";

        [MainToolbarElement(_sceneChangerDropdownPath, defaultDockPosition = MainToolbarDockPosition.Middle)]
        public static MainToolbarElement CreateSceneChangerDropdown()
        {
            string activeSceneName;

            activeSceneName = GetActiveSceneName();

            if (activeSceneName.Length == 0)
                activeSceneName = "Untitled";

            var icon = EditorGUIUtility.IconContent("UnityLogo").image as Texture2D;
            var content = new MainToolbarContent(activeSceneName, icon, "Select active scene");

            return new MainToolbarDropdown(content, ShowDropdownMenu);

            static void ShowDropdownMenu(Rect dropDownRect)
            {
                var menu = new GenericMenu();

                List<string> scenePaths = GetScenesInScenesFolder();

                if (scenePaths.Count == 0)
                {
                    menu.AddDisabledItem(new GUIContent("No Scenes in Project"));
                }
                else
                {
                    foreach (string path in scenePaths)
                    {
                        string sceneName = Path.GetFileNameWithoutExtension(path);
                        string label = sceneName;

                        if (GetActiveSceneName() == sceneName)
                            label += " (Active)";

                        menu.AddItem(new GUIContent(label), false, () =>
                            {
                                SwitchScene(path);
                            });
                    }
                }

                menu.DropDown(dropDownRect);
            }

            static void SwitchScene(string scenePath)
            {
                if (Application.isPlaying)
                {
                    string sceneName = Path.GetFileNameWithoutExtension(scenePath);

                    if (Application.CanStreamedLevelBeLoaded(sceneName))
                        SceneManager.LoadScene(sceneName);
                    else
                        Debug.LogError($"Scene '{sceneName}' is not in the Build Settings.");
                }
                else
                {
                    if (File.Exists(scenePath))
                    {
                        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                            EditorSceneManager.OpenScene(scenePath);
                    }
                    else
                    {
                        Debug.LogError($"Scene at path '{scenePath}' does not exist.");
                    }
                }

                MainToolbar.Refresh(_sceneChangerDropdownPath);
            }

            static List<string> GetScenesInScenesFolder()
            {
                const string scenesRoot = "Assets/Scenes";

                if (!AssetDatabase.IsValidFolder(scenesRoot))
                {
                    Debug.LogWarning("Assets/Scenes folder not found.");
                    return new List<string>();
                }

                string[] guids = AssetDatabase.FindAssets("t:Scene", new[] { scenesRoot });

                List<string> scenePaths = new List<string>();

                foreach (string guid in guids)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guid);
                    scenePaths.Add(path);
                }

                return scenePaths;
            }
        }

        static string GetActiveSceneName()
        {
            string activeSceneName;
            if (Application.isPlaying)
                activeSceneName = SceneManager.GetActiveScene().name;
            else
                activeSceneName = EditorSceneManager.GetActiveScene().name;
            return activeSceneName;
        }

        #endregion

    }
}