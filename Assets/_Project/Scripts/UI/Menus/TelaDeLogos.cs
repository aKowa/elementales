using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TelaDeLogos : MonoBehaviour
{
    //Variaveis
    [Header("Variaveis Iniciais")]
    [SerializeField] private SceneReference telaInicial;

    private void Awake()
    {
        Transition.GetInstance().DoTransition("FadeOutWhite", 0);
    }

    public void IrParaATelaInicial()
    {
        FazerTransicaoPraTelaInicial();
    }

    private void FazerTransicaoPraTelaInicial()
    {
        Transition.GetInstance().DoTransition("FadeInWhite", 0, () =>
        {
            MapsManager.GetInstance().LoadSceneByName(telaInicial);
        });
    }
}
