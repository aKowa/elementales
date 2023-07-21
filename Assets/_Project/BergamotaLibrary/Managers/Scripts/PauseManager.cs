using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BergamotaLibrary
{
    public class PauseManager : MonoBehaviour
    {
        private static bool jogoPausado = false;
        private static bool permitirInput = true; //Permite que o jogador use comandos da gameplay principal, isto nao inclui os menus, inventario, caixas de dialogo etc.
        private static bool permitirInputGeral = true; //Permite que o jogador use comandos no jogo, isso inclui tudo, incluindo os menus, inventario, caixas de dialogo etc.

        //Getters
        public static bool JogoPausado => jogoPausado;

        public static bool PermitirInput
        {
            get
            {
                return permitirInput;
            }

            set
            {
                permitirInput = value;
            }
        }

        public static bool PermitirInputGeral
        {
            get
            {
                return permitirInputGeral;
            }

            set
            {
                permitirInputGeral = value;
            }
        }

        private void Awake()
        {
            Pausar(false); //Despausa o jogo no Awake caso o script seja colocado em algum objeto
        }

        /// <summary>
        /// Pausa ou resume o jogo.
        /// </summary>
        /// <param name="pausar">Valor que define se o jogo sera ou nao pausado</param>
        public static void Pausar(bool pausar)
        {
            if (pausar == true)
            {
                jogoPausado = true;
                Time.timeScale = 0; //Pausa todas as operacoes calculadas com tempo
                AudioListener.pause = true; //Pausa os sons do jogo
            }
            else
            {
                jogoPausado = false;
                Time.timeScale = 1; //Resume todas as operacoes calculadas com tempo
                AudioListener.pause = false; //Resume os sons do jogo
            }
        }
    }
}
