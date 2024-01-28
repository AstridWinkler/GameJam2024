using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetActive : MonoBehaviour
{
    [SerializeField]
    bool state;
    [SerializeField]
    GameObject obj;
    [SerializeField]
    float time;

    void OnEnable()
    {

        new Tra_LoopPack(()=> { obj.SetActive(state); }, time, GameStateController.Wait_MobClassic);
    }


}
