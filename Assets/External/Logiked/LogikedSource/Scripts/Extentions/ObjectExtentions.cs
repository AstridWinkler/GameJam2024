using logiked.source.manager;
using logiked.source.utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;


namespace logiked.source.extentions
{

    /// <summary>
    /// Des extentions pour les Objets de base de Unity.
    /// </summary>
    public static class ObjectsExtension
    {


        /// <summary>
        /// Créer une instance du GameObject à une position fournie
        /// </summary>
        /// <param name="gb">le gameObject</param>
        /// <param name="position">La position locale de l'objet</param>
        /// <param name="rotation">La rotation locale de l'objet</param>
        /// <returns>Le gameObject instancié</returns>
        public static GameObject Inst(this GameObject gb, Vector3 position, Quaternion rotation = default)
        {
            return gb.Inst(BaseGameManager.TempInstParent, position, rotation);
        }

        /// <summary>
        /// Créer une instance du GameObject à une position fournie
        /// </summary>
        /// <param name="gb">le gameObject</param>
        /// <param name="position">La position locale de l'objet</param>
        /// <param name="rotation">La rotation locale de l'objet</param>
        /// <returns>Le gameObject instancié</returns>
        public static GameObject Inst(this GameObject gb, Vector3 position, Vector3 rotation)
        {
            return gb.Inst(position, Quaternion.Euler(rotation));
        }


        /// <summary>
        /// Créer une instance du GameObject à une position fournie
        /// </summary>
        /// <param name="gb">le gameObject</param>
        /// <param name="position">La position locale de l'objet</param>
        /// <returns>Le gameObject instancié</returns>
        public static GameObject Inst(this GameObject gb, Vector3 position)
        {
            return gb.Inst(position, Quaternion.identity);
        }

        /// <summary>
        /// Créer une instance du GameObject à la position de son transform
        /// </summary>
        /// <param name="gb">le gameObject</param>
        /// <returns>Le gameObject instancié</returns>
        public static GameObject Inst(this GameObject gb)
        {
            return gb.Inst(gb.transform.position, gb.transform.rotation);
        }




        /// <summary>
        /// Créer une instance du GameObject à une position fournie
        /// </summary>
        /// <param name="gb">le gameObject</param>
        /// <param name="parent">Le nouveau parent de l'objet</param>
        /// <param name="position">La world position de l'objet</param>
        /// <param name="rotation">La world rotation de l'objet</param>
        /// <returns>Le gameObject instancié</returns>
        public static GameObject Inst(this GameObject gb, Transform parent, Vector3 position = default, Quaternion rotation = default)
        {
            var obj = GameObject.Instantiate(gb, parent);
            obj.transform.localPosition = position;
            obj.transform.localRotation = rotation;
            return obj;
        }


        /// <summary>
        /// Créer une instance du GameObject à une position fournie
        /// </summary>
        /// <param name="gb">le gameObject</param>
        /// <param name="parent">Le nouveau parent de l'objet</param>
        /// <param name="localPosition">La position locale de l'objet dans le parent</param>
        /// <param name="localRotation">La rotation locale de l'objet dans le parent</param>
        /// <returns>Le gameObject instancié</returns>
        public static GameObject Inst(this GameObject gb, Transform parent, Vector3 localPosition, Vector3 localRotation)
        {
            return gb.Inst(parent, localPosition, Quaternion.Euler(localRotation));
        }

        /// <summary>
        /// Cherche ou ajoute un composant sur le gameObject et le retourne.
        /// </summary>
        /// <typeparam name="T">Le type du composant en question</typeparam>
        /// <param name="gameObject">Cet objet</param>
        /// <returns>Le composant</returns>
        public static T AddOrGetComponent<T>(this GameObject gameObject) where T : Component
        {
            var g = gameObject.GetComponent<T>();
            if (g == null) g = gameObject.AddComponent<T>();
            return g;
        }

        /// <summary>
        /// Permet d'activer / désactiver properment un particle system, pour eviter les <b>gros</b> bugs unity, style particle system qui ne se ralume jamais.
        /// </summary>
        /// <param name="particles"></param>
        /// <param name="state"></param>
        public static void SetParticleActive(this ParticleSystem particles, bool state)
        {
            if (particles == null) return;

            if (!state && particles.isPlaying)
                particles.Stop();
            if (state && !particles.isEmitting)
                particles.Play();
        }

