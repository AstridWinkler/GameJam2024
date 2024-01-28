using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class KataPlayerController : PlayerMovementHandler
{


    [Header("Mr Kata")]
    [SerializeField]
    private GameObject effectErrorParticles;





    private SpriteRenderer spriteRend;



    public Vector2 LastframeVelocity { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        spriteRend = GetComponentsInChildren<SpriteRenderer>().FirstOrDefault(m => m.enabled);
    }

    public void InputKill()
    {
        Debug.Log("kill");
        GameManager.Gameplay.KillPlayer(GameplayManager.DieEnum.Explosion);

    }


    protected override void Update()
    {
        base.Update();
        UpdateAction();
        LastframeVelocity = rb.velocity;
        SetNearItem();
    }


    public bool haveAnObject => currentGrabbedObject != null;
    SimpleObject currentGrabbedObject;

    SimpleObject targetObjectToGrab;

    public void InputAction()
    {
        if(haveAnObject)
        {
            ReleaseObject();
            return;
        }

        if (!haveAnObject && targetObjectToGrab != null)
        {
            GrabObject();
            return;
        }



        //CheckForObjectGrab();
    }


    void ReleaseObject()
    {

        currentGrabbedObject.SetReleased();
        currentGrabbedObject.Rig.AddForce( ((Vector3)rb.velocity)*0.2f + Vector3.right* transform.localScale.x*2, ForceMode2D.Impulse);
        currentGrabbedObject.Rig.angularVelocity = UnityEngine.Random.Range(-0.5f, 0.5f);

       // Physics2D.IgnoreCollision(currentGrabbedObject.Col, playerCollisionHandler.Col, false);
        currentGrabbedObject.gameObject.layer = 0;
        currentGrabbedObject = null;
    }
    void GrabObject()
    {
        currentGrabbedObject = targetObjectToGrab;
        currentGrabbedObject.SetGrab();
        currentGrabbedObject.transform.SetParent(transform);
       // Physics2D.IgnoreCollision(currentGrabbedObject.Col, playerCollisionHandler.Col, true);
        currentGrabbedObject.gameObject.layer = 10;
        currentGrabbedObject.transform.localPosition = transform.TransformDirection(Vector3.right)*0.5f;

    }


    public SimpleObject CheckForObjectGrab()
    {
        if (haveAnObject) return null;

        return Physics2D.OverlapCircleAll(transform.position, 5f).Where(m => m.tag == "Item").Select(m => m.GetComponent<SimpleObject>()).NotNull().OrderBy(m => Vector2.Distance(transform.position, m.transform.position)).Where(m => !m.cannotBeGrab).FirstOrDefault();

    }

    void SetNearItem()
    {
        var obj = CheckForObjectGrab();

        if (targetObjectToGrab != obj) {
            if (targetObjectToGrab != null )
                targetObjectToGrab.SetHilight(false);

            targetObjectToGrab = obj;

            if (targetObjectToGrab != null)
                targetObjectToGrab.SetHilight(true);
        }
    }







    public void RemoveSpawnPoint()
    {
        GameManager.Gameplay.RemoveSpawnPoint();
    }
        public void PlaceSpawnPoint()
    {
        GameObject spawn = GameManager.Gameplay.PlaceSpawnPoint(transform.position);

        if (spawn != null)
        {
            SpriteRenderer[] rds = spawn.GetComponentsInChildren<SpriteRenderer>();

            spawn.transform.localPosition += spriteRend.transform.localPosition;


            foreach (SpriteRenderer rd in rds)
            {
                rd.sprite = spriteRend.sprite;
                rd.flipX = transform.localScale.x > 0;
            }

        }
        else
            effectErrorParticles.Inst(transform.position);

    }


    int shimmerDepth = 0;

    internal void MoveUpShimmer()
    {
        if (shimmerDepth > 0)
        {
            transform.position -= Vector3.up * ShimmerLevelBlock.depthOffsetY;
            shimmerDepth--;
        }

     // ShimmerLevelBlock.depthOffsetY
    }

    internal void MoveDownShimmer()
    {
        if (shimmerDepth < 3)//que max
        {
            shimmerDepth++;
            transform.position += Vector3.up * ShimmerLevelBlock.depthOffsetY;
        }
    }





    protected void UpdateAction()
    {

        if (!Input.GetKeyDown( KeyCode.E))
            return;


        var lst = Physics2D.OverlapCircleAll(transform.position, 5f).Select(m => m.GetComponent<I_Interactable>()).NotNull().Where(m => m.CanPlayerAction()).OrderBy(m => Vector2.Distance(transform.position, (m as MonoBehaviour).transform.position)).FirstOrDefault();



        if (lst != null )
        {
            //	Debug.LogError("wouah");
            lst.CallPlayerAction(null);
        }


    }




    public void OnDie()
    {
      //  if (isGrabing != GrabMode.Nothing)
      //      GrabCorpse(null);
    }


    /* public void InputRun(bool press)
     {

     }*/












    void Start()
    {
        
    }


}
