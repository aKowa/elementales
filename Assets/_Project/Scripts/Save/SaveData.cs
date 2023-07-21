using BergamotaLibrary;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public PlayerSOSave playerSO;
    public SceneInfoSave sceneInfo;
    public FlagsSave flags;

    public SaveData(PlayerSO playerSO)
    {
        this.playerSO = new PlayerSOSave(playerSO);
        sceneInfo = new SceneInfoSave();
        flags = new FlagsSave();
    }
}

[System.Serializable]
public class PlayerSOSave
{
    //Variaveis
    public string playerName;
    public PlayerSO.Sexo sexoDoPlayer;
    public int dinheiro;
    public float tempoDeJogo;
    public SerializableDateTime data;
    public bool hasMagnet;
    public int repelente;

    //Inventario
    public List<MonsterBoxSave> monsterBank;
    public List<MonsterSave> monsterBag;

    public List<ItemHolderSave> itens;
    public List<ItemHolderSave> monsterBalls;
    public List<ItemHolderSave> habilidades;
    public List<ItemHolderSave> itensChave;

    //Monstros Encontrados
    public MonsterBookSave monsterBook;

    //Skins de Dados
    public List<SkinDeDadoSave> skinsDeDados;

    //Vasos de Planta
    public List<VasoPlantaSave> vasosDePlanta;

    public PlayerSOSave(PlayerSO playerSO)
    {
        playerName = playerSO.PlayerName;
        sexoDoPlayer = playerSO.SexoDoPlayer;
        dinheiro = playerSO.Dinheiro;
        tempoDeJogo = playerSO.TempoDeJogo;
        data = new SerializableDateTime(DateTime.Now);
        hasMagnet = playerSO.HasMagnet;
        repelente = playerSO.Repelente;

        monsterBank = new List<MonsterBoxSave>();

        foreach (MonsterBox monsterBox in playerSO.MonsterBank)
        {
            monsterBank.Add(new MonsterBoxSave(monsterBox));
        }

        monsterBag = new List<MonsterSave>();

        foreach (Monster monster in playerSO.MonsterBag)
        {
            monsterBag.Add(new MonsterSave(monster));
        }

        itens = new List<ItemHolderSave>();
        monsterBalls = new List<ItemHolderSave>();
        habilidades = new List<ItemHolderSave>();
        itensChave = new List<ItemHolderSave>();

        foreach(ItemHolder itemHolder in playerSO.Itens)
        {
            itens.Add(new ItemHolderSave(itemHolder));
        }

        foreach (ItemHolder itemHolder in playerSO.MonsterBalls)
        {
            monsterBalls.Add(new ItemHolderSave(itemHolder));
        }

        foreach (ItemHolder itemHolder in playerSO.Habilidades)
        {
            habilidades.Add(new ItemHolderSave(itemHolder));
        }

        foreach (ItemHolder itemHolder in playerSO.ItensChave)
        {
            itensChave.Add(new ItemHolderSave(itemHolder));
        }

        monsterBook = new MonsterBookSave(playerSO.MonsterBook);

        skinsDeDados = new List<SkinDeDadoSave>();

        foreach(string chaveDaSkin in playerSO.SkinsDeDados.Keys)
        {
            skinsDeDados.Add(new SkinDeDadoSave(chaveDaSkin, playerSO.SkinsDeDados[chaveDaSkin]));
        }

        vasosDePlanta = new List<VasoPlantaSave>();

        foreach (string chaveDoVaso in playerSO.VasosDePlanta.Keys)
        {
            vasosDePlanta.Add(playerSO.VasosDePlanta[chaveDoVaso]);
        }
    }
}

[System.Serializable]
public class SkinDeDadoSave
{
    public string chaveDaSkin;
    public bool skinLiberada;

    public SkinDeDadoSave(string chaveDaSkin, bool skinLiberada)
    {
        this.chaveDaSkin = chaveDaSkin;
        this.skinLiberada = skinLiberada;
    }
}

[System.Serializable]
public class MonsterSave
{
    public string nickName;
    public int monsterDataID;
    public List<StatusEffectSave> status;
    public List<AttackHolderSave> attacks;
    public MonsterAttributesSave monsterAttributes;
    public MonsterInfoSave monsterInfo;

