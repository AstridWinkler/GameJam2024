using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace logiked.source.manager
{

    public abstract class IBaseManager : MonoBehaviour
    {
       abstract public void Initialization();

       abstract protected void InitManager();
       //abstract public  GameObject gameObject { get; }
    }
}
