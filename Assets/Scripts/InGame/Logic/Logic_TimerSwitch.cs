using System;
using UnityEngine;

class Logic_TimerSwitch : LogicBlock_Base, I_Interactable
{
    [SerializeField]
    float duration = 1f;

    SpriteRenderer rd;

    [SerializeField]
    float cooldown = 0.5f;

    bool value;
    [SerializeField]
    bool invertValue;

    [SerializeField]
    Animator anim;

    Tra_LoopPack incooldown;

    private void Awake()
    {
        rd = GetComponentInChildren<SpriteRenderer>();
    }

    void Start()
    {
        UpdateAnim();
        
    }

    void UpdateAnim()
    {
        //anim.SetBool("state", value);
    }

    public bool CanPlayerAction()
    {
        return incooldown.NullOrInactive();
    }

    public override void Action()
    {
        value = !value;
        UpdateAnim();
        base.UpdateConnectedLogicsSetState(value ^ invertValue);
        if(rd != null) rd.color = Color.red;
        incooldown = new Tra_LoopPack(Reverse, duration, GameStateController.Wait_MobClassic);
    }

    void Reverse()
    {
        if (rd != null) rd.color = Color.white;

        incooldown = new Tra_LoopPack(cooldown, GameStateController.Wait_MobClassic);
        value = !value;
        UpdateAnim();
        base.UpdateConnectedLogicsSetState(value ^ invertValue);


    }



    bool I_Interactable.CallPlayerAction(PlayerBase source)
    {
        if (!CanPlayerAction())
            return false;

        Action();
        return true;
    }

    bool I_Interactable.CanPlayerAction()
    {
        return true;
    }

}

