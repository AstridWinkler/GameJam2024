using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : BasicManager
{

	TriggerCameraBox actFollowBox;

	public GameObject Target { get { return target; } }


	GameObject target;

	bool isNotPlayer;

	[SerializeField]
	Transform cameraBox;
	public Camera Camera { get => cam; private set => cam = value; }
	[NonSerialized]
	private Camera cam;

	public Animator Anim { get; private set; }


	[SerializeField]
	float moveSpeed = 10f;

	float rotX = 0;


	public void SetCamera(Transform cameraBox)
	{
		this.cameraBox = cameraBox;
		Camera = cameraBox.GetComponentInChildren<Camera>();
		Anim = cameraBox.GetComponentInChildren<Animator>();
	}

	public void SetLockBox(TriggerCameraBox box)
	{
		actFollowBox = box;
	}


	protected override void InitAction()
	{

	}




	private void FixedUpdate()
	{
		if (!IsStarted || cameraBox == null || GameManager.Gameplay.CamTargetPoint == null)
			return;


		if (target == null || (GameManager.Gameplay.CurrentPlayer != null && isNotPlayer))
		{
			target = GameManager.Gameplay.CurrentPlayer;
			isNotPlayer = false;


			if (target == null)
			{
				isNotPlayer = true;


				target = GameManager.Gameplay.CamTargetPoint.gameObject;

			}
		}
		else
		{
			Vector2 newCamPos = Vector2.zero;


			if (actFollowBox != null)
				newCamPos = Vector2.Lerp(cameraBox.transform.position, actFollowBox.ClampVec(target.transform.position), Time.fixedDeltaTime * moveSpeed);
			else
				newCamPos = Vector2.Lerp(cameraBox.transform.position, (Vector2)target.transform.position, Time.fixedDeltaTime * moveSpeed);



			//cameraBox.transform.position = newCamPos;
			cameraBox.transform.position = newCamPos;

			/*
            float newRot = Mathf.Lerp(-4f,4f, (1f + cameraBox.transform.position.y- target.transform.position.y)/5f  );
            rotX = Mathf.Lerp(rotX, newRot, Time.deltaTime * 3f); 
            cameraBox.transform.eulerAngles = new Vector3(rotX, 0,0);
			*/

		}

	}

	/*
	  Le DM de math de traitement du signal 

	float Calc(int n)
	{
		float result = 0;

		for (int i = 0; i <= n; i++)
		{
			result += (float)Mathf.Pow(4f / (Mathf.PI * (1+i*2)), 2);
		}

		return 1f - 0.5f * result;

	}


	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.F1))
		{
			for (int i = 0; i < 10; i++)
				print("IT " + (i + 1) + " = " + Calc(i));


		}
	}*/
}
