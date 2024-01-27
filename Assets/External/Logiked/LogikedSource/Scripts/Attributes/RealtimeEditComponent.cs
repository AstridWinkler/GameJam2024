using UnityEngine;
using System;
using System.Reflection;
using logiked.source.attributes.root;
using logiked.source.extentions;

#if UNITY_EDITOR
using UnityEditor;
using logiked.source.editor;
#endif


namespace logiked.source.attributes
{

    /// <summary>
    /// Permet d'editer un composant particulier sur un prefab, dans le même inspector
    /// </summary>
    public class RealtimeEditComponent : FutureFieldAttribute
    {

        public System.Type componentType;

        public bool sectionOpen;



        /// <param name="component">Le type du composant à editer sur le prefab</param>
        public RealtimeEditComponent(System.Type component) { this.componentType = component; }


#if UNITY_EDITOR


        Editor behaviourPrefabEditor;

        protected override void OnGUIRecursive(Rect position, SerializedProperty property, GUIContent label, AttributeContext Context)
        {



            position = CallNextAttribute(position, property, label, IncrementRectMode.Afterdrawing);

      


            GameObject obj;
            Component t;
        
            var inArray = property.IsInArray();

            if ((property.propertyType.HasFlag(SerializedPropertyType.ObjectReference)))
            {

                obj = property.objectReferenceValue as GameObject;
                if (obj == null) return;

                t = obj.GetComponent(componentType);

                var pos2 = position;

                pos2.x += EditorGUIUtility.labelWidth;
                pos2.width = pos2.width - EditorGUIUtility.labelWidth;

                if (t == null)
                {
                    // EditorGUILayout.HelpBox("No component of type {0} found on this GameObject !".Format(componentType.Name), MessageType.Error, true);
                    Context.AttributeHeight = 34f;
                    GUI.Box(pos2, new GUIContent("No component of type <{0}> found on this GameObject !".Format(componentType.Name)),GUILogiked.Styles.Box_Warning1);

                    return;
                }


                if (inArray)
                {
                    Context.AttributeHeight = 34f;
                    GUI.Box(pos2, new GUIContent("Component <{0}> can't be edited in array.".Format(componentType.Name)), GUILogiked.Styles.Box_HelpBox1);
          
                    return;

                }


                //GUI.color = Color.gray;



                //GUI.color = Color.white;



                Context.AttributeHeight = SIZE_LINE;



                pos2.height = SIZE_LINE;
               

                if (GUI.Button(pos2, new GUIContent("Edit <{0}> component".Format(t.GetType().Name)), GUILogiked.Styles.Button_Magenta))
                {
                    sectionOpen = !sectionOpen;
                }

                GUI.enabled = true;



                


   

                    //   else if (behaviourPrefabEditor != null) DestroyImmediate(behaviourPrefabEditor);



                 
             

                        GUILayout.Space(2);

               

                        if (behaviourPrefabEditor == null)
                        {

                            behaviourPrefabEditor = Editor.CreateEditor(t);
                            behaviourPrefabEditor.name = "editest";
                        //    Debug.LogWarning("Generate : " + behaviourPrefabEditor.name);

                        }



                if (sectionOpen)
                {
                    GUILayout.BeginVertical(GUILogiked.Styles.Box_Border); ;
                    GUILayout.Label($"Editing component {t.GetType().Name}", GUILogiked.Styles.Text_BigBold);

                    EditorGUI.BeginChangeCheck();
                    behaviourPrefabEditor.OnInspectorGUI();


                    GUILayout.EndVertical();
                    GUILayout.Space(2);
                }

               

                    





       

            }



        }












#endif
    }
}








