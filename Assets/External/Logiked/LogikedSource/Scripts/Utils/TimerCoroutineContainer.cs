using System.Collections;
using UnityEngine;

/// <summary>
/// Conatiner pour des Couroutine Unity, afin de pouvoir utiliser des couroutines un peut partout
/// </summary>
internal class TimerCoroutineContainer : MonoBehaviour
{
    /// <summary>
    /// Démarrer une coroutine
    /// </summary>
    /// <param name="cor">La fonction de la coroutine</param>
    /// <returns>Coroutine</returns>
    public Coroutine StartCor(IEnumerator cor)
    {
        return StartCoroutine(cor);
    }

    /// <summary>
    /// Stopper une Coroutine
    /// </summary>
    /// <param name="cor">La coroutine</param>
    public void StopCor(Coroutine cor)
    {
        StopCoroutine(cor);
    }



}