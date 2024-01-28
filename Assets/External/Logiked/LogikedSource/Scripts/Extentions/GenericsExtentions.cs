using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace logiked.source.extentions
{

    /// <summary>
    /// Extentions pour les types couramment utilisées
    /// </summary>
    public static class GenericsExtentions
    {




        /// <summary>
        /// Retourne le plus grand entier inferieur ou égal à ce float
        /// </summary>
        /// <param name="source">Le float</param>
        /// <returns>Le Floor du float</returns>
        public static int Floor(this float source) => Mathf.FloorToInt(source);


        /// <summary>
        /// Retourne le plus grand entier superieur ou égal à ce float
        /// </summary>
        /// <param name="source">Le float</param>
        /// <returns>Le Ceil du float</returns>
        public static int Ceil(this float source) => Mathf.CeilToInt(source);


        /// <summary>
        /// Modifie la plage de la valeur
        /// </summary>
        /// <param name="value">La valeur à modifier, dans l'intervalle 1</param>
        /// <param name="range1Min">Le minimum de l'intervalle dans lequel la valeur est utilisé</param>
        /// <param name="range1Max">Le maximum de l'intervalle dans lequel la valeur est utilisé</param>
        /// <param name="range2Min">Le nouveau minimum de l'intervalle dans lequel la valeur sera utilisé</param>
        /// <param name="range2Max">Le nouveau minimum de l'intervalle dans lequel la valeur sera utilisé</param>
        /// <returns>La valeur modifié, mappé pour l'intervalle 2</returns>
        public static float Remap(this float value, float range1Min, float range1Max, float range2Min, float range2Max)
        {
            return (value - range1Min) / (range1Max - range1Min) * (range2Max - range2Min) + range2Min;
        }


        /// <summary>
        /// Retourne l'arrondi de ce float
        /// </summary>
        /// <param name="source">Le float</param>
        /// <returns>Arrondi du float</returns>
        public static int Rnd(this float source) => source.Rnd(1);

        /// <summary>
        /// Retourne le multiple de range le plus proche de ce float (Arrondi à range prés)
        /// </summary>
        /// <param name="source">Le nombre à transformer</param>
        /// <param name="step">Le multiple sur lequel l'arrondi doit s'aligner</param>
        /// <param name="offset">Une valeur ajoutée à la source, puis soustraire à la fin du calcul.</param>
        /// <returns>La valeur transformée</returns>
        public static int Rnd(this float source, int step, int offset = 0) => Mathf.RoundToInt((source + offset) / step) * step - offset;


        /// <summary>
        /// Retourne le multiple de range le plus proche de ce float (Arrondi à range prés)
        /// </summary>
        /// <param name="source">Le nombre à transformer</param>
        /// <param name="step">Le multiple sur lequel l'arrondi doit s'aligner</param>
        /// <param name="offset">Une valeur ajoutée à la source, puis soustraire à la fin du calcul.</param>
        /// <returns>La valeur transformée</returns>
        public static float Rnd(this float source, float step, float offset = 0) => Mathf.RoundToInt((source + offset) / step) * step - offset;

        /// <summary>
        /// Retourne la valeur absolue 
        /// </summary>
        /// <param name="source">Le nombre à traiter</param>
        /// <returns>Sa valeur absolue</returns>
        public static float Abs(this float source) => Mathf.Abs(source);

        /// <summary>
        /// Retourne la valeur absolue 
        /// </summary>
        /// <param name="source">Le nombre à traiter</param>
        /// <returns>Sa valeur absolue</returns>
        public static int Abs(this int source) => Mathf.Abs(source);



        /// <summary>
        /// Retourne un Float random entre les deux bornes
        /// </summary>
        /// <param name="source">Le générateur de random</param>
        /// <param name="min">Valeur minimale</param>
        /// <param name="max">Valeur maximale</param>
        /// <returns>Valeur aléatoire entre les deux bornes</returns>
        public static float NextFloat(this System.Random source, float min, float max) => (float)source.NextDouble()* (max-min) + min ;



        /// <summary>
        /// Retourne un Long random entre les deux bornes
        /// </summary>
        /// <param name="source">Le générateur de random</param>
        /// <param name="min">Valeur minimale</param>
        /// <param name="max">Valeur maximale</param>
        /// <returns>Valeur aléatoire entre les deux bornes</returns>
        public static long NextLong(this System.Random source, long min = long.MinValue, long max = long.MaxValue)
        {
            byte[] buf = new byte[8];
            source.NextBytes(buf);
            long longRand = BitConverter.ToInt64(buf, 0);
            return (Math.Abs(longRand % (max - min)) + min);
        }




        /// <summary>
        /// Retourne un Float random entre les deux bornes
        /// </summary>
        /// <param name="source">Le générateur de random</param>
        /// <param name="min">Valeur minimale</param>
        /// <param name="max">Valeur maximale</param>
        /// <returns>Valeur aléatoire entre les deux bornes</returns>
        public static float Range(this System.Random source, float min, float max) => NextFloat(source, min, max);




        /// <summary>
        /// Rertourne le signe du nombre (-1 ou 1)
        /// </summary>
        /// <param name="source">Le nombre à traiter</param>
        /// <param name="allowZero">Possibilité de retourner 0 si ce float est nul</param>
        /// <returns>Le signe du nombre</returns>
        public static int Sign(this float source, bool allowZero = false)
        {
            if (allowZero && source == 0) return 0;
            return (int)Mathf.Sign(source);
        }

        /// <summary>
        /// Rertourne le signe du nombre (-1 ou 1)
        /// </summary>
        /// <param name="source">Le nombre à traiter</param>
        /// <param name="allowZero">Possibilité de retourner 0 si ce float est nul</param>
        /// <returns>Le signe du nombre</returns>
        public static int Sign(this int source, bool allowZero = false)
        {
            if (allowZero && source == 0) return 0;
            return (int)Mathf.Sign(source);
        }




        /// <summary>
        /// Divise le nombre et arrondi à l'inferieur (pas pareil que int A/ int B)
        /// </summary>
        /// <param name="source">Le nombre à traiter</param>
        /// <param name="divideBy">Le diviseur</param>
        /// <returns>Le nombre divisé et Floored</returns>
        public static int FlooredDivide(this int source, int divideBy)
        {
            return source / divideBy - Convert.ToInt32(source < 0 ^ divideBy < 0 && source % divideBy != 0);
        }


        /// <summary>
        /// Rogner la valeur entre 2 bornes
        /// </summary>
        /// <param name="source">La valeur à traiter</param>
        /// <param name="min">Valeur min du nombre</param>
        /// <param name="max">Valeur max du nombre [inclus]</param>
        /// <returns>Le nombre clamp</returns>
        public static int Clamp(this int source, int min, int max)
        {
            return source < min ? min : source > max ? max : source;
        }

        /// <summary>
        /// Rogner la valeur entre 0 et max
        /// </summary>
        /// <param name="source">La valeur à traiter</param>
        /// <param name="max">Valeur max du nombre  [inclus]</param>
        /// <returns>Le nombre clamp</returns>
        public static int Clamp(this int source,  int max)
        {
            return source.Clamp(0, max);
        }



        /// <summary>
        /// Rogner la valeur entre 2 bornes
        /// </summary>
        /// <param name="source">La valeur à traiter</param>
        /// <param name="min">Valeur min du nombre</param>
        /// <param name="max">Valeur max du nombre</param>
        /// <returns>Le nombre clamp</returns>
        public static float Clamp(this float source, float min, float max)
        {
            return source < min ? min : source > max ? max : source;
        }

        /// <summary>
        /// Rogner la valeur entre 0 et max
        /// </summary>
        /// <param name="source">La valeur à traiter</param>
        /// <param name="max">Valeur max du nombre</param>
        /// <returns>Le nombre clamp</returns>
        public static float Clamp(this float source, float max)
        {
            return source.Clamp(0, max);
        }



        /// <summary>
        /// Rogner la valeur entre 0 et 1
        /// </summary>
        /// <param name="source">La valeur à traiter</param>
        /// <returns>Le nombre clamp entre 0 et 1</returns>
        public static float Clamp01(this float source)
        {
            return source.Clamp(0, 1f);
        }


        /// <summary>
        /// Permet de faire cycler la valeur autour dans l'interval [0-max[, mêmme si elle est négative (contrairement à un simple opérateur %)
        /// </summary>
        /// <param name="source">Le nombre à traiter</param>
        /// <param name="max">La borne max du cycle</param>
        /// <returns>Le nombre transformé, compris entre [0-max[</returns>
        public static int Cycle(this int source, int max)
        {
            if (max == 0) return source;
            source %= max;
            source += max;
            source %= max;
            return source;
        }


        /// <summary>
        /// Permet de faire cycler la valeur autour dans l'interval [min-max[
        /// </summary>
        /// <param name="source">Le nombre à traiter</param>
        /// <param name="max">La borne max du cycle</param>
        /// <returns>Le nombre transformé, compris entre [0-max[</returns>
        public static int Cycle(this int source, int min, int max)
        {
            if (max == min) return source;
            source = (source - min).Cycle(max - min);
            return source + min;
        }



        /// <summary>
        /// Permet de faire cycler la valeur autour dans l'interval [0-max[, mêmme si elle est négative (contrairement à un simple opérateur %)
        /// </summary>
        /// <param name="source">Le nombre à traiter</param>
        /// <param name="max">La borne max du cycle</param>
        /// <returns>Le nombre transformé, compris entre [0-max[</returns>
        public static float Cycle(this float source, int max)
        {
            if (max == 0) return source;
            int sub = (int)source;
            sub %= max;
            sub += max;
            sub %= max;
            return sub + (source - (int)source);
        }

        /// <summary>
        /// [Attention:Retourne un résultat à 10^-3 prés]<br/>Permet de faire cycler la valeur autour dans l'interval [0-max[, mêmme si elle est négative (contrairement à un simple opérateur %)
        /// </summary>
        /// <param name="source">Le nombre à traiter</param>
        /// <param name="max">La borne max du cycle</param>
        /// <returns>Le nombre transformé, compris entre [0-max[</returns>
        public static float Cycle(this float source, float max)
        {
            return (source * 1000f).Cycle((int)(max * 1000f)) / 1000f;
        }


        /// <summary>
        /// Ajouter ou update une valeur dans le dictionnaire
        /// </summary>
        /// <typeparam name="T">Le type de clé du dictionnaire</typeparam>
        /// <typeparam name="G">Le type de valeur du dictionnaire</typeparam>
        /// <param name="dic">Ce dicitonnaire</param>
        /// <param name="key">La clé à chercher</param>
        /// <param name="elem">La valeur à ajouter / Mettre à jour</param>
        public static void AddOrUpdate<T, G>(this IDictionary<T, G> dic, T key, G elem)
        {
            if (dic.ContainsKey(key)) dic[key] = elem;
            else dic.Add(key, elem);
        }

        /// <inheritdoc cref="AddOrUpdate{T, G}(IDictionary{T, G}, T, G)"/>
        /// <param name="dic">Ce dicitonnaire</param>
        /// <param name="key">La clé à chercher</param>
        /// <param name="elem">La valeur à ajouter / Mettre à jour</param>
        public static void AddOrUpdate(this IDictionary dic, object key, object elem)
        {
            if (dic.Contains(key)) dic[key] = elem;
            else dic.Add(key, elem);
        }


        /// <summary>
        /// Retourne une valeur du dictionnaire en fonction d'une clé.
        /// Si cette valeur n'est pas présente, retourne la valeur par défaut du type G.
        /// </summary>
        /// <typeparam name="T">Le type des clés contenues dans le dictionnaire</typeparam>
        /// <typeparam name="G">Le type des valeurs contenues dans le dictionnaire</typeparam>
        /// <param name="dic">Ce dictionnaire</param>
        /// <param name="key">La clé associé à la valeur</param>
        /// <returns>la valeur ou la valeur par défaut de G (peut etre null)</returns>
        public static G GetOrDefault<T, G>(this IDictionary<T, G> dic, T key)
        {
            if (dic?.ContainsKey(key) == true) return dic[key];
            return default;
        }

        /// <summary>
        /// Retourne une valeur du dictionnaire en fonction d'une clé.
        /// Si cette valeur n'est pas présente, retourne la valeur par défaut du type G.
        /// </summary>
        /// <typeparam name="T">Le type des clés contenues dans le dictionnaire</typeparam>
        /// <typeparam name="G">Le type des valeurs contenues dans le dictionnaire</typeparam>
        /// <param name="dic">Ce dictionnaire</param>
        /// <param name="key">La clé associé à la valeur</param>
        /// <param name="defaultValue">La valeur par défaut spécifiée</param>
        /// <returns>la valeur ou la valeur par défaut de G (peut etre null)</returns>
        public static G GetOrDefault<T, G>(this IDictionary<T, G> dic, T key, G defaultValue)
        {
            if (dic.ContainsKey(key)) return dic[key];
            return defaultValue;
        }



        /// <summary>
        /// Obtenir la chaîne du contenu d'une collection, par succession de ToString() des élements.
        /// </summary>
        /// <typeparam name="T">Le type des élements de la collection</typeparam>
        /// <param name="array">La collection</param>
        /// <param name="separator">La chaine à inserer entre chaque élements</param>
        /// <returns>La chaîne des élements de la collection</returns>
        public static string ToStringArray<T>(this IEnumerable<T> array, string separator = "\n")
        {
            StringBuilder b = new StringBuilder();
            b.Append(typeof(T) + " array : [");


            if (array != null && array.Count() > 0)
            {
                b.Append(array.ElementAt(0).ToString());

                for (int i = 1; i < array.Count(); i++)
                {
                    b.Append(separator);
                    b.Append(array.ElementAt(i).ToString());
                }
            }

            b.Append(typeof(T) + "]");


            return b.ToString();
        }


        /// <summary>
        /// Applique une fonction à chaque éléments d'une liste
        /// </summary>
        /// <typeparam name="T">Le type des élements de la collection</typeparam>
        /// <param name="source">La collection</param>
        /// <param name="action">La fonction à appliquer</param>
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var item in source)
                action(item);
        }




        /// <summary>
        /// Obtenir la valeur de l'array à la position Id, mais avec un cycle sur la taille de l'array. Ainsi, Array.GetCyclic(-1) donnera le dernier élement;
        /// </summary>
        /// <typeparam name="T">Le type des élements de l'array</typeparam>
        /// <param name="item">L'array à traiter</param>
        /// <param name="id">La valeur de l'index (valeur négative autorisés)</param>
        /// <returns>L'élement à la position Id.Cylce(Item.Length)</returns>
        public static T GetCyclic<T>(this ICollection<T> item, int id)
        {
            if (item == null || item.Count == 0)
            {
                throw new IndexOutOfRangeException("Array is empty");
            }
            return item.ElementAt(id.Cycle(item.Count));
        }



        /// <summary>
        /// Multiplie les composantes des deux vecteur
        /// </summary>
        /// <param name="v1">Ce vecteur</param>
        /// <param name="v2">Le vecteur à multiplier</param>
        /// <returns>La multiplication des deux vecteurs</returns>
        public static Vector3 Multiply(this Vector3 v1, Vector3 v2) { return Multiply(v1, v2.x,  v2.y, v2.z ); }

        /// <summary>
        /// Multiplie les composantes des deux vecteur
        /// </summary>
        /// <param name="v1">Ce vecteur</param>
        /// <param name="x">Composante multiplicative</param>
        /// <param name="y">Composante multiplicative</param>
        /// <param name="z">Composante multiplicative</param>
        /// <returns>La multiplication des deux vecteurs</returns>
        public static Vector3 Multiply(this Vector3 v1, float x, float y, float z) { return new Vector3(v1.x * x, v1.y * y, v1.z * z); }




        /// <summary>
        /// Multiplie les composantes des deux vecteur
        /// </summary>
        /// <param name="v1">Ce vecteur</param>
        /// <param name="v2">Le vecteur à multiplier</param>
        /// <returns>La multiplication des deux vecteurs</returns>
        public static Vector2 Multiply(this Vector2 v1, Vector2 v2) { return new Vector2(v1.x * v2.x, v1.y * v2.y); }

        /// <summary>
        /// Multiplie les composantes des deux vecteur
        /// </summary>
        /// <param name="v1">Ce vecteur</param>
        /// <param name="v2">Le vecteur à multiplier</param>
        /// <returns>La multiplication des deux vecteurs</returns>
        public static Vector2 Multiply(this Vector2 v1, float x, float y) { return new Vector2(v1.x * x, v1.y * y); }


        /// <summary>
        /// Divise les composantes des deux vecteur
        /// </summary>
        /// <param name="v1">Le vecteur dividende</param>
        /// <param name="v2">Le vecteur diviseur</param>
        /// <returns>Le quotien de l'opération</returns>
        public static Vector2 Divide(this Vector2 v1, Vector2 v2) { return new Vector2(v1.x / v2.x, v1.y / v2.y); }
        /// <summary>
        /// Divise les composantes des deux vecteur
        /// </summary>
        /// <param name="v1">Le vecteur dividende</param>
        /// <param name="v2">Le vecteur diviseur</param>
        /// <returns>Le quotien de l'opération</returns>
        public static Vector2 Divide(this Vector2 v1, float x, float y) { return new Vector2(v1.x / x, v1.y / y); }


        /// <summary>
        /// Applique une rotation au vecteur selon l'axe orthogonal qu'il décrit
        /// </summary>
        /// <param name="v1">Le Vecteur à faire tourner</param>
        /// <param name="ang">L'angle de rotation en degrées</param>
        /// <returns>Le quotien de l'opération</returns>
        public static Vector2 Rotate(this Vector2 v, float degrees)
        {
            float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
            float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);

            float tx = v.x;
            float ty = v.y;
            v.x = (cos * tx) - (sin * ty);
            v.y = (sin * tx) + (cos * ty);
            return v;
        }





        /// <summary>
        /// Divise les composantes des deux vecteur
        /// </summary>
        /// <param name="v1">Le vecteur dividende</param>
        /// <param name="v2">Le vecteur diviseur</param>
        /// <returns>Le quotien de l'opération</returns>
        public static Vector3 Divide(this Vector3 v1, Vector3 v2) { return new Vector3(v1.x / v2.x, v1.y / v2.y, v1.z / v2.z); }




        /// <summary>
        /// Conversion de vecteur
        /// </summary>
        /// <param name="point">Le vecteur à convertir</param>
        /// <returns>Le vecteur converti</returns>
        public static Vector2Int ToVector2IntXZ(this Vector3Int point) { return new Vector2Int(point.x, point.z); }

        /// <inheritdoc cref="ToVector2IntXZ"/>
        public static Vector2Int ToVector2Int(this Vector3Int point) { return new Vector2Int(point.x, point.y); }

        /// <inheritdoc cref="ToVector2IntXZ"/>
        public static Vector3 ToVector3XZ(this Vector2 point) { return new Vector3(point.x, 0, point.y); }

        /// <inheritdoc cref="ToVector2IntXZ"/>
        public static Vector3 ToVector3(this Vector2 point) { return new Vector3(point.x, point.y, 0); }

        /// <inheritdoc cref="ToVector2IntXZ"/>
        public static Vector3Int ToVector3Int(this Vector2 point) { return new Vector3Int(point.x.Rnd(), point.y.Rnd(), 0); }

        /// <inheritdoc cref="ToVector2IntXZ"/>
        public static Vector3Int ToVector3IntFloor(this Vector2 point) { return new Vector3Int((int)point.x, (int)point.y, 0); }

        /// <inheritdoc cref="ToVector2IntXZ"/>
        public static Vector3Int ToVector3IntCeil(this Vector2 point) { return new Vector3Int(Mathf.CeilToInt(point.x), Mathf.CeilToInt(point.y), 0); }

        /// <inheritdoc cref="ToVector2IntXZ"/>
        public static Vector2Int ToVector2Int(this Vector2 point) { return new Vector2Int(point.x.Rnd(), point.y.Rnd()); }


        /// <inheritdoc cref="ToVector2IntXZ"/>
        public static Vector3 ToVector3XZ(this Vector2Int point) { return new Vector3(point.x, 0, point.y); }

        /// <inheritdoc cref="ToVector2IntXZ"/>
        public static Vector3 ToVector3XY(this Vector2Int point) { return new Vector3(point.x, point.y, 0); }

        /// <inheritdoc cref="ToVector2IntXZ"/>
        public static Vector3 ToVector3(this Vector3Int point) { return new Vector3(point.x, point.y, point.z); }

        /// <inheritdoc cref="ToVector2IntXZ"/>
        public static Vector3Int ToVector3Int(this Vector2Int point) { return new Vector3Int(point.x, point.y, 0); }

        /// <inheritdoc cref="ToVector2IntXZ"/>
        public static Vector2 ToVector2(this Vector3Int point) { return new Vector2(point.x, point.y); }


        /// <summary>
        /// Conversion de vecteur
        /// </summary>
        /// <param name="point">Le vecteur à convertir</param>
        /// <param name="mode">Le mode de conversion (arrondir, Floor...)</param>
        /// <returns>Le vecteur converti</returns>
        public static Vector3Int ToVector3Int(this Vector3 point, ConvertIntMode mode = ConvertIntMode.Round)
        {
            switch (mode)
            {
                case ConvertIntMode.Floor: return new Vector3Int(point.x.Floor(), point.y.Floor(), point.z.Floor());
                case ConvertIntMode.Ceil: return new Vector3Int(point.x.Ceil(), point.y.Ceil(), point.z.Ceil());
                default:
                    return new Vector3Int(point.x.Rnd(), point.y.Rnd(), point.z.Rnd());
            }
        }

        /// <summary>
        /// Conversion de vecteur
        /// </summary>
        /// <param name="point">Le vecteur à convertir</param>
        /// <returns>Le vecteur converti, avec les composantes X et Z de l'original</returns>
        public static Vector2 ToVector2XZ(this Vector3 point)
        {
            return new Vector2(point.x, point.z);
        }

        /// <summary>
        /// Conversion de vecteur
        /// </summary>
        /// <param name="point">Le vecteur à convertir</param>
        /// <returns>Le vecteur converti, avec les composantes Y et Z de l'original</returns>
        public static Vector2 ToVector2YZ(this Vector3 point)
        {
            return new Vector2(point.y, point.z);
        }


        public enum ConvertIntMode { Round, Floor, Ceil }


        /// <summary>
        /// Effectue une rotation de ce vecteur autour du point (0,0,0), selon un angle
        /// </summary>
        /// <param name="point">Le point à déplacer</param>
        /// <param name="angles">L'angle de rotation</param>
        /// <returns>Le point transformé</returns>
        public static Vector3 RotateAround(this Vector3 point, Vector3 angles)
        {
            return RotateAround(point, Vector3.zero, angles) ; // return it
        }


        /// <summary>
        /// Effectue une rotation de ce vecteur autour d'un point de pivot, selon un angle
        /// </summary>
        /// <param name="point">Le point à déplacer</param>
        /// <param name="pivot">Le point de pivot</param>
        /// <param name="angles">L'angle de rotation</param>
        /// <returns>Le point transformé</returns>
        public static Vector3 RotateAround(this Vector3 point, Vector3 pivot, Vector3 angles)
        {
            Vector3 dir = point - pivot; // get point direction relative to pivot
            dir = Quaternion.Euler(angles) * dir; // rotate it
            point = dir + pivot; // calculate rotated point
            return point; // return it
        }




        /// <summary>
        /// Effectue une rotation de ce vecteur autour du point (0,0,0), selon un angle, et le retourne en Vector3Int
        /// </summary>
        /// <param name="point">Le point à déplacer</param>
        /// <param name="angles">L'angle de rotation</param>
        /// <returns>Le point transformé</returns>
        public static Vector3Int RotateAround(this Vector3Int point, Vector3 angles)
        {
            return RotateAround(point, Vector3.zero, angles);
        }
        /// <summary>
        /// Effectue une rotation de ce vecteur autour d'un point de pivot, selon un angle, et le retourne en Vector3Int
        /// </summary>
        /// <param name="point">Le point à déplacer</param>
        /// <param name="pivot">Le point de pivot</param>
        /// <param name="angles">L'angle de rotation</param>
        /// <returns>Le point transformé</returns>
        public static Vector3Int RotateAround(this Vector3Int point, Vector3 pivot, Vector3 angles)
        {
            Vector3 point2 = point;
            Vector3 dir = point2 - pivot; // get point direction relative to pivot
            dir = Quaternion.Euler(angles) * dir; // rotate it
            point2 = dir + pivot; // calculate rotated point
            return point2.ToVector3Int(); // return it
        }




        /// <summary>
        /// Racourcis pour la fonction "isNullOrEmpty" de strings
        /// </summary>
        /// <param name="str">La chaîne à comparer</param>
        /// <returns>La chaîne est-elle nulle ou vide ?</returns>
        public static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }

        /// <summary>
        /// Racourcis pour la fonction "isNullOrWhiteSpace" de strings
        /// </summary>
        /// <param name="str">La chaîne à comparer</param>
        /// <returns>La chaîne est-elle nulle ou avec des spaces ?</returns>
        public static bool IsNullOrWhiteSpace(this string str)
        {
            return string.IsNullOrWhiteSpace(str);
        }


        /// <summary>
        /// Racourcis pour la fonction static "Format" de strings. <br></br>
        /// Pratique pour faire des comparaisons, sinon préferer l'implémentation native C# : $\"aaa {variable} bbb\" quand c'est possible
        /// </summary>
        /// <param name="str">La chaîne a formater</param>
        /// <param name="strs">Les élement à integrer</param>
        /// <returns>La nouvelle chaîne avec les élements</returns>
        //[Obsolete("Préferer l'implémentation native C# : $\"aaa {variable} bbb\"")]
        public static string Format(this string str, params object[] strs)
        {
            return string.Format(str, strs);
        }






        /// <summary>
        /// La fonction Unity LookAt étant fortement déficiente, cette extension à pour but de retourner une rotation entre deux objets afin que l'un regarde l'autre selon un axe. <br></br>
        /// Se sert du forward de l'objet comme point de départ de la rotation! Fonctionne surtout pour l'axe Y à ce jour
        /// </summary>
        /// <param name="trf">Le transform de base qui doit tourner</param>
        /// <param name="lookatPoint">L'objet à regarder</param>
        /// <param name="constraintAxis">L'axe de contraine. Vector3.up pour des déplacements au sol, c'est bien.</param>
        /// <returns>Le Quaternion de rotation.</returns>
        public static Quaternion LookAtRotation(this Transform trf, Vector3 lookatPoint, Vector3 constraintAxis)
        {
            return Quaternion.FromToRotation(Vector3.ProjectOnPlane(trf.forward, constraintAxis), Vector3.ProjectOnPlane(lookatPoint - trf.position, constraintAxis)) * trf.rotation;
        }



        /// <summary>
        /// Similaire à la fonction Distinct de linq, mais applicable sur un champ via un Predicate particulier 
        /// </summary>
        /// <typeparam name="A">Type d'élements de la liste</typeparam>
        /// <param name="collection">La liste à traîter</param>
        /// <param name="predicate">Le champ </param>
        /// <returns>La nouvelle liste</returns>
        public static List<A> Distinct<A, B>(this IList<A> collection, Func<A, B> selector)
        {
            List<A> end = new List<A>();
            HashSet<B> check = new HashSet<B>();

            B checkedValue;

            for (int i = 0; i < collection.Count; i++)
            {
                checkedValue = selector.Invoke(collection[i]);

                if (check.Add(checkedValue))
                {
                    end.Add(collection[i]);
                }
            }

            return end;
        }


