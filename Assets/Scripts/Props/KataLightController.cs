using FunkyCode;
using logiked.source.attributes;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[ExecuteAlways()]
[RequireComponent(typeof(Light2D))]
public class KataLightController : MonoBehaviour
{


    [ColorUsage(false)]
    [SerializeField]
    private Color firstLightColor = Color.cyan;

    [SerializeField]
    bool sameColorForSecondLight = true;

    [ShowIf(nameof(sameColorForSecondLight), "==", false)]
    [ColorUsage(false)]
    [SerializeField]
    private Color SecondLightColor = Color.cyan;



    [Range(0, 1)]
    [SerializeField]
    private float lightAlpha = 0.5f;


    [SerializeField]
    private bool useSublight = true;

    [SerializeField]
    private bool ingameUpdate = false;


    private Light2D mapLight;
    private Light2D entityLight;


    void Start()
    {
        AssignLights();
    }

    void AssignLights()
    {
        mapLight = GetComponent<Light2D>();

        if (mapLight == null)
            mapLight = gameObject.AddComponent<Light2D>();

        entityLight = GetComponentsInChildren<Light2D>(true).FirstOrDefault(m => m.transform != transform);

        if (entityLight == null)
        {
            var obj = new GameObject("subLight");
            obj.transform.SetParent(transform);
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localEulerAngles = Vector3.zero;

            entityLight = obj.AddComponent<Light2D>();
        }




        UpdateLightParams();
        UpdateSecondaryLight();
    }

    void UpdateLightParams()
    {

        if (useSublight)
        {
            mapLight.lightPresetId = 0;
            mapLight.lightLayer = 0;
            mapLight.occlusionLayer = 1;

            entityLight.lightPresetId = 1;
            entityLight.lightLayer = 1;
            entityLight.occlusionLayer = 2;
        }
        else
        {
            mapLight.lightPresetId = 0;
            mapLight.lightLayer = 1;
            mapLight.occlusionLayer = 1;
        }


    }




    void UpdateSecondaryLight()
    {




        entityLight.size = mapLight.size;
        entityLight.coreSize = mapLight.coreSize;
        entityLight.textureSize = mapLight.textureSize;

        entityLight.spotAngleInner = mapLight.spotAngleInner;
        entityLight.spotAngleOuter = mapLight.spotAngleOuter;
        entityLight.shadowDistanceClose = mapLight.shadowDistanceClose;
        entityLight.shadowDistanceFar = mapLight.shadowDistanceFar;
        entityLight.falloff = mapLight.falloff;

        entityLight.freeFormFalloffStrength = mapLight.freeFormFalloffStrength;
        entityLight.freeFormFalloff = mapLight.freeFormFalloff;
        entityLight.freeForm = mapLight.freeForm;
        entityLight.freeFormPoint = mapLight.freeFormPoint;
        entityLight.freeFormPoints = mapLight.freeFormPoints;

        entityLight.lightType = mapLight.lightType;

        entityLight.lightSprite = mapLight.lightSprite;
        entityLight.spriteFlipX = mapLight.spriteFlipX;
        mapLight.spriteFlipY = entityLight.spriteFlipY;
        entityLight.sprite = mapLight.sprite;


    }

    // Update is called once per frame
    void Update()
    {
        if (mapLight == null || entityLight == null) AssignLights();


        entityLight.enabled = useSublight;




        if (!Application.isPlaying || ingameUpdate)
        {



            var perLightAlpha = useSublight ? lightAlpha / 2f : lightAlpha;

            mapLight.color = new Color(firstLightColor.r, firstLightColor.g, firstLightColor.b, perLightAlpha);

            if (sameColorForSecondLight)
                entityLight.color = mapLight.color;
            else
                entityLight.color = new Color(SecondLightColor.r, SecondLightColor.g, SecondLightColor.b, perLightAlpha);

            UpdateLightParams();


            if (useSublight)
            {

                UpdateSecondaryLight();

            }



        }



        

    }
}
