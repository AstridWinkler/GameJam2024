
#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using Object = UnityEngine.Object;

namespace logiked.source.editor {


    [Obsolete("Needs revision")]
    /// <summary>
    /// Type de base des fenetres d'édition d'Asset
    /// </summary>
    public abstract class Logiked_EditorWindow : EditorWindow
    {

        /// <summary>
        /// The prefix ID of the label used for Assets generation with this Window.
        /// Example : if this window generate Texture files, you can use "tex_"...  
        /// </summary>
        protected abstract string DatasLabel_UniquePrefix { get; }

        /// <summary>
        /// The postfix of the label used for generated Assets with this Window. 
        /// </summary>
        protected abstract string DatasLabel_Postfix { get; }

        /// <summary>
        /// Le label ajouté à toutes resoures générés par ce script
        /// </summary>
        protected string GeneratedObjectsLabel { get => DatasLabel_UniquePrefix + DatasLabel_Postfix; }

        /// <summary>
        /// Est ce que le un fichier généré par ce script peut avoir plusieur label avec le meme prefix ?
        /// Exemple : une animation appartient à 1 sprite sheet (si le jeu est bien fait), alors on va autoriser que 1 seule fois le label "tex_" sur les animation 
        /// Un animation peut appartenir à plusieur animators, alors on va autoriser plusieurs fois le label "anim_" sur les animations..
        /// </summary>
        //protected virtual bool multifile_prefix { get; }
        //Simple a implementer, mais reelement utile ?


        protected void UpdateAssetLabel(Object obj)
        {

            obj.UpdateLabel(GeneratedObjectsLabel, DatasLabel_UniquePrefix);
        }

        //TODO : SaveOrUpdateAsset() generique pour appliquer les labes

    }
}
#endif
