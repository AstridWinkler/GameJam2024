using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AddForceOnSpawn : MonoBehaviour
{


    [SerializeField]
    bool local;

    [SerializeField]
    Vector2 dir;
	[SerializeField]
	Vector2 randomVariation;


	[SerializeField]
	float angularDir;
	[SerializeField]
	float randomAngularVariation;

	void OnEnable()
    {
		Rigidbody2D rig = GetComponent<Rigidbody2D>();

		if (rig == null)
			return;

		Vector2 finalDir = dir + new Vector2(Random.Range(-randomVariation.x, randomVariation.x), Random.Range(-randomVariation.y, randomVariation.y));
		float finalAng = angularDir + Random.Range(-randomAngularVariation, randomAngularVariation);

		rig.angularVelocity = finalAng;

		if (local) 
			rig.AddRelativeForce(dir, ForceMode2D.Impulse);
        else

			rig.AddForce(dir, ForceMode2D.Impulse);
    }


}
