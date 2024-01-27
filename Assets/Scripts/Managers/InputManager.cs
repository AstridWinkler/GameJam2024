using System.Collections;
using System.Collections.Generic;
using UnityEngine;








[System.Serializable]
public class InputManager : MonoBehaviour
{
	[SerializeField]
	 GameObject prefRender;


	public InputList DefaultInputs { get { return defaultInputs; } }
	[SerializeField]
	private InputList defaultInputs = null;



	SimpleInput[] inputList = new SimpleInput[0];
	public int ControllerType { get { return controllerType; } }
	int controllerType = 0;
	List<InputController> ctrl = new List<InputController>();

	bool started = false;

	public void InitInputs()
	{
		started = true;
		SetInputs(defaultInputs);
		AddController(new InputController(0));
	}

	public void AddController(InputController controller)
	{
		if (ctrl == null)
			ctrl = new List<InputController>();

		if (ctrl.Count == 0)
			ctrl.Add(controller);
	}

	public InputController GetController()
	{
		if (ctrl.Count > 0)
			return ctrl[0];

		return null;
	}


		public void SetInputs(InputList inputsInfo)
	{
		inputList = inputsInfo.keyboardLst;

		InputSprite.pref = prefRender;


		for (int i = 0; i < inputList.Length; i++)
		{
			inputList[i].Start();
		}

	}




	public SimpleInput GetInput(string name)
	{

		for (int i = 0; i < inputList.Length; i++)
		{
			if (inputList[i].Name == name)
				return inputList[i];
		}

		Debug.LogError("L'input '" + name + "' n'a pas été trouvé !");
		return null;
	}
	public string GetInputCode(string name)
	{

		for (int i = 0; i < inputList.Length; i++)
		{
			if (inputList[i].Name == name)
				return inputList[i].GetFirstKeyName(controllerType);
		}

		Debug.LogError("L'input '" + name + "' n'a pas été trouvé !");
		return null;
	}


	private void Start()
	{

		InvokeRepeating("CheckController", 0f, 2f);
	}

		private void CheckController()
	{
		//Get Joystick Names
		string[] temp = Input.GetJoystickNames();


		//Check whether array contains anything
		if (temp.Length > 0)
		{
			//Iterate over every element
			for (int i = 0; i < temp.Length; ++i)
			{
				//Check if the string is empty or not
				if (!string.IsNullOrEmpty(temp[i]))
				{
					//Not empty, controller temp[i] is connected
					if (controllerType != 1)
					{
						Debug.Log("Controller " + i + " is connected using: " + temp[i]);
						controllerType = 1;
						GetController().SetController(controllerType);
					}
					return;
				}

			}


			//If it is empty, controller i is disconnected
			//where i indicates the controller number
			if (controllerType != 0)
			{
				Debug.Log("Controllers are disconnected.");
				controllerType = 0;
				GetController().SetController(controllerType);
			}

		}

		//	SetController
	}



}










[System.Serializable]
public class InputSprite{
	public Sprite sprite;
	public static GameObject pref;
	public bool printText;
	GameObject bg;
	public string text;


	public GameObject SpawnRenderer(Transform parent, Vector3 pos){
		if (bg != null)
			DestroyRend ();


		bg = GameObject.Instantiate (pref, pos, new Quaternion(), parent);

		if(sprite != null)
		bg.GetComponent<SpriteRenderer> ().sprite = sprite;

		if (printText)
			bg.GetComponentInChildren<TextMesh>().text = text;
		else bg.GetComponentInChildren<TextMesh>().gameObject.SetActive(false);


		return bg;
	}

	public void DestroyRend(){
		if (bg != null)
			bg.AddComponent<SimpleDestroyAfter>();
		
	}

}

[System.Serializable]
public class KeyCodeOrAxis
{

	

	public object KeyState(keyMode state, int contr, bool wantABool)
	{
		bool pos = false;


		if (key != KeyCode.None)
		{

			switch (state)
			{
				case keyMode.DownPress:
					pos = (Input.GetKeyDown(key));
					break;

				case keyMode.UpPress:
					pos = (Input.GetKeyUp(key));
					break;

				case keyMode.Press:
					pos = (Input.GetKey(key));
					break;

			}

			if(wantABool)
			return pos;

			return (float)System.Convert.ToDecimal(pos) ;

		}
		else if (!string.IsNullOrEmpty(axis))
		{
			//Debug.Log(axis+ ":"+Input.GetAxis(axis).ToString());
			float axeVal = Input.GetAxis(axis);






			switch (axisMode)
			{

				case AxisMode.Both:
					pos = axeVal != 0;
					break;

				case AxisMode.OnNegative:
					pos = axeVal < 0;
					break;

				case AxisMode.OnPositive:
					pos = axeVal > 0;
					break;
			}


			switch (state)
			{
				case keyMode.DownPress:
					if ((pos && axisSavePos) || (axisSavePos && !pos))
					{
						axisSavePos = pos;

						if(wantABool)
						return false;
						return 0f;
					}
					break;

				case keyMode.UpPress:
					if ((pos && axisSavePos) || (!axisSavePos && pos))
					{
						axisSavePos = pos;
						if (wantABool)
						return false;
						return 0f;
					}
					break;
			}


			axisSavePos = pos;




			if (wantABool)
				return axisSavePos;

			if (!pos)
				axeVal = 0;
			return axeVal * ((invert) ?-1f:1f);


		}
		if (wantABool)
			return false;
		return 0f;
	}


