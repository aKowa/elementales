using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MonstroSlotBattle : MonoBehaviour
{
    //Componentes
    [Header("Componentes")]
    [SerializeField] private Image imagem;
    [SerializeField] private MonstroSlotInfo monstroSlotInfo;
    [SerializeField] private BarraExp barraExp;
    [SerializeField] private RectTransform monstersInBagHolder;
    [SerializeField] private RectTransform iconeMonstroCapturado;
    [SerializeField] private RectTransform monsterDicesHolder;

    private HoldButton holdButton;
    private ButtonSelectionEffect buttonSelectionEffect;

    private BergamotaLibrary.Animacao animacao;

    [Header("Variaveis Padroes")]
    [SerializeField] private Color corTurnoDoMonstro;
    [SerializeField] private Sprite iconeMonstroVivo;
    [SerializeField] private Sprite iconeMonstroMorto;
    [SerializeField] private Sprite iconeSemMonstro;

    //Enums
    public enum PosicaoMonsterInBag { Esquerda, Direita }
    public enum AnimacaoSlot { Nenhum, AtaqueEfetivo }

    //Variaveis
    private UnityEvent<int, int> slotSelecionado = new UnityEvent<int, int>();

    private bool ativado;
    private List<MonsterInBattle> listaDeMonstros = new List<MonsterInBattle>();

    private MonsterInBattle monstro;

    private int indiceIntegrante;
    private int indiceMonstroAtual;
    private int hpAtualDoSlot;
    private int manaAtualDoSlot;
    private List<Image> monsterInBagIcons = new List<Image>();

    //Getters
    public BarraExp BarraExp => barraExp;
    public UnityEvent<int, int> SlotSelecionado => slotSelecionado;

    //Dice Icons
    public RectTransform MonsterDicesHolder => monsterDicesHolder;
    [SerializeField] private List<DiceIcon> diceIcons = new List<DiceIcon>();

    private void Awake()
    {
        //Componentes
        holdButton = GetComponent<HoldButton>();
        buttonSelectionEffect = GetComponent<ButtonSelectionEffect>();
        animacao = GetComponent<BergamotaLibrary.Animacao>();

        //Variaveis
        ativado = false;

        hpAtualDoSlot = 0;
        manaAtualDoSlot = 0;

        //Eventos
        holdButton.OnPointerUpEvent.AddListener(OnPointerUp);

        foreach(Image icone in monstersInBagHolder.GetComponentsInChildren<Image>(true))
        {
            monsterInBagIcons.Add(icone);
        }

        DesativarMonsterInBag();
        Desativar();
        TurnoDoMonstro(false);
    }

    public void Iniciar(int indiceIntegrante, int indiceMonstroAtual)
    {
        this.indiceIntegrante = indiceIntegrante;
        this.indiceMonstroAtual = indiceMonstroAtual;
    }

    public void Ativar()
    {
        ativado = true;
        buttonSelectionEffect.interactable = true;

        if(monstro.GetMonstro.IsFainted == true)
        {
            Desativar();
        }
    }

    public void Desativar()
    {
        ativado = false;
        buttonSelectionEffect.interactable = false;
    }

    public void AtualizarMonstro(MonsterInBattle monstro)
    {
        this.monstro = monstro;
        monstroSlotInfo.Monstro = this.monstro.GetMonstro;
        RemoveDiceIcons();
        barraExp.AtualizarBarra(monstro.GetMonstro);

        AtualizarInformacoes();
    }

    public void AtualizarInformacoes()
    {
        monstroSlotInfo.AtualizarInformacoes();

        hpAtualDoSlot = monstro.GetMonstro.AtributosAtuais.Vida;
        manaAtualDoSlot = monstro.GetMonstro.AtributosAtuais.Mana;

        AtualizarMonstersInBag();
    }

    public IEnumerator AtualizarBarras()
    {
        if(hpAtualDoSlot < monstro.GetMonstro.AtributosAtuais.Vida)
        {
            yield return monstroSlotInfo.BarraHP.AumentarHP(monstro.GetMonstro);
        }
        else if(hpAtualDoSlot > monstro.GetMonstro.AtributosAtuais.Vida)
        {
            yield return monstroSlotInfo.BarraHP.DiminuirHP(monstro.GetMonstro);
        }

        if (manaAtualDoSlot < monstro.GetMonstro.AtributosAtuais.Mana)
        {
            yield return monstroSlotInfo.BarraMana.AumentarMana(monstro.GetMonstro);
        }
        else if (manaAtualDoSlot > monstro.GetMonstro.AtributosAtuais.Mana)
        {
            yield return monstroSlotInfo.BarraMana.DiminuirMana(monstro.GetMonstro);
        }

        AtualizarInformacoes();
    }

    public void AtualizarPosicaoMonstersInBag(PosicaoMonsterInBag posicao)
    {
        switch(posicao)
        {
            case PosicaoMonsterInBag.Esquerda:
                monstersInBagHolder.anchorMin = new Vector2(0, 1);
                monstersInBagHolder.anchorMax = new Vector2(0, 1);
                monstersInBagHolder.pivot = new Vector2(0, 1);

                iconeMonstroCapturado.anchorMin = new Vector2(1, 1);
                iconeMonstroCapturado.anchorMax = new Vector2(1, 1);
                iconeMonstroCapturado.pivot = new Vector2(1, 1);

                monsterDicesHolder.anchorMin = new Vector2(1, 1);
                monsterDicesHolder.anchorMax = new Vector2(1, 1);
                monsterDicesHolder.pivot = new Vector2(1, 1);
                monsterDicesHolder.anchoredPosition = new Vector2(monsterDicesHolder.sizeDelta.x, 0);
                monsterDicesHolder.GetComponent<GridLayoutGroup>().startCorner = GridLayoutGroup.Corner.UpperLeft;
                monsterDicesHolder.GetComponent<GridLayoutGroup>().childAlignment = TextAnchor.UpperLeft;
                break;

            case PosicaoMonsterInBag.Direita:
                monstersInBagHolder.anchorMin = new Vector2(1, 1);
                monstersInBagHolder.anchorMax = new Vector2(1, 1);
                monstersInBagHolder.pivot = new Vector2(1, 1);

                iconeMonstroCapturado.anchorMin = new Vector2(0, 1);
                iconeMonstroCapturado.anchorMax = new Vector2(0, 1);
                iconeMonstroCapturado.pivot = new Vector2(0, 1);

                monsterDicesHolder.anchorMin = new Vector2(0, 1);
                monsterDicesHolder.anchorMax = new Vector2(0, 1);
                monsterDicesHolder.pivot = new Vector2(0, 1);
                monsterDicesHolder.anchoredPosition = new Vector2(monsterDicesHolder.sizeDelta.x * -1, 0);
                monsterDicesHolder.GetComponent<GridLayoutGroup>().startCorner = GridLayoutGroup.Corner.UpperRight;
                monsterDicesHolder.GetComponent<GridLayoutGroup>().childAlignment = TextAnchor.UpperRight;
                break;
        }
    }

    public void AtivarMonstersInBag(Integrante integrante)
    {
        listaDeMonstros.Clear();

        foreach(MonsterInBattle monstro in integrante.ListaMonstros)
        {
            listaDeMonstros.Add(monstro);
        }

        for (int i = 0; i < monsterInBagIcons.Count; i++)
        {
            monsterInBagIcons[i].gameObject.SetActive(true);
        }

        AtualizarMonstersInBag();
    }

    public void IconeMonstroCapturadoAtivado(bool ativado, MonsterData monstro)
    {
        iconeMonstroCapturado.gameObject.SetActive(ativado);

        if(ativado == false)
        {
            return;
        }

        if (PlayerData.MonsterBook.MonsterEntries[monstro.ID].WasCaptured == false)
        {
            iconeMonstroCapturado.gameObject.SetActive(false);
        }
    }

    public void DesativarMonsterInBag()
    {
        for(int i = 0; i < monsterInBagIcons.Count; i++)
        {
            monsterInBagIcons[i].gameObject.SetActive(false);
        }
    }

    public void AtualizarMonstersInBag()
    {
        for (int i = 0; i < monsterInBagIcons.Count; i++)
        {
            if(i < listaDeMonstros.Count)
            {
                if (listaDeMonstros[i].GetMonstro.IsFainted == false)
                {
                    monsterInBagIcons[i].sprite = iconeMonstroVivo;
                }
                else
                {
                    monsterInBagIcons[i].sprite = iconeMonstroMorto;
                }
            }
            else
            {
                monsterInBagIcons[i].sprite = iconeSemMonstro;
            }
        }
    }

    public void TurnoDoMonstro(bool turnoDoMonstro)
    {
        if(turnoDoMonstro == true)
        {
            imagem.color = corTurnoDoMonstro;
        }
        else
        {
            imagem.color = Color.white;
        }
    }

    private void OnPointerUp(PointerEventData eventData)
    {
        if (ativado == false)
        {
            return;
        }

        slotSelecionado?.Invoke(indiceIntegrante, indiceMonstroAtual);
    }

    public void TrocarAnimacao(AnimacaoSlot animacao)
    {
        this.animacao.TrocarAnimacao(animacao.ToString());
    }

    public void ExecutarUmMetodoAposOFimDaAnimacao(System.Action metodo)
    {
        animacao.ExecutarUmMetodoAposOFimDaAnimacao(metodo);
    }

    public void RemoveDiceIcons()
    {
        foreach (Transform child in MonsterDicesHolder)
        {
            Destroy(child.gameObject);
        }
        diceIcons.Clear();
    }

    public void FindDiceIcons()
    {
        foreach (Transform child in MonsterDicesHolder)
        {
            Image image = child.GetComponent<Image>();
            TextMeshProUGUI text = image.GetComponentInChildren<TextMeshProUGUI>();
            diceIcons.Add(new DiceIcon(){image = image, tmPro = text});
        }
    }

    public void UpdateDiceIcons(List<int> diceResults)
    {
        for (var i = 0; i < diceIcons.Count; i++)
        {
            DiceIcon dI = diceIcons[i];
            dI.tmPro.text = diceResults[i].ToString();
        }
    }

    [System.Serializable]
    private struct DiceIcon
    {
        public Image image;
        public TextMeshProUGUI tmPro;
    }
}
