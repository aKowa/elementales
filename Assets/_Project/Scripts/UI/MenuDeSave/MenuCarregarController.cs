using System.Collections.Generic;
using UnityEngine;

public class MenuCarregarController : ViewController
{
    //Componentes
    [Header("Player SO")]
    [SerializeField] private PlayerSO playerSO;

    [Header("Componentes")]
    [SerializeField] private Transform saveSlotsHolder;
    [SerializeField] private RectTransform menuConfirmacaoCarregarSave;
    [SerializeField] private RectTransform menuConfirmacaoExcluirSave;
    [SerializeField] private RectTransform menuExportarSave;
    [SerializeField] private RectTransform menuImportarSave;
    [SerializeField] private RectTransform menuImportacaoSucesso;
    [SerializeField] private RectTransform menuImportacaoFalhou;
    [SerializeField] private RectTransform fundoBloqueadorDeAcoesDoMenu;

    //Variaveis
    private List<SaveSlot> saveSlots = new List<SaveSlot>();

    private int indiceSlotAtual;
    private SaveData saveAtual;

    private ManipuladorDeSave manipuladorDeSave;

    protected override void OnAwake()
    {
        fundoBloqueadorDeAcoesDoMenu.gameObject.SetActive(false);
        
        FecharMenusSuspensos();

        saveAtual = null;

        onOpen += IniciarMenu;

        foreach (SaveSlot saveSlot in saveSlotsHolder.GetComponentsInChildren<SaveSlot>(true))
        {
            saveSlot.EventoSlotSelecionado.AddListener(SetarCarregamento);
            saveSlot.EventoBotaoExcluirSelecionado.AddListener(ExcluirSave);
            saveSlot.EventoBotaoExportarSelecionado.AddListener(ExportarSave);
            saveSlot.EventoBotaoImportarSelecionado.AddListener(ImportarSave);
            saveSlots.Add(saveSlot);
        }

        manipuladorDeSave = GetComponent<ManipuladorDeSave>();
    }

    public override void OnOpen()
    {
        fundoBloqueadorDeAcoesDoMenu.gameObject.SetActive(true);
    }

    protected override void OnClose()
    {
        fundoBloqueadorDeAcoesDoMenu.gameObject.SetActive(false);

        ResetarSaveSlots();
    }

    private void IniciarMenu()
    {
        IniciarSaveSlots();
    }

    private void IniciarSaveSlots()
    {
        ResetarSaveSlots();

        for (int i = 0; i < saveSlots.Count; i++)
        {
            if (SaveManager.SaveExiste(i + 1) == true)
            {
                SaveData saveData = SaveManager.Carregar(i + 1);

                if(saveData != null)
                {
                    saveSlots[i].AtualizarInformacoes(saveData);

                    saveSlots[i].SetAtivo(true);
                }
                else
                {
                    saveSlots[i].SlotCorrompido();

                    saveSlots[i].SetAtivo(false);
                }
            }
            else
            {
                saveSlots[i].ResetarInformacoes();

                saveSlots[i].SetAtivo(false);
            }

            saveSlots[i].NumeroSlot = i + 1;
        }
    }

    private void ResetarSaveSlots()
    {
        foreach (SaveSlot slot in saveSlots)
        {
            slot.ResetarInformacoes();
        }
    }

    private void SetarCarregamento(int slot, SaveData save)
    {
        saveAtual = save;

        menuConfirmacaoCarregarSave.gameObject.SetActive(true);
    }

    public void ConfirmarCarregar()
    {
        Carregar();
    }

    private void ExcluirSave(int slot)
    {
        indiceSlotAtual = slot;

        menuConfirmacaoExcluirSave.gameObject.SetActive(true);
    }
    
    private void ExportarSave(int slot)
    {
        indiceSlotAtual = slot;

        menuExportarSave.gameObject.SetActive(true);
    }

    private void ImportarSave(int slot)
    {
        indiceSlotAtual = slot;

        menuImportarSave.gameObject.SetActive(true);
    }
    
    public void ConfirmarExcluirSave()
    {
        SaveManager.ExcluirSave(indiceSlotAtual);

        IniciarSaveSlots();

        FecharMenusSuspensos();
    }

    public void ConfirmarExportarSave()
    {
        StartCoroutine(manipuladorDeSave.ShareSaveFile(indiceSlotAtual));
        
        FecharMenusSuspensos();
    }

    public void ConfirmarImportarSave()
    {
        FecharMenusSuspensos();

        manipuladorDeSave.ImportFile(indiceSlotAtual);
    }

    public void FecharMenusSuspensos()
    {
        menuConfirmacaoCarregarSave.gameObject.SetActive(false);
        menuConfirmacaoExcluirSave.gameObject.SetActive(false);
        menuExportarSave.gameObject.SetActive(false);
        menuImportarSave.gameObject.SetActive(false);
        menuImportacaoSucesso.gameObject.SetActive(false);
        menuImportacaoFalhou.gameObject.SetActive(false);
    }

    private void Carregar()
    {
        SaveManager.CarregarInformacoesDoSave(saveAtual, playerSO);

        FecharMenusSuspensos();

        canvasGroup.blocksRaycasts = false;

        FazerTransicaoProMapaDoSave();
    }

    public void ImportarSave(string caminhoDoSave, int slot)
    {
        SaveData saveData = SaveManager.Carregar(caminhoDoSave);

        if (saveData != null)
        {
            if (SaveManager.Salvar(saveData, slot) == true)
            {
                menuImportacaoSucesso.gameObject.SetActive(true);
            }
            else
            {
                menuImportacaoFalhou.gameObject.SetActive(true);
            }
        }
        else
        {
            menuImportacaoFalhou.gameObject.SetActive(true);
        }

        IniciarSaveSlots();
    }

    private void FazerTransicaoProMapaDoSave()
    {
        BergamotaLibrary.PauseManager.PermitirInput = false;

        Transition.GetInstance().DoTransition("FadeIn", 0, () =>
        {
            MapsManager.GetInstance().TakeDoor(saveAtual.sceneInfo.gateway, () =>
            {
                Transition.GetInstance().DoTransition("FadeOut", 0.5f, () =>
                {
                    FindObjectOfType<JanelaMapaAtual>(true).MostrarNomeDoMapa();
                    BergamotaLibrary.PauseManager.PermitirInput = true;
                });
            });
        });
    }
}