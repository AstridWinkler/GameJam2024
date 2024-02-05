using UnityEngine;
using System.Collections;
using logiked.audio;

public class Logic_SimpleDoor : LogicBlock_Base
{

    [SerializeField]
    Animator anim;
    [SerializeField]
    bool state;

    bool invert;

    [SerializeField]
    GameAudioSource source;
    [SerializeField]
    GameSound openSound;
    [SerializeField]
    GameSound closeSound;

    private void Awake()
    {
        invert = state;
        state = false;

        UpdateAnim();
    }

    public override void SetState(object obj)
    {
        var st = (bool)obj;
        {
            state = st;
            UpdateAnim();
        }
    }

    public override void Action()
    {
        state = !state;
        UpdateConnectedLogics();
        UpdateAnim();
    }

    void UpdateAnim()
    {
        var val = state ^ invert;

        if (source != null)
        {
            if (val && openSound != null) source.PlaySound(openSound);
            if (!val && closeSound != null) source.PlaySound(closeSound);

        }

        anim.SetBool("state", state ^ (invert) );
    }

}