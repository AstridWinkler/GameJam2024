using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace logiked.source.gameplay
{
    /// <summary>
    /// Interface d'un objet dans la scène avec lequel le joueur peu interagir quand il passe à proximité (objet, item...)
    /// </summary>
  public interface I_Interactable
    {
        /// <summary>
        /// Determines whether this instance [can be interact].
        /// </summary>
        bool CanBeInteract();

        /// <summary>
        ///  Utile pour faire des effets de couleurs et tout
        /// </summary>
        void OutlineObject(bool state);

        /// <summary>
        /// Interacts with this instance.       
        /// </summary>
        bool Interact(int viewId);


    }
}
