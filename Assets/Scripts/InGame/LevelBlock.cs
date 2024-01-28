using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using static GameWorldFile;


[RequireComponent(typeof(ShimmerLevelBlock))]
[ExecuteAlways]
public class LevelBlock : MonoBehaviour
{
    public static LevelBlock instance { get; private set; }

    [SerializeField]
    private GameWorldFile linkedWorld;
  
    [BitMaskAttribute(typeof(LevelMods)), SerializeField]
    private LevelMods LevelAppliedMods;

    private ShimmerLevelBlock shimmerWorldModifier;


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


    /// <summary>
    /// Utiliser le tag "PaintMap" pour les tilemaps concernés
    /// </summary>
    public Tilemap[] MainPaintMaps { get { return mainPaintMaps; } }
    [SerializeField]
    Tilemap[] mainPaintMaps;


        [SerializeField]
    Camera mainCamera;


    private void Awake()
    {
        instance = this;

        var cams = GetComponentsInChildren<Camera>();

        mainCamera = cams.FirstOrDefault(m => m.tag == "MainCamera");

        if (mainCamera == null)
            Debug.LogError("No MainCam found");

        shimmerWorldModifier = GetComponent<ShimmerLevelBlock>();

#if UNITY_EDITOR
        spawnPoint = GameObject.FindGameObjectWithTag("SpawnPoint");
        mainPaintMaps = GameObject.FindObjectsOfType<TilemapCollider2D>().Where(m => m.tag == "PaintMap").Select(x => x.gameObject.GetComponent<Tilemap>()).ToArray();
        if (spawnPoint == null)
            Debug.LogError("Spawn point not found !" );

        if (!Application.isPlaying)
            return;
#endif
    }


}
