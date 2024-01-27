
using logiked.source.attributes;
using logiked.source.extentions;
using logiked.source.utilities;
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace logiked.Tool2D.animation
    {
    /// <summary>
    /// Un fichier d'animation avec plusieurs directions
    /// </summary>
    /// <seealso cref="logiked.LogikedAssemblySettings" />
    [CreateAssetMenu(fileName = "anim", menuName = "Logiked/2DTools/2D Animation - Multi Direction", order = 2)]


    [System.Serializable]
    public class Animation2DFileVariations : Animation2DFile
    {

        /// <summary>
        /// Nombre de directions
        /// </summary>
        public enum VariationCountModeEnum { _8DirectionsAnimation, _4DirectionsAnimation, CustomVariationCount }

        [SerializeField]
        private VariationCountModeEnum variationCountMode;
        public VariationCountModeEnum DirectionCountMode
        {
            get => variationCountMode;
#if UNITY_EDITOR
            set => variationCountMode = value;
#endif
        }

        /*
        [NonSerialized]
        public int currentDirection = 0;
        */

        public override Sprite[] Sprites
        {
            get
            {
                return GetSprites(0);
            }
#if UNITY_EDITOR
            set { Debug.LogWarning("To set sprites on a variative textrure, use SetSprites() instead"); }
#endif
        }


        /// <summary>
        /// Nombre de direction customisé
        /// </summary>
        [SerializeField] [ShowIf(nameof(variationCountMode), ShowIfOperations.Equal, VariationCountModeEnum.CustomVariationCount)]
        [Min(1)]
        int customVariationCount = 8;
        public int CustomDirectionCount
        {
            get => customVariationCount;
#if UNITY_EDITOR
            set => customVariationCount = value;
#endif
        }


        [SerializeField]
        SpriteDirectionBundle[] spritesVariations = new SpriteDirectionBundle[0];
        public SpriteDirectionBundle[] SpritesVariations => spritesVariations;
    
        public int VariationCount
        {
            get
            {
                switch (variationCountMode)
                {
                    case VariationCountModeEnum.CustomVariationCount: return customVariationCount;
                    case VariationCountModeEnum._4DirectionsAnimation: return 4;
                    case VariationCountModeEnum._8DirectionsAnimation: return 8;
                }
                return 1;
            }
        }


#if UNITY_EDITOR

        /// <summary>
        /// Assigner les sprites dans toutes les directions
        /// </summary>
        public void SetSprites(params Sprite[][] sprites)
        {
            Assert.IsNotNull(sprites, "Sprites are null");

            if (sprites.Length != VariationCount)
                Debug.LogError($"Sprites groups count is not marching the number of direction : {sprites.Length} vs {VariationCount}");

            spritesVariations = new SpriteDirectionBundle[sprites.Length];

            for (int i = 0; i < sprites.Length; i++)
            {
                spritesVariations[i] = new SpriteDirectionBundle();
                spritesVariations[i].Sprites = sprites[i] ?? new Sprite[0];
            }

        }
#endif


        /// <summary>
        /// Classe pour visualiser une liste de sprites
        /// </summary>
        [Serializable]
        public class SpriteDirectionBundle
        {
            [SerializeField]
            private Sprite[] sprites = new Sprite[0];
            public Sprite[] Sprites
            {
                get => sprites;
#if UNITY_EDITOR
                set => sprites = value;
#endif
            }
        }

        public override Sprite[] GetSprites(int variationId)
        {
            if (spritesVariations.Length == 0) return new Sprite[0];
            return spritesVariations.GetCyclic(variationId).Sprites ?? new Sprite[0];
        }





    }
}
