using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyTrajectory : MonoBehaviour
{
    public Transform objectToFollow;
    public Transform eyes;
    public Transform tail;
    public Transform tail2;

    float eyeDist = 0.5f;
    float tailDist = 1.3f;
    float tail2Dist = 0.8f;
    float distToBody;
    Vector3 distToTarget;

    public float MoveSpeed;
    public float MaxDist;
    public float MinDist;

    // Start is called before the first frame update
    void Start()
    {
        if (GameManager.Gameplay.CurrentPlayer != null)
            objectToFollow = GameManager.Gameplay.CurrentPlayer.transform;
    }

    private void directionDisplacement(Vector3 fromObject, Vector3 toObject)
    {

        var targetDistToBody = (toObject - fromObject).normalized;
        distToTarget = Vector3.Lerp(distToTarget, targetDistToBody, 3.5f * Time.deltaTime);

        eyes.localPosition = distToTarget * eyeDist;
        tail.localPosition = -distToTarget * tailDist;
        tail2.localPosition = -distToTarget * tail2Dist;

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.gameObject.tag == "Player")
        {
            GameManager.Gameplay.KillPlayer();
        }
    }

    void Update()
    {
        


        if (objectToFollow == null)
        {
            Start();
            directionDisplacement(transform.position, transform.position);
            return;
        }

        distToBody = Vector3.Distance(transform.position, objectToFollow.position);

        if (distToBody <= MaxDist)
        {
            var step = MoveSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, objectToFollow.position, step);

            directionDisplacement(transform.position, objectToFollow.position);
        }
        else
        {
            directionDisplacement(transform.position, transform.position);
        }



    }


}
