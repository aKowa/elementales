using BergamotaDialogueSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MenuDosCombatLessonsController : ViewController
{
    //Componentes
    [Header("Componentes")]
    [SerializeField] protected GameObject combatLessonSlotBase;
    [SerializeField] protected RectTransform combatLessonSlotsHolder;
    [SerializeField] private AtaqueInfo combatLessonInfo;
    [SerializeField] private RectTransform fundoBloqueadorDeAcoesDoMenu;
    [SerializeField] private RectTransform fundoDialogo;

    private DialogueActivator dialogueActivator;

    [Header("Variaveis Padroes")]
    [SerializeField] private DialogueObject dialogoMaximoDeCombatLessons;

    //Variaveis
    private UnityEvent eventoCombatLessonsAtualizados = new UnityEvent();

    private List<CombatLessonSlot> combatLessonSlots = new List<CombatLessonSlot>();

    private Monster monstroAtual;

    //Getters
    public UnityEvent EventoCombatLessonsAtualizados => eventoCombatLessonsAtualizados;

    protected override void OnAwake()
    {
        dialogueActivator = GetComponent<DialogueActivator>();

        combatLessonInfo.gameObject.SetActive(false);
        fundoBloqueadorDeAcoesDoMenu.gameObject.SetActive(false);
        fundoDialogo.gameObject.SetActive(false);
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

    public void IniciarMenu(Monster monstro)
    {
        monstroAtual = monstro;

        OpenView();

        AtualizarSlots(monstroAtual.CombatLessons);
    }

    private void ResetarInformacoes()
    {
        ResetarSlots();
    }

    protected void AtualizarSlots(List<CombatLesson> combatLessons)
    {
        float boxHeight = 0;
        float itemSlotHeight = combatLessonSlotBase.GetComponent<RectTransform>().sizeDelta.y;
        float spacing = combatLessonSlotsHolder.GetComponent<VerticalLayoutGroup>().spacing;

        ResetarSlots();

        for (int i = 0; i < combatLessons.Count; i++)
        {
            CombatLessonSlot combatLessonSlot = Instantiate(combatLessonSlotBase, combatLessonSlotsHolder).GetComponent<CombatLessonSlot>();
            combatLessonSlot.gameObject.SetActive(true);

            combatLessonSlot.Iniciar(combatLessons[i]);
            combatLessonSlot.EventoSlotSelecionado.AddListener(CombatLessonSelecionado);
            combatLessonSlot.BotaoInfoSelecionado.AddListener(AbrirCombatLessonInfo);

            combatLessonSlots.Add(combatLessonSlot);

            boxHeight += itemSlotHeight;
        }

        boxHeight += (spacing * (combatLessons.Count - 1));

        combatLessonSlotsHolder.sizeDelta = new Vector2(combatLessonSlotsHolder.sizeDelta.x, boxHeight);

        AtualizarSelecaoDosSlots();
    }

    private void ResetarSlots()
    {
        foreach (CombatLessonSlot slot in combatLessonSlots)
        {
            Destroy(slot.gameObject);
        }

        combatLessonSlots.Clear();

        combatLessonSlotsHolder.anchoredPosition = Vector2.zero;
    }

    private void AtualizarSelecaoDosSlots()
    {
        for(int i = 0; i < combatLessonSlots.Count; i++)
        {
            combatLessonSlots[i].Selecionado(monstroAtual.CombatLessonsAtivos.Contains(combatLessonSlots[i].CombatLesson));
        }
    }

    private void CombatLessonSelecionado(CombatLesson combatLesson)
    {
        if(monstroAtual.CombatLessonsAtivos.Contains(combatLesson) == true)
        {
            monstroAtual.CombatLessonsAtivos.Remove(combatLesson);

            eventoCombatLessonsAtualizados?.Invoke();
            AtualizarSelecaoDosSlots();
        }
        else if(monstroAtual.CombatLessonsAtivos.Count < Monster.maxCombatLessonsAtivas)
        {
            monstroAtual.CombatLessonsAtivos.Add(combatLesson);

            eventoCombatLessonsAtualizados?.Invoke();
            AtualizarSelecaoDosSlots();
        }
        else
        {
            DialogueUI.Instance.SetPlaceholderDeTexto("%monstro", monstroAtual.NickName);
            AbrirDialogo(dialogoMaximoDeCombatLessons);
        }
    }

    private void AbrirCombatLessonInfo(CombatLesson combatLesson)
    {
        combatLessonInfo.gameObject.SetActive(true);
        combatLessonInfo.AtualizarInformacoes(combatLesson);
    }

    public void FecharCombatLessonInfo()
    {
        combatLessonInfo.gameObject.SetActive(false);
    }

    private void AbrirDialogo(DialogueObject dialogo)
    {
        dialogueActivator.ShowDialogue(dialogo, DialogueUI.Instance);

        fundoDialogo.gameObject.SetActive(true);

        StartCoroutine(DialogoAberto(DialogueUI.Instance));
    }

    private void FecharDialogo()
    {
        fundoDialogo.gameObject.SetActive(false);
    }

    private IEnumerator DialogoAberto(DialogueUI dialogueUI)
    {
        yield return new WaitUntil(() => dialogueUI.IsOpen == false);

        FecharDialogo();
    }
}
