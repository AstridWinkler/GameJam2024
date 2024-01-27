
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;


public class NodeStyles{
	public GUIStyle nodeStyle;
	public GUIStyle nodeStyleSelected;
	public GUIStyle nodeStyleFirst;
	public GUIStyle nodeStyleSelectedFirst;
//	public GUIStyle textBold;
}





public class Animator2DWindow : EditorWindow {

	private  Animator2D ActAnim;
	private AnimatorLayer2D ActLayer;
	private int ActLayerId;
	private int defaultNodeId;

	private List<Animator2D> ListAnimator;
	private List<AnimatorLayer2D> ListLayers;

	private List<Node> nodes;
	private List<varion> variableAnim;


	private Node selectedNode, newTransition;

	private bool gameState;
	private bool needSave;


	private Vector2 drag;

	private Vector2 offset;
	private Vector2 mousePosition;

	private Rect actWindowInspector;
	private Rect actWindowpector2;
	private Rect actAnimationInspector;
	private Rect actAnimationHelper;

	private bool mouseInInspector;


	private  Vector2 nodeSize = new Vector2(200, 50); 

	private bool gui_Changed;

	private string[] variableList = new string[0];


	public NodeStyles	nodeStyle;



	public enum InspectorState
	{
		AnimatorSelect, NewAnimator, EditAnimator
	} ;

	private InspectorState stateMainWind = InspectorState.AnimatorSelect;

	public enum InspectAnimationWind
	{
		none, open
	} ;
	private InspectAnimationWind stateAnimationWind;




	public enum EditorAnimWidow
	{
		none, NewVariable, NewLayer
	} ;
	private EditorAnimWidow stateEditorAnimator = EditorAnimWidow.none;



	private string[] fields = new string[5];
	private int[] fieldsInt = new int[5];

	int fieldId;

	//private Rect leftWindowRect = 

	public static Color ColorBySeed(int seed, float coef = 4f){
		Random.InitState (seed);

		return new Color (1f-(Random.Range (0.0f, 0.1f)*coef), 1f-(Random.Range (0.0f, 0.1f)*coef), 1f-(Random.Range (0.0f, 0.1f)*coef));

	}
	static EditorWindow wd;


	[MenuItem("Window/Sprite Animator")]
	private static void OpenWindow()
	{
		Animator2DWindow window = GetWindow<Animator2DWindow>();
		window.titleContent = new GUIContent("Sprite Animator");
	}

	void Update(){
		EditorWindow wd;
		if (needSave && stateMainWind == InspectorState.EditAnimator) {
			wd = EditorWindow.focusedWindow;
			if (wd == null || wd != null && wd.titleContent.text != "Sprite Animator") {
				if(!Application.isPlaying)
				SaveActualAnimator ();
				needSave = false;
				return;
			}
			/*if (EditorWindow.focusedWindow != window) {
				SaveActualAnimator ();
				Log (EditorWindow.focusedWindow.name);
				needSave = false;
				return;
			}*/

		}
	}



	private void OnEnable()
	{
		nodeStyle = new NodeStyles ();

		nodeStyle.nodeStyle = new GUIStyle();
		nodeStyle.nodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1.png") as Texture2D;
		nodeStyle.nodeStyle.border = new RectOffset(24, 24, 24, 24);
		nodeStyle.nodeStyle.normal.textColor = Color.white;
		nodeStyle.nodeStyle.alignment = TextAnchor.MiddleCenter;
		nodeStyle.nodeStyle.fontSize = 14;

		nodeStyle.nodeStyleSelected = new GUIStyle();
		nodeStyle.nodeStyleSelected.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1 on.png") as Texture2D;
		nodeStyle.nodeStyleSelected.border = new RectOffset(12, 12, 12, 12);
		nodeStyle.nodeStyleSelected.normal.textColor = Color.white;
		nodeStyle.nodeStyleSelected.alignment = TextAnchor.MiddleCenter;
		nodeStyle.nodeStyleSelected.fontSize = 14;


		nodeStyle.nodeStyleFirst = new GUIStyle();
		nodeStyle.nodeStyleFirst.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node2.png") as Texture2D;
		nodeStyle.nodeStyleFirst.border = new RectOffset(12, 12, 12, 12);
		nodeStyle.nodeStyleFirst.normal.textColor = Color.white;
		nodeStyle.nodeStyleFirst.alignment = TextAnchor.MiddleCenter;
		nodeStyle.nodeStyleFirst.fontSize = 14;


		nodeStyle.nodeStyleSelectedFirst = new GUIStyle();
		nodeStyle.nodeStyleSelectedFirst.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node2 on.png") as Texture2D;
		nodeStyle.nodeStyleSelectedFirst.border = new RectOffset(12, 12, 12, 12);
		nodeStyle.nodeStyleSelectedFirst.normal.textColor = Color.white;
		nodeStyle.nodeStyleSelectedFirst.alignment = TextAnchor.MiddleCenter;
		nodeStyle.nodeStyleSelectedFirst.fontSize = 14;

//		nodeStyle.textBold = new GUIStyle ();
///		nodeStyle.textBold.fontStyle = FontStyle.Bold;


	}


	public void Refresh(){
		gui_Changed = true;
	}



	private void ResetFields(){
		fieldId = 0;
	}

	private void  ClearFields(){
		ResetFields ();
		for (int g = 0; g < fields.Length; g++) {
			fields [g] = "";
			fieldsInt [g] = 0;
		}
	}


	 bool first;

	private void LoadAnim(){
		ListAnimator = new List<Animator2D> (0);
        //ListAnimator = GameManager.Resources.GetAnimatorList ();
        //ListAnimator = (Animator2D[])AssetDatabase.LoadAllAssetsAtPath(Application.dataPath+"Animations").OfType<Animator2D>().ToArray();
        //Object[] slt = AssetDatabase.LoadAllAssetsAtPath("Assets/Animation/Animator/default.asset");


        string[] files = Directory.GetFiles(Application.dataPath, "*.asset", SearchOption.AllDirectories);
        


        string assetPath;
		Animator2D aa;

		foreach (string f in files) {
			assetPath = "Assets" + f.Replace (Application.dataPath, "").Replace ('\\', '/');

			aa = AssetDatabase.LoadAssetAtPath<Animator2D> (assetPath);

			if(aa != null && aa is Animator2D)
				ListAnimator.Add ((Animator2D)aa);



		}



	


			//ListAnimator = GameManager.Resources.GetAnimatorList ();
			//ListAnimator = (Animator2D[])AssetDatabase.LoadAllAssetsAtPath(Application.dataPath+"Animations").OfType<Animator2D>().ToArray();
		
	}


