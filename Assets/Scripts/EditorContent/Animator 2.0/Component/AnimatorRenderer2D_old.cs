using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorRenderer2D_old : SimpleAnimationSystem {


	public static Animation2D nullAnim{ get; set;}

	[SerializeField]
	 Animator2D controller = null;

	[SerializeField]
	SpriteRenderer[] rends;

	//privatiser tout ca get{} plus tard
	[SerializeField]
	varion[] vars;


	public float[] Percents { get { return percents; } }
	[SerializeField]
	float[] percents;


	 bool varChanged;


	IEnumerator[] voids;
	Tra_LoopPack[] actWaits ;


	[System.Serializable]
	public class NanoLayer {

		public NanoLayer(int id_, AnimatorLayer2D lay) {
			frmId = 0;
			id = id_;
			refLayer = lay;
			defAnim = lay.defaultAnimId;

		}


		public AnimatorLayer2D RefLayer { get{ return refLayer; } }
	[SerializeField]
		AnimatorLayer2D refLayer;

		public int Id { get { return id; } }
		[SerializeField]
		int id;

		public int DefAnim { get { return defAnim; } }
	[SerializeField]
		int defAnim;
	[SerializeField]
		public  int frmId { get; set; }
	[SerializeField]
		bool varChanged;
	[SerializeField]
		bool frameRunning;

		public Animation2D ActAnim { get { return actAnim; } set { actAnim = value; } }
		[SerializeField]
		Animation2D actAnim;

		public AnimatorBlock2D ActBlock { get { return actBlock; } set { actBlock = value; }  }
		 AnimatorBlock2D actBlock;


	}

	[SerializeField]
	NanoLayer[] layers;

	bool forceUpdate;



	void InitResources()
	{



		if (started)
			return;

		started = true;


		if (rends == null)
			rends = GetComponentsInChildren<SpriteRenderer>();


		layers = new NanoLayer[rends.Length];
		vars = new varion[controller.vars.Length];
		percents = new float[rends.Length];
		voids = new IEnumerator[controller.ListLayers.Length];
		actWaits = new Tra_LoopPack[controller.ListLayers.Length];




		for (int x = 0; x < vars.Length; x++)
			vars[x] = controller.vars[x].Clone();

	}

	void OnEnable()
	{
		if (started)
			LaunchAnim ();


	}

	protected new void Start(){

		if (started)
			return;

	

		if (autoStart)
			LaunchAnim ();
		else
			InitResources();

	}



	protected override void OnSpeedChanged()
	{
		if (actWaits == null)
			return;

		for (int i = 0; i < actWaits.Length; i++)
			if (actWaits[i] != null && actWaits[i].isRunning)
				actWaits[i].StopAnim();


		
	}



	protected override void LaunchAnim () {
		int x;



		InitResources();




		for (x = 0; x < layers.Length; x++) {

			
			layers [x] = new NanoLayer (x, controller.ListLayers [x] );
			voids[x] = NextFrame (layers [x]);
			percents [x] = 0;
			StartAnimation ( layers [x].DefAnim, ref layers [x]);


		}


		

	}

	public void SetDefaultAnim(int layer){
	//	if (voids [layer] != null)
	//		StopCoroutine (voids [layer]);
		
		SetAnim (ref layers [layer], layers [layer].DefAnim);

	//	voids[layer] = NextFrame (layers [layer]);
	//	StartCoroutine (voids [layer]);

	}

	public int GetParameter(string name)
	{
		for (int p = 0; p < vars.Length; p++)
		{
			if (vars[p] == null)
				continue;

			if (vars[p].name == name)
			{
				return vars[p].val;
			}
		}
		return 0;
	}

			public void SetParameter(string name, int val, bool forceUpdate_ = false)
	{

		if (!started && autoStart)
			Start();

		for (int p = 0; p < vars.Length; p++) {
			if (vars [p] == null)
				continue;
			
			if (vars [p].name == name) {
				
				if (vars [p].val == val)
					return;
				
				vars [p].val = val;
				varChanged = true;
				forceUpdate = forceUpdate_;
				if (forceUpdate)
					Update();

			//	for (b = 0; b < layers.Length; b++) {
			//	layers [p].varChanged = true;

				/*
				if (CheckAnim (layers [b], false)) {
					while (CheckAnim (layers [b], false)) {

					}
					StartLoop (layers [b]);				
				}*/



				//	}

				return;
			}
		}
			
	}



	void StartAnimation(int id, ref NanoLayer lay){

		lay.ActBlock = lay.RefLayer.ListAnims [id];
		lay.ActAnim = lay.ActBlock.animation;
		if (lay.ActAnim == null)
		{
			lay.ActAnim = nullAnim;
			CheckAnim(lay, true);

		}
		else
		{
            if(CheckNotNullAnim(lay))
			rends[lay.Id].sprite = lay.ActAnim.spritePacks[equivalent].skins[skin].sprites[((lay.ActBlock.coefSpeed < 0) ? lay.ActAnim.spritePacks[equivalent].skins[skin].sprites.Length : 0)];
		}
		lay.frmId = 0;

		//LoopAnimator(lay);
		StartLoop(lay);
	}


	 
	 public void Update(){
		int b = 0;
		int secu = 0;
	
		if (varChanged) {


			for (b = 0; b < layers.Length; b++) {
				secu = 0;
				while (secu++ < 100 && CheckAnim (layers[b], forceUpdate))
					;

			}
			forceUpdate = false;
			varChanged = false;
		}


	}


	void StartLoop(NanoLayer lay){
		
		if (voids [lay.Id] != null)
			StopCoroutine (voids [lay.Id]);

		voids[lay.Id] = NextFrame (layers [lay.Id]);
		StartCoroutine (voids [lay.Id]);
	}


	void StopAnim(NanoLayer lay){
		if (voids [lay.Id] != null)
			StopCoroutine (voids [lay.Id]);
	}


		/*
	void LoopAnimator(NanoLayer lay){
		Debug.Log("start1");
		lay.frameRunning = true;

		StartCoroutine (voids[lay.id]);	

		Debug.Log("end1");
		//actLoop
	}
		*/


	int endCheck;

	public void ResetFrmid(int layerId)
	{
		layers[layerId].frmId = 0;
	}


    bool CheckNotNullAnim(NanoLayer lay)
    {
        return lay.ActAnim != null && lay.ActAnim.spritePacks.Length != 0 && lay.ActAnim.spritePacks[equivalent].skins[skin].sprites.Length > 0;
    }


	IEnumerator NextFrame ( NanoLayer lay) {
		int frmcnt = lay.ActAnim.spritePacks [equivalent].skins[skin].sprites.Length;
		int secu;
		bool chanim = false;
		Tra_LoopPack tra = new Tra_LoopPack(Mathf.Abs((lay.ActAnim.spritePacks[equivalent].duration / frmcnt) / (lay.ActBlock.coefSpeed * speed)), GameStateType.animation, GameStateType.pauseMenu);	
	

		percents[lay.Id] = (float)lay.frmId * 100f / (lay.ActAnim.spritePacks [equivalent].skins [skin].sprites.Length - 1);

		actWaits[lay.Id] = tra;

		yield return new WaitUntil (() => !tra.isRunning);



		while (GameManager.StateGame.AnimationsPaused) {
			yield return null;
		}


		lay.frmId++;		
	
		if (lay.frmId >= frmcnt) {

			if(lay.ActBlock.looping)
				lay.frmId = 0;
			else 	lay.frmId--;
		//	Debug.LogWarning  ("check0:");
		//	print (lay.ActAnim.name);
			chanim = CheckAnim ( lay, true);
		//	print (lay.ActAnim.name);
		}

		secu = 0;
		while (secu++ < 100) {
  
            if (!CheckAnim(lay, false)) break;

		}

		lay.frmId = ReMath.MaxLim (lay.frmId, lay.ActAnim.spritePacks [equivalent].skins[skin].sprites.Length - 1);
		try{
            if(CheckNotNullAnim(lay))
			rends[lay.Id].sprite = lay.ActAnim.spritePacks [equivalent].skins[skin].sprites [((lay.ActBlock.coefSpeed >= 0)? lay.frmId: lay.ActAnim.spritePacks [equivalent].skins[skin].sprites.Length - lay.frmId -1)];
		}catch {
			Debug.LogError ("Erreur avec un frmid de " + lay.frmId.ToString () + " sur lanimation " + lay.ActAnim.name);
		}
		//frmId[layer] %= ActAnims[layer].skins [skin].sprites.Length;

	

		StartLoop (lay);
		yield break;


		//StartCoroutine(voids[lay.Id]);
	}





	/*
	AnimatorBlock2D blockId(int id, int lay){
		return controller.ListLayers [lay].ListAnims [id];
	}*/

	 bool SetAnim(ref NanoLayer lay, int blokId){
		
		if (lay.ActBlock == controller.ListLayers [lay.Id].ListAnims [blokId])
			return false;

		lay.frmId = 0;
		lay.ActBlock = controller.ListLayers [lay.Id].ListAnims [blokId];
		lay.ActAnim = lay.ActBlock.animation;
		if (lay.ActAnim == null)
			lay.ActAnim = nullAnim;

        if (CheckNotNullAnim(lay))
            rends[lay.Id].sprite = lay.ActAnim.spritePacks [equivalent].skins[skin].sprites[((lay.ActBlock.coefSpeed > 0)?0 : lay.ActAnim.spritePacks [equivalent].skins[skin].sprites.Length-1) ];

		return true;

	}



	bool CheckCond(condition cd){
		switch (cd.type){

		case CondType.Equal:
			if (vars [cd.var1].val == cd.value)
				return true;
			
		break;

		case CondType.NotEqual:
			if (vars [cd.var1].val != cd.value)
				return true;

			break;

		case CondType.Greater:
			if (vars [cd.var1].val > cd.value)
				return true;

			break;


		case CondType.Lesser:
			if (vars [cd.var1].val < cd.value)
				return true;

			break;

			}


	return false;
	}


	bool CheckAnim(NanoLayer lay , bool end_frm){
		bool transOk;
		int j, i;
		for (j = 0; j < lay.ActBlock.transitions.Length; j++) {

			if (lay.ActBlock.transitions [j].endAction == AnimEnd.End_OR && end_frm) {
				SetAnim (ref lay, lay.ActBlock.transitions [j].toAnimId);
				return true;
			}

			if(lay.ActBlock.transitions [j].conditions.Length == 0) return false;
				
			transOk = true;

			for (i = 0; i < lay.ActBlock.transitions [j].conditions.Length; i++) {				 
				if (!CheckCond (lay.ActBlock.transitions [j].conditions[i])) {
					transOk = false;
					break;
				}
			}
			if (transOk) {

				if (end_frm || lay.ActBlock.transitions [j].endAction != AnimEnd.End_AND) {
					SetAnim (ref lay, lay.ActBlock.transitions [j].toAnimId);
					return true;
			
				}
			}
		}

		return false;
	}


	public bool CheckConds(AnimatorBlock2D anim){
		return false;
	}




	void Printsprite () {

	}


}
