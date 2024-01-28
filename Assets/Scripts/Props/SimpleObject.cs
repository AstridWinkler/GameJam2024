using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class SimpleObject : MonoBehaviour
{

    public bool cannotBeGrab;
    public bool onlyOnceGrab;




    public SpriteRenderer Rend => rend;
        [SerializeField]
   private SpriteRenderer rend;

    private bool used;


    public Rigidbody2D Rig => rig;
        [SerializeField]
    private Rigidbody2D rig;

    public Collider2D Col => col;
    [SerializeField]
    private Collider2D col;

    public bool IsUsed
    {
        get { return used; }
    }



    public void SetHilight(bool val)
    {
        if (val)
            rend.color = Color.red;
        else
            rend.color = Color.white;
    }



    public void SetGrab()
    {
        used = true;
        if (onlyOnceGrab)
            cannotBeGrab = true;

        Rig.bodyType = RigidbodyType2D.Kinematic;

    }
    public  void SetReleased()
    {
        used = false;
        transform.SetParent(GameManager.TempInstances);
        Rig.bodyType = RigidbodyType2D.Dynamic;

    }


    void Awake()
    {
        if (rend == null)
            rend = GetComponent<SpriteRenderer>();

        if (rig == null)
            rig = GetComponent<Rigidbody2D>();

        if (col == null)
            col = GetComponent<Collider2D>();

        


    }



}