    Tra_LoopPack searchFileLoop;
    


 	 private void OnGUI()
	{
		gui_Changed = false;

		 if (stateMainWind == InspectorState.EditAnimator && nodes == null && ListLayers != null)
			if(ListLayers.Count > 0 )
				 LoadLayer (ActLayerId, false);


		if (gameState != Application.isPlaying) {
			gameState = Application.isPlaying;

			if (stateMainWind == InspectorState.EditAnimator) {
				LoadLayer (ActLayerId, false);
			}



			/*if (gameState) {
				if (stateMainWind == InspectorState.EditAnimator) {
				} else {
				}			
			}*/
		}



	/*	
	 if (selectedNode == null)
			Debug.Log ("nulidad");
		else 	Debug.Log ("ok");
*/

	//	if (GameManager.Resources == null)
	//		GameObject.FindObjectOfType<GameManager> ().AwakeEditor ();
	

		EditorGUI.DrawRect(new Rect(1, 1, position.width, position.height), new Color(0.25f, 0.25f, 0.25f, 1.0f) );


	



		DrawGrid(20, 0.2f, Color.gray);
		DrawGrid(100, 0.4f, Color.gray);



	
		if(ProcessEvents (Event.current))Refresh();
	


		BeginWindows();
		// All GUI.Window or GUILayout.Window must come inside here

		actAnimationInspector = GUILayout.Window (3, new Rect ( ((position.width - 220)), 1, 220, position.height), DrawAnimationInspector, "Node inspector");

	

		switch (stateAnimationWind) {
		case InspectAnimationWind.open:
			actAnimationHelper = GUILayout.Window (5, actAnimationHelper, AnimationHelper, "Animations");

			break;

		}
		DrawNodes();

		switch (stateMainWind){


		case InspectorState.AnimatorSelect:

                if (searchFileLoop == null || !searchFileLoop.isRunning)
                {
                    LoadAnim();
                    searchFileLoop = new Tra_LoopPack(10f);
                }
		/*	if (GameManager.Resources != null) {
			}*/


			actWindowInspector = GUILayout.Window (1, new Rect (1, 1, 220, position.height), DrawAnimatorList, "Animators");
			break;

		case InspectorState.NewAnimator:
			actWindowInspector = GUILayout.Window (1,  new Rect (20, 20, 200, 100), NewAnimatorFunc, "New Animator");	
			break;

		case InspectorState.EditAnimator:
		
			if(GUI.Button(new Rect( (position.width/2) - 100,10,200,20),"Switch to animation..")){
				if(stateAnimationWind == InspectAnimationWind.open)
					stateAnimationWind = InspectAnimationWind.none;
				else stateAnimationWind = InspectAnimationWind.open;

				actAnimationHelper = new Rect (mousePosition.x, mousePosition.y, 400, position.height*0.8f);
			}

			if (ActAnim == null)
				stateMainWind = InspectorState.AnimatorSelect;
				
			actWindowInspector = GUILayout.Window (1, new Rect (1, 1, 220, position.height), AnimatorLayout, ActAnim.name);	
			if(ProcessNodeEvents (Event.current)) Refresh();



		
			switch (stateEditorAnimator) {
			case EditorAnimWidow.NewVariable:
				actWindowpector2 = GUILayout.Window (2, actWindowpector2, NouvelleVariable, "New variable");	
				break;		



		case EditorAnimWidow.NewLayer:
				actWindowpector2 = GUILayout.Window (2, actWindowpector2, NouveauLayer, "New layer");	
			break;
		}


			break;
		}






		EndWindows();
	
	
			


	
	

		if (gui_Changed) Repaint();

	}


    Vector2 animatorSelectionScroll;


	void DrawAnimatorList(int unusedWindowID)
	{
		ResetFields ();


		GUILayout.Label ("Animators :");

        animatorSelectionScroll = EditorGUILayout.BeginScrollView(animatorSelectionScroll);

		if (ListAnimator != null) {
			for (int k = 0; k < ListAnimator.Count; k++) {

				if (ListAnimator[k] == null)
					ListAnimator.RemoveAt (k);
				
				GUILayout.BeginHorizontal ("box");

				if (fieldsInt [0] != k + 1) {				
					if (GUILayout.Button ("-")) {

						fieldsInt [0] = k + 1;
					}
					GUILayout.FlexibleSpace ();

					GUILayout.Label (ListAnimator [k].name + " (" + ListAnimator [k].genId.ToString () + ")");

					GUILayout.FlexibleSpace ();

					if (GUILayout.Button ("Edit")) {
						LoadAnimator (ListAnimator[k]);
					}

				
				} else {
					if (GUILayout.Button ("Non"))
						fieldsInt [0] = 0;

					GUILayout.FlexibleSpace ();
					GUILayout.Label ("Confirm delete ?");
					GUILayout.FlexibleSpace ();

					if (GUILayout.Button ("Oui")) {
						fieldsInt [0] = 0;
						AssetDatabase.DeleteAsset (AssetDatabase.GetAssetPath(ListAnimator[k]));
						//GameManager.Resources.DelAnimator ( k); 

				
			
					}


				}

				GUILayout.EndHorizontal ();


			}

            EditorGUILayout.EndScrollView();



        }

		if (GUILayout.Button ("NewAnimator")) {
			stateMainWind = InspectorState.NewAnimator;
			ClearFields ();

		}
		/*
		if (GUILayout.Button ("LoadAnimator")) {
			 string path = EditorUtility.OpenFilePanel("Overwrite with png", "", "asset");
			if (path.Length != 0)
			{

				Animator2D o = AssetDatabase.LoadAssetAtPath<Animator2D> (path);
				if (o is Animator2D) {
					ActAnim = (Animator2D)o;
				
					LoadAnim ();
				}
			}

		}*/


	//	GUI.DragWindow();

	}




	void NewAnimatorFunc(int unusedWindowID){
		ResetFields ();

		GUILayout.Label ("Name :");
		fields[fieldId] = GUILayout.TextField (fields[fieldId++]);


		if ( (GUILayout.Button ("Create") || Event.current.keyCode == KeyCode.Return ) && fields[0] != "" && stateMainWind == InspectorState.NewAnimator ) {
			CreateAnimator ();
		}

		if (GUILayout.Button ("Cancel")) {
			stateMainWind = InspectorState.AnimatorSelect;

		}

		GUI.DragWindow ();

	}


