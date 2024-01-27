using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundHurt : MonoBehaviour
{
    public enum HurtKind { Normal, Desintegration}
    [SerializeField]
    HurtKind hurtKind;


    [SerializeField]
    GameObject bloodKill;
    RigidbodyConstraints2D lockrig = RigidbodyConstraints2D.None;

    private void Awake()
    {
        if (hurtKind == HurtKind.Normal)
        {
            var deg = (ReMath.DegClamp(transform.eulerAngles.z));

            switch (deg)
            {
                case 90:
                case 270:
                    lockrig = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionY;
                    break;

                case 180:
                    lockrig = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionX;
                    break;

            }
        }
    }

    // lockrig = RigidbodyConstraints2D.FreezeRotation | (((ReMath.DegClamp(transform.eulerAngles.z) / 90) % 2 == 0) ? RigidbodyConstraints2D.FreezePositionX : RigidbodyConstraints2D.FreezePositionY);


    private void OnCollisionEnter2D(Collision2D collision)
    {

        bool player = collision.gameObject.layer == 8;

        //    var c = collision.transform.GetComponent<CorpseInteract>();
        //     if (c != null)
        //         c.transform.Find("playerCol").gameObject.SetActive(true);


        if (player || collision.gameObject.tag == "Bloody")
        {
            if (collision.relativeVelocity.magnitude > 1f)
                Instantiate(bloodKill, collision.contacts[0].point, new Quaternion(), GameManager.TempInstances);

            if (player)
            {
                if(hurtKind == HurtKind.Normal)
                GameManager.Gameplay.KillPlayer(GameplayManager.DieEnum.SimpleCorpse, GameplayManager.DieKiller.Spikes);
                else
                    GameManager.Gameplay.KillPlayer(GameplayManager.DieEnum.Desintegration, GameplayManager.DieKiller.Spikes);

            }
        }


        if (hurtKind == HurtKind.Normal)
        {


            if (collision.gameObject.layer == 13)
            {
                collision.gameObject.layer = 9;
                collision.rigidbody.drag = 10;

                var corp = collision.gameObject.GetComponentInParent<CorpseInteract>();
                if (corp != null)
                {
                    corp.ChangeStucks(collision.gameObject, true);
                    if (lockrig == RigidbodyConstraints2D.None)
                    {//si ls pics sont au sol, il faut embrocher correctement le joueur

                        var rigs = corp.GetComponentsInChildren<Rigidbody2D>();
                        foreach (var item in rigs)
                            item.gameObject.layer = 9;
                    }
                }

            }


            //    collision.rigidbody.bodyType = RigidbodyType2D.Static;

            var rg = collision.gameObject.GetComponent<Rigidbody2D>();
            if (rg != null)
                rg.constraints = lockrig;




        }
    }


    private void OnCollisionExit2D(Collision2D collision)
    {
        if (hurtKind != HurtKind.Normal) return;
        

            var rg = collision.gameObject.GetComponent<Rigidbody2D>();
        if (rg != null)
            rg.constraints = RigidbodyConstraints2D.None;

        if (collision.gameObject.layer == 9)
        {


            var corp = collision.gameObject.GetComponentInParent<CorpseInteract>();

            if (corp != null)
            {
                if (lockrig != RigidbodyConstraints2D.None)
                {//si ls pics ne sont pas au sol
                    corp.ChangeStucks(collision.gameObject, false);
                    collision.rigidbody.drag = 1;
                    collision.gameObject.layer = 13;
                }
                else
                {

                }
            }



        }

    }
}
