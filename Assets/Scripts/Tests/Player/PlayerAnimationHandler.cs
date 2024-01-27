using logiked.source.extentions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AnimatorRenderer2D = logiked.Tool2D.animation.AnimatorRenderer2D;


public class PlayerAnimationHandler : MonoBehaviour
{
    private PlayerCollisionHandler playerCollisionHandler;
    private Transform tr;
    private Rigidbody2D rb;

    [SerializeField]
    private logiked.Tool2D.animation.AnimatorRenderer2D animatorRend;
    private enum AnimState { Idl = 0, Running = 1, Walling = 2, Jumping = 3, Falling = 4 }
    private AnimState currentAnimState;

    private float startScaleX;

    private void Awake()
    {
        playerCollisionHandler = GetComponent<PlayerCollisionHandler>();
        animatorRend = GetComponent<AnimatorRenderer2D>();
        rb = GetComponent<Rigidbody2D>();
        tr = transform;
        startScaleX = tr.localScale.x;
    }



    private void Update()
    {
        if(playerCollisionHandler.OnRightWall())
        {//look left
            tr.localScale = new Vector3(-startScaleX , tr.localScale.y , tr.localScale.z);
            currentAnimState = AnimState.Walling;
        }
        else if(playerCollisionHandler.OnLeftWall())
        {//look right
            tr.localScale = new Vector3(startScaleX , tr.localScale.y , tr.localScale.z);
            currentAnimState = AnimState.Walling;
        }
        else
        {//player isn't on walls

            // float moveDir = rb.velocity.x;//on air

            // if(playerCollisionHandler.OnGround())
            float moveDir = 0;


      

            currentAnimState = AnimState.Idl;




            if (playerCollisionHandler.OnAloft())
            {
                if (rb.velocity.y > 3)
                {
                    currentAnimState = AnimState.Jumping;
                }
                else
                {
                    currentAnimState = AnimState.Falling;
                }
                 moveDir = rb.velocity.x;

            }
            else
            {
                moveDir = Input.GetAxisRaw("Horizontal");
                if (rb.velocity.x.Abs() > 0f && moveDir != 0 && !playerCollisionHandler.OnGroundWall())
                    currentAnimState = AnimState.Running;
            }

            if (moveDir > 0)//look right
                tr.localScale = new Vector3(startScaleX, tr.localScale.y, tr.localScale.z);
            if (moveDir < 0)//look left
                tr.localScale = new Vector3(-startScaleX, tr.localScale.y, tr.localScale.z);


  
        }

        animatorRend.SetStateValue("idl", (int)currentAnimState);

    }
}
