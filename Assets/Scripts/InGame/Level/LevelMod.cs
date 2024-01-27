using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LevelMod : MonoBehaviour
{



    public virtual void OnSaveCheckpoint() { }
    public virtual void OnBackToCheckPoint() { }
    public virtual void OnRemoveCheckpoint() { }
    public virtual void ExitMod() { }

    public virtual void OnFixedUpdate() { }
    public virtual void OnUpdate() { }
 


}
