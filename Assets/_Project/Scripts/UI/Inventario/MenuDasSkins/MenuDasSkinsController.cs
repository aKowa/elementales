using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MenuDasSkinsController : ViewController
{
    //Componentes
    [Header("Componentes")]
    [SerializeField] private GameObject diceRotationBase;
    [SerializeField] protected GameObject diceSkinSlotBase;
    [SerializeField] protected RectTransform diceSkinSlotsHolder;
    [SerializeField] private RectTransform fundoBloqueadorDeAcoesDoMenu;

    //Variaveis
    private UnityEvent<string> eventoSkinSelecionada = new UnityEvent<string>();

    protected List<DiceSkinSlot> skinsSlots = new List<DiceSkinSlot>();
    private List<DiceRotation> dices = new List<DiceRotation>();

    //Getters
    public UnityEvent<string> EventoSkinSelecionada => eventoSkinSelecionada;

    protected override void OnAwake()
    {
        fundoBloqueadorDeAcoesDoMenu.gameObject.SetActive(false);
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

    public void IniciarMenu(string skinAtual, Dictionary<string, bool> skins)
    {
        OpenView();

        AtualizarSlots(skinAtual, skins);
    }

    private void ResetarInformacoes()
    {
        ResetarSlots();
    }

    private void AtualizarSlots(string skinAtual, Dictionary<string, bool> skins)
    {
        float boxHeight = 0;
        float itemSlotHeight = diceSkinSlotBase.GetComponent<RectTransform>().sizeDelta.y;
        float spacing = diceSkinSlotsHolder.GetComponent<VerticalLayoutGroup>().spacing;

        ResetarSlots();

        int indiceDaLista = 0;
        foreach(string chaveDaSkin in skins.Keys)
        {
            if (skins[chaveDaSkin] == true)
            {
                //Dado
                DiceRotation dice = Instantiate(diceRotationBase).GetComponent<DiceRotation>();
                dice.gameObject.SetActive(true);

                dice.transform.position = new Vector3(indiceDaLista * 10, 10 + (indiceDaLista * 10), 0);

                dice.ChooseDiceToShow(DiceType.D6);
                dice.ChooseDiceTexture(dice.DiceDictionary[DiceType.D6].GetComponentInChildren<MeshRenderer>(), chaveDaSkin);

                dices.Add(dice);

                //Slot
                DiceSkinSlot diceSkinSlot = Instantiate(diceSkinSlotBase, diceSkinSlotsHolder).GetComponent<DiceSkinSlot>();
                diceSkinSlot.gameObject.SetActive(true);

                diceSkinSlot.Iniciar(chaveDaSkin, dice.RenderTexture);
                diceSkinSlot.EventoSkinSelecionada.AddListener(SkinSelecionada);

                skinsSlots.Add(diceSkinSlot);

                boxHeight += itemSlotHeight;

                indiceDaLista++;
            }
        }

        boxHeight += (spacing * (skinsSlots.Count - 1));

        diceSkinSlotsHolder.sizeDelta = new Vector2(diceSkinSlotsHolder.sizeDelta.x, boxHeight);

        AtualizarSelecaoDosDados(skinAtual);
    }

    public void ResetarSlots()
    {
        foreach (DiceSkinSlot skinSlot in skinsSlots)
        {
            Destroy(skinSlot.gameObject);
        }

        foreach (DiceRotation dice in dices)
        {
            Destroy(dice.gameObject);
        }

        skinsSlots.Clear();
        dices.Clear();

        diceSkinSlotsHolder.anchoredPosition = Vector2.zero;
    }

    public void AtualizarSelecaoDosDados(string skinAtual)
    {
        for(int i = 0; i < skinsSlots.Count; i++)
        {
            skinsSlots[i].Selecionado(skinsSlots[i].ChaveDaSkin == skinAtual);
        }
    }

    private void SkinSelecionada(string chaveDaSkin)
    {
        eventoSkinSelecionada?.Invoke(chaveDaSkin);
    }
}
