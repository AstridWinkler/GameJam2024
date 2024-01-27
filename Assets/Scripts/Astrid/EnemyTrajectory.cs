using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyTrajectory : MonoBehaviour
{
    public Transform objectToFollow;
    //public Vector3 offset;

    public float MoveSpeed;
    public float MaxDist;
    public float MinDist;
    public Vector3 dist;

    // Start is called before the first frame update
    void Start()
    {
        objectToFollow = GameManager.Gameplay.CurrentPlayer.transform;
    }

    void Update()
    {

        //transform.LookAt(Player);

        if (Vector3.Distance(transform.position, objectToFollow.position) >= MinDist)
        {
            var step = MoveSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, objectToFollow.position, step);

            //if (Vector3.Distance(transform.position, objectToFollow.position) <= MaxDist)
            //{
                //Here Call any function U want Like Shoot at here or something
            //}

        }

    }


}
