using UnityEngine;

[CreateAssetMenu(menuName = "Acoes/Batalha/Capturar")]

public class Capturar : AcaoNaBatalha
{
    [SerializeField] private float monsterBallLevel;
    public override void Executar(BattleManager battleManager, Comando comando)
    {
        ComandoDeItem comandoDeItem = (ComandoDeItem)comando;
        Monster monstro = comandoDeItem.AlvoAcao[0].GetMonstro;
        Monster monstroOrigem = comandoDeItem.GetMonstro;

        if(monsterBallLevel >= 999)
        {
            battleManager.CapturarMonstro(true, monstro);
            comandoDeItem.PodeMeRetirar = true;
            return;
        }

        float bonusStatus = 1f;
        float rate = 0f;
        switch (monstro.MonsterData.MonsterRarity)
        {
            case MonsterRarityEnum.basico:
                rate = 60;
                break;
            case MonsterRarityEnum.raro:
                rate = 20;
                break;
            case MonsterRarityEnum.exotico:
                rate = 10;
                break;
            case MonsterRarityEnum.lendario:
                rate = 6;
                break;
        }

        foreach (var status in monstro.Status)
        {
            if (status.GetTipoStackStatus == StatusEffectBase.TipoStackStatus.Exclusivo)
            {
                bonusStatus += 2.5f;
            }
            else
            {
                bonusStatus += 1;
            }
        }
        float vida = (monstro.AtributosAtuais.VidaMax - monstro.AtributosAtuais.Vida) * 6 + 20;
        float diferencaNivel = monstro.Nivel > monstroOrigem.Nivel ? 0.8f : 1.1f;
        float somaGeral = (vida * rate * diferencaNivel * bonusStatus * (monsterBallLevel / 80))/20;
        float chanceMonstro = (100 - monsterBallLevel * 0.5f);

        Debug.Log($"Chance captura: vida {vida} * rate {rate} * monsterBallLevel {monsterBallLevel} / 100");
        Debug.Log("Chance captura: " + (int)somaGeral + "% Em " + chanceMonstro + "%");

        int rnd = Random.Range(0, (int)chanceMonstro);
        Debug.Log($"Sucesso Captura, rnd: {rnd} <= {somaGeral}");
        
        battleManager.CapturarMonstro(rnd <= (int)somaGeral,monstro);

        comandoDeItem.PodeMeRetirar = true;
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
