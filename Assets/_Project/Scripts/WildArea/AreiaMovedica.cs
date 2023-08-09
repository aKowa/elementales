using BergamotaLibrary;
using System.Collections;
using UnityEngine;

public class AreiaMovedica : MonoBehaviour
{
    //Variaveis
    [Header("Variaveis")]
    [SerializeField] private MonsterData monstroQuePassaSobreAreia;

    [Space(10)]

    [SerializeField] private AudioClip somAfundouNaAreia;

    [Header("Animacao")]
    [SerializeField] private float tempoParaGirar = 0.15f;
   
    private bool possuiMonstro;

    private Coroutine animacaoAreiaMovedica;

    private static bool movendoPlayer = false;

    private void Awake()
    {
        movendoPlayer = false;
    }

    private void MoverPlayer(Player player)
    {
        if(movendoPlayer == true)
        {
            return;
        }

        movendoPlayer = true;

        PermitirInput(false);

        RodarAnimacao(player);

        Transition.GetInstance().DoTransition("FadeIn", 0, () => FinalizarTransicao(player));

        SoundManager.instance.TocarSom(somAfundouNaAreia);
    }

    private void FinalizarTransicao(Player player)
    {
        PararAnimacao();

        TeleportarPlayer(player);

        Transition.GetInstance().DoTransition("FadeOut", 0, () => LiberarComandos());
    }

    private void LiberarComandos()
    {
        PermitirInput(true);

        movendoPlayer = false;
    }

    public void TeleportarPlayer(Player player)
    {
        Vector3 posicaoTeleporte = player.PlayerMovement.TraveledTiles[player.PlayerMovement.TraveledTiles.Count - 1] + new Vector2(0.5f, 0.5f);

        player.transform.position = posicaoTeleporte;
        player.PlayerMovement.TraveledTiles.Clear();

        player.PlayerMovement.TraveledTiles.Add(posicaoTeleporte);
    }

    public void PermitirInput(bool value)
    {
        PauseManager.PermitirInput = value;
    }

    private void RodarAnimacao(Player player)
    {
        PararAnimacao();

        animacaoAreiaMovedica = StartCoroutine(AnimacaoAreiaMovedica(player));
    }

    private void PararAnimacao()
    {
        if(animacaoAreiaMovedica != null)
        {
            StopCoroutine(animacaoAreiaMovedica);

            animacaoAreiaMovedica = null;
        }
    }

    private IEnumerator AnimacaoAreiaMovedica(Player player)
    {
        WaitForSeconds esperaParaGirar = new WaitForSeconds(tempoParaGirar);

        while(movendoPlayer == true)
        {
            player.SetDirection(EntityModel.Direction.Down);
            yield return esperaParaGirar;

            player.SetDirection(EntityModel.Direction.Left);
            yield return esperaParaGirar;

            player.SetDirection(EntityModel.Direction.Up);
            yield return esperaParaGirar;

            player.SetDirection(EntityModel.Direction.Right);
            yield return esperaParaGirar;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            for (int i = 0; i < PlayerData.Instance.Inventario.MonsterBag.Count; i++)
            {
                var x = PlayerData.Instance.Inventario.MonsterBag[i];
                if (x.MonsterData == monstroQuePassaSobreAreia)
                    possuiMonstro = true;
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (possuiMonstro)
            return;

        if (collision.gameObject.CompareTag("Player"))
        {
            Player player = collision.GetComponent<Player>();

            MoverPlayer(player);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            possuiMonstro = false;
        }
    }
}