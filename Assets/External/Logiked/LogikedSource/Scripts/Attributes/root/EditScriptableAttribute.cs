/*MIT License

Copyright(c) 2017 Jeiel Aranal

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.*/

using logiked.source.attributes.root;
using UnityEngine;
using System.Collections.Generic;
using logiked.source.extentions;
using System;
using static logiked.source.attributes.root.FutureFieldAttribute;


#if UNITY_EDITOR
using UnityEditor;
using logiked.source.editor;
#endif

namespace logiked.source.utilities
{
    /// <summary>
    /// Display a ScriptableObject field with an inline editor
    /// </summary>
    public class EditScriptableAttribute : FutureFieldAttribute
	{
		
		
		private class EditScriptableDatas
        {
			public EditScriptableDatas(bool open_) => open = open_;
			public bool open;
#if UNITY_EDITOR
			[NonSerialized]
			public Editor editor;
#endif
		}



		static Dictionary<object, EditScriptableDatas> propValue = new Dictionary<object, EditScriptableDatas>();
		bool defaultVal;
		int deepTest = 0; //Pour eviter les drawingRecursif

		public EditScriptableAttribute(bool openByDefault=false) => defaultVal = openByDefault;


#if UNITY_EDITOR
		/// <summary>
		/// Retourne l'editeur généré avec la clé Context
		/// </summary>
		/// <returns></returns>
		public static Editor GetCachedEditor(object context)
        {
			return propValue.GetOrDefault(context)?.editor;
		}
#endif



#if UNITY_EDITOR

		protected override void OnGUIRecursive(Rect position, SerializedProperty property, GUIContent label, AttributeContext Context)
		{



			var code = property.GetParent();
			bool unlocked = !(deepTest >= 1);
			EditScriptableDatas val = new EditScriptableDatas(defaultVal);
			val = propValue.GetOrDefault(code, val);
			val.open = unlocked && val.open;


			if (property.objectReferenceValue == null) {
				property.DrawPropertyField(position, label);
				return;
			}

			EditorGUI.BeginChangeCheck();

			deepTest++;

		



			if (val.editor == null && property.objectReferenceValue != null)
			{

				
		
				val.editor = Editor.CreateEditor(property.objectReferenceValue);

		
			}

			//Undo.RecordObject(property.objectReferenceValue, "prop");


			if(val.open)
				EditorGUILayout.BeginVertical("box");

			EditorGUILayout.BeginHorizontal();

			bool g = GUI.enabled;
			GUI.enabled = unlocked;
			GUILogiked.Panels.GUIDrawEditorIcon(() => val.open = !val.open);
			GUI.enabled = g;
			property.DrawPropertyField(label);
			EditorGUILayout.EndHorizontal();

			if (val.open && val.editor!=null )
			{
				val.editor.OnInspectorGUI();


				EditorGUILayout.EndVertical();
			}

			 propValue.AddOrUpdate(code, val);

			deepTest--;
			
			if (property.objectReferenceValue != null)
			{
				val.editor.serializedObject.ApplyModifiedProperties();
				EditorUtility.SetDirty(property.objectReferenceValue);
				val.editor.Repaint();

			}

			if (EditorGUI.EndChangeCheck() )
            {

				if (property.objectReferenceValue != null)
				{
					
			
					//AssetDatabase.SaveAssets();
					//Debug.LogError(property.FindPropertyRelative("roomSize").vector3IntValue);

					//AssetDatabase.Refresh();
				}
            }


		}
#endif
	}

}
