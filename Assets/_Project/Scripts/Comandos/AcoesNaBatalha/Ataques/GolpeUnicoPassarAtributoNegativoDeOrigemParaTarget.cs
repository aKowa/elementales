using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Acoes/Batalha/Ataques/Golpe Unico Clona Modificador Origem para Alvo")]

public class GolpeUnicoPassarAtributoNegativoDeOrigemParaTarget : AcaoNaBatalha
{
    public override void Executar(BattleManager battleManager, Comando comando)
    {
        ComandoDeAtaque comandoDeAtaque = (ComandoDeAtaque)comando;
        int atributoAtaque;

        if (comandoDeAtaque.AttackData.Categoria == AttackData.CategoriaEnum.Fisico)
        {
            atributoAtaque = comandoDeAtaque.GetMonstro.AtributosAtuais.AtaqueComModificador;
        }
        else
        {
            atributoAtaque = comandoDeAtaque.GetMonstro.AtributosAtuais.SpAtaqueComModificador;
        }

        for (int i = 0; i < comandoDeAtaque.AlvoAcao.Count; i++)
        {
            if (comandoDeAtaque.AlvoComAtaquesValidos[i] == false)
            {
                comandoDeAtaque.AlvoAcao[i].Monstro.ForcarMiss(comandoDeAtaque.AlvoAcao[i], true);
            }
            else
            {
                (float dano, bool acertou) = comandoDeAtaque.AlvoAcao[i].Monstro.TomarAtaque(atributoAtaque, comandoDeAtaque, comandoDeAtaque.AlvoAcao[i], true, true, true);
                if (acertou)
                {
                    List<ModificadorTurno> modificadoresAtaques = new List<ModificadorTurno>();
                    List<ModificadorTurno> modificadoresDefesa = new List<ModificadorTurno>();
                    List<ModificadorTurno> modificadoresSpAtaques = new List<ModificadorTurno>();
                    List<ModificadorTurno> modificadoresSpDefesa = new List<ModificadorTurno>();
                    List<ModificadorTurno> modificadoresVelocidade = new List<ModificadorTurno>();
                    List<ModificadorTurno> modifcadoresGerais = new List<ModificadorTurno>();

                    AjeitarListaDeModificadores(ref modificadoresAtaques, ref modificadoresDefesa, ref modificadoresSpAtaques, ref modificadoresSpDefesa, ref modificadoresVelocidade,
                        comandoDeAtaque.GetMonstro.AtributosAtuais.ModificadorDebuffAtaque,
                        comandoDeAtaque.GetMonstro.AtributosAtuais.ModificadorDebuffDefesa,
                        comandoDeAtaque.GetMonstro.AtributosAtuais.ModificadorDebuffSpAtaque,
                        comandoDeAtaque.GetMonstro.AtributosAtuais.ModificadorDebuffSpDefesa
                        , comandoDeAtaque.GetMonstro.AtributosAtuais.ModificadorDebuffVelocidade);

                    if (modificadoresAtaques.Count > 0 || modificadoresDefesa.Count > 0 || modificadoresSpAtaques.Count > 0 || modificadoresSpDefesa.Count > 0 || modificadoresVelocidade.Count > 0)
                    {
                        (ModificadorTurno modificadorAleatorio, Modificador.Atributo atributo) = EscolherModificadorAleatorio(ref modifcadoresGerais, modificadoresAtaques, modificadoresDefesa, modificadoresSpAtaques, modificadoresSpDefesa, modificadoresVelocidade);
                        if (modificadorAleatorio != null)
                        {
                            comandoDeAtaque.AlvoAcao[i].GetMonstro.AtributosAtuais.ReceberModificadorStatus(atributo, modificadorAleatorio.ValorModificador, modificadorAleatorio.PassaComTurnos, modificadorAleatorio.QuantidadeTurnos);
                        }
                    }
                }
            }
        }
        if (comandoDeAtaque.NumeroRoundsComandoVivo <= 0)
        {
            comandoDeAtaque.PodeMeRetirar = true;
        }
        else
        {
            comandoDeAtaque.NumeroRoundsComandoVivo--;
        }
    }

