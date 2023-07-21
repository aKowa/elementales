using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventosCutsceneDanielBattle2 : MonoBehaviour
{
    //Componentes
    [SerializeField] private Cutscene cutscene;
    [SerializeField] private float tempoParaRodarACutscene;

    public void RodarACutsceneQuandoOObjetoFicarAtivo()
    {
        GameManager.Instance.StartCoroutine(RodarACutsceneQuandoOObjetoFicarAtivoCorrotina());
    }

    private IEnumerator RodarACutsceneQuandoOObjetoFicarAtivoCorrotina()
    {
        yield return new WaitUntil(() => cutscene.gameObject.activeInHierarchy == true);

        cutscene.IniciarCutscene(tempoParaRodarACutscene);
    }
}
