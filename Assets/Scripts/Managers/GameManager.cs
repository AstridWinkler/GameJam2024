using System.Collections;
using System.Collections.Generic;
using UnityEngine;




[ExecuteInEditMode]
public class GameManager : MonoBehaviour
{
    private const string prefabPath = "Prefabs/Gameplay/GAME_MANAGER";



    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void OnAfterSceneLoadRuntimeMethod()
    {
        Debug.Log("No gamemanager found. Dont worry, we are creating it for your !");
        if (GameObject.FindObjectOfType<GameManager>() == null)
            GameObject.Instantiate(UnityEngine.Resources.Load<GameObject>(prefabPath));
    }



    byte[] bytes;

static GameManager instance;


	public static GameStateController StateGame { get { return stateGame; } }
	static GameStateController stateGame;

	public static TransformAnim TransfAnim { get { return transfAnim; } }
	static TransformAnim transfAnim;

	public static GameplayManager Gameplay { get { return gameplay; } }
	static GameplayManager gameplay;


	public static CameraController CamController { get { return camController; } }
	static CameraController camController;

	public static ResourcesHelperManager Resources { get { return resources; } }
	static ResourcesHelperManager resources;


	public static InputManager Inputs { get { return inputs; } }
	static InputManager inputs;

	public static Transform TempInstances { get { return tempInstances; } }
	static Transform tempInstances;




	// Start is called before the first frame update
	void Awake()
    {

#if UNITY_EDITOR

 
            if (instance != null && Application.isPlaying)
                Destroy(gameObject);
            else
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
                PreInit();
            }
        

#else

        if (instance != null)
			Destroy(gameObject);
		else
		{
			instance = this;
			DontDestroyOnLoad(gameObject);
			PreInit();
		}
#endif




    }


    void PreInit()
	{
        Application.targetFrameRate = 90;

		tempInstances = new GameObject("TempInstances").transform;
        DontDestroyOnLoad(tempInstances.gameObject);

        stateGame = gameObject.GetComponentInChildren<GameStateController>();
		transfAnim = gameObject.GetComponentInChildren<TransformAnim>();
		stateGame = gameObject.GetComponentInChildren<GameStateController>();
		camController = gameObject.GetComponentInChildren<CameraController>();
		inputs = gameObject.GetComponentInChildren<InputManager>();
		gameplay = gameObject.GetComponentInChildren<GameplayManager>();
		resources = gameObject.GetComponentInChildren<ResourcesHelperManager>();

#if UNITY_EDITOR
        if (!Application.isPlaying)
            return;
#endif


        inputs.InitInputs();
        camController.InitManager();

        stateGame.InitStates();
        gameplay.InitManager();
		resources.InitManager();
        transfAnim.InitManager();

    }




	// Update is called once per frame
	void Update()
    {
        
    }






}
