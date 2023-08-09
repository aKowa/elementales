using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data Base/Player Data")]
public class PlayerSO : SerializedScriptableObject
{
    //Enums
    public enum Sexo { Masculino, Feminino }

    //Variaveis
    [Header("Variaveis Padroes")]
    [SerializeField] private Texture2D spriteSexoMasculino;
    [SerializeField] private Texture2D spriteSexoFeminino;

    [Header("Geral")]

    [SerializeField] private string playerName;
    [SerializeField] private Sexo sexoDoPlayer;
    [SerializeField] private int dinheiro;
    [SerializeField] private float tempoDeJogo;

    [Header("Efeitos no Jogo")]
    [SerializeField] private int repelente;
    [SerializeField] private bool hasMagnet;

    //Inventario
    [Header("Monstros")]

    [SerializeField] private List<MonsterBox> monsterBank = new List<MonsterBox>();
    [SerializeField] private List<Monster> monsterBag = new List<Monster>();

    [Header("Bag")]

    [SerializeField] private List<ItemHolder> itens = new List<ItemHolder>();
    [SerializeField] private List<ItemHolder> monsterBalls = new List<ItemHolder>();
    [SerializeField] private List<ItemHolder> habilidades = new List<ItemHolder>();
    [SerializeField] private List<ItemHolder> itensChave = new List<ItemHolder>();

    //Monstros Encontrados
    [Header("Monstros Encontrados"), InlineButton("FindAll")]
    [SerializeField] private MonsterBook monsterBook;

    //Skins de Dados
    [Header("Skins de Dados"), InlineButton("LiberarTodas")]
    [OdinSerialize] private Dictionary<string, bool> skinsDeDados = new Dictionary<string, bool>();

    //Vasos de Planta
    [Header("Vasos de Planta")]
    [OdinSerialize] private Dictionary<string, VasoPlantaSave> vasosDePlanta = new Dictionary<string, VasoPlantaSave>();

    //Getters
    public string PlayerName => playerName;
    public Sexo SexoDoPlayer => sexoDoPlayer;
    public MonsterBook MonsterBook => monsterBook;
    public Dictionary<string, bool> SkinsDeDados => skinsDeDados;
    public Dictionary<string, VasoPlantaSave> VasosDePlanta => vasosDePlanta;

    public int Dinheiro
    {
        get => dinheiro;
        set => dinheiro = value;
    }

    public float TempoDeJogo
    {
        get => tempoDeJogo;
        set => tempoDeJogo = value;
    }

    public int Repelente
    {
        get => repelente;
        set => repelente = value;
    }

    public List<MonsterBox> MonsterBank
    {
        get => monsterBank;
        set => monsterBank = value;
    }

    public List<Monster> MonsterBag
    {
        get => monsterBag;
        set => monsterBag = value;
    }

    public List<ItemHolder> Itens
    {
        get => itens;
        set => itens = value;
    }

    public List<ItemHolder> MonsterBalls
    {
        get => monsterBalls;
        set => monsterBalls = value;
    }

    public List<ItemHolder> Habilidades
    {
        get => habilidades;
        set => habilidades = value;
    }

    public List<ItemHolder> ItensChave
    {
        get => itensChave;
        set => itensChave = value;
    }

    public bool HasMagnet
    {
        get => hasMagnet;
        set => hasMagnet = value;
    }

    public void ResetarInformacoes(string novoPlayerName)
    {
        playerName = novoPlayerName;
        sexoDoPlayer = Sexo.Masculino;
        dinheiro = 0;
        tempoDeJogo = 0;

        repelente = 0;
        hasMagnet = false;

        monsterBank.Clear();
        monsterBag.Clear();

        itens.Clear();
        monsterBalls.Clear();
        habilidades.Clear();
        itensChave.Clear();
        
        monsterBook = new MonsterBook(GlobalSettings.Instance.Listas.ListaDeMonsterData);

        CriarDicionarioDeSkins();

        vasosDePlanta.Clear();
    }

    public void CarregarInformacoes(PlayerSOSave playerSOSave)
    {
        ResetarInformacoes(playerSOSave.playerName);

        dinheiro = playerSOSave.dinheiro;
        sexoDoPlayer = playerSOSave.sexoDoPlayer;
        tempoDeJogo = playerSOSave.tempoDeJogo;
        hasMagnet = playerSOSave.hasMagnet;
        repelente = playerSOSave.repelente;

        foreach (MonsterBoxSave monsterBox in playerSOSave.monsterBank)
        {
            monsterBank.Add(new MonsterBox(monsterBox));
        }

        foreach (MonsterSave monster in playerSOSave.monsterBag)
        {
            monsterBag.Add(new Monster(monster));
        }

        foreach (ItemHolderSave itemHolder in playerSOSave.itens)
        {
            itens.Add(new ItemHolder(itemHolder));
        }

        foreach (ItemHolderSave itemHolder in playerSOSave.monsterBalls)
        {
            monsterBalls.Add(new ItemHolder(itemHolder));
        }

        foreach (ItemHolderSave itemHolder in playerSOSave.habilidades)
        {
            habilidades.Add(new ItemHolder(itemHolder));
        }

        foreach (ItemHolderSave itemHolder in playerSOSave.itensChave)
        {
            itensChave.Add(new ItemHolder(itemHolder));
        }

        monsterBook = new MonsterBook(GlobalSettings.Instance.Listas.ListaDeMonsterData, playerSOSave.monsterBook);

        foreach(SkinDeDadoSave skinDeDadoSave in playerSOSave.skinsDeDados)
        {
            skinsDeDados[skinDeDadoSave.chaveDaSkin] = skinDeDadoSave.skinLiberada;
        }

        foreach(VasoPlantaSave vasoPlantaSave in playerSOSave.vasosDePlanta)
        {
            vasosDePlanta.Add(vasoPlantaSave.id, vasoPlantaSave);
        }
    }

    private void CriarDicionarioDeSkins()
    {
        skinsDeDados.Clear();

        foreach(string skin in GlobalSettings.Instance.DiceSettings.DiceArtDictionary.Keys)
        {
            skinsDeDados.Add(skin, false);
        }

        skinsDeDados["Default"] = true;
        skinsDeDados["Orange"] = true;
    }

    public Texture2D GetPlayerSprite()
    {
        switch(sexoDoPlayer)
        {
            case Sexo.Masculino:
                return spriteSexoMasculino;

            case Sexo.Feminino:
                return spriteSexoFeminino;

            default:
                throw new Exception("O sexo do jogador nao esta definido corretamente!");
        }
    }
    
    public void SetarInformacoes(string nomeDoPlayer, Sexo sexoDoPlayer)
    {
        this.playerName = nomeDoPlayer;
        this.sexoDoPlayer = sexoDoPlayer;
    }

    private void FindAll()
    {
        foreach (MonsterEntry entry in monsterBook.MonsterEntries)
        {
            entry.WasCaptured = true;
            entry.WasFound = true;
        }
    }

    private void LiberarTodas()
    {
        List<string> chaves = new List<string>();

        foreach(string chaveDaSkin in skinsDeDados.Keys)
        {
            chaves.Add(chaveDaSkin);
        }

        foreach(string chaveDaSkin in chaves)
        {
            skinsDeDados[chaveDaSkin] = true;
        }
    }
}
