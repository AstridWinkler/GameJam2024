using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerKill : MonoBehaviour
{
    [SerializeField]
    bool isEnabled=true;
    [SerializeField]
    GameplayManager.DieEnum dieMode = GameplayManager.DieEnum.Explosion;
    [SerializeField]
    GameplayManager.DieKiller killerOption = GameplayManager.DieKiller.None;

    void OnTriggerEnter2D(Collider2D col)
    {
        if (!isEnabled) return;

        if (col.gameObject.layer == 8 && col.gameObject.tag == "Player")
        {
            GameManager.Gameplay.KillPlayer(dieMode, killerOption);
        }
    }
}