	private string GetActFolder(ref int type){
		string path = "";

		var obj = Selection.activeObject;
		if (obj == null)
			path = "Assets";


		else  path = AssetDatabase.GetAssetPath(obj.GetInstanceID());

		if (path.Length > 0)
		{
			if (Directory.Exists(path))
			{
		//		Debug.Log("Folder");
				type = 0;
				return path;
			}
			else
			{
		//		Debug.Log("File");
				type = 1;
			}
		}
		else
		{
		//	Debug.Log("Not in assets folder");

			type = 2;
		}

		return path;
	}



	private void loadAnimationList(string folder, ref List<Animation2D> anims){
	
		string[] fls = Directory.GetFiles (folder);
		Object gb;
		int h;

		for ( h = 0; h < fls.Length; h++) {
			

			gb = AssetDatabase.LoadAssetAtPath<Animation2D> (fls [h]);
			if (gb is Animation2D) {
				CheckName (fls [h]);
				anims.Add ((Animation2D)gb);
			}
		}

		fls = Directory.GetDirectories (folder);



		for (h = 0; h < fls.Length; h++) {
			loadAnimationList (fls [h], ref anims);
		}
	}





	private void AddSpriteToList(ref SpriteList lst, object[] files, bool Add){

		int c = 0, e = 0;
		int j;
		Sprite[] sp;

		for ( j = 0; j < files.Length; j++) {
			if (files [j] is Sprite)
				c++;
			else
				files [j] = null;
		}

		if (c == 0)
			return;

		if (!Add) {
			lst.sprites = new Sprite[c];

		}else{
			
			sp = new Sprite[lst.sprites.Length + c];
			lst.sprites.CopyTo (sp, 0);
			lst.sprites = sp;
			e = lst.sprites.Length-1;
		}

		for ( j = 0; j < files.Length; j++) {
			if (files [j] == null)
				continue;

			lst.sprites [e++] = (Sprite)files [j];
				
		}
		 


	}


	string lastFolderSearch = "";
	List<Animation2D> animHelpList;
	int h_selAnim;
	int h , k, m;
	object[] drop;
	bool error;
	Rect spRect;

	private void CheckName(string path){
		string nam = Path.GetFileName (path);
		nam = nam.Remove (nam.Length - 6);
		Animation2D a = (Animation2D)AssetDatabase.LoadAssetAtPath<Animation2D> (path);
		if (a.name != nam)
			a.name = nam;

	}

	private void AnimationHelper(int id){
		Refresh ();

		int fileType = 0;
		string actFolder = GetActFolder (ref fileType);



		GUI.color = new Color (1f, 0.8f,0.8f,1f);
		GUILayout.BeginVertical("box");
		GUI.color = Color.white;

		GUILayout.BeginHorizontal ();


		if (actFolder == "Assets") {
			actFolder = lastFolderSearch;	
		}

	

	
		if(fileType == 1){
			if (AssetDatabase.LoadAssetAtPath<Animation2D> (actFolder) is Animation2D) {
				CheckName (actFolder);
				actFolder = actFolder.Remove (actFolder.Length - ((  (Animation2D)AssetDatabase.LoadAssetAtPath<Animation2D> (actFolder)).name.Length + 7));

			}
			else fileType = 2;
			}


		if (actFolder == "Assets" || fileType == 2) {
			actFolder = lastFolderSearch;
		}

	

	

		//	GUILayout.Label ("Animator's folder : " + actFolder);
		//} else
		GUILayout.Label ("Folder : " + lastFolderSearch);

		//if(fileType == 1){


		GUILayout.FlexibleSpace ();

		if (GUILayout.Button ("<", GUILayout.Width (20))) {
			string[] sub = actFolder.Split (new char[1]{ '/' });

			if (sub.Length > 1) {
				actFolder = "";
				Selection.activeObject = null;
				for (int k = 0; k < sub.Length - 1; k++)
					actFolder += sub [k] + (string)((k == sub.Length-2)?"":"/");

			}

		}

		err0:
		if (GUILayout.Button ("proj", GUILayout.Width (40)) ||  actFolder == "") {
			actFolder = AssetDatabase.GetAssetPath (ActAnim);	
			actFolder = actFolder.Remove (actFolder.Length - (ActAnim.name.Length + 7) );
			Selection.activeObject = null;
			error = false;
		}








		GUILayout.EndHorizontal ();
	


		if(lastFolderSearch != actFolder){


			h_selAnim = -1;
			animHelpList = new List<Animation2D> ();

			if (!error) {
				try {
					loadAnimationList (actFolder, ref animHelpList);		
				} catch {
					Debug.LogError("Overflow en chargeant a "+actFolder+",\n redemarez la fenettre ou appuyez sur 'proj' pour acceder au sprite editor");

					error = true;
				}

			}
		}
		GUILayout.EndVertical ();

		for (h = 0; h < animHelpList.Count; h++) {
			//Log (animHelpList [h].name);

			if (animHelpList [h] == null) {			
				actFolder = "";
				goto err0;
				}

			if(h_selAnim == h)
			GUI.color = new Color (0.7f, 1f,1f,1f);

			GUILayout.BeginVertical("box");
		
			if(h_selAnim == h)
			GUI.color = Color.white;

			
	
			GUILayout.BeginHorizontal ();

			if (GUILayout.Button (animHelpList [h].name, "label" )) {
				GUI.FocusControl ("");
				if (h_selAnim != h) {
					h_selAnim = h;
					fieldsInt [3] = animHelpList [h].spritePacks.Length;
				} else h_selAnim = -1;
			}

			GUILayout.FlexibleSpace ();

			if (GUILayout.Button ("o", "label")) {
				EditorUtility.FocusProjectWindow ();
				Selection.activeObject = animHelpList [h];

			}

			GUILayout.EndHorizontal ();
			//EditorStyles.boldLabel ()

			if (h_selAnim == h) {
				
				GUILayout.BeginHorizontal();
				GUILayout.Label ("Skins ");
				fieldsInt[3] = EditorGUILayout.IntField (fieldsInt[3]);

				fieldsInt [3] = ReMath.Clamp (fieldsInt [3], 1, 9);



				if (fieldsInt [3] != animHelpList [h].spritePacks[0].skins.Length) {
					SpriteList[] lst = new SpriteList[fieldsInt[3]];
					GUI.FocusControl ("");
					for ( k = 0; k < fieldsInt[3]; k++) {

						if (k < animHelpList [h].spritePacks[0].skins.Length)
							lst [k] = animHelpList [h].spritePacks[0].skins[k];
						else
							lst [k] = new SpriteList();

					}

					animHelpList [h].spritePacks[0].skins = lst;

				}

				GUILayout.EndHorizontal ();


				for ( k = 0; k < animHelpList[h].spritePacks[0].skins.Length; k++) {

				

					GUILayout.BeginHorizontal ("box");
					GUILayout.Label ("Skins " + (k + 1).ToString () + ":");

					drop = DropZone ("Drop here (set)", 350, 18, (animHelpList[h].spritePacks[0].skins[k].sprites.Length != 0) );


					if (drop != null) 						
						AddSpriteToList ( ref animHelpList[h].spritePacks[0].skins[k], drop, false);

					GUILayout.FlexibleSpace ();

					drop = DropZone ("Add", 30, 18, false );

					if (drop != null) 						
						AddSpriteToList ( ref animHelpList[h].spritePacks[0].skins[k], drop, true);

					if (GUILayout.Button ("o")) {
						ShowPicker <Sprite>();
						currentPickerWindowSp = k;
					}

					if(currentPickerWindowSp == k && Event.current.commandName == "ObjectSelectorUpdated" && EditorGUIUtility.GetObjectPickerControlID() == currentPickerWindow )
					{        

						AddSpriteToList(ref animHelpList [h].spritePacks[0].skins [k], new object[1]{EditorGUIUtility.GetObjectPickerObject()}, true);
						currentPickerWindow = -1;
						currentPickerWindowSp = -1;
						//name of selected object from picker
				
					}

						
					for (m = 0; m < animHelpList [h].spritePacks[0].skins [k].sprites.Length; m++) {
						spRect = new Rect (65 + 20 * m, 91 + (26 * h) + (30 * k), 20, 20);
						
						if (GUI.Button (spRect, "☺", "label")) {
							ProcessSpMenu (new Vector3Int(k, m, 0) );
							break;
						}

						if (svspstd != Vector3Int.down) {

							if (svspstd.z == 0) {
								
								Sprite[] s = new Sprite[animHelpList [h].spritePacks[0].skins [svspstd.x].sprites.Length - 1];
								for (int j = 0; j < s.Length; j++) {
									if (j < svspstd.y)
										s [j] = animHelpList [h].spritePacks[0].skins [svspstd.x].sprites [j];
									else
										s [j] = animHelpList [h].spritePacks[0].skins [svspstd.x].sprites [j + 1];

								}
								animHelpList [h].spritePacks[0].skins [svspstd.x].sprites = s;
								Refresh ();
								svspstd = Vector3Int.down;
								break;
							}
							if (svspstd.z == 1) {
								if (animHelpList [h].spritePacks[0].skins [svspstd.x].sprites [svspstd.y] != null) {
									//AssetDatabase.GetAssetPath (animHelpList [h].skins [svspstd.x].sprites [svspstd.y] );
									EditorUtility.FocusProjectWindow ();
									//EditorUtility.WarnPrefab(
									//Selection.
									Selection.activeObject = animHelpList [h].spritePacks[0].skins [svspstd.x].sprites [svspstd.y];
									svspstd = Vector3Int.down;
								}

							}

						}


						if (animHelpList [h].spritePacks[0].skins [k].sprites [m] == null)
							continue;
					


						DrawTexturePreview (spRect, animHelpList [h].spritePacks[0].skins [k].sprites [m]);
			
					
					}
					GUILayout.EndHorizontal ();
					GUILayout.Space (2);
			

				}
			}


			GUILayout.EndVertical ();
		}

		GUILayout.FlexibleSpace ();
		GUILayout.BeginHorizontal ();
		GUILayout.FlexibleSpace ();
		if (GUILayout.Button ("Close", GUILayout.Width(50) ) ){
			stateAnimationWind = InspectAnimationWind.none;
		}
		GUILayout.EndHorizontal ();


		lastFolderSearch = actFolder;
		GUI.DragWindow ();
	}

