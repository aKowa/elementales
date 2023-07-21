using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Acoes/Batalha/Ataques/Golpe Unico Contra certo tipo")]

public class GolpeUnicoDanoExtraContraCertoTipo : AcaoNaBatalha
{
    [SerializeField] private List<PoderExtraContraCertoTipo> poderExtraContraCertoTipo = new List<PoderExtraContraCertoTipo>();
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
                Debug.Log("B: " + comandoDeAtaque.AttackData.Poder);

                foreach (var poderExtraContraCertoTipo in poderExtraContraCertoTipo)
                {
                    for (int j = 0; j < poderExtraContraCertoTipo.TipoMonstro.Count; j++)
                    {
                        for (int k = 0; k < comandoDeAtaque.AlvoAcao[i].GetMonstro.MonsterData.GetMonsterTypes.Count; k++)
                        {
                            if (poderExtraContraCertoTipo.TipoMonstro[j] == comandoDeAtaque.AlvoAcao[i].GetMonstro.MonsterData.GetMonsterTypes[k])
                            {
                                comandoDeAtaque.AttackData.Poder *= poderExtraContraCertoTipo.Multiplicador;
                                Debug.Log("Aumentei o poder do ataque");
                            }
                        }
                    }
                }
                Debug.Log("A: " + comandoDeAtaque.AttackData.Poder);

                comandoDeAtaque.AlvoAcao[i].Monstro.TomarAtaque(atributoAtaque, comandoDeAtaque, comandoDeAtaque.AlvoAcao[i], true, true, true);
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
}
[System.Serializable]
public class PoderExtraContraCertoTipo
{
    [SerializeField] private List<MonsterType> tiposAtaquesDanoExtra = new List<MonsterType>();
    [SerializeField] private int multiplicadorPoder;
    public List<MonsterType> TipoMonstro => tiposAtaquesDanoExtra;
    public int Multiplicador => multiplicadorPoder;
}
