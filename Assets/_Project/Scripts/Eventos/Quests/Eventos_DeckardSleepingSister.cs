using BergamotaLibrary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eventos_DeckardSleepingSister : MonoBehaviour
{
    [Header("Componentes")]
    [SerializeField] private EventosCutscene eventosCutsceneDanielBattle;

    [Header("Variaveis")]
    [SerializeField] private ListaDeFlags listaDeFlags;
    [SerializeField] private string nomeDaFlag;

    [SerializeField] private string nomeDaFlagDaQuest;
    [SerializeField] private string nomeDaFlagDeBatalhaDoDaniel;

    [Space(10)]

    [SerializeField] private MonsterData[] monstrosParaCompletarAQuest;

    private void Awake()
    {
        Flags.SetFlag(listaDeFlags.name, nomeDaFlag, false);
    }

    private void Start()
    {
        foreach(MonsterData monstro in monstrosParaCompletarAQuest)
        {
            if(PlayerData.Instance.Inventario.GetMonstroNaBag(monstro) != null)
            {
                Flags.SetFlag(listaDeFlags.name, nomeDaFlag, true);
                break;
            }
        }

        IniciarCutscene();
    }

    private void IniciarCutscene()
    {
        if(Flags.GetFlag(listaDeFlags.name, nomeDaFlag) == true && Flags.GetFlag(listaDeFlags.name, nomeDaFlagDaQuest) == true && Flags.GetFlag(listaDeFlags.name, nomeDaFlagDeBatalhaDoDaniel) == false)
        {
            eventosCutsceneDanielBattle.RodarCustcene();
        }
    }
}
