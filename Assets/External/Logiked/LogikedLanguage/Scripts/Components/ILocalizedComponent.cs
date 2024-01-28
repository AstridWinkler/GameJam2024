using UnityEditor;
using UnityEngine;

namespace logiked.language.translate
{

    /// <summary>
    /// Interface à utiliser sur les composants qui affichent du texte localisé.
    /// Les composants ayant cette interface sont refresh automatiquement par <see cref="TranslationManager.UpdateSceneLocalizedStrings"/>, appelé au rechargement d'un patch de langues.
    /// 
    ///<example>
    ///
    /// <code>
    ///    public class MyComponent : ILocalizedComponent
    ///    {
    ///        lstring localizedString;
    ///        TextMesh textRender;
    ///
    ///
    ///        public void RefreshTranslate()
    ///        {          
    ///            textRender.text = localizedString;   // Cast du nouveau texte modifié rechargé
    ///        }
    ///    }
    ///</code> 
    ///</example>
    ///
    /// </summary>


    public interface ILocalizedComponent
    {
        /// <summary>
        /// Fonction à implémenter pour refresh le texte localisé. Appelée automatiquement par <see cref="TranslationManager.UpdateSceneLocalizedStrings"/>, au rechargement d'un patch de langues.
        /// </summary>
        void RefreshTranslate();

    }
}