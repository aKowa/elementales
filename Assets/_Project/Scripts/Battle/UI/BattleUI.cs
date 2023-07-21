using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleUI : MonoBehaviour
{
    //Componentes
    [Header("Menus")]
    [SerializeField] private RectTransform menuEscolhas;
    [SerializeField] private EscolhaDeAtaques escolhaDeAtaques;
    [SerializeField] private MenuTrocarMonstros menuTrocarMonstros;
    [SerializeField] private MenuBagBatalhaController menuBagBatalhaController;
    [SerializeField] private JanelaDeAtributosDoLevelUp janelaDeAtributosDoLevelUp;
    [SerializeField] private MenuMonsterEntryController menuMonsterEntryController;
    [SerializeField] private MonsterLearnAttack telaAprenderAtaque;
    [SerializeField] private JanelaTrocarNickname janelaTrocarNickname;
    [SerializeField] private MenuTutorialDeBatalhaController menuTutorialDeBatalha;
    [SerializeField] private JanelaCombatLesson janelaCombatLesson;

    [Header("Componentes")]
    [SerializeField] private ButtonSelectionEffect botaoCorrer;

    //Enums
    public enum Menu { Nenhum, Escolha, Ataque, Monstros, Bag }

    //Variaveis
    private Integrante integranteAtual;
    private int indiceMonstroAtual;
    private MonsterInBattle monstroAtual;

    private int numeroComando;

    private List<MonsterInBattle> monstrosQueVaoSerTrocados = new List<MonsterInBattle>();

    //Getters
    public Integrante IntegranteAtual => integranteAtual;
    public MenuTrocarMonstros MenuTrocarMonstros => menuTrocarMonstros;
    public JanelaDeAtributosDoLevelUp JanelaDeAtributosDoLevelUp => janelaDeAtributosDoLevelUp;
    public MenuMonsterEntryController MenuMonsterEntryController => menuMonsterEntryController;
    public MonsterLearnAttack TelaAprenderAtaque => telaAprenderAtaque;
    public JanelaTrocarNickname JanelaTrocarNickname => janelaTrocarNickname;
    public MenuTutorialDeBatalhaController MenuTutorialDeBatalha => menuTutorialDeBatalha;
    public JanelaCombatLesson JanelaCombatLesson => janelaCombatLesson;
    public int IndiceMonstroAtual => indiceMonstroAtual;

    //Setters
    public void SetMenu(Menu menu)
    {
        switch(menu)
        {
            case Menu.Nenhum:
                menuEscolhas.gameObject.SetActive(false);
                escolhaDeAtaques.SetMenu(EscolhaDeAtaques.Menu.Nenhum);
                break;

            case Menu.Escolha:
                menuEscolhas.gameObject.SetActive(true);
                escolhaDeAtaques.SetMenu(EscolhaDeAtaques.Menu.Nenhum);
                break;

            case Menu.Ataque:
                menuEscolhas.gameObject.SetActive(false);
                escolhaDeAtaques.SetMenu(EscolhaDeAtaques.Menu.Escolha);

                escolhaDeAtaques.IniciarMenu(monstroAtual.GetMonstro);
                break;

            case Menu.Monstros:
                menuEscolhas.gameObject.SetActive(false);
                escolhaDeAtaques.SetMenu(EscolhaDeAtaques.Menu.Nenhum);

                menuTrocarMonstros.IniciarMenu(integranteAtual.ListaMonstros, integranteAtual.MonstrosAtuais, monstrosQueVaoSerTrocados);
                break;

            case Menu.Bag:
                menuEscolhas.gameObject.SetActive(true);
                escolhaDeAtaques.SetMenu(EscolhaDeAtaques.Menu.Nenhum);
                menuBagBatalhaController.OpenView();
                break;
        }
    }

    private void Awake()
    {
        numeroComando = 0;

        SetMenu(Menu.Nenhum);

        janelaDeAtributosDoLevelUp.FecharJanela();
    }

    public void IniciarMonstroSlots()
    {
        foreach(Integrante integrante in BattleManager.Instance.Integrantes)
        {
            foreach(Integrante.MonstroAtual monstroAtual in integrante.MonstrosAtuais)
            {
                monstroAtual.MonstroSlotBattle.SlotSelecionado.AddListener(AlvoSelecionado);
            }
        }
    }

    public void IniciarMenu(Integrante integrante)
    {
        integranteAtual = integrante;
        numeroComando = 0;
        monstrosQueVaoSerTrocados.Clear();

        IniciarMenuEscolha(0);
    }

    private void IniciarMenuEscolha(int indiceMonstro)
    {
        indiceMonstroAtual = indiceMonstro;
        monstroAtual = integranteAtual.MonstrosAtuais[indiceMonstroAtual].Monstro;

        foreach (var item in monstroAtual.GetMonstro.StatusSecundario)
        {
            if(item.ForaDeCombate() == true)
            {
                Debug.Log("O monstro esta fora do cmobate por acaop estranha e talz");

                numeroComando--;
                PassarComando(null);
                return;
            }
        }

        if (monstroAtual.GetMonstro.IsFainted == true)
        {
            numeroComando--;
            PassarComando(null);
            return;
        }

        if (BattleManager.Instance.TemComandoQueBloqueiaAcao(monstroAtual) == true)
        {
            numeroComando--;
            PassarComando(null);
            return;
        }

        botaoCorrer.interactable = (numeroComando == 0);

        integranteAtual.MonstrosAtuais[indiceMonstroAtual].MonstroSlotBattle.TurnoDoMonstro(true);

        SetMenu(Menu.Escolha);
    }

    public void AbrirMenuDeAtaque()
    {
        SetMenu(Menu.Ataque);
    }

    public void AbrirMenuBag()
    {
        SetMenu(Menu.Bag);
    }

    public void AbrirMenuTrocarMonstros()
    {
        SetMenu(Menu.Monstros);
    }

    public void AbrirMenuMonsterEntry(MonsterData monsterData)
    {
        menuMonsterEntryController.IniciarMenu(monsterData, false);
    }

    public void Correr()
    {
        if(numeroComando == 0)
        {
            integranteAtual.MonstrosAtuais[indiceMonstroAtual].MonstroSlotBattle.TurnoDoMonstro(false);

            BattleManager.Instance.ReceberComandoDoMenu(BattleManager.Instance.PlayerEscolheuCorrer(integranteAtual), true);
            SetMenu(Menu.Nenhum);

            integranteAtual.JaEscolheu = true;
        }
    }

    public void PassarComando(Comando comando)
    {
        integranteAtual.MonstrosAtuais[indiceMonstroAtual].MonstroSlotBattle.TurnoDoMonstro(false);

        if (indiceMonstroAtual < integranteAtual.MonstrosAtuais.Count - 1)
        {
            BattleManager.Instance.ReceberComandoDoMenu(comando, false);

            numeroComando++;
            IniciarMenuEscolha(indiceMonstroAtual + 1);
        }
        else
        {
            BattleManager.Instance.ReceberComandoDoMenu(comando, true);
            SetMenu(Menu.Nenhum);

            integranteAtual.JaEscolheu = true;
        }
    }

    private void AlvoSelecionado(int indiceIntegrante, int indiceMonstro)
    {
        DesativarMonstroSlots();
        escolhaDeAtaques.FecharMenu();

        PassarComando(BattleManager.Instance.PlayerEscolheuAtaque(escolhaDeAtaques.AttackHolderAtual, indiceMonstroAtual, integranteAtual, indiceIntegrante, indiceMonstro));
    }

    public void PassarComandoAtaque(Comando comando)
    {
        DesativarMonstroSlots();
        escolhaDeAtaques.FecharMenu();

        PassarComando(comando);
    }

    public void EscolheuMonstroParaTrocar(int indice)
    {
        menuTrocarMonstros.FecharMenu();

        monstrosQueVaoSerTrocados.Add(integranteAtual.ListaMonstros[indice]);

        PassarComando(BattleManager.Instance.PlayerEscolheuTroca(integranteAtual, indice, indiceMonstroAtual));
    }

    public void EscolheuItemParaUsar(ItemHolder item, int indiceMonstroAtual)
    {
        if(item != null)
        {
            if(item.Item.Tipo == Item.TipoItem.MonsterBall)
            {
                PassarComando(BattleManager.Instance.PlayerEscolheuMonsterBall(integranteAtual, item));
                Debug.Log("Escolheu MonsterBall");
            }
            else
            {
                PassarComando(BattleManager.Instance.PlayerEscolheuItem(integranteAtual, indiceMonstroAtual, item));
            }
        }
        else
        {
            PassarComando(null);
        }
    }

    public void AtualizarInformacoesDosSlots()
    {
        foreach (Integrante integrante in BattleManager.Instance.Integrantes)
        {
            foreach (Integrante.MonstroAtual monstroAtual in integrante.MonstrosAtuais)
            {
                monstroAtual.MonstroSlotBattle.AtualizarInformacoes();
            }
        }
    }

    public void DesativarMonstroSlots()
    {
        foreach (Integrante integrante in BattleManager.Instance.Integrantes)
        {
            foreach (Integrante.MonstroAtual monstroAtual in integrante.MonstrosAtuais)
            {
                monstroAtual.MonstroSlotBattle.Desativar();
            }
        }
    }
}
