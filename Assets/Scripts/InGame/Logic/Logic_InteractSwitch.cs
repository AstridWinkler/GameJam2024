using System;
using UnityEngine;

class Logic_InteractSwitch : LogicBlock_Base, I_Interactable
{


    [SerializeField]
    float cooldown = 0.5f;
    Tra_LoopPack cooldown_;

    [SerializeField]
    bool value;

    [SerializeField]
    Animator anim;

    Tra_LoopPack incooldown;

    void Start()
    {
        UpdateAnim();
    }

    void UpdateAnim()
    {
        anim.SetBool("state", value);
    }

    public bool CanPlayerAction()
    {
        return incooldown.NullOrInactive();
    }

    public override void Action()
    {
        if (cooldown_.NullOrInactive())
        {
            value = !value;
            UpdateAnim();
            cooldown_ = new Tra_LoopPack(cooldown, GameStateController.Wait_MobClassic);
            base.UpdateConnectedLogics();

        }
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

