using UnityEngine;

[CreateAssetMenu(menuName = "Battle/Comando de Item")]
public class ComandoDeItem : Comando
{
    //Variaveis
    private ItemHolder itemHolder;

    //Getters
    
    public ItemHolder ItemHolder
    {
        get => itemHolder;
        set => itemHolder = value;
    }

    public void ReceberVariaveis(Integrante integrante, ItemHolder item, Integrante.MonstroAtual alvo)
    {
        origem = integrante;
        ItemHolder = item;
        AlvoAcao.Add(alvo);
    }
}
