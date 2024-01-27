using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trapgun : MonoBehaviour
{
    Animator anim;
    [SerializeField]
    LineRenderer line;
    [SerializeField]
    Transform rotatingPart;

    [SerializeField]
    GameObject bullet;

    [SerializeField]
    GameObject bloodHitParticles;

    Vector3 targetPos;
    Vector3 smoothPos;

    void Start()
    {
        anim = GetComponent<Animator>();
        targetPos = transform.position - Vector3.down;
    }


    void FixedUpdate()
    {
        smoothPos = Vector3.Lerp(smoothPos, targetPos, Time.fixedDeltaTime*15f + (loading/maxLoading)) ;

        rotatingPart.transform.right = smoothPos  - transform.position;
        rotatingPart.Rotate(new Vector3(0,0,-90));
        line.SetPositions(new Vector3[] { transform.position, smoothPos } );
    }

    float loading;
    const float maxLoading = 0.3f;


        void Update()
    {

         GameObject pl = GameManager.Gameplay.CurrentPlayer;
        if (pl != null) {

            var st = Physics2D.queriesHitTriggers;
            Physics2D.queriesHitTriggers = false;
           RaycastHit2D ray = Physics2D.Linecast(transform.position, pl.transform.position, 1 | (1 << 9) | (1 << 10) | ( 1 << 13));
            Physics2D.queriesHitTriggers = st;
        if (!ray)
            {
                loading += Time.deltaTime;
                targetPos = pl.transform.position;

                if (loading > maxLoading)
                {
                    GameManager.Gameplay.KillPlayer(GameplayManager.DieEnum.SimpleCorpse);
                    bloodHitParticles.Inst(targetPos, Quaternion.Euler(0, 0, rotatingPart.eulerAngles.z));
                    anim.Play("laser", -1, 0);
                    loading = 0;
                }
            }
            else
            {
                targetPos = ray.point;
                loading = 0;
            }
        }




    }
}
