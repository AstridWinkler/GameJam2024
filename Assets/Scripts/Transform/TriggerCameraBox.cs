using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class TriggerCameraBox : MonoBehaviour
{

	[SerializeField]
	Vector2 offset;

	[SerializeField]
	Vector2 size;

	[SerializeField]
	float cameraDistance = -10f;


#if UNITY_EDITOR
	void Update()
    {
		
		Vector2 c1 = new Vector2(size.x / 2f, size.y / 2f)+ offset + (Vector2)transform.position;
		Vector2 c2 = new Vector2(size.x / 2f, -size.y / 2f)+ offset + (Vector2)transform.position;
		Vector2 c3 = new Vector2(-size.x / 2f, -size.y / 2f)+ offset + (Vector2)transform.position;
		Vector2 c4 = new Vector2(-size.x / 2f, size.y / 2f)+ offset + (Vector2)transform.position;

		Debug.DrawLine(c1, c2, Color.green);
		Debug.DrawLine(c2, c3, Color.green);
		Debug.DrawLine(c3, c4, Color.green);
		Debug.DrawLine(c4, c1, Color.green);
	}
#endif


    private void OnTriggerEnter2D(Collider2D collision)
    {
        OnTriggerStay2D(collision);
    }

        private void OnTriggerStay2D(Collider2D collision)
	{

		if (collision.transform.tag == "Player")
			GameManager.CamController.SetLockBox(this);



	}


	private void OnTriggerExit2D(Collider2D collision)
	{

		//	if (!Application.isPlaying)
		//		return;



	}



	public Vector2 ClampVec(Vector2 vec)
	{
		return new Vector2(
			ReMath.Clamp(vec.x, offset.x + transform.position.x - (size.x / 2f), (size.x / 2f) + offset.x + transform.position.x),
			ReMath.Clamp(vec.y, offset.y + transform.position.y - (size.y / 2f), (size.y / 2f) + offset.y + transform.position.y));
	}




}

