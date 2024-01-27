using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementOptions : MonoBehaviour
{
    private PlayerCollisionHandler playerCollisionHandler;
    private Rigidbody2D rb;
    
    private bool collidedWithWall;
    private void Awake()    
    {
        rb = GetComponent<Rigidbody2D>();
        playerCollisionHandler = GetComponent<PlayerCollisionHandler>();
    }

    private void FixedUpdate()
    {

        /*
        //player can't moveing to left when that's collided with left wall
        if (playerCollisionHandler.OnTouchingLeftWall())
        {
             rb.velocity = new Vector2(playerCollisionHandler.OnLeftWall() ? -3f : 3f, rb.velocity.y);
            //     rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x, -3, int.MaxValue), rb.velocity.y);
        }
        //player can't moveing to right wehn that's collided with right wall
        if (playerCollisionHandler.OnTouchingRightWall())
        {
       //     rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x, -int.MaxValue, 3), rb.velocity.y);
        }*/
        

        if (playerCollisionHandler.OnWall() && !collidedWithWall)
        {
            //on player right wall enter...
            //player collided with a wall
           // collidedWithWall = true;
         //  rb.velocity = new Vector2(playerCollisionHandler.OnLeftWall() ? -3f : 3f, rb.velocity.y);//keep velocity.y and reset velocity.x (wall slide)
        }
        if(!playerCollisionHandler.OnWall())
        {
            collidedWithWall = false;
        }


    }
}
