using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class ResourcesHelperManager : BasicManager {

    [SerializeField]
    GameObject clonePrefab;
    public GameObject ClonePrefab => clonePrefab;

    List<Texture2D> textureGenerated;

    [SerializeField]
    IDictionary<Vector2Int, SpriteRenderer[]> paintableList;




    public void OnStartLevel()
    {
        paintableList.Clear();
    }


    private void Update()
    {
        PaintSprite.PaintUpdate();
    }




    public void AddPaintableSpriteRenderer(SpriteRenderer rd)
    {
        Vector2Int pos = ReMath.ToVec2IntFloor(rd.transform.position);
       //a refaire avec
        //rd.GetWorldRect
        if(rd.sprite != null)
        {
            
        }


        if (!paintableList.ContainsKey(pos))
        {
            paintableList.Add(pos, new SpriteRenderer[] { rd });
        }
        else
        {
            paintableList[pos] = ReMath.AddArray<SpriteRenderer>(paintableList[pos],  rd );
        }          
    }


    public  SpriteRenderer[] GetPaintableAtCoord(Vector2Int pos)
    {
        if (!paintableList.ContainsKey(pos))
            return new SpriteRenderer[0];
        return paintableList[pos];

    }





    protected override void InitAction()
	{
		textureGenerated = new List<Texture2D>();
        paintableList = new Dictionary<Vector2Int, SpriteRenderer[]>();

    }



    public void TryRemoveTexture(Texture2D removeOld)
	{
		if (textureGenerated.Contains(removeOld))
		{
			textureGenerated.Remove(removeOld);
			Destroy(removeOld);
		}
	}


		public void AddTexture(Texture2D tex, Texture2D removeOld = null) {

		if (!textureGenerated.Contains(tex))
			textureGenerated.Add(tex);

		if (removeOld != null)
			TryRemoveTexture(removeOld);
	}


    public bool ContainTexture(Texture2D tex)
    {
  //      Debug.LogError("check " + tex.GetInstanceID());
        return textureGenerated.Contains(tex);
    }






}
