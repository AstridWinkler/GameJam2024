using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSprite : MonoBehaviour
{

	

	[SerializeField]
	Sprite[] sprt;

	[SerializeField]
	bool randomFlipX;
	[SerializeField]
	bool randomFlipY;



	void Awake()
    {
		SpriteRenderer sp = GetComponent<SpriteRenderer>();

		if (sp == null) return;

		sp.sprite = ReMath.RandomItem<Sprite>(sprt);

		sp.flipX = randomFlipX && Random.Range(0, 2) == 1;
		sp.flipY = randomFlipY && Random.Range(0, 2) == 1;


	}
}
