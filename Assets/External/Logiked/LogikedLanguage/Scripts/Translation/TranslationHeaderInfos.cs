using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace logiked.language.translate
{

    /// <summary>
    /// Information internes du patch (Auteur, Version, Langue..)
    /// </summary>
    [Serializable]
    public class TranslationHeaderInfos
    {
        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="language">Code langue du patch (ex : En_US, FR_fr)</param>
        /// <param name="author">Auteur du patch</param>
        /// <param name="version">Version du patch</param>
        /// <param name="descritpion">Description brève</param>
        public TranslationHeaderInfos(string language, string author, string version, string descritpion)
        {
            Language = language;
            Author = author;
            Version = version;
            Description = descritpion;
        }


        /// <summary>
        /// Code language du fichier
        /// </summary>
        public string Language
        {
            get;
#if UNITY_EDITOR
                set;
#endif
        }


        /// <summary>
        /// Autheur du fichier
        /// </summary>
        public string Author
        {
            get;
#if UNITY_EDITOR
                set;
#endif
        }

        /// <summary>
        /// Auteur du fichier
        /// </summary>
        public string Version
        {
            get;
#if UNITY_EDITOR
                set;
#endif
        }

        /// <summary>
        ///  Description brève fournie par le créateur du patch
        /// </summary>
        public string Description
        {
            get;
#if UNITY_EDITOR
                set;
#endif
        }


    }
}

