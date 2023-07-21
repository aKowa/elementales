using BergamotaLibrary;
using UnityEngine;

public class NpcRival : MonoBehaviour
{
    [Header("Flags")]
    [SerializeField] private ListaDeFlags listaDeFlags;

    [SerializeField] private string flagMonstroInicial0;
    [SerializeField] private string flagMonstroInicial1;
    [SerializeField] private string flagMonstroInicial2;
    [Header("Monstros para adicionoar no inventario")]
    [SerializeField] private InventarioNPC inventarioNPCOriginal;
    [SerializeField] private InventarioNPC inventarioNpcMonstroInicial;


    private void Awake()
    {
        InventarioNPC inventairioNpcInstanciado = ScriptableObject.Instantiate(inventarioNPCOriginal);

        if (listaDeFlags.GetFlag(flagMonstroInicial0))
            inventairioNpcInstanciado.MonsterBag.Add(inventarioNpcMonstroInicial.MonsterBag[0]);

        else if (listaDeFlags.GetFlag(flagMonstroInicial1))
            inventairioNpcInstanciado.MonsterBag.Add(inventarioNpcMonstroInicial.MonsterBag[1]);

        else if (listaDeFlags.GetFlag(flagMonstroInicial2))
            inventairioNpcInstanciado.MonsterBag.Add(inventarioNpcMonstroInicial.MonsterBag[2]);

        GetComponent<NPCBatalha>().InventarioNPC = inventairioNpcInstanciado;
    }
}