	Vector3Int svspstd = Vector3Int.down;

	private void ProcessSpMenu(Vector3Int v3){

		GenericMenu genericMenu = new GenericMenu();
		genericMenu.AddItem(new GUIContent("Target"), false, () => TargetSpList(v3)); 
		genericMenu.AddItem(new GUIContent("Remove"), false, () => RemoveSpList(v3)); 

		genericMenu.ShowAsContext();



	

	}

	private void RemoveSpList(Vector3Int sv){
		svspstd = sv;
	}
	private void TargetSpList(Vector3Int v3){
		svspstd = new Vector3Int(v3.x, v3.y, 1);
	}




	//in your EditorWindow class

	int currentPickerWindow, currentPickerWindowSp, currentPickerWindowAnim;

	void ShowPicker<T>() where T : UnityEngine.Object {
		//create a window picker control ID
		currentPickerWindow = EditorGUIUtility.GetControlID(FocusType.Passive) + 100;

		//use the ID you just created
		EditorGUIUtility.ShowObjectPicker<T>(null,false,"",currentPickerWindow);
	}




private Rect DrawTexturePreview(Rect position, Sprite sprite)
	{
		Vector2 fullSize = new Vector2 (sprite.texture.width, sprite.texture.height);
		Vector2 size = new Vector2 (sprite.textureRect.width, sprite.textureRect.height);

		Rect coords = sprite.textureRect;
		coords.x /= fullSize.x;
		coords.width /= fullSize.x;
		coords.y /= fullSize.y;
		coords.height /= fullSize.y;

		Vector2 ratio;
		ratio.x = position.width / size.x;
		ratio.y = position.height / size.y;
		float minRatio = Mathf.Min (ratio.x, ratio.y);

		Vector2 center = position.center;
		position.width = size.x * minRatio;
		position.height = size.y * minRatio;
		position.center = center;

		GUI.DrawTextureWithTexCoords (position, sprite.texture, coords);
		return coords;
	}
						

