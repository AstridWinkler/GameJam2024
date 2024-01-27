using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BasicManager : MonoBehaviour
{
	public bool IsStarted { get {return  started; }  }
	bool started = false;


public void InitManager()
	{
		if (started)
			return;

		started = true;
		InitAction();
	}

	protected abstract void InitAction();

}
