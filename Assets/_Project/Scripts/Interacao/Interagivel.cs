using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BergamotaLibrary
{
    public abstract class Interagivel : MonoBehaviour
    {
        /// <summary>
        /// E executado quando o script de interacao interagir com este objeto.
        /// </summary>
        public abstract void Interagir(Player player);

        /// <summary>
        /// E executado quando o objeto entra e quando sai da area de interacao de um script de interacao.
        /// </summary>
        /// <param name="estaNaArea"></param>
        public virtual void NaAreaDeInteracao(bool estaNaArea)
        {
            //Nada
        }
    }
}
