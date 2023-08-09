using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Acoes/Batalha/Status/Aumentar Modificador Aleatorio")]

public class AumentarModificadorAleatorio : AcaoNaBatalha
{
    [SerializeField] List<ModificadorExtra> modificadores = new List<ModificadorExtra>();

    public override void Executar(BattleManager battleManager, Comando comando)
    {
        foreach (var monstro in comando.AlvoAcao)
        {
            foreach (var modificador in modificadores)
            {
                int indiceNovoAtributo = Random.Range(0, (int)Modificador.Atributo.velocidade);
                modificador.atributo = (Modificador.Atributo)indiceNovoAtributo;

                var modificadorInstanciado =  new ModificadorTurno(modificador.modificador.QuantidadeTurnos, modificador.modificador.PassaComTurnos, modificador.modificador.ValorModificador, modificador.modificador.Origem);

                monstro.GetMonstro.AtributosAtuais.ReceberModificadorStatus(modificador.atributo, modificadorInstanciado);
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

