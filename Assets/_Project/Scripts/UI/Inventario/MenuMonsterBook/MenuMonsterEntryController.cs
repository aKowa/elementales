using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuMonsterEntryController : ViewController
{
    //Componentes
    [Header("Componentes")]
    [SerializeField] private GameObject tipoLogoBase;
    [SerializeField] private TMP_Text nomeMonstro;
    [SerializeField] private TMP_Text idMonstro;
    [SerializeField] private TMP_Text categoriaMonstro;
    [SerializeField] private TMP_Text alturaMonstro;
    [SerializeField] private TMP_Text pesoMonstro;
    [SerializeField] private TMP_Text descricaoMonstro;
    [SerializeField] private Animator animatorMonstro;
    [SerializeField] private BergamotaLibrary.Animacao animacaoMonstro;
    [SerializeField] private Image iconeMonstroCapturado;
    [SerializeField] private Transform monstroTiposHolder;
    [SerializeField] private RectTransform botoesTrocarMonstroHolder;
    [SerializeField] private ButtonSelectionEffect[] botoesTrocarMonstro;
    [SerializeField] private RectTransform fundoBloqueadorDeAcoesDoMenu;

    [Header("Variaveis Padroes")]
    [SerializeField] private string textoCategoriaMonstro = "%categoria Monster";
    [SerializeField] private Sprite monstroCapturado;
    [SerializeField] private Sprite monstroNaoCapturado;
    private string textoMonstroNaoEncontrado = "???";

    //Variaveis
    private List<int> monsterEntriesFound = new List<int>();
    private List<TipoLogo> monstroTipoLogos = new List<TipoLogo>();

    private int indiceAtual;
    private int contadorAnimacao;
    private MonsterData monstroAtual;

    protected override void OnAwake()
    {
        fundoBloqueadorDeAcoesDoMenu.gameObject.SetActive(false);

        indiceAtual = 0;
        contadorAnimacao = 0;
    }

    public override void OnOpen()
    {
        fundoBloqueadorDeAcoesDoMenu.gameObject.SetActive(true);
    }

    protected override void OnClose()
    {
        fundoBloqueadorDeAcoesDoMenu.gameObject.SetActive(false);

        ResetarInformacoes();
    }

    public void IniciarMenu(MonsterData monstro)
    {
        monstroAtual = monstro;

        OpenView();

        botoesTrocarMonstroHolder.gameObject.SetActive(true);

        AtualizarMonsterEntriesFound();
        AtualizarBotoes();
        AtualizarInformacoes();
    }

    public void IniciarMenu(MonsterData monstro, bool incluirBotoesTrocarMonstro)
    {
        monstroAtual = monstro;

        OpenView();

        botoesTrocarMonstroHolder.gameObject.SetActive(incluirBotoesTrocarMonstro);

        if (incluirBotoesTrocarMonstro == true)
        {
            AtualizarMonsterEntriesFound();
            AtualizarBotoes();
        }

        AtualizarInformacoes();
    }

    private void AtualizarInformacoes()
    {
        nomeMonstro.text = monstroAtual.GetName;
        idMonstro.text = monstroAtual.MonsterBookID.ToString("D2");

        if (PlayerData.MonsterBook.MonsterEntries[monstroAtual.ID].WasCaptured == true)
        {
            categoriaMonstro.text = textoCategoriaMonstro.Replace("%categoria", monstroAtual.Categoria);

            alturaMonstro.text = monstroAtual.Altura.ToString() + " m";
            pesoMonstro.text = monstroAtual.Peso.ToString() + " kg";

            descricaoMonstro.text = monstroAtual.Descricao.ToString();

            AtualizarTipo(monstroAtual);
        }
        else
        {
            categoriaMonstro.text = textoMonstroNaoEncontrado;

            alturaMonstro.text = textoMonstroNaoEncontrado;
            pesoMonstro.text = textoMonstroNaoEncontrado;

            descricaoMonstro.text = textoMonstroNaoEncontrado;

            ResetarTipoLogos();
        }

        if (monstroAtual.Animator != null)
        {
            animatorMonstro.runtimeAnimatorController = monstroAtual.Animator;
        }
        else
        {
            Debug.LogWarning("O monstro " + monstroAtual.GetName + " nao possui um Animator Override Controller para ser usado!");
        }

        animacaoMonstro.TrocarAnimacao("Idle");
        contadorAnimacao = 0;

        SetarImagemMonstroCapturado();
    }

    private void ResetarInformacoes()
    {
        nomeMonstro.text = string.Empty;
        idMonstro.text = string.Empty;
        categoriaMonstro.text = string.Empty;
        alturaMonstro.text = string.Empty;
        pesoMonstro.text = string.Empty;
        descricaoMonstro.text = string.Empty;

        animatorMonstro.runtimeAnimatorController = null;

        monsterEntriesFound.Clear();

        ResetarTipoLogos();
    }

    private void AtualizarTipo(MonsterData monstro)
    {
        ResetarTipoLogos();

        for (int i = 0; i < monstro.GetMonsterTypes.Count; i++)
        {
            TipoLogo tipoLogo = Instantiate(tipoLogoBase, monstroTiposHolder).GetComponent<TipoLogo>();
            tipoLogo.gameObject.SetActive(true);

            tipoLogo.SetTipo(monstro.GetMonsterTypes[i]);

            monstroTipoLogos.Add(tipoLogo);
        }
    }

    private void ResetarTipoLogos()
    {
        foreach (TipoLogo tipoLogo in monstroTipoLogos)
        {
            Destroy(tipoLogo.gameObject);
        }

        monstroTipoLogos.Clear();
    }

    private void SetarImagemMonstroCapturado()
    {
        if (PlayerData.MonsterBook.MonsterEntries[monstroAtual.ID].WasCaptured == true)
        {
            iconeMonstroCapturado.sprite = monstroCapturado;
        }
        else
        {
            iconeMonstroCapturado.sprite = monstroNaoCapturado;
        }
    }

    private void AtualizarMonsterEntriesFound()
    {
        monsterEntriesFound.Clear();

        for(int i = 0; i < PlayerData.MonsterBook.MonsterEntries.Count; i++)
        {
            if (PlayerData.MonsterBook.MonsterEntries[i].WasFound == true)
            {
                monsterEntriesFound.Add(i);
            }
        }

        for (int i = 0; i < monsterEntriesFound.Count; i++)
        {
            if (monsterEntriesFound[i] == monstroAtual.ID)
            {
                indiceAtual = i;
                break;
            }
        }
    }

    private void AtualizarBotoes()
    {
        botoesTrocarMonstro[0].interactable = (indiceAtual > 0);
        botoesTrocarMonstro[1].interactable = (indiceAtual < (monsterEntriesFound.Count - 1));
    }

    public void MonstroAnterior()
    {
        if(indiceAtual > 0)
        {
            indiceAtual--;

            monstroAtual = GlobalSettings.Instance.Listas.ListaDeMonsterData.GetData(monsterEntriesFound[indiceAtual]);

            AtualizarBotoes();
            AtualizarInformacoes();
        }
    }

    public void MonstroSeguinte()
    {
        if (indiceAtual < (monsterEntriesFound.Count - 1))
        {
            indiceAtual++;

            monstroAtual = GlobalSettings.Instance.Listas.ListaDeMonsterData.GetData(monsterEntriesFound[indiceAtual]);

            AtualizarBotoes();
            AtualizarInformacoes();
        }
    }

    public void TrocarAnimacaoMonstro()
    {
        if(animacaoMonstro.AnimacaoAtual != "Idle")
        {
            return;
        }

        switch(contadorAnimacao)
        {
            case 0:
                contadorAnimacao = 1;
                animacaoMonstro.TrocarAnimacao("Atk");
                animacaoMonstro.ExecutarUmMetodoAposOFimDaAnimacao(MonstroAnimacaoIdle);
                break;

            case 1:
                contadorAnimacao = 0;
                animacaoMonstro.TrocarAnimacao("SpAtk");
                animacaoMonstro.ExecutarUmMetodoAposOFimDaAnimacao(MonstroAnimacaoIdle);
                break;

            default:
                contadorAnimacao = 0;
                animacaoMonstro.TrocarAnimacao("Idle");
                Debug.LogWarning("O contador de animacao estava com um valor incorreto!");
                break;
        }
    }

    private void MonstroAnimacaoIdle()
    {
        animacaoMonstro.TrocarAnimacao("Idle");
    }
}
