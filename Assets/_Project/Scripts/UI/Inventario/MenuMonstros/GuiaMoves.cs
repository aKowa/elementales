using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GuiaMoves : MonstroInfo
{
    //Componentes
    [Header("Componentes")]
    [SerializeField] private GameObject ataqueSlotBase;
    [SerializeField] private Transform gridSlotsHolder;
    [SerializeField] private Transform ataqueSlotsHolder;
    [SerializeField] private AtaqueInfo ataqueInfo;
    [SerializeField] private RectTransform fundoBloqueadorDeAcoes;

    //Variaveis
    private Monster monstroAtual;

    private List<AtaqueGridSlot> gridSlots = new List<AtaqueGridSlot>();
    private List<AtaqueSlot> ataqueSlots = new List<AtaqueSlot>();

    private bool iniciado = false;

    private int objetosSeMovendo;
    private Coroutine corrotinaObjetoSeMovendo;

    //Getters
    public Monster MonstroAtual => monstroAtual;
    public List<AtaqueGridSlot> GridSlots => gridSlots;
    public List<AtaqueSlot> AtaqueSlots => ataqueSlots;

    private void Iniciar()
    {
        if(iniciado == true)
        {
            return;
        }

        ataqueSlotBase.SetActive(false);
        ataqueInfo.gameObject.SetActive(false);

        foreach (AtaqueGridSlot slot in gridSlotsHolder.GetComponentsInChildren<AtaqueGridSlot>(true))
        {
            gridSlots.Add(slot);
        }

        iniciado = true;
    }

    public override void AtualizarInformacoes(Monster monstro)
    {
        Iniciar();

        monstroAtual = monstro;

        for (int i = 0; i < gridSlots.Count; i++)
        {
            gridSlots[i].Iniciar(this, i);
        }

        ResetarAtaqueSlots();

        for (int i = 0; i < monstroAtual.Attacks.Count; i++)
        {
            AtaqueSlot ataqueSlot = Instantiate(ataqueSlotBase).GetComponent<AtaqueSlot>();
            ataqueSlot.GetComponent<RectTransform>().SetParent(ataqueSlotsHolder.transform, false);
            ataqueSlot.gameObject.SetActive(true);

            ataqueSlot.GuiaMoves = this;
            ataqueSlot.Indice = i;
            ataqueSlot.AttackHolder = monstroAtual.Attacks[i];

            ataqueSlot.transform.position = gridSlots[i].transform.position;

            ataqueSlot.AtualizarInformacoes();

            ataqueSlots.Add(ataqueSlot);
        }

        fundoBloqueadorDeAcoes.gameObject.SetActive(false);
    }

    public override void ResetarInformacoes()
    {
        ResetarAtaqueSlots();
        ataqueInfo.ResetarInformacoes();
    }

    private void ResetarAtaqueSlots()
    {
        foreach (AtaqueSlot slot in ataqueSlots)
        {
            Destroy(slot.gameObject);
        }

        ataqueSlots.Clear();
    }

    public void AbrirAtaqueInfo(ComandoDeAtaque ataque)
    {
        ataqueInfo.gameObject.SetActive(true);

        ataqueInfo.AtualizarInformacoes(ataque);
    }

    public void FecharAtaqueInfo()
    {
        ataqueInfo.gameObject.SetActive(false);
    }

    public void AtaqueSlotsRaycast(bool ativar)
    {
        for (int i = 0; i < ataqueSlots.Count; i++)
        {
            ataqueSlots[i].CanvasGroup.blocksRaycasts = ativar;
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
}
