
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;


/// <summary>
/// Cette classe semble inutile, mais non ! Elle permet d'override l'integralité des custom inspector, et permet d'autoriser l'utilisation des
/// fonction GuiLayout (sans explosions) dans les custom renderers. 
/// </summary>
[CustomEditor(typeof(UnityEngine.Object), true, isFallback = true)]
[CanEditMultipleObjects]
public class DefaultGlobalInspector : Editor { }

#endif
