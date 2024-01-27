using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace logiked.source.types
{

    /// <summary>
    /// [WIP] Classe de base pour le stockage des états des inputs. A redefinir pour chage projets. Utile pour le partage des inputs au travers du code, et l'envoi en ligne. Utile aussi pour passer l'input system 2.0 de unity avec un mode d'interpretation 1.0 : convrtit des valeurs en KeyDown, KeyUp...
    /// </summary>
    [Serializable]
 public  class InputStorage
    {

        #region Classes

        [Serializable]
        public enum InputState
        {
            None=0b0001, KeyDown= 0b0010, KeyPressed= 0b01000, KeyUp= 0b1000
        }

        [Serializable]
        private class InputStatePair : Tuple<InputState, float> { public InputStatePair(InputState i1, float i2) :base (i1, i2){ } }

        [System.Serializable]
        class DictState : Dictionary<string, InputStatePair> { }

        #endregion



        [SerializeField]
        DictState inputsValues = new DictState();

        /// <summary>
        /// Vérifier l'état d'une valeur
        /// </summary>
        /// <param name="input">Nom de l'input</param>
        /// <param name="val">Etat de l'input</param>
        /// <returns></returns>
        public bool Check(string input, InputState val)
        {
            return inputsValues.ContainsKey(input) && (inputsValues[input].Item1 & val) != 0;
        }

        /// <summary>
        /// Enregistre la valeur de l'input dans le dictionnaire. A utiliser dans un update, eviter FixedUpdate
        /// </summary>
        public void RegisterValue(string input, float value)
        {
          //  Debug.LogError( value + " " + input);


            if (!inputsValues.ContainsKey(input))
                inputsValues.Add(input, new InputStatePair(InputState.None, 0));


            InputStatePair curVal = inputsValues[input];


            if (value == 0)
            {
        
                //SI l'input etait pressé
                    if( (curVal.Item1 & (InputState.KeyDown | InputState.KeyPressed ) ) != 0)
                {
                    curVal = new InputStatePair(InputState.KeyUp, value);//On relache la touche
                }
                else if ((curVal.Item1 & (InputState.KeyUp)) != 0) //Si la touche viens d'etre relachée, on la declare None
                {
                    curVal = new InputStatePair(InputState.None, value);
                }

            }
            else//Si != 0
            {

                //SI l'input n'etait pas pressé
                if ((curVal.Item1 & (InputState.None | InputState.KeyUp)) != 0)
                {
                    curVal = new InputStatePair(InputState.KeyDown | InputState.KeyPressed, value);//On presse la touche
                }
                else if ((curVal.Item1 & (InputState.KeyDown)) != 0) //Si la touche viens d'etre pressée, on la declare maintenue
                {
                    curVal = new InputStatePair(InputState.KeyPressed, value);
                }

            }




        }



    }
}
