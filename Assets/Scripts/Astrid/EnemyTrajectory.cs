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
    Vector3 distToBody;

    public float MoveSpeed;
    public float MaxDist;
    public float MinDist;

    // Start is called before the first frame update
    void Start()
    {
        objectToFollow = GameManager.Gameplay.CurrentPlayer.transform;
    }

    private void directionDisplacement(Vector3 fromObject, Vector3 toObject)
    {

        var targetDistToBody = (toObject - fromObject).normalized;
        distToBody = Vector3.Lerp(distToBody, targetDistToBody, 3.5f * Time.deltaTime);

        eyes.localPosition = distToBody * eyeDist;
        tail.localPosition = -distToBody * tailDist;
        tail2.localPosition = -distToBody * tail2Dist;

    }

    void Update()
    {
        if (objectToFollow == null)
        {
            Start();
        }

        if ((Vector3.Distance(transform.position, objectToFollow.position) >= MinDist) && (Vector3.Distance(transform.position, objectToFollow.position) <= MaxDist))
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
