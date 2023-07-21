using UnityEngine;

[CreateAssetMenu(menuName = "Acoes/Batalha/Ataques/Golpe Unico Dano Settado")]
public class GolpeUnicoDanoSettado : AcaoNaBatalha
{
    [SerializeField] private int min, max;
    public override void Executar(BattleManager battleManager, Comando comando)
    {
        ComandoDeAtaque comandoDeAtaque = (ComandoDeAtaque)comando;
        
        for (int i = 0; i < comandoDeAtaque.AlvoAcao.Count; i++)
        {
            if (comandoDeAtaque.AlvoComAtaquesValidos[i] == false)
            {
                comandoDeAtaque.AlvoAcao[i].Monstro.ForcarMiss(comandoDeAtaque.AlvoAcao[i], true);
            }
            else
            {
                comandoDeAtaque.AlvoAcao[i].Monstro.TomarAtaquePuro(Random.Range(min, max +1), comandoDeAtaque.AlvoAcao[i], true, true);
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