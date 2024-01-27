using System;
using UnityEngine;


public abstract class LogicBlock_Base : MonoBehaviour
{

    [SerializeField]
    LogicBlock_Base[] connectedTo;

    public abstract void Action();
    public virtual void SetState(object obj) { Action(); }


    public bool Enabled { get { return true; } }


    protected virtual void UpdateConnectedLogics()
    {
        foreach (var o in connectedTo)
            o.Action();
    }
    protected virtual void UpdateConnectedLogicsSetState(object obj)
    {
        foreach (var o in connectedTo)
            o.SetState(obj);
    }


}


