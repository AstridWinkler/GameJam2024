using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        LastframeVelocity = rb.velocity;
    }


    public void InputAction()
    {

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
