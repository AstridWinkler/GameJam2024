using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace logiked.source.extentions {

    /// <summary>
    /// Interface nulle pour que des composantes logiked puissent avoir de jolis logs
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
        /// <summary>
        /// Debug un texte coloré
        /// </summary>
        /// <param name="text"></param>
        /// <param name="color"></param>
        /// <param name=""></param>
        public static void Log(string text, Color color, string prefix = default, string suffix = default, int error = 0)
        {
#if DEBUG
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


            switch (error) {
                case 0: Debug.Log(end.ToString()); break;
                case 1: Debug.LogWarning(end.ToString()); break;
                case 2: Debug.LogError(end.ToString()); break;

        }
#endif

        }
        public static void LogWarning(string text, Color color, string prefix = default, string suffix = default) => Log(text, color, prefix, suffix, 1);
        public static void LogError(string text, Color color, string prefix = default, string suffix = default) => Log(text, color, prefix, suffix, 2);




    }

}
