using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerEndLevel : MonoBehaviour
{
       
    void OnTriggerEnter2D(Collider2D col)
    {
     if(col.gameObject.tag == "Player")
        GameManager.Gameplay.LevelFinished();
    }
}
