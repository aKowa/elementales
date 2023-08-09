using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public class Inventario : MonoBehaviour
{
    //Constantes
    public const int numeroMonstrosBagMax = 6;
    public const int quantidadeMaxItem = 99;

    //Componentes
    private PlayerSO playerSO;

    //Variaveis
    [SerializeField] private List<MonsterBox> monsterBank;
    [SerializeField] private List<Monster> monsterBag;

    [SerializeField] private List<ItemHolder> itens;
    [SerializeField] private List<ItemHolder> monsterBalls;
    [SerializeField] private List<ItemHolder> habilidades;
    [SerializeField] private List<ItemHolder> itensChave;

    private string boxName = "Box";

    //Getters
    public List<MonsterBox> MonsterBank => monsterBank;
    public List<Monster> MonsterBag => monsterBag;

    public List<ItemHolder> Itens => itens;
    public List<ItemHolder> MonsterBalls => monsterBalls;
    public List<ItemHolder> Habilidades => habilidades;
    public List<ItemHolder> ItensChave => itensChave;

    public int Dinheiro
    {
        get => playerSO.Dinheiro;
        set => playerSO.Dinheiro = value;
    }

    public void Setup(PlayerSO newPlayerSO)
    {
        playerSO = newPlayerSO;

        monsterBank = playerSO.MonsterBank;
        monsterBag = playerSO.MonsterBag;

        itens = playerSO.Itens;
        monsterBalls = playerSO.MonsterBalls;
        habilidades = playerSO.Habilidades;
        itensChave = playerSO.ItensChave;

        if(monsterBank.Count <= 0)
        {
            CreateNewMonsterBank();
        }
    }

    private void UpdateSO()
    {
        playerSO.MonsterBank = monsterBank;
        playerSO.MonsterBag = monsterBag;

        playerSO.Itens = itens;
        playerSO.MonsterBalls = monsterBalls;
        playerSO.Habilidades = habilidades;
        playerSO.ItensChave = itensChave;
    }

    public string AddMonsterToBank(Monster monster, bool atualizarMonstroInfo = false)
    {
        if (atualizarMonstroInfo == true)
        {
            monster.DiceMaterial = "Orange";
            monster.MonsterInfo = new MonsterInfo(PlayerData.Instance, monster);
        }

        for (int i = 0; i < monsterBank.Count; i++)
        {
            for(int y = 0; y < monsterBank[i].Monsters.Length; y++)
            {
                if (monsterBank[i].Monsters[y] == null)
                {
                    monsterBank[i].Monsters[y] = monster;

                    ChecarSePrecisaDeUmNovoMonsterBank();

                    PlayerData.MonsterBook.MonsterEntries[monster.MonsterData.ID].WasFound = true;
                    PlayerData.MonsterBook.MonsterEntries[monster.MonsterData.ID].WasCaptured = true;

                    return monsterBank[i].BoxName;
                }
            }
        }

        CreateNewMonsterBank();
        monsterBank[monsterBank.Count - 1].Monsters[0] = monster;

        PlayerData.MonsterBook.MonsterEntries[monster.MonsterData.ID].WasFound = true;
        PlayerData.MonsterBook.MonsterEntries[monster.MonsterData.ID].WasCaptured = true;

        return monsterBank[monsterBank.Count - 1].BoxName;
    }

    public void CreateNewMonsterBank()
    {
        monsterBank.Add(new MonsterBox(boxName + " " + (monsterBank.Count + 1)));
    }

    public void ChecarSePrecisaDeUmNovoMonsterBank()
    {
        int espacosLivres = 0;

        for (int i = 0; i < monsterBank.Count; i++)
        {
            for (int y = 0; y < monsterBank[i].Monsters.Length; y++)
            {
                if (monsterBank[i].Monsters[y] == null)
                {
                    espacosLivres++;
                }
            }
        }

        if(espacosLivres < numeroMonstrosBagMax)
        {
            CreateNewMonsterBank();
        }
    }

    public void AddMonsterToBag(Monster monster, bool atualizarMonstroInfo = false)
    {
        if(atualizarMonstroInfo == true)
        {
            monster.DiceMaterial = "Orange";
            monster.MonsterInfo = new MonsterInfo(PlayerData.Instance, monster);
        }

        monsterBag.Add(monster);

        PlayerData.MonsterBook.MonsterEntries[monster.MonsterData.ID].WasFound = true;
        PlayerData.MonsterBook.MonsterEntries[monster.MonsterData.ID].WasCaptured = true;
    }
    
    public void RemoveMonsterFromBag(Monster monster)
    {
        monsterBag.Remove(monster);
    }

    public void AddItem(Item item, int quantidade)
    {
        switch(item.Tipo)
        {
            case Item.TipoItem.Consumivel:
                AddItemInList(item, quantidade, itens);
                break;

            case Item.TipoItem.MonsterBall:
                AddItemInList(item, quantidade, monsterBalls);
                break;

            case Item.TipoItem.Habilidade:
                AddItemInList(item, quantidade, habilidades);
                break;

            case Item.TipoItem.ItemChave:
                AddItemInList(item, quantidade, itensChave);
                break;
        }
    }

    public bool HasMagnet() => playerSO.HasMagnet;

    public void RemoverItem(Item item, int quantidade)
    {
        switch (item.Tipo)
        {
            case Item.TipoItem.Consumivel:
                RemoveItemFromList(item, quantidade, itens);
                break;

            case Item.TipoItem.MonsterBall:
                RemoveItemFromList(item, quantidade, monsterBalls);
                break;

            case Item.TipoItem.Habilidade:
                RemoveItemFromList(item, quantidade, habilidades);
                break;

            case Item.TipoItem.ItemChave:
                RemoveItemFromList(item, quantidade, itensChave);
                break;
        }
    }

    private void AddItemInList(Item item, int quantidade, List<ItemHolder> lista)
    {
        foreach (ItemHolder itemTemp in lista)
        {
            if (item == itemTemp.Item)
            {
                itemTemp.Quantidade += quantidade;
                return;
            }
        }

        lista.Add(new ItemHolder(item, quantidade));
    }

    private void RemoveItemFromList(Item item, int quantidade, List<ItemHolder> lista)
    {
        foreach (ItemHolder itemTemp in lista)
        {
            if (item == itemTemp.Item)
            {
                itemTemp.Quantidade -= quantidade;

                if (itemTemp.Quantidade <= 0)
                {
                    lista.Remove(itemTemp);
                }

                return;
            }
        }

        Debug.LogWarning("Nao foi encontrado o item \"" + item.Nome + "\" do tipo " + item.Tipo.ToString() + " para ser excluido na lista!");
    }

    public void SortItensByName()
    {
        itens = itens.OrderBy(item => item.Item.Nome).ToList();
        monsterBalls = monsterBalls.OrderBy(item => item.Item.Nome).ToList();
        habilidades = habilidades.OrderBy(item => item.Item.Nome).ToList();
        itensChave = itensChave.OrderBy(item => item.Item.Nome).ToList();

        UpdateSO();
    }

    public void SortItensByType()
    {
        itens = itens.OrderBy(item => item.Item.Efeito).ToList();
        monsterBalls = monsterBalls.OrderBy(item => item.Item.Efeito).ToList();
        habilidades = habilidades.OrderBy(item => item.Item.Efeito).ToList();
        itensChave = itensChave.OrderBy(item => item.Item.Efeito).ToList();

        UpdateSO();
    }

    public void SortItensNewestLast()
    {
        itens = itens.OrderBy(item => item.DataGot).ToList();
        monsterBalls = monsterBalls.OrderBy(item => item.DataGot).ToList();
        habilidades = habilidades.OrderBy(item => item.DataGot).ToList();
        itensChave = itensChave.OrderBy(item => item.DataGot).ToList();

        UpdateSO();
    }

    public string MonsterCaptured(Monster monster)
    {
        if (monsterBag.Count < numeroMonstrosBagMax)
        {
            AddMonsterToBag(monster, true);
            return string.Empty;
        }
        else
        {
            return AddMonsterToBank(monster, true);
        }
    }

    public int GetQuantidadeDoItem(Item item)
    {
        List<ItemHolder> listaDeItens = new List<ItemHolder>();

        switch (item.Tipo)
        {
            case Item.TipoItem.Consumivel:
                listaDeItens = itens;
                break;

            case Item.TipoItem.MonsterBall:
                listaDeItens = monsterBalls;
                break;

            case Item.TipoItem.Habilidade:
                listaDeItens = habilidades;
                break;

            case Item.TipoItem.ItemChave:
                listaDeItens = itensChave;
                break;
        }

        for(int i = 0; i < listaDeItens.Count; i++)
        {
            if (listaDeItens[i].Item.ID == item.ID)
            {
                return listaDeItens[i].Quantidade;
            }
        }

        return 0;
    }

    public Monster GetMonstroNaBag(MonsterData monstro)
    {
        for (int i = 0; i < monsterBag.Count; i++)
        {
            if (monsterBag[i].MonsterData.ID == monstro.ID)
            {
                return monsterBag[i];
            }
        }

        return null;
    }

    public void AddMagnetBuff()
    {
        playerSO.HasMagnet = true;
    }

    public Monster GetMonstroNaBox(MonsterData monstro)
    {
        for (int i = 0; i < monsterBank.Count; i++)
        {
            for (int y = 0; y < monsterBank[i].Monsters.Length; y++)
            {
                if (monsterBank[i].Monsters[y] == null)
                {
                    if (monsterBank[i].Monsters[y].MonsterData.ID == monstro.ID)
                    {
                        return monsterBank[i].Monsters[y];
                    }
                }
            }
        }

        return null;
    }

    [Button]
    public void RestaurarTodosOsMonstros(bool recuperarMetadeDoHP = false)
    {
        foreach(Monster monstro in monsterBag)
        {
            if(recuperarMetadeDoHP == false)
            {
                monstro.AtributosAtuais.Vida = monstro.AtributosAtuais.VidaMax;
            }
            else
            {
                int quantidadeDeHP = monstro.AtributosAtuais.VidaMax / 2;

                if(monstro.AtributosAtuais.Vida < quantidadeDeHP)
                {
                    monstro.AtributosAtuais.Vida = quantidadeDeHP;
                }
            }

            monstro.AtributosAtuais.Mana = monstro.AtributosAtuais.ManaMax;

            monstro.Status.Clear();
            monstro.StatusSecundario.Clear();
        }
    }
    public void AtivarBuffAtaqueProximoCombate()
    {
        foreach (Monster monstro in monsterBag)
        {
            monstro.BuffDano = true;
        }
    }
    public bool VerificarSeMonstrosPossuemBuff()
    {
        foreach (Monster monstro in monsterBag)
        {
            if (monstro.BuffDano == true)
                return true;
        }
        return false;
    }
}
