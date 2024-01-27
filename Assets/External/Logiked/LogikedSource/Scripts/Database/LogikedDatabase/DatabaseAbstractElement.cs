using logiked.source.attributes;
using logiked.source.database;
using logiked.source.types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace logiked.source.database
{
    /// <summary>
    /// Type de base pour un élément de la BDD. <see cref="LogikedDatabase{A, D, C}"/>
    /// Override ce type pour pouvoir creer des base de données personnalisés
    /// </summary>
    public abstract class DatabaseAbstractElement : ScriptableObject
    {



        [FieldSectionBegin("Base datas")]
        [FieldSize(0.5f)]
        [DatabaseTagNumber]
        [SerializeField, Tooltip("Les tags associés à cet objet par rapport à sa Bdd (Stockés sur les bits de l'entier, en binaire)")]
        protected int tags;
#if UNITY_EDITOR
        public virtual int ItemTags { get => tags; set => tags = value; }
#else
        public virtual int ItemTags { get { return tags; } }
#endif

        [SerializeField, Tooltip("La catégorie de cet élement dans la base de données.")]
        [FieldSize(0.5f)]
        [DatabaseCategoryNumberAttribue]
        protected int categoryId;
#if UNITY_EDITOR 
        public virtual int CategoryId { get => categoryId; set => categoryId = value; }
#else
        public virtual int CategoryId { get { return categoryId; } }
#endif


        [FieldSize(0.5f)]
        [SerializeField, GreyedField]
        private LogikedDatabase database;

        /// <summary>
        /// La base de donnée associé a cet élement
        /// </summary>
        public LogikedDatabase Database
        {
            get { return database; }
            set { database = value; }
        }



        /// <summary>
        /// Nom de l'élément. Virtual pour pouvoir changer son nom en Lstring
        /// </summary>  
        public abstract string ItemName { get ; }


  

        [FieldSectionColumn]
        [DrawSpriteBox( size:60)]
        [SerializeField]
        [FieldSectionEnd]

        //[HideInNormalInspector]
        private Sprite sprite;
        /// <summary>
        /// Le sprite associé a cet élement
        /// </summary>
        public Sprite Sprite { get => sprite; }





#if UNITY_EDITOR
        /// <summary>
        /// L'objet est-t il actuellement édité par la database ?
        /// </summary>
        [SerializeField]
        [HideInInspector]
        public bool IsEditedByDatabase
        {
            get => isEditedByDatabase;
            set { isEditedByDatabase = value; UnityEditor.EditorUtility.SetDirty(this); }
        }
        private bool isEditedByDatabase;
#endif




    }
}