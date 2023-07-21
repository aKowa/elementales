using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class AtaqueSlot : MonoBehaviour
{
    //Componentes
    [SerializeField] private TMP_Text nomeAtaque;
    [SerializeField] private TMP_Text textoPP;
    [SerializeField] private TMP_Text textoMaxPP;
    [SerializeField] private TMP_Text textoMana;
    [SerializeField] private TipoLogo tipoAtaque;

    private GuiaMoves guiaMoves;

    private CanvasGroup canvasGroup;

    //Variaveis
    private int indice;
    private AttackHolder attackHolder;

    private bool apertado;

    private readonly float tempoMovimento = 0.35f;

    //Getters
    public CanvasGroup CanvasGroup => canvasGroup;

    public GuiaMoves GuiaMoves
    {
        get => guiaMoves;
        set => guiaMoves = value;
    }

    public int Indice
    {
        get => indice;
        set => indice = value;
    }

    public AttackHolder AttackHolder
    {
        get => attackHolder;
        set => attackHolder = value;
    }

    private void Awake()
    {
        //Componentes
        ObjetoArrastavel objetoArrastavel = GetComponent<ObjetoArrastavel>();
        HoldButton holdButton = GetComponent<HoldButton>();

        canvasGroup = GetComponent<CanvasGroup>();

        //Variaveis
        apertado = false;

        //Eventos
        holdButton.OnPointerDownEvent.AddListener(OnPointerDown);
        holdButton.OnPointerUpEvent.AddListener(OnPointerUp);
        objetoArrastavel.OnBeginDragEvent.AddListener(OnBeginDrag);
        objetoArrastavel.OnEndDragEvent.AddListener(OnEndDrag);
    }

    public void AtualizarInformacoes()
    {
        nomeAtaque.text = attackHolder.Attack.Nome;
        textoPP.text = attackHolder.PP.ToString();
        textoMaxPP.text = attackHolder.Attack.MaxPP.ToString();

        textoMana.text = attackHolder.Attack.CustoMana.ToString();

        tipoAtaque.SetTipo(attackHolder.Attack.AttackData.TipoAtaque);
    }

    private void OnPointerDown(PointerEventData eventData)
    {
        apertado = true;
    }

    private void OnPointerUp(PointerEventData eventData)
    {
        if (apertado == true)
        {
            apertado = false;

            guiaMoves.AbrirAtaqueInfo(attackHolder.Attack);
        }
    }

    private void OnBeginDrag(PointerEventData eventData)
    {
        apertado = false;

        transform.SetAsLastSibling();

        guiaMoves.AtaqueSlotsRaycast(false);
    }

    private void OnEndDrag(PointerEventData eventData)
    {
        guiaMoves.AtaqueSlotsRaycast(true);
    }

    public void MoverAte(Vector3 novaPosicao)
    {
        guiaMoves.AdicionarObjetoSeMovendo();

        Sequence sequencia = DOTween.Sequence();
        sequencia.SetUpdate(true);

        sequencia.Append(transform.DOMove(novaPosicao, tempoMovimento));
        sequencia.AppendCallback(FinalizarMovimento);
    }

    private void FinalizarMovimento()
    {
        guiaMoves.SubtrairObjetoSeMovendo();
    }
}
