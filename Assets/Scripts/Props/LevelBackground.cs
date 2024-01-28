using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class LevelBackground : MonoBehaviour//, ISceneObject
{
    public float sizeDecreaseFactor = 1f;
    public float distanceDecreaseMovingFactor = 1f;

    public float yOffset = 0;

    [Serializable]
    private class BackgroundObject
    {
        [SerializeField]
        public Transform backgroundObject;
        [SerializeField]
        public float sizeX = 20;
        [SerializeField][Min(0)]
        public float zAxisDrag = 1;
        public float dragCoef = 1;
        [SerializeField]
        public bool distanceScaling = true;

        [NonSerialized]
        public Vector3 basepPlanPosition;

    }

    [SerializeField]

    private BackgroundObject[] backgrounds;
    [NonSerialized]
    public Vector2 baseCamPosition;

    /*
    [SerializeField]
    private Transform backgroundTemplateParents;

    [SerializeField]
    private Transform backgroundInstanceParents;
    Dictionary<Transform, Transform> instanceParentList = new Dictionary<Transform, Transform>();
    */


    [SerializeField]
    private Camera mainCam;

    bool sceneStarted = false;


    //  void ISceneObject.OnSceneLoaded()

    void Awake()
    {
        CheckCam();
    }

    void CheckCam()
    {


        //  instanceParentList = new Dictionary<Transform, Transform>();

        //   DestroyIntances();
        //    RebuildParents();

        if(mainCam == null)
            mainCam = Application.isPlaying ? GameManager.CamController?.Camera : Camera.main;

        if(mainCam == null || mainCam.gameObject.tag != "MainCamera")
        {
            mainCam = null;

            if (Application.isPlaying)
                Debug.Log("No camera found Gamemanager.CamController");

            return;
        }


        sceneStarted = true;

        baseCamPosition = mainCam.transform.position;

        foreach (var bg in backgrounds)
            bg.basepPlanPosition = bg.backgroundObject.localPosition;


    }



    void FixedUpdate()
    {
        if (!Application.isPlaying || !sceneStarted)
            return;

        foreach (var obj in backgrounds)
        {
            MoveBackground(obj);
        }
    }


    [NonSerialized]
    bool firstOnGui;

    private void OnGUI()
    {
        if (!firstOnGui) {
            firstOnGui = true;

         //  Debug.LogError("first");

            foreach (var background in backgrounds)
            {
                background.zAxisDrag = background.backgroundObject.localPosition.z;   
            }
        }


        foreach (var background in backgrounds)
        {
            background.backgroundObject.position = new Vector3(background.backgroundObject.position.x, background.backgroundObject.position.y, background.zAxisDrag);


            if (background.distanceScaling)
            {
                background.backgroundObject.transform.localScale = Vector3.one / (1 + background.zAxisDrag*0.01f* sizeDecreaseFactor);
            }
        }



    }


//#if UNITY_EDITOR
    private void Update()
    {
        if (mainCam == null)
        {
            CheckCam();
        }
    }
//#endif

    void MoveBackground(BackgroundObject background)
    {

        background.zAxisDrag = background.basepPlanPosition.z;


        var realCamBasePos = baseCamPosition - yOffset*Vector2.up;


        var pos = background.basepPlanPosition + ((Vector3)realCamBasePos - mainCam.transform.position) / Mathf.Sqrt (1 + background.zAxisDrag * distanceDecreaseMovingFactor * background.dragCoef);
        pos.z = background.basepPlanPosition.z;


        background.backgroundObject.localPosition = pos;


    }



    /*

    void MoveBackground(Transform template)
    {
        GameObject tempInst = null;

        if (!instanceParentList.ContainsKey(template))
        {
            tempInst = new GameObject(template.name + "[inst]");
            tempInst.transform.SetParent(backgroundInstanceParents);
            tempInst.transform.localPosition = Vector3.zero;
            instanceParentList.Add(template, tempInst.transform);
        }
        else
            tempInst = instanceParentList[template].gameObject;




    }



    void DestroyIntances()
    {
        foreach (Transform c in backgroundInstanceParents)
        {
            Destroy(c.gameObject);
        }
        instanceParentList.Clear();

    }

    void RebuildParents()
    {

    }*/


}
