using logiked.source.extentions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class PaintSprite : MonoBehaviour
{


    Tilemap[] tileMap;
    SpriteRenderer rd;

    Rect realPaintRect;

    [SerializeField]
    bool destroyAfter = true;

    [SerializeField]
    bool onAwake = true;

    [SerializeField]
    bool useSpriteMaskPainting = true;

    [SerializeField]
    SpriteMask revealObject;
    [SerializeField]
    Transform maskParent;
    //   List<Tuple<Vector3Int, Tilemap, Rect>> tileTaskInfo;
    //   List<Tuple<Vector3Int, SpriteRenderer, Rect>> spriteTaskInfo;

    Stack<IEnumerator> pixelPaintTasks;
    List<Action> finishedTilesAction;

    private bool isDrawing { get; set; }

    private List<GameObject> maskList;
    private Transform tempPaint;
    private int maskLayerOrder;
    private int maskLayerId;




    #region Static Members
    //System of queue in order to recuce lags
    private static Queue<PaintSprite> PaintQueue = new Queue<PaintSprite>();
    private static PaintSprite currentPaintSprite;
    private static int generationCnt = 1;

    public static void PaintUpdate()//Call by resourcesHelperManager
    {
        if (currentPaintSprite == null && PaintQueue.Count > 0)
        {
            currentPaintSprite = PaintQueue.Dequeue();
            if(currentPaintSprite == null)            
                return;

            
          //  currentPaintSprite.PrepareAllTasks();
            currentPaintSprite.StartCoroutine(currentPaintSprite.DrawAllTasks());
        }
        else
        {
            if (currentPaintSprite != null && !currentPaintSprite.isDrawing)
                currentPaintSprite = null;
        }

    }

    #endregion




    void Awake()
    {
        transform.position += new Vector3(0, 0, -0.001f);
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, 0);
        maskList = new List<GameObject>();
        pixelPaintTasks = new Stack<IEnumerator>();
        //tileMap = GameObject.FindObjectsOfType<Tilemap>();
        tileMap = new Tilemap[] { GameManager.Gameplay.MainPaintMap };
        rd = GetComponent<SpriteRenderer>();
        finishedTilesAction = new List<Action>();
        isDrawing = false;

        if (rd.sprite == null)
        {
            StartCoroutine(DrawAllTasks());//lst = 0. Remove
            return;
        }


    }





    private void Start()
    {
        if (!onAwake)
            RegisterPaintSprite();
    }
   // bool started;


    void RegisterPaintSprite()
    {
        //      if (started)
        //           return;
        //       started = true;


        maskLayerOrder = generationCnt += 2;

        if (useSpriteMaskPainting)
        {
            maskLayerOrder = rd.sortingLayerID;
            rd.sortingOrder = maskLayerOrder;            
            rd.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
            tempPaint = new GameObject("temp_paint_" + maskLayerOrder).transform;
            if (maskParent == null)
                transform.SetParent(tempPaint);
            else
                tempPaint.SetParent(maskParent);
            if (onAwake)
                RegisterPaintSprite();
        }


        CollectSprites();
        PaintQueue.Enqueue(this);

    }






    IEnumerator DrawAllTasks()
    {

        isDrawing = true;

        const double minimumFrameSpeed = 1f/30f;//30 fps sans prendre en compte le rendu
     
        
        var start = Time.realtimeSinceStartupAsDouble;

        while (pixelPaintTasks.Count > 0)
        {
            StartCoroutine(pixelPaintTasks.Pop());

            if (Time.realtimeSinceStartupAsDouble - start > minimumFrameSpeed)//Taking too long
            {
                yield return null;//on attends une frame
                start = Time.realtimeSinceStartupAsDouble;//on met à jour le décompte
            }

        }



        //print(tilePaintTodo.Count);
        isDrawing = false;

        foreach (var c in finishedTilesAction)
            c.Invoke();


        if (destroyAfter)
        {
            Destroy(gameObject);

            if (useSpriteMaskPainting)
                Destroy(tempPaint.gameObject);
            foreach (var a in maskList)
                Destroy(a);
        }

        yield break;
    }



    public void CollectSprites()
    {





        //Rect du spriteRenderer
        realPaintRect = rd.GetWorldRect();

        //ReMath.DrawWoldRect(realPaintRect, Color.red, 10f);


        int x, y, z;
        Vector3Int currentPos;
        Sprite sp;
        Rect r = new Rect();

        for (x = (int)Mathf.Floor(realPaintRect.xMin); x < (int)Mathf.Ceil(realPaintRect.xMax); x++)
        {
            for (y = (int)Mathf.Floor(realPaintRect.yMin); y < (int)Mathf.Ceil(realPaintRect.yMax); y++)
            {
                currentPos = new Vector3Int(x, y, 0);


                foreach (SpriteRenderer rd in GameManager.Resources.GetPaintableAtCoord(new Vector2Int(x, y)))
                {
                    if (rd != null)
                    {
                        DrawOnSpriteCoroutine(currentPos, rd);
                        if (useSpriteMaskPainting)
                            PaintMask_Sprite(rd);
                    }
                }
                //       spriteTaskInfo.Add(new Tuple<Vector3Int, SpriteRenderer, Rect>(currentPos, rd, rd.GetWorldRect()));



                for (z = 0; z < tileMap.Length; z++)
                {
                    if (tileMap[z] == null)
                        continue;
                    sp = tileMap[z].GetSprite(currentPos);

                    if (sp != null && sp.texture != null)
                    {
                        DrawOnTileMapCoroutine(currentPos, tileMap[z], r = new Rect((Vector3)currentPos, tileMap[z].cellSize));

                        if (useSpriteMaskPainting)
                            PaintMask_TileMap(currentPos, tileMap[z], r);
                    }
                    //      tileTaskInfo.Add(new Tuple<Vector3Int, Tilemap, Rect>(currentPos, tileMap[z], new Rect((Vector3)currentPos, tileMap[z].cellSize) ));
                }
            }
        }

        DrawAnimation();

    //    foreach (var tileInf in tileTaskInfo)
    //       PaintMask_TileMap(tileInf);

    }




    void PaintMask_TileMap(Vector3Int coord, Tilemap map, Rect realWorldRect)
    {
        Sprite sp = map.GetSprite(coord);
        GameObject maskObj = new GameObject("mask");
        maskObj.transform.SetParent(tempPaint);
        maskObj.transform.position = ((Vector3)coord) + (Vector3)Vector2.one * 0.5f;

        SpriteMask mask = maskObj.AddComponent<SpriteMask>();
        mask.sprite = sp;
        mask.isCustomRangeActive = true;
        mask.frontSortingLayerID = maskLayerOrder;
        mask.frontSortingOrder = maskLayerOrder+1;
     
        mask.backSortingLayerID = maskLayerOrder;
        mask.backSortingOrder = maskLayerOrder-1;
    }


    void DrawAnimation()
    {
        if (revealObject == null)
            return;

        revealObject.isCustomRangeActive = true;

        revealObject.frontSortingLayerID = maskLayerOrder;
        revealObject.frontSortingOrder = maskLayerOrder + 1;

        revealObject.backSortingLayerID = maskLayerOrder;
        revealObject.backSortingOrder = maskLayerOrder - 1;
        revealObject.gameObject.SetActive(true);

    }





    void DrawOnTileMapCoroutine(Vector3Int coord, Tilemap map, Rect realWorldRect)
    {

        pixelPaintTasks.Push(ApplySprite(realWorldRect, coord, null, map, (spEnd) =>
        {
            if (spEnd == null)
                return;
            finishedTilesAction.Add(() =>
            {
                //  GameManager.Resources.AddTexture(spEnd.texture, sp.texture);
                //SetTile(map, coord, spEnd);


            }
            );
        }));
        //  Debug.LogError("ok draw "+coord);

    }

    void SetTile(Tilemap tilemap, Vector3Int pos, Sprite sp)
    {
        Tile tile = ScriptableObject.CreateInstance<Tile>();
        
        var t = tilemap.GetTile(pos);

        if (t != null) {
            if(t is Tile)
            tile.colliderType = ((Tile)t).colliderType;
            else if(t is RuleTile)
            tile.colliderType = ((RuleTile)t).m_DefaultColliderType;



        }

        //  tile.colliderType = (sp.GetPhysicsShapeCount() > 0)? Tile.ColliderType.Sprite: Tile.ColliderType.None;

        tile.sprite = sp;
        tilemap.SetTile(pos, tile);
        
        tilemap.RefreshTile(pos);
        //Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.one * 0.5f, pixelPerUnit);
    }




    void PaintMask_Sprite( SpriteRenderer rd)
    {
        Sprite sp = rd.sprite;
        GameObject maskObj = new GameObject("maskSprite");
        maskList.Add(maskObj);
        maskObj.transform.SetParent(rd.transform);
        maskObj.transform.position = rd.transform.position;

        SpriteMask mask = maskObj.AddComponent<SpriteMask>();
        mask.sprite = sp;
        mask.isCustomRangeActive = true;

        mask.frontSortingLayerID = maskLayerOrder;
        mask.frontSortingOrder = maskLayerOrder + 1;

        mask.backSortingLayerID = maskLayerOrder;
        mask.backSortingOrder = maskLayerOrder - 1;
    }


    void DrawOnSpriteCoroutine(Vector3Int coord, SpriteRenderer spRd)
    {

        pixelPaintTasks.Push(ApplySprite(new Rect(), coord, spRd, null, spEnd =>
     {
         if (spEnd == null)
             return;
         //GameManager.Resources.AddTexture(spEnd.texture, spRd.sprite.texture);
         //finishedTilesAction.Add(() => spRd.sprite = spEnd);
     }));
    }






    IEnumerator ApplySprite(Rect realTileRect, Vector3Int coord, SpriteRenderer mapRd = null, Tilemap map = null, Action<Sprite> spFunc = null)
    {

        int x, y;
        bool textureExist;
        Color[] orig;
        Sprite sp = null;

        Vector2 posRd = Vector2.zero;



        if (map != null)
        {
            sp = map.GetSprite(coord);
        } else if (mapRd != null)
        {
            sp = mapRd.sprite;
        }


        if (sp == null || sp.texture == null)
            yield break;


        if (mapRd != null)
        {
            posRd = mapRd.transform.position;
            realTileRect = mapRd.GetWorldRect();
        }



        //ReMath.DrawWoldRect(realTileRect, Color.red, 10f);



        //Intersection des deux rectangles (objet a peindre, objet peind)
        Rect realIntersectRect = ReMath.GetIntersectRect(realTileRect, realPaintRect);



    


        //Si il n'y a pas intersection entre les Rects
        if (realIntersectRect.width == 0 || realIntersectRect.height == 0)
            yield break;

        //print("intersect :" + realIntersectRect);
       // ReMath.DrawWoldRect(realIntersectRect, Color.green, 10f);



        //Clone tile texture :

        RectInt txRect = new RectInt(ReMath.ToVec2Int(sp.rect.position), ReMath.ToVec2Int(sp.rect.size));
        Texture2D tex;
        textureExist = GameManager.Resources.ContainTexture(sp.texture);

        //  print(sp.name + " " + textureExist);

        if (textureExist)
            tex = sp.texture;
        else
        {
            tex = new Texture2D(txRect.width, txRect.height, TextureFormat.ARGB32, false);
            tex.filterMode = FilterMode.Point;
            orig = sp.texture.GetPixels(txRect.x, txRect.y, txRect.width, txRect.height);
            tex.SetPixels(orig);
        
            tex.Apply();
            GameManager.Resources.AddTexture(tex, null);
            List<Vector2> ph = new List<Vector2>();
            sp = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.one * 0.5f, sp.pixelsPerUnit, 1, SpriteMeshType.FullRect, sp.border, sp.GetPhysicsShapeCount()>0 );
            if (map != null )
            {
                SetTile(map, coord, sp);
            }else if(mapRd != null)
            {    
                mapRd.sprite = sp;
            }
        }



        //Paint sprite texture :

  
        var toPaint = rd.GrabTextureByWorldRect(realIntersectRect);



        if (toPaint.width == 0 || toPaint.height == 0)
        {
            //?? wtf
            spFunc.Invoke(Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.one * 0.5f, sp.pixelsPerUnit));
            yield break;
        }





        //Calcul de la zone d'ecriteur du sprite sur la tile


        RectInt tilePixelIntersect = default(RectInt);

        Vector2 cellWorldMin = (realIntersectRect.min - (Vector2)((Vector3)coord));
        Vector2 cellWorldMax = (realIntersectRect.max - (Vector2)((Vector3)coord));

        if (mapRd == null)
        {
            Vector2Int cellPixelMin = ReMath.ToVec2Int(new Vector2(cellWorldMin.x * tex.width / realTileRect.width, cellWorldMin.y * tex.height / realTileRect.height));
            Vector2Int cellPixelMax = ReMath.ToVec2Int(new Vector2(cellWorldMax.x * tex.width / realTileRect.width, cellWorldMax.y * tex.height / realTileRect.height));

            tilePixelIntersect = ReMath.RectIntByCoord(cellPixelMin, cellPixelMax);
        }
        else
        {
            tilePixelIntersect = mapRd.WorldToTextureRect(realIntersectRect);
        }






        int roundX, roundY;
        Color colLastPaint, colToPaint;
        //  print(tilePixelIntersect);
        //  print(tilePixelIntersect.y);
        //   ReMath.DrawWoldRect(ReMath.ToRect(tilePixelIntersect), Color.red, 5f);

        if (mapRd != null && posRd != (Vector2)mapRd.transform.position)//if object move
        {

            spFunc.Invoke(Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.one * 0.5f, sp.pixelsPerUnit));
            yield break;
        }


        orig = sp.texture.GetPixels(tilePixelIntersect.x, tilePixelIntersect.y - tilePixelIntersect.height, tilePixelIntersect.width, tilePixelIntersect.height);




        for (y = 0; y < tilePixelIntersect.height; y++)
        {
            for (x = 0; x < tilePixelIntersect.width; x++)
            {

                colLastPaint = orig[x + (tilePixelIntersect.height - 1 - y) * tilePixelIntersect.width];

                if (colLastPaint.a < 0.1f)
                    continue;



                roundX = Mathf.RoundToInt(Mathf.Lerp(0, toPaint.width - 1, (float)x / (float)tilePixelIntersect.width));
                roundY = (toPaint.height - 1) - Mathf.RoundToInt(Mathf.Lerp(0, toPaint.height - 1, (float)y / (float)tilePixelIntersect.height));


                colToPaint = toPaint.colors[roundX.Clamp(0, toPaint.width-1) , roundY.Clamp(0, toPaint.height - 1)];


                if (colToPaint.a < 0.1f)
                    continue;


                tex.SetPixel(
                    tilePixelIntersect.xMin + x,
                    tilePixelIntersect.yMin - 1 - y,
                    Color.Lerp(colLastPaint, colToPaint, colToPaint.a) * new Color(1, 1, 1, 0) + new Color(0, 0, 0, colLastPaint.a));

                /*
				tex.Apply();
				tile.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.one * 0.5f, sp2.pixelsPerUnit);
				tileMap.SetTile(coord, tile);*/

            }
        }




        tex.Apply();

        //	tex.Compress(true);
        if (map != null && !textureExist)
            map.RefreshTile(coord);
        else
            spFunc.Invoke(Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.one * 0.5f, sp.pixelsPerUnit));
        //ApplyTileTex(coord, tex, sp2.pixelsPerUnit, map);
    }





}
