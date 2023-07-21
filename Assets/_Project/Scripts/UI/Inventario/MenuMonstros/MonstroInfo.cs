using UnityEngine;

public abstract class MonstroInfo : MonoBehaviour
{
    [Header("Variaveis")]
    [SerializeField] private string nomeDaGuia;

    public string NomeDaGuia => nomeDaGuia;
    public abstract void AtualizarInformacoes(Monster monstro);

    public abstract void ResetarInformacoes();
}
