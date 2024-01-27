using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


    static class Extensions
    {
        /*
    public static bool QuickCompare(this Gradient gradient, Gradient otherGradient, int testInterval = 3)
    {

        for (int i = 0; i < testInterval; i++)
        {
            float time = (float)i / (float)(testInterval - 1);
            if (gradient.Evaluate(time) != otherGradient.Evaluate(time))
            {
                return false;
            }
        }
        return true;
    }
    */


        /*
public static bool In<T>(this T item, params T[] items)
	{
		if (items == null)
			return false;

		for (int i = 0; i < items.Length; i++) { 
			if (items[i].Equals(item) )
				return true;
	}

	return false;
	}

	public static bool Intersect<T>(this T[] myItems, params T[] items)
	{
		if (items == null || myItems == null)
			return false;

		for (int o = 0; o < items.Length; o++)
		{
			for (int i = 0; i < myItems.Length; i++)
			{
				if (items[o].Equals(myItems[i]))
					return true;
			}
		}

		return false;
	}*/

        public static bool NullOrInactive(this Tra_LoopPack t)
        {
            return t == null || !t.isRunning;
        }
        public static bool ActiveAndPlaying(this Tra_LoopPack t)
        {
            return t != null && t.isRunning;
        }



        public static int Rnd(this float source)
        {
            return Mathf.RoundToInt(source);
        }

        public static GameObject Inst(this GameObject gb, Transform positionObj, Transform parentObj)
        {
            return GameObject.Instantiate(gb, positionObj.position, positionObj.rotation, parentObj);
        }
        public static GameObject Inst(this GameObject gb, Transform positionObj)
        {
            return GameObject.Instantiate(gb, positionObj.position, positionObj.rotation, GameManager.TempInstances);
        }

        public static GameObject Inst(this GameObject gb, Vector3 position, Quaternion rotation = default(Quaternion))
        {
            return GameObject.Instantiate(gb, position, rotation, GameManager.TempInstances);
        }

        public static Vector2 Mult(this Vector2 v, Vector2 toMult)
        {
            return new Vector2(v.x * toMult.x, v.y * toMult.y);
        }
        public static void SetBoolTemporary(this Animator v, string key, bool value, float time = 0.1f)
        {
            v.SetBool(key, value);
            Tra_LoopPack.EndInvokeAuto(() => { if (v != null) v.SetBool(key, !value); }, time, false);

        }



        public static void SafetyDestroyWithComponents(this GameObject g)
        {
            ParticleSystem[] p = g.GetComponentsInChildren<ParticleSystem>();
            foreach (ParticleSystem pp in p)
                pp.SafetyDestroy();

            TrailRenderer[] t = g.GetComponentsInChildren<TrailRenderer>();
            foreach (TrailRenderer tt in t)
                tt.SafetyDestroy();

            GameObject.Destroy(g);

        }


    public static void SafetyDestroy(this ParticleSystem p)
    {
        p.transform.SetParent(GameManager.TempInstances);
        p.Stop();
        p.gameObject.AddComponent<SimpleDestroyAfter>().time = p.main.duration;
    }

        public static void SafetyDestroy(this TrailRenderer p)
        {
            p.transform.SetParent(GameManager.TempInstances, true);
            p.gameObject.AddComponent<SimpleDestroyAfter>().time = p.time;
        }





        /*
            public static Vector2 position2(this Transform obj)
            {
                return (Vector2)obj.position;
            }

        public static Vector2 position2(this GameObject obj)
        {
            return (Vector2)obj.transform.position;
        }*/

    }