    public string diceMaterial;
    public List<DiceType> dices;

    public List<int> combatLessonsAtivosID;
    public List<int> combatLessonsID;

    public MonsterSave(Monster monster)
    {
        nickName = monster.NicknameRaw;
        monsterDataID = monster.MonsterData.ID;
        
        status = new List<StatusEffectSave>();

        foreach(StatusEffectBase statusEffect in monster.Status)
        {
            status.Add(new StatusEffectSave(statusEffect));
        }

        attacks = new List<AttackHolderSave>();

        foreach(AttackHolder ataque in monster.Attacks)
        {
            attacks.Add(new AttackHolderSave(ataque));
        }

        monsterAttributes = new MonsterAttributesSave(monster.AtributosAtuais);

        monsterInfo = new MonsterInfoSave(monster.MonsterInfo);

        //Dados

        diceMaterial = monster.DiceMaterial;
        dices = monster.Dices.ToList();

        combatLessonsAtivosID = new List<int>();

        foreach(CombatLesson combatLesson in monster.CombatLessonsAtivos)
        {
            combatLessonsAtivosID.Add(combatLesson.ID);
        }

        combatLessonsID = new List<int>();

        foreach (CombatLesson combatLesson in monster.CombatLessons)
        {
            combatLessonsID.Add(combatLesson.ID);
        }
    }

    public MonsterSave()
    {
        monsterDataID = -1;
    }
}

[System.Serializable]
public class MonsterAttributesSave
{
    public int nivel;
    public int exp;

    public int vida;
    public int vidaMax;
    public int mana;
    public int manaMax;


    public int ataque;
    public int defesa;
    public int spAtaque;
    public int spDefesa;
    public int velocidade;

    public int ivVida;
    public int ivMana;
    public int ivAtk;
    public int ivDef;
    public int ivSpAtk;
    public int ivSpDef;
    public int ivVel;

    public MonsterAttributesSave(MonsterAttributes monsterAttributes)
    {
        nivel = monsterAttributes.Nivel;
        exp = monsterAttributes.Exp;

        vida = monsterAttributes.Vida;
        vidaMax = monsterAttributes.VidaMax;
        mana = monsterAttributes.Mana;
        manaMax = monsterAttributes.ManaMax;

        ataque = monsterAttributes.Ataque;
        defesa = monsterAttributes.Defesa;
        spAtaque = monsterAttributes.SpAtaque;
        spDefesa = monsterAttributes.SpDefesa;
        velocidade = monsterAttributes.Velocidade;

        ivVida = monsterAttributes.IvVida;
        ivMana = monsterAttributes.IvMana;
        ivAtk = monsterAttributes.IvAtk;
        ivDef = monsterAttributes.IvDef;
        ivSpAtk = monsterAttributes.IvSpAtk;
        ivSpDef = monsterAttributes.IvSpDef;
        ivVel = monsterAttributes.IvVel;
    }
}

[System.Serializable]
public class MonsterInfoSave
{
    //Variaveis
    public string firstTrainer;
    public SerializableDateTime dataMet;
    public string where;
    public int levelMet;

    public MonsterInfoSave(MonsterInfo monsterInfo)
    {
        firstTrainer = monsterInfo.FirstTrainer;
        dataMet = new SerializableDateTime(monsterInfo.DataMet);
        where = monsterInfo.Where;
        levelMet = monsterInfo.LevelMet;
    }
}

[System.Serializable]
public class StatusEffectSave
{
    public int statusID;
    public int contador;
    public int quantidadeTurnosAtuais;
    public int quantidadeTicksAtuais;

    public StatusEffectSave(StatusEffectBase statusEffect)
    {
        statusID = statusEffect.ID;
        contador = statusEffect.Contador;
        quantidadeTurnosAtuais = statusEffect.GetStatusEffectOpcoesDentroCombate.QuantidadeTurnosAtuais;
        quantidadeTicksAtuais = statusEffect.GetStatusEffectOpcoesForaCombate.QuantidadeTicksAtuais;
    }
}

[System.Serializable]
public class ItemHolderSave
{
    public int itemID;
    public int quantidade;
    public SerializableDateTime dataGot;

