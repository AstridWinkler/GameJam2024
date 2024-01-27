using UnityEngine;
using System.Collections;

public class Logic_SimpleDoor : LogicBlock_Base
{

    [SerializeField]
    Animator anim;
    [SerializeField]
    bool state;

    bool invert;
   


    private void Awake()
    {
        invert = state;
        state = false;

        UpdateAnim();
    }

    public override void SetState(object obj)
    {
        var st = (bool)obj;
        if (st != state)
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
        anim.SetBool("state", state ^ (invert) );
    }

}