        /*

		public static GameObject Inst(this GameObject gb, Vector3 position, Transform parentObj)
		{
			return GameObject.Instantiate(gb, position, new Quaternion(), parentObj) ;
		}

		public static GameObject Inst(this GameObject gb, Transform positionObj, Transform parentObj)
		{
			return GameObject.Instantiate(gb, positionObj.position, positionObj.rotation, parentObj);
		}
		public static GameObject Inst(this GameObject gb, Transform positionObj)
		{
			return GameObject.Instantiate(gb, positionObj.position, positionObj.rotation, BaseGameManager.TempInstParent);
		}

		*/



        #region GAMEOBJECT_ICONS



        /// <summary>
        /// Type d'icone pour les GameObjects, dans la SceneView de l'inspecteur.
        /// </summary>
        public enum Icon
        {
            CircleGray = 0,
            CircleBlue,
            CircleTeal,
            CircleGreen,
            CircleYellow,
            CircleOrange,
            CircleRed,
            CirclePurple,
            DiamondGray,
            DiamondBlue,
            DiamondTeal,
            DiamondGreen,
            DiamondYellow,
            DiamondOrange,
            DiamondRed,
            DiamondPurple,
            LabelGray = 100,
            LabelBlue,
            LabelTeal,
            LabelGreen,
            LabelYellow,
            LabelOrange,
            LabelRed,
            LabelPurple
        }

        private static GUIContent[] labelIcons;
        private static GUIContent[] largeIcons;




        /// <summary>
        /// Applique l'icone au GameObject pour la visualisation dans l'editeur. (Cette fonction n'aura aucun effet dans l'executable) 
        /// </summary>
        /// <param name="gameObject">L'objet concerné</param>
        /// <param name="icon">L'icone à appliquer</param>
        public static void SetIcon(this GameObject gameObject, Icon icon)
        {
#if UNITY_EDITOR

            int iconId = (int)icon;

            if (iconId >= 100)
            {
                iconId -= 100;
                gameObject.SetIcon($"sv_label_{iconId}");
            }
            else
            {
                gameObject.SetIcon($"sv_icon_dot{iconId}_pix16_gizmo");

            }

#endif
        }

        private static void SetIcon(this GameObject gameObject, string contentName)
        {
#if UNITY_EDITOR
            GUIContent iconContent = UnityEditor.EditorGUIUtility.IconContent(contentName);
            SetIcon(gameObject, (Texture2D)iconContent.image);
#endif
        }



        /// <summary>
        /// Applique l'icone au GameObject pour la visualisation dans l'editeur. (Cette fonction n'aura aucun effet dans l'executable) 
        /// </summary>
        /// <param name="gObj">L'objet concerné</param>
        /// <param name="icon">L'icone à appliquer</param>
        public static void SetIcon(this GameObject gObj, Texture2D icon)
        {
#if UNITY_EDITOR
            var ty = typeof(UnityEditor.EditorGUIUtility);
            var mi = ty.GetMethod("SetIconForObject", BindingFlags.NonPublic | BindingFlags.Static);
            mi.Invoke(null, new object[] { gObj, icon });
#endif
        }



        #endregion



        /// <summary>
        /// Permet de suprimer le GameObject et en extrait les composants qui ont besoin de finir leur animation,
        /// comme les particlesSystem, les trails... Afin de leur laisser finir et de ne pas avoir de coupure nette
        /// à l'écran.
        /// </summary>
        /// <param name="g">Ce GameObject</param>
        /// <param name="extractParents">Sortir cet objet de sont parent à la destruction ?</param>
        public static void SafetyDestroyWithComponents(this GameObject g, bool extractParents = true)
        {

            bool cancelDestroy = false;
            if (g == null) return;

            Transform newParent = extractParents ? BaseGameManager.TempInstParent : g.transform.parent;

            ParticleSystem[] p = g.GetComponentsInChildren<ParticleSystem>();
            for (int i = 0; i < p.Length; i++)
            {
                cancelDestroy |= p[i].gameObject == g;
                p[i].SafetyDestroy(newParent);
            }
            TrailRenderer[] t = g.GetComponentsInChildren<TrailRenderer>();
            for (int i = 0; i < t.Length; i++)
            {
                cancelDestroy |= t[i].gameObject == g;
                t[i].SafetyDestroy(newParent);
            }

            if (!cancelDestroy)
                UnityEngine.Object.Destroy(g);

        }

