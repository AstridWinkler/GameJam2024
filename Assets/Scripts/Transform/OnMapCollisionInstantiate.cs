using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnMapCollisionInstantiate : MonoBehaviour
{
	[SerializeField]
	float minForce = 0.3f;
	[SerializeField]
	GameObject toInstantiate;
    
    [SerializeField]
	bool sizeByForce;
	[SerializeField]
	float size_forceCoef;
	[SerializeField]
	Vector2 size_forceInterval;



	private void OnCollisionEnter2D(Collision2D collision)
	{



		if (collision.contactCount > 0 && collision.gameObject.layer == 9 && collision.relativeVelocity.magnitude > minForce)
		{

            GameObject gb = Instantiate(toInstantiate, collision.contacts[0].point, new Quaternion(), GameManager.TempInstances);
			if (sizeByForce) {
				gb.transform.localScale *=  ReMath.Clamp( collision.relativeVelocity.magnitude *  size_forceCoef, size_forceInterval.x, size_forceInterval.y);

					}
		}
	}

}
