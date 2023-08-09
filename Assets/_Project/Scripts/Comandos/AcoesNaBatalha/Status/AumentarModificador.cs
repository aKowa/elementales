using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Acoes/Batalha/Status/Aumentar Modificador")]

public class AumentarModificador : AcaoNaBatalha
{
    [SerializeField] List<ModificadorExtra> modificadores = new List<ModificadorExtra>();

    public override void Executar(BattleManager battleManager, Comando comando)
    {
        foreach (var monstro in comando.AlvoAcao)
        {
            foreach (var modificador in modificadores)
            {
                monstro.GetMonstro.AtributosAtuais.ReceberModificadorStatus(modificador.atributo, modificador.modificador);
                monstro.GetMonstro.AtributosAtuais.TocarSomModificador(modificador.modificador.ValorModificador, monstro.GetMonstro.MonsterData);
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

