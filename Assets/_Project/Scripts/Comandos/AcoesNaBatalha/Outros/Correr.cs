using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Acoes/Batalha/Outros/Correr")]
public class Correr : AcaoNaBatalha
{
    public override void Executar(BattleManager battleManager, Comando comando)
    {
        if(battleManager.BatalhaQuePodeCorrer == false)
        {
            Debug.Log(comando.GetMonstro + " esta tentando corrr, mas essa batalha é impossivel de correr");
            comando.PodeMeRetirar = true;
            battleManager.PlayerConseguiCorrer(false);
            return;
        }
        foreach (StatusEffectSecundario status in comando.GetMonstro.StatusSecundario)
        {
            if(status.GetTipoStatus == StatusEffectSecundario.TipoStatus.Locked) // caso estaja com status de preso em batalha
            {
                Debug.Log(comando.GetMonstro + " esta tentando corrr, mas não pode devido ao seu estado");
                comando.PodeMeRetirar = true;
                battleManager.PlayerConseguiCorrer(false);
                return;
            }
        }
        if (battleManager.GetTipoBatalha != BattleManager.TipoBatalha.MonstroSelvagem) // Caso não seja contra um monstro selvagem
        {
            Debug.Log(comando.GetMonstro + " esta tentando corrr, mas não pode devido batalha não ser contra monstro selvagem");
            comando.PodeMeRetirar = true;
            battleManager.PlayerConseguiCorrer(false);
            return;
        }
        if (comando.GetMonstro.AtributosAtuais.VelocidadeComModificador >= comando.AlvoAcao[0].GetMonstro.AtributosAtuais.VelocidadeComModificador)//Velocidade do monstro origem >= que monstro alvo
        {
            Debug.Log(comando.GetMonstro + " Consegui correr por ser muito mais rapido");

            comando.PodeMeRetirar = true;
            battleManager.PlayerConseguiCorrer(true);
            return;
        }
        else if ((comando.GetMonstro.AtributosAtuais.VelocidadeComModificador * 128/ comando.AlvoAcao[0].GetMonstro.AtributosAtuais.VelocidadeComModificador)
            +30 * battleManager.GetTentativasCorrer >= Random.Range(0,256)) // Conta para ver se consegue fugir
        {
            Debug.Log(comando.GetMonstro + " consegui correr por sorte");

            comando.PodeMeRetirar = true;
            battleManager.PlayerConseguiCorrer(true);
            return;
        }

        Debug.Log(comando.GetMonstro + " Não consegui Correr");

        comando.PodeMeRetirar = true;
        battleManager.PlayerConseguiCorrer(false);
    }

    public override void DialogoComando(BattleManager battleManager, Comando comando)
    {
        //Nada
    }
}
