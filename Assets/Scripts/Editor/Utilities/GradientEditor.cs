using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;
/*
[CustomEditor(typeof(DebugScript))]
public class DebugInspector : Editor
{
    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();
        SerializedObject serializedGradient = new SerializedObject(target);
        SerializedProperty colorGradient = serializedGradient.FindProperty("gradient");
        EditorGUILayout.PropertyField(colorGradient, true, null);
        if (EditorGUI.EndChangeCheck())
            serializedGradient.ApplyModifiedProperties();
    }
}

//Another file (DebugScript.cs):
using UnityEngine;
public class DebugScript : MonoBehaviour
{
    public Gradient gradient;
}

}*/
