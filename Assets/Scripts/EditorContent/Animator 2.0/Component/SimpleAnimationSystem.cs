using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SimpleAnimationSystem : MonoBehaviour {



	public int Equivalent { get { return equivalent; } set { equivalent = value; } }
	[SerializeField]
	protected int equivalent = 0;

	public int Skin { get { return skin; } set { skin = value; } }
	[SerializeField]
	protected int skin;


	[SerializeField]
	protected bool autoStart = true;
//	public bool AutoStart{ get { return autoStart; } set { autoStart = value; } }


	[SerializeField]
	protected bool started = false;

	public float Speed { get { return speed; } set { OnSpeedChanged(); speed = value; } }
	[SerializeField]
	protected float speed = 1f;



	protected void Start(){
		if (autoStart && !started)
			LaunchAnim ();
	}

	public void ForceStart(){
		LaunchAnim ();
	}


	protected  virtual void OnSpeedChanged()
	{

	}

	protected abstract void LaunchAnim ();


}
