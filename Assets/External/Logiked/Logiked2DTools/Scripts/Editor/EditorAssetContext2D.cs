#if UNITY_EDITOR
using logiked.source.editor;
using logiked.Tool2D.editor;
using logiked.Tool2D.settings;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class CompressTexture 
{


    const string MenuItem_ApplyHacheurImportSettings = LogikedPlugin_2DTools.MenuItemName + "Apply Hacheur Import settings";
    const string MenuItem_RefreshAnimations = LogikedPlugin_2DTools.MenuItemName + "Cut And Refresh animations";


    [MenuItem(MenuItem_ApplyHacheurImportSettings, priority = LogikedProjectConfig.LogikedMenuItemPriority + 1)]
    static void ApplyImportSettings()
    {
        var objs = Selection.objects;

        foreach (var obj in objs)
        {

            if (!(obj is Texture2D))
                continue;

            string path = AssetDatabase.GetAssetPath(obj);

            TextureImporter importer = (TextureImporter)TextureImporter.GetAtPath(path);

            TextureMetadataExtension metas = TextureMetadataExtension.DeserializeFromImporter(importer, true);

            HacheurEditor.AutoFormat(importer, metas, true);

        }
    }


    [MenuItem(MenuItem_RefreshAnimations, priority = LogikedProjectConfig.LogikedMenuItemPriority + 20)]
    static void CutRefreshAnims()
    {
        var objs = Selection.objects;

        foreach (var obj in objs)
        {

            if (!(obj is Texture2D))
                continue;

            Texture2D tex = obj as Texture2D;

            AssetDatabase.Refresh();

            TextureImporter importer = (TextureImporter)TextureImporter.GetAtPath(tex.GetAssetPath(Logiked_AssetsExtention.PathFormat.AssetRelative));

            TextureMetadataExtension metas = TextureMetadataExtension.DeserializeFromImporter(importer, false);

            if (metas == null) continue;

            HacheurEditor.AutoFormat(importer, metas, true);

            HacheurEditor.CutAndUpdateAnimationsSprites(tex, metas, importer);

            AssetDatabase.SaveAssets();

        }

    }


    [MenuItem(MenuItem_RefreshAnimations, true)]
    [MenuItem(MenuItem_ApplyHacheurImportSettings, true)]
    static bool TextureValidation()
    {
        if (Selection.objects.FirstOrDefault(m => m is Texture2D) != null) return true;
        return false;
    }

}




#endif