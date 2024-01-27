using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleDestroyAfter : MonoBehaviour {

	public float time = 1f;
	public GameObject obj;

	void Start () {
		if (obj == null)
			obj = gameObject;
		StartCoroutine (Die ());
	}
	

	IEnumerator Die () {

        Tra_LoopPack t = new Tra_LoopPack(time, GameStateType.pauseMenu, GameStateType.physics);
		yield return new WaitUntil(() => !t.isRunning);
		//yield return new WaitForSeconds (time);
		Destroy (obj);
		yield break;
	}


}
