using BergamotaLibrary;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class TesteMonstros : MonoBehaviour
{
    //Componentes
    private Inventario inventario;
    private MenuCarregarController menuCarregarController;
    private MenuDaLojaController menuDaLojaController;
    private MenuPlantarFrutaController menuPlantarFruta;

    //Variaveis
    [Header("Player SO")]
    [SerializeField] private PlayerSO playerSO;
    [SerializeField] private string playerName;

    [Header("Loja")]
    [SerializeField] private InventarioLoja inventarioLoja;

    [Header("Opcoes")]
    [SerializeField] private bool setarOsMonstrosDaBag = true;
    [SerializeField] private int nivelInicial = 1;
    [SerializeField] private bool ignorarOPrimeiroMonstro;
    [SerializeField] private bool setarOHPInicial;
    [SerializeField] private int hpInicial;
    [SerializeField] private bool setarStatusEffects;
    [SerializeField] private bool setarStatusEffectsInimigo;
    [SerializeField] private bool setarAtaquesIniciais;
    [SerializeField] private bool testeZeroVida;
    [SerializeField] private StatusEffectBase[] statusEffects;
    [SerializeField] private NPCBatalha nPCBatalha;
    [SerializeField] private StatusEffectSecundario[] statusSec;

    [Header("Monstros")]
    [SerializeField] private MonsterData[] monstros;
    [SerializeField] private List<ComandoDeAtaque> ataques;

    private void Start()
    {
        inventario = PlayerData.Instance.Inventario;
        menuCarregarController = FindObjectOfType<MenuCarregarController>();
        menuDaLojaController = FindObjectOfType<MenuDaLojaController>();
        menuPlantarFruta = FindObjectOfType<MenuPlantarFrutaController>();

        if (setarOsMonstrosDaBag == false)
        {
            return;
        }

        playerSO.ResetarInformacoes(playerName);

        inventario.CreateNewMonsterBank();

        foreach(MonsterData monstro in monstros)
        {
            Monster newMonster = new Monster(monstro, nivelInicial, setarAtaquesIniciais?ataques:null);

            if(setarOHPInicial == true)
            {
                if(ignorarOPrimeiroMonstro == true)
                {
                    ignorarOPrimeiroMonstro = false;
                }
                else
                {
                    newMonster.AtributosAtuais.Vida = hpInicial;
                }
            }

            if(setarStatusEffects == true)
            {
                for(int i = 0; i < statusEffects.Length; i++)
                {
                    newMonster.AplicarStatus(statusEffects[i]);
                }
            }

            inventario.AddMonsterToBag(newMonster, true);
        }
        if (setarStatusEffectsInimigo)
        {
            for (int i = 0; i < nPCBatalha.InventarioNPC.MonsterBag.Count; i++)
            {
                nPCBatalha.InventarioNPC.MonsterBag[i].Status.Clear();
                nPCBatalha.InventarioNPC.MonsterBag[i].StatusSecundario.Clear();
            }
            nPCBatalha.InventarioNPC.MonsterBag[0].AplicarStatusSecundario(statusSec[0]);
            nPCBatalha.InventarioNPC.MonsterBag[1].AplicarStatusSecundario(statusSec[1]);
        }
        if (testeZeroVida)
        {
            inventario.MonsterBag[0].AtributosAtuais.Vida = 0;
            inventario.MonsterBag[2].AtributosAtuais.Vida = 0;
        }

    }

    private void Update()
    {
#if UNITY_EDITOR
        if(Input.GetKeyDown(KeyCode.B))
        {
            Monster newMonster = new Monster(monstros[0], nivelInicial, ataques);
            newMonster.MonsterInfo = new MonsterInfo(PlayerData.Instance, newMonster);

            Debug.Log("O monstro foi adicionado na " + PlayerData.Instance.Inventario.AddMonsterToBank(newMonster, true));
        }

        if(Input.GetKeyDown(KeyCode.P))
        {
            StringBuilder stringBuilder = new StringBuilder();

            Debug.Log("Monster Bank");

            for(int i = 0; i < inventario.MonsterBank.Count; i++)
            {
                stringBuilder.Append(inventario.MonsterBank[i].BoxName + "\n[");

                for(int y = 0; y < inventario.MonsterBank[i].Monsters.Length; y++)
                {
                    if (inventario.MonsterBank[i].Monsters[y] != null)
                    {
                        stringBuilder.Append(inventario.MonsterBank[i].Monsters[y].NickName);
                    }
                    else
                    {
                        stringBuilder.Append("null");
                    }

                    if(y < inventario.MonsterBank[i].Monsters.Length - 1)
                    {
                        stringBuilder.Append(", ");
                    }
                }

                stringBuilder.Append("]");

                Debug.Log(stringBuilder);

                stringBuilder.Clear();
            }
        }

        if(Input.GetKeyDown(KeyCode.C))
        {
            menuCarregarController.OpenOrCloseView();
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            menuDaLojaController.IniciarMenu(inventarioLoja, MenuDaLojaController.TipoLoja.Compra);
        }

        if(Input.GetKeyDown(KeyCode.V))
        {
            Debug.Log($"Volume das Musicas: {MusicManager.instance.Volume}\nVolume dos Efeitos Sonoros: {SoundManager.instance.Volume}");
        }
#endif
    }
}
