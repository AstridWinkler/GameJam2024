
#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using System;
using System.Collections.Generic;

public class FindMissingScriptsEditor : EditorWindow
{
    [MenuItem("Logiked/Utilities/Find Missing Scripts", priority = 500)]
    public static void FindMissingScripts()
    {
        EditorWindow.GetWindow(typeof(FindMissingScriptsEditor));
    }

    /*
    [MenuItem("Window/Utilities/Clear Progressbar")]
    public static void ClearProgressbar()
    {
        EditorUtility.ClearProgressBar();
    }
    */

    static int missingCount  {
    get {
            if (missingList == null) return -1;
            return missingList.Count;
        }
    }

    static List<GameObject> missingList = new List<GameObject>();
    static Vector2 pos;




    void OnGUI()
    {

        /*
        ca fonctionne mais ca va piocher dans les dossiers
        if (GUILayout.Button("Find missing scripts"))
        {

            missingList = new List<GameObject>();
            EditorUtility.DisplayProgressBar("Searching Prefabs", "", 0.0f);

            string[] files = System.IO.Directory.GetFiles(Application.dataPath, "*.prefab", System.IO.SearchOption.AllDirectories);
            EditorUtility.DisplayCancelableProgressBar("Searching Prefabs", "Found " + files.Length + " prefabs", 0.0f);

            Scene currentScene = EditorSceneManager.GetActiveScene();
            string scenePath = currentScene.path;
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene);

            for (int i = 0; i < files.Length; i++)
            {
                string prefabPath = files[i].Replace(Application.dataPath, "Assets");
                if (EditorUtility.DisplayCancelableProgressBar("Processing Prefabs " + i + "/" + files.Length, prefabPath, (float)i / (float)files.Length))
                    break;

                GameObject go = UnityEditor.AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject)) as GameObject;

                if (go != null)
                {
                    FindInGO(go);
                    go = null;
                    EditorUtility.UnloadUnusedAssetsImmediate(true);
                }
            }

            EditorUtility.DisplayProgressBar("Cleanup", "Cleaning up", 1.0f);
            EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);

            EditorUtility.UnloadUnusedAssetsImmediate(true);
            GC.Collect();

            EditorUtility.ClearProgressBar();
        }
        */


        if (GUILayout.Button("Find missing scripts"))
        {
            missingList = new List<GameObject>();

            GameObject[] objs = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();

            for (int i = 0; i < objs.Length; i++)
            {
                    FindInGO(objs[i]);                
            }
        }




        EditorGUILayout.BeginHorizontal();
        {
            EditorGUILayout.LabelField("Missing Scripts:");
            EditorGUILayout.LabelField("" + (missingCount == -1 ? "---" : missingCount.ToString()));
        }

        EditorGUILayout.EndHorizontal();

       

        if (missingCount > 0)
        {
            EditorGUILayout.BeginVertical("box");

            pos = EditorGUILayout.BeginScrollView(pos);

            foreach (var e in missingList)
            {
               if(GUILayout.Button($" on {e.name}"))
                {
                    Selection.activeObject = e;
                }
            }
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();

        }


    }

    private static void FindInGO(GameObject go, string prefabName = "")
    {
        Component[] components = go.GetComponents<Component>();
        for (int i = 0; i < components.Length; i++)
        {
            if (components[i] == null)
            {
                missingList.Add(go);

                Transform t = go.transform;

                string componentPath = go.name;
                while (t.parent != null)
                {
                    componentPath = t.parent.name + "/" + componentPath;
                    t = t.parent;
                }
                Debug.LogWarning("Prefab " + prefabName + " has an empty script attached:\n" + componentPath, go);
            }
        }

        foreach (Transform child in go.transform)
        {
            FindInGO(child.gameObject, prefabName);
        }
    }
}

#endif