using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using logiked.source.extentions;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace logiked.source.types
{

    /// <summary>
    /// Un float avec une variation. A chaque fois que ce float est cast, une nouvelle valeur est générée à partir de la variation. <br></br>
    /// Attention : aucun moyen d'étre connecté à une seed actuellement
    /// </summary>
    [System.Serializable]
    public struct vfloat
    {
        [SerializeField]
        public float value;
        [SerializeField]
        public float variation;



        public vfloat(float mainValue = 0) : this(mainValue, 0) { }

        public vfloat(float mainValue = 0, float variation = 0)
        {
            this.value = mainValue;
            this.variation = variation;
        }



        /// <summary>
        /// Valeur randomisées générée avec la variation
        /// </summary>
       public float ValueGenerated => value + new System.Random().Range(-variation, variation);


        /// <summary>
        /// Valeur randomisées générée par une seed
        /// </summary>
        public float FrozenValue(int seed) => value + new System.Random(seed).Range(-variation, variation);

        /// <summary>
        /// Valeur randomisées générée par un random custom
        /// </summary>
        public float FrozenValue(System.Random rand) => value + rand.Range(-variation, variation);





        /// <summary>
        /// Conversion automatique en float, retourne la valeur randomisée
        /// </summary>
        /// <param name="f">Le vfloat à convertir</param>
        public static implicit operator float(vfloat f) => f.ValueGenerated;
        /// <summary>
        /// Conversion automatique en vfloat.
        /// </summary>
        /// <param name="f">Le float à convertir</param>
        public static implicit operator vfloat(float f) => new vfloat(f);

        public override string ToString()
        {
            return value.ToString();
        }
    }


#if UNITY_EDITOR

    [CustomPropertyDrawer(typeof(vfloat))]
    public class Property_vfloat : PropertyDrawer
    {
        SerializedProperty X, Y;
        string name;
        bool cache = false;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!cache)
            {
                //get the name before it's gone
                name = property.displayName;

                //get the X and Y values
                property.Next(true);
                X = property.Copy();
                property.Next(true);
                Y = property.Copy();

                cache = true;
            }

        
            Rect contentPosition = EditorGUI.PrefixLabel(position, new GUIContent(name));

            //Check if there is enough space to put the name on the same line (to save space)
            if (position.height > 18f)
            {
                position.height = 18f;
                EditorGUI.indentLevel += 1;
                contentPosition = EditorGUI.IndentedRect(position);
                contentPosition.y += 20f;
            }

            float secondPart = contentPosition.width / 2;
            GUI.skin.label.padding = new RectOffset(3, 3, 4, 4);

            //show the X and Y from the point
            EditorGUIUtility.labelWidth = 0;
            contentPosition.width *= 0.5f;
            //EditorGUI.indentLevel = 1;


            if (X == null || label == null)
            {
                cache = false;
                return;
            }


            if (X == null || X.serializedObject == null || Y == null || Y.serializedObject == null) return;

            // Begin/end property & change check make each field
            // behave correctly when multi-object editing.
            EditorGUI.BeginProperty(contentPosition, label, X);
            {
                EditorGUI.BeginChangeCheck();
                float newVal = EditorGUI.FloatField(contentPosition, new GUIContent(), X.floatValue);
                if (EditorGUI.EndChangeCheck())
                    X.floatValue = newVal;
            }
            EditorGUI.EndProperty();

            EditorGUIUtility.labelWidth = 30;
            contentPosition.x += secondPart;

            EditorGUI.BeginProperty(contentPosition, label, Y);
            {
                EditorGUI.BeginChangeCheck();
                float newVal = EditorGUI.FloatField(contentPosition, new GUIContent("±", "Valeur de variation (plus ou moins cette valeur)"), Y.floatValue);
                if (EditorGUI.EndChangeCheck())
                    Y.floatValue = newVal;
            }
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return Screen.width < 333 ? (18f + 18f) : 18f;
        }
    }


#endif
}
