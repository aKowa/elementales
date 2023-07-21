using BergamotaDialogueSystem;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MonsterLearnAttack : ViewController
{
    //Componentes
    [Header("Componentes")]
    [SerializeField] private List<AtaqueSlotBatalha> ataqueSlots = new List<AtaqueSlotBatalha>();
    [SerializeField] private AtaqueSlotBatalha ataqueParaAprenderSlot;
    [SerializeField] private AtaqueInfo ataqueInfo;

    [SerializeField] private Image imagemBotaoConfirmarEscolha;
    [SerializeField] private ButtonSelectionEffect botaoConfirmarEscolha;
    [SerializeField] private TMP_Text textoBotaoConfirmarEscolha;

    [SerializeField] private RectTransform fundoBloqueadorDeAcoesDoMenu;
    [SerializeField] private RectTransform fundoDialogo;

    private DialogueUI dialogueUI;
    private DialogueActivator dialogueActivator;

    [Header("Variaveis Padroes")]
    [SerializeField] private string textoSelecioneUmAtaque;
    [SerializeField] private string textoNaoAprender;
    [SerializeField] private string textoEsquecer;

    [Header("Dialogos")]
    [SerializeField] private DialogueObject dialogoEsqueceuEAprendeuHabilidade;
    [SerializeField] private DialogueObject dialogoAprendeuHabilidade;
    [SerializeField] private DialogueObject dialogoNaoAprendeuHabilidade;
    [SerializeField] private DialogueObject dialogoQuerEsquecerHabilidade;

    //Enums
    public enum AprenderAtaque { PodeAprenderAtaque, NaoPodeAprenderAtaque, AcabouAprenderAtaque, JaSabeAtaque }

    //Variaveis
    private Monster monstroAtual;
    private AttackHolder ataqueParaAprender;

    int indice;

    protected override void OnAwake()
    {
        dialogueUI = FindObjectOfType<DialogueUI>();
        dialogueActivator = GetComponent<DialogueActivator>();

        ataqueInfo.gameObject.SetActive(false);
        fundoBloqueadorDeAcoesDoMenu.gameObject.SetActive(false);
        fundoDialogo.gameObject.SetActive(false);

        foreach (AtaqueSlotBatalha ataqueSlot in ataqueSlots)
        {
            ataqueSlot.SlotSelecionado.AddListener(EscolherAtaqueParaSubistituir);
            ataqueSlot.BotaoInfoSelecionado.AddListener(AbrirAtaqueInfo);
        }

        ataqueParaAprenderSlot.SlotSelecionado.AddListener(EscolherAtaqueParaSubistituir);
        ataqueParaAprenderSlot.BotaoInfoSelecionado.AddListener(AbrirAtaqueInfo);

        FecharMenu();
    }

    public override void OnOpen()
    {
        fundoBloqueadorDeAcoesDoMenu.gameObject.SetActive(true);
    }

    protected override void OnClose()
    {
        fundoBloqueadorDeAcoesDoMenu.gameObject.SetActive(false);
    }

    public void AbrirTelaAprenderAtaque(ComandoDeAtaque attackNovo)
    {
        ataqueParaAprender = new AttackHolder(attackNovo);

        OpenView();

        IniciarAtaqueSlots();

        fundoDialogo.gameObject.SetActive(false);
    }

    private void FecharMenu()
    {
        CloseView();

        FecharAtaqueInfo();
    }

    private void IniciarAtaqueSlots()
    {
        for (int i = 0; i < ataqueSlots.Count; i++)
        {
            if (i < monstroAtual.Attacks.Count)
            {
                ataqueSlots[i].gameObject.SetActive(true);
                ataqueSlots[i].AtualizarInformacoes(i, monstroAtual, monstroAtual.Attacks[i]);
                ataqueSlots[i].Ativado(true);

            }
            else
            {
                ataqueSlots[i].gameObject.SetActive(false);
                ataqueSlots[i].ResetarInformacoes();
            }
        }

        ataqueParaAprenderSlot.gameObject.SetActive(true);
        ataqueParaAprenderSlot.AtualizarInformacoes(4, monstroAtual, ataqueParaAprender);
        ataqueParaAprenderSlot.Ativado(true);

        imagemBotaoConfirmarEscolha.raycastTarget = false;
        botaoConfirmarEscolha.interactable = false;
        textoBotaoConfirmarEscolha.text = textoSelecioneUmAtaque;
    }

    public AprenderAtaque VerificarAprenderAtaquePorItem(Monster monster, ComandoDeAtaque attackNovo)
    {
        monstroAtual = monster;

        if (monstroAtual.VerificarSePossuiAtaque(attackNovo) == false)
        {
            if (VerificarSePodeAprenderAtaque(monster, attackNovo) == true)
            {
                if (monstroAtual.Attacks.Count >= 4) // Caso nao possua slots livres
                {
                    return AprenderAtaque.PodeAprenderAtaque;
                }
                else
                {
                    monstroAtual.Attacks.Add(new AttackHolder(attackNovo)); // So adiciona o ataque no fim da lista
                    return AprenderAtaque.AcabouAprenderAtaque;
                }
            }
            else
            {
                //N�o pode aprender determinado ataque
                return AprenderAtaque.NaoPodeAprenderAtaque;
            }
        }
        else
        {
            //Ja possui ataque
            return AprenderAtaque.JaSabeAtaque;
        }
    }

    public void VerificarAprenderAtaqueLevelUp(Monster monster, ComandoDeAtaque attackNovo)
    {
        monstroAtual = monster;

        if (monstroAtual.VerificarSePossuiAtaque(attackNovo) == false)
        {
            if (monstroAtual.Attacks.Count >= 4) // Caso nao possua slots livres
            {
                BattleManager.Instance.SetNomeDoMonstro(monster.NickName);
                BattleManager.Instance.SetNomeDoComando(attackNovo.Nome);
                BattleManager.Instance.AbrirDialogo(dialogoQuerEsquecerHabilidade);
            }
            else
            {
                monstroAtual.Attacks.Add(new AttackHolder(attackNovo)); // So adiciona o ataque no fim da lista

                BattleManager.Instance.SetNomeDoMonstro(monster.NickName);
                BattleManager.Instance.SetNomeDoComando(attackNovo.Nome);
                BattleManager.Instance.AbrirDialogo(dialogoAprendeuHabilidade);
            }
        }
    }

    private bool VerificarSePodeAprenderAtaque(Monster monster, ComandoDeAtaque attackNovo)
    {
        foreach (ComandoDeAtaque comandoDeAtaque in monster.MonsterData.GetMonsterLearnableAttacks)
        {
            if (comandoDeAtaque.ID == attackNovo.ID)
                return true;
        }

        foreach (UpgradesPerLevel attacksPerLevel in monster.MonsterData.GetMonsterUpgradesPerLevel)
        {
            if (attacksPerLevel.Ataque?.ID == attackNovo.ID)
                return true;
        }

        return false;
    }

    private void EscolherAtaqueParaSubistituir(int indiceSlot)
    {
        imagemBotaoConfirmarEscolha.raycastTarget = true;
        botaoConfirmarEscolha.interactable = true;
        indice = indiceSlot;

        if (indice >= 4) //N�o aprender ataque        
            textoBotaoConfirmarEscolha.text = textoNaoAprender + " " + ataqueParaAprender.Attack.Nome;
        else
            textoBotaoConfirmarEscolha.text = textoEsquecer + " " + ataqueSlots[indice].AttackHolder.Attack.Nome;
    }

    public void ConfirmarEscolha() //Evento
    {
        StartCoroutine(ConfirmarEscolhaCorrotina());
    }

    private void AbrirAtaqueInfo(int indice)
    {
        ataqueInfo.gameObject.SetActive(true);

        if (indice >= 4)
            ataqueInfo.AtualizarInformacoes(ataqueParaAprender.Attack);
        else
            ataqueInfo.AtualizarInformacoes(monstroAtual.Attacks[indice].Attack);
    }

    public void FecharAtaqueInfo()
    {
        ataqueInfo.gameObject.SetActive(false);
    }

    

    private void AbrirDialogo(DialogueObject dialogo)
    {
        dialogueActivator.ShowDialogue(dialogo, dialogueUI);

        fundoDialogo.gameObject.SetActive(true);

        StartCoroutine(DialogoAberto(dialogueUI));
    }

    private void FecharDialogo()
    {
        fundoDialogo.gameObject.SetActive(false);
    }

    private IEnumerator DialogoAberto(DialogueUI dialogueUI)
    {
        yield return new WaitUntil(() => dialogueUI.IsOpen == false);

        FecharDialogo();
    }

    private IEnumerator ConfirmarEscolhaCorrotina()
    {
        string nomeAtaqueAntigo;

        if (indice <= 3)
        {
            nomeAtaqueAntigo = monstroAtual.Attacks[indice].Attack.Nome;

            monstroAtual.Attacks[indice] = ataqueParaAprender;

            dialogueUI.SetPlaceholderDeTexto("%comando", nomeAtaqueAntigo);
            dialogueUI.SetPlaceholderDeTexto("%move", ataqueParaAprender.Attack.Nome);

            AbrirDialogo(dialogoEsqueceuEAprendeuHabilidade);
        }
        else
        {
            dialogueUI.SetPlaceholderDeTexto("%comando", ataqueParaAprender.Attack.Nome);

            AbrirDialogo(dialogoNaoAprendeuHabilidade);
        }

        yield return new WaitUntil(() => dialogueUI.IsOpen == false);

        FecharMenu();
    }
}
