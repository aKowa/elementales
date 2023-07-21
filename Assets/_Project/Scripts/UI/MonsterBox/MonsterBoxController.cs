using BergamotaDialogueSystem;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MonsterBoxController : ViewController
{
    //Componentes
    [Header("Componentes")]
    [SerializeField] private GameObject monstroSlotBase;

    [SerializeField] private TMP_Text nomeBox;

    [SerializeField] private Transform boxGridSlotsHolder;
    [SerializeField] private Transform partyGridSlotsHolder;
    [SerializeField] private Transform monstroSlotsHolder;
    [SerializeField] private RectTransform menuOpcoesBox;
    [SerializeField] private RectTransform menuOpcoesParty;
    [SerializeField] private RectTransform menuSoltarMonstro;
    [SerializeField] private RectTransform fundoDialogo;
    [SerializeField] private RectTransform fundoBloqueadorDeAcoesDoMenu;
    [SerializeField] private RectTransform fundoBloqueadorDeAcoes;

    [Header("Monstro Info")]
    [SerializeField] private GameObject tipoLogoBase;
    [SerializeField] private RectTransform monstroInfo;
    [SerializeField] private TMP_Text nomeMonstro;
    [SerializeField] private TMP_Text levelMontro;
    [SerializeField] private Animator animatorMonstro;
    [SerializeField] private Transform monstroInfoTiposHolder;

    [Header("Outros Menus")]
    [SerializeField] private MenuSummaryController menuSummaryController;

    private Inventario inventario;
    private DialogueActivator dialogueActivator;
    private DialogueUI dialogueUI;

    //Enums
    public enum TipoSlot { Box, Party }

    //Variaveis
    [Header("Dialogos")]
    [SerializeField] private DialogueObject dialogoHealthyMonster;
    [SerializeField] private DialogueObject dialogoReleasingMonster;
    [SerializeField] private DialogueObject dialogoPartyCheia;

    private List<BoxMonstroGridSlot> boxGridSlots = new List<BoxMonstroGridSlot>();
    private List<BoxMonstroSlot> boxMonstroSlots = new List<BoxMonstroSlot>();
    private List<BoxMonstroGridSlot> partyGridSlots = new List<BoxMonstroGridSlot>();
    private List<BoxMonstroSlot> partyMonstroSlots = new List<BoxMonstroSlot>();

    private List<TipoLogo> monstroInfoTipoLogos = new List<TipoLogo>();

    private int indiceBox;

    private TipoSlot tipoMonstroAtual;
    private int indiceMonstroAtual;
    private Monster monstroAtual;

    private int objetosSeMovendo;
    private Coroutine corrotinaObjetoSeMovendo;

    //Getters
    public int IndiceBox => indiceBox;
    public Inventario Inventario => inventario;
    public List<BoxMonstroSlot> BoxMonstroSlots => boxMonstroSlots;
    public List<BoxMonstroSlot> PartyMonstroSlots => partyMonstroSlots;

    protected override void OnAwake()
    {
        //Componentes
        dialogueActivator = GetComponent<DialogueActivator>();
        dialogueUI = FindObjectOfType<DialogueUI>();

        monstroSlotBase.SetActive(false);
        menuOpcoesBox.gameObject.SetActive(false);
        menuOpcoesParty.gameObject.SetActive(false);
        menuSoltarMonstro.gameObject.SetActive(false);
        fundoDialogo.gameObject.SetActive(false);
        fundoBloqueadorDeAcoesDoMenu.gameObject.SetActive(false);
        fundoBloqueadorDeAcoes.gameObject.SetActive(false);

        FecharMenuOpcoes();

        onLateOpen += IniciarMenu;

        foreach (BoxMonstroGridSlot slot in boxGridSlotsHolder.GetComponentsInChildren<BoxMonstroGridSlot>(true))
        {
            boxGridSlots.Add(slot);
        }

        foreach (BoxMonstroGridSlot slot in partyGridSlotsHolder.GetComponentsInChildren<BoxMonstroGridSlot>(true))
        {
            partyGridSlots.Add(slot);
        }

        for (int i = 0; i < partyGridSlots.Count; i++)
        {
            partyGridSlots[i].Iniciar(this, indiceBox, i, TipoSlot.Party);
        }
    }

    protected override void OnStart()
    {
        //Componentes
        inventario = PlayerData.Instance.Inventario;
    }

    public override void OnOpen()
    {
        fundoBloqueadorDeAcoesDoMenu.gameObject.SetActive(true);

        BergamotaLibrary.PauseManager.Pausar(true);
    }

    protected override void OnClose()
    {
        fundoBloqueadorDeAcoesDoMenu.gameObject.SetActive(false);

        BergamotaLibrary.PauseManager.Pausar(false);

        ResetarInformacoes();

        ResetarMonstroSlots();

        inventario.ChecarSePrecisaDeUmNovoMonsterBank();
    }

    private void IniciarMenu()
    {
        ResetarMonstroSlots();

        TrocarBox(0);

        CriarPartyMonstroSlots();

        fundoDialogo.gameObject.SetActive(false);
        fundoBloqueadorDeAcoes.gameObject.SetActive(false);
    }

    private void CriarBoxMonstroSlots()
    {
        for (int i = 0; i < inventario.MonsterBank[indiceBox].Monsters.Length; i++)
        {
            if (inventario.MonsterBank[indiceBox].Monsters[i] == null)
            {
                continue;
            }

            BoxMonstroSlot monstroSlot = Instantiate(monstroSlotBase, monstroSlotsHolder).GetComponent<BoxMonstroSlot>();
            monstroSlot.gameObject.SetActive(true);

            monstroSlot.Iniciar(this, i, TipoSlot.Box, inventario.MonsterBank[indiceBox].Monsters[i]);

            monstroSlot.transform.position = boxGridSlots[i].transform.position;

            boxMonstroSlots.Add(monstroSlot);
        }
    }

    private void CriarPartyMonstroSlots()
    {
        for (int i = 0; i < inventario.MonsterBag.Count; i++)
        {
            BoxMonstroSlot monstroSlot = Instantiate(monstroSlotBase, monstroSlotsHolder).GetComponent<BoxMonstroSlot>();
            monstroSlot.gameObject.SetActive(true);

            monstroSlot.Iniciar(this, i, TipoSlot.Party, inventario.MonsterBag[i]);

            monstroSlot.transform.position = partyGridSlots[i].transform.position;

            partyMonstroSlots.Add(monstroSlot);
        }
    }

    private void ResetarBoxMonstroSlots()
    {
        foreach (BoxMonstroSlot slot in boxMonstroSlots)
        {
            Destroy(slot.gameObject);
        }

        boxMonstroSlots.Clear();
    }

    private void ResetarMonstroSlots()
    {
        ResetarBoxMonstroSlots();

        foreach (BoxMonstroSlot slot in partyMonstroSlots)
        {
            Destroy(slot.gameObject);
        }

        partyMonstroSlots.Clear();
    }

    private void AtualizarInformacoesDaBox()
    {
        nomeBox.text = inventario.MonsterBank[indiceBox].BoxName;
    }

    private void ResetarInformacoes()
    {
        nomeBox.text = string.Empty;

        ResetarMonstroInfo();
    }

    private void TrocarBox(int indice)
    {
        indiceBox = indice;
        IniciarBoxGrids();

        ResetarBoxMonstroSlots();
        CriarBoxMonstroSlots();

        AtualizarInformacoesDaBox();
    }

    private void IniciarBoxGrids()
    {
        for (int i = 0; i < boxGridSlots.Count; i++)
        {
            boxGridSlots[i].Iniciar(this, indiceBox, i, TipoSlot.Box);
        }
    }

    public void AbrirMenuOpcoes(TipoSlot tipo, int indiceMonstro)
    {
        tipoMonstroAtual = tipo;
        indiceMonstroAtual = indiceMonstro;

        switch(tipoMonstroAtual)
        {
            case TipoSlot.Box:
                monstroAtual = inventario.MonsterBank[indiceBox].Monsters[indiceMonstroAtual];
                menuOpcoesBox.gameObject.SetActive(true);
                break;

            case TipoSlot.Party:
                monstroAtual = inventario.MonsterBag[indiceMonstroAtual];
                menuOpcoesParty.gameObject.SetActive(true);
                break;
        }

        monstroInfo.gameObject.SetActive(true);
        AtualizarMonstroInfo();
    }

    public void FecharMenuOpcoes()
    {
        menuOpcoesBox.gameObject.SetActive(false);
        menuOpcoesParty.gameObject.SetActive(false);
        monstroInfo.gameObject.SetActive(false);
    }

    private void AtualizarMonstroInfo()
    {
        nomeMonstro.text = monstroAtual.NickName;
        levelMontro.text = monstroAtual.Nivel.ToString();

        if (monstroAtual.MonsterData.Animator != null)
        {
            animatorMonstro.runtimeAnimatorController = monstroAtual.MonsterData.Animator;
        }
        else
        {
            Debug.LogWarning("O monstro " + monstroAtual.MonsterData.GetName + " nao possui um Animator Override Controller para ser usado!");
        }

        AtualizarTipo(monstroAtual);
    }

    private void ResetarMonstroInfo()
    {
        nomeMonstro.text = string.Empty;
        levelMontro.text = string.Empty;
        animatorMonstro.runtimeAnimatorController = null;

        ResetarTipoLogos();
    }

    private void AtualizarTipo(Monster monstro)
    {
        ResetarTipoLogos();

        for (int i = 0; i < monstro.MonsterData.GetMonsterTypes.Count; i++)
        {
            TipoLogo tipoLogo = Instantiate(tipoLogoBase, monstroInfoTiposHolder).GetComponent<TipoLogo>();
            tipoLogo.gameObject.SetActive(true);

            tipoLogo.SetTipo(monstro.MonsterData.GetMonsterTypes[i]);

            monstroInfoTipoLogos.Add(tipoLogo);
        }
    }

    private void ResetarTipoLogos()
    {
        foreach (TipoLogo tipoLogo in monstroInfoTipoLogos)
        {
            Destroy(tipoLogo.gameObject);
        }

        monstroInfoTipoLogos.Clear();
    }

    public bool TemOutroMonstroSaudavelNaParty(Monster monstro)
    {
        for(int i = 0; i < inventario.MonsterBag.Count; i++)
        {
            if (inventario.MonsterBag[i].IsFainted == false && inventario.MonsterBag[i] != monstro)
            {
                return true;
            }
        }

        return false;
    }

    public void MoverParaBoxOuParty()
    {
        switch (tipoMonstroAtual)
        {
            case TipoSlot.Box:
                MoverDaBoxParaAParty();
                break;

            case TipoSlot.Party:
                MoverDaPartyParaABox();
                break;
        }
    }

    private void MoverDaBoxParaAParty()
    {
        BoxMonstroSlot monstroSlotAtual = GetMonstroSlot(tipoMonstroAtual, indiceMonstroAtual);

        if (inventario.MonsterBag.Count < Inventario.numeroMonstrosBagMax)
        {
            inventario.MonsterBank[indiceBox].Monsters[indiceMonstroAtual] = null;
            monstroSlotAtual.MoverMonstroParaLista(inventario.MonsterBag.Count, TipoSlot.Party);

            FecharMenuOpcoes();
        }
        else
        {
            AbrirDialogo(dialogoPartyCheia);
        }
    }

    private void MoverDaPartyParaABox()
    {
        if(TemOutroMonstroSaudavelNaParty(monstroAtual) == false)
        {
            DialogoHealthyMonster();
            return;
        }

        BoxMonstroSlot monstroSlotAtual = GetMonstroSlot(tipoMonstroAtual, indiceMonstroAtual);

        for (int i = 0; i < inventario.MonsterBank.Count; i++)
        {
            for (int y = 0; y < inventario.MonsterBank[i].Monsters.Length; y++)
            {
                if (inventario.MonsterBank[i].Monsters[y] == null)
                {
                    if(i != indiceBox)
                    {
                        TrocarBox(i);
                    }

                    inventario.MonsterBag.RemoveAt(indiceMonstroAtual);
                    monstroSlotAtual.MoverMonstroParaLista(y, TipoSlot.Box);

                    AjustarMonsterBag();

                    FecharMenuOpcoes();

                    return;
                }
            }
        }

        inventario.CreateNewMonsterBank();
        TrocarBox(inventario.MonsterBank.Count - 1);

        inventario.MonsterBag.RemoveAt(indiceMonstroAtual);
        monstroSlotAtual.MoverMonstroParaLista(0, TipoSlot.Box);

        AjustarMonsterBag();

        FecharMenuOpcoes();
    }

    public void AbrirMenuSoltarMonstro()
    {
        menuSoltarMonstro.gameObject.SetActive(true);

        menuOpcoesBox.gameObject.SetActive(false);
        menuOpcoesParty.gameObject.SetActive(false);
    }

    public void FecharMenuSoltarMonstro()
    {
        menuSoltarMonstro.gameObject.SetActive(false);

        switch (tipoMonstroAtual)
        {
            case TipoSlot.Box:
                menuOpcoesBox.gameObject.SetActive(true);
                break;

            case TipoSlot.Party:
                menuOpcoesParty.gameObject.SetActive(true);
                break;
        }
    }

    public void SoltarMonstro()
    {
        BoxMonstroSlot monstroSlotAtual = GetMonstroSlot(tipoMonstroAtual, indiceMonstroAtual);

        FecharMenuSoltarMonstro();

        switch (tipoMonstroAtual)
        {
            case TipoSlot.Box:
                AbrirDialogo(dialogoReleasingMonster);

                boxMonstroSlots.Remove(monstroSlotAtual);
                monstroSlotAtual.AnimacaoDestruicao();
                inventario.MonsterBank[indiceBox].Monsters[indiceMonstroAtual] = null;

                FecharMenuOpcoes();
                break;

            case TipoSlot.Party:
                if(TemOutroMonstroSaudavelNaParty(monstroAtual) == true)
                {
                    AbrirDialogo(dialogoReleasingMonster);

                    partyMonstroSlots.Remove(monstroSlotAtual);
                    monstroSlotAtual.AnimacaoDestruicao();
                    inventario.MonsterBag.RemoveAt(indiceMonstroAtual);

                    AjustarMonsterBag();

                    FecharMenuOpcoes();
                }
                else
                {
                    DialogoHealthyMonster();
                }
                break;
        }
    }

    public void AbrirSummary()
    {
        menuSummaryController.IniciarMenuSemBotoesMonstro(monstroAtual);
    }

    public void MonstroSlotsRaycast(bool ativar)
    {
        for (int i = 0; i < boxMonstroSlots.Count; i++)
        {
            boxMonstroSlots[i].CanvasGroup.blocksRaycasts = ativar;
        }

        for (int i = 0; i < partyMonstroSlots.Count; i++)
        {
            partyMonstroSlots[i].CanvasGroup.blocksRaycasts = ativar;
        }
    }

    public BoxMonstroGridSlot GetBoxMonstroGridSlot(int indice, TipoSlot tipoSlot)
    {
        switch (tipoSlot)
        {
            case TipoSlot.Box:
                foreach(BoxMonstroGridSlot monstroGridSlot in boxGridSlots)
                {
                    if(monstroGridSlot.Indice == indice)
                    {
                        return monstroGridSlot;
                    }
                }

                Debug.LogError("O BoxMonstroGridSlot na Box com indice " + indice + " nao foi encontrado!");
                return null;

            case TipoSlot.Party:
                foreach (BoxMonstroGridSlot monstroGridSlot in partyGridSlots)
                {
                    if (monstroGridSlot.Indice == indice)
                    {
                        return monstroGridSlot;
                    }
                }

                Debug.LogError("O BoxMonstroGridSlot na Party com indice " + indice + " nao foi encontrado!");
                return null;

            default:
                Debug.LogError("Nao foi passado um tipo de slot valido!");
                return null;
        }
    }

    public void AjustarMonsterBag()
    {
        bool achouOMostro;

        for (int i = 0; i < inventario.MonsterBag.Count; i++)
        {
            achouOMostro = false;

            for(int y = i; y < Inventario.numeroMonstrosBagMax; y++)
            {
                for(int z = 0; z < partyMonstroSlots.Count; z++)
                {
                    if (partyMonstroSlots[z].Indice == y)
                    {
                        partyMonstroSlots[z].Indice = i;
                        partyMonstroSlots[z].MoverAte(partyGridSlots[i].transform.position);

                        achouOMostro = true;
                        break;
                    }
                }

                if(achouOMostro == true)
                {
                    break;
                }
            }
        }
    }

    public void BoxSeguinte()
    {
        indiceBox++;

        if(indiceBox >= inventario.MonsterBank.Count)
        {
            indiceBox = 0;
        }

        TrocarBox(indiceBox);
    }

    public void BoxAnterior()
    {
        indiceBox--;

        if (indiceBox < 0)
        {
            indiceBox = inventario.MonsterBank.Count - 1;
        }

        TrocarBox(indiceBox);
    }

    public BoxMonstroSlot GetMonstroSlot(TipoSlot tipo, int indice)
    {
        switch (tipo)
        {
            case TipoSlot.Box:
                foreach (BoxMonstroSlot monstroSlot in boxMonstroSlots)
                {
                    if (monstroSlot.Indice == indice)
                    {
                        return monstroSlot;
                    }
                }

                return null;

            case TipoSlot.Party:
                foreach (BoxMonstroSlot monstroSlot in partyMonstroSlots)
                {
                    if (monstroSlot.Indice == indice)
                    {
                        return monstroSlot;
                    }
                }

                return null;

            default:
                return null;
        }
    }

    public void AdicionarObjetoSeMovendo()
    {
        objetosSeMovendo++;

        if (corrotinaObjetoSeMovendo == null)
        {
            corrotinaObjetoSeMovendo = StartCoroutine(ObjetoSeMovendo());
        }
    }

    public void SubtrairObjetoSeMovendo()
    {
        objetosSeMovendo--;
    }

    public void DialogoHealthyMonster()
    {
        AbrirDialogo(dialogoHealthyMonster);
    }

    public void AbrirDialogo(DialogueObject dialogo)
    {
        if(monstroAtual != null)
        {
            dialogueUI.SetPlaceholderDeTexto("%monstro", monstroAtual.NickName);
        }

        dialogueActivator.ShowDialogue(dialogo, dialogueUI);

        fundoDialogo.gameObject.SetActive(true);

        StartCoroutine(DialogoAberto(dialogueUI));
    }

    private void FecharDialogo()
    {
        fundoDialogo.gameObject.SetActive(false);
    }

    private IEnumerator ObjetoSeMovendo()
    {
        fundoBloqueadorDeAcoes.gameObject.SetActive(true);

        while (objetosSeMovendo > 0)
        {
            yield return null;
        }

        fundoBloqueadorDeAcoes.gameObject.SetActive(false);

        corrotinaObjetoSeMovendo = null;
    }

    private IEnumerator DialogoAberto(DialogueUI dialogueUI)
    {
        while (dialogueUI.IsOpen == true)
        {
            yield return null;
        }

        FecharDialogo();
    }
}
