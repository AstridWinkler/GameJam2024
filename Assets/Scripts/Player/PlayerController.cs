using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerController : PlayerBase
{

    enum GrabMode {Nothing, Overhead, Push }

    [SerializeField]
    GrabMode isGrabing = GrabMode.Nothing;




    [SerializeField]
    GameObject blood;




    public Vector2 lastframeVelocity{ get; private set; }
	/*
	[SerializeField]
	Sprite idl;
	[SerializeField]
	Sprite run;
	[SerializeField]
	Sprite jump;
	[SerializeField]
	Sprite down_jump;
	[SerializeField]
	Sprite wall;
	*/


	[SerializeField]
	PhysicsMaterial2D lowFriction;
	[SerializeField]
	PhysicsMaterial2D playerPhysicBase;
	[SerializeField]
	PhysicsMaterial2D wallFriction;


	[SerializeField]
	float airControl = 0.3f;
	[SerializeField]
	float airMaxSpeedCoef = 1.3f;


	[SerializeField]
	float moveAccel = 1f;


	[SerializeField]
	float maxMoveSpeed = 10f;

	//[SerializeField]
	//float jumpForce = 10f;

	[SerializeField]
	float jumpTime = 0.5f;

	[SerializeField]
	float jumpTimeMural = 0.5f;


	[SerializeField]
	float maxJumpForce = 10f;

	[SerializeField]
	float muralJumpForce = 5f;

	[SerializeField]
	float muralAirControl = 0.3f;


	bool startJump;
	bool inJump;

	Tra_LoopPack jumpCor;



	[SerializeField]
	float fallMultiplier = 2.5f;

	float muralTimeExit = 0.035f;
	[SerializeField]
	float muralTimer = 0f;

	bool runButton;


	//[SerializeField]
	//float lowJumpMultiplier = 2f;

	Vector2 colBottom;


	bool lockMovements;

	const int mapRaycast = 1 | (1 << 9) | (1 << 12);


	protected override void Awake()
    {

		inJump = true;
		jumpCor = new Tra_LoopPack(endJump, 0.001f, GameStateController.Wait_MobClassic);
		base.Awake();
	}


//	bool jumpButtonDown;
	bool jumpButtonPress;
	float muralNormalX;
    Vector2 muralDir;

	bool laserGrounded;

	float dashDir;


	public override void UpdatePlayer()
	{

		if (rig.velocity.magnitude > 0.2f)
            lastframeVelocity = rig.velocity;

        actionKeyPressed = inputs.GetKey("Action", keyMode.DownPress);

        if (Input.GetKeyDown(KeyCode.U))
        {
            Instantiate(blood, transform.position + Vector3.left * 2f, new Quaternion());
        }

        runButton = inputs.GetKey("Run", keyMode.Press);

        //jumpButtonDown = inputs.GetKey ("Jump", keyMode.DownPress);
        jumpButtonPress = inputs.GetKey("Jump", keyMode.Press);



        if (inputs.GetKey("Suicide", keyMode.DownPress) /*|| inputs.GetKey("Suicide2", keyMode.Press)*/)
        {
            GameManager.Gameplay.KillPlayer(GameplayManager.DieEnum.Explosion);
        }

		if (Input.GetKeyDown( KeyCode.G))
		{
			//Vector3 current = GameManager.Gameplay.CurrentSpawnPointPosition;
			//PlaceSpawnPoint((Vector2)transform.position+ new Vector2(LookingDirection?1:-1, 0)*10f);

			if((inputs.GetAxis("Right", keyMode.Press) - inputs.GetAxis("Left", keyMode.Press)) != 0)
				dashDir = (inputs.GetAxis("Right", keyMode.Press) - inputs.GetAxis("Left", keyMode.Press));

		GameManager.Gameplay.KillPlayer(GameplayManager.DieEnum.Explosion, respawnPos:(Vector2)transform.position+ new Vector2(dashDir, 0) * 7.5f);
			//PlaceSpawnPoint(current);
		}








		if (Mathf.Abs(rig.velocity.x) > 0.2f)
            rend.flipX = (rig.velocity.x < 0);

        if (muralContact)
            rend.flipX = muralNormalX == 1;


        if (rig.velocity.x != 0)
        {

            if (Mathf.Abs(rig.velocity.x) > 0.5f)
                anim.SetParameter("idl", (runButton) ? 1 : 4);
            else
                anim.SetParameter("idl", 0);
        }
        else
            anim.SetParameter("idl", 0);


        if (inJump)
        {

            if (canMuralJump)
                anim.SetParameter("idl", 3);
            else// if(startJump)
                anim.SetParameter("idl", 2);
            //else
            //	rd.sprite = down_jump;
            // a faire
        }







        if (inputs.GetKey("Spawn point", keyMode.DownPress))
        {
            PlaceSpawnPoint(transform.position);
        }

        if (inputs.GetKey("Spawn point", keyMode.Press))
        {
            timepress += Time.deltaTime;
            if(timepress > 0.5f)
            {
                GameManager.Gameplay.RemoveSpawnPoint();
                timepress = 0;
            }
        }
        else timepress = 0;
    }

    float timepress = 0;


	void PlaceSpawnPoint(Vector2 pos)
	{

		GameObject spawn = GameManager.Gameplay.PlaceSpawnPoint(pos);

		if (spawn != null)
		{
			SpriteRenderer[] rds = spawn.GetComponentsInChildren<SpriteRenderer>();

			foreach (SpriteRenderer rd in rds)
			{
				rd.sprite = this.rend.sprite;
				rd.flipX = this.rend.flipX;
			}

        }
        else
        {

        }
	}


	bool inMuralJump;

	bool spacePressLock;
	bool spacePress;
	Tra_LoopPack jumpBuf;



	bool EndWallJump(Tra_LoopPack p, params object[] obj)
	{

		if (!jumpButtonPress)
		{
			endJump();
			
			rig.AddForce(Vector2.down*15f, ForceMode2D.Impulse);

			return false;
		}

		if (p.percent == 1)
			endJump();

		return true;
	}



		void endJump()
	{
		inMuralJump = false;
		startJump = false;
	}

	float runSpd;




	void FixedUpdate()
	{
        float usedMaxSpeed = maxMoveSpeed;
        float usedMaxJumpForce = maxJumpForce;
                     
        Vector2 actVel = rig.velocity;
		float xInputs = 0;
		 runSpd = (runButton ? 2f : 1f);


        if(isGrabing != GrabMode.Nothing)
        {
            usedMaxSpeed *= 0.6f;
            usedMaxJumpForce *= 0.6f;
        }


		CheckMural(Vector2.left*0.3f);
		CheckMural(Vector2.right*0.3f);


		spacePress = false;
		



		if (forcingJump)
		{
			spacePress = true;
			jumpButtonPress = true;
		}

		
		if (jumpButtonPress)
		{
			if (!spacePressLock)
			{
				spacePress = true;
				spacePressLock = true;
			}
		}
		else
			spacePressLock = false;



		if (lockMovements)
			return;



		colBottom = new Vector2(transform.position.x, (transform.position.y + col.offset.y) - (col.size.y / 2f));


		
		////////////////////////////////////////////TEST : Detection du sol avant impact (extrapolation) pour eviter les sauts qui ne marchent pas

			RaycastHit2D ray = Physics2D.Raycast(colBottom, Vector2.down, 0.05f, mapRaycast);
			Debug.DrawRay(colBottom, Vector2.down * 0.05f);


		laserGrounded = ray.transform != null && ray.normal.y > 0.5f;

	
		if (!laserGrounded)
		{
			cannotJumpWait = new Tra_LoopPack(EndCannotJump, Time.fixedDeltaTime, GameStateController.Wait_MobClassic);
			inJump = true;
		}
		else if (cannotJumpWait != null && cannotJumpWait.isRunning)
			cannotJumpWait.StopAnim();


		/*

		if (inJump && !startJump && laserGrounded && rig.velocity.y < 1f)
		{
				SetGrounded();
		}

	*/
		////////////////////////////////////////////








		xInputs = 2f * (inputs.GetAxis("Right", keyMode.Press) - inputs.GetAxis("Left", keyMode.Press));

		if (Mathf.Abs(xInputs) < 0.3f)
			xInputs = 0;


		//verouillage du player sur les murs, pour plus de confort
		if (muralContact && muralTimer < muralTimeExit)
		{
			if ( Mathf.Abs(xInputs) > 0.1f &&   Mathf.Sign(xInputs) != Mathf.Sign(muralNormalX))
			{
				muralTimer += Time.fixedDeltaTime;
			}

			xInputs = muralNormalX;
		}
		





		if (jumpButtonPress)
		{
		
			OnJumpButtonPress(usedMaxSpeed, usedMaxJumpForce);





			//ajout de force tan que le player appuie sur espace. la fonction bizzare est utile pour la decroissance de la force apliquée en fonction du temps du saut
			if (actVel.y < Mathf.Pow(Mathf.Cos(jumpCor.percent * (Mathf.PI / 2f)), 0.1f) * usedMaxJumpForce && startJump)
			{
   

                if (!inMuralJump)
					rig.AddForce(Vector2.up * usedMaxJumpForce * 5f, ForceMode2D.Force);
				else
				{
					muralDir = new Vector2(ReMath.Clamp(muralDir.x + (xInputs * moveAccel * muralAirControl), -usedMaxSpeed, usedMaxSpeed), muralDir.y);
					rig.AddForce(muralDir * muralJumpForce * 5f * ReMath.MaxLim(runSpd, 1.5f), ForceMode2D.Force);
				}
			}

		}
		else if (startJump)
		{
			if (jumpCor != null && jumpCor.isRunning)
			{
				jumpCor.StopAnim();
			}
			endJump();


		}
		else
		{
			rig.sharedMaterial = null;
		}



		if(inJump && !startJump)
			col.sharedMaterial = playerPhysicBase;


		if (canMuralJump)
			col.sharedMaterial = wallFriction;


		if (actVel.y < 0 || (!startJump && inJump) )//coeficient de chute
			rig.velocity += Vector2.up * (fallMultiplier - 1)*Time.fixedDeltaTime*Physics2D.gravity.y;

		//print(rig.velocity);

		if (!inJump && Mathf.Abs(rig.velocity.y) > 0 && Mathf.Abs(rig.velocity.y) < 3f)
			rig.velocity = new Vector2(rig.velocity.x, 0);


		if (!inJump)
		{
			rig.drag = 8f;//plus de frotements quand tu es a terre

			if (Mathf.Abs(actVel.x) > 0 && xInputs == 0)//freinage express si tu laches les commandes
				rig.AddForce(-Vector2.right * actVel.x/2f, ForceMode2D.Impulse);


			if (Mathf.Abs(actVel.x) < usedMaxSpeed * runSpd)//deplacements generaux
				rig.AddForce(Vector2.right * xInputs * moveAccel, ForceMode2D.Impulse);
		}
		else if(!inMuralJump)
		{
			rig.drag = 2f;//moin de frotements quand tu voles

			if (Mathf.Abs(actVel.x) < usedMaxSpeed * airMaxSpeedCoef * runSpd)//deplacements generaux dans les airs
				rig.AddForce(Vector2.right * xInputs * airControl, ForceMode2D.Impulse);
		}
		//rig.AddForce(Vector2.up * toAddY, ForceMode2D.Impulse);





















	}



	Tra_LoopPack muralWait;
	Tra_LoopPack cannotJumpWait;
	bool canMuralJump;
	bool muralContact;
	bool forcingJump = false;




	private void OnJumpButtonPress(float usedMaxSpeed, float usedMaxJumpForce)
	{



		if (!inJump && spacePress)
		{
//			print("jump0");



			if (jumpBuf.ActiveAndPlaying())
				jumpBuf.StopAnim();

			JumpBufVoid();




			startJump = true;
			inJump = true;
			jumpCor = new Tra_LoopPack(endJump, jumpTime, GameStateController.Wait_MobClassic);
			rig.AddForce(Vector2.up * usedMaxJumpForce, ForceMode2D.Impulse);

			if (Mathf.Abs(rig.velocity.x) < 0.5f)
				Instantiate(particle_jump_up, transform.position, new Quaternion(), GameManager.TempInstances);
			else
				Instantiate(particle_jump_left, transform.position, new Quaternion(), GameManager.TempInstances).transform.localScale = new Vector3(Mathf.Sign(rig.velocity.x), 1f,1f);

		}
		else if (inJump)
		{
			rig.sharedMaterial = lowFriction;//si tu es en saut + espace, tu glisses partout

			if (startJump)
				col.sharedMaterial = lowFriction;





			if (canMuralJump)//si tu peux faire un saut mural de mario
			{
				if (spacePress && !inMuralJump)
				{

					if (jumpCor != null && jumpCor.isRunning)
						jumpCor.StopAnim();
					endJump();

					muralDir = new Vector2(-muralNormalX * usedMaxSpeed * 1.5f, usedMaxJumpForce) * 0.75f;

				//	print("jump mural");

					rig.AddForce(muralDir * muralJumpForce * ReMath.MaxLim(runSpd, 1.5f), ForceMode2D.Impulse);

					if (jumpBuf != null && jumpBuf.isRunning)
						jumpBuf.StopAnim();
					JumpBufVoid();


					startJump = true;
					inMuralJump = true;
					canMuralJump = false;
					muralContact = false;

					Instantiate(particle_jump_mural_left, transform.position, new Quaternion(), GameManager.TempInstances).transform.localScale = new Vector3(Mathf.Sign(rig.velocity.x), 1f, 1f);

					jumpCor = new Tra_LoopPack(EndWallJump, jumpTimeMural, GameStateController.Wait_MobClassic);

				}
			}
			else
			{



				if (spacePress && !forcingJump)//si ya pas de saut possible, un petit buffer pour essayer de sauter a larriver
				{



					if (jumpBuf != null && !jumpBuf.isRunning)
						jumpBuf = null;





					if (jumpBuf == null)
					{			
						forcingJump = true;
						jumpBuf = new Tra_LoopPack(JumpBufVoid, 0.1f, GameStateController.Wait_MobClassic);

					}
				}
			}

		}
	}




	void JumpBufVoid()
	{
		forcingJump = false;
	}


	void EndMuralCol()
	{
		muralContact = false;
		canMuralJump = false;
		muralContact = false;
	}



	void SetGrounded()
	{

		muralContact = false;

		if (startJump)
			return;

		inJump = false;
		canMuralJump = false;


		if (cannotJumpWait != null && cannotJumpWait.isRunning)
			cannotJumpWait.StopAnim();

	}


	private void CheckMural(Vector2 dir)
	{

		if (!inJump)
			return;

		RaycastHit2D ray = Physics2D.Raycast(transform.position + (Vector3)dir * col.size.x, dir, 0.5f, mapRaycast);
		Debug.DrawRay(transform.position + (Vector3)dir * col.size.x, dir * 0.5f);
		


		if (ray.transform != null)
		{

			muralNormalX = Mathf.Sign(dir.x);

			if (!muralContact)
				muralTimer = 0;

			muralContact = true;

			rig.velocity = new Vector2(rig.velocity.x, ReMath.MinLim(rig.velocity.y, -5f));


			if (muralWait != null && muralWait.isRunning)
				muralWait.StopAnim();
			muralWait = new Tra_LoopPack(EndMuralCol, 0.1f, GameStateController.Wait_MobClassic);


			if (startJump)
			{
				return;
			}


			canMuralJump = isGrabing == GrabMode.Nothing;

		}
	}

    Rigidbody2D grabObject;


    public void GrabCorpse(Rigidbody2D corpse)
    {
        if (corpse != null)
            isGrabing = GrabMode.Overhead;

        else
        {
            isGrabing = GrabMode.Nothing;
            if (grabObject != null)
                grabObject.AddForce(rig.velocity.normalized*200f , ForceMode2D.Impulse);
        }
        grabObject = corpse;

    }

    public void OnDie()
    {
        if (isGrabing != GrabMode.Nothing)
            GrabCorpse(null);
    }



    private void OnCollisionEnter2D(Collision2D collision)
	{
		OnCollisionStay2D(collision);
	}

	private void OnCollisionStay2D(Collision2D collision)
	{

	
		float val;

	
	

		for (int i = 0; i < collision.contacts.Length;i++ )
		Debug.DrawRay(collision.contacts[i].point - Vector2.up*0.1f, Vector3.right, Color.cyan);
		Debug.DrawRay(colBottom, Vector3.right, Color.red);


		for (int i = 0; i < collision.contacts.Length; i++)
		{

	

			if (collision.contacts[i].point.y - 0.1f <= colBottom.y )
			{
				SetGrounded();

				return;
			}
			else {
				val = collision.contacts[i].point.x - transform.position.x;

				//si le point de contact est a coté d'un mur. C'est a dire Si le point est au dessus du nombril du joueur et en dessous de sa tête.

				//Si la tête tape, on arête tout
				if (inJump && collision.contacts[i].point.y > (transform.position.y + col.offset.y + col.size.y / 2f) - 0.1f)
				{

					if (jumpCor != null && jumpCor.isRunning)
						jumpCor.StopAnim();
					endJump();

					return;
				}




				/*
				 Ancien systeme de detection des murs
				  
				if (inJump && Mathf.Abs(val) <= (col.size.x / 2f) &&
					collision.contacts[i].point.y >= (transform.position.y + col.offset.y) - 0.2f 
					 
					)
				{

					muralNormalX = ((val < 0) ? -1f : 1f);

					if (!muralContact)
						muralTimer = 0;

					muralContact = true;

					rig.velocity = new Vector2(rig.velocity.x, ReMath.MinLim(rig.velocity.y, -5f));

					if (muralWait != null && muralWait.isRunning)
						muralWait.StopAnim();
					muralWait = new Tra_LoopPack(EndMuralCol, 0.1f, GameStateController.Wait_MobClassic);



					if (startJump)
					{
						continue;
					}			


					canMuralJump = true;

					
				}*/


			}
		}
	}




	private void OnCollisionExit2D(Collision2D collision)
	{
		//print(rig.velocity.y);

		if (rig.velocity.y < 0)
		{
			cannotJumpWait = new Tra_LoopPack(EndCannotJump, Time.fixedDeltaTime, GameStateController.Wait_MobClassic);
		}
	}


	void EndCannotJump()
	{
		if (laserGrounded || muralContact)
			return;

		inJump = true;
//		Debug.LogWarning("vlurb");
	}






}
