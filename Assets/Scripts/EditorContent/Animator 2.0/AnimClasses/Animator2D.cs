using System.Collections;
using System.Collections.Generic;
using UnityEngine;


	[System.Serializable]
	[CreateAssetMenu(fileName = "New animator", menuName = "Animator2D", order = 101)]
	public class Animator2D : ScriptableObject {

		/*public AnimatorClass2D(){
		ListAnims = new AnimatorBlock2D[0];
		vars = new varion[0];
	}*/


		public int genId = 0;

		public int listId = 0;

	public AnimatorLayer2D[] ListLayers = new AnimatorLayer2D[1]{new AnimatorLayer2D("Default")}; 

	public varion[] vars = new varion[1] {new varion(0, "idl")};
	}



