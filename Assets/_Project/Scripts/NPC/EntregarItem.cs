using BergamotaDialogueSystem;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class EntregarItem : MonoBehaviour
{

    //Variaveis
    [SerializeField] private DialogueObject dialogoGanhouItens;

    [Space(10)]

    [SerializeField] private ItemParaEntregar[] itens;

    private static StringBuilder textoDosItens = new StringBuilder();

    public void EntregarItens()
    {
        foreach(ItemParaEntregar recompensa in itens)
        {
            PlayerData.Instance.Inventario.AddItem(recompensa.Item, recompensa.Quantidade);
        }

        DialogueUI.Instance.StartCoroutine(AbrirDialogo(itens));
    }

    public void EntregarItens(ItemParaEntregar[] itens)
    {
        foreach (ItemParaEntregar recompensa in itens)
        {
            PlayerData.Instance.Inventario.AddItem(recompensa.Item, recompensa.Quantidade);
        }

        DialogueUI.Instance.StartCoroutine(AbrirDialogo(itens));
    }

    private void SetarTextoDosItens(ItemParaEntregar[] itens)
    {
        textoDosItens.Clear();

        for(int i = 0; i < itens.Length; i++)
        {
            textoDosItens.Append($"{itens[i].Item.Nome} x{itens[i].Quantidade}");

            if(i < itens.Length - 1)
            {
                textoDosItens.Append(", ");
            }
        }

        DialogueUI.Instance.SetPlaceholderDeTexto("%item", textoDosItens.ToString());

        textoDosItens.Clear();
    }

    private IEnumerator AbrirDialogo(ItemParaEntregar[] itens)
    {
        yield return new WaitUntil(() => DialogueUI.Instance.IsOpen == false);

        SetarTextoDosItens(itens);

        DialogueUI.Instance.ShowDialogue(dialogoGanhouItens);
    }

    [System.Serializable]
    public struct ItemParaEntregar
    {
        [SerializeField] private Item item;
        [SerializeField] private int quandidade;

        public Item Item => item;
        public int Quantidade => quandidade;

        public ItemParaEntregar(Item item, int quantidade)
        {
            this.item = item;
            this.quandidade = quantidade;
        }
    }
}
