using System;
using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;



[System.Serializable]
public class TransitionNode : Transition{
	
	public TransitionNode(int id, int endId, AnimEnd _atEnd = AnimEnd.nothing , condition[] _conditions = null):base(id,endId, _atEnd, _conditions ) {

	}
	public TransitionNode(Transition tr):base(tr.animId,tr.toAnimId, tr.endAction, tr.conditions ) {

	}


	public TransitionNode Copy(){
		return (TransitionNode) this.MemberwiseClone();
	}

	public Node Exit;

}







//[System.Serializable]
public class Node
{


	public Rect rect;

	//[NonSerialized]
	public GUIStyle style;
	//[NonSerialized]
	public GUIStyle selStyle;

	public bool loopingAnim;
	public float speedCoef;

	public bool isDragged;
	public bool isSelected;

	//[NonSerialized]
	public List<Node> InputList;

	//[NonSerialized]
	public List<TransitionNode> Outputs;

	bool ToRemove = false;
	public bool setTransition;
	public bool waitDef;
	public Animation2D anim;
	public int listId;
	public string localName;


	public Node(Vector2 position, float width, float height, int _listId)
	{
		rect = new Rect(position.x, position.y, width, height);

		listId = _listId;

		speedCoef = 1.0f;
		loopingAnim = true;


		Outputs = new List<TransitionNode> ();
		InputList = new List<Node> ();

	}


	public void Drag(Vector2 delta)
	{
		rect.position += delta;
	}

	public void Draw( GUIStyle style)
	{
		GUI.Box(rect, localName, style);
		 
	}

	public void DrawConnections()
	{		
		Vector2 pos;


		for (int h = 0; h < Outputs.Count; h++) {
			pos = new Vector2 (10f * Math.Sign (rect.center.y - Outputs [h].Exit.rect.center.y), 10f * Math.Sign (rect.center.x - Outputs [h].Exit.rect.center.x));
			Handles.color = Animator2DWindow.ColorBySeed (Outputs[h].Exit.listId , 6);
			Handles.DrawLine ((Vector3)(rect.center), (Vector3)Outputs [h].Exit.rect.center + (Vector3)pos);
			DrawTriangle ( ReMath.Center(rect.center,  Outputs [h].Exit.rect.center + pos ), ReMath.VecAngle(ReMath.GetVec(rect.center, Outputs [h].Exit.rect.center), Vector2.down) );

		}
	}





	public bool ProcessEvents(Event e, ref Node selected)
	{
		bool gc = false;
		localName = (string)((anim != null)?anim.name: "null anim" );

		switch (e.type)
		{
		case EventType.MouseDown:


	

			if (e.button == 0) {				
				isSelected = false;
				setTransition = false;

			


				if (rect.Contains (e.mousePosition)) {
					isDragged = true;
				


					if (selected == null) {

						isSelected = true;
						selected = this;
					}



				}	

			

				gc = true;
			}




			if (e.button == 1) {
				
				isSelected = false;
				setTransition = false;

				if (!isDragged && rect.Contains (e.mousePosition)) {		


					gc  = true;


					if (selected == null) {
						isSelected = true;
						selected = this;
					}

					GenericMenu genericMenu = new GenericMenu ();
					genericMenu.AddItem (new GUIContent ("Add Transition"), false, () => StartTransition ()); 
					genericMenu.AddItem (new GUIContent ("First node"), false, () => SetDefault ()); 
					genericMenu.AddItem (new GUIContent ("Remove (del)"), false, () => DelNode ()); 



					genericMenu.ShowAsContext ();

			

				}
			}

			break;


		case EventType.MouseUp:
			isDragged = false;
			break;

		case EventType.MouseDrag:
			if (e.button == 0 && isDragged) {
				Drag (e.delta);
				e.Use ();
			
				gc = true;
			}

			break;


		case EventType.KeyDown:
			if (e.keyCode == KeyCode.Delete && isSelected)
			{		
				DelNode ();

				gc = true;
			}
			break;


		}

		return gc;
	}

	void SetDefault(){
		 waitDef = true;
	}

	 void DelNode(){
		
		for (int u = 0; u < InputList.Count; u++) {
			InputList [u].PrepareRemove (this);
		}
		GUI.changed = true;
		ToRemove = true;
	}


	public void PrepareRemove(Node nd){
		for (int u = 0; u < Outputs.Count; u++) {

			if (Outputs [u].Exit == nd) {
				Outputs.RemoveAt (u);
				return;
			}
		}
	}



	public bool CheckRemove(){
		return ToRemove;
	}


	public void StartTransition(){
		setTransition = true;

	}

	public void AddTransitionIn(Node point){
		InputList.Add (point);
	}

	public void Unselect(){
		isSelected = false;
		GUI.changed = true;
	}

	public void AddTransitionOut(Node point){

		setTransition = false;
		TransitionNode tr = new TransitionNode (listId, point.listId);
	
		tr.Exit = point;
		point.AddTransitionIn (this);
		Outputs.Add (tr);

	}


	public void DrawTriangle(Vector3 pos, float rot){



		Handles.DrawLines (new Vector3[4]{
			pos + (Vector3)ReMath.Rotate2D( new Vector2(0f, 10f), rot, Vector2.zero),
			pos + (Vector3)ReMath.Rotate2D( new Vector2(-10f,-10f), rot, Vector2.zero),
			pos + (Vector3)ReMath.Rotate2D( new Vector2(0f, 10f), rot, Vector2.zero),
			pos + (Vector3)ReMath.Rotate2D( new Vector2(10f,-10f), rot, Vector2.zero)
		});


	}



	public AnimatorBlock2D ToAnimatorBlock( ){
		AnimatorBlock2D block = new AnimatorBlock2D ();
		block.visualPos = rect.center;
		block.looping = loopingAnim;
		block.coefSpeed = speedCoef;
		block.animation = anim;
		block.listId = listId;


		if(Outputs != null){
			block.transitions = new Transition[Outputs.Count];

			for (int h = 0; h < Outputs.Count; h++)
				block.transitions [h] = Outputs [h];

			

		}


		return block;
	}



	public Node Clone(){
		Node n = (Node)this.MemberwiseClone ();
		int j;


		if(InputList != null){
			n.InputList = new List<Node> (0);
			for (j = 0; j < InputList.Count; j++)
				n.InputList.Add (InputList[j]);
				}
		if(Outputs != null){
			n.Outputs = new List<TransitionNode> (0);
			for (j = 0; j < Outputs.Count; j++)
				n.Outputs.Add (Outputs[j].Copy());
		}


		return n;
	}





	public void ResetupTransition(int margin, List<Node> lst){
		int j;
		Debug.Log ("id block :" + listId.ToString ()+ " margin " + margin.ToString());
			
		if (InputList != null) {
			for (j = 0; j < InputList.Count; j++) {
				
				Debug.Log (InputList [j].anim.name + ": input avec id " + InputList [j].listId.ToString());

				InputList [j] = lst [InputList [j].listId + margin];		
		
			}
		}


		if (Outputs != null) {
			for (j = 0; j < Outputs.Count; j++) {
				

				Outputs [j].toAnimId = Outputs [j].Exit.listId+margin;
				Outputs [j].animId = listId;
				Outputs [j].Exit = lst [Outputs [j].Exit.listId + margin];		
			}
		}


	}


}