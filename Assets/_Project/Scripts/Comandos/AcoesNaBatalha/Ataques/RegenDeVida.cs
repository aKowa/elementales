using UnityEngine;
[CreateAssetMenu(menuName = "Acoes/Batalha/Ataques/Regeneracao De Vida")]
public class RegenDeVida : AcaoNaBatalha
{
    [SerializeField] private int taxaMinRecuperacao, taxaMaxRecuperacao;
    public override void Executar(BattleManager battleManager, Comando comando)
    {
        ComandoDeAtaque comandoDeAtaque = (ComandoDeAtaque)comando;

        for (int i = 0; i < comandoDeAtaque.AlvoAcao.Count; i++)
        {
            int vidaRecuperada = Mathf.CeilToInt(comandoDeAtaque.GetMonstro.AtributosAtuais.VidaMax * Random.Range(taxaMinRecuperacao, taxaMaxRecuperacao+1)/100);
            
            comandoDeAtaque.GetMonstro.ReceberCura(vidaRecuperada);
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