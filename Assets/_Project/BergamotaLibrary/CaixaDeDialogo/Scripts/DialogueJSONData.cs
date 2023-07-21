using UnityEngine;

namespace BergamotaDialogueSystem
{
    //Classe que contem as listas com os textos dos dialogos
    [System.Serializable]
    public class DialogueJSONData
    {
        public Dialogo[] dialogos;
        public Dialogo[] respostas;

        //Classe que contem o texto do dialogo
        [System.Serializable]
        public class Dialogo
        {
            public string texto;
        }
    }
}
