using BergamotaLibrary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Acoes/Batalha/Status/Recuperar Mana")]
public class RecuperarMana : AcaoNaBatalha
{
    //Variaveis
    [SerializeField] private int quantidadeDeMana;

    public override void Executar(BattleManager battleManager, Comando comando)
    {
        foreach (var item in comando.AlvoAcao)
        {
            item.GetMonstro.RecuperarMana(quantidadeDeMana);
        }
        battleManager.TocarSom("Mana");

        comando.PodeMeRetirar = true;
    }

    public override void DialogoComando(BattleManager battleManager, Comando comando)
    {
        if (comando is ComandoDeItem)
        {
            ComandoDeItem comandoDeItem = (ComandoDeItem)comando;

            battleManager.DialogoUsouItem(comando, comandoDeItem.ItemHolder.Item);
        }
        else
        {
            throw new System.Exception("O comando nao e um comando de item e esta usando uma acao feita para itens!");
        }
    }
}
