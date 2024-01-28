using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace logiked.source.extentions {

    /// <summary>
    /// Interface random pour que des composantes logiked puissent avoir de jolis logs
    /// </summary>
    internal interface ICustomLog
    {
        void Log(string txt);
    }

    /// <summary>
    /// Extensions pour faire de joli logs
    /// </summary>
    public static class DebugC
    {

        public enum ErrorLevel { Log=0, Warning=1, Error=2}


        /// <summary>
        /// Debug un texte coloré dans la console
        /// </summary>
        /// <param name="text">Le message à afficher</param>
        /// <param name="color">La couleur du message</param>
        /// <param name="prefix">Nom à afficher avant le message. Est affiché entre crochet : [prefix] </param>
        /// <param name="suffix">Texte affiché à la fin du message, sur une nouvelle ligne</param>
        /// <param name="errorLevel">Niveau d'erreur du message (log, warning, error) : <see cref="ErrorLevel"/> </param>
        /// <param name="context">Objet que le message concerne </param>

        public static void Log(string text, Color color, string prefix = default, string suffix = default, ErrorLevel errorLevel =  ErrorLevel.Log, Object context = null)
        {
#if DEBUG

            switch (errorLevel)
            {
                case ErrorLevel.Warning: color = Color.Lerp(color, Color.yellow, .5f); break;
                case ErrorLevel.Error: color = Color.Lerp(color, Color.red, .5f);  break;
            }

            StringBuilder end = new StringBuilder();

            end.Append($"<color=#{ColorUtility.ToHtmlStringRGB(color)}>");

            if (!prefix.IsNullOrEmpty())
                end.Append($"<size=13><b>[{prefix}]</b></size> ");

            end.Append(text);

            if (!prefix.IsNullOrEmpty())
            {
                end.Append("\n");
                end.Append(suffix);
            }
            end.Append($"</color>");


            switch (errorLevel) {
                case  ErrorLevel.Log: Debug.Log(end.ToString(), context); break;
                case  ErrorLevel.Warning: Debug.LogWarning(end.ToString(), context); break;
                case  ErrorLevel.Error: Debug.LogError(end.ToString(), context); break;

        }
#endif
        }

        /// <summary>
        /// Debug un texte coloré de warning dans la console
        /// </summary>
        /// <param name="text">Le message à afficher</param>
        /// <param name="color">La couleur du message</param>
        /// <param name="prefix">Nom à afficher avant le message. Est affiché entre crochet : [prefix] </param>
        /// <param name="suffix">Texte affiché à la fin du message, sur une nouvelle ligne</param>
        /// <param name="context">Objet que le message concerne </param>

        public static void LogWarning(string text, Color color, string prefix = default, string suffix = default, Object context = null) => Log(text, color, prefix, suffix,  ErrorLevel.Warning, context);

        /// <summary>
        /// Debug un texte coloré d'erreur dans la console
        /// </summary>
        /// <param name="text">Le message à afficher</param>
        /// <param name="color">La couleur du message</param>
        /// <param name="prefix">Nom à afficher avant le message. Est affiché entre crochet : [prefix] </param>
        /// <param name="suffix">Texte affiché à la fin du message, sur une nouvelle ligne</param>  
        /// <param name="context">Objet que le message concerne </param>
        public static void LogError(string text, Color color, string prefix = default, string suffix = default, Object context = null) => Log(text, color, prefix, suffix,  ErrorLevel.Error, context);




    }

}