	private void AnimatorLayout(int unusedWindowID){
		int j;

		//GUILayout.BeginVertical ("box");
		GUILayout.Space(5f);



		if (GUILayout.Button ("Back")) {
			SaveActualAnimator ();
		
			ActAnim = null;
			ClearEditor ();
			stateMainWind = InspectorState.AnimatorSelect;
			stateAnimationWind = InspectAnimationWind.none;
		}



		if (GUILayout.Button ("Actualise"))
			SaveActualAnimator ();

		GUILayout.Space(5f);
	
	//	GUILayout.EndVertical ();



	GUILayout.BeginVertical ("box");

	GUILayout.BeginHorizontal ();
	GUILayout.FlexibleSpace ();

		GUILayout.Label ("Layers :", EditorStyles.boldLabel);
	GUILayout.FlexibleSpace ();
	GUILayout.EndHorizontal();




		for (j = 0; j < ListLayers.Count; j++) {


			if(ActLayerId == j)
				GUI.color = new Color (0.75f, 0.75f, 0.75f);


		GUILayout.BeginHorizontal ("box");	
			if(ActLayerId == j)
				GUI.color = Color.white;

		if (fieldsInt [1] != j + 1) {
				if (GUILayout.Button ("-", GUILayout.Width(15)) )
				fieldsInt [1] = j + 1;


				if (GUILayout.Button ("d", GUILayout.Width(18))){
					//ListLayers.Add (ObjectCopier.Clone<>(ActLayer.Clone());
				//	ListLayers.Add (ActLayer.Clone());
					ListLayers.Add (ActLayer.Copy());
				}

			
					

				GUILayout.FlexibleSpace ();

				if(ActLayerId == j)
					ListLayers [j].name = EditorGUILayout.TextField (ListLayers [j].name);
				else{
					if (GUILayout.Button (ListLayers [j].name, "label", GUILayout.MinWidth(150) ))
					LoadLayer (j);
				}

				GUILayout.FlexibleSpace ();


		} else {

			if (GUILayout.Button ("Non"))
				fieldsInt [1] = 0;

			GUILayout.FlexibleSpace ();
			GUILayout.Label ("Confirm delete ?");
			GUILayout.FlexibleSpace ();

				if (GUILayout.Button ("Oui")) {
					if (ListLayers.Count > 1) {
						fieldsInt [1] = 0;
						ListLayers.Remove (ListLayers [j]);
						ActLayerId = 0;
						LoadLayer (ActLayerId, false);
					} else
						Log ("L'animator doit au moin avoir un layer. Avant de suprimer celui là faites en un autre !");
				}



		}

	

		GUILayout.EndHorizontal ();

		


	}
		GUILayout.Space (3f);

		if (GUILayout.Button ("New Layer")) {
			stateEditorAnimator = EditorAnimWidow.NewLayer;
			actWindowpector2 = new Rect (mousePosition.x, mousePosition.y, 220, 50);
			ClearFields ();

		}
		GUILayout.Space (3f);


		GUILayout.EndVertical ();









	GUILayout.Space(5f);



		GUILayout.BeginVertical ("box");

		GUILayout.BeginHorizontal ();
		GUILayout.FlexibleSpace ();
		GUILayout.Label ("Variables :", EditorStyles.boldLabel);
		GUILayout.FlexibleSpace ();
		GUILayout.EndHorizontal();


		for ( j = 0; j < variableAnim.Count; j++) {




			GUILayout.BeginHorizontal ("box");	


			if (fieldsInt [0] != j + 1) {
				if (GUILayout.Button ("del", GUILayout.Width(30) )) 
					fieldsInt [0] = j + 1;
				

				GUILayout.FlexibleSpace ();

				GUILayout.Label (variableAnim [j].name);

				GUILayout.FlexibleSpace ();



				if (GUILayout.Button ("-", GUILayout.Width (20))) {
					variableAnim [j].val--;
				}





				GUILayout.TextField (variableAnim [j].val.ToString (), 3, GUILayout.Width (25));
				if (GUILayout.Button ("+", GUILayout.Width (20))) {
					variableAnim [j].val++;
				}

			} else {

				if (GUILayout.Button ("Non"))
					fieldsInt [0] = 0;

				GUILayout.FlexibleSpace ();
				GUILayout.Label ("Confirm delete ?");
				GUILayout.FlexibleSpace ();

				if (GUILayout.Button ("Oui")){
					if (variableAnim.Count > 1) {
						fieldsInt [0] = 0;
						variableAnim.Remove (variableAnim [j]);
						LayerRefresh (1, j);
					} else 						Log ("Il n'y a qu'une seule variable !");
					

					}
					



			}
		

			GUILayout.EndHorizontal ();

		}


	





		GUILayout.Space (3f);

			if (GUILayout.Button ("New variable")) {
				stateEditorAnimator = EditorAnimWidow.NewVariable;
			actWindowpector2 = new Rect (mousePosition.x, mousePosition.y, 220, 50);
				ClearFields ();
			 
			}
		GUILayout.Space (3f);

		GUILayout.EndVertical ();

	
	}

	private void NouvelleVariable(int unusedWindowID){

		ResetFields ();
		GUILayout.BeginHorizontal ();
		GUILayout.Label ("Name :");
		fields[fieldId] = GUILayout.TextField (fields[fieldId++]);
		GUILayout.EndHorizontal ();

		GUILayout.FlexibleSpace ();

		GUILayout.BeginHorizontal ();
		if (GUILayout.Button ("Cancel")) {
			stateEditorAnimator = EditorAnimWidow.none;
		}



		if ( (GUILayout.Button ("Add") || Event.current.keyCode == KeyCode.Return ) && fields[0] != "" && stateEditorAnimator == EditorAnimWidow.NewVariable) {

			variableAnim.Add (new varion (0, fields [0]));


			stateEditorAnimator = EditorAnimWidow.none;
			return;
		}

		GUILayout.EndHorizontal ();

		GUI.DragWindow ();

	}


private void NouveauLayer(int unusedWindowID){

	ResetFields ();
	GUILayout.BeginHorizontal ();
	GUILayout.Label ("Name :");
	fields[fieldId] = GUILayout.TextField (fields[fieldId++]);
	GUILayout.EndHorizontal ();

	GUILayout.FlexibleSpace ();

	GUILayout.BeginHorizontal ();
	if (GUILayout.Button ("Cancel")) {
		stateEditorAnimator = EditorAnimWidow.none;
	}



	if ( (GUILayout.Button ("Add") || Event.current.keyCode == KeyCode.Return ) && fields[0] != "" && stateEditorAnimator == EditorAnimWidow.NewLayer) {

		ListLayers.Add (new AnimatorLayer2D(fields[0]) );


		stateEditorAnimator = EditorAnimWidow.none;
		return;
	}

	GUILayout.EndHorizontal ();

	GUI.DragWindow ();

}

    Vector2 nodeEditorScroll;

