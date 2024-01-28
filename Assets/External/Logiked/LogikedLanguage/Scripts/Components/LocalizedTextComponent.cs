using logiked.source.attributes;
using logiked.source.utilities;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace logiked.language.translate {


    /// <summary>
    /// Composant autonome qui affichage une chaine localisé sur un renderer de texte Unity (<see cref="UnityEngine.TextMesh"/>, <see cref="UnityEngine.UI.Text"/>, <see cref="TMPro.TextMeshPro"/>, <see cref="TMPro.TextMeshProUGUI"/>)
    /// </summary>
    [AddComponentMenu("Logiked/Localized UI Text")]
    [DisallowMultipleComponent]
    [ExecuteAlways]
    public class LocalizedTextComponent : MonoBehaviour, ILocalizedComponent
    {

        /// <summary>
        /// La chaine localisée à afficher
        /// </summary>
        [Tooltip("La chaine localisée à afficher")]
        [SerializeField]
        private lstring text;


        /// <summary>
        /// Type de texte sur lequel afficher la traduction
        /// </summary>
        private enum TextType
        {
            /// <summary>
            /// Un composant <see cref="Text"/> de canvas Unity
            /// </summary>
            LegacyText=0,
            /// <summary>
            /// Un composant <see cref="UnityEngine.TextMesh"/>  de Unity
            /// </summary>
            LegacyTextMesh = 1,
            /// <summary>
            /// Un composant <see cref="TMPro.TextMeshPro/> de Unity
            /// </summary>
            TextMeshPro=2,
            /// <summary>
            /// Un composant <see cref="TMPro.TextMeshProUGUI/> de Unity
            /// </summary>
            TextMeshProUGUI = 3

        }

        /// <inheritdoc cref="TextType"/>
        [SerializeField]
        private TextType usedTextType;

                


        [Tooltip("Le composant utilisé pour l'affichage de la chaine")]
        [SerializeField]
        [ShowIf(nameof(usedTextType), "==", TextType.LegacyText)]
        private Text textUi;


        [Tooltip("Le composant utilisé pour l'affichage de la chaine")]
        [SerializeField]
        [ShowIf(nameof(usedTextType), "==", TextType.TextMeshPro)]
        private TextMeshPro textMeshPro;


        [Tooltip("Le composant utilisé pour l'affichage de la chaine")]
        [SerializeField]
        [ShowIf(nameof(usedTextType), "==", TextType.LegacyTextMesh)]
        private TextMesh textMesh;

        [Tooltip("Le composant utilisé pour l'affichage de la chaine")]
        [SerializeField]
        [ShowIf(nameof(usedTextType), "==", TextType.TextMeshProUGUI)]
        private TextMeshProUGUI textMeshProUGUI;





        /// <summary>
        /// Le composant utilisé pour l'affichage de la chaine
        /// </summary>
        public Text TextUi => textUi;

        /// <inheritdoc cref="TextUi"/>
        public TextMesh TextMesh => textMesh;

        /// <inheritdoc cref="TextUi"/>
        public TextMeshPro TextMeshPro => textMeshPro;

        /// <inheritdoc cref="TextUi"/>
        public TextMeshProUGUI TextMeshProUGUI => textMeshProUGUI;

       



        private bool ApplyText(Object comp, ref string str)
        {
            if (comp == null) return false;
            str = text.GetTranslated(); 
            return true;
        }



        /// <inheritdoc cref="ILocalizedComponent.RefreshTranslate"/>
        public void RefreshTranslate()
        {

#if UNITY_EDITOR
            if (TranslationManager.DefaultTranslate == null || text == null)
                return;
#endif

            switch (usedTextType)
            {
                case TextType.LegacyText:
                    if (textUi == null) break;
                    textUi.text = text.GetTranslated();
                    return;

                case TextType.LegacyTextMesh:
                    if (TextMesh == null) break;
                    TextMesh.text = text.GetTranslated();
                    return;

                case TextType.TextMeshPro:
                    if (TextMeshPro == null) break;
                    TextMeshPro.text = text.GetTranslated();
                    return;

                case TextType.TextMeshProUGUI:
                    if (TextMeshProUGUI == null) break;
                    TextMeshProUGUI.text = text.GetTranslated();
                    return;
            }



            ///Search component

            bool changed = false;


            textUi = GetComponent<Text>();
            if (textUi != null && (changed = true))
                usedTextType = TextType.LegacyText;

            textMesh = GetComponent<TextMesh>();
            if (textMesh != null && (changed = true))
                usedTextType = TextType.LegacyTextMesh;

            textMeshPro = GetComponent<TextMeshPro>();
            if (textMeshPro != null && (changed = true))
                usedTextType = TextType.TextMeshPro;


            textMeshProUGUI = GetComponent<TextMeshProUGUI>();
            if (textMeshProUGUI != null && (changed = true))
                usedTextType = TextType.TextMeshProUGUI;
        

            if (changed)
                RefreshTranslate();


        }


        private void OnEnable()
        {
            RefreshTranslate();
        }



#if UNITY_EDITOR
        /// <summary>
        /// Auto référencement
        /// </summary>
        private void Update()
        {
            if (!Application.isPlaying)
                RefreshTranslate();
        }
#endif

    }
}
