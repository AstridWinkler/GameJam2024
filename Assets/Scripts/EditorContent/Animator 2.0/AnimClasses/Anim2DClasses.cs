using System.Collections;
using System.Collections.Generic;
using UnityEngine;








[System.Serializable]
public class varion {
	public varion(int _val, string _name){
		name = _name;
		val = _val;
	}


	public varion Clone(){
		return new varion (val, name);
	}
	
	public int val;
	public string name;
}


[SerializeField]
public enum AnimEnd{End_AND, End_OR, nothing} 


[SerializeField]
public enum CondType{Equal, Greater, Lesser, NotEqual} 



[System.Serializable]
public class condition{
	public CondType type = CondType.Equal;
	public int var1 = 0;
	public int value = 0;
}



[System.Serializable]
public class Transition{


	public Transition(int id, int endId, AnimEnd _atEnd = AnimEnd.nothing, condition[] _conditions = null){
		animId = id;
		toAnimId = endId;
		endAction = _atEnd;

		if (_conditions == null)
			conditions = new condition[0];
			else
		conditions = _conditions;
	}



	public int animId;
	public int toAnimId;
	public AnimEnd endAction;

	public condition[] conditions;
}


[System.Serializable]
public class AnimatorLayer2D {
	
	public AnimatorLayer2D(string _name){
		name = _name;
	}

	/*
	public AnimatorLayer2D Clone(){
		int h = 0;
		AnimatorLayer2D ll = new AnimatorLayer2D (name + "2");
		ll.defaultAnimId = defaultAnimId;
		ll.ListAnims = new AnimatorBlock2D[ListAnims.Length];
			for (h = 0; h < ll.ListAnims.Length; h++) {
			ll.ListAnims [h] = ListAnims [h].Clone ();
			}
	}*/

	public AnimatorLayer2D Copy()
	{
		return (AnimatorLayer2D) this.MemberwiseClone();
	}


	public string name;
	public int defaultAnimId = 0;

	public AnimatorBlock2D[] ListAnims = new AnimatorBlock2D[0]; 


}



[System.Serializable]
public class AnimatorBlock2D {



	//public float CoefSpeed { get{ return coefSpeed;} set{ coefSpeed = value;}}
	/*
	public AnimatorBlock2D clone(){
		AnimatorBlock2D b = new AnimatorBlock2D ();

		b.coefSpeed = coefSpeed;
		b.visualPos = visualPos;
		b.animation = animation;
		b.listId = listId;
		b.looping = looping;
		b.transitions = new Transition[transitions.Length];

		for(int j = 0; j < transitions.Length; j++){
			b.transitions [j] = transitions [j].Clone;

			}
	}*/
	

	public float coefSpeed = 1.0f; 


	public Vector2 visualPos = Vector2.zero;


	public Animation2D animation;

	public int listId;
	
	public bool looping = true;

	public Transition [] transitions;

}

[System.Serializable]
public class SpriteList{
	
	public Sprite[] sprites = new Sprite[0];
}