	public void DrawAnimationInspector(int WindId){
		object[] drop;
		int x, y;
		condition[] conds;

        nodeEditorScroll = EditorGUILayout.BeginScrollView(nodeEditorScroll);

		BigText ("Node inspector :");

	

		if (selectedNode == null)
			GUILayout.Label ("select a node & edit it");
		else {

		
			GUILayout.BeginVertical ("box");

			BigText ("File :", 1f);


			GUILayout.BeginHorizontal ();
			GUILayout.FlexibleSpace ();
			drop =	DropZone ((string)((selectedNode.anim != null) ? selectedNode.anim.name : "drop here"), 144, 20);

	
			if (GUILayout.Button ("o", GUILayout.Width (18))) {
				ShowPicker <Animation2D>();
				currentPickerWindowAnim = selectedNode.listId;
				}



			if(currentPickerWindowAnim == selectedNode.listId && Event.current.commandName == "ObjectSelectorUpdated" && EditorGUIUtility.GetObjectPickerControlID() == currentPickerWindow )
				{


				selectedNode.anim = (Animation2D)EditorGUIUtility.GetObjectPickerObject ();
					currentPickerWindow = -1;
				currentPickerWindowAnim = -1;
				Refresh ();
					//name of selected object from picker

				}


			if (selectedNode.anim != null)
			if (GUILayout.Button (">", GUILayout.Width (18))) {
				EditorUtility.FocusProjectWindow ();
				Selection.activeObject = selectedNode.anim;
			}


			GUILayout.FlexibleSpace ();

			GUILayout.EndHorizontal ();
			GUILayout.Space (3);
			GUILayout.EndVertical ();

	




			GUILayout.BeginVertical ("box");
			BigText ("Parameters :");




			GUILayout.BeginHorizontal ();

			GUILayout.Label ("Animation speed : ");
			selectedNode.speedCoef = EditorGUILayout.FloatField (selectedNode.speedCoef);
			GUILayout.EndHorizontal ();
			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Looping animation : ");
			selectedNode.loopingAnim = EditorGUILayout.Toggle (selectedNode.loopingAnim);


			GUILayout.EndHorizontal ();

			GUILayout.Label ("ListId:" + selectedNode.listId.ToString ());


			GUILayout.EndVertical ();



			if (drop != null) {
				if (drop [0] is  Animation2D)
					selectedNode.anim = (Animation2D)drop [0];


			}

		
		







			GUILayout.BeginVertical ("box");
			BigText ("Transitions :");



			for (x = 0; x < selectedNode.Outputs.Count; x++) {
		
				conds = selectedNode.Outputs [x].conditions;

				
				GUI.color = ColorBySeed (selectedNode.Outputs [x].Exit.listId);
				GUILayout.BeginVertical ("box");
				GUI.color = Color.white;

				GUILayout.BeginHorizontal ();
			
				GUI.color = Color.red;
				if (GUILayout.Button( "x", GUILayout.Width(16), GUILayout.Height(16))) {
					selectedNode.Outputs.RemoveAt (x);
					continue;
				}

				GUI.color = Color.white;

				if (GUILayout.Button( "^", GUILayout.Width(18), GUILayout.Height(16))) {
					selectedNode.Outputs.Insert (0, selectedNode.Outputs [x]);
					selectedNode.Outputs.RemoveAt (x+1);


					break;
				}

				GUILayout.Space (4);
				CenteredText ("To " + selectedNode.Outputs [x].Exit.localName + " :");



			

					GUILayout.EndHorizontal ();
		


				for (y = 0; y < conds.Length; y++) {
					GUILayout.BeginHorizontal ();
					if(y == 0)
						GUILayout.Label ("if", GUILayout.Width(13));
					else GUILayout.Label ("&", GUILayout.Width(13));

					conds [y].var1 = EditorGUILayout.Popup(conds [y].var1, variableList, GUILayout.Width(50) ); 
			

					conds[y].type = (CondType)EditorGUILayout.EnumPopup(conds[y].type, GUILayout.Width(60));
				
				
					conds[y].value = EditorGUILayout.IntField (conds[y].value, GUILayout.Width(35));



					if(GUILayout.Button("-", GUILayout.Height(15))){
						conds = new condition[conds.Length - 1];
						for(int c = 0; c < conds.Length; c++){
							if (c < x)
								conds[c] = selectedNode.Outputs [x].conditions [c];
							else
								conds[c] = selectedNode.Outputs [x].conditions [c+1];
							}
						selectedNode.Outputs [x].conditions = conds;

					}
					GUILayout.FlexibleSpace ();
					GUILayout.EndHorizontal ();

				}

				GUILayout.BeginHorizontal ();
				GUILayout.Label ("End interaction : ");
				selectedNode.Outputs [x].endAction = (AnimEnd)EditorGUILayout.EnumPopup(selectedNode.Outputs [x].endAction);
				GUILayout.EndHorizontal ();


				if (GUILayout.Button ("Add Condition")) {
					conds = new condition[selectedNode.Outputs [x].conditions.Length + 1];
					selectedNode.Outputs [x].conditions.CopyTo (conds, 0);
					conds [conds.Length - 1] = new condition ();
					selectedNode.Outputs [x].conditions = conds;

				}
		




				GUILayout.EndVertical ();

			}

		
	


			GUILayout.EndVertical ();


		}

        EditorGUILayout.EndScrollView();




        }



	static  object[] DropZone(string title, int w, int h, bool transparent = false){

		if (transparent)
			GUI.color = new Color (0, 0, 0, 0);


	
		try{
		GUILayout.Box(title, GUILayout.Width(w), GUILayout.Height(h));
		}catch{
		}
		GUI.color = Color.white;

		EventType eventType = Event.current.type;
		bool isAccepted = false;

		if ( (eventType == EventType.DragUpdated || eventType == EventType.DragPerform) &&  GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition)){

		
	


			DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

			if (eventType == EventType.DragPerform) {
				DragAndDrop.AcceptDrag();
				isAccepted = true;
			}
			Event.current.Use();


	
		}



