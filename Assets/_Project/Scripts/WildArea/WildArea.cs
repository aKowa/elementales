using BergamotaDialogueSystem;
using BergamotaLibrary;
using System.Collections;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class WildArea : Interagivel
{
    [Header("Informacoes da Batalha")]
    [SerializeField] private ComandoArena arenaPadrao;
    [SerializeField] private Sprite backgroundDaBatalha;

    [Space(10)]
    [SerializeField] private AudioClip musicaDeBatalha;

    //Componentes
    [Header("Componentes")]
    [SerializeField] TipoWildArea tipoWildArea;
    [SerializeField] private Item itemNecessarioInteracao;
    [SerializeField] private DialogueObject dialogoNaoPossuiItemNecessario;
    [SerializeField] private DialogueObject dialogoComecouPescar;
    [SerializeField] private DialogueObject dialogoComecouPescouAlgo;

    private DialogueActivator dialogueActivator;

    //Variaveis
    [Header("Variaveis")]
    [SerializeField] private float tempoPescandoChamarWildAreaMax;
    [SerializeField] private int contadorTilesGarantidoSemEncontro;
    [SerializeField] private float chanceSpawnGeral;

    [Header("Lista Monstros")]
    [SerializeField] private WeightedRandomList<MonstroWildArea> weightedMonsterList;

    private enum TipoWildArea { Chao, Agua };
    private Vector2 posicaoPlayer;
    private bool colidindo;
    private Player player;
    private PlayerData playerData;
    private int contadorTiles;

    float tempoPescandoChamarWildArea;
    private void Awake()
    {
        contadorTiles = 0;
        dialogueActivator = GetComponent<DialogueActivator>();
    }

    public override void Interagir(Player player)
    {
        if (VerificarSePossuiItem(player))
        {
            GetComponentesPlayer(player.gameObject);

            switch (player.GetEstadoPlayer)
            {
                case Player.EstadoPlayer.Normal:
                    dialogueActivator.ShowDialogue(dialogoComecouPescar, player.DialogueUI);

                    player.SetEstado(Player.EstadoPlayer.Pescando);
                    player.PlayerMovement.ZeroVelocity();

                    break;

                case Player.EstadoPlayer.Pescando:
                    SairEstadoPesca();
                    break;
            }
        }
        else
            dialogueActivator.ShowDialogue(dialogoNaoPossuiItemNecessario, player.DialogueUI);
    }

    private void FixedUpdate()
    {
        switch (tipoWildArea)
        {
            case TipoWildArea.Chao:
                WildAreaNormal();
                break;
            case TipoWildArea.Agua:
                WildAreaPesca();
                break;
        }
    }
    void WildAreaPesca()
    {
        if (player?.DialogueUI.IsOpen == true)
            return;

        if (player?.GetEstadoPlayer == Player.EstadoPlayer.Pescando)
        {
            tempoPescandoChamarWildArea += Time.deltaTime;
            if (tempoPescandoChamarWildArea >= tempoPescandoChamarWildAreaMax)
            {
                tempoPescandoChamarWildArea = 0;
                if (BattlePossibility())
                {
                    StartCoroutine(MostrarDialogoPesca());
                }
            }
        }
    }
    
    void WildAreaNormal()
    {
        if (colidindo)
        {
            Vector2 posicaoTemp = AtualizarPosicaoPlayer(player.transform.position);
            if (posicaoTemp != posicaoPlayer)
            {
                posicaoPlayer = posicaoTemp;
                if (VerifcarTileSemEncontro() == true)
                {
                    return;
                }
                if (BattlePossibility() == true)
                {
                    Monster monstro = PrepararBatalha();
                    if (VerificarRepelente(monstro) == true)
                    {
                        return;
                    }
                    contadorTiles = 0;
                    IniciarBatalha(monstro);
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (tipoWildArea != TipoWildArea.Agua)
        {
            if (weightedMonsterList.Count > 0)
            {
                if (collision.CompareTag("Player"))
                {
                    GetComponentesPlayer(collision.gameObject);
                    colidindo = true;
                    posicaoPlayer = AtualizarPosicaoPlayer(collision.transform.position);
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (tipoWildArea != TipoWildArea.Agua)
        {
            if (weightedMonsterList.Count > 0)
            {
                if (collision.CompareTag("Player"))
                {
                    colidindo = false;
                }
            }
        }
    }

    private void GetComponentesPlayer(GameObject player)
    {
        if (this.player == null)
        {
            playerData = player.GetComponent<PlayerData>();
            this.player = player.GetComponent<Player>();
        }
    }

    private Vector2 AtualizarPosicaoPlayer(Vector2 posicao)
    {
        return new Vector2((int)posicao.x, (int)posicao.y);
    }

    private bool VerificarSePossuiItem(Player player)
    {
        if (itemNecessarioInteracao == null)
            return true;
        
        foreach (var itemPlayer in player.PlayerData.Inventario.ItensChave)
        {
            if (itemPlayer.Item.ID == itemNecessarioInteracao.ID)
            {
                return true;
            }
        }
        return false;
    }

    private bool VerificarRepelente(Monster monstro)
    {
        if (PlayerData.Repelente > 0)
        {
            if (monstro.Nivel < PlayerData.Instance.Inventario.MonsterBag[0].Nivel)
            {
                return true;
            }
        }
        return false;
    }

    private bool VerifcarTileSemEncontro()
    {
        if (contadorTiles < contadorTilesGarantidoSemEncontro)
        {
            contadorTiles++;
            return true;
        }
        return false;
    }

    private bool BattlePossibility()
    {
        float chanceSpawn = Random.Range(0f, 100);
        if (chanceSpawn <= chanceSpawnGeral)
        {
            return true;
        }
        return false;
    }
    private IEnumerator MostrarDialogoPesca()
    {
        dialogueActivator.ShowDialogue(dialogoComecouPescouAlgo, player.DialogueUI);
        yield return new WaitUntil(() => player.DialogueUI.IsOpen == false);
        IniciarBatalha(PrepararBatalha());
        SairEstadoPesca();
    }
    private Monster PrepararBatalha()
    {
        var randomMonster = weightedMonsterList.GetRandom();
        int rndNivel = randomMonster.GetNivel;
        MonsterData rndMonster = (randomMonster.GetMonsterData);
        int attacksCount = rndMonster.GetMonsterUpgradesPerLevel.Where(upgrade => upgrade.Level <= rndNivel && upgrade.Ataque != null).Count();

        return new Monster(rndMonster, rndNivel, null, randomMonster.GetNumOfAttacks(rndNivel, attacksCount));
    }

    private void IniciarBatalha(Monster monstro)
    {
        GameManager.Instance.StartBattle(arenaPadrao, 1, playerData, backgroundDaBatalha, monstro, musicaDeBatalha, true);
        player.PlayerMovement.ZeroVelocity();
        NPCManager.IniciandoBatalha = true;
    }
    void SairEstadoPesca()
    {
        player.SetEstado(Player.EstadoPlayer.Normal);
    }

    [System.Serializable]
    public struct MonstroWildArea
    {
        [SerializeField] private int nivelMin, nivelMax;
        [SerializeField] private MonsterData monstro;
        public int GetNivel => Random.Range(nivelMin, nivelMax + 1);
        public MonsterData GetMonsterData => monstro;

        public int GetNumOfAttacks(int nivel, int attacksCount)
        {
            int numMinOfAttacks = (int)Mathf.Clamp(nivel * 0.2f * Random.Range(1, 4), 1, 4);
            int numMaxOfAttacks = Mathf.Clamp(attacksCount, 1, 4);
            return Random.Range(numMinOfAttacks, numMaxOfAttacks + 1);
        }
    }

    [System.Serializable]
    public struct MonstroAtaques
    {
        [SerializeField] private ComandoDeAtaque ataques;
        public ComandoDeAtaque GetComandoDeAtaques => ataques;
    }
}
