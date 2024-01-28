using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

#if UNITY_EDITOR
using logiked.source.editor;
using UnityEditor;
#endif
public class ShimmerPit : MonoBehaviour
{

    [Tooltip("Nombre de différents puits de shimmers dans cette map")]
    [SerializeField]
    [Range(1,4)]
    int numberOfDepth = 1;

    [SerializeField]
    List<ShimmerDepthFloor> shimmerDepthList = new List<ShimmerDepthFloor>();



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }



#if UNITY_EDITOR




  public  void UpdateShimmerDepth()
    {


        var existingPits = GetComponentsInChildren<ShimmerDepthFloor>(true);
        List<ShimmerDepthFloor> end = new List<ShimmerDepthFloor>();
        int i = 0;
        for (i = 0; i < numberOfDepth; i++)
        {
            if (i < existingPits.Length)
                end.Add(existingPits[i]);
            else
                end.Add(CreateNewDepth());
        }


        i = 0;
        foreach (var e in end)
        {
            e.transform.localPosition = new Vector3(0, (1+i) * ShimmerLevelBlock.depthOffsetY, 0);
            e.name = "depth_" + i++;
            e.gameObject.SetActive(true);
            e.UpdateTilemapClone();

        }

        foreach (var e in existingPits)
            if (!end.Contains(e))
            {
                e.name = "depth_not_used";
                e.gameObject.SetActive(false);

            }



        shimmerDepthList = end;




    }

    internal void SetDepthActive(int depth)
    {

        for (int i = 0; i < numberOfDepth; i++)
        {
            shimmerDepthList[i].SetDepthActive(i == depth);
        }

    }

    ShimmerDepthFloor CreateNewDepth()
    {
        var pit = new GameObject("Depth").AddComponent<ShimmerDepthFloor>();
        pit.transform.SetParent(transform);
        pit.transform.localPosition = Vector3.zero;
        return pit;
    }







    [CustomEditor(typeof(ShimmerPit))]
    private class ShimmerPitEditor : Editor
    {
        ShimmerPit shim;
        public override void OnInspectorGUI()
        {
            shim = target as ShimmerPit;

            DrawDefaultInspector();
            if (GUILayout.Button("Update Childs Depths"))
            {
                shim.UpdateShimmerDepth();
                shim.SetDirtyNow();
                serializedObject.ApplyModifiedProperties();
            }

        }

    
    }

#endif


}
