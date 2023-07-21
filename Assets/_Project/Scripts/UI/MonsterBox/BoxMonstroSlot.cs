using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BoxMonstroSlot : MonoBehaviour
{
    //Componentes
    [SerializeField] private Image miniatura;

    private MonsterBoxController monsterBoxController;
    private CanvasGroup canvasGroup;

    //Variaveis
    private int indice;
    private MonsterBoxController.TipoSlot tipo;

    private Monster monstro;

    private bool apertado;

    private readonly float tempoMovimento = 0.35f;

    //Getters
    public CanvasGroup CanvasGroup => canvasGroup;
    public Monster Monstro => monstro;

    public int Indice
    {
        get => indice;
        set => indice = value;
    }

    public MonsterBoxController.TipoSlot Tipo
    {
        get => tipo;
        set => tipo = value;
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

    public void Iniciar(MonsterBoxController monsterBoxController, int indice, MonsterBoxController.TipoSlot tipo, Monster monstro)
    {
        this.monsterBoxController = monsterBoxController;
        this.indice = indice;
        this.tipo = tipo;
        this.monstro = monstro;

        AtualizarInformacoes();
    }

    private void AtualizarInformacoes()
    {
        miniatura.sprite = monstro.MonsterData.Miniatura;
    }

    public void MoverMonstroParaLista(int indice, MonsterBoxController.TipoSlot tipoSlot)
    {
        switch (this.tipo)
        {
            case MonsterBoxController.TipoSlot.Box:
                monsterBoxController.BoxMonstroSlots.Remove(this);
                break;

            case MonsterBoxController.TipoSlot.Party:
                monsterBoxController.PartyMonstroSlots.Remove(this);
                break;
        }

        switch (tipoSlot)
        {
            case MonsterBoxController.TipoSlot.Box:
                monsterBoxController.Inventario.MonsterBank[monsterBoxController.IndiceBox].Monsters[indice] = monstro;

                monsterBoxController.BoxMonstroSlots.Add(this);
                break;

            case MonsterBoxController.TipoSlot.Party:
                if (indice >= monsterBoxController.Inventario.MonsterBag.Count)
                {
                    monsterBoxController.Inventario.AddMonsterToBag(monstro);
                    indice = monsterBoxController.Inventario.MonsterBag.Count - 1;
                }
                else
                {
                    monsterBoxController.Inventario.MonsterBag[indice] = monstro;
                }

                monsterBoxController.PartyMonstroSlots.Add(this);
                break;
        }

        this.indice = indice;
        this.tipo = tipoSlot;

        MoverAte(monsterBoxController.GetBoxMonstroGridSlot(this.indice, this.tipo).transform.position);
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

            monsterBoxController.AbrirMenuOpcoes(tipo, indice);
        }
    }

    private void OnBeginDrag(PointerEventData eventData)
    {
        apertado = false;

        transform.SetAsLastSibling();

        monsterBoxController.MonstroSlotsRaycast(false);
    }

    private void OnEndDrag(PointerEventData eventData)
    {
        monsterBoxController.MonstroSlotsRaycast(true);
    }

    public void MoverAte(Vector3 novaPosicao)
    {
        monsterBoxController.AdicionarObjetoSeMovendo();

        Sequence sequencia = DOTween.Sequence();
        sequencia.SetUpdate(true);

        sequencia.Append(transform.DOMove(novaPosicao, tempoMovimento));
        sequencia.AppendCallback(FinalizarMovimento);
    }

    public void AnimacaoDestruicao()
    {
        monsterBoxController.AdicionarObjetoSeMovendo();

        Sequence sequencia = DOTween.Sequence();
        sequencia.SetUpdate(true);

        sequencia.Append(transform.DOScale(Vector3.zero, tempoMovimento));
        sequencia.AppendCallback(SeDestruir);
    }

    private void FinalizarMovimento()
    {
        monsterBoxController.SubtrairObjetoSeMovendo();
    }

    public void SeDestruir()
    {
        monsterBoxController.SubtrairObjetoSeMovendo();
        Destroy(gameObject);
    }
}
