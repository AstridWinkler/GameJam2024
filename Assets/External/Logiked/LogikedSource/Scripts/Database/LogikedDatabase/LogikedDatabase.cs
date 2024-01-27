using logiked.source.attributes;
using logiked.source.utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


namespace logiked.source.database
{

    /// <summary>
    ///  Base de données d'éléments du jeu. (Des objets, des mobs, des personnages...) <br/>
    ///  Stoque et organise les élements. <br/>
    ///  Encadre leur creation, leur manipulation et leur accés <br/>
    ///  Le type de base des élements est <see cref="DatabaseAbstractElement"/>
    /// </summary>
    /// <typeparam name="A">Le type de base de donnée (Mettre le type de la classe actuelle).</typeparam>
    /// <typeparam name="D">Le type des descripteurs des objets de cette BDD.</typeparam>
    /// <typeparam name="C">Le type des catégorie des objets de cette BDD. Par défaut vous pouvez mettre <see cref="DatabaseCategory"/>. Vous pouvez créer vos propres catégories à partir de cette classe.</typeparam>
    public abstract class LogikedDatabase<A, D, C> : LogikedDatabase where C: DatabaseCategory where D : DatabaseAbstractElement where A : LogikedDatabase
    {

        /// <summary>
        /// [WIP] l'instance cette Bdd (dans le cadre ou c'est un singleton)
        /// </summary>
        public static A Instance { get { return instance; } }

        /// <summary>
        /// [WIP] l'instance cette Bdd (dans le cadre ou c'est un singleton)
        /// </summary>
        protected static A instance;

        /// <summary>
        /// [WIP] Cette Database est la seule de son type ? Dans ce cas on peut y acceder par DatabaseType.Instance
        /// </summary>
        [SerializeField, Tooltip("[WIP] Cette Database est la seule de son type ? Dans ce cas on peut y acceder par DatabaseType.Instance")]
        protected bool isSingletonDatabase;

        /// <summary>
        /// Label ajouté a tout les Assets crées dans la base de donnée
        /// </summary>
        [SerializeField, Tooltip("Label ajouté a tout les Assets crées dans la base de donnée")]
        protected string databaseElementsLabel="item";//Est utilisé en inspector, tkt



        /// <summary>
        /// La liste de tout les items contenu dans la BDD       
        /// </summary>
        [SerializeField] protected List<D> itemList = new List<D>();
       
        /// <summary>
        /// La liste de tout les items contenu dans la base       
        /// </summary>
        public D[] ItemList { get => itemList.Where(m => m != null).ToArray() ;  }

        /// <summary>
        /// La liste de tout les items contenu dans la base, avec le type <see cref="DatabaseAbstractElement"/>    
        /// </summary>       
        public override DatabaseAbstractElement[] ItemListBase { get => itemList.ToArray() ;  }

#if UNITY_EDITOR
        public override void SetItemList(List<DatabaseAbstractElement> list) => itemList = list.ConvertAll(m=>(D)m);
        public void SetItemList(List<D> list) => itemList = list;
#endif


        /// <summary>
        /// Liste de toute les catégories d'items disponibles
        /// </summary>      
        [SerializeField] C[] categories = new C[0];

        /// <summary>
        /// Liste de toute les catégories d'items disponibles
        /// </summary>      
        [SerializeField] protected override DatabaseCategory[] Categories { get => Array.ConvertAll(categories, c => (DatabaseCategory)c); }



        private void OnEnable()
        {
            if (isSingletonDatabase)
                instance = this as A;
        }

    }




    /// <summary>
    ///  Type racine d'une base de données d'éléments du jeu. (Des objets, des mobs, des personnages...) <br/>
    ///  Stoque et organise les élements. <br/>
    ///  Encadre leur, leur manipulation et leur accés <br/>
    /// </summary>
    public abstract class LogikedDatabase : ScriptableObject
    {

        /// <summary>
        /// La liste de tout les items contenu dans la base       
        /// </summary>        
        public abstract DatabaseAbstractElement[] ItemListBase { get; }
#if UNITY_EDITOR
        public abstract void SetItemList(List<DatabaseAbstractElement> list);
#endif

        /// <summary>
        /// Faut-il créer un dossier pour chaque nouvel élement de la Bdd ? Utile pour l'organisation des types complexes et de leurs fichiers.
        /// </summary>
        [SerializeField, Tooltip("Faut-il créer un dossier pour chaque nouvel élement de la Bdd ? Utile pour l'organisation des types complexes et de leurs fichiers.")]
        protected bool createFolderForEachElements = true;


        /// <summary>
        /// La liste de tout les tags d'items disponibles
        /// </summary>
        [SerializeField] List<string> itemTags;

        /// <inheritdoc cref="itemTags"/>
        public string[] ItemTags => itemTags.ToArray();


        /// <summary>
        /// Liste de toute les catégories d'items disponibles
        /// </summary>      
        [SerializeField] protected abstract DatabaseCategory[] Categories { get; /*set;*/ }



        // Conversion d'un ID en nom de catégorie      
        // public string ItemCategoryName(int id) => (categories.Length == 0 || id < 0 || id >= categories.Length) ? "null" : categories[id].CategoryName;


        /// <summary>
        /// Obtenir une Catégorie depuis un ID
        /// </summary>
        /// <param name="id">L'id de la catégorie à trouver</param>
        /// <returns></returns>
        public DatabaseCategory GetCategory(int id)
        {
            if (id < 0 || id >= Categories.Length) return null;
            return Categories[id];
        }




#if UNITY_EDITOR

        /// <summary>
        /// Obtenir une liste de string de toute les catégories d'items disponibles. Utile pour les EditorGuiLayout.Popup()
        /// </summary>   
        public string[] ItemCategories
        {
            get
            {
                if (itemCategories == null || itemCategories.Length != Categories.Length)
                {
                    itemCategories = Array.ConvertAll<DatabaseCategory, string>(Categories, m => m.CategoryName);
                }
                return itemCategories;
            }
        }
        [System.NonSerialized] private string[] itemCategories;

        /// <summary>
        /// [WIP] le nom du script généré par la bdd pour le référencement de ses objets
        /// </summary>
        [SerializeField][HideInNormalInspector]
        public string generatedItemScriptName = "Items";
#endif







    
    }
}

