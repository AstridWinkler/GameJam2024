using System;
using UnityEngine;
using System.Collections.Generic;
using logiked.source.attributes.root;

#if UNITY_EDITOR
using UnityEditor;
#endif



namespace logiked.source.attributes
{
    /// <summary>
    /// Permet d'assigner un MonoScript à une chaine de caractères. Utile pour la reflection.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class MonoScriptAttribute : FutureFieldAttribute
    {
        public System.Type type;





#if UNITY_EDITOR
        /// <summary>
        /// Classe permettant l'enregistrement d'un nom de classe avec un assignement d'un monoescript depuis l'editeur. Compatible Editeur et Runtime
        /// <para/>
        /// Utile pour faire de la reflexion.
        /// <para/>
        /// Vous pouvez ajouter une contrainte sur la classe parent à assigner avec l'attribut <see cref="BehaviourConstraint"/>
        /// </summary>

        static Dictionary<string, MonoScript> m_ScriptCache;
        public MonoScriptAttribute()
        {
            m_ScriptCache = new Dictionary<string, MonoScript>();
            var scripts = Resources.FindObjectsOfTypeAll<MonoScript>();
            for (int i = 0; i < scripts.Length; i++)
            {
                var type = scripts[i].GetClass();
                if (type != null && !m_ScriptCache.ContainsKey(type.AssemblyQualifiedName))
                {
                    m_ScriptCache.Add(type.AssemblyQualifiedName, scripts[i]);
                }
            }
        }
        bool m_ViewString = false;


        protected override void OnGUIRecursive(Rect position, SerializedProperty property, GUIContent label, AttributeContext Context)
        {
            if (property.propertyType == SerializedPropertyType.String)
            {
                // Rect r = EditorGUI.PrefixLabel(position, label);
                //Rect labelRect = position;
                // labelRect.xMax = r.xMin;
              
                Rect labelRect = position;
                 labelRect.width = EditorGUIUtility.labelWidth;

                /*
                position.width = position.width - EditorGUIUtility.labelWidth;
                position.x = EditorGUIUtility.labelWidth;
                */

                m_ViewString = GUI.Toggle(labelRect, m_ViewString, "", "label");
                if (m_ViewString)
                {
                    property.stringValue = EditorGUI.TextField(position, label, property.stringValue);
                    return;
                }
                MonoScript script = null;
                string typeName = property.stringValue;
                if (!string.IsNullOrEmpty(typeName))
                {
                    m_ScriptCache.TryGetValue(typeName, out script);
                    if (script == null)
                        GUI.color = Color.red;
                }

                script = (MonoScript)EditorGUI.ObjectField(position, label, script,  typeof(MonoScript), false);
                if (GUI.changed)
                {
                    if (script != null)
                    {
                        var type = script.GetClass();
                        MonoScriptAttribute attr = (MonoScriptAttribute)this;
                        if (attr.type != null && !attr.type.IsAssignableFrom(type))
                            type = null;
                        if (type != null)
                            property.stringValue = script.GetClass().AssemblyQualifiedName;
                        else
                            Debug.LogWarning("The script file " + script.name + " doesn't contain an assignable class");
                    }
                    else
                        property.stringValue = "";
                }
            }
            else
            {
                GUI.Label(position, "The MonoScript attribute can only be used on string variables");
            }

            propertyAlreadyDrawn = true;
            CallNextAttribute(position, property, label);
        }

        public override float GetPropertyLocalHeight(SerializedProperty property, GUIContent label)
        {
            return SIZE_LINE;
        }

  

#endif


    }

}