    private (ModificadorTurno, Modificador.Atributo) EscolherModificadorAleatorio(ref List<ModificadorTurno> modifcadoresGerais, List<ModificadorTurno> modificadoresAtaques, List<ModificadorTurno> modificadoresDefesa, List<ModificadorTurno> modificadoresSpAtaques, List<ModificadorTurno> modificadoresSpDefesa, List<ModificadorTurno> modificadoresVelocidade)
    {
        foreach (var item in modificadoresAtaques)
        {
            if (item.ValorModificador < 0)
                modifcadoresGerais.Add(item);
        }
        foreach (var item in modificadoresDefesa)
        {
            if (item.ValorModificador < 0)
                modifcadoresGerais.Add(item);
        }
        foreach (var item in modificadoresSpAtaques)
        {
            if (item.ValorModificador < 0)
                modifcadoresGerais.Add(item);
        }
        foreach (var item in modificadoresSpDefesa)
        {
            if (item.ValorModificador < 0)
                modifcadoresGerais.Add(item);
        }
        foreach (var item in modificadoresVelocidade)
        {
            if (item.ValorModificador < 0)
                modifcadoresGerais.Add(item);
        }
        if (modifcadoresGerais.Count <= 0)
        {
            return (null, Modificador.Atributo.defesa);
        }
        var modificadorAleatorio = modifcadoresGerais[Random.Range(0, modifcadoresGerais.Count)];

        if (modificadoresAtaques.Contains(modificadorAleatorio))
            return (modificadorAleatorio, Modificador.Atributo.ataque);

        if (modificadoresDefesa.Contains(modificadorAleatorio))
            return (modificadorAleatorio, Modificador.Atributo.defesa);

        if (modificadoresSpAtaques.Contains(modificadorAleatorio))
            return (modificadorAleatorio, Modificador.Atributo.spAtaque);

        if (modificadoresSpDefesa.Contains(modificadorAleatorio))
            return (modificadorAleatorio, Modificador.Atributo.spDefesa);

        if (modificadoresVelocidade.Contains(modificadorAleatorio))
            return (modificadorAleatorio, Modificador.Atributo.velocidade);
        return (null, Modificador.Atributo.defesa);
    }

    void AjeitarListaDeModificadores(ref List<ModificadorTurno> modificadoresAtaques, ref List<ModificadorTurno> modificadoresDefesa, ref List<ModificadorTurno> modificadoresSpAtaques, ref List<ModificadorTurno> modificadoresSpDefesa, ref List<ModificadorTurno> modificadoresVelocidade,
        Modificador modificadorAtaque, Modificador modificadorDefesa, Modificador modificadorSpAtaque, Modificador modificadorSpDefesa, Modificador modificadrVelocidade)
    {
        for (int m = 0; m < modificadorAtaque.ListaBuffsDebuffs.Count; m++)
        {
            modificadoresAtaques.Add(modificadorAtaque.ListaBuffsDebuffs[m]);
        }
        for (int m = 0; m < modificadorDefesa.ListaBuffsDebuffs.Count; m++)
        {
            modificadoresDefesa.Add(modificadorDefesa.ListaBuffsDebuffs[m]);
        }
        for (int m = 0; m < modificadorSpAtaque.ListaBuffsDebuffs.Count; m++)
        {
            modificadoresSpAtaques.Add(modificadorSpAtaque.ListaBuffsDebuffs[m]);
        }
        for (int m = 0; m < modificadorSpDefesa.ListaBuffsDebuffs.Count; m++)
        {
            modificadoresSpDefesa.Add(modificadorSpDefesa.ListaBuffsDebuffs[m]);
        }
        for (int m = 0; m < modificadrVelocidade.ListaBuffsDebuffs.Count; m++)
        {
            modificadoresVelocidade.Add(modificadrVelocidade.ListaBuffsDebuffs[m]);
        }
    }
}
