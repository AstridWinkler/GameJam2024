using logiked.source.attributes.root;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace logiked.language.translate
{

    /// <summary>
    /// Attribut qui permet d'afficher un message d'erreur dans l'inspecteur si aucun patch de traduction par défaut n'a été défini
    /// 
    /// <example>
    /// <code>
    ///    //Script nécessitant un patch de traduction correctement configuré  
    ///    public class MyCustomDialogSystem : MonoBehaviour
    ///    {
    ///        //Propose dans l'inespector de générer un patch de traduction si aucun n'a été crée
    ///        [TranslationFileCheck]
    ///        
    ///
    ///        [SerializeField]
    ///        private lstring text1;
    ///        [SerializeField]
    ///        private lstring text2;
    ///        [SerializeField]
    ///        private lstring text3;
    ///        [SerializeField]
    ///        private lstring text4;
    ///    }
    ///
    /// </code>
    /// 
    /// <note>
    /// Ce script pourrait implémenter <see cref="ILocalizedComponent"/>
    /// </note>
    /// 
    /// </example>
    /// 
    /// </summary>
    public class TranslationFileCheck : FutureFieldAttribute
    {


#if UNITY_EDITOR
        protected override void OnGUIRecursive(Rect position, SerializedProperty property, GUIContent label, AttributeContext Context)
        {
            TranslationManager.CheckAndDrawDefaultTranslateHelper();
            CallNextAttribute(position, property, label);
        }
#endif
    }


}

