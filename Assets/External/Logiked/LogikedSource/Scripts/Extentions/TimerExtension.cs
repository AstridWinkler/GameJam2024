using logiked.source.types;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace logiked.source.extentions
{

    /// <summary>
    /// Extension de la classe Timer 
    /// </summary>
    public static class TimerExtention
    {

        /// <summary>
        /// Permet de checker si un timer est Null ou Inactif (même sur une variable non assignée).
        /// </summary>
        /// <param name="t">Le timer (peut etre null)</param>
        /// <returns>Le timer est Null ou Inactif</returns>
        public static bool IsNullOrInactive(this GameTimer t)
        {
            return t == null || !t.IsRunning;
        }

        /// <summary>
        /// Permet de checker si un timer est défini && actif (même sur une variable non assignée).
        /// </summary>
        /// <param name="t">Le timer (peut etre null)</param>
        /// <returns>Le timer est défini et assigné</returns>
        public static bool IsDefinedAndActive(this GameTimer t)
        {
            return t != null && t.IsRunning;
        }



        /// <summary>
        /// Permet de stopper un timer (même sur une variable non assignée).
        /// </summary>
        /// <param name="t">Le timer (peut etre null)</param>
        public static void AutoStop(this GameTimer t)
        {
            if( t != null ) t.Stop();
        }


    }

}