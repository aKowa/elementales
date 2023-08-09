using BergamotaDialogueSystem;
using BergamotaLibrary;
using System.Collections;
using UnityEngine;

public class NPC_Enfermeira : MonoBehaviour
{
    //Componentes
    [SerializeField] private DialogueObject dialogoQuerCurarSeusMonstros;
    [SerializeField] private DialogueObject dialogoMonstrosCurados;

    [SerializeField] private DialogueActivator dialogueActivator;

    //Variaveis
    [Header("Som")]
    [SerializeField] protected AudioClip somCurar;
    private NPC npc;

    private void Awake()
    {
        npc = GetComponent<NPC>();
    }
    
    public void CurarMonstrosDoPlayer()
    {
        PlayerData.Instance.Inventario.RestaurarTodosOsMonstros();
    }
    
    public void AbrirDialogoMonstroCurado()
    {
        AbrirDialogo(dialogoMonstrosCurados);
    }
    public void TocarSomCurarMonstros()
    {
        SoundManager.instance.TocarSom(somCurar);
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
}
