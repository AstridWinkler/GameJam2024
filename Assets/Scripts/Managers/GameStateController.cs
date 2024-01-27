using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameStateType { animation, mob, control, loading, physics, pauseMenu, gameControl };

public class GameStateController : MonoBehaviour {

	public enum GameState
	{
		MainMenu, InGame
	}


	public GameState ActState { get { return actState; } }
	private GameState actState = GameState.InGame;

	public bool InPauseMenu { get { return inPauseMenu; } }
	private bool inPauseMenu = false;


	public bool GameOnline { get { return gameOnline; } }
	private bool gameOnline = false;

	public bool PhysicsPaused { get { return physicsPaused; } }
	private bool physicsPaused = false;



	public bool ControlsPaused { get { return controlsPaused; } }
	private bool controlsPaused = false;

	public bool GameControlPaused { get { return gameControlPaused; } }
	private bool gameControlPaused = false;


	public bool MobsPaused { get { return controlsPaused; } }
	private bool mobsPaused = false;

	public bool AnimationsPaused { get { return animationsPaused; } }
	private bool animationsPaused = false;

	public bool PlayerSpawned { get { return playerSpawned; } }
	private bool playerSpawned = false;

	public bool InLoading { get { return inLoading; } }
	private bool inLoading = false;

	public bool MapGenerated { get { return mapGenerated; } }
	private bool mapGenerated = false;


	public static GameStateType[] Wait_MobClassic
	{
		get
		{
			if(wait_MobClassic == null)
				wait_MobClassic = new GameStateType[] { GameStateType.loading, GameStateType.pauseMenu, GameStateType.physics, GameStateType.mob };
			return wait_MobClassic;
		}
	}
	static GameStateType[] wait_MobClassic;


	public static GameStateType[] Wait_PropsClassic
	{
		get
		{
			if (wait_PropsClassic == null)
				wait_PropsClassic = new GameStateType[] { GameStateType.pauseMenu, GameStateType.physics, GameStateType.loading };

			return wait_PropsClassic;
		}
	}
	static GameStateType[] wait_PropsClassic;


	//RoomGenerator rg;


	public void InitStates () {
		mapGenerated = true;
		playerSpawned = true;
	//	inputs = GameManager.Inputs.GetController();

		physSaves = new bool[0];
		rigs = new Rigidbody2D[0];
	//	rg = GameObject.FindObjectOfType<RoomGenerator>();

		//waitMobClassic = new GameStateType[] { GameStateType.loading, GameStateType.pauseMenu, GameStateType.physics, GameStateType.mob};


	}


	void Update () {




		switch (actState)
		{

			case GameState.InGame:

				GameInputLockCheck();


				if (playerSpawned && mapGenerated)
				{
					/*
					if (inputs.GetKey("Inventory", keyMode.DownPress))
					{
						inPauseMenu = !inPauseMenu;
					}*/

				}


				controlsPaused = false || inPauseMenu;
				mobsPaused = false || inPauseMenu;
				animationsPaused = false || inPauseMenu;
				//	if (inPauseMenu)
				//		Time.timeScale = 0;
				//	else Time.timeScale = 1f;

				break;
		}

		PausePhysicsCheck ();
		
	//	IA_AlphaEntity.UpdateInfs ();
	}


	void GameInputLockCheck()
	{
		//gameControlPaused = rg.PlayerIsFalling || inPauseMenu || inLoading;
	}


		bool[] physSaves;
	Rigidbody2D[] rigs;

	void PausePhysicsCheck(){
		
		bool calc = inPauseMenu && !gameOnline;
		 
		if (physicsPaused == calc)
			return;

	
		if (physicsPaused == false) {
			rigs = GameObject.FindObjectsOfType<Rigidbody2D> ();
			physSaves = new bool[rigs.Length];
		}

		for (int j = 0; j < physSaves.Length; j++) {
		
			if (physicsPaused == false) {
				physSaves [j] = rigs [j].simulated;
				rigs [j].simulated = false;
			} else {
				if(rigs [j] != null)
				rigs [j].simulated = physSaves [j];
			}
		}

		physicsPaused = calc;


	}





}
