using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "New Animation", menuName = "Animation2D", order = 100)]
public class Animation2D : ScriptableObject {

	/*
	public Animation2D(SpriteList[] splst = null, float spd = 10.0f, int folder = -1){
		baseFps = spd;
		Skins = splst;
		folderId = folder;
	}
*/


	public DoubleSpriteList[] spritePacks;


}


[System.Serializable]
public class DoubleSpriteList{
	public string subName;
	public float duration = 1.0f;
	public SpriteList[] skins;
}