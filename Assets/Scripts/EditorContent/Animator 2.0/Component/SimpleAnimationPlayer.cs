using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleAnimationPlayer : SimpleAnimationSystem {

	SpriteRenderer rd;
	public Animation2D anim;

	public bool loop = true;


	//Vector2Int equich;

	Coroutine actCor = null;
	int frmId;

	void OnEnable(){
		rd = GetComponent<SpriteRenderer> ();
		if (rd == null)
			rd = GetComponentInChildren<SpriteRenderer> ();

		if (autoStart)
			LaunchAnim ();
		
		//equich = new Vector2Int (equivalent, skin);
	}




	protected override void LaunchAnim(){
		started = true;
		StartAnim (true);
	}


	public void SetSpeed(float newSpeed){
		bool ch = ReMath.sng (newSpeed) == ReMath.sng (speed);	
		speed = newSpeed;
		if (!ch){
			if(actCor != null) 
			StopCoroutine (actCor);
			StartAnim (true);
		}
	}

	public void SetEquivalent(int newEqui){
		if (newEqui == equivalent)	
			return;
		equivalent = newEqui;			
		if (actCor != null) 
			StopCoroutine (actCor);		
			StartAnim (true);
	}

	public void SetSkin(int newSkin){
		if (newSkin == skin)	
			return;
		skin = newSkin;			
		if (actCor != null) 
			StopCoroutine (actCor);		
		StartAnim (true);
	}


	public void StartAnim(bool reset = false){

		if (!gameObject.activeInHierarchy)
			return;

		if(reset)
			frmId = 0;

		if (actCor != null)
			StopCoroutine (actCor);

	//	print ("animloop");
		actCor = StartCoroutine (LoopAnim());

		return;
	}


	void Update(){
		/*
		if (equich.x != equivalent || equich.y != skin) {
			StopCoroutine (actCor);
			equich = new Vector2Int (equivalent, skin);
			StartAnim ();
		}*/
	}


	IEnumerator LoopAnim(){

		int frmcnt = anim.spritePacks [equivalent].skins[skin].sprites.Length;

		if (speed == 0)
			speed = 0.01f;
		
		if(frmId < anim.spritePacks[equivalent].skins[skin].sprites.Length)
		 rd.sprite = anim.spritePacks [equivalent].skins[skin].sprites [((speed >= 0)? frmId: anim.spritePacks [equivalent].skins[skin].sprites.Length - frmId -1)];

		frmId++;

		if (loop)
			frmId %= frmcnt;
		else
			frmId = ReMath.MaxLim (frmId, frmcnt-1);

		//yield return new WaitUntil(() => TransformAnim.WaitUntilAuto("anim_wait_" + rd.gameObject.GetInstanceID().ToString(), Mathf.Abs((anim.spritePacks[equivalent].duration / frmcnt) / speed), GameStateType.animation));

		yield return new WaitForSeconds ( Mathf.Abs(( anim.spritePacks[equivalent].duration/frmcnt) / speed));


		if(GameManager.StateGame != null)
		while ( GameManager.StateGame.AnimationsPaused) {
			yield return null;
		}


			StartAnim ();

		yield break;
	}





}
