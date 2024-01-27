using logiked.source.extentions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public static class SpriteRendererExtention
{

		public static Rect GetWorldRect(this SpriteRenderer rd)
		{

		Sprite sp = rd.sprite;

		if (sp == null)
			return new Rect();


		Vector2 size = rd.GetWorldSize();
		Vector2 pos = rd.GetWorldPosition(size);

       // ReMath.DrawWoldRect(new Rect(pos, size), Color.red, 5f);

		return new Rect(pos, size);
	}


	public static Vector2 GetWorldSize(this SpriteRenderer rd)
	{
		if (rd.sprite == null)
			return Vector2.zero;

		return new Vector2(rd.transform.localScale.x * rd.sprite.rect.width / rd.sprite.pixelsPerUnit, rd.transform.localScale.y * rd.sprite.rect.height / rd.sprite.pixelsPerUnit);
	}

	private static Vector2 GetWorldPosition(this SpriteRenderer rd, Vector2 size)
	{
		if (rd.sprite == null)
			return Vector2.zero;


		Transform tr = rd.transform;
		Sprite sp = rd.sprite;


		return new Vector2(tr.position.x - size.x * (sp.pivot.x / sp.rect.width), tr.position.y - size.y * (sp.pivot.y/sp.rect.height ) );

	}




	public static RectInt WorldToTextureRect(this SpriteRenderer rd, Rect worldRect)
	{
	
		Sprite sp = rd.sprite;

		Vector2 worldMin = worldRect.min;
		Vector2 worldMax = worldRect.max;

		//Debug.DrawRay(worldMin, Vector2.left+ Vector2.up, Color.magenta, 10f);

		Vector2Int endMin;
		Vector2Int endMax;


		if (sp == null)
			return new RectInt();


		Vector2 size = rd.GetWorldSize();
		Vector2 pos = rd.GetWorldPosition(size);
		Rect myRect = new Rect(pos, size);


		worldMin = new Vector2( ( (worldMin.x - myRect.x)  / size.x) * sp.rect.width, (( worldMin.y - myRect.y)  / size.y) * sp.rect.height) +sp.rect.position;
		worldMax = new Vector2(((worldMax.x - myRect.x) / size.x) * sp.rect.width, ((worldMax.y - myRect.y) / size.y) * sp.rect.height) + sp.rect.position;

		endMin = ReMath.ToVec2Int(worldMin);
		endMax = ReMath.ToVec2Int(worldMax);


		return new RectInt(endMin.x, endMin.y, Mathf.Abs(endMin.x - endMax.x), Mathf.Abs(endMin.y - endMax.y));
	}



	public class SimiliTexture
    {
		public Color[,] colors;
		public  int width;
		public  int height;

	}

	public static SimiliTexture GrabTextureByWorldRect (this SpriteRenderer rd, Rect worldRect)
	{

		RectInt spRect = rd.WorldToTextureRect(worldRect);

		SimiliTexture tex = new SimiliTexture();

		Color[,] end = new Color[spRect.width, spRect.height];
		tex.colors = end;
		tex.height = spRect.height;
		tex.width = spRect.width;


		//	Texture2D tex = new Texture2D(spRect.width, spRect.height, TextureFormat.ARGB32, false);	
		//	tex.filterMode = FilterMode.Point;


		//  Debug.Log(spRect);
		Color[] orig = rd.sprite.texture.GetPixels(spRect.x, spRect.y - spRect.height, spRect.width, spRect.height);


		int x, y;

		for (y = 0; y < spRect.height; y++)
		{
			for (x = 0; x < spRect.width; x++)
			{
				end[x, (spRect.height - 1) - y] = orig[ (x + (spRect.height - 1 -y) * spRect.width).Clamp(0, orig.Length-1)  ];
			}
		}



	//	System.IO.File.WriteAllBytes(@"I:\UNITYPROJECTS\MrKata_Die_It_Yourself\a.png", tex.EncodeToPNG());


		return tex;
	}





}
