#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using logiked.source.extentions;
using System.Text;
using System.Text.RegularExpressions;
using logiked.source.attributes;
using static HacheurEditor;
using System.Linq;
using logiked.Tool2D.settings;

namespace logiked.Tool2D.editor {


    /// <summary>
    /// Classe qui se serialise dans les Meta des textures via <see cref="AssetImporter.userData"/> <br></br>
    /// Utilisés par la fenetre <see cref="HacheurEditor"/>
    /// </summary>
  [Serializable]
    public class TextureMetadataExtension
    {


        /// <summary>
        /// ID du presset pour les settings d'import. '' = Default. 0 = Pas de presset. 
        /// </summary>
        public string ImportPressedGuid { get => importPressedGuid; set => importPressedGuid = value; }
        [SerializeField]
        private string importPressedGuid = "";


        /// <summary>
        /// Utiliser un nombre de sprites plutot qu'une taille en pixels
        /// </summary>
        public bool Cut_useCountInsteadPixels { get => cut_useCountInsteadPixels; set => cut_useCountInsteadPixels = value; }
        [SerializeField]
        private bool cut_useCountInsteadPixels = false;


        public bool Cut_keepEmptySprites { get => cut_keepEmptySprites; set => cut_keepEmptySprites = value; }
        [SerializeField]
        private bool cut_keepEmptySprites = false;

        public int Cut_columnCount { get => cut_columnCount.Clamp(1, int.MaxValue); set => cut_columnCount = value.Clamp(1, int.MaxValue); }
        [SerializeField]
        private int cut_columnCount = 1;

        
        /// <summary>
        /// Valeur du Cut (pixels ou nombre de sprites) en X
        /// </summary>
        public int Cut_ValueX { get => cut_ValueX.Clamp(1, 9999); set => cut_ValueX = value.Clamp(1, 9999); }
        [SerializeField]
        private int cut_ValueX = 0;

        /// <summary>
        /// Valeur du Cut (pixels ou nombre de sprites) en Y
        /// </summary>
        public int Cut_ValueY { get => cut_ValueY.Clamp(1, 9999); set => cut_ValueY = value.Clamp(1, 9999); }
        [SerializeField]
        private int cut_ValueY = 0;


        public float Cut_PivotX { get => cut_pivotX.Clamp01(); set => cut_pivotX = value.Clamp01(); }
        [SerializeField]
        private float cut_pivotX = 0.5f;

        
        public float Cut_PivotY { get => cut_pivotY.Clamp01(); set => cut_pivotY = value.Clamp01(); }
        [SerializeField]
        private float cut_pivotY = 0.5f;


        /// <summary>
        /// GUID du dosser d'enregistrement des textures
        /// </summary>
        public string Anim_folderId { get => anim_folderId; set => anim_folderId = value; }
        [SerializeField]
        private string anim_folderId;




        /// <summary>
        /// Liste des GUID des animations crées avec cette texture
        /// </summary>
        public List<Meta2DAnimation> Anim_createdWithThisTexture { get => anim_createdWithThisTexture; }
        
        [SerializeField]
        private List<Meta2DAnimation> anim_createdWithThisTexture = new List<Meta2DAnimation>();



        /// <summary>
        /// Nombre de lignes entre chaque direction de l'animation (pour les animation multi-directionnelles)
        /// </summary>
        public int MultiDirectionnalLineOffset { get => multiDirectionnalLineOffset; set => multiDirectionnalLineOffset = value.Clamp(1, 99); }
        [SerializeField]
        private int multiDirectionnalLineOffset = 1;




        void UpdateCutterDictionary()
        {
                if (anim_dic_createdWithThisTexture == null) anim_dic_createdWithThisTexture = new Dictionary<string, Meta2DAnimation>();

            anim_dic_createdWithThisTexture.Clear();

            anim_createdWithThisTexture.RemoveAll(m => m == null || AssetDatabase.GUIDToAssetPath(m.AnimationGuid) == null);//On supprimer tout les fichiers morts
            anim_createdWithThisTexture = anim_createdWithThisTexture.Distinct(m => m.AnimationGuid);
           

            for (int i = 0; i < anim_createdWithThisTexture.Count; i++)
            {
                anim_dic_createdWithThisTexture.Add(anim_createdWithThisTexture[i].AnimationGuid, anim_createdWithThisTexture[i]);
            }
        }


        [NonSerialized]
        private Dictionary<string, Meta2DAnimation> anim_dic_createdWithThisTexture = new Dictionary<string, Meta2DAnimation>();

        /// <summary>
        /// Retourne les infos de génération de l'animation si elle à été génrée par cette texture
        /// </summary>
        public Meta2DAnimation TryGetAnimationCreationInforation(string GUID)
        {
            if(anim_dic_createdWithThisTexture == null || anim_dic_createdWithThisTexture?.Count != anim_createdWithThisTexture?.Count)
            UpdateCutterDictionary();//Not optimised

            return anim_dic_createdWithThisTexture.GetValueOrDefault(GUID);
        }
        public void AddNewAnimationInformation(Meta2DAnimation infos)
        {
            if (anim_createdWithThisTexture == null) anim_createdWithThisTexture = new List<Meta2DAnimation>();

            anim_createdWithThisTexture.Add(infos);
            UpdateCutterDictionary();
        }





        [Serializable]
        public class Meta2DAnimation
        {
            [SerializeField]
            public string AnimationGuid;

            [SerializeField]
            public AnimationGenerationMode spriteSource;

            [SerializeField]
            public int lineStartIndex;
            [SerializeField]
            public int lineCount;
         

        }



        private const string header = "<logiked>";
        private const string headerClose = "</logiked>";

        [NonSerialized]
        public TextureImporter linkedImporter;


        public TextureMetadataExtension(TextureImporter importer)
        {
            linkedImporter = importer;
            cut_pivotX = 0.5f;
            cut_pivotY = 0.5f;
            cut_ValueX = LogikedPlugin_2DTools.Instance.DefaultSpriteCutSize;
            cut_ValueY = LogikedPlugin_2DTools.Instance.DefaultSpriteCutSize;

            //UpdateCutterDictionary();
        }


        public void Serialize()
        {
            if(linkedImporter == null)
            {
                Debug.LogError("null importer");
                return;
            }

            var s = this.SerializeToByteArray();                 
            string encodedText = $"{header}{Convert.ToBase64String(s)}{headerClose}";

            linkedImporter.userData = encodedText;  
            //UpdateCutterDictionary();
        }




        public static TextureMetadataExtension DeserializeFromImporter(TextureImporter importer, bool createIfNotExist = true)
        {
            TextureMetadataExtension NullReturn() => createIfNotExist ? new TextureMetadataExtension(importer) : null;

            if (importer.userData.IsNullOrEmpty()) return NullReturn();
            var reg = Regex.Match(importer.userData, $@"{header}(.*){headerClose}");



            if (!reg.Success) return NullReturn();

            try
            {//Corrupt meta datas
                byte[] decodedBytes = Convert.FromBase64String(reg.Groups[1].Value);
                TextureMetadataExtension end = (TextureMetadataExtension)decodedBytes.DeserializeToObject();
                end.linkedImporter = importer;

                if (end.anim_createdWithThisTexture == null) end.anim_createdWithThisTexture = new List<Meta2DAnimation>();

                //end.UpdateCutterDictionary();
                return end;
            }
            catch (Exception)
            {
                importer.userData = "";
                return NullReturn();
            }

        }



    }
}


#endif