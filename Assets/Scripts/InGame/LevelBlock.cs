using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static GameWorldFile;

[ExecuteAlways]
public class LevelBlock : MonoBehaviour
{
    public static LevelBlock instance { get; private set; }

    [SerializeField]
    private GameWorldFile linkedWorld;
  
    [BitMaskAttribute(typeof(LevelMods)), SerializeField]
    private LevelMods LevelAppliedMods;

    public GameWorldFile LinkedWorld
    {
        get { return linkedWorld; }
#if UNITY_EDITOR
        set { linkedWorld = value; }
#endif
    }


    public Transform SpawnPoint
    {
        get { if (spawnPoint) return spawnPoint.transform; else Debug.LogError("spawn point is null !"); return null; }
    }

    [SerializeField]
    GameObject spawnPoint;

    public Camera MainCamera {get{return mainCamera; }}

    public Tilemap MainPaintMap { get { return mainPaintMap; } }
    [SerializeField]
    Tilemap mainPaintMap;


        [SerializeField]
    Camera mainCamera;


    private void Awake()
    {
        instance = this;
        mainCamera = GetComponentInChildren<Camera>();

#if UNITY_EDITOR
        spawnPoint = GameObject.FindGameObjectWithTag("SpawnPoint");
        mainPaintMap = GameObject.FindObjectOfType<TilemapCollider2D>().GetComponent<Tilemap>() ;
        if (spawnPoint == null)
            Debug.LogError("Spawn point not found !" );

        if (!Application.isPlaying)
            return;
#endif
    }


}