        /// <summary>
        /// Détruit le composant, mais change son parent et le laisse finir son animation.
        /// </summary>
        /// <param name="p">Ce composant</param>
        /// <param name="newParent">Le nouveau parent</param>
        public static void SafetyDestroy(this ParticleSystem p, Transform newParent = null)
        {
            if (newParent == null) newParent = BaseGameManager.TempInstParent;
            p.transform.SetParent(newParent);
            p.Stop();
            p.gameObject.AddComponent<SimpleDestroyAfter>().time = p.main.duration;
        }

        /// <summary>
        /// Détruit le composant, mais change son parent et le laisse finir son animation.
        /// </summary>
        /// <param name="p">Ce composant</param>
        /// <param name="newParent">Le nouveau parent</param>
        public static void SafetyDestroy(this TrailRenderer p, Transform newParent = null)
        {
            if (newParent == null) newParent = BaseGameManager.TempInstParent;
            p.transform.SetParent(newParent);
            p.gameObject.AddComponent<SimpleDestroyAfter>().time = p.time;
        }



        /// <summary>
        /// Copie ce composant sur un autre GameObject
        /// </summary>
        /// <param name="original">Ce composant</param>
        /// <param name="newParent">L'objet de desination</param>
        /// <returns>Le composant clonné</returns>
        public static T CopyComponent<T>(this T original, GameObject destination) where T : Component
        {
            System.Type type = original.GetType();
            var dst = destination.GetComponent(type) as T;
            if (!dst) dst = destination.AddComponent(type) as T;
            var fields = type.GetFields();
            foreach (var field in fields)
            {
                if (field.IsStatic) continue;
                field.SetValue(dst, field.GetValue(original));
            }
            var props = type.GetProperties();
            foreach (var prop in props)
            {
                if (!prop.CanWrite || !prop.CanRead || prop.Name == "name") continue;
                if (prop.IsDefined(typeof(ObsoleteAttribute), true)) continue;
                    prop.SetValue(dst, prop.GetValue(original, null), null);
            }
            return dst as T;
        }




        /// <summary>
        /// Convertir cet objet en array de Byte[]
        /// </summary>
        /// <param name="obj">L'objet à serialiser</param>
        /// <returns>L'objet serialisé</returns>
        public static byte[] SerializeToByteArray(this object obj)
        {
            if (obj == null)
                return null;
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        /// <summary>
        /// Convertir cet array d'octets en objet
        /// </summary>
        /// <param name="arrBytes">Le tableau à déserialiser</param>
        /// <returns>L'objet déserialisé</returns>
        public static object DeserializeToObject(this byte[] arrBytes)
        {
            MemoryStream memStream = new MemoryStream();
            BinaryFormatter binForm = new BinaryFormatter();
            memStream.Write(arrBytes, 0, arrBytes.Length);
            memStream.Seek(0, SeekOrigin.Begin);
            object obj = (object)binForm.Deserialize(memStream);

            return obj;
        }



        /// <summary>
        /// Ajoute une liste à une autre liste et la retourne
        /// </summary>
        /// <typeparam name="T">Le type de l'array</typeparam>
        /// <param name="target">L'array à remplir</param>
        /// <param name="items">Les élèments à ajouter</param>
        /// <returns>Le nouvel array</returns>
        public static T[] AddRangeToArray<T>(this T[] target, params T[] items)
        {
            if (target == null || items == null)
            {
                return null;
            }
            T[] result = new T[target.Length + items.Length];
            target.CopyTo(result, 0);
            items.CopyTo(result, target.Length);       
            return result;
        }






        //public static Vector2 Size(this Texture tex) { return new Vector2(tex.width, tex.height); }





        /*
		public static RaycastHit2D RaycastVector(int layermask, Vector2 pos, Vector2 dirCheck, bool hitTrigger = false)
		{

			Physics2D.queriesHitTriggers = hitTrigger;
			RaycastHit2D rayc = Physics2D.Raycast(pos, dirCheck, dirCheck.magnitude, layermask);
			Physics2D.queriesHitTriggers = true;
			return rayc;
		}


		public static RaycastHit2D RaycastVector(Vector2 vec1, Vector2 vec2, int layermask, bool hitTrigger = false)
		{
			RaycastHit2D rayc = RaycastVector(layermask, vec1, ReMath.VecBetwenPoints(vec1, vec2), hitTrigger);
			return rayc;

		}*/






    }
}


