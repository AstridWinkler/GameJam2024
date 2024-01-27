using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorpseInteract : MonoBehaviour, I_Interactable
{

    Rigidbody2D grabTorso;

    bool isGrabbed;

    public bool Stucked { get { return stuckedList.Count != 0; } }
    List<GameObject> stuckedList = new List<GameObject>();


    void Awake()
    {
        grabTorso = GetComponent<Rigidbody2D>();
    }

    public bool CallPlayerAction(PlayerBase source)
    {
        if (!CanPlayerAction()) return false;

        PlayerController player = (PlayerController) source;
        if (player == null) return false;

        isGrabbed = !isGrabbed;

        if (isGrabbed)
        {
            grabTorso.constraints = RigidbodyConstraints2D.FreezeAll;
            grabTorso.isKinematic = true;
            var col = GetComponentsInChildren<Collider2D>();
            var pcol = source.GetComponent<Collider2D>();
            foreach (var c in col) { Physics2D.IgnoreCollision(c, pcol); Physics2D.IgnoreCollision(pcol, c); }
            grabTorso.transform.SetParent(source.transform);
            grabTorso.transform.localPosition = Vector3.up * 0.5f;
            grabTorso.transform.eulerAngles = new Vector3(0, 0, 90);

            player.GrabCorpse(grabTorso);
        }
        else
        {
            grabTorso.transform.SetParent(GameManager.TempInstances);
            grabTorso.constraints = RigidbodyConstraints2D.None;
            grabTorso.isKinematic = false;
            player.GrabCorpse(null);
        }



        return true;
    }

    public bool CanPlayerAction()
    {
        return !Stucked;
    }




    public void ChangeStucks(GameObject rig, bool toStuck)
    {
        if (toStuck)
            stuckedList.Add(rig);
        else
            stuckedList.Remove(rig);
    }

}
