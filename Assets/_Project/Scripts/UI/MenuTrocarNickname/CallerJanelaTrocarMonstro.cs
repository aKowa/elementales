using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallerJanelaTrocarMonstro : MonoBehaviour
{
    private JanelaEscolherMonstroGenerica janelaEscolher;
    private JanelaTrocarNickname janelaTrocarNickname;

    private void Awake()
    {
        janelaTrocarNickname = FindObjectOfType<JanelaTrocarNickname>();
        janelaEscolher = FindObjectOfType<JanelaEscolherMonstroGenerica>();

        janelaTrocarNickname.EventoNomeTrocado.AddListener(FecharMenuEscolher);
        janelaEscolher.EventoMonstroEscolhido.AddListener(PassarMonstro);
    }

    private void PassarMonstro(Monster monster)
    {
        janelaTrocarNickname.IniciarMenu(monster);
    }

    private void FecharMenuEscolher()
    {
        janelaEscolher.FecharMenu();
    }

    public void AbrirMenuEscolherMonstros()
    {
        janelaEscolher.IniciarMenu(PlayerData.Instance.Inventario);
    }
}
