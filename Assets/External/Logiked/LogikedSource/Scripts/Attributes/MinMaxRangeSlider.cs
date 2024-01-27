using logiked.source.utilities;
using UnityEngine;
using logiked.source.attributes.root;
using logiked.source.extentions;
#if UNITY_EDITOR
using logiked.source.editor;
using UnityEditor;
#endif

namespace logiked.source.attributes
{
    /// <summary>
    /// Permet d'avoir un slider de Range min-max. A poser sur un Vector2
    /// </summary>
    public class MinMaxRangeSlider : FutureFieldAttribute
    {
        public float min;
        public float? max;

        /// <summary>
        /// Si vous souhaitez clamp la valeur min autorisée, selon une autre variable, specifier son nom ici
        /// </summary>
        public string minConstraintFloat;
        /// <summary>
        /// Si vous souhaitez clamp la valeur max autorisée, selon une autre variable, specifier son nom ici
        /// </summary>
        public string maxConstraintFloat;

        public MinMaxRangeSlider(float min, float max)
        {
            this.min = min;
            this.max = max;
        }


#if UNITY_EDITOR

        protected override void OnGUIRecursive(Rect position, UnityEditor.SerializedProperty property, GUIContent label, AttributeContext Context)
        {

            float minAuth = min;
            float maxAuth = max.GetValueOrDefault();

            if (!minConstraintFloat.IsNullOrEmpty())
            {
               var minReflect = property.GetParent().GetReflectedValueAtPath(minConstraintFloat);   
                if (minReflect != null)                
                    minAuth = (float)minReflect.Value;                
            }

            if (!maxConstraintFloat.IsNullOrEmpty())
            {
                var maxReflect = property.GetParent().GetReflectedValueAtPath(maxConstraintFloat);
                if (maxReflect != null)
                    maxAuth = (float)maxReflect.Value;
            }



            position.height = EditorGUIUtility.singleLineHeight;
            var type = property.propertyType;

            string minName = "";//att.minName.Replace("$", property.name);
            int lastDot = property.propertyPath.LastIndexOf('.');
           if (lastDot > -1)
               minName = property.propertyPath.Substring(0, lastDot) + '.' + minName;
            //Debug.Log("minName=" + minName);
         
           // var ctrlRect = position;

            if (type == SerializedPropertyType.Float)
                //label.text = string.Format("({1}-{0})", property.name, minName);
                label.text = " ";


          //  var controlID = GUIUtility.GetControlID(FocusType.Passive, position);
           // var ctrlRect = EditorGUI.PrefixLabel(position, controlID, label);

           EditorGUI.LabelField(position, label);
            var ctrlRect = position;
            ctrlRect.x += EditorGUIUtility.labelWidth;
            ctrlRect.width -= EditorGUIUtility.labelWidth;


            Rect[] r = SplitRectIn3(ctrlRect, 50, 1);

            if (type == SerializedPropertyType.Vector2)
            {
                EditorGUI.BeginChangeCheck();
                var vec = property.vector2Value;
                var origVec = vec;
                float valueMin = Mathf.Max(vec.x, minAuth);
                float valueMax = Mathf.Min(vec.y, maxAuth);
            
                valueMin = EditorGUI.FloatField(r[0], valueMin);
                valueMax = EditorGUI.FloatField(r[2], valueMax);
                EditorGUI.MinMaxSlider(r[1], ref valueMin, ref valueMax, min, max ?? valueMax);
                vec = new Vector2(valueMin < valueMax ? valueMin : valueMax,    valueMax);
                if (valueMin > valueMax) vec.y = valueMin;
             

                if (EditorGUI.EndChangeCheck() || vec.x != origVec.x || vec.y != origVec.y)
                {
                    property.vector2Value = vec;
                    property.serializedObject.ApplyModifiedProperties();
                }






            }
            else if (type == SerializedPropertyType.Vector2Int)//Si ya des bugs, checker  origVec,  if (valueMin > valueMax) 
            {
                EditorGUI.BeginChangeCheck();
                var vec = property.vector2IntValue;
                var origVec = vec;
                float valueMin = vec.x;
                float valueMax = vec.y;
                valueMin = EditorGUI.IntField(r[0], (int)valueMin);
                valueMax = EditorGUI.IntField(r[2], (int)valueMax);
                EditorGUI.MinMaxSlider(r[1], ref valueMin, ref valueMax, min, max ?? valueMax);
                vec = new Vector2Int(Mathf.RoundToInt(valueMin < valueMax ? valueMin : valueMax), Mathf.RoundToInt(valueMax));
                if (valueMin > valueMax) vec.y = valueMin.Rnd();

                if (EditorGUI.EndChangeCheck() || vec.x != origVec.x || vec.y != origVec.y)
                    property.vector2IntValue = vec;
            }
            else if (type == SerializedPropertyType.Float)//Min max avec le nom de variable pas implémenté
            {
                EditorGUI.BeginChangeCheck();
                // Line setup
                var line2 = position;
                line2.y += EditorGUIUtility.singleLineHeight;

                // Swap lines
                // Comment these 3 lines if you want the slider above
                // Or uncomment them if you want the slider sandwiched between min and max
                //var y = line2.y;
                //line2.y = r[0].y;
                //r[0].y = r[1].y = r[2].y = y;

                // First we draw the float below/above as normal
                EditorGUI.PropertyField(line2, property);

                // Then the slider               
                var minProperty = property.serializedObject.FindProperty(minName);
                if (minProperty?.propertyType != SerializedPropertyType.Float)
                {
                    EditorGUI.HelpBox(ctrlRect, "Min float not found!!", MessageType.Info);
                    return;
                }
                float minVal = minProperty.floatValue;
                float maxVal = property.floatValue;

                EditorGUI.MinMaxSlider(r[1], ref minVal, ref maxVal, min, max ?? maxVal);
                EditorGUI.LabelField(r[0], min.ToString());

                if (max.HasValue && maxVal > max.Value)
                {
                    // Shows that the max value overflowed the slider
                    // So if you just wanna try infinite range and stuff you just put 999
                    // and it shows clearly that it is a big test value with the color
                    // This is only if you specify a max value in the attribute
                    Color c = GUI.contentColor;
                    GUI.contentColor = overflowColor;
                    EditorGUI.LabelField(r[2], maxVal.ToString());
                    GUI.contentColor = c;
                }
                else
                    EditorGUI.LabelField(r[2], (max ?? maxVal).ToString());

                // Rounding you lose a tiny bit of precision but I don't mind
                // it is just to show 0.84 instead of ugly 0.840041..
                // unless you're in very small values (<0.1) then it doesn't round
                minVal = FRound(minVal);
                maxVal = FRound(maxVal);

                // Proofcheck
                maxVal = Mathf.Max(min, maxVal);
                minVal = Mathf.Clamp(minVal, min, maxVal);

                // And finally update the variables
                if (EditorGUI.EndChangeCheck())
                {
                    minProperty.floatValue = minVal;
                    property.floatValue = maxVal;
                }
            }
            else
                EditorGUI.HelpBox(ctrlRect, "MinTo is for Vector2 or float!!", MessageType.Error);


            propertyAlreadyDrawn = true;
            CallNextAttribute(position, property, label);

        }

        const float threshold = .1f;
        const float precision = .01f;
        float FRound(float f) => f > threshold ? Mathf.Floor(f / precision) * precision : f;



        /*
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            int lines = 1;
            if (property.propertyType == SerializedPropertyType.Float)
                lines = 2;
            return lines * EditorGUIUtility.singleLineHeight;
        }*/

        // That's orange
        private Color overflowColor = new Color(1f, .55f, .1f, 1);

        public static Rect[] SplitRectIn3(Rect rect, int floatSize, int space = 0)
        {
            Rect[] end = new Rect[3] { rect, rect, rect };

             

            end[0].width =  floatSize;
            end[2].width =  floatSize;
            end[1].width -= floatSize * 2 + space * 2;

            end[1].x += floatSize + space;
            end[2].x += floatSize + 2f * space + end[1].width;

            return end;
        }



    





    public override float GetPropertyLocalHeight(SerializedProperty property, GUIContent label)
        {
              return SIZE_LINE;
        }


#endif

    }
}