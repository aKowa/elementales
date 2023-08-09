using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SaveSlot : MonoBehaviour
{
    //Componentes
    [Header("Componentes")]
    [SerializeField] private GameObject miniaturaBase;
    [SerializeField] private TMP_Text nome;
    [SerializeField] private TMP_Text tempoDeJogo;
    [SerializeField] private TMP_Text monstrosCapturados;
    [SerializeField] private TMP_Text hora;
    [SerializeField] private TMP_Text dia;
    [SerializeField] private TMP_Text local;
    [SerializeField] private Transform miniaturasHolder;

    [Space(10)]

    [SerializeField] private RectTransform botaoExcluirSave;
    [SerializeField] private RectTransform botaoExportarSave;
    [SerializeField] private RectTransform botaoImportarSave;

    private ButtonSelectionEffect buttonSelectionEffect;

    //Variaveis
    [Header("Variaveis Padroes")]
    [SerializeField] private string nomeSlotVazio = "Empty Save";
    [SerializeField] private string nomeSlotCorrompido = "Corrupted Save :(";

    private UnityEvent<int, SaveData> eventoSlotSelecionado = new UnityEvent<int, SaveData>();
    private UnityEvent<int> eventoBotaoExcluirSelecionado = new UnityEvent<int>();
    private UnityEvent<int> eventoBotaoExportarSelecionado = new UnityEvent<int>();
    private UnityEvent<int> eventoBotaoImportarSelecionado = new UnityEvent<int>();

    private List<Image> miniaturas = new List<Image>();

    private int numeroSlot;
    private bool ativo;

    private SaveData save;


    //Getters
    public UnityEvent<int, SaveData> EventoSlotSelecionado => eventoSlotSelecionado;
    public UnityEvent<int> EventoBotaoExcluirSelecionado => eventoBotaoExcluirSelecionado;
    public UnityEvent<int> EventoBotaoExportarSelecionado => eventoBotaoExportarSelecionado;
    public UnityEvent<int> EventoBotaoImportarSelecionado => eventoBotaoImportarSelecionado;

    public int NumeroSlot
    {
        get => numeroSlot;
        set => numeroSlot = value;
    }

    private void Awake()
    {
        //Componentes
        buttonSelectionEffect = GetComponent<ButtonSelectionEffect>();

        HoldButton holdButton = GetComponent<HoldButton>();

        holdButton.OnPointerUpEvent.AddListener(SlotSelecionado);

        miniaturaBase.SetActive(false);

        botaoExcluirSave.gameObject.SetActive(false);
        botaoExportarSave.gameObject.SetActive(false);
        botaoImportarSave.gameObject.SetActive(true);

        numeroSlot = 0;

        SetAtivo(false);
    }

    public void SetAtivo(bool ativo)
    {
        this.ativo = ativo;
        this.buttonSelectionEffect.interactable = ativo;
    }

    public void AtualizarInformacoes(SaveData save)
    {
        this.save = save;

        ResetarMiniaturas();

        botaoExcluirSave.gameObject.SetActive(true);
        botaoExportarSave.gameObject.SetActive(true);
        botaoImportarSave.gameObject.SetActive(false);

        DateTime data = BergamotaLibrary.SerializableDateTime.NewDateTime(save.playerSO.data);

        nome.text = save.playerSO.playerName;
        tempoDeJogo.text = TimeSpan.FromSeconds(save.playerSO.tempoDeJogo).ToString(@"hh\:mm");
        monstrosCapturados.text = save.playerSO.monsterBook.MonstrosCapturados().ToString();
        hora.text = data.ToString("HH:mm:ss");
        dia.text = data.ToString("dd/MM/yyyy");
        local.text = save.sceneInfo.nomeDoMapa;

        for(int i = 0; i < save.playerSO.monsterBag.Count; i++)
        {
            Image miniatura = Instantiate(miniaturaBase, miniaturasHolder).GetComponent<Image>();
            miniatura.gameObject.SetActive(true);

            miniatura.sprite = GlobalSettings.Instance.Listas.ListaDeMonsterData.GetData(save.playerSO.monsterBag[i].monsterDataID).Miniatura;

            miniaturas.Add(miniatura);
        }
    }

    public void ResetarInformacoes()
    {
        save = null;

        ResetarMiniaturas();

        botaoExcluirSave.gameObject.SetActive(false);
        botaoExportarSave.gameObject.SetActive(false);
        botaoImportarSave.gameObject.SetActive(true);

        nome.text = nomeSlotVazio;
        tempoDeJogo.text = TimeSpan.FromSeconds(0).ToString(@"hh\:mm");
        monstrosCapturados.text = 0.ToString();
        hora.text = string.Empty;
        dia.text = string.Empty;
        local.text = string.Empty;
    }

    public void SlotCorrompido()
    {
        ResetarInformacoes();

        nome.text = nomeSlotCorrompido;

        botaoExcluirSave.gameObject.SetActive(true);
        botaoExportarSave.gameObject.SetActive(false);
        botaoImportarSave.gameObject.SetActive(false);
    }

    private void ResetarMiniaturas()
    {
        foreach (Image miniatura in miniaturas)
        {
            Destroy(miniatura.gameObject);
        }

        miniaturas.Clear();
    }

    private void SlotSelecionado(PointerEventData eventData)
    {
        if(ativo == true)
        {
            eventoSlotSelecionado?.Invoke(numeroSlot, save);
        }
        else
        {
            Debug.Log("O slot " + numeroSlot + " nao esta ativo.");
        }
    }

    public void BotaoExcluirSelecionado()
    {
        eventoBotaoExcluirSelecionado?.Invoke(numeroSlot);
    }

    public void BotaoExportarSelecionado()
    {
        eventoBotaoExportarSelecionado?.Invoke(numeroSlot);
    }

    public void BotaoImportarSelecionado()
    {
        eventoBotaoImportarSelecionado?.Invoke(numeroSlot);
    }

    public void EsconderBotoesDeImportarExportar()
    {
        botaoExportarSave.gameObject.SetActive(false);
        botaoImportarSave.gameObject.SetActive(false);
    }
}
