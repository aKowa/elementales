using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Acoes/Batalha/Status/Dragon Fruit Ações")]

public class AcaoDragonFruit : AcaoNaBatalha
{
    [SerializeField] private List<StatusEffectParaAplicar> status = new List<StatusEffectParaAplicar>();

    public override void Executar(BattleManager battleManager, Comando comando)
    {
        foreach (var monstro in comando.AlvoAcao)
        {
            foreach (var statu in status)
            {
                monstro.GetMonstro.AplicarStatus(statu.GetStatus);
            }
        }
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
