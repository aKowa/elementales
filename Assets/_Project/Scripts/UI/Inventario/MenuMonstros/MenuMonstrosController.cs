using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuMonstrosController : ViewController
{
    //Componentes

    [Header("Componentes")]
    [SerializeField] private GameObject monstroSlotBase;
    [SerializeField] private Transform gridSlotsHolder;
    [SerializeField] private Transform monstroSlotsHolder;
    [SerializeField] private RectTransform menuOpcoes;
    [SerializeField] private RectTransform menuOpcoesSuspenso;
    [SerializeField] private RectTransform fundoBloqueadorDeAcoesDoMenu;
    [SerializeField] private RectTransform fundoBloqueadorDeAcoes;

    [Header("Outros Menus")]
    [SerializeField] private MenuSummaryController menuSummaryController;

    private Inventario inventario;

    //Variaveis
    private List<MonstroGridSlot> gridSlots = new List<MonstroGridSlot>();
    private List<MonstroSlot> monstroSlots = new List<MonstroSlot>();

    private int indiceMonstroAtual;

    private int objetosSeMovendo;
    private Coroutine corrotinaObjetoSeMovendo;

    //Getters
    public Inventario Inventario => inventario;
    public List<MonstroGridSlot> GridSlots => gridSlots;
    public List<MonstroSlot> MonstroSlots => monstroSlots;

    public int IndiceMonstroAtual
    {
        get => indiceMonstroAtual;
        set => indiceMonstroAtual = value;
    }

    protected override void OnAwake()
    {
        monstroSlotBase.SetActive(false);
        menuOpcoes.gameObject.SetActive(false);
        fundoBloqueadorDeAcoesDoMenu.gameObject.SetActive(false);
        fundoBloqueadorDeAcoes.gameObject.SetActive(false);

        objetosSeMovendo = 0;

        onLateOpen += IniciarMenu;

        foreach (MonstroGridSlot slot in gridSlotsHolder.GetComponentsInChildren<MonstroGridSlot>(true))
        {
            gridSlots.Add(slot);
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
    }

    protected override void OnClose()
    {
        fundoBloqueadorDeAcoesDoMenu.gameObject.SetActive(false);

        ResetarMonstroSlots();
    }

    private void IniciarMenu()
    {
        for(int i = 0; i < gridSlots.Count; i++)
        {
            gridSlots[i].Iniciar(this, i);
        }

        ResetarMonstroSlots();

        for (int i = 0; i < inventario.MonsterBag.Count; i++)
        {
            MonstroSlot monstroSlot = Instantiate(monstroSlotBase).GetComponent<MonstroSlot>();
            monstroSlot.GetComponent<RectTransform>().SetParent(monstroSlotsHolder.transform, false);
            monstroSlot.gameObject.SetActive(true);

            monstroSlot.MenuMonstrosController = this;
            monstroSlot.Indice = i;
            monstroSlot.Monstro = inventario.MonsterBag[i];

            monstroSlot.transform.position = gridSlots[i].transform.position;

            monstroSlot.AtualizarInformacoes();

            monstroSlots.Add(monstroSlot);
        }

        fundoBloqueadorDeAcoes.gameObject.SetActive(false);
    }

    private void ResetarMonstroSlots()
    {
        foreach (MonstroSlot slot in monstroSlots)
        {
            Destroy(slot.gameObject);
        }

        monstroSlots.Clear();
    }

    public void MonstroSlotsRaycast(bool ativar)
    {
        for(int i = 0; i < monstroSlots.Count; i++)
        {
            monstroSlots[i].CanvasGroup.blocksRaycasts = ativar;
        }
    }

    public void AtualizarInformacoes()
    {
        for (int i = 0; i < monstroSlots.Count; i++)
        {
            monstroSlots[i].AtualizarInformacoes();
        }
    }

    public void AbrirMenuOpcoes(int indiceMonstro)
    {
        indiceMonstroAtual = indiceMonstro;

        menuOpcoes.gameObject.SetActive(true);

        RectTransform rectTransformMonstro = monstroSlots[indiceMonstro].GetComponent<RectTransform>();
        Rect retanguloSlotMonstro = BergamotaLibrary.LiBergamota.GetWorldRect(rectTransformMonstro);

        menuOpcoesSuspenso.transform.position = new Vector2(rectTransformMonstro.position.x + (retanguloSlotMonstro.size.x / 4), rectTransformMonstro.position.y + (retanguloSlotMonstro.size.y / 4));
    }

    public void FecharMenuOpcoes()
    {
        menuOpcoes.gameObject.SetActive(false);
    }

    public void AbrirSummary()
    {
        FecharMenuOpcoes();

        menuSummaryController.IniciarMenu(IndiceMonstroAtual);
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

    private IEnumerator ObjetoSeMovendo()
    {
        fundoBloqueadorDeAcoes.gameObject.SetActive(true);

        while(objetosSeMovendo > 0)
        {
            yield return null;
        }

        fundoBloqueadorDeAcoes.gameObject.SetActive(false);

        corrotinaObjetoSeMovendo = null;
    }
}
