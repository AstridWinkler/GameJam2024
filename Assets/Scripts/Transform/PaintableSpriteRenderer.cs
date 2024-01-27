using logiked.source.attributes;
using logiked.source.extentions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintableSpriteRenderer : MonoBehaviour
{
    [SerializeField]
    bool automaticRotation = true;
    [ShowIf(nameof(automaticRotation), "==", true)]
    [SerializeField]
    bool destroyAndCloneCollider = true;

    SpriteRenderer sp;

    T CopyComponent<T>(T original, GameObject destination) where T : Component
    {
        System.Type type = original.GetType();
        var dst = destination.GetComponent(type) as T;
        if (!dst) dst = destination.AddComponent(type) as T;
        var fields = type.GetFields();
        foreach (var field in fields)
        {
            if (field.IsStatic) continue;
            field.SetValue(dst, field.GetValue(original));
        }
        var props = type.GetProperties();
        foreach (var prop in props)
        {
            if (!prop.CanWrite || !prop.CanWrite || prop.Name == "name") continue;
            prop.SetValue(dst, prop.GetValue(original, null), null);
        }
        return dst as T;
    }

    List<Transform> clonedColliders = new List<Transform>();

    void CloneCollider(Collider2D col)
    {
        GameObject clone = new GameObject("Collider");
        clone.transform.SetPositionAndRotation(col.transform.position, col.transform.rotation);
        CopyComponent(col, clone);
        clonedColliders.Add(clone.transform);
        Destroy(col);
    }
    void SaveOcclusion(Collider2D col)
    {

    }



        void Start()
    {
        sp = GetComponentInChildren<SpriteRenderer>();
        GameManager.Resources.AddPaintableSpriteRenderer(sp);

        if (automaticRotation && transform.eulerAngles.z != 0)
        {
            Collider2D[] cols = GetComponentsInChildren<Collider2D>();


            if (cols.Length > 0)
            {
                if (transform.childCount > 0)
                {
                    foreach (var c in cols)
                    {
                        if (c.transform != transform && c.transform.localPosition != Vector3.zero)
                            Debug.LogError($"Paintable sprite containts children {c.name} with non-zero local position. Children position risk to be lost");
                    }
                }

                if (!destroyAndCloneCollider)
                    Debug.LogError($"Paintable sprite containts colliders and automatic rotation. Please enable '{nameof(destroyAndCloneCollider)}' to mak it work correclty ");
                else
                {
                    cols.ForEach(CloneCollider);
                }
            }

            MeshRenderer[] occlusion = GetComponentsInChildren<MeshRenderer>();

            foreach(var occ in occlusion)
            {
                occ.transform.SetParent(null);
                clonedColliders.Add(occ.transform);
            }




            Texture2D texFinal = new Texture2D((int)sp.sprite.rect.height, (int)sp.sprite.rect.width, TextureFormat.ARGB32, false);
            Texture2D tex2 = sp.sprite.texture;
            int rot = Mathf.RoundToInt(Mathf.DeltaAngle(transform.eulerAngles.z, 0)/90);
            RectInt rect = ReMath.ToRectInt(sp.sprite.rect);
      

            while (rot != 0)
            {

                ApplyRotate(texFinal, tex2, rect, rot > 0);
                tex2 = texFinal;
                rect = new RectInt(0,0, tex2.width, tex2.height);
                rot -= (int)Mathf.Sign(rot);
            }

            texFinal.Apply();
             transform.eulerAngles = new Vector3(0,0,0);
            sp.sprite = Sprite.Create(texFinal, new Rect(0, 0, texFinal.width, texFinal.height), Vector2.one * 0.5f);


            foreach(var obj in clonedColliders)
            {
              obj.transform.SetParent(transform, true);
            }




            GameManager.Resources.AddTexture(texFinal);
        }
    }

    void ApplyRotate(Texture2D tex, Texture2D orig, RectInt txRect, bool clockwise)
    { 

        tex.filterMode = FilterMode.Point;
        var colors = orig.GetPixels(txRect.x, txRect.y, txRect.width, txRect.height);
        Color[] rotated = new Color[colors.Length];
       // print(colors.Length + " vs " +  txRect.width * txRect.height);

        int w = txRect.width;
        int h = txRect.height;
        int iRotated, iOriginal;


        for (int j = 0; j < h; ++j)
        {
            for (int i = 0; i < w; ++i)
            {
                iRotated = (i + 1) * h - j - 1;
                iOriginal = clockwise ? colors.Length - 1 - (j * w + i) : j * w + i;
                rotated[iRotated] = colors[iOriginal];
            }
        }

        tex.SetPixels(rotated);
    }


}
