using logiked.source.extentions;
using logiked.source.utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace logiked.language.translate
{


    /// <summary>
    /// Type de base des chaines localisés. A utiliser dans vos scripts comme une chaîne de caractère standard.
    /// 
    /// <example>
    /// <code>
    ///    class Script : MonoBehaviour
    ///    {
    ///
    ///        [SerializeField]
    ///        private lstring localizedText; // Chaine traduite éditable dans unity
    ///
    ///    }
    /// </code>
    /// 
    /// Les <c>lstring</c> sont modifiables depuis unity; elles contienent un duo (clé; valeur), automatiquement enregistrés vers le patch de traduction spécifié dans <see cref="LogikedPlugin_Language.defaultTranslation"/>. Consulter la section tutorial pour plus de détails. 
    /// 
    /// Les <c>lstring</c> sont castables directement en <c>string</c>
    ///  
    /// <code>
    ///        void Test()
    ///        {
    ///            //Cast de la traduction du texte
    ///            string currentTranslatedText = localizedText;
    ///        }
    /// </code>
    /// </example>    
    /// 
    /// </summary>
    [Serializable]
    public class lstring : OnRemoveObjectEvent
    {



        /// <param name="content">Contenu brut de la chaine (sans traduction). Ce constructeur sert principalement à gerer les conversions Strings/Lstrings sans se soucier de la traduction de la chaîne.</param>
        public lstring(string content = "")
        {
            this.content = content;
        }




        /// <summary>
        /// Le contenu de cette chaine, non traduit
        /// </summary>
        [SerializeField]
        [Multiline]
        private string content;

        /// <inheritdoc cref="TranslateKey"/>
        [SerializeField]
        private string translateId;

        /// <summary>
        /// La clé de la traduction de cette chaîne, contenue dans le patch de traduction.
        /// </summary>
        public string TranslateKey => translateId;

        /// <summary>
        /// Obtenir le texte traduit. 
        /// Cast la chaine en string retourne le même résultat.
        /// </summary>
        /// <returns>Texte traduit</returns>
        public string GetTranslated()
        {
            return translateId.IsNullOrEmpty() ? content : TranslationManager.GetTranslatedValue(translateId);
        }

#if UNITY_EDITOR

        /// <inheritdoc/>
        public void OnRemoveObjectEvent()
        {
            DestroyKeyDialog();
        }

        /// <summary>
        /// [EDITOR ONLY] Assigner une nouvelle clé/valeur à cette chaîne et l'enregistre dans le patch de traduction
        /// </summary>
        /// <param name="key">La clé à enregistrer. Est créee si inexistante</param>
        /// <param name="value">la valeur à enregistrer</param>
        /// <param name="poEditComments">Les commentaires au format PoEdit à enregistrer dans le patch</param>
        public void SetAndSaveValues(string key, string value, string poEditComments = "")
        {
            content = value;
            translateId = key;
            TranslationManager.DefaultTranslate.SaveValue(key, value, true, poEditComments);
        }

        /// <summary>
        /// [EDITOR ONLY] Permet de suprimer la clé de la traduction de cette chaîne, avec un popup pour que l'utilisateur puisse confirmer.
        /// </summary>
        /// <param name="force">Ne pas afficher le popup et supprimer immédiatement la chaîne ?</param>
        public void DestroyKeyDialog(bool force = false)
        {
            if (!TranslationManager.DefaultTranslate.HasKey(translateId)) return;
            if (force || UnityEditor.EditorUtility.DisplayDialog("Remove translation key", $"Do you want to remove {translateId} key from the currentTranslation ?", "yes", "no"))
                TranslationManager.DefaultTranslate.RemoveKey(translateId);
        }
#endif

        /// <summary>
        /// Conversion automatique en String traduite
        /// </summary>
        /// <param name="d">La Lstring à traduire</param>
        public static implicit operator string(lstring d) => d.GetTranslated();

        /// <summary>
        /// Conversion automatique en Lstring des chaînes
        /// </summary>
        /// <param name="d">La String de base</param>
        public static implicit operator lstring(string s) => new lstring(s);


        /// <inheritdoc/>
        public override bool Equals(object obj) { return obj is string && content.Equals(obj); }
        /// <inheritdoc/>
        public override int GetHashCode() { return content.GetHashCode(); }
        /// <inheritdoc/>
        public override string ToString() { return content.ToString(); }



    }
}