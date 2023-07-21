using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class JanelaEscolherMonstroGenerica : MonoBehaviour
{
    //Componentes
    [Header("Componentes")]
    [SerializeField] private GameObject monstroSlotBase;
    [SerializeField] private RectTransform menu;
    [SerializeField] private Transform gridSlotsHolder;
    [SerializeField] private Transform monstroSlotsHolder;

    //Variaveis
    private List<MonstroGridSlot> gridSlots = new List<MonstroGridSlot>();
    private List<MonstroSlotDeTroca> monstroSlots = new List<MonstroSlotDeTroca>();

    private UnityEvent<Monster> eventoMonstroEscolhido = new UnityEvent<Monster>();

    //Getters
    public UnityEvent<Monster> EventoMonstroEscolhido => eventoMonstroEscolhido;


    protected void Awake()
    {
        menu.gameObject.SetActive(false);

        monstroSlotBase.SetActive(false);
        menu.gameObject.SetActive(false);

        foreach (MonstroGridSlot slot in gridSlotsHolder.GetComponentsInChildren<MonstroGridSlot>(true))
        {
            gridSlots.Add(slot);
        }
    }

    public void IniciarMenu(Inventario inventario)
    {
        BergamotaLibrary.PauseManager.Pausar(true);

        menu.gameObject.SetActive(true);

        for (int i = 0; i < gridSlots.Count; i++)
        {
            if (i < inventario.MonsterBag.Count)
            {
                gridSlots[i].SlotAtivado(true);
            }
            else
            {
                gridSlots[i].SlotAtivado(false);
            }

            gridSlots[i].GetComponent<SlotDeObjeto>().Ativado = false;
        }

        ResetarMonstroSlots();

        for (int i = 0; i < inventario.MonsterBag.Count; i++)
        {
            MonstroSlotDeTroca monstroSlot = Instantiate(monstroSlotBase, monstroSlotsHolder).GetComponent<MonstroSlotDeTroca>();
            monstroSlot.gameObject.SetActive(true);

            monstroSlot.Iniciar(i, inventario.MonsterBag[i]);

            monstroSlot.Ativado(true);

            monstroSlot.transform.position = gridSlots[i].transform.position;

            monstroSlot.SlotSelecionado.AddListener(MonstroSelecionado);

            monstroSlots.Add(monstroSlot);
        }
    }

    public void FecharMenu()
    {
        BergamotaLibrary.PauseManager.Pausar(false);

        menu.gameObject.SetActive(false);
    }

    private void ResetarMonstroSlots()
    {
        foreach (MonstroSlotDeTroca slot in monstroSlots)
        {
            Destroy(slot.gameObject);
        }

        monstroSlots.Clear();
    }

    private void MonstroSelecionado(int indice)
    {
        eventoMonstroEscolhido.Invoke(monstroSlots[indice].Monstro);
    }
}