#if !UNITY_2021_2_OR_NEWER
        //This function was implemented in Linq for NET_STANDAR_2.1. Before that, you must use this extention

        public static B GetValueOrDefault<A,B>(this IDictionary<A,B> collection, A value)
        {
            if (collection != null && collection.ContainsKey(value))
            {
               return collection[value];
            }
            return default(B);

        }
#endif

        /// <summary>
        /// Echange la position de 2 objets dans une liste
        /// </summary>
        /// <typeparam name="T">Le type des objets de la liste</typeparam>
        /// <param name="list">La liste</param>
        /// <param name="indexA">L'index du premier élément à intervertir</param>
        /// <param name="indexB">L'index du second élément à intervertir</param>
        public static void Swap<T>(this IList<T> list, int indexA, int indexB)
        {
            System.Diagnostics.Contracts.Contract.Requires(list != null);
            System.Diagnostics.Contracts.Contract.Requires(indexA >= 0 && indexA < list.Count);
            System.Diagnostics.Contracts.Contract.Requires(indexB >= 0 && indexB < list.Count);
            if (indexA == indexB)            
                return;
            
            T temp = list[indexA];
            list[indexB] = list[indexB];
            list[indexB] = temp;
        }



        /*
		/// <summary>
		/// Set the width and height positive without modifying the rect. Usefull for .Contains() and .Overlap() func which dosnt work with negatives values.
		/// </summary>
		/// <param name="Rect">The rect.</param>
		public static Rect ToPositive(this Rect rect)
		{
			if (rect.width < 0)
			{
				rect.width = -rect.width;
				rect.position -= Vector2.right* rect.width;
			}
			if (rect.height < 0)
			{
				rect.height = -rect.height;
				rect.position -= Vector2.up * rect.height;
			}
			return rect;
		}*/

    }
}