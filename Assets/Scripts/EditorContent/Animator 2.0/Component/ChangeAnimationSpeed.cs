using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeAnimationSpeed : MonoBehaviour {

	public float time = 1f;
	public float newSpd = 1f;


	void Start () {
		Invoke ("Change", time);
	}


	void Change(){
		SimpleAnimationPlayer sp = GetComponent<SimpleAnimationPlayer> ();
		if (sp == null)
			sp = GetComponentInParent<SimpleAnimationPlayer> ();

		sp.Speed = newSpd;
		sp.StartAnim (true);
	}


}
