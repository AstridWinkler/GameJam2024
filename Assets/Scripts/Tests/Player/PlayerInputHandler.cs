using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputHandler : MonoBehaviour
{
    private PlayerMovementHandler playerMovementHandler;

    private void Awake()
    {
        playerMovementHandler = GetComponent<PlayerMovementHandler>();
    }

    private void Update()
    {
        playerMovementHandler.SetBoost(Input.GetKey("left shift") || Input.GetKey("right shift"));
        playerMovementHandler.Move(Input.GetAxis("Horizontal"));
        if(Input.GetKeyDown("space"))
        playerMovementHandler.JumpPressDown();
        if (Input.GetKeyUp("space"))
            playerMovementHandler.JumpPressUp();





    }
}
