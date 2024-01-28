using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KataInputHandler : MonoBehaviour
{
    private KataPlayerController playerMovementHandler;


   public InputController inputs;

    private void Start()
    {
        playerMovementHandler = GetComponent<KataPlayerController>();
        inputs = GameManager.Inputs.GetController();
    }

    private void Update()
    {
        playerMovementHandler.Move(Input.GetAxis("Horizontal"));

        if(inputs.GetKey("Jump", keyMode.DownPress))
        playerMovementHandler.JumpPressDown();

        if(inputs.GetKey("Jump", keyMode.UpPress))
            playerMovementHandler.JumpPressUp();
        


        playerMovementHandler.SetBoost(inputs.GetKey("Run", keyMode.Press));

        

     //   if (inputs.GetKey("Suicide", keyMode.DownPress))        
      //      playerMovementHandler.InputKill();


        //  playerMovementHandler.InputRun(inputs.GetKey("Run", keyMode.Press));

        if (inputs.GetKey("Action", keyMode.DownPress))
            playerMovementHandler.InputAction();

        // if (inputs.GetKey("Place Spawn point", keyMode.DownPress))
        //   playerMovementHandler.PlaceSpawnPoint();

        //if (inputs.GetKey("Remove Spawn point", keyMode.DownPress))
        //    playerMovementHandler.RemoveSpawnPoint();


        if (Input.GetKeyDown(KeyCode.S))
        {
            GameManager.Gameplay.TravelShimerImagePopup();
            playerMovementHandler.PlaceSpawnPoint();
            playerMovementHandler.MoveDownShimmer();
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            playerMovementHandler.InputKill();

            if (GameManager.Gameplay.currentDepth > 0)
            {
                playerMovementHandler.InputKill();
          playerMovementHandler.RemoveSpawnPoint();
                playerMovementHandler.MoveUpShimmer();
            }
            else
            {
      
            }

        }


        /*
        if (Input.GetKeyDown(KeyCode.G))
        {
            //Vector3 current = GameManager.Gameplay.CurrentSpawnPointPosition;
            //PlaceSpawnPoint((Vector2)transform.position+ new Vector2(LookingDirection?1:-1, 0)*10f);

            if ((inputs.GetAxis("Right", keyMode.Press) - inputs.GetAxis("Left", keyMode.Press)) != 0)
                dashDir = (inputs.GetAxis("Right", keyMode.Press) - inputs.GetAxis("Left", keyMode.Press));

            GameManager.Gameplay.KillPlayer(GameplayManager.DieEnum.Explosion, respawnPos: (Vector2)transform.position + new Vector2(dashDir, 0) * 7.5f);
            //PlaceSpawnPoint(current);
        }*/




    }
}
