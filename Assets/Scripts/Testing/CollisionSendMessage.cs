using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionSendMessage : MonoBehaviour
{
	[SerializeField]
	GameObject obj;

	[SerializeField]
	string msg;


	private void OnCollisionEnter2D(Collision2D collision)
	{
		obj.SendMessage(msg);
	}

}
