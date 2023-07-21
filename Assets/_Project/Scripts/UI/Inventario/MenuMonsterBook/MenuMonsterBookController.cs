using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuMonsterBookController : ViewController
{
    //Componentes
    [Header("Componentes")]
    [SerializeField] protected GameObject monsterEntrySlotBase;
    [SerializeField] protected RectTransform monsterEntrySlotsHolder;
    [SerializeField] private RectTransform fundoBloqueadorDeAcoesDoMenu;

    [Header("Menus")]
    [SerializeField] private MenuMonsterEntryController menuMonsterEntryController;

    //Variaveis
    private List<MonsterEntrySlot> monsterEntrySlots = new List<MonsterEntrySlot>();

    protected override void OnAwake()
    {
        monsterEntrySlotBase.gameObject.SetActive(false);
        fundoBloqueadorDeAcoesDoMenu.gameObject.SetActive(false);

        onOpen += IniciarMenu;
    }

    public override void OnOpen()
    {
        fundoBloqueadorDeAcoesDoMenu.gameObject.SetActive(true);
    }

    protected override void OnClose()
    {
        fundoBloqueadorDeAcoesDoMenu.gameObject.SetActive(false);

        ResetarMonsterEntrySlots();
    }

    private void IniciarMenu()
    {
        float boxHeight = 0;
        float itemSlotHeight = monsterEntrySlotBase.GetComponent<RectTransform>().sizeDelta.y;
        float spacing = monsterEntrySlotsHolder.GetComponent<VerticalLayoutGroup>().spacing;

        ResetarMonsterEntrySlots();

        for (int i = 0; i < PlayerData.MonsterBook.MonsterEntries.Count; i++)
        {
            MonsterEntrySlot monsterEntrySlot = Instantiate(monsterEntrySlotBase, monsterEntrySlotsHolder).GetComponent<MonsterEntrySlot>();
            monsterEntrySlot.gameObject.SetActive(true);

            monsterEntrySlot.AtualizarInformacoes(GlobalSettings.Instance.Listas.ListaDeMonsterData.GetData(i));
            monsterEntrySlot.EventoSlotSelecionado.AddListener(AbrirMenuMonsterEntry);

            monsterEntrySlots.Add(monsterEntrySlot);

            boxHeight += itemSlotHeight;
        }

        boxHeight += (spacing * (monsterEntrySlots.Count - 1));

        monsterEntrySlotsHolder.sizeDelta = new Vector2(monsterEntrySlotsHolder.sizeDelta.x, boxHeight);
    }

    private void ResetarMonsterEntrySlots()
    {
        foreach (MonsterEntrySlot monsterEntrySlot in monsterEntrySlots)
        {
            Destroy(monsterEntrySlot.gameObject);
        }

        monsterEntrySlots.Clear();

        monsterEntrySlotsHolder.anchoredPosition = Vector2.zero;
    }

    private void AbrirMenuMonsterEntry(MonsterEntrySlot monsterEntrySlot)
    {
        if(monsterEntrySlot.Ativo == true)
        {
            menuMonsterEntryController.IniciarMenu(GlobalSettings.Instance.Listas.ListaDeMonsterData.GetData(monsterEntrySlot.MonstroID));
        }
        else
        {
            Debug.Log("O Monstro Ainda Nao Foi Encontrado!");
        }
    }
}