		return isAccepted ? DragAndDrop.objectReferences : null;
	}


	private void BigText(string text, float margin = 3f){
		
		GUILayout.BeginHorizontal ();
		GUILayout.FlexibleSpace ();
		GUILayout.Label (text, EditorStyles.boldLabel);
		GUILayout.FlexibleSpace ();
		GUILayout.EndHorizontal ();

		GUILayout.Space (margin);


	}

	private void CenteredText(string text){

		GUILayout.BeginHorizontal ();
		GUILayout.FlexibleSpace ();
		GUILayout.Label (text);
		GUILayout.FlexibleSpace ();
		GUILayout.EndHorizontal ();

	}






	void ClearEditor(bool full = true){
		offset = Vector2.zero;
		nodes = new List<Node> (0);
		selectedNode = null;
		GUI.FocusControl ("");

		if (full) {
			ListLayers = new List<AnimatorLayer2D> (0);
			variableAnim = new List<varion> (0);
			ActLayerId = 0;
			defaultNodeId = 0;
		}
	}


	private void SaveActualAnimator(){
		int b;
		Log ("AnimSaved");
		SaveLayer ();
	//	Animator2D an = new Animator2D ();
	
		ActAnim.ListLayers = new AnimatorLayer2D[ListLayers.Count];



		for (int j = 0; j < ListLayers.Count; j++) {
			
			ActAnim .ListLayers[j] = ListLayers [j];




			if (variableAnim != null) {
				ActAnim.vars = new varion[variableAnim.Count];

				for (b = 0; b < variableAnim.Count; b++) {
					ActAnim.vars [b] = variableAnim [b];
		
				}

			}
		}

		EditorUtility.SetDirty (ActAnim);
		UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty (UnityEngine.SceneManagement.SceneManager.GetActiveScene ());


		//	SerializedObject serializedObject = new UnityEditor.SerializedObject((Object)ActAnim);

		//	SerializedProperty serializedPropertyMyInt = serializedObject.FindProperty("myInt");


	/*	an.genId = ActAnim.genId;
		an.ListAnims = ActAnim.ListAnims;
		an.listId =  ActAnim.listId;
		an.name =  ActAnim.name;
		an.vars =  ActAnim.vars;
*/
		//GameManager.Resources.SaveAnimator (ActAnim, ActAnim.listId);


	
	}





	private void CreateAnimator(){		



		Animator2D anim = ScriptableObject.CreateInstance<Animator2D> ();


		anim.name = fields [0];


		AssetDatabase.CreateAsset (anim, "Assets/Animation/Animator/"+  anim.name +".asset");
		AssetDatabase.SaveAssets ();

		//ListLayers = new AnimatorLayer2D[1]{new AnimatorLayer2D("Default")}; 


		EditorUtility.FocusProjectWindow ();

		Selection.activeObject = anim;

//		GameManager.Resources.AddAnimatorList (anim);

		LoadAnimator (anim);
	}
	



	void LoadAnimator(Animator2D anim){
		int j;
		ActAnim = anim;
		stateMainWind = InspectorState.EditAnimator;
		stateEditorAnimator = EditorAnimWidow.none;



		ClearEditor ();
	

	
		for (j = 0; j < ActAnim.ListLayers.Length; j++) {
			ListLayers.Add (ActAnim.ListLayers [j]);
		}


		 


		for ( j = 0; j < ActAnim.vars.Length; j++) {
			variableAnim.Add (ActAnim.vars [j]);
			}
		
		
		if (ListLayers.Count != 0)
			LoadLayer (0, false);
			

	}



	private Node ToNode(AnimatorBlock2D anim){
		Node nd = new Node(anim.visualPos, nodeSize.x, nodeSize.y, anim.listId);
			nd.speedCoef = anim.coefSpeed;
		nd.loopingAnim = anim.looping;
		nd.anim = anim.animation;

		nd.Outputs = new List<TransitionNode> (0);


		for (int j = 0; j < anim.transitions.Length; j++) {
			nd.Outputs.Add (new TransitionNode(anim.transitions [j]));

		}


		return nd;
	}


	public static void Log(object txt){
		Debug.Log (txt);
	}

	private void LoadLayer(int id, bool saveLast = true){
		int j = 0, b = 0;
		if(saveLast)
		SaveLayer ();

		ClearEditor (false);
		ActLayer = ListLayers[id];
		ActLayerId = id;
		defaultNodeId = ActLayer.defaultAnimId;

		for ( j = 0; j < ActLayer.ListAnims.Length; j++) {
			nodes.Add(ToNode(ActLayer.ListAnims [j]));
			offset += (ActLayer.ListAnims [j].visualPos );
			}

		if(ActLayer.ListAnims.Length != 0)
			offset = new Vector2 (offset.x / ActLayer.ListAnims.Length, offset.y / ActLayer.ListAnims.Length) ;


		for (j = 0; j < nodes.Count; j++) {			
			
				for (b = 0; b < nodes [j].Outputs.Count; b++) 
				
					nodes [j].Outputs [b].Exit = nodes [nodes [j].Outputs [b].toAnimId];

				

				nodes [j].Drag ((position.size / 2.0f) - offset);
			}

	}



	private void SaveLayer(){
		int b;

		ActLayer.defaultAnimId = defaultNodeId;

		if (ActLayer == null)
			return;


		if (nodes != null) {		
			ActLayer.ListAnims = new AnimatorBlock2D[nodes.Count];




			for (b = 0; b < nodes.Count; b++)
				ActLayer.ListAnims [b] = nodes [b].ToAnimatorBlock ();




		}



	}




	private void DrawNodes()
	{

		int i = 0;

		if (nodes != null) {
			for (i = 0; i < nodes.Count; i++)
				nodes [i].DrawConnections ();
		

			for (i = 0; i < nodes.Count; i++) {
				if (i == defaultNodeId)
					nodes [i].Draw ( (GUIStyle)((selectedNode == nodes[i])? nodeStyle.nodeStyleSelectedFirst:  nodeStyle.nodeStyleFirst)  );
				else 
					nodes [i].Draw ( (GUIStyle)((selectedNode == nodes[i])? nodeStyle.nodeStyleSelected:  nodeStyle.nodeStyle ) );
				
					
			}
			

	}
	}

	private void LayerRefresh(int mode, int at){
		//refresh apres supression d'un element
		//mod 0 = node
		//mode 1 = variable
		//at = id du truc

		int d, h, j;

		if (nodes == null)
			return;


		for ( h = 0; h < nodes.Count; h++) {


			switch(mode){
			case 0:
				if (nodes [h].listId > at)///list id = h quoi
					nodes [h].listId--;

				if (defaultNodeId > at)
					defaultNodeId--;
					
				break;

			case 1:


				break;
			}



			if (nodes[h].Outputs != null)

			


				for ( j = 0; j < nodes[h].Outputs.Count; j++) {

					switch(mode){
				case 0:

					if (nodes [h].Outputs [j].toAnimId > at)
						nodes [h].Outputs [j].toAnimId--;
					

					if (nodes [h].Outputs [j].toAnimId == at) {//deja fait avec les inputs mais oups
						nodes [h].Outputs.Remove (nodes [h].Outputs [j]);
						j--;				
					}


			
							break;


					case 1:

					for ( d = 0; d < nodes[h].Outputs[j].conditions.Length; d++) {



						if (nodes [h].Outputs [j].conditions [d].var1 > at)
							nodes [h].Outputs [j].conditions [d].var1--;
						

						if (nodes [h].Outputs [j].conditions [d].var1 == at) {
							condition[] cd = new condition[nodes [h].Outputs [j].conditions.Length - 1];
				
							for(int c = 0; c < cd.Length; c++){
								if (c < at)
									cd[c] = selectedNode.Outputs [at].conditions [c];
								else
									cd[c] = selectedNode.Outputs [at].conditions [c+1];
							}
							nodes [h].Outputs [j].conditions = cd;
							d--;

						}

			

						}
					
						break;
					
						}
	


		}

	}
	}

	private bool ProcessNodeEvents(Event e)
	{
		bool guiChanged = false;

		if (nodes != null)
		{			

			for (int i = nodes.Count - 1; i >= 0; i--)
			{


				guiChanged = nodes [i].ProcessEvents (e, ref selectedNode) || guiChanged;



				if (nodes [i].waitDef) {
					nodes [i].waitDef = false;
					defaultNodeId = i;
					guiChanged = true;
				}

				if (nodes [i].CheckRemove ()) {
					if (nodes [i] == selectedNode)
						selectedNode = null; 

					nodes.RemoveAt (i);
					LayerRefresh (0, i);


					guiChanged = true;
				}



			}
		

			if (selectedNode != null) {
				if (selectedNode.setTransition) {					
					Handles.DrawLine ((Vector3)selectedNode.rect.center, (Vector3)e.mousePosition);
					guiChanged = true;
				}


				if (newTransition != null && newTransition != selectedNode) {
					newTransition.AddTransitionOut (selectedNode);
					newTransition = null;

				}
			}




			


			return guiChanged;
		}

	




		return false;
	}







	private bool ProcessEvents(Event e)
	{
		object[] drop;

		drag = Vector2.zero;
		mousePosition = e.mousePosition;


		mouseInInspector = actWindowInspector.Contains (e.mousePosition)
			|| actAnimationInspector.Contains (e.mousePosition)
			|| (actAnimationHelper.Contains(e.mousePosition) && stateAnimationWind == InspectAnimationWind.open);


		if (stateMainWind == InspectorState.EditAnimator) {
			
			if (variableList.Length != variableAnim.Count) {
				variableList = new string[variableAnim.Count];
				for (int h = 0; h < variableList.Length; h++)
					variableList [h] = variableAnim [h].name;
			}

			if (!mouseInInspector) {
				drop = DropZone ("", (int)position.width, (int)position.height, true);

				if (drop != null) {
					if (drop [0] is Animation2D) {
						nodes.Add (new Node (mousePosition - new Vector2 (nodeSize.x / 2.0f, nodeSize.y / 2.0f), nodeSize.x, nodeSize.y, nodes.Count));
						nodes [nodes.Count - 1].anim = (Animation2D)drop [0];
					}
				}

			}
		}
			


		switch (e.type) {
		case EventType.MouseDown:



			if (!mouseInInspector && stateMainWind == InspectorState.NewAnimator) {
				stateMainWind = InspectorState.AnimatorSelect; 
				return true;
			}

			if (stateMainWind == InspectorState.EditAnimator) {
				if (e.button == 0) {
					if (!mouseInInspector) {	
						if (selectedNode != null) {
							if (selectedNode.setTransition)
								newTransition = selectedNode;
						}
				
						selectedNode = null;
						GUI.FocusControl ("");
						//ClearConnectionSelection();
					
				}
				needSave = true;
				
			}
		
			

				if (mouseInInspector && !actWindowpector2.Contains(e.mousePosition)  ) {
					stateEditorAnimator = EditorAnimWidow.none;
					Refresh ();
				}


				if (e.button == 1 && !mouseInInspector) {
					ProcessContextMenu (e.mousePosition);
				}





			}
			break;

		case EventType.MouseDrag:
			if (e.button == 2) {
				OnDrag (e.delta);
			}
			break;



		case EventType.KeyDown:
			if (e.keyCode == KeyCode.Escape && selectedNode != null) {
				newTransition = null;
				if(!mouseInInspector ){
				selectedNode.Unselect();
				selectedNode = null;

				}
			}

		


			break;

		}

		return false;
	}

	private void OnDrag(Vector2 delta)
	{

	

		drag = delta;

		if (nodes != null)
		{
			for (int i = 0; i < nodes.Count; i++)
			{
				nodes[i].Drag(delta);
			}
		}

		Refresh ();
	}



	private void DrawGrid(float gridSpacing, float gridOpacity, Color gridColor)
	{
		int widthDivs = Mathf.CeilToInt(position.width / gridSpacing);
		int heightDivs = Mathf.CeilToInt(position.height / gridSpacing);

		Handles.BeginGUI();
		Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridOpacity);

		offset += drag * 0.5f;
		Vector3 newOffset = new Vector3(offset.x % gridSpacing, offset.y % gridSpacing, 0);

		for (int i = 0; i < widthDivs; i++)
		{
			Handles.DrawLine(new Vector3(gridSpacing * i, -gridSpacing, 0) + newOffset, new Vector3(gridSpacing * i, position.height, 0f) + newOffset);
		}

		for (int j = 0; j < heightDivs; j++)
		{
			Handles.DrawLine(new Vector3(-gridSpacing, gridSpacing * j, 0) + newOffset, new Vector3(position.width, gridSpacing * j, 0f) + newOffset);
		}

		Handles.color = Color.white;
		Handles.EndGUI();
	}



	private void ProcessContextMenu(Vector2 mousePosition)
	{
		GenericMenu genericMenu = new GenericMenu();
		genericMenu.AddItem(new GUIContent("Add node"), false, () => OnClickAddNode(mousePosition)); 
		genericMenu.AddItem(new GUIContent("Duplicate All"), false, () => OnClickDuplication()); 
		genericMenu.ShowAsContext();
	}

	private void OnClickAddNode(Vector2 mousePosition)
	{
		if (nodes == null)
		{
			nodes = new List<Node>();
		}

		nodes.Add(new Node(mousePosition, nodeSize.x, nodeSize.y, nodes.Count));

		Refresh ();
	}

	private void OnClickDuplication(){



		if (nodes == null)
			return;

		 int nc = nodes.Count;
		int i = 0;

		for ( i = 0; i < nc; i++) {
			nodes.Add (nodes [i].Clone ());
			nodes [i+nc].listId = nc+i;
		}

		//for ( i = 0; i < nc; i++) {
		///	nodes [i].ResetupTransition (0, nodes);



		for ( i = nc; i < nodes.Count; i++) {
			nodes [i].ResetupTransition (nc, nodes);

		}
		
	

	}
}




