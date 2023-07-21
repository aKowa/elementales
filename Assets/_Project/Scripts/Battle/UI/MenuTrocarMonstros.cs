using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuTrocarMonstros : MonoBehaviour
{
    //Componentes

    [Header("Componentes")]
    [SerializeField] private GameObject monstroSlotBase;
    [SerializeField] private RectTransform menu;
    [SerializeField] private Transform gridSlotsHolder;
    [SerializeField] private Transform monstroSlotsHolder;
    [SerializeField] private RectTransform botaoVoltar;

    private BattleUI battleUI;

    //Enums
    public enum ModoMenu { Normal, MonstroMorreu }

    //Variaveis
    private List<MonstroGridSlot> gridSlots = new List<MonstroGridSlot>();
    private List<MonstroSlotDeTroca> monstroSlots = new List<MonstroSlotDeTroca>();

    private ModoMenu modo;

    private void Awake()
    {
        battleUI = GetComponentInParent<BattleUI>();

        monstroSlotBase.SetActive(false);
        menu.gameObject.SetActive(false);

        modo = ModoMenu.Normal;

        foreach (MonstroGridSlot slot in gridSlotsHolder.GetComponentsInChildren<MonstroGridSlot>(true))
        {
            gridSlots.Add(slot);
        }
    }

    public void IniciarMenu(List<MonsterInBattle> monstros, List<Integrante.MonstroAtual> monstrosAtuais, List<MonsterInBattle> monstrosQueVaoSerTrocados)
    {
        IniciarMenu(monstros, monstrosAtuais, monstrosQueVaoSerTrocados, ModoMenu.Normal);
    }

    public void IniciarMenu(List<MonsterInBattle> monstros, List<Integrante.MonstroAtual> monstrosAtuais, List<MonsterInBattle> monstrosQueVaoSerTrocados, ModoMenu modo)
    {
        menu.gameObject.SetActive(true);

        this.modo = modo;

        switch(this.modo)
        {
            case ModoMenu.Normal:
                botaoVoltar.gameObject.SetActive(true);
                break;

            case ModoMenu.MonstroMorreu:
                botaoVoltar.gameObject.SetActive(false);
                break;
        }

        for (int i = 0; i < gridSlots.Count; i++)
        {
            if(i < monstros.Count)
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

        for (int i = 0; i < monstros.Count; i++)
        {
            MonstroSlotDeTroca monstroSlot = Instantiate(monstroSlotBase, monstroSlotsHolder).GetComponent<MonstroSlotDeTroca>();
            monstroSlot.gameObject.SetActive(true);

            monstroSlot.Iniciar(i, monstros[i].GetMonstro);

            monstroSlot.Ativado(true);

            for(int y = 0; y < monstrosAtuais.Count; y++)
            {
                if (monstros[i] == monstrosAtuais[y].Monstro)
                {
                    monstroSlot.SetCor(MonstroSlotDeTroca.CorSlot.MonstroNoTime);
                    monstroSlot.Ativado(false);
                    break;
                }
            }

            for (int y = 0; y < monstrosQueVaoSerTrocados.Count; y++)
            {
                if (monstros[i] == monstrosQueVaoSerTrocados[y])
                {
                    monstroSlot.Ativado(false);
                    break;
                }
            }

            if (monstros[i].GetMonstro.IsFainted == true)
            {
                monstroSlot.Ativado(false);
            }

            monstroSlot.transform.position = gridSlots[i].transform.position;

            monstroSlot.SlotSelecionado.AddListener(TrocarMonstro);

            monstroSlots.Add(monstroSlot);
        }
    }

    public void FecharMenu()
    {
        menu.gameObject.SetActive(false);
        ResetarMonstroSlots();

        battleUI.SetMenu(BattleUI.Menu.Escolha);
    }

    private void ResetarMonstroSlots()
    {
        foreach (MonstroSlotDeTroca slot in monstroSlots)
        {
            Destroy(slot.gameObject);
        }

        monstroSlots.Clear();
    }

    private void TrocarMonstro(int indice)
    {
        switch (modo)
        {
            case ModoMenu.Normal:
                battleUI.EscolheuMonstroParaTrocar(indice);
                break;

            case ModoMenu.MonstroMorreu:
                BattleManager.Instance.EscolheuMonstroMortoDoJogadorParaTrocar(indice);
                break;
        }
    }

    public int SlotsAtivos()
    {
        int slotsAtivos = 0;

        foreach(MonstroSlotDeTroca slot in monstroSlots)
        {
            if(slot.Ativo == true)
            {
                slotsAtivos++;
            }
        }

        return slotsAtivos;
    }
}