    public ItemHolderSave(ItemHolder itemHolder)
    {
        itemID = itemHolder.Item.ID;
        quantidade = itemHolder.Quantidade;
        dataGot = new SerializableDateTime(itemHolder.DataGot);
    }
}

[System.Serializable]
public class AttackHolderSave
{
    public int attackID;
    public int pp;

    public AttackHolderSave(AttackHolder attackHolder)
    {
        attackID = attackHolder.Attack.ID;
        pp = attackHolder.PP;
    }
}

[System.Serializable]
public class MonsterBoxSave
{
    public string boxName;
    public MonsterSave[] monsters;

    public MonsterBoxSave(MonsterBox monsterBox)
    {
        boxName = monsterBox.BoxName;
        monsters = new MonsterSave[monsterBox.Monsters.Length];

        for(int i = 0; i < monsterBox.Monsters.Length; i++)
        {
            if (monsterBox.Monsters[i] != null)
            {
                if (monsterBox.Monsters[i].MonsterData != null)
                {
                    monsters[i] = new MonsterSave(monsterBox.Monsters[i]);
                }
                else
                {
                    monsters[i] = new MonsterSave();
                }
            }
            else
            {
                monsters[i] = new MonsterSave();
            }
        }
    }
}

[System.Serializable]
public class GatewaySave
{
    public string currentDoorGuid;

    public GatewaySave(string gatewayID)
    {
        currentDoorGuid = gatewayID;
    }
}

[System.Serializable]
public class SceneInfoSave
{
    public string nomeDoMapa;
    public GatewaySave gateway;
    public GatewaySave monsterCenterGateway;

    public SceneInfoSave()
    {
        nomeDoMapa = SceneSpawnManager.NomeDoMapaAtual;
        gateway = new GatewaySave(SceneSpawnManager.LastGatewayID);
        monsterCenterGateway = new GatewaySave(SceneSpawnManager.LastMonsterCenterGatewayID);
    }
}

[System.Serializable]
public class MonsterBookSave
{
    public List<MonsterEntrySave> monsterEntries;

    public int MonstrosEncontrados()
    {
        int monstrosEncontrados = 0;

        for (int i = 0; i < monsterEntries.Count; i++)
        {
            if (monsterEntries[i].wasFound == true)
            {
                monstrosEncontrados++;
            }
        }

        return monstrosEncontrados;
    }

    public int MonstrosCapturados()
    {
        int monstrosCapturados = 0;

        for (int i = 0; i < monsterEntries.Count; i++)
        {
            if (monsterEntries[i].wasCaptured == true)
            {
                monstrosCapturados++;
            }
        }

        return monstrosCapturados;
    }

    public MonsterBookSave(MonsterBook monsterBook)
    {
        monsterEntries = new List<MonsterEntrySave>();

        for (int i = 0; i < monsterBook.MonsterEntries.Count; i++)
        {
            monsterEntries.Add(new MonsterEntrySave(monsterBook.MonsterEntries[i]));
        }
    }
}

[System.Serializable]
public class MonsterEntrySave
{
    public bool wasFound;
    public bool wasCaptured;

    public MonsterEntrySave(MonsterEntry monsterEntry)
    {
        wasFound = monsterEntry.WasFound;
        wasCaptured = monsterEntry.WasCaptured;
    }
}

[System.Serializable]
public class ConfiguracoesSave
{
    public float volumeMusicas;
    public float volumeEfeitosSonoros;

    public ConfiguracoesSave()
    {
        volumeMusicas = MusicManager.instance.Volume;
        volumeEfeitosSonoros = SoundManager.instance.Volume;
    }
}

[System.Serializable]
public class VasoPlantaSave
{
    public string id;
    public int itemPlantadoID;
    public SerializableDateTime dataPlantada;

    public VasoPlantaSave(VasoPlanta vasoPlanta)
    {
        id = vasoPlanta.ID;
        
        if(vasoPlanta.ItemPlantado != null)
        {
            itemPlantadoID = vasoPlanta.ItemPlantado.GetItem.ID;
            dataPlantada = new SerializableDateTime(vasoPlanta.DataPlantado);
        }
        else
        {
            itemPlantadoID = -1;
            dataPlantada = default;
        }
    }
}