	public string GetInputCode()
	{
		if (key == KeyCode.None)
			return axis;
		return key.ToString() ;
	}

	public enum AxisMode { OnPositive, OnNegative, Both}

	public KeyCode Key	{get { return key; }	}
	[SerializeField]
	KeyCode key = KeyCode.None;

	public string Axis { get { return axis; } }
	[SerializeField]
	string axis;
	[SerializeField]
	AxisMode axisMode;

	bool axisSavePos = false;
	[SerializeField]
	bool invert;
}


[System.Serializable]
public class SimpleInput{
					
	public string Name {
		get {
			return name;
		}
		set {
			name = value;
		}
	}
	[SerializeField]
	private string name;

	public void Start(){
		if(img == null || img.Length == 0)
		img = new InputSprite[3];
		img[0] = new InputSprite();
		img[0].printText = true;

		if (key1[0].Key != KeyCode.None)
			img[0].text = key1[0].Key.ToString();
		else
			img[0].text = key1[0].Axis.ToString();


		keyLst = new KeyCodeOrAxis[][] { key1 , keyXbox };

	}
	KeyCodeOrAxis[][] keyLst;

	public string GetFirstKeyName(int contr)
	{
		return keyLst[contr][0].GetInputCode();
	}


	[SerializeField]
	KeyCodeOrAxis[] key1 = { new KeyCodeOrAxis() };
	
	[SerializeField]
	KeyCodeOrAxis[] keyXbox = { new KeyCodeOrAxis() };

	public InputSprite[] Img { get { return img; } }
	[SerializeField]
	 InputSprite[] img;


	public object KeyState(keyMode state, int contr){
		KeyCodeOrAxis[] ax = keyLst[contr];
		bool val = false;

		for (int i = 0; i < ax.Length; i++)
		{
			val = (bool)ax[i].KeyState(state, contr, true);
			if (val)
				break;
		}
		return val;
	}

	public object KeyStateAxis(keyMode state, int contr)
	{
		KeyCodeOrAxis[] ax = keyLst[contr];
		float val = 0;

		for (int i = 0; i < ax.Length; i++)
		{
			val += (float)ax[i].KeyState(state, contr, false);
		}

		return val;
	}
}

public enum keyMode {Press, DownPress, UpPress, Null};


[System.Serializable]
public  class InputController{
	IDictionary<string, SimpleInput> dikey = new Dictionary < string, SimpleInput>();
	//SimpleInput[] lst = new SimpleInput[8];
	//int cnt = 0;
	int contr;
	int idPlayer;

	public InputController(int idPlayer_ = 0)
	{
		idPlayer = idPlayer_;


		GameManager.Inputs.AddController(this);
		Debug.Log("New game input controller..");
	}


	public void SetController(int controller){
		contr = controller;
	}



	public GameObject CreateTextureInput(string name, Transform parent, Vector3 pos){

		SimpleInput i = GetInput (name);


		if (i != null && i.Img[contr] != null)		
			return i.Img[contr].SpawnRenderer(parent, pos);
		
		
		return null;

	}


	public SimpleInput GetInput(string name){
		/*
		for (int j = 0; j < cnt; j++) {
			if (lst [j] == null)
				continue;
			if (lst [j].Name != name) 
				continue;
			return lst [j];
		}
		*/
				
		if (!dikey.ContainsKey(name))
			dikey.Add(name, GameManager.Inputs.GetInput (name));
		
		return dikey[name];
	}


	public bool GetKey(string name, keyMode mode){
		if (mode == keyMode.Null)
			return false;

		SimpleInput i = GetInput (name);

		if (i != null)
			return (bool)i.KeyState (mode, contr);
		else return false;
		}

	public float GetAxis(string name, keyMode mode)
	{
		if (mode == keyMode.Null)
			return 0;

		SimpleInput i = GetInput(name);

		if (i != null)
		{
			//Debug.Log("ret:" + i.KeyStateAxis(mode, contr).ToString());
			return (float)i.KeyStateAxis(mode, contr);
		}
		else return 0f;
	}

}






