using UnityEngine;
using BergamotaDialogueSystem;
using System.Collections.Generic;

public class PlayerData : MonoBehaviour
{
    //Variaveis
    private static PlayerData instance;

    private static PlayerSO data;

    private Inventario inventario;

    //Getters
    public static PlayerData Instance => instance;
    public Inventario Inventario => inventario;
    public string GetPlayerName => data.PlayerName;
    public static float TimePlayed => data.TempoDeJogo;
    public static MonsterBook MonsterBook => data.MonsterBook;
    public static Dictionary<string, bool> SkinsDeDados => data.SkinsDeDados;
    public static Dictionary<string, VasoPlantaSave> VasosDePlanta => data.VasosDePlanta;
    public static Texture2D GetPlayerSprite => data.GetPlayerSprite();
    public static PlayerSO.Sexo GetPlayerSexo => data.SexoDoPlayer;

    public static int Repelente
    {
        get => data.Repelente;
        set => data.Repelente = value;
    }

    //Setters
    public static void SetPlayerData(PlayerSO data)
    {
        PlayerData.data = data;
    }

    private void Awake()
    {
        instance = this;

        inventario = GetComponentInChildren<Inventario>();

        Setup(data);
    }

    public void Setup(PlayerSO playerData)
    {
        data = playerData;
        inventario.Setup(playerData);
    }
}
