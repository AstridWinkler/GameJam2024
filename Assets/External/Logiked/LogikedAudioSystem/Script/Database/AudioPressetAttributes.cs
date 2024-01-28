using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



using UnityEngine;
using logiked.source.attributes.root;

#if UNITY_EDITOR
using UnityEditor;
using logiked.source.editor;
using logiked.editor;
#endif

namespace logiked.audio
{
    /// <summary>
    /// Attribut pour pouvoir selectionner un pressets d'audio source disponible dans la BDD sur un entier
    /// </summary>
    public class AudioSourcePressetAttribute : FutureFieldAttribute
    {

#if UNITY_EDITOR
        protected override void OnGUIRecursive(Rect position, SerializedProperty property, GUIContent label, AttributeContext Context)
        {

            if (LogikedPlugin_AudioSystem.Instance.DefaultAudioDatabase != null)
            {
                if (LogikedPlugin_AudioSystem.Instance.DefaultAudioDatabase.AudioSourcePresetPrefabNames.Length > 0)
                {
                    if (property.intValue == -1) property.intValue = 0;
                    property.intValue = EditorGUILayout.Popup(label, property.intValue, LogikedPlugin_AudioSystem.Instance.DefaultAudioDatabase.AudioSourcePresetPrefabNames);
                }
                else
                {
                    EditorGUILayout.HelpBox("Aucun preset trouvé dans l'AudioDatabase", MessageType.Warning, true);
                    if(GUILayout.Button("Goto Fix"))
                        ProjectBrowserReflection.SelectAssetInProjectWindow(LogikedPlugin_AudioSystem.Instance.DefaultAudioDatabase);

                    property.intValue = -1;

                }
            }
            else
            {
                EditorGUILayout.HelpBox("Aucune AudioDatabase n'est configurée pour votre projet. Allez dans Logiked->Helper windows.", MessageType.Warning, true);
                    if(GUILayout.Button("Goto Fix"))
                    ProjectBrowserReflection.SelectAssetInProjectWindow(LogikedPlugin_AudioSystem.Instance);
                property.intValue = -1;// EditorGUILayout.IntField(label, property.intValue);
            }

            propertyAlreadyDrawn = true;
            CallNextAttribute(position, property, label);

        }
#endif
    }


    /// <summary>
    /// Attribut pour pouvoir selectionner un pressets d'audio Mixer disponible dans la BDD sur un entier
    /// </summary>
    public class AudioMixerAttribute : FutureFieldAttribute
    {

#if UNITY_EDITOR
        protected override void OnGUIRecursive(Rect position, SerializedProperty property, GUIContent label, AttributeContext Context)
        {

            if (LogikedPlugin_AudioSystem.Instance.DefaultAudioDatabase != null)
            {
                if (LogikedPlugin_AudioSystem.Instance.DefaultAudioDatabase.AudioMixerLayersNames.Length > 0)
                {
                    if (property.intValue == -1) property.intValue = 0;
                    property.intValue = EditorGUILayout.Popup(label, property.intValue, LogikedPlugin_AudioSystem.Instance.DefaultAudioDatabase.AudioMixerLayersNames);
                }
                else
                {
                    /* EditorGUILayout.HelpBox("Aucun mixer trouvé dans l'AudioDatabase", MessageType.Warning, true);
                     if (GUILayout.Button("Goto Fix"))
                         Logiked_AssetsExtention.SelectObjectInUnityInspector(LogikedPlugin_AudioSystem.Instance.DefaultAudioDatabase);
                    */
                    GUI.enabled = false;
                    EditorGUILayout.Popup(label, 0, new string[] { "None" });
                    GUI.enabled = true;
                    property.intValue = -1;

                }
            }
            else
            {
                EditorGUILayout.HelpBox("Aucune AudioDatabase n'est configurée pour votre projet. Allez dans Logiked->Helper windows.", MessageType.Warning, true);
                if (GUILayout.Button("Goto Fix"))
                    ProjectBrowserReflection.SelectAssetInProjectWindow(LogikedPlugin_AudioSystem.Instance);
                property.intValue = -1;// EditorGUILayout.IntField(label, property.intValue);
            }

            propertyAlreadyDrawn = true;
            CallNextAttribute(position, property, label);


        }
#endif
    }




}


