using System.Collections;
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
    [SerializeField] private RectTransform fundoBloqueadorDeAcoesDoMenu;

    //Variaveis
    private List<SaveSlot> saveSlots = new List<SaveSlot>();

    private int indiceSlotAtual;
    private SaveData saveAtual;

    protected override void OnAwake()
    {
        menuConfirmacaoCarregarSave.gameObject.SetActive(false);
        menuConfirmacaoExcluirSave.gameObject.SetActive(false);
        fundoBloqueadorDeAcoesDoMenu.gameObject.SetActive(false);

        FecharMenusSuspensos();

        saveAtual = null;

        onOpen += IniciarMenu;

        foreach (SaveSlot saveSlot in saveSlotsHolder.GetComponentsInChildren<SaveSlot>(true))
        {
            saveSlot.EventoSlotSelecionado.AddListener(SetarCarregamento);
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

    public void ConfirmarExcluirSave()
    {
        SaveManager.ExcluirSave(indiceSlotAtual);

        IniciarSaveSlots();

        FecharMenusSuspensos();
    }

    public void FecharMenusSuspensos()
    {
        menuConfirmacaoCarregarSave.gameObject.SetActive(false);
        menuConfirmacaoExcluirSave.gameObject.SetActive(false);
    }

    private void Carregar()
    {
        SaveManager.CarregarInformacoesDoSave(saveAtual, playerSO);

        FecharMenusSuspensos();

        canvasGroup.blocksRaycasts = false;

        FazerTransicaoProMapaDoSave();
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
