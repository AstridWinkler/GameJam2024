using logiked.source.attributes;
using logiked.source.utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace logiked.source.database
{
    /// <summary>
    /// Une cat�gorie de rangement pour des objets de la base de donn�es
    /// </summary>
    [Serializable]
    public class DatabaseCategory
    {

        [Tooltip("Nom de la cat�gorie")]
        [SerializeField] private string categoryName;
        [SerializeField] public string CategoryName => categoryName;

#if UNITY_EDITOR
        [Tooltip("La couleur de la cat�gorie")]
        [SerializeField] public Color categoryColor = Color.white;

        [Tooltip("Les descriptors des �lements de la BDD de cette cat�gorie. (Par d�fault : type de base de la bdd)")]
        [MonoScript(type = typeof(DatabaseAbstractElement))]
        [SerializeField] private string desciptorClass;
        [SerializeField] public string DesciptorClass { get => desciptorClass; set => desciptorClass = value; }

        [Tooltip("Masquer certains champs des elements de la BDD dans l'inspector. Pratique pour les elements des cat�gories qui comportent beaucoup de champs similaires.")]
        [SerializeField] public bool MaskProperties;
#endif
         


    }
}