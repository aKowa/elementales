using BergamotaDialogueSystem;
using System.Collections;
using UnityEngine;

public class NPC_Enfermeira : MonoBehaviour
{
    //Componentes
    [SerializeField] private DialogueObject dialogoQuerCurarSeusMonstros;
    [SerializeField] private DialogueObject dialogoQuerDoarParaHospital;
    [SerializeField] private DialogueActivator dialogueActivator;

    private NPC npc;

    private void Awake()
    {
        npc = GetComponent<NPC>();
    }
    
    public void CurarMonstrosDoPlayer()
    {
        PlayerData.Instance.Inventario.RestaurarTodosOsMonstros();
    }
    
    public void AbrirDialogoDoacao()
    {
        if (PlayerData.Instance.Inventario.VerificarSeMonstrosPossuemBuff() == false)
        {
            StartCoroutine(AdsManager.GetInstance().CheckInternetConnection((isConnected) =>
            {
                if (isConnected)
                {
                    AbrirDialogo(dialogoQuerDoarParaHospital);
                }
            }));
        }
    }

    public void DoacaoHospital()
    {
        Debug.Log("passar Ad");
        //Ver Anuncio
        AdsManager.GetInstance().OnRewardAdEarnedEvent_External = DarRecompensa;
        AdsManager.GetInstance().ShowRewardedAds();
    }
    
    private void AbrirDialogo(DialogueObject dialogo)
    {
        StartCoroutine(AbrirDialogoCorrotina(dialogo));
    }

    private IEnumerator AbrirDialogoCorrotina(DialogueObject dialogo)
    {
        yield return new WaitUntil(() => DialogueUI.Instance.IsOpen == false);

        npc.VirarNaDirecao(PlayerData.Instance.transform.position);
        dialogueActivator.ShowDialogue(dialogo, DialogueUI.Instance);
    }
    
    private void DarRecompensa()
    {
        PlayerData.Instance.Inventario.AtivarBuffAtaqueProximoCombate();
    }
}
