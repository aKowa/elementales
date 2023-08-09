using Cinemachine;
using LumenSection.LevelLinker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneSpawnManager : MonoBehaviour
{
    //Variaveis
    [SerializeField] private string nomeDoMapa;

    [Space(10)]

    [SerializeField] private bool monsterCenter;
    [SerializeField] private bool hiddenCheckpoint;

    private static string nomeDoMapaAtual;
    private static string lastGatewayID;
    private static string lastMonsterCenterGatewayID;

    private EntityModel.Direction direcaoPlayerInicial;
    private Vector3 posicaoPlayerInicial;

    //Getters
    public static string NomeDoMapaAtual => nomeDoMapaAtual;
    public static string LastGatewayID => lastGatewayID;
    public static string LastMonsterCenterGatewayID => lastMonsterCenterGatewayID;
    public static bool HiddenCheckpoint;

    public EntityModel.Direction DirecaoPlayerInicial
    {
        get => direcaoPlayerInicial;
        set => direcaoPlayerInicial = value;
    }

    public Vector3 PosicaoPlayerInicial
    {
        get => posicaoPlayerInicial;
        set => posicaoPlayerInicial = value;
    }

    private void Awake()
    {
        nomeDoMapaAtual = nomeDoMapa;
        HiddenCheckpoint = hiddenCheckpoint;

        if(lastGatewayID == null)
        {
            Gateway gateway = FindObjectOfType<Gateway>();
            
            if(gateway != null)
            {
                lastGatewayID = gateway.Guid;
            }
        }

        if (lastMonsterCenterGatewayID == null)
        {
            Gateway gateway = FindObjectOfType<Gateway>();

            if (gateway != null)
            {
                lastMonsterCenterGatewayID = gateway.Guid;
            }
        }
    }

    private void Start()
    {
        BergamotaDialogueSystem.DialogueUI.Instance.SetPlaceholderDeTexto("%player", PlayerData.Instance.GetPlayerName);
    }

    public void SpawnPlayer(Player player, string gatewayID)
    {
        CinemachineVirtualCamera camera = FindObjectOfType<CinemachineVirtualCamera>();
        Vector3 deltaPosition = posicaoPlayerInicial - player.transform.position;

        player.transform.position = posicaoPlayerInicial;
        player.SetDirection(direcaoPlayerInicial);

        camera.OnTargetObjectWarped(player.transform, deltaPosition);

        lastGatewayID = gatewayID;

        if(monsterCenter == true)
        {
            lastMonsterCenterGatewayID = gatewayID;
        }
    }

    public static void CarregarInformacoes(SceneInfoSave sceneInfoSave)
    {
        lastGatewayID = sceneInfoSave.gateway.currentDoorGuid;
        lastMonsterCenterGatewayID = sceneInfoSave.monsterCenterGateway.currentDoorGuid;
    }
}
