using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuSalvarController : ViewController
{
    //Componentes
    [Header("Player SO")]
    [SerializeField] private PlayerSO playerSO;

    [Header("Componentes")]
    [SerializeField] private Transform saveSlotsHolder;
    [SerializeField] private RectTransform menuConfirmacaoSobrescreverSave;
    [SerializeField] private RectTransform menuSaveSucesso;
    [SerializeField] private RectTransform menuSaveFalhou;
    [SerializeField] private RectTransform menuConfirmacaoExcluirSave;
    [SerializeField] private RectTransform fundoBloqueadorDeAcoesDoMenu;

    //Variaveis
    private List<SaveSlot> saveSlots = new List<SaveSlot>();

    private int slotAtual;

    protected override void OnAwake()
    {
        menuConfirmacaoSobrescreverSave.gameObject.SetActive(false);
        menuSaveSucesso.gameObject.SetActive(false);
        menuSaveFalhou.gameObject.SetActive(false);
        menuConfirmacaoExcluirSave.gameObject.SetActive(false);
        fundoBloqueadorDeAcoesDoMenu.gameObject.SetActive(false);

        FecharMenusSuspensos();

        slotAtual = 0;

        onOpen += IniciarMenu;

        foreach (SaveSlot saveSlot in saveSlotsHolder.GetComponentsInChildren<SaveSlot>(true))
        {
            saveSlot.EventoSlotSelecionado.AddListener(SetarSalvamento);
            saveSlot.EventoBotaoExcluirSelecionado.AddListener(ExcluirSave);

            saveSlots.Add(saveSlot);
        }
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
                }
                else
                {
                    saveSlots[i].SlotCorrompido();
                }
            }
            else
            {
                saveSlots[i].ResetarInformacoes();
            }

            saveSlots[i].NumeroSlot = i + 1;
            saveSlots[i].SetAtivo(true);

            saveSlots[i].EsconderBotoesDeImportarExportar();
        }
    }

    private void ResetarSaveSlots()
    {
        foreach (SaveSlot slot in saveSlots)
        {
            slot.ResetarInformacoes();
            slot.EsconderBotoesDeImportarExportar();
        }
    }

    private void SetarSalvamento(int slot, SaveData save)
    {
        slotAtual = slot;

        if(save == null)
        {
            Salvar();
        }
        else
        {
            menuConfirmacaoSobrescreverSave.gameObject.SetActive(true);
        }
    }

    public void ConfirmarSave()
    {
        Salvar();
    }

    private void ExcluirSave(int slot)
    {
        slotAtual = slot;

        menuConfirmacaoExcluirSave.gameObject.SetActive(true);
    }

    public void ConfirmarExcluirSave()
    {
        SaveManager.ExcluirSave(slotAtual);

        IniciarSaveSlots();

        FecharMenusSuspensos();
    }

    public void FecharMenusSuspensos()
    {
        menuConfirmacaoSobrescreverSave.gameObject.SetActive(false);
        menuSaveFalhou.gameObject.SetActive(false);
        menuSaveSucesso.gameObject.SetActive(false);
        menuConfirmacaoExcluirSave.gameObject.SetActive(false);
    }

    private void Salvar()
    {
        SaveData novoSave = new SaveData(playerSO);

        if(SaveManager.Salvar(novoSave, slotAtual) == true)
        {
            menuConfirmacaoSobrescreverSave.gameObject.SetActive(false);
            menuSaveSucesso.gameObject.SetActive(true);
        }
        else
        {
            menuConfirmacaoSobrescreverSave.gameObject.SetActive(false);
            menuSaveFalhou.gameObject.SetActive(true);
        }

        IniciarSaveSlots();
    }
}
