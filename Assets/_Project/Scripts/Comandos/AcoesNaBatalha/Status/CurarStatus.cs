using UnityEngine;

[CreateAssetMenu(menuName = "Acoes/Batalha/Status/Curar Status")]
public class CurarStatus : AcaoNaBatalha
{
    [SerializeField] private StatusEffectBase[] statusParaCurar;

    public override void Executar(BattleManager battleManager, Comando comando)
    {
        foreach (StatusEffectBase status in statusParaCurar)
        {
            Debug.Log("Curei o status: " + status);
            comando.AlvoAcao.ForEach(x => x.GetMonstro.RemoverStatusPorTipo(status));
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