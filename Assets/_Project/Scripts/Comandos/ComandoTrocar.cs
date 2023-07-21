using BergamotaDialogueSystem;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Battle/Comando de Correr")]

public class ComandoTrocar : Comando
{
    //Variaveis
    private MonsterInBattle monstroParaTrocar;
    private List<MonsterInBattle> monstros;
    private int origemIndice;
    private int trocaIndice;
    [Header("Dialogo")]
    [SerializeField] DialogueObject dialogoTrocaNaoFoiPossivel;

    //Getters

    public MonsterInBattle MonstroParaTrocar
    {
        get => monstroParaTrocar;
        set => monstroParaTrocar = value;
    }

    public List<MonsterInBattle> Monstros
    {
        get => monstros;
        set => monstros = value;
    }

    public int OrigemIndice
    {
        get => origemIndice;
        set => origemIndice = value;
    }

    public int TrocaIndice
    {
        get => trocaIndice;
        set => trocaIndice = value;
    }

    public DialogueObject Dialogue => dialogoTrocaNaoFoiPossivel;

    public void ReceberVariaveis(Integrante origem, int indiceTroca, int indiceMonstroAtual)
    {
        this.origem = origem;
        monstros = origem.ListaMonstros;
        origemIndice = indiceMonstroAtual;
        indiceMonstro = indiceMonstroAtual;
        trocaIndice = indiceTroca;
        monstroParaTrocar = monstros[trocaIndice];
    }
}
