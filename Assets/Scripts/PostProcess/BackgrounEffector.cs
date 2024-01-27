using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class BackgrounEffector : MonoBehaviour
{



    SpriteRenderer[] childs;

    [SerializeField]
    Gradient backgroundFog;
    [SerializeField]
    float gradientMaxDist = 250;

#if UNITY_EDITOR
    private void Update()
    {
        if(!Application.isPlaying)
        OnEnable();  
    }
#endif

    void OnEnable()
    {
        childs = gameObject.GetComponentsInChildren<SpriteRenderer>(true);
        foreach (var o in childs)
        {
            //o.sortingOrder = 0;// (int)(o.gameObject.transform.position.z * -10f);
            if(o.gameObject.tag!="WithoutFog")
            o.color = backgroundFog.Evaluate( Mathf.Clamp01(o.gameObject.transform.position.z / gradientMaxDist));
        }
    }





}
