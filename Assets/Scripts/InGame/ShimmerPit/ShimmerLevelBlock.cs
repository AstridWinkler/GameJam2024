using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FunkyCode;


#if UNITY_EDITOR
using logiked.source.editor;
using UnityEditor;
#endif


[ExecuteAlways]
public class ShimmerLevelBlock : MonoBehaviour
{

    public static ShimmerLevelBlock Instance { get; private set; }

    /// <summary>
    /// Déplacement en X de chaque mini-niveau de depth
    /// </summary>
    public const float depthOffsetX = 100;
    /// <summary>
    /// Déplacement en Y de chaque mini-niveau de depth
    /// </summary>
    public const float depthOffsetY = -100;

    public Transform TilemapsParent { get => tilemapsParent; }
    [SerializeField]
    Transform tilemapsParent;


    [SerializeField]
    Transform lightAndOtherSettings;



    [Tooltip("Nombre de différents puits de shimmers dans cette map")]
    [SerializeField]
    [Range(1, 10)]
    int numberOfShimmerPit = 1;

    [SerializeField]
    Transform shimmerPitParents;
    [SerializeField]
    List<ShimmerPit> shimerPitList = new List<ShimmerPit>();



#if UNITY_EDITOR
    [InitializeOnLoadMethod]
   static void AwakeReload()
    {
      Instance = FindObjectOfType<ShimmerLevelBlock>();
    }
#endif

    void Awake()
    {
        Instance = this;
        SetShimmerDepth(0);
        
        }

    void Update()
    {

    }

    public int currentPit = 0;

    public void SetShimmerDepth(int depth)
    {
        LightingManager2D.Instance.Reinit();

        if (lightAndOtherSettings != null)
            lightAndOtherSettings.gameObject.SetActive(depth == 0);


        for (int i = 0; i < numberOfShimmerPit; i++)
        {
            shimerPitList[i].SetDepthActive(depth - 1);
        }



    }



#if UNITY_EDITOR

    public void UpdateChildDepth()
    {
        if (shimmerPitParents == null)
        {
            shimmerPitParents = new GameObject("ShimmerPits").transform;
            shimmerPitParents.SetParent(transform);
            transform.localPosition = Vector3.zero;
        }


        if (shimmerPitParents != null)
        {

            var existingPits = shimmerPitParents.GetComponentsInChildren<ShimmerPit>(true);
            List<ShimmerPit> end = new List<ShimmerPit>();
            int i = 0;
            for (i = 0; i < numberOfShimmerPit; i++)
            {
                if (i < existingPits.Length)
                    end.Add(existingPits[i]);
                else
                    end.Add(CreateNewPit());
            }


            i = 0;
            foreach (var e in end)
            {
                e.transform.position = new Vector3(i * depthOffsetX, 0, 0);
                e.name = "pit_" + i++;
                e.gameObject.SetActive(true);
                e.UpdateShimmerDepth();
            }


            foreach (var e in existingPits)
                if (!end.Contains(e))
                {
                    e.name = "pit_not_used";
                    e.gameObject.SetActive(false);
                }




            shimerPitList = end;

        }


    }

    ShimmerPit CreateNewPit()
    {
        var pit = new GameObject("Pit").AddComponent<ShimmerPit>();
        pit.transform.SetParent(shimmerPitParents);
        pit.transform.localPosition = Vector3.zero;
        return pit;
    }





    [CustomEditor(typeof(ShimmerLevelBlock))]
    private class ShimmerLevelBlockEditor : Editor
    {
        ShimmerLevelBlock t;
        public override void OnInspectorGUI()
        {
            t = target as ShimmerLevelBlock;

            DrawDefaultInspector();
            if (GUILayout.Button("Update Childs Pits"))
            {
                t.UpdateChildDepth();
                t.SetDirtyNow();
                serializedObject.ApplyModifiedProperties();
            }

        }


    }

#endif

}
