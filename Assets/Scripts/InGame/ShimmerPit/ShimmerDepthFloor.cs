using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class ShimmerDepthFloor : MonoBehaviour
{
    public enum TilemapCloneMode { ExactSame, NotUpdqatedAnymore}

    [SerializeField]
    private Transform currentTilemapParent;
    [SerializeField]
    private Transform objectsToEnable;


    [SerializeField]
    private TilemapCloneMode clonningMode = TilemapCloneMode.ExactSame;




    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }


    internal void SetDepthActive(bool v)
    {
        if (objectsToEnable != null)
            objectsToEnable.gameObject.SetActive(v);
    }
#if UNITY_EDITOR

    public void UpdateTilemapClone()
    {
        switch (clonningMode)
        {
            case TilemapCloneMode.ExactSame: TilemapExactSameClone(); return;

        }
    

    }

    private void TilemapExactSameClone()
    {
        if (currentTilemapParent != null)
        {
            DestroyImmediate(currentTilemapParent.gameObject);
        }

        var clone = (ShimmerLevelBlock.Instance.TilemapsParent).gameObject.Inst(transform, transform);
        currentTilemapParent = clone.transform;
    }



    [CustomEditor(typeof(ShimmerDepthFloor))]
    private class ShimmerMiniWorldEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

        }
    }


#endif


}
