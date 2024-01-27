using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Logic_GroundButton : LogicBlock_Base
{

    Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public override void Action()
    {
        throw new System.NotImplementedException();
    }


    List<Collider2D> content = new List<Collider2D>();




    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer != 8 && other.gameObject.layer != 13 && other.gameObject.tag != "Player" )
            return;

        content.Add(other);
        ReliableOnTriggerExit.NotifyTriggerEnter(other, gameObject, OnTriggerExit2D);

        anim.SetBool("state", true);
        UpdateConnectedLogicsSetState(true);


      //  Debug.Log("OnTriggerEnter");
    }
    

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer != 8 && other.gameObject.layer != 13 && other.gameObject.tag != "Player")

            return;

        content.Remove(other);
        content = content.Where((a) => a != null).ToList();
        

        ReliableOnTriggerExit.NotifyTriggerExit(other, gameObject);

        

            anim.SetBool("state", content.Count != 0);
            UpdateConnectedLogicsSetState(content.Count != 0);
        
      //  Debug.Log("OnTriggerExit");
    }




}
